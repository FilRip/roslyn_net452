using System;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class ArrayTypeSymbol : TypeSymbol, IArrayTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.ArrayTypeSymbol _underlying;

        private ITypeSymbol? _lazyElementType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        int IArrayTypeSymbol.Rank => _underlying.Rank;

        bool IArrayTypeSymbol.IsSZArray => _underlying.IsSZArray;

        ImmutableArray<int> IArrayTypeSymbol.LowerBounds => _underlying.LowerBounds;

        ImmutableArray<int> IArrayTypeSymbol.Sizes => _underlying.Sizes;

        ITypeSymbol IArrayTypeSymbol.ElementType
        {
            get
            {
                if (_lazyElementType == null)
                {
                    Interlocked.CompareExchange(ref _lazyElementType, _underlying.ElementTypeWithAnnotations.GetPublicSymbol(), null);
                }
                return _lazyElementType;
            }
        }

        Microsoft.CodeAnalysis.NullableAnnotation IArrayTypeSymbol.ElementNullableAnnotation => _underlying.ElementTypeWithAnnotations.ToPublicAnnotation();

        ImmutableArray<CustomModifier> IArrayTypeSymbol.CustomModifiers => _underlying.ElementTypeWithAnnotations.CustomModifiers;

        public ArrayTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.ArrayTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new ArrayTypeSymbol(_underlying, nullableAnnotation);
        }

        bool IArrayTypeSymbol.Equals(IArrayTypeSymbol? other)
        {
            return Equals(other as ArrayTypeSymbol, Microsoft.CodeAnalysis.SymbolEqualityComparer.Default);
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayType(this);
    }
}
