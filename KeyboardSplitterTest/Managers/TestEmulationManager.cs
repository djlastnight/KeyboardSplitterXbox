namespace KeyboardSplitterTest.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [TestCleanup]
        public void Clean()
        {
            EmulationManager.Destroy();
        }

        [TestMethod]
        public void TestEmulationManagerCreateNormal()
        {
            EmulationManager.Create(1);
            bool created = EmulationManager.IsCreated;
            EmulationManager.Destroy();

            Assert.IsTrue(created);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerCreateZeroIndex()
        {
            EmulationManager.Create(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestEmulationManagerCreateBigIndex()
        {
            EmulationManager.Create(5);
        }

        [TestMethod]
        public void TestEmulationManagerCreateJoyControls()
        {
            int childrenCount;

            EmulationManager.Create(2);
            childrenCount = EmulationManager.JoyControls.Count;
            EmulationManager.Destroy();

            Assert.AreEqual(2, childrenCount);
        }

        [TestMethod]
        public void TestEmulationManagerStartNormal()
        {
            EmulationManager.Create(1, "Keyboard_01");

            EmulationManager.Start();

            EmulationManager.Stop();

            EmulationManager.Destroy();
        }

        [TestMethod]
        public void TestEmulationManagerStartWithoutKeyboardSet()
        {
            bool catched = false;
            EmulationManager.Create(1);

            try
            {
                EmulationManager.Start();
            }
            catch (KeyboardNotSetException)
            {
                catched = true;
            }
            finally
            {
                EmulationManager.Destroy();
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestEmulationManagerStartBeforeCreated()
        {
            bool catched = false;

            try
            {
                EmulationManager.Start();
            }
            catch (InvalidOperationException)
            {
                catched = true;
            }
            finally
            {
                EmulationManager.Destroy();
            }

            Assert.IsTrue(catched);
        }

        [TestMethod]
        public void TestEmulationManagerTwoSlots()
        {
            int childrenCount = 0;
            List<InterceptionKeyboard> keyboards = null;

            using (var input = new Input(KeyboardFilterMode.None, MouseFilterMode.None))
            {
                input.Load();
                keyboards = input.GetKeyboards();
            }

            if (keyboards.Count < 2)
            {
                return;
            }

            EmulationManager.Create(2, keyboards.Select(x => x.StrongName).ToArray());
            childrenCount = EmulationManager.JoyControls.Count;

            bool fail = false;
            string failMessage = string.Empty;
            try
            {
                EmulationManager.Start();
            }
            catch (Exception e)
            {
                fail = true;
                failMessage = e.Message;
            }
            finally
            {
                EmulationManager.Destroy();
            }

            if (fail)
            {
                Assert.Fail(failMessage);
            }

            Assert.AreEqual(2, childrenCount);
        }
    }
}