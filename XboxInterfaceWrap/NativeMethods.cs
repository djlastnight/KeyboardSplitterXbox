namespace XboxInterfaceWrap
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("XInput1_3.dll", CharSet = CharSet.Auto, EntryPoint = "#103")]
        internal static extern int TurnOffExt(int i);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "isVBusExists", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool VBusExists();

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "GetNumEmptyBusSlots", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetEmptyBusSlotsCount(out int count);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "isControllerExists", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ControllerExistsExt(uint userIndex);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "isControllerOwned", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsControllerOwnedExt(uint userIndex);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "PlugIn", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool PlugInExt(uint userIndex);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "UnPlug", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool UnPlug(uint userIndex);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "UnPlugForce", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool UnPlugForce(uint userIndex);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetAxisX", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetAxisX(uint userIndex, short value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetAxisY", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetAxisY(uint userIndex, short value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetAxisRx", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetAxisRx(uint userIndex, short value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetAxisRy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetAxisRy(uint userIndex, short value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetDpad", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetDpadExt(uint userIndex, int value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnA", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnA(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnGuide", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnGuide(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnB", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnB(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnX", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnX(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnY", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnY(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnStart", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnStart(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnBack", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnBack(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnLT", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnLT(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnRT", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnRT(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnLB", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnLB(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetBtnRB", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetBtnRB(uint userIndex, bool press);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetTriggerL", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetTriggerL(uint userIndex, byte value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "SetTriggerR", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetTriggerR(uint userIndex, byte value);

        [DllImport("XboxInterfaceNative.dll", EntryPoint = "GetLedNumber", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetLedNumber(uint userIndex, out byte ledNumber);
    }
}
