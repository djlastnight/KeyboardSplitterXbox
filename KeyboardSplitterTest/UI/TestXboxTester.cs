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
        [TestInitialize]
        public void Init()
        {
            KeyboardSplitter.App.Initialize();
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
        public void TestXboxTesterConstructor()
        {
            bool catched = false;
            var joyControl = new JoyControl(1);
            XboxTester tester = null;

            try
            {
                tester = new XboxTester(joyControl);
            }
            catch (InvalidOperationException)
            {
                catched = true;
            }
            finally
            {
                joyControl.Dispose();
                if (tester != null)
                {
                    tester.Dispose();   
                }
            }

            Assert.IsTrue(catched);
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
            bool highlighted = false;

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
                    highlighted = tester.IsButtonHighlighted(button);
                    VirtualXboxController.UnPlug(UserIndex, true);
                }
            }

            Assert.IsTrue(highlighted);
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
            bool highlighted = true;

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

                    highlighted = tester.IsButtonHighlighted(button);
                }
            }

            Assert.IsFalse(highlighted);
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
            bool highlighted = false;

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

                    highlighted = tester.IsTriggerHighlighted(trigger);
                }
            }

            Assert.IsTrue(highlighted);
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
            bool highlighted = true;

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

                    highlighted = tester.IsTriggerHighlighted(trigger);
                }
            }

            Assert.IsFalse(highlighted);
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
            bool highlighted = false;

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

                    highlighted = tester.IsDpadHightlighted(direction);
                }
            }

            Assert.IsTrue(highlighted);
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
            bool highlighted = true;

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

                    highlighted = tester.IsDpadHightlighted(direction);
                }
            }

            Assert.IsFalse(highlighted);
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
            bool hightlighted = false;

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

                    hightlighted = tester.IsAxisHighlighted(axis, pos);
                }
            }

            Assert.IsTrue(hightlighted);
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
            bool highlighted = true;

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

                    highlighted = tester.IsAxisHighlighted(axis, pos);
                }
            }

            Assert.IsFalse(highlighted);
        }
    }
}