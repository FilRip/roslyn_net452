using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class PEModuleBuilder<TCompilation, TSourceModuleSymbol, TAssemblySymbol, TTypeSymbol, TNamedTypeSymbol, TMethodSymbol, TSyntaxNode, TEmbeddedTypesManager, TModuleCompilationState> : CommonPEModuleBuilder, ITokenDeferral where TCompilation : Compilation where TSourceModuleSymbol : class, IModuleSymbolInternal where TAssemblySymbol : class, IAssemblySymbolInternal where TTypeSymbol : class, ITypeSymbolInternal where TNamedTypeSymbol : class, TTypeSymbol, INamedTypeSymbolInternal where TMethodSymbol : class, IMethodSymbolInternal where TSyntaxNode : SyntaxNode where TEmbeddedTypesManager : CommonEmbeddedTypesManager where TModuleCompilationState : ModuleCompilationState<TNamedTypeSymbol, TMethodSymbol>
    {
        private sealed class SynthesizedDefinitions
        {
            public ConcurrentQueue<INestedTypeDefinition> NestedTypes;

            public ConcurrentQueue<IMethodDefinition> Methods;

            public ConcurrentQueue<IPropertyDefinition> Properties;

            public ConcurrentQueue<IFieldDefinition> Fields;

            public ImmutableArray<ISymbolInternal> GetAllMembers()
            {
                ArrayBuilder<ISymbolInternal> instance = ArrayBuilder<ISymbolInternal>.GetInstance();
                if (Fields != null)
                {
                    foreach (IFieldDefinition field in Fields)
                    {
                        instance.Add(field.GetInternalSymbol());
                    }
                }
                if (Methods != null)
                {
                    foreach (IMethodDefinition method in Methods)
                    {
                        instance.Add(method.GetInternalSymbol());
                    }
                }
                if (Properties != null)
                {
                    foreach (IPropertyDefinition property in Properties)
                    {
                        instance.Add(property.GetInternalSymbol());
                    }
                }
                if (NestedTypes != null)
                {
                    foreach (INestedTypeDefinition nestedType in NestedTypes)
                    {
                        instance.Add(nestedType.GetInternalSymbol());
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        public readonly TSourceModuleSymbol SourceModule;

        public readonly TCompilation Compilation;

        private PrivateImplementationDetails _privateImplementationDetails;

        private ArrayMethods _lazyArrayMethods;

        private HashSet<string> _namesOfTopLevelTypes;

        public readonly TModuleCompilationState CompilationState;

        private readonly ConcurrentDictionary<TNamedTypeSymbol, SynthesizedDefinitions> _synthesizedTypeMembers = new ConcurrentDictionary<TNamedTypeSymbol, SynthesizedDefinitions>(ReferenceEqualityComparer.Instance);

        private ConcurrentDictionary<INamespaceSymbolInternal, ConcurrentQueue<INamespaceOrTypeSymbolInternal>> _lazySynthesizedNamespaceMembers;

        public RootModuleType RootModuleType { get; } = new RootModuleType();


        public abstract TEmbeddedTypesManager EmbeddedTypesManagerOpt { get; }

        internal override IAssemblySymbolInternal CommonCorLibrary => CorLibrary;

        public abstract TAssemblySymbol CorLibrary { get; }

        protected bool HaveDeterminedTopLevelTypes => _namesOfTopLevelTypes != null;

        internal sealed override IModuleSymbolInternal CommonSourceModule => SourceModule;

        public sealed override Compilation CommonCompilation => Compilation;

        internal sealed override CommonModuleCompilationState CommonModuleCompilationState => CompilationState;

        internal sealed override CommonEmbeddedTypesManager CommonEmbeddedTypesManagerOpt => EmbeddedTypesManagerOpt;

        public ArrayMethods ArrayMethods
        {
            get
            {
                ArrayMethods arrayMethods = _lazyArrayMethods;
                if (arrayMethods == null)
                {
                    arrayMethods = new ArrayMethods();
                    if (Interlocked.CompareExchange(ref _lazyArrayMethods, arrayMethods, null) != null)
                    {
                        arrayMethods = _lazyArrayMethods;
                    }
                }
                return arrayMethods;
            }
        }

        public PrivateImplementationDetails PrivateImplClass => _privateImplementationDetails;

        public override bool SupportsPrivateImplClass => true;

        protected PEModuleBuilder(TCompilation compilation, TSourceModuleSymbol sourceModule, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources, OutputKind outputKind, EmitOptions emitOptions, TModuleCompilationState compilationState)
            : base(manifestResources, emitOptions, outputKind, serializationProperties, compilation)
        {
            Compilation = compilation;
            SourceModule = sourceModule;
            CompilationState = compilationState;
        }

        internal sealed override void CompilationFinished()
        {
            CompilationState.Freeze();
        }

        public abstract INamedTypeReference GetSpecialType(SpecialType specialType, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);

        internal sealed override ITypeReference EncTranslateType(ITypeSymbolInternal type, DiagnosticBag diagnostics)
        {
            return EncTranslateLocalVariableType((TTypeSymbol)type, diagnostics);
        }

        public virtual ITypeReference EncTranslateLocalVariableType(TTypeSymbol type, DiagnosticBag diagnostics)
        {
            return Translate(type, null, diagnostics);
        }

        protected bool ContainsTopLevelType(string fullEmittedName)
        {
            return _namesOfTopLevelTypes.Contains(fullEmittedName);
        }

        public override IEnumerable<INamespaceTypeDefinition> GetTopLevelTypeDefinitions(EmitContext context)
        {
            TypeReferenceIndexer typeReferenceIndexer = null;
            HashSet<string> names = ((_namesOfTopLevelTypes != null) ? null : new HashSet<string>());
            if (EmbeddedTypesManagerOpt != null && !EmbeddedTypesManagerOpt.IsFrozen)
            {
                typeReferenceIndexer = new TypeReferenceIndexer(context);
                Dispatch(typeReferenceIndexer);
            }
            AddTopLevelType(names, RootModuleType);
            VisitTopLevelType(typeReferenceIndexer, RootModuleType);
            yield return RootModuleType;
            foreach (INamespaceTypeDefinition anonymousTypeDefinition in GetAnonymousTypeDefinitions(context))
            {
                AddTopLevelType(names, anonymousTypeDefinition);
                VisitTopLevelType(typeReferenceIndexer, anonymousTypeDefinition);
                yield return anonymousTypeDefinition;
            }
            foreach (INamespaceTypeDefinition item in GetTopLevelTypeDefinitionsCore(context))
            {
                AddTopLevelType(names, item);
                VisitTopLevelType(typeReferenceIndexer, item);
                yield return item;
            }
            PrivateImplementationDetails privateImplClass = PrivateImplClass;
            if (privateImplClass != null)
            {
                AddTopLevelType(names, privateImplClass);
                VisitTopLevelType(typeReferenceIndexer, privateImplClass);
                yield return privateImplClass;
            }
            if (EmbeddedTypesManagerOpt != null)
            {
                ImmutableArray<INamespaceTypeDefinition>.Enumerator enumerator2 = EmbeddedTypesManagerOpt.GetTypes(context.Diagnostics, names).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    INamespaceTypeDefinition current3 = enumerator2.Current;
                    AddTopLevelType(names, current3);
                    yield return current3;
                }
            }
            if (names != null)
            {
                _namesOfTopLevelTypes = names;
            }
        }

        public virtual ImmutableArray<TNamedTypeSymbol> GetAdditionalTopLevelTypes()
        {
            return ImmutableArray<TNamedTypeSymbol>.Empty;
        }

        public virtual ImmutableArray<TNamedTypeSymbol> GetEmbeddedTypes(DiagnosticBag diagnostics)
        {
            return ImmutableArray<TNamedTypeSymbol>.Empty;
        }

        public abstract IAssemblyReference Translate(TAssemblySymbol symbol, DiagnosticBag diagnostics);

        public abstract ITypeReference Translate(TTypeSymbol symbol, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);

        public abstract IMethodReference Translate(TMethodSymbol symbol, DiagnosticBag diagnostics, bool needDeclaration);

        public override IAssemblyReference Translate(IAssemblySymbolInternal symbol, DiagnosticBag diagnostics)
        {
            return Translate((TAssemblySymbol)symbol, diagnostics);
        }

        public override ITypeReference Translate(ITypeSymbolInternal symbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return Translate((TTypeSymbol)symbol, (TSyntaxNode)syntaxNodeOpt, diagnostics);
        }

        public override IMethodReference Translate(IMethodSymbolInternal symbol, DiagnosticBag diagnostics, bool needDeclaration)
        {
            return Translate((TMethodSymbol)symbol, diagnostics, needDeclaration);
        }

        public MetadataConstant CreateConstant(TTypeSymbol type, object value, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return new MetadataConstant(Translate(type, syntaxNodeOpt, diagnostics), value);
        }

        private static void AddTopLevelType(HashSet<string> names, INamespaceTypeDefinition type)
        {
            names?.Add(MetadataHelpers.BuildQualifiedName(type.NamespaceName, MetadataWriter.GetMangledName(type)));
        }

        private static void VisitTopLevelType(TypeReferenceIndexer noPiaIndexer, INamespaceTypeDefinition type)
        {
            noPiaIndexer?.Visit((ITypeDefinition)type);
        }

        public IFieldReference GetModuleVersionId(ITypeReference mvidType, TSyntaxNode syntaxOpt, DiagnosticBag diagnostics)
        {
            PrivateImplementationDetails privateImplClass = GetPrivateImplClass(syntaxOpt, diagnostics);
            EnsurePrivateImplementationDetailsStaticConstructor(privateImplClass, syntaxOpt, diagnostics);
            return privateImplClass.GetModuleVersionId(mvidType);
        }

        public IFieldReference GetInstrumentationPayloadRoot(int analysisKind, ITypeReference payloadType, TSyntaxNode syntaxOpt, DiagnosticBag diagnostics)
        {
            PrivateImplementationDetails privateImplClass = GetPrivateImplClass(syntaxOpt, diagnostics);
            EnsurePrivateImplementationDetailsStaticConstructor(privateImplClass, syntaxOpt, diagnostics);
            return privateImplClass.GetOrAddInstrumentationPayloadRoot(analysisKind, payloadType);
        }

        private void EnsurePrivateImplementationDetailsStaticConstructor(PrivateImplementationDetails details, TSyntaxNode syntaxOpt, DiagnosticBag diagnostics)
        {
            if (details.GetMethod(".cctor") == null)
            {
                details.TryAddSynthesizedMethod(CreatePrivateImplementationDetailsStaticConstructor(details, syntaxOpt, diagnostics));
            }
        }

        protected abstract IMethodDefinition CreatePrivateImplementationDetailsStaticConstructor(PrivateImplementationDetails details, TSyntaxNode syntaxOpt, DiagnosticBag diagnostics);

        public abstract IEnumerable<INestedTypeDefinition> GetSynthesizedNestedTypes(TNamedTypeSymbol container);

        public IEnumerable<INestedTypeDefinition> GetSynthesizedTypes(TNamedTypeSymbol container)
        {
            IEnumerable<INestedTypeDefinition> synthesizedNestedTypes = GetSynthesizedNestedTypes(container);
            IEnumerable<INestedTypeDefinition> enumerable = null;
            if (_synthesizedTypeMembers.TryGetValue(container, out var value))
            {
                enumerable = value.NestedTypes;
            }
            if (synthesizedNestedTypes == null)
            {
                return enumerable;
            }
            if (enumerable == null)
            {
                return synthesizedNestedTypes;
            }
            return synthesizedNestedTypes.Concat(enumerable);
        }

        private SynthesizedDefinitions GetOrAddSynthesizedDefinitions(TNamedTypeSymbol container)
        {
            return _synthesizedTypeMembers.GetOrAdd(container, (TNamedTypeSymbol _) => new SynthesizedDefinitions());
        }

        public void AddSynthesizedDefinition(TNamedTypeSymbol container, IMethodDefinition method)
        {
            SynthesizedDefinitions orAddSynthesizedDefinitions = GetOrAddSynthesizedDefinitions(container);
            if (orAddSynthesizedDefinitions.Methods == null)
            {
                Interlocked.CompareExchange(ref orAddSynthesizedDefinitions.Methods, new ConcurrentQueue<IMethodDefinition>(), null);
            }
            orAddSynthesizedDefinitions.Methods.Enqueue(method);
        }

        public void AddSynthesizedDefinition(TNamedTypeSymbol container, IPropertyDefinition property)
        {
            SynthesizedDefinitions orAddSynthesizedDefinitions = GetOrAddSynthesizedDefinitions(container);
            if (orAddSynthesizedDefinitions.Properties == null)
            {
                Interlocked.CompareExchange(ref orAddSynthesizedDefinitions.Properties, new ConcurrentQueue<IPropertyDefinition>(), null);
            }
            orAddSynthesizedDefinitions.Properties.Enqueue(property);
        }

        public void AddSynthesizedDefinition(TNamedTypeSymbol container, IFieldDefinition field)
        {
            SynthesizedDefinitions orAddSynthesizedDefinitions = GetOrAddSynthesizedDefinitions(container);
            if (orAddSynthesizedDefinitions.Fields == null)
            {
                Interlocked.CompareExchange(ref orAddSynthesizedDefinitions.Fields, new ConcurrentQueue<IFieldDefinition>(), null);
            }
            orAddSynthesizedDefinitions.Fields.Enqueue(field);
        }

        public void AddSynthesizedDefinition(TNamedTypeSymbol container, INestedTypeDefinition nestedType)
        {
            SynthesizedDefinitions orAddSynthesizedDefinitions = GetOrAddSynthesizedDefinitions(container);
            if (orAddSynthesizedDefinitions.NestedTypes == null)
            {
                Interlocked.CompareExchange(ref orAddSynthesizedDefinitions.NestedTypes, new ConcurrentQueue<INestedTypeDefinition>(), null);
            }
            orAddSynthesizedDefinitions.NestedTypes.Enqueue(nestedType);
        }

        public void AddSynthesizedDefinition(INamespaceSymbolInternal container, INamespaceOrTypeSymbolInternal typeOrNamespace)
        {
            if (_lazySynthesizedNamespaceMembers == null)
            {
                Interlocked.CompareExchange(ref _lazySynthesizedNamespaceMembers, new ConcurrentDictionary<INamespaceSymbolInternal, ConcurrentQueue<INamespaceOrTypeSymbolInternal>>(), null);
            }
            _lazySynthesizedNamespaceMembers.GetOrAdd(container, (INamespaceSymbolInternal _) => new ConcurrentQueue<INamespaceOrTypeSymbolInternal>()).Enqueue(typeOrNamespace);
        }

        public IEnumerable<IFieldDefinition> GetSynthesizedFields(TNamedTypeSymbol container)
        {
            if (!_synthesizedTypeMembers.TryGetValue(container, out var value))
            {
                return null;
            }
            return value.Fields;
        }

        public IEnumerable<IPropertyDefinition> GetSynthesizedProperties(TNamedTypeSymbol container)
        {
            if (!_synthesizedTypeMembers.TryGetValue(container, out var value))
            {
                return null;
            }
            return value.Properties;
        }

        public IEnumerable<IMethodDefinition> GetSynthesizedMethods(TNamedTypeSymbol container)
        {
            if (!_synthesizedTypeMembers.TryGetValue(container, out var value))
            {
                return null;
            }
            return value.Methods;
        }

        public override ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> GetAllSynthesizedMembers()
        {
            ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>>.Builder builder = ImmutableDictionary.CreateBuilder<ISymbolInternal, ImmutableArray<ISymbolInternal>>();
            foreach (KeyValuePair<TNamedTypeSymbol, SynthesizedDefinitions> synthesizedTypeMember in _synthesizedTypeMembers)
            {
                builder.Add(synthesizedTypeMember.Key, synthesizedTypeMember.Value.GetAllMembers());
            }
            ConcurrentDictionary<INamespaceSymbolInternal, ConcurrentQueue<INamespaceOrTypeSymbolInternal>> lazySynthesizedNamespaceMembers = _lazySynthesizedNamespaceMembers;
            if (lazySynthesizedNamespaceMembers != null)
            {
                foreach (KeyValuePair<INamespaceSymbolInternal, ConcurrentQueue<INamespaceOrTypeSymbolInternal>> item in lazySynthesizedNamespaceMembers)
                {
                    builder.Add(item.Key, ((IEnumerable<ISymbolInternal>)item.Value).ToImmutableArray());
                }
            }
            return builder.ToImmutable();
        }

        IFieldReference ITokenDeferral.GetFieldForData(ImmutableArray<byte> data, SyntaxNode syntaxNode, DiagnosticBag diagnostics)
        {
            return GetPrivateImplClass((TSyntaxNode)syntaxNode, diagnostics).CreateDataField(data);
        }

        public abstract IMethodReference GetInitArrayHelper();

        public PrivateImplementationDetails GetPrivateImplClass(TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            PrivateImplementationDetails privateImplementationDetails = _privateImplementationDetails;
            if (privateImplementationDetails == null && SupportsPrivateImplClass)
            {
                privateImplementationDetails = new PrivateImplementationDetails(this, SourceModule.Name, Compilation.GetSubmissionSlotIndex(), GetSpecialType(SpecialType.System_Object, syntaxNodeOpt, diagnostics), GetSpecialType(SpecialType.System_ValueType, syntaxNodeOpt, diagnostics), GetSpecialType(SpecialType.System_Byte, syntaxNodeOpt, diagnostics), GetSpecialType(SpecialType.System_Int16, syntaxNodeOpt, diagnostics), GetSpecialType(SpecialType.System_Int32, syntaxNodeOpt, diagnostics), GetSpecialType(SpecialType.System_Int64, syntaxNodeOpt, diagnostics), SynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
                if (Interlocked.CompareExchange(ref _privateImplementationDetails, privateImplementationDetails, null) != null)
                {
                    privateImplementationDetails = _privateImplementationDetails;
                }
            }
            return privateImplementationDetails;
        }

        public sealed override ITypeReference GetPlatformType(PlatformType platformType, EmitContext context)
        {
            if (platformType == PlatformType.SystemType)
            {
                throw ExceptionUtilities.UnexpectedValue(platformType);
            }
            return GetSpecialType((SpecialType)platformType, (TSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }
    }
}
