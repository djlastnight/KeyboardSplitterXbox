namespace KeyboardSplitter.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Models;
    using KeyboardSplitter.UI;

    /// <summary>
    /// Interaction logic for GameList.xaml
    /// </summary>
    public partial class GameList : UserControl
    {
        public GameList()
        {
            this.InitializeComponent();
        }

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            this.listView.ItemsSource = GameDataManager.Games;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(this.listView.ItemsSource);
            view.Filter = this.Filter;
        }

        private void OnFilterTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(this.listView.ItemsSource);
            if (view != null)
            {
                view.Refresh();
            }
        }

        private bool Filter(object item)
        {
            if (string.IsNullOrEmpty(this.filterTextBox.Text))
            {
                return true;
            }

            var game = item as Game;
            if (game != null)
            {
                return game.GameTitle.IndexOf(this.filterTextBox.Text, StringComparison.OrdinalIgnoreCase) != -1;
            }

            return true;
        }

        private void OnEditButtonClicked(object sender, RoutedEventArgs e)
        {
            var game = (sender as Button).Tag as Game;
            if (game == null)
            {
                return;
            }

            var editor = new GameItemEditor(game);
            editor.ShowDialog();
        }

        private void OnPlayButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var game = button.Tag as Game;
            if (game == null)
            {
                throw new InvalidOperationException("Play button tag must be a game object!");
            }

            try
            {
                game.TryStart();
                var parent = Helpers.ParentFinder.FindParent<GameEditor>(this);
                if (parent != null)
                {
                    parent.Close();
                }
            }
            catch (Exception ex)
            {
                Controls.MessageBox.Show(
                    ex.Message,
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var game = button.Tag as Game;
            if (game == null)
            {
                throw new InvalidOperationException("Remove button tag must be a game object!");
            }

            var result = Controls.MessageBox.Show(
                "Are you sure that you want to remove '" + game.GameTitle + "'?",
                "Confirm game remove",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                GameDataManager.Games.Remove(game);
            }
        }
    }
}
