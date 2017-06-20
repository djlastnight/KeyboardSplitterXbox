namespace KeyboardSplitter.Enums
{
    /// <summary>
    /// Enumeration, which determines why
    /// the slot (JoyControl) has been invalidated.
    /// </summary>
    public enum SlotInvalidationReason
    {
        /// <summary>
        /// Xbox driver bus is not installed.
        /// </summary>
        XboxBus_Not_Installed,

        /// <summary>
        /// The associated to the slot (JoyControl) virtual xbox controller is currently in use.
        /// </summary>
        Controller_In_Use,

        /// <summary>
        /// The associated to the slot (JoyControl) keyboard is unplugged from the system.
        /// </summary>
        Keyboard_Unplugged,

        /// <summary>
        /// The associated to the slot (JoyControl) mouse is unplugged from the system.
        /// </summary>
        Mouse_Unplugged,

        /// <summary>
        /// The associated to the slot (JoyControl) virtual xbox controller is unplugged.
        /// </summary>
        Controller_Unplugged,
    }
}