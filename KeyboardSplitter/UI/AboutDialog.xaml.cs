namespace KeyboardSplitter.UI
{
    using System;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Window, which displays information about the current application.
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            this.InitializeComponent();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This application is based on the following open source github projects:");
            sb.AppendLine("-Interception C++ keyboard driver, author: Francisco Lopes (oblitum)");
            sb.AppendLine("-C# wrapper for Interception driver, author: Jason Pang (jasonpang)");
            sb.AppendLine("-ScpVbus, authors: Benjamin Höglinger (nefarius), Shaul Eizikovich (shauleiz)");
            sb.AppendLine("-vXboxInterface, author: Shaul Eizikovich (shauleiz)");
            sb.AppendLine();
            sb.AppendLine("Requirements:");
            sb.AppendLine("- Microsoft Windows Xp/Vista/7/8/8.1/10");
            sb.AppendLine("- .NET Framework 4 or newer");
            sb.AppendLine("- Visual C++ Redistributable Packages for Visual Studio 2013");
            sb.AppendLine("- Xbox 360 Accessories Software (Xbox 360 Controller Driver)");
            sb.AppendLine("* Windows 8 and newer has xbox driver installed by default");
            sb.AppendLine();
            sb.AppendLine("This application comes AS IS, with no warranty of any kind!");
            sb.AppendLine("Use it on your own risk!");
            sb.AppendLine("Ivan Yankov (djlastnight), 2017");

            this.textBox.Text = sb.ToString();
        }

        public AboutDialog(string title)
            : this()
        {
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            if (Application.Current != null)
            {
                this.Owner = Application.Current.MainWindow;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;   
            }

            this.Title = title;
        }
    }
}
