namespace KeyboardSplitter.Exceptions.Emulation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class EmulationExceptionBase : KeyboardSplitterExceptionBase
    {
        public EmulationExceptionBase(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}
