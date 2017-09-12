namespace KeyboardSplitter.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using KeyboardSplitter.Models;
    using VirtualXbox;
    using VirtualXbox.Enums;
    using XinputWrapper;
    using XinputWrapper.Enums;

    public partial class XboxTestSlot : UserControl
    {
        private const int LeftStickDeadZone = XInputConstants.XinputGamepadLeftThumbDeadzone;
        private const int RightStickDeadZone = XInputConstants.XinputGamepadRightThumbDeadzone;
        private const int TriggerDeadZone = XInputConstants.XinputGamepadTriggerThreshold;

        public XboxTestSlot(XinputController xinputController)
        {
            this.InitializeComponent();
            if (xinputController == null)
            {
                throw new ArgumentNullException("xinputController");
            }

            if (!xinputController.IsConnected)
            {
                this.Content = new TextBlock()
                {
                    Text = string.Format("Xinput controller #{0} disconnected", xinputController.LedNumber),
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Foreground = Brushes.Red
                };

                return;
            }

            this.XinputController = xinputController;
            this.XinputController.StateChanged += this.OnXinputControllerStateChanged;
            var subType = this.XinputController.GetControllerSubType();
            bool isVirtual = this.XinputController.Tag != null;
            bool isGamepad = this.XinputController.IsGamepad == true;
            bool isWired = this.XinputController.IsWired == true;

            if (isVirtual)
            {
                var gamepad = this.XinputController.Tag as XboxGamepad;
                if (gamepad == null)
                {
                    this.descriptionTextBlock.Text = "Unknown virtual controller";
                    return;
                }

                this.connectionTimeTextBlock.Text = string.Format(
                    "Connection time: {0} ms", (int)gamepad.ConnectionTime.TotalMilliseconds);
            }
            else
            {
                if (VirtualXboxController.Exists(this.XinputController.LedNumber) &&
                    !VirtualXboxController.IsOwned(this.XinputController.LedNumber))
                {
                    this.descriptionTextBlock.Text = "Virtual controller is owned in another process.";
                    return;
                }
            }

            string prefix = isVirtual ? "Virtual" : isWired ? "Wired" : "Wireless";
            string suffix = isGamepad ? subType.ToString() : "Headset";

            this.descriptionTextBlock.Text = string.Format(
                "{0} {1} {2} [Led #{3}]",
                prefix,
                "Xbox 360",
                suffix,
                this.XinputController.LedNumber);
        }

        public XinputController XinputController
        {
            get;
            private set;
        }

        private void UpdateHighlights()
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
        }

        private bool IsButtonHighlighted(XboxButton button)
        {
            var buttonImage = this.GetButtonImage(button);
            if (buttonImage != null)
            {
                return buttonImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        private bool IsTriggerHighlighted(XboxTrigger trigger)
        {
            var triggerImage = this.GetTriggerImage(trigger);
            if (triggerImage != null)
            {
                return triggerImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        private bool IsDpadHightlighted(XboxDpadDirection direction)
        {
            var dpadImage = this.GetDpadImage(direction);
            if (dpadImage != null)
            {
                return dpadImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        private bool IsAxisHighlighted(XboxAxis axis, XboxAxisPosition position)
        {
            var axisImage = this.GetAxisImage(axis, position);
            if (axisImage != null)
            {
                return axisImage.Visibility == Visibility.Visible;
            }

            return false;
        }

        private void HighlightButton(XboxButton button)
        {
            Image buttonImage = this.GetButtonImage(button);
            bool shouldHighlight = this.XinputController.GetButtonState((XinputButton)(uint)button);
            this.SetImageVisibility(buttonImage, shouldHighlight);
        }

        private void HighlightTrigger(XboxTrigger trigger)
        {
            Image triggerImage = this.GetTriggerImage(trigger);
            var value = this.XinputController.GetTriggerState((XinputTrigger)(uint)trigger);
            if (trigger == XboxTrigger.Left)
            {
                this.LtValueTextBlock.Text = value != 0 ? value.ToString() : string.Empty;
            }
            else
            {
                this.RtValueTextBlock.Text = value != 0 ? value.ToString() : string.Empty;
            }

            bool shouldHighlight = value > XboxTestSlot.TriggerDeadZone;
            this.SetImageVisibility(triggerImage, shouldHighlight);
        }

        private void HighlightDpad(XboxDpadDirection direction)
        {
            if (direction == XboxDpadDirection.Off)
            {
                return;
            }

            Image dpadImage = this.GetDpadImage(direction);
            bool shouldHighlight = this.XinputController.GetButtonState((XinputButton)(uint)direction);
            this.SetImageVisibility(dpadImage, shouldHighlight);
        }

        private void HighlightAxis(XboxAxis axis)
        {
            Image minImage = this.GetAxisImage(axis, XboxAxisPosition.Min);
            Image maxImage = this.GetAxisImage(axis, XboxAxisPosition.Max);

            this.SetImageVisibility(minImage, false);
            this.SetImageVisibility(maxImage, false);

            bool isLeftStick = axis == XboxAxis.X || axis == XboxAxis.Y;
            int deadzone = isLeftStick ? XboxTestSlot.LeftStickDeadZone : XboxTestSlot.RightStickDeadZone;

            short value = this.XinputController.GetAxisState((XinputAxis)(uint)axis);
            if (Math.Abs((int)value) < deadzone)
            {
                return;
            }

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
                case XboxTrigger.Left:
                    targetImage = this.imageLT;
                    break;
                case XboxTrigger.Right:
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
                case XboxDpadDirection.Off:
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

        private void OnXinputControllerStateChanged(object sender, XinputControllerStateChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.UpdateHighlights();
            });
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateHighlights();
        }
    }
}
