namespace KeyboardSplitter.Helpers
{
    using System;
    using SplitterCore;
    using VirtualXbox;
    using VirtualXbox.Enums;

    public static class CustomFunctionHelper
    {
        public static FunctionType GetFunctionType(XboxCustomFunction function)
        {
            switch (function)
            {
                case XboxCustomFunction.Button_Guide:
                case XboxCustomFunction.Button_A:
                case XboxCustomFunction.Button_B:
                case XboxCustomFunction.Button_X:
                case XboxCustomFunction.Button_Y:
                case XboxCustomFunction.Left_Bumper:
                case XboxCustomFunction.Right_Bumper:
                case XboxCustomFunction.Button_Back:
                case XboxCustomFunction.Button_Start:
                case XboxCustomFunction.Left_Thumb:
                case XboxCustomFunction.Right_Thumb:
                    return FunctionType.Button;

                case XboxCustomFunction.Left_Trigger:
                case XboxCustomFunction.Right_Trigger:
                    return FunctionType.Trigger;

                case XboxCustomFunction.Dpad_Up:
                case XboxCustomFunction.Dpad_Down:
                case XboxCustomFunction.Dpad_Left:
                case XboxCustomFunction.Dpad_Right:
                    return FunctionType.Dpad;

                case XboxCustomFunction.Axis_X_Min:
                case XboxCustomFunction.Axis_X_Max:
                case XboxCustomFunction.Axis_Y_Min:
                case XboxCustomFunction.Axis_Y_Max:
                case XboxCustomFunction.Axis_Rx_Min:
                case XboxCustomFunction.Axis_Rx_Max:
                case XboxCustomFunction.Axis_Ry_Min:
                case XboxCustomFunction.Axis_Ry_Max:
                    return FunctionType.Axis;
                default:
                    throw new NotImplementedException(
                        "Not implemented custom function: " + function);
            }
        }

        public static XboxButton GetXboxButton(XboxCustomFunction function)
        {
            if (GetFunctionType(function) != FunctionType.Button)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not button type");
            }

            return (XboxButton)(int)function;
        }

        public static XboxDpadDirection GetDpadDirection(XboxCustomFunction function)
        {
            if (GetFunctionType(function) != FunctionType.Dpad)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not dpad type");
            }

            return (XboxDpadDirection)(int)function;
        }

        public static XboxTrigger GetXboxTrigger(XboxCustomFunction function)
        {
            if (GetFunctionType(function) != FunctionType.Trigger)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not trigger type");
            }

            return (XboxTrigger)(int)function;
        }

        public static XboxAxis GetXboxAxis(XboxCustomFunction function, out XboxAxisPosition position)
        {
            if (GetFunctionType(function) != FunctionType.Axis)
            {
                throw new InvalidOperationException(
                    "Passed function's control type is not axis type");
            }

            XboxAxis axis;
            switch (function)
            {
                case XboxCustomFunction.Axis_X_Min:
                    {
                        axis = XboxAxis.X;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Axis_X_Max:
                    {
                        axis = XboxAxis.X;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Axis_Y_Min:
                    {
                        axis = XboxAxis.Y;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Axis_Y_Max:
                    {
                        axis = XboxAxis.Y;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Axis_Rx_Min:
                    {
                        axis = XboxAxis.Rx;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Axis_Rx_Max:
                    {
                        axis = XboxAxis.Rx;
                        position = XboxAxisPosition.Max;
                    }

                    break;
                case XboxCustomFunction.Axis_Ry_Min:
                    {
                        axis = XboxAxis.Ry;
                        position = XboxAxisPosition.Min;
                    }

                    break;
                case XboxCustomFunction.Axis_Ry_Max:
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

        public static XboxCustomFunction GetFunction(XboxButton button)
        {
            switch (button)
            {
                case XboxButton.Start:
                    return XboxCustomFunction.Button_Start;
                case XboxButton.Back:
                    return XboxCustomFunction.Button_Back;
                case XboxButton.LeftThumb:
                    return XboxCustomFunction.Left_Thumb;
                case XboxButton.RightThumb:
                    return XboxCustomFunction.Right_Thumb;
                case XboxButton.LeftBumper:
                    return XboxCustomFunction.Left_Bumper;
                case XboxButton.RightBumper:
                    return XboxCustomFunction.Right_Bumper;
                case XboxButton.Guide:
                    return XboxCustomFunction.Button_Guide;
                case XboxButton.A:
                    return XboxCustomFunction.Button_A;
                case XboxButton.B:
                    return XboxCustomFunction.Button_B;
                case XboxButton.X:
                    return XboxCustomFunction.Button_X;
                case XboxButton.Y:
                    return XboxCustomFunction.Button_Y;
                default:
                    throw new NotImplementedException("Not implemented xbox button:" + button);
            }
        }

        public static bool SetFunctionState(XboxCustomFunction function, uint userIndex, bool newState)
        {
            var functionType = GetFunctionType(function);

            switch (functionType)
            {
                case FunctionType.Button:
                    var button = GetXboxButton(function);
                    return VirtualXboxController.SetButton(userIndex, button, newState);
                case FunctionType.Axis:
                    XboxAxisPosition position;
                    var axis = GetXboxAxis(function, out position);

                    if (newState == false)
                    {
                        return VirtualXboxController.SetAxis(userIndex, axis, (short)XboxAxisPosition.Center);
                    }

                    return VirtualXboxController.SetAxis(userIndex, axis, (short)position);
                case FunctionType.Dpad:
                    if (newState == false)
                    {
                        return VirtualXboxController.SetDPad(userIndex, XboxDpadDirection.Off);
                    }

                    var direction = GetDpadDirection(function);
                    return VirtualXboxController.SetDPad(userIndex, direction);
                case FunctionType.Trigger:
                    var trigger = GetXboxTrigger(function);
                    return VirtualXboxController.SetTrigger(userIndex, trigger, newState ? (byte)255 : (byte)0);
                default:
                    throw new NotImplementedException("Not implemented xbox function type: " + functionType);
            }
        }
    }
}
