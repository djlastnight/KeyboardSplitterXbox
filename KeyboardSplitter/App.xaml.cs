namespace KeyboardSplitter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using KeyboardSplitter.AssemblyLoaders;
    using KeyboardSplitter.Helpers;
    using KeyboardSplitter.Managers;
    using KeyboardSplitter.Models;
    using KeyboardSplitter.Presets;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The main application's App class.
    /// Note that just a single application instance is allowed.
    /// </summary>
    public partial class App : Application, IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        private static bool assembliesLoaded;

        public static string autostartGameName;

        private Mutex mutex;

        static App()
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var warningMessage = "There is debugger attached to the application's process!";
                warningMessage += Environment.NewLine;
                warningMessage += "If the debugger or application itself blocks the main UI thread,";
                warningMessage += Environment.NewLine;
                warningMessage += "interception driver callback will never respond and as result";
                warningMessage += Environment.NewLine;
                warningMessage += "this will block all keyboards and mice until system reboot!";
                warningMessage += Environment.NewLine;
                warningMessage += Environment.NewLine;
                warningMessage += "Are you sure that you want to continue execution?";
                var result = System.Windows.MessageBox.Show(
                    warningMessage,
                    ApplicationInfo.AppName,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
            }
#endif
            App.StartLogging();
            App.CheckForObsoleteOS();
            App.LoadAssemblies();
        }

        public static bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.mutex != null)
                {
                    this.mutex.ReleaseMutex();
                    this.mutex.Dispose();
                    this.mutex = null;
                }
            }
        }

        private static void StartLogging()
        {
            if (!LogWriter.IsInitialized)
            {
                LogWriter.Init();
            }

            var starupPath = System.Windows.Forms.Application.ExecutablePath;
            LogWriter.Write("Application started from " + starupPath);
            LogWriter.Write("User has write permissions: " + HasWriteAccessToFolder(starupPath));
            LogWriter.Write("Application version: " + ApplicationInfo.AppNameVersion);
            LogWriter.Write("OS version: " + Helpers.OSHelper.GetWindowsFullVersion());
#if DEBUG
            LogWriter.Write("Debug mode enabled");
#endif
        }

        private static void CheckForObsoleteOS()
        {
            var version = Environment.OSVersion.Version;
            if (version.Major < 5)
            {
                LogWriter.Write("Obsolete OS detected: " + Environment.OSVersion.VersionString);
                Controls.MessageBox.Show(
                    "Your operating system is not supported!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(0);
            }
        }

        private static void LoadAssemblies()
        {
            if (!App.assembliesLoaded)
            {
                // Setting the native dll search path
                LogWriter.Write("Setting environment");
                Environment.SetEnvironmentVariable(
                    "PATH",
                    ApplicationInfo.AppTempDirectory,
                    EnvironmentVariableTarget.Process);

                var assembly = Assembly.GetExecutingAssembly();

                var interceptionPath = ResourceExtractor.ExtractResource(
                    assembly,
                    "KeyboardSplitter.Lib.interception.dll",
                    ApplicationInfo.AppTempDirectory);

                LogWriter.Write("Loading interception " + FileVersionInfo.GetVersionInfo(interceptionPath).ProductVersion);
                
                var vxboxNativePath = ResourceExtractor.ExtractResource(
                    assembly,
                    "KeyboardSplitter.Lib.VirtualXboxNative.dll",
                    ApplicationInfo.AppTempDirectory);

                LogWriter.Write("Loading VirtualXboxNative " + FileVersionInfo.GetVersionInfo(vxboxNativePath).ProductVersion);

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.Interceptor.dll",
                    "Interceptor.dll");

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.SplitterCore.dll",
                    "SplitterCore.dll");

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.VirtualXbox.dll",
                    "VirtualXbox.dll");

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.XinputWrapper.dll",
                    "XinputWrapper.dll");

                App.assembliesLoaded = true;
            }
        }

        private static void CheckDrivers()
        {
            if (!DriversManager.AreBuiltInDriversInstalled)
            {
                LogWriter.Write("Built-in drivers are not installed, asking user to install them");
                var result = Controls.MessageBox.Show(
                    "It seems that the required built-in drivers are not installed.\r\n" +
                    "Do you want to install them (may require reboot)?\r\n\r\n" +
                    "Selecting \"No\" will quit the application.",
                    ApplicationInfo.AppName + " | Drivers required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DriversManager.InstallBuiltInDrivers();
                }
                else
                {
                    LogWriter.Write(
                        "User has choosen to NOT install the built-in drivers. Exitting");

                    Environment.Exit(0);
                }
            }
        }

        private static void ReportDriversState()
        {
            LogWriter.Write(string.Format(
                "Interception driver state: {0}",
                DriversManager.IsInterceptionInstalled ? "installed" : "not installed"));

            LogWriter.Write(string.Format(
                "ScpVBus driver state: {0}",
                DriversManager.IsVirtualXboxBusInstallled ? "installed" : "not installed"));

            LogWriter.Write(string.Format(
                "Xbox accessories driver state: {0}",
                XboxGamepad.AreXboxAccessoriesInstalled ? "installed" : "not installed"));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool allowMultiInstance = false;
            foreach (var arg in e.Args)
            {
                if (arg.ToLower() == "allow-multi-instance")
                {
                    LogWriter.Write("Allow multi instance parameter passed");
                    allowMultiInstance = true;
                }
                else if (arg.StartsWith("game="))
                {
                    var gameName = arg.Substring("game=".Length);
                    App.autostartGameName = gameName;
                }
            }

            if (!allowMultiInstance)
            {
                // creating mutex, which will ensure single app instance
                this.mutex = new Mutex(false, "KB_XBOX_SPLITTER_SINGLE_INSTANCE_MUTEX");

                if (!this.mutex.WaitOne(0, false))
                {
                    this.mutex.Close();
                    this.mutex = null;
                    string name = ApplicationInfo.AppName;

                    Controls.MessageBox.Show(
                        name + " is already running!",
                        name,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Environment.Exit(0);
                }
            }

            if (string.IsNullOrWhiteSpace(App.autostartGameName))
            {
                // Since we won't autostart a game through CLI, we will only show the WPF.
                FreeConsole();
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);

            App.ReportDriversState();
            App.CheckDrivers();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            LogWriter.Write(string.Format(
                "Due to pending Windows {0}, application will try to save the current settings and presets",
                e.ReasonSessionEnding.ToString()));

            GlobalSettings.TrySaveToFile();

            try
            {
                PresetDataManager.WritePresetDataToFile();
                LogWriter.Write("Presets successfully saved");
            }
            catch (Exception ex)
            {
                LogWriter.Write("Presets saving failed:");
                LogWriter.Write(ex.ToString());
            }

            var splitter = Helpers.SplitterHelper.TryFindSplitter();
            if (splitter != null)
            {
                splitter.Destroy();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            LogWriter.Write("Application exited with code: " + e.ApplicationExitCode);
            this.Dispose();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ManagedAssemblyLoader.Get(args.Name);
        }

        private void Application_DispatcherUnhandledException(
            object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!LogWriter.IsInitialized)
            {
                LogWriter.Init();
            }

            var splitter = SplitterHelper.TryFindSplitter();
            if (splitter != null)
            {
                splitter.Destroy();
            }

            LogWriter.Write("::: UNHANDLED EXCEPTION DETAILS :::");
            LogWriter.Write(e.Exception.ToString());

            string message = string.Format(
                "Unexpected app crash occured.{0}Please refer to {1} for more details.{0}{0}{2} will now close.",
                Environment.NewLine,
                LogWriter.GetLogFileName,
                ApplicationInfo.AppName);

            Controls.MessageBox.Show(
                message,
                ApplicationInfo.AppNameVersion,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Environment.Exit(0);
        }
    }
}