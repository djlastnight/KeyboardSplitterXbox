namespace Interceptor
{
    using System;
    using System.Collections.Generic;

    public class InterceptionEventArgs : EventArgs
    {
        internal InterceptionEventArgs(InterceptionDevice device, List<KeyInfo> keyInfos, bool handled = false)
            : base()
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (keyInfos == null)
            {
                throw new ArgumentNullException("keyInfos");
            }

            this.Device = device;
            this.KeyInfos = keyInfos;
            this.Handled = handled;
        }

        public InterceptionDevice Device { get; private set; }

        public List<KeyInfo> KeyInfos { get; private set; }

        public bool Handled { get; set; }
    }
}