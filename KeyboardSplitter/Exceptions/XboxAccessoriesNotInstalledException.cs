namespace KeyboardSplitter.Exceptions
{
    using System;

    [Serializable]
    public class XboxAccessoriesNotInstalledException : EmulationManagerException
    {
        public XboxAccessoriesNotInstalledException(string message)
            : base(message)
        {
            // no action is needed
        }
    }
}