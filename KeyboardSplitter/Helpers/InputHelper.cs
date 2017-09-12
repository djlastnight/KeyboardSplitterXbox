namespace KeyboardSplitter.Helpers
{
    using System.Collections.Generic;
    using Interceptor;
    using Interceptor.Enums;
    using SplitterCore.Input;

    public static class InputHelper
    {
        private static Dictionary<InterceptionDevice, InputDevice> devices;

        static InputHelper()
        {
            devices = new Dictionary<InterceptionDevice, InputDevice>();
        }

        public static InputDevice ToInputDevice(InterceptionDevice device)
        {
            if (!devices.ContainsKey(device))
            {
                InputDevice newDevice;
                if (device.DeviceType == InterceptionDeviceType.Keyboard)
                {
                    newDevice = new Keyboard(device.DeviceID, device.HardwareID, device.StrongName, device.FriendlyName);
                }
                else
                {
                    newDevice = new Mouse(device.DeviceID, device.HardwareID, device.StrongName, device.FriendlyName);
                }

                devices.Add(device, newDevice);
            }

            return devices[device];
        }

        public static InterceptionDevice ToInterceptionDevice(InputDevice device)
        {
            foreach (var item in devices)
            {
                if (item.Value == device)
                {
                    return item.Key;
                }
            }

            return null;
        }

        public static InputKey ToInputKey(InterceptionKey key)
        {
            return (InputKey)(ushort)key;
        }

        public static InterceptionKey ToInterceptionKey(InputKey key)
        {
            return (InterceptionKey)(ushort)key;
        }
    }
}
