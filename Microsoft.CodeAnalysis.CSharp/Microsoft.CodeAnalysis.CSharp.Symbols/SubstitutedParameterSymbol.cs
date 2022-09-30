using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SubstitutedParameterSymbol : WrappedParameterSymbol
    {
        private object _mapOrType;

        private readonly Symbol _containingSymbol;

        public override ParameterSymbol OriginalDefinition => _underlyingParameter.OriginalDefinition;

        public override Symbol ContainingSymbol => _containingSymbol;

        public override TypeWithAnnotations TypeWithAnnotations
        {
            get
            {
                object mapOrType = _mapOrType;
                if (mapOrType is TypeWithAnnotations)
                {
                    return (TypeWithAnnotations)mapOrType;
                }
                TypeWithAnnotations typeWithAnnotations = ((TypeMap)mapOrType).SubstituteType(_underlyingParameter.TypeWithAnnotations);
                if (typeWithAnnotations.CustomModifiers.IsEmpty && _underlyingParameter.TypeWithAnnotations.CustomModifiers.IsEmpty && _underlyingParameter.RefCustomModifiers.IsEmpty)
                {
                    _mapOrType = typeWithAnnotations;
                }
                return typeWithAnnotations;
            }
        }

        public override ImmutableArray<CustomModifier> RefCustomModifiers
        {
            get
            {
                if (!(_mapOrType is TypeMap typeMap))
                {
                    return _underlyingParameter.RefCustomModifiers;
                }
                return typeMap.SubstituteCustomModifiers(_underlyingParameter.RefCustomModifiers);
            }
        }

        internal SubstitutedParameterSymbol(MethodSymbol containingSymbol, TypeMap map, ParameterSymbol originalParameter)
            : this((Symbol)containingSymbol, map, originalParameter)
        {
        }

        internal SubstitutedParameterSymbol(PropertySymbol containingSymbol, TypeMap map, ParameterSymbol originalParameter)
            : this((Symbol)containingSymbol, map, originalParameter)
        {
        }

        private SubstitutedParameterSymbol(Symbol containingSymbol, TypeMap map, ParameterSymbol originalParameter)
            : base(originalParameter)
        {
            _containingSymbol = containingSymbol;
            _mapOrType = map;
        }

        public sealed override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)this == obj)
            {
                return true;
            }
            if (obj is SubstitutedParameterSymbol substitutedParameterSymbol && Ordinal == substitutedParameterSymbol.Ordinal)
            {
                return ContainingSymbol.Equals(substitutedParameterSymbol.ContainingSymbol, compareKind);
            }
            return false;
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(ContainingSymbol, _underlyingParameter.Ordinal);
        }
    }
}
