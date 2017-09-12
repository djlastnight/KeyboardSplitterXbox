namespace VirtualXbox
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using VirtualXbox.Enums;

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
            return NativeMethods.ControllerExists(userIndex);
        }

        public static bool IsOwned(uint userIndex)
        {
            return NativeMethods.IsControllerOwned(userIndex);
        }

        public static int GetLedNumber(uint userIndex)
        {
            byte ledNumber;
            if (NativeMethods.GetLedNumber(userIndex, out ledNumber))
            {
                return ledNumber;
            }

            return -1;
        }

        public static bool PlugIn(uint userIndex)
        {
            if (NativeMethods.PlugIn(userIndex))
            {
                VirtualXboxController.ResetStates(userIndex);
                return true;
            }

            return false;
        }

        public static bool UnPlug(uint userIndex, bool force = false)
        {
            if (NativeMethods.Unplug(userIndex, force))
            {
                VirtualXboxController.ResetStates(userIndex);
                return true;
            }

            return false;
        }

        public static bool SetAxis(uint userIndex, XboxAxis axis, short value)
        {
            if (!VirtualXboxController.IsOwned(userIndex))
            {
                return false;
            }

            if (!NativeMethods.SetAxis(userIndex, (uint)axis, value))
            {
                return false;
            }

            switch (axis)
            {
                case XboxAxis.X:
                    states[(int)userIndex - 1].AxisXValue = value;
                    break;
                case XboxAxis.Y:
                    states[(int)userIndex - 1].AxisYValue = value;
                    break;
                case XboxAxis.Rx:
                    states[(int)userIndex - 1].AxisRxValue = value;
                    break;

                case XboxAxis.Ry:
                    states[(int)userIndex - 1].AxisRyValue = value;
                    break;

                default:
                    throw new NotImplementedException(
                        "Not implemented xbox axis: " + axis);
            }

            return true;
        }

        public static bool SetAxis(uint userIndex, XboxAxis axis, XboxAxisPosition position)
        {
            if (!VirtualXboxController.IsOwned(userIndex))
            {
                return false;
            }

            return VirtualXboxController.SetAxis(userIndex, axis, (short)position);
        }

        public static bool SetButton(uint userIndex, XboxButton button, bool value)
        {
            if (!VirtualXboxController.IsOwned(userIndex))
            {
                return false;
            }

            bool isButtonSet = NativeMethods.SetButton(userIndex, (uint)button, value);

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
            if (!VirtualXboxController.IsOwned(userIndex))
            {
                return false;
            }

            if (!NativeMethods.SetTrigger(userIndex, (uint)trigger, value))
            {
                return false;
            }

            switch (trigger)
            {
                case XboxTrigger.Left:
                    states[(int)userIndex - 1].LeftTriggerValue = value;
                    break;
                case XboxTrigger.Right:
                    states[(int)userIndex - 1].RightTriggerValue = value;
                    break;
                default:
                    throw new NotImplementedException(
                        "Not implemented Xbox trigger: " + trigger);
            }

            return true;
        }

        public static bool SetDPad(uint userIndex, XboxDpadDirection directions)
        {
            if (!VirtualXboxController.IsOwned(userIndex))
            {
                return false;
            }

            if (NativeMethods.SetDpad(userIndex, (int)directions))
            {
                states[(int)userIndex - 1].DpadDirections = directions;
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
                case XboxTrigger.Left:
                    return states[(int)userIndex - 1].LeftTriggerValue;
                case XboxTrigger.Right:
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