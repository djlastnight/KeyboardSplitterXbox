namespace VirtualXbox
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport("VirtualXboxNative.dll", EntryPoint = "VBusExists", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool VBusExists();

        [DllImport("VirtualXboxNative.dll", EntryPoint = "GetEmptyBusSlotsCount", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetEmptyBusSlotsCount(out int count);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "ControllerExists", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ControllerExists(uint userIndex);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "IsControllerOwned", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsControllerOwned(uint userIndex);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "PlugIn", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool PlugIn(uint userIndex);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "Unplug", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Unplug(uint userIndex, bool isForced);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "GetLedNumber", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetLedNumber(uint userIndex, out byte ledNumber);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "SetButton", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetButton(uint userIndex, uint button, bool isPressed);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "SetTrigger", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetTrigger(uint userIndex, uint button, byte value);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "SetDpad", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetDpad(uint userIndex, int value);

        [DllImport("VirtualXboxNative.dll", EntryPoint = "SetAxis", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetAxis(uint userIndex, uint axis, short value);
    }
}
