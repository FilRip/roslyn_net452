using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class TypeParameterBounds
    {
        public static readonly TypeParameterBounds Unset = new TypeParameterBounds();

        public readonly ImmutableArray<TypeWithAnnotations> ConstraintTypes;

        public readonly ImmutableArray<NamedTypeSymbol> Interfaces;

        public readonly NamedTypeSymbol EffectiveBaseClass;

        public readonly TypeSymbol DeducedBaseType;

        public TypeParameterBounds(ImmutableArray<TypeWithAnnotations> constraintTypes, ImmutableArray<NamedTypeSymbol> interfaces, NamedTypeSymbol effectiveBaseClass, TypeSymbol deducedBaseType)
        {
            ConstraintTypes = constraintTypes;
            Interfaces = interfaces;
            EffectiveBaseClass = effectiveBaseClass;
            DeducedBaseType = deducedBaseType;
        }

        private TypeParameterBounds()
        {
            EffectiveBaseClass = null;
            DeducedBaseType = null;
        }
    }
}
