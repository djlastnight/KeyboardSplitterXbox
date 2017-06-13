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

        public static bool PlugIn(uint userIndex)
        {
            if (NativeMethods.PlugInExt(userIndex))
            {
                VirtualXboxController.ResetStates(userIndex);
                return true;
            }

            return false;
        }

        public static bool UnPlug(uint userIndex, bool force = false)
        {
            if (force)
            {
                if (NativeMethods.UnPlugForceExt(userIndex))
                {
                    VirtualXboxController.ResetStates(userIndex);
                    return true;
                }

                return false;
            }
            else
            {
                if (NativeMethods.UnPlugExt(userIndex))
                {
                    VirtualXboxController.ResetStates(userIndex);
                    return true;
                }

                return false;
            }
        }

        public static bool SetAxis(uint userIndex, XboxAxis axis, short value)
        {
            switch (axis)
            {
                case XboxAxis.X:
                    {
                        if (NativeMethods.SetAxisX(userIndex, value))
                        {
                            states[(int)userIndex - 1].AxisXValue = value;
                            return true;
                        }

                        return false;
                    }

                case XboxAxis.Y:
                    {
                        if (NativeMethods.SetAxisY(userIndex, value))
                        {
                            states[(int)userIndex - 1].AxisYValue = value;
                            return true;
                        }

                        return false;
                    }

                case XboxAxis.Rx:
                    {
                        if (NativeMethods.SetAxisRx(userIndex, value))
                        {
                            states[(int)userIndex - 1].AxisRxValue = value;
                            return true;
                        }

                        return false;
                    }

                case XboxAxis.Ry:
                    {
                        if (NativeMethods.SetAxisRy(userIndex, value))
                        {
                            states[(int)userIndex - 1].AxisRyValue = value;
                            return true;
                        }

                        return false;
                    }

                default:
                    throw new NotImplementedException(
                        "Not implemented xbox axis: " + axis);
            }
        }

        public static bool SetButton(uint userIndex, XboxButton button, bool value)
        {
            bool isButtonSet;

            switch (button)
            {
                case XboxButton.Guide:
                    isButtonSet = NativeMethods.SetBtnGuide(userIndex, value);
                    break;
                case XboxButton.A:
                    isButtonSet = NativeMethods.SetBtnA(userIndex, value);
                    break;
                case XboxButton.B:
                    isButtonSet = NativeMethods.SetBtnB(userIndex, value);
                    break;
                case XboxButton.X:
                    isButtonSet = NativeMethods.SetBtnX(userIndex, value);
                    break;
                case XboxButton.Y:
                    isButtonSet = NativeMethods.SetBtnY(userIndex, value);
                    break;
                case XboxButton.Start:
                    isButtonSet = NativeMethods.SetBtnStart(userIndex, value);
                    break;
                case XboxButton.Back:
                    isButtonSet = NativeMethods.SetBtnBack(userIndex, value);
                    break;
                case XboxButton.LeftThumb:
                    isButtonSet = NativeMethods.SetBtnLT(userIndex, value);
                    break;
                case XboxButton.RightThumb:
                    isButtonSet = NativeMethods.SetBtnRT(userIndex, value);
                    break;
                case XboxButton.LeftBumper:
                    isButtonSet = NativeMethods.SetBtnLB(userIndex, value);
                    break;
                case XboxButton.RightBumper:
                    isButtonSet = NativeMethods.SetBtnRB(userIndex, value);
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented Xbox button: " + button);
            }

            if (isButtonSet)
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
            }

            return isButtonSet;
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
            if (NativeMethods.SetDpadExt(userIndex, (int)direction))
            {
                states[(int)userIndex - 1].DpadDirections = direction;
                return true;
            }

            return false;
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

        private static void ResetStates(uint userIndex)
        {
            if (userIndex >= 1 && userIndex <= 4)
            {
                states[userIndex - 1].Reset();
            }
        }
    }
}