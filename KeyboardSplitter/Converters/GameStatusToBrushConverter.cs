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
                        return this.GetLedBrush(Colors.LightGray);
                    case KeyboardSplitter.Enums.GameStatus.InvalidSlotsCount:
                    case KeyboardSplitter.Enums.GameStatus.InvalidSlotNumber:
                    case KeyboardSplitter.Enums.GameStatus.InvalidGamepadUserIndex:
                    case KeyboardSplitter.Enums.GameStatus.KeyboardMissing:
                    case KeyboardSplitter.Enums.GameStatus.MouseMissing:
                    case KeyboardSplitter.Enums.GameStatus.PresetMissing:
                        return this.GetLedBrush(Colors.Yellow);
                    case KeyboardSplitter.Enums.GameStatus.ExeNotFound:
                        return this.GetLedBrush(Colors.Red);
                    case KeyboardSplitter.Enums.GameStatus.OK:
                        return this.GetLedBrush(Colors.Lime);
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

        private Brush GetLedBrush(Color color)
        {
            var radial = new RadialGradientBrush();
            radial.GradientStops.Add(new GradientStop(color, 0));
            radial.GradientStops.Add(new GradientStop(color, 0.5));
            radial.GradientStops.Add(new GradientStop(Colors.Black, 0.5));
            radial.GradientStops.Add(new GradientStop(Colors.Black, 0.6));
            radial.GradientStops.Add(new GradientStop(color, 0.6));
            radial.GradientStops.Add(new GradientStop(Colors.Black, 0.8));
            radial.GradientStops.Add(new GradientStop(Colors.Black, 0.90));
            radial.GradientStops.Add(new GradientStop(color, 0.90));
            radial.GradientStops.Add(new GradientStop(color, 1));
            return radial;
        }
    }
}
