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
            sb.AppendLine("This application is based on:");
            sb.AppendLine("-Interception driver, author: oblitum");
            sb.AppendLine("-ScpVbus, authors: nefarius, shauleiz");
            sb.AppendLine("-vXboxInterface, author: shauleiz");
            sb.AppendLine();
            sb.AppendLine("Requirements:");
            sb.AppendLine("- .NET Framework 4");
            sb.AppendLine("- Xbox 360 Accessories Software (Xbox 360 Controller Driver)");
            sb.AppendLine();
            sb.AppendLine("This application comes AS IS, with no warranty!");
            sb.AppendLine("Use on your own risk!");
            sb.AppendLine("djlastnight, 2016");

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
