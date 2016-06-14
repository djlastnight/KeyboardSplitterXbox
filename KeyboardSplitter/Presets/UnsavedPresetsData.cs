namespace KeyboardSplitter.Presets
{
    using System.Collections.Generic;
    using KeyboardSplitter.Controls;

    internal class UnsavedPresetsData
    {
        public UnsavedPresetsData()
        {
            this.NotSavedControls = new List<JoyControl>();
            this.IgnoredControls = new List<JoyControl>();
        }

        public List<JoyControl> NotSavedControls { get; set; }

        public List<JoyControl> IgnoredControls { get; set; }

        public string Message { get; set; }

        public void SaveNotSavedPresets()
        {
            foreach (var notSavedControl in this.NotSavedControls)
            {
                notSavedControl.SaveCurrentPreset(silently: true);
            }
        }
    }
}
