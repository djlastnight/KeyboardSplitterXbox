namespace KeyboardSplitter.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Models;
    using KeyboardSplitter.Presets;
    using SplitterCore.Input;

    /// <summary>
    /// Interaction logic for GameItemEditor.xaml
    /// </summary>
    public partial class GameItemEditor : CustomWindow
    {
        public static readonly DependencyProperty KeyboardsProperty =
            DependencyProperty.Register(
            "Keyboards",
            typeof(List<Keyboard>),
            typeof(GameItemEditor),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MiceProperty =
            DependencyProperty.Register(
            "Mice",
            typeof(List<Mouse>),
            typeof(GameItemEditor),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PresetsProperty =
            DependencyProperty.Register(
            "Presets",
            typeof(ObservableCollection<Preset>),
            typeof(GameItemEditor),
            new PropertyMetadata(null));

        private bool creatingNewGame;

        private Game game;

        private Game originalGame;

        public GameItemEditor()
        {
            this.InitializeComponent();
            this.creatingNewGame = true;
            this.Title = "Create new game";
            this.createOrOKButton.Content = "Create";
            this.game = this.DataContext as Game;
            this.game.BlockKeyboards = true;
            this.slotsGroupBox.IsEnabled = false;
            this.titleTextBox.IsEnabled = false;
            this.notesTextBox.IsEnabled = false;
            this.argumentsTextBox.IsEnabled = false;
        }

        public GameItemEditor(Game game)
        {
            this.InitializeComponent();
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            this.game = game;
            this.originalGame = new Game(game.GameTitle, game.GamePath, game.Arguments, game.GameNotes, game.BlockKeyboards, game.BlockMice, game.SlotsData);

            this.creatingNewGame = false;
            this.Title = "Edit game";
            this.DataContext = game;
            this.createOrOKButton.Content = "OK";
        }

        public List<Keyboard> Keyboards
        {
            get { return (List<Keyboard>)this.GetValue(KeyboardsProperty); }
            set { this.SetValue(KeyboardsProperty, value); }
        }

        public List<Mouse> Mice
        {
            get { return (List<Mouse>)this.GetValue(MiceProperty); }
            set { this.SetValue(MiceProperty, value); }
        }

        public ObservableCollection<Preset> Presets
        {
            get { return (ObservableCollection<Preset>)this.GetValue(PresetsProperty); }
            set { this.SetValue(PresetsProperty, value); }
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            var game = this.DataContext as Game;
            if (game != null && game.GamePath != null)
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(game.GamePath);
                dialog.FileName = System.IO.Path.GetFileName(game.GamePath);
            }

            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.exe";
            dialog.DereferenceLinks = true;
            dialog.Filter = "Executable file (*.exe)|*.exe";
            dialog.Multiselect = false;
            dialog.Title = "Choose game or application";
            dialog.ValidateNames = true;
            Interceptor.Interception.DisableMouseEvents = true;
            var result = dialog.ShowDialog();
            Interceptor.Interception.DisableMouseEvents = false;
            if (result == true)
            {
                var ext = System.IO.Path.GetExtension(dialog.FileName);
                if (ext.ToLower() != ".exe")
                {
                    Controls.MessageBox.Show(
                        "You must provide an exe file!",
                        ApplicationInfo.AppName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                this.titleTextBox.IsEnabled = true;
                this.notesTextBox.IsEnabled = true;
                this.argumentsTextBox.IsEnabled = true;
                game.GameTitle = null;
                game.GameNotes = null;
                game.GamePath = dialog.FileName;
                this.slotsGroupBox.IsEnabled = true;
            }
        }

        private void OnCreateOrOKButtonClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.game.GameTitle))
            {
                Controls.MessageBox.Show(
                    "Can not save the game, because it has no title!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (this.creatingNewGame)
            {
                this.game.UpdateStatus();

                if (this.game.Status == Enums.GameStatus.OK)
                {
                    GameDataManager.Games.Add(this.game);
                    this.Close();
                }
                else
                {
                    if (this.game.GamePath == null)
                    {
                        Controls.MessageBox.Show(
                            "Can not create the game, because no exe file is choosen!",
                            ApplicationInfo.AppName,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        return;
                    }

                    var errorMessage = new Converters.GameStatusToStringConverter().Convert(this.game.Status, typeof(string), null, null);
                    Controls.MessageBox.Show(
                        "Can not create the game, because of the following error: " + errorMessage,
                        ApplicationInfo.AppName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                // No need to save anything, because the binding done this already
                this.game.UpdateStatus();
                this.Close();
            }
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!this.creatingNewGame)
            {
                var game = this.DataContext as Game;
                game.GameTitle = this.originalGame.GameTitle;
                game.GameNotes = this.originalGame.GameNotes;
                game.GamePath = this.originalGame.GamePath;
            }

            this.Close();
        }

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.game.SlotsData == null)
            {
                this.game.SlotsData = new ObservableCollection<SlotData>();
            }

            if (this.game.SlotsData.Count == 0)
            {
                this.game.SlotsData.Add(new SlotData(1, 1, Keyboard.None, Mouse.None, Preset.Default.Name));
            }

            var inputManager = Helpers.SplitterHelper.TryFindSplitter().InputManager;
            this.Keyboards = inputManager.Keyboards;
            this.Mice = inputManager.Mice;
            this.Presets = PresetDataManager.CurrentPresets;
        }

        private void AddSlotButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.game.SlotsData.Count >= 4)
            {
                Controls.MessageBox.Show(
                    "You can not add more than 4 slots!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            uint slotNumber = 1;
            var assignedSlotNumbers = this.game.SlotsData.Select(x => x.SlotNumber).ToList();

            for (uint i = 1; i <= 4; i++)
            {
                if (!assignedSlotNumbers.Contains(i))
                {
                    slotNumber = i;
                    break;
                }
            }

            this.game.SlotsData.Add(new SlotData(slotNumber, slotNumber, Keyboard.None, Mouse.None, Preset.Default.Name));
        }

        private void OnSlotRemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var data = button.Tag as SlotData;
            if (data != null)
            {
                this.game.SlotsData.Remove(data);
            }
        }
    }
}
