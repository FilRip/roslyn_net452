using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSharpCompilationOptions : CompilationOptions, IEquatable<CSharpCompilationOptions>
    {
        public bool AllowUnsafe { get; private set; }

        public ImmutableArray<string> Usings { get; private set; }

        internal BinderFlags TopLevelBinderFlags { get; private set; }

        public override NullableContextOptions NullableContextOptions { get; protected set; }

        public override string Language => "C#";

        public CSharpCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics = false, string? moduleName = null, string? mainTypeName = null, string? scriptClassName = null, IEnumerable<string>? usings = null, OptimizationLevel optimizationLevel = OptimizationLevel.Debug, bool checkOverflow = false, bool allowUnsafe = false, string? cryptoKeyContainer = null, string? cryptoKeyFile = null, ImmutableArray<byte> cryptoPublicKey = default(ImmutableArray<byte>), bool? delaySign = null, Platform platform = Platform.AnyCpu, ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default, int warningLevel = 4, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, XmlReferenceResolver? xmlReferenceResolver = null, SourceReferenceResolver? sourceReferenceResolver = null, MetadataReferenceResolver? metadataReferenceResolver = null, AssemblyIdentityComparer? assemblyIdentityComparer = null, StrongNameProvider? strongNameProvider = null, bool publicSign = false, MetadataImportOptions metadataImportOptions = MetadataImportOptions.Public, NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
            : this(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, default(DateTime), debugPlusMode: false, xmlReferenceResolver, sourceReferenceResolver, null, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions: false, publicSign, BinderFlags.None, nullableContextOptions)
        {
        }

        public CSharpCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider, bool publicSign, MetadataImportOptions metadataImportOptions)
            : this(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign, metadataImportOptions, NullableContextOptions.Disable)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CSharpCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider, bool publicSign)
            : this(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign, MetadataImportOptions.Public)
        {
        }

        internal CSharpCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, bool deterministic, DateTime currentLocalTime, bool debugPlusMode, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, SyntaxTreeOptionsProvider? syntaxTreeOptionsProvider, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider, MetadataImportOptions metadataImportOptions, bool referencesSupersedeLowerVersions, bool publicSign, BinderFlags topLevelBinderFlags, NullableContextOptions nullableContextOptions)
            : base(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign, optimizationLevel, checkOverflow, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions.ToImmutableDictionaryOrEmpty(), concurrentBuild, deterministic, currentLocalTime, debugPlusMode, xmlReferenceResolver, sourceReferenceResolver, syntaxTreeOptionsProvider, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions)
        {
            Usings = usings.AsImmutableOrEmpty();
            AllowUnsafe = allowUnsafe;
            TopLevelBinderFlags = topLevelBinderFlags;
            NullableContextOptions = nullableContextOptions;
        }

        private CSharpCompilationOptions(CSharpCompilationOptions other)
            : this(other.OutputKind, moduleName: other.ModuleName, mainTypeName: other.MainTypeName, scriptClassName: other.ScriptClassName, usings: other.Usings, optimizationLevel: other.OptimizationLevel, checkOverflow: other.CheckOverflow, allowUnsafe: other.AllowUnsafe, cryptoKeyContainer: other.CryptoKeyContainer, cryptoKeyFile: other.CryptoKeyFile, cryptoPublicKey: other.CryptoPublicKey, delaySign: other.DelaySign, platform: other.Platform, generalDiagnosticOption: other.GeneralDiagnosticOption, warningLevel: other.WarningLevel, specificDiagnosticOptions: other.SpecificDiagnosticOptions, concurrentBuild: other.ConcurrentBuild, deterministic: other.Deterministic, currentLocalTime: other.CurrentLocalTime, debugPlusMode: other.DebugPlusMode, xmlReferenceResolver: other.XmlReferenceResolver, sourceReferenceResolver: other.SourceReferenceResolver, syntaxTreeOptionsProvider: other.SyntaxTreeOptionsProvider, metadataReferenceResolver: other.MetadataReferenceResolver, assemblyIdentityComparer: other.AssemblyIdentityComparer, strongNameProvider: other.StrongNameProvider, metadataImportOptions: other.MetadataImportOptions, referencesSupersedeLowerVersions: other.ReferencesSupersedeLowerVersions, reportSuppressedDiagnostics: other.ReportSuppressedDiagnostics, publicSign: other.PublicSign, topLevelBinderFlags: other.TopLevelBinderFlags, nullableContextOptions: other.NullableContextOptions)
        {
        }

        internal CSharpCompilationOptions WithTopLevelBinderFlags(BinderFlags flags)
        {
            if (flags != TopLevelBinderFlags)
            {
                return new CSharpCompilationOptions(this)
                {
                    TopLevelBinderFlags = flags
                };
            }
            return this;
        }

        public override ImmutableArray<string> GetImports()
        {
            return Usings;
        }

        public new CSharpCompilationOptions WithOutputKind(OutputKind kind)
        {
            if (kind == base.OutputKind)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                OutputKind = kind
            };
        }

        public new CSharpCompilationOptions WithModuleName(string? moduleName)
        {
            if (moduleName == base.ModuleName)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                ModuleName = moduleName
            };
        }

        public new CSharpCompilationOptions WithScriptClassName(string? name)
        {
            if (name == base.ScriptClassName)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                ScriptClassName = name
            };
        }

        public new CSharpCompilationOptions WithMainTypeName(string? name)
        {
            if (name == base.MainTypeName)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                MainTypeName = name
            };
        }

        public new CSharpCompilationOptions WithCryptoKeyContainer(string? name)
        {
            if (name == base.CryptoKeyContainer)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                CryptoKeyContainer = name
            };
        }

        public new CSharpCompilationOptions WithCryptoKeyFile(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = null;
            }
            if (path == base.CryptoKeyFile)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                CryptoKeyFile = path
            };
        }

        public new CSharpCompilationOptions WithCryptoPublicKey(ImmutableArray<byte> value)
        {
            if (value.IsDefault)
            {
                value = ImmutableArray<byte>.Empty;
            }
            if (value == base.CryptoPublicKey)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                CryptoPublicKey = value
            };
        }

        public new CSharpCompilationOptions WithDelaySign(bool? value)
        {
            if (value == base.DelaySign)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                DelaySign = value
            };
        }

        public CSharpCompilationOptions WithUsings(ImmutableArray<string> usings)
        {
            if (Usings == usings)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                Usings = usings
            };
        }

        public CSharpCompilationOptions WithUsings(IEnumerable<string>? usings)
        {
            return new CSharpCompilationOptions(this)
            {
                Usings = usings.AsImmutableOrEmpty()
            };
        }

        public CSharpCompilationOptions WithUsings(params string[]? usings)
        {
            return WithUsings((IEnumerable<string>?)usings);
        }

        public new CSharpCompilationOptions WithOptimizationLevel(OptimizationLevel value)
        {
            if (value == base.OptimizationLevel)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                OptimizationLevel = value
            };
        }

        public new CSharpCompilationOptions WithOverflowChecks(bool enabled)
        {
            if (enabled == base.CheckOverflow)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                CheckOverflow = enabled
            };
        }

        public CSharpCompilationOptions WithNullableContextOptions(NullableContextOptions options)
        {
            if (options == NullableContextOptions)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                NullableContextOptions = options
            };
        }

        public CSharpCompilationOptions WithAllowUnsafe(bool enabled)
        {
            if (enabled == AllowUnsafe)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                AllowUnsafe = enabled
            };
        }

        public new CSharpCompilationOptions WithPlatform(Platform platform)
        {
            if (base.Platform == platform)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                Platform = platform
            };
        }

        public new CSharpCompilationOptions WithPublicSign(bool publicSign)
        {
            if (base.PublicSign == publicSign)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                PublicSign = publicSign
            };
        }

        protected override CompilationOptions CommonWithGeneralDiagnosticOption(ReportDiagnostic value)
        {
            return WithGeneralDiagnosticOption(value);
        }

        protected override CompilationOptions CommonWithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic>? specificDiagnosticOptions)
        {
            return WithSpecificDiagnosticOptions(specificDiagnosticOptions);
        }

        protected override CompilationOptions CommonWithSpecificDiagnosticOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions)
        {
            return WithSpecificDiagnosticOptions(specificDiagnosticOptions);
        }

        protected override CompilationOptions CommonWithReportSuppressedDiagnostics(bool reportSuppressedDiagnostics)
        {
            return WithReportSuppressedDiagnostics(reportSuppressedDiagnostics);
        }

        public new CSharpCompilationOptions WithGeneralDiagnosticOption(ReportDiagnostic value)
        {
            if (base.GeneralDiagnosticOption == value)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                GeneralDiagnosticOption = value
            };
        }

        public new CSharpCompilationOptions WithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic>? values)
        {
            if (values == null)
            {
                values = ImmutableDictionary<string, ReportDiagnostic>.Empty;
            }
            if (base.SpecificDiagnosticOptions == values)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                SpecificDiagnosticOptions = values
            };
        }

        public new CSharpCompilationOptions WithSpecificDiagnosticOptions(IEnumerable<KeyValuePair<string, ReportDiagnostic>>? values)
        {
            return new CSharpCompilationOptions(this)
            {
                SpecificDiagnosticOptions = values.ToImmutableDictionaryOrEmpty()
            };
        }

        public new CSharpCompilationOptions WithReportSuppressedDiagnostics(bool reportSuppressedDiagnostics)
        {
            if (reportSuppressedDiagnostics == base.ReportSuppressedDiagnostics)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                ReportSuppressedDiagnostics = reportSuppressedDiagnostics
            };
        }

        public CSharpCompilationOptions WithWarningLevel(int warningLevel)
        {
            if (warningLevel == base.WarningLevel)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                WarningLevel = warningLevel
            };
        }

        public new CSharpCompilationOptions WithConcurrentBuild(bool concurrentBuild)
        {
            if (concurrentBuild == base.ConcurrentBuild)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                ConcurrentBuild = concurrentBuild
            };
        }

        public new CSharpCompilationOptions WithDeterministic(bool deterministic)
        {
            if (deterministic == base.Deterministic)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                Deterministic = deterministic
            };
        }

        internal CSharpCompilationOptions WithCurrentLocalTime(DateTime value)
        {
            if (value == base.CurrentLocalTime)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                CurrentLocalTime = value
            };
        }

        internal CSharpCompilationOptions WithDebugPlusMode(bool debugPlusMode)
        {
            if (debugPlusMode == base.DebugPlusMode)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                DebugPlusMode = debugPlusMode
            };
        }

        public new CSharpCompilationOptions WithMetadataImportOptions(MetadataImportOptions value)
        {
            if (value == base.MetadataImportOptions)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                MetadataImportOptions = value
            };
        }

        internal CSharpCompilationOptions WithReferencesSupersedeLowerVersions(bool value)
        {
            if (value == base.ReferencesSupersedeLowerVersions)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                ReferencesSupersedeLowerVersions = value
            };
        }

        public new CSharpCompilationOptions WithXmlReferenceResolver(XmlReferenceResolver? resolver)
        {
            if (resolver == base.XmlReferenceResolver)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                XmlReferenceResolver = resolver
            };
        }

        public new CSharpCompilationOptions WithSourceReferenceResolver(SourceReferenceResolver? resolver)
        {
            if (resolver == base.SourceReferenceResolver)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                SourceReferenceResolver = resolver
            };
        }

        public new CSharpCompilationOptions WithSyntaxTreeOptionsProvider(SyntaxTreeOptionsProvider? provider)
        {
            if (provider == base.SyntaxTreeOptionsProvider)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                SyntaxTreeOptionsProvider = provider
            };
        }

        public new CSharpCompilationOptions WithMetadataReferenceResolver(MetadataReferenceResolver? resolver)
        {
            if (resolver == base.MetadataReferenceResolver)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                MetadataReferenceResolver = resolver
            };
        }

        public new CSharpCompilationOptions WithAssemblyIdentityComparer(AssemblyIdentityComparer? comparer)
        {
            comparer = comparer ?? Microsoft.CodeAnalysis.AssemblyIdentityComparer.Default;
            if (comparer == base.AssemblyIdentityComparer)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                AssemblyIdentityComparer = comparer
            };
        }

        public new CSharpCompilationOptions WithStrongNameProvider(StrongNameProvider? provider)
        {
            if (provider == base.StrongNameProvider)
            {
                return this;
            }
            return new CSharpCompilationOptions(this)
            {
                StrongNameProvider = provider
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

        protected override CompilationOptions CommonWithAssemblyIdentityComparer(AssemblyIdentityComparer? comparer)
        {
            return WithAssemblyIdentityComparer(comparer);
        }

        protected override CompilationOptions CommonWithXmlReferenceResolver(XmlReferenceResolver? resolver)
        {
            return WithXmlReferenceResolver(resolver);
        }

        protected override CompilationOptions CommonWithSourceReferenceResolver(SourceReferenceResolver? resolver)
        {
            return WithSourceReferenceResolver(resolver);
        }

        protected override CompilationOptions CommonWithSyntaxTreeOptionsProvider(SyntaxTreeOptionsProvider? provider)
        {
            return WithSyntaxTreeOptionsProvider(provider);
        }

        protected override CompilationOptions CommonWithMetadataReferenceResolver(MetadataReferenceResolver? resolver)
        {
            return WithMetadataReferenceResolver(resolver);
        }

        protected override CompilationOptions CommonWithStrongNameProvider(StrongNameProvider? provider)
        {
            return WithStrongNameProvider(provider);
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

        public override void ValidateOptions(ArrayBuilder<Diagnostic> builder)
        {
            ValidateOptions(builder, MessageProvider.Instance);
            if (base.MainTypeName != null)
            {
                if (base.OutputKind.IsValid() && !base.OutputKind.IsApplication())
                {
                    builder.Add(Diagnostic.Create(MessageProvider.Instance, 2017));
                }
                if (!base.MainTypeName.IsValidClrTypeName())
                {
                    builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "MainTypeName", base.MainTypeName));
                }
            }
            if (!base.Platform.IsValid())
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 1672, base.Platform.ToString()));
            }
            if (base.ModuleName != null)
            {
                MetadataHelpers.CheckAssemblyOrModuleName(base.ModuleName, MessageProvider.Instance, 7087, builder);
            }
            if (!base.OutputKind.IsValid())
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "OutputKind", base.OutputKind.ToString()));
            }
            if (!base.OptimizationLevel.IsValid())
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "OptimizationLevel", base.OptimizationLevel.ToString()));
            }
            if (base.ScriptClassName == null || !base.ScriptClassName.IsValidClrTypeName())
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "ScriptClassName", base.ScriptClassName ?? "null"));
            }
            if (base.WarningLevel < 0)
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "WarningLevel", base.WarningLevel));
            }
            if (Usings != null && Usings.Any((string u) => !u.IsValidClrNamespaceName()))
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "Usings", Usings.Where((string u) => !u.IsValidClrNamespaceName()).First() ?? "null"));
            }
            if (base.Platform == Platform.AnyCpu32BitPreferred && base.OutputKind.IsValid() && base.OutputKind != 0 && base.OutputKind != OutputKind.WindowsApplication && base.OutputKind != OutputKind.WindowsRuntimeApplication)
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 4023));
            }
            if (!base.MetadataImportOptions.IsValid())
            {
                builder.Add(Diagnostic.Create(MessageProvider.Instance, 7088, "MetadataImportOptions", base.MetadataImportOptions.ToString()));
            }
        }

        public bool Equals(CSharpCompilationOptions? other)
        {
            if ((object)this == other)
            {
                return true;
            }
            if (!EqualsHelper(other))
            {
                return false;
            }
            if (AllowUnsafe == other!.AllowUnsafe && TopLevelBinderFlags == other!.TopLevelBinderFlags)
            {
                if (!(Usings == null))
                {
                    if (Usings.SequenceEqual(other!.Usings, StringComparer.Ordinal))
                    {
                        return NullableContextOptions == other!.NullableContextOptions;
                    }
                    return false;
                }
                return other!.Usings == null;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CSharpCompilationOptions);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(GetHashCodeHelper(), Hash.Combine(AllowUnsafe, Hash.Combine(Hash.CombineValues(Usings, StringComparer.Ordinal), Hash.Combine(TopLevelBinderFlags.GetHashCode(), NullableContextOptions.GetHashCode()))));
        }

        public override Diagnostic? FilterDiagnostic(Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            return CSharpDiagnosticFilter.Filter(diagnostic, base.WarningLevel, NullableContextOptions, base.GeneralDiagnosticOption, base.SpecificDiagnosticOptions, base.SyntaxTreeOptionsProvider, cancellationToken);
        }

        protected override CompilationOptions CommonWithModuleName(string? moduleName)
        {
            return WithModuleName(moduleName);
        }

        protected override CompilationOptions CommonWithMainTypeName(string? mainTypeName)
        {
            return WithMainTypeName(mainTypeName);
        }

        protected override CompilationOptions CommonWithScriptClassName(string? scriptClassName)
        {
            return WithScriptClassName(scriptClassName);
        }

        protected override CompilationOptions CommonWithCryptoKeyContainer(string? cryptoKeyContainer)
        {
            return WithCryptoKeyContainer(cryptoKeyContainer);
        }

        protected override CompilationOptions CommonWithCryptoKeyFile(string? cryptoKeyFile)
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CSharpCompilationOptions(OutputKind outputKind, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider)
            : this(outputKind, reportSuppressedDiagnostics: false, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, publicSign: false)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CSharpCompilationOptions(OutputKind outputKind, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider)
            : this(outputKind, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic: false, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CSharpCompilationOptions(OutputKind outputKind, bool reportSuppressedDiagnostics, string? moduleName, string? mainTypeName, string? scriptClassName, IEnumerable<string>? usings, OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string? cryptoKeyContainer, string? cryptoKeyFile, ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Platform platform, ReportDiagnostic generalDiagnosticOption, int warningLevel, IEnumerable<KeyValuePair<string, ReportDiagnostic>>? specificDiagnosticOptions, bool concurrentBuild, bool deterministic, XmlReferenceResolver? xmlReferenceResolver, SourceReferenceResolver? sourceReferenceResolver, MetadataReferenceResolver? metadataReferenceResolver, AssemblyIdentityComparer? assemblyIdentityComparer, StrongNameProvider? strongNameProvider)
            : this(outputKind, reportSuppressedDiagnostics: false, moduleName, mainTypeName, scriptClassName, usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, default(DateTime), debugPlusMode: false, xmlReferenceResolver, sourceReferenceResolver, null, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider, MetadataImportOptions.Public, referencesSupersedeLowerVersions: false, publicSign: false, BinderFlags.None, NullableContextOptions.Disable)
        {
        }
    }
}
