namespace XboxInterfaceWrap
{
    using System;

    [Flags]
    public enum XboxDpadDirection : int
    {
        None = 0x0000,
        Up = 0x0001,
        Down = 0x0002,
        Left = 0x0004,
        Right = 0x0008
    }
}