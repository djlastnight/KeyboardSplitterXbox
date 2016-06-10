namespace Interceptor
{
    using System;

    [Flags]
    public enum MouseFlags : ushort
    {
        MoveRelative = 0x000,
        MoveAbsolute = 0x001,
        VirtualDesktop = 0x002,
        AttributesChanged = 0x004,
        MoveWithoutCoalescing = 0x008,
        TerminalServicesSourceShadow = 0x100
    }
}