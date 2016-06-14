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
        [TestInitialize]
        public void Init()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerConstructorZero()
        {
            using (var manager = new EmulationManager(0))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerConstructorBigIndex()
        {
            using (var manager = new EmulationManager(5))
            {
            }
        }

        [TestMethod]
        public void TestEmulationManagerAccessoriesNotInstalled()
        {
            if (!DriversManager.IsXboxAccessoriesInstalled())
            {
                bool catched = false;
                var manager = new EmulationManager(slotsCount: 1);

                try
                {
                    manager.Start();
                }
                catch (XboxAccessoriesNotInstalledException)
                {
                    catched = true;
                }
                finally
                {
                    manager.Dispose();
                }

                Assert.IsTrue(catched);
            }
        }

        [TestMethod]
        public void TestEmulationManagerStartWithoutKeyboardSet()
        {
            bool catched = false;
            var manager = new EmulationManager(slotsCount: 1);

            try
            {
                manager.Start();
            }
            catch (KeyboardNotSetException)
            {
                catched = true;
            }
            finally
            {
                manager.Dispose();
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestEmulationManagerStartWithInvalidatedJoyControl()
        {
            bool catched = false;
            var manager = new EmulationManager(slotsCount: 1);

            try
            {
                manager.JoyControls[0].Invalidate(SlotInvalidationReason.Controller_In_Use);
                manager.Start();
            }
            catch (SlotInvalidatedException)
            {
                catched = true;
            }
            finally
            {
                manager.Dispose();
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestEmulationManagerStartNormal()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            bool controllerOnePlugged = false;
            bool controllerTwoPlugged = false;

            using (var manager = new EmulationManager(2))
            {
                foreach (var joyControl in manager.JoyControls)
                {
                    joyControl.SetKeyboard("Keyboard_01");
                }

                manager.Start();

                controllerOnePlugged = VirtualXboxController.Exists(1);
                controllerTwoPlugged = VirtualXboxController.Exists(2);
            }

            Assert.IsTrue(controllerOnePlugged);
            Assert.IsTrue(controllerTwoPlugged);
        }

        ////[TestMethod]
        ////public void TestEmulationManagerProcessKeyPressButton()
        ////{
        ////    if (!DriversManager.AreBuiltInDriversInstalled())
        ////    {
        ////        return;
        ////    }

        ////    bool pressed = false;

        ////    using (var manager = new EmulationManager(1))
        ////    {
        ////        const string KeyboardName = "Keyboard_01";
        ////        manager.JoyControls[0].SetKeyboard(KeyboardName);
        ////        manager.Start();

        ////        var button = XboxButton.A;
        ////        string keyName = Preset.Default.Buttons.Find(x => x.Button == button).KeyboardKey;
        ////        var key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
        ////        var keyState = KeyState.Down;

        ////        var keyEvent = new KeyPressedEventArgs()
        ////        {
        ////            Keyboard = KeyboardManager.GetKeyboards()[0],
        ////            Key = key,
        ////            State = keyState,
        ////            Handled = false,
        ////        };

        ////        manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);
        ////        pressed = VirtualXboxController.GetButtonValue(1, button);
        ////        VirtualXboxController.ResetStates(1);
        ////    }

        ////    Assert.IsTrue(pressed);
        ////}

        ////[TestMethod]
        ////public void TestEmulationManagerProcessKeyPressTrigger()
        ////{
        ////    if (!DriversManager.AreBuiltInDriversInstalled())
        ////    {
        ////        return;
        ////    }

        ////    byte result;
        ////    using (var manager = new EmulationManager(1))
        ////    {
        ////        const string KeyboardName = "Keyboard_01";
        ////        manager.JoyControls[0].SetKeyboard(KeyboardName);
        ////        manager.Start();
        ////        XboxTrigger trigger = XboxTrigger.RightTrigger;
        ////        string keyName = Preset.Default.Triggers.Find(x => x.Trigger == trigger).KeyboardKey;
        ////        InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
        ////        KeyState keyState = KeyState.Down;

        ////        var keyEvent = new KeyPressedEventArgs()
        ////        {
        ////            Keyboard = KeyboardManager.GetKeyboards()[0],
        ////            Key = key,
        ////            State = keyState,
        ////            Handled = false,
        ////        };

        ////        manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);
        ////        result = VirtualXboxController.GetTriggerValue(1, trigger);
        ////        VirtualXboxController.ResetStates(1);
        ////    }

        ////    Assert.AreEqual(byte.MaxValue, result);
        ////}

        ////[TestMethod]
        ////public void TestEmulationManagerProcessKeyPressAxis()
        ////{
        ////    if (!DriversManager.AreBuiltInDriversInstalled())
        ////    {
        ////        return;
        ////    }

        ////    short result;
        ////    XboxAxis axis = XboxAxis.Y;
        ////    XboxAxisPosition position = XboxAxisPosition.Min;
        ////    string keyName = Preset.Default.Axes.Find(x => x.Axis == axis && x.Position == position).KeyboardKey;
        ////    InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
        ////    KeyState keyState = KeyState.Down;

        ////    using (var manager = new EmulationManager(1))
        ////    {
        ////        const string KeyboardName = "Keyboard_01";
        ////        manager.JoyControls[0].SetKeyboard(KeyboardName);
        ////        manager.Start();

        ////        var keyEvent = new KeyPressedEventArgs()
        ////        {
        ////            Keyboard = KeyboardManager.GetKeyboards()[0],
        ////            Key = key,
        ////            State = keyState,
        ////            Handled = false,
        ////        };

        ////        manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);
        ////        result = VirtualXboxController.GetAxisValue(1, axis);
        ////        VirtualXboxController.ResetStates(1);
        ////    }

        ////    Assert.AreEqual((short)position, result);
        ////}

        ////[TestMethod]
        ////public void TestEmulationManagerProcessKeyPressDpad()
        ////{
        ////    if (!DriversManager.AreBuiltInDriversInstalled())
        ////    {
        ////        return;
        ////    }

        ////    XboxDpadDirection direction = XboxDpadDirection.Right;
        ////    string keyName = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
        ////    InterceptionKeys key = (InterceptionKeys)Enum.Parse(typeof(InterceptionKeys), keyName);
        ////    KeyState keyState = KeyState.Down;
        ////    bool pressed = false;

        ////    using (var manager = new EmulationManager(1))
        ////    {
        ////        const string KeyboardName = "Keyboard_01";
        ////        manager.JoyControls[0].SetKeyboard(KeyboardName);
        ////        manager.Start();

        ////        var keyEvent = new KeyPressedEventArgs()
        ////        {
        ////            Keyboard = KeyboardManager.GetKeyboards()[0],
        ////            Key = key,
        ////            State = keyState,
        ////            Handled = false,
        ////        };

        ////        KeyboardManager.SetFakeDown(KeyboardName, keyName);
        ////        manager.ProcessKeyPress(keyEvent, blockChoosenKeyboards: true);
                
        ////        pressed = VirtualXboxController.GetDpadDirectionValue(1, direction);

        ////        VirtualXboxController.ResetStates(1);
        ////        KeyboardManager.ResetFakeStates();
        ////    }

        ////    Assert.IsTrue(pressed);
        ////}
    }
}