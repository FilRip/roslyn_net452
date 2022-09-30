using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class BestTypeInferrer
    {
        public static NullableAnnotation GetNullableAnnotation(ArrayBuilder<TypeWithAnnotations> types)
        {
            NullableAnnotation nullableAnnotation = NullableAnnotation.NotAnnotated;
            ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                nullableAnnotation = nullableAnnotation.Join(enumerator.Current.NullableAnnotation);
            }
            return nullableAnnotation;
        }

        public static NullableFlowState GetNullableState(ArrayBuilder<TypeWithState> types)
        {
            NullableFlowState nullableFlowState = NullableFlowState.NotNull;
            ArrayBuilder<TypeWithState>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                nullableFlowState = nullableFlowState.Join(enumerator.Current.State);
            }
            return nullableFlowState;
        }

        public static TypeSymbol? InferBestType(ImmutableArray<BoundExpression> exprs, ConversionsBase conversions, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            HashSet<TypeSymbol> hashSet = new HashSet<TypeSymbol>(conversions.IncludeNullability ? Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything : Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.IgnoringNullable);
            ImmutableArray<BoundExpression>.Enumerator enumerator = exprs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeSymbol type = enumerator.Current.Type;
                if ((object)type != null)
                {
                    if (type.IsErrorType())
                    {
                        return type;
                    }
                    hashSet.Add(type);
                }
            }
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(hashSet.Count);
            instance.AddRange(hashSet);
            TypeSymbol bestType = GetBestType(instance, conversions, ref useSiteInfo);
            instance.Free();
            return bestType;
        }

        public static TypeSymbol? InferBestTypeForConditionalOperator(BoundExpression expr1, BoundExpression expr2, ConversionsBase conversions, out bool hadMultipleCandidates, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
            try
            {
                ConversionsBase conversionsBase = conversions.WithNullability(includeNullability: false);
                TypeSymbol type = expr1.Type;
                if ((object)type != null)
                {
                    if (type.IsErrorType())
                    {
                        hadMultipleCandidates = false;
                        return type;
                    }
                    if (conversionsBase.ClassifyImplicitConversionFromExpression(expr2, type, ref useSiteInfo).Exists)
                    {
                        instance.Add(type);
                    }
                }
                TypeSymbol type2 = expr2.Type;
                if ((object)type2 != null)
                {
                    if (type2.IsErrorType())
                    {
                        hadMultipleCandidates = false;
                        return type2;
                    }
                    if (conversionsBase.ClassifyImplicitConversionFromExpression(expr1, type2, ref useSiteInfo).Exists)
                    {
                        instance.Add(type2);
                    }
                }
                hadMultipleCandidates = instance.Count > 1;
                return GetBestType(instance, conversions, ref useSiteInfo);
            }
            finally
            {
                instance.Free();
            }
        }

        internal static TypeSymbol? GetBestType(ArrayBuilder<TypeSymbol> types, ConversionsBase conversions, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            switch (types.Count)
            {
                case 0:
                    return null;
                case 1:
                    return types[0];
                default:
                    {
                        TypeSymbol typeSymbol = null;
                        int num = -1;
                        for (int i = 0; i < types.Count; i++)
                        {
                            TypeSymbol typeSymbol2 = types[i];
                            if ((object)typeSymbol == null)
                            {
                                typeSymbol = typeSymbol2;
                                num = i;
                                continue;
                            }
                            TypeSymbol typeSymbol3 = Better(typeSymbol, typeSymbol2, conversions, ref useSiteInfo);
                            if ((object)typeSymbol3 == null)
                            {
                                typeSymbol = null;
                                continue;
                            }
                            typeSymbol = typeSymbol3;
                            num = i;
                        }
                        if ((object)typeSymbol == null)
                        {
                            return null;
                        }
                        for (int j = 0; j < num; j++)
                        {
                            TypeSymbol type = types[j];
                            TypeSymbol t = Better(typeSymbol, type, conversions, ref useSiteInfo);
                            if (!typeSymbol.Equals(t, TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                            {
                                return null;
                            }
                        }
                        return typeSymbol;
                    }
            }
        }

        private static TypeSymbol? Better(TypeSymbol type1, TypeSymbol? type2, ConversionsBase conversions, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (type1.IsErrorType())
            {
                return type2;
            }
            if ((object)type2 == null || type2.IsErrorType())
            {
                return type1;
            }
            ConversionsBase conversionsBase = conversions.WithNullability(includeNullability: false);
            bool exists = conversionsBase.ClassifyImplicitConversionFromType(type1, type2, ref useSiteInfo).Exists;
            bool exists2 = conversionsBase.ClassifyImplicitConversionFromType(type2, type1, ref useSiteInfo).Exists;
            if (exists && exists2)
            {
                if (type1.Equals(type2, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                {
                    return type1.MergeEquivalentTypes(type2, VarianceKind.Out);
                }
                return null;
            }
            if (exists)
            {
                return type2;
            }
            if (exists2)
            {
                return type1;
            }
            return null;
        }
    }
}
