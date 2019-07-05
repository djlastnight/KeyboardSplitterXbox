namespace XinputWrapper
{
    using System.Runtime.InteropServices;
    using XinputWrapper.Structs;

    internal static class NativeMethods
    {
        // Use System32 path to prevent x360ce's xinput hook, because it does not return proper XInputGetState for virtual controllers.
        private const string XinputPath = @"System32\xinput1_3.dll";

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int XInputGetState(int userIndex, ref XInputState state);

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int XInputSetState(int userIndex, ref XInputVibration vibration);

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int XInputGetCapabilities(int userIndex, int flags, ref XInputCapabilities capabilities);

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int XInputGetBatteryInformation(int userIndex, byte devType, ref XInputBatteryInformation batteryInformation);

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int XInputGetKeystroke(int userIndex, int reserved, ref XInputKeystroke keystroke);

        [DllImport(XinputPath, CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "#103")]
        public static extern int TurnOff(int userIndex);
    }
}