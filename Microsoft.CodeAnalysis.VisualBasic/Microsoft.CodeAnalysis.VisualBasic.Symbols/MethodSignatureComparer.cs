using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class MethodSignatureComparer : IEqualityComparer<MethodSymbol>
	{
		public struct LazyTypeSubstitution
		{
			private TypeSubstitution _typeSubstitution;

			private MethodSymbol _method;

			public TypeSubstitution Value
			{
				get
				{
					if (_typeSubstitution == null && (object)_method != null)
					{
						_typeSubstitution = GetTypeSubstitution(_method);
						_method = null;
					}
					return _typeSubstitution;
				}
			}

			public LazyTypeSubstitution(MethodSymbol method)
			{
				this = default(LazyTypeSubstitution);
				_method = method;
			}
		}

		public static readonly MethodSignatureComparer RuntimeMethodSignatureComparer = new MethodSignatureComparer(considerName: true, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerByRef: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly MethodSignatureComparer AllAspectsSignatureComparer = new MethodSignatureComparer(considerName: true, considerReturnType: true, considerTypeConstraints: true, considerCallingConvention: true, considerByRef: true, considerCustomModifiers: true, considerTupleNames: true);

		public static readonly MethodSignatureComparer ParametersAndReturnTypeSignatureComparer = new MethodSignatureComparer(considerName: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerByRef: true, considerCustomModifiers: false, considerTupleNames: false);

		public static readonly MethodSignatureComparer CustomModifiersAndParametersAndReturnTypeSignatureComparer = new MethodSignatureComparer(considerName: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerByRef: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly MethodSignatureComparer VisualBasicSignatureAndConstraintsAndReturnTypeComparer = new MethodSignatureComparer(considerName: true, considerReturnType: true, considerTypeConstraints: true, considerCallingConvention: true, considerByRef: true, considerCustomModifiers: false, considerTupleNames: false);

		public static readonly MethodSignatureComparer RetargetedExplicitMethodImplementationComparer = new MethodSignatureComparer(considerName: true, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerByRef: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly MethodSignatureComparer WinRTConflictComparer = new MethodSignatureComparer(considerName: true, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerByRef: false, considerCustomModifiers: false, considerTupleNames: false);

		private readonly bool _considerName;

		private readonly bool _considerReturnType;

		private readonly bool _considerTypeConstraints;

		private readonly bool _considerCallingConvention;

		private readonly bool _considerByRef;

		private readonly bool _considerCustomModifiers;

		private readonly bool _considerTupleNames;

		private MethodSignatureComparer(bool considerName, bool considerReturnType, bool considerTypeConstraints, bool considerCallingConvention, bool considerByRef, bool considerCustomModifiers, bool considerTupleNames)
		{
			_considerName = considerName;
			_considerReturnType = considerReturnType;
			_considerTypeConstraints = considerTypeConstraints;
			_considerCallingConvention = considerCallingConvention;
			_considerByRef = considerByRef;
			_considerCustomModifiers = considerCustomModifiers;
			_considerTupleNames = considerTupleNames;
		}

		public bool Equals(MethodSymbol method1, MethodSymbol method2)
		{
			if ((object)method1 == method2)
			{
				return true;
			}
			if ((object)method1 == null || (object)method2 == null)
			{
				return false;
			}
			if (method1.Arity != method2.Arity)
			{
				return false;
			}
			if (_considerName && !CaseInsensitiveComparison.Equals(method1.Name, method2.Name))
			{
				return false;
			}
			TypeSubstitution typeSubstitution = GetTypeSubstitution(method1);
			TypeSubstitution typeSubstitution2 = GetTypeSubstitution(method2);
			if (_considerReturnType && !HaveSameReturnTypes(method1, typeSubstitution, method2, typeSubstitution2, _considerCustomModifiers, _considerTupleNames))
			{
				return false;
			}
			if ((method1.ParameterCount > 0 || method2.ParameterCount > 0) && !HaveSameParameterTypes(method1.Parameters, typeSubstitution, method2.Parameters, typeSubstitution2, _considerByRef, _considerCustomModifiers, _considerTupleNames))
			{
				return false;
			}
			if (_considerCallingConvention)
			{
				if (method1.CallingConvention != method2.CallingConvention)
				{
					return false;
				}
			}
			else if (method1.IsVararg != method2.IsVararg)
			{
				return false;
			}
			if (_considerTypeConstraints && !HaveSameConstraints(method1, typeSubstitution, method2, typeSubstitution2))
			{
				return false;
			}
			return true;
		}

		bool IEqualityComparer<MethodSymbol>.Equals(MethodSymbol method1, MethodSymbol method2)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(method1, method2);
		}

		public int GetHashCode(MethodSymbol method)
		{
			int num = 1;
			if ((object)method != null)
			{
				if (_considerName)
				{
					num = Hash.Combine(method.Name, num);
				}
				if (_considerReturnType && !method.IsGenericMethod && !_considerCustomModifiers)
				{
					num = Hash.Combine(method.ReturnType, num);
				}
				num = Hash.Combine(num, method.Arity);
				num = Hash.Combine(num, method.ParameterCount);
				num = Hash.Combine(method.IsVararg, num);
			}
			return num;
		}

		int IEqualityComparer<MethodSymbol>.GetHashCode(MethodSymbol method)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
			return this.GetHashCode(method);
		}

		public static SymbolComparisonResults DetailedCompare(MethodSymbol method1, MethodSymbol method2, SymbolComparisonResults comparisons, SymbolComparisonResults stopIfAny = (SymbolComparisonResults)0)
		{
			SymbolComparisonResults symbolComparisonResults = (SymbolComparisonResults)0;
			if (method1 == method2)
			{
				return (SymbolComparisonResults)0;
			}
			if ((comparisons & SymbolComparisonResults.ArityMismatch) != 0 && method1.Arity != method2.Arity)
			{
				symbolComparisonResults |= SymbolComparisonResults.ArityMismatch;
				if ((stopIfAny & SymbolComparisonResults.ArityMismatch) != 0)
				{
					goto IL_019c;
				}
			}
			if ((stopIfAny & SymbolComparisonResults.TotalParameterCountMismatch) != 0 && method1.ParameterCount != method2.ParameterCount)
			{
				symbolComparisonResults |= SymbolComparisonResults.TotalParameterCountMismatch;
			}
			else
			{
				LazyTypeSubstitution lazyTypeSubstitution = new LazyTypeSubstitution(method1);
				LazyTypeSubstitution lazyTypeSubstitution2 = new LazyTypeSubstitution(method2);
				if ((comparisons & (SymbolComparisonResults)131106) != 0)
				{
					MethodSymbol originalDefinition = method1.OriginalDefinition;
					MethodSymbol originalDefinition2 = method2.OriginalDefinition;
					symbolComparisonResults |= DetailedReturnTypeCompare(originalDefinition.ReturnsByRef, new TypeWithModifiers(originalDefinition.ReturnType, originalDefinition.ReturnTypeCustomModifiers), originalDefinition.RefCustomModifiers, lazyTypeSubstitution.Value, originalDefinition2.ReturnsByRef, new TypeWithModifiers(originalDefinition2.ReturnType, originalDefinition2.ReturnTypeCustomModifiers), originalDefinition2.RefCustomModifiers, lazyTypeSubstitution2.Value, comparisons, stopIfAny);
					if ((stopIfAny & symbolComparisonResults) != 0)
					{
						goto IL_019c;
					}
				}
				if ((comparisons & SymbolComparisonResults.AllParameterMismatches) != 0)
				{
					symbolComparisonResults |= DetailedParameterCompare(method1.Parameters, ref lazyTypeSubstitution, method2.Parameters, ref lazyTypeSubstitution2, comparisons, stopIfAny);
					if ((stopIfAny & symbolComparisonResults) != 0)
					{
						goto IL_019c;
					}
				}
				if ((comparisons & SymbolComparisonResults.CallingConventionMismatch) != 0 && method1.CallingConvention != method2.CallingConvention)
				{
					symbolComparisonResults |= SymbolComparisonResults.CallingConventionMismatch;
					if ((stopIfAny & SymbolComparisonResults.CallingConventionMismatch) != 0)
					{
						goto IL_019c;
					}
				}
				if ((comparisons & SymbolComparisonResults.VarargMismatch) != 0 && method1.IsVararg != method2.IsVararg)
				{
					symbolComparisonResults |= SymbolComparisonResults.VarargMismatch;
					if ((stopIfAny & SymbolComparisonResults.VarargMismatch) != 0)
					{
						goto IL_019c;
					}
				}
				if ((comparisons & SymbolComparisonResults.ConstraintMismatch) != 0 && (symbolComparisonResults & SymbolComparisonResults.ArityMismatch) == 0 && !HaveSameConstraints(method1, lazyTypeSubstitution.Value, method2, lazyTypeSubstitution2.Value))
				{
					symbolComparisonResults |= SymbolComparisonResults.ConstraintMismatch;
					if ((stopIfAny & SymbolComparisonResults.ConstraintMismatch) != 0)
					{
						goto IL_019c;
					}
				}
				if ((comparisons & SymbolComparisonResults.NameMismatch) != 0 && !CaseInsensitiveComparison.Equals(method1.Name, method2.Name))
				{
					symbolComparisonResults |= SymbolComparisonResults.NameMismatch;
					_ = stopIfAny & SymbolComparisonResults.NameMismatch;
				}
			}
			goto IL_019c;
			IL_019c:
			return symbolComparisonResults & comparisons;
		}

		public static SymbolComparisonResults DetailedReturnTypeCompare(bool returnsByRef1, TypeWithModifiers type1, ImmutableArray<CustomModifier> refCustomModifiers1, TypeSubstitution typeSubstitution1, bool returnsByRef2, TypeWithModifiers type2, ImmutableArray<CustomModifier> refCustomModifiers2, TypeSubstitution typeSubstitution2, SymbolComparisonResults comparisons, SymbolComparisonResults stopIfAny = (SymbolComparisonResults)0)
		{
			if (returnsByRef1 != returnsByRef2)
			{
				return SymbolComparisonResults.ReturnTypeMismatch;
			}
			type1 = SubstituteType(typeSubstitution1, type1);
			type2 = SubstituteType(typeSubstitution2, type2);
			if (!TypeSymbolExtensions.IsSameType(type1.Type, type2.Type, TypeCompareKind.AllIgnoreOptionsForVB))
			{
				return SymbolComparisonResults.ReturnTypeMismatch;
			}
			SymbolComparisonResults symbolComparisonResults = (SymbolComparisonResults)0;
			if ((comparisons & SymbolComparisonResults.TupleNamesMismatch) != 0 && !TypeSymbolExtensions.IsSameType(type1.Type, type2.Type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds))
			{
				symbolComparisonResults |= SymbolComparisonResults.TupleNamesMismatch;
				if ((stopIfAny & SymbolComparisonResults.TupleNamesMismatch) != 0)
				{
					goto IL_00a2;
				}
			}
			if ((comparisons & SymbolComparisonResults.CustomModifierMismatch) != 0 && (!type1.IsSameType(type2, TypeCompareKind.IgnoreTupleNames) || !SubstituteModifiers(typeSubstitution1, refCustomModifiers1).SequenceEqual(SubstituteModifiers(typeSubstitution2, refCustomModifiers2))))
			{
				symbolComparisonResults |= SymbolComparisonResults.CustomModifierMismatch;
				_ = stopIfAny & SymbolComparisonResults.CustomModifierMismatch;
			}
			goto IL_00a2;
			IL_00a2:
			return symbolComparisonResults;
		}

		private static ImmutableArray<CustomModifier> SubstituteModifiers(TypeSubstitution typeSubstitution, ImmutableArray<CustomModifier> customModifiers)
		{
			return typeSubstitution?.SubstituteCustomModifiers(customModifiers) ?? customModifiers;
		}

		public static SymbolComparisonResults DetailedParameterCompare(ImmutableArray<ParameterSymbol> params1, [In] ref LazyTypeSubstitution lazyTypeSubstitution1, ImmutableArray<ParameterSymbol> params2, [In] ref LazyTypeSubstitution lazyTypeSubstitution2, SymbolComparisonResults comparisons, SymbolComparisonResults stopIfAny = (SymbolComparisonResults)0)
		{
			SymbolComparisonResults symbolComparisonResults = (SymbolComparisonResults)0;
			int length;
			ImmutableArray<ParameterSymbol> immutableArray;
			if (params1.Length > params2.Length)
			{
				length = params2.Length;
				immutableArray = params1;
			}
			else if (params1.Length < params2.Length)
			{
				length = params1.Length;
				immutableArray = params2;
			}
			else
			{
				length = params1.Length;
				immutableArray = default(ImmutableArray<ParameterSymbol>);
			}
			if (!immutableArray.IsDefault)
			{
				symbolComparisonResults |= SymbolComparisonResults.TotalParameterCountMismatch;
				if ((stopIfAny & SymbolComparisonResults.TotalParameterCountMismatch) != 0)
				{
					goto IL_02e3;
				}
				int num = length;
				int num2 = immutableArray.Length - 1;
				for (int i = num; i <= num2; i++)
				{
					if (immutableArray[i].IsOptional)
					{
						symbolComparisonResults |= SymbolComparisonResults.OptionalParameterMismatch;
						if ((stopIfAny & SymbolComparisonResults.OptionalParameterMismatch) == 0)
						{
							continue;
						}
					}
					else
					{
						symbolComparisonResults |= SymbolComparisonResults.RequiredExtraParameterMismatch;
						if ((stopIfAny & SymbolComparisonResults.RequiredExtraParameterMismatch) == 0)
						{
							continue;
						}
					}
					goto IL_02e3;
				}
			}
			if (length != 0)
			{
				bool flag;
				TypeSubstitution typeSubstitution;
				TypeSubstitution typeSubstitution2;
				if ((comparisons & (SymbolComparisonResults)131872) != 0)
				{
					flag = true;
					typeSubstitution = lazyTypeSubstitution1.Value;
					typeSubstitution2 = lazyTypeSubstitution2.Value;
				}
				else
				{
					flag = false;
					typeSubstitution = null;
					typeSubstitution2 = null;
				}
				int num3 = length - 1;
				for (int j = 0; j <= num3; j++)
				{
					ParameterSymbol parameterSymbol = params1[j];
					ParameterSymbol parameterSymbol2 = params2[j];
					bool flag2 = parameterSymbol.IsOptional && parameterSymbol2.IsOptional;
					if (parameterSymbol.IsOptional != parameterSymbol2.IsOptional)
					{
						symbolComparisonResults |= SymbolComparisonResults.OptionalParameterMismatch;
						if ((stopIfAny & SymbolComparisonResults.OptionalParameterMismatch) != 0)
						{
							break;
						}
					}
					if (flag)
					{
						TypeWithModifiers typeWithModifiers = GetTypeWithModifiers(typeSubstitution, parameterSymbol);
						TypeWithModifiers typeWithModifiers2 = GetTypeWithModifiers(typeSubstitution2, parameterSymbol2);
						if (!TypeSymbolExtensions.IsSameType(typeWithModifiers.Type, typeWithModifiers2.Type, TypeCompareKind.AllIgnoreOptionsForVB))
						{
							if (flag2)
							{
								symbolComparisonResults |= SymbolComparisonResults.OptionalParameterTypeMismatch;
								if ((stopIfAny & SymbolComparisonResults.OptionalParameterTypeMismatch) != 0)
								{
									break;
								}
							}
							else
							{
								symbolComparisonResults |= SymbolComparisonResults.RequiredParameterTypeMismatch;
								if ((stopIfAny & SymbolComparisonResults.RequiredParameterTypeMismatch) != 0)
								{
									break;
								}
							}
						}
						else
						{
							if ((comparisons & SymbolComparisonResults.TupleNamesMismatch) != 0 && !TypeSymbolExtensions.IsSameType(typeWithModifiers.Type, typeWithModifiers2.Type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds))
							{
								symbolComparisonResults |= SymbolComparisonResults.TupleNamesMismatch;
								if ((stopIfAny & SymbolComparisonResults.TupleNamesMismatch) != 0)
								{
									break;
								}
							}
							if ((comparisons & SymbolComparisonResults.CustomModifierMismatch) != 0 && (!typeWithModifiers.IsSameType(typeWithModifiers2, TypeCompareKind.IgnoreTupleNames) || !GetRefModifiers(typeSubstitution, parameterSymbol).SequenceEqual(GetRefModifiers(typeSubstitution2, parameterSymbol2))))
							{
								symbolComparisonResults |= SymbolComparisonResults.CustomModifierMismatch;
								if ((stopIfAny & SymbolComparisonResults.CustomModifierMismatch) != 0)
								{
									break;
								}
							}
						}
					}
					if (parameterSymbol.IsByRef != parameterSymbol2.IsByRef)
					{
						symbolComparisonResults |= SymbolComparisonResults.ParameterByrefMismatch;
						if ((stopIfAny & SymbolComparisonResults.ParameterByrefMismatch) != 0)
						{
							break;
						}
					}
					if ((comparisons & SymbolComparisonResults.ParamArrayMismatch) != 0 && parameterSymbol.IsParamArray != parameterSymbol2.IsParamArray)
					{
						symbolComparisonResults |= SymbolComparisonResults.ParamArrayMismatch;
						if ((stopIfAny & SymbolComparisonResults.ParamArrayMismatch) != 0)
						{
							break;
						}
					}
					if (!flag2 || (comparisons & SymbolComparisonResults.OptionalParameterValueMismatch) == 0)
					{
						continue;
					}
					bool flag3;
					if (parameterSymbol.HasExplicitDefaultValue && parameterSymbol2.HasExplicitDefaultValue)
					{
						flag3 = ParameterDefaultValueMismatch(parameterSymbol, parameterSymbol2);
					}
					else
					{
						VisualBasicCompilation declaringCompilation = parameterSymbol.DeclaringCompilation;
						VisualBasicCompilation declaringCompilation2 = parameterSymbol2.DeclaringCompilation;
						flag3 = declaringCompilation != null && declaringCompilation == declaringCompilation2;
					}
					if (flag3)
					{
						symbolComparisonResults |= SymbolComparisonResults.OptionalParameterValueMismatch;
						if ((stopIfAny & SymbolComparisonResults.OptionalParameterValueMismatch) != 0)
						{
							break;
						}
					}
				}
			}
			goto IL_02e3;
			IL_02e3:
			return symbolComparisonResults;
		}

		private static TypeWithModifiers GetTypeWithModifiers(TypeSubstitution typeSubstitution, ParameterSymbol param)
		{
			if (typeSubstitution != null)
			{
				return SubstituteType(typeSubstitution, new TypeWithModifiers(param.OriginalDefinition.Type, param.OriginalDefinition.CustomModifiers));
			}
			return new TypeWithModifiers(param.Type, param.CustomModifiers);
		}

		private static ImmutableArray<CustomModifier> GetRefModifiers(TypeSubstitution typeSubstitution, ParameterSymbol param)
		{
			return typeSubstitution?.SubstituteCustomModifiers(param.OriginalDefinition.RefCustomModifiers) ?? param.RefCustomModifiers;
		}

		private static bool ParameterDefaultValueMismatch(ParameterSymbol param1, ParameterSymbol param2)
		{
			ConstantValue constantValue = param1.ExplicitDefaultConstantValue;
			ConstantValue constantValue2 = param2.ExplicitDefaultConstantValue;
			if (constantValue.IsBad || constantValue2.IsBad)
			{
				return true;
			}
			if (constantValue.IsNothing)
			{
				ConstantValueTypeDiscriminator discriminator = ConstantValue.GetDiscriminator(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(param1.Type).SpecialType);
				if (discriminator != ConstantValueTypeDiscriminator.Bad)
				{
					constantValue = ConstantValue.Default(discriminator);
				}
			}
			if (constantValue2.IsNothing)
			{
				ConstantValueTypeDiscriminator discriminator2 = ConstantValue.GetDiscriminator(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(param2.Type).SpecialType);
				if (discriminator2 != ConstantValueTypeDiscriminator.Bad)
				{
					constantValue2 = ConstantValue.Default(discriminator2);
				}
			}
			return !constantValue.Equals(constantValue2);
		}

		public static bool HaveSameParameterTypes(ImmutableArray<ParameterSymbol> params1, TypeSubstitution typeSubstitution1, ImmutableArray<ParameterSymbol> params2, TypeSubstitution typeSubstitution2, bool considerByRef, bool considerCustomModifiers, bool considerTupleNames)
		{
			int length = params1.Length;
			if (length != params2.Length)
			{
				return false;
			}
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = params1[i];
				ParameterSymbol parameterSymbol2 = params2[i];
				TypeWithModifiers typeWithModifiers = GetTypeWithModifiers(typeSubstitution1, parameterSymbol);
				TypeWithModifiers typeWithModifiers2 = GetTypeWithModifiers(typeSubstitution2, parameterSymbol2);
				TypeCompareKind compareKind = MakeTypeCompareKind(considerCustomModifiers, considerTupleNames);
				if (!typeWithModifiers.IsSameType(typeWithModifiers2, compareKind))
				{
					return false;
				}
				if (considerCustomModifiers && !GetRefModifiers(typeSubstitution1, parameterSymbol).SequenceEqual(GetRefModifiers(typeSubstitution2, parameterSymbol2)))
				{
					return false;
				}
				if (considerByRef && parameterSymbol.IsByRef != parameterSymbol2.IsByRef)
				{
					return false;
				}
			}
			return true;
		}

		internal static TypeCompareKind MakeTypeCompareKind(bool considerCustomModifiers, bool considerTupleNames)
		{
			TypeCompareKind typeCompareKind = TypeCompareKind.ConsiderEverything;
			if (!considerCustomModifiers)
			{
				typeCompareKind |= TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds;
			}
			if (!considerTupleNames)
			{
				typeCompareKind |= TypeCompareKind.IgnoreTupleNames;
			}
			return typeCompareKind;
		}

		private static bool HaveSameReturnTypes(MethodSymbol method1, TypeSubstitution typeSubstitution1, MethodSymbol method2, TypeSubstitution typeSubstitution2, bool considerCustomModifiers, bool considerTupleNames)
		{
			bool isSub = method1.IsSub;
			bool isSub2 = method2.IsSub;
			if (isSub != isSub2)
			{
				return false;
			}
			if (isSub)
			{
				return true;
			}
			if (method1.ReturnsByRef != method2.ReturnsByRef)
			{
				return false;
			}
			MethodSymbol originalDefinition = method1.OriginalDefinition;
			MethodSymbol originalDefinition2 = method2.OriginalDefinition;
			TypeWithModifiers typeWithModifiers = SubstituteType(typeSubstitution1, new TypeWithModifiers(originalDefinition.ReturnType, originalDefinition.ReturnTypeCustomModifiers));
			TypeWithModifiers other = SubstituteType(typeSubstitution2, new TypeWithModifiers(originalDefinition2.ReturnType, originalDefinition2.ReturnTypeCustomModifiers));
			TypeCompareKind compareKind = MakeTypeCompareKind(considerCustomModifiers, considerTupleNames);
			return typeWithModifiers.IsSameType(other, compareKind) && (!considerCustomModifiers || SubstituteModifiers(typeSubstitution1, originalDefinition.RefCustomModifiers).SequenceEqual(SubstituteModifiers(typeSubstitution2, originalDefinition2.RefCustomModifiers)));
		}

		private static TypeSubstitution GetTypeSubstitution(MethodSymbol method)
		{
			NamedTypeSymbol containingType = method.ContainingType;
			if (method.Arity == 0)
			{
				if ((object)containingType == null || method.IsDefinition)
				{
					return null;
				}
				return containingType.TypeSubstitution;
			}
			ImmutableArray<TypeSymbol> args = StaticCast<TypeSymbol>.From(IndexedTypeParameterSymbol.Take(method.Arity));
			if (method.IsDefinition)
			{
				return TypeSubstitution.Create(method, method.TypeParameters, args);
			}
			return TypeSubstitution.Create(containingType.TypeSubstitution, method.OriginalDefinition, args);
		}

		private static TypeWithModifiers SubstituteType(TypeSubstitution typeSubstitution, TypeWithModifiers typeSymbol)
		{
			return typeSymbol.InternalSubstituteTypeParameters(typeSubstitution);
		}

		internal static bool HaveSameConstraints(MethodSymbol method1, MethodSymbol method2)
		{
			return HaveSameConstraints(method1, GetTypeSubstitution(method1), method2, GetTypeSubstitution(method2));
		}

		private static bool HaveSameConstraints(MethodSymbol method1, TypeSubstitution typeSubstitution1, MethodSymbol method2, TypeSubstitution typeSubstitution2)
		{
			ImmutableArray<TypeParameterSymbol> typeParameters = method1.OriginalDefinition.TypeParameters;
			ImmutableArray<TypeParameterSymbol> typeParameters2 = method2.OriginalDefinition.TypeParameters;
			int num = typeParameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!HaveSameConstraints(typeParameters[i], typeSubstitution1, typeParameters2[i], typeSubstitution2))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool HaveSameConstraints(TypeParameterSymbol typeParameter1, TypeSubstitution typeSubstitution1, TypeParameterSymbol typeParameter2, TypeSubstitution typeSubstitution2)
		{
			if (typeParameter1.HasConstructorConstraint != typeParameter2.HasConstructorConstraint || typeParameter1.HasReferenceTypeConstraint != typeParameter2.HasReferenceTypeConstraint || typeParameter1.HasValueTypeConstraint != typeParameter2.HasValueTypeConstraint || typeParameter1.Variance != typeParameter2.Variance)
			{
				return false;
			}
			ImmutableArray<TypeSymbol> constraintTypesNoUseSiteDiagnostics = typeParameter1.ConstraintTypesNoUseSiteDiagnostics;
			ImmutableArray<TypeSymbol> constraintTypesNoUseSiteDiagnostics2 = typeParameter2.ConstraintTypesNoUseSiteDiagnostics;
			if (constraintTypesNoUseSiteDiagnostics.Length == 0 && constraintTypesNoUseSiteDiagnostics2.Length == 0)
			{
				return true;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
			ArrayBuilder<TypeSymbol> instance2 = ArrayBuilder<TypeSymbol>.GetInstance();
			SubstituteConstraintTypes(constraintTypesNoUseSiteDiagnostics, instance, typeSubstitution1);
			SubstituteConstraintTypes(constraintTypesNoUseSiteDiagnostics2, instance2, typeSubstitution2);
			bool result = AreConstraintTypesSubset(instance, instance2) && AreConstraintTypesSubset(instance2, instance);
			instance.Free();
			instance2.Free();
			return result;
		}

		private static bool AreConstraintTypesSubset(ArrayBuilder<TypeSymbol> constraintTypes1, ArrayBuilder<TypeSymbol> constraintTypes2)
		{
			ArrayBuilder<TypeSymbol>.Enumerator enumerator = constraintTypes1.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				if (!TypeSymbolExtensions.IsObjectType(current) && !ContainsIgnoringCustomModifiers(constraintTypes2, current))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ContainsIgnoringCustomModifiers(ArrayBuilder<TypeSymbol> types, TypeSymbol type)
		{
			ArrayBuilder<TypeSymbol>.Enumerator enumerator = types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(enumerator.Current, type))
				{
					return true;
				}
			}
			return false;
		}

		private static void SubstituteConstraintTypes(ImmutableArray<TypeSymbol> constraintTypes, ArrayBuilder<TypeSymbol> result, TypeSubstitution substitution)
		{
			ImmutableArray<TypeSymbol>.Enumerator enumerator = constraintTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				result.Add(SubstituteType(substitution, new TypeWithModifiers(current)).Type);
			}
		}
	}
}
