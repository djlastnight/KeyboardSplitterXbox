namespace XboxInterfaceWrap
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class VirtualXboxController
    {
        private static ControllerState[] states;

        static VirtualXboxController()
        {
            states = new ControllerState[4]
            {
                new ControllerState(),
                new ControllerState(),
                new ControllerState(),
                new ControllerState()
            };
        }

        public static bool Exists(uint userIndex)
        {
            return NativeMethods.ControllerExistsExt(userIndex);
        }

        public static bool IsOwned(uint userIndex)
        {
            return NativeMethods.IsControllerOwnedExt(userIndex);
        }

        public static int GetEmptyBusSlotsCount()
        {
            int count;
            if (NativeMethods.GetEmptyBusSlotsCount(out count))
            {
                return count;
            }
            else
            {
                return -1;
            }
        }

        public static byte GetLedNumber(uint userIndex)
        {
            byte ledNumber;
            if (NativeMethods.GetLedNumber(userIndex, out ledNumber))
            {
                return ledNumber;
            }

            return 0;
        }

        public static void ResetStates(uint userIndex)
        {
            if (userIndex >= 1 && userIndex <= 4)
            {
                states[userIndex - 1].Reset();
            }
        }

        public static bool PlugIn(uint userIndex)
        {
            VirtualXboxController.ResetStates(userIndex);

            return NativeMethods.PlugInExt(userIndex);
        }

        public static bool UnPlug(uint userIndex, bool force = false)
        {
            VirtualXboxController.ResetStates(userIndex);

            if (force)
            {
                return NativeMethods.UnPlugForceExt(userIndex);
            }
            else
            {
                return NativeMethods.UnPlugExt(userIndex);
            }
        }

        public static bool SetAxis(uint userIndex, XboxAxis axis, short value)
        {
            switch (axis)
            {
                case XboxAxis.X:
                    {
                        states[(int)userIndex - 1].AxisXValue = value;
                        return NativeMethods.SetAxisX(userIndex, value);
                    }

                case XboxAxis.Y:
                    {
                        states[(int)userIndex - 1].AxisYValue = value;
                        return NativeMethods.SetAxisY(userIndex, value);
                    }

                case XboxAxis.Rx:
                    {
                        states[(int)userIndex - 1].AxisRxValue = value;
                        return NativeMethods.SetAxisRx(userIndex, value);
                    }

                case XboxAxis.Ry:
                    {
                        states[(int)userIndex - 1].AxisRyValue = value;
                        return NativeMethods.SetAxisRy(userIndex, value);
                    }

                default:
                    throw new NotImplementedException(
                        "Not implemented xbox axis: " + axis);
            }
        }

        public static bool SetButton(uint userIndex, XboxButton button, bool value)
        {
            var buttonStates = states[(int)userIndex - 1].ButtonsDown;
            if (value == true)
            {
                if (!buttonStates.Contains(button))
                {
                    buttonStates.Add(button);
                }
            }
            else
            {
                if (buttonStates.Contains(button))
                {
                    buttonStates.Remove(button);
                }
            }

            switch (button)
            {
                case XboxButton.Guide:
                    return NativeMethods.SetBtnGuide(userIndex, value);
                case XboxButton.A:
                    return NativeMethods.SetBtnA(userIndex, value);
                case XboxButton.B:
                    return NativeMethods.SetBtnB(userIndex, value);
                case XboxButton.X:
                    return NativeMethods.SetBtnX(userIndex, value);
                case XboxButton.Y:
                    return NativeMethods.SetBtnY(userIndex, value);
                case XboxButton.Start:
                    return NativeMethods.SetBtnStart(userIndex, value);
                case XboxButton.Back:
                    return NativeMethods.SetBtnBack(userIndex, value);
                case XboxButton.LeftThumb:
                    return NativeMethods.SetBtnLT(userIndex, value);
                case XboxButton.RightThumb:
                    return NativeMethods.SetBtnRT(userIndex, value);
                case XboxButton.LeftBumper:
                    return NativeMethods.SetBtnLB(userIndex, value);
                case XboxButton.RightBumper:
                    return NativeMethods.SetBtnRB(userIndex, value);
                default:
                    throw new NotImplementedException(
                        "Not implemented Xbox button: " + button);
            }
        }

        public static bool SetTrigger(uint userIndex, XboxTrigger trigger, byte value)
        {
            switch (trigger)
            {
                case XboxTrigger.LeftTrigger:
                    {
                        if (NativeMethods.SetTriggerL(userIndex, value))
                        {
                            states[(int)userIndex - 1].LeftTriggerValue = value;
                            return true;
                        }

                        return false;
                    }

                case XboxTrigger.RightTrigger:
                    {
                        if (NativeMethods.SetTriggerR(userIndex, value))
                        {
                            states[(int)userIndex - 1].RightTriggerValue = value;
                            return true;
                        }

                        return false;
                    }

                default:
                    throw new NotImplementedException(
                        "Not implemented Xbox trigger: " + trigger);
            }
        }

        public static bool SetDPad(uint userIndex, XboxDpadDirection direction)
        {
            states[(int)userIndex - 1].DpadDirections = direction;

            return NativeMethods.SetDpadExt(userIndex, (int)direction);
        }

        public static bool GetButtonValue(uint userIndex, XboxButton button)
        {
            return states[(int)userIndex - 1].ButtonsDown.Contains(button);
        }

        public static byte GetTriggerValue(uint userIndex, XboxTrigger trigger)
        {
            switch (trigger)
            {
                case XboxTrigger.LeftTrigger:
                    return states[(int)userIndex - 1].LeftTriggerValue;
                case XboxTrigger.RightTrigger:
                    return states[(int)userIndex - 1].RightTriggerValue;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox trigger: " + trigger);
            }
        }

        public static XboxDpadDirection GetDpadState(uint userIndex)
        {
            return states[(int)userIndex - 1].DpadDirections;
        }

        public static bool GetDpadDirectionValue(uint userIndex, XboxDpadDirection direction)
        {
            return states[(int)userIndex - 1].DpadDirections.HasFlag(direction);
        }

        public static short GetAxisValue(uint userIndex, XboxAxis axis)
        {
            switch (axis)
            {
                case XboxAxis.X:
                    return states[(int)userIndex - 1].AxisXValue;
                case XboxAxis.Y:
                    return states[(int)userIndex - 1].AxisYValue;
                case XboxAxis.Rx:
                    return states[(int)userIndex - 1].AxisRxValue;
                case XboxAxis.Ry:
                    return states[(int)userIndex - 1].AxisRyValue;
                default:
                    throw new NotImplementedException(
                        "Not implemented xbox axis: " + axis);
            }
        }
    }
}