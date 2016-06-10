namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Media;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.Resources;
    using KeyboardSplitter.UI;
    using XboxInterfaceWrap;

    public partial class MainWindow : Window, IDisposable
    {
        private const uint DefaultSlotsCount = 2;

        private DispatcherTimer autoCollapseTimer;

        private TimeSpan autoCollapseSpan = TimeSpan.FromSeconds(60);

        private List<InterceptionKeyboard> initialKeyboards;

        private string emergencyKey = InterceptionKeys.LeftControl.ToString();

        private int emergencyHitDownCount;

        private int emergencyHitUpCount;

        private ManagementEventWatcher usbWatcher;

        private EmulationManager emulationManager;

        public MainWindow()
        {
            this.InitializeComponent();
            this.CheckForObsoleteOS();
            this.CheckDrivers();
            this.Title = ApplicationInfo.AppNameVersion;
            this.autoCollapseTimer = new DispatcherTimer();
            this.autoCollapseTimer.Interval = this.autoCollapseSpan;
            this.autoCollapseTimer.Tick += new EventHandler(this.AutoCollapseTimer_Tick);
            KeyboardManager.KeyPressed += this.KeyboardManager_KeyPressed;
            this.emulationManager = new EmulationManager(DefaultSlotsCount);
        }

        public void Dispose()
        {
            this.StopUsbWatcher();
            this.emulationManager.Dispose();
            this.emulationManager = null;
            PresetDataManager.ExportToFile();
        }

        private void CheckForObsoleteOS()
        {
            var version = Environment.OSVersion.Version;
            if (version.Major < 5)
            {
                LogWriter.Write("Obsolete OS detected: " + Environment.OSVersion.VersionString);
                System.Windows.MessageBox.Show(
                    "Your operating system is not supported!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(0);
            }
        }

        private void CheckDrivers()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                LogWriter.Write("Built-in drivers are not installed, asking user to install them");
                var result = System.Windows.MessageBox.Show(
                    "It seems that the required built-in drivers are not installed.\r\n" +
                    "Do you want to install them (may require reboot)?\r\n\r\n" +
                    "Selecting \"No\" will quit the application.",
                    "Drivers required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DriversManager.InstallBuiltInDrivers();
                }
                else
                {
                    LogWriter.Write(
                        "User has choosen to NOT install the built-in drivers. Exitting");

                    Environment.Exit(0);
                }
            }
        }

        private void Stop()
        {
            this.emulationManager.Stop();

            this.stopButton.IsEnabled = false;
            this.startButton.IsEnabled = true;
            this.deviceCountBox.IsEnabled = true;
            this.emergencyHitDownCount = 0;
        }

        private void Reset()
        {
            uint slotsCount = Convert.ToUInt32(this.deviceCountBox.SelectedItem);
            this.emulationManager.Dispose();
            this.emulationManager = new EmulationManager(slotsCount);

            this.emergencyHitDownCount = 0;
            this.wrapPanel.Children.Clear();

            GC.Collect();

            this.initialKeyboards = KeyboardManager.GetKeyboards();

            foreach (JoyControl joyControl in this.emulationManager.JoyControls)
            {
                this.wrapPanel.Children.Add(joyControl);
            }

            // autosizing
            int screenWidth = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).Bounds.Width;
            if (screenWidth >= 1280 && this.WindowState != WindowState.Maximized)
            {
                this.SizeToContent = SizeToContent.Width;
            }
        }

        private void AddInputToMonitor(KeyPressedEventArgs e)
        {
            if (!this.expander.IsExpanded)
            {
                return;
            }

            if (this.inputMonitor.Text.Length > 10000)
            {
                this.inputMonitor.Clear();
            }

            this.inputMonitor.AppendText(
                string.Format("'{0}' on {1} ({2}) [{3}]{4}", e.CorrectedKey, e.Keyboard.StrongName, e.State, Convert.ToInt32(e.Key), Environment.NewLine));

            this.inputMonitor.ScrollToEnd();
        }

        private UnsavedPresetsData GetUnsavedPresetsData()
        {
            // collects data about all unsaved presets
            var output = new UnsavedPresetsData();

            if (this.wrapPanel.Children.Count == 0)
            {
                return output;
            }

            var notSavedControls = this.emulationManager.JoyControls.Where(x => x.CanSavePreset);

            if (notSavedControls.Count() == 0)
            {
                return output;
            }

            var sb = new StringBuilder();

            List<Preset> distintPresets = new List<Preset>();
            foreach (var notSavedControl in notSavedControls)
            {
                if (!distintPresets.Contains(notSavedControl.CurrentPreset))
                {
                    sb.AppendLine(
                        string.Format("- {0} [vXbox Device #{1}]", notSavedControl.PresetBoxText, notSavedControl.UserIndex));

                    output.NotSavedControls.Add(notSavedControl);
                    distintPresets.Add(notSavedControl.CurrentPreset);
                }
                else
                {
                    output.IgnoredControls.Add(notSavedControl);
                }
            }

            if (output.IgnoredControls.Count > 0)
            {
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("The following 'cloned' presets will be ignored to avoid ambiguity:");

                foreach (var ignoredControl in output.IgnoredControls)
                {
                    sb.AppendLine(
                        string.Format("- {0} [vXbox Device #{1}]", ignoredControl.PresetBoxText, ignoredControl.UserIndex));
                }
            }

            output.Message = sb.ToString();

            return output;
        }

        private void CheckForEmergencyHit(KeyPressedEventArgs e)
        {
            if (this.startButton.IsEnabled)
            {
                return;
            }

            if (e.CorrectedKey == this.emergencyKey)
            {
                if (e.State == KeyState.Down || e.State == KeyState.E0)
                {
                    this.emergencyHitDownCount++;
                }
                else if (e.State == KeyState.Up || e.State == (KeyState.Up | KeyState.E0))
                {
                    this.emergencyHitUpCount++;
                }
            }
            else
            {
                this.emergencyHitDownCount = 0;
                this.emergencyHitUpCount = 0;
                return;
            }

            if (this.emergencyHitDownCount >= 5 && this.emergencyHitUpCount >= 5)
            {
                if (this.blockKeyboardCheckbox.IsChecked == true)
                {
                    using (var player = new SoundPlayer(Sounds.Disconnected))
                    {
                        player.Play();
                    }

                    this.blockKeyboardCheckbox.IsChecked = false;

                    LogWriter.Write("Emergency activated: Block choosen keyboards unchecked");
                }
                else
                {
                    using (var player = new SoundPlayer(Sounds.Connected))
                    {
                        player.Play();
                    }

                    this.blockKeyboardCheckbox.IsChecked = true;

                    LogWriter.Write("Emergency activated: Block choosen keyboards checked");
                }

                this.emergencyHitDownCount = 0;
                this.emergencyHitUpCount = 0;
            }
        }

        private void StartUsbWatcher()
        {
            string queryString = "SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'";

            this.usbWatcher = new ManagementEventWatcher(queryString);
            this.usbWatcher.EventArrived += new EventArrivedEventHandler(this.OnUsbPlugUnplug);
            this.usbWatcher.Start();
        }

        private void StopUsbWatcher()
        {
            if (this.usbWatcher != null)
            {
                this.usbWatcher.EventArrived -= this.OnUsbPlugUnplug;
                this.usbWatcher.Stop();
                this.usbWatcher.Dispose();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartUsbWatcher();
            LogWriter.Write("Main window loaded");
            this.deviceCountBox.ItemsSource = new int[] { 1, 2, 3, 4 };
            this.deviceCountBox.SelectedIndex = 1;
            this.expander.ToolTip = "Click to expand/collapse the keyboards input monitor.\r\n" +
                "It will autocollapse after " + this.autoCollapseSpan.TotalSeconds + " seconds to save CPU time.";
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // checking for unsaved presets
            var unsavedPresets = this.GetUnsavedPresetsData();

            if (unsavedPresets.NotSavedControls.Count > 0)
            {
                var result = System.Windows.MessageBox.Show(
                    "Do you want to save the following unsaved presets, before you quit?\r\n" + unsavedPresets.Message,
                    "You are about to quit " + ApplicationInfo.AppNameVersion,
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // save the presets
                    unsavedPresets.SaveNotSavedPresets();
                }

                e.Cancel = result == MessageBoxResult.Cancel;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
            LogWriter.Write("Application terminated by user");
        }

        private void KeyboardManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.CheckForEmergencyHit(e);

                this.AddInputToMonitor(e);

                if (!this.IsEnabled || this.startButton.IsEnabled)
                {
                    return;
                }

                bool blockChoosenKeyboards = this.blockKeyboardCheckbox.IsChecked == true;
                this.emulationManager.ProcessKeyPress(e, blockChoosenKeyboards);
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int slotsCount = Convert.ToInt32(this.deviceCountBox.SelectedItem);
            try
            {
                this.emulationManager.Start();

                this.inputMonitor.Clear();
                this.startButton.IsEnabled = false;
                this.stopButton.IsEnabled = true;
                this.deviceCountBox.IsEnabled = false;

                AutoHideTooltip ahtt = new AutoHideTooltip(
                    "Emulation successfully started. You may now start your game.\r\n" +
                    "To block/unblock the assigned keyboards, press LeftControl 5 times.",
                    this.startButton.PointToScreen(new Point(this.startButton.Width, 0)));

                ahtt.Owner = this;
                ahtt.Show();
            }
            catch (EmulationManagerException ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message,
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        private void DeviceCountBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LogWriter.Write("Slots count changed to " + Convert.ToInt32(this.deviceCountBox.SelectedItem));

            // checking for unsaved presets
            var unsavedPresets = this.GetUnsavedPresetsData();

            if (unsavedPresets.NotSavedControls.Count > 0)
            {
                var result = System.Windows.MessageBox.Show(
                    "Do you want to save the following unsaved presets,\r\nbefore you change the slots count?\r\n" + unsavedPresets.Message,
                    "Presets save",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // save the presets
                    unsavedPresets.SaveNotSavedPresets();
                }
            }

            this.Reset();
        }

        private void AutoCollapseTimer_Tick(object sender, EventArgs e)
        {
            this.expander.IsExpanded = false;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.lastGridRow.Height = new GridLength(100);
            this.autoCollapseTimer.Start();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.autoCollapseTimer.Stop();
            this.lastGridRow.Height = new GridLength(30);
            this.inputMonitor.Clear();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenGamepadProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("joy.cpl");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Can not open gamepad properties: " + ex.Message);
            }
        }

        private void OpenXboxSite(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.microsoft.com/hardware/en-us/d/xbox-360-controller-for-windows");
            }
            catch (Exception)
            {
            }
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dialog = new AboutDialog("About " + ApplicationInfo.AppNameVersion);
            dialog.ShowDialog();
        }

        private void UninstallBuiltInDrivers_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "Are you sure that you want to remove the built-in drivers?",
                "Confirm uninstall",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DriversManager.UninstallBuiltInDrivers();
            }
        }

        private void ControllerTest_Click(object sender, RoutedEventArgs e)
        {
            if (this.startButton.IsEnabled)
            {
                System.Windows.MessageBox.Show(
                    "You can not test controllers, while the emulation is stopped!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Hand);

                return;
            }

            Window testWindow = new Window();
            WrapPanel panel = new WrapPanel();
            foreach (var joyControl in this.emulationManager.JoyControls)
            {
                if (VirtualXboxController.Exists(joyControl.UserIndex) &&
                    VirtualXboxController.IsOwned(joyControl.UserIndex))
                {
                    panel.Children.Add(new XboxTester(joyControl));
                }
            }

            if (panel.Children.Count == 0)
            {
                System.Windows.MessageBox.Show(
                    "There are no virtual controllers currently plugged-in!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            testWindow.WindowStyle = WindowStyle.ToolWindow;
            testWindow.ResizeMode = ResizeMode.NoResize;
            testWindow.Owner = this;
            testWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            testWindow.Title = "Virtual Xbox Controllers Test";
            testWindow.Content = panel;
            testWindow.SizeToContent = SizeToContent.WidthAndHeight;
            testWindow.ShowDialog();
        }

        private void HelpContents_Click(object sender, RoutedEventArgs e)
        {
            UI_Help howToUse = new UI_Help();
            howToUse.ShowDialog();
        }

        private void HowItWorks_Click(object sender, RoutedEventArgs e)
        {
            HowItWorks how = new HowItWorks();
            how.ShowDialog();
        }

        private void FAQ_Click(object sender, RoutedEventArgs e)
        {
            FAQ fac = new FAQ();
            fac.ShowDialog();
        }

        private void OnUsbPlugUnplug(object sender, EventArrivedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                if (this.emulationManager == null)
                {
                    return;
                }

                var newKeyboards = KeyboardManager.GetKeyboards();
                switch (e.NewEvent.ClassPath.ClassName)
                {
                    case "__InstanceDeletionEvent":
                        {
                            foreach (var joyControl in this.emulationManager.JoyControls)
                            {
                                if (joyControl.CurrentKeyboard == null)
                                {
                                    continue;
                                }

                                var joyKeyboard = this.initialKeyboards.Find(x => x.StrongName == joyControl.CurrentKeyboard);
                                if (newKeyboards.FirstOrDefault(x => x.IsTheSameAs(joyKeyboard)) == null)
                                {
                                    // joycontrol's keyboard was unplugged
                                    joyControl.Invalidate(SlotInvalidationReason.Keyboard_Unplugged);
                                }
                            }
                        }

                        break;
                    case "__InstanceCreationEvent":
                        break;
                    default:
                        break;
                }
            });
        }
    }
}