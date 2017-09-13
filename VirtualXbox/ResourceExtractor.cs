namespace VirtualXbox
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;

    internal static class ResourceExtractor
    {
        /// <summary>
        /// Example resource location 'MyProject.Lib.MyFile.dll'
        /// </summary>
        /// <param name="manifestResourceLocation"></param>
        /// <returns>The full path to the extracted resource file</returns>
        public static string ExtractResource(Assembly assembly, string manifestResourceLocation, string targetDirectory = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (manifestResourceLocation == null)
            {
                throw new ArgumentNullException("manifestResourceLocation");
            }

            var tokens = manifestResourceLocation.Split(new char[] { '.' });
            if (tokens.Length < 2)
            {
                throw new InvalidOperationException(
                    "manifestResourceLocation parameter must contains at least one '.' (dot)! Example 'MyProject.Lib.MyFile.dll'");
            }

            if (targetDirectory == null)
            {
                string assemblyName = assembly.GetName().Name;
                string assemblyVersion = assembly.GetName().Version.ToString();

                targetDirectory = Path.Combine(Path.GetTempPath(), assemblyName + " " + assemblyVersion + " resources");
            }

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string resourceName = tokens[tokens.Length - 2] + "." + tokens[tokens.Length - 1];
            string fileFullPath = Path.Combine(targetDirectory, resourceName);

            using (var stream = assembly.GetManifestResourceStream(manifestResourceLocation))
            {
                if (stream == null)
                {
                    throw new Exception(resourceName + " is not marked as embedded resource [assembly name: " + assembly.GetName().Name + "]!");
                }

                byte[] data = new BinaryReader(stream).ReadBytes((int)stream.Length);
                if (!File.Exists(fileFullPath))
                {
                    File.WriteAllBytes(fileFullPath, data);
                }
                else
                {
                    string existingMd5 = ComputeMD5CheckSum(File.ReadAllBytes(fileFullPath));
                    string embeddedMd5 = ComputeMD5CheckSum(data);
                    if (existingMd5 != embeddedMd5)
                    {
                        // overwriting the file, because it is not the same
                        File.WriteAllBytes(fileFullPath, data);
                    }
                }
            }

            return fileFullPath;
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
