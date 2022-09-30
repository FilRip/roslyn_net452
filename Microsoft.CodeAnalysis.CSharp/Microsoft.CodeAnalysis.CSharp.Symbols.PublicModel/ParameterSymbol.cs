using System;
using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class ParameterSymbol : Symbol, IParameterSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol _underlying;

        private ITypeSymbol _lazyType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        ITypeSymbol IParameterSymbol.Type
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

        Microsoft.CodeAnalysis.NullableAnnotation IParameterSymbol.NullableAnnotation => _underlying.TypeWithAnnotations.ToPublicAnnotation();

        ImmutableArray<CustomModifier> IParameterSymbol.CustomModifiers => _underlying.TypeWithAnnotations.CustomModifiers;

        ImmutableArray<CustomModifier> IParameterSymbol.RefCustomModifiers => _underlying.RefCustomModifiers;

        IParameterSymbol IParameterSymbol.OriginalDefinition => _underlying.OriginalDefinition.GetPublicSymbol();

        RefKind IParameterSymbol.RefKind => _underlying.RefKind;

        bool IParameterSymbol.IsDiscard => _underlying.IsDiscard;

        bool IParameterSymbol.IsParams => _underlying.IsParams;

        bool IParameterSymbol.IsOptional => _underlying.IsOptional;

        bool IParameterSymbol.IsThis => _underlying.IsThis;

        int IParameterSymbol.Ordinal => _underlying.Ordinal;

        bool IParameterSymbol.HasExplicitDefaultValue => _underlying.HasExplicitDefaultValue;

        object IParameterSymbol.ExplicitDefaultValue => _underlying.ExplicitDefaultValue;

        public ParameterSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol underlying)
        {
            _underlying = underlying;
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}
