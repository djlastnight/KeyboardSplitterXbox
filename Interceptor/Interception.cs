namespace Interceptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Interceptor.Enums;

    public class Interception : IDisposable
    {
        public const int MaxKeyboardsCount = 10;

        public const int MaxMiceCount = 10;

        public const int MouseMoveDeadZoneMin = 0;

        public const int MouseMoveDeadZoneMax = 12;

        public static readonly int MaxDeviceCount = MaxKeyboardsCount + MaxMiceCount;

        private const int HardwareIdSize = 500;

        private static int mouseMoveDeadZone;

        private IntPtr context;

        private Thread callbackThread;

        private Thread connectionThread;

        private StringBuilder hardwareId;

        private int deviceId;

        private Dictionary<string, Dictionary<InterceptionKey, bool>> keyStates;

        private InterceptionDevice currentDevice;

        private Dictionary<int, InterceptionDevice> devices;

        private List<KeyInfo> currentKeys;

        private TimeSpan mouseWheelAutoOffDelay;

        private TimeSpan mouseMoveAutoOffDelay;

        public Interception(KeyboardFilterMode keyboardFilter, MouseFilterMode mouseFilter)
        {
            KeysHelper.CheckInterceptionKeysForDuplicates();
            this.KeyboardFilterMode = keyboardFilter;
            this.MouseFilterMode = mouseFilter;

            this.hardwareId = new StringBuilder(Interception.HardwareIdSize);

            this.mouseWheelAutoOffDelay = TimeSpan.FromMilliseconds(50);
            this.mouseMoveAutoOffDelay = TimeSpan.FromMilliseconds(50);
            this.KeyPressDelay = 1;
            this.ClickDelay = 1;
            this.ScrollDelay = 15;
            Interception.MouseMoveDeadZone = 1;

            // Initializing keyStates (false for every key on every device)
            this.keyStates = new Dictionary<string, Dictionary<InterceptionKey, bool>>();
            for (int i = 1; i <= Interception.MaxDeviceCount; i++)
            {
                string deviceStrongName;

                if (NativeMethods.IsKeyboard(i) > 0)
                {
                    deviceStrongName = new InterceptionKeyboard(Convert.ToUInt16(i), "dump").StrongName;
                }
                else
                {
                    deviceStrongName = new InterceptionMouse(Convert.ToUInt16(i), "dump").StrongName;
                }

                var states = new Dictionary<InterceptionKey, bool>();
                foreach (InterceptionKey key in Enum.GetValues(typeof(InterceptionKey)))
                {
                    states.Add(key, false);
                }

                this.keyStates.Add(deviceStrongName, states);
            }

            this.devices = new Dictionary<int, InterceptionDevice>();
        }

        ~Interception()
        {
            this.Dispose(false);
        }

        public event EventHandler<InterceptionEventArgs> InputActivity;

        public event EventHandler<InterceptionDeviceEventArgs> InputDeviceConnectionChanged;

        public static List<InterceptionDevice> ConnectedInputDevices
        {
            get
            {
                using (var interceptor = new Interception(KeyboardFilterMode.None, MouseFilterMode.None))
                {
                    interceptor.Load(startThreads: false);
                    var devices = interceptor.RescanInputDevices(new Dictionary<int, InterceptionDevice>()).Values;
                    return devices.ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the mouse move deadzone.
        /// Please use values in range 0-12
        /// </summary>
        public static int MouseMoveDeadZone
        {
            get
            {
                return mouseMoveDeadZone;
            }

            set
            {
                if (value < Interception.MouseMoveDeadZoneMin)
                {
                    mouseMoveDeadZone = Interception.MouseMoveDeadZoneMin;
                }
                else if (value > Interception.MouseMoveDeadZoneMax)
                {
                    mouseMoveDeadZone = Interception.MouseMoveDeadZoneMax;
                }
                else
                {
                    mouseMoveDeadZone = value;
                }
            }
        }

        /// <summary>
        /// Gets the Keyboard Filter Mode, which
        /// determines whether the driver traps no keyboard events, all events,
        /// or a range of events in-between (down only, up only...etc).
        /// </summary>
        public KeyboardFilterMode KeyboardFilterMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Mouse Filter Mode, which
        /// determines whether the driver traps no events, all events,
        /// or a range of events in-between.
        /// </summary>
        public MouseFilterMode MouseFilterMode
        {
            get;
            private set;
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each key stroke down and up.
        /// Pressing a key requires both a key stroke down and up.
        /// A delay of 0 (inadvisable) may result in no keys being apparently pressed.
        /// A delay of 20 - 40 milliseconds makes the key presses visible.
        /// </summary>
        internal int KeyPressDelay { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each mouse event down and up.
        /// 'Clicking' the cursor (whether left or right) requires both a mouse event down and up.
        /// A delay of 0 (inadvisable) may result in no apparent click.
        /// A delay of 20 - 40 milliseconds makes the clicks apparent.
        /// </summary>
        internal int ClickDelay { get; set; }

        internal int ScrollDelay { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Attempts to load the driver.
        /// </summary>
        /// <returns>Returns true if loaded successfully</returns>
        public bool Load(bool startThreads = true)
        {
            if (this.IsLoaded)
            {
                return false;
            }

            this.context = NativeMethods.CreateContext();
            if (this.context != IntPtr.Zero)
            {
                if (startThreads)
                {
                    this.callbackThread = new Thread(new ThreadStart(this.DriverCallback));
                    this.callbackThread.Priority = ThreadPriority.Highest;
                    this.callbackThread.IsBackground = true;
                    this.callbackThread.Start();

                    this.connectionThread = new Thread(new ThreadStart(this.ConnectionCallback));
                    this.connectionThread.Priority = ThreadPriority.Normal;
                    this.connectionThread.IsBackground = true;
                    this.connectionThread.Start();
                }

                this.IsLoaded = true;
            }
            else
            {
                this.IsLoaded = false;
            }

            return this.IsLoaded;
        }

        /// <summary>
        /// Safely unloads the driver. Calling Unload() twice has no effect.
        /// </summary>
        public void Unload()
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (this.callbackThread != null)
            {
                this.callbackThread.Abort();
            }

            if (this.connectionThread != null)
            {
                this.connectionThread.Abort();
            }

            if (this.context != IntPtr.Zero)
            {
                NativeMethods.DestroyContext(this.context);
                this.IsLoaded = false;
            }
        }

        /// <summary>
        /// Gets list of all connected keyboards.
        /// </summary>
        /// <returns>List of up to date keyboards.</returns>
        public List<InterceptionKeyboard> GetKeyboards()
        {
            this.devices = this.RescanInputDevices(this.devices);
            return this.devices.Values.OfType<InterceptionKeyboard>().ToList();
        }

        /// <summary>
        /// Gets list of all connected mice.
        /// </summary>
        /// <returns>List of up to date mice.</returns>
        public List<InterceptionMouse> GetMice()
        {
            this.devices = this.RescanInputDevices(this.devices);
            return this.devices.Values.OfType<InterceptionMouse>().ToList();
        }

        public bool IsKeyDown(InterceptionDevice device, InterceptionKey key)
        {
            return this.keyStates[device.StrongName][key];
        }

        internal void SendKey(InterceptionKey key, KeyState state)
        {
            Stroke stroke = new Stroke();
            KeyStroke keyStroke = new KeyStroke();

            keyStroke.Code = key;
            keyStroke.State = state;

            stroke.Key = keyStroke;

            this.Send(this.context, this.deviceId, ref stroke, 1);

            if (this.KeyPressDelay > 0)
            {
                Thread.Sleep(this.KeyPressDelay);
            }
        }

        /// <summary>
        /// Warning: Do not use this overload of SendKey() for non-letter, non-number, or non-ENTER keys. It may require a special KeyState of not KeyState.Down or KeyState.Up, but instead KeyState.E0 and KeyState.E1.
        /// </summary>
        /// <param name="key">Key to send.</param>
        internal void SendKey(InterceptionKey key)
        {
            this.SendKey(key, KeyState.Down);

            if (this.KeyPressDelay > 0)
            {
                Thread.Sleep(this.KeyPressDelay);
            }

            this.SendKey(key, KeyState.Up);
        }

        internal void SendKeys(params InterceptionKey[] keys)
        {
            foreach (InterceptionKey key in keys)
            {
                this.SendKey(key);
            }
        }

        /// <summary>
        /// Warning: Only use this overload for sending letters, numbers 
        /// and symbols (those to the right of the letters on a U.S. keyboard
        /// and those obtained by pressing shift-#). Do not send special keys like Tab or Control or Enter.
        /// </summary>
        /// <param name="text">Text to send.</param>
        internal void SendText(string text)
        {
            foreach (char letter in text)
            {
                var tuple = this.CharacterToKeysEnum(letter);

                if (tuple.Item2 == true)
                {
                    // We need to press shift to get the next character
                    this.SendKey(InterceptionKey.LeftShift, KeyState.Down);
                }

                this.SendKey(tuple.Item1);

                if (tuple.Item2 == true)
                {
                    this.SendKey(InterceptionKey.LeftShift, KeyState.Up);
                }
            }
        }

        internal void SendMouseEvent(MouseState state)
        {
            Stroke stroke = new Stroke();
            MouseStroke mouseStroke = new MouseStroke();

            mouseStroke.State = state;

            ////if (state == MouseState.Wheel)
            ////{
            ////    mouseStroke.Rolling = 120;
            ////}
            ////else if (state == MouseState.ScrollDown)
            ////{
            ////    mouseStroke.Rolling = -120;
            ////}

            stroke.Mouse = mouseStroke;

            this.Send(this.context, 12, ref stroke, 1);
        }

        internal void SendLeftClick()
        {
            this.SendMouseEvent(MouseState.LeftDown);
            Thread.Sleep(this.ClickDelay);
            this.SendMouseEvent(MouseState.LeftUp);
        }

        internal void SendRightClick()
        {
            this.SendMouseEvent(MouseState.RightDown);
            Thread.Sleep(this.ClickDelay);
            this.SendMouseEvent(MouseState.RightUp);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.hardwareId.Clear();
                this.hardwareId = null;
            }

            this.Unload();
        }

        private void Send(IntPtr context, int device, ref Stroke stroke, uint numStrokes)
        {
            NativeMethods.Send(context, device, ref stroke, numStrokes);
        }

        /// <summary>
        /// Converts a character to a Keys enum and a 'do we need to press shift'.
        /// </summary>
        /// <returns>Tuple of desired Interception Key and boolean value,
        /// which determines whether shift key should be pressed.
        /// </returns>
        private Tuple<InterceptionKey, bool> CharacterToKeysEnum(char ch)
        {
            switch (char.ToLower(ch))
            {
                case 'a':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.A, false);
                case 'b':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.B, false);
                case 'c':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.C, false);
                case 'd':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.D, false);
                case 'e':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.E, false);
                case 'f':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.F, false);
                case 'g':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.G, false);
                case 'h':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.H, false);
                case 'i':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.I, false);
                case 'j':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.J, false);
                case 'k':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.K, false);
                case 'l':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.L, false);
                case 'm':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.M, false);
                case 'n':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.N, false);
                case 'o':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.O, false);
                case 'p':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.P, false);
                case 'q':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Q, false);
                case 'r':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.R, false);
                case 's':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.S, false);
                case 't':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.T, false);
                case 'u':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.U, false);
                case 'v':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.V, false);
                case 'w':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.W, false);
                case 'x':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.X, false);
                case 'y':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Y, false);
                case 'z':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Z, false);
                case '1':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.One, false);
                case '2':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Two, false);
                case '3':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Three, false);
                case '4':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Four, false);
                case '5':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Five, false);
                case '6':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Six, false);
                case '7':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Seven, false);
                case '8':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Eight, false);
                case '9':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Nine, false);
                case '0':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Zero, false);
                case '-':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.DashUnderscore, false);
                case '+':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.PlusEquals, false);
                case '[':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.OpenBracketBrace, false);
                case ']':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.CloseBracketBrace, false);
                case ';':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.SemicolonColon, false);
                case '\'':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.SingleDoubleQuote, false);
                case ',':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.CommaLeftArrow, false);
                case '.':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.PeriodRightArrow, false);
                case '/':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.ForwardSlashQuestionMark, false);
                case '{':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.OpenBracketBrace, true);
                case '}':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.CloseBracketBrace, true);
                case ':':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.SemicolonColon, true);
                case '\"':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.SingleDoubleQuote, true);
                case '<':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.CommaLeftArrow, true);
                case '>':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.PeriodRightArrow, true);
                case '?':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.ForwardSlashQuestionMark, true);
                case '\\':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.BackslashPipe, false);
                case '|':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.BackslashPipe, true);
                case '`':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Tilde, false);
                case '~':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Tilde, true);
                case '!':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.One, true);
                case '@':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Two, true);
                case '#':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Three, true);
                case '$':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Four, true);
                case '%':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Five, true);
                case '^':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Six, true);
                case '&':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Seven, true);
                case '*':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Eight, true);
                case '(':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Nine, true);
                case ')':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Zero, true);
                case ' ':
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.Space, true);
                default:
                    return new Tuple<InterceptionKey, bool>(InterceptionKey.ForwardSlashQuestionMark, true);
            }
        }

        private string GetHardwareID(int device)
        {
            try
            {
                int length = NativeMethods.GetHardwareId(
                    this.context,
                    device,
                    this.hardwareId,
                    Interception.HardwareIdSize);

                if (length > 0 && length < Interception.HardwareIdSize)
                {
                    return this.hardwareId.ToString();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private void DriverCallback()
        {
            NativeMethods.SetFilter(this.context, NativeMethods.IsKeyboard, (int)this.KeyboardFilterMode);
            NativeMethods.SetFilter(this.context, NativeMethods.IsMouse, (int)MouseFilterMode);

            Stroke stroke = new Stroke();

            while (NativeMethods.Receive(this.context, this.deviceId = NativeMethods.Wait(this.context), ref stroke, 1) > 0)
            {
                bool isKeyboard = NativeMethods.IsKeyboard(this.deviceId) > 0;

                // Checking if the input device is already in the device list. If not - adding it.
                if (this.devices.ContainsKey(this.deviceId))
                {
                    this.currentDevice = this.devices[this.deviceId];
                }
                else
                {
                    continue;
                }

                // Getting the keys
                if (isKeyboard)
                {
                    this.currentKeys = this.GetKeyboardKeys(stroke.Key);
                }
                else
                {
                    this.currentKeys = this.GetMouseKeys(stroke.Mouse);
                }

                // Saving the key states
                foreach (var keyInfo in this.currentKeys)
                {
                    this.keyStates[this.currentDevice.StrongName][keyInfo.Key] = keyInfo.IsDown;
                }

                if (!isKeyboard && stroke.Mouse.Rolling != 0)
                {
                    foreach (var keyInfo in this.currentKeys)
                    {
                        if (KeysHelper.IsMouseWheelKey(keyInfo.Key))
                        {
                            this.ReleaseKeyWithDelay(this.currentDevice, keyInfo.Key, this.mouseWheelAutoOffDelay);
                        }
                    }
                }

                // Raising the event, which determines whether the input will be blocked or not.
                if (this.InputActivity != null)
                {
                    var args = new InterceptionEventArgs(this.currentDevice, this.currentKeys);
                    this.InputActivity(this, args);
                    if (args.Handled)
                    {
                        continue;
                    }
                }

                this.Send(this.context, this.deviceId, ref stroke, 1);
            }

            this.Unload();

            throw new Exception(
                "Interception.Receive() failed for an unknown reason. The driver has been unloaded.");
        }

        private void ReleaseKeyWithDelay(InterceptionDevice device, InterceptionKey key, TimeSpan delay)
        {
            var action = new Action(() =>
            {
                if (this.IsKeyDown(device, key))
                {
                    this.keyStates[device.StrongName][key] = false;
                    if (this.InputActivity != null)
                    {
                        this.InputActivity(this, new InterceptionEventArgs(device, new List<KeyInfo>() { new KeyInfo(key, false) }));
                    }
                }
            });

            var task = new DelayedTask(action, delay);
            task.Run();
        }

        private void ConnectionCallback()
        {
            while (this.IsLoaded)
            {
                var newDevices = this.RescanInputDevices(this.devices);
                if (newDevices.Count != this.devices.Count)
                {
                    // Scanning for removed device
                    foreach (var oldDevice in this.devices)
                    {
                        if (!newDevices.ContainsKey(oldDevice.Key))
                        {
                            this.OnInputDeviceConnectionChanged(oldDevice.Value, true);
                        }
                    }

                    // Scanning for added device
                    foreach (var newDevice in newDevices)
                    {
                        if (!this.devices.ContainsKey(newDevice.Key))
                        {
                            this.devices.Add(newDevice.Key, newDevice.Value);
                            this.OnInputDeviceConnectionChanged(newDevice.Value, false);
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private Dictionary<int, InterceptionDevice> RescanInputDevices(Dictionary<int, InterceptionDevice> currentDeviceList)
        {
            var devices = new Dictionary<int, InterceptionDevice>();

            for (int id = 1; id < Interception.MaxDeviceCount; id++)
            {
                string hwid = this.GetHardwareID(id);
                if (hwid != null && !(NativeMethods.IsInvalid(id) > 0))
                {
                    bool isKeyboard = NativeMethods.IsKeyboard(id) > 0;
                    InterceptionDevice newDevice;

                    if (isKeyboard)
                    {
                        newDevice = new InterceptionKeyboard((uint)id, hwid);
                    }
                    else
                    {
                        newDevice = new InterceptionMouse((uint)id, hwid);
                    }

                    // Checking if we have this device in the current list
                    if (currentDeviceList.ContainsKey(id))
                    {
                        if (currentDeviceList[id].HasTheSamePropertiesAs(newDevice))
                        {
                            devices.Add(id, currentDeviceList[id]);
                            continue;
                        }
                    }

                    devices.Add(id, newDevice);
                }
            }

            return devices;
        }

        private List<KeyInfo> GetKeyboardKeys(KeyStroke keyStroke)
        {
            var output = new List<KeyInfo>();
            var corrected = KeysHelper.GetCorrectedKey(keyStroke.Code, keyStroke.State);
            bool isDown = keyStroke.State == KeyState.Down || keyStroke.State == (KeyState.Down | KeyState.E0);
            output.Add(new KeyInfo(corrected, isDown));

            return output;
        }

        private List<KeyInfo> GetMouseKeys(MouseStroke stroke)
        {
            var keys = new List<KeyInfo>();

            foreach (MouseState mouseState in Enum.GetValues(typeof(MouseState)))
            {
                if (stroke.State.HasFlag(mouseState))
                {
                    switch (mouseState)
                    {
                        case MouseState.None:
                            break;
                        case MouseState.LeftDown:
                            keys.Add(new KeyInfo(InterceptionKey.MouseLeftButton, true));
                            break;
                        case MouseState.LeftUp:
                            keys.Add(new KeyInfo(InterceptionKey.MouseLeftButton, false));
                            break;
                        case MouseState.RightDown:
                            keys.Add(new KeyInfo(InterceptionKey.MouseRightButton, true));
                            break;
                        case MouseState.RightUp:
                            keys.Add(new KeyInfo(InterceptionKey.MouseRightButton, false));
                            break;
                        case MouseState.MiddleDown:
                            keys.Add(new KeyInfo(InterceptionKey.MouseMiddleButton, true));
                            break;
                        case MouseState.MiddleUp:
                            keys.Add(new KeyInfo(InterceptionKey.MouseMiddleButton, false));
                            break;
                        case MouseState.LeftExtraDown:
                            keys.Add(new KeyInfo(InterceptionKey.MouseExtraLeft, true));
                            break;
                        case MouseState.LeftExtraUp:
                            keys.Add(new KeyInfo(InterceptionKey.MouseExtraLeft, false));
                            break;
                        case MouseState.RightExtraDown:
                            keys.Add(new KeyInfo(InterceptionKey.MouseExtraRight, true));
                            break;
                        case MouseState.RightExtraUp:
                            keys.Add(new KeyInfo(InterceptionKey.MouseExtraRight, false));
                            break;
                        case MouseState.Wheel:
                            if (stroke.Rolling > 0)
                            {
                                keys.Add(new KeyInfo(InterceptionKey.MouseWheelUp, true));
                            }
                            else if (stroke.Rolling < 0)
                            {
                                keys.Add(new KeyInfo(InterceptionKey.MouseWheelDown, true));
                            }

                            break;
                        case MouseState.HWheel:
                            if (stroke.Rolling > 0)
                            {
                                keys.Add(new KeyInfo(InterceptionKey.MouseWheelRight, true));
                            }
                            else if (stroke.Rolling < 0)
                            {
                                keys.Add(new KeyInfo(InterceptionKey.MouseWheelLeft, true));
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            if (stroke.X < -Interception.MouseMoveDeadZone)
            {
                keys.Add(new KeyInfo(InterceptionKey.MouseMoveLeft, true));
                this.ReleaseKeyWithDelay(this.currentDevice, InterceptionKey.MouseMoveLeft, this.mouseMoveAutoOffDelay);
            }
            else if (stroke.X > Interception.MouseMoveDeadZone)
            {
                keys.Add(new KeyInfo(InterceptionKey.MouseMoveRight, true));
                this.ReleaseKeyWithDelay(this.currentDevice, InterceptionKey.MouseMoveRight, this.mouseMoveAutoOffDelay);
            }

            if (stroke.Y < -Interception.MouseMoveDeadZone)
            {
                keys.Add(new KeyInfo(InterceptionKey.MouseMoveUp, true));
                this.ReleaseKeyWithDelay(this.currentDevice, InterceptionKey.MouseMoveUp, this.mouseMoveAutoOffDelay);
            }
            else if (stroke.Y > Interception.MouseMoveDeadZone)
            {
                keys.Add(new KeyInfo(InterceptionKey.MouseMoveDown, true));
                this.ReleaseKeyWithDelay(this.currentDevice, InterceptionKey.MouseMoveDown, this.mouseMoveAutoOffDelay);
            }

            return keys;
        }

        private void OnInputDeviceConnectionChanged(InterceptionDevice newDevice, bool isRemoved)
        {
            if (this.InputDeviceConnectionChanged != null)
            {
                this.InputDeviceConnectionChanged(this, new InterceptionDeviceEventArgs(newDevice, isRemoved));
            }
        }
    }
}