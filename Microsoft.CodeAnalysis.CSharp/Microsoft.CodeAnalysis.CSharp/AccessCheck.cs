using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class AccessCheck
    {
        public static bool IsSymbolAccessible(Symbol symbol, AssemblySymbol within, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return IsSymbolAccessibleCore(symbol, within, null, out bool failedThroughTypeCheck, within.DeclaringCompilation, ref useSiteInfo);
        }

        public static bool IsSymbolAccessible(Symbol symbol, NamedTypeSymbol within, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol throughTypeOpt = null)
        {
            return IsSymbolAccessibleCore(symbol, within, throughTypeOpt, out bool failedThroughTypeCheck, within.DeclaringCompilation, ref useSiteInfo);
        }

        public static bool IsSymbolAccessible(Symbol symbol, NamedTypeSymbol within, TypeSymbol throughTypeOpt, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return IsSymbolAccessibleCore(symbol, within, throughTypeOpt, out failedThroughTypeCheck, within.DeclaringCompilation, ref useSiteInfo, basesBeingResolved);
        }

        internal static bool IsEffectivelyPublicOrInternal(Symbol symbol, out bool isInternal)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.TypeParameter:
                    symbol = symbol.ContainingSymbol;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Method:
                case SymbolKind.NamedType:
                case SymbolKind.Property:
                    break;
            }
            isInternal = false;
            do
            {
                switch (symbol.DeclaredAccessibility)
                {
                    case Accessibility.ProtectedAndInternal:
                    case Accessibility.Internal:
                        isInternal = true;
                        break;
                    case Accessibility.Private:
                        return false;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
                    case Accessibility.Protected:
                    case Accessibility.ProtectedOrInternal:
                    case Accessibility.Public:
                        break;
                }
                symbol = symbol.ContainingType;
            }
            while ((object)symbol != null);
            return true;
        }

        private static bool IsSymbolAccessibleCore(Symbol symbol, Symbol within, TypeSymbol throughTypeOpt, out bool failedThroughTypeCheck, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            failedThroughTypeCheck = false;
            switch (symbol.Kind)
            {
                case SymbolKind.ArrayType:
                    return IsSymbolAccessibleCore(((ArrayTypeSymbol)symbol).ElementType, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case SymbolKind.PointerType:
                    return IsSymbolAccessibleCore(((PointerTypeSymbol)symbol).PointedAtType, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case SymbolKind.NamedType:
                    return IsNamedTypeAccessible((NamedTypeSymbol)symbol, within, ref useSiteInfo, basesBeingResolved);
                case SymbolKind.Alias:
                    return IsSymbolAccessibleCore(((AliasSymbol)symbol).Target, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case SymbolKind.Discard:
                    return IsSymbolAccessibleCore(((DiscardSymbol)symbol).TypeWithAnnotations.Type, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case SymbolKind.FunctionPointerType:
                    {
                        FunctionPointerTypeSymbol functionPointerTypeSymbol = (FunctionPointerTypeSymbol)symbol;
                        if (!IsSymbolAccessibleCore(functionPointerTypeSymbol.Signature.ReturnType, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved))
                        {
                            return false;
                        }
                        ImmutableArray<ParameterSymbol>.Enumerator enumerator = functionPointerTypeSymbol.Signature.Parameters.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (!IsSymbolAccessibleCore(enumerator.Current.Type, within, null, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case SymbolKind.ErrorType:
                    return true;
                case SymbolKind.Method:
                    if (((MethodSymbol)symbol).MethodKind == MethodKind.LocalFunction)
                    {
                        goto case SymbolKind.Assembly;
                    }
                    goto case SymbolKind.Event;
                case SymbolKind.Assembly:
                case SymbolKind.DynamicType:
                case SymbolKind.Label:
                case SymbolKind.Local:
                case SymbolKind.NetModule:
                case SymbolKind.Namespace:
                case SymbolKind.Parameter:
                case SymbolKind.RangeVariable:
                case SymbolKind.TypeParameter:
                    return true;
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                    if (!symbol.RequiresInstanceReceiver())
                    {
                        throughTypeOpt = null;
                    }
                    return IsMemberAccessible(symbol.ContainingType, symbol.DeclaredAccessibility, within, throughTypeOpt, out failedThroughTypeCheck, compilation, ref useSiteInfo);
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
            }
        }

        private static bool IsNamedTypeAccessible(NamedTypeSymbol type, Symbol within, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            CSharpCompilation declaringCompilation = within.DeclaringCompilation;
            bool failedThroughTypeCheck;
            if (!type.IsDefinition)
            {
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = type.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations current = enumerator.Current;
                    if (current.Type.Kind != SymbolKind.TypeParameter && !IsSymbolAccessibleCore(current.Type, within, null, out failedThroughTypeCheck, declaringCompilation, ref useSiteInfo, basesBeingResolved))
                    {
                        return false;
                    }
                }
            }
            NamedTypeSymbol containingType = type.ContainingType;
            if ((object)containingType != null)
            {
                return IsMemberAccessible(containingType, type.DeclaredAccessibility, within, null, out failedThroughTypeCheck, declaringCompilation, ref useSiteInfo, basesBeingResolved);
            }
            return IsNonNestedTypeAccessible(type.ContainingAssembly, type.DeclaredAccessibility, within);
        }

        private static bool IsNonNestedTypeAccessible(AssemblySymbol assembly, Accessibility declaredAccessibility, Symbol within)
        {
            switch (declaredAccessibility)
            {
                case Accessibility.NotApplicable:
                case Accessibility.Public:
                    return true;
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Protected:
                    return false;
                case Accessibility.Internal:
                case Accessibility.ProtectedOrInternal:
                    {
                        AssemblySymbol assemblySymbol = ((within is NamedTypeSymbol namedTypeSymbol) ? namedTypeSymbol.ContainingAssembly : ((AssemblySymbol)within));
                        if ((object)assemblySymbol != assembly)
                        {
                            return assemblySymbol.HasInternalAccessTo(assembly);
                        }
                        return true;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaredAccessibility);
            }
        }

        private static bool IsMemberAccessible(NamedTypeSymbol containingType, Accessibility declaredAccessibility, Symbol within, TypeSymbol throughTypeOpt, out bool failedThroughTypeCheck, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            failedThroughTypeCheck = false;
            if ((object)containingType == within)
            {
                return true;
            }
            if (!IsNamedTypeAccessible(containingType, within, ref useSiteInfo, basesBeingResolved))
            {
                return false;
            }
            if (declaredAccessibility == Accessibility.Public)
            {
                return true;
            }
            return IsNonPublicMemberAccessible(containingType, declaredAccessibility, within, throughTypeOpt, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
        }

        private static bool IsNonPublicMemberAccessible(NamedTypeSymbol containingType, Accessibility declaredAccessibility, Symbol within, TypeSymbol throughTypeOpt, out bool failedThroughTypeCheck, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            failedThroughTypeCheck = false;
            NamedTypeSymbol originalDefinition = containingType.OriginalDefinition;
            NamedTypeSymbol namedTypeSymbol = within as NamedTypeSymbol;
            AssemblySymbol fromAssembly = (((object)namedTypeSymbol != null) ? namedTypeSymbol.ContainingAssembly : ((AssemblySymbol)within));
            switch (declaredAccessibility)
            {
                case Accessibility.NotApplicable:
                    return true;
                case Accessibility.Private:
                    if (containingType.TypeKind == TypeKind.Submission)
                    {
                        return true;
                    }
                    if ((object)namedTypeSymbol != null)
                    {
                        return IsPrivateSymbolAccessible(namedTypeSymbol, originalDefinition);
                    }
                    return false;
                case Accessibility.Internal:
                    return fromAssembly.HasInternalAccessTo(containingType.ContainingAssembly);
                case Accessibility.ProtectedAndInternal:
                    if (!fromAssembly.HasInternalAccessTo(containingType.ContainingAssembly))
                    {
                        return false;
                    }
                    return IsProtectedSymbolAccessible(namedTypeSymbol, throughTypeOpt, originalDefinition, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case Accessibility.ProtectedOrInternal:
                    if (fromAssembly.HasInternalAccessTo(containingType.ContainingAssembly))
                    {
                        return true;
                    }
                    return IsProtectedSymbolAccessible(namedTypeSymbol, throughTypeOpt, originalDefinition, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                case Accessibility.Protected:
                    return IsProtectedSymbolAccessible(namedTypeSymbol, throughTypeOpt, originalDefinition, out failedThroughTypeCheck, compilation, ref useSiteInfo, basesBeingResolved);
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaredAccessibility);
            }
        }

        private static bool IsProtectedSymbolAccessible(NamedTypeSymbol withinType, TypeSymbol throughTypeOpt, NamedTypeSymbol originalContainingType, out bool failedThroughTypeCheck, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            failedThroughTypeCheck = false;
            if (originalContainingType.TypeKind == TypeKind.Submission)
            {
                return true;
            }
            if ((object)withinType == null)
            {
                return false;
            }
            if (IsNestedWithinOriginalContainingType(withinType, originalContainingType))
            {
                return true;
            }
            NamedTypeSymbol namedTypeSymbol = withinType.OriginalDefinition;
            TypeSymbol typeSymbol = throughTypeOpt?.OriginalDefinition;
            while ((object)namedTypeSymbol != null)
            {
                if (namedTypeSymbol.InheritsFromOrImplementsIgnoringConstruction(originalContainingType, compilation, ref useSiteInfo, basesBeingResolved))
                {
                    if ((object)typeSymbol == null || typeSymbol.InheritsFromOrImplementsIgnoringConstruction(namedTypeSymbol, compilation, ref useSiteInfo))
                    {
                        return true;
                    }
                    failedThroughTypeCheck = true;
                }
                namedTypeSymbol = namedTypeSymbol.ContainingType;
            }
            return false;
        }

        private static bool IsPrivateSymbolAccessible(Symbol within, NamedTypeSymbol originalContainingType)
        {
            if (!(within is NamedTypeSymbol withinType))
            {
                return false;
            }
            return IsNestedWithinOriginalContainingType(withinType, originalContainingType);
        }

        private static bool IsNestedWithinOriginalContainingType(NamedTypeSymbol withinType, NamedTypeSymbol originalContainingType)
        {
            NamedTypeSymbol namedTypeSymbol = withinType.OriginalDefinition;
            while ((object)namedTypeSymbol != null)
            {
                if ((object)namedTypeSymbol == originalContainingType)
                {
                    return true;
                }
                namedTypeSymbol = namedTypeSymbol.ContainingType;
            }
            return false;
        }

        private static bool InheritsFromOrImplementsIgnoringConstruction(this TypeSymbol type, NamedTypeSymbol baseType, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved = null)
        {
            PooledHashSet<NamedTypeSymbol> pooledHashSet = null;
            ArrayBuilder<NamedTypeSymbol> arrayBuilder = null;
            bool isInterface = baseType.IsInterface;
            if (isInterface)
            {
                pooledHashSet = PooledHashSet<NamedTypeSymbol>.GetInstance();
                arrayBuilder = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            }
            PooledHashSet<NamedTypeSymbol> visited = null;
            TypeSymbol typeSymbol = type;
            bool flag = false;
            while ((object)typeSymbol != null)
            {
                if (isInterface == typeSymbol.IsInterfaceType() && (object)typeSymbol == baseType)
                {
                    flag = true;
                    break;
                }
                if (isInterface)
                {
                    getBaseInterfaces(typeSymbol, arrayBuilder, pooledHashSet, basesBeingResolved);
                }
                TypeSymbol nextBaseTypeNoUseSiteDiagnostics = typeSymbol.GetNextBaseTypeNoUseSiteDiagnostics(basesBeingResolved, compilation, ref visited);
                if ((object)nextBaseTypeNoUseSiteDiagnostics == null)
                {
                    typeSymbol = null;
                    continue;
                }
                typeSymbol = nextBaseTypeNoUseSiteDiagnostics.OriginalDefinition;
                typeSymbol.AddUseSiteInfo(ref useSiteInfo);
            }
            visited?.Free();
            if (!flag && isInterface)
            {
                while (arrayBuilder.Count != 0)
                {
                    NamedTypeSymbol namedTypeSymbol = arrayBuilder.Pop();
                    if (namedTypeSymbol.IsInterface)
                    {
                        if ((object)namedTypeSymbol == baseType)
                        {
                            flag = true;
                            break;
                        }
                        getBaseInterfaces(namedTypeSymbol, arrayBuilder, pooledHashSet, basesBeingResolved);
                    }
                }
                if (!flag)
                {
                    foreach (NamedTypeSymbol item in pooledHashSet)
                    {
                        item.AddUseSiteInfo(ref useSiteInfo);
                    }
                }
            }
            pooledHashSet?.Free();
            arrayBuilder?.Free();
            return flag;
            static void getBaseInterfaces(TypeSymbol derived, ArrayBuilder<NamedTypeSymbol> baseInterfaces, PooledHashSet<NamedTypeSymbol> interfacesLookedAt, ConsList<TypeSymbol> basesBeingResolved)
            {
                if (basesBeingResolved == null || !basesBeingResolved.ContainsReference(derived))
                {
                    ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = ((derived is TypeParameterSymbol typeParameterSymbol) ? typeParameterSymbol.AllEffectiveInterfacesNoUseSiteDiagnostics : ((!(derived is NamedTypeSymbol namedTypeSymbol2)) ? derived.InterfacesNoUseSiteDiagnostics(basesBeingResolved) : namedTypeSymbol2.GetDeclaredInterfaces(basesBeingResolved))).GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NamedTypeSymbol originalDefinition = enumerator2.Current.OriginalDefinition;
                        if (interfacesLookedAt.Add(originalDefinition))
                        {
                            baseInterfaces.Add(originalDefinition);
                        }
                    }
                }
            }
        }

        internal static bool HasInternalAccessTo(this AssemblySymbol fromAssembly, AssemblySymbol toAssembly)
        {
            if (object.Equals(fromAssembly, toAssembly))
            {
                return true;
            }
            if (fromAssembly.AreInternalsVisibleToThisAssembly(toAssembly))
            {
                return true;
            }
            if (fromAssembly.IsInteractive && toAssembly.IsInteractive)
            {
                return true;
            }
            return false;
        }

        internal static ErrorCode GetProtectedMemberInSealedTypeError(NamedTypeSymbol containingType)
        {
            if (containingType.TypeKind != TypeKind.Struct)
            {
                return ErrorCode.WRN_ProtectedInSealed;
            }
            return ErrorCode.ERR_ProtectedInStruct;
        }
    }
}
