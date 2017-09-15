namespace KeyboardSplitter.Models
{
    using KeyboardSplitter.Enums;
using KeyboardSplitter.Presets;
using SplitterCore.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
    using System.Linq;
using System.Windows;
using System.Windows.Media;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType("game")]
    public class GameData : INotifyPropertyChanged
    {
        private ImageSource gameIcon;

        private string gameTitle;

        private GameDataStatus status;

        private string gamePath;

        private List<SlotData> slotsData;

        public GameData()
        {
        }

        public GameData(string title, string path)
            : this()
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            this.GameTitle = title;
            this.GamePath = path;
        }

        [XmlIgnore]
        public ImageSource GameIcon
        {
            get
            {
                return this.gameIcon;
            }

            set
            {
                this.gameIcon = value;
                this.OnPropertyChanged("GameIcon");
            }
        }

        [XmlAttribute("title")]
        public string GameTitle
        {
            get
            {
                return this.gameTitle;
            }

            set
            {
                this.gameTitle = value;
                this.OnPropertyChanged("GameTitle");
            }
        }

        [XmlIgnore]
        public GameDataStatus Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                this.status = value;
                this.OnPropertyChanged("Status");
            }
        }

        [XmlAttribute("path")]
        public string GamePath
        {
            get
            {
                return this.gamePath;
            }

            set
            {
                this.gamePath = value;
                this.OnPropertyChanged("GamePath");
                this.Status = this.GetStatus();
            }
        }

        [XmlElement("slots")]
        public List<SlotData> SlotsData
        {
            get
            {
                return this.slotsData;
            }

            set
            {
                this.slotsData = value;
                this.OnPropertyChanged("SlotsData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual GameDataStatus GetStatus()
        {
            if (this.gamePath == null || !File.Exists(this.gamePath) || Path.GetExtension(this.gamePath).ToLower() != ".exe")
            {
                return GameDataStatus.Broken;
            }

            if (this.slotsData.Count < 1 || this.slotsData.Count > 4)
            {
                return GameDataStatus.Warning;
            }

            foreach (var slotData in this.slotsData)
            {
                if (slotData.SlotNumber < 1 || slotData.SlotNumber > 4)
                {
                    return GameDataStatus.Warning;
                }

                if (slotData.GamepadUserIndex < 1 || slotData.GamepadUserIndex > 4)
                {
                    return GameDataStatus.Warning;
                }

                var inputDevices = KeyboardSplitter.Managers.InputManager.ConnectedInputDevices;
                if (slotData.KeyboardHardwareId != string.Empty)
                {
                    if (inputDevices.FirstOrDefault(x => x.IsKeyboard && x.HardwareID.ToLower() == slotData.KeyboardHardwareId.ToLower()) == null)
                    {
                        // Keyboard is not connected
                        return GameDataStatus.Warning;
                    }
                }

                if (slotData.MouseHardwareId != string.Empty)
                {
                    if (inputDevices.FirstOrDefault(x => !x.IsKeyboard && x.HardwareID.ToLower() == slotData.MouseHardwareId.ToLower()) == null)
                    {
                        // Mouse is not connected
                        return GameDataStatus.Warning;
                    }
                }

                if (!PresetDataManager.IsProtectedPreset(slotData.PresetName))
                {
                    if (PresetDataManager.CurrentPresets.FirstOrDefault(x => x.Name.ToLower() == slotData.PresetName) == null)
                    {
                        // Preset unavailable
                        return GameDataStatus.Warning;
                    }
                }
            }

            return GameDataStatus.OK;
        }
    }
}
