namespace KeyboardSplitter.Presets
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using XboxInterfaceWrap;

    [Serializable]
    [XmlType("button")]
    public class PresetButton
    {
        public PresetButton()
        {
        }

        public PresetButton(XboxButton button, string key)
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
}