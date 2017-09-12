namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Helpers;

    public class CustomWindow : Window
    {
        public static readonly DependencyProperty IsToolWindowProperty =
            DependencyProperty.Register(
            "IsToolWindow",
            typeof(bool),
            typeof(CustomWindow),
            new PropertyMetadata(false));

        private CustomTitlebar titleBar;

        static CustomWindow()
        {
            var setter = new Setter();
            setter.Property = CustomWindow.BackgroundProperty;
            setter.Value = Brushes.LightGray;
        }

        public CustomWindow()
            : base()
        {
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            this.Icon = new BitmapImage(new Uri(@"pack://application:,,,/KeyboardSplitter;component/Resources/icon.ico", UriKind.RelativeOrAbsolute));
            this.Loaded += this.OnLoaded;
        }

        public bool IsToolWindow
        {
            get { return (bool)this.GetValue(IsToolWindowProperty); }
            set { this.SetValue(IsToolWindowProperty, value); }
        }

        protected override void OnActivated(System.EventArgs e)
        {
            GlobalSettings.IsMainWindowActivated = true;
            base.OnActivated(e);
            if (!AeroHelper.IsAeroEnabled && this.titleBar != null)
            {
                this.titleBar.Background = System.Windows.SystemColors.ActiveCaptionBrush;
                this.titleBar.Foreground = System.Windows.SystemColors.ActiveCaptionTextBrush;
            }
        }

        protected override void OnDeactivated(System.EventArgs e)
        {
            GlobalSettings.IsMainWindowActivated = false;
            base.OnDeactivated(e);
            if (!AeroHelper.IsAeroEnabled && this.titleBar != null)
            {
                this.titleBar.Background = System.Windows.SystemColors.InactiveCaptionBrush;
                this.titleBar.Foreground = System.Windows.SystemColors.InactiveCaptionTextBrush;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (AeroHelper.IsAeroEnabled)
            {
                AeroHelper.ExtendGlassFrame(this, new Thickness(0, CustomTitlebar.TitlebarHeightInPixels + 4, 0, 0));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Inserting a custom titlebar before the window's content
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(CustomTitlebar.TitlebarHeightInPixels + 4, GridUnitType.Pixel)
            });

            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star),
            });

            this.titleBar = new CustomTitlebar();
            this.titleBar.Loaded += (ss, ee) =>
            {
                this.titleBar.Foreground = System.Windows.SystemColors.ActiveCaptionTextBrush;

                AeroHelper.AeroColorChanged += OnAeroColorChanged;
                if (AeroHelper.IsAeroEnabled)
                {
                    var aeroColor = AeroHelper.TryGetAeroColor();
                    if (aeroColor != null)
                    {
                        this.titleBar.Background = aeroColor;
                    }
                }
                else
                {
                    this.titleBar.Background = System.Windows.SystemColors.ActiveCaptionBrush;
                }
            };

            var content = this.Content as UIElement;
            this.Content = null;
            Grid.SetRow(this.titleBar, 0);
            Grid.SetRow(content, 1);
            grid.Children.Add(this.titleBar);
            grid.Children.Add(content);
            this.Content = grid;
            if (this.IsToolWindow)
            {
                this.MinWidth = this.ActualWidth;
                this.MaxWidth = this.ActualWidth;
                this.MinHeight = this.ActualHeight;
                this.MaxHeight = this.ActualHeight;
            }
        }

        private void OnAeroColorChanged(object sender, EventArgs e)
        {
            this.titleBar.Dispatcher.BeginInvoke((Action)delegate
            {
                this.titleBar.Background = AeroHelper.TryGetAeroColor();
                this.titleBar.Foreground = System.Windows.SystemColors.ActiveCaptionTextBrush;
            });
        }
    }
}