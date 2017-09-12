namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using VirtualXbox.Enums;

    public class XboxButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is uint && targetType == typeof(string))
            {
                var button = (XboxButton)(uint)value;
                if (button == XboxButton.LeftBumper)
                {
                    return "Left Bumper (LB)";
                }
                else if (button == XboxButton.RightBumper)
                {
                    return "Right Bumper (RB)";
                }
                else if (button == XboxButton.LeftThumb)
                {
                    return "Left Thumb";
                }
                else if (button == XboxButton.RightThumb)
                {
                    return "Right Thumb";
                }

                return string.Format("Button '{0}'", button.ToString());
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
