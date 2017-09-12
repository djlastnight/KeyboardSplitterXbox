namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using VirtualXbox.Enums;

    public class XboxTriggerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is uint && targetType == typeof(string))
            {
                var trigger = (XboxTrigger)(uint)value;
                if (trigger == XboxTrigger.Left)
                {
                    return "Left Trigger (LT)";
                }
                else if (trigger == XboxTrigger.Right)
                {
                    return "Right Trigger (RT)";
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
