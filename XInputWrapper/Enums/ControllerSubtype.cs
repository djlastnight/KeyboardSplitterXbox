namespace XinputWrapper.Enums
{
    using System;

    [Flags]
    public enum ControllerSubtype
    {
        Unknown = 0x00,
        Gamepad = 0x01,
        Wheel = 0x02,
        ArcadeStick = 0x03,
        FlightStick = 0x04,
        DancePad = 0x05,
        Guitar = 0x06,
        GuitarAlternate = 0x07,
        Drumkit = 0x08,
        GuitarBass = 0x0B,
        ArcadePad = 0x13
    }
}