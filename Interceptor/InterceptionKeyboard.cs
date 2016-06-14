namespace Interceptor
{
    public class InterceptionKeyboard
    {
        public uint DeviceID { get; internal set; }

        public string HardwareID { get; internal set; }

        public string FriendlyName { get; internal set; }

        public string StrongName { get; internal set; }

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