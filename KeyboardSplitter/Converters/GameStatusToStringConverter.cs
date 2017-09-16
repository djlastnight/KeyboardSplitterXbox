namespace KeyboardSplitter.Converters
{
    using System;
    using System.Windows.Data;
    using KeyboardSplitter.Enums;

    public class GameStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((targetType == typeof(string) || targetType == typeof(object)) && value.GetType() == typeof(GameStatus))
            {
                var status = (GameStatus)value;
                switch (status)
                {
                    case GameStatus.NotSet:
                        return "Game status not set or not updated.";
                    case GameStatus.InvalidSlotsCount:
                        return "Invalid slots count detected.";
                    case GameStatus.InvalidSlotNumber:
                        return "Invalid slot number detected.";
                    case GameStatus.InvalidGamepadUserIndex:
                        return "Invalid gamepad user index detected.";
                    case GameStatus.KeyboardMissing:
                        return "Missing keyboard detected.";
                    case GameStatus.MouseMissing:
                        return "Missing mouse detected.";
                    case GameStatus.PresetMissing:
                        return "Missing preset detected.";
                    case GameStatus.ExeNotFound:
                        return "The game's exe file is missing.";
                    case GameStatus.OK:
                        return "Game is playable.";
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
