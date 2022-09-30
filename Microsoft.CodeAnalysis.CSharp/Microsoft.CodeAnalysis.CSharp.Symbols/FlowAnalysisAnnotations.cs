using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [Flags()]
    public enum FlowAnalysisAnnotations
    {
        None = 0,
        AllowNull = 1,
        DisallowNull = 2,
        MaybeNullWhenTrue = 4,
        MaybeNullWhenFalse = 8,
        MaybeNull = 0xC,
        NotNullWhenTrue = 0x10,
        NotNullWhenFalse = 0x20,
        NotNull = 0x30,
        DoesNotReturnIfFalse = 0x40,
        DoesNotReturnIfTrue = 0x80,
        DoesNotReturn = 0xC0
    }
}
