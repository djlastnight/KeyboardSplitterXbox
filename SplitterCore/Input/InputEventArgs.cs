namespace SplitterCore.Input
{
    using System;

    public class InputEventArgs : EventArgs
    {
        public InputEventArgs(InputDevice device, InputKey key, bool isDown, bool handled)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            this.InputDevice = device;
            this.Key = key;
            this.IsDown = isDown;
            this.Handled = handled;
        }

        public InputDevice InputDevice { get; private set; }

        public InputKey Key { get; private set; }

        public bool IsDown { get; private set; }

        public bool Handled { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} on {1} {2}",
                this.Key,
                this.InputDevice.StrongName,
                this.IsDown ? "pressed" : "released");
        }
    }
}
