namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using VirtualXbox.Enums;

    public class XboxAxisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is short && parameter != null && parameter.ToString() == "value")
            {
                var pos = (XboxAxisPosition)(short)value;

                return pos.ToString();
            }

            if (value is uint)
            {
                var axis = (XboxAxis)(uint)value;

                if (parameter != null && parameter.ToString() == "full")
                {
                    switch (axis)
                    {
                        case XboxAxis.X:
                            return "Left Stick Axis X";
                        case XboxAxis.Y:
                            return "Left Stick Axis Y";
                        case XboxAxis.Rx:
                            return "Right Stick Axis X";
                        case XboxAxis.Ry:
                            return "Right Stick Axis Y";
                        default:
                            break;
                    }
                }

                return "Axis " + axis.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
