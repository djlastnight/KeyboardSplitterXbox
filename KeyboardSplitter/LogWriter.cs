namespace KeyboardSplitter
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public static class LogWriter
    {
        private static string logFileName = "splitter_log.txt";

        private static string logFilePath = System.Windows.Forms.Application.StartupPath + "/" + logFileName;

        public static string LogFilePath
        {
            get
            {
                return logFilePath;
            }
        }

        public static string GetLogFileName
        {
            get
            {
                return logFileName;
            }
        }

        public static bool IsInitialized
        {
            get;
            private set;
        }

        public static void Init()
        {
            try
            {
                using (var w = new StreamWriter(path: logFilePath, append: false, encoding: Encoding.Unicode))
                {
                    IsInitialized = true;
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Write(string logMessage)
        {
            try
            {
                using (var writer = new StreamWriter(logFilePath, true, Encoding.Unicode))
                {
                    double memoryUsage = Math.Round(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024, 0);
                    string memStamp = "[Mem: " + memoryUsage + " MB] - ";
                    string timeStamp = DateTime.Now.ToString("[ddd] dd MMM, yyyy [HH:mm:ss] ", CultureInfo.InvariantCulture);
                    writer.WriteLine(timeStamp + memStamp + logMessage);
                    Console.WriteLine(timeStamp + memStamp + logMessage);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}