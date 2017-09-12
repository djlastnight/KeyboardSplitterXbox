namespace XinputWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using XinputWrapper.Enums;
    using XinputWrapper.Structs;

    public class XinputController
    {
        public const int MaxControllerCount = 4;
        public const int FirstControllerIndex = 0;
        public const int LastControllerIndex = MaxControllerCount - 1;
        public const int DefaultUpdateFrequency = (int)(1000 / 30.0);

        private static XinputController[] controllers;
        private static bool keepRunning;
        private static int updateFrequency;
        private static int waitTime;
        private static bool isRunning;
        private static object syncLock;
        private static Thread pollingThread;
        private static List<uint> freeLeds;

        private int playerIndex;
        private object tag;
        private bool stopMotorTimerActive;
        private DateTime stopMotorTime;
        private XInputBatteryInformation batteryInformationGamepad;
        private XInputBatteryInformation batterInformationHeadset;
        private XInputState gamepadStatePrev = new XInputState();
        private XInputState gamepadStateCurrent = new XInputState();

        static XinputController()
        {
            XinputController.controllers = new XinputController[MaxControllerCount];
            XinputController.syncLock = new object();
            for (int i = XinputController.FirstControllerIndex; i <= XinputController.LastControllerIndex; ++i)
            {
                XinputController.controllers[i] = new XinputController(i);
            }

            XinputController.UpdateFrequency = XinputController.DefaultUpdateFrequency;
            freeLeds = new List<uint>() { 0, 1, 2, 3 };
        }

        private XinputController(int playerIndex)
        {
            this.playerIndex = playerIndex;
            this.gamepadStatePrev.Copy(this.gamepadStateCurrent);
        }

        public event EventHandler<XinputControllerStateChangedEventArgs> StateChanged;

        public event EventHandler PluggedChanged;

        public static int UpdateFrequency
        {
            get
            {
                return XinputController.updateFrequency;
            }

            set
            {
                XinputController.updateFrequency = value;
                XinputController.waitTime = 1000 / XinputController.updateFrequency;
            }
        }

        public static int EmptyBusSlotsCount
        {
            get
            {
                return controllers.Where(x => !x.IsConnected).Count();
            }
        }

        public uint PlayerIndex
        {
            get
            {
                return (uint)this.playerIndex;
            }
        }

        public uint LedNumber
        {
            get
            {
                return this.PlayerIndex + 1;
            }
        }

        public object Tag
        {
            get
            {
                lock (syncLock)
                {
                    return this.tag;
                }
            }

            set
            {
                lock (syncLock)
                {
                    this.tag = value;
                }
            }
        }

        public XInputBatteryInformation BatteryInformationGamepad
        {
            get { return this.batteryInformationGamepad; }
            internal set { this.batteryInformationGamepad = value; }
        }

        public XInputBatteryInformation BatteryInformationHeadset
        {
            get { return this.batterInformationHeadset; }
            internal set { this.batterInformationHeadset = value; }
        }

        public bool IsConnected
        {
            get;
            private set;
        }

        public bool? IsGamepad
        {
            get
            {
                if (!this.IsConnected)
                {
                    return null;
                }

                return ((BatteryType)this.BatteryInformationGamepad.BatteryType) != BatteryType.Disconnected;
            }
        }

        public bool? IsWired
        {
            get
            {
                if (!this.IsConnected)
                {
                    return null;
                }

                if (this.IsGamepad == true)
                {
                    return (BatteryType)this.BatteryInformationGamepad.BatteryType == BatteryType.Wired;
                }
                else
                {
                    return (BatteryType)this.BatteryInformationHeadset.BatteryType == BatteryType.Wired;
                }
            }
        }

        public BatteryLevel BatteryLevel
        {
            get
            {
                if (!this.IsConnected)
                {
                    return Enums.BatteryLevel.Empty;
                }

                if (this.IsGamepad == true)
                {
                    return (BatteryLevel)this.BatteryInformationGamepad.BatteryLevel;
                }
                else
                {
                    return (BatteryLevel)this.BatteryInformationHeadset.BatteryLevel;
                }
            }
        }

        public static XinputController RetrieveController(int index)
        {
            if (index < 0 || index > 3)
            {
                throw new InvalidOperationException("index must be in range 0-3");
            }

            return XinputController.controllers[index];
        }

        public static void StartPolling()
        {
            foreach (var controller in XinputController.controllers)
            {
                controller.IsConnected = false;
            }

            if (!XinputController.isRunning)
            {
                lock (XinputController.syncLock)
                {
                    if (!XinputController.isRunning)
                    {
                        XinputController.pollingThread = new Thread(XinputController.PollerLoop);
                        XinputController.pollingThread.Start();
                    }
                }
            }
        }

        public static void StopPolling()
        {
            foreach (var controller in XinputController.controllers)
            {
                controller.IsConnected = false;
            }

            if (XinputController.isRunning)
            {
                XinputController.keepRunning = false;
            }
        }

        public XInputCapabilities GetCapabilities()
        {
            var capabilities = new XInputCapabilities();
            NativeMethods.XInputGetCapabilities(this.playerIndex, XInputConstants.XinputFlagGamepad, ref capabilities);
            return capabilities;
        }

        public ControllerSubtype GetControllerSubType()
        {
            return (ControllerSubtype)this.GetCapabilities().SubType;
        }

        public void Vibrate(double leftMotor, double rightMotor)
        {
            this.Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        public void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            var vibration = new XInputVibration()
            {
                LeftMotorSpeed = (ushort)(65535d * leftMotor),
                RightMotorSpeed = (ushort)(65535d * rightMotor)
            };

            this.Vibrate(vibration, length);
        }

        public void Vibrate(XInputVibration strength)
        {
            this.stopMotorTimerActive = false;
            NativeMethods.XInputSetState(this.playerIndex, ref strength);
        }

        public void Vibrate(XInputVibration strength, TimeSpan length)
        {
            NativeMethods.XInputSetState(this.playerIndex, ref strength);
            if (length != TimeSpan.MinValue)
            {
                this.stopMotorTime = DateTime.Now.Add(length);
                this.stopMotorTimerActive = true;
            }
        }

        public bool GetButtonState(XinputButton button)
        {
            return (this.gamepadStateCurrent.Gamepad.Buttons & (int)button) == (int)button;
        }

        public byte GetTriggerState(XinputTrigger trigger)
        {
            switch (trigger)
            {
                case XinputTrigger.Left:
                    return this.gamepadStateCurrent.Gamepad.LeftTrigger;
                case XinputTrigger.Right:
                    return this.gamepadStateCurrent.Gamepad.RightTrigger;
                default:
                    throw new NotImplementedException("Not implemented xinput trigger: " + trigger);
            }
        }

        public short GetAxisState(XinputAxis axis)
        {
            switch (axis)
            {
                case XinputAxis.X:
                    return this.gamepadStateCurrent.Gamepad.ThumbLX;
                case XinputAxis.Y:
                    return this.gamepadStateCurrent.Gamepad.ThumbLY;
                case XinputAxis.Rx:
                    return this.gamepadStateCurrent.Gamepad.ThumbRX;
                case XinputAxis.Ry:
                    return this.gamepadStateCurrent.Gamepad.ThumbRY;
                default:
                    throw new NotImplementedException("Not implemented xbox axis: " + axis);
            }
        }

        public override string ToString()
        {
            return this.playerIndex.ToString();
        }

        private static void PollerLoop()
        {
            lock (XinputController.syncLock)
            {
                if (XinputController.isRunning == true)
                {
                    return;
                }

                XinputController.isRunning = true;
            }

            XinputController.keepRunning = true;

            while (XinputController.keepRunning)
            {
                for (int i = XinputController.FirstControllerIndex; i <= XinputController.LastControllerIndex; ++i)
                {
                    XinputController.controllers[i].UpdateState();
                }

                Thread.Sleep(XinputController.updateFrequency);
            }

            lock (XinputController.syncLock)
            {
                XinputController.isRunning = false;
            }
        }

        private void OnStateChanged()
        {
            if (this.StateChanged != null)
            {
                var args = new XinputControllerStateChangedEventArgs();
                args.CurrentInputState = this.gamepadStateCurrent;
                args.PreviousInputState = this.gamepadStatePrev;
                args.Controller = XinputController.controllers[this.PlayerIndex];
                this.StateChanged(this, args);
            }
        }

        private void UpdateState()
        {
            int result = NativeMethods.XInputGetState(this.playerIndex, ref this.gamepadStateCurrent);
            bool isCurrentlyConnected = this.IsConnected;
            this.IsConnected = result == 0;

            if (isCurrentlyConnected != this.IsConnected)
            {
                this.OnControllerPluggedChanged();
            }

            this.UpdateBatteryState();

            if (this.gamepadStateCurrent.PacketNumber != this.gamepadStatePrev.PacketNumber)
            {
                this.OnStateChanged();
            }

            this.gamepadStatePrev.Copy(this.gamepadStateCurrent);

            if (this.stopMotorTimerActive && (DateTime.Now >= this.stopMotorTime))
            {
                var stopStrength = new XInputVibration()
                {
                    LeftMotorSpeed = 0,
                    RightMotorSpeed = 0
                };

                NativeMethods.XInputSetState(this.playerIndex, ref stopStrength);
            }
        }

        private void UpdateBatteryState()
        {
            var headset = new XInputBatteryInformation();
            var gamepad = new XInputBatteryInformation();

            NativeMethods.XInputGetBatteryInformation(this.playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_GAMEPAD, ref gamepad);
            NativeMethods.XInputGetBatteryInformation(this.playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_HEADSET, ref headset);

            this.BatteryInformationHeadset = headset;
            this.BatteryInformationGamepad = gamepad;
        }

        private void OnControllerPluggedChanged()
        {
            if (this.PluggedChanged != null)
            {
                this.PluggedChanged(this, EventArgs.Empty);
            }
        }
    }
}