using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit.NoPia;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    public abstract class PEModuleBuilder : PEModuleBuilder<CSharpCompilation, SourceModuleSymbol, AssemblySymbol, TypeSymbol, NamedTypeSymbol, MethodSymbol, SyntaxNode, EmbeddedTypesManager, ModuleCompilationState>
    {
        protected readonly ConcurrentDictionary<Symbol, IModuleReference> AssemblyOrModuleSymbolToModuleRefMap = new ConcurrentDictionary<Symbol, IModuleReference>();

        private readonly ConcurrentDictionary<Symbol, object> _genericInstanceMap = new ConcurrentDictionary<Symbol, object>(Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything);

        private readonly ConcurrentSet<TypeSymbol> _reportedErrorTypesMap = new ConcurrentSet<TypeSymbol>();

        private readonly EmbeddedTypesManager _embeddedTypesManagerOpt;

        private readonly string _metadataName;

        private ImmutableArray<ExportedType> _lazyExportedTypes;

        private Dictionary<FieldSymbol, NamedTypeSymbol> _fixedImplementationTypes;

        private int _needsGeneratedAttributes;

        private bool _needsGeneratedAttributes_IsFrozen;

        public override EmbeddedTypesManager EmbeddedTypesManagerOpt => _embeddedTypesManagerOpt;

        public override string Name => _metadataName;

        public override string ModuleName => _metadataName;

        public sealed override AssemblySymbol CorLibrary => SourceModule.ContainingSourceAssembly.CorLibrary;

        public sealed override bool GenerateVisualBasicStylePdb => false;

        public sealed override IEnumerable<string> LinkedAssembliesDebugInfo => SpecializedCollections.EmptyEnumerable<string>();

        public sealed override string DefaultNamespace => null;

        internal virtual bool IgnoreAccessibility => false;

        internal virtual bool IsEncDelta => false;

        internal EmbeddableAttributes GetNeedsGeneratedAttributes()
        {
            _needsGeneratedAttributes_IsFrozen = true;
            return GetNeedsGeneratedAttributesInternal();
        }

        private EmbeddableAttributes GetNeedsGeneratedAttributesInternal()
        {
            return (EmbeddableAttributes)(_needsGeneratedAttributes | (int)Compilation.GetNeedsGeneratedAttributes());
        }

        private void SetNeedsGeneratedAttributes(EmbeddableAttributes attributes)
        {
            ThreadSafeFlagOperations.Set(ref _needsGeneratedAttributes, (int)attributes);
        }

        internal PEModuleBuilder(SourceModuleSymbol sourceModule, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources)
            : base(sourceModule.ContainingSourceAssembly.DeclaringCompilation, sourceModule, serializationProperties, manifestResources, outputKind, emitOptions, new ModuleCompilationState())
        {
            string metadataName = sourceModule.MetadataName;
            _metadataName = ((metadataName != "?") ? metadataName : (emitOptions.OutputNameOverride ?? metadataName));
            AssemblyOrModuleSymbolToModuleRefMap.Add(sourceModule, this);
            if (sourceModule.AnyReferencedAssembliesAreLinked)
            {
                _embeddedTypesManagerOpt = new EmbeddedTypesManager(this);
            }
        }

        public override ICustomAttribute SynthesizeAttribute(WellKnownMember attributeConstructor)
        {
            return Compilation.TrySynthesizeAttribute(attributeConstructor);
        }

        public sealed override IEnumerable<ICustomAttribute> GetSourceAssemblyAttributes(bool isRefAssembly)
        {
            return SourceModule.ContainingSourceAssembly.GetCustomAttributesToEmit(this, isRefAssembly, OutputKind.IsNetModule());
        }

        public sealed override IEnumerable<SecurityAttribute> GetSourceAssemblySecurityAttributes()
        {
            return SourceModule.ContainingSourceAssembly.GetSecurityAttributes();
        }

        public sealed override IEnumerable<ICustomAttribute> GetSourceModuleAttributes()
        {
            return SourceModule.GetCustomAttributesToEmit(this);
        }

        public sealed override ImmutableArray<UsedNamespaceOrType> GetImports()
        {
            return ImmutableArray<UsedNamespaceOrType>.Empty;
        }

        protected sealed override IEnumerable<IAssemblyReference> GetAssemblyReferencesFromAddedModules(DiagnosticBag diagnostics)
        {
            ImmutableArray<ModuleSymbol> modules = SourceModule.ContainingAssembly.Modules;
            for (int i = 1; i < modules.Length; i++)
            {
                ImmutableArray<AssemblySymbol>.Enumerator enumerator = modules[i].GetReferencedAssemblySymbols().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AssemblySymbol current = enumerator.Current;
                    yield return Translate(current, diagnostics);
                }
            }
        }

        private void ValidateReferencedAssembly(AssemblySymbol assembly, AssemblyReference asmRef, DiagnosticBag diagnostics)
        {
            AssemblyIdentity identity = SourceModule.ContainingAssembly.Identity;
            AssemblyIdentity identity2 = asmRef.Identity;
            if (identity.IsStrongName && !identity2.IsStrongName && asmRef.Identity.ContentType != AssemblyContentType.WindowsRuntime)
            {
                diagnostics.Add(new CSDiagnosticInfo(ErrorCode.WRN_ReferencedAssemblyDoesNotHaveStrongName, assembly), NoLocation.Singleton);
            }
            if (OutputKind != OutputKind.NetModule && !string.IsNullOrEmpty(identity2.CultureName) && !string.Equals(identity2.CultureName, identity.CultureName, StringComparison.OrdinalIgnoreCase))
            {
                diagnostics.Add(new CSDiagnosticInfo(ErrorCode.WRN_RefCultureMismatch, assembly, identity2.CultureName), NoLocation.Singleton);
            }
            Machine machine = assembly.Machine;
            if ((object)assembly != assembly.CorLibrary && (machine != Machine.I386 || assembly.Bit32Required))
            {
                Machine machine2 = SourceModule.Machine;
                if ((machine2 != Machine.I386 || SourceModule.Bit32Required) && machine2 != machine)
                {
                    diagnostics.Add(new CSDiagnosticInfo(ErrorCode.WRN_ConflictingMachineAssembly, assembly), NoLocation.Singleton);
                }
            }
            if (_embeddedTypesManagerOpt != null && _embeddedTypesManagerOpt.IsFrozen)
            {
                _embeddedTypesManagerOpt.ReportIndirectReferencesToLinkedAssemblies(assembly, diagnostics);
            }
        }

        public sealed override IEnumerable<INestedTypeDefinition> GetSynthesizedNestedTypes(NamedTypeSymbol container)
        {
            return null;
        }

        public sealed override MultiDictionary<DebugSourceDocument, DefinitionWithLocation> GetSymbolToLocationMap()
        {
            MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result = new MultiDictionary<DebugSourceDocument, DefinitionWithLocation>();
            Stack<NamespaceOrTypeSymbol> stack = new Stack<NamespaceOrTypeSymbol>();
            stack.Push(SourceModule.GlobalNamespace);
            Location location = null;
            while (stack.Count > 0)
            {
                NamespaceOrTypeSymbol namespaceOrTypeSymbol = stack.Pop();
                switch (namespaceOrTypeSymbol.Kind)
                {
                    case SymbolKind.Namespace:
                        {
                            location = GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol);
                            if (!(location != null))
                            {
                                break;
                            }
                            ImmutableArray<Symbol>.Enumerator enumerator = namespaceOrTypeSymbol.GetMembers().GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Symbol current2 = enumerator.Current;
                                SymbolKind kind = current2.Kind;
                                if ((uint)(kind - 11) <= 1u)
                                {
                                    stack.Push((NamespaceOrTypeSymbol)current2);
                                    continue;
                                }
                                throw ExceptionUtilities.UnexpectedValue(current2.Kind);
                            }
                            break;
                        }
                    case SymbolKind.NamedType:
                        {
                            location = GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol);
                            if (!(location != null))
                            {
                                break;
                            }
                            AddSymbolLocation(result, location, (IDefinition)namespaceOrTypeSymbol.GetCciAdapter());
                            ImmutableArray<Symbol>.Enumerator enumerator = namespaceOrTypeSymbol.GetMembers().GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Symbol current = enumerator.Current;
                                switch (current.Kind)
                                {
                                    case SymbolKind.NamedType:
                                        stack.Push((NamespaceOrTypeSymbol)current);
                                        break;
                                    case SymbolKind.Method:
                                        if (((MethodSymbol)current).ShouldEmit())
                                        {
                                            AddSymbolLocation(result, current);
                                        }
                                        break;
                                    case SymbolKind.Property:
                                        AddSymbolLocation(result, current);
                                        break;
                                    case SymbolKind.Field:
                                        if (!(current is TupleErrorFieldSymbol))
                                        {
                                            AddSymbolLocation(result, current);
                                        }
                                        break;
                                    case SymbolKind.Event:
                                        {
                                            AddSymbolLocation(result, current);
                                            FieldSymbol associatedField = ((EventSymbol)current).AssociatedField;
                                            if ((object)associatedField != null)
                                            {
                                                AddSymbolLocation(result, associatedField);
                                            }
                                            break;
                                        }
                                    default:
                                        throw ExceptionUtilities.UnexpectedValue(current.Kind);
                                }
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(namespaceOrTypeSymbol.Kind);
                }
            }
            return result;
        }

        private void AddSymbolLocation(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result, Symbol symbol)
        {
            Location smallestSourceLocationOrNull = GetSmallestSourceLocationOrNull(symbol);
            if (smallestSourceLocationOrNull != null)
            {
                AddSymbolLocation(result, smallestSourceLocationOrNull, (IDefinition)symbol.GetCciAdapter());
            }
        }

        private void AddSymbolLocation(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result, Location location, IDefinition definition)
        {
            FileLinePositionSpan lineSpan = location.GetLineSpan();
            DebugSourceDocument debugSourceDocument = DebugDocumentsBuilder.TryGetDebugDocument(lineSpan.Path, location.SourceTree!.FilePath);
            if (debugSourceDocument != null)
            {
                result.Add(debugSourceDocument, new DefinitionWithLocation(definition, lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character, lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character));
            }
        }

        private Location GetSmallestSourceLocationOrNull(Symbol symbol)
        {
            CSharpCompilation declaringCompilation = symbol.DeclaringCompilation;
            Location location = null;
            ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Location current = enumerator.Current;
                if (current.IsInSource && (location == null || declaringCompilation.CompareSourceLocations(location, current) > 0))
                {
                    location = current;
                }
            }
            return location;
        }

        internal virtual NamedTypeSymbol GetDynamicOperationContextType(NamedTypeSymbol contextType)
        {
            return contextType;
        }

        internal virtual VariableSlotAllocator TryCreateVariableSlotAllocator(MethodSymbol method, MethodSymbol topLevelMethod, DiagnosticBag diagnostics)
        {
            return null;
        }

        internal virtual ImmutableArray<AnonymousTypeKey> GetPreviousAnonymousTypes()
        {
            return ImmutableArray<AnonymousTypeKey>.Empty;
        }

        internal virtual int GetNextAnonymousTypeIndex()
        {
            return 0;
        }

        internal virtual bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeTemplateSymbol template, out string name, out int index)
        {
            name = null;
            index = -1;
            return false;
        }

        public sealed override IEnumerable<INamespaceTypeDefinition> GetAnonymousTypeDefinitions(EmitContext context)
        {
            if (context.MetadataOnly)
            {
                return SpecializedCollections.EmptyEnumerable<INamespaceTypeDefinition>();
            }
            return Compilation.AnonymousTypeManager.GetAllCreatedTemplates();
        }

        public override IEnumerable<INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context)
        {
            Stack<NamespaceSymbol> namespacesToProcess = new Stack<NamespaceSymbol>();
            namespacesToProcess.Push(SourceModule.GlobalNamespace);
            while (namespacesToProcess.Count > 0)
            {
                NamespaceSymbol namespaceSymbol = namespacesToProcess.Pop();
                ImmutableArray<Symbol>.Enumerator enumerator = namespaceSymbol.GetMembers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.Namespace)
                    {
                        namespacesToProcess.Push((NamespaceSymbol)current);
                    }
                    else
                    {
                        yield return ((NamedTypeSymbol)current).GetCciAdapter();
                    }
                }
            }
        }

        private static void GetExportedTypes(NamespaceOrTypeSymbol symbol, int parentIndex, ArrayBuilder<ExportedType> builder)
        {
            int parentIndex2;
            if (symbol.Kind == SymbolKind.NamedType)
            {
                if (symbol.DeclaredAccessibility != Accessibility.Public)
                {
                    return;
                }
                parentIndex2 = builder.Count;
                builder.Add(new ExportedType((ITypeReference)symbol.GetCciAdapter(), parentIndex, isForwarder: false));
            }
            else
            {
                parentIndex2 = -1;
            }
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is NamespaceOrTypeSymbol symbol2)
                {
                    GetExportedTypes(symbol2, parentIndex2, builder);
                }
            }
        }

        public sealed override ImmutableArray<ExportedType> GetExportedTypes(DiagnosticBag diagnostics)
        {
            if (_lazyExportedTypes.IsDefault)
            {
                _lazyExportedTypes = CalculateExportedTypes();
                if (_lazyExportedTypes.Length > 0)
                {
                    ReportExportedTypeNameCollisions(_lazyExportedTypes, diagnostics);
                }
            }
            return _lazyExportedTypes;
        }

        private ImmutableArray<ExportedType> CalculateExportedTypes()
        {
            SourceAssemblySymbol containingSourceAssembly = SourceModule.ContainingSourceAssembly;
            ArrayBuilder<ExportedType> instance = ArrayBuilder<ExportedType>.GetInstance();
            if (!OutputKind.IsNetModule())
            {
                ImmutableArray<ModuleSymbol> modules = containingSourceAssembly.Modules;
                for (int i = 1; i < modules.Length; i++)
                {
                    GetExportedTypes(modules[i].GlobalNamespace, -1, instance);
                }
            }
            GetForwardedTypes(containingSourceAssembly, instance);
            return instance.ToImmutableAndFree();
        }

        internal static HashSet<NamedTypeSymbol> GetForwardedTypes(SourceAssemblySymbol sourceAssembly, ArrayBuilder<ExportedType>? builder)
        {
            HashSet<NamedTypeSymbol> hashSet = new HashSet<NamedTypeSymbol>();
            GetForwardedTypes(hashSet, sourceAssembly.GetSourceDecodedWellKnownAttributeData(), builder);
            if (!sourceAssembly.DeclaringCompilation.Options.OutputKind.IsNetModule())
            {
                GetForwardedTypes(hashSet, sourceAssembly.GetNetModuleDecodedWellKnownAttributeData(), builder);
            }
            return hashSet;
        }

        private void ReportExportedTypeNameCollisions(ImmutableArray<ExportedType> exportedTypes, DiagnosticBag diagnostics)
        {
            SourceAssemblySymbol containingSourceAssembly = SourceModule.ContainingSourceAssembly;
            Dictionary<string, NamedTypeSymbol> dictionary = new Dictionary<string, NamedTypeSymbol>(StringOrdinalComparer.Instance);
            ImmutableArray<ExportedType>.Enumerator enumerator = exportedTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)enumerator.Current.Type.GetInternalSymbol();
                if (!namedTypeSymbol.IsTopLevelType())
                {
                    continue;
                }
                string text = MetadataHelpers.BuildQualifiedName(((INamespaceTypeReference)namedTypeSymbol.GetCciAdapter()).NamespaceName, MetadataWriter.GetMangledName(namedTypeSymbol.GetCciAdapter()));
                if (ContainsTopLevelType(text))
                {
                    if ((object)namedTypeSymbol.ContainingAssembly == containingSourceAssembly)
                    {
                        diagnostics.Add(ErrorCode.ERR_ExportedTypeConflictsWithDeclaration, NoLocation.Singleton, namedTypeSymbol, namedTypeSymbol.ContainingModule);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_ForwardedTypeConflictsWithDeclaration, NoLocation.Singleton, namedTypeSymbol);
                    }
                }
                else if (dictionary.TryGetValue(text, out NamedTypeSymbol value))
                {
                    if ((object)namedTypeSymbol.ContainingAssembly == containingSourceAssembly)
                    {
                        diagnostics.Add(ErrorCode.ERR_ExportedTypesConflict, NoLocation.Singleton, namedTypeSymbol, namedTypeSymbol.ContainingModule, value, value.ContainingModule);
                    }
                    else if ((object)value.ContainingAssembly == containingSourceAssembly)
                    {
                        diagnostics.Add(ErrorCode.ERR_ForwardedTypeConflictsWithExportedType, NoLocation.Singleton, namedTypeSymbol, namedTypeSymbol.ContainingAssembly, value, value.ContainingModule);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_ForwardedTypesConflict, NoLocation.Singleton, namedTypeSymbol, namedTypeSymbol.ContainingAssembly, value, value.ContainingAssembly);
                    }
                }
                else
                {
                    dictionary.Add(text, namedTypeSymbol);
                }
            }
        }

        private static void GetForwardedTypes(HashSet<NamedTypeSymbol> seenTopLevelTypes, CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> wellKnownAttributeData, ArrayBuilder<ExportedType>? builder)
        {
            if (wellKnownAttributeData == null || !(wellKnownAttributeData.ForwardedTypes?.Count > 0))
            {
                return;
            }
            ArrayBuilder<(NamedTypeSymbol, int)> instance = ArrayBuilder<(NamedTypeSymbol, int)>.GetInstance();
            IEnumerable<NamedTypeSymbol> enumerable = wellKnownAttributeData.ForwardedTypes;
            if (builder != null)
            {
                enumerable = enumerable.OrderBy((NamedTypeSymbol t) => t.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat));
            }
            foreach (NamedTypeSymbol item in enumerable)
            {
                NamedTypeSymbol originalDefinition = item.OriginalDefinition;
                if (!seenTopLevelTypes.Add(originalDefinition) || builder == null)
                {
                    continue;
                }
                instance.Push((originalDefinition, -1));
                while (instance.Count > 0)
                {
                    var (namedTypeSymbol, parentIndex) = instance.Pop();
                    if (namedTypeSymbol.DeclaredAccessibility != Accessibility.Private)
                    {
                        int count = builder!.Count;
                        builder!.Add(new ExportedType(namedTypeSymbol.GetCciAdapter(), parentIndex, isForwarder: true));
                        ImmutableArray<NamedTypeSymbol> typeMembers = namedTypeSymbol.GetTypeMembers();
                        for (int num = typeMembers.Length - 1; num >= 0; num--)
                        {
                            instance.Push((typeMembers[num], count));
                        }
                    }
                }
            }
            instance.Free();
        }

        internal IEnumerable<AssemblySymbol> GetReferencedAssembliesUsedSoFar()
        {
            ImmutableArray<AssemblySymbol>.Enumerator enumerator = SourceModule.GetReferencedAssemblySymbols().GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssemblySymbol current = enumerator.Current;
                if (!current.IsLinked && !current.IsMissing && AssemblyOrModuleSymbolToModuleRefMap.ContainsKey(current))
                {
                    yield return current;
                }
            }
        }

        private NamedTypeSymbol GetUntranslatedSpecialType(SpecialType specialType, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol specialType2 = SourceModule.ContainingAssembly.GetSpecialType(specialType);
            DiagnosticInfo diagnosticInfo = specialType2.GetUseSiteInfo().DiagnosticInfo;
            if (diagnosticInfo != null)
            {
                Symbol.ReportUseSiteDiagnostic(diagnosticInfo, diagnostics, (syntaxNodeOpt != null) ? syntaxNodeOpt.Location : NoLocation.Singleton);
            }
            return specialType2;
        }

        public sealed override INamedTypeReference GetSpecialType(SpecialType specialType, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return Translate(GetUntranslatedSpecialType(specialType, syntaxNodeOpt, diagnostics), syntaxNodeOpt, diagnostics, fromImplements: false, needDeclaration: true);
        }

        public sealed override IMethodReference GetInitArrayHelper()
        {
            return ((MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle))?.GetCciAdapter();
        }

        public sealed override bool IsPlatformType(ITypeReference typeRef, PlatformType platformType)
        {
            if (typeRef.GetInternalSymbol() is NamedTypeSymbol namedTypeSymbol)
            {
                if (platformType == PlatformType.SystemType)
                {
                    return (object)namedTypeSymbol == Compilation.GetWellKnownType(WellKnownType.System_Type);
                }
                return (int)namedTypeSymbol.SpecialType == (sbyte)platformType;
            }
            return false;
        }

        protected sealed override IAssemblyReference GetCorLibraryReferenceToEmit(EmitContext context)
        {
            AssemblySymbol corLibrary = CorLibrary;
            if (!corLibrary.IsMissing && !corLibrary.IsLinked && (object)corLibrary != SourceModule.ContainingAssembly)
            {
                return Translate(corLibrary, context.Diagnostics);
            }
            return null;
        }

        public override IAssemblyReference Translate(AssemblySymbol assembly, DiagnosticBag diagnostics)
        {
            if ((object)SourceModule.ContainingAssembly == assembly)
            {
                return (IAssemblyReference)this;
            }
            if (AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(assembly, out var value))
            {
                return (IAssemblyReference)value;
            }
            AssemblyReference assemblyReference = new AssemblyReference(assembly);
            AssemblyReference assemblyReference2 = (AssemblyReference)AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(assembly, assemblyReference);
            if (assemblyReference2 == assemblyReference)
            {
                ValidateReferencedAssembly(assembly, assemblyReference2, diagnostics);
            }
            AssemblyOrModuleSymbolToModuleRefMap.TryAdd(assembly.Modules[0], assemblyReference2);
            return assemblyReference2;
        }

        internal IModuleReference Translate(ModuleSymbol module, DiagnosticBag diagnostics)
        {
            if ((object)SourceModule == module)
            {
                return this;
            }
            if ((object)module == null)
            {
                return null;
            }
            if (AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(module, out var value))
            {
                return value;
            }
            value = TranslateModule(module, diagnostics);
            return AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(module, value);
        }

        protected virtual IModuleReference TranslateModule(ModuleSymbol module, DiagnosticBag diagnostics)
        {
            AssemblySymbol containingAssembly = module.ContainingAssembly;
            if ((object)containingAssembly != null && (object)containingAssembly.Modules[0] == module)
            {
                IModuleReference moduleReference = new AssemblyReference(containingAssembly);
                IModuleReference orAdd = AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(containingAssembly, moduleReference);
                if (orAdd == moduleReference)
                {
                    ValidateReferencedAssembly(containingAssembly, (AssemblyReference)moduleReference, diagnostics);
                }
                else
                {
                    moduleReference = orAdd;
                }
                return moduleReference;
            }
            return new ModuleReference(this, module);
        }

        internal INamedTypeReference Translate(NamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool fromImplements = false, bool needDeclaration = false)
        {
            if (namedTypeSymbol.IsAnonymousType)
            {
                namedTypeSymbol = AnonymousTypeManager.TranslateAnonymousTypeSymbol(namedTypeSymbol);
            }
            else if (namedTypeSymbol.IsTupleType)
            {
                CheckTupleUnderlyingType(namedTypeSymbol, syntaxNodeOpt, diagnostics);
            }
            if (namedTypeSymbol.OriginalDefinition.Kind == SymbolKind.ErrorType)
            {
                ErrorTypeSymbol errorTypeSymbol = (ErrorTypeSymbol)namedTypeSymbol.OriginalDefinition;
                DiagnosticInfo diagnosticInfo = errorTypeSymbol.GetUseSiteInfo().DiagnosticInfo ?? errorTypeSymbol.ErrorInfo;
                if (diagnosticInfo == null && namedTypeSymbol.Kind == SymbolKind.ErrorType)
                {
                    errorTypeSymbol = (ErrorTypeSymbol)namedTypeSymbol;
                    diagnosticInfo = errorTypeSymbol.GetUseSiteInfo().DiagnosticInfo ?? errorTypeSymbol.ErrorInfo;
                }
                if (_reportedErrorTypesMap.Add(errorTypeSymbol))
                {
                    diagnostics.Add(new CSDiagnostic(diagnosticInfo ?? new CSDiagnosticInfo(ErrorCode.ERR_BogusType, string.Empty), (syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.Location));
                }
                return ErrorType.Singleton;
            }
            if (!namedTypeSymbol.IsDefinition)
            {
                if (!namedTypeSymbol.IsUnboundGenericType)
                {
                    return (INamedTypeReference)GetCciAdapter(namedTypeSymbol);
                }
                namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
            }
            else if (!needDeclaration)
            {
                NamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
                object value;
                if (namedTypeSymbol.Arity > 0)
                {
                    if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out value))
                    {
                        return (INamedTypeReference)value;
                    }
                    INamedTypeReference value2 = (((object)containingType == null) ? new GenericNamespaceTypeInstanceReference(namedTypeSymbol) : ((!IsGenericType(containingType)) ? new GenericNestedTypeInstanceReference(namedTypeSymbol) : ((INamedTypeReference)new SpecializedGenericNestedTypeInstanceReference(namedTypeSymbol))));
                    return (INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, value2);
                }
                if (IsGenericType(containingType))
                {
                    if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out value))
                    {
                        return (INamedTypeReference)value;
                    }
                    INamedTypeReference value2 = new SpecializedNestedTypeReference(namedTypeSymbol);
                    return (INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, value2);
                }
                NamedTypeSymbol nativeIntegerUnderlyingType = namedTypeSymbol.NativeIntegerUnderlyingType;
                if ((object)nativeIntegerUnderlyingType != null)
                {
                    namedTypeSymbol = nativeIntegerUnderlyingType;
                }
            }
            return _embeddedTypesManagerOpt?.EmbedTypeIfNeedTo(namedTypeSymbol, fromImplements, syntaxNodeOpt, diagnostics) ?? namedTypeSymbol.GetCciAdapter();
        }

        private object GetCciAdapter(Symbol symbol)
        {
            return _genericInstanceMap.GetOrAdd(symbol, (Symbol s) => s.GetCciAdapter());
        }

        private void CheckTupleUnderlyingType(NamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
            if (((object)baseTypeNoUseSiteDiagnostics != null && baseTypeNoUseSiteDiagnostics.SpecialType == SpecialType.System_ValueType) || !_reportedErrorTypesMap.Add(namedTypeSymbol))
            {
                return;
            }
            Location location = ((syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.Location);
            if ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                DiagnosticInfo diagnosticInfo = baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo;
                if (diagnosticInfo != null && diagnosticInfo.Severity == DiagnosticSeverity.Error)
                {
                    diagnostics.Add(diagnosticInfo, location);
                    return;
                }
            }
            diagnostics.Add(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_PredefinedValueTupleTypeMustBeStruct, namedTypeSymbol.MetadataName), location));
        }

        public static bool IsGenericType(NamedTypeSymbol toCheck)
        {
            while ((object)toCheck != null)
            {
                if (toCheck.Arity > 0)
                {
                    return true;
                }
                toCheck = toCheck.ContainingType;
            }
            return false;
        }

        internal static IGenericParameterReference Translate(TypeParameterSymbol param)
        {
            if (!param.IsDefinition)
            {
                throw new InvalidOperationException(string.Format(CSharpResources.GenericParameterDefinition, param.Name));
            }
            return param.GetCciAdapter();
        }

        public override ITypeReference Translate(TypeSymbol typeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.DynamicType:
                    return Translate((DynamicTypeSymbol)typeSymbol, syntaxNodeOpt, diagnostics);
                case SymbolKind.ArrayType:
                    return Translate((ArrayTypeSymbol)typeSymbol);
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    return Translate((NamedTypeSymbol)typeSymbol, syntaxNodeOpt, diagnostics);
                case SymbolKind.PointerType:
                    return Translate((PointerTypeSymbol)typeSymbol);
                case SymbolKind.TypeParameter:
                    return Translate((TypeParameterSymbol)typeSymbol);
                case SymbolKind.FunctionPointerType:
                    return Translate((FunctionPointerTypeSymbol)typeSymbol);
                default:
                    throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind);
            }
        }

        internal IFieldReference Translate(FieldSymbol fieldSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool needDeclaration = false)
        {
            if (!fieldSymbol.IsDefinition)
            {
                return (IFieldReference)GetCciAdapter(fieldSymbol);
            }
            if (needDeclaration || !IsGenericType(fieldSymbol.ContainingType))
            {
                return _embeddedTypesManagerOpt?.EmbedFieldIfNeedTo(fieldSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics) ?? fieldSymbol.GetCciAdapter();
            }
            if (_genericInstanceMap.TryGetValue(fieldSymbol, out var value))
            {
                return (IFieldReference)value;
            }
            IFieldReference value2 = new SpecializedFieldReference(fieldSymbol);
            return (IFieldReference)_genericInstanceMap.GetOrAdd(fieldSymbol, value2);
        }

        public static TypeMemberVisibility MemberVisibility(Symbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    return TypeMemberVisibility.Public;
                case Accessibility.Private:
                    {
                        NamedTypeSymbol containingType = symbol.ContainingType;
                        if ((object)containingType != null && containingType.TypeKind == TypeKind.Submission)
                        {
                            return TypeMemberVisibility.Public;
                        }
                        return TypeMemberVisibility.Private;
                    }
                case Accessibility.Internal:
                    if (symbol.ContainingAssembly.IsInteractive)
                    {
                        return TypeMemberVisibility.Public;
                    }
                    return TypeMemberVisibility.Assembly;
                case Accessibility.Protected:
                    if (symbol.ContainingType.TypeKind == TypeKind.Submission)
                    {
                        return TypeMemberVisibility.Public;
                    }
                    return TypeMemberVisibility.Family;
                case Accessibility.ProtectedAndInternal:
                    return TypeMemberVisibility.FamilyAndAssembly;
                case Accessibility.ProtectedOrInternal:
                    if (symbol.ContainingAssembly.IsInteractive)
                    {
                        return TypeMemberVisibility.Public;
                    }
                    return TypeMemberVisibility.FamilyOrAssembly;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
            }
        }

        public override IMethodReference Translate(MethodSymbol symbol, DiagnosticBag diagnostics, bool needDeclaration)
        {
            return Translate(symbol, null, diagnostics, null, needDeclaration);
        }

        internal IMethodReference Translate(MethodSymbol methodSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, BoundArgListOperator optArgList = null, bool needDeclaration = false)
        {
            IMethodReference methodReference = Translate(methodSymbol, syntaxNodeOpt, diagnostics, needDeclaration);
            if (optArgList != null && optArgList.Arguments.Length > 0)
            {
                IParameterTypeInformation[] array = new IParameterTypeInformation[optArgList.Arguments.Length];
                int num = methodSymbol.ParameterCount;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new ArgListParameterTypeInformation(num, !optArgList.ArgumentRefKindsOpt.IsDefaultOrEmpty && optArgList.ArgumentRefKindsOpt[i] != RefKind.None, Translate(optArgList.Arguments[i].Type, syntaxNodeOpt, diagnostics));
                    num++;
                }
                return new ExpandedVarargsMethodReference(methodReference, array.AsImmutableOrNull());
            }
            return methodReference;
        }

        private IMethodReference Translate(MethodSymbol methodSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool needDeclaration)
        {
            NamedTypeSymbol containingType = methodSymbol.ContainingType;
            if (containingType.IsAnonymousType)
            {
                methodSymbol = AnonymousTypeManager.TranslateAnonymousTypeMethodSymbol(methodSymbol);
            }
            if (!methodSymbol.IsDefinition)
            {
                return (IMethodReference)GetCciAdapter(methodSymbol);
            }
            if (!needDeclaration)
            {
                bool isGenericMethod = methodSymbol.IsGenericMethod;
                bool flag = IsGenericType(containingType);
                if (isGenericMethod || flag)
                {
                    if (_genericInstanceMap.TryGetValue(methodSymbol, out var value))
                    {
                        return (IMethodReference)value;
                    }
                    IMethodReference value2 = ((!isGenericMethod) ? new SpecializedMethodReference(methodSymbol) : ((!flag) ? new GenericMethodInstanceReference(methodSymbol) : ((IMethodReference)new SpecializedGenericMethodInstanceReference(methodSymbol))));
                    return (IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, value2);
                }
                if (methodSymbol is NativeIntegerMethodSymbol nativeIntegerMethodSymbol)
                {
                    MethodSymbol underlyingMethod = nativeIntegerMethodSymbol.UnderlyingMethod;
                    if ((object)underlyingMethod != null)
                    {
                        methodSymbol = underlyingMethod;
                    }
                }
            }
            if (_embeddedTypesManagerOpt != null)
            {
                return _embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics);
            }
            return methodSymbol.GetCciAdapter();
        }

        internal IMethodReference TranslateOverriddenMethodReference(MethodSymbol methodSymbol, CSharpSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            if (IsGenericType(methodSymbol.ContainingType))
            {
                if (methodSymbol.IsDefinition)
                {
                    if (_genericInstanceMap.TryGetValue(methodSymbol, out var value))
                    {
                        return (IMethodReference)value;
                    }
                    IMethodReference value2 = new SpecializedMethodReference(methodSymbol);
                    return (IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, value2);
                }
                return new SpecializedMethodReference(methodSymbol);
            }
            if (_embeddedTypesManagerOpt != null)
            {
                return _embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics);
            }
            return methodSymbol.GetCciAdapter();
        }

        internal ImmutableArray<IParameterTypeInformation> Translate(ImmutableArray<ParameterSymbol> @params)
        {
            if (!@params.Any() || !MustBeWrapped(@params.First()))
            {
                return StaticCast<IParameterTypeInformation>.From(@params);
            }
            return TranslateAll(@params);
        }

        private static bool MustBeWrapped(ParameterSymbol param)
        {
            if (param.IsDefinition && ContainerIsGeneric(param.ContainingSymbol))
            {
                return true;
            }
            return false;
        }

        private ImmutableArray<IParameterTypeInformation> TranslateAll(ImmutableArray<ParameterSymbol> @params)
        {
            ArrayBuilder<IParameterTypeInformation> instance = ArrayBuilder<IParameterTypeInformation>.GetInstance();
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = @params.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                instance.Add(CreateParameterTypeInformationWrapper(current));
            }
            return instance.ToImmutableAndFree();
        }

        private IParameterTypeInformation CreateParameterTypeInformationWrapper(ParameterSymbol param)
        {
            if (_genericInstanceMap.TryGetValue(param, out var value))
            {
                return (IParameterTypeInformation)value;
            }
            IParameterTypeInformation value2 = new ParameterTypeInformation(param);
            return (IParameterTypeInformation)_genericInstanceMap.GetOrAdd(param, value2);
        }

        private static bool ContainerIsGeneric(Symbol container)
        {
            if (container.Kind != SymbolKind.Method || !((MethodSymbol)container).IsGenericMethod)
            {
                return IsGenericType(container.ContainingType);
            }
            return true;
        }

        internal ITypeReference Translate(DynamicTypeSymbol symbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return GetSpecialType(SpecialType.System_Object, syntaxNodeOpt, diagnostics);
        }

        internal IArrayTypeReference Translate(ArrayTypeSymbol symbol)
        {
            return (IArrayTypeReference)GetCciAdapter(symbol);
        }

        internal IPointerTypeReference Translate(PointerTypeSymbol symbol)
        {
            return (IPointerTypeReference)GetCciAdapter(symbol);
        }

        internal IFunctionPointerTypeReference Translate(FunctionPointerTypeSymbol symbol)
        {
            return (IFunctionPointerTypeReference)GetCciAdapter(symbol);
        }

        public NamedTypeSymbol SetFixedImplementationType(SourceMemberFieldSymbol field)
        {
            if (_fixedImplementationTypes == null)
            {
                Interlocked.CompareExchange(ref _fixedImplementationTypes, new Dictionary<FieldSymbol, NamedTypeSymbol>(), null);
            }
            lock (_fixedImplementationTypes)
            {
                if (_fixedImplementationTypes.TryGetValue(field, out var value))
                {
                    return value;
                }
                value = new FixedFieldImplementationType(field);
                _fixedImplementationTypes.Add(field, value);
                AddSynthesizedDefinition(value.ContainingType, value.GetCciAdapter());
                return value;
            }
        }

        internal NamedTypeSymbol GetFixedImplementationType(FieldSymbol field)
        {
            _fixedImplementationTypes.TryGetValue(field, out var value);
            return value;
        }

        protected override IMethodDefinition CreatePrivateImplementationDetailsStaticConstructor(PrivateImplementationDetails details, SyntaxNode syntaxOpt, DiagnosticBag diagnostics)
        {
            return new SynthesizedPrivateImplementationDetailsStaticConstructor(SourceModule, details, GetUntranslatedSpecialType(SpecialType.System_Void, syntaxOpt, diagnostics)).GetCciAdapter();
        }

        internal abstract SynthesizedAttributeData SynthesizeEmbeddedAttribute();

        internal SynthesizedAttributeData SynthesizeIsReadOnlyAttribute(Symbol symbol)
        {
            if ((object)Compilation.SourceModule != symbol.ContainingModule)
            {
                return null;
            }
            return TrySynthesizeIsReadOnlyAttribute();
        }

        internal SynthesizedAttributeData SynthesizeIsUnmanagedAttribute(Symbol symbol)
        {
            if ((object)Compilation.SourceModule != symbol.ContainingModule)
            {
                return null;
            }
            return TrySynthesizeIsUnmanagedAttribute();
        }

        internal SynthesizedAttributeData SynthesizeIsByRefLikeAttribute(Symbol symbol)
        {
            if ((object)Compilation.SourceModule != symbol.ContainingModule)
            {
                return null;
            }
            return TrySynthesizeIsByRefLikeAttribute();
        }

        internal SynthesizedAttributeData SynthesizeNullableAttributeIfNecessary(Symbol symbol, byte? nullableContextValue, TypeWithAnnotations type)
        {
            if ((object)Compilation.SourceModule != symbol.ContainingModule)
            {
                return null;
            }
            ArrayBuilder<byte> instance = ArrayBuilder<byte>.GetInstance();
            type.AddNullableTransforms(instance);
            SynthesizedAttributeData result;
            if (!instance.Any())
            {
                result = null;
            }
            else
            {
                byte? commonValue = MostCommonNullableValueBuilder.GetCommonValue(instance);
                if (commonValue.HasValue)
                {
                    result = SynthesizeNullableAttributeIfNecessary(nullableContextValue, commonValue.GetValueOrDefault());
                }
                else
                {
                    NamedTypeSymbol specialType = Compilation.GetSpecialType(SpecialType.System_Byte);
                    ArrayTypeSymbol type2 = ArrayTypeSymbol.CreateSZArray(specialType.ContainingAssembly, TypeWithAnnotations.Create(specialType));
                    ImmutableArray<TypedConstant> array = instance.SelectAsArray((byte flag, NamedTypeSymbol byteType) => new TypedConstant(byteType, TypedConstantKind.Primitive, flag), specialType);
                    result = SynthesizeNullableAttribute(WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags, ImmutableArray.Create(new TypedConstant(type2, array)));
                }
            }
            instance.Free();
            return result;
        }

        internal SynthesizedAttributeData SynthesizeNullableAttributeIfNecessary(byte? nullableContextValue, byte nullableValue)
        {
            if (nullableValue == nullableContextValue || (!nullableContextValue.HasValue && nullableValue == 0))
            {
                return null;
            }
            NamedTypeSymbol specialType = Compilation.GetSpecialType(SpecialType.System_Byte);
            return SynthesizeNullableAttribute(WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorByte, ImmutableArray.Create(new TypedConstant(specialType, TypedConstantKind.Primitive, nullableValue)));
        }

        internal virtual SynthesizedAttributeData SynthesizeNullableAttribute(WellKnownMember member, ImmutableArray<TypedConstant> arguments)
        {
            return Compilation.TrySynthesizeAttribute(member, arguments, default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true);
        }

        internal SynthesizedAttributeData SynthesizeNullableContextAttribute(Symbol symbol, byte value)
        {
            ModuleSymbol sourceModule = Compilation.SourceModule;
            if ((object)sourceModule != symbol && (object)sourceModule != symbol.ContainingModule)
            {
                return null;
            }
            return SynthesizeNullableContextAttribute(ImmutableArray.Create(new TypedConstant(Compilation.GetSpecialType(SpecialType.System_Byte), TypedConstantKind.Primitive, value)));
        }

        internal virtual SynthesizedAttributeData SynthesizeNullableContextAttribute(ImmutableArray<TypedConstant> arguments)
        {
            return Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_NullableContextAttribute__ctor, arguments, default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true);
        }

        internal SynthesizedAttributeData SynthesizePreserveBaseOverridesAttribute()
        {
            return Compilation.TrySynthesizeAttribute(SpecialMember.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute__ctor, isOptionalUse: true);
        }

        internal SynthesizedAttributeData SynthesizeNativeIntegerAttribute(Symbol symbol, TypeSymbol type)
        {
            if ((object)Compilation.SourceModule != symbol.ContainingModule)
            {
                return null;
            }
            ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance();
            CSharpCompilation.NativeIntegerTransformsEncoder.Encode(instance, type);
            SynthesizedAttributeData result;
            if (instance.Count == 1 && instance[0])
            {
                result = SynthesizeNativeIntegerAttribute(WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctor, ImmutableArray<TypedConstant>.Empty);
            }
            else
            {
                NamedTypeSymbol specialType = Compilation.GetSpecialType(SpecialType.System_Boolean);
                ImmutableArray<TypedConstant> array = instance.SelectAsArray((bool flag, NamedTypeSymbol constantType) => new TypedConstant(constantType, TypedConstantKind.Primitive, flag), specialType);
                ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(ArrayTypeSymbol.CreateSZArray(specialType.ContainingAssembly, TypeWithAnnotations.Create(specialType)), array));
                result = SynthesizeNativeIntegerAttribute(WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctorTransformFlags, arguments);
            }
            instance.Free();
            return result;
        }

        internal virtual SynthesizedAttributeData SynthesizeNativeIntegerAttribute(WellKnownMember member, ImmutableArray<TypedConstant> arguments)
        {
            return Compilation.TrySynthesizeAttribute(member, arguments, default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true);
        }

        internal bool ShouldEmitNullablePublicOnlyAttribute()
        {
            if (Compilation.GetUsesNullableAttributes())
            {
                return Compilation.EmitNullablePublicOnly;
            }
            return false;
        }

        internal virtual SynthesizedAttributeData SynthesizeNullablePublicOnlyAttribute(ImmutableArray<TypedConstant> arguments)
        {
            return Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_NullablePublicOnlyAttribute__ctor, arguments);
        }

        protected virtual SynthesizedAttributeData TrySynthesizeIsReadOnlyAttribute()
        {
            return Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor);
        }

        protected virtual SynthesizedAttributeData TrySynthesizeIsUnmanagedAttribute()
        {
            return Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor);
        }

        protected virtual SynthesizedAttributeData TrySynthesizeIsByRefLikeAttribute()
        {
            return Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor);
        }

        private void EnsureEmbeddableAttributeExists(EmbeddableAttributes attribute)
        {
            if ((GetNeedsGeneratedAttributesInternal() & attribute) == 0 && Compilation.CheckIfAttributeShouldBeEmbedded(attribute, null, null))
            {
                SetNeedsGeneratedAttributes(attribute);
            }
        }

        internal void EnsureIsReadOnlyAttributeExists()
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.IsReadOnlyAttribute);
        }

        internal void EnsureIsUnmanagedAttributeExists()
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.IsUnmanagedAttribute);
        }

        internal void EnsureNullableAttributeExists()
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.NullableAttribute);
        }

        internal void EnsureNativeIntegerAttributeExists()
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.NativeIntegerAttribute);
        }

        public override IEnumerable<INamespaceTypeDefinition> GetAdditionalTopLevelTypeDefinitions(EmitContext context)
        {
            return GetAdditionalTopLevelTypes();
        }

        public override IEnumerable<INamespaceTypeDefinition> GetEmbeddedTypeDefinitions(EmitContext context)
        {
            return GetEmbeddedTypes(context.Diagnostics);
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetEmbeddedTypes(DiagnosticBag diagnostics)
        {
            return GetEmbeddedTypes(new BindingDiagnosticBag(diagnostics));
        }

        internal virtual ImmutableArray<NamedTypeSymbol> GetEmbeddedTypes(BindingDiagnosticBag diagnostics)
        {
            return base.GetEmbeddedTypes(diagnostics.DiagnosticBag);
        }
    }
}
