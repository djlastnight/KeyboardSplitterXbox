namespace KeyboardSplitter.Managers
{
    using System;
    using System.IO;
    using System.Windows;
    using Interceptor;
    using XboxInterfaceWrap;

    public static class DriversManager
    {
        public static bool AreBuiltInDriversInstalled()
        {
            bool interceptionOK = InterceptionDriver.GetDriverState() == DriverState.Installed;
            bool xboxBusOK = XboxBus.IsInstalled();

            return interceptionOK && xboxBusOK;
        }

        public static bool IsXboxAccessoriesInstalled()
        {
            var version = Environment.OSVersion.Version;
            bool isWin7 = version.Major == 6 && version.Minor == 1;
            bool isVista = version.Major == 6 && version.Minor == 0;
            bool isXp = version.Major == 5 && version.Minor == 1;

            if (isWin7 || isVista || isXp)
            {
                string rootDriveLetter = Path.GetPathRoot(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows));

                string xboxAccessoriesStatFile = Path.Combine(
                    rootDriveLetter, "Program Files", "Microsoft Xbox 360 Accessories", "XBoxStat.exe");

                return File.Exists(xboxAccessoriesStatFile);
            }

            // windows 8 and above has this driver built-in
            return true;
        }

        public static void InstallBuiltInDrivers()
        {
            bool interceptionOK = InterceptionDriver.GetDriverState() == DriverState.Installed;
            bool xboxBusOK = XboxBus.IsInstalled();

            if (!interceptionOK || !xboxBusOK)
            {
                ProcessBuiltInInstallation(interceptionOK, xboxBusOK);
            }
        }

        public static void UninstallBuiltInDrivers()
        {
            LogWriter.Write("Uninstalling Built-in drivers");
            try
            {
                const string KEYBOARD_DRIVER_FILENAME = "keyboard_driver.exe";
                string fullPath = Path.Combine(Path.GetTempPath(), ApplicationInfo.AppNameVersion, KEYBOARD_DRIVER_FILENAME);
                InterceptionDriver.Uninstall(fullPath);
                int exitCode;
                string msg = XboxBus.Uninstall(out exitCode);
                LogWriter.Write(msg);
                System.Windows.MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                LogWriter.Write("Uninstalling Built-in drivers FAILED: " + ex);

                System.Windows.MessageBox.Show(
                    "Uninstall failed: " + ex.Message, "Process failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LogWriter.Write("Uninstall completed, closing the application");
            Environment.Exit(0);
        }

        private static void ProcessBuiltInInstallation(bool interceptionOK, bool xboxBusOK)
        {
            if (!interceptionOK)
            {
                const string KeyboardDriverFileName = "keyboard_driver.exe";
                string fullPath = Path.Combine(
                    Path.GetTempPath(),
                    ApplicationInfo.AppNameVersion,
                    KeyboardDriverFileName);

                LogWriter.Write("Installing interception driver");
                InterceptionDriver.Install(fullPath);
            }

            if (!xboxBusOK)
            {
                LogWriter.Write("Installing xbox bus driver");
                XboxBus.Install();
            }

            LogWriter.Write("Built-in drivers installation finished");

            var driverState = InterceptionDriver.GetDriverState();
            switch (driverState)
            {
                case DriverState.Installed:
                    {
                        System.Windows.MessageBox.Show(
                            "Installation finished, please restart the application.",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LogWriter.Write("Closing the application");
                        Environment.Exit(0);
                    }

                    break;
                case DriverState.NotInstalled:
                    {
                        System.Windows.MessageBox.Show(
                            "Built-In Drivers Installation failed!",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Environment.Exit(0);
                    }

                    break;
                case DriverState.RebootRequired:
                    {
                        var result = System.Windows.MessageBox.Show(
                            "Built-in drivers installation finished.\r\n" +
                            "In order to use them, you should reboot your PC.\r\n\r\n" +
                            "Do you want to reboot now?",
                            "Reboot required",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                        }

                        Environment.Exit(0);
                    }

                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented driver state: " + driverState);
            }
        }
    }
}