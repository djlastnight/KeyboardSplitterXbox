namespace KeyboardSplitter.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Interceptor;
    using KeyboardSplitter.Detectors;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.UI;
    using XboxInterfaceWrap;

    /// <summary>
    /// User control, which interacts with a
    /// single xbox function.
    /// </summary>
    public partial class KeyControl : UserControl, IDisposable
    {
        private readonly JoyControl parent;

        private readonly bool isRemoveable;

        private XboxButton button;

        private XboxAxis axis;

        private XboxAxisPosition axisPosition;

        private XboxTrigger trigger;

        private XboxDpadDirection dpadDirection;

        private KeyControlType controlType;

        private XboxCustomFunction customFunction;

        public KeyControl(JoyControl parent, bool isRemoveable = false)
            : this()
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            this.parent = parent;
            this.isRemoveable = isRemoveable;
            this.comboBox1.ItemsSource = Enum.GetNames(typeof(InterceptionKeys));

            if (isRemoveable)
            {
                this.label1.Visibility = Visibility.Hidden;
                this.removeButton.Visibility = Visibility.Visible;
                this.xboxFunctionBox.Visibility = Visibility.Visible;
                this.xboxFunctionBox.ItemsSource = Enum.GetValues(typeof(XboxCustomFunction));
                this.xboxFunctionBox.SelectedIndex = 0;
                this.comboBox1.SelectedIndex = 0;
            }
        }

        public KeyControl(JoyControl parent, XboxButton button, bool removeable = false)
            : this(parent, removeable)
        {
            this.label1.Content = button;
            this.button = button;
            this.controlType = KeyControlType.Button;
        }

        public KeyControl(JoyControl parent, XboxAxis axis, XboxAxisPosition position, bool removeable = false)
            : this(parent, removeable)
        {
            this.label1.Content = axis + " " + position;
            this.axis = axis;
            this.axisPosition = position;
            this.controlType = KeyControlType.Axis;
        }

        public KeyControl(JoyControl parent, XboxDpadDirection direction, bool removeable = false)
            : this(parent, removeable)
        {
            this.label1.Content = "Dpad" + " " + direction;
            this.dpadDirection = direction;
            this.controlType = KeyControlType.Dpad;
        }

        public KeyControl(JoyControl parent, XboxTrigger trigger, bool removeable = false)
            : this(parent, removeable)
        {
            this.label1.Content = trigger;
            this.controlType = KeyControlType.Trigger;
            this.trigger = trigger;
        }

        private KeyControl()
        {
            this.InitializeComponent();
        }

        public event KeyGestureChangedHandler KeyGestureChanged;

        public event EventHandler Removed;

        public event EventHandler FunctionChanged;

        public JoyControl JoyParent
        {
            get
            {
                return this.parent;
            }
        }

        public string KeyGesture
        {
            get
            {
                if (this.comboBox1.SelectedItem == null)
                {
                    return "None";
                }

                return this.comboBox1.SelectedItem.ToString();
            }

            set
            {
                this.comboBox1.SelectedItem = value;
            }
        }

        public KeyControlType ControlType
        {
            get
            {
                return this.controlType;
            }
        }

        public XboxAxis Axis
        {
            get
            {
                return this.axis;
            }
        }

        public XboxButton Button
        {
            get
            {
                return this.button;
            }
        }

        public XboxAxisPosition Position
        {
            get
            {
                return this.axisPosition;
            }
        }

        public XboxDpadDirection DpadDirection
        {
            get
            {
                return this.dpadDirection;
            }
        }

        public XboxTrigger Trigger
        {
            get
            {
                return this.trigger;
            }
        }

        public bool IsRemoveable
        {
            get
            {
                return this.isRemoveable;
            }
        }

        public XboxCustomFunction CustomFunction
        {
            get
            {
                return this.customFunction;
            }

            set
            {
                this.customFunction = value;
                this.xboxFunctionBox.SelectedItem = value;
                switch (value)
                {
                    case XboxCustomFunction.Guide:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.Guide;
                        }

                        break;
                    case XboxCustomFunction.A:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.A;
                        }

                        break;
                    case XboxCustomFunction.B:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.B;
                        }

                        break;
                    case XboxCustomFunction.X:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.X;
                        }

                        break;
                    case XboxCustomFunction.Y:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.Y;
                        }

                        break;
                    case XboxCustomFunction.LeftBumper:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.LeftBumper;
                        }

                        break;
                    case XboxCustomFunction.RightBumper:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.RightBumper;
                        }

                        break;
                    case XboxCustomFunction.Back:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.Back;
                        }

                        break;
                    case XboxCustomFunction.Start:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.Start;
                        }

                        break;
                    case XboxCustomFunction.LeftThumb:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.LeftThumb;
                        }

                        break;
                    case XboxCustomFunction.RightThumb:
                        {
                            this.controlType = KeyControlType.Button;
                            this.button = XboxButton.RightThumb;
                        }

                        break;
                    case XboxCustomFunction.LeftTrigger:
                        {
                            this.controlType = KeyControlType.Trigger;
                            this.trigger = XboxTrigger.LeftTrigger;
                        }

                        break;
                    case XboxCustomFunction.RightTrigger:
                        {
                            this.controlType = KeyControlType.Trigger;
                            this.trigger = XboxTrigger.RightTrigger;
                        }

                        break;
                    case XboxCustomFunction.Dpad_Up:
                        {
                            this.controlType = KeyControlType.Dpad;
                            this.dpadDirection = XboxDpadDirection.Up;
                        }

                        break;
                    case XboxCustomFunction.Dpad_Down:
                        {
                            this.controlType = KeyControlType.Dpad;
                            this.dpadDirection = XboxDpadDirection.Down;
                        }

                        break;
                    case XboxCustomFunction.Dpad_Left:
                        {
                            this.controlType = KeyControlType.Dpad;
                            this.dpadDirection = XboxDpadDirection.Left;
                        }

                        break;
                    case XboxCustomFunction.Dpad_Right:
                        {
                            this.controlType = KeyControlType.Dpad;
                            this.dpadDirection = XboxDpadDirection.Right;
                        }

                        break;
                    case XboxCustomFunction.X_Min:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.X;
                            this.axisPosition = XboxAxisPosition.Min;
                        }

                        break;
                    case XboxCustomFunction.X_Max:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.X;
                            this.axisPosition = XboxAxisPosition.Max;
                        }

                        break;
                    case XboxCustomFunction.Y_Min:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Y;
                            this.axisPosition = XboxAxisPosition.Min;
                        }

                        break;
                    case XboxCustomFunction.Y_Max:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Y;
                            this.axisPosition = XboxAxisPosition.Max;
                        }

                        break;
                    case XboxCustomFunction.Rx_Min:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Rx;
                            this.axisPosition = XboxAxisPosition.Min;
                        }

                        break;
                    case XboxCustomFunction.Rx_Max:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Rx;
                            this.axisPosition = XboxAxisPosition.Max;
                        }

                        break;
                    case XboxCustomFunction.Ry_Min:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Ry;
                            this.axisPosition = XboxAxisPosition.Min;
                        }

                        break;
                    case XboxCustomFunction.Ry_Max:
                        {
                            this.controlType = KeyControlType.Axis;
                            this.axis = XboxAxis.Ry;
                            this.axisPosition = XboxAxisPosition.Max;
                        }

                        break;
                    default:
                        throw new NotImplementedException(
                            "Not implemented xbox custom function: " + value);
                }
            }
        }

        public void Dispose()
        {
            this.KeyGestureChanged = null;
            this.Removed = null;
            this.FunctionChanged = null;
        }

        private void OnKeyGestureChanged()
        {
            InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), this.KeyGesture);
            if (key == InterceptionKeys.LeftControl || key == InterceptionKeys.ShiftModifier)
            {
                // notifying the user to use this key gesture with caution
                this.label1.Foreground = Brushes.Red;
                this.label1.FontStyle = FontStyles.Oblique;
                this.label1.ToolTip = "It's not recommended to use this key. See Help->FAQ for details";
                AutoHideTooltip ahtt = new AutoHideTooltip(
                    "It's not recommended       \r\nto use this key", this.label1.PointToScreen(new Point()), 3000);

                ahtt.Show();
            }
            else
            {
                this.label1.Foreground = Brushes.Black;
                this.label1.FontStyle = FontStyles.Normal;
                this.label1.ToolTip = null;
            }

            if (this.KeyGestureChanged != null)
            {
                this.KeyGestureChanged(this, new KeyGestureChangedEventArgs(this.KeyGesture));
            }
        }

        private void OnRemoved()
        {
            if (this.Removed != null)
            {
                this.Removed(this, EventArgs.Empty);
            }
        }

        private void AssignButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string functionName;

            if (this.isRemoveable)
            {
                functionName = this.xboxFunctionBox.SelectedItem.ToString();
                functionName = functionName.Replace("_", " ") + " (custom)";
            }
            else
            {
                functionName = this.label1.Content.ToString();
            }

            string functionText = string.Format(
                "Virtual Xbox 360 Controller #{0}{1}{2}", this.parent.UserIndex, Environment.NewLine, functionName);

            var detector = new KeyDetector(functionText, this.parent.CurrentKeyboard);
            detector.KeyDetected += (ss, ee) =>
            {
                this.KeyGesture = (ee as KeyPressedEventArgs).CorrectedKey;
                detector.Close();
            };

            this.parent.IsEnabled = false;
            detector.ShowDialog();
            this.parent.IsEnabled = true;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.OnKeyGestureChanged();
        }

        private void XboxFunctionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // note that we are setting the property, not the field!
            this.CustomFunction = (XboxCustomFunction)xboxFunctionBox.SelectedItem;
            if (this.FunctionChanged != null)
            {
                this.FunctionChanged(this, EventArgs.Empty);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnRemoved();
        }
    }
}
