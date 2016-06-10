namespace XboxInterfaceWrap
{
    using System.Runtime.InteropServices;

    public static class RealXboxController
    {
        public static bool TurnOff(uint userIndex)
        {
            return NativeMethods.TurnOffExt((int)userIndex - 1) > 0;
        }
    }
}