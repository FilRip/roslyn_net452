using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class CommandLineArguments
    {
        public bool IsScriptRunner { get; set; }

        public bool InteractiveMode { get; set; }

        public string? BaseDirectory { get; set; }

        public ImmutableArray<KeyValuePair<string, string>> PathMap { get; set; }

        public ImmutableArray<string> ReferencePaths { get; set; }

        public ImmutableArray<string> SourcePaths { get; set; }

        public ImmutableArray<string> KeyFileSearchPaths { get; set; }

        public bool Utf8Output { get; set; }

        public string? CompilationName { get; set; }

        public EmitOptions EmitOptions { get; set; }

        public string? OutputFileName { get; set; }

        public string? OutputRefFilePath { get; set; }

        public string? PdbPath { get; set; }

        public string? SourceLink { get; set; }

        public string? RuleSetPath { get; set; }

        public bool EmitPdb { get; set; }

        public string OutputDirectory { get; set; }

        public string? DocumentationPath { get; set; }

        public string? GeneratedFilesOutputDirectory { get; set; }

        public ErrorLogOptions? ErrorLogOptions { get; set; }

        public string? ErrorLogPath => ErrorLogOptions?.Path;

        public string? AppConfigPath { get; set; }

        public ImmutableArray<Diagnostic> Errors { get; set; }

        public ImmutableArray<CommandLineReference> MetadataReferences { get; set; }

        public ImmutableArray<CommandLineAnalyzerReference> AnalyzerReferences { get; set; }

        public ImmutableArray<string> AnalyzerConfigPaths { get; set; }

        public ImmutableArray<CommandLineSourceFile> AdditionalFiles { get; set; }

        public ImmutableArray<CommandLineSourceFile> EmbeddedFiles { get; set; }

        public bool ReportAnalyzer { get; set; }

        public bool SkipAnalyzers { get; set; }

        public bool DisplayLogo { get; set; }

        public bool DisplayHelp { get; set; }

        public bool DisplayVersion { get; set; }

        public bool DisplayLangVersions { get; set; }

        public string? Win32ResourceFile { get; set; }

        public string? Win32Icon { get; set; }

        public string? Win32Manifest { get; set; }

        public bool NoWin32Manifest { get; set; }

        public ImmutableArray<ResourceDescription> ManifestResources { get; set; }

        public Encoding? Encoding { get; set; }

        public SourceHashAlgorithm ChecksumAlgorithm { get; set; }

        public ImmutableArray<string> ScriptArguments { get; set; }

        public ImmutableArray<CommandLineSourceFile> SourceFiles { get; set; }

        public string? TouchedFilesPath { get; set; }

        public bool PrintFullPaths { get; set; }

        public ParseOptions ParseOptions => ParseOptionsCore;

        public CompilationOptions CompilationOptions => CompilationOptionsCore;

        protected abstract ParseOptions ParseOptionsCore { get; }

        protected abstract CompilationOptions CompilationOptionsCore { get; }

        public CultureInfo? PreferredUILang { get; set; }

        public bool EmitPdbFile
        {
            get
            {
                if (EmitPdb)
                {
                    return EmitOptions.DebugInformationFormat != DebugInformationFormat.Embedded;
                }
                return false;
            }
        }

        public StrongNameProvider GetStrongNameProvider(StrongNameFileSystem fileSystem)
        {
            return new DesktopStrongNameProvider(KeyFileSearchPaths, fileSystem);
        }

        public CommandLineArguments()
        {
        }

        public string GetOutputFilePath(string outputFileName)
        {
            if (outputFileName == null)
            {
                throw new ArgumentNullException("outputFileName");
            }
            return Path.Combine(OutputDirectory, outputFileName);
        }

        public string GetPdbFilePath(string outputFileName)
        {
            if (outputFileName == null)
            {
                throw new ArgumentNullException("outputFileName");
            }
            return PdbPath ?? Path.Combine(OutputDirectory, Path.ChangeExtension(outputFileName, ".pdb"));
        }

        public IEnumerable<MetadataReference> ResolveMetadataReferences(MetadataReferenceResolver metadataResolver)
        {
            if (metadataResolver == null)
            {
                throw new ArgumentNullException("metadataResolver");
            }
            return ResolveMetadataReferences(metadataResolver, null, null);
        }

        internal IEnumerable<MetadataReference> ResolveMetadataReferences(MetadataReferenceResolver metadataResolver, List<DiagnosticInfo>? diagnosticsOpt, CommonMessageProvider? messageProviderOpt)
        {
            List<MetadataReference> list = new List<MetadataReference>();
            ResolveMetadataReferences(metadataResolver, diagnosticsOpt, messageProviderOpt, list);
            return list;
        }

        internal virtual bool ResolveMetadataReferences(MetadataReferenceResolver metadataResolver, List<DiagnosticInfo>? diagnosticsOpt, CommonMessageProvider? messageProviderOpt, List<MetadataReference> resolved)
        {
            bool result = true;
            ImmutableArray<CommandLineReference>.Enumerator enumerator = MetadataReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CommandLineReference current = enumerator.Current;
                ImmutableArray<PortableExecutableReference> immutableArray = ResolveMetadataReference(current, metadataResolver, diagnosticsOpt, messageProviderOpt);
                if (!immutableArray.IsDefaultOrEmpty)
                {
                    resolved.AddRange(immutableArray);
                    continue;
                }
                result = false;
                if (diagnosticsOpt == null)
                {
                    resolved.Add(new UnresolvedMetadataReference(current.Reference, current.Properties));
                }
            }
            return result;
        }

        internal static ImmutableArray<PortableExecutableReference> ResolveMetadataReference(CommandLineReference cmdReference, MetadataReferenceResolver metadataResolver, List<DiagnosticInfo>? diagnosticsOpt, CommonMessageProvider? messageProviderOpt)
        {
            ImmutableArray<PortableExecutableReference> result;
            try
            {
                result = metadataResolver.ResolveReference(cmdReference.Reference, null, cmdReference.Properties);
            }
            catch (Exception ex) when (diagnosticsOpt != null && (ex is BadImageFormatException || ex is IOException))
            {
                Diagnostic diagnostic = PortableExecutableReference.ExceptionToDiagnostic(ex, messageProviderOpt, Location.None, cmdReference.Reference, cmdReference.Properties.Kind);
                diagnosticsOpt!.Add(((DiagnosticWithInfo)diagnostic).Info);
                return ImmutableArray<PortableExecutableReference>.Empty;
            }
            if (result.IsDefaultOrEmpty && diagnosticsOpt != null)
            {
                diagnosticsOpt!.Add(new DiagnosticInfo(messageProviderOpt, messageProviderOpt!.ERR_MetadataFileNotFound, cmdReference.Reference));
                return ImmutableArray<PortableExecutableReference>.Empty;
            }
            return result;
        }

        public IEnumerable<AnalyzerReference> ResolveAnalyzerReferences(IAnalyzerAssemblyLoader analyzerLoader)
        {
            ImmutableArray<CommandLineAnalyzerReference>.Enumerator enumerator = AnalyzerReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CommandLineAnalyzerReference current = enumerator.Current;
                yield return (AnalyzerReference)(ResolveAnalyzerReference(current, analyzerLoader) ?? ((object)new UnresolvedAnalyzerReference(current.FilePath)));
            }
        }

        public void ResolveAnalyzersFromArguments(string language, List<DiagnosticInfo> diagnostics, CommonMessageProvider messageProvider, IAnalyzerAssemblyLoader analyzerLoader, bool skipAnalyzers, out ImmutableArray<DiagnosticAnalyzer> analyzers, out ImmutableArray<ISourceGenerator> generators)
        {
            CommonMessageProvider messageProvider2 = messageProvider;
            List<DiagnosticInfo> diagnostics2 = diagnostics;
            ImmutableArray<DiagnosticAnalyzer>.Builder builder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            ImmutableArray<ISourceGenerator>.Builder builder2 = ImmutableArray.CreateBuilder<ISourceGenerator>();
            void value(object o, AnalyzerLoadFailureEventArgs e)
            {
                AnalyzerFileReference analyzerFileReference2 = o as AnalyzerFileReference;
                DiagnosticInfo diagnosticInfo;
                switch (e.ErrorCode)
                {
                    default:
                        return;
                    case AnalyzerLoadFailureEventArgs.FailureErrorCode.UnableToLoadAnalyzer:
                        diagnosticInfo = new DiagnosticInfo(messageProvider2, messageProvider2.WRN_UnableToLoadAnalyzer, analyzerFileReference2.FullPath, e.Message);
                        break;
                    case AnalyzerLoadFailureEventArgs.FailureErrorCode.UnableToCreateAnalyzer:
                        diagnosticInfo = new DiagnosticInfo(messageProvider2, messageProvider2.WRN_AnalyzerCannotBeCreated, e.TypeName ?? "", analyzerFileReference2.FullPath, e.Message);
                        break;
                    case AnalyzerLoadFailureEventArgs.FailureErrorCode.NoAnalyzers:
                        diagnosticInfo = new DiagnosticInfo(messageProvider2, messageProvider2.WRN_NoAnalyzerInAssembly, analyzerFileReference2.FullPath);
                        break;
                    case AnalyzerLoadFailureEventArgs.FailureErrorCode.ReferencesFramework:
                        diagnosticInfo = new DiagnosticInfo(messageProvider2, messageProvider2.WRN_AnalyzerReferencesFramework, analyzerFileReference2.FullPath, e.TypeName);
                        break;
                    case AnalyzerLoadFailureEventArgs.FailureErrorCode.None:
                        return;
                }
                diagnosticInfo = messageProvider2.FilterDiagnosticInfo(diagnosticInfo, CompilationOptions);
                if (diagnosticInfo != null)
                {
                    diagnostics2.Add(diagnosticInfo);
                }
            }
            ArrayBuilder<AnalyzerFileReference> instance = ArrayBuilder<AnalyzerFileReference>.GetInstance();
            ImmutableArray<CommandLineAnalyzerReference>.Enumerator enumerator = AnalyzerReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CommandLineAnalyzerReference current = enumerator.Current;
                AnalyzerFileReference analyzerFileReference = ResolveAnalyzerReference(current, analyzerLoader);
                if (analyzerFileReference != null)
                {
                    instance.Add(analyzerFileReference);
                    analyzerLoader.AddDependencyLocation(analyzerFileReference.FullPath);
                }
                else
                {
                    diagnostics2.Add(new DiagnosticInfo(messageProvider2, messageProvider2.ERR_MetadataFileNotFound, current.FilePath));
                }
            }
            ArrayBuilder<AnalyzerFileReference>.Enumerator enumerator2 = instance.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                AnalyzerFileReference current2 = enumerator2.Current;
                current2.AnalyzerLoadFailed += value;
                if (!skipAnalyzers)
                {
                    current2.AddAnalyzers(builder, language);
                }
                current2.AddGenerators(builder2, language);
                current2.AnalyzerLoadFailed -= value;
            }
            instance.Free();
            generators = builder2.ToImmutable();
            analyzers = builder.ToImmutable();
        }

        private AnalyzerFileReference? ResolveAnalyzerReference(CommandLineAnalyzerReference reference, IAnalyzerAssemblyLoader analyzerLoader)
        {
            string text = FileUtilities.ResolveRelativePath(reference.FilePath, null, BaseDirectory, ReferencePaths, File.Exists);
            if (text != null)
            {
                text = FileUtilities.TryNormalizeAbsolutePath(text);
            }
            if (text != null)
            {
                return new AnalyzerFileReference(text, analyzerLoader);
            }
            return null;
        }
    }
}
