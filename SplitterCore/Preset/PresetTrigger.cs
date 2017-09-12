namespace SplitterCore.Preset
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    [XmlType("trigger")]
    public class PresetTrigger : IPresetElement
    {
        public PresetTrigger()
        {
        }

        public PresetTrigger(uint trigger, InputKey key)
        {
            this.Trigger = trigger;
            this.Key = key;
        }

        [XmlAttribute("id")]
        public uint Trigger { get; set; }

        [XmlText]
        public InputKey Key { get; set; }

        public FunctionType FunctionType
        {
            get { return FunctionType.Trigger; }
        }

        public override string ToString()
        {
            return this.Trigger.ToString();
        }
    }
}