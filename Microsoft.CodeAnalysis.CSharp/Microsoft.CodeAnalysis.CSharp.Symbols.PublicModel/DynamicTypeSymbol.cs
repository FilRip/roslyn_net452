using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class DynamicTypeSymbol : TypeSymbol, IDynamicTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.DynamicTypeSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        public DynamicTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.DynamicTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new DynamicTypeSymbol(_underlying, nullableAnnotation);
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitDynamicType(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default => visitor.VisitDynamicType(this);
    }
}
