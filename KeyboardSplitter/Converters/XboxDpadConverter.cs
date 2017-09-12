namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using VirtualXbox.Enums;

    public class XboxDpadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && targetType == typeof(string))
            {
                var direction = (XboxDpadDirection)(int)value;
                return "D-pad " + direction.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
