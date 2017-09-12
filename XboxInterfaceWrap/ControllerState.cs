namespace VirtualXbox
{
    using System.Collections.Generic;
    using VirtualXbox.Enums;

    internal class ControllerState
    {
        public ControllerState()
        {
            this.ButtonsDown = new List<XboxButton>(11);
        }

        public List<XboxButton> ButtonsDown { get; internal set; }

        public XboxDpadDirection DpadDirections { get; internal set; }

        public byte LeftTriggerValue { get; internal set; }

        public byte RightTriggerValue { get; internal set; }

        public short AxisXValue { get; internal set; }

        public short AxisYValue { get; internal set; }

        public short AxisRxValue { get; internal set; }

        public short AxisRyValue { get; internal set; }

        public void Reset()
        {
            this.ButtonsDown = new List<XboxButton>(11);
            this.DpadDirections = XboxDpadDirection.Off;
            this.LeftTriggerValue = 0;
            this.RightTriggerValue = 0;
            this.AxisXValue = 0;
            this.AxisYValue = 0;
            this.AxisRxValue = 0;
            this.AxisRyValue = 0;
        }
    }
}