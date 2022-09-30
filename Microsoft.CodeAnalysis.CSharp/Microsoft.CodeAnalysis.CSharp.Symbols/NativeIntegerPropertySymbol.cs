using System;
using System.Collections.Immutable;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class NativeIntegerPropertySymbol : WrappedPropertySymbol, IReference
    {
        private readonly NativeIntegerTypeSymbol _container;

        public override Symbol ContainingSymbol => _container;

        public override TypeWithAnnotations TypeWithAnnotations => _container.SubstituteUnderlyingType(_underlyingProperty.TypeWithAnnotations);

        public override ImmutableArray<CustomModifier> RefCustomModifiers => base.UnderlyingProperty.RefCustomModifiers;

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

        public override MethodSymbol? GetMethod { get; }

        public override MethodSymbol? SetMethod { get; }

        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

        internal override bool MustCallMethodsDirectly => _underlyingProperty.MustCallMethodsDirectly;

        internal NativeIntegerPropertySymbol(NativeIntegerTypeSymbol container, PropertySymbol underlyingProperty, Func<NativeIntegerTypeSymbol, NativeIntegerPropertySymbol, MethodSymbol?, NativeIntegerMethodSymbol?> getAccessor)
            : base(underlyingProperty)
        {
            _container = container;
            GetMethod = getAccessor(container, this, underlyingProperty.GetMethod);
            SetMethod = getAccessor(container, this, underlyingProperty.SetMethod);
        }

        public override bool Equals(Symbol? other, TypeCompareKind comparison)
        {
            return NativeIntegerTypeSymbol.EqualsHelper(this, other, comparison, (NativeIntegerPropertySymbol symbol) => symbol._underlyingProperty);
        }

        public override int GetHashCode()
        {
            return _underlyingProperty.GetHashCode();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
