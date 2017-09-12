namespace KeyboardSplitter.Helpers
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        public static extern int RegOpenKeyEx(
            IntPtr h_key,
            string subKey,
            uint options,
            int samDesired,
            out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegNotifyChangeKeyValue(
            IntPtr h_key,
            bool b_watchSubtree,
            RegChangeNotifyFilter dw_NotifyFilter,
            IntPtr h_event,
            bool f_asynchronous);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(IntPtr h_key);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();
    }
}
