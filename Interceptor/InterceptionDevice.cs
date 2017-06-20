using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interceptor
{
    public abstract class InterceptionDevice
    {
        private string friendlyName;

        public uint DeviceID { get; internal set; }

        public string HardwareID { get; internal set; }

        public string StrongName { get; internal set; }

        public string FriendlyName
        {
            get
            {
                if (this.friendlyName != null)
                {
                    return this.friendlyName;
                }

                if (this.StrongName == "None")
                {
                    return "OnScreen Mouse Feeder";
                }

                try
                {
                    string hardwareID = this.HardwareID;
                    if (hardwareID.Contains("REV"))
                    {
                        // removing the revision from the hardware id
                        int revision_index = hardwareID.IndexOf("REV");
                        int ampersandIndex = hardwareID.IndexOf("&", revision_index);
                        hardwareID = hardwareID.Substring(0, revision_index) + hardwareID.Substring(ampersandIndex + 1);
                    }

                    using (var rootKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Enum\" + hardwareID))
                    {
                        var deviceType = this.IsKeyboard ? "Keyboard" : "Mouse";
                        var subKeys = this.GetAllSubKeys(rootKey).Where(x => x.GetValue("Class") != null && x.GetValue("Class").ToString() == deviceType);

                        var fullDescription = subKeys.Select(x => x.GetValue("DeviceDesc")).First().ToString();
                        foreach (var subkey in subKeys)
                        {
                            subkey.Dispose();
                        }

                        string friendlyName = fullDescription.Substring(fullDescription.ToString().IndexOf(';') + 1);

                        this.friendlyName = friendlyName;
                    }
                }
                catch (Exception)
                {
                    this.friendlyName = "n/a";
                }

                return this.friendlyName;
            }
        }

        public abstract bool IsKeyboard { get; }

        public virtual bool IsTheSameAs(InterceptionDevice deviceToCompare)
        {
            var props = deviceToCompare.GetType().GetProperties();

            foreach (var property in props)
            {
                var v1 = property.GetValue(this, null);
                var v2 = property.GetValue(deviceToCompare, null);
                if (!v1.Equals(v2))
                {
                    return false;
                }
            }

            return true;
        }

        private List<RegistryKey> GetAllSubKeys(RegistryKey rootKey, bool recursive = false)
        {
            List<RegistryKey> allKeys = new List<RegistryKey>();
            foreach (var subkeyName in rootKey.GetSubKeyNames())
            {
                if (subkeyName == "Properties")
                {
                    continue;
                }

                var subkey = rootKey.OpenSubKey(subkeyName);
                if (subkey != null)
                {
                    allKeys.Add(subkey);
                    allKeys.AddRange(this.GetAllSubKeys(subkey, true));
                }
            }

            return allKeys;
        }
    }
}
