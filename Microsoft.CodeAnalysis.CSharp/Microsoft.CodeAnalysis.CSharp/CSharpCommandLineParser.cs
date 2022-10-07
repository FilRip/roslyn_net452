using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public class CSharpCommandLineParser : CommandLineParser
    {
        private static readonly char[] s_quoteOrEquals = new char[2] { '"', '=' };

        public static CSharpCommandLineParser Default { get; } = new CSharpCommandLineParser();


        public static CSharpCommandLineParser Script { get; } = new CSharpCommandLineParser(isScriptCommandLineParser: true);


        protected override string RegularFileExtension => ".cs";

        protected override string ScriptFileExtension => ".csx";

        internal CSharpCommandLineParser(bool isScriptCommandLineParser = false)
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, isScriptCommandLineParser)
        {
        }

        public sealed override CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory, string? sdkDirectory, string? additionalReferenceDirectories)
        {
            return Parse(args, baseDirectory, sdkDirectory, additionalReferenceDirectories);
        }

        public new CSharpCommandLineArguments Parse(IEnumerable<string> args, string? baseDirectory, string? sdkDirectory, string? additionalReferenceDirectories = null)
        {
            List<Diagnostic> list = new List<Diagnostic>();
            List<string> list2 = new List<string>();
            List<string> list3 = (IsScriptCommandLineParser ? new List<string>() : null);
            List<string> list4 = (IsScriptCommandLineParser ? new List<string>() : null);
            FlattenArgs(args, list, list2, list3, baseDirectory, list4);
            string appConfigPath = null;
            bool displayLogo = true;
            bool displayHelp = false;
            bool displayVersion = false;
            bool displayLangVersions = false;
            bool flag = false;
            bool flag2 = false;
            NullableContextOptions nullableContextOptions = NullableContextOptions.Disable;
            bool flag3 = false;
            bool flag4 = true;
            bool flag5 = false;
            bool flag6 = false;
            DebugInformationFormat debugInformationFormat = ((!PathUtilities.IsUnixLikePlatform) ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
            bool flag7 = false;
            string pdbPath = null;
            bool flag8 = IsScriptCommandLineParser;
            string outputDirectory = baseDirectory;
            ImmutableArray<KeyValuePair<string, string>> immutableArray = ImmutableArray<KeyValuePair<string, string>>.Empty;
            string outputFileName = null;
            string text = null;
            bool flag9 = false;
            string generatedFilesOutputDirectory = null;
            string documentationPath = null;
            ErrorLogOptions errorLogOptions = null;
            bool flag10 = false;
            bool utf8Output = false;
            OutputKind outputKind = OutputKind.ConsoleApplication;
            SubsystemVersion subsystemVersion = SubsystemVersion.None;
            LanguageVersion result = LanguageVersion.Default;
            string text2 = null;
            string text3 = null;
            string win32ResourceFile = null;
            string text4 = null;
            bool noWin32Manifest = false;
            Platform platform = Platform.AnyCpu;
            ulong num = 0uL;
            int fileAlignment = 0;
            bool? flag11 = null;
            string text5 = null;
            string text6 = null;
            List<ResourceDescription> list5 = new List<ResourceDescription>();
            List<CommandLineSourceFile> list6 = new List<CommandLineSourceFile>();
            List<CommandLineSourceFile> list7 = new List<CommandLineSourceFile>();
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            List<CommandLineSourceFile> list8 = new List<CommandLineSourceFile>();
            bool flag12 = false;
            bool flag13 = false;
            bool flag14 = false;
            Encoding encoding = null;
            SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha256;
            ArrayBuilder<string> instance2 = ArrayBuilder<string>.GetInstance();
            List<CommandLineReference> list9 = new List<CommandLineReference>();
            List<CommandLineAnalyzerReference> list10 = new List<CommandLineAnalyzerReference>();
            List<string> list11 = new List<string>();
            List<string> list12 = new List<string>();
            List<string> list13 = new List<string>();
            List<string> list14 = new List<string>();
            ReportDiagnostic reportDiagnostic = ReportDiagnostic.Default;
            Dictionary<string, ReportDiagnostic> diagnosticOptions = new Dictionary<string, ReportDiagnostic>();
            Dictionary<string, ReportDiagnostic> dictionary = new Dictionary<string, ReportDiagnostic>();
            Dictionary<string, ReportDiagnostic> dictionary2 = new Dictionary<string, ReportDiagnostic>();
            int num2 = 4;
            bool flag15 = false;
            bool printFullPaths = false;
            string moduleAssemblyName = null;
            string moduleName = null;
            List<string> list15 = new List<string>();
            string runtimeMetadataVersion = null;
            bool shouldIncludeErrorEndLocation = false;
            bool reportAnalyzer = false;
            bool skipAnalyzers = false;
            ArrayBuilder<InstrumentationKind> instance3 = ArrayBuilder<InstrumentationKind>.GetInstance();
            CultureInfo cultureInfo = null;
            string touchedFilesPath = null;
            bool flag16 = false;
            bool flag17 = false;
            bool flag18 = false;
            string text7 = null;
            string text8 = null;
            if (!IsScriptCommandLineParser)
            {
                foreach (string item in list2)
                {
                    if (CommandLineParser.TryParseOption(item, out var name, out var value) && name == "ruleset")
                    {
                        string text9 = CommandLineParser.RemoveQuotesAndSlashes(value);
                        if (RoslynString.IsNullOrEmpty(text9))
                        {
                            AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name);
                        }
                        else
                        {
                            text8 = ParseGenericPathToFile(text9, list, baseDirectory);
                            reportDiagnostic = GetDiagnosticOptionsFromRulesetFile(text8, out diagnosticOptions, list);
                        }
                    }
                }
            }
            foreach (string item2 in list2)
            {
                if (flag16 || !CommandLineParser.TryParseOption(item2, out var name2, out var value2))
                {
                    foreach (string item3 in ParseFileArgument(item2, baseDirectory, list))
                    {
                        list6.Add(ToCommandLineSourceFile(item3));
                    }
                    if (list6.Count > 0)
                    {
                        flag12 = true;
                    }
                    continue;
                }
                switch (name2)
                {
                    case "?":
                    case "help":
                        displayHelp = true;
                        continue;
                    case "version":
                        displayVersion = true;
                        continue;
                    case "langversion":
                        value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                        if (RoslynString.IsNullOrEmpty(value2))
                        {
                            AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/langversion:");
                        }
                        else if (value2.StartsWith("0", StringComparison.Ordinal))
                        {
                            AddDiagnostic(list, ErrorCode.ERR_LanguageVersionCannotHaveLeadingZeroes, value2);
                        }
                        else if (value2 == "?")
                        {
                            displayLangVersions = true;
                        }
                        else if (!LanguageVersionFacts.TryParse(value2, out result))
                        {
                            AddDiagnostic(list, ErrorCode.ERR_BadCompatMode, value2);
                        }
                        continue;
                    case "r":
                    case "reference":
                        list9.AddRange(ParseAssemblyReferences(item2, value2, list, embedInteropTypes: false));
                        continue;
                    case "features":
                        if (value2 == null)
                        {
                            list15.Clear();
                        }
                        else
                        {
                            list15.Add(value2);
                        }
                        continue;
                    case "lib":
                    case "libpath":
                    case "libpaths":
                        ParseAndResolveReferencePaths(name2, value2, baseDirectory, list11, MessageID.IDS_LIB_OPTION, list);
                        continue;
                }
                if (IsScriptCommandLineParser)
                {
                    switch (name2)
                    {
                        case "-":
                            if (value2 != null)
                            {
                                break;
                            }
                            if (item2 == "-")
                            {
                                if (Console.IsInputRedirected)
                                {
                                    list6.Add(new CommandLineSourceFile("-", isScript: true, isInputRedirected: true));
                                    flag12 = true;
                                }
                                else
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_StdInOptionProvidedButConsoleInputIsNotRedirected);
                                }
                            }
                            else
                            {
                                flag16 = true;
                            }
                            continue;
                        case "i":
                        case "i+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag17 = true;
                            continue;
                        case "i-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag17 = false;
                            continue;
                        case "loadpath":
                        case "loadpaths":
                            ParseAndResolveReferencePaths(name2, value2, baseDirectory, list12, MessageID.IDS_REFERENCEPATH_OPTION, list);
                            continue;
                        case "u":
                        case "using":
                        case "usings":
                        case "import":
                        case "imports":
                            list14.AddRange(ParseUsings(item2, value2, list));
                            continue;
                    }
                }
                else
                {
                    switch (name2)
                    {
                        case "a":
                        case "analyzer":
                            list10.AddRange(ParseAnalyzers(item2, value2, list));
                            continue;
                        case "d":
                        case "define":
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", item2);
                            }
                            else
                            {
                                instance2.AddRange(ParseConditionalCompilationSymbols(CommandLineParser.RemoveQuotesAndSlashes(value2), out var diagnostics));
                                list.AddRange(diagnostics);
                            }
                            continue;
                        case "codepage":
                            {
                                value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (value2 == null)
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name2);
                                    continue;
                                }
                                Encoding encoding2 = CommandLineParser.TryParseEncodingName(value2);
                                if (encoding2 == null)
                                {
                                    AddDiagnostic(list, ErrorCode.FTL_BadCodepage, value2);
                                }
                                else
                                {
                                    encoding = encoding2;
                                }
                                continue;
                            }
                        case "checksumalgorithm":
                            {
                                if (RoslynString.IsNullOrEmpty(value2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name2);
                                    continue;
                                }
                                SourceHashAlgorithm sourceHashAlgorithm = CommandLineParser.TryParseHashAlgorithmName(value2);
                                if (sourceHashAlgorithm == SourceHashAlgorithm.None)
                                {
                                    AddDiagnostic(list, ErrorCode.FTL_BadChecksumAlgorithm, value2);
                                }
                                else
                                {
                                    checksumAlgorithm = sourceHashAlgorithm;
                                }
                                continue;
                            }
                        case "checked":
                        case "checked+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag2 = true;
                            continue;
                        case "checked-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag2 = false;
                            continue;
                        case "nullable":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (value2 != null)
                            {
                                if (value2.IsEmpty())
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), name2);
                                    continue;
                                }
                                switch (value2.ToLower())
                                {
                                    case "disable":
                                        nullableContextOptions = NullableContextOptions.Disable;
                                        break;
                                    case "enable":
                                        nullableContextOptions = NullableContextOptions.Enable;
                                        break;
                                    case "warnings":
                                        nullableContextOptions = NullableContextOptions.Warnings;
                                        break;
                                    case "annotations":
                                        nullableContextOptions = NullableContextOptions.Annotations;
                                        break;
                                    default:
                                        AddDiagnostic(list, ErrorCode.ERR_BadNullableContextOption, value2);
                                        break;
                                }
                            }
                            else
                            {
                                nullableContextOptions = NullableContextOptions.Enable;
                            }
                            continue;
                        case "nullable+":
                            if (value2 != null)
                            {
                                break;
                            }
                            nullableContextOptions = NullableContextOptions.Enable;
                            continue;
                        case "nullable-":
                            if (value2 != null)
                            {
                                break;
                            }
                            nullableContextOptions = NullableContextOptions.Disable;
                            continue;
                        case "instrument":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name2);
                                continue;
                            }
                            foreach (InstrumentationKind item4 in ParseInstrumentationKinds(value2, list))
                            {
                                if (!instance3.Contains(item4))
                                {
                                    instance3.Add(item4);
                                }
                            }
                            continue;
                        case "sqmsessionguid":
                            {
                                if (value2 == null)
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_MissingGuidForOption, "<text>", name2);
                                }
                                else if (!Guid.TryParse(value2, out Guid result2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_InvalidFormatForGuidForOption, value2, name2);
                                }
                                continue;
                            }
                        case "preferreduilang":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", item2);
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
                            catch (CultureNotFoundException)
                            {
                            }
                            if (cultureInfo == null)
                            {
                                AddDiagnostic(list, ErrorCode.WRN_BadUILang, value2);
                            }
                            continue;
                        case "nosdkpath":
                            sdkDirectory = null;
                            continue;
                        case "out":
                            if (RoslynString.IsNullOrWhiteSpace(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                            }
                            else
                            {
                                ParseOutputFile(value2, list, baseDirectory, out outputFileName, out outputDirectory);
                            }
                            continue;
                        case "refout":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                            }
                            else
                            {
                                text = ParseGenericPathToFile(value2, list, baseDirectory);
                            }
                            continue;
                        case "refonly":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag9 = true;
                            continue;
                        case "t":
                        case "target":
                            if (value2 == null)
                            {
                                break;
                            }
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.FTL_InvalidTarget);
                            }
                            else
                            {
                                outputKind = ParseTarget(value2, list);
                            }
                            continue;
                        case "moduleassemblyname":
                            value2 = value2?.Unquote();
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", item2);
                            }
                            else if (!MetadataHelpers.IsValidAssemblyOrModuleName(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_InvalidAssemblyName, "<text>", item2);
                            }
                            else
                            {
                                moduleAssemblyName = value2;
                            }
                            continue;
                        case "modulename":
                            {
                                string text11 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (string.IsNullOrEmpty(text11))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "modulename");
                                }
                                else
                                {
                                    moduleName = text11;
                                }
                                continue;
                            }
                        case "platform":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<string>", item2);
                            }
                            else
                            {
                                platform = ParsePlatform(value2, list);
                            }
                            continue;
                        case "recurse":
                            {
                                value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (value2 == null)
                                {
                                    break;
                                }
                                if (RoslynString.IsNullOrEmpty(value2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                                    continue;
                                }
                                int count = list6.Count;
                                list6.AddRange(ParseRecurseArgument(value2, baseDirectory, list));
                                if (list6.Count > count)
                                {
                                    flag12 = true;
                                }
                                continue;
                            }
                        case "generatedfilesout":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (string.IsNullOrWhiteSpace(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), item2);
                            }
                            else
                            {
                                generatedFilesOutputDirectory = ParseGenericPathToFile(value2, list, baseDirectory);
                            }
                            continue;
                        case "doc":
                            {
                                flag10 = true;
                                if (RoslynString.IsNullOrEmpty(value2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), item2);
                                    continue;
                                }
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (RoslynString.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/doc:");
                                }
                                else
                                {
                                    documentationPath = ParseGenericPathToFile(text10, list, baseDirectory);
                                }
                                continue;
                            }
                        case "addmodule":
                            if (value2 == null)
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "/addmodule:");
                            }
                            else if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                            }
                            else
                            {
                                list9.AddRange(from path in CommandLineParser.ParseSeparatedPaths(value2)
                                               select new CommandLineReference(path, MetadataReferenceProperties.Module));
                                flag14 = true;
                            }
                            continue;
                        case "l":
                        case "link":
                            list9.AddRange(ParseAssemblyReferences(item2, value2, list, embedInteropTypes: true));
                            continue;
                        case "win32res":
                            win32ResourceFile = GetWin32Setting(item2, value2, list);
                            continue;
                        case "win32icon":
                            text4 = GetWin32Setting(item2, value2, list);
                            continue;
                        case "win32manifest":
                            text3 = GetWin32Setting(item2, value2, list);
                            noWin32Manifest = false;
                            continue;
                        case "nowin32manifest":
                            noWin32Manifest = true;
                            text3 = null;
                            continue;
                        case "res":
                        case "resource":
                            {
                                if (value2 == null)
                                {
                                    break;
                                }
                                ResourceDescription resourceDescription2 = ParseResourceDescription(item2, value2, baseDirectory, list, embedded: true);
                                if (resourceDescription2 != null)
                                {
                                    list5.Add(resourceDescription2);
                                    flag14 = true;
                                }
                                continue;
                            }
                        case "linkres":
                        case "linkresource":
                            {
                                if (value2 == null)
                                {
                                    break;
                                }
                                ResourceDescription resourceDescription = ParseResourceDescription(item2, value2, baseDirectory, list, embedded: false);
                                if (resourceDescription != null)
                                {
                                    list5.Add(resourceDescription);
                                    flag14 = true;
                                }
                                continue;
                            }
                        case "sourcelink":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                            }
                            else
                            {
                                text7 = ParseGenericPathToFile(value2, list, baseDirectory);
                            }
                            continue;
                        case "debug":
                            flag6 = true;
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (value2 == null)
                            {
                                continue;
                            }
                            if (value2.IsEmpty())
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), name2);
                                continue;
                            }
                            switch (value2.ToLower())
                            {
                                case "full":
                                case "pdbonly":
                                    debugInformationFormat = ((!PathUtilities.IsUnixLikePlatform) ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
                                    break;
                                case "portable":
                                    debugInformationFormat = DebugInformationFormat.PortablePdb;
                                    break;
                                case "embedded":
                                    debugInformationFormat = DebugInformationFormat.Embedded;
                                    break;
                                default:
                                    AddDiagnostic(list, ErrorCode.ERR_BadDebugType, value2);
                                    break;
                            }
                            continue;
                        case "debug+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag6 = true;
                            flag7 = true;
                            continue;
                        case "debug-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag6 = false;
                            flag7 = false;
                            continue;
                        case "o":
                        case "optimize":
                        case "o+":
                        case "optimize+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag = true;
                            continue;
                        case "o-":
                        case "optimize-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag = false;
                            continue;
                        case "deterministic":
                        case "deterministic+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag5 = true;
                            continue;
                        case "deterministic-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag5 = false;
                            continue;
                        case "p":
                        case "parallel":
                        case "p+":
                        case "parallel+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag4 = true;
                            continue;
                        case "p-":
                        case "parallel-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag4 = false;
                            continue;
                        case "warnaserror":
                        case "warnaserror+":
                            if (value2 == null)
                            {
                                reportDiagnostic = ReportDiagnostic.Error;
                                dictionary2.Clear();
                                foreach (string key in diagnosticOptions.Keys)
                                {
                                    if (diagnosticOptions[key] == ReportDiagnostic.Warn)
                                    {
                                        dictionary2[key] = ReportDiagnostic.Error;
                                    }
                                }
                            }
                            else if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                            }
                            else
                            {
                                AddWarnings(dictionary2, ReportDiagnostic.Error, ParseWarnings(value2));
                            }
                            continue;
                        case "warnaserror-":
                            if (value2 == null)
                            {
                                reportDiagnostic = ReportDiagnostic.Default;
                                dictionary2.Clear();
                                continue;
                            }
                            if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
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
                        case "w":
                        case "warn":
                            {
                                value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (value2 == null)
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                                }
                                else if (string.IsNullOrEmpty(value2) || !int.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result3))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                                }
                                else if (result3 < 0)
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_BadWarningLevel, name2);
                                }
                                else
                                {
                                    num2 = result3;
                                }
                                continue;
                            }
                        case "nowarn":
                            if (value2 == null)
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                            }
                            else if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                            }
                            else
                            {
                                AddWarnings(dictionary, ReportDiagnostic.Suppress, ParseWarnings(value2));
                            }
                            continue;
                        case "unsafe":
                        case "unsafe+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag3 = true;
                            continue;
                        case "unsafe-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag3 = false;
                            continue;
                        case "delaysign":
                        case "delaysign+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag11 = true;
                            continue;
                        case "delaysign-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag11 = false;
                            continue;
                        case "publicsign":
                        case "publicsign+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag18 = true;
                            continue;
                        case "publicsign-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag18 = false;
                            continue;
                        case "keyfile":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, "keyfile");
                            }
                            else
                            {
                                text5 = value2;
                            }
                            continue;
                        case "keycontainer":
                            if (string.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "keycontainer");
                            }
                            else
                            {
                                text6 = value2;
                            }
                            continue;
                        case "highentropyva":
                        case "highentropyva+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag15 = true;
                            continue;
                        case "highentropyva-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag15 = false;
                            continue;
                        case "nologo":
                            displayLogo = false;
                            continue;
                        case "baseaddress":
                            {
                                value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (string.IsNullOrEmpty(value2) || !CommandLineParser.TryParseUInt64(value2, out var result5))
                                {
                                    if (RoslynString.IsNullOrEmpty(value2))
                                    {
                                        AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                                    }
                                    else
                                    {
                                        AddDiagnostic(list, ErrorCode.ERR_BadBaseNumber, value2);
                                    }
                                }
                                else
                                {
                                    num = result5;
                                }
                                continue;
                            }
                        case "subsystemversion":
                            {
                                if (RoslynString.IsNullOrEmpty(value2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "subsystemversion");
                                    continue;
                                }
                                SubsystemVersion version = SubsystemVersion.None;
                                if (SubsystemVersion.TryParse(value2, out version))
                                {
                                    subsystemVersion = version;
                                    continue;
                                }
                                AddDiagnostic(list, ErrorCode.ERR_InvalidSubsystemVersion, value2);
                                continue;
                            }
                        case "touchedfiles":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (string.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), "touchedfiles");
                                }
                                else
                                {
                                    touchedFilesPath = text10;
                                }
                                continue;
                            }
                        case "bugreport":
                            UnimplementedSwitch(list, name2);
                            continue;
                        case "utf8output":
                            if (value2 != null)
                            {
                                break;
                            }
                            utf8Output = true;
                            continue;
                        case "m":
                        case "main":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (string.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name2);
                                }
                                else
                                {
                                    text2 = text10;
                                }
                                continue;
                            }
                        case "fullpaths":
                            if (value2 != null)
                            {
                                break;
                            }
                            printFullPaths = true;
                            continue;
                        case "pathmap":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (text10 == null)
                                {
                                    break;
                                }
                                immutableArray = immutableArray.Concat<KeyValuePair<string, string>>(ParsePathMap(text10, list));
                                continue;
                            }
                        case "filealign":
                            {
                                value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (RoslynString.IsNullOrEmpty(value2))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsNumber, name2);
                                }
                                else if (!CommandLineParser.TryParseUInt16(value2, out ushort result4))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_InvalidFileAlignment, value2);
                                }
                                else if (!CompilationOptions.IsValidFileAlignment(result4))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_InvalidFileAlignment, value2);
                                }
                                else
                                {
                                    fileAlignment = result4;
                                }
                                continue;
                            }
                        case "pdb":
                            value2 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_NoFileSpec, item2);
                            }
                            else
                            {
                                pdbPath = ParsePdbPath(value2, list, baseDirectory);
                            }
                            continue;
                        case "errorendlocation":
                            shouldIncludeErrorEndLocation = true;
                            continue;
                        case "reportanalyzer":
                            reportAnalyzer = true;
                            continue;
                        case "skipanalyzers":
                        case "skipanalyzers+":
                            if (value2 != null)
                            {
                                break;
                            }
                            skipAnalyzers = true;
                            continue;
                        case "skipanalyzers-":
                            if (value2 != null)
                            {
                                break;
                            }
                            skipAnalyzers = false;
                            continue;
                        case "nostdlib":
                        case "nostdlib+":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag8 = true;
                            continue;
                        case "nostdlib-":
                            if (value2 != null)
                            {
                                break;
                            }
                            flag8 = false;
                            continue;
                        case "errorlog":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (RoslynString.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<file>[,version={1|1.0|2|2.1}]", CommandLineParser.RemoveQuotesAndSlashes(item2));
                                    continue;
                                }
                                errorLogOptions = ParseErrorLogOptions(text10, list, baseDirectory, out var diagnosticAlreadyReported);
                                if (errorLogOptions == null && !diagnosticAlreadyReported)
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_BadSwitchValue, text10, "/errorlog:", "<file>[,version={1|1.0|2|2.1}]");
                                }
                                continue;
                            }
                        case "appconfig":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (RoslynString.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, ":<text>", CommandLineParser.RemoveQuotesAndSlashes(item2));
                                }
                                else
                                {
                                    appConfigPath = ParseGenericPathToFile(text10, list, baseDirectory);
                                }
                                continue;
                            }
                        case "runtimemetadataversion":
                            {
                                string text10 = CommandLineParser.RemoveQuotesAndSlashes(value2);
                                if (string.IsNullOrEmpty(text10))
                                {
                                    AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<text>", name2);
                                }
                                else
                                {
                                    runtimeMetadataVersion = text10;
                                }
                                continue;
                            }
                        case "additionalfile":
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<file list>", name2);
                                continue;
                            }
                            foreach (string item6 in ParseSeparatedFileArgument(value2, baseDirectory, list))
                            {
                                list7.Add(ToCommandLineSourceFile(item6));
                            }
                            continue;
                        case "analyzerconfig":
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                AddDiagnostic(list, ErrorCode.ERR_SwitchNeedsString, "<file list>", name2);
                            }
                            else
                            {
                                instance.AddRange(ParseSeparatedFileArgument(value2, baseDirectory, list));
                            }
                            continue;
                        case "embed":
                            if (RoslynString.IsNullOrEmpty(value2))
                            {
                                flag13 = true;
                                continue;
                            }
                            foreach (string item7 in ParseSeparatedFileArgument(value2, baseDirectory, list))
                            {
                                list8.Add(ToCommandLineSourceFile(item7));
                            }
                            continue;
                        case "-":
                            if (Console.IsInputRedirected)
                            {
                                list6.Add(new CommandLineSourceFile("-", isScript: false, isInputRedirected: true));
                                flag12 = true;
                            }
                            else
                            {
                                AddDiagnostic(list, ErrorCode.ERR_StdInOptionProvidedButConsoleInputIsNotRedirected);
                            }
                            continue;
                        case "noconfig":
                        case "errorreport":
                        case "ruleset":
                            continue;
                    }
                }
                AddDiagnostic(list, ErrorCode.ERR_BadSwitch, item2);
            }
            foreach (KeyValuePair<string, ReportDiagnostic> item8 in dictionary2)
            {
                diagnosticOptions[item8.Key] = item8.Value;
            }
            foreach (KeyValuePair<string, ReportDiagnostic> item9 in dictionary)
            {
                diagnosticOptions[item9.Key] = item9.Value;
            }
            if (flag9 && text != null)
            {
                AddDiagnostic(list, diagnosticOptions, ErrorCode.ERR_NoRefOutWhenRefOnly);
            }
            if (outputKind == OutputKind.NetModule && (flag9 || text != null))
            {
                AddDiagnostic(list, diagnosticOptions, ErrorCode.ERR_NoNetModuleOutputWhenRefOutOrRefOnly);
            }
            if (!IsScriptCommandLineParser && !flag12 && (outputKind.IsNetModule() || !flag14))
            {
                AddDiagnostic(list, diagnosticOptions, ErrorCode.WRN_NoSources);
            }
            if (!flag8 && sdkDirectory != null)
            {
                list9.Insert(0, new CommandLineReference(Path.Combine(sdkDirectory, "mscorlib.dll"), MetadataReferenceProperties.Assembly));
            }
            if (!platform.Requires64Bit() && num > 4294934527u)
            {
                AddDiagnostic(list, ErrorCode.ERR_BadBaseNumber, $"0x{num:X}");
                num = 0uL;
            }
            if (!string.IsNullOrEmpty(additionalReferenceDirectories))
            {
                ParseAndResolveReferencePaths(null, additionalReferenceDirectories, baseDirectory, list11, MessageID.IDS_LIB_ENV, list);
            }
            ImmutableArray<string> referencePaths = BuildSearchPaths(sdkDirectory, list11, list4);
            ValidateWin32Settings(win32ResourceFile, text4, text3, outputKind, list);
            if (!RoslynString.IsNullOrEmpty(baseDirectory))
            {
                list13.Add(baseDirectory);
            }
            if (RoslynString.IsNullOrEmpty(outputDirectory))
            {
                AddDiagnostic(list, ErrorCode.ERR_NoOutputDirectory);
            }
            else if (baseDirectory != outputDirectory)
            {
                list13.Add(outputDirectory);
            }
            if (flag18 && !RoslynString.IsNullOrEmpty(text5))
            {
                text5 = ParseGenericPathToFile(text5, list, baseDirectory);
            }
            if (text7 != null && !flag6)
            {
                AddDiagnostic(list, ErrorCode.ERR_SourceLinkRequiresPdb);
            }
            if (flag13)
            {
                list8.AddRange(list6);
            }
            if (list8.Count > 0 && !flag6)
            {
                AddDiagnostic(list, ErrorCode.ERR_CannotEmbedWithoutPdb);
            }
            ImmutableDictionary<string, string> features = CommandLineParser.ParseFeatures(list15);
            GetCompilationAndModuleNames(list, outputKind, list6, flag12, moduleAssemblyName, ref outputFileName, ref moduleName, out var compilationName);
            CSharpParseOptions cSharpParseOptions = new CSharpParseOptions(result, preprocessorSymbols: instance2.ToImmutableAndFree(), documentationMode: flag10 ? DocumentationMode.Diagnose : DocumentationMode.None, kind: IsScriptCommandLineParser ? SourceCodeKind.Script : SourceCodeKind.Regular, features: features);
            bool reportSuppressedDiagnostics = errorLogOptions != null;
            OutputKind outputKind2 = outputKind;
            string moduleName2 = moduleName;
            string mainTypeName = text2;
            IEnumerable<string> usings = list14;
            OptimizationLevel optimizationLevel = (flag ? OptimizationLevel.Release : OptimizationLevel.Debug);
            bool checkOverflow = flag2;
            NullableContextOptions nullableContextOptions2 = nullableContextOptions;
            bool allowUnsafe = flag3;
            bool deterministic = flag5;
            bool concurrentBuild = flag4;
            string cryptoKeyContainer = text6;
            string cryptoKeyFile = text5;
            bool? delaySign = flag11;
            Platform platform2 = platform;
            ReportDiagnostic generalDiagnosticOption = reportDiagnostic;
            int warningLevel = num2;
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = diagnosticOptions;
            bool publicSign = flag18;
            CSharpCompilationOptions cSharpCompilationOptions = new CSharpCompilationOptions(outputKind2, reportSuppressedDiagnostics, moduleName2, mainTypeName, "Script", usings, optimizationLevel, checkOverflow, allowUnsafe, cryptoKeyContainer, cryptoKeyFile, default(ImmutableArray<byte>), delaySign, platform2, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic, null, null, null, null, null, publicSign, MetadataImportOptions.Public, nullableContextOptions2);
            if (flag7)
            {
                cSharpCompilationOptions = cSharpCompilationOptions.WithDebugPlusMode(flag7);
            }
            bool metadataOnly = flag9;
            publicSign = !flag9 && text == null;
            DebugInformationFormat debugInformationFormat2 = debugInformationFormat;
            ulong baseAddress = num;
            concurrentBuild = flag15;
            EmitOptions emitOptions = new EmitOptions(metadataOnly, debugInformationFormat2, null, null, fileAlignment, baseAddress, concurrentBuild, subsystemVersion, runtimeMetadataVersion, tolerateErrors: false, publicSign, instance3.ToImmutableAndFree(), SourceHashAlgorithm.Sha256, encoding);
            list.AddRange(cSharpCompilationOptions.Errors);
            list.AddRange(cSharpParseOptions.Errors);
            if (nullableContextOptions != 0 && cSharpParseOptions.LanguageVersion < MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion())
            {
                list.Add(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_NullableOptionNotAvailable, "nullable", nullableContextOptions, cSharpParseOptions.LanguageVersion.ToDisplayString(), new CSharpRequiredLanguageVersion(MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion())), Location.None));
            }
            immutableArray = CommandLineParser.SortPathMap(immutableArray);
            return new CSharpCommandLineArguments
            {
                IsScriptRunner = IsScriptCommandLineParser,
                InteractiveMode = (flag17 || (IsScriptCommandLineParser && list6.Count == 0)),
                BaseDirectory = baseDirectory,
                PathMap = immutableArray,
                Errors = list.AsImmutable(),
                Utf8Output = utf8Output,
                CompilationName = compilationName,
                OutputFileName = outputFileName,
                OutputRefFilePath = text,
                PdbPath = pdbPath,
                EmitPdb = (flag6 && !flag9),
                SourceLink = text7,
                RuleSetPath = text8,
                OutputDirectory = outputDirectory,
                DocumentationPath = documentationPath,
                GeneratedFilesOutputDirectory = generatedFilesOutputDirectory,
                ErrorLogOptions = errorLogOptions,
                AppConfigPath = appConfigPath,
                SourceFiles = list6.AsImmutable(),
                Encoding = encoding,
                ChecksumAlgorithm = checksumAlgorithm,
                MetadataReferences = list9.AsImmutable(),
                AnalyzerReferences = list10.AsImmutable(),
                AnalyzerConfigPaths = instance.ToImmutableAndFree(),
                AdditionalFiles = list7.AsImmutable(),
                ReferencePaths = referencePaths,
                SourcePaths = list12.AsImmutable(),
                KeyFileSearchPaths = list13.AsImmutable(),
                Win32ResourceFile = win32ResourceFile,
                Win32Icon = text4,
                Win32Manifest = text3,
                NoWin32Manifest = noWin32Manifest,
                DisplayLogo = displayLogo,
                DisplayHelp = displayHelp,
                DisplayVersion = displayVersion,
                DisplayLangVersions = displayLangVersions,
                ManifestResources = list5.AsImmutable(),
                CompilationOptions = cSharpCompilationOptions,
                ParseOptions = cSharpParseOptions,
                EmitOptions = emitOptions,
                ScriptArguments = list3.AsImmutableOrEmpty(),
                TouchedFilesPath = touchedFilesPath,
                PrintFullPaths = printFullPaths,
                ShouldIncludeErrorEndLocation = shouldIncludeErrorEndLocation,
                PreferredUILang = cultureInfo,
                ReportAnalyzer = reportAnalyzer,
                SkipAnalyzers = skipAnalyzers,
                EmbeddedFiles = list8.AsImmutable()
            };
        }

        private static void ParseAndResolveReferencePaths(string? switchName, string? switchValue, string? baseDirectory, List<string> builder, MessageID origin, List<Diagnostic> diagnostics)
        {
            if (string.IsNullOrEmpty(switchValue))
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_PathList.Localize(), switchName);
                return;
            }
            foreach (string item in CommandLineParser.ParseSeparatedPaths(switchValue))
            {
                string text = FileUtilities.ResolveRelativePath(item, baseDirectory);
                if (text == null)
                {
                    AddDiagnostic(diagnostics, ErrorCode.WRN_InvalidSearchPathDir, item, origin.Localize(), MessageID.IDS_DirectoryHasInvalidPath.Localize());
                }
                else if (!Directory.Exists(text))
                {
                    AddDiagnostic(diagnostics, ErrorCode.WRN_InvalidSearchPathDir, item, origin.Localize(), MessageID.IDS_DirectoryDoesNotExist.Localize());
                }
                else
                {
                    builder.Add(text);
                }
            }
        }

        private static string? GetWin32Setting(string arg, string? value, List<Diagnostic> diagnostics)
        {
            if (value == null)
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
            }
            else
            {
                string text = CommandLineParser.RemoveQuotesAndSlashes(value);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
            }
            return null;
        }

        private void GetCompilationAndModuleNames(List<Diagnostic> diagnostics, OutputKind outputKind, List<CommandLineSourceFile> sourceFiles, bool sourceFilesSpecified, string? moduleAssemblyName, ref string? outputFileName, ref string? moduleName, out string? compilationName)
        {
            string text;
            if (outputFileName == null)
            {
                if (!IsScriptCommandLineParser && !sourceFilesSpecified)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_OutputNeedsName);
                    text = null;
                }
                else if (outputKind.IsApplication())
                {
                    text = null;
                }
                else
                {
                    text = PathUtilities.RemoveExtension(PathUtilities.GetFileName(sourceFiles.FirstOrDefault().Path));
                    outputFileName = text + outputKind.GetDefaultExtension();
                    if (text.Length == 0 && !outputKind.IsNetModule())
                    {
                        AddDiagnostic(diagnostics, ErrorCode.FTL_InvalidInputFileName, outputFileName);
                        text = (outputFileName = null);
                    }
                }
            }
            else
            {
                text = PathUtilities.RemoveExtension(outputFileName);
                if (text.Length == 0)
                {
                    AddDiagnostic(diagnostics, ErrorCode.FTL_InvalidInputFileName, outputFileName);
                    text = (outputFileName = null);
                }
            }
            if (outputKind.IsNetModule())
            {
                compilationName = moduleAssemblyName;
            }
            else
            {
                if (moduleAssemblyName != null)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_AssemblyNameOnNonModule);
                }
                compilationName = text;
            }
            if (moduleName == null)
            {
                moduleName = outputFileName;
            }
        }

        private ImmutableArray<string> BuildSearchPaths(string? sdkDirectoryOpt, List<string> libPaths, List<string>? responsePathsOpt)
        {
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            if (sdkDirectoryOpt != null)
            {
                instance.Add(sdkDirectoryOpt);
            }
            instance.AddRange(libPaths);
            if (responsePathsOpt != null)
            {
                instance.AddRange(responsePathsOpt);
            }
            return instance.ToImmutableAndFree();
        }

        public static IEnumerable<string> ParseConditionalCompilationSymbols(string value, out IEnumerable<Diagnostic> diagnostics)
        {
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            value = value.TrimEnd(null);
            if (!value.IsEmpty() && (value.Last() == ';' || value.Last() == ','))
            {
                value = value.Substring(0, value.Length - 1);
            }
            string[] array = value.Split(';', ',');
            ArrayBuilder<string> arrayBuilder = new ArrayBuilder<string>(array.Length);
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i].Trim();
                if (SyntaxFacts.IsValidIdentifier(text))
                {
                    arrayBuilder.Add(text);
                    continue;
                }
                instance.Add(Diagnostic.Create(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, 2029, text));
            }
            diagnostics = instance.ToReadOnlyAndFree();
            return arrayBuilder.AsEnumerable();
        }

        private static Platform ParsePlatform(string value, IList<Diagnostic> diagnostics)
        {
            switch (value.ToLowerInvariant())
            {
                case "x86":
                    return Platform.X86;
                case "x64":
                    return Platform.X64;
                case "itanium":
                    return Platform.Itanium;
                case "anycpu":
                    return Platform.AnyCpu;
                case "anycpu32bitpreferred":
                    return Platform.AnyCpu32BitPreferred;
                case "arm":
                    return Platform.Arm;
                case "arm64":
                    return Platform.Arm64;
                default:
                    AddDiagnostic(diagnostics, ErrorCode.ERR_BadPlatformType, value);
                    return Platform.AnyCpu;
            }
        }

        private static OutputKind ParseTarget(string value, IList<Diagnostic> diagnostics)
        {
            switch (value.ToLowerInvariant())
            {
                case "exe":
                    return OutputKind.ConsoleApplication;
                case "winexe":
                    return OutputKind.WindowsApplication;
                case "library":
                    return OutputKind.DynamicallyLinkedLibrary;
                case "module":
                    return OutputKind.NetModule;
                case "appcontainerexe":
                    return OutputKind.WindowsRuntimeApplication;
                case "winmdobj":
                    return OutputKind.WindowsRuntimeMetadata;
                default:
                    AddDiagnostic(diagnostics, ErrorCode.FTL_InvalidTarget);
                    return OutputKind.ConsoleApplication;
            }
        }

        private static IEnumerable<string> ParseUsings(string arg, string? value, IList<Diagnostic> diagnostics)
        {
            if (RoslynString.IsNullOrEmpty(value))
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Namespace1.Localize(), arg);
                yield break;
            }
            string[] array = value!.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i];
            }
        }

        private static IEnumerable<CommandLineAnalyzerReference> ParseAnalyzers(string arg, string? value, List<Diagnostic> diagnostics)
        {
            if (value == null)
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), arg);
                yield break;
            }
            if (value!.Length == 0)
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
                yield break;
            }
            List<string> list = (from path in CommandLineParser.ParseSeparatedPaths(value)
                                 where !string.IsNullOrWhiteSpace(path)
                                 select path).ToList();
            foreach (string item in list)
            {
                yield return new CommandLineAnalyzerReference(item);
            }
        }

        private static IEnumerable<CommandLineReference> ParseAssemblyReferences(string arg, string? value, IList<Diagnostic> diagnostics, bool embedInteropTypes)
        {
            if (value == null)
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_SwitchNeedsString, MessageID.IDS_Text.Localize(), arg);
                yield break;
            }
            if (value!.Length == 0)
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
                yield break;
            }
            int num = value!.IndexOfAny(s_quoteOrEquals);
            string alias;
            if (num >= 0 && value![num] == '=')
            {
                alias = value!.Substring(0, num);
                value = value!.Substring(num + 1);
                if (!SyntaxFacts.IsValidIdentifier(alias))
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_BadExternIdentifier, alias);
                    yield break;
                }
            }
            else
            {
                alias = null;
            }
            List<string> list = (from path in CommandLineParser.ParseSeparatedPaths(value)
                                 where !string.IsNullOrWhiteSpace(path)
                                 select path).ToList();
            if (alias != null)
            {
                if (list.Count > 1)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_OneAliasPerReference, value);
                    yield break;
                }
                if (list.Count == 0)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_AliasMissingFile, alias);
                    yield break;
                }
            }
            foreach (string item in list)
            {
                ImmutableArray<string> aliases = ((alias != null) ? ImmutableArray.Create(alias) : ImmutableArray<string>.Empty);
                MetadataReferenceProperties properties = new MetadataReferenceProperties(MetadataImageKind.Assembly, aliases, embedInteropTypes);
                yield return new CommandLineReference(item, properties);
            }
        }

        private static void ValidateWin32Settings(string? win32ResourceFile, string? win32IconResourceFile, string? win32ManifestFile, OutputKind outputKind, IList<Diagnostic> diagnostics)
        {
            if (win32ResourceFile != null)
            {
                if (win32IconResourceFile != null)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_CantHaveWin32ResAndIcon);
                }
                if (win32ManifestFile != null)
                {
                    AddDiagnostic(diagnostics, ErrorCode.ERR_CantHaveWin32ResAndManifest);
                }
            }
            if (outputKind.IsNetModule() && win32ManifestFile != null)
            {
                AddDiagnostic(diagnostics, ErrorCode.WRN_CantHaveManifestForModule);
            }
        }

        private static IEnumerable<InstrumentationKind> ParseInstrumentationKinds(string value, IList<Diagnostic> diagnostics)
        {
            string[] array = value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] array2 = array;
            foreach (string text in array2)
            {
                if (text.ToLower() == "testcoverage")
                {
                    yield return InstrumentationKind.TestCoverage;
                    continue;
                }
                AddDiagnostic(diagnostics, ErrorCode.ERR_InvalidInstrumentationKind, text);
            }
        }

        internal static ResourceDescription? ParseResourceDescription(string arg, string resourceDescriptor, string? baseDirectory, IList<Diagnostic> diagnostics, bool embedded)
        {
            CommandLineParser.ParseResourceDescription(resourceDescriptor, baseDirectory, skipLeadingSeparators: false, out var filePath, out var fullPath, out var fileName, out var resourceName, out var accessibility);
            bool isPublic;
            if (accessibility == null)
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
                    AddDiagnostic(diagnostics, ErrorCode.ERR_BadResourceVis, accessibility);
                    return null;
                }
                isPublic = false;
            }
            if (RoslynString.IsNullOrWhiteSpace(filePath))
            {
                AddDiagnostic(diagnostics, ErrorCode.ERR_NoFileSpec, arg);
                return null;
            }
            if (!PathUtilities.IsValidFilePath(fullPath))
            {
                AddDiagnostic(diagnostics, ErrorCode.FTL_InvalidInputFileName, filePath);
                return null;
            }
            Stream dataProvider() => new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new ResourceDescription(resourceName, fileName, dataProvider, isPublic, embedded, checkArgs: false);
        }

        private static IEnumerable<string> ParseWarnings(string value)
        {
            value = value.Unquote();
            string[] array = value.Split(new char[3] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] array2 = array;
            foreach (string text in array2)
            {
                if (string.Equals(text, "nullable", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string nullableWarning in ErrorFacts.NullableWarnings)
                    {
                        yield return nullableWarning;
                    }
                    yield return Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance.GetIdForErrorCode(8632);
                    yield return Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance.GetIdForErrorCode(8669);
                }
                else if (ushort.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort result) && ErrorFacts.IsWarning((ErrorCode)result))
                {
                    yield return Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance.GetIdForErrorCode(result);
                }
                else
                {
                    yield return text;
                }
            }
        }

        private static void AddWarnings(Dictionary<string, ReportDiagnostic> d, ReportDiagnostic kind, IEnumerable<string> items)
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
            AddDiagnostic(diagnostics, ErrorCode.WRN_UnimplementedCommandLineSwitch, "/" + switchName);
        }

        public override void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> diagnostics)
        {
        }

        private static void AddDiagnostic(IList<Diagnostic> diagnostics, ErrorCode errorCode)
        {
            diagnostics.Add(Diagnostic.Create(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, (int)errorCode));
        }

        private static void AddDiagnostic(IList<Diagnostic> diagnostics, ErrorCode errorCode, params object[] arguments)
        {
            diagnostics.Add(Diagnostic.Create(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, (int)errorCode, arguments));
        }

        private static void AddDiagnostic(IList<Diagnostic> diagnostics, Dictionary<string, ReportDiagnostic> warningOptions, ErrorCode errorCode, params object[] arguments)
        {
            warningOptions.TryGetValue(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance.GetIdForErrorCode((int)errorCode), out var value);
            if (value != ReportDiagnostic.Suppress)
            {
                AddDiagnostic(diagnostics, errorCode, arguments);
            }
        }
    }
}
