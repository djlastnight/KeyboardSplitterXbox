namespace XinputWrapper
{
    using System.Runtime.InteropServices;
    using XinputWrapper.Structs;

    internal static class NativeMethods
    {
        [DllImport("xinput1_3.dll")]
        public static extern int XInputGetState(int userIndex, ref XInputState state);

        [DllImport("xinput1_3.dll")]
        public static extern int XInputSetState(int userIndex, ref XInputVibration vibration);

        [DllImport("xinput1_3.dll")]
        public static extern int XInputGetCapabilities(int userIndex, int flags, ref XInputCapabilities capabilities);

        [DllImport("xinput1_3.dll")]
        public static extern int XInputGetBatteryInformation(int userIndex, byte devType, ref XInputBatteryInformation batteryInformation);

        [DllImport("xinput1_3.dll")]
        public static extern int XInputGetKeystroke(int userIndex, int reserved, ref XInputKeystroke keystroke);

        [DllImport("xinput1_3.dll", CharSet = CharSet.Auto, EntryPoint = "#103")]
        public static extern int TurnOff(int userIndex);
    }
}