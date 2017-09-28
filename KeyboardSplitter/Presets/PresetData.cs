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
            PresetData.InsertImuttablePresets(this);
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
            serializer.UnknownNode += (ss, ee) => { throw new InvalidOperationException("Deserialize found unknown node"); };
            using (var fileStream = new FileStream(xmlFileLocation, FileMode.Open))
            {
                var reader = new XmlTextReader(fileStream);
                PresetData data = (PresetData)serializer.Deserialize(reader);

                // Removing protected presets from file data
                var array = new Preset[data.Presets.Count];
                data.Presets.CopyTo(array, 0);

                foreach (var item in array)
                {
                    if (PresetDataManager.IsProtectedPreset(item.Name))
                    {
                        data.Presets.Remove(item);
                    }
                }

                InsertImuttablePresets(data);
                return data;
            }
        }

        public void Serialize(string xmlFileLocation)
        {
            var filteredPresets = new ObservableCollection<Preset>();
            foreach (var preset in this.Presets)
            {
                if (PresetDataManager.IsProtectedPreset(preset.Name))
                {
                    continue;
                }

                filteredPresets.Add(preset);
            }

            var filteredData = new PresetData();
            filteredData.Presets = filteredPresets;

            using (var writer = new StreamWriter(path: xmlFileLocation, append: false, encoding: Encoding.Unicode))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, filteredData);
            }
        }

        private static void InsertImuttablePresets(PresetData data)
        {
            for (int i = 0; i < Preset.ImuttablePresets.Count; i++)
            {
                data.Presets.Insert(i, Preset.ImuttablePresets[i]);
            }
        }
    }
}
