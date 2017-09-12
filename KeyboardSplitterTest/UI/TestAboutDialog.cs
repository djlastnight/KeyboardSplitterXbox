namespace KeyboardSplitterTest.UI
{
    using System;
    using KeyboardSplitter.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestAboutDialog
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAboutDialogConstructorNull()
        {
            AboutWindow about = new AboutWindow(null);
        }

        [TestMethod]
        public void TestAboutDialogConstructor()
        {
            AboutWindow about = new AboutWindow("some Title");
        }
    }
}