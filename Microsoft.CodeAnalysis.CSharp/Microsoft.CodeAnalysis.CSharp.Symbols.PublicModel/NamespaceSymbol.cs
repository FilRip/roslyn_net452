using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class NamespaceSymbol : NamespaceOrTypeSymbol, INamespaceSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceSymbol UnderlyingNamespaceSymbol => _underlying;

        bool INamespaceSymbol.IsGlobalNamespace => _underlying.IsGlobalNamespace;

        NamespaceKind INamespaceSymbol.NamespaceKind => _underlying.NamespaceKind;

        Compilation INamespaceSymbol.ContainingCompilation => _underlying.ContainingCompilation;

        ImmutableArray<INamespaceSymbol> INamespaceSymbol.ConstituentNamespaces => _underlying.ConstituentNamespaces.GetPublicSymbols();

        public NamespaceSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceSymbol underlying)
        {
            _underlying = underlying;
        }

        IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol.GetMembers()
        {
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbol>.Enumerator enumerator = _underlying.GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Symbol current = enumerator.Current;
                yield return ((Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol)current).GetPublicSymbol();
            }
        }

        IEnumerable<INamespaceOrTypeSymbol> INamespaceSymbol.GetMembers(string name)
        {
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbol>.Enumerator enumerator = _underlying.GetMembers(name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Symbol current = enumerator.Current;
                yield return ((Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol)current).GetPublicSymbol();
            }
        }

        IEnumerable<INamespaceSymbol> INamespaceSymbol.GetNamespaceMembers()
        {
            foreach (Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceSymbol namespaceMember in _underlying.GetNamespaceMembers())
            {
                yield return namespaceMember.GetPublicSymbol();
            }
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitNamespace(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitNamespace(this);
        }
    }
}
