namespace KeyboardSplitterTest
{
    using KeyboardSplitter.Managers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestMainWindow
    {
        public TestMainWindow()
        {
            KeyboardSplitter.App.Initialize();
        }

        [TestMethod]
        public void TestMainWindowConstructor()
        {
            if (!DriversManager.AreBuiltInDriversInstalled())
            {
                return;
            }

            using (KeyboardSplitter.MainWindow window = new KeyboardSplitter.MainWindow())
            {
            }
        }
    }
}