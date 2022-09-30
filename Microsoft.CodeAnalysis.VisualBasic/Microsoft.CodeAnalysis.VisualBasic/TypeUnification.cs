using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class TypeUnification
	{
		public static bool CanUnify(NamedTypeSymbol containingGenericType, TypeSymbol t1, TypeSymbol t2)
		{
			if (!containingGenericType.IsGenericType)
			{
				return false;
			}
			if (TypeSymbol.Equals(t1, t2, TypeCompareKind.ConsiderEverything))
			{
				return true;
			}
			TypeSubstitution substitution = null;
			return CanUnifyHelper(containingGenericType, ((object)t1 == null) ? default(TypeWithModifiers) : new TypeWithModifiers(t1), ((object)t2 == null) ? default(TypeWithModifiers) : new TypeWithModifiers(t2), ref substitution);
		}

		private static bool CanUnifyHelper(NamedTypeSymbol containingGenericType, TypeWithModifiers t1, TypeWithModifiers t2, ref TypeSubstitution substitution)
		{
			if (t1 == t2)
			{
				return true;
			}
			if ((object)t1.Type == null || (object)t2.Type == null)
			{
				return false;
			}
			if (substitution != null)
			{
				t1 = t1.InternalSubstituteTypeParameters(substitution);
				t2 = t2.InternalSubstituteTypeParameters(substitution);
			}
			if (t1 == t2)
			{
				return true;
			}
			if (!TypeSymbolExtensions.IsTypeParameter(t1.Type) && TypeSymbolExtensions.IsTypeParameter(t2.Type))
			{
				TypeWithModifiers typeWithModifiers = t1;
				t1 = t2;
				t2 = typeWithModifiers;
			}
			switch (t1.Type.Kind)
			{
			case SymbolKind.ArrayType:
			{
				if (t2.Type.TypeKind != t1.Type.TypeKind || !t1.CustomModifiers.SequenceEqual(t2.CustomModifiers))
				{
					return false;
				}
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)t1.Type;
				ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)t2.Type;
				if (!arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
				{
					return false;
				}
				return CanUnifyHelper(containingGenericType, new TypeWithModifiers(arrayTypeSymbol.ElementType, arrayTypeSymbol.CustomModifiers), new TypeWithModifiers(arrayTypeSymbol2.ElementType, arrayTypeSymbol2.CustomModifiers), ref substitution);
			}
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
			{
				if (t2.Type.TypeKind != t1.Type.TypeKind || !t1.CustomModifiers.SequenceEqual(t2.CustomModifiers))
				{
					return false;
				}
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)t1.Type;
				NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)t2.Type;
				if (namedTypeSymbol.IsTupleType || namedTypeSymbol2.IsTupleType)
				{
					return CanUnifyHelper(containingGenericType, new TypeWithModifiers(TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(namedTypeSymbol)), new TypeWithModifiers(TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(namedTypeSymbol2)), ref substitution);
				}
				if (!namedTypeSymbol.IsGenericType)
				{
					return !namedTypeSymbol2.IsGenericType && TypeSymbol.Equals(namedTypeSymbol, namedTypeSymbol2, TypeCompareKind.ConsiderEverything);
				}
				if (!namedTypeSymbol2.IsGenericType)
				{
					return false;
				}
				int arity = namedTypeSymbol.Arity;
				if (namedTypeSymbol2.Arity != arity || !TypeSymbol.Equals(namedTypeSymbol2.OriginalDefinition, namedTypeSymbol.OriginalDefinition, TypeCompareKind.ConsiderEverything))
				{
					return false;
				}
				ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics;
				ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics2 = namedTypeSymbol2.TypeArgumentsNoUseSiteDiagnostics;
				bool hasTypeArgumentsCustomModifiers = namedTypeSymbol.HasTypeArgumentsCustomModifiers;
				bool hasTypeArgumentsCustomModifiers2 = namedTypeSymbol2.HasTypeArgumentsCustomModifiers;
				int num = arity - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!CanUnifyHelper(containingGenericType, new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics[i], hasTypeArgumentsCustomModifiers ? namedTypeSymbol.GetTypeArgumentCustomModifiers(i) : default(ImmutableArray<CustomModifier>)), new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics2[i], hasTypeArgumentsCustomModifiers2 ? namedTypeSymbol2.GetTypeArgumentCustomModifiers(i) : default(ImmutableArray<CustomModifier>)), ref substitution))
					{
						return false;
					}
				}
				return (object)namedTypeSymbol.ContainingType == null || CanUnifyHelper(containingGenericType, new TypeWithModifiers(namedTypeSymbol.ContainingType), new TypeWithModifiers(namedTypeSymbol2.ContainingType), ref substitution);
			}
			case SymbolKind.TypeParameter:
			{
				if (t2.Type.SpecialType == SpecialType.System_Void)
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
					AddSubstitution(ref substitution, containingGenericType, typeParameterSymbol, t2);
					return true;
				}
				if (t1.CustomModifiers.SequenceEqual(t2.CustomModifiers))
				{
					AddSubstitution(ref substitution, containingGenericType, typeParameterSymbol, new TypeWithModifiers(t2.Type));
					return true;
				}
				if (t1.CustomModifiers.Length < t2.CustomModifiers.Length && t1.CustomModifiers.SequenceEqual(t2.CustomModifiers.Take(t1.CustomModifiers.Length)))
				{
					AddSubstitution(ref substitution, containingGenericType, typeParameterSymbol, new TypeWithModifiers(t2.Type, ImmutableArray.Create(t2.CustomModifiers, t1.CustomModifiers.Length, t2.CustomModifiers.Length - t1.CustomModifiers.Length)));
					return true;
				}
				if (TypeSymbolExtensions.IsTypeParameter(t2.Type))
				{
					TypeParameterSymbol tp = (TypeParameterSymbol)t2.Type;
					if (t2.CustomModifiers.IsDefaultOrEmpty)
					{
						AddSubstitution(ref substitution, containingGenericType, tp, t1);
						return true;
					}
					if (t2.CustomModifiers.Length < t1.CustomModifiers.Length && t2.CustomModifiers.SequenceEqual(t1.CustomModifiers.Take(t2.CustomModifiers.Length)))
					{
						AddSubstitution(ref substitution, containingGenericType, tp, new TypeWithModifiers(t1.Type, ImmutableArray.Create(t1.CustomModifiers, t2.CustomModifiers.Length, t1.CustomModifiers.Length - t2.CustomModifiers.Length)));
						return true;
					}
				}
				return false;
			}
			default:
				return t1 == t2;
			}
		}

		private static void AddSubstitution(ref TypeSubstitution substitution, NamedTypeSymbol targetGenericType, TypeParameterSymbol tp, TypeWithModifiers typeArgument)
		{
			if (substitution != null)
			{
				ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> instance = ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.GetInstance();
				instance.AddRange(substitution.PairsIncludingParent);
				int count = instance.Count;
				for (int i = 0; i <= count; i++)
				{
					if (i > instance.Count - 1 || TypeSymbolExtensions.IsSameOrNestedWithin(instance[i].Key.ContainingType, tp.ContainingType))
					{
						instance.Insert(i, new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(tp, typeArgument));
						break;
					}
				}
				int count2 = instance.Count;
				TypeParameterSymbol[] array = new TypeParameterSymbol[count2 - 1 + 1];
				TypeWithModifiers[] array2 = new TypeWithModifiers[count2 - 1 + 1];
				int num = count2 - 1;
				for (int j = 0; j <= num; j++)
				{
					array[j] = instance[j].Key;
					array2[j] = instance[j].Value;
				}
				instance.Free();
				substitution = TypeSubstitution.Create(targetGenericType, array, array2);
			}
			else
			{
				substitution = TypeSubstitution.Create(targetGenericType, new TypeParameterSymbol[1] { tp }, new TypeWithModifiers[1] { typeArgument });
			}
		}

		private static bool Contains(TypeSymbol type, TypeParameterSymbol typeParam)
		{
			switch (type.Kind)
			{
			case SymbolKind.ArrayType:
				return Contains(((ArrayTypeSymbol)type).ElementType, typeParam);
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
				while ((object)namedTypeSymbol != null)
				{
					ImmutableArray<TypeSymbol>.Enumerator enumerator = (namedTypeSymbol.IsTupleType ? namedTypeSymbol.TupleElementTypes : namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics).GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (Contains(enumerator.Current, typeParam))
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
