namespace Interceptor
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    public class KeyPressedEventArgs : EventArgs
    {
        public InterceptionKeys Key { get; set; }

        public KeyState State { get; set; }

        public bool Handled { get; set; }

        public InterceptionKeyboard Keyboard { get; set; }

        public string CorrectedKey
        {
            get
            {
                return KeysHelper.GetCorrectedKeyName(this.Key, this.State);
            }
        }
    }
}