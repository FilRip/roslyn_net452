using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class MemberSignatureComparer : IEqualityComparer<Symbol>
    {
        public static readonly MemberSignatureComparer ExplicitImplementationComparer = new MemberSignatureComparer(considerName: false, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer CSharpImplicitImplementationComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer CSharpCloseImplicitImplementationComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer DuplicateSourceComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: false, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer RecordAPISignatureComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer PartialMethodsComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer CSharpOverrideComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        private static readonly MemberSignatureComparer CSharpWithTupleNamesComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: false, TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreDynamic | TypeCompareKind.IgnoreNativeIntegers);

        private static readonly MemberSignatureComparer CSharpWithoutTupleNamesComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: false, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer CSharpAccessorOverrideComparer = new MemberSignatureComparer(considerName: false, considerExplicitlyImplementedInterfaces: false, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        public static readonly MemberSignatureComparer CSharpCustomModifierOverrideComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        internal static readonly MemberSignatureComparer SloppyOverrideComparer = new MemberSignatureComparer(considerName: false, considerExplicitlyImplementedInterfaces: false, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: false, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

        public static readonly MemberSignatureComparer RuntimeSignatureComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: false, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        public static readonly MemberSignatureComparer RuntimePlusRefOutSignatureComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: true, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        public static readonly MemberSignatureComparer RuntimeImplicitImplementationComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: false, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        public static readonly MemberSignatureComparer CSharpSignatureAndConstraintsAndReturnTypeComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: true, considerReturnType: true, considerTypeConstraints: true, considerCallingConvention: true, considerRefKindDifferences: true, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        public static readonly MemberSignatureComparer RetargetedExplicitImplementationComparer = new MemberSignatureComparer(considerName: true, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: true, considerRefKindDifferences: true, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes | TypeCompareKind.IgnoreNativeIntegers);

        public static readonly MemberSignatureComparer CrefComparer = new MemberSignatureComparer(considerName: false, considerExplicitlyImplementedInterfaces: false, considerReturnType: false, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);

        internal static readonly MemberSignatureComparer MethodGroupSignatureComparer = new MemberSignatureComparer(considerName: false, considerExplicitlyImplementedInterfaces: false, considerReturnType: true, considerTypeConstraints: false, considerCallingConvention: false, considerRefKindDifferences: true, TypeCompareKind.AllIgnoreOptions);

        private readonly bool _considerName;

        private readonly bool _considerExplicitlyImplementedInterfaces;

        private readonly bool _considerReturnType;

        private readonly bool _considerTypeConstraints;

        private readonly bool _considerCallingConvention;

        private readonly bool _considerRefKindDifferences;

        private readonly TypeCompareKind _typeComparison;

        private MemberSignatureComparer(bool considerName, bool considerExplicitlyImplementedInterfaces, bool considerReturnType, bool considerTypeConstraints, bool considerCallingConvention, bool considerRefKindDifferences, TypeCompareKind typeComparison = TypeCompareKind.IgnoreDynamic | TypeCompareKind.IgnoreNativeIntegers)
        {
            _considerName = considerName;
            _considerExplicitlyImplementedInterfaces = considerExplicitlyImplementedInterfaces;
            _considerReturnType = considerReturnType;
            _considerTypeConstraints = considerTypeConstraints;
            _considerCallingConvention = considerCallingConvention;
            _considerRefKindDifferences = considerRefKindDifferences;
            _typeComparison = typeComparison;
            if (!considerRefKindDifferences)
            {
                _typeComparison |= TypeCompareKind.FunctionPointerRefMatchesOutInRefReadonly;
            }
        }

        public bool Equals(Symbol member1, Symbol member2)
        {
            if ((object)member1 == member2)
            {
                return true;
            }
            if ((object)member1 == null || (object)member2 == null || member1.Kind != member2.Kind)
            {
                return false;
            }
            bool flag = false;
            bool flag2 = false;
            if (_considerName)
            {
                string memberNameWithoutInterfaceName = ExplicitInterfaceHelpers.GetMemberNameWithoutInterfaceName(member1.Name);
                string memberNameWithoutInterfaceName2 = ExplicitInterfaceHelpers.GetMemberNameWithoutInterfaceName(member2.Name);
                flag = memberNameWithoutInterfaceName != member1.Name;
                flag2 = memberNameWithoutInterfaceName2 != member2.Name;
                if (memberNameWithoutInterfaceName != memberNameWithoutInterfaceName2)
                {
                    return false;
                }
            }
            if (member1.GetMemberArity() != member2.GetMemberArity() || member1.GetParameterCount() != member2.GetParameterCount())
            {
                return false;
            }
            TypeMap typeMap = GetTypeMap(member1);
            TypeMap typeMap2 = GetTypeMap(member2);
            if (_considerReturnType && !HaveSameReturnTypes(member1, typeMap, member2, typeMap2, _typeComparison))
            {
                return false;
            }
            if (member1.GetParameterCount() > 0 && !HaveSameParameterTypes(member1.GetParameters(), typeMap, member2.GetParameters(), typeMap2, _considerRefKindDifferences, _typeComparison))
            {
                return false;
            }
            if (_considerCallingConvention)
            {
                if (GetCallingConvention(member1) != GetCallingConvention(member2))
                {
                    return false;
                }
            }
            else if (IsVarargMethod(member1) != IsVarargMethod(member2))
            {
                return false;
            }
            if (_considerExplicitlyImplementedInterfaces)
            {
                if (flag != flag2)
                {
                    return false;
                }
                if (flag)
                {
                    if (member1.IsExplicitInterfaceImplementation() != member2.IsExplicitInterfaceImplementation())
                    {
                        return false;
                    }
                    ImmutableArray<Symbol> explicitInterfaceImplementations = member1.GetExplicitInterfaceImplementations();
                    ImmutableArray<Symbol> explicitInterfaceImplementations2 = member2.GetExplicitInterfaceImplementations();
                    if (!explicitInterfaceImplementations.SetEquals(explicitInterfaceImplementations2, SymbolEqualityComparer.ConsiderEverything))
                    {
                        return false;
                    }
                }
            }
            if (_considerTypeConstraints)
            {
                return HaveSameConstraints(member1, typeMap, member2, typeMap2);
            }
            return true;
        }

        public int GetHashCode(Symbol member)
        {
            int num = 1;
            if ((object)member != null)
            {
                num = Hash.Combine((int)member.Kind, num);
                if (_considerName)
                {
                    num = Hash.Combine(ExplicitInterfaceHelpers.GetMemberNameWithoutInterfaceName(member.Name), num);
                }
                if (_considerReturnType && member.GetMemberArity() == 0 && (_typeComparison & TypeCompareKind.AllIgnoreOptions) == 0)
                {
                    num = Hash.Combine(member.GetTypeOrReturnType().GetHashCode(), num);
                }
                if (member.Kind != SymbolKind.Field)
                {
                    num = Hash.Combine(member.GetMemberArity(), num);
                    num = Hash.Combine(member.GetParameterCount(), num);
                }
            }
            return num;
        }

        private static bool HaveSameReturnTypes(Symbol member1, TypeMap typeMap1, Symbol member2, TypeMap typeMap2, TypeCompareKind typeComparison)
        {
            member1.GetTypeOrReturnType(out var refKind, out var returnType, out var refCustomModifiers);
            member2.GetTypeOrReturnType(out var refKind2, out var returnType2, out var refCustomModifiers2);
            if (refKind != refKind2)
            {
                return false;
            }
            bool flag = returnType.IsVoidType();
            bool flag2 = returnType2.IsVoidType();
            if (flag != flag2)
            {
                return false;
            }
            if (flag && ((typeComparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) != 0 || (returnType.CustomModifiers.IsEmpty && returnType2.CustomModifiers.IsEmpty)))
            {
                return true;
            }
            TypeWithAnnotations typeWithAnnotations = SubstituteType(typeMap1, returnType);
            TypeWithAnnotations other = SubstituteType(typeMap2, returnType2);
            if (!typeWithAnnotations.Equals(other, typeComparison))
            {
                return false;
            }
            if ((typeComparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !HaveSameCustomModifiers(refCustomModifiers, typeMap1, refCustomModifiers2, typeMap2))
            {
                return false;
            }
            return true;
        }

        private static TypeMap GetTypeMap(Symbol member)
        {
            ImmutableArray<TypeParameterSymbol> memberTypeParameters = member.GetMemberTypeParameters();
            if (!memberTypeParameters.IsEmpty)
            {
                return new TypeMap(memberTypeParameters, IndexedTypeParameterSymbol.Take(member.GetMemberArity()), allowAlpha: true);
            }
            return null;
        }

        private static bool HaveSameConstraints(Symbol member1, TypeMap typeMap1, Symbol member2, TypeMap typeMap2)
        {
            if (member1.GetMemberArity() == 0)
            {
                return true;
            }
            ImmutableArray<TypeParameterSymbol> memberTypeParameters = member1.GetMemberTypeParameters();
            ImmutableArray<TypeParameterSymbol> memberTypeParameters2 = member2.GetMemberTypeParameters();
            return HaveSameConstraints(memberTypeParameters, typeMap1, memberTypeParameters2, typeMap2);
        }

        public static bool HaveSameConstraints(ImmutableArray<TypeParameterSymbol> typeParameters1, TypeMap typeMap1, ImmutableArray<TypeParameterSymbol> typeParameters2, TypeMap typeMap2)
        {
            int length = typeParameters1.Length;
            for (int i = 0; i < length; i++)
            {
                if (!HaveSameConstraints(typeParameters1[i], typeMap1, typeParameters2[i], typeMap2))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HaveSameConstraints(TypeParameterSymbol typeParameter1, TypeMap typeMap1, TypeParameterSymbol typeParameter2, TypeMap typeMap2)
        {
            if (typeParameter1.HasConstructorConstraint != typeParameter2.HasConstructorConstraint || typeParameter1.HasReferenceTypeConstraint != typeParameter2.HasReferenceTypeConstraint || typeParameter1.HasValueTypeConstraint != typeParameter2.HasValueTypeConstraint || typeParameter1.HasUnmanagedTypeConstraint != typeParameter2.HasUnmanagedTypeConstraint || typeParameter1.Variance != typeParameter2.Variance)
            {
                return false;
            }
            return HaveSameTypeConstraints(typeParameter1, typeMap1, typeParameter2, typeMap2, SymbolEqualityComparer.IgnoringDynamicTupleNamesAndNullability);
        }

        private static bool HaveSameTypeConstraints(TypeParameterSymbol typeParameter1, TypeMap typeMap1, TypeParameterSymbol typeParameter2, TypeMap typeMap2, IEqualityComparer<TypeSymbol> comparer)
        {
            ImmutableArray<TypeWithAnnotations> constraintTypesNoUseSiteDiagnostics = typeParameter1.ConstraintTypesNoUseSiteDiagnostics;
            ImmutableArray<TypeWithAnnotations> constraintTypesNoUseSiteDiagnostics2 = typeParameter2.ConstraintTypesNoUseSiteDiagnostics;
            if (constraintTypesNoUseSiteDiagnostics.Length == 0 && constraintTypesNoUseSiteDiagnostics2.Length == 0)
            {
                return true;
            }
            HashSet<TypeSymbol> hashSet = new HashSet<TypeSymbol>(comparer);
            HashSet<TypeSymbol> hashSet2 = new HashSet<TypeSymbol>(comparer);
            SubstituteConstraintTypes(constraintTypesNoUseSiteDiagnostics, typeMap1, hashSet);
            SubstituteConstraintTypes(constraintTypesNoUseSiteDiagnostics2, typeMap2, hashSet2);
            if (AreConstraintTypesSubset(hashSet, hashSet2, typeParameter2))
            {
                return AreConstraintTypesSubset(hashSet2, hashSet, typeParameter1);
            }
            return false;
        }

        public static bool HaveSameNullabilityInConstraints(TypeParameterSymbol typeParameter1, TypeMap typeMap1, TypeParameterSymbol typeParameter2, TypeMap typeMap2)
        {
            if (!typeParameter1.IsValueType)
            {
                bool? isNotNullable = typeParameter1.IsNotNullable;
                bool? isNotNullable2 = typeParameter2.IsNotNullable;
                if (isNotNullable.HasValue && isNotNullable2.HasValue && isNotNullable.GetValueOrDefault() != isNotNullable2.GetValueOrDefault())
                {
                    return false;
                }
            }
            return HaveSameTypeConstraints(typeParameter1, typeMap1, typeParameter2, typeMap2, SymbolEqualityComparer.AllIgnoreOptionsPlusNullableWithUnknownMatchesAny);
        }

        private static bool AreConstraintTypesSubset(HashSet<TypeSymbol> constraintTypes1, HashSet<TypeSymbol> constraintTypes2, TypeParameterSymbol typeParameter2)
        {
            foreach (TypeSymbol item in constraintTypes1)
            {
                if (item.SpecialType != SpecialType.System_Object && !constraintTypes2.Contains(item) && (item.SpecialType != SpecialType.System_ValueType || !typeParameter2.HasValueTypeConstraint))
                {
                    return false;
                }
            }
            return true;
        }

        private static void SubstituteConstraintTypes(ImmutableArray<TypeWithAnnotations> types, TypeMap typeMap, HashSet<TypeSymbol> result)
        {
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations current = enumerator.Current;
                result.Add(typeMap.SubstituteType(current).Type);
            }
        }

        private static bool HaveSameParameterTypes(ImmutableArray<ParameterSymbol> params1, TypeMap typeMap1, ImmutableArray<ParameterSymbol> params2, TypeMap typeMap2, bool considerRefKindDifferences, TypeCompareKind typeComparison)
        {
            int length = params1.Length;
            for (int i = 0; i < length; i++)
            {
                ParameterSymbol parameterSymbol = params1[i];
                ParameterSymbol parameterSymbol2 = params2[i];
                TypeWithAnnotations typeWithAnnotations = SubstituteType(typeMap1, parameterSymbol.TypeWithAnnotations);
                TypeWithAnnotations other = SubstituteType(typeMap2, parameterSymbol2.TypeWithAnnotations);
                if (!typeWithAnnotations.Equals(other, typeComparison))
                {
                    return false;
                }
                if ((typeComparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !HaveSameCustomModifiers(parameterSymbol.RefCustomModifiers, typeMap1, parameterSymbol2.RefCustomModifiers, typeMap2))
                {
                    return false;
                }
                RefKind refKind = parameterSymbol.RefKind;
                RefKind refKind2 = parameterSymbol2.RefKind;
                if (considerRefKindDifferences)
                {
                    if (refKind != refKind2)
                    {
                        return false;
                    }
                }
                else if (refKind == RefKind.None != (refKind2 == RefKind.None))
                {
                    return false;
                }
            }
            return true;
        }

        private static TypeWithAnnotations SubstituteType(TypeMap typeMap, TypeWithAnnotations typeSymbol)
        {
            if (typeMap != null)
            {
                return typeSymbol.SubstituteType(typeMap);
            }
            return typeSymbol;
        }

        private static bool HaveSameCustomModifiers(ImmutableArray<CustomModifier> customModifiers1, TypeMap typeMap1, ImmutableArray<CustomModifier> customModifiers2, TypeMap typeMap2)
        {
            return SubstituteModifiers(typeMap1, customModifiers1).SequenceEqual(SubstituteModifiers(typeMap2, customModifiers2));
        }

        private static ImmutableArray<CustomModifier> SubstituteModifiers(TypeMap typeMap, ImmutableArray<CustomModifier> customModifiers)
        {
            return typeMap?.SubstituteCustomModifiers(customModifiers) ?? customModifiers;
        }

        private static CallingConvention GetCallingConvention(Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    return ((MethodSymbol)member).CallingConvention;
                case SymbolKind.Event:
                case SymbolKind.Property:
                    if (!member.IsStatic)
                    {
                        return CallingConvention.HasThis;
                    }
                    return CallingConvention.Default;
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        private static bool IsVarargMethod(Symbol member)
        {
            if (member.Kind == SymbolKind.Method)
            {
                return ((MethodSymbol)member).IsVararg;
            }
            return false;
        }

        internal static bool ConsideringTupleNamesCreatesDifference(Symbol member1, Symbol member2)
        {
            if (!CSharpWithTupleNamesComparer.Equals(member1, member2))
            {
                return CSharpWithoutTupleNamesComparer.Equals(member1, member2);
            }
            return false;
        }
    }
}
