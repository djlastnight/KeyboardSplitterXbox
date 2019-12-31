namespace InterceptionUninstall
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private enum InterceptionDriverState
        {
            Installed,
            NotInstalled,
            RebootRequired
        }

        private string Uninstall(string interceptionFullPath)
        {
            string output = null;

            try
            {
                ProcessStartInfo ps = new ProcessStartInfo(interceptionFullPath, "/uninstall");
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
                    return message;
                }
                else
                {
                    message = ex.Message;
                }

                return "Interception driver uninstall failed!\r\n\r\n" + message;
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string lastLine = lines[lines.Length - 1];
                return lastLine;
            }

            return "Interception driver uninstalled";
        }

        private InterceptionDriverState GetDriverState()
        {
            const string DriverName = "keyboard";

            var query = new SelectQuery("Win32_SystemDriver");
            query.Condition = string.Format("Name = '{0}'", DriverName);

            ManagementObjectCollection drivers;
            using (var searcher = new ManagementObjectSearcher(query))
            {
                drivers = searcher.Get();
            }

            bool fileOK = false;
            string fileName = Path.Combine(this.GetSystem32DirectoryPath(), "drivers", "keyboard.sys");
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
                return InterceptionDriverState.RebootRequired;
            }

            if (drivers.Count > 0 && fileOK)
            {
                return InterceptionDriverState.Installed;
            }

            return InterceptionDriverState.NotInstalled;
        }

        private string GetSystem32DirectoryPath()
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

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.GetDriverState() == InterceptionDriverState.NotInstalled)
                {
                    MessageBox.Show("Interception is not installed on this computer.", "No action needed");
                    this.Close();
                    return;
                }
            }
            catch (ManagementException)
            {
            }

            string filename = "interception.exe";

            if (!File.Exists(filename))
            {
                MessageBox.Show("Unable to find " + filename + "\r\nPlease download the app again.", "Required file missing");
                this.Close();
                return;
            }

            var fileInfo = FileVersionInfo.GetVersionInfo(filename);
            bool companyOK = fileInfo.CompanyName == "Francisco Lopes";
            bool descriptionOK = fileInfo.FileDescription == "Interception command line installation tool";
            bool versionOK = fileInfo.FileVersion == "1.00 built by: WinDDK";
            bool productOK = fileInfo.ProductVersion == "1.00";
            if (!companyOK || !descriptionOK || !versionOK || !productOK)
            {
                MessageBox.Show(string.Format("Wrong {0} file detected", filename), "Error");
                this.Close();
                return;
            }

            var message = this.Uninstall(filename);
            if (message == "You must run the application as administrator.")
            {
                MessageBox.Show(message, "Administrator required");
                this.Close();
                return;
            }
            else if (message == "Interception uninstalled. You must reboot for this to take effect.")
            {
                message = "Success. Please restart the computer.";
                MessageBox.Show(message, "Uninstall successfull");
                this.Close();
                return;
            }
            else if (message.Contains("keyboard.sys") || message == "Error deleting keyboard driver key")
            {
                message = "FAILED. Restart the computer and try again.";
                MessageBox.Show(message, "Uninstall failed");
                this.Close();
                return;
            }

            MessageBox.Show(message, "Uninstall interception");
        }
    }
}