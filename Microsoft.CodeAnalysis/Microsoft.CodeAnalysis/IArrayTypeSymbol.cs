using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IArrayTypeSymbol : ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        int Rank { get; }

        bool IsSZArray { get; }

        ImmutableArray<int> LowerBounds { get; }

        ImmutableArray<int> Sizes { get; }

        ITypeSymbol ElementType { get; }

        NullableAnnotation ElementNullableAnnotation { get; }

        ImmutableArray<CustomModifier> CustomModifiers { get; }

        bool Equals(IArrayTypeSymbol? other);
    }
}
