namespace KeyboardSplitter.Exceptions.Gamepad
{
    [System.Serializable]
    public class GamepadOwnedException : GamepadExceptionBase
    {
        public GamepadOwnedException(string message)
            : base(message)
        {
        }
    }
}
