namespace XinputWrapper.Structs
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Warning: This struct is non-portable
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputGamepad
    {
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public short Buttons;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public byte LeftTrigger;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public byte RightTrigger;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public short ThumbLX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public short ThumbLY;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public short ThumbRX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public short ThumbRY;

        public static bool operator ==(XInputGamepad a, XInputGamepad b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(XInputGamepad a, XInputGamepad b)
        {
            return !(a == b);
        }

        public void Copy(XInputGamepad source)
        {
            this.ThumbLX = source.ThumbLX;
            this.ThumbLY = source.ThumbLY;
            this.ThumbRX = source.ThumbRX;
            this.ThumbRY = source.ThumbRY;
            this.LeftTrigger = source.LeftTrigger;
            this.RightTrigger = source.RightTrigger;
            this.Buttons = source.Buttons;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XInputGamepad))
            {
                return false;
            }

            var source = (XInputGamepad)obj;
            return (this.ThumbLX == source.ThumbLX)
            && (this.ThumbLY == source.ThumbLY)
            && (this.ThumbRX == source.ThumbRX)
            && (this.ThumbRY == source.ThumbRY)
            && (this.LeftTrigger == source.LeftTrigger)
            && (this.RightTrigger == source.RightTrigger)
            && (this.Buttons == source.Buttons);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
