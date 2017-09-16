namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class GameStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Brush) && value.GetType() == typeof(Enums.GameStatus))
            {
                var status = (Enums.GameStatus)value;
                switch (status)
                {
                    case KeyboardSplitter.Enums.GameStatus.NotSet:
                        return Brushes.LightGray;
                    case KeyboardSplitter.Enums.GameStatus.InvalidSlotsCount:
                    case KeyboardSplitter.Enums.GameStatus.InvalidSlotNumber:
                    case KeyboardSplitter.Enums.GameStatus.InvalidGamepadUserIndex:
                    case KeyboardSplitter.Enums.GameStatus.KeyboardMissing:
                    case KeyboardSplitter.Enums.GameStatus.MouseMissing:
                    case KeyboardSplitter.Enums.GameStatus.PresetMissing:
                        return Brushes.Yellow;
                    case KeyboardSplitter.Enums.GameStatus.ExeNotFound:
                        return Brushes.Red;
                    case KeyboardSplitter.Enums.GameStatus.OK:
                        return Brushes.Lime;
                    default:
                        throw new NotImplementedException("Not implemented game status: " + status);
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
