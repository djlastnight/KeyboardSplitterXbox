namespace Interceptor
{
    using System;

    public class InterceptionDeviceEventArgs : EventArgs
    {
        public InterceptionDeviceEventArgs(InterceptionDevice device, bool isRemoved)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            this.Device = device;
            this.IsRemoved = isRemoved;
        }

        public InterceptionDevice Device
        {
            get;
            private set;
        }

        public bool IsRemoved
        {
            get;
            private set;
        }
    }
}