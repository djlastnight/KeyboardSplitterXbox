namespace KeyboardSplitter.Exceptions.Gamepad
{
    using System;

    [Serializable]
    public class VirtualBusNotInstalledException : GamepadExceptionBase
    {
        public VirtualBusNotInstalledException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}