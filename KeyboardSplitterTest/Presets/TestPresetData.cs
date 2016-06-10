namespace KeyboardSplitterTest.Presets
{
    using System.IO;
    using KeyboardSplitter;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Presets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestPresetData
    {
        private readonly Preset testPreset;

        private readonly string testFileLocation;

        public TestPresetData()
        {
            KeyboardSplitter.App.Initialize();

            this.testPreset = new Preset();
            this.testPreset.Name = "test Preset";
            this.testPreset.Buttons.Add(new PresetButton(XboxButton.Back, "Enter"));
            this.testPreset.Buttons.Add(new PresetButton(XboxButton.RightBumper, "A"));
            this.testPreset.Triggers.Add(new PresetTrigger(XboxTrigger.RightTrigger, "LeftControl"));
            this.testPreset.Axes.Add(new PresetAxis(XboxAxis.Rx, XboxAxisPosition.Min, "None"));
            this.testPreset.Povs.Add(new PresetDpad(XboxDpadDirection.Right, "Zero"));
            this.testPreset.CustomFunctions.Add(new PresetCustom(XboxCustomFunction.Y_Max, "Up"));

            this.testFileLocation = Path.Combine(
                Path.GetTempPath(), ApplicationInfo.AppNameVersion, "preset_serialize_test.xml");
        }

        [TestMethod]
        public void TestPresetDataSerialize()
        {
            this.Serialize();

            Assert.IsTrue(File.Exists(this.testFileLocation));

            string actual = File.ReadAllText(this.testFileLocation);

            string expected;
            using (var stream = this.GetType().Assembly.GetManifestResourceStream(
                "KeyboardSplitterTest.Presets.Hardcoded_preset_test_result.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPresetDataDeserialize()
        {
            this.Serialize();

            PresetData data = PresetData.Deserialize(this.testFileLocation);
            foreach (var item in Preset.ImuttablePresets)
            {
                data.Presets.Remove(item);
            }

            Preset deserializedPreset = data.Presets[0];

            Assert.IsTrue(deserializedPreset.Name == "test Preset");
            Assert.IsTrue(deserializedPreset.Buttons[0].Button == XboxButton.Back);
            Assert.IsTrue(deserializedPreset.Buttons[0].KeyboardKey == "Enter");
            Assert.IsTrue(deserializedPreset.Buttons[1].Button == XboxButton.RightBumper);
            Assert.IsTrue(deserializedPreset.Buttons[1].KeyboardKey == "A");
            Assert.IsTrue(deserializedPreset.Axes[0].Axis == XboxAxis.Rx);
            Assert.IsTrue(deserializedPreset.Axes[0].Position == XboxAxisPosition.Min);
            Assert.IsTrue(deserializedPreset.Axes[0].KeyboardKey == "None");
            Assert.IsTrue(deserializedPreset.Triggers[0].Trigger == XboxTrigger.RightTrigger);
            Assert.IsTrue(deserializedPreset.Triggers[0].KeyboardKey == "LeftControl");
            Assert.IsTrue(deserializedPreset.Povs[0].Direction == XboxDpadDirection.Right);
            Assert.IsTrue(deserializedPreset.Povs[0].KeyboardKey == "Zero");
            Assert.IsTrue(deserializedPreset.CustomFunctions[0].Function == XboxCustomFunction.Y_Max);
            Assert.IsTrue(deserializedPreset.CustomFunctions[0].KeyboardKey == "Up");

            File.Delete(this.testFileLocation);
        }

        private void Serialize()
        {
            PresetData data = new PresetData();
            data.Presets.Add(this.testPreset);

            data.Serialize(this.testFileLocation);
        }
    }
}