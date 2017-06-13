namespace KeyboardSplitter.Detectors
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using Interceptor;
    using KeyboardSplitter.Managers;

    public partial class KeyDetector : Window, IDisposable
    {
        private readonly string currentKeyboard;

        private bool disposed;

        public KeyDetector(string functionText, string keyboardStrongName = null)
            : this()
        {
            this.functionTextBlock.Text = functionText;
            this.currentKeyboard = keyboardStrongName;

            if (this.currentKeyboard != null)
            {
                this.Title = string.Format(
                    "Waiting for {0} input", this.currentKeyboard);
            }

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                this.Owner = Application.Current.MainWindow;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            InputManager.KeyPressed += this.KeyboardManager_KeyPressed;
        }

        private KeyDetector()
        {
            this.InitializeComponent();
        }

        public event EventHandler KeyDetected;

        public void Dispose()
        {
            if (!this.disposed)
            {
                InputManager.KeyPressed -= this.KeyboardManager_KeyPressed;
                this.KeyDetected = null;
                this.disposed = true;
            }
        }

        private void KeyboardManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (this.currentKeyboard != null &&
                    this.currentKeyboard != e.Keyboard.StrongName)
                {
                    // returning, because we need to
                    // ignore the input from 'foreign' keyboard
                    return;
                }

                this.OnKeyDetected(e);
            });
        }

        private void OnKeyDetected(KeyPressedEventArgs e)
        {
            if (this.KeyDetected != null)
            {
                this.KeyDetected(this, e);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
