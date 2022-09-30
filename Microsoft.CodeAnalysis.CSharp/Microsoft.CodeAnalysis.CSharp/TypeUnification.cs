using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class TypeUnification
    {
        public static bool CanUnify(TypeSymbol t1, TypeSymbol t2)
        {
            if (TypeSymbol.Equals(t1, t2, TypeCompareKind.CLRSignatureCompareOptions))
            {
                return true;
            }
            MutableTypeMap substitution = null;
            return CanUnifyHelper(t1, t2, ref substitution);
        }

        private static bool CanUnifyHelper(TypeSymbol t1, TypeSymbol t2, ref MutableTypeMap? substitution)
        {
            return CanUnifyHelper(TypeWithAnnotations.Create(t1), TypeWithAnnotations.Create(t2), ref substitution);
        }

        private static bool CanUnifyHelper(TypeWithAnnotations t1, TypeWithAnnotations t2, ref MutableTypeMap? substitution)
        {
            if (!t1.HasType || !t2.HasType)
            {
                return t1.IsSameAs(t2);
            }
            if (substitution != null)
            {
                t1 = t1.SubstituteType(substitution);
                t2 = t2.SubstituteType(substitution);
            }
            if (TypeSymbol.Equals(t1.Type, t2.Type, TypeCompareKind.CLRSignatureCompareOptions) && t1.CustomModifiers.SequenceEqual(t2.CustomModifiers))
            {
                return true;
            }
            if (!t1.Type.IsTypeParameter() && t2.Type.IsTypeParameter())
            {
                TypeWithAnnotations typeWithAnnotations = t1;
                t1 = t2;
                t2 = typeWithAnnotations;
            }
            switch (t1.Type.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        if (t2.TypeKind != t1.TypeKind || !t2.CustomModifiers.SequenceEqual(t1.CustomModifiers))
                        {
                            return false;
                        }
                        ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)t1.Type;
                        ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)t2.Type;
                        if (!arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
                        {
                            return false;
                        }
                        return CanUnifyHelper(arrayTypeSymbol.ElementTypeWithAnnotations, arrayTypeSymbol2.ElementTypeWithAnnotations, ref substitution);
                    }
                case SymbolKind.PointerType:
                    {
                        if (t2.TypeKind != t1.TypeKind || !t2.CustomModifiers.SequenceEqual(t1.CustomModifiers))
                        {
                            return false;
                        }
                        PointerTypeSymbol obj = (PointerTypeSymbol)t1.Type;
                        return CanUnifyHelper(t2: ((PointerTypeSymbol)t2.Type).PointedAtTypeWithAnnotations, t1: obj.PointedAtTypeWithAnnotations, substitution: ref substitution);
                    }
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    {
                        if (t2.TypeKind != t1.TypeKind || !t2.CustomModifiers.SequenceEqual(t1.CustomModifiers))
                        {
                            return false;
                        }
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)t1.Type;
                        NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)t2.Type;
                        if (!namedTypeSymbol.IsGenericType || !namedTypeSymbol2.IsGenericType)
                        {
                            return false;
                        }
                        int arity = namedTypeSymbol.Arity;
                        if (namedTypeSymbol2.Arity != arity || !TypeSymbol.Equals(namedTypeSymbol2.OriginalDefinition, namedTypeSymbol.OriginalDefinition, TypeCompareKind.ConsiderEverything))
                        {
                            return false;
                        }
                        ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                        ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics2 = namedTypeSymbol2.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                        for (int i = 0; i < arity; i++)
                        {
                            if (!CanUnifyHelper(typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i], typeArgumentsWithAnnotationsNoUseSiteDiagnostics2[i], ref substitution))
                            {
                                return false;
                            }
                        }
                        if ((object)namedTypeSymbol.ContainingType != null)
                        {
                            return CanUnifyHelper(namedTypeSymbol.ContainingType, namedTypeSymbol2.ContainingType, ref substitution);
                        }
                        return true;
                    }
                case SymbolKind.TypeParameter:
                    {
                        if (t2.Type.IsPointerOrFunctionPointer() || t2.IsVoidType())
                        {
                            return false;
                        }
                        TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)t1.Type;
                        if (Contains(t2.Type, typeParameterSymbol))
                        {
                            return false;
                        }
                        if (t1.CustomModifiers.IsDefaultOrEmpty)
                        {
                            AddSubstitution(ref substitution, typeParameterSymbol, t2);
                            return true;
                        }
                        if (t1.CustomModifiers.SequenceEqual(t2.CustomModifiers))
                        {
                            AddSubstitution(ref substitution, typeParameterSymbol, TypeWithAnnotations.Create(t2.Type));
                            return true;
                        }
                        if (t1.CustomModifiers.Length < t2.CustomModifiers.Length && t1.CustomModifiers.SequenceEqual(t2.CustomModifiers.Take(t1.CustomModifiers.Length)))
                        {
                            AddSubstitution(ref substitution, typeParameterSymbol, TypeWithAnnotations.Create(t2.Type, NullableAnnotation.Oblivious, ImmutableArray.Create(t2.CustomModifiers, t1.CustomModifiers.Length, t2.CustomModifiers.Length - t1.CustomModifiers.Length)));
                            return true;
                        }
                        if (t2.Type.IsTypeParameter())
                        {
                            TypeParameterSymbol tp = (TypeParameterSymbol)t2.Type;
                            if (t2.CustomModifiers.IsDefaultOrEmpty)
                            {
                                AddSubstitution(ref substitution, tp, t1);
                                return true;
                            }
                            if (t2.CustomModifiers.Length < t1.CustomModifiers.Length && t2.CustomModifiers.SequenceEqual(t1.CustomModifiers.Take(t2.CustomModifiers.Length)))
                            {
                                AddSubstitution(ref substitution, tp, TypeWithAnnotations.Create(t1.Type, NullableAnnotation.Oblivious, ImmutableArray.Create(t1.CustomModifiers, t2.CustomModifiers.Length, t1.CustomModifiers.Length - t2.CustomModifiers.Length)));
                                return true;
                            }
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        private static void AddSubstitution(ref MutableTypeMap? substitution, TypeParameterSymbol tp1, TypeWithAnnotations t2)
        {
            if (substitution == null)
            {
                substitution = new MutableTypeMap();
            }
            substitution!.Add(tp1, t2);
        }

        private static bool Contains(TypeSymbol type, TypeParameterSymbol typeParam)
        {
            switch (type.Kind)
            {
                case SymbolKind.ArrayType:
                    return Contains(((ArrayTypeSymbol)type).ElementType, typeParam);
                case SymbolKind.PointerType:
                    return Contains(((PointerTypeSymbol)type).PointedAtType, typeParam);
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    {
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                        while ((object)namedTypeSymbol != null)
                        {
                            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = (namedTypeSymbol.IsTupleType ? namedTypeSymbol.TupleElementTypesWithAnnotations : namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (Contains(enumerator.Current.Type, typeParam))
                                {
                                    return true;
                                }
                            }
                            namedTypeSymbol = namedTypeSymbol.ContainingType;
                        }
                        return false;
                    }
                case SymbolKind.TypeParameter:
                    return TypeSymbol.Equals(type, typeParam, TypeCompareKind.ConsiderEverything);
                default:
                    return false;
            }
        }
    }
}
