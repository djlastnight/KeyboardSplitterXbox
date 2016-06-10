namespace KeyboardSplitter.Exceptions
{
    using System;

    [Serializable]
    public class SlotsCountMismatchException : EmulationManagerException
    {
        public SlotsCountMismatchException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}