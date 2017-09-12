namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType("SplitterSettings")]
    public class GlobalSettings : DependencyObject
    {
        public static readonly DependencyProperty MouseMoveDeadZoneProperty =
            DependencyProperty.Register(
            "MouseMoveDeadZone",
            typeof(int),
            typeof(GlobalSettings),
            new PropertyMetadata(1));

        public static readonly DependencyProperty DisplayEmulationInformationProperty =
            DependencyProperty.Register(
            "DisplayEmulationInformation",
            typeof(bool),
            typeof(GlobalSettings),
            new PropertyMetadata(true));

        public static readonly DependencyProperty SuggestInputDevicesForNewSlotsProperty =
            DependencyProperty.Register(
            "SuggestInputDevicesForNewSlots",
            typeof(bool),
            typeof(GlobalSettings),
            new PropertyMetadata(true));

        public static readonly DependencyProperty StartingVirtualControllerUserIndexProperty =
            DependencyProperty.Register(
            "StartingVirtualControllerUserIndex",
            typeof(int),
            typeof(GlobalSettings),
            new PropertyMetadata(1));
        
        private const string SettingsFileName = "splitter_settings.xml";

        static GlobalSettings()
        {
            GlobalSettings.Instance = new GlobalSettings();
        }

        [XmlIgnore]
        public static GlobalSettings Instance
        {
            get;
            private set;
        }

        [XmlIgnore]
        public static bool IsMainWindowActivated
        {
            get;
            set;
        }

        [XmlElement("MouseMoveDeadZone")]
        public int MouseMoveDeadZone
        {
            get
            {
                return (int)this.GetValue(MouseMoveDeadZoneProperty);
            }

            set
            {
                this.SetValue(MouseMoveDeadZoneProperty, value);
                Interceptor.Interception.MouseMoveDeadZone = value;
            }
        }

        [XmlElement("DisplayEmulationInformation")]
        public bool DisplayEmulationInformation
        {
            get { return (bool)this.GetValue(DisplayEmulationInformationProperty); }
            set { this.SetValue(DisplayEmulationInformationProperty, value); }
        }

        [XmlElement("SuggestInputDevicesForNewSlots")]
        public bool SuggestInputDevicesForNewSlots
        {
            get { return (bool)this.GetValue(SuggestInputDevicesForNewSlotsProperty); }
            set { this.SetValue(SuggestInputDevicesForNewSlotsProperty, value); }
        }

        [XmlElement("StartingVirtualControllerUserIndex")]
        public int StartingVirtualControllerUserIndex
        {
            get { return (int)this.GetValue(StartingVirtualControllerUserIndexProperty); }
            set { this.SetValue(StartingVirtualControllerUserIndexProperty, value); }
        }

        /// <summary>
        /// Saves the settings to file without raising an exception and logs the result.
        /// </summary>
        public static void TrySaveToFile()
        {
            try
            {
                using (var writer = new StreamWriter(path: GlobalSettings.SettingsFileName, append: false, encoding: Encoding.Unicode))
                {
                    var serializer = new XmlSerializer(Instance.GetType());
                    serializer.Serialize(writer, Instance);
                }

                LogWriter.Write("Settings successfully saved to " + GlobalSettings.SettingsFileName);
            }
            catch (Exception e)
            {
                LogWriter.Write("Failed to serialize Global Settings:");
                LogWriter.Write(e.ToString());
            }
        }

        /// <summary>
        /// Reads the settings from file and tries to apply them without raising an exception.
        /// </summary>
        public static bool TryApplySettings()
        {
            if (!File.Exists(GlobalSettings.SettingsFileName))
            {
                LogWriter.Write("Settings file not found");
                return false;
            }

            try
            {
                LogWriter.Write("Trying to apply app [settings]");
                var data = GlobalSettings.Deserialize();
                if (data.MouseMoveDeadZone >= Interceptor.Interception.MouseMoveDeadZoneMin &&
                    data.MouseMoveDeadZone <= Interceptor.Interception.MouseMoveDeadZoneMax)
                {
                    GlobalSettings.Instance.MouseMoveDeadZone = data.MouseMoveDeadZone;
                    LogWriter.Write("[settings] 'Mouse move dead zone' set to: " + data.MouseMoveDeadZone);
                }
                else
                {
                    LogWriter.Write(
                        string.Format(
                        "[settings] Ignored 'Mouse move dead zone', because the value '{0}' is out of range! You must use values in range {1}-{2}",
                        data.MouseMoveDeadZone,
                        Interceptor.Interception.MouseMoveDeadZoneMin,
                        Interceptor.Interception.MouseMoveDeadZoneMax));
                }

                GlobalSettings.Instance.DisplayEmulationInformation = data.DisplayEmulationInformation;
                LogWriter.Write("[settings] 'Display emulation information' set to: " + data.DisplayEmulationInformation);
                GlobalSettings.Instance.SuggestInputDevicesForNewSlots = data.SuggestInputDevicesForNewSlots;
                LogWriter.Write("[settings] 'Suggest input devices for new slots' set to: " + data.SuggestInputDevicesForNewSlots);
                if (data.StartingVirtualControllerUserIndex >= 1 && data.StartingVirtualControllerUserIndex <= 4)
                {
                    GlobalSettings.Instance.StartingVirtualControllerUserIndex = data.StartingVirtualControllerUserIndex;
                    LogWriter.Write("[settings] 'Starting virtual controller user index' set to: " + data.StartingVirtualControllerUserIndex);
                }
                else
                {
                    LogWriter.Write(
                        string.Format(
                        "[settings] Ignored 'Starting virtual controller user index', because the value '{0}' is out of range! You must use values in range 1-4",
                        data.StartingVirtualControllerUserIndex));
                }

                return true;
            }
            catch (Exception e)
            {
                LogWriter.Write(string.Format("Failed to apply settings from {0}:", GlobalSettings.SettingsFileName));
                LogWriter.Write(e.ToString());
            }

            return false;
        }

        private static GlobalSettings Deserialize()
        {
            if (!File.Exists(GlobalSettings.SettingsFileName))
            {
                throw new FileNotFoundException(GlobalSettings.SettingsFileName);
            }

            var serializer = new XmlSerializer(typeof(GlobalSettings));
            using (var fileStream = new FileStream(GlobalSettings.SettingsFileName, FileMode.Open))
            {
                var reader = new XmlTextReader(fileStream);
                GlobalSettings data = (GlobalSettings)serializer.Deserialize(reader);
                return data;
            }
        }
    }
}