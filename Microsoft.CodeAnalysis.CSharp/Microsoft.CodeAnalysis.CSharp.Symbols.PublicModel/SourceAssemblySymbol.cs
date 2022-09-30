using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class SourceAssemblySymbol : AssemblySymbol, ISourceAssemblySymbol, IAssemblySymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.SourceAssemblySymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol UnderlyingAssemblySymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        Compilation ISourceAssemblySymbol.Compilation => _underlying.DeclaringCompilation;

        public SourceAssemblySymbol(Microsoft.CodeAnalysis.CSharp.Symbols.SourceAssemblySymbol underlying)
        {
            _underlying = underlying;
        }
    }
}
