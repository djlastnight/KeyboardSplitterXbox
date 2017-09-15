namespace KeyboardSplitter.Enums
{
    public enum GameDataStatus
    {
        /// <summary>
        /// Status is not checked yet.
        /// </summary>
        None = 0,

        /// <summary>
        /// Game could be started.
        /// </summary>
        OK,

        /// <summary>
        /// Some SlotData item has invalid parameters or missing input device.
        /// </summary>
        Warning,

        /// <summary>
        /// Game is deleted or its executable file is not *.exe
        /// </summary>
        Broken
    }
}