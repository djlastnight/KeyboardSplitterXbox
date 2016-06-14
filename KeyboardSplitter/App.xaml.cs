namespace KeyboardSplitter
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using KeyboardSplitter.AssemblyLoaders;

    /// <summary>
    /// The main application's App class.
    /// Note that just a single application instance is allowed.
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private static bool loaded;

        private Mutex mutex;

        /// <summary>
        /// Start the logging and loads all the needed dlls.
        /// Calling this method more than once has no effect.
        /// </summary>
        public static void Initialize()
        {
            StartLogging();

            LoadAssemblies();
        }

        public void Dispose()
        {
            if (this.mutex != null)
            {
                this.mutex.ReleaseMutex();
            }
        }

        private static void StartLogging()
        {
            if (!LogWriter.IsInitialized)
            {
                LogWriter.Init();
                LogWriter.Write("Application started from " +
                    System.Windows.Forms.Application.ExecutablePath);

                LogWriter.Write("OS version: " + Environment.OSVersion.VersionString);
            }
        }

        private static void LoadAssemblies()
        {
            if (!loaded)
            {
                UnmanagedAssemblyLoader.Load(
                    Assembly.GetExecutingAssembly(),
                    "KeyboardSplitter.Lib.interception.dll",
                    "interception.dll",
                    ApplicationInfo.AppNameVersion,
                    true);

                UnmanagedAssemblyLoader.Load(
                    Assembly.GetExecutingAssembly(),
                    "KeyboardSplitter.Lib.keyboard_driver.exe",
                    "keyboard_driver.exe",
                    ApplicationInfo.AppNameVersion,
                    true);

                UnmanagedAssemblyLoader.Load(
                    Assembly.GetExecutingAssembly(),
                    "KeyboardSplitter.Lib.XboxInterfaceNative.dll",
                    "XboxInterfaceNative.dll",
                    ApplicationInfo.AppNameVersion,
                    true);

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.Interceptor.dll",
                    "Interceptor.dll");

                ManagedAssemblyLoader.Load(
                    "KeyboardSplitter.Lib.XboxInterfaceWrap.dll",
                    "XboxInterfaceWrap.dll");

                loaded = true;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // creating mutex, which will ensure single app instance
            this.mutex = new Mutex(false, "KB_XBOX_SPLITTER_SINGLE_INSTANCE_MUTEX");

            if (!this.mutex.WaitOne(0, false))
            {
                this.mutex.Close();
                this.mutex = null;
                string name = ApplicationInfo.AppName;

                System.Windows.MessageBox.Show(
                    name + " is already running!",
                    name,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(0);
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);

            Initialize();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ManagedAssemblyLoader.Get(args.Name);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            this.Dispose();
        }

        private void Application_DispatcherUnhandledException(
            object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!LogWriter.IsInitialized)
            {
                LogWriter.Init();
            }

            LogWriter.Write("::: UNHANDLED EXCEPTION DETAILS :::");
            LogWriter.Write(e.Exception.ToString());
        }
    }
}