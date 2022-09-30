using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal abstract class PEAssemblyBuilderBase : PEModuleBuilder, IAssemblyReference, IModuleReference, IUnitReference, IReference, INamedEntity
    {
        private readonly SourceAssemblySymbol _sourceAssembly;

        private readonly ImmutableArray<NamedTypeSymbol> _additionalTypes;

        private ImmutableArray<IFileReference> _lazyFiles;

        private ImmutableArray<IFileReference> _lazyFilesWithoutManifestResources;

        private SynthesizedEmbeddedAttributeSymbol _lazyEmbeddedAttribute;

        private SynthesizedEmbeddedAttributeSymbol _lazyIsReadOnlyAttribute;

        private SynthesizedEmbeddedAttributeSymbol _lazyIsByRefLikeAttribute;

        private SynthesizedEmbeddedAttributeSymbol _lazyIsUnmanagedAttribute;

        private SynthesizedEmbeddedNullableAttributeSymbol _lazyNullableAttribute;

        private SynthesizedEmbeddedNullableContextAttributeSymbol _lazyNullableContextAttribute;

        private SynthesizedEmbeddedNullablePublicOnlyAttributeSymbol _lazyNullablePublicOnlyAttribute;

        private SynthesizedEmbeddedNativeIntegerAttributeSymbol _lazyNativeIntegerAttribute;

        private readonly string _metadataName;

        public sealed override ISourceAssemblySymbolInternal SourceAssemblyOpt => _sourceAssembly;

        public override string Name => _metadataName;

        public AssemblyIdentity Identity => _sourceAssembly.Identity;

        public Version AssemblyVersionPattern => _sourceAssembly.AssemblyVersionPattern;

        public PEAssemblyBuilderBase(SourceAssemblySymbol sourceAssembly, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources, ImmutableArray<NamedTypeSymbol> additionalTypes)
            : base((SourceModuleSymbol)sourceAssembly.Modules[0], emitOptions, outputKind, serializationProperties, manifestResources)
        {
            _sourceAssembly = sourceAssembly;
            _additionalTypes = additionalTypes.NullToEmpty();
            _metadataName = ((emitOptions.OutputNameOverride == null) ? sourceAssembly.MetadataName : FileNameUtilities.ChangeExtension(emitOptions.OutputNameOverride, null));
            AssemblyOrModuleSymbolToModuleRefMap.Add(sourceAssembly, this);
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetAdditionalTopLevelTypes()
        {
            return _additionalTypes;
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> GetEmbeddedTypes(BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            CreateEmbeddedAttributesIfNeeded(diagnostics);
            instance.AddIfNotNull(_lazyEmbeddedAttribute);
            instance.AddIfNotNull(_lazyIsReadOnlyAttribute);
            instance.AddIfNotNull(_lazyIsUnmanagedAttribute);
            instance.AddIfNotNull(_lazyIsByRefLikeAttribute);
            instance.AddIfNotNull(_lazyNullableAttribute);
            instance.AddIfNotNull(_lazyNullableContextAttribute);
            instance.AddIfNotNull(_lazyNullablePublicOnlyAttribute);
            instance.AddIfNotNull(_lazyNativeIntegerAttribute);
            return instance.ToImmutableAndFree();
        }

        public sealed override IEnumerable<IFileReference> GetFiles(EmitContext context)
        {
            if (!context.IsRefAssembly)
            {
                return getFiles(ref _lazyFiles);
            }
            return getFiles(ref _lazyFilesWithoutManifestResources);
            ImmutableArray<IFileReference> getFiles(ref ImmutableArray<IFileReference> lazyFiles)
            {
                if (lazyFiles.IsDefault)
                {
                    ArrayBuilder<IFileReference> instance = ArrayBuilder<IFileReference>.GetInstance();
                    try
                    {
                        ImmutableArray<ModuleSymbol> modules = _sourceAssembly.Modules;
                        for (int i = 1; i < modules.Length; i++)
                        {
                            instance.Add((IFileReference)Translate(modules[i], context.Diagnostics));
                        }
                        if (!context.IsRefAssembly)
                        {
                            foreach (ResourceDescription manifestResource in ManifestResources)
                            {
                                if (!manifestResource.IsEmbedded)
                                {
                                    instance.Add(manifestResource);
                                }
                            }
                        }
                        if (ImmutableInterlocked.InterlockedInitialize(ref lazyFiles, instance.ToImmutable()) && lazyFiles.Length > 0 && !CryptographicHashProvider.IsSupportedAlgorithm(_sourceAssembly.HashAlgorithm))
                        {
                            context.Diagnostics.Add(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_CryptoHashFailed), NoLocation.Singleton));
                        }
                    }
                    finally
                    {
                        instance.Free();
                    }
                }
                return lazyFiles;
            }
        }

        protected override void AddEmbeddedResourcesFromAddedModules(ArrayBuilder<ManagedResource> builder, DiagnosticBag diagnostics)
        {
            ImmutableArray<ModuleSymbol> modules = _sourceAssembly.Modules;
            int length = modules.Length;
            for (int i = 1; i < length; i++)
            {
                IFileReference fileReference = (IFileReference)Translate(modules[i], diagnostics);
                try
                {
                    ImmutableArray<EmbeddedResource>.Enumerator enumerator = ((PEModuleSymbol)modules[i]).Module.GetEmbeddedResourcesOrThrow().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        EmbeddedResource current = enumerator.Current;
                        builder.Add(new ManagedResource(current.Name, (current.Attributes & ManifestResourceAttributes.Public) != 0, null, fileReference, current.Offset));
                    }
                }
                catch (BadImageFormatException)
                {
                    diagnostics.Add(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, modules[i]), NoLocation.Singleton);
                }
            }
        }

        internal override SynthesizedAttributeData SynthesizeEmbeddedAttribute()
        {
            return new SynthesizedAttributeData(_lazyEmbeddedAttribute.Constructors[0], ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
        }

        internal override SynthesizedAttributeData SynthesizeNullableAttribute(WellKnownMember member, ImmutableArray<TypedConstant> arguments)
        {
            if ((object)_lazyNullableAttribute != null)
            {
                int index = ((member == WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags) ? 1 : 0);
                return new SynthesizedAttributeData(_lazyNullableAttribute.Constructors[index], arguments, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.SynthesizeNullableAttribute(member, arguments);
        }

        internal override SynthesizedAttributeData SynthesizeNullableContextAttribute(ImmutableArray<TypedConstant> arguments)
        {
            if ((object)_lazyNullableContextAttribute != null)
            {
                return new SynthesizedAttributeData(_lazyNullableContextAttribute.Constructors[0], arguments, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.SynthesizeNullableContextAttribute(arguments);
        }

        internal override SynthesizedAttributeData SynthesizeNullablePublicOnlyAttribute(ImmutableArray<TypedConstant> arguments)
        {
            if ((object)_lazyNullablePublicOnlyAttribute != null)
            {
                return new SynthesizedAttributeData(_lazyNullablePublicOnlyAttribute.Constructors[0], arguments, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.SynthesizeNullablePublicOnlyAttribute(arguments);
        }

        internal override SynthesizedAttributeData SynthesizeNativeIntegerAttribute(WellKnownMember member, ImmutableArray<TypedConstant> arguments)
        {
            if ((object)_lazyNativeIntegerAttribute != null)
            {
                int index = ((member == WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctorTransformFlags) ? 1 : 0);
                return new SynthesizedAttributeData(_lazyNativeIntegerAttribute.Constructors[index], arguments, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.SynthesizeNativeIntegerAttribute(member, arguments);
        }

        protected override SynthesizedAttributeData TrySynthesizeIsReadOnlyAttribute()
        {
            if ((object)_lazyIsReadOnlyAttribute != null)
            {
                return new SynthesizedAttributeData(_lazyIsReadOnlyAttribute.Constructors[0], ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.TrySynthesizeIsReadOnlyAttribute();
        }

        protected override SynthesizedAttributeData TrySynthesizeIsUnmanagedAttribute()
        {
            if ((object)_lazyIsUnmanagedAttribute != null)
            {
                return new SynthesizedAttributeData(_lazyIsUnmanagedAttribute.Constructors[0], ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.TrySynthesizeIsUnmanagedAttribute();
        }

        protected override SynthesizedAttributeData TrySynthesizeIsByRefLikeAttribute()
        {
            if ((object)_lazyIsByRefLikeAttribute != null)
            {
                return new SynthesizedAttributeData(_lazyIsByRefLikeAttribute.Constructors[0], ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return base.TrySynthesizeIsByRefLikeAttribute();
        }

        private void CreateEmbeddedAttributesIfNeeded(BindingDiagnosticBag diagnostics)
        {
            EmbeddableAttributes embeddableAttributes = GetNeedsGeneratedAttributes();
            if (ShouldEmitNullablePublicOnlyAttribute() && Compilation.CheckIfAttributeShouldBeEmbedded(EmbeddableAttributes.NullablePublicOnlyAttribute, diagnostics, Location.None))
            {
                embeddableAttributes |= EmbeddableAttributes.NullablePublicOnlyAttribute;
            }
            else if (embeddableAttributes == 0)
            {
                return;
            }
            Func<string, NamespaceSymbol, BindingDiagnosticBag, SynthesizedEmbeddedAttributeSymbol> factory = CreateParameterlessEmbeddedAttributeSymbol;
            CreateAttributeIfNeeded(ref _lazyEmbeddedAttribute, diagnostics, AttributeDescription.CodeAnalysisEmbeddedAttribute, factory);
            if ((embeddableAttributes & EmbeddableAttributes.IsReadOnlyAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyIsReadOnlyAttribute, diagnostics, AttributeDescription.IsReadOnlyAttribute, factory);
            }
            if ((embeddableAttributes & EmbeddableAttributes.IsByRefLikeAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyIsByRefLikeAttribute, diagnostics, AttributeDescription.IsByRefLikeAttribute, factory);
            }
            if ((embeddableAttributes & EmbeddableAttributes.IsUnmanagedAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyIsUnmanagedAttribute, diagnostics, AttributeDescription.IsUnmanagedAttribute, factory);
            }
            if ((embeddableAttributes & EmbeddableAttributes.NullableAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyNullableAttribute, diagnostics, AttributeDescription.NullableAttribute, CreateNullableAttributeSymbol);
            }
            if ((embeddableAttributes & EmbeddableAttributes.NullableContextAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyNullableContextAttribute, diagnostics, AttributeDescription.NullableContextAttribute, CreateNullableContextAttributeSymbol);
            }
            if ((embeddableAttributes & EmbeddableAttributes.NullablePublicOnlyAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyNullablePublicOnlyAttribute, diagnostics, AttributeDescription.NullablePublicOnlyAttribute, CreateNullablePublicOnlyAttributeSymbol);
            }
            if ((embeddableAttributes & EmbeddableAttributes.NativeIntegerAttribute) != 0)
            {
                CreateAttributeIfNeeded(ref _lazyNativeIntegerAttribute, diagnostics, AttributeDescription.NativeIntegerAttribute, CreateNativeIntegerAttributeSymbol);
            }
        }

        private SynthesizedEmbeddedAttributeSymbol CreateParameterlessEmbeddedAttributeSymbol(string name, NamespaceSymbol containingNamespace, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedEmbeddedAttributeSymbol(name, containingNamespace, SourceModule, GetWellKnownType(WellKnownType.System_Attribute, diagnostics));
        }

        private SynthesizedEmbeddedNullableAttributeSymbol CreateNullableAttributeSymbol(string name, NamespaceSymbol containingNamespace, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedEmbeddedNullableAttributeSymbol(name, containingNamespace, SourceModule, GetWellKnownType(WellKnownType.System_Attribute, diagnostics), GetSpecialType(SpecialType.System_Byte, diagnostics));
        }

        private SynthesizedEmbeddedNullableContextAttributeSymbol CreateNullableContextAttributeSymbol(string name, NamespaceSymbol containingNamespace, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedEmbeddedNullableContextAttributeSymbol(name, containingNamespace, SourceModule, GetWellKnownType(WellKnownType.System_Attribute, diagnostics), GetSpecialType(SpecialType.System_Byte, diagnostics));
        }

        private SynthesizedEmbeddedNullablePublicOnlyAttributeSymbol CreateNullablePublicOnlyAttributeSymbol(string name, NamespaceSymbol containingNamespace, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedEmbeddedNullablePublicOnlyAttributeSymbol(name, containingNamespace, SourceModule, GetWellKnownType(WellKnownType.System_Attribute, diagnostics), GetSpecialType(SpecialType.System_Boolean, diagnostics));
        }

        private SynthesizedEmbeddedNativeIntegerAttributeSymbol CreateNativeIntegerAttributeSymbol(string name, NamespaceSymbol containingNamespace, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedEmbeddedNativeIntegerAttributeSymbol(name, containingNamespace, SourceModule, GetWellKnownType(WellKnownType.System_Attribute, diagnostics), GetSpecialType(SpecialType.System_Boolean, diagnostics));
        }

        private void CreateAttributeIfNeeded<T>(ref T symbol, BindingDiagnosticBag diagnostics, AttributeDescription description, Func<string, NamespaceSymbol, BindingDiagnosticBag, T> factory) where T : SynthesizedEmbeddedAttributeSymbolBase
        {
            if (symbol is null)
            {
                AddDiagnosticsForExistingAttribute(description, diagnostics);
                NamespaceSymbol orSynthesizeNamespace = GetOrSynthesizeNamespace(description.Namespace);
                symbol = factory(description.Name, orSynthesizeNamespace, diagnostics);
                if (symbol.GetAttributeUsageInfo() != AttributeUsageInfo.Default)
                {
                    EnsureAttributeUsageAttributeMembersAvailable(diagnostics);
                }
                AddSynthesizedDefinition(orSynthesizeNamespace, symbol);
            }
        }

        private void AddDiagnosticsForExistingAttribute(AttributeDescription description, BindingDiagnosticBag diagnostics)
        {
            MetadataTypeName emittedName = MetadataTypeName.FromFullName(description.FullName);
            NamedTypeSymbol namedTypeSymbol = _sourceAssembly.SourceModule.LookupTopLevelMetadataType(ref emittedName);
            if (!(namedTypeSymbol is MissingMetadataTypeSymbol))
            {
                diagnostics.Add(ErrorCode.ERR_TypeReserved, namedTypeSymbol.Locations[0], description.FullName);
            }
        }

        private NamespaceSymbol GetOrSynthesizeNamespace(string namespaceFullName)
        {
            NamespaceSymbol namespaceSymbol = SourceModule.GlobalNamespace;
            string[] array = namespaceFullName.Split(new char[1] { '.' });
            foreach (string name in array)
            {
                NamespaceSymbol namespaceSymbol2 = (NamespaceSymbol)namespaceSymbol.GetMembers(name).FirstOrDefault((Symbol m) => m.Kind == SymbolKind.Namespace);
                if (namespaceSymbol2 == null)
                {
                    namespaceSymbol2 = new SynthesizedNamespaceSymbol(namespaceSymbol, name);
                    AddSynthesizedDefinition(namespaceSymbol, namespaceSymbol2);
                }
                namespaceSymbol = namespaceSymbol2;
            }
            return namespaceSymbol;
        }

        private NamedTypeSymbol GetWellKnownType(WellKnownType type, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol wellKnownType = _sourceAssembly.DeclaringCompilation.GetWellKnownType(type);
            Binder.ReportUseSite(wellKnownType, diagnostics, Location.None);
            return wellKnownType;
        }

        private NamedTypeSymbol GetSpecialType(SpecialType type, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol specialType = _sourceAssembly.DeclaringCompilation.GetSpecialType(type);
            Binder.ReportUseSite(specialType, diagnostics, Location.None);
            return specialType;
        }

        private void EnsureAttributeUsageAttributeMembersAvailable(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = _sourceAssembly.DeclaringCompilation;
            Binder.GetWellKnownTypeMember(declaringCompilation, WellKnownMember.System_AttributeUsageAttribute__ctor, diagnostics, Location.None);
            Binder.GetWellKnownTypeMember(declaringCompilation, WellKnownMember.System_AttributeUsageAttribute__AllowMultiple, diagnostics, Location.None);
            Binder.GetWellKnownTypeMember(declaringCompilation, WellKnownMember.System_AttributeUsageAttribute__Inherited, diagnostics, Location.None);
        }
    }
}
