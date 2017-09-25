namespace KeyboardSplitter.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Xml.Serialization;
    using KeyboardSplitter.Controls;
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

        private ObservableCollection<SlotData> slotsData;

        public Game()
        {
        }

        public Game(string title, string path, string gameNotes, ObservableCollection<SlotData> slotsData)
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

        public event PropertyChangedEventHandler PropertyChanged;

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
        public ObservableCollection<SlotData> SlotsData
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

        public void TryStart()
        {
            var splitter = Helpers.SplitterHelper.TryFindSplitter();
            if (splitter == null)
            {
                throw new Exception("Unable to find the splitter");
            }

            if (splitter.EmulationManager == null)
            {
                throw new Exception("Splitter's Emulation manager is null");
            }

            if (splitter.EmulationManager.IsEmulationStarted)
            {
                throw new Exception("You must stop emulation, before starting a game!");
            }

            if (!File.Exists(this.gamePath))
            {
                throw new Exception("Can not run a game with non exsisting exe file!");
            }

            this.UpdateStatus();

            if (this.status != GameStatus.OK)
            {
                var errorMessage = new Converters.GameStatusToStringConverter().Convert(this.status, typeof(string), null, null);
                throw new Exception("Can not run the game: " + errorMessage);
            }

            Process.Start(this.gamePath);

            var slots = new ObservableCollection<SplitterCore.Emulation.IEmulationSlot>();
            foreach (var slotData in this.SlotsData)
            {
                var keyboard = SplitterCore.Input.Keyboard.None;
                var mouse = SplitterCore.Input.Mouse.None;

                if (slotData.KeyboardHardwareId != string.Empty)
                {
                    keyboard = splitter.InputManager.Keyboards.Find(x => x.HardwareID == slotData.KeyboardHardwareId);
                }

                if (slotData.MouseHardwareId != string.Empty)
                {
                    mouse = splitter.InputManager.Mice.Find(x => x.HardwareID == slotData.MouseHardwareId);
                }

                var preset = PresetDataManager.CurrentPresets.First(x => x.Name == slotData.PresetName);
                slots.Add(new EmulationSlot(slotData.SlotNumber, new XboxGamepad(slotData.GamepadUserIndex), keyboard, mouse, preset));
            }

            var mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.SlotsCount = slots.Count;
            splitter.EmulationManager.Slots.Clear();
            foreach (var slot in slots)
            {
                splitter.EmulationManager.Slots.Add(slot);
            }

            splitter.EmulationManager.Start(true);
        }

        public void UpdateStatus()
        {
            this.Status = this.GetStatus();
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
