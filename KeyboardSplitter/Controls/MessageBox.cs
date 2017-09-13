namespace KeyboardSplitter.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using KeyboardSplitter.UI;

    public static class MessageBox
    {
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            var box = new MessageBoxWindow(message, caption, buttons, image);
            box.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var result = MessageBoxResult.Cancel;

            box.Activated += (ss, ee) =>
                {
                    switch (buttons)
                    {
                        case MessageBoxButton.OK:
                            result = MessageBoxResult.OK;
                            break;
                        case MessageBoxButton.OKCancel:
                            break;
                        case MessageBoxButton.YesNo:
                            box.CustomTitlebar.EnableCloseButton = false;
                            break;
                        case MessageBoxButton.YesNoCancel:
                            break;
                        default:
                            break;
                    }
                };

            box.ButtonClicked += (ss, ee) =>
            {
                result = ee.MessageBoxResult;
                box.Close();
            };

            switch (image)
            {
                case MessageBoxImage.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case MessageBoxImage.Exclamation:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxImage.Hand:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
                default:
                    break;
            }

            box.ShowDialog();

            return result;
        }
    }
}
