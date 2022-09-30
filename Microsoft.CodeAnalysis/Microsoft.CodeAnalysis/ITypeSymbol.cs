using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface ITypeSymbol : INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        TypeKind TypeKind { get; }

        INamedTypeSymbol? BaseType { get; }

        ImmutableArray<INamedTypeSymbol> Interfaces { get; }

        ImmutableArray<INamedTypeSymbol> AllInterfaces { get; }

        bool IsReferenceType { get; }

        bool IsValueType { get; }

        bool IsAnonymousType { get; }

        bool IsTupleType { get; }

        bool IsNativeIntegerType { get; }

        new ITypeSymbol OriginalDefinition { get; }

        SpecialType SpecialType { get; }

        bool IsRefLikeType { get; }

        bool IsUnmanagedType { get; }

        bool IsReadOnly { get; }

        bool IsRecord { get; }

        NullableAnnotation NullableAnnotation { get; }

        ISymbol? FindImplementationForInterfaceMember(ISymbol interfaceMember);

        string ToDisplayString(NullableFlowState topLevelNullability, SymbolDisplayFormat? format = null);

        ImmutableArray<SymbolDisplayPart> ToDisplayParts(NullableFlowState topLevelNullability, SymbolDisplayFormat? format = null);

        string ToMinimalDisplayString(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat? format = null);

        ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat? format = null);

        ITypeSymbol WithNullableAnnotation(NullableAnnotation nullableAnnotation);
    }
}
