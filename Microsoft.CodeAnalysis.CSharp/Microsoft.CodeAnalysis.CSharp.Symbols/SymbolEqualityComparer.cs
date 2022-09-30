using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SymbolEqualityComparer : EqualityComparer<Symbol>
    {
        internal static readonly EqualityComparer<Symbol> ConsiderEverything = new SymbolEqualityComparer(TypeCompareKind.ConsiderEverything);

        internal static readonly EqualityComparer<Symbol> IgnoringTupleNamesAndNullability = new SymbolEqualityComparer(TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

        internal static readonly EqualityComparer<Symbol> IgnoringDynamicTupleNamesAndNullability = new SymbolEqualityComparer(TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

        internal static readonly EqualityComparer<Symbol> IgnoringNullable = new SymbolEqualityComparer(TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

        internal static readonly EqualityComparer<Symbol> ObliviousNullableModifierMatchesAny = new SymbolEqualityComparer(TypeCompareKind.ObliviousNullableModifierMatchesAny);

        internal static readonly EqualityComparer<Symbol> AllIgnoreOptions = new SymbolEqualityComparer(TypeCompareKind.AllIgnoreOptions);

        internal static readonly EqualityComparer<Symbol> AllIgnoreOptionsPlusNullableWithUnknownMatchesAny = new SymbolEqualityComparer(TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.ObliviousNullableModifierMatchesAny | TypeCompareKind.IgnoreNativeIntegers);

        internal static readonly EqualityComparer<Symbol> CLRSignature = new SymbolEqualityComparer(TypeCompareKind.CLRSignatureCompareOptions);

        private readonly TypeCompareKind _comparison;

        internal static EqualityComparer<Symbol> IncludeNullability => ConsiderEverything;

        private SymbolEqualityComparer(TypeCompareKind comparison)
        {
            _comparison = comparison;
        }

        public override int GetHashCode(Symbol obj)
        {
            return obj?.GetHashCode() ?? 0;
        }

        public override bool Equals(Symbol x, Symbol y)
        {
            return x?.Equals(y, _comparison) ?? ((object)y == null);
        }
    }
}
