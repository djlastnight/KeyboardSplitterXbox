using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interceptor
{
    public class InterceptionMouse : InterceptionDevice
    {
        public InterceptionMouse()
        {

        }

        public InterceptionMouse(uint deviceId, string hardwareId, string strongName)
        {
            this.DeviceID = deviceId;
            this.HardwareID = hardwareId;
            this.StrongName = strongName;
        }

        public override bool IsKeyboard
        {
            get
            {
                return false;
            }
        }
    }
}
