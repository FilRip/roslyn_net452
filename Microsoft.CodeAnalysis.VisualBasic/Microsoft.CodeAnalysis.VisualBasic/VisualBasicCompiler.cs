using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class VisualBasicCompiler : CommonCompiler
	{
		internal const string ResponseFileName = "vbc.rsp";

		internal const string VbcCommandLinePrefix = "vbc : ";

		private static readonly string s_responseFileName;

		private readonly string _responseFile;

		private readonly CommandLineDiagnosticFormatter _diagnosticFormatter;

		private readonly string _tempDirectory;

		private ImmutableArray<AdditionalTextFile> _additionalTextFiles;

		internal new VisualBasicCommandLineArguments Arguments => (VisualBasicCommandLineArguments)base.Arguments;

		public override DiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

		internal override Type Type => typeof(VisualBasicCompiler);

		protected VisualBasicCompiler(VisualBasicCommandLineParser parser, string responseFile, string[] args, BuildPaths buildPaths, string additionalReferenceDirectories, IAnalyzerAssemblyLoader analyzerLoader)
			: base(parser, responseFile, args, buildPaths, additionalReferenceDirectories, analyzerLoader)
		{
			_diagnosticFormatter = new CommandLineDiagnosticFormatter(buildPaths.WorkingDirectory, GetAdditionalTextFiles);
			_additionalTextFiles = default(ImmutableArray<AdditionalTextFile>);
			_tempDirectory = buildPaths.TempDirectory;
		}

		private ImmutableArray<AdditionalTextFile> GetAdditionalTextFiles()
		{
			return _additionalTextFiles;
		}

		protected override ImmutableArray<AdditionalTextFile> ResolveAdditionalFilesFromArguments(List<DiagnosticInfo> diagnostics, CommonMessageProvider messageProvider, TouchedFileLogger touchedFilesLogger)
		{
			_additionalTextFiles = base.ResolveAdditionalFilesFromArguments(diagnostics, messageProvider, touchedFilesLogger);
			return _additionalTextFiles;
		}

		private SyntaxTree ParseFile(TextWriter consoleOutput, VisualBasicParseOptions parseOptions, VisualBasicParseOptions scriptParseOptions, ref bool hadErrors, CommandLineSourceFile file, ErrorLogger errorLogger)
		{
			List<DiagnosticInfo> list = new List<DiagnosticInfo>();
			SourceText sourceText = TryReadFileContent(file, list);
			if (sourceText == null)
			{
				ReportDiagnostics(list, consoleOutput, errorLogger, null);
				list.Clear();
				hadErrors = true;
				return null;
			}
			SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(sourceText, file.IsScript ? scriptParseOptions : parseOptions, file.Path);
			syntaxTree.GetMappedLineSpanAndVisibility(default(TextSpan), out var _);
			return syntaxTree;
		}

		public override Compilation CreateCompilation(TextWriter consoleOutput, TouchedFileLogger touchedFilesLogger, ErrorLogger errorLogger, ImmutableArray<AnalyzerConfigOptionsResult> analyzerConfigOptions, AnalyzerConfigOptionsResult globalAnalyzerConfigOptions)
		{
			_Closure_0024__15_002D0 arg = default(_Closure_0024__15_002D0);
			_Closure_0024__15_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__15_002D0(arg);
			CS_0024_003C_003E8__locals0._0024VB_0024Me = this;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_consoleOutput = consoleOutput;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_errorLogger = errorLogger;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_parseOptions = Arguments.ParseOptions;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_scriptParseOptions = CS_0024_003C_003E8__locals0._0024VB_0024Local_parseOptions.WithKind(SourceCodeKind.Script);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_hadErrors = false;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles = Arguments.SourceFiles;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_trees = new SyntaxTree[CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles.Length - 1 + 1];
			if (Arguments.CompilationOptions.ConcurrentBuild)
			{
				RoslynParallel.For(0, CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles.Length, UICultureUtilities.WithCurrentUICulture(delegate(int i)
				{
					CS_0024_003C_003E8__locals0._0024VB_0024Local_trees[i] = CS_0024_003C_003E8__locals0._0024VB_0024Me.ParseFile(CS_0024_003C_003E8__locals0._0024VB_0024Local_consoleOutput, CS_0024_003C_003E8__locals0._0024VB_0024Local_parseOptions, CS_0024_003C_003E8__locals0._0024VB_0024Local_scriptParseOptions, ref CS_0024_003C_003E8__locals0._0024VB_0024Local_hadErrors, CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles[i], CS_0024_003C_003E8__locals0._0024VB_0024Local_errorLogger);
				}), CancellationToken.None);
			}
			else
			{
				int num = CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles.Length - 1;
				for (int j = 0; j <= num; j++)
				{
					CS_0024_003C_003E8__locals0._0024VB_0024Local_trees[j] = ParseFile(CS_0024_003C_003E8__locals0._0024VB_0024Local_consoleOutput, CS_0024_003C_003E8__locals0._0024VB_0024Local_parseOptions, CS_0024_003C_003E8__locals0._0024VB_0024Local_scriptParseOptions, ref CS_0024_003C_003E8__locals0._0024VB_0024Local_hadErrors, CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles[j], CS_0024_003C_003E8__locals0._0024VB_0024Local_errorLogger);
				}
			}
			if (CS_0024_003C_003E8__locals0._0024VB_0024Local_hadErrors)
			{
				return null;
			}
			if (Arguments.TouchedFilesPath != null)
			{
				ImmutableArray<CommandLineSourceFile>.Enumerator enumerator = CS_0024_003C_003E8__locals0._0024VB_0024Local_sourceFiles.GetEnumerator();
				while (enumerator.MoveNext())
				{
					touchedFilesLogger.AddRead(enumerator.Current.Path);
				}
			}
			List<DiagnosticInfo> diagnostics = new List<DiagnosticInfo>();
			DesktopAssemblyIdentityComparer @default = DesktopAssemblyIdentityComparer.Default;
			MetadataReferenceResolver referenceDirectiveResolver = null;
			List<MetadataReference> list = ResolveMetadataReferences(diagnostics, touchedFilesLogger, out referenceDirectiveResolver);
			if (ReportDiagnostics(diagnostics, CS_0024_003C_003E8__locals0._0024VB_0024Local_consoleOutput, CS_0024_003C_003E8__locals0._0024VB_0024Local_errorLogger, null))
			{
				return null;
			}
			if (Arguments.OutputLevel == OutputLevel.Verbose)
			{
				PrintReferences(list, CS_0024_003C_003E8__locals0._0024VB_0024Local_consoleOutput);
			}
			LoggingXmlFileResolver resolver = new LoggingXmlFileResolver(Arguments.BaseDirectory, touchedFilesLogger);
			LoggingSourceFileResolver resolver2 = new LoggingSourceFileResolver(ImmutableArray<string>.Empty, Arguments.BaseDirectory, Arguments.PathMap, touchedFilesLogger);
			LoggingStrongNameFileSystem fileSystem = new LoggingStrongNameFileSystem(touchedFilesLogger, _tempDirectory);
			CompilerSyntaxTreeOptionsProvider provider = new CompilerSyntaxTreeOptionsProvider(CS_0024_003C_003E8__locals0._0024VB_0024Local_trees, analyzerConfigOptions, globalAnalyzerConfigOptions);
			return VisualBasicCompilation.Create(Arguments.CompilationName, CS_0024_003C_003E8__locals0._0024VB_0024Local_trees, list, Arguments.CompilationOptions.WithMetadataReferenceResolver(referenceDirectiveResolver).WithAssemblyIdentityComparer(@default).WithXmlReferenceResolver(resolver)
				.WithStrongNameProvider(Arguments.GetStrongNameProvider(fileSystem))
				.WithSourceReferenceResolver(resolver2)
				.WithSyntaxTreeOptionsProvider(provider));
		}

		protected override string GetOutputFileName(Compilation compilation, CancellationToken cancellationToken)
		{
			return Arguments.OutputFileName;
		}

		private void PrintReferences(List<MetadataReference> resolvedReferences, TextWriter consoleOutput)
		{
			foreach (MetadataReference resolvedReference in resolvedReferences)
			{
				if (resolvedReference.Properties.Kind == MetadataImageKind.Module)
				{
					consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDMODULE, Culture), resolvedReference.Display);
				}
				else if (resolvedReference.Properties.EmbedInteropTypes)
				{
					consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDLINKREFERENCE, Culture), resolvedReference.Display);
				}
				else
				{
					consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_MSG_ADDREFERENCE, Culture), resolvedReference.Display);
				}
			}
			consoleOutput.WriteLine();
		}

		internal override bool SuppressDefaultResponseFile(IEnumerable<string> args)
		{
			foreach (string arg in args)
			{
				string left = arg.ToLowerInvariant();
				if (EmbeddedOperators.CompareString(left, "/noconfig", TextCompare: false) == 0 || EmbeddedOperators.CompareString(left, "-noconfig", TextCompare: false) == 0 || EmbeddedOperators.CompareString(left, "/nostdlib", TextCompare: false) == 0 || EmbeddedOperators.CompareString(left, "-nostdlib", TextCompare: false) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public override void PrintLogo(TextWriter consoleOutput)
		{
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LogoLine1, Culture), GetToolName(), GetCompilerVersion());
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LogoLine2, Culture));
			consoleOutput.WriteLine();
		}

		internal override string GetToolName()
		{
			return ErrorFactory.IdToString(ERRID.IDS_ToolName, Culture);
		}

		public override void PrintHelp(TextWriter consoleOutput)
		{
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_VBCHelp, Culture));
		}

		public override void PrintLangVersions(TextWriter consoleOutput)
		{
			consoleOutput.WriteLine(ErrorFactory.IdToString(ERRID.IDS_LangVersions, Culture));
			LanguageVersion languageVersion = LanguageVersionFacts.MapSpecifiedToEffectiveVersion(LanguageVersion.Default);
			LanguageVersion languageVersion2 = LanguageVersionFacts.MapSpecifiedToEffectiveVersion(LanguageVersion.Latest);
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = Enum.GetValues(typeof(LanguageVersion)).GetEnumerator();
				while (enumerator.MoveNext())
				{
					LanguageVersion languageVersion3 = (LanguageVersion)Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(enumerator.Current);
					if (languageVersion3 == languageVersion)
					{
						consoleOutput.WriteLine($"{LanguageVersionFacts.ToDisplayString(languageVersion3)} (default)");
					}
					else if (languageVersion3 == languageVersion2)
					{
						consoleOutput.WriteLine($"{LanguageVersionFacts.ToDisplayString(languageVersion3)} (latest)");
					}
					else
					{
						consoleOutput.WriteLine(LanguageVersionFacts.ToDisplayString(languageVersion3));
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			consoleOutput.WriteLine();
		}

		protected override bool TryGetCompilerDiagnosticCode(string diagnosticId, ref uint code)
		{
			return CommonCompiler.TryGetCompilerDiagnosticCode(diagnosticId, "BC", out code);
		}

		protected override void ResolveAnalyzersFromArguments(List<DiagnosticInfo> diagnostics, CommonMessageProvider messageProvider, bool skipAnalyzers, ref ImmutableArray<DiagnosticAnalyzer> analyzers, ref ImmutableArray<ISourceGenerator> generators)
		{
			Arguments.ResolveAnalyzersFromArguments("Visual Basic", diagnostics, messageProvider, base.AssemblyLoader, skipAnalyzers, out analyzers, out generators);
		}

		protected override void ResolveEmbeddedFilesFromExternalSourceDirectives(SyntaxTree tree, SourceReferenceResolver resolver, OrderedSet<string> embeddedFiles, DiagnosticBag diagnostics)
		{
			foreach (ExternalSourceDirectiveTriviaSyntax directive in VisualBasicExtensions.GetDirectives(tree.GetRoot(), (DirectiveTriviaSyntax d) => d.Kind() == SyntaxKind.ExternalSourceDirectiveTrivia))
			{
				if (directive.ExternalSource.IsMissing)
				{
					continue;
				}
				string text = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(directive.ExternalSource.Value);
				if (text != null)
				{
					string text2 = resolver.ResolveReference(text, tree.FilePath);
					if (text2 == null)
					{
						diagnostics.Add(base.MessageProvider.CreateDiagnostic(base.MessageProvider.ERR_FileNotFound, directive.ExternalSource.GetLocation(), text));
					}
					else
					{
						embeddedFiles.Add(text2);
					}
				}
			}
		}

		private protected override Compilation RunGenerators(Compilation input, ParseOptions parseOptions, ImmutableArray<ISourceGenerator> generators, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, ImmutableArray<AdditionalText> additionalTexts, DiagnosticBag diagnostics)
		{
			VisualBasicGeneratorDriver visualBasicGeneratorDriver = VisualBasicGeneratorDriver.Create(generators, additionalTexts, (VisualBasicParseOptions)parseOptions, analyzerConfigOptionsProvider);
			Compilation outputCompilation = null;
			ImmutableArray<Diagnostic> diagnostics2 = default(ImmutableArray<Diagnostic>);
			visualBasicGeneratorDriver.RunGeneratorsAndUpdateCompilation(input, out outputCompilation, out diagnostics2);
			diagnostics.AddRange(diagnostics2);
			return outputCompilation;
		}
	}
}
