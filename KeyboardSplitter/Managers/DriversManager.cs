namespace KeyboardSplitter.Managers
{
    using System;
    using System.IO;
    using System.Windows;
    using Interceptor;
    using Interceptor.Enums;
    using VirtualXbox;

    public static class DriversManager
    {
        public static bool IsInterceptionInstalled
        {
            get
            {
                return InterceptionDriver.DriverState == InterceptionDriverState.Installed;
            }
        }

        public static bool IsVirtualXboxBusInstallled
        {
            get
            {
                return VirtualXboxBus.IsInstalled;
            }
        }

        public static bool AreBuiltInDriversInstalled
        {
            get
            {
                return IsInterceptionInstalled && IsVirtualXboxBusInstallled;
            }
        }

        public static void InstallBuiltInDrivers()
        {
            if (!IsInterceptionInstalled)
            {
                LogWriter.Write("Installing interception driver");
                var path = KeyboardSplitter.Helpers.ResourceExtractor.ExtractResourceToDirectory("KeyboardSplitter.Lib.keyboard_driver.exe");
                InterceptionDriver.Install(path);
            }

            if (!IsVirtualXboxBusInstallled)
            {
                LogWriter.Write("Installing virtual xbox bus (SCP) driver");
                VirtualXboxBus.Install();
            }

            LogWriter.Write("Built-in drivers installation finished");

            var driverState = InterceptionDriver.DriverState;
            switch (driverState)
            {
                case InterceptionDriverState.Installed:
                    {
                        Controls.MessageBox.Show(
                            "Installation finished, please start the application again.",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LogWriter.Write("Closing the application");
                        Environment.Exit(0);
                    }

                    break;
                case InterceptionDriverState.NotInstalled:
                    {
                        Controls.MessageBox.Show(
                            "Built-In Drivers Installation failed!",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Environment.Exit(0);
                    }

                    break;
                case InterceptionDriverState.RebootRequired:
                    {
                        var result = Controls.MessageBox.Show(
                            "Built-in drivers installation finished.\r\n" +
                            "In order to use them, you should reboot your PC.\r\n\r\n" +
                            "Do you want to reboot now?",
                            "Reboot required",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                            }
                            catch (Exception)
                            {
                                Controls.MessageBox.Show(
                                    "Reboot command failed! Please reboot manually!",
                                    ApplicationInfo.AppNameVersion,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Hand);
                            }
                        }

                        Environment.Exit(0);
                    }

                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented driver state: " + driverState);
            }
        }

        public static void UninstallBuiltInDrivers()
        {
            LogWriter.Write("Uninstalling Built-in drivers");
            try
            {
                var path = Helpers.ResourceExtractor.ExtractResourceToDirectory("KeyboardSplitter.Lib.keyboard_driver.exe");
                string msg1 = InterceptionDriver.Uninstall(path);
                string msg2 = VirtualXboxBus.Uninstall();
                LogWriter.Write(msg1);
                LogWriter.Write(msg2);

                Controls.MessageBox.Show(
                    "Uninstallation finished (see log for details). Application will now close.",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogWriter.Write("Uninstalling Built-in drivers FAILED: " + ex);

                Controls.MessageBox.Show(
                    "Uninstall failed: " + ex.Message,
                    "Process failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}