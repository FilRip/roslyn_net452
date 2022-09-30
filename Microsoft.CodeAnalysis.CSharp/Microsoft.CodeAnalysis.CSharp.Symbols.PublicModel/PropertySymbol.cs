using System;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class PropertySymbol : Symbol, IPropertySymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol _underlying;

        private ITypeSymbol _lazyType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        bool IPropertySymbol.IsIndexer => _underlying.IsIndexer;

        ITypeSymbol IPropertySymbol.Type
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

        Microsoft.CodeAnalysis.NullableAnnotation IPropertySymbol.NullableAnnotation => _underlying.TypeWithAnnotations.ToPublicAnnotation();

        ImmutableArray<IParameterSymbol> IPropertySymbol.Parameters => _underlying.Parameters.GetPublicSymbols();

        IMethodSymbol IPropertySymbol.GetMethod => _underlying.GetMethod.GetPublicSymbol();

        IMethodSymbol IPropertySymbol.SetMethod => _underlying.SetMethod.GetPublicSymbol();

        IPropertySymbol IPropertySymbol.OriginalDefinition => _underlying.OriginalDefinition.GetPublicSymbol();

        IPropertySymbol IPropertySymbol.OverriddenProperty => _underlying.OverriddenProperty.GetPublicSymbol();

        ImmutableArray<IPropertySymbol> IPropertySymbol.ExplicitInterfaceImplementations => _underlying.ExplicitInterfaceImplementations.GetPublicSymbols();

        bool IPropertySymbol.IsReadOnly => _underlying.IsReadOnly;

        bool IPropertySymbol.IsWriteOnly => _underlying.IsWriteOnly;

        bool IPropertySymbol.IsWithEvents => false;

        ImmutableArray<CustomModifier> IPropertySymbol.TypeCustomModifiers => _underlying.TypeWithAnnotations.CustomModifiers;

        ImmutableArray<CustomModifier> IPropertySymbol.RefCustomModifiers => _underlying.RefCustomModifiers;

        bool IPropertySymbol.ReturnsByRef => _underlying.ReturnsByRef;

        bool IPropertySymbol.ReturnsByRefReadonly => _underlying.ReturnsByRefReadonly;

        RefKind IPropertySymbol.RefKind => _underlying.RefKind;

        public PropertySymbol(Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol underlying)
        {
            _underlying = underlying;
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitProperty(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}
