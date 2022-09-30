namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class NonErrorNamedTypeSymbol : NamedTypeSymbol
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol UnderlyingNamedTypeSymbol => _underlying;

        public NonErrorNamedTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new NonErrorNamedTypeSymbol(_underlying, nullableAnnotation);
        }
    }
}
