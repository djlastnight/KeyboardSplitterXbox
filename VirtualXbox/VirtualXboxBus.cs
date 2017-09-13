namespace VirtualXbox
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class VirtualXboxBus
    {
        public static bool IsInstalled
        {
            get { return NativeMethods.VBusExists(); }
        }

        public static int EmptySlotsCount
        {
            get
            {
                int count;
                if (NativeMethods.GetEmptyBusSlotsCount(out count))
                {
                    return count;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static string Install()
        {
            if (VirtualXboxBus.IsInstalled)
            {
                return "Error: ScpVBus is already installed!";
            }

            string output = null;
            try
            {
                string devConFullPath = LoadDriverResourcesAndGetDevconPath();
                var startInfo = new ProcessStartInfo(devConFullPath + " ", @"install ScpVBus.inf Root\ScpVBus");
                startInfo.WorkingDirectory = Path.GetDirectoryName(devConFullPath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";
                using (var proc = Process.Start(startInfo))
                {
                    output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                }

                return "Output from ScpVBus installtion: " + output;
            }
            catch (Exception e)
            {
                return "ScpVBus installation failed!\r\n" + e.ToString();
            }
        }

        public static string Uninstall()
        {
            if (!VirtualXboxBus.IsInstalled)
            {
                return "Error: ScpVBus is already uninstalled";
            }

            VirtualXboxController.UnPlug(1, true);
            VirtualXboxController.UnPlug(2, true);
            VirtualXboxController.UnPlug(3, true);
            VirtualXboxController.UnPlug(4, true);

            string output = null;
            try
            {
                string devConFullPath = LoadDriverResourcesAndGetDevconPath();
                var startInfo = new ProcessStartInfo(devConFullPath + " ", @"remove Root\ScpVBus");
                startInfo.WorkingDirectory = Path.GetDirectoryName(devConFullPath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";
                using (var proc = Process.Start(startInfo))
                {
                    output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    return "Output from ScpVBus uninstall: " + output;
                }
            }
            catch (Exception ex)
            {
                return "ScpVbus uninstall failed:\r\n" + ex.ToString();
            }
        }

        private static string LoadDriverResourcesAndGetDevconPath()
        {
            string arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            string manifestPath = "VirtualXbox.Driver." + arch + ".";
            ResourceExtractor.ExtractResourceToDirectory(manifestPath + "scpvbus.cat");
            ResourceExtractor.ExtractResourceToDirectory(manifestPath + "ScpVBus.inf");
            ResourceExtractor.ExtractResourceToDirectory(manifestPath + "ScpVBus.sys");
            ResourceExtractor.ExtractResourceToDirectory(manifestPath + "WdfCoinstaller01009.dll");
            return ResourceExtractor.ExtractResourceToDirectory(manifestPath + "devcon.exe");
        }
    }
}