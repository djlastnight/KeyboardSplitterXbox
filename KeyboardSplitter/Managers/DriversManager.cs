namespace KeyboardSplitter.Managers
{
    using System;
    using System.Reflection;
    using System.Windows;
    using Interceptor;
    using Interceptor.Enums;
    using KeyboardSplitter.Helpers;
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
                var path = KeyboardSplitter.Helpers.ResourceExtractor.ExtractResource(
                    Assembly.GetExecutingAssembly(),
                    "KeyboardSplitter.Lib.keyboard_driver.exe");

                LogWriter.Write(InterceptionDriver.Install(path));
            }
            else
            {
                LogWriter.Write("Skipping interception driver installation, because it is already installed");
            }

            if (!IsVirtualXboxBusInstallled)
            {
                LogWriter.Write("Installing virtual xbox bus (SCP) driver");
                LogWriter.Write(VirtualXboxBus.Install());
            }
            else
            {
                LogWriter.Write("Skipping virtual xbox bus (SCP) driver installation, because it is already installed");
            }

            var driverState = InterceptionDriver.DriverState;

            // Original Messagebox is allowed here, because Interception is not loaded yet.
            switch (driverState)
            {
                case InterceptionDriverState.Installed:
                    {
                        MessageBox.Show(
                            "Installation finished, please start the application again.",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LogWriter.Write("Installtion OK. Closing the application, because it needs to be restarted.");
                        Environment.Exit(0);
                    }

                    break;
                case InterceptionDriverState.NotInstalled:
                    {
                        MessageBox.Show(
                            "Built-In Drivers Installation failed!",
                            ApplicationInfo.AppNameVersion,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        LogWriter.Write("Installation Error: Interception driver reports it is not installed");
                        Environment.Exit(0);
                    }

                    break;
                case InterceptionDriverState.RebootRequired:
                    {
                        var result = MessageBox.Show(
                            "Built-in drivers installation finished.\r\n" +
                            "In order to use them, you should reboot your PC.\r\n\r\n" +
                            "Do you want to reboot now?",
                            "Reboot required",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        LogWriter.Write("Installation finishes, but system reboot is required.");

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
                var path = ResourceExtractor.ExtractResource(Assembly.GetExecutingAssembly(), "KeyboardSplitter.Lib.keyboard_driver.exe");
                string msg1 = InterceptionDriver.Uninstall(path);
                string msg2 = VirtualXboxBus.Uninstall();
                LogWriter.Write(msg1);
                LogWriter.Write(msg2);

                Controls.MessageBox.Show(
                    "Uninstallation finished. You must reboot the system to completely remove the drivers.\r\n\r\n Application will now close.",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogWriter.Write("Uninstalling Built-in drivers FAILED:\r\n" + ex.ToString());

                Controls.MessageBox.Show(
                    "Uninstall failed: " + ex.Message,
                    "Uninstall failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}