namespace KeyboardSplitter.Exceptions.Gamepad
{
    using System;

    [Serializable]
    public class GamepadExceptionBase : KeyboardSplitterExceptionBase
    {
        public GamepadExceptionBase(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}