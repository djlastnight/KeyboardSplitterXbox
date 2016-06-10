namespace KeyboardSplitterTest.Helpers
{
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Helpers;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestPovHelper
    {
        public TestPovHelper()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestPovUpPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var direction = XboxDpadDirection.Up;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, keyGesture);

                var result = PovHelper.CalculatePovDirection(joyControl, direction, KeyState.Down);

                Assert.AreEqual(direction, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestPovDownPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var direction = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, keyGesture);

                var result = PovHelper.CalculatePovDirection(joyControl, direction, KeyState.Down);

                Assert.AreEqual(direction, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestPovLeftPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var direction = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, keyGesture);

                var result = PovHelper.CalculatePovDirection(joyControl, direction, KeyState.Down);

                Assert.AreEqual(direction, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestPovRightPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var direction = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == direction).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, keyGesture);

                var result = PovHelper.CalculatePovDirection(joyControl, direction, KeyState.Down);

                Assert.AreEqual(direction, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpLeftPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpRightPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpDownPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestRightUpPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Right;
                var secondDirection = XboxDpadDirection.Up;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestRightDownPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Right;
                var secondDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestRightLeftPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Right;
                var secondDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestDownUpPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Down;
                var secondDirection = XboxDpadDirection.Up;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestDownRightPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Down;
                var secondDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestDownLeftPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Down;
                var secondDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestLeftUpPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Left;
                var secondDirection = XboxDpadDirection.Up;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestLeftRightPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Left;
                var secondDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestLeftDownPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Left;
                var secondDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

                Assert.AreEqual(firstDirection | secondDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestLeftUpRightPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Left;
                var secondDirection = XboxDpadDirection.Up;
                var thirdDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thirdKey);

                var result = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

                Assert.AreEqual(thirdDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpRightDownPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Right;
                var thirdDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thirdKey);

                var result = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

                Assert.AreEqual(thirdDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestRightDownLeftPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Right;
                var secondDirection = XboxDpadDirection.Down;
                var thirdDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thirdKey);

                var result = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

                Assert.AreEqual(thirdDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestDownLeftUpPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Down;
                var secondDirection = XboxDpadDirection.Left;
                var thirdDirection = XboxDpadDirection.Up;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thirdKey);

                var result = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

                Assert.AreEqual(thirdDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestAllDirectionsPressed()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Right;
                var thirdDirection = XboxDpadDirection.Down;
                var fourthDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                string fourthKey = Preset.Default.Povs.Find(x => x.Direction == fourthDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thirdKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, fourthKey);

                var result = PovHelper.CalculatePovDirection(joyControl, fourthDirection, KeyState.Down);

                Assert.AreEqual(fourthDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpPressedRightReleased()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

                Assert.AreEqual(firstDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpPressedDownReleased()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

                Assert.AreEqual(firstDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestUpPressedLeftReleased()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Left;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);

                var result = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

                Assert.AreEqual(firstDirection, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestOppositeDirectionsUpDown()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            // up and down and right are pressed.
            // releasing right. Expected result is Dpad.None
            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Down;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Right, KeyState.Up);

                Assert.AreEqual(XboxDpadDirection.None, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestOppositeDirectionsLeftRight()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            /// left, right and down are pressed.
            /// releasing down. Expected result is Dpad.None
            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Left;
                var secondDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);

                var result = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Down, KeyState.Up);

                Assert.AreEqual(XboxDpadDirection.None, result);

                KeyboardManager.ResetFakeStates();
            }
        }

        [TestMethod]
        public void TestThreeDirectionsDownOneReleasing()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (var joyControl = new JoyControl(1))
            {
                var firstDirection = XboxDpadDirection.Up;
                var secondDirection = XboxDpadDirection.Down;
                var thirdDirection = XboxDpadDirection.Right;

                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);

                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
                string thridKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
                KeyboardManager.SetFakeDown(KeyboardStrongName, firstKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, secondKey);
                KeyboardManager.SetFakeDown(KeyboardStrongName, thridKey);

                // releasing the 4th direction
                var result = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Left, KeyState.Up);

                Assert.AreEqual(XboxDpadDirection.None, result);

                KeyboardManager.ResetFakeStates();
            }
        }
    }
}