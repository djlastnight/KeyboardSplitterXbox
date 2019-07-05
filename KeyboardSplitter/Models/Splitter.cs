namespace KeyboardSplitter.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Media;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.Resources;
    using SplitterCore;
    using SplitterCore.Emulation;
    using SplitterCore.Input;
    using SplitterCore.Preset;

    using VirtualXbox.Enums;
    using XinputWrapper.Enums;

    public class Splitter : SplitterBase
    {
        public Splitter(int slotsCount)
            : base()
        {
            LogWriter.Write(string.Format("Creating splitter with {0} slots ", slotsCount));
            this.InputManager = new InputManager();
            this.InputManager.EmergencyStop += this.InputManager_EmergencyStop;
            this.InputManager.EmergencyLeft += this.InputManager_EmergencyLeft;
            this.InputManager.EmergencyRight += this.InputManager_EmergencyRight;
            this.InputManager.InputActivity += this.InputManager_InputActivity;
            this.InputManager.InputDeviceChanged += this.InputManager_InputDeviceChanged;

            var slots = new ObservableCollection<IEmulationSlot>();
            for (int i = 1; i <= slotsCount; i++)
            {
                Keyboard keyboard = Keyboard.None;
                Mouse mouse = Mouse.None;
                if (GlobalSettings.Instance.SuggestInputDevicesForNewSlots)
                {
                    if (i < this.InputManager.Keyboards.Count)
                    {
                        keyboard = this.InputManager.Keyboards[i];
                    }

                    if (i < this.InputManager.Mice.Count)
                    {
                        mouse = this.InputManager.Mice[i];
                    }
                }

                var additiveUserIndex = GlobalSettings.Instance.StartingVirtualControllerUserIndex - 1;
                uint userIndex = (uint)((i + additiveUserIndex - 1) % 4) + 1;
                var gamepad = new XboxGamepad(userIndex);
                var slot = new EmulationSlot((uint)i, gamepad, keyboard, mouse, Preset.Default);
                slots.Add(slot);
            }

            this.EmulationManager = new EmulationManager(slots);
            this.EmulationManager.EmulationStarted += this.EmulationManager_EmulationStarted;
            this.EmulationManager.EmulationStopped += this.EmulationManager_EmulationStopped;
        }

        public Splitter(IInputManager inputManager, IEmulationManager emulationManager)
            : base(inputManager, emulationManager)
        {
        }

        private void TranslateInput(InputEventArgs e, IEmulationSlot slot, FunctionType functionType)
        {
            if (!slot.Gamepad.IsOwned)
            {
                LogWriter.Write("owned");
                return;
            }

            var mappings = this.GetMappings(e.Key, slot, functionType);

            foreach (var mapping in mappings)
            {
                if (!e.IsDown && !this.AreAllKeysUp(e.InputDevice, mapping.Keys))
                {
                    // Not all mapped to the function keys are currently released, so leaving it active
                    continue;
                }

                string functionName;
                switch (functionType)
                {
                    case FunctionType.Button:
                        functionName = this.SetButton(slot.Gamepad, mapping, e.IsDown);
                        break;
                    case FunctionType.Axis:
                        functionName = this.SetAxis(slot.Gamepad, mapping, e.IsDown, this.HasOppositeAxisKeysDown(slot, mapping, e.InputDevice));
                        break;
                    case FunctionType.Dpad:
                        functionName = this.SetDpad(slot.Gamepad, mapping, e.IsDown);
                        break;
                    case FunctionType.Trigger:
                        functionName = this.SetTrigger(slot.Gamepad, mapping, e.IsDown);
                        break;
                    default:
                        throw new NotImplementedException("Not implemented function type: " + functionType);
                }

#if DEBUG
                if (functionName != null)
                {
                    bool suppress = (e.InputDevice.IsKeyboard && this.ShouldBlockKeyboards) || (!e.InputDevice.IsKeyboard && this.ShouldBlockMice);
                    LogWriter.Write(string.Format(
                        "Translate {0}{1} {2} ({3}) --> Gamepad #{4} {5} ||| Slot #{6} ||| Preset '{7}'",
                        suppress ? "(suppress) " : string.Empty,
                        e.InputDevice.StrongName,
                        e.Key,
                        e.IsDown ? "\u2193" : "\u2191",
                        slot.Gamepad.UserIndex,
                        functionName,
                        slot.SlotNumber,
                        slot.Preset.Name));
                }
#endif
            }
        }

        private string SetButton(IVirtualGamepad gamepad, Mapping mapping, bool isKeyDown)
        {
            uint button = (uint)mapping.Function;
            bool currentValue = gamepad.GetButtonState(button);
            bool newValue = isKeyDown;

            if (currentValue == newValue)
            {
                return null;
            }

            gamepad.SetButtonState(button, newValue);
            return ((XinputButton)button).ToString();
        }

        private string SetTrigger(IVirtualGamepad gamepad, Mapping mapping, bool isKeyDown)
        {
            uint trigger = (uint)mapping.Function;
            byte currentValue = gamepad.GetTriggerState(trigger);
            byte newValue = isKeyDown ? byte.MaxValue : byte.MinValue;

            if (currentValue == newValue)
            {
                return null;
            }

            gamepad.SetTriggerState(trigger, newValue);
            return ((XinputTrigger)newValue).ToString();
        }

        private string SetAxis(IVirtualGamepad gamepad, Mapping mapping, bool isKeyDown, bool hasOppositeDown)
        {
            uint axis = (uint)mapping.Function;
            short axisValue = (short)mapping.TargetValue;
            short oldValue = gamepad.GetAxisState(axis);
            short newValue = isKeyDown ? axisValue : (short)XboxAxisPosition.Center;

            if (hasOppositeDown && !isKeyDown)
            {
                if ((short)mapping.TargetValue == (short)XboxAxisPosition.Min)
                {
                    newValue = (short)XboxAxisPosition.Max;
                }
                else
                {
                    newValue = (short)XboxAxisPosition.Min;
                }
            }

            if (oldValue == newValue)
            {
                return null;
            }

            gamepad.SetAxisState(axis, newValue);
            return ((XinputAxis)axis) + " Axis " + ((XboxAxisPosition)newValue);
        }

        private string SetDpad(IVirtualGamepad gamepad, Mapping mapping, bool isKeyDown)
        {
            int direction = (int)mapping.Function;
            int oldValue = gamepad.GetDpadState();
            int newValue = isKeyDown ? oldValue | direction : oldValue & ~direction;
            if (oldValue == newValue)
            {
                return null;
            }

            gamepad.SetDpadState(newValue);
            return ((XinputButton)newValue).ToString();
        }

        private bool HasOppositeAxisKeysDown(IEmulationSlot slot, Mapping mapping, InputDevice inputDevice)
        {
            uint axis = (uint)mapping.Function;
            short value = (short)mapping.TargetValue;

            // Reversing the axis position
            if (value == (short)XboxAxisPosition.Min)
            {
                value = (short)XboxAxisPosition.Max;
            }
            else
            {
                value = (short)XboxAxisPosition.Min;
            }

            var keys = slot.Preset.GetKeys(new PresetAxis(axis, value, InputKey.None));
            var hasOppositeKeyDown = !this.AreAllKeysUp(inputDevice, keys);

            return hasOppositeKeyDown;
        }

        private List<Mapping> GetMappings(InputKey inputKey, IEmulationSlot slot, FunctionType functionType)
        {
            var filtered = slot.Preset.FilterByKey(inputKey);
            var mappings = new List<Mapping>();

            switch (functionType)
            {
                case FunctionType.Button:
                    {
                        // Normal buttons
                        foreach (var presetButton in filtered.Buttons)
                        {
                            var button = presetButton.Button;
                            var keys = slot.Preset.GetKeys(presetButton);
                            mappings.Add(new Mapping(button, keys));
                        }

                        // Custom buttons
                        foreach (var customFunction in filtered.CustomFunctions)
                        {
                            var xboxFunction = (XboxCustomFunction)customFunction.Function;
                            var funcType = Helpers.CustomFunctionHelper.GetFunctionType(xboxFunction);
                            if (funcType == functionType)
                            {
                                var button = (uint)Helpers.CustomFunctionHelper.GetXboxButton(xboxFunction);
                                var keys = slot.Preset.GetKeys(customFunction);
                                mappings.Add(new Mapping(button, keys));
                            }
                        }
                    }

                    break;
                case FunctionType.Axis:
                    {
                        // Normal axes
                        foreach (var presetAxis in filtered.Axes)
                        {
                            mappings.Add(new Mapping(presetAxis.Axis, slot.Preset.GetKeys(presetAxis), presetAxis.Value));
                        }

                        // Custom axes
                        foreach (var presetCustom in filtered.CustomFunctions)
                        {
                            var xboxCustomFunction = (XboxCustomFunction)presetCustom.Function;
                            var funcType = Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomFunction);
                            if (funcType == functionType)
                            {
                                XboxAxisPosition pos;
                                var axis = Helpers.CustomFunctionHelper.GetXboxAxis(xboxCustomFunction, out pos);
                                mappings.Add(new Mapping((uint)axis, slot.Preset.GetKeys(presetCustom), pos));
                            }
                        }
                    }

                    break;
                case FunctionType.Dpad:
                    // Normal dpad directions
                    foreach (var presetDpad in filtered.Dpads)
                    {
                        mappings.Add(new Mapping(presetDpad.Direction, slot.Preset.GetKeys(presetDpad)));
                    }

                    // Custom dpad directions
                    foreach (var customDpad in filtered.CustomFunctions)
                    {
                        var xboxFunction = (XboxCustomFunction)customDpad.Function;
                        var funcType = Helpers.CustomFunctionHelper.GetFunctionType(xboxFunction);
                        if (funcType == functionType)
                        {
                            var direction = Helpers.CustomFunctionHelper.GetDpadDirection(xboxFunction);
                            var keys = slot.Preset.GetKeys(customDpad);
                            mappings.Add(new Mapping(direction, keys));
                        }
                    }

                    break;
                case FunctionType.Trigger:
                    {
                        // Normal triggers
                        foreach (var presetTrigger in filtered.Triggers)
                        {
                            var trigger = presetTrigger.Trigger;
                            var keys = slot.Preset.GetKeys(presetTrigger);
                            mappings.Add(new Mapping(trigger, keys));
                        }

                        // Custom triggers
                        foreach (var customFunction in filtered.CustomFunctions)
                        {
                            var xboxFunction = (XboxCustomFunction)customFunction.Function;
                            var funcType = Helpers.CustomFunctionHelper.GetFunctionType(xboxFunction);
                            if (funcType == functionType)
                            {
                                var trigger = (uint)Helpers.CustomFunctionHelper.GetXboxTrigger(xboxFunction);
                                var keys = slot.Preset.GetKeys(customFunction);
                                mappings.Add(new Mapping(trigger, keys));
                            }
                        }
                    }

                    break;
                default:
                    break;
            }

            return mappings;
        }

        private bool AreAllKeysUp(InputDevice device, List<InputKey> keys)
        {
            return Enumerable.All(keys, key => this.InputManager.IsKeyDown(device, key) == false);
        }

        private void InputManager_EmergencyStop(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.EmulationManager.Stop();
            });
        }

        private void InputManager_EmergencyLeft(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                if (this.ShouldBlockKeyboards)
                {
                    using (var player = new SoundPlayer(Sounds.Disconnected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockKeyboards = false;

                    LogWriter.Write("Emergency activated: Block choosen keyboards unchecked");
                }
                else
                {
                    using (var player = new SoundPlayer(Sounds.Connected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockKeyboards = true;

                    LogWriter.Write("Emergency activated: Block choosen keyboards checked");
                }
            });
        }

        private void InputManager_EmergencyRight(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                if (this.ShouldBlockMice)
                {
                    using (var player = new SoundPlayer(Sounds.Disconnected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockMice = false;

                    LogWriter.Write("Emergency activated: Block choosen mice unchecked");
                }
                else
                {
                    using (var player = new SoundPlayer(Sounds.Connected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockMice = true;

                    LogWriter.Write("Emergency activated: Block choosen mice checked");
                }
            });
        }

        private void InputManager_InputActivity(object sender, InputEventArgs e)
        {
            if (!this.EmulationManager.IsEmulationStarted)
            {
                return;
            }

            if (this.AssignedInputDevices.Contains(e.InputDevice))
            {
                e.Handled = e.InputDevice.IsKeyboard ? this.ShouldBlockKeyboards : this.ShouldBlockMice;
            }

            foreach (var slot in this.EmulationManager.Slots)
            {
                if (slot.Keyboard != e.InputDevice && slot.Mouse != e.InputDevice)
                {
                    continue;
                }

                this.TranslateInput(e, slot, FunctionType.Button);
                this.TranslateInput(e, slot, FunctionType.Trigger);
                this.TranslateInput(e, slot, FunctionType.Axis);
                this.TranslateInput(e, slot, FunctionType.Dpad);
            }
        }

        private void InputManager_InputDeviceChanged(object sender, InputDeviceChangedEventArgs e)
        {
            if (!e.IsRemoved)
            {
                return;
            }

            this.Dispatcher.BeginInvoke((Action)delegate
            {
                foreach (var slot in this.EmulationManager.Slots)
                {
                    if (slot.Keyboard == null || slot.Keyboard == e.ChangedDevice)
                    {
                        slot.InvalidateReason = SlotInvalidationReason.Keyboard_Unplugged;
                        slot.Gamepad.Unplug(false);
                    }

                    if (slot.Mouse == null || slot.Mouse == e.ChangedDevice)
                    {
                        slot.InvalidateReason = SlotInvalidationReason.Mouse_Unplugged;
                        slot.Gamepad.Unplug(false);
                    }
                }
            });
        }

        private void EmulationManager_EmulationStarted(object sender, EventArgs e)
        {
            foreach (var slot in this.EmulationManager.Slots)
            {
                if (slot.Keyboard != null && !this.AssignedInputDevices.Contains(slot.Keyboard))
                {
                    this.AssignedInputDevices.Add(slot.Keyboard);
                }

                if (slot.Mouse != null && !this.AssignedInputDevices.Contains(slot.Mouse))
                {
                    this.AssignedInputDevices.Add(slot.Mouse);
                }
            }
        }

        private void EmulationManager_EmulationStopped(object sender, EventArgs e)
        {
            this.AssignedInputDevices.Clear();
        }
    }
}
