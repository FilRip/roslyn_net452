using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class ConstructedMethodSymbol : SubstitutedMethodSymbol
    {
        private readonly ImmutableArray<TypeWithAnnotations> _typeArgumentsWithAnnotations;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => _typeArgumentsWithAnnotations;

        internal ConstructedMethodSymbol(MethodSymbol constructedFrom, ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotations)
            : base(constructedFrom.ContainingType, new TypeMap(constructedFrom.ContainingType, constructedFrom.OriginalDefinition.TypeParameters, typeArgumentsWithAnnotations), constructedFrom.OriginalDefinition, constructedFrom)
        {
            _typeArgumentsWithAnnotations = typeArgumentsWithAnnotations;
        }
    }
}
