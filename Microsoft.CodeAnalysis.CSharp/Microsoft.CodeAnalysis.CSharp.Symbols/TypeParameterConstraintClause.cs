using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class TypeParameterConstraintClause
    {
        internal static readonly TypeParameterConstraintClause Empty = new TypeParameterConstraintClause(TypeParameterConstraintKind.None, ImmutableArray<TypeWithAnnotations>.Empty);

        internal static readonly TypeParameterConstraintClause ObliviousNullabilityIfReferenceType = new TypeParameterConstraintClause(TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType, ImmutableArray<TypeWithAnnotations>.Empty);

        public readonly TypeParameterConstraintKind Constraints;

        public readonly ImmutableArray<TypeWithAnnotations> ConstraintTypes;

        internal static TypeParameterConstraintClause Create(TypeParameterConstraintKind constraints, ImmutableArray<TypeWithAnnotations> constraintTypes)
        {
            if (constraintTypes.IsEmpty)
            {
                switch (constraints)
                {
                    case TypeParameterConstraintKind.None:
                        return Empty;
                    case TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType:
                        return ObliviousNullabilityIfReferenceType;
                }
            }
            return new TypeParameterConstraintClause(constraints, constraintTypes);
        }

        private TypeParameterConstraintClause(TypeParameterConstraintKind constraints, ImmutableArray<TypeWithAnnotations> constraintTypes)
        {
            Constraints = constraints;
            ConstraintTypes = constraintTypes;
        }

        internal static SmallDictionary<TypeParameterSymbol, bool> BuildIsValueTypeMap(Symbol container, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeParameterConstraintClause> constraintClauses)
        {
            SmallDictionary<TypeParameterSymbol, bool> smallDictionary = new SmallDictionary<TypeParameterSymbol, bool>(ReferenceEqualityComparer.Instance);
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                isValueType(enumerator.Current, constraintClauses, smallDictionary, ConsList<TypeParameterSymbol>.Empty);
            }
            return smallDictionary;
            static bool isValueType(TypeParameterSymbol thisTypeParameter, ImmutableArray<TypeParameterConstraintClause> constraintClauses, SmallDictionary<TypeParameterSymbol, bool> isValueTypeMap, ConsList<TypeParameterSymbol> inProgress)
            {
                if (inProgress.ContainsReference(thisTypeParameter))
                {
                    return false;
                }
                if (isValueTypeMap.TryGetValue(thisTypeParameter, out var value))
                {
                    return value;
                }
                TypeParameterConstraintClause typeParameterConstraintClause = constraintClauses[thisTypeParameter.Ordinal];
                bool flag = false;
                if ((typeParameterConstraintClause.Constraints & TypeParameterConstraintKind.AllValueTypeKinds) != 0)
                {
                    flag = true;
                }
                else
                {
                    Symbol containingSymbol = thisTypeParameter.ContainingSymbol;
                    inProgress = inProgress.Prepend(thisTypeParameter);
                    ImmutableArray<TypeWithAnnotations>.Enumerator enumerator2 = typeParameterConstraintClause.ConstraintTypes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        TypeWithAnnotations current = enumerator2.Current;
                        TypeSymbol typeSymbol = (current.IsResolved ? current.Type : current.DefaultType);
                        if (typeSymbol is TypeParameterSymbol typeParameterSymbol && (object)typeParameterSymbol.ContainingSymbol == containingSymbol)
                        {
                            if (isValueType(typeParameterSymbol, constraintClauses, isValueTypeMap, inProgress))
                            {
                                flag = true;
                                break;
                            }
                        }
                        else if (typeSymbol.IsValueType)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                isValueTypeMap.Add(thisTypeParameter, flag);
                return flag;
            }
        }

        internal static SmallDictionary<TypeParameterSymbol, bool> BuildIsReferenceTypeFromConstraintTypesMap(Symbol container, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeParameterConstraintClause> constraintClauses)
        {
            SmallDictionary<TypeParameterSymbol, bool> smallDictionary = new SmallDictionary<TypeParameterSymbol, bool>(ReferenceEqualityComparer.Instance);
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                isReferenceTypeFromConstraintTypes(enumerator.Current, constraintClauses, smallDictionary, ConsList<TypeParameterSymbol>.Empty);
            }
            return smallDictionary;
            static bool isReferenceTypeFromConstraintTypes(TypeParameterSymbol thisTypeParameter, ImmutableArray<TypeParameterConstraintClause> constraintClauses, SmallDictionary<TypeParameterSymbol, bool> isReferenceTypeFromConstraintTypesMap, ConsList<TypeParameterSymbol> inProgress)
            {
                if (inProgress.ContainsReference(thisTypeParameter))
                {
                    return false;
                }
                if (isReferenceTypeFromConstraintTypesMap.TryGetValue(thisTypeParameter, out var value))
                {
                    return value;
                }
                TypeParameterConstraintClause typeParameterConstraintClause = constraintClauses[thisTypeParameter.Ordinal];
                bool flag = false;
                Symbol containingSymbol = thisTypeParameter.ContainingSymbol;
                inProgress = inProgress.Prepend(thisTypeParameter);
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator2 = typeParameterConstraintClause.ConstraintTypes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeWithAnnotations current = enumerator2.Current;
                    TypeSymbol typeSymbol = (current.IsResolved ? current.Type : current.DefaultType);
                    if (typeSymbol is TypeParameterSymbol typeParameterSymbol)
                    {
                        if ((object)typeParameterSymbol.ContainingSymbol == containingSymbol)
                        {
                            if (isReferenceTypeFromConstraintTypes(typeParameterSymbol, constraintClauses, isReferenceTypeFromConstraintTypesMap, inProgress))
                            {
                                flag = true;
                                break;
                            }
                        }
                        else if (typeParameterSymbol.IsReferenceTypeFromConstraintTypes)
                        {
                            flag = true;
                            break;
                        }
                    }
                    else if (TypeParameterSymbol.NonTypeParameterConstraintImpliesReferenceType(typeSymbol))
                    {
                        flag = true;
                        break;
                    }
                }
                isReferenceTypeFromConstraintTypesMap.Add(thisTypeParameter, flag);
                return flag;
            }
        }
    }
}
