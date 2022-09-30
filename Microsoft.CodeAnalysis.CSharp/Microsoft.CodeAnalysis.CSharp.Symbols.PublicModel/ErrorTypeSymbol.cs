using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class ErrorTypeSymbol : NamedTypeSymbol, IErrorTypeSymbol, INamedTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol UnderlyingNamedTypeSymbol => _underlying;

        ImmutableArray<ISymbol> IErrorTypeSymbol.CandidateSymbols => _underlying.CandidateSymbols.SelectAsArray((Microsoft.CodeAnalysis.CSharp.Symbol s) => s.GetPublicSymbol());

        CandidateReason IErrorTypeSymbol.CandidateReason => _underlying.CandidateReason;

        public ErrorTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new ErrorTypeSymbol(_underlying, nullableAnnotation);
        }
    }
}
