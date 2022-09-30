using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class TypeSymbolExtensions
	{
		private static readonly Func<TypeSymbol, HashSet<TypeParameterSymbol>, bool> s_addIfTypeParameterFunc = AddIfTypeParameter;

		private static readonly Func<TypeSymbol, HashSet<TypeParameterSymbol>, bool> s_isTypeParameterNotInSetFunc = IsTypeParameterNotInSet;

		private static readonly Func<TypeSymbol, MethodSymbol, bool> s_isMethodTypeParameterFunc = IsMethodTypeParameter;

		private static readonly Func<TypeSymbol, object, bool> s_isTypeParameterFunc = (TypeSymbol type, object arg) => type.TypeKind == TypeKind.TypeParameter;

		private static readonly Func<TypeSymbol, object, bool> s_isTupleTypeFunc = (TypeSymbol type, object arg) => type.IsTupleType;

		private static readonly Func<TypeSymbol, object, bool> s_hasTupleNamesFunc = (TypeSymbol type, object arg) => !type.TupleElementNames.IsDefault;

		public static bool IsNullableType(this TypeSymbol @this)
		{
			return @this.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
		}

		public static bool IsNullableOfBoolean(this TypeSymbol @this)
		{
			if (IsNullableType(@this))
			{
				return IsBooleanType(GetNullableUnderlyingType(@this));
			}
			return false;
		}

		public static TypeSymbol GetNullableUnderlyingType(this TypeSymbol type)
		{
			return ((NamedTypeSymbol)type).TypeArgumentsNoUseSiteDiagnostics[0];
		}

		public static TypeSymbol GetNullableUnderlyingTypeOrSelf(this TypeSymbol type)
		{
			if (IsNullableType(type))
			{
				return GetNullableUnderlyingType(type);
			}
			return type;
		}

		public static TypeSymbol GetEnumUnderlyingType(this TypeSymbol type)
		{
			return (type as NamedTypeSymbol)?.EnumUnderlyingType;
		}

		public static TypeSymbol GetEnumUnderlyingTypeOrSelf(this TypeSymbol type)
		{
			return GetEnumUnderlyingType(type) ?? type;
		}

		public static TypeSymbol GetTupleUnderlyingType(this TypeSymbol type)
		{
			return (type as NamedTypeSymbol)?.TupleUnderlyingType;
		}

		public static TypeSymbol GetTupleUnderlyingTypeOrSelf(this TypeSymbol type)
		{
			return GetTupleUnderlyingType(type) ?? type;
		}

		public static bool TryGetElementTypesIfTupleOrCompatible(this TypeSymbol type, out ImmutableArray<TypeSymbol> elementTypes)
		{
			if (type.IsTupleType)
			{
				elementTypes = ((TupleTypeSymbol)type).TupleElementTypes;
				return true;
			}
			if (!type.IsTupleCompatible(out var tupleCardinality))
			{
				elementTypes = default(ImmutableArray<TypeSymbol>);
				return false;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(tupleCardinality);
			TupleTypeSymbol.AddElementTypes((NamedTypeSymbol)type, instance);
			elementTypes = instance.ToImmutableAndFree();
			return true;
		}

		public static ImmutableArray<TypeSymbol> GetElementTypesOfTupleOrCompatible(this TypeSymbol Type)
		{
			if (Type.IsTupleType)
			{
				return ((TupleTypeSymbol)Type).TupleElementTypes;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
			TupleTypeSymbol.AddElementTypes((NamedTypeSymbol)Type, instance);
			return instance.ToImmutableAndFree();
		}

		internal static bool IsEnumType(this TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Enum;
		}

		internal static bool IsValidEnumUnderlyingType(this TypeSymbol type)
		{
			return type.SpecialType.IsValidEnumUnderlyingType();
		}

		internal static bool IsClassOrInterfaceType(this TypeSymbol type)
		{
			if (!IsClassType(type))
			{
				return IsInterfaceType(type);
			}
			return true;
		}

		internal static bool IsInterfaceType(this TypeSymbol type)
		{
			if (type.Kind == SymbolKind.NamedType)
			{
				return ((NamedTypeSymbol)type).IsInterface;
			}
			return false;
		}

		internal static bool IsClassType(this TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Class;
		}

		internal static bool IsStructureType(this TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Struct;
		}

		internal static bool IsModuleType(this TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Module;
		}

		internal static bool IsErrorType(this TypeSymbol type)
		{
			return type.Kind == SymbolKind.ErrorType;
		}

		internal static bool IsArrayType(this TypeSymbol type)
		{
			return type.Kind == SymbolKind.ArrayType;
		}

		internal static bool IsCharSZArray(this TypeSymbol type)
		{
			if (IsArrayType(type))
			{
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
				if (arrayTypeSymbol.IsSZArray && arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_Char)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsDBNullType(this TypeSymbol type)
		{
			if (type.SpecialType == SpecialType.None && type.Kind == SymbolKind.NamedType && string.Equals(type.Name, "DBNull", StringComparison.Ordinal) && HasNameQualifier((NamedTypeSymbol)type, "System", StringComparison.Ordinal))
			{
				return true;
			}
			return false;
		}

		internal static bool IsMicrosoftVisualBasicCollection(this TypeSymbol type)
		{
			if (type.SpecialType == SpecialType.None && type.Kind == SymbolKind.NamedType && string.Equals(type.Name, "Collection", StringComparison.Ordinal) && HasNameQualifier((NamedTypeSymbol)type, "Microsoft.VisualBasic", StringComparison.Ordinal))
			{
				return true;
			}
			return false;
		}

		internal static bool IsTypeParameter(this TypeSymbol type)
		{
			return type.Kind == SymbolKind.TypeParameter;
		}

		internal static bool IsDelegateType(this TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Delegate;
		}

		internal static bool IsSameTypeIgnoringAll(this TypeSymbol t1, TypeSymbol t2)
		{
			return IsSameType(t1, t2, TypeCompareKind.AllIgnoreOptionsForVB);
		}

		internal static bool IsSameType(this TypeSymbol t1, TypeSymbol t2, TypeCompareKind compareKind)
		{
			return TypeSymbol.Equals(t1, t2, compareKind);
		}

		internal static bool HasSameTypeArgumentCustomModifiers(NamedTypeSymbol type1, NamedTypeSymbol type2)
		{
			bool hasTypeArgumentsCustomModifiers = type1.HasTypeArgumentsCustomModifiers;
			bool hasTypeArgumentsCustomModifiers2 = type2.HasTypeArgumentsCustomModifiers;
			if (!hasTypeArgumentsCustomModifiers && !hasTypeArgumentsCustomModifiers2)
			{
				return true;
			}
			if (!hasTypeArgumentsCustomModifiers || !hasTypeArgumentsCustomModifiers2)
			{
				return false;
			}
			int num = type1.Arity - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!AreSameCustomModifiers(type1.GetTypeArgumentCustomModifiers(i), type2.GetTypeArgumentCustomModifiers(i)))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool AreSameCustomModifiers(this ImmutableArray<CustomModifier> mod, ImmutableArray<CustomModifier> otherMod)
		{
			if (mod.Length != otherMod.Length)
			{
				return false;
			}
			return mod.SequenceEqual(otherMod);
		}

		private static bool HasSameTupleNames(TypeSymbol t1, TypeSymbol t2)
		{
			ImmutableArray<string> tupleElementNames = t1.TupleElementNames;
			ImmutableArray<string> tupleElementNames2 = t2.TupleElementNames;
			if (tupleElementNames.IsDefault && tupleElementNames2.IsDefault)
			{
				return true;
			}
			if (tupleElementNames.IsDefault || tupleElementNames2.IsDefault)
			{
				return false;
			}
			return tupleElementNames.SequenceEqual(tupleElementNames2, CaseInsensitiveComparison.Equals);
		}

		public static SpecialType GetSpecialTypeSafe(this TypeSymbol @this)
		{
			return @this?.SpecialType ?? SpecialType.None;
		}

		public static bool IsNumericType(this TypeSymbol @this)
		{
			return @this.SpecialType.IsNumericType();
		}

		public static bool IsIntegralType(this TypeSymbol @this)
		{
			return @this.SpecialType.IsIntegralType();
		}

		public static bool IsUnsignedIntegralType(this TypeSymbol @this)
		{
			return @this.SpecialType.IsUnsignedIntegralType();
		}

		public static bool IsSignedIntegralType(this TypeSymbol @this)
		{
			return @this.SpecialType.IsSignedIntegralType();
		}

		public static bool IsFloatingType(this TypeSymbol @this)
		{
			return SpecialTypeExtensions.IsFloatingType(@this.SpecialType);
		}

		public static bool IsSingleType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Single;
		}

		public static bool IsDoubleType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Double;
		}

		public static bool IsBooleanType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Boolean;
		}

		public static bool IsCharType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Char;
		}

		public static bool IsStringType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_String;
		}

		public static bool IsObjectType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Object;
		}

		public static bool IsStrictSupertypeOfConcreteDelegate(this TypeSymbol @this)
		{
			return SpecialTypeExtensions.IsStrictSupertypeOfConcreteDelegate(@this.SpecialType);
		}

		public static bool IsVoidType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Void;
		}

		public static bool IsDecimalType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_Decimal;
		}

		public static bool IsDateTimeType(this TypeSymbol @this)
		{
			return @this.SpecialType == SpecialType.System_DateTime;
		}

		public static bool IsRestrictedType(this TypeSymbol @this)
		{
			return SpecialTypeExtensions.IsRestrictedType(@this.SpecialType);
		}

		public static bool IsRestrictedArrayType(this TypeSymbol @this, out TypeSymbol restrictedType)
		{
			if (@this.Kind == SymbolKind.ArrayType)
			{
				return IsRestrictedTypeOrArrayType(@this, out restrictedType);
			}
			restrictedType = null;
			return false;
		}

		public static bool IsRestrictedTypeOrArrayType(this TypeSymbol @this, out TypeSymbol restrictedType)
		{
			while (@this.Kind == SymbolKind.ArrayType)
			{
				@this = ((ArrayTypeSymbol)@this).ElementType;
			}
			if (IsRestrictedType(@this))
			{
				restrictedType = @this;
				return true;
			}
			restrictedType = null;
			return false;
		}

		public static bool IsIntrinsicType(this TypeSymbol @this)
		{
			return SpecialTypeExtensions.IsIntrinsicType(@this.SpecialType);
		}

		public static bool IsIntrinsicValueType(this TypeSymbol @this)
		{
			return SpecialTypeExtensions.IsIntrinsicValueType(@this.SpecialType);
		}

		public static bool IsNotInheritable(this TypeSymbol @this)
		{
			switch (@this.TypeKind)
			{
			case TypeKind.Array:
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Module:
			case TypeKind.Struct:
				return true;
			case TypeKind.Unknown:
			case TypeKind.Interface:
			case TypeKind.TypeParameter:
				return false;
			case TypeKind.Class:
			case TypeKind.Error:
			case TypeKind.Submission:
				return ((NamedTypeSymbol)@this).IsNotInheritable;
			default:
				throw ExceptionUtilities.UnexpectedValue(@this.TypeKind);
			}
		}

		public static ConstantValueTypeDiscriminator GetConstantValueTypeDiscriminator(this TypeSymbol @this)
		{
			if ((object)@this == null)
			{
				return ConstantValueTypeDiscriminator.Nothing;
			}
			@this = GetEnumUnderlyingTypeOrSelf(@this);
			switch (@this.SpecialType)
			{
			case SpecialType.System_Boolean:
				return ConstantValueTypeDiscriminator.Boolean;
			case SpecialType.System_Byte:
				return ConstantValueTypeDiscriminator.Byte;
			case SpecialType.System_SByte:
				return ConstantValueTypeDiscriminator.SByte;
			case SpecialType.System_Int16:
				return ConstantValueTypeDiscriminator.Int16;
			case SpecialType.System_UInt16:
				return ConstantValueTypeDiscriminator.UInt16;
			case SpecialType.System_Int32:
				return ConstantValueTypeDiscriminator.Int32;
			case SpecialType.System_UInt32:
				return ConstantValueTypeDiscriminator.UInt32;
			case SpecialType.System_Int64:
				return ConstantValueTypeDiscriminator.Int64;
			case SpecialType.System_UInt64:
				return ConstantValueTypeDiscriminator.UInt64;
			case SpecialType.System_Single:
				return ConstantValueTypeDiscriminator.Single;
			case SpecialType.System_Double:
				return ConstantValueTypeDiscriminator.Double;
			case SpecialType.System_Decimal:
				return ConstantValueTypeDiscriminator.Decimal;
			case SpecialType.System_DateTime:
				return ConstantValueTypeDiscriminator.DateTime;
			case SpecialType.System_Char:
				return ConstantValueTypeDiscriminator.Char;
			case SpecialType.System_String:
				return ConstantValueTypeDiscriminator.String;
			default:
				if (!IsTypeParameter(@this) && @this.IsReferenceType)
				{
					return ConstantValueTypeDiscriminator.Nothing;
				}
				return ConstantValueTypeDiscriminator.Bad;
			}
		}

		public static bool IsValidForConstantValue(this TypeSymbol @this, ConstantValue value)
		{
			ConstantValueTypeDiscriminator constantValueTypeDiscriminator = GetConstantValueTypeDiscriminator(@this);
			if (constantValueTypeDiscriminator == ConstantValueTypeDiscriminator.Bad || constantValueTypeDiscriminator != value.Discriminator)
			{
				if (value.Discriminator == ConstantValueTypeDiscriminator.Nothing)
				{
					return IsStringType(@this);
				}
				return false;
			}
			return true;
		}

		public static bool AllowsCompileTimeConversions(this TypeSymbol @this)
		{
			return CompileTimeCalculations.TypeAllowsCompileTimeConversions(GetConstantValueTypeDiscriminator(@this));
		}

		public static bool AllowsCompileTimeOperations(this TypeSymbol @this)
		{
			return CompileTimeCalculations.TypeAllowsCompileTimeOperations(GetConstantValueTypeDiscriminator(@this));
		}

		public static bool CanContainUserDefinedOperators(this TypeSymbol @this, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (@this.Kind == SymbolKind.TypeParameter)
			{
				ImmutableArray<TypeSymbol>.Enumerator enumerator = ((TypeParameterSymbol)@this).ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (CanContainUserDefinedOperators(enumerator.Current, ref useSiteInfo))
					{
						return true;
					}
				}
			}
			else if (@this.Kind == SymbolKind.NamedType && !((NamedTypeSymbol)@this).IsInterface)
			{
				TypeSymbol enumUnderlyingTypeOrSelf = GetEnumUnderlyingTypeOrSelf(GetNullableUnderlyingTypeOrSelf(@this));
				if (!IsIntrinsicType(enumUnderlyingTypeOrSelf) && !IsObjectType(enumUnderlyingTypeOrSelf))
				{
					return true;
				}
			}
			return false;
		}

		public static int? TypeToIndex(this TypeSymbol type)
		{
			return SpecialTypeExtensions.TypeToIndex(type.SpecialType);
		}

		public static TypeSymbol DigThroughArrayType(this TypeSymbol possiblyArrayType)
		{
			while (possiblyArrayType.Kind == SymbolKind.ArrayType)
			{
				possiblyArrayType = ((ArrayTypeSymbol)possiblyArrayType).ElementType;
			}
			return possiblyArrayType;
		}

		public static bool IsSameOrNestedWithin(this NamedTypeSymbol inner, NamedTypeSymbol outer)
		{
			do
			{
				if (TypeSymbol.Equals(inner, outer, TypeCompareKind.ConsiderEverything))
				{
					return true;
				}
				inner = inner.ContainingType;
			}
			while ((object)inner != null);
			return false;
		}

		public static bool ImplementsInterface(this TypeSymbol subType, TypeSymbol superInterface, EqualityComparer<TypeSymbol> comparer, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TypeSymbol>.Default;
			}
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = subType.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				if (current.IsInterface && comparer.Equals(current, superInterface))
				{
					return true;
				}
			}
			return false;
		}

		public static void AddUseSiteInfo(this TypeSymbol type, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (useSiteInfo.AccumulatesDiagnostics)
			{
				useSiteInfo.Add(type.GetUseSiteInfo());
			}
		}

		public static void AddUseSiteDiagnosticsForBaseDefinitions(this TypeSymbol source, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = source;
			do
			{
				typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
			}
			while ((object)typeSymbol != null);
		}

		public static void AddConstraintsUseSiteInfo(this TypeParameterSymbol type, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			useSiteInfo.Add(type.GetConstraintsUseSiteInfo());
		}

		public static bool IsBaseTypeOf(this TypeSymbol superType, TypeSymbol subType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = subType;
			while ((object)typeSymbol != null)
			{
				if ((object)typeSymbol != subType)
				{
					AddUseSiteInfo(typeSymbol.OriginalDefinition, ref useSiteInfo);
				}
				if (IsSameTypeIgnoringAll(typeSymbol, superType))
				{
					return true;
				}
				typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			return false;
		}

		public static bool IsOrDerivedFrom(this NamedTypeSymbol derivedType, TypeSymbol baseType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol namedTypeSymbol = derivedType;
			while ((object)namedTypeSymbol != null)
			{
				if (IsSameTypeIgnoringAll(namedTypeSymbol, baseType))
				{
					return true;
				}
				namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
			}
			return false;
		}

		public static bool IsOrDerivedFrom(this TypeSymbol derivedType, TypeSymbol baseType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			while ((object)derivedType != null)
			{
				switch (derivedType.TypeKind)
				{
				case TypeKind.Array:
					derivedType = derivedType.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
					break;
				case TypeKind.TypeParameter:
					derivedType = ConstraintsHelper.GetNonInterfaceConstraint((TypeParameterSymbol)derivedType, ref useSiteInfo);
					break;
				default:
					return IsOrDerivedFrom((NamedTypeSymbol)derivedType, baseType, ref useSiteInfo);
				}
			}
			return false;
		}

		public static bool IsOrDerivedFromWellKnownClass(this TypeSymbol derivedType, WellKnownType wellKnownBaseType, VisualBasicCompilation compilation, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return IsOrDerivedFrom(derivedType, compilation.GetWellKnownType(wellKnownBaseType), ref useSiteInfo);
		}

		public static bool IsCompatibleWithGenericIEnumerableOfType(this TypeSymbol type, TypeSymbol typeArgument, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (IsErrorType(typeArgument))
			{
				return false;
			}
			TypeSymbol typeSymbol = typeArgument;
			while (IsArrayType(typeSymbol))
			{
				typeSymbol = ((ArrayTypeSymbol)typeSymbol).ElementType;
			}
			if (IsErrorType(typeSymbol))
			{
				return false;
			}
			NamedTypeSymbol specialType = typeSymbol.ContainingAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
			HashSet<NamedTypeSymbol> hashSet = new HashSet<NamedTypeSymbol>();
			if (Binder.IsOrInheritsFromOrImplementsInterface(type, specialType, ref useSiteInfo, hashSet) && hashSet.Count > 0)
			{
				foreach (NamedTypeSymbol item in hashSet)
				{
					TypeSymbol typeSymbol2 = item.TypeArgumentWithDefinitionUseSiteDiagnostics(0, ref useSiteInfo);
					if (IsErrorType(typeSymbol2))
					{
						return false;
					}
					if (Conversions.IsWideningConversion(Conversions.ClassifyDirectCastConversion(typeSymbol2, typeArgument, ref useSiteInfo)))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsOrImplementsIEnumerableOfXElement(this TypeSymbol type, VisualBasicCompilation compilation, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(WellKnownType.System_Xml_Linq_XElement);
			return IsCompatibleWithGenericIEnumerableOfType(type, wellKnownType, ref useSiteInfo);
		}

		public static bool IsBaseTypeOrInterfaceOf(this TypeSymbol superType, TypeSymbol subType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (IsInterfaceType(superType))
			{
				return ImplementsInterface(subType, superType, EqualsIgnoringComparer.InstanceCLRSignatureCompare, ref useSiteInfo);
			}
			return IsBaseTypeOf(superType, subType, ref useSiteInfo);
		}

		internal static bool IsValidTypeForConstField(this TypeSymbol fieldType)
		{
			if (!IsIntrinsicType(fieldType) && fieldType.SpecialType != SpecialType.System_Object)
			{
				return fieldType.TypeKind == TypeKind.Enum;
			}
			return true;
		}

		internal static void CollectReferencedTypeParameters(this TypeSymbol @this, HashSet<TypeParameterSymbol> typeParameters)
		{
			VisitType(@this, s_addIfTypeParameterFunc, typeParameters);
		}

		private static bool AddIfTypeParameter(TypeSymbol type, HashSet<TypeParameterSymbol> typeParameters)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				typeParameters.Add((TypeParameterSymbol)type);
			}
			return false;
		}

		internal static bool ReferencesTypeParameterNotInTheSet(this TypeSymbol @this, HashSet<TypeParameterSymbol> typeParameters)
		{
			return (object)VisitType(@this, s_isTypeParameterNotInSetFunc, typeParameters) != null;
		}

		private static bool IsTypeParameterNotInSet(TypeSymbol type, HashSet<TypeParameterSymbol> typeParameters)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				return !typeParameters.Contains((TypeParameterSymbol)type);
			}
			return false;
		}

		internal static bool ReferencesMethodsTypeParameter(this TypeSymbol @this, MethodSymbol method)
		{
			return (object)VisitType(@this, s_isMethodTypeParameterFunc, method) != null;
		}

		private static bool IsMethodTypeParameter(TypeSymbol type, MethodSymbol method)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				return type.ContainingSymbol.Equals(method);
			}
			return false;
		}

		public static bool IsUnboundGenericType(this TypeSymbol @this)
		{
			if (@this is NamedTypeSymbol namedTypeSymbol)
			{
				return namedTypeSymbol.IsUnboundGenericType;
			}
			return false;
		}

		internal static bool IsOrRefersToTypeParameter(this TypeSymbol @this)
		{
			return (object)VisitType(@this, s_isTypeParameterFunc, null) != null;
		}

		internal static bool ContainsTuple(this TypeSymbol type)
		{
			return (object)VisitType(type, s_isTupleTypeFunc, null) != null;
		}

		internal static bool ContainsTupleNames(this TypeSymbol type)
		{
			return (object)VisitType(type, s_hasTupleNamesFunc, null) != null;
		}

		internal static TypeSymbol VisitType<T>(this TypeSymbol type, Func<TypeSymbol, T, bool> predicate, T arg)
		{
			TypeSymbol typeSymbol = type;
			while (true)
			{
				switch (typeSymbol.TypeKind)
				{
				case TypeKind.Class:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Interface:
				case TypeKind.Struct:
				{
					NamedTypeSymbol containingType = typeSymbol.ContainingType;
					if ((object)containingType != null)
					{
						TypeSymbol typeSymbol2 = VisitType(containingType, predicate, arg);
						if ((object)typeSymbol2 != null)
						{
							return typeSymbol2;
						}
					}
					break;
				}
				}
				if (predicate(typeSymbol, arg))
				{
					break;
				}
				switch (typeSymbol.TypeKind)
				{
				case TypeKind.Dynamic:
				case TypeKind.Enum:
				case TypeKind.Module:
				case TypeKind.TypeParameter:
				case TypeKind.Submission:
					return null;
				case TypeKind.Class:
				case TypeKind.Delegate:
				case TypeKind.Error:
				case TypeKind.Interface:
				case TypeKind.Struct:
				{
					if (typeSymbol.IsTupleType)
					{
						typeSymbol = typeSymbol.TupleUnderlyingType;
					}
					ImmutableArray<TypeSymbol>.Enumerator enumerator = ((NamedTypeSymbol)typeSymbol).TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator.MoveNext())
					{
						TypeSymbol typeSymbol3 = VisitType(enumerator.Current, predicate, arg);
						if ((object)typeSymbol3 != null)
						{
							return typeSymbol3;
						}
					}
					return null;
				}
				case TypeKind.Array:
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(typeSymbol.TypeKind);
				}
				typeSymbol = ((ArrayTypeSymbol)typeSymbol).ElementType;
			}
			return typeSymbol;
		}

		public static bool IsValidTypeForAttributeArgument(this TypeSymbol type, VisualBasicCompilation compilation)
		{
			if ((object)type == null)
			{
				return false;
			}
			if (IsArrayType(type))
			{
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
				if (!arrayTypeSymbol.IsSZArray)
				{
					return false;
				}
				type = arrayTypeSymbol.ElementType;
			}
			return SpecialTypeExtensions.IsValidTypeForAttributeArgument(GetEnumUnderlyingTypeOrSelf(type).SpecialType) || TypeSymbol.Equals(type, compilation.GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.ConsiderEverything);
		}

		public static bool IsValidTypeForSwitchTable(this TypeSymbol type)
		{
			type = GetNullableUnderlyingTypeOrSelf(type);
			type = GetEnumUnderlyingTypeOrSelf(type);
			return SpecialTypeExtensions.IsValidTypeForSwitchTable(type.SpecialType);
		}

		public static bool IsIntrinsicOrEnumType(this TypeSymbol type)
		{
			if ((object)type != null)
			{
				return IsIntrinsicType(GetEnumUnderlyingTypeOrSelf(type));
			}
			return false;
		}

		public static bool MarkCheckedIfNecessary(this TypeSymbol type, ref HashSet<TypeSymbol> checkedTypes)
		{
			if (checkedTypes == null)
			{
				checkedTypes = new HashSet<TypeSymbol>();
			}
			return checkedTypes.Add(type);
		}

		internal static void CheckTypeArguments(this ImmutableArray<TypeSymbol> typeArguments, int expectedCount)
		{
			if (typeArguments.IsDefault)
			{
				throw new ArgumentNullException("typeArguments");
			}
			ImmutableArray<TypeSymbol>.Enumerator enumerator = typeArguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if ((object)enumerator.Current == null)
				{
					throw new ArgumentException(VBResources.TypeArgumentCannotBeNothing, "typeArguments");
				}
			}
			if (typeArguments.Length == 0 || typeArguments.Length != expectedCount)
			{
				throw new ArgumentException(VBResources.WrongNumberOfTypeArguments, "typeArguments");
			}
		}

		internal static ImmutableArray<TypeSymbol> TransformToCanonicalFormFor(this ImmutableArray<TypeSymbol> typeArguments, SubstitutedNamedType.SpecializedGenericType genericType)
		{
			return TransformToCanonicalFormFor(typeArguments, genericType, genericType.TypeParameters);
		}

		internal static ImmutableArray<TypeSymbol> TransformToCanonicalFormFor(this ImmutableArray<TypeSymbol> typeArguments, SubstitutedMethodSymbol.SpecializedGenericMethod genericMethod)
		{
			return TransformToCanonicalFormFor(typeArguments, genericMethod, genericMethod.TypeParameters);
		}

		private static ImmutableArray<TypeSymbol> TransformToCanonicalFormFor(ImmutableArray<TypeSymbol> typeArguments, Symbol specializedGenericTypeOrMethod, ImmutableArray<TypeParameterSymbol> specializedTypeParameters)
		{
			TypeSymbol[] array = null;
			int num = 0;
			TypeSymbol typeSymbol;
			do
			{
				typeSymbol = typeArguments[num];
				if (IsTypeParameter(typeSymbol) && !typeSymbol.IsDefinition)
				{
					Symbol containingSymbol = typeSymbol.ContainingSymbol;
					if ((object)containingSymbol != specializedGenericTypeOrMethod && containingSymbol.Equals(specializedGenericTypeOrMethod))
					{
						array = typeArguments.ToArray();
						break;
					}
				}
				num++;
			}
			while (num < typeArguments.Length);
			if (array != null)
			{
				array[num] = specializedTypeParameters[((TypeParameterSymbol)typeSymbol).Ordinal];
				for (num++; num < typeArguments.Length; num++)
				{
					typeSymbol = typeArguments[num];
					if (IsTypeParameter(typeSymbol) && !typeSymbol.IsDefinition)
					{
						Symbol containingSymbol2 = typeSymbol.ContainingSymbol;
						if ((object)containingSymbol2 != specializedGenericTypeOrMethod && containingSymbol2.Equals(specializedGenericTypeOrMethod))
						{
							array[num] = specializedTypeParameters[((TypeParameterSymbol)typeSymbol).Ordinal];
						}
					}
				}
				typeArguments = array.AsImmutableOrNull();
			}
			int num2 = specializedTypeParameters.Length - 1;
			for (num = 0; num <= num2; num++)
			{
				if ((object)specializedTypeParameters[num] != typeArguments[num])
				{
					return typeArguments;
				}
			}
			return default(ImmutableArray<TypeSymbol>);
		}

		public static NamedTypeSymbol ExpressionTargetDelegate(this TypeSymbol type, VisualBasicCompilation compilation)
		{
			if (type.TypeKind == TypeKind.Class)
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
				if (namedTypeSymbol.Arity == 1 && TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything))
				{
					TypeSymbol typeSymbol = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[0];
					if (typeSymbol.TypeKind == TypeKind.Delegate)
					{
						return (NamedTypeSymbol)typeSymbol;
					}
				}
			}
			return null;
		}

		public static NamedTypeSymbol DelegateOrExpressionDelegate(this TypeSymbol type, Binder binder)
		{
			if (type.TypeKind == TypeKind.Delegate)
			{
				return (NamedTypeSymbol)type;
			}
			return ExpressionTargetDelegate(type, binder.Compilation);
		}

		public static NamedTypeSymbol DelegateOrExpressionDelegate(this TypeSymbol type, Binder binder, ref bool wasExpression)
		{
			if (type.TypeKind == TypeKind.Delegate)
			{
				wasExpression = false;
				return (NamedTypeSymbol)type;
			}
			NamedTypeSymbol namedTypeSymbol = ExpressionTargetDelegate(type, binder.Compilation);
			wasExpression = (object)namedTypeSymbol != null;
			return namedTypeSymbol;
		}

		public static bool IsExpressionTree(this TypeSymbol type, Binder binder)
		{
			return (object)ExpressionTargetDelegate(type, binder.Compilation) != null;
		}

		public static bool IsExtensibleInterfaceNoUseSiteDiagnostics(this TypeSymbol type)
		{
			if (IsInterfaceType(type))
			{
				return ((NamedTypeSymbol)type).IsExtensibleInterfaceNoUseSiteDiagnostics;
			}
			return false;
		}

		public static string GetNativeCompilerVType(this TypeSymbol type)
		{
			object obj = SpecialTypeExtensions.GetNativeCompilerVType(type.SpecialType);
			if (obj == null)
			{
				if (!IsTypeParameter(type))
				{
					if (!IsArrayType(type))
					{
						if (!type.IsValueType)
						{
							return "t_ref";
						}
						return "t_struct";
					}
					return "t_array";
				}
				obj = "t_generic";
			}
			return (string)obj;
		}

		public static bool IsVerifierReference(this TypeSymbol type)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				return false;
			}
			return type.IsReferenceType;
		}

		public static bool IsVerifierValue(this TypeSymbol type)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				return false;
			}
			return type.IsValueType;
		}

		public static bool IsPrimitiveType(this TypeSymbol t)
		{
			return SpecialTypeExtensions.IsPrimitiveType(t.SpecialType);
		}

		public static bool IsTopLevelType(this NamedTypeSymbol type)
		{
			return (object)type.ContainingType == null;
		}

		public static ImmutableArray<TypeParameterSymbol> GetAllTypeParameters(this NamedTypeSymbol type)
		{
			if ((object)type.ContainingType == null)
			{
				return type.TypeParameters;
			}
			ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
			GetAllTypeParameters(type, instance);
			return instance.ToImmutableAndFree();
		}

		public static void GetAllTypeParameters(this NamedTypeSymbol type, ArrayBuilder<TypeParameterSymbol> builder)
		{
			NamedTypeSymbol containingType = type.ContainingType;
			if ((object)containingType != null)
			{
				GetAllTypeParameters(containingType, builder);
			}
			builder.AddRange(type.TypeParameters);
		}

		public static ImmutableArray<TypeSymbol> GetAllTypeArguments(this NamedTypeSymbol type)
		{
			ImmutableArray<TypeSymbol> immutableArray = type.TypeArgumentsNoUseSiteDiagnostics;
			while (true)
			{
				type = type.ContainingType;
				if ((object)type == null)
				{
					break;
				}
				immutableArray = type.TypeArgumentsNoUseSiteDiagnostics.Concat(immutableArray);
			}
			return immutableArray;
		}

		public static ImmutableArray<TypeWithModifiers> GetAllTypeArgumentsWithModifiers(this NamedTypeSymbol type)
		{
			ArrayBuilder<TypeWithModifiers> instance = ArrayBuilder<TypeWithModifiers>.GetInstance();
			do
			{
				ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = type.TypeArgumentsNoUseSiteDiagnostics;
				for (int i = typeArgumentsNoUseSiteDiagnostics.Length - 1; i >= 0; i += -1)
				{
					instance.Add(new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics[i], type.GetTypeArgumentCustomModifiers(i)));
				}
				type = type.ContainingType;
			}
			while ((object)type != null);
			instance.ReverseContents();
			return instance.ToImmutableAndFree();
		}

		internal static bool HasNameQualifier(this NamedTypeSymbol type, string qualifiedName, StringComparison comparison)
		{
			Symbol containingSymbol = type.ContainingSymbol;
			if (containingSymbol.Kind != SymbolKind.Namespace)
			{
				return string.Equals(containingSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), qualifiedName, comparison);
			}
			string emittedNamespaceName = type.GetEmittedNamespaceName();
			if (emittedNamespaceName != null)
			{
				return string.Equals(qualifiedName, emittedNamespaceName, comparison);
			}
			NamespaceSymbol namespaceSymbol = (NamespaceSymbol)containingSymbol;
			if (namespaceSymbol.IsGlobalNamespace)
			{
				return qualifiedName.Length == 0;
			}
			return HasNamespaceName(namespaceSymbol, qualifiedName, comparison, qualifiedName.Length);
		}

		private static bool HasNamespaceName(NamespaceSymbol @namespace, string namespaceName, StringComparison comparison, int length)
		{
			if (length == 0)
			{
				return false;
			}
			NamespaceSymbol containingNamespace = @namespace.ContainingNamespace;
			int num = namespaceName.LastIndexOf('.', length - 1, length);
			int indexB = 0;
			if (num >= 0)
			{
				if (containingNamespace.IsGlobalNamespace)
				{
					return false;
				}
				if (!HasNamespaceName(containingNamespace, namespaceName, comparison, num))
				{
					return false;
				}
				int num2 = num + 1;
				indexB = num2;
				length -= num2;
			}
			else if (!containingNamespace.IsGlobalNamespace)
			{
				return false;
			}
			string name = @namespace.Name;
			return name.Length == length && string.Compare(name, 0, namespaceName, indexB, length, comparison) == 0;
		}

		internal static TypeReferenceWithAttributes GetTypeRefWithAttributes(this TypeSymbol type, VisualBasicCompilation declaringCompilation, ITypeReference typeRef)
		{
			TypeReferenceWithAttributes result;
			if (ContainsTupleNames(type))
			{
				SynthesizedAttributeData synthesizedAttributeData = declaringCompilation.SynthesizeTupleNamesAttribute(type);
				if (synthesizedAttributeData != null)
				{
					result = new TypeReferenceWithAttributes(typeRef, ImmutableArray.Create((ICustomAttribute)synthesizedAttributeData));
					goto IL_0034;
				}
			}
			result = new TypeReferenceWithAttributes(typeRef);
			goto IL_0034;
			IL_0034:
			return result;
		}

		internal static bool IsWellKnownTypeIsExternalInit(this TypeSymbol typeSymbol)
		{
			return IsWellKnownCompilerServicesTopLevelType(typeSymbol, "IsExternalInit");
		}

		private static bool IsWellKnownCompilerServicesTopLevelType(this TypeSymbol typeSymbol, string name)
		{
			if (!string.Equals(typeSymbol.Name, name))
			{
				return false;
			}
			return IsCompilerServicesTopLevelType(typeSymbol);
		}

		internal static bool IsCompilerServicesTopLevelType(this TypeSymbol typeSymbol)
		{
			if ((object)typeSymbol.ContainingType == null)
			{
				return IsContainedInNamespace(typeSymbol, "System", "Runtime", "CompilerServices");
			}
			return false;
		}

		private static bool IsContainedInNamespace(this TypeSymbol typeSymbol, string outerNS, string midNS, string innerNS)
		{
			NamespaceSymbol containingNamespace = typeSymbol.ContainingNamespace;
			if (!string.Equals(containingNamespace?.Name, innerNS))
			{
				return false;
			}
			NamespaceSymbol containingNamespace2 = containingNamespace.ContainingNamespace;
			if (!string.Equals(containingNamespace2?.Name, midNS))
			{
				return false;
			}
			NamespaceSymbol containingNamespace3 = containingNamespace2.ContainingNamespace;
			if (!string.Equals(containingNamespace3?.Name, outerNS))
			{
				return false;
			}
			return containingNamespace3.ContainingNamespace?.IsGlobalNamespace ?? false;
		}
	}
}
