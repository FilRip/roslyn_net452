using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum TypeCompareKind
    {
        ConsiderEverything = 0,
        ConsiderEverything2 = 0,
        IgnoreCustomModifiersAndArraySizesAndLowerBounds = 1,
        IgnoreDynamic = 2,
        IgnoreTupleNames = 4,
        IgnoreDynamicAndTupleNames = 6,
        IgnoreNullableModifiersForReferenceTypes = 8,
        ObliviousNullableModifierMatchesAny = 0x10,
        IgnoreNativeIntegers = 0x20,
        FunctionPointerRefMatchesOutInRefReadonly = 0x40,
        AllNullableIgnoreOptions = 0x18,
        AllIgnoreOptions = 0x3F,
        AllIgnoreOptionsForVB = 5,
        CLRSignatureCompareOptions = 0x3E
    }
}
