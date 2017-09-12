namespace Interceptor
{
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int Predicate(int device);
}