namespace XinputWrapper
{
    public class XInputConstants
    {
        public const int XinputDevtypeGamepad = 0x01;

        //// Gamepad thresholds
        public const int XinputGamepadLeftThumbDeadzone = 7849;
        public const int XinputGamepadRightThumbDeadzone = 8689;
        public const int XinputGamepadTriggerThreshold = 30;

        //// Flags to pass to XInputGetCapabilities
        public const int XinputFlagGamepad = 0x00000001;
    }
}
