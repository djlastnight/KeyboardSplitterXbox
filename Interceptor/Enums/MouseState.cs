namespace Interceptor.Enums
{
    using System;

    [Flags]
    internal enum MouseState : ushort
    {
        None = 0x00,
        LeftDown = 0x01,
        LeftUp = 0x02,
        RightDown = 0x04,
        RightUp = 0x08,
        MiddleDown = 0x10,
        MiddleUp = 0x20,
        LeftExtraDown = 0x40,
        LeftExtraUp = 0x80,
        RightExtraDown = 0x100,
        RightExtraUp = 0x200,
        Wheel = 0x400,
        HWheel = 0x800
    }
}