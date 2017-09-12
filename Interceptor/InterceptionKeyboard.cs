namespace Interceptor
{
    using Interceptor.Enums;

    public class InterceptionKeyboard : InterceptionDevice
    {
        internal InterceptionKeyboard(uint deviceID, string hardwareID)
            : base(deviceID, hardwareID)
        {
        }

        public override InterceptionDeviceType DeviceType
        {
            get { return InterceptionDeviceType.Keyboard; }
        }
    }
}