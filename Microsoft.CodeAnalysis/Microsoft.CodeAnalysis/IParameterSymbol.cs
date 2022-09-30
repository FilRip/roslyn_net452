using System;
using System.Collections.Immutable;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IParameterSymbol : ISymbol, IEquatable<ISymbol?>
    {
        RefKind RefKind { get; }

        bool IsParams { get; }

        bool IsOptional { get; }

        bool IsThis { get; }

        bool IsDiscard { get; }

        ITypeSymbol Type { get; }

        NullableAnnotation NullableAnnotation { get; }

        ImmutableArray<CustomModifier> CustomModifiers { get; }

        ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        int Ordinal { get; }

        bool HasExplicitDefaultValue { get; }

        object? ExplicitDefaultValue { get; }

        new IParameterSymbol OriginalDefinition { get; }
    }
}
