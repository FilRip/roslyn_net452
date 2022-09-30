using System.Collections.Generic;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class SymbolEqualityComparer : IEqualityComparer<ISymbol?>
    {
        public static readonly SymbolEqualityComparer Default = new SymbolEqualityComparer(TypeCompareKind.AllNullableIgnoreOptions);

        public static readonly SymbolEqualityComparer IncludeNullability = new SymbolEqualityComparer(TypeCompareKind.ConsiderEverything);

        public static readonly SymbolEqualityComparer ConsiderEverything = new SymbolEqualityComparer(TypeCompareKind.ConsiderEverything);

        public static readonly SymbolEqualityComparer IgnoreAll = new SymbolEqualityComparer(TypeCompareKind.AllIgnoreOptions);

        public static readonly SymbolEqualityComparer CLRSignature = new SymbolEqualityComparer(TypeCompareKind.CLRSignatureCompareOptions);

        public TypeCompareKind CompareKind { get; }

        internal SymbolEqualityComparer(TypeCompareKind compareKind)
        {
            CompareKind = compareKind;
        }

        public bool Equals(ISymbol? x, ISymbol? y)
        {
            return x?.Equals(y, this) ?? (y == null);
        }

        public int GetHashCode(ISymbol? obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
