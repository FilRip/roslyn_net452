using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum ManagedKind : byte
    {
        Unknown = 0,
        Unmanaged = 1,
        UnmanagedWithGenerics = 2,
        Managed = 3
    }
}
