using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class ConstructedErrorTypeSymbol : SubstitutedErrorTypeSymbol
    {
        private readonly ErrorTypeSymbol _constructedFrom;

        private readonly ImmutableArray<TypeWithAnnotations> _typeArgumentsWithAnnotations;

        private readonly TypeMap _map;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _constructedFrom.TypeParameters;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => _typeArgumentsWithAnnotations;

        public override NamedTypeSymbol ConstructedFrom => _constructedFrom;

        public override Symbol? ContainingSymbol => _constructedFrom.ContainingSymbol;

        internal override TypeMap TypeSubstitution => _map;

        public ConstructedErrorTypeSymbol(ErrorTypeSymbol constructedFrom, ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotations, TupleExtraData? tupleData = null)
            : base((ErrorTypeSymbol)constructedFrom.OriginalDefinition, tupleData)
        {
            _constructedFrom = constructedFrom;
            _typeArgumentsWithAnnotations = typeArgumentsWithAnnotations;
            _map = new TypeMap(constructedFrom.ContainingType, constructedFrom.OriginalDefinition.TypeParameters, typeArgumentsWithAnnotations);
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new ConstructedErrorTypeSymbol(_constructedFrom, _typeArgumentsWithAnnotations, newData);
        }
    }
}
