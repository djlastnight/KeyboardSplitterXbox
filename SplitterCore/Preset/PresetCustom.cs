namespace SplitterCore.Preset
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    [XmlType("custom")]
    public class PresetCustom : IPresetElement
    {
        public PresetCustom()
        {
        }

        public PresetCustom(uint function, InputKey key)
        {
            this.Function = function;
            this.Key = key;
        }

        [XmlAttribute("function")]
        public uint Function { get; set; }

        [XmlText]
        public InputKey Key { get; set; }

        public FunctionType FunctionType
        {
            get { return FunctionType.Custom; }
        }

        public override string ToString()
        {
            return this.Function.ToString();
        }
    }
}