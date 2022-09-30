using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceAssemblySymbol : MetadataOrSourceAssemblySymbol, ISourceAssemblySymbolInternal, IAssemblySymbolInternal, ISymbolInternal, IAttributeTargetSymbol
    {
        private class NameCollisionForAddedModulesTypeComparer : IComparer<NamedTypeSymbol>
        {
            public static readonly NameCollisionForAddedModulesTypeComparer Singleton = new NameCollisionForAddedModulesTypeComparer();

            private NameCollisionForAddedModulesTypeComparer()
            {
            }

            public int Compare(NamedTypeSymbol x, NamedTypeSymbol y)
            {
                int num = string.CompareOrdinal(x.Name, y.Name);
                if (num == 0)
                {
                    num = x.Arity - y.Arity;
                    if (num == 0)
                    {
                        num = x.ContainingModule.Ordinal - y.ContainingModule.Ordinal;
                    }
                }
                return num;
            }
        }

        private readonly CSharpCompilation _compilation;

        private SymbolCompletionState _state;

        internal AssemblyIdentity lazyAssemblyIdentity;

        private readonly string _assemblySimpleName;

        private StrongNameKeys _lazyStrongNameKeys;

        private readonly ImmutableArray<ModuleSymbol> _modules;

        private CustomAttributesBag<CSharpAttributeData> _lazySourceAttributesBag;

        private CustomAttributesBag<CSharpAttributeData> _lazyNetModuleAttributesBag;

        private IDictionary<string, NamedTypeSymbol> _lazyForwardedTypesFromSource;

        private ConcurrentSet<int> _lazyOmittedAttributeIndices;

        private ThreeState _lazyContainsExtensionMethods;

        private readonly ConcurrentDictionary<FieldSymbol, bool> _unassignedFieldsMap = new ConcurrentDictionary<FieldSymbol, bool>();

        private readonly ConcurrentSet<FieldSymbol> _unreadFields = new ConcurrentSet<FieldSymbol>();

        internal ConcurrentSet<TypeSymbol> TypesReferencedInExternalMethods = new ConcurrentSet<TypeSymbol>();

        private ImmutableArray<Diagnostic> _unusedFieldWarnings;

        private ConcurrentDictionary<AssemblySymbol, bool> _optimisticallyGrantedInternalsAccess;

        [ThreadStatic]
        private static AssemblySymbol t_assemblyForWhichCurrentThreadIsComputingKeys;

        private ConcurrentDictionary<string, ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>> _lazyInternalsVisibleToMap;

        public override string Name => _assemblySimpleName;

        internal sealed override CSharpCompilation DeclaringCompilation => _compilation;

        public override bool IsInteractive => _compilation.IsSubmission;

        public override AssemblyIdentity Identity
        {
            get
            {
                if (lazyAssemblyIdentity == null)
                {
                    Interlocked.CompareExchange(ref lazyAssemblyIdentity, ComputeIdentity(), null);
                }
                return lazyAssemblyIdentity;
            }
        }

        internal bool RuntimeCompatibilityWrapNonExceptionThrows => (GetSourceDecodedWellKnownAttributeData() ?? GetNetModuleDecodedWellKnownAttributeData())?.RuntimeCompatibilityWrapNonExceptionThrows ?? true;

        internal string FileVersion => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyFileVersionAttributeSetting);

        internal string Title => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyTitleAttributeSetting);

        internal string Description => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyDescriptionAttributeSetting);

        internal string Company => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCompanyAttributeSetting);

        internal string Product => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyProductAttributeSetting);

        internal string InformationalVersion => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyInformationalVersionAttributeSetting);

        internal string Copyright => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCopyrightAttributeSetting);

        internal string Trademark => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyTrademarkAttributeSetting);

        private ThreeState AssemblyDelaySignAttributeSetting
        {
            get
            {
                ThreeState threeState = ThreeState.Unknown;
                ThreeState threeState2 = threeState;
                CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
                if (sourceDecodedWellKnownAttributeData != null)
                {
                    threeState2 = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting;
                }
                if (threeState2 == threeState)
                {
                    sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
                    if (sourceDecodedWellKnownAttributeData != null)
                    {
                        threeState2 = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting;
                    }
                }
                return threeState2;
            }
        }

        private string AssemblyKeyContainerAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyKeyContainerAttributeSetting, WellKnownAttributeData.StringMissingValue, QuickAttributes.AssemblyKeyName);

        private string AssemblyKeyFileAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyKeyFileAttributeSetting, WellKnownAttributeData.StringMissingValue, QuickAttributes.AssemblyKeyFile);

        private string AssemblyCultureAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCultureAttributeSetting);

        public string SignatureKey => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblySignatureKeyAttributeSetting, null, QuickAttributes.AssemblySignatureKey);

        private Version AssemblyVersionAttributeSetting
        {
            get
            {
                Version version = null;
                Version version2 = version;
                CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
                if (sourceDecodedWellKnownAttributeData != null)
                {
                    version2 = sourceDecodedWellKnownAttributeData.AssemblyVersionAttributeSetting;
                }
                if (version2 == version)
                {
                    sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
                    if (sourceDecodedWellKnownAttributeData != null)
                    {
                        version2 = sourceDecodedWellKnownAttributeData.AssemblyVersionAttributeSetting;
                    }
                }
                return version2;
            }
        }

        public override Version AssemblyVersionPattern
        {
            get
            {
                Version assemblyVersionAttributeSetting = AssemblyVersionAttributeSetting;
                if ((object)assemblyVersionAttributeSetting != null && (assemblyVersionAttributeSetting.Build == 65535 || assemblyVersionAttributeSetting.Revision == 65535))
                {
                    return assemblyVersionAttributeSetting;
                }
                return null;
            }
        }

        public AssemblyHashAlgorithm HashAlgorithm => AssemblyAlgorithmIdAttributeSetting ?? AssemblyHashAlgorithm.Sha1;

        internal AssemblyHashAlgorithm? AssemblyAlgorithmIdAttributeSetting
        {
            get
            {
                AssemblyHashAlgorithm? result = null;
                CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
                if (sourceDecodedWellKnownAttributeData != null)
                {
                    result = sourceDecodedWellKnownAttributeData.AssemblyAlgorithmIdAttributeSetting;
                }
                if (!result.HasValue)
                {
                    sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
                    if (sourceDecodedWellKnownAttributeData != null)
                    {
                        result = sourceDecodedWellKnownAttributeData.AssemblyAlgorithmIdAttributeSetting;
                    }
                }
                return result;
            }
        }

        public AssemblyFlags AssemblyFlags
        {
            get
            {
                AssemblyFlags assemblyFlags = 0;
                CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
                if (sourceDecodedWellKnownAttributeData != null)
                {
                    assemblyFlags = sourceDecodedWellKnownAttributeData.AssemblyFlagsAttributeSetting;
                }
                sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
                if (sourceDecodedWellKnownAttributeData != null)
                {
                    assemblyFlags |= sourceDecodedWellKnownAttributeData.AssemblyFlagsAttributeSetting;
                }
                return assemblyFlags;
            }
        }

        internal StrongNameKeys StrongNameKeys
        {
            get
            {
                if (_lazyStrongNameKeys == null)
                {
                    try
                    {
                        t_assemblyForWhichCurrentThreadIsComputingKeys = this;
                        Interlocked.CompareExchange(ref _lazyStrongNameKeys, ComputeStrongNameKeys(), null);
                    }
                    finally
                    {
                        t_assemblyForWhichCurrentThreadIsComputingKeys = null;
                    }
                }
                return _lazyStrongNameKeys;
            }
        }

        internal override ImmutableArray<byte> PublicKey => StrongNameKeys.PublicKey;

        public override ImmutableArray<ModuleSymbol> Modules => _modules;

        public override ImmutableArray<Location> Locations => Modules.SelectMany((ModuleSymbol m) => m.Locations).AsImmutable();

        public bool InternalsAreVisible
        {
            get
            {
                EnsureAttributesAreBound();
                return _lazyInternalsVisibleToMap != null;
            }
        }

        internal bool IsDelaySigned
        {
            get
            {
                if (_compilation.Options.DelaySign.HasValue)
                {
                    return _compilation.Options.DelaySign.Value;
                }
                if (_compilation.Options.PublicSign)
                {
                    return false;
                }
                return AssemblyDelaySignAttributeSetting == ThreeState.True;
            }
        }

        internal SourceModuleSymbol SourceModule => (SourceModuleSymbol)Modules[0];

        internal override bool RequiresCompletion => true;

        internal override bool IsLinked => false;

        internal bool DeclaresTheObjectClass
        {
            get
            {
                if ((object)base.CorLibrary != this)
                {
                    return false;
                }
                NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Object);
                if (!specialType.IsErrorType())
                {
                    return specialType.DeclaredAccessibility == Accessibility.Public;
                }
                return false;
            }
        }

        public override bool MightContainExtensionMethods
        {
            get
            {
                if (_lazyContainsExtensionMethods.HasValue())
                {
                    return _lazyContainsExtensionMethods.Value();
                }
                return true;
            }
        }

        private bool HasDebuggableAttribute => GetSourceDecodedWellKnownAttributeData()?.HasDebuggableAttribute ?? false;

        private bool HasReferenceAssemblyAttribute => GetSourceDecodedWellKnownAttributeData()?.HasReferenceAssemblyAttribute ?? false;

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => this;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Assembly;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                if (!IsInteractive)
                {
                    return AttributeLocation.Assembly | AttributeLocation.Module;
                }
                return AttributeLocation.None;
            }
        }

        internal IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder, bool emittingRefAssembly, bool emittingAssemblyAttributesInNetModule)
        {
            ImmutableArray<CSharpAttributeData> attributes = GetAttributes();
            ArrayBuilder<SynthesizedAttributeData> attributes2 = null;
            AddSynthesizedAttributes(moduleBuilder, ref attributes2);
            if (emittingRefAssembly && !HasReferenceAssemblyAttribute)
            {
                SynthesizedAttributeData attribute = DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor, default(ImmutableArray<TypedConstant>), default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true);
                Symbol.AddSynthesizedAttribute(ref attributes2, attribute);
            }
            return GetCustomAttributesToEmit(attributes, attributes2, isReturnType: false, emittingAssemblyAttributesInNetModule);
        }

        internal SourceAssemblySymbol(CSharpCompilation compilation, string assemblySimpleName, string moduleName, ImmutableArray<PEModule> netModules)
        {
            _compilation = compilation;
            _assemblySimpleName = assemblySimpleName;
            ArrayBuilder<ModuleSymbol> arrayBuilder = new ArrayBuilder<ModuleSymbol>(1 + netModules.Length)
            {
                new SourceModuleSymbol(this, compilation.Declarations, moduleName)
            };
            MetadataImportOptions importOptions = ((compilation.Options.MetadataImportOptions != MetadataImportOptions.All) ? MetadataImportOptions.Internal : MetadataImportOptions.All);
            ImmutableArray<PEModule>.Enumerator enumerator = netModules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PEModule current = enumerator.Current;
                arrayBuilder.Add(new PEModuleSymbol(this, current, importOptions, arrayBuilder.Count));
            }
            _modules = arrayBuilder.ToImmutableAndFree();
            if (!compilation.Options.CryptoPublicKey.IsEmpty)
            {
                _lazyStrongNameKeys = StrongNameKeys.Create(compilation.Options.CryptoPublicKey, null, hasCounterSignature: false, MessageProvider.Instance);
            }
        }

        internal bool MightContainNoPiaLocalTypes()
        {
            for (int i = 1; i < _modules.Length; i++)
            {
                if (((PEModuleSymbol)_modules[i]).Module.ContainsNoPiaLocalTypes())
                {
                    return true;
                }
            }
            return SourceModule.MightContainNoPiaLocalTypes();
        }

        internal override Symbol GetSpecialTypeMember(SpecialMember member)
        {
            if (!_compilation.IsMemberMissing(member))
            {
                return base.GetSpecialTypeMember(member);
            }
            return null;
        }

        private string? GetWellKnownAttributeDataStringField(Func<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>, string> fieldGetter, string? missingValue = null, QuickAttributes? attributeMatchesOpt = null)
        {
            string text = missingValue;
            CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> commonAssemblyWellKnownAttributeData = ((!attributeMatchesOpt.HasValue) ? GetSourceDecodedWellKnownAttributeData() : GetSourceDecodedWellKnownAttributeData(attributeMatchesOpt.Value));
            if (commonAssemblyWellKnownAttributeData != null)
            {
                text = fieldGetter(commonAssemblyWellKnownAttributeData);
            }
            if ((object)text == missingValue)
            {
                commonAssemblyWellKnownAttributeData = ((!attributeMatchesOpt.HasValue || _lazyNetModuleAttributesBag != null) ? GetNetModuleDecodedWellKnownAttributeData() : GetLimitedNetModuleDecodedWellKnownAttributeData(attributeMatchesOpt.Value));
                if (commonAssemblyWellKnownAttributeData != null)
                {
                    text = fieldGetter(commonAssemblyWellKnownAttributeData);
                }
            }
            return text;
        }

        private StrongNameKeys ComputeStrongNameKeys()
        {
            string text = _compilation.Options.CryptoKeyFile;
            if (DeclaringCompilation.Options.PublicSign)
            {
                if (!string.IsNullOrEmpty(text) && !PathUtilities.IsAbsolute(text))
                {
                    return StrongNameKeys.None;
                }
                return StrongNameKeys.Create(text, MessageProvider.Instance);
            }
            if (string.IsNullOrEmpty(text))
            {
                text = AssemblyKeyFileAttributeSetting;
                if ((object)text == WellKnownAttributeData.StringMissingValue)
                {
                    text = null;
                }
            }
            string text2 = _compilation.Options.CryptoKeyContainer;
            if (string.IsNullOrEmpty(text2))
            {
                text2 = AssemblyKeyContainerAttributeSetting;
                if ((object)text2 == WellKnownAttributeData.StringMissingValue)
                {
                    text2 = null;
                }
            }
            bool hasCounterSignature = !string.IsNullOrEmpty(SignatureKey);
            return StrongNameKeys.Create(DeclaringCompilation.Options.StrongNameProvider, text, text2, hasCounterSignature, MessageProvider.Instance);
        }

        private void ValidateAttributeSemantics(BindingDiagnosticBag diagnostics)
        {
            if (StrongNameKeys.DiagnosticOpt != null && !_compilation.Options.OutputKind.IsNetModule())
            {
                diagnostics.Add(StrongNameKeys.DiagnosticOpt);
            }
            ValidateIVTPublicKeys(diagnostics);
            CheckOptimisticIVTAccessGrants(diagnostics);
            DetectAttributeAndOptionConflicts(diagnostics);
            if (IsDelaySigned && !Identity.HasPublicKey)
            {
                diagnostics.Add(ErrorCode.WRN_DelaySignButNoKey, NoLocation.Singleton);
            }
            if (DeclaringCompilation.Options.PublicSign)
            {
                if (_compilation.Options.OutputKind.IsNetModule())
                {
                    diagnostics.Add(ErrorCode.ERR_PublicSignNetModule, NoLocation.Singleton);
                }
                else if (!Identity.HasPublicKey)
                {
                    diagnostics.Add(ErrorCode.ERR_PublicSignButNoKey, NoLocation.Singleton);
                }
            }
            if (DeclaringCompilation.Options.OutputKind != OutputKind.NetModule && DeclaringCompilation.Options.CryptoPublicKey.IsEmpty && Identity.HasPublicKey && !IsDelaySigned && !DeclaringCompilation.Options.PublicSign && !StrongNameKeys.CanSign && StrongNameKeys.DiagnosticOpt == null)
            {
                diagnostics.Add(ErrorCode.ERR_SignButNoPrivateKey, NoLocation.Singleton, StrongNameKeys.KeyFilePath);
            }
            ReportDiagnosticsForSynthesizedAttributes(_compilation, diagnostics);
        }

        private static void ReportDiagnosticsForSynthesizedAttributes(CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
        {
            ReportDiagnosticsForUnsafeSynthesizedAttributes(compilation, diagnostics);
            if (!compilation.Options.OutputKind.IsNetModule())
            {
                if (!(compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) is MissingMetadataTypeSymbol))
                {
                    Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, diagnostics, NoLocation.Singleton);
                }
                if (!(compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) is MissingMetadataTypeSymbol))
                {
                    Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, diagnostics, NoLocation.Singleton);
                    Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, diagnostics, NoLocation.Singleton);
                }
            }
        }

        private static void ReportDiagnosticsForUnsafeSynthesizedAttributes(CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
        {
            if (compilation.Options.AllowUnsafe && !(compilation.GetWellKnownType(WellKnownType.System_Security_UnverifiableCodeAttribute) is MissingMetadataTypeSymbol))
            {
                Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Security_UnverifiableCodeAttribute__ctor, diagnostics, NoLocation.Singleton);
                if (!(compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityPermissionAttribute) is MissingMetadataTypeSymbol) && !(compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityAction) is MissingMetadataTypeSymbol))
                {
                    Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__ctor, diagnostics, NoLocation.Singleton);
                    Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(compilation, WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__SkipVerification, diagnostics, NoLocation.Singleton);
                }
            }
        }

        private void ValidateIVTPublicKeys(BindingDiagnosticBag diagnostics)
        {
            EnsureAttributesAreBound();
            if (!Identity.IsStrongName || _lazyInternalsVisibleToMap == null)
            {
                return;
            }
            foreach (ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> value in _lazyInternalsVisibleToMap.Values)
            {
                foreach (KeyValuePair<ImmutableArray<byte>, Tuple<Location, string>> item in value)
                {
                    if (item.Key.IsDefaultOrEmpty)
                    {
                        diagnostics.Add(ErrorCode.ERR_FriendAssemblySNReq, item.Value.Item1, item.Value.Item2);
                    }
                }
            }
        }

        private void DetectAttributeAndOptionConflicts(BindingDiagnosticBag diagnostics)
        {
            EnsureAttributesAreBound();
            ThreeState assemblyDelaySignAttributeSetting = AssemblyDelaySignAttributeSetting;
            if (_compilation.Options.DelaySign.HasValue && assemblyDelaySignAttributeSetting != 0 && DeclaringCompilation.Options.DelaySign.Value != (assemblyDelaySignAttributeSetting == ThreeState.True))
            {
                diagnostics.Add(ErrorCode.WRN_CmdOptionConflictsSource, NoLocation.Singleton, "DelaySign", AttributeDescription.AssemblyDelaySignAttribute.FullName);
            }
            if (_compilation.Options.PublicSign && assemblyDelaySignAttributeSetting == ThreeState.True)
            {
                diagnostics.Add(ErrorCode.WRN_CmdOptionConflictsSource, NoLocation.Singleton, "PublicSign", AttributeDescription.AssemblyDelaySignAttribute.FullName);
            }
            if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyContainer))
            {
                string assemblyKeyContainerAttributeSetting = AssemblyKeyContainerAttributeSetting;
                if ((object)assemblyKeyContainerAttributeSetting == WellKnownAttributeData.StringMissingValue)
                {
                    if (_compilation.Options.OutputKind == OutputKind.NetModule)
                    {
                        Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(_compilation, WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, diagnostics, NoLocation.Singleton);
                    }
                }
                else if (string.Compare(_compilation.Options.CryptoKeyContainer, assemblyKeyContainerAttributeSetting, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (_compilation.Options.OutputKind == OutputKind.NetModule)
                    {
                        diagnostics.Add(ErrorCode.ERR_CmdOptionConflictsSource, NoLocation.Singleton, AttributeDescription.AssemblyKeyNameAttribute.FullName, "CryptoKeyContainer");
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.WRN_CmdOptionConflictsSource, NoLocation.Singleton, "CryptoKeyContainer", AttributeDescription.AssemblyKeyNameAttribute.FullName);
                    }
                }
            }
            if (_compilation.Options.PublicSign && !_compilation.Options.OutputKind.IsNetModule() && (object)AssemblyKeyContainerAttributeSetting != WellKnownAttributeData.StringMissingValue)
            {
                diagnostics.Add(ErrorCode.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, AttributeDescription.AssemblyKeyNameAttribute.FullName);
            }
            if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyFile))
            {
                string assemblyKeyFileAttributeSetting = AssemblyKeyFileAttributeSetting;
                if ((object)assemblyKeyFileAttributeSetting == WellKnownAttributeData.StringMissingValue)
                {
                    if (_compilation.Options.OutputKind == OutputKind.NetModule)
                    {
                        Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(_compilation, WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, diagnostics, NoLocation.Singleton);
                    }
                }
                else if (string.Compare(_compilation.Options.CryptoKeyFile, assemblyKeyFileAttributeSetting, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (_compilation.Options.OutputKind == OutputKind.NetModule)
                    {
                        diagnostics.Add(ErrorCode.ERR_CmdOptionConflictsSource, NoLocation.Singleton, AttributeDescription.AssemblyKeyFileAttribute.FullName, "CryptoKeyFile");
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.WRN_CmdOptionConflictsSource, NoLocation.Singleton, "CryptoKeyFile", AttributeDescription.AssemblyKeyFileAttribute.FullName);
                    }
                }
            }
            if (_compilation.Options.PublicSign && !_compilation.Options.OutputKind.IsNetModule() && (object)AssemblyKeyFileAttributeSetting != WellKnownAttributeData.StringMissingValue)
            {
                diagnostics.Add(ErrorCode.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, AttributeDescription.AssemblyKeyFileAttribute.FullName);
            }
        }

        internal override bool HasComplete(CompletionPart part)
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
                        EnsureAttributesAreBound();
                        break;
                    case CompletionPart.StartBaseType:
                    case CompletionPart.FinishBaseType:
                        if (_state.NotePartComplete(CompletionPart.StartBaseType))
                        {
                            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                            ValidateAttributeSemantics(instance);
                            AddDeclarationDiagnostics(instance);
                            _state.NotePartComplete(CompletionPart.FinishBaseType);
                            instance.Free();
                        }
                        break;
                    case CompletionPart.StartInterfaces:
                        SourceModule.ForceComplete(locationOpt, cancellationToken);
                        if (SourceModule.HasComplete(CompletionPart.MembersCompleted))
                        {
                            _state.NotePartComplete(CompletionPart.StartInterfaces);
                            break;
                        }
                        return;
                    case CompletionPart.EnumUnderlyingType:
                    case CompletionPart.TypeArguments:
                        if (_state.NotePartComplete(CompletionPart.EnumUnderlyingType))
                        {
                            ReportDiagnosticsForAddedModules();
                            _state.NotePartComplete(CompletionPart.TypeArguments);
                        }
                        break;
                    case CompletionPart.None:
                        return;
                    default:
                        _state.NotePartComplete(CompletionPart.NamespaceSymbolAll | CompletionPart.ReturnTypeAttributes | CompletionPart.Parameters | CompletionPart.Type | CompletionPart.FinishInterfaces | CompletionPart.TypeParameters | CompletionPart.TypeMembers | CompletionPart.SynthesizedExplicitImplementations | CompletionPart.StartMemberChecks | CompletionPart.FinishMemberChecks);
                        break;
                }
                _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        private void ReportDiagnosticsForAddedModules()
        {
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            foreach (KeyValuePair<MetadataReference, int> item in _compilation.GetBoundReferenceManager().ReferencedModuleIndexMap)
            {
                if (item.Key is PortableExecutableReference portableExecutableReference && portableExecutableReference.FilePath != null)
                {
                    string fileName = FileNameUtilities.GetFileName(portableExecutableReference.FilePath);
                    string name = _modules[item.Value].Name;
                    if (!string.Equals(fileName, name, StringComparison.OrdinalIgnoreCase))
                    {
                        instance.Add(ErrorCode.ERR_NetModuleNameMismatch, NoLocation.Singleton, name, fileName);
                    }
                }
            }
            if (_modules.Length > 1 && !_compilation.Options.OutputKind.IsNetModule())
            {
                Machine machine = base.Machine;
                bool flag = machine == Machine.I386 && !base.Bit32Required;
                HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 1; i < _modules.Length; i++)
                {
                    ModuleSymbol moduleSymbol = _modules[i];
                    if (!hashSet.Add(moduleSymbol.Name))
                    {
                        instance.Add(ErrorCode.ERR_NetModuleNameMustBeUnique, NoLocation.Singleton, moduleSymbol.Name);
                    }
                    if (((PEModuleSymbol)moduleSymbol).Module.IsCOFFOnly)
                    {
                        continue;
                    }
                    Machine machine2 = moduleSymbol.Machine;
                    if (machine2 != Machine.I386 || moduleSymbol.Bit32Required)
                    {
                        if (flag)
                        {
                            instance.Add(ErrorCode.ERR_AgnosticToMachineModule, NoLocation.Singleton, moduleSymbol);
                        }
                        else if (machine != machine2)
                        {
                            instance.Add(ErrorCode.ERR_ConflictingMachineModule, NoLocation.Singleton, moduleSymbol);
                        }
                    }
                }
                for (int j = 1; j < _modules.Length; j++)
                {
                    PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)_modules[j];
                    try
                    {
                        foreach (string item2 in pEModuleSymbol.Module.GetReferencedManagedModulesOrThrow())
                        {
                            if (hashSet.Add(item2))
                            {
                                instance.Add(ErrorCode.ERR_MissingNetModuleReference, NoLocation.Singleton, item2);
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        instance.Add(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, pEModuleSymbol), NoLocation.Singleton);
                    }
                }
            }
            ReportNameCollisionDiagnosticsForAddedModules(GlobalNamespace, instance);
            AddDeclarationDiagnostics(instance);
            instance.Free();
        }

        private void ReportNameCollisionDiagnosticsForAddedModules(NamespaceSymbol ns, BindingDiagnosticBag diagnostics)
        {
            if (!(ns is MergedNamespaceSymbol mergedNamespaceSymbol))
            {
                return;
            }
            ImmutableArray<NamespaceSymbol> constituentNamespaces = mergedNamespaceSymbol.ConstituentNamespaces;
            if (constituentNamespaces.Length <= 2 && (constituentNamespaces.Length != 2 || constituentNamespaces[0].ContainingModule.Ordinal == 0 || constituentNamespaces[1].ContainingModule.Ordinal == 0))
            {
                return;
            }
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            ImmutableArray<NamespaceSymbol>.Enumerator enumerator = constituentNamespaces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceSymbol current = enumerator.Current;
                if (current.ContainingModule.Ordinal != 0)
                {
                    instance.AddRange(current.GetTypeMembers());
                }
            }
            instance.Sort(NameCollisionForAddedModulesTypeComparer.Singleton);
            bool flag = false;
            for (int i = 0; i < instance.Count - 1; i++)
            {
                NamedTypeSymbol namedTypeSymbol = instance[i];
                NamedTypeSymbol namedTypeSymbol2 = instance[i + 1];
                if (namedTypeSymbol.Arity == namedTypeSymbol2.Arity && namedTypeSymbol.Name == namedTypeSymbol2.Name)
                {
                    if (!flag)
                    {
                        if (namedTypeSymbol.Arity != 0 || !namedTypeSymbol.ContainingNamespace.IsGlobalNamespace || namedTypeSymbol.Name != "<Module>")
                        {
                            diagnostics.Add(ErrorCode.ERR_DuplicateNameInNS, namedTypeSymbol2.Locations.FirstOrNone(), namedTypeSymbol2.ToDisplayString(SymbolDisplayFormat.ShortFormat), namedTypeSymbol2.ContainingNamespace);
                        }
                        flag = true;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            instance.Free();
            ImmutableArray<Symbol>.Enumerator enumerator2 = mergedNamespaceSymbol.GetMembers().GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Symbol current2 = enumerator2.Current;
                if (current2.Kind == SymbolKind.Namespace)
                {
                    ReportNameCollisionDiagnosticsForAddedModules((NamespaceSymbol)current2, diagnostics);
                }
            }
        }

        private bool IsKnownAssemblyAttribute(CSharpAttributeData attribute)
        {
            if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTitleAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDescriptionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyConfigurationAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCultureAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCompanyAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyProductAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyInformationalVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCopyrightAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTrademarkAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyFileAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyNameAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyAlgorithmIdAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFlagsAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDelaySignAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFileVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.SatelliteContractVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblySignatureKeyAttribute))
            {
                return true;
            }
            return false;
        }

        private void AddOmittedAttributeIndex(int index)
        {
            if (_lazyOmittedAttributeIndices == null)
            {
                Interlocked.CompareExchange(ref _lazyOmittedAttributeIndices, new ConcurrentSet<int>(), null);
            }
            _lazyOmittedAttributeIndices.Add(index);
        }

        private HashSet<CSharpAttributeData> GetUniqueSourceAssemblyAttributes()
        {
            ImmutableArray<CSharpAttributeData> attributes = GetSourceAttributesBag().Attributes;
            HashSet<CSharpAttributeData> uniqueAttributes = null;
            for (int i = 0; i < attributes.Length; i++)
            {
                CSharpAttributeData cSharpAttributeData = attributes[i];
                if (!cSharpAttributeData.HasErrors && !AddUniqueAssemblyAttribute(cSharpAttributeData, ref uniqueAttributes))
                {
                    AddOmittedAttributeIndex(i);
                }
            }
            return uniqueAttributes;
        }

        private static bool AddUniqueAssemblyAttribute(CSharpAttributeData attribute, ref HashSet<CSharpAttributeData> uniqueAttributes)
        {
            if (uniqueAttributes == null)
            {
                uniqueAttributes = new HashSet<CSharpAttributeData>(CommonAttributeDataComparer.Instance);
            }
            return uniqueAttributes.Add(attribute);
        }

        private bool ValidateAttributeUsageForNetModuleAttribute(CSharpAttributeData attribute, string netModuleName, BindingDiagnosticBag diagnostics, ref HashSet<CSharpAttributeData> uniqueAttributes)
        {
            NamedTypeSymbol attributeClass = attribute.AttributeClass;
            if (attributeClass.GetAttributeUsageInfo().AllowMultiple)
            {
                return AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes);
            }
            if (uniqueAttributes == null || !uniqueAttributes.Contains((CSharpAttributeData a) => TypeSymbol.Equals(a.AttributeClass, attributeClass, TypeCompareKind.ConsiderEverything)))
            {
                AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes);
                return true;
            }
            if (IsKnownAssemblyAttribute(attribute))
            {
                if (!uniqueAttributes.Contains(attribute))
                {
                    diagnostics.Add(ErrorCode.WRN_AssemblyAttributeFromModuleIsOverridden, NoLocation.Singleton, attribute.AttributeClass, netModuleName);
                }
            }
            else if (AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes))
            {
                diagnostics.Add(ErrorCode.ERR_DuplicateAttributeInNetModule, NoLocation.Singleton, attribute.AttributeClass!.Name, netModuleName);
            }
            return false;
        }

        private ImmutableArray<CSharpAttributeData> GetNetModuleAttributes(out ImmutableArray<string> netModuleNames)
        {
            ArrayBuilder<CSharpAttributeData> arrayBuilder = null;
            ArrayBuilder<string> arrayBuilder2 = null;
            for (int i = 1; i < _modules.Length; i++)
            {
                PEModuleSymbol obj = (PEModuleSymbol)_modules[i];
                string name = obj.Name;
                ImmutableArray<CSharpAttributeData>.Enumerator enumerator = obj.GetAssemblyAttributes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CSharpAttributeData current = enumerator.Current;
                    if (arrayBuilder2 == null)
                    {
                        arrayBuilder2 = ArrayBuilder<string>.GetInstance();
                        arrayBuilder = ArrayBuilder<CSharpAttributeData>.GetInstance();
                    }
                    arrayBuilder2.Add(name);
                    arrayBuilder.Add(current);
                }
            }
            if (arrayBuilder2 == null)
            {
                netModuleNames = ImmutableArray<string>.Empty;
                return ImmutableArray<CSharpAttributeData>.Empty;
            }
            netModuleNames = arrayBuilder2.ToImmutableAndFree();
            return arrayBuilder.ToImmutableAndFree();
        }

        private WellKnownAttributeData ValidateAttributeUsageAndDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> attributesFromNetModules, ImmutableArray<string> netModuleNames, BindingDiagnosticBag diagnostics)
        {
            int length = attributesFromNetModules.Length;
            int length2 = GetSourceAttributesBag().Attributes.Length;
            HashSet<CSharpAttributeData> uniqueAttributes = GetUniqueSourceAssemblyAttributes();
            DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments = default(DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation>);
            arguments.AttributesCount = length;
            arguments.Diagnostics = diagnostics;
            arguments.SymbolPart = AttributeLocation.None;
            for (int num = length - 1; num >= 0; num--)
            {
                int index = num + length2;
                CSharpAttributeData cSharpAttributeData = attributesFromNetModules[num];
                if (!cSharpAttributeData.HasErrors && ValidateAttributeUsageForNetModuleAttribute(cSharpAttributeData, netModuleNames[num], diagnostics, ref uniqueAttributes))
                {
                    arguments.Attribute = cSharpAttributeData;
                    arguments.Index = num;
                    arguments.AttributeSyntaxOpt = null;
                    DecodeWellKnownAttribute(ref arguments, index, isFromNetModule: true);
                }
                else
                {
                    AddOmittedAttributeIndex(index);
                }
            }
            if (!arguments.HasDecodedData)
            {
                return null;
            }
            return arguments.DecodedData;
        }

        private void LoadAndValidateNetModuleAttributes(ref CustomAttributesBag<CSharpAttributeData> lazyNetModuleAttributesBag)
        {
            if (_compilation.Options.OutputKind.IsNetModule())
            {
                Interlocked.CompareExchange(ref lazyNetModuleAttributesBag, CustomAttributesBag<CSharpAttributeData>.Empty, null);
                return;
            }
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            ImmutableArray<CSharpAttributeData> netModuleAttributes = GetNetModuleAttributes(out ImmutableArray<string> netModuleNames);
            WellKnownAttributeData wellKnownAttributeData = null;
            if (netModuleAttributes.Any())
            {
                wellKnownAttributeData = ValidateAttributeUsageAndDecodeWellKnownAttributes(netModuleAttributes, netModuleNames, instance);
            }
            else
            {
                GetUniqueSourceAssemblyAttributes();
            }
            HashSet<NamedTypeSymbol> hashSet = null;
            for (int num = _modules.Length - 1; num > 0; num--)
            {
                foreach (NamedTypeSymbol forwardedType in ((PEModuleSymbol)_modules[num]).GetForwardedTypes())
                {
                    if (hashSet == null)
                    {
                        if (wellKnownAttributeData == null)
                        {
                            wellKnownAttributeData = new CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>();
                        }
                        hashSet = ((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)wellKnownAttributeData).ForwardedTypes;
                        if (hashSet == null)
                        {
                            hashSet = new HashSet<NamedTypeSymbol>();
                            ((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)wellKnownAttributeData).ForwardedTypes = hashSet;
                        }
                    }
                    if (hashSet.Add(forwardedType) && forwardedType.IsErrorType() && !instance.ReportUseSite(forwardedType, NoLocation.Singleton))
                    {
                        DiagnosticInfo errorInfo = ((ErrorTypeSymbol)forwardedType).ErrorInfo;
                        if (errorInfo != null)
                        {
                            instance.Add(errorInfo, NoLocation.Singleton);
                        }
                    }
                }
            }
            CustomAttributesBag<CSharpAttributeData> customAttributesBag;
            if (wellKnownAttributeData != null || netModuleAttributes.Any())
            {
                customAttributesBag = new CustomAttributesBag<CSharpAttributeData>();
                customAttributesBag.SetEarlyDecodedWellKnownAttributeData(null);
                customAttributesBag.SetDecodedWellKnownAttributeData(wellKnownAttributeData);
                customAttributesBag.SetAttributes(netModuleAttributes);
                if (customAttributesBag.IsEmpty)
                {
                    customAttributesBag = CustomAttributesBag<CSharpAttributeData>.Empty;
                }
            }
            else
            {
                customAttributesBag = CustomAttributesBag<CSharpAttributeData>.Empty;
            }
            if (Interlocked.CompareExchange(ref lazyNetModuleAttributesBag, customAttributesBag, null) == null)
            {
                AddDeclarationDiagnostics(instance);
            }
            instance.Free();
        }

        private CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> GetLimitedNetModuleDecodedWellKnownAttributeData(QuickAttributes attributeMatches)
        {
            if (_compilation.Options.OutputKind.IsNetModule())
            {
                return null;
            }
            ImmutableArray<CSharpAttributeData> netModuleAttributes = GetNetModuleAttributes(out ImmutableArray<string> netModuleNames2);
            WellKnownAttributeData wellKnownAttributeData = null;
            if (netModuleAttributes.Any())
            {
                wellKnownAttributeData = limitedDecodeWellKnownAttributes(netModuleAttributes, netModuleNames2, attributeMatches);
            }
            return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)wellKnownAttributeData;
            void limitedDecodeWellKnownAttribute(CSharpAttributeData attribute, QuickAttributes attributeMatches, ref CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> result)
            {
                if (attributeMatches == QuickAttributes.AssemblySignatureKey && attribute.IsTargetAttribute(this, AttributeDescription.AssemblySignatureKeyAttribute))
                {
                    if (result == null)
                    {
                        result = new CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>();
                    }
                    result.AssemblySignatureKeyAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                }
                else if (attributeMatches == QuickAttributes.AssemblyKeyFile && attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyFileAttribute))
                {
                    if (result == null)
                    {
                        result = new CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>();
                    }
                    result.AssemblyKeyFileAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                }
                else if (attributeMatches == QuickAttributes.AssemblyKeyName && attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyNameAttribute))
                {
                    if (result == null)
                    {
                        result = new CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>();
                    }
                    result.AssemblyKeyContainerAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                }
            }
            WellKnownAttributeData limitedDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> attributesFromNetModules, ImmutableArray<string> netModuleNames, QuickAttributes attributeMatches)
            {
                int length = attributesFromNetModules.Length;
                HashSet<CSharpAttributeData> uniqueAttributes = null;
                CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> result2 = null;
                for (int num = length - 1; num >= 0; num--)
                {
                    CSharpAttributeData cSharpAttributeData = attributesFromNetModules[num];
                    if (!cSharpAttributeData.HasErrors && ValidateAttributeUsageForNetModuleAttribute(cSharpAttributeData, netModuleNames[num], BindingDiagnosticBag.Discarded, ref uniqueAttributes))
                    {
                        limitedDecodeWellKnownAttribute(cSharpAttributeData, attributeMatches, ref result2);
                    }
                }
                return result2;
            }
        }

        private CustomAttributesBag<CSharpAttributeData> GetNetModuleAttributesBag()
        {
            if (_lazyNetModuleAttributesBag == null)
            {
                LoadAndValidateNetModuleAttributes(ref _lazyNetModuleAttributesBag);
            }
            return _lazyNetModuleAttributesBag;
        }

        internal CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> GetNetModuleDecodedWellKnownAttributeData()
        {
            return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)GetNetModuleAttributesBag().DecodedWellKnownAttributeData;
        }

        internal ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            ArrayBuilder<SyntaxList<AttributeListSyntax>> instance = ArrayBuilder<SyntaxList<AttributeListSyntax>>.GetInstance();
            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = DeclaringCompilation.MergedRootDeclaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RootSingleNamespaceDeclaration rootSingleNamespaceDeclaration = (RootSingleNamespaceDeclaration)enumerator.Current;
                if (rootSingleNamespaceDeclaration.HasAssemblyAttributes)
                {
                    CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)rootSingleNamespaceDeclaration.Location.SourceTree.GetRoot();
                    instance.Add(compilationUnitSyntax.AttributeLists);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private void EnsureAttributesAreBound()
        {
            if ((_lazySourceAttributesBag == null || !_lazySourceAttributesBag.IsSealed) && LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref _lazySourceAttributesBag))
            {
                _state.NotePartComplete(CompletionPart.Attributes);
            }
        }

        private CustomAttributesBag<CSharpAttributeData> GetSourceAttributesBag()
        {
            EnsureAttributesAreBound();
            return _lazySourceAttributesBag;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            ImmutableArray<CSharpAttributeData> immutableArray = GetSourceAttributesBag().Attributes;
            ImmutableArray<CSharpAttributeData> attributes = GetNetModuleAttributesBag().Attributes;
            if (immutableArray.Length > 0)
            {
                if (attributes.Length > 0)
                {
                    immutableArray = immutableArray.Concat(attributes);
                }
            }
            else
            {
                immutableArray = attributes;
            }
            return immutableArray;
        }

        internal bool IsIndexOfOmittedAssemblyAttribute(int index)
        {
            if (_lazyOmittedAttributeIndices != null)
            {
                return _lazyOmittedAttributeIndices.Contains(index);
            }
            return false;
        }

        internal CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> GetSourceDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazySourceAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetSourceAttributesBag();
            }
            return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)customAttributesBag.DecodedWellKnownAttributeData;
        }

        private CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>? GetSourceDecodedWellKnownAttributeData(QuickAttributes attribute)
        {
            CustomAttributesBag<CSharpAttributeData> lazySourceAttributesBag = _lazySourceAttributesBag;
            if (lazySourceAttributesBag != null && lazySourceAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)lazySourceAttributesBag.DecodedWellKnownAttributeData;
            }
            lazySourceAttributesBag = null;
            Func<AttributeSyntax, bool> attributeMatchesOpt = attribute switch
            {
                QuickAttributes.AssemblySignatureKey => isPossibleAssemblySignatureKeyAttribute,
                QuickAttributes.AssemblyKeyName => isPossibleAssemblyKeyNameAttribute,
                QuickAttributes.AssemblyKeyFile => isPossibleAssemblyKeyFileAttribute,
                _ => throw ExceptionUtilities.UnexpectedValue(attribute),
            };
            LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref lazySourceAttributesBag, AttributeLocation.None, earlyDecodingOnly: false, null, attributeMatchesOpt);
            return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)(lazySourceAttributesBag?.DecodedWellKnownAttributeData);
            bool isPossibleAssemblyKeyFileAttribute(AttributeSyntax node)
            {
                return DeclaringCompilation.GetBinderFactory(node.SyntaxTree).GetBinder(node).QuickAttributeChecker.IsPossibleMatch(node, QuickAttributes.AssemblyKeyFile);
            }
            bool isPossibleAssemblyKeyNameAttribute(AttributeSyntax node)
            {
                return DeclaringCompilation.GetBinderFactory(node.SyntaxTree).GetBinder(node).QuickAttributeChecker.IsPossibleMatch(node, QuickAttributes.AssemblyKeyName);
            }
            bool isPossibleAssemblySignatureKeyAttribute(AttributeSyntax node)
            {
                return DeclaringCompilation.GetBinderFactory(node.SyntaxTree).GetBinder(node).QuickAttributeChecker.IsPossibleMatch(node, QuickAttributes.AssemblySignatureKey);
            }
        }

        internal HashSet<NamedTypeSymbol> GetForwardedTypes()
        {
            CustomAttributesBag<CSharpAttributeData> lazySourceAttributesBag = _lazySourceAttributesBag;
            if (lazySourceAttributesBag != null && lazySourceAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                return ((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)lazySourceAttributesBag.DecodedWellKnownAttributeData)?.ForwardedTypes;
            }
            lazySourceAttributesBag = null;
            LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref lazySourceAttributesBag, AttributeLocation.None, earlyDecodingOnly: false, null, IsPossibleForwardedTypesAttribute);
            return ((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)(lazySourceAttributesBag?.DecodedWellKnownAttributeData))?.ForwardedTypes;
        }

        private bool IsPossibleForwardedTypesAttribute(AttributeSyntax node)
        {
            return DeclaringCompilation.GetBinderFactory(node.SyntaxTree).GetBinder(node).QuickAttributeChecker.IsPossibleMatch(node, QuickAttributes.TypeForwardedTo);
        }

        private static IEnumerable<SecurityAttribute> GetSecurityAttributes(CustomAttributesBag<CSharpAttributeData> attributesBag)
        {
            CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> commonAssemblyWellKnownAttributeData = (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)attributesBag.DecodedWellKnownAttributeData;
            if (commonAssemblyWellKnownAttributeData == null)
            {
                yield break;
            }
            SecurityWellKnownAttributeData securityInformation = commonAssemblyWellKnownAttributeData.SecurityInformation;
            if (securityInformation == null)
            {
                yield break;
            }
            foreach (SecurityAttribute securityAttribute in securityInformation.GetSecurityAttributes(attributesBag.Attributes))
            {
                yield return securityAttribute;
            }
        }

        internal IEnumerable<SecurityAttribute> GetSecurityAttributes()
        {
            foreach (SecurityAttribute securityAttribute in GetSecurityAttributes(GetSourceAttributesBag()))
            {
                yield return securityAttribute;
            }
            foreach (SecurityAttribute securityAttribute2 in GetSecurityAttributes(GetNetModuleAttributesBag()))
            {
                yield return securityAttribute2;
            }
            if (!_compilation.Options.AllowUnsafe || _compilation.GetWellKnownType(WellKnownType.System_Security_UnverifiableCodeAttribute) is MissingMetadataTypeSymbol || _compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityPermissionAttribute) is MissingMetadataTypeSymbol)
            {
                yield break;
            }
            NamedTypeSymbol wellKnownType = _compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityAction);
            if (!(wellKnownType is MissingMetadataTypeSymbol))
            {
                FieldSymbol fieldSymbol = (FieldSymbol)_compilation.GetWellKnownTypeMember(WellKnownMember.System_Security_Permissions_SecurityAction__RequestMinimum);
                object obj = (((object)fieldSymbol == null || fieldSymbol.HasUseSiteError) ? 0 : fieldSymbol.ConstantValue);
                TypedConstant item = new TypedConstant(wellKnownType, TypedConstantKind.Enum, obj);
                NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                TypedConstant value = new TypedConstant(specialType, TypedConstantKind.Primitive, true);
                SynthesizedAttributeData synthesizedAttributeData = _compilation.TrySynthesizeAttribute(WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__ctor, ImmutableArray.Create(item), ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_Security_Permissions_SecurityPermissionAttribute__SkipVerification, value)));
                if (synthesizedAttributeData != null)
                {
                    yield return new SecurityAttribute((DeclarativeSecurityAction)(int)obj, synthesizedAttributeData);
                }
            }
        }

        internal override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies()
        {
            return _modules[0].GetReferencedAssemblySymbols();
        }

        internal override void SetNoPiaResolutionAssemblies(ImmutableArray<AssemblySymbol> assemblies)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies()
        {
            return default(ImmutableArray<AssemblySymbol>);
        }

        internal override void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            bool num = _compilation.Options.OutputKind.IsNetModule();
            if (ContainsExtensionMethods())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor));
            }
            if (!num && !Modules.Any((ModuleSymbol m) => m.HasAssemblyCompilationRelaxationsAttribute) && !(_compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) is MissingMetadataTypeSymbol))
            {
                NamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Int32);
                TypedConstant item = new TypedConstant(specialType, TypedConstantKind.Primitive, 8);
                Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, ImmutableArray.Create(item)));
            }
            if (!num && !Modules.Any((ModuleSymbol m) => m.HasAssemblyRuntimeCompatibilityAttribute) && !(_compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) is MissingMetadataTypeSymbol))
            {
                NamedTypeSymbol specialType2 = _compilation.GetSpecialType(SpecialType.System_Boolean);
                TypedConstant value = new TypedConstant(specialType2, TypedConstantKind.Primitive, true);
                Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, ImmutableArray<TypedConstant>.Empty, ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, value))));
            }
            if (!num && !HasDebuggableAttribute)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, _compilation.SynthesizeDebuggableAttribute());
            }
            if (_compilation.Options.OutputKind == OutputKind.NetModule)
            {
                if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyContainer) && (object)AssemblyKeyContainerAttributeSetting == WellKnownAttributeData.StringMissingValue)
                {
                    NamedTypeSymbol specialType3 = _compilation.GetSpecialType(SpecialType.System_String);
                    TypedConstant item2 = new TypedConstant(specialType3, TypedConstantKind.Primitive, _compilation.Options.CryptoKeyContainer);
                    Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, ImmutableArray.Create(item2)));
                }
                if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyFile) && (object)AssemblyKeyFileAttributeSetting == WellKnownAttributeData.StringMissingValue)
                {
                    NamedTypeSymbol specialType4 = _compilation.GetSpecialType(SpecialType.System_String);
                    TypedConstant item3 = new TypedConstant(specialType4, TypedConstantKind.Primitive, _compilation.Options.CryptoKeyFile);
                    Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, ImmutableArray.Create(item3)));
                }
            }
        }

        private bool ContainsExtensionMethods()
        {
            if (!_lazyContainsExtensionMethods.HasValue())
            {
                _lazyContainsExtensionMethods = ContainsExtensionMethods(_modules).ToThreeState();
            }
            return _lazyContainsExtensionMethods.Value();
        }

        private static bool ContainsExtensionMethods(ImmutableArray<ModuleSymbol> modules)
        {
            ImmutableArray<ModuleSymbol>.Enumerator enumerator = modules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (ContainsExtensionMethods(enumerator.Current.GlobalNamespace))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsExtensionMethods(NamespaceSymbol ns)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = ns.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                switch (current.Kind)
                {
                    case SymbolKind.Namespace:
                        if (ContainsExtensionMethods((NamespaceSymbol)current))
                        {
                            return true;
                        }
                        break;
                    case SymbolKind.NamedType:
                        if (((NamedTypeSymbol)current).MightContainExtensionMethods)
                        {
                            return true;
                        }
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(current.Kind);
                }
            }
            return false;
        }

        private void CheckOptimisticIVTAccessGrants(BindingDiagnosticBag bag)
        {
            ConcurrentDictionary<AssemblySymbol, bool> optimisticallyGrantedInternalsAccess = _optimisticallyGrantedInternalsAccess;
            if (optimisticallyGrantedInternalsAccess == null)
            {
                return;
            }
            foreach (AssemblySymbol key in optimisticallyGrantedInternalsAccess.Keys)
            {
                switch (MakeFinalIVTDetermination(key))
                {
                    case IVTConclusion.PublicKeyDoesntMatch:
                        bag.Add(ErrorCode.ERR_FriendRefNotEqualToThis, NoLocation.Singleton, key.Identity, Identity);
                        break;
                    case IVTConclusion.OneSignedOneNot:
                        bag.Add(ErrorCode.ERR_FriendRefSigningMismatch, NoLocation.Singleton, key.Identity);
                        break;
                }
            }
        }

        internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
        {
            EnsureAttributesAreBound();
            if (_lazyInternalsVisibleToMap == null)
            {
                return SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
            }
            _lazyInternalsVisibleToMap.TryGetValue(simpleName, out ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> value);
            if (value == null)
            {
                return SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
            }
            return value.Keys;
        }

        internal override bool AreInternalsVisibleToThisAssembly(AssemblySymbol potentialGiverOfAccess)
        {
            if (_lazyStrongNameKeys == null && (object)t_assemblyForWhichCurrentThreadIsComputingKeys != null)
            {
                if (!potentialGiverOfAccess.GetInternalsVisibleToPublicKeys(Name).IsEmpty())
                {
                    if (_optimisticallyGrantedInternalsAccess == null)
                    {
                        Interlocked.CompareExchange(ref _optimisticallyGrantedInternalsAccess, new ConcurrentDictionary<AssemblySymbol, bool>(), null);
                    }
                    _optimisticallyGrantedInternalsAccess.TryAdd(potentialGiverOfAccess, value: true);
                    return true;
                }
                return false;
            }
            IVTConclusion iVTConclusion = MakeFinalIVTDetermination(potentialGiverOfAccess);
            if (iVTConclusion != 0)
            {
                return iVTConclusion == IVTConclusion.OneSignedOneNot;
            }
            return true;
        }

        private AssemblyIdentity ComputeIdentity()
        {
            return new AssemblyIdentity(_assemblySimpleName, VersionHelper.GenerateVersionFromPatternAndCurrentTime(_compilation.Options.CurrentLocalTime, AssemblyVersionAttributeSetting), AssemblyCultureAttributeSetting, StrongNameKeys.PublicKey, !StrongNameKeys.PublicKey.IsDefault);
        }

        private static Location GetAssemblyAttributeLocationForDiagnostic(AttributeSyntax attributeSyntaxOpt)
        {
            if (attributeSyntaxOpt == null)
            {
                return NoLocation.Singleton;
            }
            return attributeSyntaxOpt.Location;
        }

        private void DecodeTypeForwardedToAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            TypeSymbol typeSymbol = (TypeSymbol)arguments.Attribute.CommonConstructorArguments[0].ValueInternal;
            if ((object)typeSymbol == null)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidFwdType, GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt));
                return;
            }
            UseSiteInfo<AssemblySymbol> useSiteInfo = typeSymbol.GetUseSiteInfo();
            DiagnosticInfo? diagnosticInfo = useSiteInfo.DiagnosticInfo;
            if ((diagnosticInfo == null || diagnosticInfo!.Code != 7003) && bindingDiagnosticBag.Add(useSiteInfo, (useSiteInfo.DiagnosticInfo != null) ? GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt) : Location.None))
            {
                return;
            }
            if (typeSymbol.ContainingAssembly == this)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ForwardedTypeInThisAssembly, GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt), typeSymbol);
                return;
            }
            if ((object)typeSymbol.ContainingType != null)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ForwardedTypeIsNested, GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt), typeSymbol, typeSymbol.ContainingType);
                return;
            }
            if (typeSymbol.Kind != SymbolKind.NamedType)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidFwdType, GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt));
                return;
            }
            CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> orCreateData = arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>();
            HashSet<NamedTypeSymbol> forwardedTypes = orCreateData.ForwardedTypes;
            if (forwardedTypes == null)
            {
                forwardedTypes = (orCreateData.ForwardedTypes = new HashSet<NamedTypeSymbol> { (NamedTypeSymbol)typeSymbol });
            }
            else if (!forwardedTypes.Add((NamedTypeSymbol)typeSymbol))
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_DuplicateTypeForwarder, GetAssemblyAttributeLocationForDiagnostic(arguments.AttributeSyntaxOpt), typeSymbol);
            }
        }

        private void DecodeOneInternalsVisibleToAttribute(AttributeSyntax nodeOpt, CSharpAttributeData attrData, BindingDiagnosticBag diagnostics, int index, ref ConcurrentDictionary<string, ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>> lazyInternalsVisibleToMap)
        {
            string text = (string)attrData.CommonConstructorArguments[0].ValueInternal;
            if (text == null)
            {
                diagnostics.Add(ErrorCode.ERR_CannotPassNullForFriendAssembly, GetAssemblyAttributeLocationForDiagnostic(nodeOpt));
                return;
            }
            if (!AssemblyIdentity.TryParseDisplayName(text, out var identity, out var parts))
            {
                diagnostics.Add(ErrorCode.WRN_InvalidAssemblyName, GetAssemblyAttributeLocationForDiagnostic(nodeOpt), text);
                AddOmittedAttributeIndex(index);
                return;
            }
            if (((uint)parts & 0xFFFFFF3Eu) != 0)
            {
                diagnostics.Add(ErrorCode.ERR_FriendAssemblyBadArgs, GetAssemblyAttributeLocationForDiagnostic(nodeOpt), text);
                return;
            }
            if (lazyInternalsVisibleToMap == null)
            {
                Interlocked.CompareExchange(ref lazyInternalsVisibleToMap, new ConcurrentDictionary<string, ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>>(StringComparer.OrdinalIgnoreCase), null);
            }
            Tuple<Location, string> value = null;
            if (identity.PublicKey.IsEmpty)
            {
                value = new Tuple<Location, string>(GetAssemblyAttributeLocationForDiagnostic(nodeOpt), text);
            }
            if (lazyInternalsVisibleToMap.TryGetValue(identity.Name, out ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> value2))
            {
                value2.TryAdd(identity.PublicKey, value);
                return;
            }
            value2 = new ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>();
            value2.TryAdd(identity.PublicKey, value);
            lazyInternalsVisibleToMap.TryAdd(identity.Name, value2);
        }

        internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            DecodeWellKnownAttribute(ref arguments, arguments.Index, isFromNetModule: false);
        }

        private void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments, int index, bool isFromNetModule)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            int targetAttributeSignatureIndex;
            if (attribute.IsTargetAttribute(this, AttributeDescription.InternalsVisibleToAttribute))
            {
                DecodeOneInternalsVisibleToAttribute(arguments.AttributeSyntaxOpt, attribute, bindingDiagnosticBag, index, ref _lazyInternalsVisibleToMap);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblySignatureKeyAttribute))
            {
                string text = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblySignatureKeyAttributeSetting = text;
                if (!StrongNameKeys.IsValidPublicKeyString(text))
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidSignaturePublicKey, attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt));
                }
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyFileAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyKeyFileAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyNameAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyKeyContainerAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDelaySignAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyDelaySignAttributeSetting = ((!(bool)attribute.CommonConstructorArguments[0].ValueInternal) ? ThreeState.False : ThreeState.True);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyVersionAttribute))
            {
                string text2 = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                if (!VersionHelper.TryParseAssemblyVersion(text2, !_compilation.IsEmitDeterministic, out var version))
                {
                    Location attributeArgumentSyntaxLocation = attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt);
                    bool flag = _compilation.IsEmitDeterministic && (text2?.Contains('*') ?? false);
                    bindingDiagnosticBag.Add(flag ? ErrorCode.ERR_InvalidVersionFormatDeterministic : ErrorCode.ERR_InvalidVersionFormat, attributeArgumentSyntaxLocation);
                }
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyVersionAttributeSetting = version;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFileVersionAttribute))
            {
                string text3 = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                if (!VersionHelper.TryParse(text3, out var _))
                {
                    Location attributeArgumentSyntaxLocation2 = attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt);
                    bindingDiagnosticBag.Add(ErrorCode.WRN_InvalidVersionFormat, attributeArgumentSyntaxLocation2);
                }
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyFileVersionAttributeSetting = text3;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTitleAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyTitleAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDescriptionAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyDescriptionAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCultureAttribute))
            {
                string text4 = (string)attribute.CommonConstructorArguments[0].ValueInternal;
                if (!string.IsNullOrEmpty(text4))
                {
                    if (_compilation.Options.OutputKind.IsApplication())
                    {
                        bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidAssemblyCultureForExe, attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt));
                    }
                    else if (!AssemblyIdentity.IsValidCultureName(text4))
                    {
                        bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidAssemblyCulture, attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt));
                        text4 = null;
                    }
                }
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyCultureAttributeSetting = text4;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCompanyAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyCompanyAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyProductAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyProductAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyInformationalVersionAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyInformationalVersionAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SatelliteContractVersionAttribute))
            {
                if (!VersionHelper.TryParseAssemblyVersion((string)attribute.CommonConstructorArguments[0].ValueInternal, allowWildcard: false, out var _))
                {
                    Location attributeArgumentSyntaxLocation3 = attribute.GetAttributeArgumentSyntaxLocation(0, arguments.AttributeSyntaxOpt);
                    bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidVersionFormat2, attributeArgumentSyntaxLocation3);
                }
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCopyrightAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyCopyrightAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTrademarkAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyTrademarkAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
            }
            else if ((targetAttributeSignatureIndex = attribute.GetTargetAttributeSignatureIndex(this, AttributeDescription.AssemblyFlagsAttribute)) != -1)
            {
                object valueInternal = attribute.CommonConstructorArguments[0].ValueInternal;
                AssemblyFlags assemblyFlagsAttributeSetting = ((targetAttributeSignatureIndex != 0 && targetAttributeSignatureIndex != 1) ? ((AssemblyFlags)(uint)valueInternal) : ((AssemblyFlags)(AssemblyNameFlags)valueInternal));
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyFlagsAttributeSetting = assemblyFlagsAttributeSetting;
            }
            else if (attribute.IsSecurityAttribute(_compilation))
            {
                attribute.DecodeSecurityAttribute<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>(this, _compilation, ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ClassInterfaceAttribute))
            {
                attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.TypeLibVersionAttribute))
            {
                ValidateIntegralAttributeNonNegativeArguments(attribute, arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ComCompatibleVersionAttribute))
            {
                ValidateIntegralAttributeNonNegativeArguments(attribute, arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.GuidAttribute))
            {
                attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CompilationRelaxationsAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().HasCompilationRelaxationsAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ReferenceAssemblyAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().HasReferenceAssemblyAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.RuntimeCompatibilityAttribute))
            {
                bool runtimeCompatibilityWrapNonExceptionThrows = true;
                ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = attribute.CommonNamedArguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, TypedConstant> current = enumerator.Current;
                    if (current.Key == "WrapNonExceptionThrows")
                    {
                        runtimeCompatibilityWrapNonExceptionThrows = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                    }
                }
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().RuntimeCompatibilityWrapNonExceptionThrows = runtimeCompatibilityWrapNonExceptionThrows;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DebuggableAttribute))
            {
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().HasDebuggableAttribute = true;
            }
            else if (!isFromNetModule && attribute.IsTargetAttribute(this, AttributeDescription.TypeForwardedToAttribute))
            {
                DecodeTypeForwardedToAttribute(ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CaseSensitiveExtensionAttribute))
            {
                if (arguments.AttributeSyntaxOpt != null)
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_ExplicitExtension, arguments.AttributeSyntaxOpt!.Location);
                }
            }
            else if ((targetAttributeSignatureIndex = attribute.GetTargetAttributeSignatureIndex(this, AttributeDescription.AssemblyAlgorithmIdAttribute)) != -1)
            {
                object valueInternal2 = attribute.CommonConstructorArguments[0].ValueInternal;
                AssemblyHashAlgorithm value = ((targetAttributeSignatureIndex != 0) ? ((AssemblyHashAlgorithm)(uint)valueInternal2) : ((AssemblyHashAlgorithm)valueInternal2));
                arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyAlgorithmIdAttributeSetting = value;
            }
        }

        private static void ValidateIntegralAttributeNonNegativeArguments(CSharpAttributeData attribute, AttributeSyntax nodeOpt, BindingDiagnosticBag diagnostics)
        {
            int length = attribute.CommonConstructorArguments.Length;
            for (int i = 0; i < length; i++)
            {
                if (attribute.GetConstructorArgument<int>(i, SpecialType.System_Int32) < 0)
                {
                    Location attributeArgumentSyntaxLocation = attribute.GetAttributeArgumentSyntaxLocation(i, nodeOpt);
                    diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntaxLocation, (nodeOpt != null) ? nodeOpt.GetErrorDisplayName() : "");
                }
            }
        }

        internal void NoteFieldAccess(FieldSymbol field, bool read, bool write)
        {
            if (!(field.ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol))
            {
                return;
            }
            sourceMemberContainerTypeSymbol.EnsureFieldDefinitionsNoted();
            if (_unusedFieldWarnings.IsDefault)
            {
                if (read)
                {
                    _unreadFields.Remove(field);
                }
                if (write)
                {
                    _unassignedFieldsMap.TryRemove(field, out var _);
                }
            }
        }

        internal void NoteFieldDefinition(FieldSymbol field, bool isInternal, bool isUnread)
        {
            _unassignedFieldsMap.TryAdd(field, isInternal);
            if (isUnread)
            {
                _unreadFields.Add(field);
            }
        }

        internal override bool IsNetModule()
        {
            return _compilation.Options.OutputKind.IsNetModule();
        }

        internal ImmutableArray<Diagnostic> GetUnusedFieldWarnings(CancellationToken cancellationToken)
        {
            if (_unusedFieldWarnings.IsDefault)
            {
                ForceComplete(null, cancellationToken);
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                bool flag = InternalsAreVisible || IsNetModule();
                HashSet<FieldSymbol> hashSet = null;
                foreach (FieldSymbol key in _unassignedFieldsMap.Keys)
                {
                    _unassignedFieldsMap.TryGetValue(key, out var value);
                    if ((value && flag) || !key.CanBeReferencedByName || !(key.ContainingType is SourceNamedTypeSymbol sourceNamedTypeSymbol) || key is TupleErrorFieldSymbol)
                    {
                        continue;
                    }
                    bool flag2 = _unreadFields.Contains(key);
                    if (flag2)
                    {
                        if (hashSet == null)
                        {
                            hashSet = new HashSet<FieldSymbol>();
                        }
                        hashSet.Add(key);
                    }
                    if (sourceNamedTypeSymbol.HasStructLayoutAttribute)
                    {
                        continue;
                    }
                    Symbol associatedSymbol = key.AssociatedSymbol;
                    if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Event)
                    {
                        if (flag2)
                        {
                            instance.Add(ErrorCode.WRN_UnreferencedEvent, associatedSymbol.Locations.FirstOrNone(), associatedSymbol);
                        }
                    }
                    else if (flag2)
                    {
                        instance.Add(ErrorCode.WRN_UnreferencedField, key.Locations.FirstOrNone(), key);
                    }
                    else
                    {
                        instance.Add(ErrorCode.WRN_UnassignedInternalField, key.Locations.FirstOrNone(), key, DefaultValue(key.Type));
                    }
                }
                ConcurrentSet<FieldSymbol>.KeyEnumerator enumerator2 = _unreadFields.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    FieldSymbol current2 = enumerator2.Current;
                    if ((hashSet == null || !hashSet.Contains(current2)) && current2.CanBeReferencedByName && current2.ContainingType is SourceNamedTypeSymbol sourceNamedTypeSymbol2 && !sourceNamedTypeSymbol2.HasStructLayoutAttribute)
                    {
                        instance.Add(ErrorCode.WRN_UnreferencedFieldAssg, current2.Locations.FirstOrNone(), current2);
                    }
                }
                ImmutableInterlocked.InterlockedInitialize(ref _unusedFieldWarnings, instance.ToReadOnlyAndFree());
            }
            return _unusedFieldWarnings;
        }

        private static string DefaultValue(TypeSymbol type)
        {
            if (type.IsReferenceType)
            {
                return "null";
            }
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return "false";
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
                    return "0";
                default:
                    return "";
            }
        }

        internal override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies)
        {
            int num = emittedName.ForcedArity;
            if (emittedName.UseCLSCompliantNameArityEncoding)
            {
                if (num == -1)
                {
                    num = emittedName.InferredArity;
                }
                else if (num != emittedName.InferredArity)
                {
                    return null;
                }
            }
            if (_lazyForwardedTypesFromSource == null)
            {
                HashSet<NamedTypeSymbol> forwardedTypes = GetForwardedTypes();
                IDictionary<string, NamedTypeSymbol> dictionary;
                if (forwardedTypes != null)
                {
                    dictionary = new Dictionary<string, NamedTypeSymbol>(StringOrdinalComparer.Instance);
                    foreach (NamedTypeSymbol item in forwardedTypes)
                    {
                        NamedTypeSymbol originalDefinition = item.OriginalDefinition;
                        string key = MetadataHelpers.BuildQualifiedName(originalDefinition.ContainingSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), originalDefinition.MetadataName);
                        dictionary[key] = originalDefinition;
                    }
                }
                else
                {
                    dictionary = SpecializedCollections.EmptyDictionary<string, NamedTypeSymbol>();
                }
                _lazyForwardedTypesFromSource = dictionary;
            }
            if (_lazyForwardedTypesFromSource.TryGetValue(emittedName.FullName, out var value))
            {
                if ((num == -1 || value.Arity == num) && (!emittedName.UseCLSCompliantNameArityEncoding || value.Arity == 0 || value.MangleName))
                {
                    return value;
                }
            }
            else if (!_compilation.Options.OutputKind.IsNetModule())
            {
                for (int num2 = _modules.Length - 1; num2 > 0; num2--)
                {
                    PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)_modules[num2];
                    var (assemblySymbol, assemblySymbol2) = pEModuleSymbol.GetAssembliesForForwardedType(ref emittedName);
                    if ((object)assemblySymbol != null)
                    {
                        if ((object)assemblySymbol2 != null)
                        {
                            return CreateMultipleForwardingErrorTypeSymbol(ref emittedName, pEModuleSymbol, assemblySymbol, assemblySymbol2);
                        }
                        if (visitedAssemblies != null && visitedAssemblies.Contains(assemblySymbol))
                        {
                            return CreateCycleInTypeForwarderErrorTypeSymbol(ref emittedName);
                        }
                        visitedAssemblies = new ConsList<AssemblySymbol>(this, visitedAssemblies ?? ConsList<AssemblySymbol>.Empty);
                        return assemblySymbol.LookupTopLevelMetadataTypeWithCycleDetection(ref emittedName, visitedAssemblies, digThroughForwardedTypes: true);
                    }
                }
            }
            return null;
        }

        internal override IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes()
        {
            return PEModuleBuilder.GetForwardedTypes(this, null);
        }

        public override AssemblyMetadata GetMetadata()
        {
            return null;
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.SourceAssemblySymbol(this);
        }
    }
}
