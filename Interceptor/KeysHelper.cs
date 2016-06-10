namespace Interceptor
{
    using System;
    using System.Linq;

    public static class KeysHelper
    {
        public static string GetCorrectedKeyName(InterceptionKeys key, KeyState state)
        {
            // checking for duplicates
            var enums = (InterceptionKeys[])Enum.GetValues(typeof(InterceptionKeys));
            if (enums.Count() != enums.Distinct().Count())
            {
                throw new InvalidOperationException("Keys enum is not distinct!");
            }

            if (state == KeyState.Up || state == KeyState.Down)
            {
                return key.ToString();
            }

            if ((state == KeyState.E1 || (state == (KeyState.E1 | KeyState.Up))) && key == InterceptionKeys.LeftControl)
            {
                return InterceptionKeys.Pause.ToString();
            }

            if (state == (KeyState.E0 | KeyState.Down) || state == (KeyState.Up | KeyState.E0))
            {
                switch (key)
                {
                    case InterceptionKeys.Q:
                        return InterceptionKeys.MediaPreviousTrack.ToString();
                    case InterceptionKeys.G:
                        return InterceptionKeys.MediaPlayPause.ToString();
                    case InterceptionKeys.P:
                        return InterceptionKeys.MediaNextTrack.ToString();
                    case InterceptionKeys.B:
                        return InterceptionKeys.VolumeUp.ToString();
                    case InterceptionKeys.C:
                        return InterceptionKeys.VolumeDown.ToString();
                    case InterceptionKeys.D:
                        return InterceptionKeys.VolumeMute.ToString();

                    case InterceptionKeys.Numpad0:
                        return InterceptionKeys.Insert.ToString();
                    case InterceptionKeys.Numpad1:
                        return InterceptionKeys.End.ToString();
                    case InterceptionKeys.Numpad2:
                        return InterceptionKeys.Down.ToString();
                    case InterceptionKeys.Numpad3:
                        return InterceptionKeys.PageDown.ToString();
                    case InterceptionKeys.Numpad4:
                        return InterceptionKeys.Left.ToString();

                    // 5 is not used
                    case InterceptionKeys.Numpad6:
                        return InterceptionKeys.Right.ToString();
                    case InterceptionKeys.Numpad7:
                        return InterceptionKeys.Home.ToString();
                    case InterceptionKeys.Numpad8:
                        return InterceptionKeys.Up.ToString();
                    case InterceptionKeys.Numpad9:
                        return InterceptionKeys.PageUp.ToString();

                    case InterceptionKeys.NumpadAsterisk:
                        return InterceptionKeys.PrintScreen.ToString();

                    case InterceptionKeys.NumpadDelete:
                        return InterceptionKeys.Delete.ToString();

                    case InterceptionKeys.LeftControl:
                        return InterceptionKeys.RightControl.ToString();
                    case InterceptionKeys.LeftAlt:
                        return InterceptionKeys.RightAlt.ToString();
                    case InterceptionKeys.Menu:
                        return InterceptionKeys.Menu.ToString();
                    case (InterceptionKeys)120:
                        return InterceptionKeys.Oem13.ToString();
                    case InterceptionKeys.E:
                        return InterceptionKeys.Oem2.ToString();
                    case InterceptionKeys.I:
                        return InterceptionKeys.Oem3.ToString();
                    case (InterceptionKeys)95:
                        return InterceptionKeys.Oem5.ToString();
                    case InterceptionKeys.Nine:
                        return InterceptionKeys.Oem6.ToString();
                    case InterceptionKeys.M:
                        return InterceptionKeys.Oem7.ToString();
                    case InterceptionKeys.LeftWindows:
                        return InterceptionKeys.LeftWindows.ToString();
                    case (InterceptionKeys)110:
                        return InterceptionKeys.Oem4.ToString();
                    case InterceptionKeys.LeftShift:
                        return InterceptionKeys.ShiftModifier.ToString();
                    case InterceptionKeys.ScrollLock:
                        return InterceptionKeys.Break.ToString();

                    case InterceptionKeys.Escape:
                        return InterceptionKeys.Oem0.ToString();
                    case InterceptionKeys.ForwardSlashQuestionMark:
                        return InterceptionKeys.NumpadDivide.ToString();

                    case InterceptionKeys.RightWindows:
                        return InterceptionKeys.RightWindows.ToString();

                    case InterceptionKeys.Enter:
                        return InterceptionKeys.NumpadEnter.ToString();
                }
            }

            return "unknown";
        }
    }
}
