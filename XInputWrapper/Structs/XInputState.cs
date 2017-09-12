namespace XinputWrapper.Structs
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public struct XInputState
    {
         [FieldOffset(0)]
        public int PacketNumber;

         [FieldOffset(4)]
        public XInputGamepad Gamepad;

         public static bool operator ==(XInputState a, XInputState b)
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

         public static bool operator !=(XInputState a, XInputState b)
         {
             return !(a == b);
         }

         public void Copy(XInputState source)
         {
             this.PacketNumber = source.PacketNumber;
             this.Gamepad.Copy(source.Gamepad);
         }

         public override bool Equals(object obj)
         {
             if ((obj == null) || (!(obj is XInputState)))
             {
                 return false;
             }

             XInputState source = (XInputState)obj;

             return this.PacketNumber == source.PacketNumber && this.Gamepad.Equals(source.Gamepad);
         }

         public override int GetHashCode()
         {
             return base.GetHashCode();
         }
    }
}
