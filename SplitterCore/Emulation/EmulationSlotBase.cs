namespace SplitterCore.Emulation
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SplitterCore.Input;
    using SplitterCore.Preset;

    public class EmulationSlotBase : UserControl, IEmulationSlot
    {
        public static readonly DependencyProperty SlotNumberProperty =
            DependencyProperty.Register(
            "SlotNumber",
            typeof(uint),
            typeof(EmulationSlotBase),
            new PropertyMetadata((uint)0));

        public static readonly DependencyProperty GamepadProperty =
            DependencyProperty.Register(
            "Gamepad",
            typeof(IVirtualGamepad),
            typeof(EmulationSlotBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty KeyboardProperty =
            DependencyProperty.Register(
            "Keyboard",
            typeof(Keyboard),
            typeof(EmulationSlotBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MouseProperty =
            DependencyProperty.Register(
            "Mouse",
            typeof(Mouse),
            typeof(EmulationSlotBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PresetProperty =
            DependencyProperty.Register(
            "Preset",
            typeof(IPreset),
            typeof(EmulationSlotBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty InvalidateReasonProperty =
            DependencyProperty.Register(
            "InvalidateReason",
            typeof(SlotInvalidationReason),
            typeof(EmulationSlotBase),
            new PropertyMetadata(SlotInvalidationReason.None));

        private SlotInvalidationReason lastInvalidateReason;

        public EmulationSlotBase()
        {
            this.Content = "Please write new ControlTemplate for " + this.GetType().Name;
            this.lastInvalidateReason = SlotInvalidationReason.None;
        }

        protected EmulationSlotBase(uint slotNumber, IVirtualGamepad gamepad, Keyboard keyboard, Mouse mouse, IPreset preset)
            : this()
        {
            if (gamepad == null)
            {
                throw new ArgumentNullException("gamepad");
            }

            if (preset == null)
            {
                throw new ArgumentNullException("preset");
            }

            if (keyboard == null)
            {
                throw new ArgumentNullException("keyboard");
            }

            if (mouse == null)
            {
                throw new ArgumentNullException("mouse");
            }

            this.SlotNumber = slotNumber;
            this.Gamepad = gamepad;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.Preset = preset;
        }

        public event EventHandler ResetRequested;

        public uint SlotNumber
        {
            get { return (uint)this.GetValue(SlotNumberProperty); }
            set { this.SetValue(SlotNumberProperty, value); }
        }

        public IVirtualGamepad Gamepad
        {
            get { return (IVirtualGamepad)this.GetValue(GamepadProperty); }
            set { this.SetValue(GamepadProperty, value); }
        }

        public Keyboard Keyboard
        {
            get { return (Keyboard)this.GetValue(KeyboardProperty); }
            set { this.SetValue(KeyboardProperty, value); }
        }

        public Mouse Mouse
        {
            get { return (Mouse)this.GetValue(MouseProperty); }
            set { this.SetValue(MouseProperty, value); }
        }

        public IPreset Preset
        {
            get { return (IPreset)this.GetValue(PresetProperty); }
            set { this.SetValue(PresetProperty, value); }
        }

        public SlotInvalidationReason InvalidateReason
        {
            get
            {
                return (SlotInvalidationReason)this.GetValue(InvalidateReasonProperty);
            }

            set
            {
                this.SetValue(InvalidateReasonProperty, value);
                if (this.lastInvalidateReason != value)
                {
                    this.OnSlotInvalidateChanged();
                    this.lastInvalidateReason = value;
                }
            }
        }

        public virtual void Lock()
        {
            this.IsEnabled = false;
        }

        public virtual void Unlock()
        {
            this.IsEnabled = true;
        }

        public virtual void OnSlotInvalidateChanged()
        {
        }

        protected virtual void OnResetRequested()
        {
            if (this.ResetRequested != null)
            {
                this.ResetRequested(this, EventArgs.Empty);
            }
        }
    }
}
