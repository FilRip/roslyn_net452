using System;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class PointerTypeSymbol : TypeSymbol, IPointerTypeSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.PointerTypeSymbol _underlying;

        private ITypeSymbol _lazyPointedAtType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        ITypeSymbol IPointerTypeSymbol.PointedAtType
        {
            get
            {
                if (_lazyPointedAtType == null)
                {
                    Interlocked.CompareExchange(ref _lazyPointedAtType, _underlying.PointedAtTypeWithAnnotations.GetPublicSymbol(), null);
                }
                return _lazyPointedAtType;
            }
        }

        ImmutableArray<CustomModifier> IPointerTypeSymbol.CustomModifiers => _underlying.PointedAtTypeWithAnnotations.CustomModifiers;

        public PointerTypeSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.PointerTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new PointerTypeSymbol(_underlying, nullableAnnotation);
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitPointerType(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitPointerType(this);
        }
    }
}
