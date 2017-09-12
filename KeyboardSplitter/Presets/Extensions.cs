namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SplitterCore.Input;
    using SplitterCore.Preset;
    using VirtualXbox;
    using VirtualXbox.Enums;

    public static class Extensions
    {
        public static void Reset(this Preset preset)
        {
            var noneKey = InputKey.None;

            preset.Buttons = new ObservableCollection<PresetButton>();
            foreach (uint button in Enum.GetValues(typeof(XboxButton)))
            {
                preset.Buttons.Add(new PresetButton(button, noneKey));
            }

            preset.Triggers = new ObservableCollection<PresetTrigger>();
            foreach (uint trigger in Enum.GetValues(typeof(XboxTrigger)))
            {
                preset.Triggers.Add(new PresetTrigger(trigger, noneKey));
            }

            preset.Axes = new ObservableCollection<PresetAxis>();
            foreach (uint axis in Enum.GetValues(typeof(XboxAxis)))
            {
                preset.Axes.Add(new PresetAxis(axis, (short)XboxAxisPosition.Min, noneKey));
                preset.Axes.Add(new PresetAxis(axis, (short)XboxAxisPosition.Max, noneKey));
            }

            preset.Dpads = new ObservableCollection<PresetDpad>();
            foreach (int direction in Enum.GetValues(typeof(XboxDpadDirection)))
            {
                if (direction != (int)XboxDpadDirection.Off)
                {
                    preset.Dpads.Add(new PresetDpad(direction, noneKey));
                }
            }

            preset.CustomFunctions = new ObservableCollection<PresetCustom>();
        }
    }
}
