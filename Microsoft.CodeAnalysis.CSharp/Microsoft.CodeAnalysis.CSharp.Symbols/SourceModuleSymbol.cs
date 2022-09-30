using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SourceModuleSymbol : NonMissingModuleSymbol, IAttributeTargetSymbol
    {
        private readonly SourceAssemblySymbol _assemblySymbol;

        private ImmutableArray<AssemblySymbol> _lazyAssembliesToEmbedTypesFrom;

        private ThreeState _lazyContainsExplicitDefinitionOfNoPiaLocalTypes;

        private readonly DeclarationTable _sources;

        private SymbolCompletionState _state;

        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        private ImmutableArray<Location> _locations;

        private NamespaceSymbol _globalNamespace;

        private bool _hasBadAttributes;

        private readonly string _name;

        internal bool HasBadAttributes => _hasBadAttributes;

        internal override int Ordinal => 0;

        internal override Machine Machine => DeclaringCompilation.Options.Platform switch
        {
            Platform.Arm => Machine.ArmThumb2,
            Platform.X64 => Machine.Amd64,
            Platform.Arm64 => Machine.Arm64,
            Platform.Itanium => Machine.IA64,
            _ => Machine.I386,
        };

        internal override bool Bit32Required => DeclaringCompilation.Options.Platform == Platform.X86;

        internal bool AnyReferencedAssembliesAreLinked => GetAssembliesToEmbedTypesFrom().Length > 0;

        internal bool ContainsExplicitDefinitionOfNoPiaLocalTypes
        {
            get
            {
                if (_lazyContainsExplicitDefinitionOfNoPiaLocalTypes == ThreeState.Unknown)
                {
                    _lazyContainsExplicitDefinitionOfNoPiaLocalTypes = NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(GlobalNamespace).ToThreeState();
                }
                return _lazyContainsExplicitDefinitionOfNoPiaLocalTypes == ThreeState.True;
            }
        }

        public override NamespaceSymbol GlobalNamespace
        {
            get
            {
                if ((object)_globalNamespace == null)
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    SourceNamespaceSymbol value = new SourceNamespaceSymbol(this, this, DeclaringCompilation.MergedRootDeclaration, instance);
                    if (Interlocked.CompareExchange(ref _globalNamespace, value, null) == null)
                    {
                        AddDeclarationDiagnostics(instance);
                    }
                    instance.Free();
                }
                return _globalNamespace;
            }
        }

        internal sealed override bool RequiresCompletion => true;

        public override ImmutableArray<Location> Locations
        {
            get
            {
                if (_locations.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _locations, DeclaringCompilation.MergedRootDeclaration.Declarations.SelectAsArray((Func<SingleNamespaceDeclaration, Location>)((SingleNamespaceDeclaration d) => d.Location)));
                }
                return _locations;
            }
        }

        public override string Name => _name;

        public override Symbol ContainingSymbol => _assemblySymbol;

        public override AssemblySymbol ContainingAssembly => _assemblySymbol;

        internal SourceAssemblySymbol ContainingSourceAssembly => _assemblySymbol;

        internal override CSharpCompilation DeclaringCompilation => _assemblySymbol.DeclaringCompilation;

        internal override ICollection<string> TypeNames => _sources.TypeNames;

        internal override ICollection<string> NamespaceNames => _sources.NamespaceNames;

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => _assemblySymbol;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Module;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                if (!ContainingAssembly.IsInteractive)
                {
                    return AttributeLocation.Assembly | AttributeLocation.Module;
                }
                return AttributeLocation.None;
            }
        }

        internal override bool HasAssemblyCompilationRelaxationsAttribute => ((SourceAssemblySymbol)ContainingAssembly).GetSourceDecodedWellKnownAttributeData()?.HasCompilationRelaxationsAttribute ?? false;

        internal override bool HasAssemblyRuntimeCompatibilityAttribute => ((SourceAssemblySymbol)ContainingAssembly).GetSourceDecodedWellKnownAttributeData()?.HasRuntimeCompatibilityAttribute ?? false;

        internal override CharSet? DefaultMarshallingCharSet
        {
            get
            {
                ModuleWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasDefaultCharSetAttribute)
                {
                    return null;
                }
                return decodedWellKnownAttributeData.DefaultCharacterSet;
            }
        }

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                ModuleWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData == null)
                {
                    return true;
                }
                return !decodedWellKnownAttributeData.HasSkipLocalsInitAttribute;
            }
        }

        internal SourceModuleSymbol(SourceAssemblySymbol assemblySymbol, DeclarationTable declarations, string moduleName)
        {
            _assemblySymbol = assemblySymbol;
            _sources = declarations;
            _name = moduleName;
        }

        internal void RecordPresenceOfBadAttributes()
        {
            _hasBadAttributes = true;
        }

        internal bool MightContainNoPiaLocalTypes()
        {
            if (!AnyReferencedAssembliesAreLinked)
            {
                return ContainsExplicitDefinitionOfNoPiaLocalTypes;
            }
            return true;
        }

        internal ImmutableArray<AssemblySymbol> GetAssembliesToEmbedTypesFrom()
        {
            if (_lazyAssembliesToEmbedTypesFrom.IsDefault)
            {
                ArrayBuilder<AssemblySymbol> instance = ArrayBuilder<AssemblySymbol>.GetInstance();
                ImmutableArray<AssemblySymbol>.Enumerator enumerator = GetReferencedAssemblySymbols().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AssemblySymbol current = enumerator.Current;
                    if (current.IsLinked)
                    {
                        instance.Add(current);
                    }
                }
                ImmutableInterlocked.InterlockedCompareExchange(ref _lazyAssembliesToEmbedTypesFrom, instance.ToImmutableAndFree(), default(ImmutableArray<AssemblySymbol>));
            }
            return _lazyAssembliesToEmbedTypesFrom;
        }

        private static bool NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes(NamespaceSymbol ns)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = ns.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                switch (current.Kind)
                {
                    case SymbolKind.Namespace:
                        if (NamespaceContainsExplicitDefinitionOfNoPiaLocalTypes((NamespaceSymbol)current))
                        {
                            return true;
                        }
                        break;
                    case SymbolKind.NamedType:
                        if (((NamedTypeSymbol)current).IsExplicitDefinitionOfNoPiaLocalType)
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return _state.HasComplete(part);
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompletionPart nextIncompletePart = _state.NextIncompletePart;
                switch (nextIncompletePart)
                {
                    case CompletionPart.Attributes:
                        GetAttributes();
                        break;
                    case CompletionPart.StartBaseType:
                        {
                            BindingDiagnosticBag bindingDiagnosticBag = null;
                            if (AnyReferencedAssembliesAreLinked)
                            {
                                bindingDiagnosticBag = BindingDiagnosticBag.GetInstance();
                                ValidateLinkedAssemblies(bindingDiagnosticBag, cancellationToken);
                            }
                            if (_state.NotePartComplete(CompletionPart.StartBaseType))
                            {
                                if (bindingDiagnosticBag != null)
                                {
                                    _assemblySymbol.AddDeclarationDiagnostics(bindingDiagnosticBag);
                                }
                                _state.NotePartComplete(CompletionPart.FinishBaseType);
                            }
                            bindingDiagnosticBag?.Free();
                            break;
                        }
                    case CompletionPart.FinishBaseType:
                        _state.SpinWaitComplete(CompletionPart.FinishBaseType, cancellationToken);
                        break;
                    case CompletionPart.MembersCompleted:
                        GlobalNamespace.ForceComplete(locationOpt, cancellationToken);
                        if (GlobalNamespace.HasComplete(CompletionPart.MembersCompleted))
                        {
                            _state.NotePartComplete(CompletionPart.MembersCompleted);
                            break;
                        }
                        return;
                    case CompletionPart.None:
                        return;
                    default:
                        _state.NotePartComplete(nextIncompletePart);
                        break;
                }
                _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        private void ValidateLinkedAssemblies(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            ImmutableArray<AssemblySymbol>.Enumerator enumerator = GetReferencedAssemblySymbols().GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssemblySymbol current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                if (current.IsMissing || !current.IsLinked)
                {
                    continue;
                }
                bool flag = false;
                bool flag2 = false;
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator2 = current.GetAttributes().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    CSharpAttributeData current2 = enumerator2.Current;
                    if (current2.IsTargetAttribute(current, AttributeDescription.GuidAttribute))
                    {
                        if (current2.TryGetGuidAttributeValue(out var _))
                        {
                            flag = true;
                        }
                    }
                    else if (current2.IsTargetAttribute(current, AttributeDescription.ImportedFromTypeLibAttribute))
                    {
                        if (current2.CommonConstructorArguments.Length == 1)
                        {
                            flag2 = true;
                        }
                    }
                    else if (current2.IsTargetAttribute(current, AttributeDescription.PrimaryInteropAssemblyAttribute) && current2.CommonConstructorArguments.Length == 2)
                    {
                        flag2 = true;
                    }
                    if (flag && flag2)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    diagnostics.Add(ErrorCode.ERR_NoPIAAssemblyMissingAttribute, NoLocation.Singleton, current, AttributeDescription.GuidAttribute.FullName);
                }
                if (!flag2)
                {
                    diagnostics.Add(ErrorCode.ERR_NoPIAAssemblyMissingAttributes, NoLocation.Singleton, current, AttributeDescription.ImportedFromTypeLibAttribute.FullName, AttributeDescription.PrimaryInteropAssemblyAttribute.FullName);
                }
            }
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
            {
                ImmutableArray<SyntaxList<AttributeListSyntax>> attributeDeclarations = ((SourceAssemblySymbol)ContainingAssembly).GetAttributeDeclarations();
                if (LoadAndValidateAttributes(OneOrMany.Create(attributeDeclarations), ref _lazyCustomAttributesBag))
                {
                    _state.NotePartComplete(CompletionPart.Attributes);
                }
            }
            return _lazyCustomAttributesBag;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        private ModuleWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (ModuleWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultCharSetAttribute))
            {
                CharSet constructorArgument = attribute.GetConstructorArgument<CharSet>(0, SpecialType.System_Enum);
                if (!CommonModuleWellKnownAttributeData.IsValidCharSet(constructorArgument))
                {
                    CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, arguments.AttributeSyntaxOpt);
                    ((BindingDiagnosticBag)arguments.Diagnostics).Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, arguments.AttributeSyntaxOpt!.GetErrorDisplayName());
                }
                else
                {
                    arguments.GetOrCreateData<ModuleWellKnownAttributeData>().DefaultCharacterSet = constructorArgument;
                }
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.NullableContextAttribute | ReservedAttributes.NullablePublicOnlyAttribute) && attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
            {
                CSharpAttributeData.DecodeSkipLocalsInitAttribute<ModuleWellKnownAttributeData>(DeclaringCompilation, ref arguments);
            }
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = _assemblySymbol.DeclaringCompilation;
            if (declaringCompilation.Options.AllowUnsafe && !(declaringCompilation.GetWellKnownType(WellKnownType.System_Security_UnverifiableCodeAttribute) is MissingMetadataTypeSymbol))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Security_UnverifiableCodeAttribute__ctor));
            }
            if (moduleBuilder.ShouldEmitNullablePublicOnlyAttribute())
            {
                ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(declaringCompilation.GetSpecialType(SpecialType.System_Boolean), TypedConstantKind.Primitive, _assemblySymbol.InternalsAreVisible));
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullablePublicOnlyAttribute(arguments));
            }
        }

        public override ModuleMetadata GetMetadata()
        {
            return null;
        }
    }
}
