using System.Collections.Immutable;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class NativeIntegerParameterSymbol : WrappedParameterSymbol, IReference
    {
        private readonly NativeIntegerTypeSymbol _containingType;

        private readonly NativeIntegerMethodSymbol _container;

        public override Symbol ContainingSymbol => _container;

        public override TypeWithAnnotations TypeWithAnnotations => _containingType.SubstituteUnderlyingType(_underlyingParameter.TypeWithAnnotations);

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _underlyingParameter.RefCustomModifiers;

        internal NativeIntegerParameterSymbol(NativeIntegerTypeSymbol containingType, NativeIntegerMethodSymbol container, ParameterSymbol underlyingParameter)
            : base(underlyingParameter)
        {
            _containingType = containingType;
            _container = container;
        }

        public override bool Equals(Symbol? other, TypeCompareKind comparison)
        {
            return NativeIntegerTypeSymbol.EqualsHelper(this, other, comparison, (NativeIntegerParameterSymbol symbol) => symbol._underlyingParameter);
        }

        public override int GetHashCode()
        {
            return _underlyingParameter.GetHashCode();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
