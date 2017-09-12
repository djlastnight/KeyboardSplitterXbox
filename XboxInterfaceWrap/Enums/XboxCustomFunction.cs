namespace VirtualXbox.Enums
{
    /// <summary>
    /// Represents all possible xbox functions
    /// in a single enumeration.
    /// </summary>
    public enum XboxCustomFunction : uint
    {
        /// <summary>
        /// Xbox controller's D-Pad up direction.
        /// </summary>
        Dpad_Up = 0x0000001,

        /// <summary>
        /// Xbox controller's D-Pad down direction.
        /// </summary>
        Dpad_Down = 0x0000002,

        /// <summary>
        /// Xbox controller's D-Pad left direction.
        /// </summary>
        Dpad_Left = 0x0000004,

        /// <summary>
        /// Xbox controller's D-Pad right direction.
        /// </summary>
        Dpad_Right = 0x0000008,

        /// <summary>
        /// Xbox controller's start button.
        /// </summary>
        Button_Start = 0x0000010,

        /// <summary>
        /// Xbox controller's back button.
        /// </summary>
        Button_Back = 0x0000020,

        /// <summary>
        /// Xbox controller's left thumb button.
        /// </summary>
        Left_Thumb = 0x0000040,

        /// <summary>
        /// Xbox controller's right thumb button.
        /// </summary>
        Right_Thumb = 0x0000080,

        /// <summary>
        /// Xbox controller's left bumper button.
        /// </summary>
        Left_Bumper = 0x0000100,

        /// <summary>
        /// Xbox controller's right bumper button.
        /// </summary>
        Right_Bumper = 0x0000200,

        /// <summary>
        /// Xbox controller's guide button.
        /// </summary>
        Button_Guide = 0x0000400,

        /// <summary>
        /// Xbox controller's button A.
        /// </summary>
        Button_A = 0x0001000,

        /// <summary>
        /// Xbox controller's button B.
        /// </summary>
        Button_B = 0x0002000,

        /// <summary>
        /// Xbox controller's button X.
        /// </summary>
        Button_X = 0x0004000,

        /// <summary>
        /// Xbox controller's button Y.
        /// </summary>
        Button_Y = 0x0008000,

        /// <summary>
        /// Xbox controller's left trigger.
        /// </summary>
        Left_Trigger = 0x0010000,

        /// <summary>
        /// Xbox controller's right trigger.
        /// </summary>
        Right_Trigger = 0x0020000,

        /// <summary>
        /// Xbox controller's left stick left direction.
        /// </summary>
        Axis_X_Min = 0x0100000,

        /// <summary>
        /// Xbox controller's left stick right direction.
        /// </summary>
        Axis_X_Max = 0x0200000,

        /// <summary>
        /// Xbox controller's left stick down direction.
        /// </summary>
        Axis_Y_Min = 0x0400000,

        /// <summary>
        /// Xbox controller's left stick up direction.
        /// </summary>
        Axis_Y_Max = 0x0800000,

        /// <summary>
        /// Xbox controller's right stick left direction.
        /// </summary>
        Axis_Rx_Min = 0x1000000,

        /// <summary>
        /// Xbox controller's right stick right direction.
        /// </summary>
        Axis_Rx_Max = 0x2000000,

        /// <summary>
        /// Xbox controller's right stick down direction.
        /// </summary>
        Axis_Ry_Min = 0x4000000,

        /// <summary>
        /// Xbox controller's right stick up direction.
        /// </summary>
        Axis_Ry_Max = 0x8000000,
    }
}
