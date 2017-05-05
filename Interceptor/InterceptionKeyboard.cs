namespace Interceptor
{
    public class InterceptionKeyboard
    {
        public uint DeviceID { get; internal set; }

        public string HardwareID { get; internal set; }

        public string FriendlyName { get; internal set; }

        public string StrongName { get; internal set; }

        public InterceptionKeyboard()
        {

        }

        public InterceptionKeyboard(uint deviceID, string hardwareID, string friendlyName, string strongName)
        {
            this.DeviceID = deviceID;
            this.HardwareID = hardwareID;
            this.FriendlyName = friendlyName;
            this.StrongName = strongName;
        }

        public bool IsTheSameAs(InterceptionKeyboard keyboardToCompare)
        {
            var props = typeof(InterceptionKeyboard).GetProperties();

            foreach (var property in props)
            {
                var v1 = property.GetValue(this, null);
                var v2 = property.GetValue(keyboardToCompare, null);
                if (!v1.Equals(v2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}