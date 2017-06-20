namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interceptor;
    using KeyboardSplitter.Helpers;

    public static class InputManager
    {
        private static Input interceptor;

        private static KeyboardKeyStateInfo[] keyboardsInfo;

        static InputManager()
        {
            // All flags, except mouse move
            var mouseFilter = MouseFilterMode.All &~MouseFilterMode.MouseMove;
            interceptor = new Input(KeyboardFilterMode.All, mouseFilter);
            if (!interceptor.Load())
            {
                LogWriter.Write("Interceptor.Load() failed!");
                throw new Exception("Interceptor failed to load!");
            }

            interceptor.OnKeyPressed += new EventHandler<KeyPressedEventArgs>(Interceptor_OnKeyPressed);
            interceptor.OnMousePressed += new EventHandler<MousePressedEventArgs>(Interceptor_OnMousePressed);
            keyboardsInfo = new KeyboardKeyStateInfo[10]
            {
                new KeyboardKeyStateInfo("Keyboard_01"),
                new KeyboardKeyStateInfo("Keyboard_02"),
                new KeyboardKeyStateInfo("Keyboard_03"),
                new KeyboardKeyStateInfo("Keyboard_04"),
                new KeyboardKeyStateInfo("Keyboard_05"),
                new KeyboardKeyStateInfo("Keyboard_06"),
                new KeyboardKeyStateInfo("Keyboard_07"),
                new KeyboardKeyStateInfo("Keyboard_08"),
                new KeyboardKeyStateInfo("Keyboard_09"),
                new KeyboardKeyStateInfo("Keyboard_10"),
            };
        }

        public static event EventHandler KeyPressed;

        public static event EventHandler MousePressed;

        public static void Dispose()
        {
            interceptor.Unload();
        }

        public static List<InterceptionKeyboard> GetKeyboards()
        {
            var manual = new InterceptionKeyboard(0, string.Empty, "None");
            var keyboards = new List<InterceptionKeyboard>();
            keyboards.Add(manual);
            keyboards.AddRange(interceptor.GetKeyboards());

            return keyboards;
        }

        public static List<InterceptionMouse> GetMouses()
        {
            var mouses = interceptor.GetMouses();
            foreach (var mouse in mouses)
            {
                LogWriter.Write(mouse.StrongName + " " + mouse.FriendlyName + " " + mouse.HardwareID);
            }

            return interceptor.GetMouses();
        }

        public static bool IsKeyDown(string keyboardStrongName, string key)
        {
            if (keyboardStrongName == "None")
            {
                return false;
            }

            var info = keyboardsInfo.FirstOrDefault(x => x.KeyboardSource == keyboardStrongName);
            if (info == null)
            {
                var allowed = "Allowed values are in range 'Keyboard_01' to 'Keyboard_10'";

                throw new InvalidOperationException(
                    string.Format("Invalid keyboard strong name: '{0}'. {1}", keyboardStrongName, allowed));
            }

            return info.IsKeyDown(key);
        }

        public static void SetFakeDown(string keyboardStrongName, string key)
        {
            var info = keyboardsInfo.FirstOrDefault(x => x.KeyboardSource == keyboardStrongName);
            if (info == null)
            {
                var allowed = "Allowed values are in range 'Keyboard_01' to 'Keyboard_10'";
                throw new InvalidOperationException(
                    string.Format("Invalid keyboard strong name: '{0}'. {1}", keyboardStrongName, allowed));
            }

            info.SetKeyState(key, KeyState.Down);
        }

        public static void ResetFakeStates()
        {
            foreach (var info in keyboardsInfo)
            {
                foreach (var key in Enum.GetValues(typeof(InterceptionKeys)))
                {
                    info.SetKeyState(key.ToString(), KeyState.Up);
                }
            }
        }

        private static void Interceptor_OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            keyboardsInfo[e.Keyboard.DeviceID - 1].SetKeyState(e.CorrectedKey, e.State);

            if (InputManager.KeyPressed != null)
            {
                InputManager.KeyPressed(sender, e);
            }
        }

        private static void Interceptor_OnMousePressed(object sender, MousePressedEventArgs e)
        {
            var state = e.IsDown ? KeyState.Down : KeyState.Up;

            foreach (var keyboardInfo in keyboardsInfo)
            {
                keyboardInfo.SetKeyState(e.Key.ToString(), state);
            }

            OnMousePressed(sender, e);
            if (e.Rolling != 0)
            {
                var action = new Action(() =>
                {
                    var newArgs = new MousePressedEventArgs()
                    {
                        Handled = e.Handled,
                        Rolling = 0,
                        State = e.State,
                        X = e.X,
                        Y = e.Y,
                        DelayedKey = e.Key
                    };

                    InputManager.OnMousePressed(sender, newArgs);
                });

                var task = new DelayedTask(action, TimeSpan.FromMilliseconds(100));
                task.Run();
            }
        }

        private static void OnMousePressed(object sender, EventArgs e)
        {
            if (InputManager.MousePressed != null)
            {
                var args = e as MousePressedEventArgs;
                LogWriter.Write("Firing MousePressed " + args.Key + " " + args.IsDown);
                InputManager.MousePressed(sender, e);
            }
        }
    }
}