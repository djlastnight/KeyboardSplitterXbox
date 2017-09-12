namespace Interceptor
{
    using System.Runtime.InteropServices;
    using Interceptor.Enums;

    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyStroke
    {
        public InterceptionKey Code;
        public KeyState State;
        public uint Information;
    }
}