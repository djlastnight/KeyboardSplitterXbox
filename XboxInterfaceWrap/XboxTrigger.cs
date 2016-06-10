namespace XboxInterfaceWrap
{
    using System;

    [Flags]
    public enum XboxTrigger : int
    {
        LeftTrigger = 0x10000,
        RightTrigger = 0x20000,
    }
}
