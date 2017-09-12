namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using KeyboardSplitter.Controls;
    using XinputWrapper;

    /// <summary>
    /// Interaction logic for ContollerTestWindow.xaml
    /// </summary>
    public partial class XinputControllerTestWindow : CustomWindow
    {
        private bool isLocationChanged;
        private bool changingLocationManually;
        private DispatcherTimer timer;
        private TimeSpan rescanDelay = TimeSpan.FromSeconds(1);

        public XinputControllerTestWindow(Window owner)
        {
            this.InitializeComponent();
            this.Owner = owner;
            this.timer = new DispatcherTimer();
            this.timer.Interval = this.rescanDelay;
            this.timer.Tick += this.OnTimerTick;
            this.Refresh();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (this.IsLoaded && !this.changingLocationManually && !this.isLocationChanged)
            {
                this.isLocationChanged = true;
            }

            base.OnLocationChanged(e);
        }

        private void Refresh()
        {
            this.button.IsEnabled = false;
            this.panel.Children.Clear();
            this.panel.Children.Add(new TextBlock()
            {
                Text = "Scanning xinput controllers, please wait...",
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            });

            this.Cursor = Cursors.Wait;
            this.timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.SizeToContent = SizeToContent.Manual;
            this.panel.Children.Clear();
            for (int i = 0; i < 4; i++)
            {
                var xinputController = XinputController.RetrieveController(i);
                xinputController.PluggedChanged += this.OnXinputControllerPluggedChanged;
                if (xinputController.IsConnected)
                {
                    var slot = new XboxTestSlot(xinputController);
                    this.panel.Children.Add(slot);
                }
            }

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.button.IsEnabled = true;
            this.Cursor = Cursors.Arrow;
        }

        private void OnXinputControllerPluggedChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                if (this.checkbox.IsChecked == true)
                {
                    this.Refresh();
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.isLocationChanged)
            {
                this.changingLocationManually = true;
                this.Left = this.Owner.Left + ((this.Owner.ActualWidth - this.ActualWidth) / 2);
                this.Top = this.Owner.Top + ((this.Owner.ActualHeight - this.ActualHeight) / 2);
                this.changingLocationManually = false;
            }
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}