namespace XinputWrapper
{
    using System;
    using XinputWrapper.Structs;

    public class XinputControllerStateChangedEventArgs : EventArgs
    {
        public XinputController Controller { get; internal set; }

        public XInputState CurrentInputState { get; internal set; }

        public XInputState PreviousInputState { get; internal set; }
    }
}