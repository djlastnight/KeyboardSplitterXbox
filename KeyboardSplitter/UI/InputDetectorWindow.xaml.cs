namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using KeyboardSplitter.Converters;
    using KeyboardSplitter.Enums;
    using SplitterCore.Emulation;
    using SplitterCore.Input;
    using SplitterCore.Preset;

    /// <summary>
    /// Interaction logic for InputDetector.xaml
    /// </summary>
    public partial class InputDetectorWindow : CustomWindow
    {
        public static readonly DependencyProperty InputDetectorTargetProperty =
            DependencyProperty.Register(
            "InputDetectorTarget",
            typeof(InputDetectorTarget),
            typeof(InputDetectorWindow),
            new PropertyMetadata(InputDetectorTarget.Device));

        public static readonly DependencyProperty DeviceFilterProperty =
            DependencyProperty.Register(
            "DeviceFilter",
            typeof(InputDetectorDeviceFilter),
            typeof(InputDetectorWindow),
            new PropertyMetadata(InputDetectorDeviceFilter.KeyboardAndMouse));

        public static readonly DependencyProperty FunctionNameProperty =
            DependencyProperty.Register(
            "FunctionName",
            typeof(string),
            typeof(InputDetectorWindow),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty GamepadNameProperty =
            DependencyProperty.Register(
            "GamepadName",
            typeof(string),
            typeof(InputDetectorWindow),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty WarningTextProperty =
            DependencyProperty.Register(
            "WarningText",
            typeof(string),
            typeof(InputDetectorWindow),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty EnableMouseMoveDetectionProperty =
            DependencyProperty.Register(
            "EnableMouseMoveDetection",
            typeof(bool),
            typeof(InputDetectorWindow),
            new PropertyMetadata(false));

        private IInputManager inputManager;

        private Keyboard keyboard;

        private Mouse mouse;

        private bool isMouseHover;

        public InputDetectorWindow(IInputManager inputManager, InputDetectorDeviceFilter filter)
        {
            this.InitializeComponent();
            if (inputManager == null)
            {
                throw new ArgumentNullException("inputManager");
            }

            this.inputManager = inputManager;
            this.inputManager.InputActivity += this.OnInputActivity;
            this.DeviceFilter = filter;
            this.InputDetectorTarget = InputDetectorTarget.Device;
            switch (this.DeviceFilter)
            {
                case InputDetectorDeviceFilter.KeyboardOnly:
                    this.Title = "Keyboard detector";
                    break;
                case InputDetectorDeviceFilter.MouseOnly:
                    this.Title = "Mouse detector";
                    break;
                case InputDetectorDeviceFilter.KeyboardAndMouse:
                    this.Title = "Error: Wrong device filter passed!";
                    break;
                default:
                    this.Title = "Not implemented device filter passed!";
                    break;
            }
        }

        public InputDetectorWindow(IInputManager inputManager, InputDetectorDeviceFilter filter, IPresetElement presetElement, IEmulationSlot slot)
            : this(inputManager, filter)
        {
            this.InputDetectorTarget = InputDetectorTarget.Key;
            this.FunctionName = this.RetrieveFunctionName(presetElement);
            this.GamepadName = "on " + slot.Gamepad.FriendlyName;
            this.keyboard = slot.Keyboard;
            this.mouse = slot.Mouse;
            this.Title = "Waiting for ";
            switch (this.DeviceFilter)
            {
                case InputDetectorDeviceFilter.KeyboardOnly:
                    this.Title += this.keyboard.StrongName;
                    break;
                case InputDetectorDeviceFilter.MouseOnly:
                    this.Title += this.mouse.StrongName;
                    break;
                case InputDetectorDeviceFilter.KeyboardAndMouse:
                    this.Title += this.keyboard.StrongName + " and " + this.mouse.StrongName;
                    break;
                default:
                    break;
            }

            this.Title += " input...";
        }

        public event EventHandler<InputEventArgs> InputDetected;

        public InputDetectorTarget InputDetectorTarget
        {
            get { return (InputDetectorTarget)this.GetValue(InputDetectorTargetProperty); }
            set { this.SetValue(InputDetectorTargetProperty, value); }
        }

        public InputDetectorDeviceFilter DeviceFilter
        {
            get { return (InputDetectorDeviceFilter)this.GetValue(DeviceFilterProperty); }
            set { this.SetValue(DeviceFilterProperty, value); }
        }

        public string FunctionName
        {
            get { return (string)this.GetValue(FunctionNameProperty); }
            set { this.SetValue(FunctionNameProperty, value); }
        }

        public string GamepadName
        {
            get { return (string)this.GetValue(GamepadNameProperty); }
            set { this.SetValue(GamepadNameProperty, value); }
        }

        public string WarningText
        {
            get { return (string)this.GetValue(WarningTextProperty); }
            set { this.SetValue(WarningTextProperty, value); }
        }

        public bool EnableMouseMoveDetection
        {
            get { return (bool)this.GetValue(EnableMouseMoveDetectionProperty); }
            set { this.SetValue(EnableMouseMoveDetectionProperty, value); }
        }

        private string RetrieveFunctionName(IPresetElement presetElement)
        {
            var targetType = typeof(string);
            switch (presetElement.FunctionType)
            {
                case SplitterCore.FunctionType.Button:
                    {
                        var button = presetElement as PresetButton;
                        return new XboxButtonConverter().Convert(button.Button, targetType, null, null).ToString();
                    }

                case SplitterCore.FunctionType.Trigger:
                    {
                        var trigger = presetElement as PresetTrigger;
                        return new XboxTriggerConverter().Convert(trigger.Trigger, targetType, null, null).ToString();
                    }

                case SplitterCore.FunctionType.Axis:
                    {
                        var axis = presetElement as PresetAxis;
                        var axisConverter = new XboxAxisConverter();
                        var xboxAxisName = axisConverter.Convert(axis.Axis, targetType, null, null);
                        var pos = axisConverter.Convert(axis.Value, targetType, "value", null);
                        return string.Format("{0} {1}", xboxAxisName, pos);
                    }

                case SplitterCore.FunctionType.Dpad:
                    {
                        var dpad = presetElement as PresetDpad;
                        return new XboxDpadConverter().Convert(dpad.Direction, targetType, null, null).ToString();
                    }

                case SplitterCore.FunctionType.Custom:
                    {
                        var custom = presetElement as PresetCustom;
                        return new XboxCustomFunctionConverter().Convert(custom.Function, targetType, null, null).ToString();
                    }

                default:
                    throw new NotImplementedException("Not implemented function type: " + presetElement.FunctionType);
            }
        }

        private void OnInputActivity(object sender, InputEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.WarningText = string.Empty;

                bool isKeyboard = e.InputDevice.IsKeyboard;
                bool isMouse = !e.InputDevice.IsKeyboard;

                if (isKeyboard && this.DeviceFilter == InputDetectorDeviceFilter.MouseOnly)
                {
                    return;
                }

                if (isMouse && this.DeviceFilter == InputDetectorDeviceFilter.KeyboardOnly)
                {
                    return;
                }

                if (isMouse && !this.isMouseHover)
                {
                    // We detected mouse event, which is handled outside the mouse square zone
                    return;
                }

                if (isMouse && Interceptor.KeysHelper.IsMouseMoveKey(Helpers.InputHelper.ToInterceptionKey(e.Key)) && !this.EnableMouseMoveDetection)
                {
                    // Ignoring mouse move event
                    return;
                }

                if (this.InputDetectorTarget == InputDetectorTarget.Key)
                {
                    if (isKeyboard && !this.keyboard.Match(e.InputDevice))
                    {
                        this.WarningText = string.Format("Different keyboard detected: " + e.InputDevice.StrongName);
                        return;
                    }

                    if (isMouse && !this.mouse.Match(e.InputDevice))
                    {
                        this.WarningText = string.Format("Different mouse detected: " + e.InputDevice.StrongName);
                        return;
                    }
                }

                e.Handled = true;
                if (this.InputDetected != null)
                {
                    this.InputDetected(this, e);
                }
            });
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.InputDetected = null;
            if (this.inputManager != null)
            {
                this.inputManager.InputActivity -= this.OnInputActivity;
                this.inputManager = null;
            }
        }

        private void OnMouseIconMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.isMouseHover = true;
        }

        private void OnMouseIconMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.isMouseHover = false;
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}