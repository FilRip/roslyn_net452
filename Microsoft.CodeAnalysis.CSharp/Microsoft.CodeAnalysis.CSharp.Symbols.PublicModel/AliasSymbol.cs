using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class AliasSymbol : Symbol, IAliasSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.AliasSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        INamespaceOrTypeSymbol IAliasSymbol.Target => _underlying.Target.GetPublicSymbol();

        public AliasSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.AliasSymbol underlying)
        {
            _underlying = underlying;
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitAlias(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default => visitor.VisitAlias(this);
    }
}
