namespace KeyboardSplitter.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using KeyboardSplitter.UI;

    /// <summary>
    /// Interaction logic for CustomTitlebar.xaml
    /// </summary>
    internal partial class CustomTitlebar : UserControl
    {
        public const int TitlebarHeightInPixels = 24;

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
            "Icon",
            typeof(ImageSource),
            typeof(CustomTitlebar),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(CustomTitlebar),
            new PropertyMetadata("Titlebar"));

        public static readonly DependencyProperty ShowMinimizeButtonProperty =
            DependencyProperty.Register(
            "ShowMinimizeButton",
            typeof(bool),
            typeof(CustomTitlebar),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowRestoreButtonProperty =
            DependencyProperty.Register(
            "ShowRestoreButton",
            typeof(bool),
            typeof(CustomTitlebar),
            new PropertyMetadata(true));

        public static readonly DependencyProperty EnableRestoreButtonProperty =
            DependencyProperty.Register(
            "EnableRestoreButton",
            typeof(bool),
            typeof(CustomTitlebar),
            new PropertyMetadata(true));

        private Window parent;

        public CustomTitlebar()
        {
            this.InitializeComponent();
        }

        public ImageSource Icon
        {
            get { return (ImageSource)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public bool ShowMinimizeButton
        {
            get { return (bool)this.GetValue(ShowMinimizeButtonProperty); }
            set { this.SetValue(ShowMinimizeButtonProperty, value); }
        }

        public bool ShowRestoreButton
        {
            get { return (bool)this.GetValue(ShowRestoreButtonProperty); }
            set { this.SetValue(ShowRestoreButtonProperty, value); }
        }

        public bool EnableRestoreButton
        {
            get { return (bool)this.GetValue(EnableRestoreButtonProperty); }
            set { this.SetValue(EnableRestoreButtonProperty, value); }
        }

        private static Size MeasureText(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    textBlock.FontFamily,
                    textBlock.FontStyle,
                    textBlock.FontWeight,
                    textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void ToggleRestore()
        {
            if (this.parent != null)
            {
                if (this.parent.WindowState == WindowState.Maximized)
                {
                    this.parent.WindowState = WindowState.Normal;
                }
                else if (this.parent.WindowState == WindowState.Normal)
                {
                    this.parent.WindowState = WindowState.Maximized;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.parent = Helpers.ParentFinder.FindParent<Window>(this);
            if (this.parent == null)
            {
                return;
            }

            this.parent.StateChanged += this.OnParentWindowStateChanged;
            this.Title = this.parent.Title;
            var textSize = MeasureText(this.titlebarTextblock);
            this.titlebarTextBackground.Width = textSize.Width + 5;
            this.titlebarTextBackground.Height = textSize.Height;
            this.Icon = this.parent.Icon;

            var customWindow = this.parent as CustomWindow;
            if (customWindow != null && customWindow.IsToolWindow)
            {
                this.ShowMinimizeButton = false;
                this.ShowRestoreButton = false;
            }

            switch (this.parent.ResizeMode)
            {
                case ResizeMode.CanMinimize:
                    this.ShowMinimizeButton = this.ShowMinimizeButton && true;
                    this.ShowRestoreButton = this.ShowRestoreButton && true;
                    this.EnableRestoreButton = this.EnableRestoreButton && false;
                    break;
                case ResizeMode.CanResize:
                    this.ShowMinimizeButton = this.ShowMinimizeButton && true;
                    this.ShowRestoreButton = this.ShowRestoreButton && true;
                    this.EnableRestoreButton = this.EnableRestoreButton && true;
                    break;
                case ResizeMode.CanResizeWithGrip:
                    this.ShowMinimizeButton = this.ShowRestoreButton && true;
                    this.ShowRestoreButton = this.ShowRestoreButton && true;
                    this.EnableRestoreButton = this.EnableRestoreButton && true;
                    break;
                case ResizeMode.NoResize:
                    this.ShowMinimizeButton = this.ShowMinimizeButton && false;
                    this.ShowRestoreButton = this.ShowRestoreButton && false;
                    this.EnableRestoreButton = this.EnableRestoreButton && false;
                    break;
                default:
                    break;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.parent != null)
                {
                    if (this.parent.WindowState == WindowState.Maximized)
                    {
                        var bigWidth = this.parent.ActualWidth;
                        var bigHeight = this.parent.ActualHeight;
                        var x = e.MouseDevice.GetPosition(this).X;
                        this.parent.WindowState = WindowState.Normal;
                        var zoomFactor = bigWidth / this.parent.ActualWidth;

                        this.parent.Top = 0;
                        this.parent.Left = e.MouseDevice.GetPosition(this.parent).X - (x / zoomFactor);
                    }

                    this.parent.DragMove();
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.EnableRestoreButton)
                {
                    this.ToggleRestore();
                }
            }
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.parent != null)
            {
                this.parent.WindowState = WindowState.Minimized;
            }
        }

        private void OnRestoreButtonClicked(object sender, RoutedEventArgs e)
        {
            this.ToggleRestore();
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.parent != null)
            {
                this.parent.Close();
            }
        }

        private void OnParentWindowStateChanged(object sender, EventArgs e)
        {
            if (this.parent != null)
            {
                if (this.parent.WindowState == WindowState.Normal)
                {
                    this.restoreButton.Content = "1";
                    this.restoreButton.ToolTip = "Maximize";
                }
                else if (this.parent.WindowState == WindowState.Maximized)
                {
                    this.restoreButton.Content = "2";
                    this.restoreButton.ToolTip = "Restore down";
                }
            }
        }
    }
}