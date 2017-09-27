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
            PresetData.AddImuttablePresets(this);
        }

        private static void AddImuttablePresets(PresetData data)
        {
            data.Presets.Insert(0, Preset.Default);
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
            serializer.UnknownNode += (ss, ee) => { throw new InvalidOperationException(); };
            using (var fileStream = new FileStream(xmlFileLocation, FileMode.Open))
            {
                var reader = new XmlTextReader(fileStream);
                PresetData data = (PresetData)serializer.Deserialize(reader);
                var array = new Preset[data.Presets.Count];
                data.Presets.CopyTo(array, 0);

                foreach (var item in array)
                {
                    if (PresetDataManager.IsProtectedPreset(item.Name))
                    {
                        data.Presets.Remove(item);
                    }
                }

                AddImuttablePresets(data);
                return data;
            }
        }

        public void Serialize(string xmlFileLocation, bool removeImuttablePresets = true)
        {
            if (removeImuttablePresets)
            {
                this.RemoveImuttablePresets();
            }

            using (var writer = new StreamWriter(path: xmlFileLocation, append: false, encoding: Encoding.Unicode))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
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
