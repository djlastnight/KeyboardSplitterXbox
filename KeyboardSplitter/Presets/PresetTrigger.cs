namespace KeyboardSplitter.Presets
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using XboxInterfaceWrap;

    [Serializable]
    [XmlType("trigger")]
    public class PresetTrigger
    {
        public PresetTrigger()
        {
        }

        public PresetTrigger(XboxTrigger trigger, string key)
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
}