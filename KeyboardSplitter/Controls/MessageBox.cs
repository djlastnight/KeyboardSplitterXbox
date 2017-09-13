using KeyboardSplitter.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KeyboardSplitter.Controls
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            var box = new MessageBoxWindow(message, caption, buttons, image);
            var result = MessageBoxResult.None;
            box.ButtonClicked += (ss, ee) =>
            {
                result = ee.MessageBoxResult;
                box.Close();
            };

            box.ShowDialog();
            return result;
        }
    }
}
