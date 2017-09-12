namespace KeyboardSplitter.Models
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Exceptions.Gamepad;
    using SplitterCore;
    using SplitterCore.Emulation;
    using VirtualXbox;
    using VirtualXbox.Enums;
    using XinputWrapper;

    public class XboxGamepad : DependencyObject, IVirtualGamepad, INotifyPropertyChanged
    {
        public static readonly DependencyProperty FriendlyNameProperty =
            DependencyProperty.Register(
            "FriendlyName",
            typeof(string),
            typeof(XboxGamepad),
            new PropertyMetadata(XboxGamepad.FriendlyNamePrefix));

        private const string FriendlyNamePrefix = "Virtual Xbox 360 Controller";
        private XinputController xinputController;
        private uint ledNumber;
        private DateTime connectionRequestTime;

        public XboxGamepad(uint userIndex)
        {
            if (userIndex < 1 || userIndex > 4)
            {
                throw new ArgumentException("userIndex must be in range 1 - 4");
            }

            this.UserIndex = userIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Disconnected;

        public static bool AreXboxAccessoriesInstalled
        {
            get
            {
                var version = Environment.OSVersion.Version;
                bool isWin7 = version.Major == 6 && version.Minor == 1;
                bool isVista = version.Major == 6 && version.Minor == 0;
                bool isXp = version.Major == 5 && version.Minor == 1;

                if (isWin7 || isVista || isXp)
                {
                    string rootDriveLetter = Path.GetPathRoot(
                        Environment.GetFolderPath(Environment.SpecialFolder.Windows));

                    string xboxAccessoriesStatFile = Path.Combine(
                        rootDriveLetter, "Program Files", "Microsoft Xbox 360 Accessories", "XBoxStat.exe");

                    return File.Exists(xboxAccessoriesStatFile);
                }

                // Windows 8 and newer has this driver built-in
                return true;
            }
        }

        public string FriendlyName
        {
            get { return (string)this.GetValue(FriendlyNameProperty); }
            set { this.SetValue(FriendlyNameProperty, value); }
        }

        public uint UserIndex
        {
            get;
            private set;
        }

        public bool Exsists
        {
            get { return VirtualXboxController.Exists(this.UserIndex); }
        }

        public bool IsOwned
        {
            get { return VirtualXboxController.IsOwned(this.UserIndex); }
        }

        public uint LedNumber
        {
            get
            {
                return this.ledNumber;
            }

            set
            {
                this.ledNumber = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LedNumber"));
                }

                if (value < 1 || value > 4)
                {
                    this.FriendlyName = string.Format(
                        "{0} #{1}",
                        XboxGamepad.FriendlyNamePrefix,
                        this.UserIndex);
                }
                else
                {
                    this.FriendlyName = string.Format(
                        "{0} #{1} [Led #{2}]",
                        XboxGamepad.FriendlyNamePrefix,
                        this.UserIndex,
                        this.LedNumber);
                }
            }
        }

        public TimeSpan ConnectionTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Plugs the xbox controller into the virtual bus.
        /// </summary>
        /// <returns>Returns true if the process is successfull</returns>
        /// <exception cref="XboxAccessoriesNotInstalledException">XboxAccessoriesNotInstalledException</exception>
        /// <exception cref="VirtualBusNotInstalledException">VirtualBusNotInstalledException</exception>
        /// <exception cref="GamepadIsInUseException">GamepadIsInUseException</exception>
        /// <exception cref="GamepadOwnedException">GamepadOwnedException</exception>
        public bool PlugIn()
        {
            try
            {
                this.CheckDrivers();
            }
            catch (Exception)
            {
                throw;
            }

            if (VirtualXboxBus.EmptySlotsCount == 0)
            {
                throw new VirtualBusFullException("There is no free slot in the virtual bus");
            }

            if (this.Exsists)
            {
                throw new GamepadIsInUseException(this.FriendlyName + " is already mounted!");
            }

            var xinputControllers = new System.Collections.Generic.List<XinputController>();
            for (int i = 0; i < 4; i++)
            {
                xinputControllers.Add(XinputController.RetrieveController(i));
            }

            if (xinputControllers.TrueForAll(x => x.Tag != null))
            {
                throw new KeyboardSplitterExceptionBase("Internal Error: all xinput controllers are marked as virtual!");
            }

            this.xinputController = xinputControllers.FirstOrDefault(x => !x.IsConnected && x.Tag == null);
            if (this.xinputController != null)
            {
                this.xinputController.Tag = this;
                this.xinputController.PluggedChanged += this.OnXinputControllerPluggedChanged;
            }
            else
            {
                LogWriter.Write("--- Xinput controllers information ---");
                foreach (var xinputController in xinputControllers)
                {
                    LogWriter.Write(
                        string.Format(
                        "Controller #{0} [IsConnected:{1}] [IsVirtual:{2}]",
                        xinputController.LedNumber,
                        xinputController.IsConnected,
                        xinputController.Tag != null));
                }

                LogWriter.Write("--- End of Xinput controllers information---");

                throw new InvalidOperationException("Unknown error occured, just before plugging in the virtual controller!");
            }

            this.connectionRequestTime = DateTime.Now;
            if (!VirtualXboxController.PlugIn(this.UserIndex))
            {
                this.LedNumber = 0;
                this.xinputController.Tag = null;
                this.xinputController.PluggedChanged -= this.OnXinputControllerPluggedChanged;
                this.xinputController = null;
                return false;
            }

            return true;
        }

        public bool Unplug(bool isForced)
        {
            return VirtualXboxController.UnPlug(this.UserIndex, isForced);
        }

        public bool SetButtonState(uint button, bool value)
        {
            if (!Enum.IsDefined(typeof(XboxButton), button))
            {
                return false;
            }

            return VirtualXboxController.SetButton(this.UserIndex, (XboxButton)button, value);
        }

        public bool SetTriggerState(uint trigger, byte value)
        {
            if (!Enum.IsDefined(typeof(XboxTrigger), trigger))
            {
                return false;
            }

            return VirtualXboxController.SetTrigger(this.UserIndex, (XboxTrigger)trigger, value);
        }

        public bool SetDpadState(int value)
        {
            var flags = XboxDpadDirection.Off;
            foreach (XboxDpadDirection direction in Enum.GetValues(typeof(XboxDpadDirection)))
            {
                if ((value & (int)direction) > 0)
                {
                    flags |= direction;
                }
            }

            return VirtualXboxController.SetDPad(this.UserIndex, flags);
        }

        public bool SetAxisState(uint axis, short value)
        {
            if (!Enum.IsDefined(typeof(XboxAxis), axis))
            {
                return false;
            }

            return VirtualXboxController.SetAxis(this.UserIndex, (XboxAxis)axis, value);
        }

        public bool GetButtonState(uint button)
        {
            if (!Enum.IsDefined(typeof(XboxButton), button))
            {
                throw new InvalidOperationException("Xbox controller has no such button: " + button);
            }

            return VirtualXboxController.GetButtonValue(this.UserIndex, (XboxButton)button);
        }

        public byte GetTriggerState(uint trigger)
        {
            if (!Enum.IsDefined(typeof(XboxTrigger), trigger))
            {
                throw new InvalidOperationException("Xbox controller has no such trigger: " + trigger);
            }

            return VirtualXboxController.GetTriggerValue(this.UserIndex, (XboxTrigger)trigger);
        }

        public int GetDpadState()
        {
            return (int)VirtualXboxController.GetDpadState(this.UserIndex);
        }

        public short GetAxisState(uint axis)
        {
            if (!Enum.IsDefined(typeof(XboxAxis), axis))
            {
                throw new InvalidOperationException("Xbox controller has no such axis: " + axis);
            }

            return VirtualXboxController.GetAxisValue(this.UserIndex, (XboxAxis)axis);
        }

        public bool SetCustomFunctionState(uint function, bool value)
        {
            if (!Enum.IsDefined(typeof(XboxCustomFunction), function))
            {
                throw new InvalidOperationException("Xbox controller has no such custom function: " + function);
            }

            var xboxCustomFunction = (XboxCustomFunction)function;
            var functionType = Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomFunction);
            switch (functionType)
            {
                case FunctionType.Button:
                    return VirtualXboxController.SetButton(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetXboxButton(xboxCustomFunction),
                        value);
                case FunctionType.Axis:
                    {
                        XboxAxisPosition pos;
                        var axis = Helpers.CustomFunctionHelper.GetXboxAxis(xboxCustomFunction, out pos);
                        return VirtualXboxController.SetAxis(this.UserIndex, axis, (short)pos);
                    }

                case FunctionType.Dpad:
                    return VirtualXboxController.SetDPad(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetDpadDirection(xboxCustomFunction));
                case FunctionType.Trigger:
                    return VirtualXboxController.SetTrigger(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetXboxTrigger(xboxCustomFunction),
                        value ? byte.MaxValue : byte.MinValue);
                default:
                    throw new NotImplementedException("Not implemented Function Type: " + functionType);
            }
        }

        public bool GetCustomFunctionState(uint function)
        {
            if (!Enum.IsDefined(typeof(XboxCustomFunction), function))
            {
                throw new InvalidOperationException("Xbox controller has no such custom function: " + function);
            }

            var xboxCustomFunction = (XboxCustomFunction)function;
            var functionType = Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomFunction);
            switch (functionType)
            {
                case FunctionType.Button:
                    return VirtualXboxController.GetButtonValue(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetXboxButton(xboxCustomFunction));
                case FunctionType.Axis:
                    XboxAxisPosition pos;
                    var axis = Helpers.CustomFunctionHelper.GetXboxAxis(xboxCustomFunction, out pos);
                    return VirtualXboxController.GetAxisValue(this.UserIndex, axis) != 0;
                case FunctionType.Dpad:
                    return VirtualXboxController.GetDpadDirectionValue(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetDpadDirection(xboxCustomFunction));
                case FunctionType.Trigger:
                    return VirtualXboxController.GetTriggerValue(
                        this.UserIndex,
                        Helpers.CustomFunctionHelper.GetXboxTrigger(xboxCustomFunction)) != 0;
                default:
                    throw new NotImplementedException("Not implemented Function Type: " + functionType);
            }
        }

        private void CheckDrivers()
        {
            if (!XboxGamepad.AreXboxAccessoriesInstalled)
            {
                string error = "Microsoft Xbox 360 Accessories is not installed! To download it\r\n" +
                    "Click on 'Drivers' -> 'Get Xbox 360 Accessories Driver'";

                throw new XboxAccessoriesNotInstalledException(error);
            }

            if (!VirtualXboxBus.IsInstalled)
            {
                string error = "ScpVBus is not installed!";
                throw new VirtualBusNotInstalledException(error);
            }
        }

        private void OnXinputControllerPluggedChanged(object sender, EventArgs e)
        {
            var controller = sender as XinputController;
            if (controller == null || controller != this.xinputController)
            {
                return;
            }

            this.Dispatcher.BeginInvoke((Action)delegate
            {
                if (controller.IsConnected)
                {
                    this.LedNumber = controller.LedNumber;
                    controller.Tag = this;
                    this.xinputController = controller;
                    this.ConnectionTime = DateTime.Now - this.connectionRequestTime;
                }
                else
                {
                    this.OnDisconnected();
                    this.LedNumber = 0;
                    controller.Tag = null;
                    controller.PluggedChanged -= this.OnXinputControllerPluggedChanged;
                    this.xinputController = null;
                }
            });
        }

        private void OnDisconnected()
        {
            if (this.Disconnected != null)
            {
                this.Disconnected(this, EventArgs.Empty);
            }
        }
    }
}
