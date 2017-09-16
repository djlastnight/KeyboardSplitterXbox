namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using KeyboardSplitter.Enums;

    public class GameStatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(GameStatus) && targetType == typeof(bool))
            {
                var status = (GameStatus)value;
                if (status == GameStatus.OK)
                {
                    return true;
                }
                else
                {
                    return false;
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
