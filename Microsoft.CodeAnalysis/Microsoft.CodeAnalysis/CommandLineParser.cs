using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class CommandLineParser
    {
        private readonly CommonMessageProvider _messageProvider;

        public readonly bool IsScriptCommandLineParser;

        private static readonly char[] s_searchPatternTrimChars = new char[8] { '\t', '\n', '\v', '\f', '\r', ' ', '\u0085', '\u00a0' };

        public const string ErrorLogOptionFormat = "<file>[,version={1|1.0|2|2.1}]";

        private static readonly char[] s_resourceSeparators = new char[1] { ',' };

        private static readonly char[] s_pathSeparators = new char[2] { ';', ',' };

        private static readonly char[] s_wildcards = new char[2] { '*', '?' };

        internal CommonMessageProvider MessageProvider => _messageProvider;

        protected abstract string RegularFileExtension { get; }

        protected abstract string ScriptFileExtension { get; }

        internal static string MismatchedVersionErrorText => CodeAnalysisResources.MismatchedVersion;

        public CommandLineParser(CommonMessageProvider messageProvider, bool isScriptCommandLineParser)
        {
            _messageProvider = messageProvider;
            IsScriptCommandLineParser = isScriptCommandLineParser;
        }

        internal virtual TextReader CreateTextFileReader(string fullPath)
        {
            return new StreamReader(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read), detectEncodingFromByteOrderMarks: true);
        }

        internal virtual IEnumerable<string> EnumerateFiles(string? directory, string fileNamePattern, SearchOption searchOption)
        {
            if (directory == null)
            {
                return SpecializedCollections.EmptyEnumerable<string>();
            }
            return Directory.EnumerateFiles(directory, fileNamePattern, searchOption);
        }

        public abstract CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory, string? sdkDirectory, string? additionalReferenceDirectories);

        public CommandLineArguments Parse(IEnumerable<string> args, string baseDirectory, string? sdkDirectory, string? additionalReferenceDirectories)
        {
            return CommonParse(args, baseDirectory, sdkDirectory, additionalReferenceDirectories);
        }

        private static bool IsOption(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                if (arg[0] != '/')
                {
                    return arg[0] == '-';
                }
                return true;
            }
            return false;
        }

        public static bool TryParseOption(string arg, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? name, out string? value)
        {
            if (!IsOption(arg))
            {
                name = null;
                value = null;
                return false;
            }
            if (arg == "-")
            {
                name = arg;
                value = null;
                return true;
            }
            int num = arg.IndexOf(':');
            if (arg.Length > 1 && arg[0] != '-')
            {
                int num2 = arg.IndexOf('/', 1);
                if (num2 > 0 && (num < 0 || num2 < num))
                {
                    name = null;
                    value = null;
                    return false;
                }
            }
            if (num >= 0)
            {
                name = arg.Substring(1, num - 1);
                value = arg.Substring(num + 1);
            }
            else
            {
                name = arg.Substring(1);
                value = null;
            }
            name = name!.ToLowerInvariant();
            return true;
        }

        public ErrorLogOptions? ParseErrorLogOptions(string arg, IList<Diagnostic> diagnostics, string? baseDirectory, out bool diagnosticAlreadyReported)
        {
            diagnosticAlreadyReported = false;
            IEnumerator<string> enumerator = ParseSeparatedStrings(arg, s_pathSeparators, StringSplitOptions.RemoveEmptyEntries).GetEnumerator();
            if (!enumerator.MoveNext() || string.IsNullOrEmpty(enumerator.Current))
            {
                return null;
            }
            string text = ParseGenericPathToFile(enumerator.Current, diagnostics, baseDirectory);
            if (text == null)
            {
                diagnosticAlreadyReported = true;
                return null;
            }
            SarifVersion result = SarifVersion.Sarif1;
            if (enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current))
            {
                string current = enumerator.Current;
                string text2 = "version=";
                int length = text2.Length;
                if (current.Length <= length || !current.Substring(0, length).Equals(text2, StringComparison.OrdinalIgnoreCase) || !SarifVersionFacts.TryParse(current.Substring(length), out result))
                {
                    return null;
                }
            }
            if (enumerator.MoveNext())
            {
                return null;
            }
            return new ErrorLogOptions(text, result);
        }

        internal static void ParseAndNormalizeFile(string unquoted, string? baseDirectory, out string? outputFileName, out string? outputDirectory, out string invalidPath)
        {
            outputFileName = null;
            outputDirectory = null;
            invalidPath = unquoted;
            string text = FileUtilities.ResolveRelativePath(unquoted, baseDirectory);
            if (text != null)
            {
                try
                {
                    text = (invalidPath = Path.GetFullPath(text));
                    outputFileName = Path.GetFileName(text);
                    outputDirectory = Path.GetDirectoryName(text);
                }
                catch (Exception)
                {
                    text = null;
                }
                if (outputFileName != null)
                {
                    outputFileName = RemoveTrailingSpacesAndDots(outputFileName);
                }
            }
            if (text == null || !MetadataHelpers.IsValidMetadataIdentifier(outputDirectory) || !MetadataHelpers.IsValidMetadataIdentifier(outputFileName))
            {
                outputFileName = null;
            }
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("path")]
        internal static string? RemoveTrailingSpacesAndDots(string? path)
        {
            if (path == null)
            {
                return path;
            }
            int length = path!.Length;
            for (int num = length - 1; num >= 0; num--)
            {
                char c = path![num];
                if (!char.IsWhiteSpace(c) && c != '.')
                {
                    if (num != length - 1)
                    {
                        return path!.Substring(0, num + 1);
                    }
                    return path;
                }
            }
            return string.Empty;
        }

        protected ImmutableArray<KeyValuePair<string, string>> ParsePathMap(string pathMap, IList<Diagnostic> errors)
        {
            if (pathMap.IsEmpty())
            {
                return ImmutableArray<KeyValuePair<string, string>>.Empty;
            }
            ArrayBuilder<KeyValuePair<string, string>> instance = ArrayBuilder<KeyValuePair<string, string>>.GetInstance();
            string[] array = SplitWithDoubledSeparatorEscaping(pathMap, ',');
            foreach (string text in array)
            {
                if (text.IsEmpty())
                {
                    continue;
                }
                string[] array2 = SplitWithDoubledSeparatorEscaping(text, '=');
                if (array2.Length != 2)
                {
                    errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.ERR_InvalidPathMap, text));
                    continue;
                }
                string text2 = array2[0];
                string text3 = array2[1];
                if (text2.Length == 0 || text3.Length == 0)
                {
                    errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.ERR_InvalidPathMap, text));
                }
                else
                {
                    text2 = PathUtilities.EnsureTrailingSeparator(text2);
                    text3 = PathUtilities.EnsureTrailingSeparator(text3);
                    instance.Add(new KeyValuePair<string, string>(text2, text3));
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal static string[] SplitWithDoubledSeparatorEscaping(string str, char separator)
        {
            if (str.Length == 0)
            {
                return new string[0];
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            PooledStringBuilder instance2 = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance2.Builder;
            int num = 0;
            while (num < str.Length)
            {
                char c = str[num++];
                if (c == separator)
                {
                    if (num >= str.Length || str[num] != separator)
                    {
                        instance.Add(builder.ToString());
                        builder.Clear();
                        continue;
                    }
                    num++;
                }
                builder.Append(c);
            }
            instance.Add(builder.ToString());
            instance2.Free();
            return instance.ToArrayAndFree();
        }

        public void ParseOutputFile(string value, IList<Diagnostic> errors, string? baseDirectory, out string? outputFileName, out string? outputDirectory)
        {
            ParseAndNormalizeFile(RemoveQuotesAndSlashes(value), baseDirectory, out outputFileName, out outputDirectory, out var invalidPath);
            if (outputFileName == null || !MetadataHelpers.IsValidAssemblyOrModuleName(outputFileName))
            {
                errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, invalidPath));
                outputFileName = null;
                outputDirectory = baseDirectory;
            }
        }

        public string? ParsePdbPath(string value, IList<Diagnostic> errors, string? baseDirectory)
        {
            string result = null;
            ParseAndNormalizeFile(RemoveQuotesAndSlashes(value), baseDirectory, out var outputFileName, out var outputDirectory, out var invalidPath);
            if (outputFileName == null || PathUtilities.ChangeExtension(outputFileName, null).Length == 0)
            {
                errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, invalidPath));
            }
            else
            {
                result = Path.ChangeExtension(Path.Combine(outputDirectory, outputFileName), ".pdb");
            }
            return result;
        }

        public string? ParseGenericPathToFile(string unquoted, IList<Diagnostic> errors, string? baseDirectory, bool generateDiagnostic = true)
        {
            string result = null;
            ParseAndNormalizeFile(unquoted, baseDirectory, out var outputFileName, out var outputDirectory, out var invalidPath);
            if (string.IsNullOrWhiteSpace(outputFileName))
            {
                if (generateDiagnostic)
                {
                    errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, invalidPath));
                }
            }
            else
            {
                result = Path.Combine(outputDirectory, outputFileName);
            }
            return result;
        }

        public void FlattenArgs(IEnumerable<string> rawArguments, IList<Diagnostic> diagnostics, List<string> processedArgs, List<string>? scriptArgsOpt, string? baseDirectory, List<string>? responsePaths = null)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            Stack<string> stack = new Stack<string>(rawArguments.Reverse());
            while (stack.Count > 0)
            {
                string text = stack.Pop().TrimEnd(new char[0]);
                if (flag)
                {
                    scriptArgsOpt!.Add(text);
                    continue;
                }
                if (scriptArgsOpt != null)
                {
                    if (flag2)
                    {
                        flag = true;
                        scriptArgsOpt!.Add(text);
                        continue;
                    }
                    if (!flag3 && text == "--")
                    {
                        flag3 = true;
                        processedArgs.Add(text);
                        continue;
                    }
                }
                if (!flag3 && text.StartsWith("@", StringComparison.Ordinal))
                {
                    string text2 = RemoveQuotesAndSlashes(text.Substring(1))!.TrimEnd(null);
                    string text3 = FileUtilities.ResolveRelativePath(text2, baseDirectory);
                    if (text3 != null)
                    {
                        foreach (string item in ParseResponseFile(text3, diagnostics).Reverse())
                        {
                            if (!string.Equals(item, "/noconfig", StringComparison.OrdinalIgnoreCase) && !string.Equals(item, "-noconfig", StringComparison.OrdinalIgnoreCase))
                            {
                                stack.Push(item);
                            }
                            else
                            {
                                diagnostics.Add(Diagnostic.Create(_messageProvider, _messageProvider.WRN_NoConfigNotOnCommandLine));
                            }
                        }
                        if (responsePaths != null)
                        {
                            string directoryName = PathUtilities.GetDirectoryName(text3);
                            if (directoryName == null)
                            {
                                diagnostics.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, text2));
                            }
                            else
                            {
                                responsePaths!.Add(FileUtilities.NormalizeAbsolutePath(directoryName));
                            }
                        }
                    }
                    else
                    {
                        diagnostics.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, text2));
                    }
                }
                else
                {
                    processedArgs.Add(text);
                    flag2 |= flag3 || !IsOption(text);
                }
            }
        }

        public static bool TryParseClientArgs(IEnumerable<string> args, out List<string>? parsedArgs, out bool containsShared, out string? keepAliveValue, out string? pipeName, out string? errorMessage)
        {
            containsShared = false;
            keepAliveValue = null;
            errorMessage = null;
            parsedArgs = null;
            pipeName = null;
            List<string> list = new List<string>();
            foreach (string arg in args)
            {
                if (isClientArgsOption(arg, "keepalive", out var hasValue2, out var optionValue2))
                {
                    if (string.IsNullOrEmpty(optionValue2))
                    {
                        errorMessage = CodeAnalysisResources.MissingKeepAlive;
                        return false;
                    }
                    if (!int.TryParse(optionValue2, out var result))
                    {
                        errorMessage = CodeAnalysisResources.KeepAliveIsNotAnInteger;
                        return false;
                    }
                    if (result < -1)
                    {
                        errorMessage = CodeAnalysisResources.KeepAliveIsTooSmall;
                        return false;
                    }
                    keepAliveValue = optionValue2;
                }
                else if (isClientArgsOption(arg, "shared", out hasValue2, out optionValue2))
                {
                    if (hasValue2)
                    {
                        if (string.IsNullOrEmpty(optionValue2))
                        {
                            errorMessage = CodeAnalysisResources.SharedArgumentMissing;
                            return false;
                        }
                        pipeName = optionValue2;
                    }
                    containsShared = true;
                }
                else
                {
                    list.Add(arg);
                }
            }
            if (keepAliveValue != null && !containsShared)
            {
                errorMessage = CodeAnalysisResources.KeepAliveWithoutShared;
                return false;
            }
            parsedArgs = list;
            return true;
            static bool isClientArgsOption(string arg, string optionName, out bool hasValue, out string? optionValue)
            {
                hasValue = false;
                optionValue = null;
                if (arg.Length == 0 || (arg[0] != '/' && arg[0] != '-'))
                {
                    return false;
                }
                arg = arg.Substring(1);
                if (!arg.StartsWith(optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (arg.Length > optionName.Length)
                {
                    if (arg[optionName.Length] != ':' && arg[optionName.Length] != '=')
                    {
                        return false;
                    }
                    hasValue = true;
                    optionValue = arg.Substring(optionName.Length + 1).Trim(new char[1] { '"' });
                }
                return true;
            }
        }

        internal IEnumerable<string> ParseResponseFile(string fullPath, IList<Diagnostic> errors)
        {
            List<string> list = new List<string>();
            try
            {
                using TextReader textReader = CreateTextFileReader(fullPath);
                string item;
                while ((item = textReader.ReadLine()) != null)
                {
                    list.Add(item);
                }
            }
            catch (Exception)
            {
                errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.ERR_OpenResponseFile, fullPath));
                return SpecializedCollections.EmptyEnumerable<string>();
            }
            return ParseResponseLines(list);
        }

        internal static IEnumerable<string> ParseResponseLines(IEnumerable<string> lines)
        {
            List<string> list = new List<string>();
            foreach (string line in lines)
            {
                list.AddRange(SplitCommandLineIntoArguments(line, removeHashComments: true));
            }
            return list;
        }

        public static void ParseResourceDescription(string resourceDescriptor, string? baseDirectory, bool skipLeadingSeparators, out string? filePath, out string? fullPath, out string? fileName, out string resourceName, out string? accessibility)
        {
            filePath = null;
            fullPath = null;
            fileName = null;
            resourceName = "";
            accessibility = null;
            string[] array = ParseSeparatedStrings(resourceDescriptor, s_resourceSeparators).ToArray();
            int i = 0;
            int num = array.Length;
            if (skipLeadingSeparators)
            {
                for (; i < num && string.IsNullOrEmpty(array[i]); i++)
                {
                }
                num -= i;
            }
            if (num >= 1)
            {
                filePath = RemoveQuotesAndSlashes(array[i]);
            }
            if (num >= 2)
            {
                resourceName = RemoveQuotesAndSlashes(array[i + 1]);
            }
            if (num >= 3)
            {
                accessibility = RemoveQuotesAndSlashes(array[i + 2]);
            }
            if (!RoslynString.IsNullOrWhiteSpace(filePath))
            {
                fileName = PathUtilities.GetFileName(filePath);
                fullPath = FileUtilities.ResolveRelativePath(filePath, baseDirectory);
                if (RoslynString.IsNullOrWhiteSpace(resourceName))
                {
                    resourceName = fileName;
                }
            }
        }

        public static IEnumerable<string> SplitCommandLineIntoArguments(string commandLine, bool removeHashComments)
        {
            return CommandLineUtilities.SplitCommandLineIntoArguments(commandLine, removeHashComments);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("arg")]
        public static string? RemoveQuotesAndSlashes(string? arg)
        {
            if (arg == null)
            {
                return arg;
            }
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            int i = 0;
            while (i < arg!.Length)
            {
                char c = arg![i];
                switch (c)
                {
                    case '\\':
                        ProcessSlashes(builder, arg, ref i);
                        break;
                    case '"':
                        i++;
                        break;
                    default:
                        builder.Append(c);
                        i++;
                        break;
                }
            }
            return instance.ToStringAndFree();
        }

        internal static void ProcessSlashes(StringBuilder builder, string arg, ref int i)
        {
            int num = 0;
            while (i < arg.Length && arg[i] == '\\')
            {
                num++;
                i++;
            }
            if (i < arg.Length && arg[i] == '"')
            {
                while (num >= 2)
                {
                    builder.Append('\\');
                    num -= 2;
                }
                if (num == 1)
                {
                    builder.Append('"');
                }
                i++;
            }
            else
            {
                while (num > 0)
                {
                    builder.Append('\\');
                    num--;
                }
            }
        }

        private static IEnumerable<string> Split(string? str, Func<char, bool> splitHere)
        {
            if (str == null)
            {
                yield break;
            }
            int num = 0;
            for (int c = 0; c < str!.Length; c++)
            {
                if (splitHere(str![c]))
                {
                    yield return str!.Substring(num, c - num);
                    num = c + 1;
                }
            }
            yield return str!.Substring(num);
        }

        public static IEnumerable<string> ParseSeparatedPaths(string? str)
        {
            return ParseSeparatedStrings(str, s_pathSeparators, StringSplitOptions.RemoveEmptyEntries).Select(RemoveQuotesAndSlashes);
        }

        internal static IEnumerable<string> ParseSeparatedStrings(string? str, char[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            char[] separators2 = separators;
            bool inQuotes = false;
            IEnumerable<string> enumerable = Split(str, delegate (char c)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                return !inQuotes && separators2.Contains(c);
            });
            if (options != StringSplitOptions.RemoveEmptyEntries)
            {
                return enumerable;
            }
            return enumerable.Where((string s) => s.Length > 0);
        }

        internal IEnumerable<string> ResolveRelativePaths(IEnumerable<string> paths, string baseDirectory, IList<Diagnostic> errors)
        {
            foreach (string path in paths)
            {
                string text = FileUtilities.ResolveRelativePath(path, baseDirectory);
                if (text == null)
                {
                    errors.Add(Diagnostic.Create(_messageProvider, _messageProvider.FTL_InvalidInputFileName, path));
                }
                else
                {
                    yield return text;
                }
            }
        }

        protected CommandLineSourceFile ToCommandLineSourceFile(string resolvedPath, bool isInputRedirected = false)
        {
            string extension = PathUtilities.GetExtension(resolvedPath);
            bool isScript = IsScriptCommandLineParser && !string.Equals(extension, RegularFileExtension, StringComparison.OrdinalIgnoreCase);
            return new CommandLineSourceFile(resolvedPath, isScript, isInputRedirected);
        }

        public IEnumerable<string> ParseFileArgument(string arg, string? baseDirectory, IList<Diagnostic> errors)
        {
            string text = RemoveQuotesAndSlashes(arg);
            if (text.IndexOfAny(s_wildcards) != -1)
            {
                foreach (string item in ExpandFileNamePattern(text, baseDirectory, SearchOption.TopDirectoryOnly, errors))
                {
                    yield return item;
                }
                yield break;
            }
            string text2 = FileUtilities.ResolveRelativePath(text, baseDirectory);
            if (text2 == null)
            {
                errors.Add(Diagnostic.Create(MessageProvider, MessageProvider.FTL_InvalidInputFileName, text));
            }
            else
            {
                yield return text2;
            }
        }

        protected IEnumerable<string> ParseSeparatedFileArgument(string value, string? baseDirectory, IList<Diagnostic> errors)
        {
            foreach (string item in from path in ParseSeparatedPaths(value)
                                    where !string.IsNullOrWhiteSpace(path)
                                    select path)
            {
                foreach (string item2 in ParseFileArgument(item, baseDirectory, errors))
                {
                    yield return item2;
                }
            }
        }

        public IEnumerable<CommandLineSourceFile> ParseRecurseArgument(string arg, string? baseDirectory, IList<Diagnostic> errors)
        {
            foreach (string item in ExpandFileNamePattern(arg, baseDirectory, SearchOption.AllDirectories, errors))
            {
                yield return ToCommandLineSourceFile(item);
            }
        }

        public static Encoding? TryParseEncodingName(string arg)
        {
            if (!string.IsNullOrWhiteSpace(arg) && long.TryParse(arg, NumberStyles.None, CultureInfo.InvariantCulture, out var result) && result > 0)
            {
                try
                {
                    return Encoding.GetEncoding((int)result);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static SourceHashAlgorithm TryParseHashAlgorithmName(string arg)
        {
            if (string.Equals("sha1", arg, StringComparison.OrdinalIgnoreCase))
            {
                return SourceHashAlgorithm.Sha1;
            }
            if (string.Equals("sha256", arg, StringComparison.OrdinalIgnoreCase))
            {
                return SourceHashAlgorithm.Sha256;
            }
            return SourceHashAlgorithm.None;
        }

        private IEnumerable<string> ExpandFileNamePattern(
            string path,
            string? baseDirectory,
            SearchOption searchOption,
            IList<Diagnostic> errors)
        {
            string? directory = PathUtilities.GetDirectoryName(path);
            string pattern = PathUtilities.GetFileName(path);

            var resolvedDirectoryPath = string.IsNullOrEmpty(directory) ?
                baseDirectory :
                FileUtilities.ResolveRelativePath(directory, baseDirectory);

            IEnumerator<string>? enumerator = null;
            try
            {
                bool yielded = false;

                // NOTE: Directory.EnumerateFiles(...) surprisingly treats pattern "." the 
                //       same way as "*"; as we don't expect anything to be found by this 
                //       pattern, let's just not search in this case
                pattern = pattern.Trim(s_searchPatternTrimChars);
                bool singleDotPattern = string.Equals(pattern, ".", StringComparison.Ordinal);

                if (!singleDotPattern)
                {
                    while (true)
                    {
                        string? resolvedPath = null;
                        try
                        {
                            if (enumerator == null)
                            {
                                enumerator = EnumerateFiles(resolvedDirectoryPath, pattern, searchOption).GetEnumerator();
                            }

                            if (!enumerator.MoveNext())
                            {
                                break;
                            }

                            resolvedPath = enumerator.Current;
                        }
                        catch
                        {
                            resolvedPath = null;
                        }

                        if (resolvedPath != null)
                        {
                            // just in case EnumerateFiles returned a relative path
                            resolvedPath = FileUtilities.ResolveRelativePath(resolvedPath, baseDirectory);
                        }

                        if (resolvedPath == null)
                        {
                            errors.Add(Diagnostic.Create(MessageProvider, (int)MessageProvider.FTL_InvalidInputFileName, path));
                            break;
                        }

                        yielded = true;
                        yield return resolvedPath;
                    }
                }

                // the pattern didn't match any files:
                if (!yielded)
                {
                    if (searchOption == SearchOption.AllDirectories)
                    {
                        // handling /recurse
                        GenerateErrorForNoFilesFoundInRecurse(path, errors);
                    }
                    else
                    {
                        // handling wildcard in file spec
                        errors.Add(Diagnostic.Create(MessageProvider, (int)MessageProvider.ERR_FileNotFound, path));
                    }
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
        }

        public abstract void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> errors);

        public ReportDiagnostic GetDiagnosticOptionsFromRulesetFile(string? fullPath, out Dictionary<string, ReportDiagnostic> diagnosticOptions, IList<Diagnostic> diagnostics)
        {
            return RuleSet.GetDiagnosticOptionsFromRulesetFile(fullPath, out diagnosticOptions, diagnostics, _messageProvider);
        }

        public static bool TryParseUInt64(string? value, out ulong result)
        {
            result = 0uL;
            if (RoslynString.IsNullOrEmpty(value))
            {
                return false;
            }
            int fromBase = 10;
            if (value!.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                fromBase = 16;
            }
            else if (value!.StartsWith("0", StringComparison.OrdinalIgnoreCase))
            {
                fromBase = 8;
            }
            try
            {
                result = Convert.ToUInt64(value, fromBase);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool TryParseUInt16(string? value, out ushort result)
        {
            result = 0;
            if (RoslynString.IsNullOrEmpty(value))
            {
                return false;
            }
            int fromBase = 10;
            if (value!.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                fromBase = 16;
            }
            else if (value!.StartsWith("0", StringComparison.OrdinalIgnoreCase))
            {
                fromBase = 8;
            }
            try
            {
                result = Convert.ToUInt16(value, fromBase);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static ImmutableDictionary<string, string> ParseFeatures(List<string> features)
        {
            ImmutableDictionary<string, string>.Builder builder = ImmutableDictionary.CreateBuilder<string, string>();
            CompilerOptionParseUtilities.ParseFeatures(builder, features);
            return builder.ToImmutable();
        }

        public static ImmutableArray<KeyValuePair<string, string>> SortPathMap(ImmutableArray<KeyValuePair<string, string>> pathMap)
        {
            return pathMap.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => -x.Key.Length.CompareTo(y.Key.Length));
        }
    }
}
