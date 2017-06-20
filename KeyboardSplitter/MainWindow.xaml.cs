namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Media;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using Interceptor;
    using KeyboardSplitter.Commands;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.Resources;
    using KeyboardSplitter.UI;
    using XboxInterfaceWrap;
    using System.Threading;

    public partial class MainWindow : Window, IDisposable
    {
        public static readonly DependencyProperty ShouldBlockKeyboardsProperty =
            DependencyProperty.Register(
            "ShouldBlockKeyboards",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShouldBlockMouseProperty =
            DependencyProperty.Register(
            "ShouldBlockMouse",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsEmulationStartedProperty =
            DependencyProperty.Register(
            "IsEmulationStarted",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SlotsCountProperty =
            DependencyProperty.Register(
            "SlotsCount",
            typeof(int),
            typeof(MainWindow),
            new PropertyMetadata(0));

        public static readonly DependencyProperty SlotsCountItemsSourceProperty =
            DependencyProperty.Register(
            "SlotsCountItemsSource",
            typeof(IEnumerable<int>),
            typeof(MainWindow),
            new PropertyMetadata(new List<int> { 1, 2, 3, 4 }));

        public static readonly DependencyProperty JoyControlsProperty =
            DependencyProperty.Register(
            "JoyControls",
            typeof(ObservableCollection<JoyControl>),
            typeof(MainWindow),
            new PropertyMetadata(new ObservableCollection<JoyControl>()));

        public static readonly DependencyProperty InputMonitorHistoryProperty =
            DependencyProperty.Register(
            "InputMonitorHistory",
            typeof(string),
            typeof(MainWindow),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsInputMonitorExpandedProperty =
            DependencyProperty.Register(
            "IsInputMonitorExpanded",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty InputMonitorTooltipProperty =
            DependencyProperty.Register(
            "InputMonitorTooltip",
            typeof(string),
            typeof(MainWindow),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty EmulationInformationProperty =
            DependencyProperty.Register(
            "EmulationInformation",
            typeof(string),
            typeof(MainWindow),
            new PropertyMetadata(string.Empty));

        private bool disposed;

        private DispatcherTimer autoCollapseTimer;

        private TimeSpan autoCollapseSpan = TimeSpan.FromSeconds(60);

        private List<InterceptionKeyboard> initialKeyboards;

        private List<InterceptionMouse> initialMouses;

        private string emergencyKey = InterceptionKeys.LeftControl.ToString();

        private int emergencyHitDownCount;

        private int emergencyHitUpCount;

        private ManagementEventWatcher usbWatcher;

        private System.Windows.Input.ICommand startEmulationCommand;

        private System.Windows.Input.ICommand stopEmulationCommand;

        public MainWindow()
        {
            this.InitializeComponent();
            this.CheckForObsoleteOS();
            this.CheckDrivers();
            this.Title = ApplicationInfo.AppNameVersion;
            this.autoCollapseTimer = new DispatcherTimer();
            this.autoCollapseTimer.Interval = this.autoCollapseSpan;
            this.autoCollapseTimer.Tick += new EventHandler(this.AutoCollapseTimer_Tick);
            InputManager.KeyPressed += new EventHandler(this.InputManagerKeyPressed);
            InputManager.MousePressed += new EventHandler(this.InputManagerMousePressed);
        }

        public bool ShouldBlockKeyboards
        {
            get { return (bool)this.GetValue(ShouldBlockKeyboardsProperty); }
            set { this.SetValue(ShouldBlockKeyboardsProperty, value); }
        }

        public bool ShouldBlockMouse
        {
            get { return (bool)this.GetValue(ShouldBlockMouseProperty); }
            set { this.SetValue(ShouldBlockMouseProperty, value); }
        }

        public bool IsEmulationStarted
        {
            get { return (bool)this.GetValue(IsEmulationStartedProperty); }
            set { this.SetValue(IsEmulationStartedProperty, value); }
        }

        public int SlotsCount
        {
            get { return (int)this.GetValue(SlotsCountProperty); }
            set { this.SetValue(SlotsCountProperty, value); }
        }

        public ObservableCollection<JoyControl> JoyControls
        {
            get { return (ObservableCollection<JoyControl>)this.GetValue(JoyControlsProperty); }
            set { this.SetValue(JoyControlsProperty, value); }
        }

        public IEnumerable<int> SlotsCountItemsSource
        {
            get { return (IEnumerable<int>)this.GetValue(SlotsCountItemsSourceProperty); }
            set { this.SetValue(SlotsCountItemsSourceProperty, value); }
        }

        public string InputMonitorHistory
        {
            get { return (string)this.GetValue(InputMonitorHistoryProperty); }
            set { this.SetValue(InputMonitorHistoryProperty, value); }
        }

        public bool IsInputMonitorExpanded
        {
            get { return (bool)this.GetValue(IsInputMonitorExpandedProperty); }
            set { this.SetValue(IsInputMonitorExpandedProperty, value); }
        }

        public string InputMonitorTooltip
        {
            get { return (string)this.GetValue(InputMonitorTooltipProperty); }
            set { this.SetValue(InputMonitorTooltipProperty, value); }
        }

        public string EmulationInformation
        {
            get { return (string)this.GetValue(EmulationInformationProperty); }
            set { this.SetValue(EmulationInformationProperty, value); }
        }

        public System.Windows.Input.ICommand StartEmulationCommand
        {
            get
            {
                if (this.startEmulationCommand == null)
                {
                    this.startEmulationCommand = new RelayCommand(this.OnStartEmulationRequested);
                }

                return this.startEmulationCommand;
            }
        }

        public System.Windows.Input.ICommand StopEmulationCommand
        {
            get
            {
                if (this.stopEmulationCommand == null)
                {
                    this.stopEmulationCommand = new RelayCommand(this.OnStopEmulationRequested);
                }

                return this.stopEmulationCommand;
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.Dispose(true);
            GC.SuppressFinalize(this);
            this.disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.StopUsbWatcher();
                Thread.Sleep(100);
                EmulationManager.Destroy();
                PresetDataManager.ExportToFile();
                LogWriter.Write("Main window disposed");
            }
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
                    ApplicationInfo.AppName + " | Drivers required",
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

        private void Reset()
        {
            this.emergencyHitDownCount = 0;
            this.JoyControls.Clear();
            this.initialKeyboards = InputManager.GetKeyboards();
            this.initialMouses = InputManager.GetMouses();

            EmulationManager.Create((uint)this.SlotsCount, this.initialKeyboards.Select(x => x.StrongName).ToArray());

            foreach (JoyControl joyControl in EmulationManager.JoyControls)
            {
                this.JoyControls.Add(joyControl);
            }

            // autosizing
            int screenWidth = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).Bounds.Width;
            if (screenWidth >= 1280 && this.WindowState != WindowState.Maximized)
            {
                this.SizeToContent = SizeToContent.Width;
            }
        }

        private UnsavedPresetsData GetUnsavedPresetsData()
        {
            // collects data about all unsaved presets
            var output = new UnsavedPresetsData();

            if (this.JoyControls.Count == 0)
            {
                return output;
            }

            var notSavedControls = EmulationManager.JoyControls.Where(x => x.CanSavePreset);

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
                        string.Format(
                        "- {0} [vXbox Device #{1}]",
                        ignoredControl.PresetBoxText,
                        ignoredControl.UserIndex));
                }
            }

            output.Message = sb.ToString();

            return output;
        }

        private void CheckForEmergencyHit(KeyPressedEventArgs e)
        {
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
                if (this.ShouldBlockKeyboards)
                {
                    using (var player = new SoundPlayer(Sounds.Disconnected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockKeyboards = false;

                    LogWriter.Write("Emergency activated: Block choosen keyboards unchecked");
                }
                else
                {
                    using (var player = new SoundPlayer(Sounds.Connected))
                    {
                        player.Play();
                    }

                    this.ShouldBlockKeyboards = true;

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
                this.usbWatcher = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartUsbWatcher();
            LogWriter.Write("Main window loaded");
            this.InputMonitorTooltip = "Click to expand/collapse the keyboards input monitor.\r\n" +
                "It will autocollapse after " + this.autoCollapseSpan.TotalSeconds + " seconds to save CPU time.";

            // Getting keyboards count, but removing 1, because None keyboard is always returned as result.
            var realKeyboardsCount = InputManager.GetKeyboards().Count - 1;
            if (realKeyboardsCount <= 0)
            {
                // We have some error. No real keyboard was detected!
                LogWriter.Write("Illegal real keyboards count (" + realKeyboardsCount + "). Terminating application.");
                MessageBox.Show(
                    "No keyboards were detected!\r\nApplication will now close!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(0);
            }

            this.SlotsCount = Math.Min(realKeyboardsCount, 4);
            this.Reset();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            LogWriter.Write("Application is about to close. Checking for unsaved presets...");

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
            LogWriter.Write("Application closed");
        }

        private void InputManagerKeyPressed(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                var args = e as KeyPressedEventArgs;

                if (this.IsInputMonitorExpanded)
                {
                    // Adding the pressed key to the monitor
                    if (this.InputMonitorHistory.Length > 10000)
                    {
                        this.InputMonitorHistory = string.Empty;
                    }

                    this.InputMonitorHistory +=
                        string.Format(
                        "'{0}' on {1} ({2}) [{3}]{4}",
                        args.CorrectedKey,
                        args.Keyboard.StrongName,
                        args.State,
                        Convert.ToInt32(args.Key),
                        Environment.NewLine);
                }

                if (!this.IsEmulationStarted)
                {
                    return;
                }

                this.CheckForEmergencyHit(args);

                EmulationManager.ProcessKeyPress(args, this.ShouldBlockKeyboards);
            });
        }

        private void InputManagerMousePressed(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                EmulationManager.ProcessMousePress(e as MousePressedEventArgs, this.ShouldBlockMouse);
            });
        }

        private void OnSlotsCountChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            LogWriter.Write("Slots count changed to " + this.SlotsCount);

            if (this.SlotsCount == 1)
            {
                this.EmulationInformation = "There is 1 Virtual Xbox Controller mounted into the system.";
                this.EmulationInformation += Environment.NewLine + "To feed it, use the assigned keyboard and/or the mouse.";
            }
            else
            {
                this.EmulationInformation = string.Format(
                    "There are {0} Virtual Xbox Controllers mounted into the system.",
                    this.SlotsCount);
                this.EmulationInformation += Environment.NewLine + "To feed them, use the assigned keyboards and/or the mouse.";
            }

            // checking for unsaved presets
            var unsavedPresets = this.GetUnsavedPresetsData();

            if (unsavedPresets.NotSavedControls.Count > 0)
            {
                var result = System.Windows.MessageBox.Show(
                    "Do you want to save the following unsaved presets,\r\n" +
                    "before you change the slots count?\r\n" + unsavedPresets.Message,
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
            this.IsInputMonitorExpanded = false;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.autoCollapseTimer.Start();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.autoCollapseTimer.Stop();
            this.InputMonitorHistory = string.Empty;
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
                Process.Start("https://www.microsoft.com/accessories/en-us/products/gaming/xbox-360-controller-for-windows/52a-00004#techspecs-connect");
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
            if (!this.IsEmulationStarted)
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
            foreach (var joyControl in EmulationManager.JoyControls)
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

        private object usbLockObject = new object();

        private void OnUsbPlugUnplug(object sender, EventArrivedEventArgs e)
        {
            lock (this.usbLockObject)
            {
                if (this.disposed || EmulationManager.JoyControls == null)
                {
                    return;
                }

                Dispatcher.Invoke((Action)delegate
                {
                    var newKeyboards = InputManager.GetKeyboards();
                    var newMouses = InputManager.GetMouses();

                    switch (e.NewEvent.ClassPath.ClassName)
                    {
                        case "__InstanceDeletionEvent":
                            {
                                foreach (var joyControl in EmulationManager.JoyControls)
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
                                        continue;
                                    }

                                    if (joyControl.CurrentMouse == null)
                                    {
                                        continue;
                                    }

                                    var joyMouse = this.initialMouses.Find(x => x.StrongName == joyControl.CurrentMouse);
                                    if (newMouses.FirstOrDefault(x => x.IsTheSameAs(joyMouse)) == null)
                                    {
                                        // joycontrol's mouse was unplugged
                                        joyControl.Invalidate(SlotInvalidationReason.Mouse_Unplugged);
                                        continue;
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

        private void OnStartEmulationRequested(object parameter)
        {
            try
            {
                EmulationManager.Start();

                this.InputMonitorHistory = string.Empty;
                this.IsEmulationStarted = true;
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

        private void OnStopEmulationRequested(object parameter)
        {
            EmulationManager.Stop();

            this.IsEmulationStarted = false;
            this.emergencyHitDownCount = 0;
        }
    }
}