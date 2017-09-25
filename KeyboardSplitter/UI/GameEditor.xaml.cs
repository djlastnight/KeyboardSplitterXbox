namespace KeyboardSplitter.UI
{
    /// <summary>
    /// Interaction logic for GameEditor.xaml
    /// </summary>
    public partial class GameEditor : CustomWindow
    {
        public GameEditor()
        {
            this.InitializeComponent();
        }

        private void OnAddGameClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var editor = new GameItemEditor();
            editor.ShowDialog();
        }
    }
}
