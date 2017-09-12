namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Exceptions.Emulation;
    using KeyboardSplitter.Exceptions.Gamepad;
    using KeyboardSplitter.Models;
    using SplitterCore.Emulation;
    using SplitterCore.Input;
    using XinputWrapper;

    public class EmulationManager : DependencyObject, IEmulationManager
    {
        public static readonly DependencyProperty SlotsProperty =
            DependencyProperty.Register(
            "Slots",
            typeof(ObservableCollection<IEmulationSlot>),
            typeof(EmulationManager),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsEmulationStartedProperty =
            DependencyProperty.Register(
            "IsEmulationStarted",
            typeof(bool),
            typeof(EmulationManager),
            new PropertyMetadata(false));

        private const int MaxSlotsCount = 4;

        private DateTime lastStartOrStopTime;

        public EmulationManager(ObservableCollection<IEmulationSlot> slots)
        {
            if (slots == null)
            {
                throw new ArgumentNullException("slots");
            }

            foreach (var slot in slots)
            {
                if (!(slot is UserControl))
                {
                    throw new EmulationSlotTypeException(
                        "The following slot must inherit from UserControl too: " + slot.SlotNumber);
                }

                slot.ResetRequested += this.OnSlotResetRequested;
            }

            this.Slots = slots;
        }

        public event EventHandler EmulationStarted;

        public event EventHandler EmulationStopped;

        public ObservableCollection<IEmulationSlot> Slots
        {
            get { return (ObservableCollection<IEmulationSlot>)this.GetValue(SlotsProperty); }
            set { this.SetValue(SlotsProperty, value); }
        }

        public bool IsEmulationStarted
        {
            get { return (bool)this.GetValue(IsEmulationStartedProperty); }
            set { this.SetValue(IsEmulationStartedProperty, value); }
        }

        public void Start()
        {
            if (this.lastStartOrStopTime == null)
            {
                this.lastStartOrStopTime = DateTime.Now;
            }
            else
            {
                if (DateTime.Now - this.lastStartOrStopTime < TimeSpan.FromSeconds(5))
                {
                    throw new KeyboardSplitterExceptionBase("You should wait at least 5 seconds, between each emulation start/stop!");
                }
            }

            this.lastStartOrStopTime = DateTime.Now;

            try
            {
                this.CheckForInvalidatedSlots();
                this.CheckXinputBus();
            }
            catch (Exception)
            {
                throw;
            }

            // Releasing xinput controller tags
            for (int i = 0; i < 4; i++)
            {
                XinputController.RetrieveController(i).Tag = null;
            }

            // Plugging in the virtual controllers
            foreach (var slot in this.Slots)
            {
                if (slot.Keyboard == null && slot.Mouse == null)
                {
                    slot.InvalidateReason = SlotInvalidationReason.No_Input_Device_Selected;
                    throw new SlotInvalidatedException(string.Format("Slot #{0} has no valid input device selected", slot.SlotNumber));
                }

                string errorMessage = string.Format("Plug in {0} failed!", slot.Gamepad.FriendlyName);
                bool success = false;
                try
                {
                    success = slot.Gamepad.PlugIn();
                }
                catch (VirtualBusNotInstalledException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.VirtualBus_Not_Installed;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }
                catch (XboxAccessoriesNotInstalledException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.Additional_Drivers_Not_Installed;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }
                catch (VirtualBusFullException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.VirtualBus_Full;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }
                catch (GamepadIsInUseException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.Controller_Already_Plugged_In;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }
                catch (GamepadOwnedException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.Controller_In_Use;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }
                catch (XinputSlotsFullException)
                {
                    slot.InvalidateReason = SlotInvalidationReason.XinputBus_Full;
                    errorMessage += " " + slot.InvalidateReason.ToString();
                    LogWriter.Write(errorMessage);
                    throw;
                }

                if (success)
                {
                    slot.Lock();
                    LogWriter.Write(string.Format("Plug in {0} - OK", slot.Gamepad.FriendlyName));
                    slot.Gamepad.Disconnected += this.OnGamepadDisconnected;
                }
                else
                {
                    slot.InvalidateReason = SlotInvalidationReason.Controller_Plug_In_Failed;
                    LogWriter.Write(string.Format("Plug in {0} - Failed!", slot.Gamepad.FriendlyName));

                    // Releasing xinput controller tags
                    for (int i = 0; i < 4; i++)
                    {
                        XinputController.RetrieveController(i).Tag = null;
                    }

                    return;
                }
            }

            LogWriter.Write(string.Format("Emulation started. Slots count: {0}", this.Slots.Count));

            this.IsEmulationStarted = true;

            if (this.EmulationStarted != null)
            {
                this.EmulationStarted(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            foreach (var slot in this.Slots)
            {
                slot.Gamepad.Disconnected -= this.OnGamepadDisconnected;
                if (slot.Gamepad.LedNumber != 0)
                {
                    XinputController.RetrieveController((int)slot.Gamepad.LedNumber - 1).Tag = null;
                    slot.Gamepad.LedNumber = 0;
                }

                slot.Unlock();
                if (slot.Gamepad.IsOwned)
                {
                    if (slot.Gamepad.Unplug(true))
                    {
                        LogWriter.Write(string.Format("Successfully unplugged {0}", slot.Gamepad.FriendlyName));
                    }
                    else
                    {
                        LogWriter.Write(string.Format("Unplug failed: {0}", slot.Gamepad.FriendlyName));
                    }
                }

                this.lastStartOrStopTime = DateTime.Now;
            }

            LogWriter.Write("Emulation stopped");

            this.IsEmulationStarted = false;
            if (this.EmulationStopped != null)
            {
                this.EmulationStopped(this, EventArgs.Empty);
            }
        }

        public void ChangeSlotsCountBy(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount > 0)
            {
                if (this.Slots.Count == EmulationManager.MaxSlotsCount)
                {
                    throw new NotSupportedException(
                        string.Format(
                        "You can not add more than {0} emulation slots!",
                        EmulationManager.MaxSlotsCount));
                }

                for (uint i = 0; i < amount; i++)
                {
                    for (uint j = 1; j <= 4; j++)
                    {
                        if (this.Slots.Select(x => x.SlotNumber).Contains(j))
                        {
                            continue;
                        }

                        var keyboard = Keyboard.None;
                        var mouse = Mouse.None;
                        var devices = InputManager.ConnectedInputDevices;
                        var splitter = Helpers.SplitterHelper.TryFindSplitter();

                        if (splitter != null && GlobalSettings.Instance.SuggestInputDevicesForNewSlots)
                        {
                            foreach (var device in devices)
                            {
                                if (device.IsKeyboard)
                                {
                                    if (keyboard == Keyboard.None)
                                    {
                                        var busyKeyboards = this.Slots.Select(slot => slot.Keyboard).Where(kb => kb != Keyboard.None);
                                        if (busyKeyboards.FirstOrDefault(x => x.Match(device)) == null)
                                        {
                                            var keyboardMatch = splitter.InputManager.Keyboards.FirstOrDefault(x => x.Match(device));
                                            if (keyboardMatch != null)
                                            {
                                                keyboard = keyboardMatch;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (mouse == Mouse.None)
                                    {
                                        var busyMice = this.Slots.Select(slot => slot.Mouse).Where(ms => ms != Mouse.None);
                                        if (busyMice.FirstOrDefault(x => x.Match(device)) == null)
                                        {
                                            var mouseMatch = splitter.InputManager.Mice.FirstOrDefault(x => x.Match(device));
                                            if (mouseMatch != null)
                                            {
                                                mouse = mouseMatch;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        uint userIndex = (uint)Math.Abs(j - GlobalSettings.Instance.StartingVirtualControllerUserIndex) + 1;
                        this.Slots.Add(new EmulationSlot(j, new XboxGamepad(userIndex), keyboard, mouse, Presets.Preset.Default));
                        break;
                    }
                }
            }
            else
            {
                if (this.Slots.Count == 1)
                {
                    throw new NotSupportedException("You can not remove all emulation slots! You should always keep at least one!");
                }

                for (int i = 0; i < -amount; i++)
                {
                    this.Slots.RemoveAt(this.Slots.Count - 1);
                }
            }
        }

        public void Destroy()
        {
            if (this.IsEmulationStarted)
            {
                this.Stop();
                this.Slots.Clear();
            }
        }

        private void CheckForInvalidatedSlots()
        {
            var invalidSlots = this.Slots.ToList().FindAll(x => x.InvalidateReason != SlotInvalidationReason.None);
            if (invalidSlots.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Due to the following slots are invalidated, emulation can not start:");

                foreach (var invalidJControl in invalidSlots)
                {
                    sb.AppendLine(string.Format("-Slot #{0} [{1}]", invalidJControl.SlotNumber, invalidJControl.InvalidateReason));
                }

                throw new SlotInvalidatedException(sb.ToString());
            }
        }

        private void CheckXinputBus()
        {
            if (this.Slots.Count > XinputController.EmptyBusSlotsCount)
            {
                int currentXinputControllersCount = 4 - XinputController.EmptyBusSlotsCount;
                string line1 = "Windows can not plug in more than 4 xinput controllers at a time!";

                string line2 = string.Format(
                    "Currently, there are {0} xinput controllers connected",
                    currentXinputControllersCount);
                if (currentXinputControllersCount == 1)
                {
                    line2 = "Currently, there is 1 xinput controller connected";
                }

                string line3 = string.Format("and you are trying to add {0} more controller", this.Slots.Count);
                if (currentXinputControllersCount > 1)
                {
                    line3 += "s";
                }

                string errorMessage = string.Format(
                    "{0}{1}{1}{2},{1}{3}!",
                    line1,
                    Environment.NewLine,
                    line2,
                    line3);

                throw new XinputSlotsFullException(errorMessage);
            }
        }

        private void OnGamepadDisconnected(object sender, EventArgs e)
        {
            var gamepad = sender as IVirtualGamepad;
            if (gamepad == null)
            {
                return;
            }

            var slot = this.Slots.First(x => x.Gamepad == gamepad);
            if (slot != null)
            {
                if (slot.InvalidateReason == SlotInvalidationReason.None)
                {
                    slot.InvalidateReason = SlotInvalidationReason.Controller_Unplugged;
                }
            }
        }

        private void OnSlotResetRequested(object sender, EventArgs e)
        {
            if (this.IsEmulationStarted)
            {
                return;
            }

            var slot = sender as IEmulationSlot;
            if (slot == null)
            {
                return;
            }

            var index = this.Slots.IndexOf(slot);
            if (index == -1)
            {
                return;
            }

            if (slot.InvalidateReason == SlotInvalidationReason.Controller_Unplugged ||
                slot.InvalidateReason == SlotInvalidationReason.Controller_Already_Plugged_In ||
                slot.InvalidateReason == SlotInvalidationReason.Controller_In_Use ||
                slot.InvalidateReason == SlotInvalidationReason.Controller_Plug_In_Failed)
            {
                if (slot.Gamepad.Exsists)
                {
                    if (!slot.Gamepad.Unplug(true))
                    {
                        return;
                    }
                }
            }

            var keyboard = slot.Keyboard;
            if (keyboard == null ||
                slot.InvalidateReason == SlotInvalidationReason.Keyboard_Unplugged ||
                slot.InvalidateReason == SlotInvalidationReason.No_Input_Device_Selected)
            {
                keyboard = Keyboard.None;
            }

            var mouse = slot.Mouse;
            if (mouse == null ||
                slot.InvalidateReason == SlotInvalidationReason.Mouse_Unplugged ||
                slot.InvalidateReason == SlotInvalidationReason.No_Input_Device_Selected)
            {
                mouse = Mouse.None;
            }

            this.Slots[index] = new EmulationSlot(
                slot.SlotNumber,
                slot.Gamepad as XboxGamepad,
                keyboard,
                mouse,
                slot.Preset as Presets.Preset);

            this.Slots[index].ResetRequested += this.OnSlotResetRequested;
        }
    }
}