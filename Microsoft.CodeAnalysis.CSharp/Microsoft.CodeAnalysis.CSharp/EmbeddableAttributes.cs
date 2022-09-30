using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    [Flags()]
    internal enum EmbeddableAttributes
    {
        IsReadOnlyAttribute = 1,
        IsByRefLikeAttribute = 2,
        IsUnmanagedAttribute = 4,
        NullableAttribute = 8,
        NullableContextAttribute = 0x10,
        NullablePublicOnlyAttribute = 0x20,
        NativeIntegerAttribute = 0x40
    }
}
