namespace KeyboardSplitter
{
    using System;
    using SplitterCore.Input;

    public delegate void KeyGestureChangedHandler(object sender, KeyGestureChangedEventArgs e);

    public class KeyGestureChangedEventArgs : EventArgs
    {
        public KeyGestureChangedEventArgs(InputKey newKey)
        {
            this.NewKey = newKey;
        }

        public InputKey NewKey
        {
            get;
            private set;
        }
    }
}