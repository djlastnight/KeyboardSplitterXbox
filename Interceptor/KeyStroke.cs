namespace Interceptor
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public InterceptionKeys Code;
        public KeyState State;
        public uint Information;
    }
}