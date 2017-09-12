namespace KeyboardSplitter.Exceptions.Gamepad
{
    using System;

    [Serializable]
    public class XboxAccessoriesNotInstalledException : GamepadExceptionBase
    {
        public XboxAccessoriesNotInstalledException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}