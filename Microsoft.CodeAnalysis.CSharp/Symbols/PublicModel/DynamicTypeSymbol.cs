// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class DynamicTypeSymbol : TypeSymbol, IDynamicTypeSymbol
    {
        private readonly Symbols.DynamicTypeSymbol _underlying;

        public DynamicTypeSymbol(Symbols.DynamicTypeSymbol underlying, CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new DynamicTypeSymbol(_underlying, nullableAnnotation);
        }

        internal override CSharp.Symbol UnderlyingSymbol => _underlying;
        internal override Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;
        internal override Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        #region ISymbol Members

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitDynamicType(this);
        }

        protected override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor)
            where TResult : default
        {
            return visitor.VisitDynamicType(this);
        }

        #endregion
    }
}
