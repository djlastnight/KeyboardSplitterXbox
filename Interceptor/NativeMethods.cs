namespace Interceptor
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        [DllImport("interception.dll", EntryPoint = "interception_create_context", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateContext();

        [DllImport("interception.dll", EntryPoint = "interception_destroy_context", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyContext(IntPtr context);

        [DllImport("interception.dll", EntryPoint = "interception_get_precedence", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetPrecedence(IntPtr context, int device);

        [DllImport("interception.dll", EntryPoint = "interception_set_precedence", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPrecedence(IntPtr context, int device, int Precedence);

        [DllImport("interception.dll", EntryPoint = "interception_get_filter", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetFilter(IntPtr context, int device);

        [DllImport("interception.dll", EntryPoint = "interception_set_filter", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetFilter(IntPtr context, Predicate predicate, int filterMode);

        [DllImport("interception.dll", EntryPoint = "interception_wait", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wait(IntPtr context);

        [DllImport("interception.dll", EntryPoint = "interception_wait_with_timeout", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int WaitWithTimeout(IntPtr context, ulong milliseconds);

        [DllImport("interception.dll", EntryPoint = "interception_send", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Send(IntPtr context, int device, ref Stroke stroke, uint numStrokes);

        [DllImport("interception.dll", EntryPoint = "interception_receive", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Receive(IntPtr context, int device, ref Stroke stroke, uint numStrokes);

        [DllImport("interception.dll", EntryPoint = "interception_get_hardware_id", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int GetHardwareId(IntPtr context, int device, StringBuilder hardware_id_buffer, int bufferSize);

        [DllImport("interception.dll", EntryPoint = "interception_is_invalid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IsInvalid(int device);

        [DllImport("interception.dll", EntryPoint = "interception_is_keyboard", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IsKeyboard(int device);

        [DllImport("interception.dll", EntryPoint = "interception_is_mouse", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IsMouse(int device);
    }
}
