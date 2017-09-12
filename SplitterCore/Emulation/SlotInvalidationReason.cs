namespace SplitterCore.Emulation
{
    /// <summary>
    /// Enumeration, which determines why
    /// the Slot has been invalidated.
    /// </summary>
    public enum SlotInvalidationReason
    {
        /// <summary>
        /// Slot is not invalidated
        /// </summary>
        None,

        /// <summary>
        /// Virtual bus driver is not installed.
        /// </summary>
        VirtualBus_Not_Installed,

        /// <summary>
        /// Specific to the virtual controller drivers are not installed
        /// </summary>
        Additional_Drivers_Not_Installed,

        /// <summary>
        /// Virtual bus can not mount the desired virtual controller, because there are no free slots left
        /// </summary>
        VirtualBus_Full,

        /// <summary>
        /// Slot's gamepad is already plugged in.
        /// </summary>
        Controller_Already_Plugged_In,

        /// <summary>
        /// Slot's gamepad is currently in use, because probably it is owned by another process
        /// </summary>
        Controller_In_Use,

        /// <summary>
        /// Slot's keyboard is unplugged from the system.
        /// </summary>
        Keyboard_Unplugged,

        /// <summary>
        /// Slot's mouse is unplugged from the system.
        /// </summary>
        Mouse_Unplugged,

        /// <summary>
        /// Presets parse failed error
        /// </summary>
        Presets_Parse_Failed,

        Controller_Plug_In_Failed,

        XinputBus_Full,

        Controller_Unplugged,

        No_Input_Device_Selected
    }
}
