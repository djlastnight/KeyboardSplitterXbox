namespace KeyboardSplitter.Presets
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using XboxInterfaceWrap;

    [Serializable]
    [XmlType("dpad")]
    public class PresetDpad
    {
        public PresetDpad()
        {
        }

        public PresetDpad(XboxDpadDirection direction, string key)
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
}