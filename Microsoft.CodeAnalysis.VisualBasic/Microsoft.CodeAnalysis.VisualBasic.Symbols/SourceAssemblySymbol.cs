using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceAssemblySymbol : MetadataOrSourceAssemblySymbol, ISourceAssemblySymbol, ISourceAssemblySymbolInternal, IAttributeTargetSymbol
	{
		private readonly VisualBasicCompilation _compilation;

		private StrongNameKeys _lazyStrongNameKeys;

		internal AssemblyIdentity m_lazyIdentity;

		private readonly ImmutableArray<ModuleSymbol> _modules;

		private CustomAttributesBag<VisualBasicAttributeData> _lazySourceAttributesBag;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyNetModuleAttributesBag;

		private HashSet<int> _lazyDuplicateAttributeIndices;

		private byte _lazyEmitExtensionAttribute;

		private ThreeState _lazyIsVbRuntime;

		private ImmutableArray<Diagnostic> _lazyAssemblyLevelDeclarationErrors;

		private ImmutableArray<AssemblySymbol> _lazyAssemblyLevelDeclarationDependencies;

		private readonly string _assemblySimpleName;

		private ConcurrentDictionary<string, ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>> _lazyInternalsVisibleToMap;

		[ThreadStatic]
		private static AssemblySymbol s_AssemblyForWhichCurrentThreadIsComputingKeys;

		private ConcurrentDictionary<AssemblySymbol, bool> _optimisticallyGrantedInternalsAccess;

		internal override VisualBasicCompilation DeclaringCompilation => _compilation;

		public override bool IsInteractive => _compilation.IsSubmission;

		public override AssemblyIdentity Identity
		{
			get
			{
				if ((object)m_lazyIdentity == null)
				{
					Interlocked.CompareExchange(ref m_lazyIdentity, ComputeIdentity(), null);
				}
				return m_lazyIdentity;
			}
		}

		public override string Name => _assemblySimpleName;

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Assembly;

		internal string FileVersion => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyFileVersionAttributeSetting);

		internal string Title => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyTitleAttributeSetting);

		internal string Description => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyDescriptionAttributeSetting);

		internal string Company => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCompanyAttributeSetting);

		internal string Product => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyProductAttributeSetting);

		internal string InformationalVersion => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyInformationalVersionAttributeSetting);

		internal string Copyright => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCopyrightAttributeSetting);

		internal string Trademark => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyTrademarkAttributeSetting);

		public AssemblyFlags AssemblyFlags
		{
			get
			{
				AssemblyFlags assemblyFlags = (AssemblyFlags)0;
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

		private bool DelaySignAttributeSetting
		{
			get
			{
				bool flag = false;
				bool flag2 = flag;
				CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
				if (sourceDecodedWellKnownAttributeData != null)
				{
					flag2 = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting == ThreeState.True;
				}
				if (flag2 == flag)
				{
					sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
					if (sourceDecodedWellKnownAttributeData != null)
					{
						flag2 = sourceDecodedWellKnownAttributeData.AssemblyDelaySignAttributeSetting == ThreeState.True;
					}
				}
				return flag2;
			}
		}

		public string SignatureKey => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblySignatureKeyAttributeSetting);

		private string AssemblyKeyContainerAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyKeyContainerAttributeSetting, WellKnownAttributeData.StringMissingValue);

		private string AssemblyKeyFileAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyKeyFileAttributeSetting, WellKnownAttributeData.StringMissingValue);

		private string AssemblyCultureAttributeSetting => GetWellKnownAttributeDataStringField((CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> data) => data.AssemblyCultureAttributeSetting);

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
				if ((object)version2 == version)
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

		public AssemblyHashAlgorithm HashAlgorithm
		{
			get
			{
				AssemblyHashAlgorithm? assemblyAlgorithmIdAttributeSetting;
				AssemblyHashAlgorithm? assemblyHashAlgorithm = (assemblyAlgorithmIdAttributeSetting = AssemblyAlgorithmIdAttributeSetting);
				if (!assemblyHashAlgorithm.HasValue)
				{
					return AssemblyHashAlgorithm.Sha1;
				}
				return assemblyAlgorithmIdAttributeSetting.GetValueOrDefault();
			}
		}

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

		public override ImmutableArray<Location> Locations => Modules.SelectMany((ModuleSymbol m) => m.Locations).AsImmutable();

		public override ImmutableArray<ModuleSymbol> Modules => _modules;

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
				return !TypeSymbolExtensions.IsErrorType(specialType) && specialType.DeclaredAccessibility == Accessibility.Public;
			}
		}

		internal SourceModuleSymbol SourceModule => (SourceModuleSymbol)_modules[0];

		public override bool MightContainExtensionMethods => SourceModule.MightContainExtensionMethods;

		internal bool IsDelaySigned
		{
			get
			{
				EnsureAttributesAreBound();
				if (DeclaringCompilation.Options.DelaySign.HasValue)
				{
					return DeclaringCompilation.Options.DelaySign.Value;
				}
				return DelaySignAttributeSetting;
			}
		}

		public bool InternalsAreVisible
		{
			get
			{
				EnsureAttributesAreBound();
				return _lazyInternalsVisibleToMap != null;
			}
		}

		private bool HasAssemblyOrModuleDebuggableAttribute
		{
			get
			{
				CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
				if (sourceDecodedWellKnownAttributeData != null && sourceDecodedWellKnownAttributeData.HasDebuggableAttribute)
				{
					return true;
				}
				CommonModuleWellKnownAttributeData decodedWellKnownAttributeData = SourceModule.GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData != null && decodedWellKnownAttributeData.HasDebuggableAttribute)
				{
					return true;
				}
				return false;
			}
		}

		private bool HasReferenceAssemblyAttribute => GetSourceDecodedWellKnownAttributeData()?.HasReferenceAssemblyAttribute ?? false;

		internal override ImmutableArray<byte> PublicKey => StrongNameKeys.PublicKey;

		internal StrongNameKeys StrongNameKeys
		{
			get
			{
				if (_lazyStrongNameKeys == null)
				{
					try
					{
						s_AssemblyForWhichCurrentThreadIsComputingKeys = this;
						ComputeAndSetStrongNameKeys();
					}
					finally
					{
						s_AssemblyForWhichCurrentThreadIsComputingKeys = null;
					}
				}
				return _lazyStrongNameKeys;
			}
		}

		internal bool IsVbRuntime
		{
			get
			{
				if (_lazyIsVbRuntime == ThreeState.Unknown)
				{
					_lazyIsVbRuntime = CheckForRuntime().ToThreeState();
				}
				return _lazyIsVbRuntime == ThreeState.True;
			}
		}

		private Compilation ISourceAssemblySymbol_Compilation => _compilation;

		internal IEnumerable<VisualBasicAttributeData> GetAssemblyCustomAttributesToEmit(ModuleCompilationState compilationState, bool emittingRefAssembly, bool emittingAssemblyAttributesInNetModule)
		{
			ArrayBuilder<SynthesizedAttributeData> attributes = null;
			AddSynthesizedAttributes(compilationState, ref attributes);
			if (emittingRefAssembly && !HasReferenceAssemblyAttribute)
			{
				SynthesizedAttributeData attribute = DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor, default(ImmutableArray<TypedConstant>), default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true);
				Symbol.AddSynthesizedAttribute(ref attributes, attribute);
			}
			return GetCustomAttributesToEmit(GetAttributes(), attributes, isReturnType: false, emittingAssemblyAttributesInNetModule);
		}

		internal SourceAssemblySymbol(VisualBasicCompilation compilation, string assemblySimpleName, string moduleName, ImmutableArray<PEModule> netModules)
		{
			_lazyEmitExtensionAttribute = 0;
			_lazyIsVbRuntime = ThreeState.Unknown;
			_compilation = compilation;
			_assemblySimpleName = assemblySimpleName;
			ArrayBuilder<ModuleSymbol> arrayBuilder = new ArrayBuilder<ModuleSymbol>(1 + netModules.Length)
			{
				new SourceModuleSymbol(this, compilation.Declarations, compilation.Options, moduleName)
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
			int num = _modules.Length - 1;
			for (int i = 1; i <= num; i++)
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
			if (_compilation.IsMemberMissing(member))
			{
				return null;
			}
			return base.GetSpecialTypeMember(member);
		}

		private bool IsKnownAssemblyAttribute(VisualBasicAttributeData attribute)
		{
			if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTitleAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDescriptionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyConfigurationAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCultureAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCompanyAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyProductAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyInformationalVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyCopyrightAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyTrademarkAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyFileAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyKeyNameAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyAlgorithmIdAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFlagsAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyDelaySignAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFileVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.SatelliteContractVersionAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.AssemblySignatureKeyAttribute))
			{
				return true;
			}
			return false;
		}

		private HashSet<VisualBasicAttributeData> GetUniqueSourceAssemblyAttributes(out HashSet<int> attributeIndicesToSkip)
		{
			ImmutableArray<VisualBasicAttributeData> attributes = GetSourceAttributesBag().Attributes;
			HashSet<VisualBasicAttributeData> uniqueAttributes = null;
			attributeIndicesToSkip = null;
			int num = attributes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				VisualBasicAttributeData visualBasicAttributeData = attributes[i];
				if (!visualBasicAttributeData.HasErrors && !AddUniqueAssemblyAttribute(visualBasicAttributeData, ref uniqueAttributes))
				{
					if (attributeIndicesToSkip == null)
					{
						attributeIndicesToSkip = new HashSet<int>();
					}
					attributeIndicesToSkip.Add(i);
				}
			}
			return uniqueAttributes;
		}

		private static bool AddUniqueAssemblyAttribute(VisualBasicAttributeData attribute, ref HashSet<VisualBasicAttributeData> uniqueAttributes)
		{
			if (uniqueAttributes == null)
			{
				uniqueAttributes = new HashSet<VisualBasicAttributeData>(CommonAttributeDataComparer.Instance);
			}
			return uniqueAttributes.Add(attribute);
		}

		private bool ValidateAttributeUsageForNetModuleAttribute(VisualBasicAttributeData attribute, string netModuleName, BindingDiagnosticBag diagnostics, ref HashSet<VisualBasicAttributeData> uniqueAttributes)
		{
			NamedTypeSymbol attributeClass = attribute.AttributeClass;
			if (attributeClass.GetAttributeUsageInfo().AllowMultiple)
			{
				return AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes);
			}
			if (uniqueAttributes == null || !uniqueAttributes.Contains((VisualBasicAttributeData a) => TypeSymbol.Equals(a.AttributeClass, attributeClass, TypeCompareKind.ConsiderEverything)))
			{
				AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes);
				return true;
			}
			if (IsKnownAssemblyAttribute(attribute))
			{
				if (!uniqueAttributes.Contains(attribute))
				{
					diagnostics.Add(ERRID.WRN_AssemblyAttributeFromModuleIsOverridden, NoLocation.Singleton, attribute.AttributeClass, netModuleName);
				}
			}
			else if (AddUniqueAssemblyAttribute(attribute, ref uniqueAttributes))
			{
				diagnostics.Add(ERRID.ERR_InvalidMultipleAttributeUsageInNetModule2, NoLocation.Singleton, attribute.AttributeClass.Name, netModuleName);
			}
			return false;
		}

		private ImmutableArray<VisualBasicAttributeData> GetNetModuleAttributes(out ImmutableArray<string> netModuleNames)
		{
			ArrayBuilder<string> arrayBuilder = null;
			ArrayBuilder<VisualBasicAttributeData> arrayBuilder2 = null;
			int num = _modules.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				PEModuleSymbol obj = (PEModuleSymbol)_modules[i];
				string name = obj.Name;
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = obj.GetAssemblyAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<string>.GetInstance();
						arrayBuilder2 = ArrayBuilder<VisualBasicAttributeData>.GetInstance();
					}
					arrayBuilder.Add(name);
					arrayBuilder2.Add(current);
				}
			}
			if (arrayBuilder == null)
			{
				netModuleNames = ImmutableArray<string>.Empty;
				return ImmutableArray<VisualBasicAttributeData>.Empty;
			}
			netModuleNames = arrayBuilder.ToImmutableAndFree();
			return arrayBuilder2.ToImmutableAndFree();
		}

		private WellKnownAttributeData ValidateAttributeUsageAndDecodeWellKnownNetModuleAttributes(ImmutableArray<VisualBasicAttributeData> attributesFromNetModules, ImmutableArray<string> netModuleNames, BindingDiagnosticBag diagnostics, out HashSet<int> attributeIndicesToSkip)
		{
			VisualBasicSyntaxTree.Dummy.GetRoot();
			BinderBuilder.CreateSourceModuleBinder(SourceModule);
			int length = attributesFromNetModules.Length;
			int length2 = GetSourceAttributesBag().Attributes.Length;
			HashSet<VisualBasicAttributeData> uniqueAttributes = GetUniqueSourceAssemblyAttributes(out attributeIndicesToSkip);
			DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments = default(DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation>);
			arguments.AttributesCount = length;
			arguments.Diagnostics = diagnostics;
			arguments.SymbolPart = AttributeLocation.None;
			for (int i = length - 1; i >= 0; i += -1)
			{
				VisualBasicAttributeData visualBasicAttributeData = attributesFromNetModules[i];
				if (!visualBasicAttributeData.HasErrors && ValidateAttributeUsageForNetModuleAttribute(visualBasicAttributeData, netModuleNames[i], diagnostics, ref uniqueAttributes))
				{
					arguments.Attribute = visualBasicAttributeData;
					arguments.Index = i;
					arguments.AttributeSyntaxOpt = null;
					DecodeWellKnownAttribute(ref arguments);
				}
				else
				{
					if (attributeIndicesToSkip == null)
					{
						attributeIndicesToSkip = new HashSet<int>();
					}
					attributeIndicesToSkip.Add(i + length2);
				}
			}
			if (!arguments.HasDecodedData)
			{
				return null;
			}
			return arguments.DecodedData;
		}

		private void LoadAndValidateNetModuleAttributes(ref CustomAttributesBag<VisualBasicAttributeData> lazyNetModuleAttributesBag)
		{
			HashSet<int> attributeIndicesToSkip = null;
			if (_compilation.Options.OutputKind.IsNetModule())
			{
				GetUniqueSourceAssemblyAttributes(out attributeIndicesToSkip);
				Interlocked.CompareExchange(ref lazyNetModuleAttributesBag, CustomAttributesBag<VisualBasicAttributeData>.Empty, null);
				return;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			ImmutableArray<string> netModuleNames = default(ImmutableArray<string>);
			ImmutableArray<VisualBasicAttributeData> netModuleAttributes = GetNetModuleAttributes(out netModuleNames);
			WellKnownAttributeData wellKnownAttributeData = null;
			if (netModuleAttributes.Any())
			{
				wellKnownAttributeData = ValidateAttributeUsageAndDecodeWellKnownNetModuleAttributes(netModuleAttributes, netModuleNames, instance, out attributeIndicesToSkip);
			}
			else
			{
				GetUniqueSourceAssemblyAttributes(out attributeIndicesToSkip);
			}
			HashSet<NamedTypeSymbol> hashSet = null;
			for (int i = _modules.Length - 1; i >= 1; i += -1)
			{
				PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)_modules[i];
				foreach (NamedTypeSymbol forwardedType in pEModuleSymbol.GetForwardedTypes())
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
					if (hashSet.Add(forwardedType) && TypeSymbolExtensions.IsErrorType(forwardedType))
					{
						DiagnosticInfo diagnosticInfo = forwardedType.GetUseSiteInfo().DiagnosticInfo ?? ((ErrorTypeSymbol)forwardedType).ErrorInfo;
						if (diagnosticInfo != null)
						{
							instance.Add(diagnosticInfo, NoLocation.Singleton);
						}
					}
				}
			}
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag;
			if (wellKnownAttributeData != null || netModuleAttributes.Any())
			{
				customAttributesBag = new CustomAttributesBag<VisualBasicAttributeData>();
				customAttributesBag.SetEarlyDecodedWellKnownAttributeData(null);
				customAttributesBag.SetDecodedWellKnownAttributeData(wellKnownAttributeData);
				customAttributesBag.SetAttributes(netModuleAttributes);
			}
			else
			{
				customAttributesBag = CustomAttributesBag<VisualBasicAttributeData>.Empty;
			}
			if (attributeIndicesToSkip != null)
			{
				Interlocked.CompareExchange(ref _lazyDuplicateAttributeIndices, attributeIndicesToSkip, null);
			}
			SourceModule.AtomicStoreReferenceAndDiagnostics(ref lazyNetModuleAttributesBag, customAttributesBag, instance);
			instance.Free();
		}

		internal override IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes()
		{
			return PEModuleBuilder.GetForwardedTypes(this, null);
		}

		private void EnsureNetModuleAttributesAreBound()
		{
			if (_lazyNetModuleAttributesBag == null)
			{
				LoadAndValidateNetModuleAttributes(ref _lazyNetModuleAttributesBag);
			}
		}

		internal bool IsIndexOfDuplicateAssemblyAttribute(int index)
		{
			if (_lazyDuplicateAttributeIndices != null)
			{
				return _lazyDuplicateAttributeIndices.Contains(index);
			}
			return false;
		}

		internal ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			ArrayBuilder<SyntaxList<AttributeListSyntax>> instance = ArrayBuilder<SyntaxList<AttributeListSyntax>>.GetInstance();
			ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = DeclaringCompilation.MergedRootDeclaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RootSingleNamespaceDeclaration rootSingleNamespaceDeclaration = (RootSingleNamespaceDeclaration)enumerator.Current;
				if (!rootSingleNamespaceDeclaration.HasAssemblyAttributes)
				{
					continue;
				}
				SyntaxList<AttributesStatementSyntax> attributes = ((CompilationUnitSyntax)rootSingleNamespaceDeclaration.Location.SourceTree!.GetRoot()).Attributes;
				if (attributes.Any())
				{
					SyntaxList<AttributesStatementSyntax>.Enumerator enumerator2 = attributes.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						AttributesStatementSyntax current = enumerator2.Current;
						instance.Add(current.AttributeLists);
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal CustomAttributesBag<VisualBasicAttributeData> GetNetModuleAttributesBag()
		{
			EnsureNetModuleAttributesAreBound();
			return _lazyNetModuleAttributesBag;
		}

		private ImmutableArray<VisualBasicAttributeData> GetNetModuleAttributes()
		{
			return GetNetModuleAttributesBag().Attributes;
		}

		internal CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> GetNetModuleDecodedWellKnownAttributeData()
		{
			return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)GetNetModuleAttributesBag().DecodedWellKnownAttributeData;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			ImmutableArray<VisualBasicAttributeData> immutableArray = GetSourceAttributesBag().Attributes;
			ImmutableArray<VisualBasicAttributeData> attributes = GetNetModuleAttributesBag().Attributes;
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

		internal CustomAttributesBag<VisualBasicAttributeData> GetSourceAttributesBag()
		{
			EnsureAttributesAreBound();
			return _lazySourceAttributesBag;
		}

		internal CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> GetSourceDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazySourceAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetSourceAttributesBag();
			}
			return (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)customAttributesBag.DecodedWellKnownAttributeData;
		}

		private string GetWellKnownAttributeDataStringField(Func<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>, string> fieldGetter, string missingValue = null)
		{
			string text = missingValue;
			CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> sourceDecodedWellKnownAttributeData = GetSourceDecodedWellKnownAttributeData();
			if (sourceDecodedWellKnownAttributeData != null)
			{
				text = fieldGetter(sourceDecodedWellKnownAttributeData);
			}
			if ((object)text == missingValue)
			{
				sourceDecodedWellKnownAttributeData = GetNetModuleDecodedWellKnownAttributeData();
				if (sourceDecodedWellKnownAttributeData != null)
				{
					text = fieldGetter(sourceDecodedWellKnownAttributeData);
				}
			}
			return text;
		}

		internal IEnumerable<SecurityAttribute> GetSecurityAttributes()
		{
			IEnumerable<SecurityAttribute> enumerable = null;
			CustomAttributesBag<VisualBasicAttributeData> sourceAttributesBag = GetSourceAttributesBag();
			CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> commonAssemblyWellKnownAttributeData = (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)sourceAttributesBag.DecodedWellKnownAttributeData;
			if (commonAssemblyWellKnownAttributeData != null)
			{
				SecurityWellKnownAttributeData securityInformation = commonAssemblyWellKnownAttributeData.SecurityInformation;
				if (securityInformation != null)
				{
					enumerable = securityInformation.GetSecurityAttributes(sourceAttributesBag.Attributes);
				}
			}
			IEnumerable<SecurityAttribute> enumerable2 = null;
			sourceAttributesBag = GetNetModuleAttributesBag();
			commonAssemblyWellKnownAttributeData = (CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>)sourceAttributesBag.DecodedWellKnownAttributeData;
			if (commonAssemblyWellKnownAttributeData != null)
			{
				SecurityWellKnownAttributeData securityInformation2 = commonAssemblyWellKnownAttributeData.SecurityInformation;
				if (securityInformation2 != null)
				{
					enumerable2 = securityInformation2.GetSecurityAttributes(sourceAttributesBag.Attributes);
				}
			}
			IEnumerable<SecurityAttribute> enumerable3 = null;
			if (enumerable != null)
			{
				if (enumerable2 != null)
				{
					return enumerable.Concat(enumerable2);
				}
				return enumerable;
			}
			if (enumerable2 != null)
			{
				return enumerable2;
			}
			return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
		}

		private void EnsureAttributesAreBound()
		{
			if (_lazySourceAttributesBag == null || !_lazySourceAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref _lazySourceAttributesBag);
			}
		}

		internal override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies()
		{
			return SourceModule.GetReferencedAssemblySymbols();
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
					bag.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_FriendRefNotEqualToThis, key.Identity, Identity), NoLocation.Singleton));
					break;
				case IVTConclusion.OneSignedOneNot:
					bag.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_FriendRefSigningMismatch, key.Identity, Identity), NoLocation.Singleton));
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
			ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> value = null;
			_lazyInternalsVisibleToMap.TryGetValue(simpleName, out value);
			return (value != null) ? value.Keys : SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
		}

		private bool ProcessOneInternalsVisibleToAttribute(AttributeSyntax nodeOpt, VisualBasicAttributeData attrData, BindingDiagnosticBag diagnostics)
		{
			string text = attrData.CommonConstructorArguments[0].ValueInternal as string;
			if (text == null)
			{
				diagnostics.Add(ERRID.ERR_FriendAssemblyNameInvalid, (nodeOpt != null) ? nodeOpt.GetLocation() : NoLocation.Singleton, text);
				return false;
			}
			AssemblyIdentity identity = null;
			AssemblyIdentityParts parts = (AssemblyIdentityParts)0;
			if (!AssemblyIdentity.TryParseDisplayName(text, out identity, out parts))
			{
				diagnostics.Add(ERRID.ERR_FriendAssemblyNameInvalid, (nodeOpt != null) ? nodeOpt.GetLocation() : NoLocation.Singleton, text);
				return false;
			}
			if (((uint)parts & 0xFFFFFF3Eu) != 0)
			{
				diagnostics.Add(ERRID.ERR_FriendAssemblyBadArguments, (nodeOpt != null) ? nodeOpt.GetLocation() : NoLocation.Singleton, text);
				return false;
			}
			if (_lazyInternalsVisibleToMap == null)
			{
				Interlocked.CompareExchange(ref _lazyInternalsVisibleToMap, new ConcurrentDictionary<string, ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>>(StringComparer.OrdinalIgnoreCase), null);
			}
			Tuple<Location, string> value = null;
			if (identity.PublicKey.IsEmpty)
			{
				value = new Tuple<Location, string>((nodeOpt != null) ? nodeOpt.GetLocation() : NoLocation.Singleton, text);
			}
			ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>> value2 = null;
			if (_lazyInternalsVisibleToMap.TryGetValue(identity.Name, out value2))
			{
				value2.TryAdd(identity.PublicKey, value);
			}
			else
			{
				value2 = new ConcurrentDictionary<ImmutableArray<byte>, Tuple<Location, string>>();
				value2.TryAdd(identity.PublicKey, value);
				_lazyInternalsVisibleToMap.TryAdd(identity.Name, value2);
			}
			return true;
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			if (attribute.IsTargetAttribute(this, AttributeDescription.CaseInsensitiveExtensionAttribute))
			{
				_lazyEmitExtensionAttribute = 1;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.InternalsVisibleToAttribute))
			{
				ProcessOneInternalsVisibleToAttribute(arguments.AttributeSyntaxOpt, attribute, bindingDiagnosticBag);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblySignatureKeyAttribute))
			{
				string text = (string)attribute.CommonConstructorArguments[0].ValueInternal;
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblySignatureKeyAttributeSetting = text;
				if (!StrongNameKeys.IsValidPublicKeyString(text))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_InvalidSignaturePublicKey, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
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
				string s = (string)attribute.CommonConstructorArguments[0].ValueInternal;
				Version version = null;
				if (!VersionHelper.TryParseAssemblyVersion(s, !_compilation.IsEmitDeterministic, out version))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_InvalidVersionFormat, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
				}
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyVersionAttributeSetting = version;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyFileVersionAttribute))
			{
				Version version2 = null;
				string text2 = (string)attribute.CommonConstructorArguments[0].ValueInternal;
				if (!VersionHelper.TryParse(text2, out version2))
				{
					bindingDiagnosticBag.Add(ERRID.WRN_InvalidVersionFormat, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
				}
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyFileVersionAttributeSetting = text2;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.AssemblyInformationalVersionAttribute))
			{
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyInformationalVersionAttributeSetting = (string)attribute.CommonConstructorArguments[0].ValueInternal;
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
				string text3 = (string)attribute.CommonConstructorArguments[0].ValueInternal;
				if (!string.IsNullOrEmpty(text3))
				{
					if (DeclaringCompilation.Options.OutputKind.IsApplication())
					{
						bindingDiagnosticBag.Add(ERRID.ERR_InvalidAssemblyCultureForExe, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
					}
					else if (!AssemblyIdentity.IsValidCultureName(text3))
					{
						bindingDiagnosticBag.Add(ERRID.ERR_InvalidAssemblyCulture, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
						text3 = null;
					}
				}
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyCultureAttributeSetting = text3;
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
				Version version3 = null;
				if (!VersionHelper.TryParseAssemblyVersion((string)attribute.CommonConstructorArguments[0].ValueInternal, allowWildcard: false, out version3))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_InvalidVersionFormat2, GetAssemblyAttributeFirstArgumentLocation(arguments.AttributeSyntaxOpt));
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
			else if (attribute.IsSecurityAttribute(DeclaringCompilation))
			{
				attribute.DecodeSecurityAttribute<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>(this, DeclaringCompilation, ref arguments);
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
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().RuntimeCompatibilityWrapNonExceptionThrows = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DebuggableAttribute))
			{
				arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().HasDebuggableAttribute = true;
			}
			else
			{
				int targetAttributeSignatureIndex = attribute.GetTargetAttributeSignatureIndex(this, AttributeDescription.AssemblyAlgorithmIdAttribute);
				if (targetAttributeSignatureIndex != -1)
				{
					object objectValue = RuntimeHelpers.GetObjectValue(attribute.CommonConstructorArguments[0].ValueInternal);
					AssemblyHashAlgorithm value = ((targetAttributeSignatureIndex != 0) ? ((AssemblyHashAlgorithm)Microsoft.VisualBasic.CompilerServices.Conversions.ToUInteger(objectValue)) : ((AssemblyHashAlgorithm)Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(objectValue)));
					arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyAlgorithmIdAttributeSetting = value;
				}
				else
				{
					targetAttributeSignatureIndex = attribute.GetTargetAttributeSignatureIndex(this, AttributeDescription.AssemblyFlagsAttribute);
					if (targetAttributeSignatureIndex != -1)
					{
						object objectValue2 = RuntimeHelpers.GetObjectValue(attribute.CommonConstructorArguments[0].ValueInternal);
						AssemblyFlags assemblyFlagsAttributeSetting = ((targetAttributeSignatureIndex != 0 && targetAttributeSignatureIndex != 1) ? ((AssemblyFlags)Microsoft.VisualBasic.CompilerServices.Conversions.ToUInteger(objectValue2)) : ((AssemblyFlags)Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(objectValue2)));
						arguments.GetOrCreateData<CommonAssemblyWellKnownAttributeData<NamedTypeSymbol>>().AssemblyFlagsAttributeSetting = assemblyFlagsAttributeSetting;
					}
				}
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		private static Location GetAssemblyAttributeFirstArgumentLocation(AttributeSyntax attributeSyntaxOpt)
		{
			if (attributeSyntaxOpt == null)
			{
				return NoLocation.Singleton;
			}
			return attributeSyntaxOpt.ArgumentList.Arguments.First().GetLocation();
		}

		private void ValidateIntegralAttributeNonNegativeArguments(VisualBasicAttributeData attrData, AttributeSyntax nodeOpt, BindingDiagnosticBag diagnostics)
		{
			int num = attrData.CommonConstructorArguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (attrData.GetConstructorArgument<int>(i, SpecialType.System_Int32) < 0)
				{
					diagnostics.Add(ERRID.ERR_BadAttribute1, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[i].GetLocation() : NoLocation.Singleton, attrData.AttributeClass);
				}
			}
		}

		internal void AnErrorHasBeenReportedAboutExtensionAttribute()
		{
			_lazyEmitExtensionAttribute = 1;
		}

		internal void GetAllDeclarationErrors(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken = default(CancellationToken))
		{
			bool hasExtensionMethods = false;
			SourceModule.GetAllDeclarationErrors(diagnostics, cancellationToken, ref hasExtensionMethods);
			diagnostics.AddRange(GetAssemblyLevelDeclarationErrors(hasExtensionMethods), allowMismatchInDependencyAccumulation: true);
		}

		private ImmutableBindingDiagnostic<AssemblySymbol> GetAssemblyLevelDeclarationErrors(bool haveExtensionMethodsInSource)
		{
			if (_lazyAssemblyLevelDeclarationErrors.IsDefault || _lazyAssemblyLevelDeclarationDependencies.IsDefault)
			{
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				ThreeState threeState = (ThreeState)_lazyEmitExtensionAttribute;
				if (threeState == ThreeState.Unknown)
				{
					if (!haveExtensionMethodsInSource)
					{
						threeState = ThreeState.False;
					}
					else
					{
						UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
						_compilation.GetExtensionAttributeConstructor(out useSiteInfo);
						threeState = ((useSiteInfo.DiagnosticInfo != null) ? ThreeState.False : ThreeState.True);
					}
				}
				_lazyEmitExtensionAttribute = (byte)threeState;
				if (StrongNameKeys.DiagnosticOpt != null && !this.IsNetModule())
				{
					instance.Add(StrongNameKeys.DiagnosticOpt);
				}
				ValidateIVTPublicKeys(instance);
				CheckOptimisticIVTAccessGrants(instance);
				DetectAttributeAndOptionConflicts(instance);
				if (IsDelaySigned && !Identity.HasPublicKey)
				{
					instance.Add(ERRID.WRN_DelaySignButNoKey, NoLocation.Singleton);
				}
				if (DeclaringCompilation.Options.PublicSign)
				{
					if (this.IsNetModule())
					{
						instance.Add(ERRID.ERR_PublicSignNetModule, NoLocation.Singleton);
					}
					else if (!Identity.HasPublicKey)
					{
						instance.Add(ERRID.ERR_PublicSignNoKey, NoLocation.Singleton);
					}
				}
				if (DeclaringCompilation.Options.OutputKind != OutputKind.NetModule && DeclaringCompilation.Options.CryptoPublicKey.IsEmpty && Identity.HasPublicKey && !IsDelaySigned && !DeclaringCompilation.Options.PublicSign && !StrongNameKeys.CanSign)
				{
					instance.Add(ERRID.ERR_SignButNoPrivateKey, NoLocation.Singleton, StrongNameKeys.KeyFilePath);
				}
				ReportDiagnosticsForSynthesizedAttributes(DeclaringCompilation, instance);
				ReportDiagnosticsForAddedModules(instance);
				ImmutableBindingDiagnostic<AssemblySymbol> immutableBindingDiagnostic = instance.ToReadOnlyAndFree();
				ImmutableInterlocked.InterlockedInitialize(ref _lazyAssemblyLevelDeclarationDependencies, immutableBindingDiagnostic.Dependencies);
				ImmutableInterlocked.InterlockedInitialize(ref _lazyAssemblyLevelDeclarationErrors, immutableBindingDiagnostic.Diagnostics);
			}
			return new ImmutableBindingDiagnostic<AssemblySymbol>(_lazyAssemblyLevelDeclarationErrors, _lazyAssemblyLevelDeclarationDependencies);
		}

		private void DetectAttributeAndOptionConflicts(BindingDiagnosticBag diagnostics)
		{
			EnsureAttributesAreBound();
			if (_compilation.Options.PublicSign && DelaySignAttributeSetting)
			{
				diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, AttributeDescription.AssemblyDelaySignAttribute.FullName, "PublicSign");
			}
			if (_compilation.Options.OutputKind == OutputKind.NetModule)
			{
				if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyContainer))
				{
					string assemblyKeyContainerAttributeSetting = AssemblyKeyContainerAttributeSetting;
					if ((object)assemblyKeyContainerAttributeSetting == WellKnownAttributeData.StringMissingValue)
					{
						Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, _compilation, NoLocation.Singleton, diagnostics);
					}
					else if (string.Compare(_compilation.Options.CryptoKeyContainer, assemblyKeyContainerAttributeSetting, StringComparison.OrdinalIgnoreCase) != 0)
					{
						diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, AttributeDescription.AssemblyKeyNameAttribute.FullName, "CryptoKeyContainer");
					}
				}
				if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyFile))
				{
					string assemblyKeyFileAttributeSetting = AssemblyKeyFileAttributeSetting;
					if ((object)assemblyKeyFileAttributeSetting == WellKnownAttributeData.StringMissingValue)
					{
						Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, _compilation, NoLocation.Singleton, diagnostics);
					}
					else if (string.Compare(_compilation.Options.CryptoKeyFile, assemblyKeyFileAttributeSetting, StringComparison.OrdinalIgnoreCase) != 0)
					{
						diagnostics.Add(ERRID.ERR_CmdOptionConflictsSource, NoLocation.Singleton, AttributeDescription.AssemblyKeyFileAttribute.FullName, "CryptoKeyFile");
					}
				}
			}
			else if (_compilation.Options.PublicSign)
			{
				if ((object)AssemblyKeyContainerAttributeSetting != WellKnownAttributeData.StringMissingValue)
				{
					diagnostics.Add(ERRID.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, AttributeDescription.AssemblyKeyNameAttribute.FullName);
				}
				if ((object)AssemblyKeyFileAttributeSetting != WellKnownAttributeData.StringMissingValue)
				{
					diagnostics.Add(ERRID.WRN_AttributeIgnoredWhenPublicSigning, NoLocation.Singleton, AttributeDescription.AssemblyKeyFileAttribute.FullName);
				}
			}
		}

		private void ReportDiagnosticsForAddedModules(BindingDiagnosticBag diagnostics)
		{
			foreach (KeyValuePair<MetadataReference, int> item in _compilation.GetBoundReferenceManager().ReferencedModuleIndexMap)
			{
				if (item.Key is PortableExecutableReference portableExecutableReference && portableExecutableReference.FilePath != null)
				{
					string fileName = FileNameUtilities.GetFileName(portableExecutableReference.FilePath);
					string name = _modules[item.Value].Name;
					if (!string.Equals(fileName, name, StringComparison.OrdinalIgnoreCase))
					{
						diagnostics.Add(ERRID.ERR_NetModuleNameMismatch, NoLocation.Singleton, name, fileName);
					}
				}
			}
			if (_modules.Length <= 1 || _compilation.Options.OutputKind.IsNetModule())
			{
				return;
			}
			Machine machine = base.Machine;
			bool flag = machine == Machine.I386 && !base.Bit32Required;
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			int num = Modules.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				ModuleSymbol moduleSymbol = Modules[i];
				if (!hashSet.Add(moduleSymbol.Name))
				{
					diagnostics.Add(ERRID.ERR_NetModuleNameMustBeUnique, NoLocation.Singleton, moduleSymbol.Name);
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
						diagnostics.Add(ERRID.ERR_AgnosticToMachineModule, NoLocation.Singleton, moduleSymbol);
					}
					else if (machine != machine2)
					{
						diagnostics.Add(ERRID.ERR_ConflictingMachineModule, NoLocation.Singleton, moduleSymbol);
					}
				}
			}
			int num2 = Modules.Length - 1;
			for (int j = 1; j <= num2; j++)
			{
				PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)Modules[j];
				try
				{
					foreach (string item2 in pEModuleSymbol.Module.GetReferencedManagedModulesOrThrow())
					{
						if (hashSet.Add(item2))
						{
							diagnostics.Add(ERRID.ERR_MissingNetModuleReference, NoLocation.Singleton, item2);
						}
					}
				}
				catch (BadImageFormatException ex)
				{
					ProjectData.SetProjectError(ex);
					BadImageFormatException ex2 = ex;
					diagnostics.Add(ERRID.ERR_UnsupportedModule1, NoLocation.Singleton, pEModuleSymbol);
					ProjectData.ClearProjectError();
				}
			}
		}

		protected void ValidateIVTPublicKeys(BindingDiagnosticBag diagnostics)
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
						diagnostics.Add(ERRID.ERR_FriendAssemblyStrongNameRequired, item.Value.Item1, item.Value.Item2);
					}
				}
			}
		}

		private static void ReportDiagnosticsForSynthesizedAttributes(VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			if (!compilation.Options.OutputKind.IsNetModule())
			{
				if (!(compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) is MissingMetadataTypeSymbol))
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, compilation, NoLocation.Singleton, diagnostics);
				}
				if (!(compilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) is MissingMetadataTypeSymbol))
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, compilation, NoLocation.Singleton, diagnostics);
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, compilation, NoLocation.Singleton, diagnostics);
				}
			}
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilationOptions options = DeclaringCompilation.Options;
			bool num = options.OutputKind.IsNetModule();
			if (_lazyEmitExtensionAttribute == 2)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, _compilation.SynthesizeExtensionAttribute());
			}
			if (DeclaringCompilation.EmbeddedSymbolManager.IsAnySymbolReferenced)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.Microsoft_VisualBasic_Embedded__ctor));
			}
			if (!num && !Modules.Any((ModuleSymbol m) => m.HasAssemblyCompilationRelaxationsAttribute) && !(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_CompilationRelaxationsAttribute) is MissingMetadataTypeSymbol))
			{
				NamedTypeSymbol specialType = DeclaringCompilation.GetSpecialType(SpecialType.System_Int32);
				TypedConstant item = new TypedConstant(specialType, TypedConstantKind.Primitive, 8);
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32, ImmutableArray.Create(item)));
			}
			if (!num && !Modules.Any((ModuleSymbol m) => m.HasAssemblyRuntimeCompatibilityAttribute) && !(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute) is MissingMetadataTypeSymbol))
			{
				NamedTypeSymbol specialType2 = DeclaringCompilation.GetSpecialType(SpecialType.System_Boolean);
				TypedConstant value = new TypedConstant(specialType2, TypedConstantKind.Primitive, true);
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor, ImmutableArray<TypedConstant>.Empty, ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows, value))));
			}
			if (!num && !HasAssemblyOrModuleDebuggableAttribute)
			{
				NamedTypeSymbol specialType3 = DeclaringCompilation.GetSpecialType(SpecialType.System_Int32);
				if (specialType3.GetUseSiteInfo().DiagnosticInfo == null)
				{
					DebuggableAttribute.DebuggingModes debuggingModes = DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints;
					if (options.OptimizationLevel == OptimizationLevel.Debug)
					{
						debuggingModes = debuggingModes | DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations;
					}
					if (options.EnableEditAndContinue)
					{
						debuggingModes |= DebuggableAttribute.DebuggingModes.EnableEditAndContinue;
					}
					TypedConstant item2 = new TypedConstant(specialType3, TypedConstantKind.Enum, (int)debuggingModes);
					Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggableAttribute__ctorDebuggingModes, ImmutableArray.Create(item2)));
				}
			}
			if (_compilation.Options.OutputKind == OutputKind.NetModule)
			{
				if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyContainer) && (object)AssemblyKeyContainerAttributeSetting == WellKnownAttributeData.StringMissingValue)
				{
					NamedTypeSymbol specialType4 = _compilation.GetSpecialType(SpecialType.System_String);
					TypedConstant item3 = new TypedConstant(specialType4, TypedConstantKind.Primitive, _compilation.Options.CryptoKeyContainer);
					Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyNameAttribute__ctor, ImmutableArray.Create(item3)));
				}
				if (!string.IsNullOrEmpty(_compilation.Options.CryptoKeyFile) && (object)AssemblyKeyFileAttributeSetting == WellKnownAttributeData.StringMissingValue)
				{
					NamedTypeSymbol specialType5 = _compilation.GetSpecialType(SpecialType.System_String);
					TypedConstant item4 = new TypedConstant(specialType5, TypedConstantKind.Primitive, _compilation.Options.CryptoKeyFile);
					Symbol.AddSynthesizedAttribute(ref attributes, _compilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_AssemblyKeyFileAttribute__ctor, ImmutableArray.Create(item4)));
				}
			}
		}

		internal override bool AreInternalsVisibleToThisAssembly(AssemblySymbol potentialGiverOfAccess)
		{
			if (_lazyStrongNameKeys == null && (object)s_AssemblyForWhichCurrentThreadIsComputingKeys != null)
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
			return MakeFinalIVTDetermination(potentialGiverOfAccess) == IVTConclusion.Match;
		}

		private void ComputeAndSetStrongNameKeys()
		{
			EnsureAttributesAreBound();
			if (_lazyStrongNameKeys != null)
			{
				return;
			}
			string text = _compilation.Options.CryptoKeyFile;
			if (DeclaringCompilation.Options.PublicSign)
			{
				Interlocked.CompareExchange(value: (string.IsNullOrEmpty(text) || PathUtilities.IsAbsolute(text)) ? StrongNameKeys.Create(text, MessageProvider.Instance) : StrongNameKeys.None, location1: ref _lazyStrongNameKeys, comparand: null);
				return;
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
			StrongNameKeys value2 = StrongNameKeys.Create(DeclaringCompilation.Options.StrongNameProvider, text, text2, hasCounterSignature, MessageProvider.Instance);
			Interlocked.CompareExchange(ref _lazyStrongNameKeys, value2, null);
		}

		private AssemblyIdentity ComputeIdentity()
		{
			EnsureAttributesAreBound();
			return new AssemblyIdentity(_assemblySimpleName, VersionHelper.GenerateVersionFromPatternAndCurrentTime(_compilation.Options.CurrentLocalTime, AssemblyVersionAttributeSetting), AssemblyCultureAttributeSetting, StrongNameKeys.PublicKey, !StrongNameKeys.PublicKey.IsDefault);
		}

		private bool CheckForRuntime()
		{
			NamedTypeSymbol wellKnownType = DeclaringCompilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute);
			if (!TypeSymbolExtensions.IsErrorType(wellKnownType) && !wellKnownType.IsEmbedded)
			{
				return (object)wellKnownType.ContainingAssembly == this;
			}
			return false;
		}

		internal override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool ignoreCase)
		{
			if (!_compilation.Options.OutputKind.IsNetModule())
			{
				string matchedName = null;
				for (int i = _modules.Length - 1; i >= 1; i += -1)
				{
					PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)_modules[i];
					(AssemblySymbol, AssemblySymbol) assembliesForForwardedType = pEModuleSymbol.GetAssembliesForForwardedType(ref emittedName, ignoreCase, out matchedName);
					if ((object)assembliesForForwardedType.Item1 != null)
					{
						if ((object)assembliesForForwardedType.Item2 != null)
						{
							return CreateMultipleForwardingErrorTypeSymbol(ref emittedName, pEModuleSymbol, assembliesForForwardedType.Item1, assembliesForForwardedType.Item2);
						}
						if (visitedAssemblies != null && visitedAssemblies.Contains(assembliesForForwardedType.Item1))
						{
							return CreateCycleInTypeForwarderErrorTypeSymbol(ref emittedName);
						}
						visitedAssemblies = new ConsList<AssemblySymbol>(this, visitedAssemblies ?? ConsList<AssemblySymbol>.Empty);
						if (ignoreCase && !string.Equals(emittedName.FullName, matchedName, StringComparison.Ordinal))
						{
							emittedName = MetadataTypeName.FromFullName(matchedName, emittedName.UseCLSCompliantNameArityEncoding, emittedName.ForcedArity);
						}
						return assembliesForForwardedType.Item1.LookupTopLevelMetadataTypeWithCycleDetection(ref emittedName, visitedAssemblies, digThroughForwardedTypes: true);
					}
				}
			}
			return null;
		}

		public override AssemblyMetadata GetMetadata()
		{
			return null;
		}
	}
}
