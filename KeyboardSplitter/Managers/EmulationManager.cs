namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Helpers;
    using KeyboardSplitter.Presets;
    using XboxInterfaceWrap;

    public class EmulationManager : IDisposable
    {
        private List<JoyControl> joyControls;

        public EmulationManager(uint slotsCount)
        {
            if (slotsCount < 1 || slotsCount > 4)
            {
                throw new ArgumentOutOfRangeException(
                    "slotsCount must be in range 1-4");
            }

            this.SlotsCount = slotsCount;
            PresetDataManager.PresetsChanged += this.PresetDataManager_PresetsChanged;
            UnplugAllJoys();

            this.joyControls = new List<JoyControl>();
            for (uint i = 1; i <= this.SlotsCount; i++)
            {
                this.joyControls.Add(new JoyControl(i));
            }
        }

        public uint SlotsCount
        {
            get;
            private set;
        }

        public List<JoyControl> JoyControls
        {
            get
            {
                return this.joyControls;
            }
        }

        public static void UnplugAllJoys()
        {
            for (uint i = 1; i <= 4; i++)
            {
                if (VirtualXboxController.Exists(i))
                {
                    if (VirtualXboxController.UnPlug(i, force: true))
                    {
                        LogWriter.Write("Unplugged device #" + i);
                    }
                    else
                    {
                        LogWriter.Write("Unplug device #" + i + " failed");
                    }
                }
            }
        }

        public void Start()
        {
            this.DoStartUpCheck();

            // plugging in the virtual controllers
            foreach (var joyControl in this.joyControls)
            {
                if (joyControl.IsInvalidated)
                {
                    continue;
                }

                if (!VirtualXboxController.PlugIn(joyControl.UserIndex))
                {
                    LogWriter.Write("Plug in device #" + joyControl.UserIndex + " failed");
                    continue;
                }

                joyControl.IsEnabled = false;
                LogWriter.Write("Plug in device #" + joyControl.UserIndex + " OK");
            }

            int mountedGamepadsCount = 4 - VirtualXboxController.GetEmptyBusSlotsCount();

            if (this.SlotsCount != mountedGamepadsCount)
            {
                var error = string.Format(
                    "Unexpected error occured: " +
                    "The slots count ({0}) is different from the created virtual gamepads count ({1})!",
                    this.SlotsCount,
                    mountedGamepadsCount);

                LogWriter.Write(error);

                EmulationManager.UnplugAllJoys();
                this.Stop();

                throw new SlotsCountMismatchException(error);
            }

            // logging
            LogWriter.Write(string.Format("Emulation started. Slots count: {0}", this.SlotsCount));

            foreach (var joyControl in this.joyControls)
            {
                var msg = string.Format(
                    "Emulation info for vXbox device #{0}: {1} | preset: {2}",
                    joyControl.UserIndex,
                    joyControl.CurrentKeyboard,
                    joyControl.PresetBoxText);

                LogWriter.Write(msg);
            }
        }

        public void Stop()
        {
            foreach (var joyControl in this.joyControls)
            {
                joyControl.IsEnabled = true;
            }

            UnplugAllJoys();
            LogWriter.Write("Emulation stopped");
        }

        public void ProcessKeyPress(KeyPressedEventArgs e, bool blockChoosenKeyboards)
        {
            foreach (var joyControl in this.joyControls)
            {
                if (joyControl.CurrentKeyboard == null ||
                    joyControl.CurrentKeyboard != e.Keyboard.StrongName ||
                    joyControl.IsInvalidated)
                {
                    continue;
                }

                if (!VirtualXboxController.Exists(joyControl.UserIndex))
                {
                    joyControl.Invalidate(SlotInvalidationReason.Controller_Unplugged);
                    continue;
                }

                foreach (KeyControl keyControl in joyControl.Children)
                {
                    if (keyControl.KeyGesture != e.CorrectedKey)
                    {
                        continue;
                    }

                    FeedXboxController(keyControl, e);
                }

                if (blockChoosenKeyboards)
                {
                    var choosenKeyboards = this.joyControls.Select(x => x.CurrentKeyboard).ToList();
                    if (choosenKeyboards.Contains(e.Keyboard.StrongName))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            if (this.joyControls != null)
            {
                this.Stop();

                foreach (var joyControl in this.joyControls)
                {
                    joyControl.Dispose();
                }
            }

            PresetDataManager.PresetsChanged -= this.PresetDataManager_PresetsChanged;
            UnplugAllJoys();
        }

        private static void FeedXboxController(KeyControl keyControl, KeyPressedEventArgs e)
        {
            bool isKeyDown = e.State == KeyState.Down || e.State == KeyState.E0;
            switch (keyControl.ControlType)
            {
                case KeyControlType.Button:
                    {
                        if (!isKeyDown)
                        {
                            // checking whether all keys, mapped to current xbox button are released
                            var similarKeyControls = keyControl.JoyParent.Children.FindAll(
                                x => x.ControlType == KeyControlType.Button && x != keyControl && x.Button == keyControl.Button);

                            if (!similarKeyControls.TrueForAll(x => KeyboardManager.IsKeyDown(x.JoyParent.CurrentKeyboard, x.KeyGesture) == false))
                            {
                                // not all keys, mapped to current xbox button are released
                                // leaving the xbox button pressed
                                return;
                            }
                        }

                        VirtualXboxController.SetButton(keyControl.JoyParent.UserIndex, keyControl.Button, isKeyDown);
                    }

                    break;
                case KeyControlType.Axis:
                    {
                        short newAxisValue = AxisHelper.CalculateAxisValue(keyControl.JoyParent, keyControl.Axis, keyControl.Position, e.State);
                        VirtualXboxController.SetAxis(keyControl.JoyParent.UserIndex, keyControl.Axis, newAxisValue);
                    }

                    break;
                case KeyControlType.Dpad:
                    {
                        var newDirection = PovHelper.CalculatePovDirection(keyControl.JoyParent, keyControl.DpadDirection, e.State);
                        VirtualXboxController.SetDPad(keyControl.JoyParent.UserIndex, newDirection);
                    }

                    break;
                case KeyControlType.Trigger:
                    {
                        if (!isKeyDown)
                        {
                            // checking whether all keys, mapped to current xbox trigger are released
                            var similarKeyControls = keyControl.JoyParent.Children.FindAll(
                                x => x.ControlType == KeyControlType.Trigger && x != keyControl && x.Trigger == keyControl.Trigger);

                            if (!similarKeyControls.TrueForAll(x => KeyboardManager.IsKeyDown(x.JoyParent.CurrentKeyboard, x.KeyGesture) == false))
                            {
                                // not all keys, mapped to current xbox trigger are released
                                // leaving the xbox trigger pressed
                                return;
                            }
                        }

                        byte newTriggerValue = isKeyDown ? byte.MaxValue : byte.MinValue;
                        VirtualXboxController.SetTrigger(keyControl.JoyParent.UserIndex, keyControl.Trigger, newTriggerValue);
                    }

                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented control type: " + keyControl.ControlType);
            }
        }

        private void DoStartUpCheck()
        {
            if (!DriversManager.IsXboxAccessoriesInstalled())
            {
                LogWriter.Write("Xbox accessories driver - Not installed, notifying the user");
                string error = "Microsoft Xbox 360 Accessories is not installed! To download it\r\n" +
                    "Click on 'Drivers' -> 'Get Xbox 360 Accessories Driver'";

                throw new XboxAccessoriesNotInstalledException(error);
            }

            var invalidJControls = this.joyControls.FindAll(x => x.IsInvalidated);
            if (invalidJControls.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Due to the following slots are invalidated, emulation can not start:");

                foreach (var invalidJControl in invalidJControls)
                {
                    sb.AppendLine(string.Format("-Slot #{0} [{1}]", invalidJControl.UserIndex, invalidJControl.InvalidateReason));
                }

                throw new SlotInvalidatedException(sb.ToString());
            }

            invalidJControls = this.joyControls.FindAll(x => x.IsInvalidated == false
                && x.CurrentKeyboard == null);

            if (invalidJControls.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("The following slots has NOT been assigned to keyboards:");
                foreach (var invalidJControl in invalidJControls)
                {
                    sb.AppendLine(string.Format("-Slot #{0} [vXbox Device #{0}]", invalidJControl.UserIndex));
                }

                sb.AppendLine();
                sb.AppendLine("Please choose keyboards for all the slots and hit the 'Start' button again.");

                throw new KeyboardNotSetException(sb.ToString());
            }
        }

        private void PresetDataManager_PresetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.joyControls == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var joyControl in this.joyControls)
                {
                    if (joyControl.CurrentPreset == e.OldItems[0])
                    {
                        joyControl.CurrentPreset = Preset.Empty;
                    }
                }

                LogWriter.Write("Preset deleted: " + (e.OldItems[0] as Preset).Name);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                Preset preset = e.NewItems[0] as Preset;
                foreach (var joyControl in this.joyControls)
                {
                    if (joyControl.PresetBoxText == preset.Name)
                    {
                        joyControl.CurrentPreset = preset;
                    }
                }

                var msg = string.Format(
                    "Preset {0}: {1}",
                    e.Action == NotifyCollectionChangedAction.Add ? "created" : "overwritted",
                    preset.Name);

                LogWriter.Write(msg);
            }
        }
    }
}