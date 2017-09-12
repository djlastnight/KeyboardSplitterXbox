namespace XinputWrapper.Enums
{
    public enum CapabilityFlags
    {
        XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,

        // For Windows 8 only
        XINPUT_CAPS_FFB_SUPPORTED = 0x0001,
        XINPUT_CAPS_WIRELESS = 0x0002,
        XINPUT_CAPS_PMD_SUPPORTED = 0x0008,
        XINPUT_CAPS_NO_NAVIGATION = 0x0010,
    }
}