namespace Interceptor
{
    using System;

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

        public bool IsMouseKey
        {
            get
            {
                return (ushort)this.Key > 20000;
            }
        }

        public override string ToString()
        {
            if (!IsMouseKey)
            {
                return string.Format(
                    "Source: {0} {1} {2}",
                    this.Keyboard.StrongName,
                    this.CorrectedKey,
                    this.State);
            }

            return string.Format(
                "Source: {0} {1}",
                this.CorrectedKey,
                this.State);
        }
    }
}