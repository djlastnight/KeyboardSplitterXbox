namespace KeyboardSplitter.Exceptions
{
    using System;

    [Serializable]
    public class KeyboardSplitterExceptionBase : Exception
    {
        public KeyboardSplitterExceptionBase(string message)
            : base(message)
        {
        }
    }
}