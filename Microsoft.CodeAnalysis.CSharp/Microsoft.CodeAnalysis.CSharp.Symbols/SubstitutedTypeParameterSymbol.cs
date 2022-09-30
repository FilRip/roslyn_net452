using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class SubstitutedTypeParameterSymbol : WrappedTypeParameterSymbol
    {
        private readonly Symbol _container;

        private readonly TypeMap _map;

        private readonly int _ordinal;

        public override Symbol ContainingSymbol => _container;

        public override TypeParameterSymbol OriginalDefinition
        {
            get
            {
                if (!(ContainingSymbol.OriginalDefinition != _underlyingTypeParameter.ContainingSymbol.OriginalDefinition))
                {
                    return _underlyingTypeParameter.OriginalDefinition;
                }
                return this;
            }
        }

        public override TypeParameterSymbol ReducedFrom
        {
            get
            {
                if (_container.Kind == SymbolKind.Method)
                {
                    MethodSymbol reducedFrom = ((MethodSymbol)_container).ReducedFrom;
                    if ((object)reducedFrom != null)
                    {
                        return reducedFrom.TypeParameters[Ordinal];
                    }
                }
                return null;
            }
        }

        public override int Ordinal => _ordinal;

        public override string Name => base.Name;

        internal override bool? IsNotNullable
        {
            get
            {
                if (_underlyingTypeParameter.ConstraintTypesNoUseSiteDiagnostics.IsEmpty)
                {
                    return _underlyingTypeParameter.IsNotNullable;
                }
                if (!HasNotNullConstraint && !HasValueTypeConstraint && !HasReferenceTypeConstraint)
                {
                    ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
                    _map.SubstituteConstraintTypesDistinctWithoutModifiers(_underlyingTypeParameter, _underlyingTypeParameter.GetConstraintTypes(ConsList<TypeParameterSymbol>.Empty), instance, null);
                    return TypeParameterSymbol.IsNotNullableFromConstraintTypes(instance.ToImmutableAndFree());
                }
                return CalculateIsNotNullable();
            }
        }

        internal SubstitutedTypeParameterSymbol(Symbol newContainer, TypeMap map, TypeParameterSymbol substitutedFrom, int ordinal)
            : base(substitutedFrom)
        {
            _container = newContainer;
            _map = map;
            _ordinal = ordinal;
        }

        internal override ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            _map.SubstituteConstraintTypesDistinctWithoutModifiers(_underlyingTypeParameter, _underlyingTypeParameter.GetConstraintTypes(inProgress), instance, null);
            TypeWithAnnotations bestObjectConstraint = default(TypeWithAnnotations);
            for (int num = instance.Count - 1; num >= 0; num--)
            {
                if (ConstraintsHelper.IsObjectConstraint(instance[num], ref bestObjectConstraint))
                {
                    instance.RemoveAt(num);
                }
            }
            if (bestObjectConstraint.HasType && ConstraintsHelper.IsObjectConstraintSignificant(CalculateIsNotNullableFromNonTypeConstraints(), bestObjectConstraint))
            {
                if (instance.Count == 0)
                {
                    if (bestObjectConstraint.NullableAnnotation.IsOblivious() && !HasReferenceTypeConstraint)
                    {
                        bestObjectConstraint = default(TypeWithAnnotations);
                    }
                }
                else
                {
                    ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator = instance.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (!ConstraintsHelper.IsObjectConstraintSignificant(TypeParameterSymbol.IsNotNullableFromConstraintType(enumerator.Current, out var _), bestObjectConstraint))
                        {
                            bestObjectConstraint = default(TypeWithAnnotations);
                            break;
                        }
                    }
                }
                if (bestObjectConstraint.HasType)
                {
                    instance.Insert(0, bestObjectConstraint);
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
        {
            return _map.SubstituteNamedTypes(_underlyingTypeParameter.GetInterfaces(inProgress));
        }

        internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
        {
            return _map.SubstituteNamedType(_underlyingTypeParameter.GetEffectiveBaseClass(inProgress));
        }

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
        {
            return _map.SubstituteType(_underlyingTypeParameter.GetDeducedBaseType(inProgress)).AsTypeSymbolOnly();
        }
    }
}
