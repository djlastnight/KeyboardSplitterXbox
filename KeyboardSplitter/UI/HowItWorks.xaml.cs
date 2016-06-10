namespace KeyboardSplitter.UI
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Helper window, which display information about
    /// how the current application works.
    /// </summary>
    public partial class HowItWorks : Window
    {
        public HowItWorks()
        {
            this.InitializeComponent();
            this.Title = "How " + ApplicationInfo.AppNameVersion + " works";

            // animations
            this.BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(1))));
            var scrollAnimation = new DoubleAnimation(1200, 0, new Duration(TimeSpan.FromSeconds(1)));
            var sb = new Storyboard();
            sb.Children.Add(scrollAnimation);
            Storyboard.SetTarget(scrollAnimation, this.scrollViewer);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));
            sb.Begin();
        }
    }
}
