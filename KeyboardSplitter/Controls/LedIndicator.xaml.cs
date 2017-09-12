namespace KeyboardSplitter.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for LedIndicator.xaml
    /// </summary>
    public partial class LedIndicator : UserControl
    {
        public static readonly DependencyProperty LedNumberProperty =
            DependencyProperty.Register(
            "LedNumber",
            typeof(int),
            typeof(LedIndicator),
            new PropertyMetadata(0));

        public LedIndicator()
        {
            this.InitializeComponent();
        }

        public int LedNumber
        {
            get { return (int)this.GetValue(LedNumberProperty); }
            set { this.SetValue(LedNumberProperty, value); }
        }
    }
}