namespace KeyboardSplitter
{
    using System;

    public delegate void KeyGestureChangedHandler(object sender, KeyGestureChangedEventArgs e);

    public class KeyGestureChangedEventArgs : EventArgs
    {
        public KeyGestureChangedEventArgs(string newKey)
        {
            this.NewKey = newKey;
        }

        public string NewKey
        {
            get;
            private set;
        }
    }
}