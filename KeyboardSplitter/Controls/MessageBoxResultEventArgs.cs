namespace KeyboardSplitter.Controls
{
    using System;
    using System.Windows;

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
