////namespace KeyboardSplitterTest.Controls
////{
////    using System;
////    using System.Linq;
////    using Interceptor;
////    using KeyboardSplitter;
////    using KeyboardSplitter.Controls;
////    using KeyboardSplitter.Enums;
////    using KeyboardSplitter.Managers;
////    using KeyboardSplitter.Presets;
////    using Microsoft.VisualStudio.TestTools.UnitTesting;
////    using XboxInterfaceWrap;

////    [TestClass]
////    public class TestJoyControl
////    {
////        [TestInitialize]
////        public void Init()
////        {
////            KeyboardSplitter.App.Initialize();
////        }

////        [TestMethod]
////        public void TestJoyControlValidIndex()
////        {
////            using (var joyControl = new JoyControl(1))
////            {
////            }
////        }

////        [TestMethod]
////        [ExpectedException(typeof(ArgumentOutOfRangeException))]
////        public void TestJoyControlHugeIndex()
////        {
////            using (var joyControl = new JoyControl(uint.MaxValue))
////            {
////            }
////        }

////        [TestMethod]
////        [ExpectedException(typeof(ArgumentOutOfRangeException))]
////        public void TestJoyControlZeroIndex()
////        {
////            using (var joyControl = new JoyControl(0))
////            {
////            }
////        }

////        [TestMethod]
////        public void TestJoyControlDefaultChildrenCount()
////        {
////            int regularChildrenCount = 0;
////            int removeableChildrenCount = 0;

////            using (var joyControl = new JoyControl(1))
////            {
////                foreach (var keyControl in joyControl.Children)
////                {
////                    if (!keyControl.IsRemoveable)
////                    {
////                        regularChildrenCount++;
////                    }
////                    else
////                    {
////                        removeableChildrenCount++;
////                    }
////                }
////            }

////            Assert.AreEqual(25, regularChildrenCount);
////            Assert.AreEqual(1, removeableChildrenCount);
////        }

////        [TestMethod]
////        public void TestInitPreset()
////        {
////            Preset actualPreset;

////            using (var joyControl = new JoyControl(1))
////            {
////                actualPreset = joyControl.CurrentPreset;
////            }

////            Assert.AreEqual(Preset.Default, actualPreset);
////        }

////        [TestMethod]
////        public void TestPresetLoad()
////        {
////            var newPreset = new Preset();
////            newPreset.Name = "testPreset";
////            PresetDataManager.Presets.Add(newPreset);
////            Preset actualPreset;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.CurrentPreset = newPreset;
////                actualPreset = joyControl.CurrentPreset;
////            }

////            PresetDataManager.Presets.Remove(newPreset);

////            Assert.AreEqual(newPreset, actualPreset);
////        }

////        [TestMethod]
////        public void TestUnlistedPresetLoad()
////        {
////            Preset actualPreset;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.CurrentPreset = new Preset()
////                {
////                    Name = "unlistedPreset"
////                };

////                actualPreset = joyControl.CurrentPreset;
////            }

////            ///preset should remain the default preset,
////            ///because the presetsBox items source is binded to
////            ///PresetsManager.Presets, which does not contain the new preset.
////            ///therefore this will not fire the presetBox selectedIndex changed event,
////            ///which calls the LoadPreset() method.
////            Assert.AreEqual(Preset.Default, actualPreset);
////        }

////        [TestMethod]
////        public void TestPresetSave()
////        {
////            var newPreset = new Preset();
////            newPreset.Name = "JoyControl save preset test";
////            newPreset.Reset();

////            PresetDataManager.Presets.Add(newPreset);
////            string key = InterceptionKeys.Space.ToString();
////            Preset savedPreset;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.CurrentPreset = newPreset;

////                // enabling the save
////                joyControl.Children.Find(x => x.Button == XboxButton.LeftThumb).KeyGesture = key;

////                // saving the modified preset
////                joyControl.SaveCurrentPreset(silently: true);

////                savedPreset = PresetDataManager.Presets.First(x => x.Name == newPreset.Name);
////                PresetDataManager.Presets.Remove(newPreset);
////            }

