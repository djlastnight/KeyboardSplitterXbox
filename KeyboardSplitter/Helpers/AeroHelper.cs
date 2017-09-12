namespace KeyboardSplitter.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Microsoft.Win32;

    internal struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public Margins(Thickness t)
        {
            this.Left = (int)t.Left;
            this.Right = (int)t.Right;
            this.Top = (int)t.Top;
            this.Bottom = (int)t.Bottom;
        }
    }

    public class AeroHelper
    {
        private static RegistryMonitor registyMonitor;

        static AeroHelper()
        {
            registyMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "Software\\Microsoft\\Windows\\DWM");
            registyMonitor.RegChanged += OnAeroColorChanged;
            registyMonitor.Start();
        }

        public static event EventHandler AeroColorChanged;

        public static bool IsAeroEnabled
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6 && NativeMethods.DwmIsCompositionEnabled();
            }
        }

        public static Brush TryGetAeroColor(bool ignoreTransperancy = false)
        {
            if (!AeroHelper.IsAeroEnabled)
            {
                throw new InvalidOperationException("Aero is not supported or disabled");
            }

            try
            {
                var regValue = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", "00000000");
                string colorizationValue = string.Format("{0:x}", regValue);

                if (ignoreTransperancy)
                {
                    var builder = new StringBuilder(colorizationValue);
                    builder[0] = 'd';
                    builder[1] = '9';
                    colorizationValue = builder.ToString();
                }

                var converter = new BrushConverter();
                var brush = (Brush)converter.ConvertFrom("#" + colorizationValue);
                return brush;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool ExtendGlassFrame(Window window, Thickness margin, bool useTransparentBackground = false)
        {
            if (!AeroHelper.IsAeroEnabled)
            {
                throw new InvalidOperationException("Aero is not supported or disabled.");
            }

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("The Window must be shown before extending glass.");
            }

            // Set the background to transparent from both the WPF and Win32 perspectives
            if (useTransparentBackground)
            {
                window.Background = Brushes.Transparent;
                HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;
            }

            var margins = new Margins(margin);
            NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins);
            return true;
        }

        private static void OnAeroColorChanged(object sender, EventArgs e)
        {
            if (AeroHelper.IsAeroEnabled && AeroColorChanged != null)
            {
                AeroColorChanged(null, EventArgs.Empty);
            }
        }
    }
}
