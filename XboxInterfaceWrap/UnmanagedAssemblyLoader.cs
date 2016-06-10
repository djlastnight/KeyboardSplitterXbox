namespace XboxInterfaceWrap
{
        using System;
        using System.IO;
        using System.Reflection;
        using System.Runtime.InteropServices;
        using System.Security.Cryptography;

        internal static class UnmanagedAssemblyLoader
        {
            public static string Load(Assembly assembly, string libraryResourceName, string libraryName, string tempSubFolderName, bool load)
            {
                string tempDir = Path.Combine(Path.GetTempPath(), tempSubFolderName);
                string tempDllPath = Path.Combine(tempDir, libraryName);

                using (Stream s = assembly.GetManifestResourceStream(libraryResourceName))
                {
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    byte[] data = new BinaryReader(s).ReadBytes((int)s.Length);
                    if (!File.Exists(tempDllPath))
                    {
                        File.WriteAllBytes(tempDllPath, data);
                    }
                    else
                    {
                        string existingMd5 = ComputeMD5CheckSum(File.ReadAllBytes(tempDllPath));
                        string embeddedMd5 = ComputeMD5CheckSum(data);
                        if (existingMd5 != embeddedMd5)
                        {
                            // overwriting the file, because it is not the same
                            File.WriteAllBytes(tempDllPath, data);
                        }
                    }
                }

                Environment.SetEnvironmentVariable("PATH", tempDir + ";" + tempDllPath);
                if (load)
                {
                    NativeMethods.LoadLibrary(libraryName);
                }

                return tempDllPath;
            }

            private static string ComputeMD5CheckSum(byte[] buffer)
            {
                using (var md5 = MD5.Create())
                {
                    var checkSum = BitConverter.ToString(md5.ComputeHash(buffer));
                    var correctedCheckSum = checkSum.Replace("-", string.Empty).ToLower();

                    return correctedCheckSum;
                }
            }
        }
}
