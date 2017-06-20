namespace Interceptor
{
    public class InterceptionKeyboard : InterceptionDevice
    {

        public InterceptionKeyboard()
        {

        }

        public InterceptionKeyboard(uint deviceID, string hardwareID, string strongName)
        {
            base.DeviceID = deviceID;
            base.HardwareID = hardwareID;
            base.StrongName = strongName;
        }

        public override bool IsKeyboard
        {
            get { return true; }
        }
    }
}