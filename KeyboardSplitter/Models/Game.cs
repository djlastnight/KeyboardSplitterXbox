namespace KeyboardSplitter.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
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

        private string gameArguments;

        private bool blockKeyboards;

        private bool blockMice;

        private ObservableCollection<SlotData> slotsData;

        public Game()
        {
            this.BlockKeyboards = true;
            this.BlockMice = false;
            this.Arguments = null;
        }

        public Game(string title, string path, string arguments, string gameNotes, bool blockKeyboards, bool blockMice, ObservableCollection<SlotData> slotsData)
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

            this.gameArguments = arguments;
            this.SlotsData = slotsData;
            this.GameTitle = title;
            this.GameNotes = gameNotes;
            this.GamePath = path;
            this.BlockKeyboards = blockKeyboards;
            this.BlockMice = blockMice;
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
                this.RetrieveGameDetails();
            }
        }

        [XmlAttribute("Arguments")]
        public string Arguments
        {
            get
            {
                return this.gameArguments;
            }

            set
            {
                this.gameArguments = value;
                this.OnPropertyChanged("Arguments");
            }
        }

        [XmlAttribute("BlockKeyboards")]
        public bool BlockKeyboards
        {
            get
            {
                return this.blockKeyboards;
            }

            set
            {
                this.blockKeyboards = value;
                this.OnPropertyChanged("BlockKeyboards");
            }
        }

        [XmlAttribute("BlockMice")]
        public bool BlockMice
        {
            get
            {
                return this.blockMice;
            }

            set
            {
                this.blockMice = value;
                this.OnPropertyChanged("BlockMice");
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

            splitter.ShouldBlockKeyboards = this.blockKeyboards;
            splitter.ShouldBlockMice = this.blockMice;
            splitter.EmulationManager.Start(true);

            var result = Controls.MessageBox.Show(
                "Your game settings are applied.\r\n\r\nWould you like to run the game now?",
                "Run a Keyboard Splitter Game?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                /*
                var process = Process.Start(this.gamePath);
                process.EnableRaisingEvents = true;
                process.Exited += this.OnProcessExited;
                */
                var now = DateTime.Now;
                Task.Factory.StartNew(() =>
                {
                    var startInfo = new ProcessStartInfo();
                    startInfo.FileName = this.gamePath;
                    startInfo.WorkingDirectory = Path.GetDirectoryName(this.gamePath);
                    if (this.gameArguments != null)
                    {
                        startInfo.Arguments = this.gameArguments;
                    }

                    startInfo.UseShellExecute = true;
                    var process = Process.Start(startInfo);
                    process.WaitForExit();
                }).ContinueWith((task) => 
                    {
                        if ((DateTime.Now - now).TotalSeconds > 3)
                        {
                            this.OnProcessExited();
                        }

                        return task;
                    });
            }
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

        private void OnProcessExited()
        {
            if (App.Current != null && App.Current.Dispatcher != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var splitter = SplitterHelper.TryFindSplitter();
                    if (splitter.EmulationManager == null)
                    {
                        return;
                    }

                    if (splitter.EmulationManager.IsEmulationStarted)
                    {
                        var result = Controls.MessageBox.Show(
                            "'" + this.gameTitle + "' has been closed.\r\n\r\nDo you want to stop the emulation?",
                            ApplicationInfo.AppName,
                            System.Windows.MessageBoxButton.YesNo,
                            System.Windows.MessageBoxImage.Question);

                        if (result == System.Windows.MessageBoxResult.Yes)
                        {
                            splitter.EmulationManager.Stop();
                        }
                    }
                });
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
                    if (PresetDataManager.CurrentPresets.FirstOrDefault(x => x.Name.ToLower() == slotData.PresetName.ToLower()) == null)
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

        private void RetrieveGameDetails()
        {
            if (this.gamePath == null || !File.Exists(this.gamePath))
            {
                return;
            }

            var fileInfo = FileVersionInfo.GetVersionInfo(this.gamePath);

            if (string.IsNullOrEmpty(this.gameTitle))
            {
                this.GameTitle = fileInfo.ProductName;
            }

            if (string.IsNullOrEmpty(this.gameNotes))
            {
                this.GameNotes = fileInfo.FileDescription;
            }
        }
    }
}
