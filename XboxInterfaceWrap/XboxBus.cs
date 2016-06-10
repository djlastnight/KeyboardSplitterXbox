namespace XboxInterfaceWrap
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using XboxInterfaceWrap;

    public static class XboxBus
    {
        public static bool IsInstalled()
        {
            return NativeMethods.VBusExists();
        }

        public static bool Install()
        {
            if (IsInstalled())
            {
                return true;
            }

            string output = null;
            try
            {
                string devConFullPath = LoadDriverResourcesAndGetDevconPath();
                var startInfo = new ProcessStartInfo("devcon.exe ", @"install ScpVBus.inf Root\ScpVBus");
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

                bool result = IsInstalled();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Uninstall(out int exitCode)
        {
            if (!IsInstalled())
            {
                exitCode = 0;
                return "Already uninstalled";
            }

            VirtualXboxController.UnPlug(1, true);
            VirtualXboxController.UnPlug(2, true);
            VirtualXboxController.UnPlug(3, true);
            VirtualXboxController.UnPlug(4, true);

            string output = null;
            try
            {
                string devConFullPath = LoadDriverResourcesAndGetDevconPath();
                var startInfo = new ProcessStartInfo("devcon.exe ", @"remove Root\ScpVBus");
                startInfo.WorkingDirectory = Path.GetDirectoryName(devConFullPath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";
                using (var proc = Process.Start(startInfo))
                {
                    output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    exitCode = proc.ExitCode;
                    return output;
                }
            }
            catch (Exception ex)
            {
                exitCode = -1;
                return ex.Message;
            }
        }

        private static string LoadDriverResourcesAndGetDevconPath()
        {
            string resName = "XboxInterfaceWrap.Driver.";
            resName += Environment.Is64BitOperatingSystem ? "x64." : "x86.";
            var executingAssembly = Assembly.GetExecutingAssembly();
            const string TempSubfolder = "XboxInterfaceWrap";

            UnmanagedAssemblyLoader.Load(executingAssembly, resName + "scpvbus.cat", "scpvbus.cat", TempSubfolder, false);
            UnmanagedAssemblyLoader.Load(executingAssembly, resName + "ScpVBus.inf", "ScpVBus.inf", TempSubfolder, false);
            UnmanagedAssemblyLoader.Load(executingAssembly, resName + "ScpVBus.sys", "ScpVBus.sys", TempSubfolder, false);
            UnmanagedAssemblyLoader.Load(executingAssembly, resName + "WdfCoinstaller01009.dll", "WdfCoinstaller01009.dll", TempSubfolder, false);
            string devConFullPath = UnmanagedAssemblyLoader.Load(executingAssembly, resName + "devcon.exe", "devcon.exe", TempSubfolder, true);

            return devConFullPath;
        }
    }
}