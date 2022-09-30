using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class FunctionPointerTypeSymbol : TypeSymbol, IFunctionPointerTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.FunctionPointerTypeSymbol _underlying;

        public IMethodSymbol Signature => _underlying.Signature.GetPublicSymbol();

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        public FunctionPointerTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.FunctionPointerTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFunctionPointerType(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerType(this);

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new FunctionPointerTypeSymbol(_underlying, nullableAnnotation);
        }
    }
}
