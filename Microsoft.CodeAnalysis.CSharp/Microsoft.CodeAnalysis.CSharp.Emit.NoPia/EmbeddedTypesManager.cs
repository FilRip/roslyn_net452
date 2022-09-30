using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Emit.NoPia
{
    public sealed class EmbeddedTypesManager : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, CSharpAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>
    {
        private readonly ConcurrentDictionary<AssemblySymbol, string> _assemblyGuidMap = new ConcurrentDictionary<AssemblySymbol, string>(ReferenceEqualityComparer.Instance);

        private readonly ConcurrentDictionary<Symbol, bool> _reportedSymbolsMap = new ConcurrentDictionary<Symbol, bool>(ReferenceEqualityComparer.Instance);

        private NamedTypeSymbol _lazySystemStringType = ErrorTypeSymbol.UnknownResultType;

        private readonly MethodSymbol[] _lazyWellKnownTypeMethods;

        public EmbeddedTypesManager(PEModuleBuilder moduleBeingBuilt)
            : base(moduleBeingBuilt)
        {
            _lazyWellKnownTypeMethods = new MethodSymbol[418];
            for (int i = 0; i < _lazyWellKnownTypeMethods.Length; i++)
            {
                _lazyWellKnownTypeMethods[i] = ErrorMethodSymbol.UnknownMethod;
            }
        }

        public NamedTypeSymbol GetSystemStringType(SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            if ((object)_lazySystemStringType == ErrorTypeSymbol.UnknownResultType)
            {
                NamedTypeSymbol namedTypeSymbol = ModuleBeingBuilt.Compilation.GetSpecialType(SpecialType.System_String);
                UseSiteInfo<AssemblySymbol> useSiteInfo = namedTypeSymbol.GetUseSiteInfo();
                if (namedTypeSymbol.IsErrorType())
                {
                    namedTypeSymbol = null;
                }
                if (TypeSymbol.Equals(Interlocked.CompareExchange(ref _lazySystemStringType, namedTypeSymbol, ErrorTypeSymbol.UnknownResultType), ErrorTypeSymbol.UnknownResultType, TypeCompareKind.ConsiderEverything) && useSiteInfo.DiagnosticInfo != null)
                {
                    Symbol.ReportUseSiteDiagnostic(useSiteInfo.DiagnosticInfo, diagnostics, (syntaxNodeOpt != null) ? syntaxNodeOpt.Location : NoLocation.Singleton);
                }
            }
            return _lazySystemStringType;
        }

        public MethodSymbol GetWellKnownMethod(WellKnownMember method, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return LazyGetWellKnownTypeMethod(ref _lazyWellKnownTypeMethods[(int)method], method, syntaxNodeOpt, diagnostics);
        }

        private MethodSymbol LazyGetWellKnownTypeMethod(ref MethodSymbol lazyMethod, WellKnownMember member, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            if ((object)lazyMethod == ErrorMethodSymbol.UnknownMethod)
            {
                MethodSymbol value = (MethodSymbol)Binder.GetWellKnownTypeMember(ModuleBeingBuilt.Compilation, member, out UseSiteInfo<AssemblySymbol> useSiteInfo);
                DiagnosticInfo? diagnosticInfo = useSiteInfo.DiagnosticInfo;
                if (diagnosticInfo != null && diagnosticInfo!.Severity == DiagnosticSeverity.Error)
                {
                    value = null;
                }
                if (Interlocked.CompareExchange(ref lazyMethod, value, ErrorMethodSymbol.UnknownMethod) == ErrorMethodSymbol.UnknownMethod && useSiteInfo.DiagnosticInfo != null)
                {
                    Symbol.ReportUseSiteDiagnostic(useSiteInfo.DiagnosticInfo, diagnostics, (syntaxNodeOpt != null) ? syntaxNodeOpt.Location : NoLocation.Singleton);
                }
            }
            return lazyMethod;
        }

        public override int GetTargetAttributeSignatureIndex(Symbol underlyingSymbol, CSharpAttributeData attrData, AttributeDescription description)
        {
            return attrData.GetTargetAttributeSignatureIndex(underlyingSymbol.AdaptedSymbol, description);
        }

        public override CSharpAttributeData CreateSynthesizedAttribute(WellKnownMember constructor, CSharpAttributeData attrData, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            MethodSymbol wellKnownMethod = GetWellKnownMethod(constructor, syntaxNodeOpt, diagnostics);
            if ((object)wellKnownMethod == null)
            {
                return null;
            }
            return constructor switch
            {
                WellKnownMember.System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor => new SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(attrData.CommonConstructorArguments[0], attrData.CommonConstructorArguments[0]), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty),
                WellKnownMember.System_Runtime_InteropServices_CoClassAttribute__ctor => new SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(new TypedConstant(wellKnownMethod.Parameters[0].Type, TypedConstantKind.Type, wellKnownMethod.ContainingAssembly.GetSpecialType(SpecialType.System_Object))), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty),
                _ => new SynthesizedAttributeData(wellKnownMethod, attrData.CommonConstructorArguments, attrData.CommonNamedArguments),
            };
        }

        internal string GetAssemblyGuidString(AssemblySymbol assembly)
        {
            if (_assemblyGuidMap.TryGetValue(assembly, out var value))
            {
                return value;
            }
            assembly.GetGuidString(out value);
            return _assemblyGuidMap.GetOrAdd(assembly, value);
        }

        protected override void OnGetTypesCompleted(ImmutableArray<EmbeddedType> types, DiagnosticBag diagnostics)
        {
            ImmutableArray<EmbeddedType>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EmbeddedType current = enumerator.Current;
                _assemblyGuidMap.TryAdd(current.UnderlyingNamedType.AdaptedSymbol.ContainingAssembly, null);
            }
            foreach (AssemblySymbol item in ModuleBeingBuilt.GetReferencedAssembliesUsedSoFar())
            {
                ReportIndirectReferencesToLinkedAssemblies(item, diagnostics);
            }
        }

        protected override void ReportNameCollisionBetweenEmbeddedTypes(EmbeddedType typeA, EmbeddedType typeB, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol underlyingNamedType = typeA.UnderlyingNamedType;
            NamedTypeSymbol underlyingNamedType2 = typeB.UnderlyingNamedType;
            Error(diagnostics, ErrorCode.ERR_InteropTypesWithSameNameAndGuid, null, underlyingNamedType.AdaptedNamedTypeSymbol, underlyingNamedType.AdaptedSymbol.ContainingAssembly, underlyingNamedType2.AdaptedSymbol.ContainingAssembly);
        }

        protected override void ReportNameCollisionWithAlreadyDeclaredType(EmbeddedType type, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol underlyingNamedType = type.UnderlyingNamedType;
            Error(diagnostics, ErrorCode.ERR_LocalTypeNameClash, null, underlyingNamedType.AdaptedNamedTypeSymbol, underlyingNamedType.AdaptedSymbol.ContainingAssembly);
        }

        public override void ReportIndirectReferencesToLinkedAssemblies(AssemblySymbol a, DiagnosticBag diagnostics)
        {
            ImmutableArray<ModuleSymbol>.Enumerator enumerator = a.Modules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<AssemblySymbol>.Enumerator enumerator2 = enumerator.Current.GetReferencedAssemblySymbols().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    AssemblySymbol current = enumerator2.Current;
                    if (!current.IsMissing && current.IsLinked && _assemblyGuidMap.ContainsKey(current))
                    {
                        Error(diagnostics, ErrorCode.WRN_ReferencedAssemblyReferencesLinkedPIA, null, current, a);
                    }
                }
            }
        }

        internal static bool IsValidEmbeddableType(NamedTypeSymbol namedType, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, EmbeddedTypesManager optTypeManager = null)
        {
            if (namedType.SpecialType != 0 || namedType.IsErrorType() || !namedType.ContainingAssembly.IsLinked)
            {
                return false;
            }
            ErrorCode errorCode = ErrorCode.Unknown;
            switch (namedType.TypeKind)
            {
                case TypeKind.Interface:
                    {
                        ImmutableArray<Symbol>.Enumerator enumerator = namedType.GetMembersUnordered().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Symbol current = enumerator.Current;
                            if (current.Kind != SymbolKind.NamedType)
                            {
                                if (!current.IsAbstract)
                                {
                                    errorCode = ErrorCode.ERR_DefaultInterfaceImplementationInNoPIAType;
                                    break;
                                }
                                if (current.IsSealed)
                                {
                                    errorCode = ErrorCode.ERR_ReAbstractionInNoPIAType;
                                    break;
                                }
                            }
                        }
                        if (errorCode != ErrorCode.Unknown)
                        {
                            break;
                        }
                        goto case TypeKind.Delegate;
                    }
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Struct:
                    if ((object)namedType.ContainingType != null)
                    {
                        errorCode = ErrorCode.ERR_NoPIANestedType;
                    }
                    else if (namedType.IsGenericType)
                    {
                        errorCode = ErrorCode.ERR_GenericsUsedInNoPIAType;
                    }
                    break;
                default:
                    errorCode = ErrorCode.ERR_NewCoClassOnLink;
                    break;
            }
            if (errorCode != ErrorCode.Unknown)
            {
                ReportNotEmbeddableSymbol(errorCode, namedType, syntaxNodeOpt, diagnostics, optTypeManager);
                return false;
            }
            return true;
        }

        private static void ReportNotEmbeddableSymbol(ErrorCode error, Symbol symbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, EmbeddedTypesManager optTypeManager)
        {
            if (optTypeManager == null || optTypeManager._reportedSymbolsMap.TryAdd(symbol.OriginalDefinition, value: true))
            {
                Error(diagnostics, error, syntaxNodeOpt, symbol.OriginalDefinition);
            }
        }

        internal static void Error(DiagnosticBag diagnostics, ErrorCode code, SyntaxNode syntaxOpt, params object[] args)
        {
            Error(diagnostics, syntaxOpt, new CSDiagnosticInfo(code, args));
        }

        private static void Error(DiagnosticBag diagnostics, SyntaxNode syntaxOpt, DiagnosticInfo info)
        {
            diagnostics.Add(new CSDiagnostic(info, (syntaxOpt == null) ? NoLocation.Singleton : syntaxOpt.Location));
        }

        internal INamedTypeReference EmbedTypeIfNeedTo(NamedTypeSymbol namedType, bool fromImplements, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            if (IsValidEmbeddableType(namedType, syntaxNodeOpt, diagnostics, this))
            {
                return EmbedType(namedType, fromImplements, syntaxNodeOpt, diagnostics);
            }
            return null;
        }

        private EmbeddedType EmbedType(NamedTypeSymbol namedType, bool fromImplements, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol cciAdapter = namedType.GetCciAdapter();
            EmbeddedType embeddedType = new EmbeddedType(this, cciAdapter);
            EmbeddedType orAdd = EmbeddedTypesMap.GetOrAdd(cciAdapter, embeddedType);
            bool isInterface = namedType.IsInterface;
            if (isInterface && fromImplements)
            {
                orAdd.EmbedAllMembersOfImplementedInterface(syntaxNodeOpt, diagnostics);
            }
            if (embeddedType != orAdd)
            {
                return orAdd;
            }
            new TypeReferenceIndexer(new EmitContext(ModuleBeingBuilt, syntaxNodeOpt, diagnostics, metadataOnly: false, includePrivateMembers: true)).VisitTypeDefinitionNoMembers(embeddedType);
            if (!isInterface)
            {
                if (namedType.TypeKind != TypeKind.Struct)
                {
                    _ = namedType.TypeKind;
                    _ = 5;
                }
                foreach (FieldSymbol item in namedType.GetFieldsToEmit())
                {
                    EmbedField(embeddedType, item.GetCciAdapter(), syntaxNodeOpt, diagnostics);
                }
                {
                    foreach (MethodSymbol item2 in namedType.GetMethodsToEmit())
                    {
                        if ((object)item2 != null)
                        {
                            EmbedMethod(embeddedType, item2.GetCciAdapter(), syntaxNodeOpt, diagnostics);
                        }
                    }
                    return embeddedType;
                }
            }
            return embeddedType;
        }

        public override EmbeddedField EmbedField(EmbeddedType type, FieldSymbol field, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            EmbeddedField embeddedField = new EmbeddedField(type, field);
            EmbeddedField orAdd = EmbeddedFieldsMap.GetOrAdd(field, embeddedField);
            if (embeddedField != orAdd)
            {
                return orAdd;
            }
            EmbedReferences(embeddedField, syntaxNodeOpt, diagnostics);
            TypeKind typeKind = field.AdaptedFieldSymbol.ContainingType.TypeKind;
            if (typeKind == TypeKind.Interface || typeKind == TypeKind.Delegate || (typeKind == TypeKind.Struct && (field.AdaptedFieldSymbol.IsStatic || field.AdaptedFieldSymbol.DeclaredAccessibility != Accessibility.Public)))
            {
                ReportNotEmbeddableSymbol(ErrorCode.ERR_InteropStructContainsMethods, field.AdaptedFieldSymbol.ContainingType, syntaxNodeOpt, diagnostics, this);
            }
            return embeddedField;
        }

        public override EmbeddedMethod EmbedMethod(EmbeddedType type, MethodSymbol method, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            EmbeddedMethod embeddedMethod = new EmbeddedMethod(type, method);
            EmbeddedMethod orAdd = EmbeddedMethodsMap.GetOrAdd(method, embeddedMethod);
            if (embeddedMethod != orAdd)
            {
                return orAdd;
            }
            EmbedReferences(embeddedMethod, syntaxNodeOpt, diagnostics);
            TypeKind typeKind = type.UnderlyingNamedType.AdaptedNamedTypeSymbol.TypeKind;
            if (typeKind == TypeKind.Enum || typeKind == TypeKind.Struct)
            {
                ReportNotEmbeddableSymbol(ErrorCode.ERR_InteropStructContainsMethods, type.UnderlyingNamedType.AdaptedNamedTypeSymbol, syntaxNodeOpt, diagnostics, this);
            }
            else if (embeddedMethod.HasBody())
            {
                Error(diagnostics, ErrorCode.ERR_InteropMethodWithBody, syntaxNodeOpt, method.AdaptedMethodSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }
            Symbol associatedSymbol = method.AdaptedMethodSymbol.AssociatedSymbol;
            if ((object)associatedSymbol != null)
            {
                switch (associatedSymbol.Kind)
                {
                    case SymbolKind.Property:
                        EmbedProperty(type, ((PropertySymbol)associatedSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics);
                        break;
                    case SymbolKind.Event:
                        EmbedEvent(type, ((EventSymbol)associatedSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding: false);
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(associatedSymbol.Kind);
                }
            }
            return embeddedMethod;
        }

        public override EmbeddedProperty EmbedProperty(EmbeddedType type, PropertySymbol property, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            MethodSymbol methodSymbol = property.AdaptedPropertySymbol.GetMethod?.GetCciAdapter();
            MethodSymbol methodSymbol2 = property.AdaptedPropertySymbol.SetMethod?.GetCciAdapter();
            EmbeddedMethod getter = (((object)methodSymbol != null) ? EmbedMethod(type, methodSymbol, syntaxNodeOpt, diagnostics) : null);
            EmbeddedMethod setter = (((object)methodSymbol2 != null) ? EmbedMethod(type, methodSymbol2, syntaxNodeOpt, diagnostics) : null);
            EmbeddedProperty embeddedProperty = new EmbeddedProperty(property, getter, setter);
            EmbeddedProperty orAdd = EmbeddedPropertiesMap.GetOrAdd(property, embeddedProperty);
            if (embeddedProperty != orAdd)
            {
                return orAdd;
            }
            EmbedReferences(embeddedProperty, syntaxNodeOpt, diagnostics);
            return embeddedProperty;
        }

        public override EmbeddedEvent EmbedEvent(EmbeddedType type, EventSymbol @event, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding)
        {
            MethodSymbol methodSymbol = @event.AdaptedEventSymbol.AddMethod?.GetCciAdapter();
            MethodSymbol methodSymbol2 = @event.AdaptedEventSymbol.RemoveMethod?.GetCciAdapter();
            EmbeddedMethod adder = (((object)methodSymbol != null) ? EmbedMethod(type, methodSymbol, syntaxNodeOpt, diagnostics) : null);
            EmbeddedMethod remover = (((object)methodSymbol2 != null) ? EmbedMethod(type, methodSymbol2, syntaxNodeOpt, diagnostics) : null);
            EmbeddedEvent embeddedEvent = new EmbeddedEvent(@event, adder, remover);
            EmbeddedEvent orAdd = EmbeddedEventsMap.GetOrAdd(@event, embeddedEvent);
            if (embeddedEvent != orAdd)
            {
                if (isUsedForComAwareEventBinding)
                {
                    orAdd.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding);
                }
                return orAdd;
            }
            EmbedReferences(embeddedEvent, syntaxNodeOpt, diagnostics);
            embeddedEvent.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding);
            return embeddedEvent;
        }

        protected override EmbeddedType GetEmbeddedTypeForMember(Symbol member, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol containingType = member.AdaptedSymbol.ContainingType;
            if (IsValidEmbeddableType(containingType, syntaxNodeOpt, diagnostics, this))
            {
                return EmbedType(containingType, fromImplements: false, syntaxNodeOpt, diagnostics);
            }
            return null;
        }

        internal static ImmutableArray<EmbeddedParameter> EmbedParameters(CommonEmbeddedMember containingPropertyOrMethod, ImmutableArray<ParameterSymbol> underlyingParameters)
        {
            return underlyingParameters.SelectAsArray((ParameterSymbol p, CommonEmbeddedMember c) => new EmbeddedParameter(c, p.GetCciAdapter()), containingPropertyOrMethod);
        }

        protected override CSharpAttributeData CreateCompilerGeneratedAttribute()
        {
            return ModuleBeingBuilt.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor);
        }
    }
}
