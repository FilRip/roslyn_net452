namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class NonSourceAssemblySymbol : AssemblySymbol
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol UnderlyingAssemblySymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        public NonSourceAssemblySymbol(Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol underlying)
        {
            _underlying = underlying;
        }
    }
}
