namespace Interceptor
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Stroke
    {
        [FieldOffset(0)]
        public MouseStroke Mouse;

        [FieldOffset(0)]
        public KeyStroke Key;
    }
}