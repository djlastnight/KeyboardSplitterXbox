namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType("preset_data")]
    public class PresetData
    {
        public PresetData()
        {
            this.Presets = new ObservableCollection<Preset>();
            this.Presets.Add(Preset.Empty);
            this.Presets.Add(Preset.Default);
        }

        [XmlElement("preset")]
        public ObservableCollection<Preset> Presets { get; set; }

        public static PresetData Deserialize(string xmlFileLocation)
        {
            if (!File.Exists(xmlFileLocation))
            {
                throw new FileNotFoundException(xmlFileLocation);
            }

            var serializer = new XmlSerializer(typeof(PresetData));
            using (var fileStream = new FileStream(xmlFileLocation, FileMode.Open))
            {
                var reader = new XmlTextReader(fileStream);
                PresetData data = (PresetData)serializer.Deserialize(reader);
                return data;
            }
        }

        public bool Serialize(string xmlFileLocation)
        {
            this.RemoveImuttablePresets();

            try
            {
                using (var writer = new StreamWriter(path: xmlFileLocation, append: false, encoding: Encoding.Unicode))
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(writer, this);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RemoveImuttablePresets()
        {
            foreach (Preset imuttablePreset in Preset.ImuttablePresets)
            {
                this.Presets.Remove(imuttablePreset);
            }
        }
    }
}
