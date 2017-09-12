namespace Interceptor
{
    using System.Runtime.InteropServices;
    using Interceptor.Enums;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseStroke
    {
        public MouseState State;
        public MouseFlags Flags;
        public short Rolling;
        public int X;
        public int Y;
        public ushort Information;
    }  
}