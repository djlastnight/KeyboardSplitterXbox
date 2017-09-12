namespace KeyboardSplitter.Helpers
{
    using SplitterCore;

    public static class SplitterHelper
    {
        public static ISplitter TryFindSplitter()
        {
            if (App.Current != null && App.Current.MainWindow != null)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                if (mainWindow != null && mainWindow.Splitter != null)
                {
                    return mainWindow.Splitter;
                }
            }

            return null;
        }
    }
}
