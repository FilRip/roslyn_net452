using System;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class LocalSymbol : Symbol, ILocalSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol _underlying;

        private ITypeSymbol _lazyType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        ITypeSymbol ILocalSymbol.Type
        {
            get
            {
                if (_lazyType == null)
                {
                    Interlocked.CompareExchange(ref _lazyType, _underlying.TypeWithAnnotations.GetPublicSymbol(), null);
                }
                return _lazyType;
            }
        }

        Microsoft.CodeAnalysis.NullableAnnotation ILocalSymbol.NullableAnnotation => _underlying.TypeWithAnnotations.ToPublicAnnotation();

        bool ILocalSymbol.IsFunctionValue => false;

        bool ILocalSymbol.IsConst => _underlying.IsConst;

        bool ILocalSymbol.IsRef => _underlying.IsRef;

        RefKind ILocalSymbol.RefKind => _underlying.RefKind;

        bool ILocalSymbol.HasConstantValue => _underlying.HasConstantValue;

        object ILocalSymbol.ConstantValue => _underlying.ConstantValue;

        bool ILocalSymbol.IsFixed => _underlying.IsFixed;

        public LocalSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol underlying)
        {
            _underlying = underlying;
        }

        protected sealed override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitLocal(this);
        }

        protected sealed override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitLocal(this);
        }
    }
}
