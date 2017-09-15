namespace KeyboardSplitter.Models
{
    using SplitterCore.Input;
using System;
using System.Xml.Serialization;

    [Serializable]
    public class SlotData
    {
        public SlotData()
        {

        }

        public SlotData(uint slotNumber, uint gamepadUserIndex, Keyboard keyboard, Mouse mouse, string presetName)
        {
            this.SlotNumber = slotNumber;
            this.GamepadUserIndex = gamepadUserIndex;
            this.KeyboardHardwareId = keyboard.HardwareID;
            this.MouseHardwareId = mouse.HardwareID;
            this.PresetName = presetName;
        }

        [XmlAttribute("slot")]
        public uint SlotNumber { get; set; }

        [XmlAttribute("gamepad")]
        public uint GamepadUserIndex { get; set; }

        [XmlAttribute("keyboard")]
        public string KeyboardHardwareId { get; set; }

        [XmlAttribute("mouse")]
        public string MouseHardwareId { get; set; }

        [XmlAttribute("preset")]
        public string PresetName { get; set; }
    }
}
