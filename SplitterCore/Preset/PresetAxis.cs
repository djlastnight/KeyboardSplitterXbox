namespace SplitterCore.Preset
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    [XmlType("axis")]
    public class PresetAxis : IPresetElement
    {
        public PresetAxis()
        {
            // needed for serialization
        }

        public PresetAxis(uint axis, short value, InputKey key)
        {
            this.Axis = axis;
            this.Value = value;
            this.Key = key;
        }

        [XmlAttribute("id")]
        public uint Axis { get; set; }

        [XmlAttribute("value")]
        public short Value { get; set; }

        [XmlText]
        public InputKey Key { get; set; }

        public FunctionType FunctionType
        {
            get { return FunctionType.Axis; }
        }

        public override string ToString()
        {
            return this.Axis.ToString();
        }
    }
}