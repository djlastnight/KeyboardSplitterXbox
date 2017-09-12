namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using VirtualXbox.Enums;

    public class XboxCustomFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is uint)
            {
                var function = (XboxCustomFunction)(uint)value;
                return function;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(uint))
            {
                XboxCustomFunction function;
                if (Enum.TryParse(value.ToString(), out function))
                {
                    return (uint)function;
                }
            }

            return null;
        }
    }
}