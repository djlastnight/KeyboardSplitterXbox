namespace SplitterCore.Input
{
    public class Keyboard : InputDevice
    {
        public static readonly Keyboard None = new Keyboard(0, string.Empty, "None", "No keyboard selected");

        public Keyboard(uint deviceId, string hardwareId, string strongName, string friendlyName)
            : base(deviceId, hardwareId, strongName, friendlyName)
        {
        }

        public override bool IsKeyboard
        {
            get { return true; }
        }
    }
}