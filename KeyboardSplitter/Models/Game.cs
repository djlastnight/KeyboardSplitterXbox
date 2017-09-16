namespace KeyboardSplitter.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml.Serialization;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Helpers;
    using KeyboardSplitter.Presets;

    [Serializable]
    [XmlType("Game")]
    public class Game : INotifyPropertyChanged
    {
        private ImageSource gameIcon;

        private string gameTitle;

        private string gameNotes;

        private GameStatus status;

        private string gamePath;

        private List<SlotData> slotsData;

        public Game()
        {
        }

        public Game(string title, string path, string gameNotes, List<SlotData> slotsData)
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

            if (slotsData == null)
            {
                throw new ArgumentNullException("slotsData");
            }

            this.SlotsData = slotsData;
            this.GameTitle = title;
            this.GameNotes = gameNotes;
            this.GamePath = path;
        }

        public delegate bool GameStartHandler(Game game);

        public event PropertyChangedEventHandler PropertyChanged;

        public event GameStartHandler GameAboutToStart;

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

        [XmlAttribute("Title")]
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

        [XmlAttribute("Notes")]
        public string GameNotes
        {
            get
            {
                return this.gameNotes;
            }

            set
            {
                this.gameNotes = value;
                this.OnPropertyChanged("GameNotes");
            }
        }

        [XmlIgnore]
        public GameStatus Status
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

        [XmlAttribute("Path")]
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
                this.GameIcon = this.RetrieveGameIcon();
            }
        }

        [XmlElement("Slot")]
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

        public void UpdateStatus()
        {
            this.Status = this.GetStatus();
        }

        public void Play()
        {
            if (!File.Exists(this.gamePath))
            {
                Controls.MessageBox.Show(
                    this.gamePath + " does not exsists!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (this.status != GameStatus.OK)
            {
                var errorMessage = new Converters.GameStatusToStringConverter().Convert(this.status, typeof(string), null, null);
                Controls.MessageBox.Show(
                    "Can not start the game, because of the following error: " + errorMessage,
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (this.GameAboutToStart == null)
            {
                throw new InvalidOperationException("You must subscribe to GameAboutToStart event, before calling Play()");
            }

            bool startTheGame = this.GameAboutToStart(this);

            if (startTheGame)
            {
                try
                {
                    Process.Start(this.gamePath);
                }
                catch (Exception ex)
                {
                    LogWriter.Write(ex.ToString());
                    Controls.MessageBox.Show(
                        "Unable to start " + this.gameTitle + ". See " + LogWriter.GetLogFileName + " for more details!",
                        ApplicationInfo.AppName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        public override string ToString()
        {
            return this.gameTitle;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private GameStatus GetStatus()
        {
            if (this.gamePath == null)
            {
                // Serializer is creating empty SlotsData collection
                return GameStatus.NotSet;
            }

            if (!File.Exists(this.gamePath) || Path.GetExtension(this.gamePath).ToLower() != ".exe")
            {
                return GameStatus.ExeNotFound;
            }

            if (this.slotsData.Count < 1 || this.slotsData.Count > 4)
            {
                return GameStatus.InvalidSlotsCount;
            }

            foreach (var slotData in this.slotsData)
            {
                if (slotData.SlotNumber < 1 || slotData.SlotNumber > 4)
                {
                    return GameStatus.InvalidSlotNumber;
                }

                if (slotData.GamepadUserIndex < 1 || slotData.GamepadUserIndex > 4)
                {
                    return GameStatus.InvalidGamepadUserIndex;
                }

                var inputDevices = KeyboardSplitter.Managers.InputManager.ConnectedInputDevices;
                if (slotData.KeyboardHardwareId != string.Empty)
                {
                    if (inputDevices.FirstOrDefault(x => x.IsKeyboard && x.HardwareID.ToLower() == slotData.KeyboardHardwareId.ToLower()) == null)
                    {
                        // Keyboard is not connected
                        return GameStatus.KeyboardMissing;
                    }
                }

                if (slotData.MouseHardwareId != string.Empty)
                {
                    if (inputDevices.FirstOrDefault(x => !x.IsKeyboard && x.HardwareID.ToLower() == slotData.MouseHardwareId.ToLower()) == null)
                    {
                        // Mouse is not connected
                        return GameStatus.MouseMissing;
                    }
                }

                if (!PresetDataManager.IsProtectedPreset(slotData.PresetName))
                {
                    if (PresetDataManager.CurrentPresets.FirstOrDefault(x => x.Name.ToLower() == slotData.PresetName) == null)
                    {
                        // Preset unavailable
                        return GameStatus.PresetMissing;
                    }
                }
            }

            return GameStatus.OK;
        }

        private ImageSource RetrieveGameIcon()
        {
            try
            {
                return Icon.ExtractAssociatedIcon(this.gamePath).ToImageSource();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
