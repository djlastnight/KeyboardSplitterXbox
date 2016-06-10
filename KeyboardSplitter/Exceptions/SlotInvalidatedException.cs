namespace KeyboardSplitter.Exceptions
{
    using System;

    [Serializable]
    public class SlotInvalidatedException : EmulationManagerException
    {
        public SlotInvalidatedException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}