namespace Interceptor
{
    using System;

    public class MousePressedEventArgs : EventArgs
    {
        public MouseState State { get; set; }

        public bool Handled { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public short Rolling { get; set; }

        public int DeviceID { get; set; }

        public bool IsDown
        {
            get
            {
                if (this.State == MouseState.Wheel || this.State == MouseState.HWheel)
                {
                    return this.Rolling != 0;
                }

                bool isLeftDown = this.State == MouseState.LeftDown || this.State == MouseState.LeftExtraDown;
                bool isRightDown = this.State == MouseState.RightDown || this.State == MouseState.RightExtraDown;
                bool isMiddleDown = this.State == MouseState.MiddleDown;

                return isLeftDown || isRightDown || isMiddleDown;
            }
        }

        public InterceptionKeys Key
        {
            get
            {
                if (this.DelayedKey != InterceptionKeys.None)
                {
                    InterceptionKeys key;
                    Enum.TryParse<InterceptionKeys>(this.DelayedKey.ToString(), out key);
                    this.DelayedKey = InterceptionKeys.None;
                    return key;
                }

                switch (this.State)
                {
                    case MouseState.None:
                        return InterceptionKeys.None;
                    case MouseState.LeftDown:
                    case MouseState.LeftUp:
                        return InterceptionKeys.MouseLeftButton;
                    case MouseState.RightDown:
                    case MouseState.RightUp:
                        return InterceptionKeys.MouseRightButton;
                    case MouseState.MiddleDown:
                    case MouseState.MiddleUp:
                        return InterceptionKeys.MouseMiddleButton;
                    case MouseState.LeftExtraDown:
                    case MouseState.LeftExtraUp:
                        return InterceptionKeys.MouseExtraLeft;
                    case MouseState.RightExtraDown:
                    case MouseState.RightExtraUp:
                        return InterceptionKeys.MouseExtraRight;
                    case MouseState.Wheel:
                        if (this.Rolling == 120)
                        {
                            return InterceptionKeys.MouseWheelUp;
                        }
                        else if (this.Rolling == -120)
                        {
                            return InterceptionKeys.MouseWheelDown;
                        }

                        return InterceptionKeys.None;
                    case MouseState.HWheel:
                        return this.Rolling > 0 ? InterceptionKeys.MouseWheelRight : InterceptionKeys.MouseWheelLeft;
                    default:
                        throw new NotImplementedException("Not implemented mouse state: " + this.State.ToString());
                }
            }
        }

        public InterceptionKeys DelayedKey
        {
            get;
            set;
        }

        public bool IsExtraButton
        {
            get
            {
                return this.State == MouseState.LeftExtraDown ||
                       this.State == MouseState.LeftExtraUp ||
                       this.State == MouseState.RightExtraDown ||
                       this.State == MouseState.RightExtraUp;
            }
        }
    }
}