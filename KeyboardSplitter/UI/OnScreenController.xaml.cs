using KeyboardSplitter.Controls;
using KeyboardSplitter.Enums;
using KeyboardSplitter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XboxInterfaceWrap;

namespace KeyboardSplitter.UI
{
    /// <summary>
    /// Interaction logic for OnScreenController.xaml
    /// </summary>
    public partial class OnScreenController : UserControl
    {
        private Dictionary<Button, Image> pairs;

        public OnScreenController()
        {
            InitializeComponent();
            this.pairs = new Dictionary<Button, Image>();
            this.pairs.Add(this.buttonGuide, this.imageGuide);
            this.pairs.Add(this.buttonA, this.imageA);
            this.pairs.Add(this.buttonB, this.imageB);
            this.pairs.Add(this.buttonX, this.imageX);
            this.pairs.Add(this.buttonY, this.imageY);
            this.pairs.Add(this.buttonLB, this.imageLB);
            this.pairs.Add(this.buttonRB, this.imageRB);
            this.pairs.Add(this.buttonBack, this.imageBack);
            this.pairs.Add(this.buttonStart, this.imageStart);
            this.pairs.Add(this.buttonLT, this.imageLT);
            this.pairs.Add(this.buttonRT, this.imageRT);
            this.pairs.Add(this.buttonDpadLeft, this.imageDpadLeft);
            this.pairs.Add(this.buttonDpadRight, this.imageDpadRight);
            this.pairs.Add(this.buttonDpadUp, this.imageDpadUp);
            this.pairs.Add(this.buttonDpadDown, this.imageDpadDown);
            this.pairs.Add(this.buttonLeftThumb, this.imageLeftThumb);
            this.pairs.Add(this.buttonRightThumb, this.imageRightThumb);
            this.pairs.Add(this.buttonLsLeft, this.imageLsLeft);
            this.pairs.Add(this.buttonLsRight, this.imageLsRight);
            this.pairs.Add(this.buttonLsUp, this.imageLsUp);
            this.pairs.Add(this.buttonLsDown, this.imageLsDown);
            this.pairs.Add(this.buttonRsLeft, this.imageRsLeft);
            this.pairs.Add(this.buttonRsRight, this.imageRsRight);
            this.pairs.Add(this.buttonRsUp, this.imageRsUp);
            this.pairs.Add(this.buttonRsDown, this.imageRsDown);

            foreach (var pair in pairs)
            {
                var button = pair.Key;
                var image = pair.Value;

                image.Opacity = 0;
                button.Opacity = 0;
                button.PreviewMouseDown += this.OnButtonDown;
                button.PreviewMouseUp += this.OnButtonUp;
                button.MouseEnter += this.OnButtonEnter;
                button.MouseLeave += this.OnButtonLeave;
            }
        }

        public uint UserIndex
        {
            get;
            private set;
        }

        private void SetFunctionState(Image image, XboxCustomFunction function, bool state)
        {
            if (VirtualXboxController.IsOwned(this.UserIndex))
            {
                CustomFunctionHelper.SetFunctionState(function, this.UserIndex, state);
                image.Opacity = state ? 1 : 0;
            }
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            var image = pairs.First(x => x.Key == button).Value;
            var function = (XboxCustomFunction)Enum.Parse(typeof(XboxCustomFunction), button.Tag as string);
            this.SetFunctionState(image, function, true);
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            var image = pairs.First(x => x.Key == button).Value;
            var function = (XboxCustomFunction)Enum.Parse(typeof(XboxCustomFunction), button.Tag as string);
            this.SetFunctionState(image, function, false);
        }

        private void OnButtonEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            var image = pairs.First(x => x.Key == button).Value;
            image.Opacity = 0.3;
        }

        private void OnButtonLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            var image = pairs.First(x => x.Key == button).Value;
            image.Opacity = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UserIndex = ParentFinder.FindParent<JoyControl>(this).UserIndex;
        }
    }
}
