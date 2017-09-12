namespace KeyboardSplitter.Exceptions.Emulation
{
    using System;

    [Serializable]
    public class SlotInvalidatedException : EmulationExceptionBase
    {
        public SlotInvalidatedException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}