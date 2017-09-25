namespace KeyboardSplitter.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType("Games")]
    public class GameData
    {
        public GameData()
        {
            this.Games = new ObservableCollection<Game>();
        }

        [XmlElement("Game")]
        public ObservableCollection<Game> Games { get; set; }

        public static GameData Deserialize(string xmlFileLocation)
        {
            if (!File.Exists(xmlFileLocation))
            {
                throw new FileNotFoundException(xmlFileLocation);
            }

            var serializer = new XmlSerializer(typeof(GameData));
            using (var fileStream = new FileStream(xmlFileLocation, FileMode.Open))
            {
                var reader = new XmlTextReader(fileStream);
                GameData data = (GameData)serializer.Deserialize(reader);
                foreach (var game in data.Games)
                {
                    foreach (var slotData in game.SlotsData)
                    {
                        if (slotData.PresetName == null)
                        {
                            slotData.PresetName = string.Empty;
                        }
                    }

                    game.UpdateStatus();
                }

                return data;
            }
        }

        public void Serialize(string xmlFileLocation)
        {
            using (var writer = new StreamWriter(path: xmlFileLocation, append: false, encoding: Encoding.Unicode))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
            }
        }
    }
}
