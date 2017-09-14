namespace KeyboardSplitter.Helpers
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.Win32;

    /// <summary>
    /// Filter for notifications reported by <see cref="RegistryMonitor"/>.
    /// </summary>
    [Flags]
    public enum RegChangeNotifyFilter
    {
        /// <summary>Notify the caller if a subkey is added or deleted.</summary>
        Key = 1,

        /// <summary>Notify the caller of changes to the attributes of the key,
        /// such as the security descriptor information.</summary>
        Attribute = 2,

        /// <summary>Notify the caller of changes to a value of the key. This can
        /// include adding or deleting a value, or changing an existing value.</summary>
        Value = 4,

        /// <summary>Notify the caller of changes to the security descriptor
        /// of the key.</summary>
        Security = 8,
    }

    /// <summary>
    /// <b>RegistryMonitor</b> allows you to monitor specific registry key.
    /// </summary>
    /// <remarks>
    /// If a monitored registry key changes, an event is fired. You can subscribe to these
    /// events by adding a delegate to <see cref="RegChanged"/>.
    /// <para>The Windows API provides a function
    /// <a href="http://msdn.microsoft.com/library/en-us/sysinfo/base/regnotifychangekeyvalue.asp">
    /// RegNotifyChangeKeyValue</a>, which is not covered by the
    /// <see cref="Microsoft.Win32.RegistryKey"/> class. <see cref="RegistryMonitor"/> imports
    /// that function and encapsulates it in a convenient manner.
    /// </para>
    /// </remarks>
    /// <example>
    /// This sample shows how to monitor <c>HKEY_CURRENT_USER\Environment</c> for changes:
    /// <code>
    /// public class MonitorSample
    /// {
    ///     static void Main() 
    ///     {
    ///         RegistryMonitor monitor = new RegistryMonitor(RegistryHive.CurrentUser, "Environment");
    ///         monitor.RegChanged += new EventHandler(OnRegChanged);
    ///         monitor.Start();
    ///
    ///         while(true);
    /// 
    ///         monitor.Stop();
    ///     }
    ///
    ///     private void OnRegChanged(object sender, EventArgs e)
    ///     {
    ///         Console.WriteLine("registry key has changed");
    ///     }
    /// }
    /// </code>
    /// </example>
    public class RegistryMonitor : IDisposable
    {
        private const int KeyQueryValue = 0x0001;
        private const int KeyNotify = 0x0010;
        private const int StandardRightsRead = 0x00020000;

        private static readonly IntPtr HkeyClassesRoot = new IntPtr(unchecked((int)0x80000000));
        private static readonly IntPtr HkeyCurrentUser = new IntPtr(unchecked((int)0x80000001));
        private static readonly IntPtr HkeyLocalMachine = new IntPtr(unchecked((int)0x80000002));
        private static readonly IntPtr HkeyUsers = new IntPtr(unchecked((int)0x80000003));
        private static readonly IntPtr HkeyPerformanceData = new IntPtr(unchecked((int)0x80000004));
        private static readonly IntPtr HkeyCurrentConfig = new IntPtr(unchecked((int)0x80000005));
        private static readonly IntPtr HkeyDynData = new IntPtr(unchecked((int)0x80000006));

        private IntPtr registryHive;
        private string registrySubName;
        private object threadLock = new object();
        private Thread thread;
        private bool disposed = false;
        private ManualResetEvent eventTerminate = new ManualResetEvent(false);

        private RegChangeNotifyFilter regFilter = RegChangeNotifyFilter.Key | RegChangeNotifyFilter.Attribute |
                                                   RegChangeNotifyFilter.Value | RegChangeNotifyFilter.Security;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
        /// </summary>
        /// <param name="registryKey">The registry key to monitor.</param>
        public RegistryMonitor(RegistryKey registryKey)
        {
            this.InitRegistryKey(registryKey.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public RegistryMonitor(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentNullException("name");
            }

            this.InitRegistryKey(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
        /// </summary>
        /// <param name="registryHive">The registry hive.</param>
        /// <param name="subKey">The sub key.</param>
        public RegistryMonitor(RegistryHive registryHive, string subKey)
        {
            this.InitRegistryKey(registryHive, subKey);
        }

        /// <summary>
        /// Occurs when the specified registry key has changed.
        /// </summary>
        public event EventHandler RegChanged;

        /// <summary>
        /// Occurs when the access to the registry fails.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Gets or sets the <see cref="RegChangeNotifyFilter">RegChangeNotifyFilter</see>.
        /// </summary>
        public RegChangeNotifyFilter RegChangeNotifyFilter
        {
            get
            {
                return this.regFilter;
            }

            set
            {
                lock (this.threadLock)
                {
                    if (this.IsMonitoring)
                    {
                        throw new InvalidOperationException("Monitoring thread is already running");
                    }

                    this.regFilter = value;
                }
            }
        }

        /// <summary>
        /// <b>true</b> if this <see cref="RegistryMonitor"/> object is currently monitoring;
        /// otherwise, <b>false</b>.
        /// </summary>
        public bool IsMonitoring
        {
            get { return this.thread != null; }
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start monitoring.
        /// </summary>
        public void Start()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(null, "This instance is already disposed");
            }

            lock (this.threadLock)
            {
                if (!this.IsMonitoring)
                {
                    this.eventTerminate.Reset();
                    this.thread = new Thread(new ThreadStart(this.MonitorThread));
                    this.thread.IsBackground = true;
                    this.thread.Start();
                }
            }
        }

        /// <summary>
        /// Stops the monitoring thread.
        /// </summary>
        public void Stop()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(null, "This instance is already disposed");
            }

            lock (this.threadLock)
            {
                Thread thread = this.thread;
                if (thread != null)
                {
                    this.eventTerminate.Set();
                    thread.Join();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();
                this.disposed = true;
                if (this.eventTerminate != null)
                {
                    this.eventTerminate.Dispose();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="RegChanged"/> event.
        /// </summary>
        /// <remarks>
        /// <p>
        /// <b>OnRegChanged</b> is called when the specified registry key has changed.
        /// </p>
        /// <note type="inheritinfo">
        /// When overriding <see cref="OnRegChanged"/> in a derived class, be sure to call
        /// the base class's <see cref="OnRegChanged"/> method.
        /// </note>
        /// </remarks>
        protected virtual void OnRegChanged()
        {
            EventHandler handler = this.RegChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Raises the <see cref="Error"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> which occured while watching the registry.</param>
        /// <remarks>
        /// <p>
        /// <b>OnError</b> is called when an exception occurs while watching the registry.
        /// </p>
        /// <note type="inheritinfo">
        /// When overriding <see cref="OnError"/> in a derived class, be sure to call
        /// the base class's <see cref="OnError"/> method.
        /// </note>
        /// </remarks>
        protected virtual void OnError(Exception e)
        {
            ErrorEventHandler handler = this.Error;
            if (handler != null)
            {
                handler(this, new ErrorEventArgs(e));
            }
        }

        private void InitRegistryKey(RegistryHive hive, string name)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    this.registryHive = HkeyClassesRoot;
                    break;

                case RegistryHive.CurrentConfig:
                    this.registryHive = HkeyCurrentConfig;
                    break;

                case RegistryHive.CurrentUser:
                    this.registryHive = HkeyCurrentUser;
                    break;

                case RegistryHive.DynData:
                    this.registryHive = HkeyDynData;
                    break;

                case RegistryHive.LocalMachine:
                    this.registryHive = HkeyLocalMachine;
                    break;

                case RegistryHive.PerformanceData:
                    this.registryHive = HkeyPerformanceData;
                    break;

                case RegistryHive.Users:
                    this.registryHive = HkeyUsers;
                    break;

                default:
                    throw new InvalidEnumArgumentException("hive", (int)hive, typeof(RegistryHive));
            }

            this.registrySubName = name;
        }

        private void InitRegistryKey(string name)
        {
            string[] nameParts = name.Split('\\');

            switch (nameParts[0])
            {
                case "HKEY_CLASSES_ROOT":
                case "HKCR":
                    this.registryHive = HkeyClassesRoot;
                    break;

                case "HKEY_CURRENT_USER":
                case "HKCU":
                    this.registryHive = HkeyCurrentUser;
                    break;

                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    this.registryHive = HkeyLocalMachine;
                    break;

                case "HKEY_USERS":
                    this.registryHive = HkeyUsers;
                    break;

                case "HKEY_CURRENT_CONFIG":
                    this.registryHive = HkeyCurrentConfig;
                    break;

                default:
                    this.registryHive = IntPtr.Zero;
                    throw new ArgumentException("The registry hive '" + nameParts[0] + "' is not supported", "value");
            }

            this.registrySubName = string.Join("\\", nameParts, 1, nameParts.Length - 1);
        }

        private void MonitorThread()
        {
            try
            {
                this.ThreadLoop();
            }
            catch (Exception e)
            {
                this.OnError(e);
            }

            this.thread = null;
        }

        private void ThreadLoop()
        {
            IntPtr registryKey;
            int result = NativeMethods.RegOpenKeyEx(
                this.registryHive,
                this.registrySubName,
                0,
                StandardRightsRead | KeyQueryValue | KeyNotify,
                out registryKey);

            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            try
            {
                AutoResetEvent eventNotify = new AutoResetEvent(false);
                WaitHandle[] waitHandles = new WaitHandle[] { eventNotify, this.eventTerminate };
                while (!this.eventTerminate.WaitOne(0, true))
                {
                    /// Removing the following line, because it uses deprecated property
                    /// IntPtr handle = eventNotify.Handle;
                    IntPtr handle = eventNotify.SafeWaitHandle.DangerousGetHandle();
                    result = NativeMethods.RegNotifyChangeKeyValue(registryKey, true, this.regFilter, handle, true);
                    if (result != 0)
                    {
                        throw new Win32Exception(result);
                    }

                    if (WaitHandle.WaitAny(waitHandles) == 0)
                    {
                        this.OnRegChanged();
                    }
                }
            }
            finally
            {
                if (registryKey != IntPtr.Zero)
                {
                    NativeMethods.RegCloseKey(registryKey);
                }
            }
        }
    }
}