////            string actualKey = savedPreset.Buttons.Find(x => x.Button == XboxButton.LeftThumb).KeyboardKey;

////            Assert.AreEqual(key, actualKey);
////        }

////        [TestMethod]
////        public void TestJoyControlInvalidateWhenControllerExists()
////        {
////            const uint UserIndex = 1;
////            if (!VirtualXboxController.PlugIn(UserIndex))
////            {
////                return;
////            }

////            bool exists;
////            bool isInvalidated;

////            using (var joyControl = new JoyControl(UserIndex))
////            {
////                exists = VirtualXboxController.Exists(UserIndex);
////                isInvalidated = joyControl.IsInvalidated;
////            }

////            VirtualXboxController.UnPlug(UserIndex, force: true);

////            Assert.AreEqual(exists, isInvalidated);
////        }

////        [TestMethod]
////        [ExpectedException(typeof(InvalidOperationException))]
////        public void TestSetInvalidKeyboard()
////        {
////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.SetKeyboard("SomethingWrong");
////            }
////        }

////        [TestMethod]
////        public void TestSetValidKeyboard()
////        {
////            if (!DriversManager.AreBuiltInDriversInstalled())
////            {
////                return;
////            }

////            string currentKeyboard;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.SetKeyboard("Keyboard_01");
////                currentKeyboard = joyControl.CurrentKeyboard;
////            }

////            Assert.AreEqual(currentKeyboard, "Keyboard_01");
////        }

////        [TestMethod]
////        public void TestCustomPresetChildrenCount()
////        {
////            var customPreset = new Preset();
////            customPreset.Name = "Custom Preset Test!";
////            customPreset.CustomFunctions.Add(
////                new PresetCustom(XboxCustomFunction.LeftBumper, "Enter"));
////            customPreset.CustomFunctions.Add(
////                new PresetCustom(XboxCustomFunction.Guide, "Zero"));

////            KeyControl[] children;
////            using (var joyControl = new JoyControl(userIndex: 1))
////            {
////                joyControl.CurrentPreset = customPreset;
////                children = joyControl.Children.ToArray();
////            }

////            Assert.AreEqual(28, children.Length);

////            Assert.IsTrue(children.ToList().Exists(x => x.KeyGesture == "Enter"
////                && x.CustomFunction == XboxCustomFunction.LeftBumper));

////            Assert.IsTrue(children.ToList().Exists(x => x.KeyGesture == "Zero"
////                && x.CustomFunction == XboxCustomFunction.Guide));
////        }

////        [TestMethod]
////        public void TestEnableSaveOnKeyGestureChange()
////        {
////            PresetDataManager.Presets.Add(new Preset() { Name = "test" });

////            bool canSave;
////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.CurrentPreset = PresetDataManager.Presets.First(x => x.Name == "test");
////                joyControl.Children[3].KeyGesture = InterceptionKeys.Zero.ToString();
////                canSave = joyControl.CanSavePreset;
////            }

////            Assert.IsTrue(canSave);
////        }

////        [TestMethod]
////        public void TestSaveProtectedPreset()
////        {
////            bool canSave;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.Children[8].KeyGesture = InterceptionKeys.Home.ToString();
////                canSave = joyControl.CanSavePreset;
////            }

////            Assert.IsFalse(canSave);
////        }

////        [TestMethod]
////        [ExpectedException(typeof(ArgumentNullException))]
////        public void TestNullPresetLoad()
////        {
////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.CurrentPreset = null;
////            }
////        }

////        [TestMethod]
////        public void TestInvalidate()
////        {
////            bool isInvalidated;

////            using (var joyControl = new JoyControl(1))
////            {
////                joyControl.Invalidate(
////                    KeyboardSplitter.Enums.SlotInvalidationReason.XboxBus_Not_Installed);

////                isInvalidated = joyControl.IsInvalidated;
////            }

////            Assert.IsTrue(isInvalidated);
////        }
////    }
////}
