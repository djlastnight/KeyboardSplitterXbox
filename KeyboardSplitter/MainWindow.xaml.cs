namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using KeyboardSplitter.Commands;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Models;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.UI;
    using SplitterCore;
    using SplitterCore.Preset;

    public partial class MainWindow : CustomWindow, IDisposable
    {
        public static readonly DependencyProperty SplitterProperty =
            DependencyProperty.Register(
            "Splitter",
            typeof(ISplitter),
            typeof(MainWindow),
            new PropertyMetadata(null));

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

        private ICommand startEmulationCommand;

        private ICommand stopEmulationCommand;

        private bool isControllersTestActive;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = ApplicationInfo.AppNameVersion;
            this.autoCollapseTimer = new DispatcherTimer();
            this.autoCollapseTimer.Interval = this.autoCollapseSpan;
            this.autoCollapseTimer.Tick += new EventHandler(this.AutoCollapseTimer_Tick);
        }

        public ISplitter Splitter
        {
            get { return (ISplitter)this.GetValue(SplitterProperty); }
            set { this.SetValue(SplitterProperty, value); }
        }

        public int SlotsCount
        {
            get { return (int)this.GetValue(SlotsCountProperty); }
            set { this.SetValue(SlotsCountProperty, value); }
        }

        public IEnumerable<int> SlotsCountItemsSource
        {
            get { return (IEnumerable<int>)this.GetValue(SlotsCountItemsSourceProperty); }
            set { this.SetValue(SlotsCountItemsSourceProperty, value); }
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

        public ICommand StartEmulationCommand
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

        public ICommand StopEmulationCommand
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
                Thread.Sleep(100);
                this.Splitter.Destroy();
                try
                {
                    PresetDataManager.WritePresetDataToFile();
                }
                catch (Exception e)
                {
                    LogWriter.Write("Preset save failed! Exception details: " + Environment.NewLine + e);
                }

                try
                {
                    GameDataManager.WriteGameDataToFile();
                }
                catch (Exception e)
                {
                    LogWriter.Write("Game data save failed! Exception details: " + Environment.NewLine + e.ToString());
                }

                LogWriter.Write("Main window disposed");
            }
        }

        private void OnStartEmulationRequested(object parameter)
        {
            if (this.Splitter == null)
            {
                return;
            }

            try
            {
                this.Splitter.EmulationManager.Start();
                this.Splitter.InputManager.ClearInputMonitorHistory();
            }
            catch (KeyboardSplitterExceptionBase ex)
            {
                Controls.MessageBox.Show(
                    ex.Message,
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnStopEmulationRequested(object parameter)
        {
            if (this.Splitter != null)
            {
                this.Splitter.EmulationManager.Stop();
            }
        }

        private void FadeInEmulationInformation()
        {
            this.helperGrid.Opacity = 0;
            this.helperGrid.BeginAnimation(Grid.OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1))));
            this.helperGrid.IsHitTestVisible = true;
        }

        private void FadeOutEmulationInformation()
        {
            this.helperGrid.Opacity = 1;
            this.helperGrid.BeginAnimation(Grid.OpacityProperty, new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1))));
            this.helperGrid.IsHitTestVisible = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalSettings.TryApplySettings();
            LogWriter.Write("Main window loaded");
            this.InputMonitorTooltip = "Click to expand/collapse the input device monitor.\r\n" +
                "It will autocollapse after " + this.autoCollapseSpan.TotalSeconds + " seconds to save CPU time.";

            XinputWrapper.XinputController.StartPolling();

            var inputDevices = KeyboardSplitter.Managers.InputManager.ConnectedInputDevices;
            if (inputDevices.Count == 0)
            {
                // We have some error or nothing is attached to the system.
                LogWriter.Write("No input devices were detected! Terminating application.");
                Controls.MessageBox.Show(
                    "No input devices were detected!\r\nApplication will now close!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(0);
            }

            if (!GlobalSettings.Instance.SuggestInputDevicesForNewSlots)
            {
                this.SlotsCount = 1;
            }
            else
            {
                var keyboardsCount = inputDevices.Where(x => x.IsKeyboard).Count();
                var miceCount = inputDevices.Count - keyboardsCount;

                this.SlotsCount = Math.Min(Math.Max(keyboardsCount, miceCount), 4);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            LogWriter.Write("Application is about to close. Checking for unsaved presets...");

            // checking for unsaved presets
            var unsavedPresets = new List<IPreset>();
            foreach (var preset in PresetDataManager.CurrentPresets)
            {
                if (PresetDataManager.IsPresetChanged(preset))
                {
                    unsavedPresets.Add(preset);
                }
            }

            string message = string.Empty;
            foreach (var preset in unsavedPresets)
            {
                message += string.Format("Preset '{0}'{1}", preset.Name, Environment.NewLine);
            }

            if (message.Length > 0)
            {
                var result = Controls.MessageBox.Show(
                    "Do you want to save the following unsaved presets, before you quit?\r\n\r\n" + message,
                    "You are about to quit " + ApplicationInfo.AppNameVersion,
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // save the presets
                    PresetDataManager.WritePresetDataToFile();
                }

                e.Cancel = result == MessageBoxResult.Cancel;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
            XinputWrapper.XinputController.StopPolling();
            GlobalSettings.TrySaveToFile();
        }

        private void OnSlotsCountChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!this.IsActive || (sender as System.Windows.Controls.ComboBox).SelectedIndex == -1)
            {
                return;
            }

            // Creating new splitter
            if (this.Splitter == null)
            {
                this.Splitter = new Splitter(this.SlotsCount);
                this.Splitter.EmulationManager.EmulationStarted += this.EmulationManager_EmulationStarted;
                this.Splitter.EmulationManager.EmulationStopped += this.EmulationManager_EmulationStopped;
            }
            else
            {
                this.Splitter.EmulationManager.ChangeSlotsCountBy(this.SlotsCount - this.Splitter.EmulationManager.Slots.Count);
            }

            if (this.SizeToContent != System.Windows.SizeToContent.Width)
            {
                // Autosizing the main window
                int screenWidth = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).Bounds.Width;
                if (screenWidth >= 1280 && this.WindowState != WindowState.Maximized)
                {
                    this.SizeToContent = SizeToContent.Width;
                }
            }

            // Preparing the emulation information
            if (this.SlotsCount == 1)
            {
                this.EmulationInformation = "There is 1 Virtual Xbox 360 Controller mounted into the system.";
                this.EmulationInformation += Environment.NewLine + "To feed it, use the assigned keyboard/mouse.";
            }
            else
            {
                this.EmulationInformation = string.Format(
                    "There are {0} Virtual Xbox 360 Controllers mounted into the system.",
                    this.SlotsCount);
                this.EmulationInformation += Environment.NewLine + "To feed them, use the assigned keyboards/mice.";
            }
        }

        private void EmulationManager_EmulationStarted(object sender, EventArgs e)
        {
            if (this.Splitter.EmulationManager.Slots.Any(x => x.Keyboard == SplitterCore.Input.Keyboard.None && x.Mouse == SplitterCore.Input.Mouse.None))
            {
                // We have mouse click feeder active
                return;
            }

            this.FadeInEmulationInformation();
        }

        private void EmulationManager_EmulationStopped(object sender, EventArgs e)
        {
            if (this.Splitter.EmulationManager.Slots.Any(x => x.Keyboard == SplitterCore.Input.Keyboard.None && x.Mouse == SplitterCore.Input.Mouse.None))
            {
                // We have mouse click feeder active
                return;
            }

            this.FadeOutEmulationInformation();
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
            this.Splitter.InputManager.ClearInputMonitorHistory();
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
                Controls.MessageBox.Show(
                    "Can not open gamepad properties: " + Environment.NewLine + ex.Message,
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenXboxSite(object sender, RoutedEventArgs e)
        {
            if (XboxGamepad.AreXboxAccessoriesInstalled)
            {
                Controls.MessageBox.Show(
                    "Xbox accessories is already installed on your computer!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            try
            {
                Process.Start("https://www.microsoft.com/accessories/en-gb/d/xbox-360-controller-for-windows");
            }
            catch (Exception)
            {
            }
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow dialog = new AboutWindow("About " + ApplicationInfo.AppNameVersion);
            dialog.ShowDialog();
        }

        private void UninstallBuiltInDrivers_Click(object sender, RoutedEventArgs e)
        {
            var result = Controls.MessageBox.Show(
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
            if (this.isControllersTestActive)
            {
                Controls.MessageBox.Show(
                    "Xinput controller test window is already open!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            XinputControllerTestWindow window = new XinputControllerTestWindow(this);
            window.Closed += (oo, ss) =>
            {
                this.isControllersTestActive = false;
            };

            this.isControllersTestActive = true;
            window.Show();
        }

        private void HelpContents_Click(object sender, RoutedEventArgs e)
        {
            HelpDialog howToUse = new HelpDialog();
            howToUse.ShowDialog();
        }

        private void HowItWorks_Click(object sender, RoutedEventArgs e)
        {
            HowItWorksWindow how = new HowItWorksWindow();
            how.ShowDialog();
        }

        private void FAQ_Click(object sender, RoutedEventArgs e)
        {
            FaqWindow fac = new FaqWindow();
            fac.ShowDialog();
        }

        private void OptionsClicked(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            settings.Owner = this;
            settings.ShowDialog();
        }

        private void OnHelperGridCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.FadeOutEmulationInformation();
        }

        private void OnControllerSubtypesClicked(object sender, RoutedEventArgs e)
        {
            var wind = new XinputSubTypesWindow();
            wind.ShowDialog();
        }

        private void OnEditGamesListClicked(object sender, RoutedEventArgs e)
        {
            var gameEditor = new GameEditor();
            gameEditor.ShowDialog();
        }

        private void GamesSubmenuOpened(object sender, RoutedEventArgs e)
        {
            this.playGameMenuItem.Items.Clear();

            foreach (var game in GameDataManager.Games)
            {
                game.GameAboutToStart -= this.OnGameAboutToStart;

                if (game.Status == Enums.GameStatus.OK)
                {
                    var newItem = new MenuItem() { Header = game.GameTitle };
                    newItem.Click += (ss, ee) => { game.Play(); };
                    newItem.Icon = new Image() { Source = game.GameIcon, Width = 20, Height = 20 };
                    game.GameAboutToStart += this.OnGameAboutToStart;
                    this.playGameMenuItem.Items.Add(newItem);
                }
            }
        }

        private bool OnGameAboutToStart(Game game)
        {
            if (this.Splitter.EmulationManager.IsEmulationStarted)
            {
                Controls.MessageBox.Show("You must stop the emulation, before starting a game!", ApplicationInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (game == null)
            {
                return false;
            }

            if (game.Status != Enums.GameStatus.OK)
            {
                return false;
            }

            var slots = new ObservableCollection<SplitterCore.Emulation.IEmulationSlot>();
            foreach (var slotData in game.SlotsData)
            {
                var keyboard = SplitterCore.Input.Keyboard.None;
                var mouse = SplitterCore.Input.Mouse.None;

                if (slotData.KeyboardHardwareId != string.Empty)
                {
                    keyboard = this.Splitter.InputManager.Keyboards.Find(x => x.HardwareID == slotData.KeyboardHardwareId);
                }

                if (slotData.MouseHardwareId != string.Empty)
                {
                    mouse = this.Splitter.InputManager.Mice.Find(x => x.HardwareID == slotData.MouseHardwareId);
                }

                var preset = PresetDataManager.CurrentPresets.First(x => x.Name == slotData.PresetName);
                slots.Add(new EmulationSlot(slotData.SlotNumber, new XboxGamepad(slotData.GamepadUserIndex), keyboard, mouse, preset));
            }

            var inputManager = this.Splitter.InputManager;
            var emulationManager = new EmulationManager(slots);
            this.Splitter = new Splitter(inputManager, emulationManager);
            this.Splitter.EmulationManager.Start(true);

            return true;
        }
    }
}