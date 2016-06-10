namespace KeyboardSplitter.Helpers
{
    using System;
    using System.Linq;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Managers;
    using XboxInterfaceWrap;

    public class AxisHelper
    {
        public static short CalculateAxisValue(JoyControl parent, XboxAxis axis, XboxAxisPosition position, KeyState keyState)
        {
            if (keyState == KeyState.Down || keyState == KeyState.E0)
            {
                return (short)position;
            }

            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            var minKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Axis &&
                x.Axis == axis && x.Position == XboxAxisPosition.Min).Select(x => x.KeyGesture);

            var maxKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Axis &&
                x.Axis == axis && x.Position == XboxAxisPosition.Max).Select(x => x.KeyGesture);

            bool isMinDown = minKeys.Any(x => KeyboardManager.IsKeyDown(parent.CurrentKeyboard, x));
            bool isMaxDown = maxKeys.Any(x => KeyboardManager.IsKeyDown(parent.CurrentKeyboard, x));

            if (!isMinDown && !isMaxDown)
            {
                return (short)XboxAxisPosition.Center;
            }
            else if (isMinDown && isMaxDown)
            {
                // throwing exception, because
                // both keys are down, but the state is up
                throw new InvalidOperationException(
                    string.Format("Fatal Internal Error: Axis {0} min and max keys are both pressed, " + "but the passed keystate was {1}!", axis, keyState));
            }
            else if (isMinDown)
            {
                return (short)XboxAxisPosition.Min;
            }
            else
            {
                return (short)XboxAxisPosition.Max;
            }
        }
    }
}