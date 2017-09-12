namespace Interceptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interceptor.Enums;
    using Microsoft.Win32;

    public abstract class InterceptionDevice
    {
        private const string FallbackDeviceFriendlyName = "n/a";

        private string friendlyName;

        private string strongName;

        protected InterceptionDevice(uint deviceId, string hardwareId)
        {
            if (hardwareId == null)
            {
                throw new ArgumentNullException("hardwareId");
            }

            this.DeviceID = deviceId;
            this.HardwareID = hardwareId;
        }

        public abstract InterceptionDeviceType DeviceType { get; }

        public uint DeviceID { get; private set; }

        public string HardwareID { get; private set; }

        public string StrongName
        {
            get
            {
                if (this.strongName != null)
                {
                    return this.strongName;
                }

                if (this.DeviceID < 1 ||
                    this.DeviceID > Interception.MaxDeviceCount ||
                    this.HardwareID == string.Empty)
                {
                    this.strongName = "Unknown";
                    return this.strongName;
                }

                var seqientialNumber = this.DeviceType == InterceptionDeviceType.Keyboard ? this.DeviceID : this.DeviceID - 10;
                this.strongName = this.DeviceType + "_" + seqientialNumber.ToString().PadLeft(2, '0');

                return this.strongName;
            }
        }

        public string FriendlyName
        {
            get
            {
                if (this.friendlyName != null)
                {
                    return this.friendlyName;
                }

                try
                {
                    string guid = this.HardwareID;
                    if (guid.Contains("REV"))
                    {
                        // removing the revision from the hardware id
                        int revision_index = guid.IndexOf("REV");
                        int ampersandIndex = guid.IndexOf("&", revision_index);
                        guid = guid.Substring(0, revision_index) + guid.Substring(ampersandIndex + 1);
                    }

                    using (var rootKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Enum\" + guid))
                    {
                        var subKeys = this.GetAllSubKeys(rootKey).Where(x => x.GetValue("Class") != null && x.GetValue("Class").ToString() == this.DeviceType.ToString());

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
                    this.friendlyName = InterceptionDevice.FallbackDeviceFriendlyName;
                }

                return this.friendlyName;
            }
        }

        public virtual bool HasTheSamePropertiesAs(InterceptionDevice deviceToCompare)
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
