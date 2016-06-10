namespace KeyboardSplitter.UI
{
    using System;
    using System.Timers;
    using System.Windows;
    using System.Windows.Media.Animation;

    internal partial class AutoHideTooltip : Window
    {
        private Timer timer = new Timer();

        public AutoHideTooltip(string text, Point position, double intervalInMs = 7000)
            : this()
        {
            this.textBlock.Text = text;
            this.Left = position.X;
            this.Top = position.Y;
            this.timer.Interval = intervalInMs;
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        }

        private AutoHideTooltip()
        {
            this.InitializeComponent();
            this.timer.Interval = 7000;
            this.timer.AutoReset = true;
            this.timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.Close();
            });
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            var anim = new DoubleAnimation(0.8, new Duration(TimeSpan.FromMilliseconds(500)));
            anim.AutoReverse = true;
            anim.RepeatBehavior = new RepeatBehavior(this.timer.Interval / anim.Duration.TimeSpan.TotalMilliseconds);
            this.BeginAnimation(Window.OpacityProperty, anim);
            this.timer.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}