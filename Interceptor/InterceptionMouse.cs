namespace Interceptor
{
    using Interceptor.Enums;

    public class InterceptionMouse : InterceptionDevice
    {
        internal InterceptionMouse(uint deviceID, string hardwareId)
            : base(deviceID, hardwareId)
        {
        }

        public override InterceptionDeviceType DeviceType
        {
            get { return InterceptionDeviceType.Mouse; }
        }
    }
}