namespace KeyboardSplitter.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using KeyboardSplitter.Commands;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Presets;
    using KeyboardSplitter.UI;
    using SplitterCore.Input;
    using SplitterCore.Preset;
    using VirtualXbox.Enums;

    /// <summary>
    /// Interaction logic for PresetControl.xaml
    /// </summary>
    public partial class PresetControl : UserControl
    {
        public static readonly DependencyProperty CurrentPresetsProperty =
            DependencyProperty.Register(
            "CurrentPresets",
            typeof(ObservableCollection<Preset>),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PresetProperty =
            DependencyProperty.Register(
            "Preset",
            typeof(IPreset),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsSaveAllowedProperty =
            DependencyProperty.Register(
            "IsSaveAllowed",
            typeof(bool),
            typeof(PresetControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsDeleteAllowedProperty =
            DependencyProperty.Register(
            "IsDeleteAllowed",
            typeof(bool),
            typeof(PresetControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SavePresetCommandProperty =
            DependencyProperty.Register(
            "SavePresetCommand",
            typeof(System.Windows.Input.ICommand),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DeletePresetCommandProperty =
            DependencyProperty.Register(
            "DeletePresetCommand",
            typeof(System.Windows.Input.ICommand),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty AddCustomFunctionCommandProperty =
            DependencyProperty.Register(
            "AddCustomFunctionCommand",
            typeof(System.Windows.Input.ICommand),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty RemoveCustomFunctionCommandProperty =
            DependencyProperty.Register(
            "RemoveCustomFunctionCommand",
            typeof(System.Windows.Input.ICommand),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DetectKeyCommandProperty =
            DependencyProperty.Register(
            "DetectKeyCommand",
            typeof(System.Windows.Input.ICommand),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PresetBoxTextProperty =
            DependencyProperty.Register(
            "PresetBoxText",
            typeof(string),
            typeof(PresetControl),
            new PropertyMetadata(null));

        public PresetControl()
        {
            this.InitializeComponent();

            this.SavePresetCommand = new RelayCommand(this.OnSavePresetRequested);
            this.DeletePresetCommand = new RelayCommand(this.OnDeletePresetRequested);
            this.AddCustomFunctionCommand = new RelayCommand(this.OnAddCustomFunctionRequested);
            this.RemoveCustomFunctionCommand = new RelayCommand(this.OnRemoveCustomFunctionRequested);
            this.DetectKeyCommand = new RelayCommand(this.OnDetectKeyRequested);
        }

        public ObservableCollection<Preset> CurrentPresets
        {
            get { return (ObservableCollection<Preset>)this.GetValue(CurrentPresetsProperty); }
            set { this.SetValue(CurrentPresetsProperty, value); }
        }

        public IPreset Preset
        {
            get { return (IPreset)this.GetValue(PresetProperty); }
            set { this.SetValue(PresetProperty, value); }
        }

        public bool IsSaveAllowed
        {
            get { return (bool)this.GetValue(IsSaveAllowedProperty); }
            set { this.SetValue(IsSaveAllowedProperty, value); }
        }

        public bool IsDeleteAllowed
        {
            get { return (bool)this.GetValue(IsDeleteAllowedProperty); }
            set { this.SetValue(IsDeleteAllowedProperty, value); }
        }

        public System.Windows.Input.ICommand SavePresetCommand
        {
            get { return (System.Windows.Input.ICommand)this.GetValue(SavePresetCommandProperty); }
            set { this.SetValue(SavePresetCommandProperty, value); }
        }

        public System.Windows.Input.ICommand DeletePresetCommand
        {
            get { return (System.Windows.Input.ICommand)this.GetValue(DeletePresetCommandProperty); }
            set { this.SetValue(DeletePresetCommandProperty, value); }
        }

        public System.Windows.Input.ICommand AddCustomFunctionCommand
        {
            get { return (System.Windows.Input.ICommand)this.GetValue(AddCustomFunctionCommandProperty); }
            set { this.SetValue(AddCustomFunctionCommandProperty, value); }
        }

        public System.Windows.Input.ICommand RemoveCustomFunctionCommand
        {
            get { return (System.Windows.Input.ICommand)this.GetValue(RemoveCustomFunctionCommandProperty); }
            set { this.SetValue(RemoveCustomFunctionCommandProperty, value); }
        }

        public System.Windows.Input.ICommand DetectKeyCommand
        {
            get { return (System.Windows.Input.ICommand)this.GetValue(DetectKeyCommandProperty); }
            set { this.SetValue(DetectKeyCommandProperty, value); }
        }

        public string PresetBoxText
        {
            get { return (string)this.GetValue(PresetBoxTextProperty); }
            set { this.SetValue(PresetBoxTextProperty, value); }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CurrentPresets = PresetDataManager.CurrentPresets;
        }

        private void OnSavePresetRequested(object parameter)
        {
            var name = this.PresetBoxText;

            if (string.IsNullOrWhiteSpace(name) || this.IsExsistingPreset(name))
            {
                throw new InvalidOperationException(
                    "It is not allowed to save preset with blank or exsisting name!");
            }

            var preset = new Preset();
            preset.Name = name;
            preset.Reset();
            this.AddGlobalPreset(preset);
            this.Preset = preset;
            this.IsSaveAllowed = false;
            this.IsDeleteAllowed = true;
        }

        private void OnDeletePresetRequested(object parameter)
        {
            if (this.Preset == null)
            {
                return;
            }

            var confirm = Controls.MessageBox.Show(
                string.Format("Are you sure that you want to delete '{0}' preset?", this.Preset.Name),
                "Confirm preset delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                this.DeleteGlobalPreset(this.Preset as Preset);
                this.IsDeleteAllowed = false;
            }
        }

        private void OnAddCustomFunctionRequested(object parameter)
        {
            if (this.Preset == null)
            {
                return;
            }

            var newPreset = new PresetCustom((uint)XboxCustomFunction.Button_A, InputKey.None);
            this.Preset.CustomFunctions.Add(newPreset);

            // Auto scrolling to end
            var scrollViewer = Helpers.ParentFinder.FindParent<ScrollViewer>(this);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToEnd();
            }
        }

        private void OnRemoveCustomFunctionRequested(object parameter)
        {
            var customFunction = parameter as PresetCustom;
            if (customFunction != null)
            {
                this.Preset.CustomFunctions.Remove(customFunction);
            }
        }

        private void OnDetectKeyRequested(object parameter)
        {
            var presetElement = parameter as IPresetElement;
            if (presetElement == null)
            {
                return;
            }

            var slot = Helpers.ParentFinder.FindParent<EmulationSlot>(this);
            if (slot == null)
            {
                return;
            }

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var splitter = mainWindow.Splitter;
            if (splitter == null)
            {
                return;
            }

            bool isKeyboardSet = slot.Keyboard != null && slot.Keyboard != Keyboard.None;
            bool isMouseSet = slot.Mouse != null && slot.Mouse != Mouse.None;
            if (!isKeyboardSet && !isMouseSet)
            {
                Controls.MessageBox.Show(
                    "You can not detect an input key, because the slot is not assosiated with any valid input device!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Hand);

                return;
            }

            InputDetectorDeviceFilter filter = InputDetectorDeviceFilter.KeyboardAndMouse;
            if (isKeyboardSet && !isMouseSet)
            {
                filter = InputDetectorDeviceFilter.KeyboardOnly;
            }

            if (isMouseSet && !isKeyboardSet)
            {
                filter = InputDetectorDeviceFilter.MouseOnly;
            }

            var detector = new InputDetectorWindow(splitter.InputManager, filter, presetElement, slot);
            detector.Owner = mainWindow;
            detector.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InputKey newKey = InputKey.None;
            detector.InputDetected += (ss, ee) =>
                {
                    newKey = ee.Key;
                    detector.Close();
                };

            detector.ShowDialog();
            if (newKey == InputKey.None)
            {
                return;
            }

            int index = 0;
            switch (presetElement.FunctionType)
            {
                case SplitterCore.FunctionType.Button:
                    index = this.Preset.Buttons.IndexOf(presetElement as PresetButton);
                    this.Preset.Buttons[index] = new PresetButton(this.Preset.Buttons[index].Button, newKey);
                    break;
                case SplitterCore.FunctionType.Trigger:
                    index = this.Preset.Triggers.IndexOf(presetElement as PresetTrigger);
                    this.Preset.Triggers[index] = new PresetTrigger(this.Preset.Triggers[index].Trigger, newKey);
                    break;
                case SplitterCore.FunctionType.Axis:
                    index = this.Preset.Axes.IndexOf(presetElement as PresetAxis);
                    this.Preset.Axes[index] = new PresetAxis(this.Preset.Axes[index].Axis, this.Preset.Axes[index].Value, newKey);
                    break;
                case SplitterCore.FunctionType.Dpad:
                    index = this.Preset.Dpads.IndexOf(presetElement as PresetDpad);
                    this.Preset.Dpads[index] = new PresetDpad(this.Preset.Dpads[index].Direction, newKey);
                    break;
                case SplitterCore.FunctionType.Custom:
                    index = this.Preset.CustomFunctions.IndexOf(presetElement as PresetCustom);
                    this.Preset.CustomFunctions[index] = new PresetCustom(this.Preset.CustomFunctions[index].Function, newKey);
                    break;
                default:
                    break;
            }
        }

        private void OnPresetSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Preset == null)
            {
                return;
            }

            bool isProtected = this.IsProtectedPreset(this.Preset.Name);
            this.IsSaveAllowed = !isProtected;
            this.IsDeleteAllowed = !isProtected;
        }

        private void OnPresetTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsSaveAllowed = false;
            string name = this.PresetBoxText;

            if (string.IsNullOrWhiteSpace(name))
            {
                this.Preset = null;
                return;
            }

            if (this.IsExsistingPreset(name))
            {
                this.Preset = this.GetPreset(name);
                return;
            }

            this.IsSaveAllowed = true;
            this.IsDeleteAllowed = false;
        }

        //// PresetDataManager Wrap Functions

        private Preset GetPreset(string name)
        {
            return PresetDataManager.CurrentPresets.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
        }

        private bool IsProtectedPreset(string presetName)
        {
            return PresetDataManager.IsProtectedPreset(presetName);
        }

        private bool IsExsistingPreset(string presetName)
        {
            return this.GetPreset(presetName) != null;
        }

        private void AddGlobalPreset(Preset preset)
        {
            PresetDataManager.AddNewPreset(preset);
        }

        private void DeleteGlobalPreset(Preset preset)
        {
            PresetDataManager.DeletePreset(preset);
        }
    }
}