namespace SplitterCore.Preset
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SplitterCore.Input;

    public interface IPreset
    {
        string Name { get; set; }

        ObservableCollection<PresetButton> Buttons { get; set; }

        ObservableCollection<PresetTrigger> Triggers { get; set; }

        ObservableCollection<PresetAxis> Axes { get; set; }

        ObservableCollection<PresetDpad> Dpads { get; set; }

        ObservableCollection<PresetCustom> CustomFunctions { get; set; }

        IPreset FilterByKey(InputKey key);

        List<InputKey> GetKeys(PresetButton button);

        List<InputKey> GetKeys(PresetTrigger trigger);

        List<InputKey> GetKeys(PresetAxis axis);

        List<InputKey> GetKeys(PresetDpad dpad);

        List<InputKey> GetKeys(PresetCustom custom);
    }
}
