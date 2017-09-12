namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool && targetType == typeof(Visibility))
            {
                bool enable = (bool)value;
                
                if (parameter != null)
                {
                    bool paramResult;
                    if (bool.TryParse(parameter.ToString(), out paramResult))
                    {
                        if (paramResult == true)
                        {
                            enable = !enable;
                        }
                    }
                }

                if (enable)
                {
                    return Visibility.Visible;
                }
                else
                {
                    if (parameter != null && parameter.ToString() == "collapsed")
                    {
                        return Visibility.Collapsed;
                    }

                    return Visibility.Hidden;
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
