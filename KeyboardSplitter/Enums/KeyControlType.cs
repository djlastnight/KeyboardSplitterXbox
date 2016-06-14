namespace KeyboardSplitter.Enums
{
    /// <summary>
    /// Enumeration, used to determine the KeyControl target type.
    /// </summary>
    public enum KeyControlType : int
    {
        /// <summary>
        /// Button key control type.
        /// </summary>
        Button = 1,

        /// <summary>
        /// Axis key control type.
        /// </summary>
        Axis = 2,

        /// <summary>
        /// D-pad key control type.
        /// </summary>
        Dpad = 3,

        /// <summary>
        /// Trigger key control type.
        /// </summary>
        Trigger = 4,
    }
}