namespace Interceptor
{
    using Interceptor.Enums;

    public class KeyInfo
    {
        internal KeyInfo(InterceptionKey key, bool isDown)
        {
            this.Key = key;
            this.IsDown = isDown;
        }

        public InterceptionKey Key { get; private set; }

        public bool IsDown { get; private set; }
    }
}
