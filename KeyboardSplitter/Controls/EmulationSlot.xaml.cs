namespace KeyboardSplitter.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using KeyboardSplitter.Commands;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Models;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.UI;
    using SplitterCore.Emulation;
    using SplitterCore.Input;

    /// <summary>
    /// Interaction logic for EmulationSlot.xaml
    /// </summary>
    public partial class EmulationSlot : EmulationSlotBase
    {
        public static readonly DependencyProperty IsOnScreenControllerActiveProperty =
            DependencyProperty.Register(
            "IsOnScreenControllerActive",
            typeof(bool),
            typeof(EmulationSlot),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register(
            "IsLocked",
            typeof(bool),
            typeof(EmulationSlot),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsInvalidatedProperty =
            DependencyProperty.Register(
            "IsInvalidated",
            typeof(bool),
            typeof(EmulationSlot),
            new PropertyMetadata(false));

        private System.Windows.Input.ICommand detectKeyboardCommand;

        private System.Windows.Input.ICommand detectMouseCommand;

        private System.Windows.Input.ICommand resetSlotCommand;

        public EmulationSlot(uint slotNumber, XboxGamepad gamepad, Keyboard keyboard, Mouse mouse, Preset preset)
            : base(slotNumber, gamepad, keyboard, mouse, preset)
        {
            this.InitializeComponent();
            if (gamepad.Exsists)
            {
                this.InvalidateReason = SlotInvalidationReason.Controller_Already_Plugged_In;
            }
        }

        public bool IsOnScreenControllerActive
        {
            get { return (bool)this.GetValue(IsOnScreenControllerActiveProperty); }
            set { this.SetValue(IsOnScreenControllerActiveProperty, value); }
        }

        public bool IsLocked
        {
            get { return (bool)this.GetValue(IsLockedProperty); }
            set { this.SetValue(IsLockedProperty, value); }
        }

        public bool IsInvalidated
        {
            get { return (bool)this.GetValue(IsInvalidatedProperty); }
            set { this.SetValue(IsInvalidatedProperty, value); }
        }

        public System.Windows.Input.ICommand DetectKeyboardCommand
        {
            get
            {
                if (this.detectKeyboardCommand == null)
                {
                    this.detectKeyboardCommand = new RelayCommand(this.OnDetectKeyboardRequested);
                }

                return this.detectKeyboardCommand;
            }
        }

        public System.Windows.Input.ICommand DetectMouseCommand
        {
            get
            {
                if (this.detectMouseCommand == null)
                {
                    this.detectMouseCommand = new RelayCommand(this.OnDetectMouseRequested);
                }

                return this.detectMouseCommand;
            }
        }

        public System.Windows.Input.ICommand ResetSlotCommand
        {
            get
            {
                if (this.resetSlotCommand == null)
                {
                    this.resetSlotCommand = new RelayCommand(this.OnResetSlotRequested);
                }

                return this.resetSlotCommand;
            }
        }

        public override void Lock()
        {
            this.IsLocked = true;
        }

        public override void Unlock()
        {
            this.IsLocked = false;
        }

        public override void OnSlotInvalidateChanged()
        {
            this.IsInvalidated = this.InvalidateReason != SlotInvalidationReason.None;
            base.OnSlotInvalidateChanged();
        }

        private void OnDetectKeyboardRequested(object parameter)
        {
            var splitter = Helpers.SplitterHelper.TryFindSplitter();
            if (splitter == null || splitter.InputManager == null)
            {
                return;
            }

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            if (mainWindow.Splitter == null || mainWindow.Splitter.InputManager == null)
            {
                return;
            }

            var detector = new InputDetectorWindow(mainWindow.Splitter.InputManager, InputDetectorDeviceFilter.KeyboardOnly);
            detector.Owner = mainWindow;
            detector.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detector.InputDetected += (oo, ss) =>
            {
                if (ss.InputDevice.IsKeyboard)
                {
                    this.Keyboard = ss.InputDevice as Keyboard;
                    detector.Close();
                }
            };

            detector.ShowDialog();
        }

        private void OnDetectMouseRequested(object parameter)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            if (mainWindow.Splitter == null || mainWindow.Splitter.InputManager == null)
            {
                return;
            }

            var detector = new InputDetectorWindow(mainWindow.Splitter.InputManager, InputDetectorDeviceFilter.MouseOnly);
            detector.Owner = mainWindow;
            detector.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detector.InputDetected += (oo, ss) =>
            {
                if (!ss.InputDevice.IsKeyboard)
                {
                    this.Mouse = ss.InputDevice as Mouse;
                    detector.Close();
                }
            };

            detector.ShowDialog();
        }

        private void OnResetSlotRequested(object parameter)
        {
            this.OnResetRequested();
        }

        private void KeyboardOrMouseChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IsOnScreenControllerActive = this.Keyboard == Keyboard.None && this.Mouse == Mouse.None;
        }
    }
}