namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interceptor;

    public static class InputManager
    {
        private static Input interceptor;

        private static KeyboardKeyStateInfo[] keyboardsInfo;

        static InputManager()
        {
            var mouseFilter = MouseFilterMode.LeftDown | MouseFilterMode.RightDown | MouseFilterMode.LeftUp | MouseFilterMode.RightUp;
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

        public delegate void KeyPressedEventHandler(object sender, KeyPressedEventArgs e);

        public delegate void MousePressedEventHandler(object sender, MousePressedEventArgs e);

        public static event EventHandler KeyPressed;

        public static event EventHandler MousePressed;

        public static void Dispose()
        {
            interceptor.Unload();
        }

        public static List<InterceptionKeyboard> GetKeyboards()
        {
            var manual = new InterceptionKeyboard(0, string.Empty, "OnScreen Mouse Feeder", "None");
            var keyboards = new List<InterceptionKeyboard>();
            keyboards.Add(manual);
            keyboards.AddRange(interceptor.GetKeyboards());

            return keyboards;
        }

        public static bool IsKeyDown(string keyboardStrongName, string key)
        {
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
            bool isDown = e.State == MouseState.LeftDown || e.State == MouseState.RightDown;
            bool isLeft = e.State == MouseState.LeftDown || e.State == MouseState.LeftUp;
            var key = isLeft ? InterceptionKeys.MouseLeftButton : InterceptionKeys.MouseRightButton;
            var state = isDown ? KeyState.Down : KeyState.Up;

            foreach (var keyboardInfo in keyboardsInfo)
            {
                keyboardInfo.SetKeyState(key.ToString(), state);
            }

            if (InputManager.MousePressed != null)
            {
                InputManager.MousePressed(sender, e);
            }
        }
    }
}