namespace KeyboardSplitter.Exceptions.Gamepad
{
    using System;

    [Serializable]
    public class XinputSlotsFullException : GamepadExceptionBase
    {
        public XinputSlotsFullException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}