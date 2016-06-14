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
        [TestInitialize]
        public void Init()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestButtonKeyControl()
        {
            XboxButton button;
            KeyControlType type;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, XboxButton.A, removeable: false))
                {
                    button = keyControl.Button;
                    type = keyControl.ControlType;
                }
            }

            Assert.AreEqual(button, XboxButton.A);
            Assert.AreEqual(type, KeyControlType.Button);
        }

        [TestMethod]
        public void TestDpadKeyControl()
        {
            XboxDpadDirection direction;
            KeyControlType type;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(
                    joyControl, XboxDpadDirection.Left, removeable: false))
                {
                    direction = keyControl.DpadDirection;
                    type = keyControl.ControlType;
                }
            }

            Assert.AreEqual(direction, XboxDpadDirection.Left);
            Assert.AreEqual(type, KeyControlType.Dpad);
        }

        [TestMethod]
        public void TestTriggerKeyControl()
        {
            XboxTrigger trigger;
            KeyControlType type;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(
                    joyControl, XboxTrigger.RightTrigger, false))
                {
                    trigger = keyControl.Trigger;
                    type = keyControl.ControlType;
                }
            }
            
            Assert.AreEqual(trigger, XboxTrigger.RightTrigger);
            Assert.AreEqual(type, KeyControlType.Trigger);
        }

        [TestMethod]
        public void TestAxisKeyControl()
        {
            XboxAxis axis;
            XboxAxisPosition position;
            KeyControlType type;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(
                    joyControl, XboxAxis.Ry, XboxAxisPosition.Max, false))
                {
                    axis = keyControl.Axis;
                    position = keyControl.Position;
                    type = keyControl.ControlType;
                }
            }
            
            Assert.AreEqual(axis, XboxAxis.Ry);
            Assert.AreEqual(position, XboxAxisPosition.Max);
            Assert.AreEqual(type, KeyControlType.Axis);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestKeyControlNullParent()
        {
            using (var keyControl = new KeyControl(null, false))
            {
            }
        }

        [TestMethod]
        public void TestParent()
        {
            JoyControl expected;
            JoyControl actual;

            using (var parent = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(parent, false))
                {
                    expected = parent;
                    actual = keyControl.JoyParent;
                }
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRemoveableKeyControl()
        {
            bool isRemoveable;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    isRemoveable = keyControl.IsRemoveable;
                }
            }

            Assert.IsTrue(isRemoveable);
        }

        [TestMethod]
        public void TestDefaultKeyGesture()
        {
            string keyGesture;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl))
                {
                    keyGesture = keyControl.KeyGesture;
                }
            }

            Assert.IsTrue(keyGesture == InterceptionKeys.None.ToString());
        }

        [TestMethod]
        public void TestKeyGesture()
        {
            string expectedKeyGesture = InterceptionKeys.RightControl.ToString();
            string actualKeyGesture;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl))
                {
                    keyControl.KeyGesture = expectedKeyGesture;
                    actualKeyGesture = keyControl.KeyGesture;
                }
            }

            Assert.AreEqual(expectedKeyGesture, actualKeyGesture);
        }

        [TestMethod]
        public void TestKeyGestureChanged()
        {
            bool eventRisen = false;
            string expectedKeyGesture = InterceptionKeys.H.ToString();
            string eventKeyGesture = string.Empty;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl))
                {
                    keyControl.KeyGestureChanged += (sender, e) =>
                    {
                        eventKeyGesture = e.NewKey;
                        eventRisen = true;
                    };

                    keyControl.KeyGesture = expectedKeyGesture;
                }
            }

            Assert.AreEqual(
                expectedKeyGesture,
                eventKeyGesture,
                "Wrong event args key gesture!");

            Assert.IsTrue(eventRisen, "Key Gesture Changed event was not raised!");
        }

        [TestMethod]
        public void TestSetCustomFunction()
        {
            bool testFailed = false;
            XboxCustomFunction failedFunction;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    foreach (XboxCustomFunction function in Enum.GetValues(typeof(XboxCustomFunction)))
                    {
                        keyControl.CustomFunction = function;
                        var controlType = CustomFunctionHelper.GetControlType(function);

                        if (!controlType.Equals(keyControl.ControlType))
                        {
                            testFailed = true;
                            failedFunction = function;
                            break;
                        }

                        switch (controlType)
                        {
                            case KeyControlType.Button:
                                {
                                    var button = CustomFunctionHelper.GetXboxButton(function);
                                    if (!button.Equals(keyControl.Button))
                                    {
                                        testFailed = true;
                                        break;
                                    }
                                }

                                break;
                            case KeyControlType.Axis:
                                {
                                    XboxAxisPosition pos = XboxAxisPosition.Center;
                                    var axis = CustomFunctionHelper.GetXboxAxis(function, out pos);
                                    if (!axis.Equals(keyControl.Axis) ||
                                        !pos.Equals(keyControl.Position))
                                    {
                                        testFailed = true;
                                        break;
                                    }
                                }

                                break;
                            case KeyControlType.Dpad:
                                {
                                    var direction = CustomFunctionHelper.GetDpadDirection(function);
                                    if (!direction.Equals(keyControl.DpadDirection))
                                    {
                                        testFailed = true;
                                        break;
                                    }
                                }

                                break;
                            case KeyControlType.Trigger:
                                {
                                    var trigger = CustomFunctionHelper.GetXboxTrigger(function);
                                    if (!trigger.Equals(keyControl.Trigger))
                                    {
                                        testFailed = true;
                                        break;
                                    }
                                }

                                break;
                            default:
                                throw new NotImplementedException(
                                    "Not implemented control type: " + controlType);
                        }

                        if (testFailed)
                        {
                            failedFunction = function;
                            break;
                        }
                    }
                }
            }

            if (testFailed)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestButtonCustomFunctionChanged()
        {
            bool eventRisen = false;
            var expectedFunction = XboxCustomFunction.Start;
            XboxCustomFunction actualFunction = XboxCustomFunction.A;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (sender, e) =>
                        {
                            actualFunction = keyControl.CustomFunction;
                            eventRisen = true;
                        };

                    keyControl.CustomFunction = expectedFunction;
                }
            }

            Assert.AreEqual(
                expectedFunction,
                actualFunction,
                "Button custom function was not set correctly!");

            Assert.IsTrue(eventRisen, "Function Changed event was not raised!");
        }

        [TestMethod]
        public void TestAxisCustomFunctionChanged()
        {
            bool eventRisen = false;
            var expectedFunction = XboxCustomFunction.Rx_Max;
            XboxCustomFunction actualFunction = XboxCustomFunction.A;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (sender, e) =>
                    {
                        actualFunction = keyControl.CustomFunction;
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = expectedFunction;
                }
            }

            Assert.AreEqual(
                expectedFunction,
                actualFunction,
                "Axis custom function was not set correctly!");

            Assert.IsTrue(eventRisen, "Function Changed event was not raised!");
        }

        [TestMethod]
        public void TestTriggerCustomFunctionChanged()
        {
            bool eventRisen = false;
            var expectedFunction = XboxCustomFunction.RightTrigger;
            XboxCustomFunction actualFunction = XboxCustomFunction.A;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (sender, e) =>
                    {
                        actualFunction = keyControl.CustomFunction;
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = expectedFunction;
                }
            }

            Assert.AreEqual(
                expectedFunction,
                actualFunction,
                "Trigger custom function was not set correctly!");

            Assert.IsTrue(eventRisen, "Function Changed event was not raised!");
        }

        [TestMethod]
        public void TestDpadCustomFunctionChanged()
        {
            bool eventRisen = false;
            var expectedFunction = XboxCustomFunction.Dpad_Left;
            XboxCustomFunction actualFunction = XboxCustomFunction.A;

            using (var joyControl = new JoyControl(1))
            {
                using (var keyControl = new KeyControl(joyControl, isRemoveable: true))
                {
                    keyControl.FunctionChanged += (sender, e) =>
                    {
                        actualFunction = keyControl.CustomFunction;
                        eventRisen = true;
                    };

                    keyControl.CustomFunction = expectedFunction;
                }
            }

            Assert.AreEqual(
                expectedFunction,
                actualFunction,
                "D-pad custom function was not set correctly!");

            Assert.IsTrue(eventRisen, "Function Changed event was not raised!");
        }
    }
}