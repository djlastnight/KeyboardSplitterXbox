namespace Interceptor
{
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Predicate(int device);
}