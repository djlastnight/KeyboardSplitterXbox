namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using SplitterCore;
    using SplitterCore.Input;
    using SplitterCore.Preset;
    using VirtualXbox.Enums;

    [Serializable]
    [XmlType("preset")]
    public class Preset : IPreset
    {
        public static readonly Preset Default = CreateDefaultPreset();

        public static readonly List<Preset> ImuttablePresets =
            new List<Preset>() { Default };

        public Preset()
        {
            this.Buttons = new ObservableCollection<PresetButton>();
            this.Triggers = new ObservableCollection<PresetTrigger>();
            this.Axes = new ObservableCollection<PresetAxis>();
            this.Dpads = new ObservableCollection<PresetDpad>();
            this.CustomFunctions = new ObservableCollection<PresetCustom>();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("button")]
        public ObservableCollection<PresetButton> Buttons { get; set; }

        [XmlElement("trigger")]
        public ObservableCollection<PresetTrigger> Triggers { get; set; }

        [XmlElement("axis")]
        public ObservableCollection<PresetAxis> Axes { get; set; }

        [XmlElement("dpad")]
        public ObservableCollection<PresetDpad> Dpads { get; set; }

        [XmlElement("custom")]
        public ObservableCollection<PresetCustom> CustomFunctions { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public IPreset FilterByKey(InputKey key)
        {
            var preset = new Preset()
            {
                Name = this.Name,
                Buttons = new ObservableCollection<PresetButton>(this.Buttons.Where(x => x.Key == key)),
                Triggers = new ObservableCollection<PresetTrigger>(this.Triggers.Where(x => x.Key == key)),
                Axes = new ObservableCollection<PresetAxis>(this.Axes.Where(x => x.Key == key)),
                Dpads = new ObservableCollection<PresetDpad>(this.Dpads.Where(x => x.Key == key)),
                CustomFunctions = new ObservableCollection<PresetCustom>(this.CustomFunctions.Where(x => x.Key == key))
            };

            return preset;
        }

        public List<InputKey> GetKeys(PresetButton presetButton)
        {
            var keys = new List<InputKey>();
            keys.AddRange(this.Buttons.Where(x => x == presetButton).Select(x => x.Key));

            // Adding buttons from custom functions
            foreach (var customFunction in this.CustomFunctions)
            {
                XboxCustomFunction xboxCustomfunction = (XboxCustomFunction)customFunction.Function;
                if (Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomfunction) == FunctionType.Button)
                {
                    uint customXboxButton = (uint)Helpers.CustomFunctionHelper.GetXboxButton(xboxCustomfunction);
                    if (customXboxButton == presetButton.Button)
                    {
                        keys.Add(customFunction.Key);
                    }
                }
            }

            return keys;
        }

        public List<InputKey> GetKeys(PresetTrigger presetTrigger)
        {
            var keys = new List<InputKey>();
            keys.AddRange(this.Triggers.Where(x => x == presetTrigger).Select(x => x.Key));

            // Adding triggers from custom functions
            foreach (var customFunction in this.CustomFunctions)
            {
                XboxCustomFunction xboxCustomfunction = (XboxCustomFunction)customFunction.Function;
                if (Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomfunction) == FunctionType.Trigger)
                {
                    uint customXboxTrigger = (uint)Helpers.CustomFunctionHelper.GetXboxTrigger(xboxCustomfunction);
                    if (customXboxTrigger == presetTrigger.Trigger)
                    {
                        keys.Add(customFunction.Key);
                    }
                }
            }

            return keys;
        }

        public List<InputKey> GetKeys(PresetAxis axis)
        {
            var keys = new List<InputKey>();
            keys.AddRange(this.Axes.Where(x => x.Axis == axis.Axis && x.Value == axis.Value).Select(x => x.Key));

            // Adding dpads from custom functions
            foreach (var customFunction in this.CustomFunctions)
            {
                XboxCustomFunction xboxCustomfunction = (XboxCustomFunction)customFunction.Function;
                if (Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomfunction) == FunctionType.Axis)
                {
                    XboxAxisPosition pos;
                    uint customXboxAxis = (uint)Helpers.CustomFunctionHelper.GetXboxAxis(xboxCustomfunction, out pos);
                    if (customXboxAxis == axis.Axis && (short)pos == axis.Value)
                    {
                        keys.Add(customFunction.Key);
                    }
                }
            }

            return keys;
        }

        public List<InputKey> GetKeys(PresetDpad dpad)
        {
            var keys = new List<InputKey>();
            keys.AddRange(this.Dpads.Where(x => x.Direction == dpad.Direction).Select(x => x.Key));

            // Adding dpads from custom functions
            foreach (var customFunction in this.CustomFunctions)
            {
                XboxCustomFunction xboxCustomfunction = (XboxCustomFunction)customFunction.Function;
                if (Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomfunction) == FunctionType.Dpad)
                {
                    int customXboxDpad = (int)Helpers.CustomFunctionHelper.GetDpadDirection(xboxCustomfunction);
                    if (customXboxDpad == dpad.Direction)
                    {
                        keys.Add(customFunction.Key);
                    }
                }
            }

            return keys;
        }

        public List<InputKey> GetKeys(PresetCustom presetCustom)
        {
            var keys = new List<InputKey>();
            keys.AddRange(this.CustomFunctions.Where(x => x.Function == presetCustom.Function).Select(x => x.Key));

            foreach (var customFunction in this.CustomFunctions)
            {
                if (customFunction.Function != presetCustom.Function)
                {
                    continue;
                }

                var xboxCustomFunction = (XboxCustomFunction)presetCustom.Function;
                var functionType = Helpers.CustomFunctionHelper.GetFunctionType(xboxCustomFunction);
                switch (functionType)
                {
                    case FunctionType.Button:
                        foreach (var presetButton in this.Buttons)
                        {
                            var xboxButton = Helpers.CustomFunctionHelper.GetXboxButton(xboxCustomFunction);
                            if (xboxButton == (XboxButton)presetButton.Button)
                            {
                                keys.Add(presetButton.Key);
                            }
                        }

                        break;
                    case FunctionType.Axis:
                        foreach (var presetAxis in this.Axes)
                        {
                            XboxAxisPosition pos;
                            var xboxAxis = Helpers.CustomFunctionHelper.GetXboxAxis(xboxCustomFunction, out pos);
                            if (xboxAxis == (XboxAxis)presetAxis.Axis && pos == (XboxAxisPosition)presetAxis.Value)
                            {
                                keys.Add(presetAxis.Key);
                            }
                        }

                        break;
                    case FunctionType.Dpad:
                        foreach (var presetDpad in this.Dpads)
                        {
                            var direction = Helpers.CustomFunctionHelper.GetDpadDirection(xboxCustomFunction);
                            if (direction == (XboxDpadDirection)presetDpad.Direction)
                            {
                                keys.Add(presetDpad.Key);
                            }
                        }

                        break;
                    case FunctionType.Trigger:
                        foreach (var presetTrigger in this.Triggers)
                        {
                            var xboxTrigger = Helpers.CustomFunctionHelper.GetXboxTrigger(xboxCustomFunction);
                            if (xboxTrigger == (XboxTrigger)presetTrigger.Trigger)
                            {
                                keys.Add(presetTrigger.Key);
                            }
                        }

                        break;
                    default:
                        throw new NotImplementedException(
                            "Not implemented Function Type: " + functionType);
                }
            }

            return keys;
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                return sb.ToString();
            }
        }

        private static Preset CreateEmptyPreset()
        {
            Preset empty = new Preset();
            empty.Name = "empty";
            empty.Reset();

            return empty;
        }

        private static Preset CreateDefaultPreset()
        {
            Preset preset = new Preset();
            preset.Name = "default";
            preset.Buttons.Add(new PresetButton((uint)XboxButton.Start, InputKey.Escape));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.Back, InputKey.Backspace));

            preset.Buttons.Add(new PresetButton((uint)XboxButton.LeftThumb, InputKey.LeftShift));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.RightThumb, InputKey.RightShift));

            preset.Buttons.Add(new PresetButton((uint)XboxButton.LeftBumper, InputKey.Z));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.RightBumper, InputKey.C));

            preset.Buttons.Add(new PresetButton((uint)XboxButton.Guide, InputKey.LeftWindows));

            preset.Buttons.Add(new PresetButton((uint)XboxButton.A, InputKey.S));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.B, InputKey.D));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.X, InputKey.A));
            preset.Buttons.Add(new PresetButton((uint)XboxButton.Y, InputKey.W));

            preset.Triggers.Add(new PresetTrigger((uint)XboxTrigger.Left, InputKey.Q));
            preset.Triggers.Add(new PresetTrigger((uint)XboxTrigger.Right, InputKey.E));

            preset.Axes.Add(new PresetAxis((uint)XboxAxis.X, (short)XboxAxisPosition.Min, InputKey.Left));
            preset.Axes.Add(new PresetAxis((uint)XboxAxis.X, (short)XboxAxisPosition.Max, InputKey.Right));

            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Y, (short)XboxAxisPosition.Min, InputKey.Down));
            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Y, (short)XboxAxisPosition.Max, InputKey.Up));

            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Rx, (short)XboxAxisPosition.Min, InputKey.Numpad4));
            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Rx, (short)XboxAxisPosition.Max, InputKey.Numpad6));

            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Ry, (short)XboxAxisPosition.Min, InputKey.Numpad2));
            preset.Axes.Add(new PresetAxis((uint)XboxAxis.Ry, (short)XboxAxisPosition.Max, InputKey.Numpad8));

            preset.Dpads.Add(new PresetDpad((int)XboxDpadDirection.Up, InputKey.I));
            preset.Dpads.Add(new PresetDpad((int)XboxDpadDirection.Down, InputKey.K));
            preset.Dpads.Add(new PresetDpad((int)XboxDpadDirection.Left, InputKey.J));
            preset.Dpads.Add(new PresetDpad((int)XboxDpadDirection.Right, InputKey.L));

            preset.CustomFunctions.Add(new PresetCustom((uint)XboxButton.A, InputKey.Enter));

            return preset;
        }
    }
}