namespace XinputWrapper.Enums
{
    public enum BatteryType : byte
    {
        Disconnected = 0x00,    // This device is not connected
        Wired = 0x01,    // Wired device, no battery
        Alkaline = 0x02,    // Alkaline battery source
        NIMH = 0x03,    // Nickel Metal Hydride battery source
        Unknown = 0xFF,    // Cannot determine the battery type
    }
}