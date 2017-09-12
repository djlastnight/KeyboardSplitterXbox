namespace SplitterCore.Preset
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    [XmlType("dpad")]
    public class PresetDpad : IPresetElement
    {
        public PresetDpad()
        {
        }

        public PresetDpad(int direction, InputKey key)
        {
            this.Direction = direction;
            this.Key = key;
        }

        [XmlAttribute("direction")]
        public int Direction { get; set; }

        [XmlText]
        public InputKey Key { get; set; }

        public FunctionType FunctionType
        {
            get { return FunctionType.Dpad; }
        }

        public override string ToString()
        {
            return this.Direction.ToString();
        }
    }
}