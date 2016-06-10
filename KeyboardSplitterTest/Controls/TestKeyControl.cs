namespace KeyboardSplitterTest.Controls
{
    using System;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestKeyControl
    {
        public TestKeyControl()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestButtonKeyControl()
        {
            var joyControl = new JoyControl(1);
            var keyControl = new KeyControl(joyControl, XboxButton.A, false);
            Assert.AreEqual(keyControl.Button, XboxButton.A);
            Assert.AreEqual(keyControl.ControlType, KeyControlType.Button);
        }

        [TestMethod]
        public void TestDpadKeyControl()
        {
            var joyControl = new JoyControl(1);
            var keyControl = new KeyControl(joyControl, XboxDpadDirection.Left, false);
            Assert.AreEqual(keyControl.DpadDirection, XboxDpadDirection.Left);
            Assert.AreEqual(keyControl.ControlType, KeyControlType.Dpad);
        }

        [TestMethod]
        public void TestTriggerKeyControl()
        {
            var joyControl = new JoyControl(1);
            var keyControl = new KeyControl(joyControl, XboxTrigger.RightTrigger, false);
            Assert.AreEqual(keyControl.Trigger, XboxTrigger.RightTrigger);
            Assert.AreEqual(keyControl.ControlType, KeyControlType.Trigger);
        }

        [TestMethod]
        public void TestAxisKeyControl()
        {
            var joyControl = new JoyControl(1);
            var keyControl = new KeyControl(joyControl, XboxAxis.Ry, XboxAxisPosition.Max, false);
            Assert.AreEqual(keyControl.Axis, XboxAxis.Ry);
            Assert.AreEqual(keyControl.Position, XboxAxisPosition.Max);
            Assert.AreEqual(keyControl.ControlType, KeyControlType.Axis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestKeyControlNullParent()
        {
            var keyControl = new KeyControl(null, false);
        }

        [TestMethod]
        public void TestParent()
        {
            var parent = new JoyControl(1);
            var keyControl = new KeyControl(parent, false);

            Assert.AreEqual(parent, keyControl.JoyParent);
        }

        [TestMethod]
        public void TestRemoveableKeyControl()
        {
            var parent = new JoyControl(1);
            var keyControl = new KeyControl(parent, isRemoveable: true);

            Assert.IsTrue(keyControl.IsRemoveable);
        }

        [TestMethod]
        public void TestDefaultKeyGesture()
        {
            var keyControl = new KeyControl(new JoyControl(1));

            Assert.IsTrue(keyControl.KeyGesture == InterceptionKeys.None.ToString());
        }

        [TestMethod]
        public void TestKeyGesture()
        {
            var keyControl = new KeyControl(new JoyControl(1));
            string keyGesture = InterceptionKeys.RightControl.ToString();
            keyControl.KeyGesture = keyGesture;

            Assert.AreEqual(keyGesture, keyControl.KeyGesture);
        }

        [TestMethod]
        public void TestKeyGestureChanged()
        {
            bool eventRisen = false;
            using (var keyControl = new KeyControl(new JoyControl(1)))
            {
                string keyGesture = InterceptionKeys.H.ToString();

                keyControl.KeyGestureChanged += (ss, ee) =>
                {
                    Assert.AreEqual(keyGesture, keyControl.KeyGesture);
                    Assert.AreEqual(keyGesture, ee.NewKey);
                    eventRisen = true;
                };

                keyControl.KeyGesture = keyGesture;
                Assert.AreEqual(keyGesture, keyControl.KeyGesture);
            }

            Assert.IsTrue(eventRisen);
        }

        [TestMethod]
        public void TestSetCustomFunction()
        {
            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, true))
                {
                    foreach (XboxCustomFunction function in Enum.GetValues(typeof(XboxCustomFunction)))
                    {
                        keyControl.CustomFunction = function;
                        var controlType = CustomFunctionHelper.GetControlType(function);

                        switch (controlType)
                        {
                            case KeyControlType.Button:
                                {
                                    var button = CustomFunctionHelper.GetXboxButton(function);
                                    Assert.AreEqual(button, keyControl.Button);
                                }

                                break;
                            case KeyControlType.Axis:
                                {
                                    XboxAxisPosition pos = XboxAxisPosition.Center;
                                    var axis = CustomFunctionHelper.GetXboxAxis(function, out pos);
                                    Assert.AreEqual(axis, keyControl.Axis);
                                    Assert.AreEqual(pos, keyControl.Position);
                                }

                                break;
                            case KeyControlType.Dpad:
                                {
                                    var direction = CustomFunctionHelper.GetDpadDirection(function);
                                    Assert.AreEqual(direction, keyControl.DpadDirection);
                                }

                                break;
                            case KeyControlType.Trigger:
                                {
                                    var trigger = CustomFunctionHelper.GetXboxTrigger(function);
                                    Assert.AreEqual(trigger, keyControl.Trigger);
                                }

                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented control type: " + controlType);
                        }

                        Assert.AreEqual(keyControl.ControlType, controlType);
                    }
                }
            }
        }

        [TestMethod]
        public void TestButtonCustomFunctionChanged()
        {
            bool eventRisen = false;
            var function = XboxCustomFunction.Start;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (ss, ee) =>
                        {
                            Assert.AreEqual(function, keyControl.CustomFunction);
                            eventRisen = true;
                        };

                    keyControl.CustomFunction = function;
                    Assert.AreEqual(function, keyControl.CustomFunction);
                }
            }

            Assert.IsTrue(eventRisen);
        }

        [TestMethod]
        public void TestAxisCustomFunctionChanged()
        {
            bool eventRisen = false;
            var function = XboxCustomFunction.Rx_Max;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (ss, ee) =>
                    {
                        Assert.AreEqual(function, keyControl.CustomFunction);
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = function;
                    Assert.AreEqual(function, keyControl.CustomFunction);
                }
            }

            Assert.IsTrue(eventRisen);
        }

        [TestMethod]
        public void TestTriggerCustomFunctionChanged()
        {
            bool eventRisen = false;
            var function = XboxCustomFunction.RightTrigger;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (ss, ee) =>
                    {
                        Assert.AreEqual(function, keyControl.CustomFunction);
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = function;
                    Assert.AreEqual(function, keyControl.CustomFunction);
                }
            }

            Assert.IsTrue(eventRisen);
        }

        [TestMethod]
        public void TestDpadCustomFunctionChanged()
        {
            bool eventRisen = false;
            var function = XboxCustomFunction.Dpad_Left;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (ss, ee) =>
                    {
                        Assert.AreEqual(function, keyControl.CustomFunction);
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = function;
                    Assert.AreEqual(function, keyControl.CustomFunction);
                }
            }

            Assert.IsTrue(eventRisen);
        }
    }
}