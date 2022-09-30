using System.Collections.Immutable;

using Microsoft.Cci;

#nullable enable

namespace Microsoft.CodeAnalysis.Symbols
{
    public interface ISymbolInternal
    {
        SymbolKind Kind { get; }

        string Name { get; }

        string MetadataName { get; }

        Compilation DeclaringCompilation { get; }

        ISymbolInternal ContainingSymbol { get; }

        IAssemblySymbolInternal ContainingAssembly { get; }

        IModuleSymbolInternal ContainingModule { get; }

        INamedTypeSymbolInternal ContainingType { get; }

        INamespaceSymbolInternal ContainingNamespace { get; }

        bool IsDefinition { get; }

        ImmutableArray<Location> Locations { get; }

        bool IsImplicitlyDeclared { get; }

        Accessibility DeclaredAccessibility { get; }

        bool IsStatic { get; }

        bool IsVirtual { get; }

        bool IsOverride { get; }

        bool IsAbstract { get; }

        bool Equals(ISymbolInternal? other, TypeCompareKind compareKind);

        ISymbol GetISymbol();

        IReference GetCciAdapter();
    }
}
