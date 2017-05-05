using KeyboardSplitter.Controls;
using KeyboardSplitter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XboxInterfaceWrap;

namespace KeyboardSplitter.UI
{
    /// <summary>
    /// Interaction logic for OnScreenController.xaml
    /// </summary>
    public partial class OnScreenController : UserControl
    {
        public OnScreenController()
        {
            InitializeComponent();
        }

        public uint UserIndex
        {
            get;
            private set;
        }

        // Guide button
        private void OnGuideDown(object sender, MouseButtonEventArgs e)
        {
            if (VirtualXboxController.IsOwned(this.UserIndex))
            {
                VirtualXboxController.SetButton(this.UserIndex, XboxButton.Guide, true);
                this.imageGuide.Opacity = 1;
            }
        }

        private void OnGuideUp(object sender, MouseButtonEventArgs e)
        {
            if (VirtualXboxController.IsOwned(this.UserIndex))
            {
                VirtualXboxController.SetButton(this.UserIndex, XboxButton.Guide, false);
                this.imageGuide.Opacity = 0;
            }
        }

        private void OnGuideEnter(object sender, MouseEventArgs e)
        {
            this.imageGuide.Opacity = 0.3;
        }

        private void OnGuideLeave(object sender, MouseEventArgs e)
        {
            this.imageGuide.Opacity = 0;
        }

        // Button A
        private void OnADown(object sender, MouseButtonEventArgs e)
        {
            if (VirtualXboxController.IsOwned(this.UserIndex))
            {
                VirtualXboxController.SetButton(this.UserIndex, XboxButton.A, true);
                this.imageA.Opacity = 1;
            }
        }

        private void OnAUp(object sender, MouseButtonEventArgs e)
        {
            if (VirtualXboxController.IsOwned(this.UserIndex))
            {
                VirtualXboxController.SetButton(this.UserIndex, XboxButton.A, false);
                this.imageA.Opacity = 0;
            }
        }

        private void OnAEnter(object sender, MouseEventArgs e)
        {
            this.imageA.Opacity = 0.3;
        }

        private void OnALeave(object sender, MouseEventArgs e)
        {
            this.imageA.Opacity = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UserIndex = ParentFinder.FindParent<JoyControl>(this).UserIndex;
        }
    }
}
