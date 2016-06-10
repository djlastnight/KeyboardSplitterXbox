namespace KeyboardSplitter.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class EmulationManagerException : Exception
    {
        public EmulationManagerException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}
