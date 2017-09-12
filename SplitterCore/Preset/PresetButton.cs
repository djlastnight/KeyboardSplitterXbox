namespace SplitterCore.Preset
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    [XmlType("button")]
    public class PresetButton : IPresetElement
    {
        public PresetButton()
        {
        }

        public PresetButton(uint button, InputKey key)
        {
            this.Button = button;
            this.Key = key;
        }

        [XmlAttribute("id")]
        public uint Button { get; set; }

        [XmlText]
        public InputKey Key { get; set; }

        public FunctionType FunctionType
        {
            get { return FunctionType.Button; }
        }

        public override string ToString()
        {
            return this.Button.ToString();
        }
    }
}