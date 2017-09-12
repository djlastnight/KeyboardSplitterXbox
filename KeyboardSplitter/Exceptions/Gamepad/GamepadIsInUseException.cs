namespace KeyboardSplitter.Exceptions.Gamepad
{
    [System.Serializable]
    public class GamepadIsInUseException : GamepadExceptionBase
    {
        public GamepadIsInUseException(string message)
            : base(message)
        {
        }
    }
}
