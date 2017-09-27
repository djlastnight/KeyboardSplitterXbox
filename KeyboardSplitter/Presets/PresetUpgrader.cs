namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using VirtualXbox.Enums;

    public static class PresetUpgrader
    {
        public static IEnumerable<Preset> GetUpgradedPresets(string versionOneXmlFileLocation)
        {
            var data = PresetDataVersionOne.Deserialize(versionOneXmlFileLocation);
            return data.Presets;
        }

        [Serializable]
        [XmlType("preset")]
        internal class PresetVersionOne
        {
            public PresetVersionOne()
            {
                this.Buttons = new List<PresetButtonVersionOne>();
                this.Triggers = new List<PresetTriggerVersionOne>();
                this.Axes = new List<PresetAxisVersionOne>();
                this.Povs = new List<PresetDpadVersionOne>();
                this.CustomFunctions = new List<PresetCustomVersionOne>();
            }

            [XmlAttribute("Name")]
            public string Name { get; set; }

            [XmlElement("button")]
            public List<PresetButtonVersionOne> Buttons { get; set; }

            [XmlElement("trigger")]
            public List<PresetTriggerVersionOne> Triggers { get; set; }

            [XmlElement("axis")]
            public List<PresetAxisVersionOne> Axes { get; set; }

            [XmlElement("dpad")]
            public List<PresetDpadVersionOne> Povs { get; set; }

            [XmlElement("custom")]
            public List<PresetCustomVersionOne> CustomFunctions { get; set; }
        }

        [Serializable]
        [XmlType("axis")]
        internal class PresetAxisVersionOne
        {
            public PresetAxisVersionOne()
            {
                // needed for serialization
            }

            public PresetAxisVersionOne(XboxAxis axis, XboxAxisPosition position, string key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                this.Axis = axis;
                this.Position = position;
                this.KeyboardKey = key;
            }

            [XmlAttribute("ID")]
            public XboxAxis Axis { get; set; }

            [XmlAttribute("Position")]
            public XboxAxisPosition Position { get; set; }

            [XmlText]
            public string KeyboardKey { get; set; }

            public override string ToString()
            {
                return this.KeyboardKey.ToString();
            }
        }

        [Serializable]
        [XmlType("button")]
        internal class PresetButtonVersionOne
        {
            public PresetButtonVersionOne()
            {
            }

            public PresetButtonVersionOne(XboxButton button, string key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                this.Button = button;
                this.KeyboardKey = key;
            }

            [XmlAttribute("ID")]
            public XboxButton Button { get; set; }

            [XmlText]
            public string KeyboardKey { get; set; }

            public override string ToString()
            {
                return this.KeyboardKey.ToString();
            }
        }

        [Serializable]
        [XmlType("custom")]
        internal class PresetCustomVersionOne
        {
            public PresetCustomVersionOne()
            {
            }

            public PresetCustomVersionOne(XboxCustomFunction function, string key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                this.Function = function;
                this.KeyboardKey = key;
            }

            [XmlAttribute("ID")]
            public XboxCustomFunction Function { get; set; }

            [XmlText]
            public string KeyboardKey { get; set; }

            public override string ToString()
            {
                return this.KeyboardKey.ToString();
            }
        }

        [Serializable]
        [XmlType("dpad")]
        internal class PresetDpadVersionOne
        {
            public PresetDpadVersionOne()
            {
            }

            public PresetDpadVersionOne(XboxDpadDirection direction, string key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                this.Direction = direction;
                this.KeyboardKey = key;
            }

            [XmlAttribute("ID")]
            public XboxDpadDirection Direction { get; set; }

            [XmlText]
            public string KeyboardKey { get; set; }

            public override string ToString()
            {
                return this.KeyboardKey.ToString();
            }
        }

        [Serializable]
        [XmlType("trigger")]
        internal class PresetTriggerVersionOne
        {
            public PresetTriggerVersionOne()
            {
            }

            public PresetTriggerVersionOne(XboxTrigger trigger, string key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                this.Trigger = trigger;
                this.KeyboardKey = key;
            }

            [XmlAttribute("ID")]
            public XboxTrigger Trigger { get; set; }

            [XmlText]
            public string KeyboardKey { get; set; }

            public override string ToString()
            {
                return this.KeyboardKey.ToString();
            }
        }

        [Serializable]
        [XmlType("preset_data")]
        internal class PresetDataVersionOne
        {
            public PresetDataVersionOne()
            {
                this.Presets = new ObservableCollection<Preset>();
            }

            [XmlElement("preset")]
            public ObservableCollection<Preset> Presets { get; set; }

            public static PresetDataVersionOne Deserialize(string xmlFileLocation)
            {
                if (!File.Exists(xmlFileLocation))
                {
                    throw new FileNotFoundException(xmlFileLocation);
                }

                var serializer = new XmlSerializer(typeof(PresetDataVersionOne));
                using (var fileStream = new FileStream(xmlFileLocation, FileMode.Open))
                {
                    var reader = new XmlTextReader(fileStream);
                    PresetDataVersionOne data = (PresetDataVersionOne)serializer.Deserialize(reader);
                    return data;
                }
            }
        }
    }
}