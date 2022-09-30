using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public sealed class VisualBasicCompilationOptions : CompilationOptions, IEquatable<VisualBasicCompilationOptions>
	{
		private ImmutableArray<GlobalImport> _globalImports;

		private string _rootNamespace;

		private OptionStrict _optionStrict;

		private bool _optionInfer;

		private bool _optionExplicit;

		private bool _optionCompareText;

		private bool _embedVbCoreRuntime;

		private VisualBasicParseOptions _parseOptions;

		private bool _suppressEmbeddedDeclarations;

		private bool _ignoreCorLibraryDuplicatedTypes;

		public override string Language => "Visual Basic";

		public ImmutableArray<GlobalImport> GlobalImports => _globalImports;

		public string RootNamespace => _rootNamespace;

		public OptionStrict OptionStrict => _optionStrict;

		public bool OptionInfer => _optionInfer;

		public bool OptionExplicit => _optionExplicit;

		public bool OptionCompareText => _optionCompareText;

		public bool EmbedVbCoreRuntime => _embedVbCoreRuntime;

		internal bool SuppressEmbeddedDeclarations => _suppressEmbeddedDeclarations;

		internal bool IgnoreCorLibraryDuplicatedTypes => _ignoreCorLibraryDuplicatedTypes;

		public VisualBasicParseOptions ParseOptions => _parseOptions;

		public override NullableContextOptions NullableContextOptions
		{
			get
			{
				return NullableContextOptions.Disable;
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		public VisualBasicCompilationOptions(OutputKind outputKind, string moduleName = null, string mainTypeName = null, string scriptClassName = "Script", IEnumerable<GlobalImport> globalImports = null, string rootNamespace = null, OptionStrict optionStrict = OptionStrict.Off, bool optionInfer = true, bool optionExplicit = true, bool optionCompareText = false, VisualBasicParseOptions parseOptions = null, bool embedVbCoreRuntime = false, OptimizationLevel optimizationLevel = OptimizationLevel.Debug, bool checkOverflow = true, string cryptoKeyContainer = null, string cryptoKeyFile = null, ImmutableArray<byte> cryptoPublicKey = default(ImmutableArray<byte>), bool? delaySign = null, Platform platform = Platform.AnyCpu, ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, XmlReferenceResolver xmlReferenceResolver = null, SourceReferenceResolver sourceReferenceResolver = null, MetadataReferenceResolver metadataReferenceResolver = null, AssemblyIdentityComparer assemblyIdentityComparer = null, StrongNameProvider strongNameProvider = null, bool publicSign = false, bool reportSuppressedDiagnostics = false, MetadataImportOptions metadataImportOptions = MetadataImportOptions.Public)
			: this(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, DateTime.MinValue, suppressEmbeddedDeclarations: false, debugPlusMode: false, xmlReferenceResolver, sourceReferenceResolver, null, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions: false, ignoreCorLibraryDuplicatedTypes: false)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public VisualBasicCompilationOptions(OutputKind outputKind, string moduleName, string mainTypeName, string scriptClassName, IEnumerable<GlobalImport> globalImports, string rootNamespace, OptionStrict optionStrict, bool optionInfer, bool optionExplicit, bool optionCompareText, VisualBasicParseOptions parseOptions, bool embedVbCoreRuntime, OptimizationLevel optimizationLevel, bool checkOverflow, string cryptoKeyContainer, string cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver xmlReferenceResolver, SourceReferenceResolver sourceReferenceResolver, MetadataReferenceResolver metadataReferenceResolver, AssemblyIdentityComparer assemblyIdentityComparer, StrongNameProvider strongNameProvider, bool publicSign, bool reportSuppressedDiagnostics)
			: this(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign, reportSuppressedDiagnostics, MetadataImportOptions.Public)
		{
		}

		private VisualBasicCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string moduleName, string mainTypeName, string scriptClassName, IEnumerable<GlobalImport> globalImports, string rootNamespace, OptionStrict optionStrict, bool optionInfer, bool optionExplicit, bool optionCompareText, VisualBasicParseOptions parseOptions, bool embedVbCoreRuntime, OptimizationLevel optimizationLevel, bool checkOverflow, string cryptoKeyContainer, string cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, bool publicSign, Platform platform, ReportDiagnostic generalDiagnosticOption, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, bool deterministic, DateTime currentLocalTime, bool suppressEmbeddedDeclarations, bool debugPlusMode, XmlReferenceResolver xmlReferenceResolver, SourceReferenceResolver sourceReferenceResolver, SyntaxTreeOptionsProvider syntaxTreeOptionsProvider, MetadataReferenceResolver metadataReferenceResolver, AssemblyIdentityComparer assemblyIdentityComparer, StrongNameProvider strongNameProvider, MetadataImportOptions metadataImportOptions, bool referencesSupersedeLowerVersions, bool ignoreCorLibraryDuplicatedTypes)
			: base(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign, optimizationLevel, checkOverflow, platform, generalDiagnosticOption, 1, specificDiagnosticOptions.ToImmutableDictionaryOrEmpty(CaseInsensitiveComparison.Comparer), concurrentBuild, deterministic, currentLocalTime, debugPlusMode, xmlReferenceResolver, sourceReferenceResolver, syntaxTreeOptionsProvider, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions)
		{
			_globalImports = globalImports.AsImmutableOrEmpty();
			_rootNamespace = rootNamespace ?? string.Empty;
			_optionStrict = optionStrict;
			_optionInfer = optionInfer;
			_optionExplicit = optionExplicit;
			_optionCompareText = optionCompareText;
			_embedVbCoreRuntime = embedVbCoreRuntime;
			_suppressEmbeddedDeclarations = suppressEmbeddedDeclarations;
			_parseOptions = parseOptions;
			_ignoreCorLibraryDuplicatedTypes = ignoreCorLibraryDuplicatedTypes;
		}

		internal VisualBasicCompilationOptions(VisualBasicCompilationOptions other)
			: this(other.OutputKind, other.ReportSuppressedDiagnostics, other.ModuleName, other.MainTypeName, other.ScriptClassName, other.GlobalImports, other.RootNamespace, other.OptionStrict, other.OptionInfer, other.OptionExplicit, other.OptionCompareText, other.ParseOptions, other.EmbedVbCoreRuntime, other.OptimizationLevel, other.CheckOverflow, other.CryptoKeyContainer, other.CryptoKeyFile, other.CryptoPublicKey, other.DelaySign, other.PublicSign, other.Platform, other.GeneralDiagnosticOption, other.SpecificDiagnosticOptions, other.ConcurrentBuild, other.Deterministic, other.CurrentLocalTime, other.SuppressEmbeddedDeclarations, other.DebugPlusMode, other.XmlReferenceResolver, other.SourceReferenceResolver, other.SyntaxTreeOptionsProvider, other.MetadataReferenceResolver, other.AssemblyIdentityComparer, other.StrongNameProvider, other.MetadataImportOptions, other.ReferencesSupersedeLowerVersions, other.IgnoreCorLibraryDuplicatedTypes)
		{
		}

		internal override ImmutableArray<string> GetImports()
		{
			ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance(GlobalImports.Length);
			ImmutableArray<GlobalImport>.Enumerator enumerator = GlobalImports.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GlobalImport current = enumerator.Current;
				if (!current.IsXmlClause)
				{
					instance.Add(current.Name);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal ImmutableArray<string> GetRootNamespaceParts()
		{
			if (string.IsNullOrEmpty(_rootNamespace) || !OptionsValidator.IsValidNamespaceName(_rootNamespace))
			{
				return ImmutableArray<string>.Empty;
			}
			return MetadataHelpers.SplitQualifiedName(_rootNamespace);
		}

		public new VisualBasicCompilationOptions WithOutputKind(OutputKind kind)
		{
			if (kind == base.OutputKind)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				OutputKind = kind
			};
		}

		public new VisualBasicCompilationOptions WithModuleName(string moduleName)
		{
			if (string.Equals(moduleName, base.ModuleName, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				ModuleName = moduleName
			};
		}

		public new VisualBasicCompilationOptions WithScriptClassName(string name)
		{
			if (string.Equals(name, base.ScriptClassName, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				ScriptClassName = name
			};
		}

		public new VisualBasicCompilationOptions WithMainTypeName(string name)
		{
			if (string.Equals(name, base.MainTypeName, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				MainTypeName = name
			};
		}

		public VisualBasicCompilationOptions WithGlobalImports(ImmutableArray<GlobalImport> globalImports)
		{
			if (GlobalImports.Equals(globalImports))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_globalImports = globalImports
			};
		}

		public VisualBasicCompilationOptions WithGlobalImports(IEnumerable<GlobalImport> globalImports)
		{
			return new VisualBasicCompilationOptions(this)
			{
				_globalImports = globalImports.AsImmutableOrEmpty()
			};
		}

		public VisualBasicCompilationOptions WithGlobalImports(params GlobalImport[] globalImports)
		{
			return WithGlobalImports((IEnumerable<GlobalImport>)globalImports);
		}

		public VisualBasicCompilationOptions WithRootNamespace(string rootNamespace)
		{
			if (string.Equals(rootNamespace, RootNamespace, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_rootNamespace = rootNamespace
			};
		}

		public VisualBasicCompilationOptions WithOptionStrict(OptionStrict value)
		{
			if (value == OptionStrict)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_optionStrict = value
			};
		}

		public VisualBasicCompilationOptions WithOptionInfer(bool value)
		{
			if (value == OptionInfer)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_optionInfer = value
			};
		}

		public VisualBasicCompilationOptions WithOptionExplicit(bool value)
		{
			if (value == OptionExplicit)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_optionExplicit = value
			};
		}

		public VisualBasicCompilationOptions WithOptionCompareText(bool value)
		{
			if (value == OptionCompareText)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_optionCompareText = value
			};
		}

		public VisualBasicCompilationOptions WithEmbedVbCoreRuntime(bool value)
		{
			if (value == EmbedVbCoreRuntime)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_embedVbCoreRuntime = value
			};
		}

		public new VisualBasicCompilationOptions WithOverflowChecks(bool enabled)
		{
			if (enabled == base.CheckOverflow)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				CheckOverflow = enabled
			};
		}

		public new VisualBasicCompilationOptions WithConcurrentBuild(bool concurrentBuild)
		{
			if (concurrentBuild == base.ConcurrentBuild)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				ConcurrentBuild = concurrentBuild
			};
		}

		public new VisualBasicCompilationOptions WithDeterministic(bool deterministic)
		{
			if (deterministic == base.Deterministic)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				Deterministic = deterministic
			};
		}

		internal VisualBasicCompilationOptions WithCurrentLocalTime(DateTime value)
		{
			if (value.Equals(base.CurrentLocalTime))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				CurrentLocalTime = value
			};
		}

		internal VisualBasicCompilationOptions WithDebugPlusMode(bool debugPlusMode)
		{
			if (debugPlusMode == base.DebugPlusMode)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				DebugPlusMode = debugPlusMode
			};
		}

		internal VisualBasicCompilationOptions WithSuppressEmbeddedDeclarations(bool suppressEmbeddedDeclarations)
		{
			if (suppressEmbeddedDeclarations == _suppressEmbeddedDeclarations)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_suppressEmbeddedDeclarations = suppressEmbeddedDeclarations
			};
		}

		internal VisualBasicCompilationOptions WithIgnoreCorLibraryDuplicatedTypes(bool ignoreCorLibraryDuplicatedTypes)
		{
			if (ignoreCorLibraryDuplicatedTypes == _ignoreCorLibraryDuplicatedTypes)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_ignoreCorLibraryDuplicatedTypes = ignoreCorLibraryDuplicatedTypes
			};
		}

		public new VisualBasicCompilationOptions WithCryptoKeyContainer(string name)
		{
			if (string.Equals(name, base.CryptoKeyContainer, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				CryptoKeyContainer = name
			};
		}

		public new VisualBasicCompilationOptions WithCryptoKeyFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = null;
			}
			if (string.Equals(path, base.CryptoKeyFile, StringComparison.Ordinal))
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				CryptoKeyFile = path
			};
		}

		public new VisualBasicCompilationOptions WithCryptoPublicKey(ImmutableArray<byte> value)
		{
			if (value.IsDefault)
			{
				value = ImmutableArray<byte>.Empty;
			}
			if (value == base.CryptoPublicKey)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				CryptoPublicKey = value
			};
		}

		public new VisualBasicCompilationOptions WithDelaySign(bool? value)
		{
			bool? flag = value;
			bool? delaySign = base.DelaySign;
			if (((flag.HasValue & delaySign.HasValue) ? new bool?(flag.GetValueOrDefault() == delaySign.GetValueOrDefault()) : null).GetValueOrDefault())
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				DelaySign = value
			};
		}

		public new VisualBasicCompilationOptions WithPlatform(Platform value)
		{
			if (value == base.Platform)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				Platform = value
			};
		}

		public new VisualBasicCompilationOptions WithPublicSign(bool value)
		{
			if (value == base.PublicSign)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				PublicSign = value
			};
		}

		protected override CompilationOptions CommonWithConcurrentBuild(bool concurrent)
		{
			return WithConcurrentBuild(concurrent);
		}

		protected override CompilationOptions CommonWithDeterministic(bool deterministic)
		{
			return WithDeterministic(deterministic);
		}

		protected override CompilationOptions CommonWithGeneralDiagnosticOption(ReportDiagnostic value)
		{
			return WithGeneralDiagnosticOption(value);
		}

		protected override CompilationOptions CommonWithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions)
		{
			return WithSpecificDiagnosticOptions(specificDiagnosticOptions);
		}

		protected override CompilationOptions CommonWithSpecificDiagnosticOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions)
		{
			return WithSpecificDiagnosticOptions(specificDiagnosticOptions);
		}

		protected override CompilationOptions CommonWithReportSuppressedDiagnostics(bool reportSuppressedDiagnostics)
		{
			return WithReportSuppressedDiagnostics(reportSuppressedDiagnostics);
		}

		protected override CompilationOptions CommonWithMetadataImportOptions(MetadataImportOptions value)
		{
			return WithMetadataImportOptions(value);
		}

		[Obsolete]
		protected override CompilationOptions CommonWithFeatures(ImmutableArray<string> features)
		{
			throw new NotImplementedException();
		}

		public new VisualBasicCompilationOptions WithGeneralDiagnosticOption(ReportDiagnostic value)
		{
			if (value == base.GeneralDiagnosticOption)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				GeneralDiagnosticOption = value
			};
		}

		public new VisualBasicCompilationOptions WithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> value)
		{
			if (value == null)
			{
				value = ImmutableDictionary<string, ReportDiagnostic>.Empty;
			}
			if (value == base.SpecificDiagnosticOptions)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				SpecificDiagnosticOptions = value
			};
		}

		public new VisualBasicCompilationOptions WithSpecificDiagnosticOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>> value)
		{
			return new VisualBasicCompilationOptions(this)
			{
				SpecificDiagnosticOptions = value.ToImmutableDictionaryOrEmpty()
			};
		}

		public new VisualBasicCompilationOptions WithReportSuppressedDiagnostics(bool value)
		{
			if (value == base.ReportSuppressedDiagnostics)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				ReportSuppressedDiagnostics = value
			};
		}

		public new VisualBasicCompilationOptions WithOptimizationLevel(OptimizationLevel value)
		{
			if (value == base.OptimizationLevel)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				OptimizationLevel = value
			};
		}

		public new VisualBasicCompilationOptions WithMetadataImportOptions(MetadataImportOptions value)
		{
			if (value == base.MetadataImportOptions)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				MetadataImportOptions = value
			};
		}

		internal VisualBasicCompilationOptions WithReferencesSupersedeLowerVersions(bool value)
		{
			if (value == base.ReferencesSupersedeLowerVersions)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				ReferencesSupersedeLowerVersions = value
			};
		}

		public VisualBasicCompilationOptions WithParseOptions(VisualBasicParseOptions options)
		{
			if ((object)options == ParseOptions)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				_parseOptions = options
			};
		}

		public new VisualBasicCompilationOptions WithXmlReferenceResolver(XmlReferenceResolver resolver)
		{
			if (resolver == base.XmlReferenceResolver)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				XmlReferenceResolver = resolver
			};
		}

		public new VisualBasicCompilationOptions WithSourceReferenceResolver(SourceReferenceResolver resolver)
		{
			if (resolver == base.SourceReferenceResolver)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				SourceReferenceResolver = resolver
			};
		}

		public new VisualBasicCompilationOptions WithSyntaxTreeOptionsProvider(SyntaxTreeOptionsProvider provider)
		{
			if (provider == base.SyntaxTreeOptionsProvider)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				SyntaxTreeOptionsProvider = provider
			};
		}

		public new VisualBasicCompilationOptions WithMetadataReferenceResolver(MetadataReferenceResolver resolver)
		{
			if (resolver == base.MetadataReferenceResolver)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				MetadataReferenceResolver = resolver
			};
		}

		public new VisualBasicCompilationOptions WithAssemblyIdentityComparer(AssemblyIdentityComparer comparer)
		{
			comparer = comparer ?? Microsoft.CodeAnalysis.AssemblyIdentityComparer.Default;
			if (comparer == base.AssemblyIdentityComparer)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				AssemblyIdentityComparer = comparer
			};
		}

		public new VisualBasicCompilationOptions WithStrongNameProvider(StrongNameProvider provider)
		{
			if (provider == base.StrongNameProvider)
			{
				return this;
			}
			return new VisualBasicCompilationOptions(this)
			{
				StrongNameProvider = provider
			};
		}

		protected override CompilationOptions CommonWithOutputKind(OutputKind kind)
		{
			return WithOutputKind(kind);
		}

		protected override CompilationOptions CommonWithPlatform(Platform platform)
		{
			return WithPlatform(platform);
		}

		protected override CompilationOptions CommonWithPublicSign(bool publicSign)
		{
			return WithPublicSign(publicSign);
		}

		protected override CompilationOptions CommonWithOptimizationLevel(OptimizationLevel value)
		{
			return WithOptimizationLevel(value);
		}

		protected override CompilationOptions CommonWithAssemblyIdentityComparer(AssemblyIdentityComparer comparer)
		{
			return WithAssemblyIdentityComparer(comparer);
		}

		protected override CompilationOptions CommonWithXmlReferenceResolver(XmlReferenceResolver resolver)
		{
			return WithXmlReferenceResolver(resolver);
		}

		protected override CompilationOptions CommonWithSourceReferenceResolver(SourceReferenceResolver resolver)
		{
			return WithSourceReferenceResolver(resolver);
		}

		protected override CompilationOptions CommonWithSyntaxTreeOptionsProvider(SyntaxTreeOptionsProvider provider)
		{
			return WithSyntaxTreeOptionsProvider(provider);
		}

		protected override CompilationOptions CommonWithMetadataReferenceResolver(MetadataReferenceResolver resolver)
		{
			return WithMetadataReferenceResolver(resolver);
		}

		protected override CompilationOptions CommonWithStrongNameProvider(StrongNameProvider provider)
		{
			return WithStrongNameProvider(provider);
		}

		internal override void ValidateOptions(ArrayBuilder<Diagnostic> builder)
		{
			ValidateOptions(builder, MessageProvider.Instance);
			if ((object)ParseOptions != null)
			{
				builder.AddRange(ParseOptions.Errors);
			}
			if (EmbedVbCoreRuntime && base.OutputKind.IsNetModule())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2042));
			}
			if (!base.Platform.IsValid())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "Platform", base.Platform.ToString()));
			}
			if (base.ModuleName != null)
			{
				MetadataHelpers.CheckAssemblyOrModuleName(base.ModuleName, MessageProvider.Instance, 37206, builder);
			}
			if (!base.OutputKind.IsValid())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "OutputKind", base.OutputKind.ToString()));
			}
			if (!base.OptimizationLevel.IsValid())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "OptimizationLevel", base.OptimizationLevel.ToString()));
			}
			if (base.ScriptClassName == null || !base.ScriptClassName.IsValidClrTypeName())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "ScriptClassName", base.ScriptClassName ?? "Nothing"));
			}
			if (base.MainTypeName != null && !base.MainTypeName.IsValidClrTypeName())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "MainTypeName", base.MainTypeName));
			}
			if (!string.IsNullOrEmpty(RootNamespace) && !OptionsValidator.IsValidNamespaceName(RootNamespace))
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "RootNamespace", RootNamespace));
			}
			if (!OptionStrictEnumBounds.IsValid(OptionStrict))
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "OptionStrict", OptionStrict.ToString()));
			}
			if (base.Platform == Platform.AnyCpu32BitPreferred && base.OutputKind.IsValid() && base.OutputKind != 0 && base.OutputKind != OutputKind.WindowsApplication && base.OutputKind != OutputKind.WindowsRuntimeApplication)
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 31392, "Platform", base.Platform.ToString()));
			}
			if (!base.MetadataImportOptions.IsValid())
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 2014, "MetadataImportOptions", base.MetadataImportOptions.ToString()));
			}
		}

		public bool Equals(VisualBasicCompilationOptions other)
		{
			if ((object)this == other)
			{
				return true;
			}
			if (!EqualsHelper(other))
			{
				return false;
			}
			return (GlobalImports.IsDefault ? other.GlobalImports.IsDefault : GlobalImports.SequenceEqual(other.GlobalImports)) && string.Equals(RootNamespace, other.RootNamespace, StringComparison.Ordinal) && OptionStrict == other.OptionStrict && OptionInfer == other.OptionInfer && OptionExplicit == other.OptionExplicit && OptionCompareText == other.OptionCompareText && EmbedVbCoreRuntime == other.EmbedVbCoreRuntime && SuppressEmbeddedDeclarations == other.SuppressEmbeddedDeclarations && IgnoreCorLibraryDuplicatedTypes == other.IgnoreCorLibraryDuplicatedTypes && (((object)ParseOptions == null) ? ((object)other.ParseOptions == null) : ParseOptions.Equals(other.ParseOptions));
		}

		bool IEquatable<VisualBasicCompilationOptions>.Equals(VisualBasicCompilationOptions other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as VisualBasicCompilationOptions);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(GetHashCodeHelper(), Hash.Combine(Hash.CombineValues(GlobalImports), Hash.Combine((RootNamespace != null) ? StringComparer.Ordinal.GetHashCode(RootNamespace) : 0, Hash.Combine((int)OptionStrict, Hash.Combine(OptionInfer, Hash.Combine(OptionExplicit, Hash.Combine(OptionCompareText, Hash.Combine(EmbedVbCoreRuntime, Hash.Combine(SuppressEmbeddedDeclarations, Hash.Combine(IgnoreCorLibraryDuplicatedTypes, Hash.Combine(ParseOptions, 0)))))))))));
		}

		internal override Diagnostic FilterDiagnostic(Diagnostic diagnostic, CancellationToken cancellationToken)
		{
			return VisualBasicDiagnosticFilter.Filter(diagnostic, base.GeneralDiagnosticOption, base.SpecificDiagnosticOptions, base.SyntaxTreeOptionsProvider, cancellationToken);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public VisualBasicCompilationOptions(OutputKind outputKind, string moduleName, string mainTypeName, string scriptClassName, IEnumerable<GlobalImport> globalImports, string rootNamespace, OptionStrict optionStrict, bool optionInfer, bool optionExplicit, bool optionCompareText, VisualBasicParseOptions parseOptions, bool embedVbCoreRuntime, OptimizationLevel optimizationLevel, bool checkOverflow, string cryptoKeyContainer, string cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver xmlReferenceResolver, SourceReferenceResolver sourceReferenceResolver, MetadataReferenceResolver metadataReferenceResolver, AssemblyIdentityComparer assemblyIdentityComparer, StrongNameProvider strongNameProvider)
			: this(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic: false, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign: false, reportSuppressedDiagnostics: false, MetadataImportOptions.Public)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public VisualBasicCompilationOptions(OutputKind outputKind, string moduleName, string mainTypeName, string scriptClassName, IEnumerable<GlobalImport> globalImports, string rootNamespace, OptionStrict optionStrict, bool optionInfer, bool optionExplicit, bool optionCompareText, VisualBasicParseOptions parseOptions, bool embedVbCoreRuntime, OptimizationLevel optimizationLevel, bool checkOverflow, string cryptoKeyContainer, string cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, XmlReferenceResolver xmlReferenceResolver, SourceReferenceResolver sourceReferenceResolver, MetadataReferenceResolver metadataReferenceResolver, AssemblyIdentityComparer assemblyIdentityComparer, StrongNameProvider strongNameProvider)
			: this(outputKind, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic: false, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public VisualBasicCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string moduleName = null, string mainTypeName = null, string scriptClassName = "Script", IEnumerable<GlobalImport> globalImports = null, string rootNamespace = null, OptionStrict optionStrict = OptionStrict.Off, bool optionInfer = true, bool optionExplicit = true, bool optionCompareText = false, VisualBasicParseOptions parseOptions = null, bool embedVbCoreRuntime = false, OptimizationLevel optimizationLevel = OptimizationLevel.Debug, bool checkOverflow = true, string cryptoKeyContainer = null, string cryptoKeyFile = null, ImmutableArray<byte> cryptoPublicKey = default(ImmutableArray<byte>), bool? delaySign = null, Platform platform = Platform.AnyCpu, ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, XmlReferenceResolver xmlReferenceResolver = null, SourceReferenceResolver sourceReferenceResolver = null, MetadataReferenceResolver metadataReferenceResolver = null, AssemblyIdentityComparer assemblyIdentityComparer = null, StrongNameProvider strongNameProvider = null)
			: this(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign: false, platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic, DateTime.MinValue, suppressEmbeddedDeclarations: false, debugPlusMode: false, xmlReferenceResolver, sourceReferenceResolver, null, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, MetadataImportOptions.Public, referencesSupersedeLowerVersions: false, ignoreCorLibraryDuplicatedTypes: false)
		{
		}

		protected override CompilationOptions CommonWithModuleName(string moduleName)
		{
			return WithModuleName(moduleName);
		}

		protected override CompilationOptions CommonWithMainTypeName(string mainTypeName)
		{
			return WithMainTypeName(mainTypeName);
		}

		protected override CompilationOptions CommonWithScriptClassName(string scriptClassName)
		{
			return WithScriptClassName(scriptClassName);
		}

		protected override CompilationOptions CommonWithCryptoKeyContainer(string cryptoKeyContainer)
		{
			return WithCryptoKeyContainer(cryptoKeyContainer);
		}

		protected override CompilationOptions CommonWithCryptoKeyFile(string cryptoKeyFile)
		{
			return WithCryptoKeyFile(cryptoKeyFile);
		}

		protected override CompilationOptions CommonWithCryptoPublicKey(ImmutableArray<byte> cryptoPublicKey)
		{
			return WithCryptoPublicKey(cryptoPublicKey);
		}

		protected override CompilationOptions CommonWithDelaySign(bool? delaySign)
		{
			return WithDelaySign(delaySign);
		}

		protected override CompilationOptions CommonWithCheckOverflow(bool checkOverflow)
		{
			return WithOverflowChecks(checkOverflow);
		}
	}
}
