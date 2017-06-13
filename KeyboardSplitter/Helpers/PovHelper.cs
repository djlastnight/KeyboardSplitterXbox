namespace KeyboardSplitter.Helpers
{
    using System;
    using System.Linq;
    using Interceptor;
    using KeyboardSplitter.Controls;
    using KeyboardSplitter.Enums;
    using KeyboardSplitter.Managers;
    using XboxInterfaceWrap;

    public static class PovHelper
    {
        public static XboxDpadDirection CalculatePovDirection(
            JoyControl parent, XboxDpadDirection direction, KeyState keyState)
        {
            PovKeyInfo povInfo = GetPovKeyInfo(parent);

            if (povInfo.PressedKeysCount > 2)
            {
                if (keyState.HasFlag(KeyState.Up))
                {
                    return XboxDpadDirection.None;
                }

                return direction;
            }

            if ((povInfo.IsLeftKeyPressed && povInfo.IsRightKeyPressed) ||
                (povInfo.IsUpKeyPressed && povInfo.IsDownKeyPressed))
            {
                // opposite directions are pressed
                if (keyState.HasFlag(KeyState.Up))
                {
                    return XboxDpadDirection.None;
                }

                return direction;
            }

            XboxDpadDirection combinedDirection = XboxDpadDirection.None;

            if (povInfo.IsUpKeyPressed)
            {
                combinedDirection = combinedDirection | XboxDpadDirection.Up;
            }

            if (povInfo.IsDownKeyPressed)
            {
                combinedDirection = combinedDirection | XboxDpadDirection.Down;
            }

            if (povInfo.IsLeftKeyPressed)
            {
                combinedDirection = combinedDirection | XboxDpadDirection.Left;
            }

            if (povInfo.IsRightKeyPressed)
            {
                combinedDirection = combinedDirection | XboxDpadDirection.Right;
            }

            return combinedDirection;
        }

        private static PovKeyInfo GetPovKeyInfo(JoyControl parent)
        {
            var povUpKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Dpad
                && x.DpadDirection == XboxDpadDirection.Up).Select(x => x.KeyGesture);

            var povRightKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Dpad
                && x.DpadDirection == XboxDpadDirection.Right).Select(x => x.KeyGesture);

            var povDownKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Dpad
                && x.DpadDirection == XboxDpadDirection.Down).Select(x => x.KeyGesture);

            var povLeftKeys = parent.Children.Where(x => x.ControlType == KeyControlType.Dpad
                && x.DpadDirection == XboxDpadDirection.Left).Select(x => x.KeyGesture);

            PovKeyInfo povKeyInfo = new PovKeyInfo();

            povKeyInfo.IsUpKeyPressed = povUpKeys.Any(x => InputManager.IsKeyDown(parent.CurrentKeyboard, x));
            povKeyInfo.IsDownKeyPressed = povDownKeys.Any(x => InputManager.IsKeyDown(parent.CurrentKeyboard, x));
            povKeyInfo.IsLeftKeyPressed = povLeftKeys.Any(x => InputManager.IsKeyDown(parent.CurrentKeyboard, x));
            povKeyInfo.IsRightKeyPressed = povRightKeys.Any(x => InputManager.IsKeyDown(parent.CurrentKeyboard, x));

            return povKeyInfo;
        }

        private class PovKeyInfo
        {
            public bool IsUpKeyPressed { get; set; }

            public bool IsRightKeyPressed { get; set; }

            public bool IsDownKeyPressed { get; set; }

            public bool IsLeftKeyPressed { get; set; }

            public int PressedKeysCount
            {
                get
                {
                    int result =
                        Convert.ToInt32(this.IsUpKeyPressed) +
                        Convert.ToInt32(this.IsRightKeyPressed) +
                        Convert.ToInt32(this.IsDownKeyPressed) +
                        Convert.ToInt32(this.IsLeftKeyPressed);

                    return result;
                }
            }
        }
    }
}