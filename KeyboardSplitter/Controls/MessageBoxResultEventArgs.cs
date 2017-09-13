using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KeyboardSplitter.Controls
{
    public class MessageBoxResultEventArgs : EventArgs
    {
        public MessageBoxResultEventArgs(MessageBoxResult result)
        {
            this.MessageBoxResult = result;
        }

        public MessageBoxResult MessageBoxResult
        {
            get;
            private set;
        }
    }
}
