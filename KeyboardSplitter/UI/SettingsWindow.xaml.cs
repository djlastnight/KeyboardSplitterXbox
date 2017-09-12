namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using KeyboardSplitter.Models;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : CustomWindow
    {
        private bool initialInitializationInProgress;
        private bool manualChangeUpcomming;

        public SettingsWindow()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider.Value = GlobalSettings.Instance.MouseMoveDeadZone;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GlobalSettings.Instance.MouseMoveDeadZone = (int)e.NewValue;
        }

        private void UnplugAllClicked(object sender, RoutedEventArgs e)
        {
            int unpluggedControllersCount = 0;
            for (uint i = 1; i <= 4; i++)
            {
                if (VirtualXbox.VirtualXboxController.UnPlug(i, true))
                {
                    unpluggedControllersCount++;
                }
            }

            MessageBox.Show(
                unpluggedControllersCount + " virtual controllers were unplugged!",
                ApplicationInfo.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                this.initialInitializationInProgress = true;
                comboBox.SelectedIndex = GlobalSettings.Instance.StartingVirtualControllerUserIndex - 1;
                this.initialInitializationInProgress = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.initialInitializationInProgress)
            {
                return;
            }

            if (this.manualChangeUpcomming)
            {
                return;
            }

            var comboBox = sender as ComboBox;

            var dialogResult = MessageBox.Show(
                string.Format(
                "The requested operation requires slots reset.{0}Do you want to do that?",
                Environment.NewLine),
                ApplicationInfo.AppName,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    GlobalSettings.Instance.StartingVirtualControllerUserIndex = comboBox.SelectedIndex + 1;
                    mainWindow.Splitter.EmulationManager.Stop();
                    mainWindow.Splitter = new Splitter(mainWindow.SlotsCount);
                }
            }
            else
            {
                this.manualChangeUpcomming = true;
                comboBox.SelectedIndex = (int)e.RemovedItems[0] - 1;
                this.manualChangeUpcomming = false;
            }
        }
    }
}