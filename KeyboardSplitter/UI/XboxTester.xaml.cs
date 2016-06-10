namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Managers;
    using XboxInterfaceWrap;

    public partial class XboxTester : UserControl, IDisposable
    {
        private readonly JoyControl joyControl;

        public XboxTester(JoyControl control)
            : this()
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            if (!VirtualXboxController.Exists(control.UserIndex))
            {
                throw new InvalidOperationException(
                    "You can not test non existing virtual xbox 360 controller. UserIndex: " +
                    control.UserIndex);
            }

            this.joyControl = control;
            KeyboardManager.KeyPressed += this.KeyboardManager_KeyPressed;
            this.presetNameLabel.Content += this.joyControl.CurrentPreset.Name;
            this.xboxDeviceNameLabel.Content += this.joyControl.UserIndex.ToString();
            this.keyboardNameLabel.Content = this.joyControl.CurrentKeyboard;
        }

        private XboxTester()
        {
            this.InitializeComponent();
        }

        public void UpdateHighlights()
        {
            Dispatcher.Invoke((Action)delegate
            {
                foreach (XboxButton button in Enum.GetValues(typeof(XboxButton)))
                {
                    this.HighlightButton(button);
                }

                foreach (XboxTrigger trigger in Enum.GetValues(typeof(XboxTrigger)))
                {
                    this.HighlightTrigger(trigger);
                }

                foreach (XboxDpadDirection direction in Enum.GetValues(typeof(XboxDpadDirection)))
                {
                    this.HighlightDpad(direction);
                }

                foreach (XboxAxis axis in Enum.GetValues(typeof(XboxAxis)))
                {
                    this.HighlightAxis(axis);
                }
            });
        }

        public bool IsButtonHighlighted(XboxButton button)
        {
            var buttonImage = this.GetButtonImage(button);
            if (buttonImage != null)
            {
                return buttonImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        public bool IsTriggerHighlighted(XboxTrigger trigger)
        {
            var triggerImage = this.GetTriggerImage(trigger);
            if (triggerImage != null)
            {
                return triggerImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        public bool IsDpadHightlighted(XboxDpadDirection direction)
        {
            var dpadImage = this.GetDpadImage(direction);
            if (dpadImage != null)
            {
                return dpadImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        public bool IsAxisHighlighted(XboxAxis axis, XboxAxisPosition position)
        {
            var axisImage = this.GetAxisImage(axis, position);
            if (axisImage != null)
            {
                return axisImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        public void Dispose()
        {
            KeyboardManager.KeyPressed -= this.KeyboardManager_KeyPressed;
        }

        private void HighlightButton(XboxButton button)
        {
            Image buttonImage = this.GetButtonImage(button);
            bool shouldHighlight = VirtualXboxController.GetButtonValue(this.joyControl.UserIndex, button);

            this.SetImageVisibility(buttonImage, shouldHighlight);
        }

        private void HighlightTrigger(XboxTrigger trigger)
        {
            Image triggerImage = this.GetTriggerImage(trigger);
            bool shouldHighlight = VirtualXboxController.GetTriggerValue(
                this.joyControl.UserIndex, trigger) > 0;

            this.SetImageVisibility(triggerImage, shouldHighlight);
        }

        private void HighlightDpad(XboxDpadDirection direction)
        {
            Image dpadImage = this.GetDpadImage(direction);
            bool shouldHighlight = VirtualXboxController.GetDpadDirectionValue(this.joyControl.UserIndex, direction);

            this.SetImageVisibility(dpadImage, shouldHighlight);
        }

        private void HighlightAxis(XboxAxis axis)
        {
            Image minImage = this.GetAxisImage(axis, XboxAxisPosition.Min);
            Image maxImage = this.GetAxisImage(axis, XboxAxisPosition.Max);

            this.SetImageVisibility(minImage, false);
            this.SetImageVisibility(maxImage, false);

            short value = VirtualXboxController.GetAxisValue(this.joyControl.UserIndex, axis);
            if (value < 0)
            {
                this.SetImageVisibility(minImage, true);
            }
            else if (value > 0)
            {
                this.SetImageVisibility(maxImage, true);
            }
        }

        private void SetImageVisibility(Image imageToHighlight, bool value)
        {
            if (imageToHighlight != null)
            {
                if (value == true)
                {
                    imageToHighlight.Visibility = Visibility.Visible;
                }
                else
                {
                    imageToHighlight.Visibility = Visibility.Hidden;
                }
            }
        }

        private Image GetButtonImage(XboxButton button)
        {
            Image targetImage;
            switch (button)
            {
                case XboxButton.Guide:
                    targetImage = this.imageGuide;
                    break;
                case XboxButton.A:
                    targetImage = this.imageA;
                    break;
                case XboxButton.B:
                    targetImage = this.imageB;
                    break;
                case XboxButton.X:
                    targetImage = this.imageX;
                    break;
                case XboxButton.Y:
                    targetImage = this.imageY;
                    break;
                case XboxButton.LeftBumper:
                    targetImage = this.imageLB;
                    break;
                case XboxButton.RightBumper:
                    targetImage = this.imageRB;
                    break;
                case XboxButton.Back:
                    targetImage = this.imageBack;
                    break;
                case XboxButton.Start:
                    targetImage = this.imageStart;
                    break;
                case XboxButton.LeftThumb:
                    targetImage = this.imageLeftThumb;
                    break;
                case XboxButton.RightThumb:
                    targetImage = this.imageRightThumb;
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox button: " + button);
            }

            return targetImage;
        }

        private Image GetTriggerImage(XboxTrigger trigger)
        {
            Image targetImage;
            switch (trigger)
            {
                case XboxTrigger.LeftTrigger:
                    targetImage = this.imageLT;
                    break;
                case XboxTrigger.RightTrigger:
                    targetImage = this.imageRT;
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox trigger: " + trigger);
            }

            return targetImage;
        }

        private Image GetDpadImage(XboxDpadDirection direction)
        {
            Image targetImage;
            switch (direction)
            {
                case XboxDpadDirection.None:
                    targetImage = null;
                    break;
                case XboxDpadDirection.Up:
                    targetImage = this.imageDpadUp;
                    break;
                case XboxDpadDirection.Down:
                    targetImage = this.imageDpadDown;
                    break;
                case XboxDpadDirection.Left:
                    targetImage = this.imageDpadLeft;
                    break;
                case XboxDpadDirection.Right:
                    targetImage = this.imageDpadRight;
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox dpad direction: " + direction);
            }

            return targetImage;
        }

        private Image GetAxisImage(XboxAxis axis, XboxAxisPosition position)
        {
            Image targetImage;
            switch (axis)
            {
                case XboxAxis.X:
                    {
                        switch (position)
                        {
                            case XboxAxisPosition.Min:
                                targetImage = this.imageLsLeft;
                                break;
                            case XboxAxisPosition.Center:
                                targetImage = null;
                                break;
                            case XboxAxisPosition.Max:
                                targetImage = this.imageLsRight;
                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented axis position: " + position);
                        }
                    }

                    break;
                case XboxAxis.Y:
                    {
                        switch (position)
                        {
                            case XboxAxisPosition.Min:
                                targetImage = this.imageLsDown;
                                break;
                            case XboxAxisPosition.Center:
                                targetImage = null;
                                break;
                            case XboxAxisPosition.Max:
                                targetImage = this.imageLsUp;
                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented axis position: " + position);
                        }
                    }

                    break;
                case XboxAxis.Rx:
                    {
                        switch (position)
                        {
                            case XboxAxisPosition.Min:
                                targetImage = this.imageRsLeft;
                                break;
                            case XboxAxisPosition.Center:
                                targetImage = null;
                                break;
                            case XboxAxisPosition.Max:
                                targetImage = this.imageRsRight;
                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented axis position: " + position);
                        }
                    }

                    break;
                case XboxAxis.Ry:
                    {
                        switch (position)
                        {
                            case XboxAxisPosition.Min:
                                targetImage = this.imageRsDown;
                                break;
                            case XboxAxisPosition.Center:
                                targetImage = null;
                                break;
                            case XboxAxisPosition.Max:
                                targetImage = this.imageRsUp;
                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented axis position: " + position);
                        }
                    }

                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox axis: " + axis);
            }

            return targetImage;
        }

        private void KeyboardManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Keyboard.StrongName == this.joyControl.CurrentKeyboard)
            {
                this.UpdateHighlights();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }
    }
}
