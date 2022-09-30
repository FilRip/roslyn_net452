using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [Flags()]
    public enum TypeParameterConstraintKind
    {
        None = 0,
        ReferenceType = 1,
        ValueType = 2,
        Constructor = 4,
        Unmanaged = 8,
        NullableReferenceType = 0x11,
        NotNullableReferenceType = 0x21,
        ObliviousNullabilityIfReferenceType = 0x40,
        NotNull = 0x80,
        Default = 0x100,
        PartialMismatch = 0x200,
        ValueTypeFromConstraintTypes = 0x400,
        ReferenceTypeFromConstraintTypes = 0x800,
        AllReferenceTypeKinds = 0x31,
        AllValueTypeKinds = 0xA,
        AllNonNullableKinds = 0xF
    }
}
