namespace KeyboardSplitter.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Interceptor;
    using XboxInterfaceWrap;

    [Serializable]
    [XmlType("preset")]
    public class Preset
    {
        public static readonly Preset Empty = CreateEmptyPreset();

        public static readonly Preset Default = CreateDefaultPreset();

        public static readonly List<Preset> ImuttablePresets =
            new List<Preset>() { Empty, Default };

        public Preset()
        {
            this.Buttons = new List<PresetButton>();
            this.Triggers = new List<PresetTrigger>();
            this.Axes = new List<PresetAxis>();
            this.Povs = new List<PresetDpad>();
            this.CustomFunctions = new List<PresetCustom>();
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("button")]
        public List<PresetButton> Buttons { get; set; }

        [XmlElement("trigger")]
        public List<PresetTrigger> Triggers { get; set; }

        [XmlElement("axis")]
        public List<PresetAxis> Axes { get; set; }

        [XmlElement("dpad")]
        public List<PresetDpad> Povs { get; set; }

        [XmlElement("custom")]
        public List<PresetCustom> CustomFunctions { get; set; }

        public void Reset()
        {
            string noneKey = InterceptionKeys.None.ToString();

            this.Buttons = new List<PresetButton>();
            foreach (XboxButton button in Enum.GetValues(typeof(XboxButton)))
            {
                this.Buttons.Add(new PresetButton(button, noneKey));
            }

            this.Triggers = new List<PresetTrigger>();
            foreach (XboxTrigger trigger in Enum.GetValues(typeof(XboxTrigger)))
            {
                this.Triggers.Add(new PresetTrigger(trigger, noneKey));
            }

            this.Axes = new List<PresetAxis>();
            foreach (XboxAxis axis in Enum.GetValues(typeof(XboxAxis)))
            {
                this.Axes.Add(new PresetAxis(axis, XboxAxisPosition.Min, noneKey));
                this.Axes.Add(new PresetAxis(axis, XboxAxisPosition.Max, noneKey));
            }

            this.Povs = new List<PresetDpad>();
            foreach (XboxDpadDirection direction in Enum.GetValues(typeof(XboxDpadDirection)))
            {
                if (direction != XboxDpadDirection.None)
                {
                    this.Povs.Add(new PresetDpad(direction, noneKey));
                }
            }

            this.CustomFunctions = new List<PresetCustom>();
        }

        public override string ToString()
        {
            return this.Name;
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

            preset.Buttons.Add(new PresetButton(XboxButton.Guide, InterceptionKeys.LeftWindows.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.A, InterceptionKeys.S.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.B, InterceptionKeys.D.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.X, InterceptionKeys.A.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.Y, InterceptionKeys.W.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.LeftBumper, InterceptionKeys.Z.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.RightBumper, InterceptionKeys.C.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.Back, InterceptionKeys.Backspace.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.Start, InterceptionKeys.Escape.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.LeftThumb, InterceptionKeys.LeftShift.ToString()));
            preset.Buttons.Add(new PresetButton(XboxButton.RightThumb, InterceptionKeys.RightShift.ToString()));

            preset.Triggers.Add(new PresetTrigger(XboxTrigger.LeftTrigger, InterceptionKeys.Q.ToString()));
            preset.Triggers.Add(new PresetTrigger(XboxTrigger.RightTrigger, InterceptionKeys.E.ToString()));

            preset.Axes.Add(new PresetAxis(XboxAxis.X, XboxAxisPosition.Min, InterceptionKeys.Left.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.X, XboxAxisPosition.Max, InterceptionKeys.Right.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.Y, XboxAxisPosition.Max, InterceptionKeys.Up.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.Y, XboxAxisPosition.Min, InterceptionKeys.Down.ToString()));

            preset.Axes.Add(new PresetAxis(XboxAxis.Rx, XboxAxisPosition.Min, InterceptionKeys.Numpad4.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.Rx, XboxAxisPosition.Max, InterceptionKeys.Numpad6.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.Ry, XboxAxisPosition.Min, InterceptionKeys.Numpad2.ToString()));
            preset.Axes.Add(new PresetAxis(XboxAxis.Ry, XboxAxisPosition.Max, InterceptionKeys.Numpad8.ToString()));

            preset.Povs.Add(new PresetDpad(XboxDpadDirection.Up, InterceptionKeys.I.ToString()));
            preset.Povs.Add(new PresetDpad(XboxDpadDirection.Down, InterceptionKeys.K.ToString()));
            preset.Povs.Add(new PresetDpad(XboxDpadDirection.Left, InterceptionKeys.J.ToString()));
            preset.Povs.Add(new PresetDpad(XboxDpadDirection.Right, InterceptionKeys.L.ToString()));

            return preset;
        }
    }
}