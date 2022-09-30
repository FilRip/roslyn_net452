using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum SymbolDisplayMiscellaneousOptions
    {
        None = 0,
        UseSpecialTypes = 1,
        EscapeKeywordIdentifiers = 2,
        UseAsterisksInMultiDimensionalArrays = 4,
        UseErrorTypeSymbolName = 8,
        RemoveAttributeSuffix = 0x10,
        ExpandNullable = 0x20,
        IncludeNullableReferenceTypeModifier = 0x40,
        AllowDefaultLiteral = 0x80,
        IncludeNotNullableReferenceTypeModifier = 0x100
    }
}
