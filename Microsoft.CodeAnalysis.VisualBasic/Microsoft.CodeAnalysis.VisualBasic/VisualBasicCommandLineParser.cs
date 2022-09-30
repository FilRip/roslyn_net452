using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public class VisualBasicCommandLineParser : CommandLineParser
	{
		private const string s_win32Manifest = "win32manifest";

		private const string s_win32Icon = "win32icon";

		private const string s_win32Res = "win32resource";

		public static VisualBasicCommandLineParser Default { get; } = new VisualBasicCommandLineParser();


		public static VisualBasicCommandLineParser Script { get; } = new VisualBasicCommandLineParser(isScriptCommandLineParser: true);


		protected override string RegularFileExtension => ".vb";

		protected override string ScriptFileExtension => ".vbx";

		internal VisualBasicCommandLineParser(bool isScriptCommandLineParser = false)
			: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, isScriptCommandLineParser)
		{
		}

		internal sealed override CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory, string sdkDirectoryOpt, string additionalReferenceDirectories)
		{
			return Parse(args, baseDirectory, sdkDirectoryOpt, additionalReferenceDirectories);
		}

		public new VisualBasicCommandLineArguments Parse(IEnumerable<string> args, string baseDirectory, string sdkDirectory, string additionalReferenceDirectories = null)
		{
			List<Diagnostic> list = new List<Diagnostic>();
			List<string> list2 = new List<string>();
			List<string> list3 = (IsScriptCommandLineParser ? new List<string>() : null);
			List<string> responsePaths = new List<string>();
			FlattenArgs(args, list, list2, list3, baseDirectory, responsePaths);
			bool displayLogo = true;
			bool displayHelp = false;
			bool displayVersion = false;
			bool displayLangVersions = false;
			OutputLevel outputLevel = OutputLevel.Normal;
			bool flag = false;
			bool checkOverflow = true;
			bool concurrentBuild = true;
			bool deterministic = false;
			DebugInformationFormat debugInformationFormat = ((!PathUtilities.IsUnixLikePlatform) ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
			bool flag2 = false;
			bool utf8Output = false;
			string outputFileName = null;
			string text = null;
			bool flag3 = false;
			string outputDirectory = baseDirectory;
			string text2 = null;
			ErrorLogOptions errorLogOptions = null;
			bool flag4 = false;
			OutputKind outputKind = OutputKind.ConsoleApplication;
			SubsystemVersion subsystemVersion = SubsystemVersion.None;
			LanguageVersion result = LanguageVersion.Default;
			string mainTypeName = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			bool noWin32Manifest = false;
			List<ResourceDescription> list4 = new List<ResourceDescription>();
			List<CommandLineSourceFile> list5 = new List<CommandLineSourceFile>();
			bool flag5 = false;
			List<CommandLineSourceFile> list6 = new List<CommandLineSourceFile>();
			ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
			List<CommandLineSourceFile> list7 = new List<CommandLineSourceFile>();
			bool flag6 = false;
			Encoding encoding = null;
			SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha256;
			IReadOnlyDictionary<string, object> readOnlyDictionary = null;
			List<CommandLineReference> list8 = new List<CommandLineReference>();
			List<CommandLineAnalyzerReference> list9 = new List<CommandLineAnalyzerReference>();
			List<string> list10 = new List<string>();
			List<string> list11 = new List<string>();
			List<string> list12 = new List<string>();
			List<string> list13 = new List<string>();
			List<GlobalImport> globalImports = new List<GlobalImport>();
			string text6 = "";
			OptionStrict optionStrict = OptionStrict.Off;
			bool optionInfer = false;
			bool optionExplicit = true;
			bool optionCompareText = false;
			bool embedVbCoreRuntime = false;
			Platform platform = Platform.AnyCpu;
			CultureInfo cultureInfo = null;
			int fileAlignment = 0;
			ulong baseAddress = 0uL;
			bool highEntropyVirtualAddressSpace = false;
			string text7 = null;
			bool flag7 = true;
			ReportDiagnostic reportDiagnostic = ReportDiagnostic.Default;
			ImmutableArray<KeyValuePair<string, string>> immutableArray = ImmutableArray<KeyValuePair<string, string>>.Empty;
			Dictionary<string, ReportDiagnostic> diagnosticOptions = new Dictionary<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
			Dictionary<string, ReportDiagnostic> dictionary = new Dictionary<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
			Dictionary<string, ReportDiagnostic> dictionary2 = new Dictionary<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
			Dictionary<string, ReportDiagnostic> dictionary3 = new Dictionary<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
			string text8 = null;
			string cryptoKeyContainer = null;
			bool? delaySign = null;
			string moduleAssemblyName = null;
			string moduleName = null;
			string touchedFilesPath = null;
			List<string> list14 = new List<string>();
			bool reportAnalyzer = false;
			bool skipAnalyzers = false;
			bool flag8 = false;
			bool flag9 = false;
			ArrayBuilder<InstrumentationKind> instance2 = ArrayBuilder<InstrumentationKind>.GetInstance();
			string text9 = null;
			string text10 = null;
			if (!IsScriptCommandLineParser)
			{
				foreach (string item in list2)
				{
					string name = null;
					string value = null;
					if (CommandLineParser.TryParseOption(item, out name, out value) && EmbeddedOperators.CompareString(name, "ruleset", TextCompare: false) == 0)
					{
						string text11 = CommandLineParser.RemoveQuotesAndSlashes(value);
						if (string.IsNullOrEmpty(text11))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name, ":<file>");
						}
						else
						{
							text10 = ParseGenericPathToFile(text11, list, baseDirectory);
							reportDiagnostic = GetDiagnosticOptionsFromRulesetFile(text10, out diagnosticOptions, list);
						}
					}
				}
			}
			bool flag10 = default(bool);
			foreach (string item2 in list2)
			{
				string name2 = null;
				string value2 = null;
				if (!CommandLineParser.TryParseOption(item2, out name2, out value2))
				{
					foreach (string item3 in ParseFileArgument(item2, baseDirectory, list))
					{
						list5.Add(ToCommandLineSourceFile(item3));
					}
					flag5 = true;
					continue;
				}
				switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(name2))
				{
				case 973910158u:
					if (EmbeddedOperators.CompareString(name2, "?", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07b6;
				case 946971642u:
					if (EmbeddedOperators.CompareString(name2, "help", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07b6;
				case 1181855383u:
					if (EmbeddedOperators.CompareString(name2, "version", TextCompare: false) != 0 || value2 != null)
					{
						break;
					}
					displayVersion = true;
					continue;
				case 4144776981u:
					if (EmbeddedOperators.CompareString(name2, "r", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07d4;
				case 1518465946u:
					if (EmbeddedOperators.CompareString(name2, "reference", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07d4;
				case 3826002220u:
					if (EmbeddedOperators.CompareString(name2, "a", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07eb;
				case 3068394867u:
					if (EmbeddedOperators.CompareString(name2, "analyzer", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07eb;
				case 3775669363u:
					if (EmbeddedOperators.CompareString(name2, "d", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0801;
				case 1788839250u:
					if (EmbeddedOperators.CompareString(name2, "define", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0801;
				case 2902570725u:
					if (EmbeddedOperators.CompareString(name2, "imports", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_084a;
				case 288002260u:
					if (EmbeddedOperators.CompareString(name2, "import", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_084a;
				case 1600842985u:
					if (EmbeddedOperators.CompareString(name2, "optionstrict", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					if (value2 == null)
					{
						optionStrict = OptionStrict.On;
						continue;
					}
					if (string.Equals(value2, "custom", StringComparison.OrdinalIgnoreCase))
					{
						optionStrict = OptionStrict.Custom;
						continue;
					}
					AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "optionstrict", ":custom");
					continue;
				case 4149392742u:
					if (EmbeddedOperators.CompareString(name2, "optionstrict+", TextCompare: false) != 0)
					{
						break;
					}
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optionstrict");
					}
					else
					{
						optionStrict = OptionStrict.On;
					}
					continue;
				case 4182947980u:
					if (EmbeddedOperators.CompareString(name2, "optionstrict-", TextCompare: false) != 0)
					{
						break;
					}
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optionstrict");
					}
					else
					{
						optionStrict = OptionStrict.Off;
					}
					continue;
				case 3421630771u:
					if (EmbeddedOperators.CompareString(name2, "optioncompare", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					if (value2 == null)
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "optioncompare", ":binary|text");
					}
					else if (string.Equals(value2, "text", StringComparison.OrdinalIgnoreCase))
					{
						optionCompareText = true;
					}
					else if (string.Equals(value2, "binary", StringComparison.OrdinalIgnoreCase))
					{
						optionCompareText = false;
					}
					else
					{
						AddDiagnostic(list, ERRID.ERR_InvalidSwitchValue, "optioncompare", value2);
					}
					continue;
				case 2788672424u:
					if (EmbeddedOperators.CompareString(name2, "optionexplicit", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_09c5;
				case 751355705u:
					if (EmbeddedOperators.CompareString(name2, "optionexplicit+", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_09c5;
				case 784910943u:
					if (EmbeddedOperators.CompareString(name2, "optionexplicit-", TextCompare: false) != 0)
					{
						break;
					}
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optionexplicit");
					}
					else
					{
						optionExplicit = false;
					}
					continue;
				case 3811183436u:
					if (EmbeddedOperators.CompareString(name2, "optioninfer", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0a19;
				case 36696869u:
					if (EmbeddedOperators.CompareString(name2, "optioninfer+", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0a19;
				case 4230998451u:
					if (EmbeddedOperators.CompareString(name2, "optioninfer-", TextCompare: false) != 0)
					{
						break;
					}
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optioninfer");
					}
					else
					{
						optionInfer = false;
					}
					continue;
				case 2573005743u:
				{
					if (EmbeddedOperators.CompareString(name2, "codepage", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "codepage", ":<number>");
						continue;
					}
					Encoding encoding2 = CommandLineParser.TryParseEncodingName(value2);
					if (encoding2 == null)
					{
						AddDiagnostic(list, ERRID.ERR_BadCodepage, value2);
					}
					else
					{
						encoding = encoding2;
					}
					continue;
				}
				case 1805613231u:
				{
					if (EmbeddedOperators.CompareString(name2, "checksumalgorithm", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "checksumalgorithm", ":<algorithm>");
						continue;
					}
					SourceHashAlgorithm sourceHashAlgorithm = CommandLineParser.TryParseHashAlgorithmName(value2);
					if (sourceHashAlgorithm == SourceHashAlgorithm.None)
					{
						AddDiagnostic(list, ERRID.ERR_BadChecksumAlgorithm, value2);
					}
					else
					{
						checksumAlgorithm = sourceHashAlgorithm;
					}
					continue;
				}
				case 1151312207u:
					if (EmbeddedOperators.CompareString(name2, "removeintchecks", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0b3f;
				case 1800081516u:
					if (EmbeddedOperators.CompareString(name2, "removeintchecks+", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0b3f;
				case 1766526278u:
					if (EmbeddedOperators.CompareString(name2, "removeintchecks-", TextCompare: false) != 0)
					{
						break;
					}
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "removeintchecks");
					}
					else
					{
						checkOverflow = true;
					}
					continue;
				case 1750515365u:
				{
					if (EmbeddedOperators.CompareString(name2, "sqmsessionguid", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					Guid result2;
					if (string.IsNullOrWhiteSpace(value2))
					{
						AddDiagnostic(list, ERRID.ERR_MissingGuidForOption, value2, name2);
					}
					else if (!Guid.TryParse(value2, out result2))
					{
						AddDiagnostic(list, ERRID.ERR_InvalidFormatForGuidForOption, value2, name2);
					}
					continue;
				}
				case 1312070580u:
					if (EmbeddedOperators.CompareString(name2, "preferreduilang", TextCompare: false) != 0)
					{
						break;
					}
					value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<string>");
						continue;
					}
					try
					{
						cultureInfo = new CultureInfo(value2);
						if ((cultureInfo.CultureTypes & CultureTypes.UserCustomCulture) != 0)
						{
							cultureInfo = null;
						}
					}
					catch (CultureNotFoundException ex)
					{
						ProjectData.SetProjectError(ex);
						CultureNotFoundException ex2 = ex;
						ProjectData.ClearProjectError();
					}
					if (cultureInfo == null)
					{
						AddDiagnostic(list, ERRID.WRN_BadUILang, value2);
					}
					continue;
				case 843890604u:
					if (EmbeddedOperators.CompareString(name2, "lib", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0c73;
				case 1163961547u:
					if (EmbeddedOperators.CompareString(name2, "libpath", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_0c73;
				case 4012068264u:
					{
						if (EmbeddedOperators.CompareString(name2, "libpaths", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_0c73;
					}
					IL_0c73:
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<path_list>");
					}
					else
					{
						list11.AddRange(CommandLineParser.ParseSeparatedPaths(value2));
					}
					continue;
					IL_0a19:
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optioninfer");
					}
					else
					{
						optionInfer = true;
					}
					continue;
					IL_0801:
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<symbol_list>");
					}
					else
					{
						IEnumerable<Diagnostic> diagnostics = null;
						readOnlyDictionary = ParseConditionalCompilationSymbols(value2, out diagnostics, readOnlyDictionary);
						list.AddRange(diagnostics);
					}
					continue;
					IL_07b6:
					if (value2 == null)
					{
						displayHelp = true;
						continue;
					}
					break;
					IL_09c5:
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optionexplicit");
					}
					else
					{
						optionExplicit = true;
					}
					continue;
					IL_0b3f:
					if (value2 != null)
					{
						AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "removeintchecks");
					}
					else
					{
						checkOverflow = false;
					}
					continue;
					IL_07eb:
					list9.AddRange(ParseAnalyzers(name2, value2, list));
					continue;
					IL_084a:
					if (string.IsNullOrEmpty(value2))
					{
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, (EmbeddedOperators.CompareString(name2, "import", TextCompare: false) == 0) ? ":<str>" : ":<import_list>");
					}
					else
					{
						ParseGlobalImports(value2, globalImports, list);
					}
					continue;
					IL_07d4:
					list8.AddRange(ParseAssemblyReferences(name2, value2, list, embedInteropTypes: false));
					continue;
				}
				if (IsScriptCommandLineParser)
				{
					if (EmbeddedOperators.CompareString(name2, "-", TextCompare: false) == 0)
					{
						if (Console.IsInputRedirected)
						{
							list5.Add(new CommandLineSourceFile("-", isScript: true, isInputRedirected: true));
							flag5 = true;
						}
						else
						{
							AddDiagnostic(list, ERRID.ERR_StdInOptionProvidedButConsoleInputIsNotRedirected);
						}
						continue;
					}
					if (EmbeddedOperators.CompareString(name2, "i", TextCompare: false) == 0 || EmbeddedOperators.CompareString(name2, "i+", TextCompare: false) == 0)
					{
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "i");
						}
						flag9 = true;
						continue;
					}
					if (EmbeddedOperators.CompareString(name2, "i-", TextCompare: false) == 0)
					{
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "i");
						}
						flag9 = false;
						continue;
					}
					if (EmbeddedOperators.CompareString(name2, "loadpath", TextCompare: false) == 0 || EmbeddedOperators.CompareString(name2, "loadpaths", TextCompare: false) == 0)
					{
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<path_list>");
						}
						else
						{
							list12.AddRange(CommandLineParser.ParseSeparatedPaths(value2));
						}
						continue;
					}
				}
				else
				{
					ResourceDescription resourceDescription;
					ResourceDescription resourceDescription2;
					switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(name2))
					{
					case 2870621791u:
						if (EmbeddedOperators.CompareString(name2, "out", TextCompare: false) != 0)
						{
							break;
						}
						if (string.IsNullOrWhiteSpace(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<file>");
						}
						else
						{
							ParseOutputFile(value2, list, baseDirectory, out outputFileName, out outputDirectory);
						}
						continue;
					case 81969206u:
					{
						if (EmbeddedOperators.CompareString(name2, "refout", TextCompare: false) != 0)
						{
							break;
						}
						string text12 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(text12))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<file>");
						}
						else
						{
							text = ParseGenericPathToFile(text12, list, baseDirectory);
						}
						continue;
					}
					case 3286799396u:
						if (EmbeddedOperators.CompareString(name2, "refonly", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_1e59;
					case 1981879197u:
						if (EmbeddedOperators.CompareString(name2, "refonly+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_1e59;
					case 4044111267u:
						if (EmbeddedOperators.CompareString(name2, "t", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_1e7e;
					case 845187144u:
						if (EmbeddedOperators.CompareString(name2, "target", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_1e7e;
					case 2714222702u:
					{
						if (EmbeddedOperators.CompareString(name2, "moduleassemblyname", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						AssemblyIdentity identity = null;
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "moduleassemblyname", ":<string>");
						}
						else if (!AssemblyIdentity.TryParseDisplayName(value2, out identity) || !MetadataHelpers.IsValidAssemblyOrModuleName(identity.Name))
						{
							AddDiagnostic(list, ERRID.ERR_InvalidAssemblyName, value2, item2);
						}
						else
						{
							moduleAssemblyName = identity.Name;
						}
						continue;
					}
					case 87639072u:
						if (EmbeddedOperators.CompareString(name2, "rootnamespace", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "rootnamespace", ":<string>");
						}
						else
						{
							text6 = value2;
						}
						continue;
					case 3932734293u:
					{
						if (EmbeddedOperators.CompareString(name2, "doc", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						flag4 = true;
						if (value2 == null)
						{
							text2 = "USE-OUTPUT-NAME";
							continue;
						}
						string text14 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (text14.Length == 0)
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "doc", ":<file>");
							continue;
						}
						text2 = ParseGenericPathToFile(text14, list, baseDirectory, generateDiagnostic: false);
						if (string.IsNullOrWhiteSpace(text2))
						{
							AddDiagnostic(list, ERRID.WRN_XMLCannotWriteToXMLDocFile2, text14, new LocalizableErrorArgument(ERRID.IDS_TheSystemCannotFindThePathSpecified));
							text2 = null;
						}
						continue;
					}
					case 2162933594u:
						if (EmbeddedOperators.CompareString(name2, "doc+", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "doc");
						}
						text2 = "USE-OUTPUT-NAME";
						flag4 = true;
						continue;
					case 2062267880u:
						if (EmbeddedOperators.CompareString(name2, "doc-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "doc");
						}
						text2 = null;
						flag4 = false;
						continue;
					case 1810949389u:
					{
						if (EmbeddedOperators.CompareString(name2, "errorlog", TextCompare: false) != 0)
						{
							break;
						}
						string text16 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrWhiteSpace(text16))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "errorlog", "<file>[,version={1|1.0|2|2.1}]");
							continue;
						}
						errorLogOptions = ParseErrorLogOptions(text16, list, baseDirectory, out var diagnosticAlreadyReported);
						if (errorLogOptions == null && !diagnosticAlreadyReported)
						{
							AddDiagnostic(list, ERRID.ERR_BadSwitchValue, text16, "errorlog", "<file>[,version={1|1.0|2|2.1}]");
						}
						continue;
					}
					case 631775621u:
						if (EmbeddedOperators.CompareString(name2, "netcf", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 365140172u:
						if (EmbeddedOperators.CompareString(name2, "sdkpath", TextCompare: false) != 0)
						{
							break;
						}
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "sdkpath", ":<path>");
						}
						else
						{
							list10.Clear();
							list10.AddRange(CommandLineParser.ParseSeparatedPaths(value2));
						}
						continue;
					case 3697880417u:
						if (EmbeddedOperators.CompareString(name2, "nosdkpath", TextCompare: false) != 0)
						{
							break;
						}
						sdkDirectory = null;
						list10.Clear();
						continue;
					case 2590505776u:
						if (EmbeddedOperators.CompareString(name2, "instrument", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "instrument", ":<string>");
							continue;
						}
						foreach (InstrumentationKind item4 in ParseInstrumentationKinds(value2, list))
						{
							if (!instance2.Contains(item4))
							{
								instance2.Add(item4);
							}
						}
						continue;
					case 2644754498u:
					{
						if (EmbeddedOperators.CompareString(name2, "recurse", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "recurse", ":<wildcard>");
							continue;
						}
						int count = list5.Count;
						list5.AddRange(ParseRecurseArgument(value2, baseDirectory, list));
						if (list5.Count > count)
						{
							flag5 = true;
						}
						continue;
					}
					case 2138038108u:
						if (EmbeddedOperators.CompareString(name2, "addmodule", TextCompare: false) != 0)
						{
							break;
						}
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "addmodule", ":<file_list>");
						}
						else
						{
							list8.AddRange(from path in CommandLineParser.ParseSeparatedPaths(value2)
								select new CommandLineReference(path, new MetadataReferenceProperties(MetadataImageKind.Module)));
						}
						continue;
					case 3909890315u:
						if (EmbeddedOperators.CompareString(name2, "l", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2279;
					case 232457833u:
						if (EmbeddedOperators.CompareString(name2, "link", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2279;
					case 1065140344u:
						if (EmbeddedOperators.CompareString(name2, "win32resource", TextCompare: false) != 0)
						{
							break;
						}
						text4 = GetWin32Setting("win32resource", CommandLineParser.RemoveQuotesAndSlashes(value2), list);
						continue;
					case 2104310437u:
						if (EmbeddedOperators.CompareString(name2, "win32icon", TextCompare: false) != 0)
						{
							break;
						}
						text5 = GetWin32Setting("win32icon", CommandLineParser.RemoveQuotesAndSlashes(value2), list);
						continue;
					case 632330593u:
						if (EmbeddedOperators.CompareString(name2, "win32manifest", TextCompare: false) != 0)
						{
							break;
						}
						text3 = GetWin32Setting("win32manifest", CommandLineParser.RemoveQuotesAndSlashes(value2), list);
						continue;
					case 706876140u:
						if (EmbeddedOperators.CompareString(name2, "nowin32manifest", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						noWin32Manifest = true;
						continue;
					case 804546073u:
						if (EmbeddedOperators.CompareString(name2, "res", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_22ea;
					case 702513141u:
						if (EmbeddedOperators.CompareString(name2, "resource", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_22ea;
					case 2695268429u:
						if (EmbeddedOperators.CompareString(name2, "linkres", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_230d;
					case 1370291513u:
						if (EmbeddedOperators.CompareString(name2, "linkresource", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_230d;
					case 3116353128u:
						if (EmbeddedOperators.CompareString(name2, "sourcelink", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "sourcelink", ":<file>");
						}
						else
						{
							text9 = ParseGenericPathToFile(value2, list, baseDirectory);
						}
						continue;
					case 1483009432u:
						if (EmbeddedOperators.CompareString(name2, "debug", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (value2 != null)
						{
							string left = value2.ToLower();
							if (EmbeddedOperators.CompareString(left, "full", TextCompare: false) != 0 && EmbeddedOperators.CompareString(left, "pdbonly", TextCompare: false) != 0)
							{
								if (EmbeddedOperators.CompareString(left, "portable", TextCompare: false) != 0)
								{
									if (EmbeddedOperators.CompareString(left, "embedded", TextCompare: false) == 0)
									{
										debugInformationFormat = DebugInformationFormat.Embedded;
									}
									else
									{
										AddDiagnostic(list, ERRID.ERR_InvalidSwitchValue, "debug", value2);
									}
								}
								else
								{
									debugInformationFormat = DebugInformationFormat.PortablePdb;
								}
							}
							else
							{
								debugInformationFormat = ((!PathUtilities.IsUnixLikePlatform) ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
							}
						}
						flag10 = true;
						continue;
					case 3655479497u:
						if (EmbeddedOperators.CompareString(name2, "debug+", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "debug");
						}
						flag10 = true;
						continue;
					case 3689034735u:
						if (EmbeddedOperators.CompareString(name2, "debug-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "debug");
						}
						flag10 = false;
						continue;
					case 4192678414u:
						if (EmbeddedOperators.CompareString(name2, "optimize", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2457;
					case 2348019775u:
						if (EmbeddedOperators.CompareString(name2, "optimize+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2457;
					case 2314464537u:
						if (EmbeddedOperators.CompareString(name2, "optimize-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optimize");
						}
						else
						{
							flag = false;
						}
						continue;
					case 3638623282u:
						if (EmbeddedOperators.CompareString(name2, "parallel", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_24ab;
					case 4111221743u:
						if (EmbeddedOperators.CompareString(name2, "p", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_24ab;
					case 2793360781u:
						if (EmbeddedOperators.CompareString(name2, "deterministic", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_24d2;
					case 3227991122u:
						if (EmbeddedOperators.CompareString(name2, "deterministic+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_24d2;
					case 3127325408u:
						if (EmbeddedOperators.CompareString(name2, "deterministic-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2);
						}
						else
						{
							deterministic = false;
						}
						continue;
					case 2200755035u:
						if (EmbeddedOperators.CompareString(name2, "parallel+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2520;
					case 2253303180u:
						if (EmbeddedOperators.CompareString(name2, "p+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2520;
					case 2301420749u:
						if (EmbeddedOperators.CompareString(name2, "parallel-", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2556;
					case 2219747942u:
						if (EmbeddedOperators.CompareString(name2, "p-", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2556;
					case 3056178685u:
						if (EmbeddedOperators.CompareString(name2, "warnaserror", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_258c;
					case 2574704610u:
						if (EmbeddedOperators.CompareString(name2, "warnaserror+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_258c;
					case 2474038896u:
						if (EmbeddedOperators.CompareString(name2, "warnaserror-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 == null)
						{
							if (reportDiagnostic != ReportDiagnostic.Suppress)
							{
								reportDiagnostic = ReportDiagnostic.Default;
							}
							dictionary.Clear();
							continue;
						}
						foreach (string item5 in ParseWarnings(value2))
						{
							if (diagnosticOptions.TryGetValue(item5, out var value3))
							{
								dictionary2[item5] = value3;
							}
							else
							{
								dictionary2[item5] = ReportDiagnostic.Default;
							}
						}
						continue;
					case 3562318266u:
						if (EmbeddedOperators.CompareString(name2, "nowarn", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 == null)
						{
							reportDiagnostic = ReportDiagnostic.Suppress;
							dictionary.Clear();
							foreach (KeyValuePair<string, ReportDiagnostic> item6 in diagnosticOptions)
							{
								if (item6.Value != ReportDiagnostic.Error)
								{
									dictionary.Add(item6.Key, ReportDiagnostic.Suppress);
								}
							}
						}
						else
						{
							AddWarnings(dictionary3, ReportDiagnostic.Suppress, ParseWarnings(value2));
						}
						continue;
					case 3089736743u:
						if (EmbeddedOperators.CompareString(name2, "langversion", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "langversion", ":<number>");
						}
						else if (EmbeddedOperators.CompareString(value2, "?", TextCompare: false) == 0)
						{
							displayLangVersions = true;
						}
						else if (!LanguageVersionFacts.TryParse(value2, ref result))
						{
							AddDiagnostic(list, ERRID.ERR_InvalidSwitchValue, "langversion", value2);
						}
						continue;
					case 3589461161u:
						if (EmbeddedOperators.CompareString(name2, "delaysign", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2752;
					case 1329891494u:
						if (EmbeddedOperators.CompareString(name2, "delaysign+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2752;
					case 1363446732u:
						if (EmbeddedOperators.CompareString(name2, "delaysign-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "delaysign");
						}
						else
						{
							delaySign = false;
						}
						continue;
					case 2559445937u:
						if (EmbeddedOperators.CompareString(name2, "publicsign", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_27b0;
					case 3248243566u:
						if (EmbeddedOperators.CompareString(name2, "publicsign+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_27b0;
					case 3281798804u:
						if (EmbeddedOperators.CompareString(name2, "publicsign-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "publicsign");
						}
						else
						{
							flag8 = false;
						}
						continue;
					case 447319523u:
						if (EmbeddedOperators.CompareString(name2, "keycontainer", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						text8 = null;
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "keycontainer", ":<string>");
						}
						else
						{
							cryptoKeyContainer = value2;
						}
						continue;
					case 638064026u:
						if (EmbeddedOperators.CompareString(name2, "keyfile", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						cryptoKeyContainer = null;
						if (string.IsNullOrWhiteSpace(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "keyfile", ":<file>");
						}
						else
						{
							text8 = value2;
						}
						continue;
					case 2177680879u:
						if (EmbeddedOperators.CompareString(name2, "highentropyva", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_288c;
					case 425415564u:
						if (EmbeddedOperators.CompareString(name2, "highentropyva+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_288c;
					case 391860326u:
						if (EmbeddedOperators.CompareString(name2, "highentropyva-", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						highEntropyVirtualAddressSpace = false;
						continue;
					case 4007191885u:
						if (EmbeddedOperators.CompareString(name2, "nologo", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_28aa;
					case 1701912466u:
						if (EmbeddedOperators.CompareString(name2, "nologo+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_28aa;
					case 1601246752u:
						if (EmbeddedOperators.CompareString(name2, "nologo-", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						displayLogo = true;
						continue;
					case 3988657268u:
						if (EmbeddedOperators.CompareString(name2, "quiet+", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "quiet");
						}
						else
						{
							outputLevel = OutputLevel.Quiet;
						}
						continue;
					case 3929354775u:
						if (EmbeddedOperators.CompareString(name2, "quiet", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						outputLevel = OutputLevel.Quiet;
						continue;
					case 3429628189u:
						if (EmbeddedOperators.CompareString(name2, "verbose", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						outputLevel = OutputLevel.Verbose;
						continue;
					case 66670594u:
						if (EmbeddedOperators.CompareString(name2, "verbose+", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "verbose");
						}
						else
						{
							outputLevel = OutputLevel.Verbose;
						}
						continue;
					case 3955102030u:
						if (EmbeddedOperators.CompareString(name2, "quiet-", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_293a;
					case 4260972176u:
						if (EmbeddedOperators.CompareString(name2, "verbose-", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_293a;
					case 3598478835u:
						if (EmbeddedOperators.CompareString(name2, "utf8output", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2970;
					case 2111892232u:
						if (EmbeddedOperators.CompareString(name2, "utf8output+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2970;
					case 2212557946u:
						if (EmbeddedOperators.CompareString(name2, "utf8output-", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "utf8output");
						}
						utf8Output = false;
						continue;
					case 625233524u:
						if (EmbeddedOperators.CompareString(name2, "noconfig", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 3283884827u:
						if (EmbeddedOperators.CompareString(name2, "bugreport", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 1475868319u:
						if (EmbeddedOperators.CompareString(name2, "errorreport", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 4087352963u:
						if (EmbeddedOperators.CompareString(name2, "novbruntimeref", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 3893112696u:
						if (EmbeddedOperators.CompareString(name2, "m", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_29ba;
					case 3935363592u:
						if (EmbeddedOperators.CompareString(name2, "main", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_29ba;
					case 2458330400u:
					{
						if (EmbeddedOperators.CompareString(name2, "subsystemversion", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<version>");
							continue;
						}
						SubsystemVersion version = default(SubsystemVersion);
						if (SubsystemVersion.TryParse(value2, out version))
						{
							subsystemVersion = version;
							continue;
						}
						AddDiagnostic(list, ERRID.ERR_InvalidSubsystemVersion, value2);
						continue;
					}
					case 738948484u:
					{
						if (EmbeddedOperators.CompareString(name2, "touchedfiles", TextCompare: false) != 0)
						{
							break;
						}
						string text15 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(text15))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<touchedfiles>");
						}
						else
						{
							touchedFilesPath = text15;
						}
						continue;
					}
					case 3612909048u:
						if (EmbeddedOperators.CompareString(name2, "fullpaths", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2aa2;
					case 1248630781u:
						if (EmbeddedOperators.CompareString(name2, "errorendlocation", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2aa2;
					case 3257533744u:
						if (EmbeddedOperators.CompareString(name2, "pathmap", TextCompare: false) == 0)
						{
							string text13 = CommandLineParser.RemoveQuotesAndSlashes(value2);
							if (EmbeddedOperators.CompareString(text13, null, TextCompare: false) != 0)
							{
								immutableArray = immutableArray.Concat<KeyValuePair<string, string>>(ParsePathMap(text13, list));
								continue;
							}
						}
						break;
					case 1026574197u:
						if (EmbeddedOperators.CompareString(name2, "reportanalyzer", TextCompare: false) != 0)
						{
							break;
						}
						reportAnalyzer = true;
						continue;
					case 3068697161u:
						if (EmbeddedOperators.CompareString(name2, "skipanalyzers", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2ae5;
					case 1378551878u:
						if (EmbeddedOperators.CompareString(name2, "skipanalyzers+", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_2ae5;
					case 1412107116u:
						if (EmbeddedOperators.CompareString(name2, "skipanalyzers-", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						skipAnalyzers = false;
						continue;
					case 2912915442u:
						if (EmbeddedOperators.CompareString(name2, "nostdlib", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						flag2 = true;
						continue;
					case 4052595223u:
						if (EmbeddedOperators.CompareString(name2, "vbruntime", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 != null)
						{
							text7 = CommandLineParser.RemoveQuotesAndSlashes(value2);
							flag7 = true;
							embedVbCoreRuntime = false;
							continue;
						}
						goto IL_2b31;
					case 2114950260u:
						if (EmbeddedOperators.CompareString(name2, "vbruntime+", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						goto IL_2b31;
					case 2081395022u:
						if (EmbeddedOperators.CompareString(name2, "vbruntime-", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						text7 = null;
						flag7 = false;
						embedVbCoreRuntime = false;
						continue;
					case 2131727879u:
						if (EmbeddedOperators.CompareString(name2, "vbruntime*", TextCompare: false) != 0 || value2 != null)
						{
							break;
						}
						text7 = null;
						flag7 = false;
						embedVbCoreRuntime = true;
						continue;
					case 709505714u:
						if (EmbeddedOperators.CompareString(name2, "platform", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (value2 != null)
						{
							platform = ParsePlatform(name2, value2, list);
							continue;
						}
						AddDiagnostic(list, ERRID.ERR_ArgumentRequired, "platform", ":<string>");
						continue;
					case 2128741192u:
						if (EmbeddedOperators.CompareString(name2, "filealign", TextCompare: false) != 0)
						{
							break;
						}
						fileAlignment = ParseFileAlignment(name2, CommandLineParser.RemoveQuotesAndSlashes(value2), list);
						continue;
					case 514869512u:
						if (EmbeddedOperators.CompareString(name2, "baseaddress", TextCompare: false) != 0)
						{
							break;
						}
						baseAddress = ParseBaseAddress(name2, CommandLineParser.RemoveQuotesAndSlashes(value2), list);
						continue;
					case 1369110961u:
						if (EmbeddedOperators.CompareString(name2, "ruleset", TextCompare: false) == 0)
						{
							continue;
						}
						break;
					case 1602454474u:
						if (EmbeddedOperators.CompareString(name2, "features", TextCompare: false) != 0)
						{
							break;
						}
						if (value2 == null)
						{
							list14.Clear();
						}
						else
						{
							list14.Add(CommandLineParser.RemoveQuotesAndSlashes(value2));
						}
						continue;
					case 847356170u:
						if (EmbeddedOperators.CompareString(name2, "additionalfile", TextCompare: false) != 0)
						{
							break;
						}
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<file_list>");
							continue;
						}
						foreach (string item7 in ParseSeparatedFileArgument(value2, baseDirectory, list))
						{
							list6.Add(ToCommandLineSourceFile(item7));
						}
						continue;
					case 4036644977u:
						if (EmbeddedOperators.CompareString(name2, "analyzerconfig", TextCompare: false) != 0)
						{
							break;
						}
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<file_list>");
						}
						else
						{
							instance.AddRange(ParseSeparatedFileArgument(value2, baseDirectory, list));
						}
						continue;
					case 672900686u:
						if (EmbeddedOperators.CompareString(name2, "embed", TextCompare: false) != 0)
						{
							break;
						}
						if (string.IsNullOrEmpty(value2))
						{
							flag6 = true;
							continue;
						}
						foreach (string item8 in ParseSeparatedFileArgument(value2, baseDirectory, list))
						{
							list7.Add(ToCommandLineSourceFile(item8));
						}
						continue;
					case 671913016u:
						{
							if (EmbeddedOperators.CompareString(name2, "-", TextCompare: false) != 0)
							{
								break;
							}
							if (Console.IsInputRedirected)
							{
								list5.Add(new CommandLineSourceFile("-", isScript: false, isInputRedirected: true));
								flag5 = true;
							}
							else
							{
								AddDiagnostic(list, ERRID.ERR_StdInOptionProvidedButConsoleInputIsNotRedirected);
							}
							continue;
						}
						IL_22ea:
						resourceDescription = ParseResourceDescription(name2, value2, baseDirectory, list, embedded: true);
						if (resourceDescription != null)
						{
							list4.Add(resourceDescription);
						}
						continue;
						IL_2457:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "optimize");
						}
						else
						{
							flag = true;
						}
						continue;
						IL_27b0:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "publicsign");
						}
						else
						{
							flag8 = true;
						}
						continue;
						IL_2752:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "delaysign");
						}
						else
						{
							delaySign = true;
						}
						continue;
						IL_1e59:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "refonly");
						}
						flag3 = true;
						continue;
						IL_2b31:
						text7 = null;
						flag7 = true;
						embedVbCoreRuntime = false;
						continue;
						IL_2279:
						list8.AddRange(ParseAssemblyReferences(name2, value2, list, embedInteropTypes: true));
						continue;
						IL_2ae5:
						if (value2 == null)
						{
							skipAnalyzers = true;
							continue;
						}
						break;
						IL_2aa2:
						UnimplementedSwitch(list, name2);
						continue;
						IL_1e7e:
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						outputKind = ParseTarget(name2, value2, list);
						continue;
						IL_29ba:
						value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
						if (string.IsNullOrEmpty(value2))
						{
							AddDiagnostic(list, ERRID.ERR_ArgumentRequired, name2, ":<class>");
						}
						else
						{
							mainTypeName = value2;
						}
						continue;
						IL_258c:
						if (value2 == null)
						{
							reportDiagnostic = ReportDiagnostic.Error;
							dictionary.Clear();
							foreach (KeyValuePair<string, ReportDiagnostic> item9 in diagnosticOptions)
							{
								if (item9.Value == ReportDiagnostic.Warn)
								{
									dictionary.Add(item9.Key, ReportDiagnostic.Error);
								}
							}
						}
						else
						{
							AddWarnings(dictionary2, ReportDiagnostic.Error, ParseWarnings(value2));
						}
						continue;
						IL_2970:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, "utf8output");
						}
						utf8Output = true;
						continue;
						IL_2556:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2.Substring(0, name2.Length - 1));
						}
						else
						{
							concurrentBuild = false;
						}
						continue;
						IL_293a:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2.Substring(0, name2.Length - 1));
						}
						else
						{
							outputLevel = OutputLevel.Normal;
						}
						continue;
						IL_2520:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2.Substring(0, name2.Length - 1));
						}
						else
						{
							concurrentBuild = true;
						}
						continue;
						IL_24d2:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2);
						}
						else
						{
							deterministic = true;
						}
						continue;
						IL_230d:
						resourceDescription2 = ParseResourceDescription(name2, value2, baseDirectory, list, embedded: false);
						if (resourceDescription2 != null)
						{
							list4.Add(resourceDescription2);
						}
						continue;
						IL_28aa:
						if (value2 == null)
						{
							displayLogo = false;
							continue;
						}
						break;
						IL_288c:
						if (value2 == null)
						{
							highEntropyVirtualAddressSpace = true;
							continue;
						}
						break;
						IL_24ab:
						if (value2 != null)
						{
							AddDiagnostic(list, ERRID.ERR_SwitchNeedsBool, name2);
						}
						else
						{
							concurrentBuild = true;
						}
						continue;
					}
				}
				AddDiagnostic(list, ERRID.WRN_BadSwitch, item2);
			}
			Dictionary<string, ReportDiagnostic> dictionary4 = new Dictionary<string, ReportDiagnostic>(diagnosticOptions, CaseInsensitiveComparison.Comparer);
			foreach (KeyValuePair<string, ReportDiagnostic> item10 in dictionary)
			{
				dictionary4[item10.Key] = item10.Value;
			}
			foreach (KeyValuePair<string, ReportDiagnostic> item11 in dictionary2)
			{
				dictionary4[item11.Key] = item11.Value;
			}
			foreach (KeyValuePair<string, ReportDiagnostic> item12 in dictionary3)
			{
				dictionary4[item12.Key] = item12.Value;
			}
			if (flag3 && text != null)
			{
				AddDiagnostic(list, ERRID.ERR_NoRefOutWhenRefOnly);
			}
			if (outputKind == OutputKind.NetModule && (flag3 || text != null))
			{
				AddDiagnostic(list, ERRID.ERR_NoNetModuleOutputWhenRefOutOrRefOnly);
			}
			if (!IsScriptCommandLineParser && !flag5 && list4.IsEmpty())
			{
				if (list2.Any())
				{
					AddDiagnostic(list, ERRID.ERR_NoSources);
				}
				else
				{
					displayHelp = true;
				}
			}
			if (sdkDirectory != null && list10.Count == 0)
			{
				list10.Add(sdkDirectory);
			}
			CommandLineReference? defaultCoreLibraryReference = LoadCoreLibraryReference(list10, baseDirectory);
			if (!flag2)
			{
				string text17 = FindFileInSdkPath(list10, "System.dll", baseDirectory);
				if (text17 == null)
				{
					AddDiagnostic(list, ERRID.WRN_CannotFindStandardLibrary1, "System.dll");
				}
				else
				{
					list8.Add(new CommandLineReference(text17, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false)));
				}
			}
			if (flag7)
			{
				if (text7 == null)
				{
					string text18 = FindFileInSdkPath(list10, "Microsoft.VisualBasic.dll", baseDirectory);
					if (text18 == null)
					{
						AddDiagnostic(list, ERRID.ERR_LibNotFound, "Microsoft.VisualBasic.dll");
					}
					else
					{
						list8.Add(new CommandLineReference(text18, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false)));
					}
				}
				else
				{
					list8.Add(new CommandLineReference(text7, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false)));
				}
			}
			if (!string.IsNullOrEmpty(additionalReferenceDirectories))
			{
				list11.AddRange(CommandLineParser.ParseSeparatedPaths(additionalReferenceDirectories));
			}
			ImmutableArray<string> referencePaths = BuildSearchPaths(baseDirectory, list10, responsePaths, list11);
			if (flag8 && !string.IsNullOrEmpty(text8))
			{
				text8 = ParseGenericPathToFile(text8, list, baseDirectory);
			}
			ValidateWin32Settings(noWin32Manifest, text4, text5, text3, outputKind, list);
			if (text9 != null && !flag10)
			{
				AddDiagnostic(list, ERRID.ERR_SourceLinkRequiresPdb);
			}
			if (flag6)
			{
				list7.AddRange(list5);
			}
			if (list7.Count > 0 && !flag10)
			{
				AddDiagnostic(list, ERRID.ERR_CannotEmbedWithoutPdb);
			}
			if (!string.Empty.Equals(text6))
			{
				text6 = text6.Unquote();
				if (string.IsNullOrWhiteSpace(text6) || !OptionsValidator.IsValidNamespaceName(text6))
				{
					AddDiagnostic(list, ERRID.ERR_BadNamespaceName1, text6);
					text6 = "";
				}
			}
			if (!string.IsNullOrEmpty(baseDirectory))
			{
				list13.Add(baseDirectory);
			}
			if (!string.IsNullOrEmpty(outputDirectory) && EmbeddedOperators.CompareString(baseDirectory, outputDirectory, TextCompare: false) != 0)
			{
				list13.Add(outputDirectory);
			}
			ImmutableDictionary<string, string> features = CommandLineParser.ParseFeatures(list14);
			string compilationName = null;
			GetCompilationAndModuleNames(list, outputKind, list5, moduleAssemblyName, ref outputFileName, ref moduleName, out compilationName);
			if (!IsScriptCommandLineParser && !flag5 && !list4.IsEmpty() && EmbeddedOperators.CompareString(outputFileName, null, TextCompare: false) == 0 && !list2.IsEmpty())
			{
				AddDiagnostic(list, ERRID.ERR_NoSourcesOut);
			}
			VisualBasicParseOptions parseOptions = new VisualBasicParseOptions(result, flag4 ? DocumentationMode.Diagnose : DocumentationMode.None, IsScriptCommandLineParser ? SourceCodeKind.Script : SourceCodeKind.Regular, PredefinedPreprocessorSymbols.AddPredefinedPreprocessorSymbols(outputKind, readOnlyDictionary.AsImmutableOrEmpty()), features);
			bool reportSuppressedDiagnostics = errorLogOptions != null;
			VisualBasicCompilationOptions visualBasicCompilationOptions = new VisualBasicCompilationOptions(outputKind, moduleName, mainTypeName, "Script", globalImports, text6, optionStrict, optionInfer, optionExplicit, optionCompareText, parseOptions, embedVbCoreRuntime, flag ? OptimizationLevel.Release : OptimizationLevel.Debug, checkOverflow, cryptoKeyContainer, text8, default(ImmutableArray<byte>), delaySign, platform, reportDiagnostic, dictionary4, concurrentBuild, deterministic, null, null, null, null, null, flag8, reportSuppressedDiagnostics, MetadataImportOptions.Public);
			EmitOptions emitOptions = new EmitOptions(flag3, debugInformationFormat, null, null, fileAlignment, baseAddress, highEntropyVirtualAddressSpace, subsystemVersion, null, tolerateErrors: false, !flag3 && text == null, instance2.ToImmutableAndFree(), HashAlgorithmName.SHA256, null, null);
			list.AddRange(visualBasicCompilationOptions.Errors);
			if ((object)text2 == "USE-OUTPUT-NAME")
			{
				text2 = PathUtilities.CombineAbsoluteAndRelativePaths(outputDirectory, PathUtilities.RemoveExtension(outputFileName));
				text2 += ".xml";
			}
			flag9 |= IsScriptCommandLineParser && list5.Count == 0;
			immutableArray = CommandLineParser.SortPathMap(immutableArray);
			return new VisualBasicCommandLineArguments
			{
				IsScriptRunner = IsScriptCommandLineParser,
				InteractiveMode = flag9,
				BaseDirectory = baseDirectory,
				Errors = list.AsImmutable(),
				Utf8Output = utf8Output,
				CompilationName = compilationName,
				OutputFileName = outputFileName,
				OutputRefFilePath = text,
				OutputDirectory = outputDirectory,
				DocumentationPath = text2,
				ErrorLogOptions = errorLogOptions,
				SourceFiles = list5.AsImmutable(),
				PathMap = immutableArray,
				Encoding = encoding,
				ChecksumAlgorithm = checksumAlgorithm,
				MetadataReferences = list8.AsImmutable(),
				AnalyzerReferences = list9.AsImmutable(),
				AdditionalFiles = list6.AsImmutable(),
				AnalyzerConfigPaths = instance.ToImmutableAndFree(),
				ReferencePaths = referencePaths,
				SourcePaths = list12.AsImmutable(),
				KeyFileSearchPaths = list13.AsImmutable(),
				Win32ResourceFile = text4,
				Win32Icon = text5,
				Win32Manifest = text3,
				NoWin32Manifest = noWin32Manifest,
				DisplayLogo = displayLogo,
				DisplayHelp = displayHelp,
				DisplayVersion = displayVersion,
				DisplayLangVersions = displayLangVersions,
				ManifestResources = list4.AsImmutable(),
				CompilationOptions = visualBasicCompilationOptions,
				ParseOptions = parseOptions,
				EmitOptions = emitOptions,
				ScriptArguments = list3.AsImmutableOrEmpty(),
				TouchedFilesPath = touchedFilesPath,
				OutputLevel = outputLevel,
				EmitPdb = (flag10 && !flag3),
				SourceLink = text9,
				RuleSetPath = text10,
				DefaultCoreLibraryReference = defaultCoreLibraryReference,
				PreferredUILang = cultureInfo,
				ReportAnalyzer = reportAnalyzer,
				SkipAnalyzers = skipAnalyzers,
				EmbeddedFiles = list7.AsImmutable()
			};
		}

		private CommandLineReference? LoadCoreLibraryReference(List<string> sdkPaths, string baseDirectory)
		{
			string text = FindFileInSdkPath(sdkPaths, "mscorlib.dll", baseDirectory);
			string text2 = FindFileInSdkPath(sdkPaths, "System.Runtime.dll", baseDirectory);
			CommandLineReference? result;
			if (text2 == null)
			{
				result = ((text == null) ? null : new CommandLineReference?(new CommandLineReference(text, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false))));
			}
			else if (text == null)
			{
				result = new CommandLineReference(text2, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false));
			}
			else
			{
				try
				{
					using AssemblyMetadata assemblyMetadata = AssemblyMetadata.CreateFromFile(text2);
					if (assemblyMetadata.GetModules()[0].Module.IsLinkedModule && assemblyMetadata.GetAssembly()!.AssemblyReferences.Length == 0)
					{
						result = new CommandLineReference(text2, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false));
						return result;
					}
				}
				catch (Exception projectError)
				{
					ProjectData.SetProjectError(projectError);
					ProjectData.ClearProjectError();
				}
				result = new CommandLineReference(text, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes: false));
			}
			return result;
		}

		private static string FindFileInSdkPath(List<string> sdkPaths, string fileName, string baseDirectory)
		{
			foreach (string sdkPath in sdkPaths)
			{
				string text = FileUtilities.ResolveRelativePath(sdkPath, baseDirectory);
				if (text != null)
				{
					string text2 = PathUtilities.CombineAbsoluteAndRelativePaths(text, fileName);
					if (File.Exists(text2))
					{
						return text2;
					}
				}
			}
			return null;
		}

		private static string GetWin32Setting(string arg, string value, List<Diagnostic> diagnostics)
		{
			if (value == null)
			{
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, arg, ":<file>");
			}
			else
			{
				string text = CommandLineParser.RemoveQuotesAndSlashes(value);
				if (!string.IsNullOrWhiteSpace(text))
				{
					return text;
				}
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, arg, ":<file>");
			}
			return null;
		}

		private static ImmutableArray<string> BuildSearchPaths(string baseDirectory, List<string> sdkPaths, List<string> responsePaths, List<string> libPaths)
		{
			ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
			AddNormalizedPaths(instance, sdkPaths, baseDirectory);
			instance.AddRange(responsePaths);
			AddNormalizedPaths(instance, libPaths, baseDirectory);
			return instance.ToImmutableAndFree();
		}

		private static void AddNormalizedPaths(ArrayBuilder<string> builder, List<string> paths, string baseDirectory)
		{
			foreach (string path in paths)
			{
				string text = FileUtilities.NormalizeRelativePath(path, null, baseDirectory);
				if (text != null)
				{
					builder.Add(text);
				}
			}
		}

		private static void ValidateWin32Settings(bool noWin32Manifest, string win32ResSetting, string win32IconSetting, string win32ManifestSetting, OutputKind outputKind, List<Diagnostic> diagnostics)
		{
			if (noWin32Manifest && win32ManifestSetting != null)
			{
				AddDiagnostic(diagnostics, ERRID.ERR_ConflictingManifestSwitches);
			}
			if (win32ResSetting != null)
			{
				if (win32IconSetting != null)
				{
					AddDiagnostic(diagnostics, ERRID.ERR_IconFileAndWin32ResFile);
				}
				if (win32ManifestSetting != null)
				{
					AddDiagnostic(diagnostics, ERRID.ERR_CantHaveWin32ResAndManifest);
				}
			}
			if (win32ManifestSetting != null && outputKind.IsNetModule())
			{
				AddDiagnostic(diagnostics, ERRID.WRN_IgnoreModuleManifest);
			}
		}

		private static OutputKind ParseTarget(string optionName, string value, IList<Diagnostic> diagnostics)
		{
			string text = (value ?? "").ToLowerInvariant();
			switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(text))
			{
			case 1738962391u:
				if (EmbeddedOperators.CompareString(text, "exe", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.ConsoleApplication;
			case 278472723u:
				if (EmbeddedOperators.CompareString(text, "winexe", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.WindowsApplication;
			case 2432105424u:
				if (EmbeddedOperators.CompareString(text, "library", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.DynamicallyLinkedLibrary;
			case 3617558685u:
				if (EmbeddedOperators.CompareString(text, "module", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.NetModule;
			case 1774589953u:
				if (EmbeddedOperators.CompareString(text, "appcontainerexe", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.WindowsRuntimeApplication;
			case 728968349u:
				if (EmbeddedOperators.CompareString(text, "winmdobj", TextCompare: false) != 0)
				{
					break;
				}
				return OutputKind.WindowsRuntimeMetadata;
			case 2166136261u:
				if (EmbeddedOperators.CompareString(text, "", TextCompare: false) != 0)
				{
					break;
				}
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, optionName, ":exe|winexe|library|module|appcontainerexe|winmdobj");
				return OutputKind.ConsoleApplication;
			}
			AddDiagnostic(diagnostics, ERRID.ERR_InvalidSwitchValue, optionName, value);
			return OutputKind.ConsoleApplication;
		}

		internal static IEnumerable<CommandLineReference> ParseAssemblyReferences(string name, string value, IList<Diagnostic> diagnostics, bool embedInteropTypes)
		{
			if (string.IsNullOrEmpty(value))
			{
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, name, ":<file_list>");
				return SpecializedCollections.EmptyEnumerable<CommandLineReference>();
			}
			return from path in CommandLineParser.ParseSeparatedPaths(value)
				select new CommandLineReference(path, new MetadataReferenceProperties(MetadataImageKind.Assembly, default(ImmutableArray<string>), embedInteropTypes));
		}

		private static IEnumerable<CommandLineAnalyzerReference> ParseAnalyzers(string name, string value, IList<Diagnostic> diagnostics)
		{
			if (string.IsNullOrEmpty(value))
			{
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, name, ":<file_list>");
				return SpecializedCollections.EmptyEnumerable<CommandLineAnalyzerReference>();
			}
			return from path in CommandLineParser.ParseSeparatedPaths(value)
				select new CommandLineAnalyzerReference(path);
		}

		internal static ResourceDescription ParseResourceDescription(string name, string resourceDescriptor, string baseDirectory, IList<Diagnostic> diagnostics, bool embedded)
		{
			if (string.IsNullOrEmpty(resourceDescriptor))
			{
				AddDiagnostic(diagnostics, ERRID.ERR_ArgumentRequired, name, ":<resinfo>");
				return null;
			}
			string filePath = null;
			string fullPath = null;
			string fileName = null;
			string resourceName = null;
			string accessibility = null;
			CommandLineParser.ParseResourceDescription(resourceDescriptor, baseDirectory, skipLeadingSeparators: true, out filePath, out fullPath, out fileName, out resourceName, out accessibility);
			if (string.IsNullOrWhiteSpace(filePath))
			{
				AddInvalidSwitchValueDiagnostic(diagnostics, name, filePath);
				return null;
			}
			if (!PathUtilities.IsValidFilePath(fullPath))
			{
				AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, filePath);
				return null;
			}
			bool isPublic;
			if (string.IsNullOrEmpty(accessibility))
			{
				isPublic = true;
			}
			else if (string.Equals(accessibility, "public", StringComparison.OrdinalIgnoreCase))
			{
				isPublic = true;
			}
			else
			{
				if (!string.Equals(accessibility, "private", StringComparison.OrdinalIgnoreCase))
				{
					AddInvalidSwitchValueDiagnostic(diagnostics, name, accessibility);
					return null;
				}
				isPublic = false;
			}
			Func<Stream> dataProvider = () => new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			return new ResourceDescription(resourceName, fileName, dataProvider, isPublic, embedded, checkArgs: false);
		}

		private static void AddInvalidSwitchValueDiagnostic(IList<Diagnostic> diagnostics, string name, string nullStringText)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "(null)";
			}
			AddDiagnostic(diagnostics, ERRID.ERR_InvalidSwitchValue, name, nullStringText);
		}

		private static void ParseGlobalImports(string value, List<GlobalImport> globalImports, List<Diagnostic> errors)
		{
			IEnumerable<string> enumerable = CommandLineParser.ParseSeparatedPaths(value);
			foreach (string item2 in enumerable)
			{
				ImmutableArray<Diagnostic> diagnostics = default(ImmutableArray<Diagnostic>);
				GlobalImport item = GlobalImport.Parse(item2, out diagnostics);
				errors.AddRange(diagnostics);
				globalImports.Add(item);
			}
		}

		private static ImmutableDictionary<string, CConst> PublicSymbolsToInternalDefines(IEnumerable<KeyValuePair<string, object>> symbols, ArrayBuilder<Diagnostic> diagnosticBuilder)
		{
			ImmutableDictionary<string, CConst>.Builder builder = ImmutableDictionary.CreateBuilder<string, CConst>(CaseInsensitiveComparison.Comparer);
			if (symbols != null)
			{
				foreach (KeyValuePair<string, object> symbol in symbols)
				{
					CConst cConst = CConst.TryCreate(RuntimeHelpers.GetObjectValue(symbol.Value));
					if (cConst == null)
					{
						diagnosticBuilder.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 37288, symbol.Key, symbol.Value.GetType()));
					}
					builder[symbol.Key] = cConst;
				}
			}
			return builder.ToImmutable();
		}

		private static IReadOnlyDictionary<string, object> InternalDefinesToPublicSymbols(ImmutableDictionary<string, CConst> defines)
		{
			ImmutableDictionary<string, object>.Builder builder = ImmutableDictionary.CreateBuilder<string, object>(CaseInsensitiveComparison.Comparer);
			foreach (KeyValuePair<string, CConst> define in defines)
			{
				builder[define.Key] = RuntimeHelpers.GetObjectValue(define.Value.ValueAsObject);
			}
			return builder.ToImmutable();
		}

		public static IReadOnlyDictionary<string, object> ParseConditionalCompilationSymbols(string symbolList, out IEnumerable<Diagnostic> diagnostics, IEnumerable<KeyValuePair<string, object>> symbols = null)
		{
			ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance();
			StringBuilder stringBuilder = new StringBuilder();
			ImmutableDictionary<string, CConst> immutableDictionary = PublicSymbolsToInternalDefines(symbols, instance);
			string b;
			do
			{
				b = symbolList;
				symbolList = symbolList.Unquote();
			}
			while (!string.Equals(symbolList, b, StringComparison.Ordinal));
			symbolList = symbolList.Replace("\\\"", "\"");
			string text = symbolList.TrimEnd(new char[0]);
			if (text.Length > 0 && SyntaxFacts.IsConnectorPunctuation(text[text.Length - 1]))
			{
				symbolList += ",";
			}
			using (IEnumerator<SyntaxToken> enumerator = SyntaxFactory.ParseTokens(symbolList).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					while (true)
					{
						if (enumerator.Current.Position > 0 && !IsSeparatorOrEndOfFile(enumerator.Current))
						{
							stringBuilder.Append(" ^^ ^^ ");
							while (!IsSeparatorOrEndOfFile(enumerator.Current))
							{
								stringBuilder.Append(enumerator.Current.ToFullString());
								enumerator.MoveNext();
							}
							instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedEOS), stringBuilder.ToString()), Location.None));
							break;
						}
						SyntaxToken token = default(SyntaxToken);
						while (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.CommaToken || VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.ColonToken)
						{
							if (VisualBasicExtensions.Kind(token) == SyntaxKind.None)
							{
								token = enumerator.Current;
							}
							else if (VisualBasicExtensions.Kind(token) != VisualBasicExtensions.Kind(enumerator.Current))
							{
								GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder, includeCurrentToken: true, VisualBasicExtensions.Kind(token));
								instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString()), Location.None));
							}
							stringBuilder.Append(enumerator.Current.ToString());
							if (VisualBasicExtensions.Kind(enumerator.Current) != SyntaxKind.EndOfFileToken)
							{
								enumerator.MoveNext();
							}
						}
						stringBuilder.Clear();
						if (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.EndOfFileToken)
						{
							SyntaxToken current = enumerator.Current;
							if (current.FullWidth > 0 && !current.LeadingTrivia.All((SyntaxTrivia t) => VisualBasicExtensions.Kind(t) == SyntaxKind.WhitespaceTrivia))
							{
								GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder, includeCurrentToken: true);
								instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString()), Location.None));
							}
							break;
						}
						stringBuilder.Append(enumerator.Current.ToFullString());
						if (VisualBasicExtensions.Kind(enumerator.Current) != SyntaxKind.IdentifierToken)
						{
							GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder);
							instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), stringBuilder.ToString()), Location.None));
							break;
						}
						string valueText = enumerator.Current.ValueText;
						enumerator.MoveNext();
						if (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.EqualsToken)
						{
							stringBuilder.Append(enumerator.Current.ToFullString());
							enumerator.MoveNext();
							int spanStart = enumerator.Current.SpanStart;
							Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = ParseConditionalCompilationExpression(symbolList, spanStart);
							int num = spanStart + expressionSyntax.Span.End;
							bool flag = IsSeparatorOrEndOfFile(enumerator.Current);
							while (VisualBasicExtensions.Kind(enumerator.Current) != SyntaxKind.EndOfFileToken && enumerator.Current.Span.End <= num)
							{
								stringBuilder.Append(enumerator.Current.ToFullString());
								enumerator.MoveNext();
								flag = IsSeparatorOrEndOfFile(enumerator.Current);
							}
							if (expressionSyntax.ContainsDiagnostics)
							{
								stringBuilder.Append(" ^^ ^^ ");
								while (!IsSeparatorOrEndOfFile(enumerator.Current))
								{
									stringBuilder.Append(enumerator.Current.ToFullString());
									enumerator.MoveNext();
								}
								bool flag2 = false;
								foreach (DiagnosticInfo syntaxError in expressionSyntax.VbGreen.GetSyntaxErrors())
								{
									if (syntaxError.Code != 30201 && syntaxError.Code != 31427)
									{
										instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, syntaxError, stringBuilder.ToString()), Location.None));
									}
									else
									{
										flag2 = true;
									}
								}
								if (flag2)
								{
									instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_ExpectedExpression : ERRID.ERR_BadCCExpression), stringBuilder.ToString()), Location.None));
								}
								break;
							}
							CConst cConst = ExpressionEvaluator.EvaluateExpression((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expressionSyntax.Green, immutableDictionary);
							ERRID errorId = cConst.ErrorId;
							if (errorId != 0)
							{
								GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder);
								instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(errorId, cConst.ErrorArgs), stringBuilder.ToString()), Location.None));
								break;
							}
							immutableDictionary = immutableDictionary.SetItem(valueText, cConst);
							continue;
						}
						if (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.CommaToken || VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.ColonToken || VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.EndOfFileToken)
						{
							immutableDictionary = immutableDictionary.SetItem(valueText, CConst.Create(value: true));
							continue;
						}
						if (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.BadToken)
						{
							GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder);
							instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_IllegalChar), stringBuilder.ToString()), Location.None));
						}
						else
						{
							GetErrorStringForRemainderOfConditionalCompilation(enumerator, stringBuilder);
							instance.Add(new DiagnosticWithInfo(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedEOS), stringBuilder.ToString()), Location.None));
						}
						break;
					}
				}
			}
			diagnostics = instance.ToArrayAndFree();
			return InternalDefinesToPublicSymbols(immutableDictionary);
		}

		private static Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax ParseConditionalCompilationExpression(string symbolList, int offset)
		{
			using Parser parser = new Parser(SyntaxFactory.MakeSourceText(symbolList, offset), VisualBasicParseOptions.Default);
			parser.GetNextToken();
			return (Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)parser.ParseConditionalCompilationExpression().CreateRed(null, 0);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_33_ParseInstrumentationKinds))]
		private static IEnumerable<InstrumentationKind> ParseInstrumentationKinds(string value, IList<Diagnostic> diagnostics)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_33_ParseInstrumentationKinds(-2)
			{
				_0024P_value = value,
				_0024P_diagnostics = diagnostics
			};
		}

		private static bool IsSeparatorOrEndOfFile(SyntaxToken token)
		{
			if (VisualBasicExtensions.Kind(token) != SyntaxKind.EndOfFileToken && VisualBasicExtensions.Kind(token) != SyntaxKind.ColonToken)
			{
				return VisualBasicExtensions.Kind(token) == SyntaxKind.CommaToken;
			}
			return true;
		}

		private static void GetErrorStringForRemainderOfConditionalCompilation(IEnumerator<SyntaxToken> tokens, StringBuilder remainderErrorLine, bool includeCurrentToken = false, SyntaxKind stopTokenKind = SyntaxKind.CommaToken)
		{
			if (includeCurrentToken)
			{
				remainderErrorLine.Append(" ^^ ");
				if (VisualBasicExtensions.Kind(tokens.Current) == SyntaxKind.ColonToken && tokens.Current.FullWidth == 0)
				{
					remainderErrorLine.Append(SyntaxFacts.GetText(SyntaxKind.ColonToken));
				}
				else
				{
					remainderErrorLine.Append(tokens.Current.ToFullString());
				}
				remainderErrorLine.Append(" ^^ ");
			}
			else
			{
				remainderErrorLine.Append(" ^^ ^^ ");
			}
			while (tokens.MoveNext() && VisualBasicExtensions.Kind(tokens.Current) != stopTokenKind)
			{
				remainderErrorLine.Append(tokens.Current.ToFullString());
			}
		}

		private static Platform ParsePlatform(string name, string value, List<Diagnostic> errors)
		{
			if (value.IsEmpty())
			{
				AddDiagnostic(errors, ERRID.ERR_ArgumentRequired, name, ":<string>");
			}
			else
			{
				string text = value.ToLowerInvariant();
				switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(text))
				{
				case 2449868801u:
					if (EmbeddedOperators.CompareString(text, "x86", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.X86;
				case 2212716469u:
					if (EmbeddedOperators.CompareString(text, "x64", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.X64;
				case 1554085904u:
					if (EmbeddedOperators.CompareString(text, "itanium", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.Itanium;
				case 4240884737u:
					if (EmbeddedOperators.CompareString(text, "anycpu", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.AnyCpu;
				case 2786156940u:
					if (EmbeddedOperators.CompareString(text, "anycpu32bitpreferred", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.AnyCpu32BitPreferred;
				case 946808757u:
					if (EmbeddedOperators.CompareString(text, "arm", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.Arm;
				case 3010551159u:
					if (EmbeddedOperators.CompareString(text, "arm64", TextCompare: false) != 0)
					{
						break;
					}
					return Platform.Arm64;
				}
				AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, name, value);
			}
			return Platform.AnyCpu;
		}

		private static int ParseFileAlignment(string name, string value, List<Diagnostic> errors)
		{
			ushort result;
			if (string.IsNullOrEmpty(value))
			{
				AddDiagnostic(errors, ERRID.ERR_ArgumentRequired, name, ":<number>");
			}
			else if (!CommandLineParser.TryParseUInt16(value, out result))
			{
				AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, name, value);
			}
			else
			{
				if (CompilationOptions.IsValidFileAlignment(result))
				{
					return result;
				}
				AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, name, value);
			}
			return 0;
		}

		private static ulong ParseBaseAddress(string name, string value, List<Diagnostic> errors)
		{
			if (string.IsNullOrEmpty(value))
			{
				AddDiagnostic(errors, ERRID.ERR_ArgumentRequired, name, ":<number>");
			}
			else
			{
				string s = value;
				if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				{
					s = value.Substring(2);
				}
				if (ulong.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
				{
					return result;
				}
				AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, name, value.ToString());
			}
			return 0uL;
		}

		private static IEnumerable<string> ParseWarnings(string value)
		{
			IEnumerable<string> enumerable = CommandLineParser.ParseSeparatedPaths(value);
			List<string> list = new List<string>();
			foreach (string item in enumerable)
			{
				if (ushort.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) && Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetSeverity(result) == DiagnosticSeverity.Warning && Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetWarningLevel(result) == 1)
				{
					list.Add(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance.GetIdForErrorCode(result));
				}
				else
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static void AddWarnings(IDictionary<string, ReportDiagnostic> d, ReportDiagnostic kind, IEnumerable<string> items)
		{
			foreach (string item in items)
			{
				if (d.TryGetValue(item, out var value))
				{
					if (value != ReportDiagnostic.Suppress)
					{
						d[item] = kind;
					}
				}
				else
				{
					d.Add(item, kind);
				}
			}
		}

		private static void UnimplementedSwitch(IList<Diagnostic> diagnostics, string switchName)
		{
			AddDiagnostic(diagnostics, ERRID.WRN_UnimplementedCommandLineSwitch, "/" + switchName);
		}

		internal override void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> errors)
		{
			AddDiagnostic(errors, ERRID.ERR_InvalidSwitchValue, "recurse", path);
		}

		private static void AddDiagnostic(IList<Diagnostic> diagnostics, ERRID errorCode, params object[] arguments)
		{
			diagnostics.Add(Diagnostic.Create(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, (int)errorCode, arguments));
		}

		private void GetCompilationAndModuleNames(List<Diagnostic> diagnostics, OutputKind kind, List<CommandLineSourceFile> sourceFiles, string moduleAssemblyName, ref string outputFileName, ref string moduleName, out string compilationName)
		{
			string text = null;
			if (outputFileName == null)
			{
				CommandLineSourceFile commandLineSourceFile = sourceFiles.FirstOrDefault();
				if (commandLineSourceFile.Path != null)
				{
					text = PathUtilities.RemoveExtension(PathUtilities.GetFileName(commandLineSourceFile.Path));
					outputFileName = text + kind.GetDefaultExtension();
					if (text.Length == 0 && !kind.IsNetModule())
					{
						AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, outputFileName);
						text = null;
						outputFileName = null;
					}
				}
			}
			else
			{
				string extension = PathUtilities.GetExtension(outputFileName);
				if (kind.IsNetModule())
				{
					if (extension.Length == 0)
					{
						outputFileName += ".netmodule";
					}
				}
				else
				{
					if (!extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) & !extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) & !extension.Equals(".netmodule", StringComparison.OrdinalIgnoreCase) & !extension.Equals(".winmdobj", StringComparison.OrdinalIgnoreCase))
					{
						text = outputFileName;
						outputFileName += kind.GetDefaultExtension();
					}
					if (text == null)
					{
						text = PathUtilities.RemoveExtension(outputFileName);
						if (text.Length == 0)
						{
							AddDiagnostic(diagnostics, ERRID.FTL_InvalidInputFileName, outputFileName);
							text = null;
							outputFileName = null;
						}
					}
				}
			}
			if (kind.IsNetModule())
			{
				compilationName = moduleAssemblyName;
			}
			else
			{
				if (moduleAssemblyName != null)
				{
					AddDiagnostic(diagnostics, ERRID.ERR_NeedModule);
				}
				compilationName = text;
			}
			if (moduleName == null)
			{
				moduleName = outputFileName;
			}
		}
	}
}
