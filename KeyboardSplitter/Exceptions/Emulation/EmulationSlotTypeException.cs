namespace KeyboardSplitter.Exceptions.Emulation
{
    [System.Serializable]
    public class EmulationSlotTypeException : EmulationExceptionBase
    {
        public EmulationSlotTypeException(string message)
            : base(message)
        {
        }
    }
}