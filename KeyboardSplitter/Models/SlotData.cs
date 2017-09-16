namespace KeyboardSplitter.Models
{
    using System;
    using System.Xml.Serialization;
    using SplitterCore.Input;

    [Serializable]
    public class SlotData
    {
        public SlotData()
        {
        }

        public SlotData(uint slotNumber, uint gamepadUserIndex, Keyboard keyboard, Mouse mouse, string presetName)
        {
            if (keyboard == null)
            {
                throw new ArgumentNullException("keyboard");
            }

            if (mouse == null)
            {
                throw new ArgumentNullException("mouse");
            }

            if (presetName == null)
            {
                throw new ArgumentNullException("presetName");
            }

            this.SlotNumber = slotNumber;
            this.GamepadUserIndex = gamepadUserIndex;
            this.KeyboardHardwareId = keyboard.HardwareID;
            this.MouseHardwareId = mouse.HardwareID;
            this.PresetName = presetName;
        }

        [XmlAttribute("Number")]
        public uint SlotNumber { get; set; }

        [XmlAttribute("GamepadUserIndex")]
        public uint GamepadUserIndex { get; set; }

        [XmlAttribute("Keyboard")]
        public string KeyboardHardwareId { get; set; }

        [XmlAttribute("Mouse")]
        public string MouseHardwareId { get; set; }

        [XmlAttribute("Preset")]
        public string PresetName { get; set; }
    }
}
