namespace Interceptor
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseStroke
    {
        public MouseState State;
        public MouseFlags Flags;
        public short Rolling;
        public int X;
        public int Y;
        public ushort Information;
    }  
}