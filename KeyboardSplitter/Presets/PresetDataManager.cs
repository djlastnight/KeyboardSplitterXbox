namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    public static class PresetDataManager
    {
        private const string PresetsFilename = "presets.xml";

        private static PresetData data;

        static PresetDataManager()
        {
            data = new PresetData();

            try
            {
                data = PresetData.Deserialize(PresetsFilename);
            }
            catch (Exception)
            {
            }

            data.Presets.CollectionChanged += new NotifyCollectionChangedEventHandler(Presets_CollectionChanged);
        }

        public static event NotifyCollectionChangedEventHandler PresetsChanged;

        public static ObservableCollection<Preset> Presets
        {
            get
            {
                return data.Presets;
            }
        }

        public static bool IsProtectedPreset(string presetName)
        {
            if (Preset.ImuttablePresets.Select(x => x.Name).Contains(presetName, StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static void ExportToFile()
        {
            data.Serialize(PresetsFilename);
        }

        private static void Presets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PresetDataManager.PresetsChanged != null)
            {
                PresetDataManager.PresetsChanged(sender, e);
            }
        }
    }
}