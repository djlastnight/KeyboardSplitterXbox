namespace KeyboardSplitterTest.Helpers
{
    using System;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Helpers;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Presets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestAxisHelper
    {
        [TestInitialize]
        public void Init()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestAxisMaxWhenReleasingMin()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;

            using (var joyControl = new JoyControl(1))
            {
                string keyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(keyboardStrongName);
                var testAxis = XboxAxis.Rx;

                string maxKey = Preset.Default.Axes.Find(x => x.Axis == testAxis &&
                    x.Position == XboxAxisPosition.Max).KeyboardKey;

                InputManager.SetFakeDown(keyboardStrongName, maxKey);

                result = AxisHelper.CalculateAxisValue(
                    joyControl, testAxis, XboxAxisPosition.Min, KeyState.Up);

                InputManager.ResetFakeStates();
            }

            Assert.AreEqual(short.MaxValue, result);
        }

        [TestMethod]
        public void TestAxisMinWhenReleasingMax()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;

            using (var joyControl = new JoyControl(1))
            {
                string keyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(keyboardStrongName);
                var testAxis = XboxAxis.Ry;

                string minKey = Preset.Default.Axes.Find(x => x.Axis == testAxis &&
                    x.Position == XboxAxisPosition.Min).KeyboardKey;

                InputManager.SetFakeDown(keyboardStrongName, minKey);

                result = AxisHelper.CalculateAxisValue(
                    joyControl, testAxis, XboxAxisPosition.Max, KeyState.Up);

                InputManager.ResetFakeStates();
            }

            Assert.AreEqual(short.MinValue, result);
        }

        [TestMethod]
        public void TestAxisMinDown()
        {
            short result = AxisHelper.CalculateAxisValue(
                null,
                XboxAxis.X,
                XboxAxisPosition.Min,
                KeyState.Down);

            Assert.AreEqual(short.MinValue, result);
        }

        [TestMethod]
        public void TestAxisMaxDown()
        {
            var result = AxisHelper.CalculateAxisValue(
                null,
                XboxAxis.X,
                XboxAxisPosition.Max,
                KeyState.Down);

            Assert.AreEqual(short.MaxValue, result);
        }

        [TestMethod]
        public void TestAxisMinUp()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;

            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Min,
                    KeyState.Up);
            }

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAxisMaxUp()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Max,
                    KeyState.Up);
            }

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAxisCenterDown()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Center,
                    KeyState.Down);
            }

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAxisCenterUp()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Center,
                    KeyState.Up);
            }

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAxisMinE0()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Min,
                    KeyState.E0);
            }

            Assert.AreEqual(short.MinValue, result);
        }

        [TestMethod]
        public void TestAxisMaxE0()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                joyControl.SetKeyboard("Keyboard_01");

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    XboxAxis.X,
                    XboxAxisPosition.Max,
                    KeyState.E0);
            }

            Assert.AreEqual(short.MaxValue, result);
        }

        [TestMethod]
        public void TestAxisMaxWhileMinIsDown()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                var testAxis = XboxAxis.X;
                string minKey = Preset.Default.Axes.Find(x => x.Axis == testAxis &&
                    x.Position == XboxAxisPosition.Min).KeyboardKey;

                InputManager.SetFakeDown(KeyboardStrongName, minKey);

                result = AxisHelper.CalculateAxisValue(
                    joyControl, testAxis, XboxAxisPosition.Max, KeyState.Down);

                InputManager.ResetFakeStates();
            }

            Assert.AreEqual(short.MaxValue, result);
        }

        [TestMethod]
        public void TestAxisMinWhileMaxIsDown()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            short result;
            using (var joyControl = new JoyControl(1))
            {
                const string KeyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(KeyboardStrongName);
                var testAxis = XboxAxis.Y;
                string maxKey = Preset.Default.Axes.Find(
                    x => x.Axis == testAxis && x.Position == XboxAxisPosition.Max).KeyboardKey;

                InputManager.SetFakeDown(KeyboardStrongName, maxKey);

                result = AxisHelper.CalculateAxisValue(
                    joyControl,
                    testAxis,
                    XboxAxisPosition.Min,
                    KeyState.Down);

                InputManager.ResetFakeStates();
            }

            Assert.AreEqual(short.MinValue, result);
        }

        [TestMethod]
        public void TestAxisBothPressed()
        {
            bool testPassed = false;
            using (var joyControl = new JoyControl(1))
            {
                string keyboardStrongName = "Keyboard_01";
                joyControl.SetKeyboard(keyboardStrongName);
                var testAxis = XboxAxis.Rx;

                string minKey = Preset.Default.Axes.Find(x => x.Axis == testAxis &&
                    x.Position == XboxAxisPosition.Min).KeyboardKey;

                string maxKey = Preset.Default.Axes.Find(x => x.Axis == testAxis &&
                    x.Position == XboxAxisPosition.Max).KeyboardKey;

                InputManager.SetFakeDown(keyboardStrongName, minKey);
                InputManager.SetFakeDown(keyboardStrongName, maxKey);

                // sending up state to fail the function
                try
                {
                    AxisHelper.CalculateAxisValue(
                        joyControl,
                        testAxis,
                        XboxAxisPosition.Min,
                        KeyState.Up);
                }
                catch (InvalidOperationException)
                {
                    testPassed = true;
                }
                finally
                {
                    InputManager.ResetFakeStates();
                }
            }

            Assert.IsTrue(testPassed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullParentKeyUp()
        {
            AxisHelper.CalculateAxisValue(
                null, XboxAxis.Rx, XboxAxisPosition.Min, KeyState.Up);
        }
    }
}