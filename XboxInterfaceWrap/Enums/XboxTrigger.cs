namespace VirtualXbox.Enums
{
    using System;

    [Flags]
    public enum XboxTrigger : uint
    {
        Left = 0x10000,
        Right = 0x20000,
    }
}
