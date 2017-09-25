namespace KeyboardSplitter.Converters
{
    using System;
    using System.Diagnostics;
    using System.Windows.Data;

    /// <summary>
    /// This provides a debugging output for a binding converter
    /// </summary>
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.WriteLine(
                string.Format(
                culture,
                "Convert: Value={0}, TargetType={1}, Parameter={2}, Culture={3}",
                value,
                targetType,
                parameter,
                culture));

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.WriteLine(
                string.Format(
                culture,
                "ConvertBack: Value={0}, TargetType={1}, Parameter={2}, Culture={3}",
                value,
                targetType,
                parameter,
                culture));

            return value;
        }
    }
}