namespace KeyboardSplitter.Models
{
    using KeyboardSplitter.Enums;
using KeyboardSplitter.Presets;
using SplitterCore.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType("game")]
    public class GameData : INotifyPropertyChanged
    {
        private ImageSource gameIcon;

        private string gameTitle;

        private GameItemStatus status;

        private string executablePath;

        private List<Keyboard> keyboards;

        private List<Mouse> mice;

        private List<Preset> presets;

        public GameData()
        {
            this.status = GameItemStatus.OK;
        }

        public GameData(ImageSource icon, string title)
            : this()
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            this.GameIcon = icon;
            this.GameTitle = title;
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
        public GameItemStatus Status
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
        public string ExecutablePath
        {
            get
            {
                return this.executablePath;
            }

            set
            {
                this.executablePath = value;
                this.OnPropertyChanged("ExecutablePath");
                this.UpdateStatus();
            }
        }

        [XmlElement("keyboards")]
        public List<Keyboard> Keyboards
        {
            get
            {
                return this.keyboards;
            }

            set
            {
                this.keyboards = value;
                this.OnPropertyChanged("Keyboards");
            }
        }

        [XmlElement("mice")]
        public List<Mouse> Mice
        {
            get
            {
                return this.mice;
            }

            set
            {
                this.mice = value;
                this.OnPropertyChanged("Mice");
            }
        }

        [XmlElement("presets")]
        public List<Preset> Presets
        {
            get
            {
                return this.presets;
            }

            set
            {
                this.presets = value;
                this.OnPropertyChanged("Presets");
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

        protected virtual void UpdateStatus()
        {
            if (this.executablePath == null || !File.Exists(this.executablePath))
            {
                this.Status = GameItemStatus.Broken;
                return;
            }

            // TODO: check if selected input devices are currently availabe
            // TODO: check if selected presets are currently availabe

            this.Status = GameItemStatus.OK;
        }
    }
}
