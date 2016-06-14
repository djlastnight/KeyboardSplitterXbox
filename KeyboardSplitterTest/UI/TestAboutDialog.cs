namespace KeyboardSplitterTest.UI
{
    using System;
    using KeyboardSplitter.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestAboutDialog
    {
        [TestInitialize]
        public void Init()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAboutDialogConstructorNull()
        {
            AboutDialog about = new AboutDialog(null);
        }

        [TestMethod]
        public void TestAboutDialogConstructor()
        {
            AboutDialog about = new AboutDialog("some Title");
        }
    }
}