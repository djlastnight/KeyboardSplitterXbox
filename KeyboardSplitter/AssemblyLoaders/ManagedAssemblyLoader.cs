namespace KeyboardSplitter.AssemblyLoaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;

    internal static class ManagedAssemblyLoader
    {
        private static Dictionary<string, Assembly> loadedAssemblies = null;

        public static void Load(string embeddedResource, string fileName)
        {
            if (loadedAssemblies == null)
            {
                loadedAssemblies = new Dictionary<string, Assembly>();
            }

            byte[] data = null;
            Assembly assembly = null;
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            using (var stream = currentAssembly.GetManifestResourceStream(embeddedResource))
            {
                // Either the file is not existed or it is not mark as embedded resource
                if (stream == null)
                {
                    throw new Exception(embeddedResource + " is not found in Embedded Resources.");
                }

                // Get byte[] from the file from embedded resource
                data = new byte[(int)stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                try
                {
                    assembly = Assembly.Load(data);

                    // Add the assembly/dll into dictionary
                    loadedAssemblies.Add(assembly.FullName, assembly);
                    return;
                }
                catch
                {
                    // Purposely do nothing
                    // Unmanaged dll or assembly cannot be loaded directly from byte[]
                    // Let the process fall through for next part
                }
            }

            bool fileOk = false;
            string tempFile = string.Empty;

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                string fileHash = BitConverter.ToString(sha1.ComputeHash(data)).Replace("-", string.Empty);

                tempFile = Path.GetTempPath() + fileName;

                if (File.Exists(tempFile))
                {
                    byte[] bb = File.ReadAllBytes(tempFile);
                    string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                    if (fileHash == fileHash2)
                    {
                        fileOk = true;
                    }
                    else
                    {
                        fileOk = false;
                    }
                }
                else
                {
                    fileOk = false;
                }
            }

            if (!fileOk)
            {
                System.IO.File.WriteAllBytes(tempFile, data);
            }

            assembly = Assembly.LoadFile(tempFile);

            loadedAssemblies.Add(assembly.FullName, assembly);
        }

        public static Assembly Get(string assemblyFullName)
        {
            if (loadedAssemblies == null || loadedAssemblies.Count == 0)
            {
                return null;
            }

            if (loadedAssemblies.ContainsKey(assemblyFullName))
            {
                return loadedAssemblies[assemblyFullName];
            }

            return null;
        }
    }
}
