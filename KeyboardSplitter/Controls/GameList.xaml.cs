namespace KeyboardSplitter.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using KeyboardSplitter.Managers;

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
        }
    }
}
