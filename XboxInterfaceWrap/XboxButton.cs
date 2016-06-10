namespace XboxInterfaceWrap
{
    using System;

    [Flags]
    public enum XboxButton : int
    {
        Start = 0x0010,
        Back = 0x0020,
        LeftThumb = 0x0040,
        RightThumb = 0x0080,
        LeftBumper = 0x0100,
        RightBumper = 0x0200,
        Guide = 0x0400,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000,
    }
}
