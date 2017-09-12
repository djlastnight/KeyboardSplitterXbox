namespace SplitterCore.Preset
{
    using SplitterCore.Emulation;

    public delegate IPreset PresetModifiedEventHandler(IEmulationSlot sender, string presetXml);
}