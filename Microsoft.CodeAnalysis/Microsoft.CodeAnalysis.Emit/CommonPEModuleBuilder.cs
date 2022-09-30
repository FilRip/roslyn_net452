using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class CommonPEModuleBuilder : IUnit, IUnitReference, IReference, INamedEntity, IDefinition, IModuleReference
    {
        public readonly DebugDocumentsBuilder DebugDocumentsBuilder;

        public readonly IEnumerable<ResourceDescription> ManifestResources;

        internal readonly ModulePropertiesForSerialization SerializationProperties;

        public readonly OutputKind OutputKind;

        internal Stream RawWin32Resources;

        internal IEnumerable<IWin32Resource> Win32Resources;

        internal ResourceSection Win32ResourceSection;

        public Stream SourceLinkStreamOpt;

        internal IMethodReference PEEntryPoint;

        internal IMethodReference DebugEntryPoint;

        private readonly ConcurrentDictionary<IMethodSymbolInternal, IMethodBody> _methodBodyMap;

        private readonly TokenMap _referencesInILMap = new TokenMap();

        private readonly ItemTokenMap<string> _stringsInILMap = new ItemTokenMap<string>();

        private readonly ItemTokenMap<DebugSourceDocument> _sourceDocumentsInILMap = new ItemTokenMap<DebugSourceDocument>();

        private ImmutableArray<AssemblyReferenceAlias> _lazyAssemblyReferenceAliases;

        private ImmutableArray<ManagedResource> _lazyManagedResources;

        private IEnumerable<EmbeddedText> _embeddedTexts = SpecializedCollections.EmptyEnumerable<EmbeddedText>();

        internal ConcurrentDictionary<IMethodSymbolInternal, CompilationTestData.MethodData> TestData { get; private set; }

        internal EmitOptions EmitOptions { get; }

        internal DebugInformationFormat DebugInformationFormat => EmitOptions.DebugInformationFormat;

        internal SourceHashAlgorithm PdbChecksumAlgorithm => EmitOptions.PdbChecksumAlgorithm;

        public abstract int CurrentGenerationOrdinal { get; }

        public abstract string Name { get; }

        public abstract string ModuleName { get; }

        public abstract bool SupportsPrivateImplClass { get; }

        public abstract Compilation CommonCompilation { get; }

        internal abstract IModuleSymbolInternal CommonSourceModule { get; }

        internal abstract IAssemblySymbolInternal CommonCorLibrary { get; }

        internal abstract CommonModuleCompilationState CommonModuleCompilationState { get; }

        internal abstract CommonEmbeddedTypesManager CommonEmbeddedTypesManagerOpt { get; }

        public abstract bool GenerateVisualBasicStylePdb { get; }

        public abstract IEnumerable<string> LinkedAssembliesDebugInfo { get; }

        public abstract string DefaultNamespace { get; }

        public int DebugDocumentCount => DebugDocumentsBuilder.DebugDocumentCount;

        public abstract ISourceAssemblySymbolInternal SourceAssemblyOpt { get; }

        public int HintNumberOfMethodDefinitions => (int)(_methodBodyMap.Count * 1.5);

        public IEnumerable<EmbeddedText> EmbeddedTexts
        {
            get
            {
                return _embeddedTexts;
            }
            set
            {
                _embeddedTexts = value;
            }
        }

        public bool SaveTestData => TestData != null;

        public CommonPEModuleBuilder(IEnumerable<ResourceDescription> manifestResources, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, Compilation compilation)
        {
            ManifestResources = manifestResources;
            DebugDocumentsBuilder = new DebugDocumentsBuilder(compilation.Options.SourceReferenceResolver, compilation.IsCaseSensitive);
            OutputKind = outputKind;
            SerializationProperties = serializationProperties;
            _methodBodyMap = new ConcurrentDictionary<IMethodSymbolInternal, IMethodBody>(ReferenceEqualityComparer.Instance);
            EmitOptions = emitOptions;
        }

        public abstract IAssemblyReference Translate(IAssemblySymbolInternal symbol, DiagnosticBag diagnostics);

        public abstract ITypeReference Translate(ITypeSymbolInternal symbol, SyntaxNode syntaxOpt, DiagnosticBag diagnostics);

        public abstract IMethodReference Translate(IMethodSymbolInternal symbol, DiagnosticBag diagnostics, bool needDeclaration);

        internal abstract void CompilationFinished();

        public abstract ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> GetAllSynthesizedMembers();

        internal abstract ITypeReference EncTranslateType(ITypeSymbolInternal type, DiagnosticBag diagnostics);

        public abstract IEnumerable<ICustomAttribute> GetSourceAssemblyAttributes(bool isRefAssembly);

        public abstract IEnumerable<SecurityAttribute> GetSourceAssemblySecurityAttributes();

        public abstract IEnumerable<ICustomAttribute> GetSourceModuleAttributes();

        public abstract ICustomAttribute SynthesizeAttribute(WellKnownMember attributeConstructor);

        public abstract ImmutableArray<ExportedType> GetExportedTypes(DiagnosticBag diagnostics);

        public abstract ImmutableArray<UsedNamespaceOrType> GetImports();

        protected abstract IAssemblyReference GetCorLibraryReferenceToEmit(EmitContext context);

        protected abstract IEnumerable<IAssemblyReference> GetAssemblyReferencesFromAddedModules(DiagnosticBag diagnostics);

        protected abstract void AddEmbeddedResourcesFromAddedModules(ArrayBuilder<ManagedResource> builder, DiagnosticBag diagnostics);

        public abstract ITypeReference GetPlatformType(PlatformType platformType, EmitContext context);

        public abstract bool IsPlatformType(ITypeReference typeRef, PlatformType platformType);

        public abstract IEnumerable<INamespaceTypeDefinition> GetTopLevelTypeDefinitions(EmitContext context);

        public IEnumerable<INamespaceTypeDefinition> GetTopLevelTypeDefinitionsCore(EmitContext context)
        {
            foreach (INamespaceTypeDefinition additionalTopLevelTypeDefinition in GetAdditionalTopLevelTypeDefinitions(context))
            {
                yield return additionalTopLevelTypeDefinition;
            }
            foreach (INamespaceTypeDefinition embeddedTypeDefinition in GetEmbeddedTypeDefinitions(context))
            {
                yield return embeddedTypeDefinition;
            }
            foreach (INamespaceTypeDefinition topLevelSourceTypeDefinition in GetTopLevelSourceTypeDefinitions(context))
            {
                yield return topLevelSourceTypeDefinition;
            }
        }

        public abstract IEnumerable<INamespaceTypeDefinition> GetAdditionalTopLevelTypeDefinitions(EmitContext context);

        public abstract IEnumerable<INamespaceTypeDefinition> GetAnonymousTypeDefinitions(EmitContext context);

        public abstract IEnumerable<INamespaceTypeDefinition> GetEmbeddedTypeDefinitions(EmitContext context);

        public abstract IEnumerable<INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context);

        public abstract IEnumerable<IFileReference> GetFiles(EmitContext context);

        public abstract MultiDictionary<DebugSourceDocument, DefinitionWithLocation> GetSymbolToLocationMap();

        public void Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return this;
        }

        ISymbolInternal IReference.GetInternalSymbol()
        {
            return null;
        }

        public IMethodBody GetMethodBody(IMethodSymbolInternal methodSymbol)
        {
            if (_methodBodyMap.TryGetValue(methodSymbol, out var value))
            {
                return value;
            }
            return null;
        }

        public void SetMethodBody(IMethodSymbolInternal methodSymbol, IMethodBody body)
        {
            _methodBodyMap.Add(methodSymbol, body);
        }

        public void SetPEEntryPoint(IMethodSymbolInternal method, DiagnosticBag diagnostics)
        {
            PEEntryPoint = Translate(method, diagnostics, needDeclaration: true);
        }

        public void SetDebugEntryPoint(IMethodSymbolInternal method, DiagnosticBag diagnostics)
        {
            DebugEntryPoint = Translate(method, diagnostics, needDeclaration: true);
        }

        private bool IsSourceDefinition(IMethodSymbolInternal method)
        {
            if (method.ContainingModule == CommonSourceModule)
            {
                return method.IsDefinition;
            }
            return false;
        }

        public IAssemblyReference GetCorLibrary(EmitContext context)
        {
            return Translate(CommonCorLibrary, context.Diagnostics);
        }

        public IAssemblyReference GetContainingAssembly(EmitContext context)
        {
            if (OutputKind != OutputKind.NetModule)
            {
                return (IAssemblyReference)this;
            }
            return null;
        }

        public IEnumerable<string> GetStrings()
        {
            return _stringsInILMap.GetAllItems();
        }

        public uint GetFakeSymbolTokenForIL(IReference symbol, SyntaxNode syntaxNode, DiagnosticBag diagnostics)
        {
            uint orAddTokenFor = _referencesInILMap.GetOrAddTokenFor(symbol, out bool referenceAdded);
            if (referenceAdded)
            {
                ReferenceDependencyWalker.VisitReference(symbol, new EmitContext(this, syntaxNode, diagnostics, metadataOnly: false, includePrivateMembers: true));
            }
            return orAddTokenFor;
        }

        public uint GetFakeSymbolTokenForIL(ISignature symbol, SyntaxNode syntaxNode, DiagnosticBag diagnostics)
        {
            uint orAddTokenFor = _referencesInILMap.GetOrAddTokenFor(symbol, out bool referenceAdded);
            if (referenceAdded)
            {
                ReferenceDependencyWalker.VisitSignature(symbol, new EmitContext(this, syntaxNode, diagnostics, metadataOnly: false, includePrivateMembers: true));
            }
            return orAddTokenFor;
        }

        public uint GetSourceDocumentIndexForIL(DebugSourceDocument document)
        {
            return _sourceDocumentsInILMap.GetOrAddTokenFor(document);
        }

        internal DebugSourceDocument GetSourceDocumentFromIndex(uint token)
        {
            return _sourceDocumentsInILMap.GetItem(token);
        }

        public object GetReferenceFromToken(uint token)
        {
            return _referencesInILMap.GetItem(token);
        }

        public uint GetFakeStringTokenForIL(string str)
        {
            return _stringsInILMap.GetOrAddTokenFor(str);
        }

        public string GetStringFromToken(uint token)
        {
            return _stringsInILMap.GetItem(token);
        }

        public ReadOnlySpan<object> ReferencesInIL()
        {
            return _referencesInILMap.GetAllItems();
        }

        public ImmutableArray<AssemblyReferenceAlias> GetAssemblyReferenceAliases(EmitContext context)
        {
            if (_lazyAssemblyReferenceAliases.IsDefault)
            {
                ImmutableInterlocked.InterlockedCompareExchange(ref _lazyAssemblyReferenceAliases, CalculateAssemblyReferenceAliases(context), default(ImmutableArray<AssemblyReferenceAlias>));
            }
            return _lazyAssemblyReferenceAliases;
        }

        private ImmutableArray<AssemblyReferenceAlias> CalculateAssemblyReferenceAliases(EmitContext context)
        {
            ArrayBuilder<AssemblyReferenceAlias> instance = ArrayBuilder<AssemblyReferenceAlias>.GetInstance();
            foreach (var referencedAssemblyAlias in CommonCompilation.GetBoundReferenceManager().GetReferencedAssemblyAliases())
            {
                IAssemblySymbolInternal item = referencedAssemblyAlias.AssemblySymbol;
                ImmutableArray<string> item2 = referencedAssemblyAlias.Aliases;
                for (int i = 0; i < item2.Length; i++)
                {
                    string text = item2[i];
                    if (text != MetadataReferenceProperties.GlobalAlias && item2.IndexOf(text, 0, i) < 0)
                    {
                        instance.Add(new AssemblyReferenceAlias(text, Translate(item, context.Diagnostics)));
                    }
                }
            }
            return instance.ToImmutableAndFree();
        }

        public IEnumerable<IAssemblyReference> GetAssemblyReferences(EmitContext context)
        {
            IAssemblyReference corLibraryReferenceToEmit = GetCorLibraryReferenceToEmit(context);
            if (corLibraryReferenceToEmit != null)
            {
                yield return corLibraryReferenceToEmit;
            }
            if (OutputKind == OutputKind.NetModule)
            {
                yield break;
            }
            foreach (IAssemblyReference assemblyReferencesFromAddedModule in GetAssemblyReferencesFromAddedModules(context.Diagnostics))
            {
                yield return assemblyReferencesFromAddedModule;
            }
        }

        public ImmutableArray<ManagedResource> GetResources(EmitContext context)
        {
            if (context.IsRefAssembly)
            {
                return ImmutableArray<ManagedResource>.Empty;
            }
            if (_lazyManagedResources.IsDefault)
            {
                ArrayBuilder<ManagedResource> instance = ArrayBuilder<ManagedResource>.GetInstance();
                foreach (ResourceDescription manifestResource in ManifestResources)
                {
                    instance.Add(manifestResource.ToManagedResource(this));
                }
                if (OutputKind != OutputKind.NetModule)
                {
                    AddEmbeddedResourcesFromAddedModules(instance, context.Diagnostics);
                }
                _lazyManagedResources = instance.ToImmutableAndFree();
            }
            return _lazyManagedResources;
        }

        public void SetMethodTestData(IMethodSymbolInternal method, ILBuilder builder)
        {
            TestData.Add(method, new CompilationTestData.MethodData(builder, method));
        }

        public void SetMethodTestData(ConcurrentDictionary<IMethodSymbolInternal, CompilationTestData.MethodData> methods)
        {
            TestData = methods;
        }
    }
}
