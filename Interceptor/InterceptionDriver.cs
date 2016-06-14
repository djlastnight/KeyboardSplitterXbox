namespace Interceptor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Windows.Forms;

    /// <summary>
    /// The .NET wrapper class around the C++ library interception.dll.
    /// </summary>
    public static class InterceptionDriver
    {
        /// <summary>
        /// Gets the current interception driver state.
        /// </summary>
        /// <returns>
        /// Current state of the interception driver.
        /// </returns>
        public static DriverState GetDriverState()
        {
            const string DriverName = "keyboard";

            SelectQuery query = new SelectQuery("Win32_SystemDriver");
            query.Condition = string.Format("Name = '{0}'", DriverName);
            
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            var drivers = searcher.Get();

            bool fileOK = false;
            string fileName = Path.Combine(GetSystem32DirectoryPath(), "drivers", "keyboard.sys");
            if (File.Exists(fileName))
            {
                var fileInfo = FileVersionInfo.GetVersionInfo(fileName);
                if (fileInfo.CompanyName == "Oblita" &&
                    fileInfo.ProductVersion == "1.00" &&
                    fileInfo.FileDescription == "Keyboard Upper Filter Driver")
                {
                    fileOK = true;
                }
            }

            if (drivers.Count == 0 && fileOK)
            {
                return DriverState.RebootRequired;
            }

            if (drivers.Count > 0 && fileOK)
            {
                return DriverState.Installed;
            }

            return DriverState.NotInstalled;
        }

        public static void Install(string fullPath)
        {
            string output = null;

            try
            {
                ProcessStartInfo ps = new ProcessStartInfo(fullPath, "/install");
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.UseShellExecute = false;
                ps.RedirectStandardOutput = true;
                ps.Verb = "runas";

                using (Process p = Process.Start(ps))
                {
                    output = p.StandardOutput.ReadToEnd();

                    // Waits for the process to exit must come *after* StandardOutput is "empty"
                    // so that we don't deadlock because the intermediate kernel pipe is full.
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                string message;
                if (ex.Message == "The requested operation requires elevation")
                {
                    message = "You must run the application as administrator.";
                }
                else
                {
                    message = ex.Message;
                }

                MessageBox.Show(
                    "Keyboard driver installation failed!\r\n" + message,
                    "Keyboard driver error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string lastLine = lines[lines.Length - 1];
                MessageBox.Show(lastLine, "Driver information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void Uninstall(string fullPath)
        {
            string output = null;

            try
            {
                ProcessStartInfo ps = new ProcessStartInfo(fullPath, "/uninstall");
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.UseShellExecute = false;
                ps.RedirectStandardOutput = true;
                ps.Verb = "runas";

                // Starts the process
                using (Process p = Process.Start(ps))
                {
                    // Reads the output to a string
                    output = p.StandardOutput.ReadToEnd();

                    // Waits for the process to exit must come *after* StandardOutput is "empty"
                    // so that we don't deadlock because the intermediate kernel pipe is full.
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                string message;
                if (ex.Message == "The requested operation requires elevation")
                {
                    message = "You must run the application as administrator.";
                }
                else
                {
                    message = ex.Message;
                }

                MessageBox.Show(
                    "Keyboard driver uninstallation failed!\r\n" + message,
                    "Keyboard driver error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string lastLine = lines[lines.Length - 1];
                MessageBox.Show(lastLine, "Driver information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static bool IsKeyboard(uint device)
        {
            return NativeMethods.IsKeyboard((int)device) > 0;
        }

        private static string GetSystem32DirectoryPath()
        {
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string system32Directory = Path.Combine(winDir, "system32");
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                // For 32-bit processes on 64-bit systems, %windir%\system32 folder
                // can only be accessed by specifying %windir%\sysnative folder.
                system32Directory = Path.Combine(winDir, "sysnative");
            }

            return system32Directory;
        }
    }
}
