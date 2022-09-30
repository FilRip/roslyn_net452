using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    [Flags()]
    public enum DeclarationModifiers : uint
    {
        None = 0u,
        Abstract = 1u,
        Sealed = 2u,
        Static = 4u,
        New = 8u,
        Public = 0x10u,
        Protected = 0x20u,
        Internal = 0x40u,
        ProtectedInternal = 0x80u,
        Private = 0x100u,
        PrivateProtected = 0x200u,
        ReadOnly = 0x400u,
        Const = 0x800u,
        Volatile = 0x1000u,
        Extern = 0x2000u,
        Partial = 0x4000u,
        Unsafe = 0x8000u,
        Fixed = 0x10000u,
        Virtual = 0x20000u,
        Override = 0x40000u,
        Indexer = 0x80000u,
        Async = 0x100000u,
        Ref = 0x200000u,
        Data = 0x400000u,
        All = 0x7FFFFFu,
        Unset = 0x800000u,
        AccessibilityMask = 0x3F0u
    }
}
