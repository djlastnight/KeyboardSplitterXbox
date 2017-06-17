namespace KeyboardSplitter.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Interceptor;
    using KeyboardSplitter.Detectors;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using XboxInterfaceWrap;

    /// <summary>
    /// User control, which interacts with a single
    /// virtual xbox controller.
    /// </summary>
    public partial class JoyControl : UserControl, IDisposable
    {
        private static SolidColorBrush presetOkColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F6CF8A4"));

        private static SolidColorBrush presetErrorColor = new SolidColorBrush(Color.FromArgb(60, 255, 100, 100));

        private uint userIndex;

        private List<KeyControl> keyControls;

        private List<InterceptionKeyboard> keyboards;

        private Button addCustomFunctionButton;

        private KeyControl bindEnterKeyControl;

        private SlotInvalidationReason invalidateReason;

        public JoyControl(uint userIndex)
            : this()
        {
            if (userIndex < 1 || userIndex > 4)
            {
                throw new ArgumentOutOfRangeException(
                    "userIndex must be in range 1-4");
            }

            this.userIndex = userIndex;
            this.slotLabel.Content = string.Format(
                "Slot #{0} - Virtual Xbox 360 Controller #{0}", this.userIndex);

            this.CreateKeyControls();

            if (!XboxBus.IsInstalled())
            {
                this.Invalidate(SlotInvalidationReason.XboxBus_Not_Installed);
            }

            if (VirtualXboxController.Exists(this.userIndex))
            {
                this.Invalidate(SlotInvalidationReason.Controller_In_Use);
            }

            this.presetsBox.SelectedItem = Preset.Default;
        }

        private JoyControl()
        {
            this.InitializeComponent();
            this.InitializePresetsBox();
            this.bindEnterKeyControl = new KeyControl(this, isRemoveable: true)
            {
                CustomFunction = XboxCustomFunction.A,
                KeyGesture = InterceptionKeys.Enter.ToString(),
            };
        }

        public List<KeyControl> Children
        {
            get
            {
                return this.keyControls;
            }
        }

        public uint UserIndex
        {
            get
            {
                return this.userIndex;
            }
        }

        public bool IsInvalidated
        {
            get;
            private set;
        }

        public SlotInvalidationReason InvalidateReason
        {
            get
            {
                return this.invalidateReason;
            }
        }

        public Preset CurrentPreset
        {
            get
            {
                Preset output = null;
                Dispatcher.Invoke((Action)delegate
                {
                    output = this.presetsBox.SelectedItem as Preset;
                });

                return output;
            }

            set
            {
                this.LoadPreset(value);
            }
        }

        public string PresetBoxText
        {
            get
            {
                return this.presetsBox.Text;
            }
        }

        public string CurrentKeyboard
        {
            get
            {
                string output = string.Empty;
                Dispatcher.Invoke((Action)delegate
                {
                    output = this.keyboardDeviceBox.SelectedItem as string;
                });

                return output;
            }

            private set
            {
                this.keyboardDeviceBox.SelectedItem = value;
            }
        }

        public bool CanSavePreset
        {
            get
            {
                return this.savePresetButton.IsEnabled;
            }
        }

        public bool IsOnManualMode
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (this.keyControls != null)
            {
                foreach (var keyControl in this.keyControls)
                {
                    keyControl.KeyGestureChanged -= this.KeyControl_KeyGestureChanged;
                    keyControl.Dispose();
                }

                this.stackPanel.Children.Clear();
                this.keyControls.Clear();
                this.keyControls = null;
            }
        }

        public void SetKeyboard(string keyboardStrongName)
        {
            this.UpdateKeyboardsList();
            var searchedKeyboard = this.keyboards.Find(x => x.StrongName == keyboardStrongName);
            if (searchedKeyboard != null)
            {
                this.CurrentKeyboard = searchedKeyboard.StrongName;
            }
            else
            {
                string errorMessageHint = "Allowed values are in range 'Keyboard_01' to 'Keyboard_10'";
                throw new InvalidOperationException(
                    string.Format("Invalid keyboard strong name: '{0}'. {1}", keyboardStrongName, errorMessageHint));
            }
        }

        public void SaveCurrentPreset(bool silently = false)
        {
            if (presetsBox.SelectedIndex == -1)
            {
                return;
            }

            var newPreset = new Preset();
            newPreset.Name = this.presetsBox.Text;
            var existingPreset = PresetDataManager.Presets.FirstOrDefault(x => x.Name == newPreset.Name);

            if (silently == false)
            {
                string action = existingPreset == null ? "save" : "overwrite";

                string question = string.Format("Are you sure that you want to {0} \"{1}\" preset?", action, newPreset.Name);
                var result = MessageBox.Show(question, "Confirm preset " + action, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            foreach (var keyControl in this.keyControls)
            {
                if (keyControl.IsRemoveable)
                {
                    newPreset.CustomFunctions.Add(new PresetCustom(keyControl.CustomFunction, keyControl.KeyGesture));
                    continue;
                }

                switch (keyControl.ControlType)
                {
                    case KeyControlType.Button:
                        newPreset.Buttons.Add(new PresetButton(keyControl.Button, keyControl.KeyGesture));
                        break;
                    case KeyControlType.Axis:
                        newPreset.Axes.Add(new PresetAxis(keyControl.Axis, keyControl.Position, keyControl.KeyGesture));
                        break;
                    case KeyControlType.Dpad:
                        newPreset.Povs.Add(new PresetDpad(keyControl.DpadDirection, keyControl.KeyGesture));
                        break;
                    case KeyControlType.Trigger:
                        newPreset.Triggers.Add(new PresetTrigger(keyControl.Trigger, keyControl.KeyGesture));
                        break;
                    default:
                        throw new NotImplementedException(
                            "Not implemented key control type: " + keyControl.ControlType);
                }
            }

            if (existingPreset != null)
            {
                // overwrite
                int index = PresetDataManager.Presets.IndexOf(existingPreset);
                PresetDataManager.Presets[index] = newPreset;
                if (!silently)
                {
                    // selecting the preset
                    this.presetsBox.SelectedIndex = index;
                }
            }
            else
            {
                PresetDataManager.Presets.Add(newPreset);
            }
        }

        public void Invalidate(SlotInvalidationReason reason)
        {
            if (this.IsInvalidated)
            {
                return;
            }

            this.invalidateReason = reason;

            Grid invalidateGrid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0))
            };

            string errorMsg;

            switch (reason)
            {
                case SlotInvalidationReason.XboxBus_Not_Installed:
                    errorMsg = "Xbox bus driver is not installed!";
                    break;
                case SlotInvalidationReason.Controller_In_Use:
                    errorMsg = string.Format("Controller #{0} is busy!", this.userIndex);
                    break;
                case SlotInvalidationReason.Keyboard_Unplugged:
                    {
                        errorMsg = this.CurrentKeyboard + " was unplugged!";
                        this.keyboardDeviceBox.SelectedIndex = -1;

                        // unplugging the virtual joystick too
                        VirtualXboxController.UnPlug(this.UserIndex, true);
                    }

                    break;
                case SlotInvalidationReason.Controller_Unplugged:
                    errorMsg = string.Format("Controller #{0} is not plugged-in!", this.userIndex);
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented slot invalidation reason: " + reason);
            }

            Label reasonLabel = new Label()
            {
                Content = errorMsg,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 50, 0, 0),
            };

            Button resetBtn = new Button()
            {
                Content = "Click to reset the slot",
                Width = 150,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 80, 0, 0),
            };

            resetBtn.Click += (oo, ss) =>
            {
                if (this.CanSavePreset)
                {
                    SaveCurrentPreset(silently: false);
                }

                switch (reason)
                {
                    case SlotInvalidationReason.XboxBus_Not_Installed:
                    case SlotInvalidationReason.Controller_In_Use:
                    case SlotInvalidationReason.Controller_Unplugged:
                        {
                            resetBtn.IsEnabled = false;
                            return;
                        }

                    case SlotInvalidationReason.Keyboard_Unplugged:
                        {
                            this.keyboardDeviceBox.SelectedIndex = -1;
                            break;
                        }

                    default:
                        throw new NotImplementedException(
                            "Not implemented slot invalidation reason: " + reason);
                }

                this.mainGrid.Children.Remove(invalidateGrid);
                CreateKeyControls();
                LoadPreset(Preset.Default);
                this.presetsBox.SelectedItem = Preset.Default;
                this.IsInvalidated = false;
            };

            invalidateGrid.Children.Add(reasonLabel);
            invalidateGrid.Children.Add(resetBtn);
            this.mainGrid.Children.Add(invalidateGrid);
            this.IsInvalidated = true;
        }

        public void Lock()
        {
            if (this.IsOnManualMode)
            {
                this.keyboardGrid.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = false;
            }
        }

        public void UnLock()
        {
            this.IsEnabled = true;
            this.keyboardGrid.IsEnabled = true;
        }

        private void InitializePresetsBox()
        {
            this.presetsBox.ItemsSource = PresetDataManager.Presets;
            this.deletePresetButton.IsEnabled = false;
            this.savePresetButton.IsEnabled = false;
        }

        private void CreateKeyControls()
        {
            this.stackPanel.Children.Clear();
            this.stackPanel.Background = Brushes.Transparent;
            this.keyControls = new List<KeyControl>();

            foreach (XboxButton button in Enum.GetValues(typeof(XboxButton)))
            {
                this.keyControls.Add(new KeyControl(this, button));
            }

            foreach (XboxTrigger trigger in Enum.GetValues(typeof(XboxTrigger)))
            {
                this.keyControls.Add(new KeyControl(this, trigger));
            }

            foreach (XboxAxis axis in Enum.GetValues(typeof(XboxAxis)))
            {
                this.keyControls.Add(new KeyControl(this, axis, XboxAxisPosition.Min));
                this.keyControls.Add(new KeyControl(this, axis, XboxAxisPosition.Max));
            }

            foreach (XboxDpadDirection direction in Enum.GetValues(typeof(XboxDpadDirection)))
            {
                if (direction != XboxDpadDirection.None)
                {
                    this.keyControls.Add(new KeyControl(this, direction));
                }
            }

            foreach (var keyControl in this.keyControls)
            {
                keyControl.KeyGestureChanged += this.KeyControl_KeyGestureChanged;
                this.stackPanel.Children.Add(keyControl);
            }

            this.addCustomFunctionButton = new Button() { Content = "Add custom function", Width = this.Width, Height = 25 };
            this.addCustomFunctionButton.Click += new RoutedEventHandler(this.AddBtn_Click);
            this.stackPanel.Children.Add(this.addCustomFunctionButton);
        }

        private void LoadPreset(Preset preset)
        {
            if (preset == null)
            {
                throw new ArgumentNullException("preset");
            }

            if (this.keyControls == null)
            {
                throw new InvalidOperationException(
                    "Can not load preset, because there is no keycontrols created yet!");
            }

            // removing old custom presets if have
            foreach (var keycontrol in this.keyControls)
            {
                if (keycontrol.IsRemoveable)
                {
                    this.stackPanel.Children.Remove(keycontrol);
                }
            }

            this.keyControls.RemoveAll(x => x.IsRemoveable);

            foreach (var keyControl in this.keyControls)
            {
                keyControl.KeyGesture = "None";
                switch (keyControl.ControlType)
                {
                    case KeyControlType.Button:
                        var buttonMatch = preset.Buttons.FirstOrDefault(x => x.Button == keyControl.Button);
                        if (buttonMatch != null)
                        {
                            keyControl.KeyGesture = buttonMatch.KeyboardKey;
                        }

                        break;
                    case KeyControlType.Axis:
                        var axisMatch = preset.Axes.FirstOrDefault(x => x.Axis == keyControl.Axis && x.Position == keyControl.Position);
                        if (axisMatch != null)
                        {
                            keyControl.KeyGesture = axisMatch.KeyboardKey;
                        }

                        break;
                    case KeyControlType.Dpad:
                        var povMatch = preset.Povs.FirstOrDefault(x => x.Direction == keyControl.DpadDirection);
                        if (povMatch != null)
                        {
                            keyControl.KeyGesture = povMatch.KeyboardKey;
                        }

                        break;
                    case KeyControlType.Trigger:
                        {
                            var triggerMatch = preset.Triggers.FirstOrDefault(x => x.Trigger == keyControl.Trigger);
                            if (triggerMatch != null)
                            {
                                keyControl.KeyGesture = triggerMatch.KeyboardKey;
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            // adding custom controls
            foreach (var custom in preset.CustomFunctions)
            {
                var customKeyControl = new KeyControl(this, isRemoveable: true)
                {
                    CustomFunction = custom.Function,
                    KeyGesture = custom.KeyboardKey
                };

                customKeyControl.Removed += this.RemoveableKeyControl_Removed;
                customKeyControl.KeyGestureChanged += this.KeyControl_KeyGestureChanged;
                customKeyControl.FunctionChanged += this.RemoveableKeyControl_FunctionChanged;
                this.keyControls.Add(customKeyControl);
                this.stackPanel.Children.Insert(this.stackPanel.Children.Count - 1, customKeyControl);
            }

            // adding the optional bind xbox button 'A' -> keyboard 'Enter'
            if (this.bindEnterCheckBox.IsChecked == true)
            {
                this.keyControls.Add(this.bindEnterKeyControl);
            }

            this.savePresetButton.IsEnabled = false;
            this.addCustomFunctionButton.IsEnabled = !PresetDataManager.IsProtectedPreset(preset.Name);
            this.presetsBox.SelectedItem = preset;
        }

        private void DeleteCurrentPreset()
        {
            string message = string.Format("Are you sure that you want to delete \"{0}\" preset?", this.presetsBox.Text);
            var result = MessageBox.Show(message, "Confirm delete", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                var presetToDelete = this.presetsBox.SelectedItem as Preset;
                PresetDataManager.Presets.Remove(presetToDelete);
            }
        }

        private void UpdateKeyboardsList()
        {
            this.keyboards = InputManager.GetKeyboards();
            this.keyboardDeviceBox.ItemsSource = this.keyboards.Select(x => x.StrongName);
        }

        private void KeyboardDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.keyboardInfoLabel.Content = "n/a";
            this.bodyGrid.Visibility = System.Windows.Visibility.Visible;
            this.onScreenController.Visibility = System.Windows.Visibility.Hidden;
            this.detectKeyboardButton.IsEnabled = true;
            this.keyboardDeviceBox.IsEnabled = true;
            this.IsOnManualMode = this.keyboardDeviceBox.SelectedIndex == 0;

            if (this.keyboardDeviceBox.SelectedIndex == -1)
            {
                return;
            }

            if (this.IsOnManualMode)
            {
                this.bodyGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.onScreenController.Visibility = System.Windows.Visibility.Visible;
            }

            this.keyboardInfoLabel.Content = this.keyboards[this.keyboardDeviceBox.SelectedIndex].FriendlyName;
        }

        private void PresetsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.presetsBox.SelectedIndex == -1)
            {
                return;
            }

            var preset = this.presetsBox.SelectedItem as Preset;
            if (preset == null)
            {
                throw new NotImplementedException();
            }

            this.deletePresetButton.IsEnabled = !PresetDataManager.IsProtectedPreset(preset.Name);
            this.LoadPreset(preset);
        }

        private void PresetsBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool isProtected = PresetDataManager.IsProtectedPreset(this.presetsBox.Text);
            var preset = this.presetsBox.SelectedItem as Preset;

            savePresetButton.IsEnabled = !isProtected && this.presetsBox.Text.Length > 0;

            deletePresetButton.IsEnabled = !isProtected && preset != null
                && preset.Name.Equals(this.presetsBox.Text, StringComparison.InvariantCultureIgnoreCase)
                && PresetDataManager.Presets.Contains(preset);
        }

        private void DeletePresetButton_Click(object sender, RoutedEventArgs e)
        {
            this.DeleteCurrentPreset();
        }

        private void SavePresetButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveCurrentPreset();
        }

        private void KeyControl_KeyGestureChanged(object sender, KeyGestureChangedEventArgs e)
        {
            if (!PresetDataManager.IsProtectedPreset(presetsBox.Text) && !this.savePresetButton.IsEnabled)
            {
                this.savePresetButton.IsEnabled = true;
            }
        }

        private void DetectKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateKeyboardsList();
            using (var detector = new KeyboardDetector())
            {
                detector.KeyboardDetected += (ss, ee) =>
                    {
                        this.CurrentKeyboard = ((KeyPressedEventArgs)ee).Keyboard.StrongName;
                    };

                this.IsEnabled = false;
                detector.ShowDialog();
                this.IsEnabled = true;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var removeableKeyControl = new KeyControl(this, isRemoveable: true);
            removeableKeyControl.Removed += new EventHandler(this.RemoveableKeyControl_Removed);
            removeableKeyControl.FunctionChanged += new EventHandler(this.RemoveableKeyControl_FunctionChanged);
            removeableKeyControl.KeyGestureChanged += this.KeyControl_KeyGestureChanged;
            this.keyControls.Add(removeableKeyControl);
            this.stackPanel.Children.Insert(this.stackPanel.Children.Count - 1, removeableKeyControl);
            if (!PresetDataManager.IsProtectedPreset(presetsBox.Text) && !this.savePresetButton.IsEnabled)
            {
                this.savePresetButton.IsEnabled = true;
            }
        }

        private void RemoveableKeyControl_FunctionChanged(object sender, EventArgs e)
        {
            if (!PresetDataManager.IsProtectedPreset(presetsBox.Text) && !this.savePresetButton.IsEnabled)
            {
                this.savePresetButton.IsEnabled = true;
            }
        }

        private void RemoveableKeyControl_Removed(object sender, EventArgs e)
        {
            var keyControl = sender as KeyControl;
            this.keyControls.Remove(keyControl);
            this.stackPanel.Children.Remove(keyControl);
            if (!PresetDataManager.IsProtectedPreset(presetsBox.Text) && !this.savePresetButton.IsEnabled)
            {
                this.savePresetButton.IsEnabled = true;
            }
        }

        private void KeyboardDeviceBox_DropDownOpened(object sender, EventArgs e)
        {
            this.UpdateKeyboardsList();
        }

        private void BindEnterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!bindEnterCheckBox.IsLoaded)
            {
                return;
            }

            if (!this.keyControls.Contains(this.bindEnterKeyControl))
            {
                this.keyControls.Add(this.bindEnterKeyControl);
            }
        }

        private void BindEnterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.bindEnterCheckBox.IsLoaded)
            {
                return;
            }

            if (this.keyControls.Contains(this.bindEnterKeyControl))
            {
                this.keyControls.Remove(this.bindEnterKeyControl);
            }
        }

        private void SavePresetButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.presetsBox != null)
            {
                this.presetsBox.BorderThickness = e.NewValue.Equals(true) ? new Thickness(1) : new Thickness(0.3);
                this.presetsBox.BorderBrush = e.NewValue.Equals(true) ? Brushes.Blue : Brushes.Black;
            }
        }
    }
}