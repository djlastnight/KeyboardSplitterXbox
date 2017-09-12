namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class LedNumberToBrushConverter : IValueConverter
    {
        private static Brush ledOffBrush;

        private static Brush ledOnBrush;

        static LedNumberToBrushConverter()
        {
            var offStops = new GradientStopCollection();
            offStops.Add(new GradientStop(Colors.Transparent, 1.00));
            offStops.Add(new GradientStop(Colors.Transparent, 0.80));
            offStops.Add(new GradientStop(Colors.Black, 0.80));
            offStops.Add(new GradientStop(Colors.Black, 0.70));
            offStops.Add(new GradientStop(Colors.Gray, 0.70));
            offStops.Add(new GradientStop(Colors.White, 0.00));

            LedNumberToBrushConverter.ledOffBrush = new RadialGradientBrush(offStops);

            var onStops = new GradientStopCollection();
            onStops.Add(new GradientStop(Colors.Lime, 1.00));
            onStops.Add(new GradientStop(Colors.Black, 0.85));
            onStops.Add(new GradientStop(Colors.Black, 0.80));
            onStops.Add(new GradientStop(Colors.Lime, 0.70));
            onStops.Add(new GradientStop(Colors.White, 0.00));

            LedNumberToBrushConverter.ledOnBrush = new RadialGradientBrush(onStops);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }

            int ledNumber, ellipseNumber;
            if (int.TryParse(value.ToString(), out ledNumber) &&
                int.TryParse(parameter.ToString(), out ellipseNumber) &&
                targetType == typeof(Brush))
            {
                if (ellipseNumber == ledNumber)
                {
                    return LedNumberToBrushConverter.ledOnBrush;
                }
                else
                {
                    return LedNumberToBrushConverter.ledOffBrush;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
