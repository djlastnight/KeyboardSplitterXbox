namespace KeyboardSplitter.UI
{
    using System;
    using System.Text;
    using System.Windows;

    public partial class FAQ : Window
    {
        public FAQ()
        {
            this.InitializeComponent();
            this.Title = "Frequently Asked Questions";
            var sb = new StringBuilder();

            sb.AppendLine("Q: I have installed the built-in drivers required by this application, and then I restarted my PC. " +
                "Does this means I do not need this application anymore?");
            sb.AppendLine("A: No, you need it. Drivers are required for the application to do its work. Everytime you want to play " +
                "a game with more than 1 keyboard, you should use this application.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I want to uninstall the built-in drivers. Is this possible?");
            sb.AppendLine("A: Yes, just go to 'Drivers' menu and click 'Uninstall built-in drivers'. " +
                "Then you need to restart your PC to completely remove them from the system.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: What should I start first: the game or this app?");
            sb.AppendLine("A: Generally, this app should be started first, but it depends on the game. " +
                "This app mounts the virtual gamepads, when the emulation is started " +
                "(when start button is grayed out). If the game supports realtime gamepad plugging/unplugging, " +
                "you can start the app after the game. If you are not sure - start this app first, then the game.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: This app is running, but nothing happens ingame. Does it works?");
            sb.AppendLine("A: Probably, you forget to press the start button.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I tried several times to press the start button, but the app displays an error. What to do?");
            sb.AppendLine("A: For every slot, choose keyboard from the list. You can also use the keyboard " +
                "autodetect option. Once the keyboards are choosen, you can start the emulation process.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I choose keyboards for every slot, but when I press the start button, but the app still displays an error. What to do?");
            sb.AppendLine("A: Probably the xbox accessoary driver is not installed on your system. Go to 'Drivers' menu to get it (link to Microsoft's website).");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Ok, the start button is grayed out.. What to do now?");
            sb.AppendLine("A: Start the game that you want to play.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Ok, the emulation is started, the game is running, but what are my keys?");
            sb.AppendLine("A: It depends on the preset that you are using. You can scroll down the slot " +
                "to view all mapped keyboard keys.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Ok, everything works fine, but I want to change my keys. Is it possible?");
            sb.AppendLine("A: Yes, it is. Examine the current mapped keys (scroll down the slot), and change them for your needs. " +
                "If you want you could save the changes to new preset. (See next question for details).");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I changed the keys manually, but I do not want to set them everytime I want to play. Can I save them?");
            sb.AppendLine("A: Yes, you can. In the presets list (see Help->User Interface pt.6) type some name, then click on preset " +
                "save button (see Help->User Interface pt.7). App will ask you to confirm that action. Click Yes. " +
                "The next time you load up the app, just select your new preset by its name.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I have deleted a preset, can this be undone?");
            sb.AppendLine("A: No, you have to create the preset again.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I have overwritted a preset, can this be undone?");
            sb.AppendLine("A: No, you have to rollback the changes manually.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: My keyboard does not work, when running this app. Is it normal?");
            sb.AppendLine("A: Yes, it is. The app blocks the keyboard input - this is by design. " +
                "To unblock it, uncheck the \"Block choosen keyboards\" checkbox.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Not all keyboards are blocked. Is this normal?");
            sb.AppendLine("A: Yes, it is. Only the choosen (assigned) keyboards are blocked.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I have connected n keyboards, but the app shows me n + 1 (or more) keyboards, so I have " +
                "one (or more) useless keyboard listed. Is this normal?");
            sb.AppendLine("A: Yes, it is. This behavior occurs because USB keyboards require the standard 101/102- key " +
                "or Microsoft Natural Keyboard driver to work properly. This behavior is by Windows design. Note that some " +
                "hardware devices (such as mouses) are also detected as keyboards.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Do I need to restart the app, when I connect/disconnect usb keyboard?");
            sb.AppendLine("A: Generally, No. The app always keeps updated keyboards list and has " +
                "built-in usb listener, which will notify you when an assigned keyboard is unplugged. " + 
                "Restart the app, only if you encounter some keyboard problem.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: My presets are not saved to presets.xml. Why?");
            sb.AppendLine("A: Generally, on each exit, the app saves its presets to presets.xml file. " +
                "Probably you do not own write permissions to save the file. " +
                "Try to move the application to place, where you have write permissions.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I created presets for my favourite games. Can I share them with friends?");
            sb.AppendLine("A: Yes, just send them the presets.xml file (or its contents) and all your presets will become usable for them.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I have deleted presets.xml file. Is this fatal?");
            sb.AppendLine("A: No, the app will continue to work, but all your presets are now lost.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I changed the default preset mappings, but the save button is not active. Why?");
            sb.AppendLine("A: By design you can temporarily modify the default preset, but you can not save it. " +
                "The same applies to the empty preset. You can save the modified preset, using another name for it.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: I want to add custom function to a preset, but the button for this goal is grayed out. Why?");
            sb.AppendLine("A: Probably, you are trying to add custom function to a protected preset. Just try another preset.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Is there a way to block/unblock keyboards, remotely while running game at full screen mode?");
            sb.AppendLine("A: Yes, you can activate emergency mode by hitting \"LeftControl\" key 5 times in a row. " +
                "This will toggle the \"Block choosen keyboards\" checkbox. " +
                "The both operations (checking and unchecking) are accompanied by a specific sound. " +
                "Note that emergency mode could be used, only when the emulation is started.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: When I use LeftControl key for my preset, its label becomes red. Why is that?");
            sb.AppendLine("A: There are two reasons for that: " +
                "a) LeftControl key is very special key. Some function keys send its scancode along with the main data. " +
                "This behavior may lead to unexpected results for you. " +
                "b) LeftControl key repeating is used to trigger the mentioned above emergency mode.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Is this app works only for games?");
            sb.AppendLine("A: No, you can use it for any application that works with xbox 360 controllers.");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Q: Can I change the virtual controller sub type?");
            sb.AppendLine("A: Yes, you can change the virtual controller sub type from gamepad to wheel, arcade stick, " +
                "flight stick, dance pad, guitar, guitar alternate, drumkit, bass guitar or arcade pad " +
                "using the ToCaEdit's xbox emulator (x360ce) application. Website: http://www.x360ce.com/");

            this.textBox.Text = sb.ToString();
        }
    }
}