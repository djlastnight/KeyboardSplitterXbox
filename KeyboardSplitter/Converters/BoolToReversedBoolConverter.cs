namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;

    public class BoolToReversedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool && targetType == typeof(bool))
            {
                return !((bool)value);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
