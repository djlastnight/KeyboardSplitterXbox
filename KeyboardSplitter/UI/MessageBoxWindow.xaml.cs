using KeyboardSplitter.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KeyboardSplitter.UI
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxWindow : CustomWindow
    {
        private MessageBoxWindow()
        {
            this.InitializeComponent();
        }

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
        }

        public event EventHandler<MessageBoxResultEventArgs> ButtonClicked;

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.Yes);
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.No);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.OnButtonClicked(MessageBoxResult.Cancel);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
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
