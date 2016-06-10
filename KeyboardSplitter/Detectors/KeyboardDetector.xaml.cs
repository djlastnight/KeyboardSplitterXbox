namespace KeyboardSplitter.Detectors
{
    using System;
    using System.Windows;
    using Interceptor;
    using KeyboardSplitter.Managers;

    public partial class KeyboardDetector : Window, IDisposable
    {
        private bool disposed;

        public KeyboardDetector()
        {
            this.InitializeComponent();
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                this.Owner = Application.Current.MainWindow;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            KeyboardManager.KeyPressed += this.KeyboardManager_KeyPressed;
        }

        public event EventHandler KeyboardDetected;

        public void Dispose()
        {
            if (!this.disposed)
            {
                KeyboardManager.KeyPressed -= this.KeyboardManager_KeyPressed;
                this.KeyboardDetected = null;
                this.disposed = true;
            }
        }

        private void OnKeyboardDetected(KeyPressedEventArgs e)
        {
            if (this.KeyboardDetected != null)
            {
                this.KeyboardDetected(this, e);
            }
        }

        private void KeyboardManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.OnKeyboardDetected(e);
                this.Close();
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
