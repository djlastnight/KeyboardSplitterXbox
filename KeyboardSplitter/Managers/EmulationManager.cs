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

    public static class EmulationManager
    {
        private static List<JoyControl> joyControls;

        public static bool IsCreated
        {
            get;
            private set;
        }

        public static bool IsRunning
        {
            get;
            private set;
        }

        public static uint SlotsCount
        {
            get;
            private set;
        }

        public static List<JoyControl> JoyControls
        {
            get
            {
                return EmulationManager.joyControls;
            }
        }

        public static void Create(uint slotsCount, params string[] keyboards)
        {
            if (slotsCount < 1 || slotsCount > 4)
            {
                throw new ArgumentOutOfRangeException(
                    "slotsCount must be in range 1-4");
            }

            if (EmulationManager.IsCreated)
            {
                EmulationManager.Destroy();
            }

            EmulationManager.SlotsCount = slotsCount;
            PresetDataManager.PresetsChanged += EmulationManager.PresetDataManager_PresetsChanged;

            EmulationManager.joyControls = new List<JoyControl>();
            for (uint index = 1; index <= EmulationManager.SlotsCount; index++)
            {
                var joyControl = new JoyControl(index);
                if (keyboards != null && keyboards.Length > index)
                {
                    joyControl.SetKeyboard(keyboards[index]);
                }

                EmulationManager.joyControls.Add(joyControl);
            }

            EmulationManager.IsCreated = true;
        }

        public static void Destroy()
        {
            if (EmulationManager.IsCreated)
            {
                EmulationManager.Stop();

                foreach (var joyControl in EmulationManager.joyControls)
                {
                    joyControl.Dispose();
                }

                EmulationManager.joyControls.Clear();
                EmulationManager.joyControls = null;
                PresetDataManager.PresetsChanged -= EmulationManager.PresetDataManager_PresetsChanged;
                EmulationManager.IsCreated = false;
            }
        }

        public static void Start()
        {
            if (!EmulationManager.IsCreated)
            {
                throw new InvalidOperationException(
                    "You can not start the emulation manager, before you created it");
            }

            EmulationManager.DoStartUpCheck();

            // plugging in the virtual controllers
            foreach (var joyControl in EmulationManager.joyControls)
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

                joyControl.Lock();
                LogWriter.Write("Plug in device #" + joyControl.UserIndex + " OK");
            }

            int mountedGamepadsCount = 4 - VirtualXboxController.GetEmptyBusSlotsCount();

            if (EmulationManager.SlotsCount != mountedGamepadsCount)
            {
                var error = string.Format(
                    "Virtual controllers (bus) error occured: " +
                    "The slots count ({0}) is different from the created virtual gamepads count ({1})! " +
                    "Please restart the application. If the problem still persists, restart your PC.",
                    EmulationManager.SlotsCount,
                    mountedGamepadsCount);

                LogWriter.Write(error);

                EmulationManager.Stop();

                throw new SlotsCountMismatchException(error);
            }

            EmulationManager.IsRunning = true;

            // logging
            LogWriter.Write(string.Format("Emulation started. Slots count: {0}", EmulationManager.SlotsCount));

            foreach (var joyControl in EmulationManager.joyControls)
            {
                var msg = string.Format(
                    "Emulation info for vXbox device #{0}: {1} | preset: {2}",
                    joyControl.UserIndex,
                    joyControl.CurrentKeyboard,
                    joyControl.PresetBoxText);

                LogWriter.Write(msg);
            }
        }

        public static void Stop()
        {
            if (EmulationManager.IsRunning)
            {
                foreach (var joyControl in EmulationManager.joyControls)
                {
                    joyControl.UnLock();
                }

                for (uint i = 1; i <= 4; i++)
                {
                    VirtualXboxController.UnPlug(i, force: true);
                }

                EmulationManager.IsRunning = false;

                LogWriter.Write("Emulation stopped");
            }
        }

        public static void ProcessKeyPress(KeyPressedEventArgs e, bool blockChoosenKeyboards)
        {
            if (!EmulationManager.IsCreated)
            {
                throw new InvalidOperationException(
                    "You can not process key press, before you create the emulation manager.");
            }

            if (!EmulationManager.IsRunning)
            {
                throw new InvalidOperationException(
                    "You can not process key press, because emulation manager is not running.");
            }

            foreach (var joyControl in EmulationManager.joyControls)
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
                    var choosenKeyboards = EmulationManager.joyControls.Select(x => x.CurrentKeyboard).ToList();
                    if (choosenKeyboards.Contains(e.Keyboard.StrongName))
                    {
                        e.Handled = true;
                    }
                }
            }
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

        private static void DoStartUpCheck()
        {
            if (!DriversManager.IsXboxAccessoriesInstalled())
            {
                LogWriter.Write("Xbox accessories driver - Not installed, notifying the user");
                string error = "Microsoft Xbox 360 Accessories is not installed! To download it\r\n" +
                    "Click on 'Drivers' -> 'Get Xbox 360 Accessories Driver'";

                throw new XboxAccessoriesNotInstalledException(error);
            }

            var invalidJControls = EmulationManager.joyControls.FindAll(x => x.IsInvalidated);
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

            invalidJControls = EmulationManager.joyControls.FindAll(x => x.IsInvalidated == false
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

        private static void PresetDataManager_PresetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (EmulationManager.joyControls == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var joyControl in EmulationManager.joyControls)
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
                foreach (var joyControl in EmulationManager.joyControls)
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