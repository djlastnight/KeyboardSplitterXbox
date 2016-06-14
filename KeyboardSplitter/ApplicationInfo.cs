namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class ApplicationInfo
    {
        public const string AppName = "djlastnight's Gaming Keyboard Splitter";

        public static readonly string AppNameVersion = string.Empty;

        static ApplicationInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = info.FileVersion;
            AppNameVersion = AppName + " v" + version;
        }
    }
}
