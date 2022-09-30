using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class TypeSymbolExtensions
    {
        private static readonly string[] s_expressionsNamespaceName = new string[4] { "Expressions", "Linq", "System", "" };

        private static readonly Func<TypeSymbol, TypeParameterSymbol?, bool, bool> s_containsTypeParameterPredicate = (TypeSymbol type, TypeParameterSymbol? parameter, bool unused) => type.TypeKind == TypeKind.TypeParameter && ((object)parameter == null || TypeSymbol.Equals(type, parameter, TypeCompareKind.ConsiderEverything));

        private static readonly Func<TypeSymbol, Symbol, bool, bool> s_isTypeParameterWithSpecificContainerPredicate = (TypeSymbol type, Symbol parameterContainer, bool unused) => type.TypeKind == TypeKind.TypeParameter && (object)type.ContainingSymbol == parameterContainer;

        private static readonly Func<TypeSymbol, HashSet<TypeParameterSymbol>, bool, bool> s_containsTypeParametersPredicate = (TypeSymbol type, HashSet<TypeParameterSymbol> parameters, bool unused) => type.TypeKind == TypeKind.TypeParameter && parameters.Contains((TypeParameterSymbol)type);

        private static readonly Func<TypeSymbol, object?, bool, bool> s_containsDynamicPredicate = (TypeSymbol type, object? unused1, bool unused2) => type.TypeKind == TypeKind.Dynamic;

        public static bool ImplementsInterface(this TypeSymbol subType, TypeSymbol superInterface, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = subType.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (current.IsInterface && TypeSymbol.Equals(current, superInterface, TypeCompareKind.ConsiderEverything))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanBeAssignedNull(this TypeSymbol type)
        {
            if (!type.IsReferenceType && !type.IsPointerOrFunctionPointer())
            {
                return type.IsNullableType();
            }
            return true;
        }

        public static bool CanContainNull(this TypeSymbol type)
        {
            if (type.IsValueType)
            {
                return type.IsNullableTypeOrTypeParameter();
            }
            return true;
        }

        public static bool CanBeConst(this TypeSymbol typeSymbol)
        {
            if (!typeSymbol.IsReferenceType && !typeSymbol.IsEnumType() && !typeSymbol.SpecialType.CanBeConst())
            {
                return typeSymbol.IsNativeIntegerType;
            }
            return true;
        }

        public static bool IsTypeParameterDisallowingAnnotationInCSharp8(this TypeSymbol type)
        {
            if (type.TypeKind != TypeKind.TypeParameter)
            {
                return false;
            }
            TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
            if (!typeParameterSymbol.IsValueType)
            {
                if (typeParameterSymbol.IsReferenceType)
                {
                    return typeParameterSymbol.IsNotNullable != true;
                }
                return true;
            }
            return false;
        }

        public static bool IsPossiblyNullableReferenceTypeTypeParameter(this TypeSymbol type)
        {
            if (type is TypeParameterSymbol typeParameterSymbol && !typeParameterSymbol.IsValueType)
            {
                bool? isNotNullable = typeParameterSymbol.IsNotNullable;
                if (isNotNullable.HasValue)
                {
                    return !isNotNullable.GetValueOrDefault();
                }
            }
            return false;
        }

        public static bool IsNonNullableValueType(this TypeSymbol typeArgument)
        {
            if (!typeArgument.IsValueType)
            {
                return false;
            }
            return !typeArgument.IsNullableTypeOrTypeParameter();
        }

        public static bool IsVoidType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Void;
        }

        public static bool IsNullableTypeOrTypeParameter(this TypeSymbol? type)
        {
            if ((object)type == null)
            {
                return false;
            }
            if (type!.TypeKind == TypeKind.TypeParameter)
            {
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = ((TypeParameterSymbol)type).ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Type.IsNullableTypeOrTypeParameter())
                    {
                        return true;
                    }
                }
                return false;
            }
            return type.IsNullableType();
        }

        public static bool IsNullableType(this TypeSymbol type)
        {
            return type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
        }

        public static TypeSymbol GetNullableUnderlyingType(this TypeSymbol type)
        {
            return type.GetNullableUnderlyingTypeWithAnnotations().Type;
        }

        public static TypeWithAnnotations GetNullableUnderlyingTypeWithAnnotations(this TypeSymbol type)
        {
            return ((NamedTypeSymbol)type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0];
        }

        public static TypeSymbol StrippedType(this TypeSymbol type)
        {
            if (!type.IsNullableType())
            {
                return type;
            }
            return type.GetNullableUnderlyingType();
        }

        public static TypeSymbol EnumUnderlyingTypeOrSelf(this TypeSymbol type)
        {
            return type.GetEnumUnderlyingType() ?? type;
        }

        public static bool IsNativeIntegerOrNullableNativeIntegerType(this TypeSymbol? type)
        {
            return type?.StrippedType().IsNativeIntegerType ?? false;
        }

        public static bool IsObjectType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Object;
        }

        public static bool IsStringType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_String;
        }

        public static bool IsCharType(this TypeSymbol type)
        {
            return type.SpecialType == SpecialType.System_Char;
        }

        public static bool IsIntegralType(this TypeSymbol type)
        {
            return type.SpecialType.IsIntegralType();
        }

        public static NamedTypeSymbol? GetEnumUnderlyingType(this TypeSymbol? type)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol))
            {
                return null;
            }
            return namedTypeSymbol.EnumUnderlyingType;
        }

        public static bool IsEnumType(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Enum;
        }

        public static bool IsValidEnumType(this TypeSymbol type)
        {
            NamedTypeSymbol enumUnderlyingType = type.GetEnumUnderlyingType();
            if ((object)enumUnderlyingType != null)
            {
                return enumUnderlyingType.SpecialType != SpecialType.None;
            }
            return false;
        }

        public static bool IsValidAttributeParameterType(this TypeSymbol type, CSharpCompilation compilation)
        {
            return type.GetAttributeParameterTypedConstantKind(compilation) != TypedConstantKind.Error;
        }

        public static TypedConstantKind GetAttributeParameterTypedConstantKind(this TypeSymbol type, CSharpCompilation compilation)
        {
            TypedConstantKind typedConstantKind = TypedConstantKind.Error;
            if ((object)type == null)
            {
                return TypedConstantKind.Error;
            }
            if (type.Kind == SymbolKind.ArrayType)
            {
                ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
                if (!arrayTypeSymbol.IsSZArray)
                {
                    return TypedConstantKind.Error;
                }
                typedConstantKind = TypedConstantKind.Array;
                type = arrayTypeSymbol.ElementType;
            }
            if (type.IsEnumType())
            {
                if (typedConstantKind == TypedConstantKind.Error)
                {
                    typedConstantKind = TypedConstantKind.Enum;
                }
                type = type.GetEnumUnderlyingType();
            }
            TypedConstantKind typedConstantKind2 = TypedConstant.GetTypedConstantKind(type, compilation);
            switch (typedConstantKind2)
            {
                case TypedConstantKind.Error:
                case TypedConstantKind.Enum:
                case TypedConstantKind.Array:
                    return TypedConstantKind.Error;
                default:
                    if (typedConstantKind == TypedConstantKind.Array || typedConstantKind == TypedConstantKind.Enum)
                    {
                        return typedConstantKind;
                    }
                    return typedConstantKind2;
            }
        }

        public static bool IsValidExtensionParameterType(this TypeSymbol type)
        {
            TypeKind typeKind = type.TypeKind;
            if (typeKind == TypeKind.Dynamic || typeKind == TypeKind.Pointer || typeKind == TypeKind.FunctionPointer)
            {
                return false;
            }
            return true;
        }

        public static bool IsInterfaceType(this TypeSymbol type)
        {
            if (type.Kind == SymbolKind.NamedType)
            {
                return ((NamedTypeSymbol)type).IsInterface;
            }
            return false;
        }

        public static bool IsClassType(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Class;
        }

        public static bool IsStructType(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Struct;
        }

        public static bool IsErrorType(this TypeSymbol type)
        {
            return type.Kind == SymbolKind.ErrorType;
        }

        public static bool IsMethodTypeParameter(this TypeParameterSymbol p)
        {
            return p.ContainingSymbol.Kind == SymbolKind.Method;
        }

        public static bool IsDynamic(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Dynamic;
        }

        public static bool IsTypeParameter(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.TypeParameter;
        }

        public static bool IsArray(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Array;
        }

        public static bool IsSZArray(this TypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Array)
            {
                return ((ArrayTypeSymbol)type).IsSZArray;
            }
            return false;
        }

        public static bool IsFunctionPointer(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.FunctionPointer;
        }

        public static bool IsPointerOrFunctionPointer(this TypeSymbol type)
        {
            TypeKind typeKind = type.TypeKind;
            if (typeKind == TypeKind.Pointer || typeKind == TypeKind.FunctionPointer)
            {
                return true;
            }
            return false;
        }

        public static NamedTypeSymbol? GetDelegateType(this TypeSymbol? type)
        {
            if ((object)type == null)
            {
                return null;
            }
            if (type.IsExpressionTree())
            {
                type = ((NamedTypeSymbol)type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type;
            }
            if (!type.IsDelegateType())
            {
                return null;
            }
            return (NamedTypeSymbol)type;
        }

        public static TypeSymbol? GetDelegateOrFunctionPointerType(this TypeSymbol? type)
        {
            return (TypeSymbol?)(type.GetDelegateType() ?? ((object)(type as FunctionPointerTypeSymbol)));
        }

        public static bool IsExpressionTree(this TypeSymbol type)
        {
            return type.IsGenericOrNonGenericExpressionType(out bool isGenericType) && isGenericType;
        }

        public static bool IsNonGenericExpressionType(this TypeSymbol type)
        {
            if (type.IsGenericOrNonGenericExpressionType(out var isGenericType))
            {
                return !isGenericType;
            }
            return false;
        }

        public static bool IsGenericOrNonGenericExpressionType(this TypeSymbol _type, out bool isGenericType)
        {
            if (_type.OriginalDefinition is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == "Expression" && CheckFullName(namedTypeSymbol.ContainingSymbol, s_expressionsNamespaceName))
            {
                if (namedTypeSymbol.Arity == 0)
                {
                    isGenericType = false;
                    return true;
                }
                if (namedTypeSymbol.Arity == 1 && namedTypeSymbol.MangleName)
                {
                    isGenericType = true;
                    return true;
                }
            }
            isGenericType = false;
            return false;
        }

        public static bool IsPossibleArrayGenericInterface(this TypeSymbol type)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            NamedTypeSymbol originalDefinition = namedTypeSymbol.OriginalDefinition;
            SpecialType specialType = originalDefinition.SpecialType;
            if (specialType == SpecialType.System_Collections_Generic_IList_T || specialType == SpecialType.System_Collections_Generic_ICollection_T || specialType == SpecialType.System_Collections_Generic_IEnumerable_T || specialType == SpecialType.System_Collections_Generic_IReadOnlyList_T || specialType == SpecialType.System_Collections_Generic_IReadOnlyCollection_T)
            {
                return true;
            }
            return false;
        }

        private static bool CheckFullName(Symbol symbol, string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if ((object)symbol == null || symbol.Name != names[i])
                {
                    return false;
                }
                symbol = symbol.ContainingSymbol;
            }
            return true;
        }

        public static bool IsDelegateType(this TypeSymbol type)
        {
            return type.TypeKind == TypeKind.Delegate;
        }

        public static ImmutableArray<ParameterSymbol> DelegateParameters(this TypeSymbol type)
        {
            return type.DelegateInvokeMethod()?.Parameters ?? default(ImmutableArray<ParameterSymbol>);
        }

        public static ImmutableArray<ParameterSymbol> DelegateOrFunctionPointerParameters(this TypeSymbol type)
        {
            if (type is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                if ((object)signature != null)
                {
                    return signature.Parameters;
                }
            }
            return type.DelegateParameters();
        }

        public static bool TryGetElementTypesWithAnnotationsIfTupleType(this TypeSymbol type, out ImmutableArray<TypeWithAnnotations> elementTypes)
        {
            if (type.IsTupleType)
            {
                elementTypes = ((NamedTypeSymbol)type).TupleElementTypesWithAnnotations;
                return true;
            }
            elementTypes = default(ImmutableArray<TypeWithAnnotations>);
            return false;
        }

        public static MethodSymbol DelegateInvokeMethod(this TypeSymbol type)
        {
            return type.GetDelegateType()!.DelegateInvokeMethod;
        }

        public static ConstantValue? GetDefaultValue(this TypeSymbol type)
        {
            if (type.IsErrorType())
            {
                return null;
            }
            if (type.IsReferenceType)
            {
                return ConstantValue.Null;
            }
            if (type.IsValueType)
            {
                type = type.EnumUnderlyingTypeOrSelf();
                switch (type.SpecialType)
                {
                    case SpecialType.System_IntPtr:
                        if (!type.IsNativeIntegerType)
                        {
                            break;
                        }
                        goto case SpecialType.System_Boolean;
                    case SpecialType.System_UIntPtr:
                        if (!type.IsNativeIntegerType)
                        {
                            break;
                        }
                        goto case SpecialType.System_Boolean;
                    case SpecialType.System_Boolean:
                    case SpecialType.System_Char:
                    case SpecialType.System_SByte:
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Decimal:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                        return ConstantValue.Default(type.SpecialType);
                }
            }
            return null;
        }

        public static SpecialType GetSpecialTypeSafe(this TypeSymbol? type)
        {
            return type?.SpecialType ?? SpecialType.None;
        }

        public static bool IsAtLeastAsVisibleAs(this TypeSymbol type, Symbol sym, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            CompoundUseSiteInfo<AssemblySymbol> localUseSiteInfo = useSiteInfo;
            TypeSymbol? typeSymbol = type.VisitType((TypeSymbol type1, Symbol symbol, bool unused) => IsTypeLessVisibleThan(type1, symbol, ref localUseSiteInfo), sym, canDigThroughNullable: true);
            useSiteInfo = localUseSiteInfo;
            return (object)typeSymbol == null;
        }

        private static bool IsTypeLessVisibleThan(TypeSymbol type, Symbol sym, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Struct:
                case TypeKind.Submission:
                    return !IsAsRestrictive((NamedTypeSymbol)type, sym, ref useSiteInfo);
                default:
                    return false;
            }
        }

        public static TypeSymbol? VisitType<T>(this TypeSymbol type, Func<TypeSymbol, T, bool, bool> predicate, T arg, bool canDigThroughNullable = false)
        {
            return default(TypeWithAnnotations).VisitType(type, null, predicate, arg, canDigThroughNullable);
        }

        public static TypeSymbol? VisitType<T>(this TypeWithAnnotations typeWithAnnotationsOpt, TypeSymbol? type, Func<TypeWithAnnotations, T, bool, bool>? typeWithAnnotationsPredicate, Func<TypeSymbol, T, bool, bool>? typePredicate, T arg, bool canDigThroughNullable = false, bool useDefaultType = false)
        {
            TypeSymbol typeSymbol;
            while (true)
            {
                typeSymbol = type ?? (useDefaultType ? typeWithAnnotationsOpt.DefaultType : typeWithAnnotationsOpt.Type);
                bool arg2 = false;
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
                                arg2 = true;
                                TypeSymbol typeSymbol2 = default(TypeWithAnnotations).VisitType(containingType, typeWithAnnotationsPredicate, typePredicate, arg, canDigThroughNullable, useDefaultType);
                                if ((object)typeSymbol2 != null)
                                {
                                    return typeSymbol2;
                                }
                            }
                            break;
                        }
                }
                if (typeWithAnnotationsOpt.HasType && typeWithAnnotationsPredicate != null)
                {
                    if (typeWithAnnotationsPredicate!(typeWithAnnotationsOpt, arg, arg2))
                    {
                        return typeSymbol;
                    }
                }
                else if (typePredicate != null && typePredicate!(typeSymbol, arg, arg2))
                {
                    break;
                }
                TypeWithAnnotations next2;
                switch (typeSymbol.TypeKind)
                {
                    case TypeKind.Dynamic:
                    case TypeKind.Enum:
                    case TypeKind.TypeParameter:
                    case TypeKind.Submission:
                        return null;
                    case TypeKind.Class:
                    case TypeKind.Delegate:
                    case TypeKind.Error:
                    case TypeKind.Interface:
                    case TypeKind.Struct:
                        {
                            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = ((NamedTypeSymbol)typeSymbol).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                            if (typeArgumentsWithAnnotationsNoUseSiteDiagnostics.IsEmpty)
                            {
                                return null;
                            }
                            int i;
                            for (i = 0; i < typeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length - 1; i++)
                            {
                                (TypeWithAnnotations, TypeSymbol?) tuple = getNextIterationElements(typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i], canDigThroughNullable);
                                TypeWithAnnotations item = tuple.Item1;
                                TypeSymbol item2 = tuple.Item2;
                                TypeSymbol typeSymbol4 = item.VisitType(item2, typeWithAnnotationsPredicate, typePredicate, arg, canDigThroughNullable, useDefaultType);
                                if ((object)typeSymbol4 != null)
                                {
                                    return typeSymbol4;
                                }
                            }
                            next2 = typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i];
                            break;
                        }
                    case TypeKind.Array:
                        next2 = ((ArrayTypeSymbol)typeSymbol).ElementTypeWithAnnotations;
                        break;
                    case TypeKind.Pointer:
                        next2 = ((PointerTypeSymbol)typeSymbol).PointedAtTypeWithAnnotations;
                        break;
                    case TypeKind.FunctionPointer:
                        {
                            TypeSymbol typeSymbol3 = visitFunctionPointerType((FunctionPointerTypeSymbol)typeSymbol, typeWithAnnotationsPredicate, typePredicate, arg, useDefaultType, canDigThroughNullable, out next2);
                            if ((object)typeSymbol3 != null)
                            {
                                return typeSymbol3;
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(typeSymbol.TypeKind);
                }
                typeWithAnnotationsOpt = (canDigThroughNullable ? default(TypeWithAnnotations) : next2);
                type = (canDigThroughNullable ? next2.NullableUnderlyingTypeOrSelf : null);
            }
            return typeSymbol;
            static (TypeWithAnnotations, TypeSymbol?) getNextIterationElements(TypeWithAnnotations type, bool canDigThroughNullable)
            {
                if (!canDigThroughNullable)
                {
                    return (type, null);
                }
                return (default(TypeWithAnnotations), type.NullableUnderlyingTypeOrSelf);
            }
            static TypeSymbol? visitFunctionPointerType(FunctionPointerTypeSymbol type, Func<TypeWithAnnotations, T, bool, bool>? typeWithAnnotationsPredicate, Func<TypeSymbol, T, bool, bool>? typePredicate, T arg, bool useDefaultType, bool canDigThroughNullable, out TypeWithAnnotations next)
            {
                MethodSymbol signature = type.Signature;
                if (signature.ParameterCount == 0)
                {
                    next = signature.ReturnTypeWithAnnotations;
                    return null;
                }
                TypeSymbol typeSymbol5 = (canDigThroughNullable ? default(TypeWithAnnotations) : signature.ReturnTypeWithAnnotations).VisitType(canDigThroughNullable ? signature.ReturnTypeWithAnnotations.NullableUnderlyingTypeOrSelf : null, typeWithAnnotationsPredicate, typePredicate, arg, canDigThroughNullable, useDefaultType);
                if ((object)typeSymbol5 != null)
                {
                    next = default(TypeWithAnnotations);
                    return typeSymbol5;
                }
                int j;
                for (j = 0; j < signature.ParameterCount - 1; j++)
                {
                    (TypeWithAnnotations, TypeSymbol?) nextIterationElements = getNextIterationElements(signature.Parameters[j].TypeWithAnnotations, canDigThroughNullable);
                    TypeWithAnnotations item3 = nextIterationElements.Item1;
                    TypeSymbol item4 = nextIterationElements.Item2;
                    typeSymbol5 = item3.VisitType(item4, typeWithAnnotationsPredicate, typePredicate, arg, canDigThroughNullable, useDefaultType);
                    if ((object)typeSymbol5 != null)
                    {
                        next = default(TypeWithAnnotations);
                        return typeSymbol5;
                    }
                }
                next = signature.Parameters[j].TypeWithAnnotations;
                return null;
            }
        }

        private static bool IsAsRestrictive(NamedTypeSymbol s1, Symbol sym2, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            Accessibility declaredAccessibility = s1.DeclaredAccessibility;
            if (declaredAccessibility == Accessibility.Public)
            {
                return true;
            }
            Symbol symbol = sym2;
            while (symbol.Kind != SymbolKind.Namespace)
            {
                Accessibility declaredAccessibility2 = symbol.DeclaredAccessibility;
                switch (declaredAccessibility)
                {
                    case Accessibility.Internal:
                        if ((declaredAccessibility2 == Accessibility.Private || declaredAccessibility2 == Accessibility.Internal || declaredAccessibility2 == Accessibility.ProtectedAndInternal) && symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly))
                        {
                            return true;
                        }
                        break;
                    case Accessibility.ProtectedAndInternal:
                        if ((declaredAccessibility2 != Accessibility.Private && declaredAccessibility2 != Accessibility.Internal && declaredAccessibility2 != Accessibility.ProtectedAndInternal) || !symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly))
                        {
                            break;
                        }
                        goto case Accessibility.Protected;
                    case Accessibility.Protected:
                        {
                            NamedTypeSymbol containingType3 = s1.ContainingType;
                            if ((object)containingType3 == null)
                            {
                                break;
                            }
                            switch (declaredAccessibility2)
                            {
                                case Accessibility.Private:
                                    {
                                        NamedTypeSymbol containingType5 = symbol.ContainingType;
                                        while ((object)containingType5 != null)
                                        {
                                            if (containingType3.IsAccessibleViaInheritance(containingType5, ref useSiteInfo))
                                            {
                                                return true;
                                            }
                                            containingType5 = containingType5.ContainingType;
                                        }
                                        break;
                                    }
                                case Accessibility.ProtectedAndInternal:
                                case Accessibility.Protected:
                                    {
                                        NamedTypeSymbol containingType4 = symbol.ContainingType;
                                        if ((object)containingType4 != null && containingType3.IsAccessibleViaInheritance(containingType4, ref useSiteInfo))
                                        {
                                            return true;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case Accessibility.ProtectedOrInternal:
                        {
                            NamedTypeSymbol containingType6 = s1.ContainingType;
                            if ((object)containingType6 == null)
                            {
                                break;
                            }
                            switch (declaredAccessibility2)
                            {
                                case Accessibility.Private:
                                    {
                                        if (symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly))
                                        {
                                            return true;
                                        }
                                        NamedTypeSymbol containingType7 = symbol.ContainingType;
                                        while ((object)containingType7 != null)
                                        {
                                            if (containingType6.IsAccessibleViaInheritance(containingType7, ref useSiteInfo))
                                            {
                                                return true;
                                            }
                                            containingType7 = containingType7.ContainingType;
                                        }
                                        break;
                                    }
                                case Accessibility.Internal:
                                    if (symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly))
                                    {
                                        return true;
                                    }
                                    break;
                                case Accessibility.Protected:
                                    if (containingType6.IsAccessibleViaInheritance(symbol.ContainingType, ref useSiteInfo))
                                    {
                                        return true;
                                    }
                                    break;
                                case Accessibility.ProtectedAndInternal:
                                    if (symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly) || containingType6.IsAccessibleViaInheritance(symbol.ContainingType, ref useSiteInfo))
                                    {
                                        return true;
                                    }
                                    break;
                                case Accessibility.ProtectedOrInternal:
                                    if (symbol.ContainingAssembly.HasInternalAccessTo(s1.ContainingAssembly) && containingType6.IsAccessibleViaInheritance(symbol.ContainingType, ref useSiteInfo))
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            break;
                        }
                    case Accessibility.Private:
                        {
                            if (declaredAccessibility2 != Accessibility.Private)
                            {
                                break;
                            }
                            NamedTypeSymbol containingType = s1.ContainingType;
                            if ((object)containingType == null)
                            {
                                break;
                            }
                            NamedTypeSymbol originalDefinition = containingType.OriginalDefinition;
                            NamedTypeSymbol containingType2 = symbol.ContainingType;
                            while ((object)containingType2 != null)
                            {
                                if ((object)containingType2.OriginalDefinition == originalDefinition || (originalDefinition.TypeKind == TypeKind.Submission && containingType2.TypeKind == TypeKind.Submission))
                                {
                                    return true;
                                }
                                containingType2 = containingType2.ContainingType;
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(declaredAccessibility);
                }
                symbol = symbol.ContainingSymbol;
            }
            return false;
        }

        public static bool IsUnboundGenericType(this TypeSymbol type)
        {
            if (type is NamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.IsUnboundGenericType;
            }
            return false;
        }

        public static bool IsTopLevelType(this NamedTypeSymbol type)
        {
            return (object)type.ContainingType == null;
        }

        public static bool ContainsTypeParameter(this TypeSymbol type, TypeParameterSymbol? parameter = null)
        {
            return (object)type.VisitType<TypeParameterSymbol>(s_containsTypeParameterPredicate, parameter) != null;
        }

        public static bool ContainsTypeParameter(this TypeSymbol type, MethodSymbol parameterContainer)
        {
            return (object)type.VisitType(s_isTypeParameterWithSpecificContainerPredicate, parameterContainer) != null;
        }

        public static bool ContainsTypeParameters(this TypeSymbol type, HashSet<TypeParameterSymbol> parameters)
        {
            return (object)type.VisitType<HashSet<TypeParameterSymbol>>(s_containsTypeParametersPredicate, parameters) != null;
        }

        public static bool ContainsDynamic(this TypeSymbol type)
        {
            return (object)type.VisitType(s_containsDynamicPredicate, null, canDigThroughNullable: true) != null;
        }

        internal static bool ContainsNativeInteger(this TypeSymbol type)
        {
            return (object)type.VisitType((TypeSymbol type, object unused1, bool unused2) => type.IsNativeIntegerType, null, canDigThroughNullable: true) != null;
        }

        internal static bool ContainsNativeInteger(this TypeWithAnnotations type)
        {
            return type.Type?.ContainsNativeInteger() ?? false;
        }

        internal static bool ContainsErrorType(this TypeSymbol type)
        {
            return (object)type.VisitType((TypeSymbol type, object unused1, bool unused2) => type.IsErrorType(), null, canDigThroughNullable: true) != null;
        }

        internal static bool ContainsTuple(this TypeSymbol type)
        {
            return (object)type.VisitType((TypeSymbol t, object? _1, bool _2) => t.IsTupleType, null) != null;
        }

        internal static bool ContainsTupleNames(this TypeSymbol type)
        {
            return (object)type.VisitType((TypeSymbol t, object? _1, bool _2) => !t.TupleElementNames.IsDefault, null) != null;
        }

        internal static bool ContainsFunctionPointer(this TypeSymbol type)
        {
            return (object)type.VisitType((TypeSymbol t, object? _, bool _) => t.IsFunctionPointer(), null) != null;
        }

        internal static TypeSymbol? GetNonErrorGuess(this TypeSymbol type)
        {
            return ExtendedErrorTypeSymbol.ExtractNonErrorType(type);
        }

        internal static TypeKind GetNonErrorTypeKindGuess(this TypeSymbol type)
        {
            return ExtendedErrorTypeSymbol.ExtractNonErrorTypeKind(type);
        }

        internal static bool IsValidV6SwitchGoverningType(this TypeSymbol type, bool isTargetTypeOfUserDefinedOp = false)
        {
            if (type.IsNullableType())
            {
                type = type.GetNullableUnderlyingType();
            }
            if (!isTargetTypeOfUserDefinedOp)
            {
                type = type.EnumUnderlyingTypeOrSelf();
            }
            switch (type.SpecialType)
            {
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_String:
                    return true;
                case SpecialType.System_Boolean:
                    return !isTargetTypeOfUserDefinedOp;
                default:
                    return false;
            }
        }

        internal static bool IsRestrictedType(this TypeSymbol type, bool ignoreSpanLikeTypes = false)
        {
            SpecialType specialType = type.SpecialType;
            if ((uint)(specialType - 36) <= 2u)
            {
                return true;
            }
            if (!ignoreSpanLikeTypes)
            {
                return type.IsRefLikeType;
            }
            return false;
        }

        public static bool IsIntrinsicType(this TypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_IntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Boolean;
                case SpecialType.System_UIntPtr:
                    if (!type.IsNativeIntegerType)
                    {
                        break;
                    }
                    goto case SpecialType.System_Boolean;
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return true;
            }
            return false;
        }

        public static bool IsPartial(this TypeSymbol type)
        {
            if (type is SourceNamedTypeSymbol sourceNamedTypeSymbol)
            {
                return sourceNamedTypeSymbol.IsPartial;
            }
            return false;
        }

        public static bool IsPointerType(this TypeSymbol type)
        {
            return type is PointerTypeSymbol;
        }

        internal static int FixedBufferElementSizeInBytes(this TypeSymbol type)
        {
            return type.SpecialType.FixedBufferElementSizeInBytes();
        }

        internal static bool IsValidVolatileFieldType(this TypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Struct:
                    return type.SpecialType.IsValidVolatileFieldType();
                case TypeKind.Array:
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Error:
                case TypeKind.Interface:
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                    return true;
                case TypeKind.Enum:
                    return ((NamedTypeSymbol)type).EnumUnderlyingType.SpecialType.IsValidVolatileFieldType();
                case TypeKind.TypeParameter:
                    return type.IsReferenceType;
                case TypeKind.Submission:
                    throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
                default:
                    return false;
            }
        }

        public static bool MarkCheckedIfNecessary(this TypeSymbol type, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (checkedTypes == null)
            {
                checkedTypes = new HashSet<TypeSymbol>();
            }
            return checkedTypes.Add(type);
        }

        internal static bool IsUnsafe(this TypeSymbol type)
        {
            while (true)
            {
                switch (type.TypeKind)
                {
                    case TypeKind.Pointer:
                    case TypeKind.FunctionPointer:
                        return true;
                    case TypeKind.Array:
                        break;
                    default:
                        return false;
                }
                type = ((ArrayTypeSymbol)type).ElementType;
            }
        }

        internal static bool IsVoidPointer(this TypeSymbol type)
        {
            if (type is PointerTypeSymbol pointerTypeSymbol)
            {
                return pointerTypeSymbol.PointedAtType.IsVoidType();
            }
            return false;
        }

        internal static bool IsPrimitiveRecursiveStruct(this TypeSymbol t)
        {
            return t.SpecialType.IsPrimitiveRecursiveStruct();
        }

        internal static int ComputeHashCode(this NamedTypeSymbol type)
        {
            if (wasConstructedForAnnotations(type))
            {
                return type.OriginalDefinition.GetHashCode();
            }
            int hashCode = type.OriginalDefinition.GetHashCode();
            hashCode = Hash.Combine(type.ContainingType, hashCode);
            if ((object)type.ConstructedFrom != type)
            {
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    hashCode = Hash.Combine(enumerator.Current.Type, hashCode);
                }
            }
            if (hashCode == 0)
            {
                hashCode++;
            }
            return hashCode;
            static bool wasConstructedForAnnotations(NamedTypeSymbol type)
            {
                do
                {
                    ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                    ImmutableArray<TypeParameterSymbol> typeParameters = type.OriginalDefinition.TypeParameters;
                    for (int i = 0; i < typeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length; i++)
                    {
                        if (!typeParameters[i].Equals(typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i].Type.OriginalDefinition, TypeCompareKind.ConsiderEverything))
                        {
                            return false;
                        }
                    }
                    type = type.ContainingType;
                }
                while ((object)type != null && !type.IsDefinition);
                return true;
            }
        }

        public static TypeSymbol AsDynamicIfNoPia(this TypeSymbol type, NamedTypeSymbol containingType)
        {
            if (!type.TryAsDynamicIfNoPia(containingType, out var result))
            {
                return type;
            }
            return result;
        }

        public static bool TryAsDynamicIfNoPia(this TypeSymbol type, NamedTypeSymbol containingType, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TypeSymbol? result)
        {
            if (type.SpecialType == SpecialType.System_Object)
            {
                AssemblySymbol containingAssembly = containingType.ContainingAssembly;
                if ((object)containingAssembly != null && containingAssembly.IsLinked && containingType.IsComImport)
                {
                    result = DynamicTypeSymbol.Instance;
                    return true;
                }
            }
            result = null;
            return false;
        }

        internal static bool IsVerifierReference(this TypeSymbol type)
        {
            if (type.IsReferenceType)
            {
                return type.TypeKind != TypeKind.TypeParameter;
            }
            return false;
        }

        internal static bool IsVerifierValue(this TypeSymbol type)
        {
            if (type.IsValueType)
            {
                return type.TypeKind != TypeKind.TypeParameter;
            }
            return false;
        }

        internal static ImmutableArray<TypeParameterSymbol> GetAllTypeParameters(this NamedTypeSymbol type)
        {
            if ((object)type.ContainingType == null)
            {
                return type.TypeParameters;
            }
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            type.GetAllTypeParameters(instance);
            return instance.ToImmutableAndFree();
        }

        internal static void GetAllTypeParameters(this NamedTypeSymbol type, ArrayBuilder<TypeParameterSymbol> result)
        {
            type.ContainingType?.GetAllTypeParameters(result);
            result.AddRange(type.TypeParameters);
        }

        internal static TypeParameterSymbol? FindEnclosingTypeParameter(this NamedTypeSymbol type, string name)
        {
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            type.GetAllTypeParameters(instance);
            TypeParameterSymbol result = null;
            ArrayBuilder<TypeParameterSymbol>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                if (name == current.Name)
                {
                    result = current;
                    break;
                }
            }
            instance.Free();
            return result;
        }

        internal static TypeParameterSymbol? FindEnclosingTypeParameter(this Symbol methodOrType, string name)
        {
            while (methodOrType != null)
            {
                switch (methodOrType.Kind)
                {
                    default:
                        return null;
                    case SymbolKind.ErrorType:
                    case SymbolKind.Event:
                    case SymbolKind.Field:
                    case SymbolKind.Method:
                    case SymbolKind.NamedType:
                    case SymbolKind.Property:
                        {
                            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = methodOrType.GetMemberTypeParameters().GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                TypeParameterSymbol current = enumerator.Current;
                                if (current.Name == name)
                                {
                                    return current;
                                }
                            }
                            methodOrType = methodOrType.ContainingSymbol;
                            break;
                        }
                }
            }
            return null;
        }

        internal static bool HasNameQualifier(this NamedTypeSymbol type, string qualifiedName)
        {
            Symbol containingSymbol = type.ContainingSymbol;
            if (containingSymbol.Kind != SymbolKind.Namespace)
            {
                return string.Equals(containingSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), qualifiedName, StringComparison.Ordinal);
            }
            NamespaceSymbol namespaceSymbol = (NamespaceSymbol)containingSymbol;
            if (namespaceSymbol.IsGlobalNamespace)
            {
                return qualifiedName.Length == 0;
            }
            return HasNamespaceName(namespaceSymbol, qualifiedName, StringComparison.Ordinal, qualifiedName.Length);
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
            if (name.Length == length)
            {
                return string.Compare(name, 0, namespaceName, indexB, length, comparison) == 0;
            }
            return false;
        }

        internal static bool IsNonGenericTaskType(this TypeSymbol type, CSharpCompilation compilation)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol) || namedTypeSymbol.Arity != 0)
            {
                return false;
            }
            if ((object)namedTypeSymbol == compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task))
            {
                return true;
            }
            if (namedTypeSymbol.IsVoidType())
            {
                return false;
            }
            return namedTypeSymbol.IsCustomTaskType(out object builderArgument);
        }

        internal static bool IsGenericTaskType(this TypeSymbol type, CSharpCompilation compilation)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol) || namedTypeSymbol.Arity != 1)
            {
                return false;
            }
            if ((object)namedTypeSymbol.ConstructedFrom == compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T))
            {
                return true;
            }
            return namedTypeSymbol.IsCustomTaskType(out object builderArgument);
        }

        internal static bool IsIAsyncEnumerableType(this TypeSymbol type, CSharpCompilation compilation)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol) || namedTypeSymbol.Arity != 1)
            {
                return false;
            }
            return (object)namedTypeSymbol.ConstructedFrom == compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T);
        }

        internal static bool IsIAsyncEnumeratorType(this TypeSymbol type, CSharpCompilation compilation)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol) || namedTypeSymbol.Arity != 1)
            {
                return false;
            }
            return (object)namedTypeSymbol.ConstructedFrom == compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T);
        }

        internal static bool IsCustomTaskType(this NamedTypeSymbol type, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out object? builderArgument)
        {
            if (type.Arity < 2)
            {
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator = type.GetAttributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpAttributeData current = enumerator.Current;
                    if (current.IsTargetAttribute(type, AttributeDescription.AsyncMethodBuilderAttribute) && current.CommonConstructorArguments.Length == 1 && current.CommonConstructorArguments[0].Kind == TypedConstantKind.Type)
                    {
                        builderArgument = current.CommonConstructorArguments[0].ValueInternal;
                        return true;
                    }
                }
            }
            builderArgument = null;
            return false;
        }

        internal static TypeSymbol NormalizeTaskTypes(this TypeSymbol type, CSharpCompilation compilation)
        {
            NormalizeTaskTypesInType(compilation, ref type);
            return type;
        }

        private static bool NormalizeTaskTypesInType(CSharpCompilation compilation, ref TypeSymbol type)
        {
            switch (type.Kind)
            {
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    {
                        NamedTypeSymbol type2 = (NamedTypeSymbol)type;
                        bool result4 = NormalizeTaskTypesInNamedType(compilation, ref type2);
                        type = type2;
                        return result4;
                    }
                case SymbolKind.ArrayType:
                    {
                        ArrayTypeSymbol arrayType = (ArrayTypeSymbol)type;
                        bool result3 = NormalizeTaskTypesInArray(compilation, ref arrayType);
                        type = arrayType;
                        return result3;
                    }
                case SymbolKind.PointerType:
                    {
                        PointerTypeSymbol pointerType = (PointerTypeSymbol)type;
                        bool result2 = NormalizeTaskTypesInPointer(compilation, ref pointerType);
                        type = pointerType;
                        return result2;
                    }
                case SymbolKind.FunctionPointerType:
                    {
                        FunctionPointerTypeSymbol funcPtrType = (FunctionPointerTypeSymbol)type;
                        bool result = NormalizeTaskTypesInFunctionPointer(compilation, ref funcPtrType);
                        type = funcPtrType;
                        return result;
                    }
                default:
                    return false;
            }
        }

        private static bool NormalizeTaskTypesInType(CSharpCompilation compilation, ref TypeWithAnnotations typeWithAnnotations)
        {
            TypeSymbol type = typeWithAnnotations.Type;
            if (NormalizeTaskTypesInType(compilation, ref type))
            {
                typeWithAnnotations = TypeWithAnnotations.Create(type, NullableAnnotation.Oblivious, typeWithAnnotations.CustomModifiers);
                return true;
            }
            return false;
        }

        private static bool NormalizeTaskTypesInNamedType(CSharpCompilation compilation, ref NamedTypeSymbol type)
        {
            bool flag = false;
            if (!type.IsDefinition)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                type.GetAllTypeArguments(instance, ref useSiteInfo);
                for (int i = 0; i < instance.Count; i++)
                {
                    TypeWithAnnotations typeWithAnnotations = instance[i];
                    TypeSymbol type2 = typeWithAnnotations.Type;
                    if (NormalizeTaskTypesInType(compilation, ref type2))
                    {
                        flag = true;
                        instance[i] = TypeWithAnnotations.Create(type2, NullableAnnotation.Oblivious, typeWithAnnotations.CustomModifiers);
                    }
                }
                if (flag)
                {
                    NamedTypeSymbol namedTypeSymbol = type;
                    NamedTypeSymbol originalDefinition = namedTypeSymbol.OriginalDefinition;
                    TypeMap typeMap = new TypeMap(originalDefinition.GetAllTypeParameters(), instance.ToImmutable(), allowAlpha: true);
                    type = typeMap.SubstituteNamedType(originalDefinition).WithTupleDataFrom(namedTypeSymbol);
                }
                instance.Free();
            }
            if (type.OriginalDefinition.IsCustomTaskType(out var _))
            {
                int arity = type.Arity;
                NamedTypeSymbol wellKnownType = compilation.GetWellKnownType((arity == 0) ? WellKnownType.System_Threading_Tasks_Task : WellKnownType.System_Threading_Tasks_Task_T);
                if (wellKnownType.TypeKind == TypeKind.Error)
                {
                    return false;
                }
                type = ((arity == 0) ? wellKnownType : wellKnownType.Construct(ImmutableArray.Create(type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0]), unbound: false));
                flag = true;
            }
            return flag;
        }

        private static bool NormalizeTaskTypesInArray(CSharpCompilation compilation, ref ArrayTypeSymbol arrayType)
        {
            TypeWithAnnotations typeWithAnnotations = arrayType.ElementTypeWithAnnotations;
            if (!NormalizeTaskTypesInType(compilation, ref typeWithAnnotations))
            {
                return false;
            }
            arrayType = arrayType.WithElementType(typeWithAnnotations);
            return true;
        }

        private static bool NormalizeTaskTypesInPointer(CSharpCompilation compilation, ref PointerTypeSymbol pointerType)
        {
            TypeWithAnnotations typeWithAnnotations = pointerType.PointedAtTypeWithAnnotations;
            if (!NormalizeTaskTypesInType(compilation, ref typeWithAnnotations))
            {
                return false;
            }
            pointerType = new PointerTypeSymbol(typeWithAnnotations);
            return true;
        }

        private static bool NormalizeTaskTypesInFunctionPointer(CSharpCompilation compilation, ref FunctionPointerTypeSymbol funcPtrType)
        {
            TypeWithAnnotations typeWithAnnotations = funcPtrType.Signature.ReturnTypeWithAnnotations;
            bool flag = NormalizeTaskTypesInType(compilation, ref typeWithAnnotations);
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            if (funcPtrType.Signature.ParameterCount > 0)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(funcPtrType.Signature.ParameterCount);
                bool flag2 = false;
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = funcPtrType.Signature.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations typeWithAnnotations2 = enumerator.Current.TypeWithAnnotations;
                    flag2 |= NormalizeTaskTypesInType(compilation, ref typeWithAnnotations2);
                    instance.Add(typeWithAnnotations2);
                }
                if (flag2)
                {
                    flag = true;
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                }
                else
                {
                    substitutedParameterTypes = funcPtrType.Signature.ParameterTypesWithAnnotations;
                    instance.Free();
                }
            }
            if (flag)
            {
                funcPtrType = funcPtrType.SubstituteTypeSymbol(typeWithAnnotations, substitutedParameterTypes, default(ImmutableArray<CustomModifier>), default(ImmutableArray<ImmutableArray<CustomModifier>>));
                return true;
            }
            return false;
        }

        internal static TypeReferenceWithAttributes GetTypeRefWithAttributes(this TypeWithAnnotations type, PEModuleBuilder moduleBuilder, Symbol declaringSymbol, ITypeReference typeRef)
        {
            ArrayBuilder<ICustomAttribute> instance = ArrayBuilder<ICustomAttribute>.GetInstance();
            CSharpCompilation declaringCompilation = declaringSymbol.DeclaringCompilation;
            if (declaringCompilation != null)
            {
                if (type.Type.ContainsTupleNames())
                {
                    addIfNotNull(instance, declaringCompilation.SynthesizeTupleNamesAttribute(type.Type));
                }
                if (type.Type.ContainsNativeInteger())
                {
                    addIfNotNull(instance, moduleBuilder.SynthesizeNativeIntegerAttribute(declaringSymbol, type.Type));
                }
                if (declaringCompilation.ShouldEmitNullableAttributes(declaringSymbol))
                {
                    addIfNotNull(instance, moduleBuilder.SynthesizeNullableAttributeIfNecessary(declaringSymbol, declaringSymbol.GetNullableContextValue(), type));
                }
            }
            return new TypeReferenceWithAttributes(typeRef, instance.ToImmutableAndFree());
            static void addIfNotNull(ArrayBuilder<ICustomAttribute> builder, SynthesizedAttributeData? attr)
            {
                if (attr != null)
                {
                    builder.Add(attr);
                }
            }
        }

        internal static bool IsWellKnownTypeInAttribute(this TypeSymbol typeSymbol)
        {
            return typeSymbol.IsWellKnownInteropServicesTopLevelType("InAttribute");
        }

        internal static bool IsWellKnownTypeUnmanagedType(this TypeSymbol typeSymbol)
        {
            return typeSymbol.IsWellKnownInteropServicesTopLevelType("UnmanagedType");
        }

        internal static bool IsWellKnownTypeIsExternalInit(this TypeSymbol typeSymbol)
        {
            return typeSymbol.IsWellKnownCompilerServicesTopLevelType("IsExternalInit");
        }

        internal static bool IsWellKnownTypeOutAttribute(this TypeSymbol typeSymbol)
        {
            return typeSymbol.IsWellKnownInteropServicesTopLevelType("OutAttribute");
        }

        private static bool IsWellKnownInteropServicesTopLevelType(this TypeSymbol typeSymbol, string name)
        {
            if (typeSymbol.Name != name || (object)typeSymbol.ContainingType != null)
            {
                return false;
            }
            return typeSymbol.IsContainedInNamespace("System", "Runtime", "InteropServices");
        }

        private static bool IsWellKnownCompilerServicesTopLevelType(this TypeSymbol typeSymbol, string name)
        {
            if (typeSymbol.Name != name)
            {
                return false;
            }
            return typeSymbol.IsCompilerServicesTopLevelType();
        }

        internal static bool IsCompilerServicesTopLevelType(this TypeSymbol typeSymbol)
        {
            if ((object)typeSymbol.ContainingType == null)
            {
                return typeSymbol.IsContainedInNamespace("System", "Runtime", "CompilerServices");
            }
            return false;
        }

        private static bool IsContainedInNamespace(this TypeSymbol typeSymbol, string outerNS, string midNS, string innerNS)
        {
            NamespaceSymbol containingNamespace = typeSymbol.ContainingNamespace;
            if (containingNamespace?.Name != innerNS)
            {
                return false;
            }
            NamespaceSymbol containingNamespace2 = containingNamespace.ContainingNamespace;
            if (containingNamespace2?.Name != midNS)
            {
                return false;
            }
            NamespaceSymbol containingNamespace3 = containingNamespace2.ContainingNamespace;
            if (containingNamespace3?.Name != outerNS)
            {
                return false;
            }
            NamespaceSymbol containingNamespace4 = containingNamespace3.ContainingNamespace;
            if (containingNamespace4 != null)
            {
                return containingNamespace4.IsGlobalNamespace;
            }
            return false;
        }

        public static bool IsBadAsyncReturn(this TypeSymbol returnType, CSharpCompilation declaringCompilation)
        {
            if (!returnType.IsErrorType() && !returnType.IsVoidType() && !returnType.IsNonGenericTaskType(declaringCompilation) && !returnType.IsGenericTaskType(declaringCompilation) && !returnType.IsIAsyncEnumerableType(declaringCompilation))
            {
                return !returnType.IsIAsyncEnumeratorType(declaringCompilation);
            }
            return false;
        }

        internal static int TypeToIndex(this TypeSymbol type)
        {
            switch (type.GetSpecialTypeSafe())
            {
                case SpecialType.System_Object:
                    return 0;
                case SpecialType.System_String:
                    return 1;
                case SpecialType.System_Boolean:
                    return 2;
                case SpecialType.System_Char:
                    return 3;
                case SpecialType.System_SByte:
                    return 4;
                case SpecialType.System_Int16:
                    return 5;
                case SpecialType.System_Int32:
                    return 6;
                case SpecialType.System_Int64:
                    return 7;
                case SpecialType.System_Byte:
                    return 8;
                case SpecialType.System_UInt16:
                    return 9;
                case SpecialType.System_UInt32:
                    return 10;
                case SpecialType.System_UInt64:
                    return 11;
                case SpecialType.System_IntPtr:
                    if (type.IsNativeIntegerType)
                    {
                        return 12;
                    }
                    break;
                case SpecialType.System_UIntPtr:
                    if (type.IsNativeIntegerType)
                    {
                        return 13;
                    }
                    break;
                case SpecialType.System_Single:
                    return 14;
                case SpecialType.System_Double:
                    return 15;
                case SpecialType.System_Decimal:
                    return 16;
                case SpecialType.None:
                    {
                        if ((object)type == null || !type.IsNullableType())
                        {
                            break;
                        }
                        TypeSymbol nullableUnderlyingType = type.GetNullableUnderlyingType();
                        switch (nullableUnderlyingType.GetSpecialTypeSafe())
                        {
                            case SpecialType.System_Boolean:
                                return 17;
                            case SpecialType.System_Char:
                                return 18;
                            case SpecialType.System_SByte:
                                return 19;
                            case SpecialType.System_Int16:
                                return 20;
                            case SpecialType.System_Int32:
                                return 21;
                            case SpecialType.System_Int64:
                                return 22;
                            case SpecialType.System_Byte:
                                return 23;
                            case SpecialType.System_UInt16:
                                return 24;
                            case SpecialType.System_UInt32:
                                return 25;
                            case SpecialType.System_UInt64:
                                return 26;
                            case SpecialType.System_IntPtr:
                                if (nullableUnderlyingType.IsNativeIntegerType)
                                {
                                    return 27;
                                }
                                break;
                            case SpecialType.System_UIntPtr:
                                if (nullableUnderlyingType.IsNativeIntegerType)
                                {
                                    return 28;
                                }
                                break;
                            case SpecialType.System_Single:
                                return 29;
                            case SpecialType.System_Double:
                                return 30;
                            case SpecialType.System_Decimal:
                                return 31;
                        }
                        break;
                    }
            }
            return -1;
        }

        public static NamedTypeSymbol AsUnboundGenericType(this NamedTypeSymbol type)
        {
            if (!type.IsGenericType)
            {
                throw new InvalidOperationException();
            }
            NamedTypeSymbol originalDefinition = type.OriginalDefinition;
            int arity = originalDefinition.Arity;
            NamedTypeSymbol containingType = originalDefinition.ContainingType;
            NamedTypeSymbol namedTypeSymbol = (((object)containingType == null) ? originalDefinition : originalDefinition.AsMember(containingType.IsGenericType ? containingType.AsUnboundGenericType() : containingType));
            if (arity == 0)
            {
                return namedTypeSymbol;
            }
            ImmutableArray<TypeWithAnnotations> typeArguments = UnboundArgumentErrorTypeSymbol.CreateTypeArguments(namedTypeSymbol.TypeParameters, arity, new CSDiagnosticInfo(ErrorCode.ERR_UnexpectedUnboundGenericName));
            return namedTypeSymbol.Construct(typeArguments, unbound: true);
        }

        public static int CustomModifierCount(this TypeSymbol type)
        {
            if ((object)type == null)
            {
                return 0;
            }
            switch (type.Kind)
            {
                case SymbolKind.ArrayType:
                    return customModifierCountForTypeWithAnnotations(((ArrayTypeSymbol)type).ElementTypeWithAnnotations);
                case SymbolKind.PointerType:
                    return customModifierCountForTypeWithAnnotations(((PointerTypeSymbol)type).PointedAtTypeWithAnnotations);
                case SymbolKind.FunctionPointerType:
                    return ((FunctionPointerTypeSymbol)type).Signature.CustomModifierCount();
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    {
                        if (type.IsDefinition)
                        {
                            break;
                        }
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                        int num = 0;
                        while ((object)namedTypeSymbol != null)
                        {
                            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                TypeWithAnnotations current = enumerator.Current;
                                num += customModifierCountForTypeWithAnnotations(current);
                            }
                            namedTypeSymbol = namedTypeSymbol.ContainingType;
                        }
                        return num;
                    }
            }
            return 0;
            static int customModifierCountForTypeWithAnnotations(TypeWithAnnotations typeWithAnnotations)
            {
                return typeWithAnnotations.CustomModifiers.Length + typeWithAnnotations.Type.CustomModifierCount();
            }
        }

        public static bool HasCustomModifiers(this TypeSymbol type, bool flagNonDefaultArraySizesOrLowerBounds)
        {
            if ((object)type == null)
            {
                return false;
            }
            switch (type.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
                        if (!checkTypeWithAnnotations(arrayTypeSymbol.ElementTypeWithAnnotations, flagNonDefaultArraySizesOrLowerBounds))
                        {
                            if (flagNonDefaultArraySizesOrLowerBounds)
                            {
                                return !arrayTypeSymbol.HasDefaultSizesAndLowerBounds;
                            }
                            return false;
                        }
                        return true;
                    }
                case SymbolKind.PointerType:
                    return checkTypeWithAnnotations(((PointerTypeSymbol)type).PointedAtTypeWithAnnotations, flagNonDefaultArraySizesOrLowerBounds);
                case SymbolKind.FunctionPointerType:
                    {
                        FunctionPointerTypeSymbol functionPointerTypeSymbol = (FunctionPointerTypeSymbol)type;
                        if (!functionPointerTypeSymbol.Signature.RefCustomModifiers.IsEmpty || checkTypeWithAnnotations(functionPointerTypeSymbol.Signature.ReturnTypeWithAnnotations, flagNonDefaultArraySizesOrLowerBounds))
                        {
                            return true;
                        }
                        ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = functionPointerTypeSymbol.Signature.Parameters.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            ParameterSymbol current = enumerator2.Current;
                            if (!current.RefCustomModifiers.IsEmpty || checkTypeWithAnnotations(current.TypeWithAnnotations, flagNonDefaultArraySizesOrLowerBounds))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    {
                        if (type.IsDefinition)
                        {
                            break;
                        }
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                        while ((object)namedTypeSymbol != null)
                        {
                            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (checkTypeWithAnnotations(enumerator.Current, flagNonDefaultArraySizesOrLowerBounds))
                                {
                                    return true;
                                }
                            }
                            namedTypeSymbol = namedTypeSymbol.ContainingType;
                        }
                        break;
                    }
            }
            return false;
            static bool checkTypeWithAnnotations(TypeWithAnnotations typeWithAnnotations, bool flagNonDefaultArraySizesOrLowerBounds)
            {
                if (!typeWithAnnotations.CustomModifiers.Any())
                {
                    return typeWithAnnotations.Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds);
                }
                return true;
            }
        }

        public static bool CanUnifyWith(this TypeSymbol thisType, TypeSymbol otherType)
        {
            return TypeUnification.CanUnify(thisType, otherType);
        }

        internal static TypeSymbol GetNextBaseTypeNoUseSiteDiagnostics(this TypeSymbol type, ConsList<TypeSymbol> basesBeingResolved, CSharpCompilation compilation, ref PooledHashSet<NamedTypeSymbol> visited)
        {
            switch (type.TypeKind)
            {
                case TypeKind.TypeParameter:
                    return ((TypeParameterSymbol)type).EffectiveBaseClassNoUseSiteDiagnostics;
                case TypeKind.Class:
                case TypeKind.Error:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    return GetNextDeclaredBase((NamedTypeSymbol)type, basesBeingResolved, compilation, ref visited);
                default:
                    return type.BaseTypeNoUseSiteDiagnostics;
            }
        }

        private static TypeSymbol GetNextDeclaredBase(NamedTypeSymbol type, ConsList<TypeSymbol> basesBeingResolved, CSharpCompilation compilation, ref PooledHashSet<NamedTypeSymbol> visited)
        {
            if (basesBeingResolved != null && basesBeingResolved.ContainsReference(type.OriginalDefinition))
            {
                return null;
            }
            if (type.SpecialType == SpecialType.System_Object)
            {
                type.SetKnownToHaveNoDeclaredBaseCycles();
                return null;
            }
            NamedTypeSymbol declaredBaseType = type.GetDeclaredBaseType(basesBeingResolved);
            if ((object)declaredBaseType == null)
            {
                SetKnownToHaveNoDeclaredBaseCycles(ref visited);
                return GetDefaultBaseOrNull(type, compilation);
            }
            NamedTypeSymbol originalDefinition = type.OriginalDefinition;
            if (declaredBaseType.KnownToHaveNoDeclaredBaseCycles)
            {
                originalDefinition.SetKnownToHaveNoDeclaredBaseCycles();
                SetKnownToHaveNoDeclaredBaseCycles(ref visited);
            }
            else
            {
                visited = visited ?? PooledHashSet<NamedTypeSymbol>.GetInstance();
                visited.Add(originalDefinition);
                if (visited.Contains(declaredBaseType.OriginalDefinition))
                {
                    return GetDefaultBaseOrNull(type, compilation);
                }
            }
            return declaredBaseType;
        }

        private static void SetKnownToHaveNoDeclaredBaseCycles(ref PooledHashSet<NamedTypeSymbol> visited)
        {
            if (visited == null)
            {
                return;
            }
            foreach (NamedTypeSymbol item in visited)
            {
                item.SetKnownToHaveNoDeclaredBaseCycles();
            }
            visited.Free();
            visited = null;
        }

        private static NamedTypeSymbol GetDefaultBaseOrNull(NamedTypeSymbol type, CSharpCompilation compilation)
        {
            if (compilation == null)
            {
                return null;
            }
            switch (type.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Error:
                    return compilation.Assembly.GetSpecialType(SpecialType.System_Object);
                case TypeKind.Interface:
                    return null;
                case TypeKind.Struct:
                    return compilation.Assembly.GetSpecialType(SpecialType.System_ValueType);
                default:
                    throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
            }
        }
    }
}
