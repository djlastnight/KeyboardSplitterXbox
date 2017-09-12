namespace KeyboardSplitter.Exceptions.Gamepad
{
    [System.Serializable]
    public class VirtualBusFullException : GamepadExceptionBase
    {
        public VirtualBusFullException(string message)
            : base(message)
        {
        }
    }
}
