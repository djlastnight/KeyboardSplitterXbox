namespace KeyboardSplitterTest.Managers
{
    using System;
    using Interceptor;
    using KeyboardSplitter;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Exceptions;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestEmulationManager
    {
        public TestEmulationManager()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerConstructorZero()
        {
            LogWriter.Write("TestEmulationManagerConstructorZero");
            using (var manager = new EmulationManager(0))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerConstructorBigIndex()
        {
            LogWriter.Write("TestEmulationManagerConstructorBigIndex");
            using (var manager = new EmulationManager(5))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(XboxAccessoriesNotInstalledException))]
        public void TestEmulationManagerAccessoriesNotInstalled()
        {
            LogWriter.Write("TestEmulationManagerAccessoriesNotInstalled");
            if (!DriversManager.IsXboxAccessoriesInstalled())
            {
                using (var manager = new EmulationManager(slotsCount: 1))
                {
                    manager.Start();
                }
            }
            else
            {
                throw new XboxAccessoriesNotInstalledException(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyboardNotSetException))]
        public void TestEmulationManagerStartWithoutKeyboardSet()
        {
            LogWriter.Write("TestEmulationManagerStartWithoutKeyboardSet");
            using (var manager = new EmulationManager(slotsCount: 1))
            {
                if (manager.JoyControls[0].InvalidateReason ==
                    KeyboardSplitter.Enums.SlotInvalidationReason.XboxBus_Not_Installed)
                {
                    throw new KeyboardNotSetException(
                        "Due to the xbox bus is not installed, this test will always fail.");
                }

                // leaving the keyboard unset
                // manager.JoyControls[0].SetKeyboard("Keyboard_01");
                manager.Start();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SlotInvalidatedException))]
        public void TestEmulationManagerStartWithInvalidatedJoyControl()
        {
            using (var manager = new EmulationManager(slotsCount: 1))
            {
                manager.JoyControls[0].Invalidate(SlotInvalidationReason.Controller_In_Use);
                manager.Start();
            }
        }

        [TestMethod]
        public void TestEmulationManagerStartNormal()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            LogWriter.Write("TestEmulationManagerStartNormal");
            using (var manager = new EmulationManager(2))
            {
                foreach (var item in manager.JoyControls)
                {
                    item.SetKeyboard("Keyboard_01");
                }

                manager.Start();

                Assert.IsTrue(VirtualXboxController.Exists(1));
                Assert.IsTrue(VirtualXboxController.Exists(2));

                Assert.IsTrue(VirtualXboxController.IsOwned(1));
                Assert.IsTrue(VirtualXboxController.IsOwned(2));

                manager.Stop();
            }
        }

        [TestMethod]
        public void TestEmulationManagerProcessKeyPressButton()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var manager = new EmulationManager(1))
            {
                const string KeyboardName = "Keyboard_01";
                manager.JoyControls[0].SetKeyboard(KeyboardName);
                manager.Start();
                XboxButton button = XboxButton.A;
                string keyName = Preset.Default.Buttons.Find(x => x.Button == button).KeyboardKey;
                InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
                KeyState keyState = KeyState.Down;

                var keyEvent = new KeyPressedEventArgs()
                {
                    Keyboard = KeyboardManager.GetKeyboards()[0],
                    Key = key,
                    State = keyState,
                    Handled = false,
                };

                manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);

                Assert.IsTrue(VirtualXboxController.GetButtonValue(1, button));
            }
        }

        [TestMethod]
        public void TestEmulationManagerProcessKeyPressTrigger()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var manager = new EmulationManager(1))
            {
                const string KeyboardName = "Keyboard_01";
                manager.JoyControls[0].SetKeyboard(KeyboardName);
                manager.Start();
                XboxTrigger trigger = XboxTrigger.RightTrigger;
                string keyName = Preset.Default.Triggers.Find(x => x.Trigger == trigger).KeyboardKey;
                InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
                KeyState keyState = KeyState.Down;

                var keyEvent = new KeyPressedEventArgs()
                {
                    Keyboard = KeyboardManager.GetKeyboards()[0],
                    Key = key,
                    State = keyState,
                    Handled = false,
                };

                manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);

                Assert.AreEqual(byte.MaxValue, VirtualXboxController.GetTriggerValue(1, trigger));
            }
        }

        [TestMethod]
        public void TestEmulationManagerProcessKeyPressAxis()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var manager = new EmulationManager(1))
            {
                const string KeyboardName = "Keyboard_01";
                manager.JoyControls[0].SetKeyboard(KeyboardName);
                manager.Start();
                XboxAxis axis = XboxAxis.Y;
                XboxAxisPosition position = XboxAxisPosition.Min;
                string keyName = Preset.Default.Axes.Find(x => x.Axis == axis && x.Position == position).KeyboardKey;
                InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
                KeyState keyState = KeyState.Down;

                var keyEvent = new KeyPressedEventArgs()
                {
                    Keyboard = KeyboardManager.GetKeyboards()[0],
                    Key = key,
                    State = keyState,
                    Handled = false,
                };

                manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);

                Assert.AreEqual((short)position, VirtualXboxController.GetAxisValue(1, axis));
            }
        }

        [TestMethod]
        public void TestEmulationManagerProcessKeyPressDpad()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var manager = new EmulationManager(1))
            {
                const string KeyboardName = "Keyboard_01";
                manager.JoyControls[0].SetKeyboard(KeyboardName);
                manager.Start();
                XboxDpadDirection direction = XboxDpadDirection.Right;
                string keyName = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
                InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
                KeyState keyState = KeyState.Down;

                var keyEvent = new KeyPressedEventArgs()
                {
                    Keyboard = KeyboardManager.GetKeyboards()[0],
                    Key = key,
                    State = keyState,
                    Handled = false,
                };

                KeyboardManager.SetFakeDown(KeyboardName, keyName);
                manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);

                KeyboardManager.ResetFakeStates();
                Assert.IsTrue(VirtualXboxController.GetDpadDirectionValue(1, direction));
            }
        }
    }
}