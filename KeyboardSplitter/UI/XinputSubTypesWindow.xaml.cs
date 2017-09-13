namespace KeyboardSplitter.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using KeyboardSplitter.Helpers;
    using XinputWrapper.Enums;

    /// <summary>
    /// Interaction logic for ManageSubTypes.xaml
    /// </summary>
    public partial class XinputSubTypesWindow : CustomWindow
    {
        public XinputSubTypesWindow()
        {
            this.InitializeComponent();
        }

        private void BrowseClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.AutoUpgradeEnabled = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "*.exe";
            dialog.DereferenceLinks = true;
            dialog.Filter = "Executable file (*.exe)|*.exe";
            dialog.Multiselect = false;
            dialog.SupportMultiDottedExtensions = false;
            dialog.Title = "Choose game or application";
            dialog.ValidateNames = true;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                var ext = System.IO.Path.GetExtension(dialog.FileName);
                if (ext.ToLower() != ".exe")
                {
                    Controls.MessageBox.Show(
                        "You must provide an exe file!",
                        ApplicationInfo.AppName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                this.fileName.Text = System.IO.Path.GetFileName(dialog.FileName);
                this.fileName.ToolTip = dialog.FileName;
                this.subTypeGroupBox.IsEnabled = true;
                this.patchButton.IsEnabled = true;
                this.CheckIfAlreadyPatched();
            }
        }

        private void CheckIfAlreadyPatched()
        {
            var exePath = this.fileName.ToolTip.ToString();
            var iniPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "xinput.ini");
            if (!System.IO.File.Exists(iniPath))
            {
                return;
            }

            var lines = System.IO.File.ReadAllLines(iniPath);
            if (lines.Length == 0)
            {
                return;
            }
            else
            {
                if (lines[0].ToLower() != "[SubTypes]".ToLower())
                {
                    return;
                }
            }

            for (int i = 1; i < Math.Min(5, lines.Length); i++)
            {
                var line = lines[i];
                if (!line.Contains('='))
                {
                    continue;
                }

                var tokens = line.Split(new char[] { '=' });
                if (tokens.Length != 2)
                {
                    continue;
                }

                int tokenOne;
                if (!int.TryParse(tokens[0], out tokenOne))
                {
                    continue;
                }

                if (tokenOne < 1 || tokenOne > 4)
                {
                    continue;
                }

                int tokenTwo;
                if (!int.TryParse(tokens[1], out tokenTwo))
                {
                    continue;
                }

                if (!Enum.IsDefined(typeof(ControllerSubtype), tokenTwo))
                {
                    continue;
                }

                switch (tokenOne)
                {
                    case 1:
                        this.id1.SelectedItem = (ControllerSubtype)tokenTwo;
                        break;
                    case 2:
                        this.id2.SelectedItem = (ControllerSubtype)tokenTwo;
                        break;
                    case 3:
                        this.id3.SelectedItem = (ControllerSubtype)tokenTwo;
                        break;
                    case 4:
                        this.id4.SelectedItem = (ControllerSubtype)tokenTwo;
                        break;
                    default:
                        break;
                }
            }
        }

        private void PathButtonClicked(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(this.fileName.Text));
            bool isRunning = processes.FirstOrDefault(p => p.MainModule.FileName.StartsWith(this.fileName.ToolTip.ToString())) != default(Process);

            if (!isRunning)
            {
                this.PatchExe();
            }
            else
            {
                Controls.MessageBox.Show(
                    "Patching failed, because " + this.fileName.Text + " is currently running!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void PatchExe()
        {
            string exePath = this.fileName.ToolTip.ToString();
            if (!System.IO.File.Exists(exePath))
            {
                Controls.MessageBox.Show(
                    exePath + " does not exists!",
                    ApplicationInfo.AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            var dir = System.IO.Path.GetDirectoryName(exePath);
            var iniPath = System.IO.Path.Combine(dir, "xinput.ini");
            List<string> iniLines = new List<string>();
            iniLines.Add("[SubTypes]");
            List<string> comboboxValues = new List<string>();
            comboboxValues.Add(this.GetControllerTypeNumber(this.id1));
            comboboxValues.Add(this.GetControllerTypeNumber(this.id2));
            comboboxValues.Add(this.GetControllerTypeNumber(this.id3));
            comboboxValues.Add(this.GetControllerTypeNumber(this.id4));

            for (int i = 0; i < 4; i++)
            {
                if (comboboxValues[i] != string.Empty)
                {
                    iniLines.Add((i + 1) + "=" + comboboxValues[i]);
                }
            }

            if (iniLines.Count > 1)
            {
                // Check write access
                if (!App.HasWriteAccessToFolder(dir))
                {
                    Controls.MessageBox.Show(
                        "Patching failed, because you do not have a write permissions to exe's directory!",
                        ApplicationInfo.AppNameVersion,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                System.IO.File.WriteAllLines(iniPath, iniLines);
                this.CopyXinputDlls(dir);
                Controls.MessageBox.Show(
                    this.fileName.Text + " was succesfully patched. Now you can start it.",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.Close();
            }
            else
            {
                Controls.MessageBox.Show(
                    this.fileName.Text + " not patched, because you have to change at least one controller subtype!",
                    ApplicationInfo.AppNameVersion,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }
        }

        private void CopyXinputDlls(string dir)
        {
            var resourceFile = ResourceExtractor.ExtractResource(Assembly.GetExecutingAssembly(), "KeyboardSplitter.Lib.xinput.dll", dir);

            var xinput13 = System.IO.Path.Combine(dir, "xinput1_3.dll");
            var xinput14 = System.IO.Path.Combine(dir, "xinput1_4.dll");
            var xinput910 = System.IO.Path.Combine(dir, "XINPUT9_1_0.DLL");
            
            System.IO.File.Copy(resourceFile, xinput13, true);
            System.IO.File.Copy(resourceFile, xinput14, true);
            System.IO.File.Copy(resourceFile, xinput910, true);
            System.IO.File.Delete(resourceFile);
        }

        private string GetControllerTypeNumber(System.Windows.Controls.ComboBox comboBox)
        {
            if (comboBox.SelectedIndex != -1)
            {
                ControllerSubtype subType;
                var parseOK = Enum.TryParse(comboBox.SelectedItem.ToString(), out subType);
                if (parseOK)
                {
                    int subTypeInt = (int)subType;
                    return subTypeInt.ToString();
                }
            }

            return string.Empty;
        }
    }
}
