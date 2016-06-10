namespace KeyboardSplitterTest.Controls
{
    using System;
    using System.Linq;
    using Interceptor;
    using KeyboardSplitter;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestJoyControl
    {
        public TestJoyControl()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestJoyControlValidIndex()
        {
            LogWriter.Write("TestJoyControlValidIndex");
            var joyControl = new JoyControl(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestJoyControlHugeIndex()
        {
            LogWriter.Write("TestJoyControlHugeIndex");
            var joyControl = new JoyControl(uint.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestJoyControlZeroIndex()
        {
            LogWriter.Write("TestJoyControlZeroIndex");
            var joyControl = new JoyControl(0);
        }

        [TestMethod]
        public void TestJoyControlDefaultChildrenCount()
        {
            LogWriter.Write("TestJoyControlDefaultChildrenCount");
            using (var joyControl = new JoyControl(1))
            {
                int regularChildrenCount = 0;
                int removeableChildrenCount = 0;
                foreach (var keyControl in joyControl.Children)
                {
                    if (!keyControl.IsRemoveable)
                    {
                        regularChildrenCount++;
                    }
                    else
                    {
                        removeableChildrenCount++;
                    }
                }

                Assert.AreEqual(25, regularChildrenCount);
                Assert.AreEqual(1, removeableChildrenCount);
            }
        }

        [TestMethod]
        public void TestInitPreset()
        {
            LogWriter.Write("TestInitPreset");
            var joyControl = new JoyControl(1);
            Assert.AreEqual(Preset.Default, joyControl.CurrentPreset);
        }

        [TestMethod]
        public void TestPresetLoad()
        {
            LogWriter.Write("TestPresetLoad");
            var newPreset = new Preset();
            newPreset.Name = "testPreset";
            PresetDataManager.Presets.Add(newPreset);

            var joyControl = new JoyControl(1);
            joyControl.CurrentPreset = newPreset;

            Assert.AreEqual(newPreset, joyControl.CurrentPreset);
        }

        [TestMethod]
        public void TestUnlistedPresetLoad()
        {
            LogWriter.Write("TestUnlistedPresetLoad");
            var joyControl = new JoyControl(1);

            joyControl.CurrentPreset = new Preset()
            {
                Name = "unlistedPreset"
            };

            ///preset should remain the default preset,
            ///because the presetsBox items source is binded to
            ///PresetsManager.Presets, which does not contain the new preset.
            ///therefore this will not fire the presetBox selectedIndex changed event,
            ///which calls the LoadPreset() method.
            Assert.AreEqual(Preset.Default, joyControl.CurrentPreset);
        }

        [TestMethod]
        public void TestPresetSave()
        {
            LogWriter.Write("TestPresetSave");
            var newPreset = new Preset();
            newPreset.Name = "JoyControl save preset test";
            newPreset.Reset();
            PresetDataManager.Presets.Clear();
            foreach (Preset preset in Preset.ImuttablePresets)
            {
                PresetDataManager.Presets.Add(preset);
            }

            PresetDataManager.Presets.Add(newPreset);

            using (var joyControl = new JoyControl(1))
            {
                joyControl.CurrentPreset = newPreset;

                // enabling the save
                string key = InterceptionKeys.Space.ToString();
                joyControl.Children.Find(x => x.Button == XboxButton.LeftThumb).KeyGesture = key;
                Assert.IsTrue(joyControl.CanSavePreset);

                // saving the modified preset
                joyControl.SaveCurrentPreset(silently: true);
                Assert.IsFalse(PresetDataManager.Presets.Contains(newPreset));
                var savedPreset = PresetDataManager.Presets.FirstOrDefault(x => x.Name == newPreset.Name);
                Assert.IsNotNull(savedPreset);
                Assert.AreEqual(key, savedPreset.Buttons.Find(x => x.Button == XboxButton.LeftThumb).KeyboardKey);
            }

            PresetDataManager.Presets.Remove(newPreset);
        }

        [TestMethod]
        public void TestJoyControlInvalidateWhenControllerExists()
        {
            LogWriter.Write("TestJoyControlInvalidateWhenControllerExists");
            const uint UserIndex = 1;
            if (!VirtualXboxController.PlugIn(UserIndex))
            {
                return;
            }

            using (var joyControl = new JoyControl(UserIndex))
            {
                Assert.AreEqual(
                    VirtualXboxController.Exists(UserIndex),
                    joyControl.IsInvalidated);

                VirtualXboxController.UnPlug(UserIndex, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSetInvalidKeyboard()
        {
            LogWriter.Write("TestSetInvalidKeyboard");
            var joyControl = new JoyControl(1);
            joyControl.SetKeyboard("SomethingWrong");
        }

        [TestMethod]
        public void TestSetValidKeyboard()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            LogWriter.Write("TestSetValidKeyboard");
            var joyControl = new JoyControl(1);
            joyControl.SetKeyboard("Keyboard_01");

            Assert.AreEqual(joyControl.CurrentKeyboard, "Keyboard_01");
        }

        [TestMethod]
        public void TestCustomPresetChildrenCount()
        {
            LogWriter.Write("TestCustomPresetChildrenCount");
            var customPreset = new Preset();
            customPreset.Name = "Custom Preset Test!";
            customPreset.CustomFunctions.Add(
                new PresetCustom(XboxCustomFunction.LeftBumper, "Enter"));
            customPreset.CustomFunctions.Add(
                new PresetCustom(XboxCustomFunction.Guide, "Zero"));

            var joyControl = new JoyControl(userIndex: 1);
            joyControl.CurrentPreset = customPreset;

            Assert.AreEqual(28, joyControl.Children.Count);

            Assert.IsTrue(joyControl.Children.Exists(x => x.KeyGesture == "Enter"
                && x.CustomFunction == XboxCustomFunction.LeftBumper));

            Assert.IsTrue(joyControl.Children.Exists(x => x.KeyGesture == "Zero"
                && x.CustomFunction == XboxCustomFunction.Guide));
        }

        [TestMethod]
        public void TestEnableSaveOnKeyGestureChange()
        {
            LogWriter.Write("TestEnableSaveOnKeyGestureChange");
            PresetDataManager.Presets.Add(new Preset() { Name = "test" });
            var joyControl = new JoyControl(1);

            // saving should be enabled for not protected preset
            joyControl.CurrentPreset = PresetDataManager.Presets.First(x => x.Name == "test");
            joyControl.Children[3].KeyGesture = InterceptionKeys.Four.ToString();

            Assert.IsTrue(joyControl.CanSavePreset);

            // saving should be disabled for the default preset
            joyControl.CurrentPreset = Preset.Default;
            joyControl.Children[8].KeyGesture = InterceptionKeys.Home.ToString();

            Assert.IsFalse(joyControl.CanSavePreset);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPresetLoad()
        {
            LogWriter.Write("TestNullPresetLoad");
            using (var joyControl = new JoyControl(1))
            {
                joyControl.CurrentPreset = null;
            }
        }

        [TestMethod]
        public void TestInvalidate()
        {
            LogWriter.Write("TestInvalidate");
            using (var joyControl = new JoyControl(1))
            {
                joyControl.Invalidate(KeyboardSplitter.Enums.SlotInvalidationReason.XboxBus_Not_Installed);
                Assert.IsTrue(joyControl.IsInvalidated);
            }
        }
    }
}
