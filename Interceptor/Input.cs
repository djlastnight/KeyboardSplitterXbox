namespace Interceptor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.Win32;

    public class Input : IDisposable
    {
        private const int HardwareIdSize = 500;

        private IntPtr context;

        private Thread callbackThread;

        private StringBuilder hardwareId;

        private int deviceId;

        public Input(KeyboardFilterMode keyboardFilter, MouseFilterMode mouseFilter)
        {
            this.KeyboardFilterMode = keyboardFilter;
            this.MouseFilterMode = mouseFilter;

            this.KeyPressDelay = 1;
            this.ClickDelay = 1;
            this.ScrollDelay = 15;
            this.hardwareId = new StringBuilder(Input.HardwareIdSize);
        }

        public event EventHandler<KeyPressedEventArgs> OnKeyPressed;

        public event EventHandler<MousePressedEventArgs> OnMousePressed;

        /// <summary>
        /// Gets or sets the Keyboard Filter Mode, which
        /// determines whether the driver traps no keyboard events, all events,
        /// or a range of events in-between (down only, up only...etc).
        /// Set this before loading otherwise the driver will not filter any events and no key presses can be sent.
        /// </summary>
        public KeyboardFilterMode KeyboardFilterMode { get; set; }

        /// <summary>
        /// Gets or sets the Mouse Filter Mode, which
        /// determines whether the driver traps no events, all events,
        /// or a range of events in-between.
        /// Set this before loading otherwise the driver will not filter any events
        /// and no mouse clicks can be sent.
        /// </summary>
        public MouseFilterMode MouseFilterMode { get; set; }

        public bool IsLoaded { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each key stroke down and up.
        /// Pressing a key requires both a key stroke down and up.
        /// A delay of 0 (inadvisable) may result in no keys being apparently pressed.
        /// A delay of 20 - 40 milliseconds makes the key presses visible.
        /// </summary>
        public int KeyPressDelay { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each mouse event down and up.
        /// 'Clicking' the cursor (whether left or right) requires both a mouse event down and up.
        /// A delay of 0 (inadvisable) may result in no apparent click.
        /// A delay of 20 - 40 milliseconds makes the clicks apparent.
        /// </summary>
        public int ClickDelay { get; set; }

        public int ScrollDelay { get; set; }

        public void Dispose()
        {
            this.Unload();
        }

        /*
         * Attempts to load the driver. You may get an error if the C++ library 'interception.dll' is not in the same folder as the executable and other DLLs. MouseFilterMode and KeyboardFilterMode must be set before Load() is called. Calling Load() twice has no effect if already loaded.
         */
        public bool Load()
        {
            if (this.IsLoaded)
            {
                return false;
            }

            this.context = NativeMethods.CreateContext();

            if (this.context != IntPtr.Zero)
            {
                this.callbackThread = new Thread(new ThreadStart(this.DriverCallback));
                this.callbackThread.Priority = ThreadPriority.Highest;
                this.callbackThread.IsBackground = true;
                this.callbackThread.Start();

                this.IsLoaded = true;

                return true;
            }
            else
            {
                this.IsLoaded = false;

                return false;
            }
        }

        /*
         * Safely unloads the driver. Calling Unload() twice has no effect.
         */
        public void Unload()
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (this.context != IntPtr.Zero)
            {
                this.callbackThread.Abort();
                NativeMethods.DestroyContext(this.context);
                this.IsLoaded = false;
            }
        }

        public void SendKey(InterceptionKeys key, KeyState state)
        {
            Stroke stroke = new Stroke();
            KeyStroke keyStroke = new KeyStroke();

            keyStroke.Code = key;
            keyStroke.State = state;

            stroke.Key = keyStroke;

            NativeMethods.Send(this.context, this.deviceId, ref stroke, 1);

            if (this.KeyPressDelay > 0)
            {
                Thread.Sleep(this.KeyPressDelay);
            }
        }

        /// <summary>
        /// Warning: Do not use this overload of SendKey() for non-letter, non-number, or non-ENTER keys. It may require a special KeyState of not KeyState.Down or KeyState.Up, but instead KeyState.E0 and KeyState.E1.
        /// </summary>
        /// <param name="key">Key to send.</param>
        public void SendKey(InterceptionKeys key)
        {
            this.SendKey(key, KeyState.Down);

            if (this.KeyPressDelay > 0)
            {
                Thread.Sleep(this.KeyPressDelay);
            }

            this.SendKey(key, KeyState.Up);
        }

        public void SendKeys(params InterceptionKeys[] keys)
        {
            foreach (InterceptionKeys key in keys)
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
        public void SendText(string text)
        {
            foreach (char letter in text)
            {
                var tuple = this.CharacterToKeysEnum(letter);

                if (tuple.Item2 == true)
                {
                    // We need to press shift to get the next character
                    this.SendKey(InterceptionKeys.LeftShift, KeyState.Down);
                }

                this.SendKey(tuple.Item1);

                if (tuple.Item2 == true)
                {
                    this.SendKey(InterceptionKeys.LeftShift, KeyState.Up);
                }
            }
        }

        public void SendMouseEvent(MouseState state)
        {
            Stroke stroke = new Stroke();
            MouseStroke mouseStroke = new MouseStroke();

            mouseStroke.State = state;

            if (state == MouseState.ScrollUp)
            {
                mouseStroke.Rolling = 120;
            }
            else if (state == MouseState.ScrollDown)
            {
                mouseStroke.Rolling = -120;
            }

            stroke.Mouse = mouseStroke;

            NativeMethods.Send(this.context, 12, ref stroke, 1);
        }

        public void SendLeftClick()
        {
            this.SendMouseEvent(MouseState.LeftDown);
            Thread.Sleep(this.ClickDelay);
            this.SendMouseEvent(MouseState.LeftUp);
        }

        public void SendRightClick()
        {
            this.SendMouseEvent(MouseState.RightDown);
            Thread.Sleep(this.ClickDelay);
            this.SendMouseEvent(MouseState.RightUp);
        }

        public void ScrollMouse(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Down:
                    this.SendMouseEvent(MouseState.ScrollDown);
                    break;
                case ScrollDirection.Up:
                    this.SendMouseEvent(MouseState.ScrollUp);
                    break;
            }
        }

        /// <summary>
        /// Gets list of all connected keyboards.
        /// </summary>
        /// <returns>List of up to date keyboards.</returns>
        public List<InterceptionKeyboard> GetKeyboards()
        {
            var output = new List<InterceptionKeyboard>();

            for (int i = 1; i <= 10; i++)
            {
                string hwid = this.GetHardwareID(i);
                if (hwid != null)
                {
                    output.Add(new InterceptionKeyboard()
                    {
                        DeviceID = (uint)i,
                        HardwareID = hwid,
                        FriendlyName = this.GetKeyboardFriendlyName(hwid),
                        StrongName = "Keyboard_" + i.ToString().PadLeft(2, '0'),
                    });
                }
            }

            return output;
        }

        /// <summary>
        /// Converts a character to a Keys enum and a 'do we need to press shift'.
        /// </summary>
        /// <returns>Tuple of desired Interception Key and boolean value,
        /// which determines whether shift key should be pressed.
        /// </returns>
        private Tuple<InterceptionKeys, bool> CharacterToKeysEnum(char ch)
        {
            switch (char.ToLower(ch))
            {
                case 'a':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.A, false);
                case 'b':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.B, false);
                case 'c':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.C, false);
                case 'd':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.D, false);
                case 'e':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.E, false);
                case 'f':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.F, false);
                case 'g':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.G, false);
                case 'h':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.H, false);
                case 'i':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.I, false);
                case 'j':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.J, false);
                case 'k':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.K, false);
                case 'l':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.L, false);
                case 'm':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.M, false);
                case 'n':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.N, false);
                case 'o':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.O, false);
                case 'p':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.P, false);
                case 'q':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Q, false);
                case 'r':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.R, false);
                case 's':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.S, false);
                case 't':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.T, false);
                case 'u':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.U, false);
                case 'v':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.V, false);
                case 'w':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.W, false);
                case 'x':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.X, false);
                case 'y':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Y, false);
                case 'z':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Z, false);
                case '1':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.One, false);
                case '2':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Two, false);
                case '3':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Three, false);
                case '4':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Four, false);
                case '5':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Five, false);
                case '6':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Six, false);
                case '7':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Seven, false);
                case '8':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Eight, false);
                case '9':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Nine, false);
                case '0':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Zero, false);
                case '-':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.DashUnderscore, false);
                case '+':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PlusEquals, false);
                case '[':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.OpenBracketBrace, false);
                case ']':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CloseBracketBrace, false);
                case ';':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SemicolonColon, false);
                case '\'':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SingleDoubleQuote, false);
                case ',':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CommaLeftArrow, false);
                case '.':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PeriodRightArrow, false);
                case '/':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, false);
                case '{':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.OpenBracketBrace, true);
                case '}':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CloseBracketBrace, true);
                case ':':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SemicolonColon, true);
                case '\"':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SingleDoubleQuote, true);
                case '<':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CommaLeftArrow, true);
                case '>':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PeriodRightArrow, true);
                case '?':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, true);
                case '\\':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.BackslashPipe, false);
                case '|':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.BackslashPipe, true);
                case '`':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Tilde, false);
                case '~':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Tilde, true);
                case '!':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.One, true);
                case '@':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Two, true);
                case '#':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Three, true);
                case '$':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Four, true);
                case '%':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Five, true);
                case '^':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Six, true);
                case '&':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Seven, true);
                case '*':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Eight, true);
                case '(':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Nine, true);
                case ')':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Zero, true);
                case ' ':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Space, true);
                default:
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, true);
            }
        }

        private string GetHardwareID(int device)
        {
            int length = NativeMethods.GetHardwareId(
                this.context,
                device,
                this.hardwareId,
                Input.HardwareIdSize);

            if (length > 0 && length < Input.HardwareIdSize)
            {
                return this.hardwareId.ToString();
            }

            return null;
        }

        private string GetKeyboardFriendlyName(string hardwareID)
        {
            try
            {
                if (hardwareID.Contains("REV"))
                {
                    // removing the revision from the hardware id
                    int revision_index = hardwareID.IndexOf("REV");
                    int apersandIndex = hardwareID.IndexOf("&", revision_index);
                    hardwareID = hardwareID.Substring(0, revision_index) + hardwareID.Substring(apersandIndex + 1);
                }

                using (var rootKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Enum\" + hardwareID))
                {
                    var subKeys = this.GetAllSubKeys(rootKey).Where(x => x.GetValue("Class") != null && x.GetValue("Class").ToString() == "Keyboard");

                    var fullDescription = subKeys.Select(x => x.GetValue("DeviceDesc")).First().ToString();
                    foreach (var subkey in subKeys)
                    {
                        subkey.Dispose();
                    }

                    string friendlyName = fullDescription.Substring(fullDescription.ToString().IndexOf(';') + 1);

                    return friendlyName;
                }
            }
            catch (Exception)
            {
                return "n/a";
            }
        }

        private List<RegistryKey> GetAllSubKeys(RegistryKey rootKey, bool recursive = false)
        {
            List<RegistryKey> allKeys = new List<RegistryKey>();
            foreach (var subkeyName in rootKey.GetSubKeyNames())
            {
                if (subkeyName == "Properties")
                {
                    continue;
                }

                var subkey = rootKey.OpenSubKey(subkeyName);
                if (subkey != null)
                {
                    allKeys.Add(subkey);
                    allKeys.AddRange(this.GetAllSubKeys(subkey, true));
                }
            }

            return allKeys;
        }

        private void DriverCallback()
        {
            NativeMethods.SetFilter(this.context, NativeMethods.IsKeyboard, (int)this.KeyboardFilterMode);
            NativeMethods.SetFilter(this.context, NativeMethods.IsMouse, (int)MouseFilterMode);

            Stroke stroke = new Stroke();

            while (NativeMethods.Receive(this.context, this.deviceId = NativeMethods.Wait(this.context), ref stroke, 1) > 0)
            {
                if (NativeMethods.IsMouse(this.deviceId) > 0)
                {
                    if (this.OnMousePressed != null)
                    {
                        var args = new MousePressedEventArgs()
                        {
                            X = stroke.Mouse.X,
                            Y = stroke.Mouse.Y,
                            State = stroke.Mouse.State,
                            Rolling = stroke.Mouse.Rolling
                        };

                        this.OnMousePressed(this, args);

                        if (args.Handled)
                        {
                            continue;
                        }

                        stroke.Mouse.X = args.X;
                        stroke.Mouse.Y = args.Y;
                        stroke.Mouse.State = args.State;
                        stroke.Mouse.Rolling = args.Rolling;
                    }
                }

                if (NativeMethods.IsKeyboard(this.deviceId) > 0)
                {
                    if (this.OnKeyPressed != null)
                    {
                        string hardwareID = this.GetHardwareID(this.deviceId);
                        var args = new KeyPressedEventArgs()
                        {
                            Key = stroke.Key.Code,
                            State = stroke.Key.State,
                            Keyboard = new InterceptionKeyboard()
                            {
                                HardwareID = hardwareID,
                                DeviceID = (uint)this.deviceId,
                                FriendlyName = this.GetKeyboardFriendlyName(hardwareID),
                                StrongName = "Keyboard_" + this.deviceId.ToString().PadLeft(2, '0'),
                            }
                        };

                        this.OnKeyPressed(this, args);

                        if (args.Handled)
                        {
                            continue;
                        }

                        stroke.Key.Code = args.Key;
                        stroke.Key.State = args.State;
                    }
                }

                NativeMethods.Send(this.context, this.deviceId, ref stroke, 1);
            }

            this.Unload();

            throw new Exception(
                "Interception.Receive() failed for an unknown reason. The driver has been unloaded.");
        }
    }
}
