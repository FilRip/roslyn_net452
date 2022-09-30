using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class RoslynUnsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ref T NullRef<T>()
        {
            return ref Unsafe.AsRef<T>(null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool IsNullRef<T>(ref T source)
        {
            return Unsafe.AsPointer(ref source) == null;
        }
    }
}
