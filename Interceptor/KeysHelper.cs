namespace Interceptor
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Interceptor.Enums;

    public static class KeysHelper
    {
        public static void CheckInterceptionKeysForDuplicates()
        {
            // checking for duplicates
            var type = typeof(InterceptionKey);
            var enums = (InterceptionKey[])Enum.GetValues(type);
            if (enums.Count() != enums.Distinct().Count())
            {
                throw new InvalidEnumArgumentException(type.Name + " enum is not distinct!");
            }
        }

        public static bool IsMouseWheelKey(InterceptionKey key)
        {
            if (key == InterceptionKey.MouseWheelUp ||
                key == InterceptionKey.MouseWheelDown ||
                key == InterceptionKey.MouseWheelLeft ||
                key == InterceptionKey.MouseWheelRight)
            {
                return true;
            }

            return false;
        }

        public static bool IsMouseMoveKey(InterceptionKey key)
        {
            if (key == InterceptionKey.MouseMoveLeft ||
                key == InterceptionKey.MouseMoveRight ||
                key == InterceptionKey.MouseMoveUp ||
                key == InterceptionKey.MouseMoveDown)
            {
                return true;
            }

            return false;
        }

        public static bool IsMouseClickKey(InterceptionKey key)
        {
            if (key == InterceptionKey.MouseLeftButton ||
                key == InterceptionKey.MouseMiddleButton ||
                key == InterceptionKey.MouseRightButton ||
                key == InterceptionKey.MouseExtraLeft ||
                key == InterceptionKey.MouseExtraRight)
            {
                return true;
            }

            return false;
        }

        internal static InterceptionKey GetCorrectedKey(InterceptionKey key, KeyState state)
        {
            if (state == KeyState.Up || state == KeyState.Down)
            {
                return key;
            }

            if ((state == KeyState.E1 || (state == (KeyState.E1 | KeyState.Up))) && key == InterceptionKey.LeftControl)
            {
                return InterceptionKey.Pause;
            }

            if (state == (KeyState.E0 | KeyState.Down) || state == (KeyState.Up | KeyState.E0))
            {
                switch (key)
                {
                    case InterceptionKey.Q:
                        return InterceptionKey.MediaPreviousTrack;
                    case InterceptionKey.G:
                        return InterceptionKey.MediaPlayPause;
                    case InterceptionKey.P:
                        return InterceptionKey.MediaNextTrack;
                    case InterceptionKey.B:
                        return InterceptionKey.VolumeUp;
                    case InterceptionKey.C:
                        return InterceptionKey.VolumeDown;
                    case InterceptionKey.D:
                        return InterceptionKey.VolumeMute;

                    case InterceptionKey.Numpad0:
                        return InterceptionKey.Insert;
                    case InterceptionKey.Numpad1:
                        return InterceptionKey.End;
                    case InterceptionKey.Numpad2:
                        return InterceptionKey.Down;
                    case InterceptionKey.Numpad3:
                        return InterceptionKey.PageDown;
                    case InterceptionKey.Numpad4:
                        return InterceptionKey.Left;

                    // 5 is not used
                    case InterceptionKey.Numpad6:
                        return InterceptionKey.Right;
                    case InterceptionKey.Numpad7:
                        return InterceptionKey.Home;
                    case InterceptionKey.Numpad8:
                        return InterceptionKey.Up;
                    case InterceptionKey.Numpad9:
                        return InterceptionKey.PageUp;

                    case InterceptionKey.NumpadAsterisk:
                        return InterceptionKey.PrintScreen;

                    case InterceptionKey.NumpadDelete:
                        return InterceptionKey.Delete;

                    case InterceptionKey.LeftControl:
                        return InterceptionKey.RightControl;
                    case InterceptionKey.LeftAlt:
                        return InterceptionKey.RightAlt;
                    case InterceptionKey.Menu:
                        return InterceptionKey.Menu;
                    case (InterceptionKey)120:
                        return InterceptionKey.Oem13;
                    case InterceptionKey.E:
                        return InterceptionKey.Oem2;
                    case InterceptionKey.I:
                        return InterceptionKey.Oem3;
                    case (InterceptionKey)95:
                        return InterceptionKey.Oem5;
                    case InterceptionKey.Nine:
                        return InterceptionKey.Oem6;
                    case InterceptionKey.M:
                        return InterceptionKey.Oem7;
                    case InterceptionKey.LeftWindows:
                        return InterceptionKey.LeftWindows;
                    case (InterceptionKey)110:
                        return InterceptionKey.Oem4;
                    case InterceptionKey.LeftShift:
                        return InterceptionKey.ShiftModifier;
                    case InterceptionKey.ScrollLock:
                        return InterceptionKey.Break;

                    case InterceptionKey.Escape:
                        return InterceptionKey.Oem0;
                    case InterceptionKey.ForwardSlashQuestionMark:
                        return InterceptionKey.NumpadDivide;

                    case InterceptionKey.RightWindows:
                        return InterceptionKey.RightWindows;

                    case InterceptionKey.Enter:
                        return InterceptionKey.NumpadEnter;
                }
            }

            return InterceptionKey.Unknown;
        }
    }
}
