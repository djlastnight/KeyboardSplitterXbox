namespace KeyboardSplitter.Presets
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using XboxInterfaceWrap;

    [Serializable]
    [XmlType("axis")]
    public class PresetAxis
    {
        public PresetAxis()
        {
            // needed for serialization
        }

        public PresetAxis(XboxAxis axis, XboxAxisPosition position, string key)
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
}