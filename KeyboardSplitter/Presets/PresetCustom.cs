namespace KeyboardSplitter.Presets
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using KeyboardSplitter.Enums;

    [Serializable]
    [XmlType("custom")]
    public class PresetCustom
    {
        public PresetCustom()
        {
        }

        public PresetCustom(XboxCustomFunction function, string key)
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
}