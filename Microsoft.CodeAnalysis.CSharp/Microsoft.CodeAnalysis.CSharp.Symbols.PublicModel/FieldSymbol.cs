using System;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class FieldSymbol : Symbol, IFieldSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol _underlying;

        private ITypeSymbol _lazyType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        ISymbol IFieldSymbol.AssociatedSymbol => _underlying.AssociatedSymbol.GetPublicSymbol();

        ITypeSymbol IFieldSymbol.Type
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

        Microsoft.CodeAnalysis.NullableAnnotation IFieldSymbol.NullableAnnotation => _underlying.TypeWithAnnotations.ToPublicAnnotation();

        ImmutableArray<CustomModifier> IFieldSymbol.CustomModifiers => _underlying.TypeWithAnnotations.CustomModifiers;

        IFieldSymbol IFieldSymbol.OriginalDefinition => _underlying.OriginalDefinition.GetPublicSymbol();

        IFieldSymbol IFieldSymbol.CorrespondingTupleField => _underlying.CorrespondingTupleField.GetPublicSymbol();

        bool IFieldSymbol.IsExplicitlyNamedTupleElement => _underlying.IsExplicitlyNamedTupleElement;

        bool IFieldSymbol.IsConst => _underlying.IsConst;

        bool IFieldSymbol.IsReadOnly => _underlying.IsReadOnly;

        bool IFieldSymbol.IsVolatile => _underlying.IsVolatile;

        bool IFieldSymbol.IsFixedSizeBuffer => _underlying.IsFixedSizeBuffer;

        bool IFieldSymbol.HasConstantValue => _underlying.HasConstantValue;

        object IFieldSymbol.ConstantValue => _underlying.ConstantValue;

        public FieldSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol underlying)
        {
            _underlying = underlying;
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitField(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitField(this);
        }
    }
}
