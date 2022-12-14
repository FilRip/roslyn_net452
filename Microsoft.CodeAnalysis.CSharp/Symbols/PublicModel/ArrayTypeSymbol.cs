// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class ArrayTypeSymbol : TypeSymbol, IArrayTypeSymbol
    {
        private readonly Symbols.ArrayTypeSymbol _underlying;
        private ITypeSymbol? _lazyElementType;

        public ArrayTypeSymbol(Symbols.ArrayTypeSymbol underlying, CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new ArrayTypeSymbol(_underlying, nullableAnnotation);
        }

        internal override CSharp.Symbol UnderlyingSymbol => _underlying;
        internal override Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;
        internal override Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        int IArrayTypeSymbol.Rank => _underlying.Rank;

        bool IArrayTypeSymbol.IsSZArray => _underlying.IsSZArray;

        ImmutableArray<int> IArrayTypeSymbol.LowerBounds => _underlying.LowerBounds;

        ImmutableArray<int> IArrayTypeSymbol.Sizes => _underlying.Sizes;

        ITypeSymbol IArrayTypeSymbol.ElementType
        {
            get
            {
                if (_lazyElementType is null)
                {
                    Interlocked.CompareExchange(ref _lazyElementType, _underlying.ElementTypeWithAnnotations.GetPublicSymbol(), null);
                }

                return _lazyElementType;
            }
        }

        CodeAnalysis.NullableAnnotation IArrayTypeSymbol.ElementNullableAnnotation
        {
            get
            {
                return _underlying.ElementTypeWithAnnotations.ToPublicAnnotation();
            }
        }

        ImmutableArray<CustomModifier> IArrayTypeSymbol.CustomModifiers => _underlying.ElementTypeWithAnnotations.CustomModifiers;

        bool IArrayTypeSymbol.Equals(IArrayTypeSymbol? other)
        {
            return this.Equals(other as ArrayTypeSymbol, CodeAnalysis.SymbolEqualityComparer.Default);
        }

        #region ISymbol Members

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor)
            where TResult : default
        {
            return visitor.VisitArrayType(this);
        }

        #endregion
    }
}
