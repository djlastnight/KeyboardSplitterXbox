namespace SplitterCore.Input
{
    public class Mouse : InputDevice
    {
        public static readonly Mouse None = new Mouse(0, string.Empty, "None", "No mouse selected");

        public Mouse(uint deviceId, string hardwareId, string strongName, string friendlyName)
            : base(deviceId, hardwareId, strongName, friendlyName)
        {
        }

        public override bool IsKeyboard
        {
            get { return false; }
        }
    }
}