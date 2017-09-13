namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Interceptor;
    using Interceptor.Enums;
    using KeyboardSplitter.Helpers;
    using SplitterCore.Input;

    public class InputManager : DependencyObject, IInputManager, IDisposable
    {
        public const InputKey EmergencyLeftKey = InputKey.LeftControl;

        public const InputKey EmergencyRightKey = InputKey.RightControl;

        public static readonly DependencyProperty KeyboardsProperty =
            DependencyProperty.Register(
            "Keyboards",
            typeof(List<Keyboard>),
            typeof(InputManager),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MiceProperty =
            DependencyProperty.Register(
            "Mice",
            typeof(List<Mouse>),
            typeof(InputManager),
            new PropertyMetadata(null));

        public static readonly DependencyProperty InputMonitorHistoryProperty =
            DependencyProperty.Register(
            "InputMonitorHistory",
            typeof(string),
            typeof(InputManager),
            new PropertyMetadata(string.Empty));

        private Interception interceptor;

        private int emergencyLeftDownCount;

        private int emergencyLeftUpCount;

        private int emergencyRightDownCount;

        private int emergencyRightUpCount;

        public InputManager()
        {
            this.CheckKeysEnumerations();
            LogWriter.Write("Creating input manager");
            this.interceptor = new Interception(KeyboardFilterMode.All, MouseFilterMode.All);
            if (!this.interceptor.Load())
            {
                LogWriter.Write("Interceptor.Load() failed!");
                throw new Exception("Interceptor failed to load!");
            }

            this.interceptor.InputActivity += this.OnInterceptionInputActivity;
            this.interceptor.InputDeviceConnectionChanged += this.OnInterceptionInputDeviceConnectionChanged;
            LogWriter.Write("Gathering keyboards");
            this.Keyboards = this.GetInterceptionKeyboards();
            LogWriter.Write("Gathering mice");
            this.Mice = this.GetInterceptionMice();
        }

        public event EventHandler<InputEventArgs> InputActivity;

        public event EventHandler<InputDeviceChangedEventArgs> InputDeviceChanged;

        public event EventHandler EmergencyLeft;

        public event EventHandler EmergencyRight;

        public event EventHandler EmergencyStop;

        public static List<InputDevice> ConnectedInputDevices
        {
            get
            {
                var interceptionDevices = Interception.ConnectedInputDevices;
                var list = new List<InputDevice>();
                foreach (var interceptionDevice in interceptionDevices)
                {
                    list.Add(Helpers.InputHelper.ToInputDevice(interceptionDevice));
                }

                return list;
            }
        }

        public List<Keyboard> Keyboards
        {
            get { return (List<Keyboard>)this.GetValue(KeyboardsProperty); }
            set { this.SetValue(KeyboardsProperty, value); }
        }

        public List<Mouse> Mice
        {
            get { return (List<Mouse>)this.GetValue(MiceProperty); }
            set { this.SetValue(MiceProperty, value); }
        }

        public string InputMonitorHistory
        {
            get { return (string)this.GetValue(InputMonitorHistoryProperty); }
            set { this.SetValue(InputMonitorHistoryProperty, value); }
        }

        protected bool IsDestroyed { get; set; }

        public void ClearInputMonitorHistory()
        {
            this.InputMonitorHistory = string.Empty;
        }

        public bool IsKeyDown(InputDevice inputDevice, InputKey key)
        {
            if (inputDevice == null)
            {
                throw new ArgumentNullException("inputDevice");
            }

            var interceptionDevice = InputHelper.ToInterceptionDevice(inputDevice);
            if (interceptionDevice == null)
            {
                return false;
            }

            var interceptionKey = InputHelper.ToInterceptionKey(key);

            return this.interceptor.IsKeyDown(interceptionDevice, interceptionKey);
        }

        public void Destroy()
        {
            if (this.IsDestroyed)
            {
                return;
            }

            this.interceptor.InputActivity -= this.OnInterceptionInputActivity;
            this.interceptor.Unload();
            this.InputActivity = null;
            this.InputDeviceChanged = null;
            this.IsDestroyed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.interceptor != null)
                {
                    this.interceptor.Dispose();
                }
            }
        }

        private void CheckKeysEnumerations()
        {
            // Checking both keys enumerations
            var intercetionKeys = Enum.GetValues(typeof(InterceptionKey));
            var inputKeys = Enum.GetValues(typeof(InputKey));

            if (intercetionKeys.Length != inputKeys.Length)
            {
                throw new InvalidProgramException(
                    "InterceptionKey enum and InputKey enum elements count mismatch!");
            }

            if (Enum.GetUnderlyingType(typeof(InterceptionKey)) != typeof(ushort))
            {
                throw new InvalidProgramException("InterceptionKey enumeration's underlaying type must be ushort!");
            }

            if (Enum.GetUnderlyingType(typeof(InputKey)) != typeof(ushort))
            {
                throw new InvalidProgramException("InputKey enumeration's underlaying type must be ushort!");
            }

            foreach (InterceptionKey interceptionKey in intercetionKeys)
            {
                bool foundMatch = false;
                foreach (InputKey inputKey in inputKeys)
                {
                    if ((ushort)interceptionKey == (ushort)inputKey &&
                        interceptionKey.ToString() == inputKey.ToString())
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    throw new InvalidProgramException(
                        string.Format(
                        "Can not find '{0}' corresponding element at InputKey enumeration",
                        interceptionKey.ToString()));
                }
            }
        }

        private List<Keyboard> GetInterceptionKeyboards()
        {
            var keyboards = new List<Keyboard>();
            keyboards.Add(Keyboard.None);

            foreach (var interceptionKeyboard in this.interceptor.GetKeyboards())
            {
                var keyboard = (Keyboard)InputHelper.ToInputDevice(interceptionKeyboard);
                keyboards.Add(keyboard);
            }

            return keyboards;
        }

        private List<Mouse> GetInterceptionMice()
        {
            var mice = new List<Mouse>();
            mice.Add(Mouse.None);
            foreach (var interceptionMouse in this.interceptor.GetMice())
            {
                var mouse = (Mouse)InputHelper.ToInputDevice(interceptionMouse);
                mice.Add(mouse);
            }

            return mice;
        }

        private void OnInterceptionInputActivity(object sender, InterceptionEventArgs e)
        {
            var action = new Action(() =>
            {
                foreach (var keyInfo in e.KeyInfos)
                {
                    var device = InputHelper.ToInputDevice(e.Device);
                    var key = InputHelper.ToInputKey(keyInfo.Key);
                    var args = new InputEventArgs(device, key, keyInfo.IsDown, e.Handled);

                    if (this.InputActivity != null)
                    {
                        this.InputActivity(this, args);
                        e.Handled = args.Handled;
                    }

                    if (!KeysHelper.IsMouseMoveKey(keyInfo.Key))
                    {
                        var sb = new StringBuilder(this.InputMonitorHistory);
                        sb.AppendLine(args.ToString());
                        this.InputMonitorHistory = sb.ToString();
                    }
                }

                this.CheckForEmergencyHit(e);
            });

            bool isMouseClick = e.Device.DeviceType == InterceptionDeviceType.Mouse && e.KeyInfos.Any(x => KeysHelper.IsMouseClickKey(x.Key));
            
            // App.IsFocused is maybe too slow and the ui freezes the splitter main window
            if (isMouseClick && GlobalSettings.IsMainWindowActivated)
            {
                this.Dispatcher.BeginInvoke(action);
            }
            else
            {
                this.Dispatcher.Invoke(action);
            }

            if (GlobalSettings.IsMainWindowActivated)
            {
                e.Handled = false;
            }
        }

        private void OnInterceptionInputDeviceConnectionChanged(object sender, InterceptionDeviceEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                if (e.Device.DeviceType == InterceptionDeviceType.Keyboard)
                {
                    this.Keyboards = this.GetInterceptionKeyboards();
                }
                else
                {
                    this.Mice = this.GetInterceptionMice();
                }

                this.OnInputDeviceChanged(Helpers.InputHelper.ToInputDevice(e.Device), e.IsRemoved);
            });
        }

        private void CheckForEmergencyHit(InterceptionEventArgs e)
        {
            bool isCtrlDown = this.interceptor.IsKeyDown(e.Device, InterceptionKey.LeftControl) || this.interceptor.IsKeyDown(e.Device, InterceptionKey.RightControl);
            bool isAltDown = this.interceptor.IsKeyDown(e.Device, InterceptionKey.LeftAlt) || this.interceptor.IsKeyDown(e.Device, InterceptionKey.RightAlt);
            bool isDeleteDown = this.interceptor.IsKeyDown(e.Device, InterceptionKey.Delete) || this.interceptor.IsKeyDown(e.Device, InterceptionKey.NumpadDelete);

            if (isCtrlDown && isAltDown && isDeleteDown)
            {
                if (this.EmergencyStop != null)
                {
                    this.EmergencyStop(this, EventArgs.Empty);
                }

                this.ResetEmergency();
                return;
            }

            foreach (var keyInfo in e.KeyInfos)
            {
                if (keyInfo.Key == InputHelper.ToInterceptionKey(InputManager.EmergencyLeftKey))
                {
                    if (keyInfo.IsDown)
                    {
                        this.emergencyLeftDownCount++;
                    }
                    else
                    {
                        this.emergencyLeftUpCount++;
                    }
                }
                else if (keyInfo.Key == InputHelper.ToInterceptionKey(InputManager.EmergencyRightKey))
                {
                    if (keyInfo.IsDown)
                    {
                        this.emergencyRightDownCount++;
                    }
                    else
                    {
                        this.emergencyRightUpCount++;
                    }
                }
                else
                {
                    this.ResetEmergency();
                    return;
                }
            }

            if (this.emergencyLeftDownCount >= 5 && this.emergencyLeftUpCount >= 5)
            {
                this.OnEmergencyDetected(InputManager.EmergencyLeftKey);
            }
            else if (this.emergencyRightDownCount >= 5 && this.emergencyRightUpCount >= 5)
            {
                this.OnEmergencyDetected(InputManager.EmergencyRightKey);
            }
        }

        private void ResetEmergency()
        {
            this.emergencyLeftDownCount = 0;
            this.emergencyLeftUpCount = 0;
            this.emergencyRightDownCount = 0;
            this.emergencyRightUpCount = 0;
        }

        private void OnInputDeviceChanged(InputDevice device, bool isRemoved)
        {
            // Refreshing the devices
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.Keyboards = this.GetInterceptionKeyboards();
                this.Mice = this.GetInterceptionMice();

                if (this.InputDeviceChanged != null)
                {
                    this.InputDeviceChanged(this, new InputDeviceChangedEventArgs(device, isRemoved));
                }
            });
        }

        private void OnEmergencyDetected(InputKey key)
        {
            if (key == InputManager.EmergencyLeftKey)
            {
                if (this.EmergencyLeft != null)
                {
                    this.EmergencyLeft(this, EventArgs.Empty);
                    this.ResetEmergency();
                }
            }
            else if (key == InputManager.EmergencyRightKey)
            {
                if (this.EmergencyRight != null)
                {
                    this.EmergencyRight(this, EventArgs.Empty);
                    this.ResetEmergency();
                }
            }
        }
    }
}