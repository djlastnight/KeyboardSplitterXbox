namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Windows;

    public static class PresetDataManager
    {
        public const string PresetsFilename = "splitter_presets.xml";

        private const string PresetsBackupFilename = "splitter_presets_backup.xml";

        private static PresetData data;

        static PresetDataManager()
        {
            PresetDataManager.data = PresetDataManager.ReadPresetDataFromFile();
        }

        public static ObservableCollection<Preset> CurrentPresets
        {
            get
            {
                return PresetDataManager.data.Presets;
            }
        }

        public static PresetData ReadPresetDataFromFile()
        {
            var data = new PresetData();

            if (!File.Exists(PresetDataManager.PresetsFilename))
            {
                return data;
            }

            try
            {
                data = PresetData.Deserialize(PresetDataManager.PresetsFilename);
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "Reading presets file [{0}] failed: {1}",
                    PresetDataManager.PresetsFilename,
                    e.ToString());

                LogWriter.Write(message);

                bool isBackupCreated = false;

                // Creating a presets backup file
                if (File.Exists(PresetDataManager.PresetsFilename) && !File.Exists(PresetDataManager.PresetsBackupFilename))
                {
                    try
                    {
                        File.Copy(PresetDataManager.PresetsFilename, PresetDataManager.PresetsBackupFilename);
                        isBackupCreated = true;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Write("Presets file backup failed: " + ex.Message);
                    }

                    LogWriter.Write("Backup of presets file succesfully created at " + PresetDataManager.PresetsBackupFilename);
                }

                string alertMessage = "Presets file parse failed. Please refer to " + LogWriter.GetLogFileName + " for more details.";
                if (isBackupCreated)
                {
                    alertMessage += Environment.NewLine + "Backup created at " + PresetDataManager.PresetsBackupFilename;
                }

                var splitter = Helpers.SplitterHelper.TryFindSplitter();
                if (splitter != null && splitter.EmulationManager != null && splitter.EmulationManager.Slots != null)
                {
                    foreach (var slot in splitter.EmulationManager.Slots)
                    {
                        slot.InvalidateReason = SplitterCore.Emulation.SlotInvalidationReason.Presets_Parse_Failed;
                    }
                }

                Controls.MessageBox.Show(
                    alertMessage,
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return data;
        }

        public static void WritePresetDataToFile(bool removeImuttablePresets = true)
        {
            PresetDataManager.data.Serialize(PresetsFilename, removeImuttablePresets);
        }

        public static void AddNewPreset(Preset preset)
        {
            PresetDataManager.data.Presets.Add(preset);
        }

        public static void DeletePreset(Preset preset)
        {
            if (PresetDataManager.data.Presets.Contains(preset))
            {
                PresetDataManager.data.Presets.Remove(preset);
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

        public static bool IsPresetChanged(Preset preset)
        {
            if (preset == null)
            {
                throw new ArgumentNullException("preset");
            }

            if (IsProtectedPreset(preset.Name))
            {
                return false;
            }

            var fileData = PresetDataManager.ReadPresetDataFromFile();
            var currentData = PresetDataManager.data;

            if (!currentData.Presets.Contains(preset))
            {
                throw new InvalidOperationException(
                    "The following preset does not exsists in the current preset data: " + preset);
            }

            var original = fileData.Presets.FirstOrDefault(x => x.Name == preset.Name);
            if (original == null)
            {
                // We found a new preset
                return true;
            }
            else
            {
                var origialSerialized = original.ToXml();
                var presetSerialized = preset.ToXml();
                return origialSerialized != presetSerialized;
            }
        }

        public static Preset FindPreset(string presetName)
        {
            return PresetDataManager.CurrentPresets.FirstOrDefault(x => x.Name.ToLower() == presetName.ToLower());
        }
    }
}