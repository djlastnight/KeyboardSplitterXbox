namespace SplitterCore.Input
{
    using System;

    public class InputDeviceChangedEventArgs : EventArgs
    {
        public InputDeviceChangedEventArgs(InputDevice changedDevice, bool isRemoved)
        {
            if (changedDevice == null)
            {
                throw new ArgumentNullException("changedDevice");
            }

            this.ChangedDevice = changedDevice;
            this.IsRemoved = isRemoved;
        }

        public InputDevice ChangedDevice { get; private set; }

        public bool IsRemoved { get; private set; }

        public override string ToString()
        {
            return string.Format(
                "{0} [{1}] was {2}",
                this.ChangedDevice.StrongName,
                this.ChangedDevice.FriendlyName,
                this.IsRemoved ? "removed" : "attached");
        }
    }
}
