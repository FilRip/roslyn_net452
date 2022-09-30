using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal abstract class NamespaceOrTypeSymbol : Symbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        internal abstract Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol { get; }

        bool INamespaceOrTypeSymbol.IsNamespace => UnderlyingSymbol.Kind == SymbolKind.Namespace;

        bool INamespaceOrTypeSymbol.IsType => UnderlyingSymbol.Kind != SymbolKind.Namespace;

        ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers()
        {
            return UnderlyingNamespaceOrTypeSymbol.GetMembers().GetPublicSymbols();
        }

        ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers(string name)
        {
            return UnderlyingNamespaceOrTypeSymbol.GetMembers(name).GetPublicSymbols();
        }

        ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers()
        {
            return UnderlyingNamespaceOrTypeSymbol.GetTypeMembers().GetPublicSymbols();
        }

        ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name)
        {
            return UnderlyingNamespaceOrTypeSymbol.GetTypeMembers(name).GetPublicSymbols();
        }

        ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name, int arity)
        {
            return UnderlyingNamespaceOrTypeSymbol.GetTypeMembers(name, arity).GetPublicSymbols();
        }
    }
}
