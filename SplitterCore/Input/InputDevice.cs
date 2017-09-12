namespace SplitterCore.Input
{
    public abstract class InputDevice
    {
        protected InputDevice(uint deviceID, string hardwareId, string strongName, string friendlyName)
        {
            this.DeviceID = deviceID;
            this.HardwareID = hardwareId;
            this.StrongName = strongName;
            this.FriendlyName = friendlyName;
        }

        public virtual uint DeviceID { get; private set; }

        public virtual string HardwareID { get; private set; }

        public virtual string StrongName { get; private set; }

        public virtual string FriendlyName { get; private set; }

        public abstract bool IsKeyboard { get; }

        public virtual bool Match(InputDevice device)
        {
            if (device == null)
            {
                return false;
            }

            bool match = device.DeviceID == this.DeviceID &&
                         device.HardwareID == this.HardwareID &&
                         device.StrongName == this.StrongName &&
                         device.FriendlyName == this.FriendlyName;

            return match;
        }

        public override string ToString()
        {
            return this.StrongName;
        }
    }
}