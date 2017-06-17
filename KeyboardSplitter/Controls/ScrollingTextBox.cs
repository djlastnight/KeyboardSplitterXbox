namespace KeyboardSplitter.Controls
{
    using System;
    using System.Windows.Controls;

    public class ScrollingTextBox : TextBox
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            this.CaretIndex = Text.Length;
            this.ScrollToEnd();
        }
    }
}
