using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class AnalyzerConfig
    {
        internal sealed class Section
        {
            public static StringComparison NameComparer { get; } = StringComparison.Ordinal;


            public static IEqualityComparer<string> NameEqualityComparer { get; } = StringComparer.Ordinal;


            public static StringComparer PropertiesKeyComparer { get; } = CaseInsensitiveComparison.Comparer;


            public string Name { get; }

            public ImmutableDictionary<string, string> Properties { get; }

            public Section(string name, ImmutableDictionary<string, string> properties)
            {
                Name = name;
                Properties = properties;
            }
        }

        internal readonly struct SectionNameMatcher
        {
            private readonly ImmutableArray<(int minValue, int maxValue)> _numberRangePairs;

            internal Regex Regex { get; }

            internal SectionNameMatcher(Regex regex, ImmutableArray<(int minValue, int maxValue)> numberRangePairs)
            {
                Regex = regex;
                _numberRangePairs = numberRangePairs;
            }

            public bool IsMatch(string s)
            {
                if (_numberRangePairs.IsEmpty)
                {
                    return Regex.IsMatch(s);
                }
                Match match = Regex.Match(s);
                if (!match.Success)
                {
                    return false;
                }
                for (int i = 0; i < _numberRangePairs.Length; i++)
                {
                    var (num, num2) = _numberRangePairs[i];
                    if (!int.TryParse(match.Groups[i + 1].Value, out var result) || result < num || result > num2)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private struct SectionNameLexer
        {
            private readonly string _sectionName;

            public int Position { get; set; }

            public bool IsDone => Position >= _sectionName.Length;

            public char CurrentCharacter => _sectionName[Position];

            public char this[int position] => _sectionName[position];

            public SectionNameLexer(string sectionName)
            {
                _sectionName = sectionName;
                Position = 0;
            }

            public TokenKind Lex()
            {
                _ = Position;
                switch (_sectionName[Position])
                {
                    case '*':
                        {
                            int num = Position + 1;
                            if (num < _sectionName.Length && _sectionName[num] == '*')
                            {
                                Position += 2;
                                return TokenKind.StarStar;
                            }
                            Position++;
                            return TokenKind.Star;
                        }
                    case '?':
                        Position++;
                        return TokenKind.Question;
                    case '{':
                        Position++;
                        return TokenKind.OpenCurly;
                    case ',':
                        Position++;
                        return TokenKind.Comma;
                    case '}':
                        Position++;
                        return TokenKind.CloseCurly;
                    case '[':
                        Position++;
                        return TokenKind.OpenBracket;
                    case '\\':
                        Position++;
                        if (IsDone)
                        {
                            return TokenKind.BadToken;
                        }
                        return TokenKind.SimpleCharacter;
                    default:
                        return TokenKind.SimpleCharacter;
                }
            }

            public char EatCurrentCharacter()
            {
                return _sectionName[Position++];
            }

            public bool TryEatCurrentCharacter(out char nextChar)
            {
                if (IsDone)
                {
                    nextChar = '\0';
                    return false;
                }
                nextChar = EatCurrentCharacter();
                return true;
            }

            public string? TryLexNumber()
            {
                bool flag = true;
                StringBuilder stringBuilder = new();
                while (!IsDone)
                {
                    char currentCharacter = CurrentCharacter;
                    if (flag && currentCharacter == '-')
                    {
                        Position++;
                        stringBuilder.Append('-');
                    }
                    else
                    {
                        if (!char.IsDigit(currentCharacter))
                        {
                            break;
                        }
                        Position++;
                        stringBuilder.Append(currentCharacter);
                    }
                    flag = false;
                }
                string text = stringBuilder.ToString();
                if (text.Length != 0 && !(text == "-"))
                {
                    return text;
                }
                return null;
            }
        }

        private enum TokenKind
        {
            BadToken,
            SimpleCharacter,
            Star,
            StarStar,
            Question,
            OpenCurly,
            CloseCurly,
            Comma,
            DoubleDot,
            OpenBracket
        }

        private static readonly Regex s_sectionMatcher = new("^\\s*\\[(([^#;]|\\\\#|\\\\;)+)\\]\\s*([#;].*)?$", RegexOptions.Compiled);

        private static readonly Regex s_propertyMatcher = new("^\\s*([\\w\\.\\-_]+)\\s*[=:]\\s*(.*?)\\s*([#;].*)?$", RegexOptions.Compiled);

        internal const string GlobalKey = "is_global";

        internal const string GlobalLevelKey = "global_level";

        internal const string UserGlobalConfigName = ".globalconfig";

        private readonly bool _hasGlobalFileName;

        internal static ImmutableHashSet<string> ReservedKeys { get; } = ImmutableHashSet.CreateRange(Section.PropertiesKeyComparer, new string[8] { "root", "indent_style", "indent_size", "tab_width", "end_of_line", "charset", "trim_trailing_whitespace", "insert_final_newline" });


        internal static ImmutableHashSet<string> ReservedValues { get; } = ImmutableHashSet.CreateRange(CaseInsensitiveComparison.Comparer, new string[1] { "unset" });


        internal Section GlobalSection { get; }

        internal string NormalizedDirectory { get; }

        internal string PathToFile { get; }

        internal static Comparer<AnalyzerConfig> DirectoryLengthComparer { get; } = Comparer<AnalyzerConfig>.Create((AnalyzerConfig e1, AnalyzerConfig e2) => e1.NormalizedDirectory.Length.CompareTo(e2.NormalizedDirectory.Length));


        internal ImmutableArray<Section> NamedSections { get; }

        internal bool IsRoot
        {
            get
            {
                if (GlobalSection.Properties.TryGetValue("root", out var value))
                {
                    return value == "true";
                }
                return false;
            }
        }

        internal bool IsGlobal
        {
            get
            {
                if (!_hasGlobalFileName)
                {
                    return GlobalSection.Properties.ContainsKey("is_global");
                }
                return true;
            }
        }

        internal int GlobalLevel
        {
            get
            {
                if (GlobalSection.Properties.TryGetValue("global_level", out var value) && int.TryParse(value, out var result))
                {
                    return result;
                }
                if (_hasGlobalFileName)
                {
                    return 100;
                }
                return 0;
            }
        }

        private AnalyzerConfig(Section globalSection, ImmutableArray<Section> namedSections, string pathToFile)
        {
            GlobalSection = globalSection;
            NamedSections = namedSections;
            PathToFile = pathToFile;
            _hasGlobalFileName = Path.GetFileName(pathToFile).Equals(".globalconfig", StringComparison.OrdinalIgnoreCase);
            string p = Path.GetDirectoryName(pathToFile) ?? pathToFile;
            NormalizedDirectory = PathUtilities.NormalizeWithForwardSlash(p);
        }

        public static AnalyzerConfig Parse(string text, string? pathToFile)
        {
            return Parse(SourceText.From(text), pathToFile);
        }

        public static AnalyzerConfig Parse(SourceText text, string? pathToFile)
        {
            if (pathToFile == null || !Path.IsPathRooted(pathToFile) || string.IsNullOrEmpty(Path.GetFileName(pathToFile)))
            {
                throw new ArgumentException("Must be an absolute path to an editorconfig file", "pathToFile");
            }
            Section globalSection = null;
            ImmutableArray<Section>.Builder namedSectionBuilder = ImmutableArray.CreateBuilder<Section>();
            ImmutableDictionary<string, string>.Builder activeSectionProperties = ImmutableDictionary.CreateBuilder<string, string>(Section.PropertiesKeyComparer);
            string activeSectionName = "";
            foreach (TextLine line in text.Lines)
            {
                string text2 = line.ToString();
                if (string.IsNullOrWhiteSpace(text2) || IsComment(text2))
                {
                    continue;
                }
                MatchCollection matchCollection = s_sectionMatcher.Matches(text2);
                if (matchCollection.Count > 0 && matchCollection[0].Groups.Count > 0)
                {
                    addNewSection();
                    activeSectionName = matchCollection[0].Groups[1].Value;
                    activeSectionProperties = ImmutableDictionary.CreateBuilder<string, string>(Section.PropertiesKeyComparer);
                    continue;
                }
                MatchCollection matchCollection2 = s_propertyMatcher.Matches(text2);
                if (matchCollection2.Count > 0 && matchCollection2[0].Groups.Count > 1)
                {
                    string value = matchCollection2[0].Groups[1].Value;
                    string text3 = matchCollection2[0].Groups[2].Value;
                    value = CaseInsensitiveComparison.ToLower(value);
                    if (ReservedKeys.Contains(value) || ReservedValues.Contains(text3))
                    {
                        text3 = CaseInsensitiveComparison.ToLower(text3);
                    }
                    activeSectionProperties[value] = text3 ?? "";
                }
            }
            addNewSection();
            return new AnalyzerConfig(globalSection, namedSectionBuilder.ToImmutable(), pathToFile);
            void addNewSection()
            {
                Section section = new(activeSectionName, activeSectionProperties.ToImmutable());
                if (activeSectionName == "")
                {
                    globalSection = section;
                }
                else
                {
                    namedSectionBuilder.Add(section);
                }
            }
        }

        private static bool IsComment(string line)
        {
            foreach (char c in line)
            {
                if (!char.IsWhiteSpace(c))
                {
                    if (c != '#')
                    {
                        return c == ';';
                    }
                    return true;
                }
            }
            return false;
        }

        internal static SectionNameMatcher? TryCreateSectionNameMatcher(string sectionName)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append('^');
            if (!sectionName.Contains("/"))
            {
                stringBuilder.Append(".*/");
            }
            else if (sectionName[0] != '/')
            {
                stringBuilder.Append('/');
            }
            SectionNameLexer lexer = new(sectionName);
            ArrayBuilder<(int, int)> instance = ArrayBuilder<(int, int)>.GetInstance();
            if (!TryCompilePathList(ref lexer, stringBuilder, parsingChoice: false, instance))
            {
                instance.Free();
                return null;
            }
            stringBuilder.Append('$');
            return new SectionNameMatcher(new Regex(stringBuilder.ToString(), RegexOptions.Compiled), instance.ToImmutableAndFree());
        }

        internal static bool IsAbsoluteEditorConfigPath(string sectionName)
        {
            SectionNameLexer sectionNameLexer = new SectionNameLexer(sectionName);
            bool flag = false;
            int num = 0;
            while (!sectionNameLexer.IsDone)
            {
                if (sectionNameLexer.Lex() != TokenKind.SimpleCharacter)
                {
                    return false;
                }
                char c = sectionNameLexer.EatCurrentCharacter();
                if (num == 0)
                {
                    if (c == '/')
                    {
                        flag = true;
                    }
                    else if (Path.DirectorySeparatorChar == '/')
                    {
                        return false;
                    }
                }
                else if (!flag && Path.DirectorySeparatorChar == '\\')
                {
                    if (num == 1 && c != ':')
                    {
                        return false;
                    }
                    if (num == 2)
                    {
                        if (c != '/')
                        {
                            return false;
                        }
                        flag = true;
                    }
                }
                num++;
            }
            return flag;
        }

        private static bool TryCompilePathList(ref SectionNameLexer lexer, StringBuilder sb, bool parsingChoice, ArrayBuilder<(int minValue, int maxValue)> numberRangePairs)
        {
            while (!lexer.IsDone)
            {
                TokenKind tokenKind = lexer.Lex();
                switch (tokenKind)
                {
                    case TokenKind.BadToken:
                        return false;
                    case TokenKind.SimpleCharacter:
                        sb.Append(Regex.Escape(lexer.EatCurrentCharacter().ToString()));
                        break;
                    case TokenKind.Question:
                        sb.Append('.');
                        break;
                    case TokenKind.Star:
                        sb.Append("[^/]*");
                        break;
                    case TokenKind.StarStar:
                        sb.Append(".*");
                        break;
                    case TokenKind.OpenCurly:
                        {
                            lexer.Position--;
                            (string, string)? tuple = TryParseNumberRange(ref lexer);
                            if (!tuple.HasValue)
                            {
                                if (!TryCompileChoice(ref lexer, sb, numberRangePairs))
                                {
                                    return false;
                                }
                                break;
                            }
                            var (s, s2) = tuple.GetValueOrDefault();
                            if (int.TryParse(s, out var result) && int.TryParse(s2, out var result2))
                            {
                                (int, int) item = ((result < result2) ? (result, result2) : (result2, result));
                                numberRangePairs.Add(item);
                                sb.Append("(-?[0-9]+)");
                                break;
                            }
                            return false;
                        }
                    case TokenKind.CloseCurly:
                        return parsingChoice;
                    case TokenKind.Comma:
                        return parsingChoice;
                    case TokenKind.OpenBracket:
                        sb.Append('[');
                        if (!TryCompileCharacterClass(ref lexer, sb))
                        {
                            return false;
                        }
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(tokenKind);
                }
            }
            return !parsingChoice;
        }

        private static bool TryCompileCharacterClass(ref SectionNameLexer lexer, StringBuilder sb)
        {
            if (!lexer.IsDone && lexer.CurrentCharacter == '!')
            {
                sb.Append('^');
                lexer.Position++;
            }
            while (!lexer.IsDone)
            {
                char c = lexer.EatCurrentCharacter();
                switch (c)
                {
                    case '-':
                        sb.Append(c);
                        break;
                    case '\\':
                        if (lexer.IsDone)
                        {
                            return false;
                        }
                        sb.Append('\\');
                        sb.Append(lexer.EatCurrentCharacter());
                        break;
                    case ']':
                        sb.Append(c);
                        return true;
                    default:
                        sb.Append(Regex.Escape(c.ToString()));
                        break;
                }
            }
            return false;
        }

        private static bool TryCompileChoice(ref SectionNameLexer lexer, StringBuilder sb, ArrayBuilder<(int, int)> numberRangePairs)
        {
            if (lexer.Lex() != TokenKind.OpenCurly)
            {
                return false;
            }
            sb.Append("(?:");
            while (TryCompilePathList(ref lexer, sb, parsingChoice: true, numberRangePairs))
            {
                char c = lexer[lexer.Position - 1];
                switch (c)
                {
                    case ',':
                        break;
                    case '}':
                        sb.Append(")");
                        return true;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(c);
                }
                sb.Append("|");
            }
            return false;
        }

        private static (string numStart, string numEnd)? TryParseNumberRange(ref SectionNameLexer lexer)
        {
            int position = lexer.Position;
            if (lexer.Lex() != TokenKind.OpenCurly)
            {
                lexer.Position = position;
                return null;
            }
            string text = lexer.TryLexNumber();
            if (text == null)
            {
                lexer.Position = position;
                return null;
            }
            if (!lexer.TryEatCurrentCharacter(out var nextChar) || nextChar != '.' || !lexer.TryEatCurrentCharacter(out nextChar) || nextChar != '.')
            {
                lexer.Position = position;
                return null;
            }
            string text2 = lexer.TryLexNumber();
            if (text2 == null || lexer.IsDone || lexer.Lex() != TokenKind.CloseCurly)
            {
                lexer.Position = position;
                return null;
            }
            return (text, text2);
        }
    }
}
