namespace KeyboardSplitter.UI
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Animation;
    using KeyboardSplitter.Behaviors;

    /// <summary>
    /// Helper window, which displays information
    /// about the current application User Interface.
    /// </summary>
    public partial class HelpDialog : CustomWindow
    {
        public HelpDialog()
        {
            this.InitializeComponent();
            this.FillContents();

            // animations
            this.BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(1))));
            DoubleAnimation scrollAnimation = new DoubleAnimation(1200, 0, new Duration(TimeSpan.FromSeconds(1)));
            var sb = new Storyboard();
            sb.Children.Add(scrollAnimation);
            Storyboard.SetTarget(scrollAnimation, this.scrollViewer);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));
            sb.Begin();
        }

        private void FillContents()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("1. Slots count: Determines the virtual gamepads total count. Possible values 1, 2, 3 or 4.");
            sb.AppendLine("2. Block choosen keyboards: If checked, intercepts the keyboard input, so Windows OS will not receive it.");
            sb.AppendLine("3. Start/Stop buttons: Starts or stops the emulation process. When you hit start, the application mounts " +
                "the virtual controllers and waits for keyboard input, which will feed them.");
            sb.AppendLine("4. Choose keyboard. Selects the keyboard, which will feed the virtual controller.");
            sb.AppendLine("5. Detect keyboard. Waiting for user to press any key from the desired keyboard and selects it (see pt.4)");
            sb.AppendLine("6. Preset: Determines the used preset. To create new preset, just type its name in this field and save it (see pt.7)");
            sb.AppendLine("7. Save preset: Saves the current preset into memory. On app exit, presets will be saved to presets.xml file.");
            sb.AppendLine("8. Delete preset: Deletes the current preset from the memory. All slots that uses that preset, will load the empty preset");
            sb.AppendLine("9. Bind button 'A' to keyboard 'Enter': When checked, enables custom binding rule, for users who likes to accept the menus " +
                "by hitting 'Enter' key. The same action could be reached by adding custom function.");
            sb.AppendLine("10. Choose keyboard key: Selects keyboard key, which will feed the coresponding xbox function (button/trigger/axis/d-pad).");
            sb.AppendLine("11. Detect key: Waiting for user to press the desired key to which the coresponding function will be assigned (see pt.10).");
            sb.AppendLine("12. Delete custom function: Removes the custom function from the current preset.");
            sb.AppendLine("13. Choose custom function: Selects xbox custom function from the list. Custom functions are used, " +
                "when user wants to simulate two or more xbox functions at a time. Example scenario: Xbox Button A and Xbox Button B are pressed together, when " +
                "user press 'Space' on keyboard.");
            sb.AppendLine("14. Add custom function: Adds custom xbox function to the current preset. This button is disabled for built-in presets.");
            sb.AppendLine("15. Keyboards input monitor: Monitors all connected keyboards input. By default this panel is collapsed, " +
                "also it auto collapse after 60 seconds to save CPU time and to avoid using this app as keylogger.");

            this.contentsTextBox.Text = sb.ToString();
        }
    }
}
