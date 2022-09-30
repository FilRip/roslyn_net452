using System;
using System.Collections.Immutable;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface ITypeParameterSymbol : ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        int Ordinal { get; }

        VarianceKind Variance { get; }

        TypeParameterKind TypeParameterKind { get; }

        IMethodSymbol? DeclaringMethod { get; }

        INamedTypeSymbol? DeclaringType { get; }

        bool HasReferenceTypeConstraint { get; }

        NullableAnnotation ReferenceTypeConstraintNullableAnnotation { get; }

        bool HasValueTypeConstraint { get; }

        bool HasUnmanagedTypeConstraint { get; }

        bool HasNotNullConstraint { get; }

        bool HasConstructorConstraint { get; }

        ImmutableArray<ITypeSymbol> ConstraintTypes { get; }

        ImmutableArray<NullableAnnotation> ConstraintNullableAnnotations { get; }

        new ITypeParameterSymbol OriginalDefinition { get; }

        ITypeParameterSymbol? ReducedFrom { get; }
    }
}
