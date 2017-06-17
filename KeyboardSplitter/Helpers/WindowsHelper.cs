﻿using KeyboardSplitter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyboardSplitter.Helpers
{
    public static class WindowsHelper
    {
        public static string GetCurrentWindowsVersion()
        {
            var name = GetWindowsName();
            var arch = GetWindowsArch();
            var build = GetWindowsBuild();
            var sp = GetWindowsServicePack();
            return string.Format("{0} {1} build {2} {3}", name, arch, build, sp);
        }

        private static string GetWindowsName()
        {
            var os = Environment.OSVersion;

            if (os.Version.Major == 5)
            {
                return "Windows Xp";
            }

            if (os.Version.Major == 6)
            {
                switch (os.Version.Minor)
                {
                    case 0: return "Windows Vista";
                    case 1: return "Windows 7";
                    case 2: return "Windows 8";
                    case 3: return "Windows 8.1";
                }
            }

            if (os.Version.Major == 10)
            {
                if (os.Version.Minor == 0)
                {
                    return "Windows 10";
                }
            }

            return Environment.OSVersion.VersionString;
        }

        private static string GetWindowsArch()
        {
            return Environment.Is64BitOperatingSystem ? "x64" : "x86";
        }

        private static string GetWindowsBuild()
        {
            return Environment.OSVersion.Version.Build.ToString();
        }

        private static string GetWindowsServicePack()
        {
            return Environment.OSVersion.ServicePack;
        }
    }
}