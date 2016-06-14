namespace KeyboardSplitter.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interceptor;

    public static class KeyboardManager
    {
        private static Input interceptor;

        private static KeyboardKeyStateInfo[] keyboardsInfo;

        static KeyboardManager()
        {
            interceptor = new Input(KeyboardFilterMode.All, MouseFilterMode.None);
            if (!interceptor.Load())
            {
                LogWriter.Write("Interceptor.Load() failed!");
                throw new Exception("Interceptor failed to load!");
            }

            interceptor.OnKeyPressed += new EventHandler<KeyPressedEventArgs>(Interceptor_OnKeyPressed);
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

        public static event KeyPressedEventHandler KeyPressed;

        public static void Dispose()
        {
            interceptor.Unload();
        }

        public static List<InterceptionKeyboard> GetKeyboards()
        {
            return interceptor.GetKeyboards();
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

            if (KeyPressed != null)
            {
                KeyPressed(sender, e);
            }
        }
    }
}