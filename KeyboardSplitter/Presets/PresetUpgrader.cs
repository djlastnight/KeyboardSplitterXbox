namespace KeyboardSplitter.Presets
{
    using SplitterCore.Input;
    using SplitterCore.Preset;
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
            var xmlText = File.ReadAllText(versionOneXmlFileLocation);
            var correctedXmlText = xmlText.Replace("LeftTrigger", "Left").Replace("RightTrigger", "Right");
            var upgraded = new List<Preset>();
            var oldData = PresetDataVersionOne.Deserialize(correctedXmlText);
            foreach (var oldPreset in oldData.Presets)
            {
                var preset = new Preset();
                preset.Name = oldPreset.Name;
                foreach (var axis in oldPreset.Axes)
                {
                    preset.Axes.Add(new PresetAxis((uint)axis.Axis, (short)axis.Position, (InputKey)Enum.Parse(typeof(InputKey), axis.KeyboardKey)));
                }

                foreach (var button in oldPreset.Buttons)
                {
                    preset.Buttons.Add(new PresetButton((uint)button.Button, (InputKey)Enum.Parse(typeof(InputKey), button.KeyboardKey)));
                }

                foreach (var trigger in oldPreset.Triggers)
                {
                    preset.Triggers.Add(new PresetTrigger((uint)trigger.Trigger, (InputKey)Enum.Parse(typeof(InputKey), trigger.KeyboardKey)));
                }

                foreach (var pov in oldPreset.Povs)
                {
                    preset.Dpads.Add(new PresetDpad((int)pov.Direction, (InputKey)Enum.Parse(typeof(InputKey), pov.KeyboardKey)));
                }

                foreach (var custom in oldPreset.CustomFunctions)
                {
                    preset.CustomFunctions.Add(new PresetCustom((uint)custom.Function, (InputKey)Enum.Parse(typeof(InputKey), custom.KeyboardKey)));
                }

                upgraded.Add(preset);
            }

            return upgraded;
        }

        [Serializable]
        [XmlType("preset")]
        public class PresetVersionOne
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
        public class PresetAxisVersionOne
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
        public class PresetButtonVersionOne
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
        public class PresetCustomVersionOne
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
        public class PresetDpadVersionOne
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
        public class PresetTriggerVersionOne
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
        public class PresetDataVersionOne
        {
            public PresetDataVersionOne()
            {
                this.Presets = new ObservableCollection<PresetVersionOne>();
            }

            [XmlElement("preset")]
            public ObservableCollection<PresetVersionOne> Presets { get; set; }

            public static PresetDataVersionOne Deserialize(string xmlText)
            {
                var serializer = new XmlSerializer(typeof(PresetDataVersionOne));

                using (var reader = new StringReader(xmlText))
                {
                    return (PresetDataVersionOne)serializer.Deserialize(reader);
                }
            }
        }
    }
}