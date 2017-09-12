namespace KeyboardSplitter
{
    using System;
    using System.Collections.Generic;
    using Interceptor;

    public class KeyboardKeyStateInfo
    {
        private readonly List<string> keysDownCollection;

        public KeyboardKeyStateInfo(string keyboardSource)
        {
            if (keyboardSource == null)
            {
                throw new ArgumentNullException("keyboardSource");
            }

            this.KeyboardSource = keyboardSource;
            this.keysDownCollection = new List<string>();
        }

        public string KeyboardSource
        {
            get;
            private set;
        }

        public bool IsKeyDown(string key)
        {
            return this.keysDownCollection.Contains(key);
        }

        public void SetKeyState(string key, bool isDown)
        {
            bool stateExsists = this.keysDownCollection.Contains(key);
            if (isDown)
            {
                if (!stateExsists)
                {
                    this.keysDownCollection.Add(key);
                }
            }
            else
            {
                if (!stateExsists)
                {
                    return;
                }

                this.keysDownCollection.Remove(key);
            }
        }
    }
}