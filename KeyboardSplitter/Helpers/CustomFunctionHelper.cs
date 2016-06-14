namespace KeyboardSplitter.Helpers
{
    using System;
    using KeyboardSplitter.Enums;
    using XboxInterfaceWrap;

    public static class CustomFunctionHelper
    {
        public static KeyControlType GetControlType(XboxCustomFunction function)
        {
            switch (function)
            {
                case XboxCustomFunction.Guide:
                case XboxCustomFunction.A:
                case XboxCustomFunction.B:
                case XboxCustomFunction.X:
                case XboxCustomFunction.Y:
                case XboxCustomFunction.LeftBumper:
                case XboxCustomFunction.RightBumper:
                case XboxCustomFunction.Back:
                case XboxCustomFunction.Start:
                case XboxCustomFunction.LeftThumb:
                case XboxCustomFunction.RightThumb:
                    return KeyControlType.Button;

                case XboxCustomFunction.LeftTrigger:
                case XboxCustomFunction.RightTrigger:
                    return KeyControlType.Trigger;

                case XboxCustomFunction.Dpad_Up:
                case XboxCustomFunction.Dpad_Down:
                case XboxCustomFunction.Dpad_Left:
                case XboxCustomFunction.Dpad_Right:
                    return KeyControlType.Dpad;

                case XboxCustomFunction.X_Min:
                case XboxCustomFunction.X_Max:
                case XboxCustomFunction.Y_Min:
                case XboxCustomFunction.Y_Max:
                case XboxCustomFunction.Rx_Min:
                case XboxCustomFunction.Rx_Max:
                case XboxCustomFunction.Ry_Min:
                case XboxCustomFunction.Ry_Max:
                    return KeyControlType.Axis;
                default:
                    throw new NotImplementedException(
                        "Not implemented custom function: " + function);
            }
        }

        public static XboxButton GetXboxButton(XboxCustomFunction function)
        {
            if (GetControlType(function) != KeyControlType.Button)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not button type");
            }

            return (XboxButton)(int)function;
        }

        public static XboxDpadDirection GetDpadDirection(XboxCustomFunction function)
        {
            if (GetControlType(function) != KeyControlType.Dpad)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not dpad type");
            }

            return (XboxDpadDirection)(int)function;
        }

        public static XboxTrigger GetXboxTrigger(XboxCustomFunction function)
        {
            if (GetControlType(function) != KeyControlType.Trigger)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not trigger type");
            }

            return (XboxTrigger)(int)function;
        }

        public static XboxAxis GetXboxAxis(XboxCustomFunction function, out XboxAxisPosition position)
        {
            if (GetControlType(function) != KeyControlType.Axis)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not axis type");
            }

            XboxAxis axis;
            switch (function)
            {
                case XboxCustomFunction.X_Min:
                    {
                        axis = XboxAxis.X;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.X_Max:
                    {
                        axis = XboxAxis.X;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Y_Min:
                    {
                        axis = XboxAxis.Y;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Y_Max:
                    {
                        axis = XboxAxis.Y;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Rx_Min:
                    {
                        axis = XboxAxis.Rx;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Rx_Max:
                    {
                        axis = XboxAxis.Rx;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Ry_Min:
                    {
                        axis = XboxAxis.Ry;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Ry_Max:
                    {
                        axis = XboxAxis.Ry;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

            return axis;
        }
    }
}
