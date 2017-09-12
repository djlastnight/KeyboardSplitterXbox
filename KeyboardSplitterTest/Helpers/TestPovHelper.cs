//namespace KeyboardSplitterTest.Helpers
//{
//    using Interceptor;
//    using KeyboardSplitter.Controls;
//    using KeyboardSplitter.Helpers;
//    using KeyboardSplitter.Managers;
//    using KeyboardSplitter.Presets;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using XboxAPI;

//    [TestClass]
//    public class TestPovHelper
//    {
//        [TestInitialize]
//        public void Init()
//        {
//            KeyboardSplitter.App.Initialize();
//        }

//        [TestMethod]
//        public void TestPovUpPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var expected = XboxDpadDirection.Up;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);
//                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == expected).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, keyGesture);

//                actual = PovHelper.CalculatePovDirection(joyControl, expected, KeyState.Down);
//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(expected, actual);
//        }

//        [TestMethod]
//        public void TestPovDownPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var expected = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);
//                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == expected).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, keyGesture);

//                actual = PovHelper.CalculatePovDirection(joyControl, expected, KeyState.Down);
//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(expected, actual);
//        }

//        [TestMethod]
//        public void TestPovLeftPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var expected = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);
//                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == expected).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, keyGesture);

//                actual = PovHelper.CalculatePovDirection(joyControl, expected, KeyState.Down);
//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(expected, actual);
//        }

//        [TestMethod]
//        public void TestPovRightPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var expected = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);
//                string keyGesture = Preset.Default.Povs.Find(x => x.Direction == expected).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, keyGesture);

//                actual = PovHelper.CalculatePovDirection(joyControl, expected, KeyState.Down);
//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(expected, actual);
//        }

//        [TestMethod]
//        public void TestUpLeftPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpRightPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpDownPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestRightUpPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Right;
//            var secondDirection = XboxDpadDirection.Up;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestRightDownPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Right;
//            var secondDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestRightLeftPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Right;
//            var secondDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestDownUpPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Down;
//            var secondDirection = XboxDpadDirection.Up;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestDownRightPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Down;
//            var secondDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestDownLeftPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Down;
//            var secondDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestLeftUpPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Left;
//            var secondDirection = XboxDpadDirection.Up;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestLeftRightPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Left;
//            var secondDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestLeftDownPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Left;
//            var secondDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection | secondDirection, actual);
//        }

//        [TestMethod]
//        public void TestLeftUpRightPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Left;
//            var secondDirection = XboxDpadDirection.Up;
//            var thirdDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thirdKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(thirdDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpRightDownPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Right;
//            var thirdDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thirdKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(thirdDirection, actual);
//        }

//        [TestMethod]
//        public void TestRightDownLeftPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Right;
//            var secondDirection = XboxDpadDirection.Down;
//            var thirdDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thirdKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(thirdDirection, actual);
//        }

//        [TestMethod]
//        public void TestDownLeftUpPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Down;
//            var secondDirection = XboxDpadDirection.Left;
//            var thirdDirection = XboxDpadDirection.Up;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thirdKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, thirdDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(thirdDirection, actual);
//        }

//        [TestMethod]
//        public void TestAllDirectionsPressed()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Right;
//            var thirdDirection = XboxDpadDirection.Down;
//            var fourthDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thirdKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                string fourthKey = Preset.Default.Povs.Find(x => x.Direction == fourthDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thirdKey);
//                InputManager.SetFakeDown(KeyboardStrongName, fourthKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, fourthDirection, KeyState.Down);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(fourthDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpPressedRightReleased()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpPressedDownReleased()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection, actual);
//        }

//        [TestMethod]
//        public void TestUpPressedLeftReleased()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Left;
//            XboxDpadDirection actual = XboxDpadDirection.None;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, secondDirection, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(firstDirection, actual);
//        }

//        [TestMethod]
//        public void TestOppositeDirectionsUpDown()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            // up and down and right are pressed.
//            // releasing right. Expected result is Dpad.None
//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Down;
//            XboxDpadDirection actual = XboxDpadDirection.Right;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Right, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(XboxDpadDirection.None, actual);
//        }

//        [TestMethod]
//        public void TestOppositeDirectionsLeftRight()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            /// left, right and down are pressed.
//            /// releasing down. Expected result is Dpad.None
            
//            var firstDirection = XboxDpadDirection.Left;
//            var secondDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.Right;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);

//                actual = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Down, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(XboxDpadDirection.None, actual);
//        }

//        [TestMethod]
//        public void TestThreeDirectionsDownOneReleasing()
//        {
//            if (!DriversManager.AreBuiltInDriversInstalled())
//            {
//                return;
//            }

//            var firstDirection = XboxDpadDirection.Up;
//            var secondDirection = XboxDpadDirection.Down;
//            var thirdDirection = XboxDpadDirection.Right;
//            XboxDpadDirection actual = XboxDpadDirection.Right;

//            using (var joyControl = new JoyControl(1))
//            {
//                const string KeyboardStrongName = "Keyboard_01";
//                joyControl.SetKeyboard(KeyboardStrongName);

//                string firstKey = Preset.Default.Povs.Find(x => x.Direction == firstDirection).KeyboardKey;
//                string secondKey = Preset.Default.Povs.Find(x => x.Direction == secondDirection).KeyboardKey;
//                string thridKey = Preset.Default.Povs.Find(x => x.Direction == thirdDirection).KeyboardKey;
//                InputManager.SetFakeDown(KeyboardStrongName, firstKey);
//                InputManager.SetFakeDown(KeyboardStrongName, secondKey);
//                InputManager.SetFakeDown(KeyboardStrongName, thridKey);

//                // releasing the 4th direction
//                actual = PovHelper.CalculatePovDirection(joyControl, XboxDpadDirection.Left, KeyState.Up);

//                InputManager.ResetFakeStates();
//            }

//            Assert.AreEqual(XboxDpadDirection.None, actual);
//        }
//    }
//}