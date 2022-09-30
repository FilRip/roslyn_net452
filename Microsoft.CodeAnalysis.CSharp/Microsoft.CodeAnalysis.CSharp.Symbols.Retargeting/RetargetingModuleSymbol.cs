using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    internal sealed class RetargetingModuleSymbol : NonMissingModuleSymbol
    {
        private struct DestinationData
        {
            public AssemblySymbol To;

            private ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> _symbolMap;

            public ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> SymbolMap => LazyInitializer.EnsureInitialized(ref _symbolMap);
        }

        internal class RetargetingSymbolTranslator : CSharpSymbolVisitor<RetargetOptions, Symbol>
        {
            private class RetargetedTypeMethodFinder : RetargetingSymbolTranslator
            {
                private RetargetedTypeMethodFinder(RetargetingModuleSymbol retargetingModule)
                    : base(retargetingModule)
                {
                }

                public static MethodSymbol Find(RetargetingSymbolTranslator translator, MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
                {
                    if (!method.IsGenericMethod)
                    {
                        return FindWorker(translator, method, retargetedType, retargetedMethodComparer);
                    }
                    return FindWorker(new RetargetedTypeMethodFinder(translator._retargetingModule), method, retargetedType, retargetedMethodComparer);
                }

                private static MethodSymbol FindWorker(RetargetingSymbolTranslator translator, MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
                {
                    ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(method.Parameters.Length);
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
                    bool modifiersHaveChanged;
                    while (enumerator.MoveNext())
                    {
                        ParameterSymbol current = enumerator.Current;
                        instance.Add(new SignatureOnlyParameterSymbol(translator.Retarget(current.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(current.RefCustomModifiers, out modifiersHaveChanged), current.IsParams, current.RefKind));
                    }
                    SignatureOnlyMethodSymbol y = new SignatureOnlyMethodSymbol(method.Name, retargetedType, method.MethodKind, method.CallingConvention, IndexedTypeParameterSymbol.TakeSymbols(method.Arity), instance.ToImmutableAndFree(), method.RefKind, method.IsInitOnly, translator.Retarget(method.ReturnTypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(method.RefCustomModifiers, out modifiersHaveChanged), ImmutableArray<MethodSymbol>.Empty);
                    ImmutableArray<Symbol>.Enumerator enumerator2 = retargetedType.GetMembers(method.Name).GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Symbol current2 = enumerator2.Current;
                        if (current2.Kind == SymbolKind.Method)
                        {
                            MethodSymbol methodSymbol = (MethodSymbol)current2;
                            if (retargetedMethodComparer.Equals(methodSymbol, y))
                            {
                                return methodSymbol;
                            }
                        }
                    }
                    return null;
                }

                public override TypeParameterSymbol Retarget(TypeParameterSymbol typeParameter)
                {
                    if ((object)typeParameter.ContainingModule == base.UnderlyingModule)
                    {
                        return base.Retarget(typeParameter);
                    }
                    return IndexedTypeParameterSymbol.GetTypeParameter(typeParameter.Ordinal);
                }
            }

            private readonly RetargetingModuleSymbol _retargetingModule;

            private ConcurrentDictionary<Symbol, Symbol> SymbolMap => _retargetingModule._symbolMap;

            private RetargetingAssemblySymbol RetargetingAssembly => _retargetingModule._retargetingAssembly;

            private SourceModuleSymbol UnderlyingModule => _retargetingModule._underlyingModule;

            private Dictionary<AssemblySymbol, DestinationData> RetargetingAssemblyMap => _retargetingModule._retargetingAssemblyMap;

            public RetargetingSymbolTranslator(RetargetingModuleSymbol retargetingModule)
            {
                _retargetingModule = retargetingModule;
            }

            public Symbol Retarget(Symbol symbol)
            {
                return symbol.Accept(this, RetargetOptions.RetargetPrimitiveTypesByName);
            }

            public MarshalPseudoCustomAttributeData Retarget(MarshalPseudoCustomAttributeData marshallingInfo)
            {
                return marshallingInfo?.WithTranslatedTypes((TypeSymbol type, RetargetingSymbolTranslator translator) => translator.Retarget(type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), this);
            }

            public TypeSymbol Retarget(TypeSymbol symbol, RetargetOptions options)
            {
                return (TypeSymbol)symbol.Accept(this, options);
            }

            public TypeWithAnnotations Retarget(TypeWithAnnotations underlyingType, RetargetOptions options, NamedTypeSymbol asDynamicIfNoPiaContainingType = null)
            {
                TypeSymbol typeSymbol = Retarget(underlyingType.Type, options);
                if ((object)asDynamicIfNoPiaContainingType != null)
                {
                    typeSymbol = typeSymbol.AsDynamicIfNoPia(asDynamicIfNoPiaContainingType);
                }
                ImmutableArray<CustomModifier> customModifiers = RetargetModifiers(underlyingType.CustomModifiers, out bool modifiersHaveChanged);
                if (modifiersHaveChanged || !TypeSymbol.Equals(underlyingType.Type, typeSymbol, TypeCompareKind.ConsiderEverything))
                {
                    return underlyingType.WithTypeAndModifiers(typeSymbol, customModifiers);
                }
                return underlyingType;
            }

            public NamespaceSymbol Retarget(NamespaceSymbol ns)
            {
                return (NamespaceSymbol)SymbolMap.GetOrAdd(ns, _retargetingModule._createRetargetingNamespace);
            }

            private NamedTypeSymbol RetargetNamedTypeDefinition(NamedTypeSymbol type, RetargetOptions options)
            {
                if (type.IsNativeIntegerType)
                {
                    NamedTypeSymbol namedTypeSymbol = RetargetNamedTypeDefinition(type.NativeIntegerUnderlyingType, options);
                    if (namedTypeSymbol.SpecialType != 0)
                    {
                        return namedTypeSymbol.AsNativeInteger();
                    }
                    return namedTypeSymbol;
                }
                if (options == RetargetOptions.RetargetPrimitiveTypesByTypeCode)
                {
                    PrimitiveTypeCode primitiveTypeCode = type.PrimitiveTypeCode;
                    if (primitiveTypeCode != PrimitiveTypeCode.NotPrimitive)
                    {
                        return RetargetingAssembly.GetPrimitiveType(primitiveTypeCode);
                    }
                }
                if (type.Kind == SymbolKind.ErrorType)
                {
                    return Retarget((ErrorTypeSymbol)type);
                }
                AssemblySymbol containingAssembly = type.ContainingAssembly;
                if (((object)containingAssembly != RetargetingAssembly.UnderlyingAssembly) ? containingAssembly.IsLinked : type.IsExplicitDefinitionOfNoPiaLocalType)
                {
                    return RetargetNoPiaLocalType(type);
                }
                if ((object)containingAssembly == RetargetingAssembly.UnderlyingAssembly)
                {
                    return RetargetNamedTypeDefinitionFromUnderlyingAssembly(type);
                }
                if (!RetargetingAssemblyMap.TryGetValue(containingAssembly, out var value))
                {
                    return type;
                }
                type = PerformTypeRetargeting(ref value, type);
                RetargetingAssemblyMap[containingAssembly] = value;
                return type;
            }

            private NamedTypeSymbol RetargetNamedTypeDefinitionFromUnderlyingAssembly(NamedTypeSymbol type)
            {
                ModuleSymbol containingModule = type.ContainingModule;
                if ((object)containingModule == UnderlyingModule)
                {
                    NamedTypeSymbol containingType = type.ContainingType;
                    while ((object)containingType != null)
                    {
                        if (containingType.IsExplicitDefinitionOfNoPiaLocalType)
                        {
                            return (NamedTypeSymbol)SymbolMap.GetOrAdd(type, new UnsupportedMetadataTypeSymbol());
                        }
                        containingType = containingType.ContainingType;
                    }
                    return (NamedTypeSymbol)SymbolMap.GetOrAdd(type, _retargetingModule._createRetargetingNamedType);
                }
                PEModuleSymbol addedModule = (PEModuleSymbol)RetargetingAssembly.Modules[containingModule.Ordinal];
                return RetargetNamedTypeDefinition((PENamedTypeSymbol)type, addedModule);
            }

            private NamedTypeSymbol RetargetNoPiaLocalType(NamedTypeSymbol type)
            {
                ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> noPiaUnificationMap = RetargetingAssembly.NoPiaUnificationMap;
                if (noPiaUnificationMap.TryGetValue(type, out var value))
                {
                    return value;
                }
                NamedTypeSymbol value2;
                if (type.ContainingSymbol.Kind != SymbolKind.NamedType && type.Arity == 0)
                {
                    bool isInterface = type.IsInterface;
                    bool flag = false;
                    string guidString = null;
                    string guidString2 = null;
                    if (isInterface)
                    {
                        flag = type.GetGuidString(out guidString);
                    }
                    MetadataTypeName name = MetadataTypeName.FromFullName(type.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), useCLSCompliantNameArityEncoding: false, type.Arity);
                    string identifier = null;
                    if ((object)type.ContainingModule == _retargetingModule.UnderlyingModule)
                    {
                        ImmutableArray<CSharpAttributeData>.Enumerator enumerator = type.GetAttributes().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            CSharpAttributeData current = enumerator.Current;
                            int targetAttributeSignatureIndex = current.GetTargetAttributeSignatureIndex(type, AttributeDescription.TypeIdentifierAttribute);
                            if (targetAttributeSignatureIndex != -1)
                            {
                                if (targetAttributeSignatureIndex == 1 && current.CommonConstructorArguments.Length == 2)
                                {
                                    guidString2 = current.CommonConstructorArguments[0].ValueInternal as string;
                                    identifier = current.CommonConstructorArguments[1].ValueInternal as string;
                                }
                                break;
                            }
                        }
                    }
                    else if (!(flag && isInterface))
                    {
                        type.ContainingAssembly.GetGuidString(out guidString2);
                        identifier = name.FullName;
                    }
                    value2 = MetadataDecoder.SubstituteNoPiaLocalType(ref name, isInterface, type.BaseTypeNoUseSiteDiagnostics, guidString, guidString2, identifier, RetargetingAssembly);
                }
                else
                {
                    value2 = new UnsupportedMetadataTypeSymbol();
                }
                return noPiaUnificationMap.GetOrAdd(type, value2);
            }

            private static NamedTypeSymbol RetargetNamedTypeDefinition(PENamedTypeSymbol type, PEModuleSymbol addedModule)
            {
                if (addedModule.TypeHandleToTypeMap.TryGetValue(type.Handle, out var value))
                {
                    return (NamedTypeSymbol)value;
                }
                NamedTypeSymbol containingType = type.ContainingType;
                MetadataTypeName emittedTypeName;
                if ((object)containingType != null)
                {
                    NamedTypeSymbol namedTypeSymbol = RetargetNamedTypeDefinition((PENamedTypeSymbol)containingType, addedModule);
                    emittedTypeName = MetadataTypeName.FromTypeName(type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
                    return namedTypeSymbol.LookupMetadataType(ref emittedTypeName);
                }
                emittedTypeName = MetadataTypeName.FromNamespaceAndTypeName(type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
                return addedModule.LookupTopLevelMetadataType(ref emittedTypeName);
            }

            private static NamedTypeSymbol PerformTypeRetargeting(ref DestinationData destination, NamedTypeSymbol type)
            {
                if (!destination.SymbolMap.TryGetValue(type, out var value))
                {
                    NamedTypeSymbol containingType = type.ContainingType;
                    NamedTypeSymbol value2;
                    if ((object)containingType != null)
                    {
                        NamedTypeSymbol namedTypeSymbol = PerformTypeRetargeting(ref destination, containingType);
                        MetadataTypeName emittedTypeName = MetadataTypeName.FromTypeName(type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
                        value2 = namedTypeSymbol.LookupMetadataType(ref emittedTypeName);
                    }
                    else
                    {
                        MetadataTypeName emittedTypeName = MetadataTypeName.FromNamespaceAndTypeName(type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
                        value2 = destination.To.LookupTopLevelMetadataType(ref emittedTypeName, digThroughForwardedTypes: true);
                    }
                    return destination.SymbolMap.GetOrAdd(type, value2);
                }
                return value;
            }

            public NamedTypeSymbol Retarget(NamedTypeSymbol type, RetargetOptions options)
            {
                NamedTypeSymbol originalDefinition = type.OriginalDefinition;
                NamedTypeSymbol namedTypeSymbol = RetargetNamedTypeDefinition(originalDefinition, options);
                if ((object)type == originalDefinition)
                {
                    return namedTypeSymbol;
                }
                if (namedTypeSymbol.Kind == SymbolKind.ErrorType && !namedTypeSymbol.IsGenericType)
                {
                    return namedTypeSymbol;
                }
                if (type.IsUnboundGenericType)
                {
                    if ((object)namedTypeSymbol == originalDefinition)
                    {
                        return type;
                    }
                    return namedTypeSymbol.AsUnboundGenericType();
                }
                NamedTypeSymbol namedTypeSymbol2 = type;
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
                int num = int.MaxValue;
                while ((object)namedTypeSymbol2 != null)
                {
                    if (num == int.MaxValue && !namedTypeSymbol2.IsInterface)
                    {
                        num = instance.Count;
                    }
                    instance.AddRange(namedTypeSymbol2.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics);
                    namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
                }
                bool flag = !originalDefinition.Equals(namedTypeSymbol);
                ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance(instance.Count);
                ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator = instance.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations current = enumerator.Current;
                    TypeWithAnnotations item = Retarget(current, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                    if (!flag && !item.IsSameAs(current))
                    {
                        flag = true;
                    }
                    instance2.Add(item);
                }
                bool flag2 = IsNoPiaIllegalGenericInstantiation(instance, instance2, num);
                instance.Free();
                NamedTypeSymbol namedTypeSymbol3;
                if (!flag)
                {
                    namedTypeSymbol3 = type;
                }
                else
                {
                    namedTypeSymbol2 = namedTypeSymbol;
                    ArrayBuilder<TypeParameterSymbol> instance3 = ArrayBuilder<TypeParameterSymbol>.GetInstance(instance2.Count);
                    while ((object)namedTypeSymbol2 != null)
                    {
                        if (namedTypeSymbol2.Arity > 0)
                        {
                            instance3.AddRange(namedTypeSymbol2.TypeParameters);
                        }
                        namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
                    }
                    namedTypeSymbol3 = new TypeMap(instance3.ToImmutableAndFree(), instance2.ToImmutable()).SubstituteNamedType(namedTypeSymbol).WithTupleDataFrom(type);
                }
                instance2.Free();
                if (flag2)
                {
                    return new NoPiaIllegalGenericInstantiationSymbol(_retargetingModule, namedTypeSymbol3);
                }
                return namedTypeSymbol3;
            }

            private bool IsNoPiaIllegalGenericInstantiation(ArrayBuilder<TypeWithAnnotations> oldArguments, ArrayBuilder<TypeWithAnnotations> newArguments, int startOfNonInterfaceArguments)
            {
                if (UnderlyingModule.ContainsExplicitDefinitionOfNoPiaLocalTypes)
                {
                    for (int i = startOfNonInterfaceArguments; i < oldArguments.Count; i++)
                    {
                        if (IsOrClosedOverAnExplicitLocalType(oldArguments[i].Type))
                        {
                            return true;
                        }
                    }
                }
                ImmutableArray<AssemblySymbol> assembliesToEmbedTypesFrom = UnderlyingModule.GetAssembliesToEmbedTypesFrom();
                if (assembliesToEmbedTypesFrom.Length > 0)
                {
                    for (int j = startOfNonInterfaceArguments; j < oldArguments.Count; j++)
                    {
                        if (MetadataDecoder.IsOrClosedOverATypeFromAssemblies(oldArguments[j].Type, assembliesToEmbedTypesFrom))
                        {
                            return true;
                        }
                    }
                }
                ImmutableArray<AssemblySymbol> linkedReferencedAssemblies = RetargetingAssembly.GetLinkedReferencedAssemblies();
                if (!linkedReferencedAssemblies.IsDefaultOrEmpty)
                {
                    for (int k = startOfNonInterfaceArguments; k < newArguments.Count; k++)
                    {
                        if (MetadataDecoder.IsOrClosedOverATypeFromAssemblies(newArguments[k].Type, linkedReferencedAssemblies))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            private bool IsOrClosedOverAnExplicitLocalType(TypeSymbol symbol)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.TypeParameter:
                        return false;
                    case SymbolKind.ArrayType:
                        return IsOrClosedOverAnExplicitLocalType(((ArrayTypeSymbol)symbol).ElementType);
                    case SymbolKind.PointerType:
                        return IsOrClosedOverAnExplicitLocalType(((PointerTypeSymbol)symbol).PointedAtType);
                    case SymbolKind.DynamicType:
                        return false;
                    case SymbolKind.ErrorType:
                    case SymbolKind.NamedType:
                        {
                            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
                            if ((object)symbol.OriginalDefinition.ContainingModule == _retargetingModule.UnderlyingModule && namedTypeSymbol.IsExplicitDefinitionOfNoPiaLocalType)
                            {
                                return true;
                            }
                            do
                            {
                                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    if (IsOrClosedOverAnExplicitLocalType(enumerator.Current.Type))
                                    {
                                        return true;
                                    }
                                }
                                namedTypeSymbol = namedTypeSymbol.ContainingType;
                            }
                            while ((object)namedTypeSymbol != null);
                            return false;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
                }
            }

            public virtual TypeParameterSymbol Retarget(TypeParameterSymbol typeParameter)
            {
                return (TypeParameterSymbol)SymbolMap.GetOrAdd(typeParameter, _retargetingModule._createRetargetingTypeParameter);
            }

            public ArrayTypeSymbol Retarget(ArrayTypeSymbol type)
            {
                TypeWithAnnotations elementTypeWithAnnotations = type.ElementTypeWithAnnotations;
                TypeWithAnnotations typeWithAnnotations = Retarget(elementTypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                if (elementTypeWithAnnotations.IsSameAs(typeWithAnnotations))
                {
                    return type;
                }
                if (type.IsSZArray)
                {
                    return ArrayTypeSymbol.CreateSZArray(RetargetingAssembly, typeWithAnnotations);
                }
                return ArrayTypeSymbol.CreateMDArray(RetargetingAssembly, typeWithAnnotations, type.Rank, type.Sizes, type.LowerBounds);
            }

            internal ImmutableArray<CustomModifier> RetargetModifiers(ImmutableArray<CustomModifier> oldModifiers, out bool modifiersHaveChanged)
            {
                ArrayBuilder<CustomModifier> arrayBuilder = null;
                for (int i = 0; i < oldModifiers.Length; i++)
                {
                    CustomModifier customModifier = oldModifiers[i];
                    NamedTypeSymbol modifierSymbol = ((CSharpCustomModifier)customModifier).ModifierSymbol;
                    NamedTypeSymbol namedTypeSymbol = Retarget(modifierSymbol, RetargetOptions.RetargetPrimitiveTypesByName);
                    if (!namedTypeSymbol.Equals(modifierSymbol))
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<CustomModifier>.GetInstance(oldModifiers.Length);
                            arrayBuilder.AddRange(oldModifiers, i);
                        }
                        arrayBuilder.Add(customModifier.IsOptional ? CSharpCustomModifier.CreateOptional(namedTypeSymbol) : CSharpCustomModifier.CreateRequired(namedTypeSymbol));
                    }
                    else
                    {
                        arrayBuilder?.Add(customModifier);
                    }
                }
                modifiersHaveChanged = arrayBuilder != null;
                if (!modifiersHaveChanged)
                {
                    return oldModifiers;
                }
                return arrayBuilder.ToImmutableAndFree();
            }

            public PointerTypeSymbol Retarget(PointerTypeSymbol type)
            {
                TypeWithAnnotations pointedAtTypeWithAnnotations = type.PointedAtTypeWithAnnotations;
                TypeWithAnnotations typeWithAnnotations = Retarget(pointedAtTypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                if (pointedAtTypeWithAnnotations.IsSameAs(typeWithAnnotations))
                {
                    return type;
                }
                return new PointerTypeSymbol(typeWithAnnotations);
            }

            public FunctionPointerTypeSymbol Retarget(FunctionPointerTypeSymbol type)
            {
                FunctionPointerMethodSymbol signature = type.Signature;
                TypeWithAnnotations typeWithAnnotations = Retarget(signature.ReturnTypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                ImmutableArray<CustomModifier> refCustomModifiers = RetargetModifiers(signature.RefCustomModifiers, out bool modifiersHaveChanged);
                modifiersHaveChanged = modifiersHaveChanged || !signature.ReturnTypeWithAnnotations.IsSameAs(typeWithAnnotations);
                ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
                ImmutableArray<ImmutableArray<CustomModifier>> paramRefCustomModifiers = default(ImmutableArray<ImmutableArray<CustomModifier>>);
                int parameterCount = signature.ParameterCount;
                if (parameterCount > 0)
                {
                    ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(parameterCount);
                    ArrayBuilder<ImmutableArray<CustomModifier>> instance2 = ArrayBuilder<ImmutableArray<CustomModifier>>.GetInstance(parameterCount);
                    bool flag = false;
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator = signature.Parameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ParameterSymbol current = enumerator.Current;
                        TypeWithAnnotations typeWithAnnotations2 = Retarget(current.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                        ImmutableArray<CustomModifier> item = RetargetModifiers(current.RefCustomModifiers, out bool modifiersHaveChanged2);
                        instance.Add(typeWithAnnotations2);
                        instance2.Add(item);
                        flag = flag || !current.TypeWithAnnotations.IsSameAs(typeWithAnnotations2) || modifiersHaveChanged2;
                    }
                    if (flag)
                    {
                        substitutedParameterTypes = instance.ToImmutableAndFree();
                        paramRefCustomModifiers = instance2.ToImmutableAndFree();
                        modifiersHaveChanged = true;
                    }
                    else
                    {
                        instance.Free();
                        instance2.Free();
                        substitutedParameterTypes = signature.ParameterTypesWithAnnotations;
                    }
                }
                if (modifiersHaveChanged)
                {
                    return type.SubstituteTypeSymbol(typeWithAnnotations, substitutedParameterTypes, refCustomModifiers, paramRefCustomModifiers);
                }
                return type;
            }

            public static ErrorTypeSymbol Retarget(ErrorTypeSymbol type)
            {
                DiagnosticInfo? diagnosticInfo = type.GetUseSiteInfo().DiagnosticInfo;
                if (diagnosticInfo == null || diagnosticInfo!.Severity != DiagnosticSeverity.Error)
                {
                    return (type as ExtendedErrorTypeSymbol)?.AsUnreported() ?? new ExtendedErrorTypeSymbol(type, type.ResultKind, type.ErrorInfo ?? new CSDiagnosticInfo(ErrorCode.ERR_ErrorInReferencedAssembly, type.ContainingAssembly?.Identity.GetDisplayName() ?? string.Empty), unreported: true);
                }
                return type;
            }

            public ImmutableArray<Symbol> Retarget(ImmutableArray<Symbol> arr)
            {
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(arr.Length);
                ImmutableArray<Symbol>.Enumerator enumerator = arr.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    instance.Add(Retarget(current));
                }
                return instance.ToImmutableAndFree();
            }

            public ImmutableArray<NamedTypeSymbol> Retarget(ImmutableArray<NamedTypeSymbol> sequence)
            {
                ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance(sequence.Length);
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = sequence.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamedTypeSymbol current = enumerator.Current;
                    instance.Add(Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
                }
                return instance.ToImmutableAndFree();
            }

            public ImmutableArray<TypeSymbol> Retarget(ImmutableArray<TypeSymbol> sequence)
            {
                ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(sequence.Length);
                ImmutableArray<TypeSymbol>.Enumerator enumerator = sequence.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeSymbol current = enumerator.Current;
                    instance.Add(Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
                }
                return instance.ToImmutableAndFree();
            }

            public ImmutableArray<TypeWithAnnotations> Retarget(ImmutableArray<TypeWithAnnotations> sequence)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(sequence.Length);
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = sequence.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations current = enumerator.Current;
                    instance.Add(Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
                }
                return instance.ToImmutableAndFree();
            }

            public ImmutableArray<TypeParameterSymbol> Retarget(ImmutableArray<TypeParameterSymbol> list)
            {
                ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance(list.Length);
                ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeParameterSymbol current = enumerator.Current;
                    instance.Add(Retarget(current));
                }
                return instance.ToImmutableAndFree();
            }

            public MethodSymbol Retarget(MethodSymbol method)
            {
                return (MethodSymbol)SymbolMap.GetOrAdd(method, _retargetingModule._createRetargetingMethod);
            }

            public MethodSymbol Retarget(MethodSymbol method, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
            {
                if ((object)method.ContainingModule == UnderlyingModule && (object)method == method.OriginalDefinition)
                {
                    return Retarget(method);
                }
                NamedTypeSymbol containingType = method.ContainingType;
                NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
                if ((object)namedTypeSymbol != containingType)
                {
                    return FindMethodInRetargetedType(method, namedTypeSymbol, retargetedMethodComparer);
                }
                return method;
            }

            public FieldSymbol Retarget(FieldSymbol field)
            {
                return (FieldSymbol)SymbolMap.GetOrAdd(field, _retargetingModule._createRetargetingField);
            }

            public PropertySymbol Retarget(PropertySymbol property)
            {
                return (PropertySymbol)SymbolMap.GetOrAdd(property, _retargetingModule._createRetargetingProperty);
            }

            public PropertySymbol Retarget(PropertySymbol property, IEqualityComparer<PropertySymbol> retargetedPropertyComparer)
            {
                if ((object)property.ContainingModule == UnderlyingModule && (object)property == property.OriginalDefinition)
                {
                    return Retarget(property);
                }
                NamedTypeSymbol containingType = property.ContainingType;
                NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
                if ((object)namedTypeSymbol != containingType)
                {
                    return FindPropertyInRetargetedType(property, namedTypeSymbol, retargetedPropertyComparer);
                }
                return property;
            }

            public EventSymbol Retarget(EventSymbol @event)
            {
                if ((object)@event.ContainingModule == UnderlyingModule && (object)@event == @event.OriginalDefinition)
                {
                    return (EventSymbol)SymbolMap.GetOrAdd(@event, _retargetingModule._createRetargetingEvent);
                }
                NamedTypeSymbol containingType = @event.ContainingType;
                NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
                if ((object)namedTypeSymbol != containingType)
                {
                    return FindEventInRetargetedType(@event, namedTypeSymbol);
                }
                return @event;
            }

            private MethodSymbol FindMethodInRetargetedType(MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
            {
                return RetargetedTypeMethodFinder.Find(this, method, retargetedType, retargetedMethodComparer);
            }

            private PropertySymbol FindPropertyInRetargetedType(PropertySymbol property, NamedTypeSymbol retargetedType, IEqualityComparer<PropertySymbol> retargetedPropertyComparer)
            {
                ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(property.Parameters.Length);
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = property.Parameters.GetEnumerator();
                bool modifiersHaveChanged;
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    instance.Add(new SignatureOnlyParameterSymbol(Retarget(current.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode), RetargetModifiers(current.RefCustomModifiers, out modifiersHaveChanged), current.IsParams, current.RefKind));
                }
                SignatureOnlyPropertySymbol y = new SignatureOnlyPropertySymbol(property.Name, retargetedType, instance.ToImmutableAndFree(), property.RefKind, Retarget(property.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode), RetargetModifiers(property.RefCustomModifiers, out modifiersHaveChanged), property.IsStatic, ImmutableArray<PropertySymbol>.Empty);
                ImmutableArray<Symbol>.Enumerator enumerator2 = retargetedType.GetMembers(property.Name).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (current2.Kind == SymbolKind.Property)
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)current2;
                        if (retargetedPropertyComparer.Equals(propertySymbol, y))
                        {
                            return propertySymbol;
                        }
                    }
                }
                return null;
            }

            private EventSymbol FindEventInRetargetedType(EventSymbol @event, NamedTypeSymbol retargetedType)
            {
                TypeWithAnnotations typeWithAnnotations = Retarget(@event.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
                ImmutableArray<Symbol>.Enumerator enumerator = retargetedType.GetMembers(@event.Name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.Event)
                    {
                        EventSymbol eventSymbol = (EventSymbol)current;
                        if (TypeSymbol.Equals(eventSymbol.Type, typeWithAnnotations.Type, TypeCompareKind.ConsiderEverything))
                        {
                            return eventSymbol;
                        }
                    }
                }
                return null;
            }

            internal ImmutableArray<CustomModifier> RetargetModifiers(ImmutableArray<CustomModifier> oldModifiers, ref ImmutableArray<CustomModifier> lazyCustomModifiers)
            {
                if (lazyCustomModifiers.IsDefault)
                {
                    ImmutableArray<CustomModifier> value = RetargetModifiers(oldModifiers, out bool modifiersHaveChanged);
                    ImmutableInterlocked.InterlockedCompareExchange(ref lazyCustomModifiers, value, default(ImmutableArray<CustomModifier>));
                }
                return lazyCustomModifiers;
            }

            private ImmutableArray<CSharpAttributeData> RetargetAttributes(ImmutableArray<CSharpAttributeData> oldAttributes)
            {
                return oldAttributes.SelectAsArray((CSharpAttributeData a, RetargetingSymbolTranslator t) => t.RetargetAttributeData(a), this);
            }

            internal IEnumerable<CSharpAttributeData> RetargetAttributes(IEnumerable<CSharpAttributeData> attributes)
            {
                foreach (CSharpAttributeData attribute in attributes)
                {
                    yield return RetargetAttributeData(attribute);
                }
            }

            private CSharpAttributeData RetargetAttributeData(CSharpAttributeData oldAttributeData)
            {
                SourceAttributeData sourceAttributeData = (SourceAttributeData)oldAttributeData;
                MethodSymbol attributeConstructor = sourceAttributeData.AttributeConstructor;
                MethodSymbol methodSymbol = (((object)attributeConstructor == null) ? null : Retarget(attributeConstructor, MemberSignatureComparer.RetargetedExplicitImplementationComparer));
                NamedTypeSymbol attributeClass = sourceAttributeData.AttributeClass;
                NamedTypeSymbol attributeClass2 = (((object)methodSymbol != null) ? methodSymbol.ContainingType : (((object)attributeClass == null) ? null : Retarget(attributeClass, RetargetOptions.RetargetPrimitiveTypesByTypeCode)));
                ImmutableArray<TypedConstant> commonConstructorArguments = sourceAttributeData.CommonConstructorArguments;
                ImmutableArray<TypedConstant> constructorArguments = RetargetAttributeConstructorArguments(commonConstructorArguments);
                ImmutableArray<KeyValuePair<string, TypedConstant>> commonNamedArguments = sourceAttributeData.CommonNamedArguments;
                ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = RetargetAttributeNamedArguments(commonNamedArguments);
                return new RetargetingAttributeData(sourceAttributeData.ApplicationSyntaxReference, attributeClass2, methodSymbol, constructorArguments, sourceAttributeData.ConstructorArgumentsSourceIndices, namedArguments, sourceAttributeData.HasErrors, sourceAttributeData.IsConditionallyOmitted);
            }

            private ImmutableArray<TypedConstant> RetargetAttributeConstructorArguments(ImmutableArray<TypedConstant> constructorArguments)
            {
                ImmutableArray<TypedConstant> result = constructorArguments;
                bool typedConstantChanged = false;
                if (!constructorArguments.IsDefault && constructorArguments.Any())
                {
                    ArrayBuilder<TypedConstant> instance = ArrayBuilder<TypedConstant>.GetInstance(constructorArguments.Length);
                    ImmutableArray<TypedConstant>.Enumerator enumerator = constructorArguments.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        TypedConstant current = enumerator.Current;
                        TypedConstant item = RetargetTypedConstant(current, ref typedConstantChanged);
                        instance.Add(item);
                    }
                    if (typedConstantChanged)
                    {
                        result = instance.ToImmutable();
                    }
                    instance.Free();
                }
                return result;
            }

            private TypedConstant RetargetTypedConstant(TypedConstant oldConstant, ref bool typedConstantChanged)
            {
                TypeSymbol typeSymbol = (TypeSymbol)oldConstant.TypeInternal;
                TypeSymbol typeSymbol2 = (((object)typeSymbol == null) ? null : Retarget(typeSymbol, RetargetOptions.RetargetPrimitiveTypesByTypeCode));
                if (oldConstant.Kind == TypedConstantKind.Array)
                {
                    ImmutableArray<TypedConstant> immutableArray = RetargetAttributeConstructorArguments(oldConstant.Values);
                    if (!TypeSymbol.Equals(typeSymbol2, typeSymbol, TypeCompareKind.ConsiderEverything) || immutableArray != oldConstant.Values)
                    {
                        typedConstantChanged = true;
                        return new TypedConstant(typeSymbol2, immutableArray);
                    }
                    return oldConstant;
                }
                object valueInternal = oldConstant.ValueInternal;
                object obj = ((oldConstant.Kind != TypedConstantKind.Type || valueInternal == null) ? valueInternal : Retarget((TypeSymbol)valueInternal, RetargetOptions.RetargetPrimitiveTypesByTypeCode));
                if (!TypeSymbol.Equals(typeSymbol2, typeSymbol, TypeCompareKind.ConsiderEverything) || obj != valueInternal)
                {
                    typedConstantChanged = true;
                    return new TypedConstant(typeSymbol2, oldConstant.Kind, obj);
                }
                return oldConstant;
            }

            private ImmutableArray<KeyValuePair<string, TypedConstant>> RetargetAttributeNamedArguments(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
            {
                ImmutableArray<KeyValuePair<string, TypedConstant>> result = namedArguments;
                bool flag = false;
                if (namedArguments.Any())
                {
                    ArrayBuilder<KeyValuePair<string, TypedConstant>> instance = ArrayBuilder<KeyValuePair<string, TypedConstant>>.GetInstance(namedArguments.Length);
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = namedArguments.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, TypedConstant> current = enumerator.Current;
                        TypedConstant value = current.Value;
                        bool typedConstantChanged = false;
                        TypedConstant value2 = RetargetTypedConstant(value, ref typedConstantChanged);
                        if (typedConstantChanged)
                        {
                            instance.Add(new KeyValuePair<string, TypedConstant>(current.Key, value2));
                            flag = true;
                        }
                        else
                        {
                            instance.Add(current);
                        }
                    }
                    if (flag)
                    {
                        result = instance.ToImmutable();
                    }
                    instance.Free();
                }
                return result;
            }

            internal ImmutableArray<CSharpAttributeData> GetRetargetedAttributes(ImmutableArray<CSharpAttributeData> underlyingAttributes, ref ImmutableArray<CSharpAttributeData> lazyCustomAttributes)
            {
                if (lazyCustomAttributes.IsDefault)
                {
                    ImmutableArray<CSharpAttributeData> value = RetargetAttributes(underlyingAttributes);
                    ImmutableInterlocked.InterlockedCompareExchange(ref lazyCustomAttributes, value, default(ImmutableArray<CSharpAttributeData>));
                }
                return lazyCustomAttributes;
            }

            public override Symbol VisitModule(ModuleSymbol symbol, RetargetOptions options)
            {
                return _retargetingModule;
            }

            public override Symbol VisitNamespace(NamespaceSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitNamedType(NamedTypeSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol, options);
            }

            public override Symbol VisitArrayType(ArrayTypeSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitPointerType(PointerTypeSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitFunctionPointerType(FunctionPointerTypeSymbol symbol, RetargetOptions argument)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitMethod(MethodSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitParameter(ParameterSymbol symbol, RetargetOptions options)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override Symbol VisitField(FieldSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitProperty(PropertySymbol symbol, RetargetOptions argument)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitTypeParameter(TypeParameterSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitErrorType(ErrorTypeSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitEvent(EventSymbol symbol, RetargetOptions options)
            {
                return Retarget(symbol);
            }

            public override Symbol VisitDynamicType(DynamicTypeSymbol symbol, RetargetOptions argument)
            {
                return symbol;
            }
        }

        private readonly RetargetingAssemblySymbol _retargetingAssembly;

        private readonly SourceModuleSymbol _underlyingModule;

        private readonly Dictionary<AssemblySymbol, DestinationData> _retargetingAssemblyMap = new Dictionary<AssemblySymbol, DestinationData>();

        internal readonly RetargetingSymbolTranslator RetargetingTranslator;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private readonly ConcurrentDictionary<Symbol, Symbol> _symbolMap = new ConcurrentDictionary<Symbol, Symbol>(2, 4);

        private readonly Func<Symbol, RetargetingMethodSymbol> _createRetargetingMethod;

        private readonly Func<Symbol, RetargetingNamespaceSymbol> _createRetargetingNamespace;

        private readonly Func<Symbol, RetargetingTypeParameterSymbol> _createRetargetingTypeParameter;

        private readonly Func<Symbol, RetargetingNamedTypeSymbol> _createRetargetingNamedType;

        private readonly Func<Symbol, FieldSymbol> _createRetargetingField;

        private readonly Func<Symbol, RetargetingPropertySymbol> _createRetargetingProperty;

        private readonly Func<Symbol, RetargetingEventSymbol> _createRetargetingEvent;

        internal override int Ordinal => 0;

        internal override Machine Machine => _underlyingModule.Machine;

        internal override bool Bit32Required => _underlyingModule.Bit32Required;

        public SourceModuleSymbol UnderlyingModule => _underlyingModule;

        public override NamespaceSymbol GlobalNamespace => RetargetingTranslator.Retarget(_underlyingModule.GlobalNamespace);

        public override bool IsImplicitlyDeclared => _underlyingModule.IsImplicitlyDeclared;

        public override string Name => _underlyingModule.Name;

        public override Symbol ContainingSymbol => _retargetingAssembly;

        public override AssemblySymbol ContainingAssembly => _retargetingAssembly;

        public override ImmutableArray<Location> Locations => _underlyingModule.Locations;

        internal override ICollection<string> TypeNames => _underlyingModule.TypeNames;

        internal override ICollection<string> NamespaceNames => _underlyingModule.NamespaceNames;

        internal override bool HasAssemblyCompilationRelaxationsAttribute => _underlyingModule.HasAssemblyCompilationRelaxationsAttribute;

        internal override bool HasAssemblyRuntimeCompatibilityAttribute => _underlyingModule.HasAssemblyRuntimeCompatibilityAttribute;

        internal override CharSet? DefaultMarshallingCharSet => _underlyingModule.DefaultMarshallingCharSet;

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public RetargetingModuleSymbol(RetargetingAssemblySymbol retargetingAssembly, SourceModuleSymbol underlyingModule)
        {
            _retargetingAssembly = retargetingAssembly;
            _underlyingModule = underlyingModule;
            RetargetingTranslator = new RetargetingSymbolTranslator(this);
            _createRetargetingMethod = CreateRetargetingMethod;
            _createRetargetingNamespace = CreateRetargetingNamespace;
            _createRetargetingNamedType = CreateRetargetingNamedType;
            _createRetargetingField = CreateRetargetingField;
            _createRetargetingProperty = CreateRetargetingProperty;
            _createRetargetingEvent = CreateRetargetingEvent;
            _createRetargetingTypeParameter = CreateRetargetingTypeParameter;
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _underlyingModule.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
        }

        internal override void SetReferences(ModuleReferences<AssemblySymbol> moduleReferences, SourceAssemblySymbol originatingSourceAssemblyDebugOnly)
        {
            base.SetReferences(moduleReferences, originatingSourceAssemblyDebugOnly);
            _retargetingAssemblyMap.Clear();
            ImmutableArray<AssemblySymbol> referencedAssemblySymbols = _underlyingModule.GetReferencedAssemblySymbols();
            ImmutableArray<AssemblySymbol> symbols = moduleReferences.Symbols;
            int num = 0;
            int i = 0;
            while (num < symbols.Length)
            {
                for (; referencedAssemblySymbols[i].IsLinked; i++)
                {
                }
                if ((object)symbols[num] != referencedAssemblySymbols[i] && !_retargetingAssemblyMap.TryGetValue(referencedAssemblySymbols[i], out var _))
                {
                    _retargetingAssemblyMap.Add(referencedAssemblySymbols[i], new DestinationData
                    {
                        To = symbols[num]
                    });
                }
                num++;
                i++;
            }
        }

        internal bool RetargetingDefinitions(AssemblySymbol from, out AssemblySymbol to)
        {
            if (!_retargetingAssemblyMap.TryGetValue(from, out var value))
            {
                to = null;
                return false;
            }
            to = value.To;
            return true;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return RetargetingTranslator.GetRetargetedAttributes(_underlyingModule.GetAttributes(), ref _lazyCustomAttributes);
        }

        public override ModuleMetadata GetMetadata()
        {
            return _underlyingModule.GetMetadata();
        }

        private RetargetingMethodSymbol CreateRetargetingMethod(Symbol symbol)
        {
            return new RetargetingMethodSymbol(this, (MethodSymbol)symbol);
        }

        private RetargetingNamespaceSymbol CreateRetargetingNamespace(Symbol symbol)
        {
            return new RetargetingNamespaceSymbol(this, (NamespaceSymbol)symbol);
        }

        private RetargetingNamedTypeSymbol CreateRetargetingNamedType(Symbol symbol)
        {
            return new RetargetingNamedTypeSymbol(this, (NamedTypeSymbol)symbol);
        }

        private FieldSymbol CreateRetargetingField(Symbol symbol)
        {
            if (symbol is TupleErrorFieldSymbol tupleErrorFieldSymbol)
            {
                FieldSymbol correspondingTupleField = tupleErrorFieldSymbol.CorrespondingTupleField;
                TupleErrorFieldSymbol correspondingDefaultFieldOpt = (((object)correspondingTupleField == tupleErrorFieldSymbol) ? null : ((TupleErrorFieldSymbol)RetargetingTranslator.Retarget(correspondingTupleField)));
                return new TupleErrorFieldSymbol(RetargetingTranslator.Retarget(tupleErrorFieldSymbol.ContainingType, RetargetOptions.RetargetPrimitiveTypesByName), tupleErrorFieldSymbol.Name, tupleErrorFieldSymbol.TupleElementIndex, tupleErrorFieldSymbol.Locations.IsEmpty ? null : tupleErrorFieldSymbol.Locations[0], RetargetingTranslator.Retarget(tupleErrorFieldSymbol.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode), tupleErrorFieldSymbol.GetUseSiteInfo().DiagnosticInfo, tupleErrorFieldSymbol.IsImplicitlyDeclared, correspondingDefaultFieldOpt);
            }
            return new RetargetingFieldSymbol(this, (FieldSymbol)symbol);
        }

        private RetargetingPropertySymbol CreateRetargetingProperty(Symbol symbol)
        {
            return new RetargetingPropertySymbol(this, (PropertySymbol)symbol);
        }

        private RetargetingEventSymbol CreateRetargetingEvent(Symbol symbol)
        {
            return new RetargetingEventSymbol(this, (EventSymbol)symbol);
        }

        private RetargetingTypeParameterSymbol CreateRetargetingTypeParameter(Symbol symbol)
        {
            return new RetargetingTypeParameterSymbol(this, (TypeParameterSymbol)symbol);
        }
    }
}
