namespace VirtualXbox.Enums
{
    using System;

    [Flags]
    public enum XboxDpadDirection : int
    {
        Off = 0x0000,
        Up = 0x0001,
        Down = 0x0002,
        Left = 0x0004,
        Right = 0x0008
    }
}