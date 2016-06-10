namespace KeyboardSplitterTest.UI
{
    using System;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XboxInterfaceWrap;

    [TestClass]
    public class TestXboxTester
    {
        public TestXboxTester()
        {
            KeyboardSplitter.App.Initialize();
            VirtualXboxController.UnPlug(1, true);
            VirtualXboxController.UnPlug(2, true);
            VirtualXboxController.UnPlug(3, true);
            VirtualXboxController.UnPlug(4, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxTesterConstructorNull()
        {
            using (var tester = new XboxTester(null))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestXboxTesterConstructor()
        {
            using (var joyControl = new JoyControl(1))
            {
                if (!VirtualXboxController.Exists(1))
                {
                    /// due to controller is not plugged in yet
                    /// Xbox tester's constructor should throw
                    /// an exception
                    using (var tester = new XboxTester(joyControl))
                    {
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterHighlightButton()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var button = XboxButton.Start;
                    VirtualXboxController.SetButton(UserIndex, button, true);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);
                    
                    Assert.IsTrue(tester.IsButtonHighlighted(button));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterUnHighlightButton()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var button = XboxButton.Back;
                    VirtualXboxController.SetButton(UserIndex, button, true);
                    tester.UpdateHighlights();

                    VirtualXboxController.SetButton(UserIndex, button, false);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsFalse(tester.IsButtonHighlighted(button));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterHighlightTrigger()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var trigger = XboxTrigger.LeftTrigger;
                    VirtualXboxController.SetTrigger(UserIndex, trigger, 255);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsTrue(tester.IsTriggerHighlighted(trigger));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterUnHighlightTrigger()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var trigger = XboxTrigger.RightTrigger;
                    VirtualXboxController.SetTrigger(UserIndex, trigger, 255);
                    tester.UpdateHighlights();

                    VirtualXboxController.SetTrigger(UserIndex, trigger, 0);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsFalse(tester.IsTriggerHighlighted(trigger));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterHighlightDpad()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var direction = XboxDpadDirection.Right;
                    VirtualXboxController.SetDPad(UserIndex, direction);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsTrue(tester.IsDpadHightlighted(direction));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterUnHighlightDpad()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var direction = XboxDpadDirection.Left;
                    VirtualXboxController.SetDPad(UserIndex, direction);
                    tester.UpdateHighlights();

                    VirtualXboxController.SetDPad(UserIndex, XboxDpadDirection.None);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsFalse(tester.IsDpadHightlighted(direction));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterHighlightAxis()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var axis = XboxAxis.Ry;
                    var pos = XboxAxisPosition.Min;
                    VirtualXboxController.SetAxis(UserIndex, axis, (short)pos);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsTrue(tester.IsAxisHighlighted(axis, pos));
                }
            }
        }

        [TestMethod]
        public void TestXboxTesterUnHighlightAxis()
        {
            if (!XboxBus.IsInstalled() ||
                !DriversManager.IsXboxAccessoriesInstalled())
            {
                return;
            }

            const uint UserIndex = 1;

            using (var joyControl = new JoyControl(UserIndex))
            {
                VirtualXboxController.UnPlug(UserIndex, true);

                if (!VirtualXboxController.PlugIn(UserIndex))
                {
                    return;
                }

                using (var tester = new XboxTester(joyControl))
                {
                    var axis = XboxAxis.Y;
                    var pos = XboxAxisPosition.Max;
                    VirtualXboxController.SetAxis(UserIndex, axis, (short)pos);
                    tester.UpdateHighlights();

                    VirtualXboxController.SetAxis(UserIndex, axis, (short)XboxAxisPosition.Center);
                    tester.UpdateHighlights();

                    VirtualXboxController.UnPlug(UserIndex, true);

                    Assert.IsFalse(tester.IsAxisHighlighted(axis, pos));
                }
            }
        }
    }
}