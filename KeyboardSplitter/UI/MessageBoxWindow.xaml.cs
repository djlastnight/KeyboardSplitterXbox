namespace KeyboardSplitter.UI
{
    using System;
    using System.Drawing;
    using System.Windows;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Helpers;

    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxWindow : CustomWindow
    {
        public MessageBoxWindow(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
            : this()
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (caption == null)
            {
                throw new ArgumentNullException("caption");
            }

            this.text.Text = message;
            this.text.Loaded += (ss, ee) =>
                {
                    var newHeight = this.text.ActualHeight + 150;
                    this.MinHeight = newHeight;
                    this.MaxHeight = newHeight;
                    this.Height = newHeight;
                };

            this.Title = caption;
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    this.buttonCancel.Visibility = System.Windows.Visibility.Collapsed;
                    this.buttonNo.Visibility = System.Windows.Visibility.Collapsed;
                    this.buttonYes.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    this.buttonNo.Visibility = System.Windows.Visibility.Collapsed;
                    this.buttonYes.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    this.buttonCancel.Visibility = System.Windows.Visibility.Collapsed;
                    this.buttonOK.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    this.buttonOK.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            switch (image)
            {
                case MessageBoxImage.Asterisk:
                    this.icon.Source = SystemIcons.Asterisk.ToImageSource();
                    break;
                case MessageBoxImage.Error:
                    this.icon.Source = SystemIcons.Error.ToImageSource();
                    break;
                case MessageBoxImage.Exclamation:
                    this.icon.Source = SystemIcons.Exclamation.ToImageSource();
                    break;
                case MessageBoxImage.None:
                    this.iconColumn.Width = new GridLength(0);
                    break;
                case MessageBoxImage.Question:
                    this.icon.Source = SystemIcons.Question.ToImageSource();
                    break;
                default:
                    this.iconColumn.Width = new GridLength(0);
                    break;
            }
        }

        private MessageBoxWindow()
        {
            this.InitializeComponent();
        }

        public event EventHandler<MessageBoxResultEventArgs> ButtonClicked;

        private void YesClicked(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.Yes);
        }

        private void NoClicked(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.No);
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.Cancel);
        }

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.OK);
        }

        private void OnButtonClicked(MessageBoxResult result)
        {
            if (this.ButtonClicked != null)
            {
                this.ButtonClicked(this, new MessageBoxResultEventArgs(result));
            }
        }
    }
}
