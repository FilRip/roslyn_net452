using System;
using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public class Lexer : AbstractLexer
    {
        internal struct TokenInfo
        {
            internal SyntaxKind Kind;

            internal SyntaxKind ContextualKind;

            internal string Text;

            internal SpecialType ValueKind;

            internal bool RequiresTextForXmlEntity;

            internal bool HasIdentifierEscapeSequence;

            internal string StringValue;

            internal char CharValue;

            internal int IntValue;

            internal uint UintValue;

            internal long LongValue;

            internal ulong UlongValue;

            internal float FloatValue;

            internal double DoubleValue;

            internal decimal DecimalValue;

            internal bool IsVerbatim;
        }

        internal struct Interpolation
        {
            public readonly int OpenBracePosition;

            public readonly int ColonPosition;

            public readonly int CloseBracePosition;

            public readonly bool CloseBraceMissing;

            public bool ColonMissing => ColonPosition <= 0;

            public bool HasColon => ColonPosition > 0;

            public int LastPosition
            {
                get
                {
                    if (!CloseBraceMissing)
                    {
                        return CloseBracePosition;
                    }
                    return CloseBracePosition - 1;
                }
            }

            public int FormatEndPosition => CloseBracePosition - 1;

            public Interpolation(int openBracePosition, int colonPosition, int closeBracePosition, bool closeBraceMissing)
            {
                OpenBracePosition = openBracePosition;
                ColonPosition = colonPosition;
                CloseBracePosition = closeBracePosition;
                CloseBraceMissing = closeBraceMissing;
            }
        }

        private class InterpolatedStringScanner
        {
            public readonly Lexer lexer;

            public bool isVerbatim;

            public bool allowNewlines;

            public SyntaxDiagnosticInfo error;

            public InterpolatedStringScanner(Lexer lexer, bool isVerbatim)
            {
                this.lexer = lexer;
                this.isVerbatim = isVerbatim;
                allowNewlines = isVerbatim;
            }

            private bool IsAtEnd()
            {
                return IsAtEnd(isVerbatim && allowNewlines);
            }

            private bool IsAtEnd(bool allowNewline)
            {
                char c = lexer.TextWindow.PeekChar();
                if (allowNewline || !SyntaxFacts.IsNewLine(c))
                {
                    if (c == '\uffff')
                    {
                        return lexer.TextWindow.IsReallyAtEnd();
                    }
                    return false;
                }
                return true;
            }

            internal void ScanInterpolatedStringLiteralTop(ArrayBuilder<Interpolation> interpolations, ref TokenInfo info, out bool closeQuoteMissing)
            {
                if (isVerbatim)
                {
                    lexer.TextWindow.AdvanceChar();
                    lexer.TextWindow.AdvanceChar();
                }
                else
                {
                    lexer.TextWindow.AdvanceChar();
                }
                lexer.TextWindow.AdvanceChar();
                ScanInterpolatedStringLiteralContents(interpolations);
                if (lexer.TextWindow.PeekChar() != '"')
                {
                    if (error == null)
                    {
                        int position = (IsAtEnd(allowNewline: true) ? (lexer.TextWindow.Position - 1) : lexer.TextWindow.Position);
                        error = lexer.MakeError(position, 1, isVerbatim ? ErrorCode.ERR_UnterminatedStringLit : ErrorCode.ERR_NewlineInConst);
                    }
                    closeQuoteMissing = true;
                }
                else
                {
                    lexer.TextWindow.AdvanceChar();
                    closeQuoteMissing = false;
                }
                info.Kind = SyntaxKind.InterpolatedStringToken;
            }

            private void ScanInterpolatedStringLiteralContents(ArrayBuilder<Interpolation> interpolations)
            {
                while (!IsAtEnd())
                {
                    switch (lexer.TextWindow.PeekChar())
                    {
                        case '"':
                            if (RecoveringFromRunawayLexing() || !isVerbatim || lexer.TextWindow.PeekChar(1) != '"')
                            {
                                return;
                            }
                            lexer.TextWindow.AdvanceChar();
                            lexer.TextWindow.AdvanceChar();
                            continue;
                        case '}':
                            {
                                int position2 = lexer.TextWindow.Position;
                                lexer.TextWindow.AdvanceChar();
                                if (lexer.TextWindow.PeekChar() == '}')
                                {
                                    lexer.TextWindow.AdvanceChar();
                                }
                                else if (error == null)
                                {
                                    error = lexer.MakeError(position2, 1, ErrorCode.ERR_UnescapedCurly, "}");
                                }
                                continue;
                            }
                        case '{':
                            {
                                if (lexer.TextWindow.PeekChar(1) == '{')
                                {
                                    lexer.TextWindow.AdvanceChar();
                                    lexer.TextWindow.AdvanceChar();
                                    continue;
                                }
                                int position3 = lexer.TextWindow.Position;
                                lexer.TextWindow.AdvanceChar();
                                int colonPosition = 0;
                                ScanInterpolatedStringLiteralHoleBalancedText('}', isHole: true, ref colonPosition);
                                int position4 = lexer.TextWindow.Position;
                                bool closeBraceMissing = false;
                                if (lexer.TextWindow.PeekChar() == '}')
                                {
                                    lexer.TextWindow.AdvanceChar();
                                }
                                else
                                {
                                    closeBraceMissing = true;
                                    if (error == null)
                                    {
                                        error = lexer.MakeError(position3 - 1, 2, ErrorCode.ERR_UnclosedExpressionHole);
                                    }
                                }
                                interpolations?.Add(new Interpolation(position3, colonPosition, position4, closeBraceMissing));
                                continue;
                            }
                        case '\\':
                            if (!isVerbatim)
                            {
                                int position = lexer.TextWindow.Position;
                                char c = lexer.ScanEscapeSequence(out char surrogateCharacter);
                                if ((c == '{' || c == '}') && error == null)
                                {
                                    error = lexer.MakeError(position, lexer.TextWindow.Position - position, ErrorCode.ERR_EscapedCurly, c);
                                }
                                continue;
                            }
                            break;
                    }
                    lexer.TextWindow.AdvanceChar();
                }
            }

            private void ScanFormatSpecifier()
            {
                lexer.TextWindow.AdvanceChar();
                while (true)
                {
                    char c = lexer.TextWindow.PeekChar();
                    if (c == '\\' && !isVerbatim)
                    {
                        int position = lexer.TextWindow.Position;
                        c = lexer.ScanEscapeSequence(out var _);
                        if ((c == '{' || c == '}') && error == null)
                        {
                            error = lexer.MakeError(position, 1, ErrorCode.ERR_EscapedCurly, c);
                        }
                        continue;
                    }
                    switch (c)
                    {
                        case '"':
                            if (isVerbatim && lexer.TextWindow.PeekChar(1) == '"')
                            {
                                lexer.TextWindow.AdvanceChar();
                                lexer.TextWindow.AdvanceChar();
                                break;
                            }
                            return;
                        case '{':
                            {
                                int position2 = lexer.TextWindow.Position;
                                lexer.TextWindow.AdvanceChar();
                                if (lexer.TextWindow.PeekChar() == '{')
                                {
                                    lexer.TextWindow.AdvanceChar();
                                }
                                else if (error == null)
                                {
                                    error = lexer.MakeError(position2, 1, ErrorCode.ERR_UnescapedCurly, "{");
                                }
                                break;
                            }
                        case '}':
                            if (lexer.TextWindow.PeekChar(1) == '}')
                            {
                                lexer.TextWindow.AdvanceChar();
                                lexer.TextWindow.AdvanceChar();
                                break;
                            }
                            return;
                        default:
                            if (IsAtEnd())
                            {
                                return;
                            }
                            lexer.TextWindow.AdvanceChar();
                            break;
                    }
                }
            }

            private void ScanInterpolatedStringLiteralHoleBalancedText(char endingChar, bool isHole, ref int colonPosition)
            {
                while (!IsAtEnd())
                {
                    char c = lexer.TextWindow.PeekChar();
                    switch (c)
                    {
                        case '#':
                            if (error == null)
                            {
                                error = lexer.MakeError(lexer.TextWindow.Position, 1, ErrorCode.ERR_SyntaxError, endingChar.ToString());
                            }
                            lexer.TextWindow.AdvanceChar();
                            continue;
                        case '$':
                            if (lexer.TextWindow.PeekChar(1) == '"' || (lexer.TextWindow.PeekChar(1) == '@' && lexer.TextWindow.PeekChar(2) == '"'))
                            {
                                bool flag3 = lexer.TextWindow.PeekChar(1) == '@';
                                ArrayBuilder<Interpolation> interpolations2 = null;
                                TokenInfo info2 = default(TokenInfo);
                                bool flag4 = isVerbatim;
                                bool flag5 = allowNewlines;
                                try
                                {
                                    isVerbatim = flag3;
                                    allowNewlines &= isVerbatim;
                                    ScanInterpolatedStringLiteralTop(interpolations2, ref info2, out var _);
                                }
                                finally
                                {
                                    isVerbatim = flag4;
                                    allowNewlines = flag5;
                                }
                                continue;
                            }
                            break;
                        case ':':
                            if (isHole)
                            {
                                colonPosition = lexer.TextWindow.Position;
                                ScanFormatSpecifier();
                                return;
                            }
                            break;
                        case ')':
                        case ']':
                        case '}':
                            if (c == endingChar)
                            {
                                return;
                            }
                            if (error == null)
                            {
                                error = lexer.MakeError(lexer.TextWindow.Position, 1, ErrorCode.ERR_SyntaxError, endingChar.ToString());
                            }
                            break;
                        case '"':
                            if (RecoveringFromRunawayLexing())
                            {
                                return;
                            }
                            goto case '\'';
                        case '\'':
                            ScanInterpolatedStringLiteralNestedString();
                            continue;
                        case '@':
                            if (lexer.TextWindow.PeekChar(1) == '"' && !RecoveringFromRunawayLexing())
                            {
                                ScanInterpolatedStringLiteralNestedVerbatimString();
                                continue;
                            }
                            if (lexer.TextWindow.PeekChar(1) == '$' && lexer.TextWindow.PeekChar(2) == '"')
                            {
                                lexer.CheckFeatureAvailability(MessageID.IDS_FeatureAltInterpolatedVerbatimStrings);
                                ArrayBuilder<Interpolation> interpolations = null;
                                TokenInfo info = default(TokenInfo);
                                bool flag = isVerbatim;
                                bool flag2 = allowNewlines;
                                try
                                {
                                    isVerbatim = true;
                                    allowNewlines = true;
                                    ScanInterpolatedStringLiteralTop(interpolations, ref info, out var _);
                                }
                                finally
                                {
                                    isVerbatim = flag;
                                    allowNewlines = flag2;
                                }
                                continue;
                            }
                            break;
                        case '/':
                            switch (lexer.TextWindow.PeekChar(1))
                            {
                                case '/':
                                    if (isVerbatim && allowNewlines)
                                    {
                                        lexer.TextWindow.AdvanceChar();
                                        lexer.TextWindow.AdvanceChar();
                                        while (!IsAtEnd(allowNewline: false))
                                        {
                                            lexer.TextWindow.AdvanceChar();
                                        }
                                        break;
                                    }
                                    if (error == null)
                                    {
                                        error = lexer.MakeError(lexer.TextWindow.Position, 2, ErrorCode.ERR_SingleLineCommentInExpressionHole);
                                    }
                                    lexer.TextWindow.AdvanceChar();
                                    lexer.TextWindow.AdvanceChar();
                                    break;
                                case '*':
                                    ScanInterpolatedStringLiteralNestedComment();
                                    break;
                                default:
                                    lexer.TextWindow.AdvanceChar();
                                    break;
                            }
                            continue;
                        case '{':
                            ScanInterpolatedStringLiteralHoleBracketed('{', '}');
                            continue;
                        case '(':
                            ScanInterpolatedStringLiteralHoleBracketed('(', ')');
                            continue;
                        case '[':
                            ScanInterpolatedStringLiteralHoleBracketed('[', ']');
                            continue;
                    }
                    lexer.TextWindow.AdvanceChar();
                }
            }

            private bool RecoveringFromRunawayLexing()
            {
                return error != null;
            }

            private void ScanInterpolatedStringLiteralNestedComment()
            {
                lexer.TextWindow.AdvanceChar();
                lexer.TextWindow.AdvanceChar();
                char num;
                do
                {
                    if (IsAtEnd())
                    {
                        return;
                    }
                    num = lexer.TextWindow.PeekChar();
                    lexer.TextWindow.AdvanceChar();
                }
                while (num != '*' || lexer.TextWindow.PeekChar() != '/');
                lexer.TextWindow.AdvanceChar();
            }

            private void ScanInterpolatedStringLiteralNestedString()
            {
                TokenInfo info = default(TokenInfo);
                lexer.ScanStringLiteral(ref info);
            }

            private void ScanInterpolatedStringLiteralNestedVerbatimString()
            {
                TokenInfo info = default(TokenInfo);
                lexer.ScanVerbatimStringLiteral(ref info, allowNewlines);
            }

            private void ScanInterpolatedStringLiteralHoleBracketed(char start, char end)
            {
                lexer.TextWindow.AdvanceChar();
                int colonPosition = 0;
                ScanInterpolatedStringLiteralHoleBalancedText(end, isHole: false, ref colonPosition);
                if (lexer.TextWindow.PeekChar() == end)
                {
                    lexer.TextWindow.AdvanceChar();
                }
            }
        }

        private enum QuickScanState : byte
        {
            Initial,
            FollowingWhite,
            FollowingCR,
            Ident,
            Number,
            Punctuation,
            Dot,
            CompoundPunctStart,
            DoneAfterNext,
            Done,
            Bad
        }

        private enum CharFlags : byte
        {
            White,
            CR,
            LF,
            Letter,
            Digit,
            Punct,
            Dot,
            CompoundPunctStart,
            Slash,
            Complex,
            EndOfFile
        }

        private const int TriviaListInitialCapacity = 8;

        private readonly CSharpParseOptions _options;

        private LexerMode _mode;

        private readonly StringBuilder _builder;

        private char[] _identBuffer;

        private int _identLen;

        private DirectiveStack _directives;

        private readonly LexerCache _cache;

        private readonly bool _allowPreprocessorDirectives;

        private readonly bool _interpolationFollowedByColon;

        private DocumentationCommentParser _xmlParser;

        private int _badTokenCount;

        private SyntaxListBuilder _leadingTriviaCache = new SyntaxListBuilder(10);

        private SyntaxListBuilder _trailingTriviaCache = new SyntaxListBuilder(10);

        private static readonly int s_conflictMarkerLength = "<<<<<<<".Length;

        private Func<SyntaxTrivia> _createWhitespaceTriviaFunction;

        internal const int MaxCachedTokenSize = 42;

        private static readonly byte[,] s_stateTransitions = new byte[9, 11]
        {
            {
                0, 0, 0, 3, 4, 5, 6, 7, 10, 10,
                10
            },
            {
                1, 2, 8, 9, 9, 9, 9, 9, 10, 10,
                9
            },
            {
                9, 9, 8, 9, 9, 9, 9, 9, 9, 9,
                9
            },
            {
                1, 2, 8, 3, 3, 9, 9, 9, 10, 10,
                9
            },
            {
                1, 2, 8, 10, 4, 9, 10, 9, 10, 10,
                9
            },
            {
                1, 2, 8, 9, 9, 9, 9, 9, 10, 10,
                9
            },
            {
                1, 2, 8, 9, 4, 9, 10, 9, 10, 10,
                9
            },
            {
                1, 2, 8, 9, 9, 10, 9, 10, 10, 10,
                9
            },
            {
                9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
                9
            }
        };

        private readonly Func<SyntaxToken> _createQuickTokenFunction;

        private static readonly byte[] s_charProperties = new byte[384]
        {
            9, 9, 9, 9, 9, 9, 9, 9, 9, 0,
            2, 0, 0, 1, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 0, 7, 9, 9, 9, 7, 7, 9,
            5, 5, 7, 7, 5, 7, 6, 8, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 7, 5,
            7, 7, 7, 7, 9, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 5, 9, 5, 7, 3, 9, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 5, 7, 5, 7, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            3, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 3, 9, 9, 9, 9, 3, 9, 9, 9,
            9, 9, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 9, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 9, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3
        };

        public bool SuppressDocumentationCommentParse => (int)_options.DocumentationMode < 1;

        public CSharpParseOptions Options => _options;

        public DirectiveStack Directives => _directives;

        public bool InterpolationFollowedByColon => _interpolationFollowedByColon;

        private bool InDocumentationComment
        {
            get
            {
                switch (ModeOf(_mode))
                {
                    case LexerMode.XmlDocComment:
                    case LexerMode.XmlElementTag:
                    case LexerMode.XmlAttributeTextQuote:
                    case LexerMode.XmlAttributeTextDoubleQuote:
                    case LexerMode.XmlCrefQuote:
                    case LexerMode.XmlCrefDoubleQuote:
                    case LexerMode.XmlNameQuote:
                    case LexerMode.XmlNameDoubleQuote:
                    case LexerMode.XmlCDataSectionText:
                    case LexerMode.XmlCommentText:
                    case LexerMode.XmlProcessingInstructionText:
                    case LexerMode.XmlCharacter:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool InXmlCrefOrNameAttributeValue
        {
            get
            {
                switch (_mode & LexerMode.MaskLexMode)
                {
                    case LexerMode.XmlCrefQuote:
                    case LexerMode.XmlCrefDoubleQuote:
                    case LexerMode.XmlNameQuote:
                    case LexerMode.XmlNameDoubleQuote:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private bool InXmlNameAttributeValue
        {
            get
            {
                LexerMode lexerMode = _mode & LexerMode.MaskLexMode;
                if (lexerMode == LexerMode.XmlNameQuote || lexerMode == LexerMode.XmlNameDoubleQuote)
                {
                    return true;
                }
                return false;
            }
        }

        public Lexer(SourceText text, CSharpParseOptions options, bool allowPreprocessorDirectives = true, bool interpolationFollowedByColon = false)
            : base(text)
        {
            _options = options;
            _builder = new StringBuilder();
            _identBuffer = new char[32];
            _cache = new LexerCache();
            _createQuickTokenFunction = CreateQuickToken;
            _allowPreprocessorDirectives = allowPreprocessorDirectives;
            _interpolationFollowedByColon = interpolationFollowedByColon;
        }

        public override void Dispose()
        {
            _cache.Free();
            if (_xmlParser != null)
            {
                _xmlParser.Dispose();
            }
            base.Dispose();
        }

        public void Reset(int position, DirectiveStack directives)
        {
            TextWindow.Reset(position);
            _directives = directives;
        }

        private static LexerMode ModeOf(LexerMode mode)
        {
            return mode & LexerMode.MaskLexMode;
        }

        private bool ModeIs(LexerMode mode)
        {
            return ModeOf(_mode) == mode;
        }

        private static XmlDocCommentLocation LocationOf(LexerMode mode)
        {
            return (XmlDocCommentLocation)((int)(mode & LexerMode.MaskXmlDocCommentLocation) >> 16);
        }

        private bool LocationIs(XmlDocCommentLocation location)
        {
            return LocationOf(_mode) == location;
        }

        private void MutateLocation(XmlDocCommentLocation location)
        {
            _mode &= ~LexerMode.MaskXmlDocCommentLocation;
            _mode |= (LexerMode)((int)location << 16);
        }

        private static XmlDocCommentStyle StyleOf(LexerMode mode)
        {
            return (XmlDocCommentStyle)((int)(mode & LexerMode.MaskXmlDocCommentStyle) >> 20);
        }

        private bool StyleIs(XmlDocCommentStyle style)
        {
            return StyleOf(_mode) == style;
        }

        public SyntaxToken Lex(ref LexerMode mode)
        {
            SyntaxToken result = Lex(mode);
            mode = _mode;
            return result;
        }

        public SyntaxToken Lex(LexerMode mode)
        {
            _mode = mode;
            switch (_mode)
            {
                case LexerMode.Syntax:
                case LexerMode.DebuggerSyntax:
                    return QuickScanSyntaxToken() ?? LexSyntaxToken();
                case LexerMode.Directive:
                    return LexDirectiveToken();
                default:
                    switch (ModeOf(_mode))
                    {
                        case LexerMode.XmlDocComment:
                            return LexXmlToken();
                        case LexerMode.XmlElementTag:
                            return LexXmlElementTagToken();
                        case LexerMode.XmlAttributeTextQuote:
                        case LexerMode.XmlAttributeTextDoubleQuote:
                            return LexXmlAttributeTextToken();
                        case LexerMode.XmlCDataSectionText:
                            return LexXmlCDataSectionTextToken();
                        case LexerMode.XmlCommentText:
                            return LexXmlCommentTextToken();
                        case LexerMode.XmlProcessingInstructionText:
                            return LexXmlProcessingInstructionTextToken();
                        case LexerMode.XmlCrefQuote:
                        case LexerMode.XmlCrefDoubleQuote:
                            return LexXmlCrefOrNameToken();
                        case LexerMode.XmlNameQuote:
                        case LexerMode.XmlNameDoubleQuote:
                            return LexXmlCrefOrNameToken();
                        case LexerMode.XmlCharacter:
                            return LexXmlCharacter();
                        default:
                            throw ExceptionUtilities.UnexpectedValue(ModeOf(_mode));
                    }
            }
        }

        private static int GetFullWidth(SyntaxListBuilder builder)
        {
            int num = 0;
            if (builder != null)
            {
                for (int i = 0; i < builder.Count; i++)
                {
                    num += builder[i]!.FullWidth;
                }
            }
            return num;
        }

        private SyntaxToken LexSyntaxToken()
        {
            _leadingTriviaCache.Clear();
            LexSyntaxTrivia(TextWindow.Position > 0, isTrailing: false, ref _leadingTriviaCache);
            SyntaxListBuilder leadingTriviaCache = _leadingTriviaCache;
            TokenInfo info = default(TokenInfo);
            Start();
            ScanSyntaxToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(leadingTriviaCache));
            _trailingTriviaCache.Clear();
            LexSyntaxTrivia(afterFirstToken: true, isTrailing: true, ref _trailingTriviaCache);
            SyntaxListBuilder trailingTriviaCache = _trailingTriviaCache;
            return Create(ref info, leadingTriviaCache, trailingTriviaCache, errors);
        }

        internal SyntaxTriviaList LexSyntaxLeadingTrivia()
        {
            _leadingTriviaCache.Clear();
            LexSyntaxTrivia(TextWindow.Position > 0, isTrailing: false, ref _leadingTriviaCache);
            Microsoft.CodeAnalysis.SyntaxToken token = default(Microsoft.CodeAnalysis.SyntaxToken);
            return new SyntaxTriviaList(in token, _leadingTriviaCache.ToListNode(), 0);
        }

        internal SyntaxTriviaList LexSyntaxTrailingTrivia()
        {
            _trailingTriviaCache.Clear();
            LexSyntaxTrivia(afterFirstToken: true, isTrailing: true, ref _trailingTriviaCache);
            Microsoft.CodeAnalysis.SyntaxToken token = default(Microsoft.CodeAnalysis.SyntaxToken);
            return new SyntaxTriviaList(in token, _trailingTriviaCache.ToListNode(), 0);
        }

        private SyntaxToken Create(ref TokenInfo info, SyntaxListBuilder leading, SyntaxListBuilder trailing, SyntaxDiagnosticInfo[] errors)
        {
            GreenNode leading2 = leading?.ToListNode();
            GreenNode trailing2 = trailing?.ToListNode();
            SyntaxToken syntaxToken;
            if (info.RequiresTextForXmlEntity)
            {
                syntaxToken = SyntaxFactory.Token(leading2, info.Kind, info.Text, info.StringValue, trailing2);
            }
            else
            {
                switch (info.Kind)
                {
                    case SyntaxKind.IdentifierToken:
                        syntaxToken = SyntaxFactory.Identifier(info.ContextualKind, leading2, info.Text, info.StringValue, trailing2);
                        break;
                    case SyntaxKind.NumericLiteralToken:
                        syntaxToken = info.ValueKind switch
                        {
                            SpecialType.System_Int32 => SyntaxFactory.Literal(leading2, info.Text, info.IntValue, trailing2),
                            SpecialType.System_UInt32 => SyntaxFactory.Literal(leading2, info.Text, info.UintValue, trailing2),
                            SpecialType.System_Int64 => SyntaxFactory.Literal(leading2, info.Text, info.LongValue, trailing2),
                            SpecialType.System_UInt64 => SyntaxFactory.Literal(leading2, info.Text, info.UlongValue, trailing2),
                            SpecialType.System_Single => SyntaxFactory.Literal(leading2, info.Text, info.FloatValue, trailing2),
                            SpecialType.System_Double => SyntaxFactory.Literal(leading2, info.Text, info.DoubleValue, trailing2),
                            SpecialType.System_Decimal => SyntaxFactory.Literal(leading2, info.Text, info.DecimalValue, trailing2),
                            _ => throw ExceptionUtilities.UnexpectedValue(info.ValueKind),
                        };
                        break;
                    case SyntaxKind.InterpolatedStringToken:
                        syntaxToken = SyntaxFactory.Literal(leading2, info.Text, info.Kind, info.Text, trailing2);
                        break;
                    case SyntaxKind.StringLiteralToken:
                        syntaxToken = SyntaxFactory.Literal(leading2, info.Text, info.Kind, info.StringValue, trailing2);
                        break;
                    case SyntaxKind.CharacterLiteralToken:
                        syntaxToken = SyntaxFactory.Literal(leading2, info.Text, info.CharValue, trailing2);
                        break;
                    case SyntaxKind.XmlTextLiteralNewLineToken:
                        syntaxToken = SyntaxFactory.XmlTextNewLine(leading2, info.Text, info.StringValue, trailing2);
                        break;
                    case SyntaxKind.XmlTextLiteralToken:
                        syntaxToken = SyntaxFactory.XmlTextLiteral(leading2, info.Text, info.StringValue, trailing2);
                        break;
                    case SyntaxKind.XmlEntityLiteralToken:
                        syntaxToken = SyntaxFactory.XmlEntity(leading2, info.Text, info.StringValue, trailing2);
                        break;
                    case SyntaxKind.EndOfDocumentationCommentToken:
                    case SyntaxKind.EndOfFileToken:
                        syntaxToken = SyntaxFactory.Token(leading2, info.Kind, trailing2);
                        break;
                    case SyntaxKind.None:
                        syntaxToken = SyntaxFactory.BadToken(leading2, info.Text, trailing2);
                        break;
                    default:
                        syntaxToken = SyntaxFactory.Token(leading2, info.Kind, trailing2);
                        break;
                }
            }
            if (errors != null && ((int)_options.DocumentationMode >= 2 || !InDocumentationComment))
            {
                syntaxToken = syntaxToken.WithDiagnosticsGreen(errors);
            }
            return syntaxToken;
        }

        private void ScanSyntaxToken(ref TokenInfo info)
        {
            info.Kind = SyntaxKind.None;
            info.ContextualKind = SyntaxKind.None;
            info.Text = null;
            char surrogateCharacter = '\uffff';
            bool flag = false;
            int position = TextWindow.Position;
            char c = TextWindow.PeekChar();
            switch (c)
            {
                case '"':
                case '\'':
                    ScanStringLiteral(ref info);
                    break;
                case '/':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.SlashEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.SlashToken;
                    }
                    break;
                case '.':
                    if (ScanNumericLiteral(ref info))
                    {
                        break;
                    }
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '.')
                    {
                        TextWindow.AdvanceChar();
                        if (TextWindow.PeekChar() == '.')
                        {
                            AddError(ErrorCode.ERR_TripleDotNotAllowed);
                        }
                        info.Kind = SyntaxKind.DotDotToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.DotToken;
                    }
                    break;
                case ',':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CommaToken;
                    break;
                case ':':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == ':')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.ColonColonToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.ColonToken;
                    }
                    break;
                case ';':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.SemicolonToken;
                    break;
                case '~':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.TildeToken;
                    break;
                case '!':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.ExclamationEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.ExclamationToken;
                    }
                    break;
                case '=':
                    TextWindow.AdvanceChar();
                    if ((c = TextWindow.PeekChar()) == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.EqualsEqualsToken;
                    }
                    else if (c == '>')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.EqualsGreaterThanToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.EqualsToken;
                    }
                    break;
                case '*':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.AsteriskEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.AsteriskToken;
                    }
                    break;
                case '(':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.OpenParenToken;
                    break;
                case ')':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CloseParenToken;
                    break;
                case '{':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.OpenBraceToken;
                    break;
                case '}':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CloseBraceToken;
                    break;
                case '[':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.OpenBracketToken;
                    break;
                case ']':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CloseBracketToken;
                    break;
                case '?':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '?')
                    {
                        TextWindow.AdvanceChar();
                        if (TextWindow.PeekChar() == '=')
                        {
                            TextWindow.AdvanceChar();
                            info.Kind = SyntaxKind.QuestionQuestionEqualsToken;
                        }
                        else
                        {
                            info.Kind = SyntaxKind.QuestionQuestionToken;
                        }
                    }
                    else
                    {
                        info.Kind = SyntaxKind.QuestionToken;
                    }
                    break;
                case '+':
                    TextWindow.AdvanceChar();
                    if ((c = TextWindow.PeekChar()) == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.PlusEqualsToken;
                    }
                    else if (c == '+')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.PlusPlusToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.PlusToken;
                    }
                    break;
                case '-':
                    TextWindow.AdvanceChar();
                    if ((c = TextWindow.PeekChar()) == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.MinusEqualsToken;
                        break;
                    }
                    switch (c)
                    {
                        case '-':
                            TextWindow.AdvanceChar();
                            info.Kind = SyntaxKind.MinusMinusToken;
                            break;
                        case '>':
                            TextWindow.AdvanceChar();
                            info.Kind = SyntaxKind.MinusGreaterThanToken;
                            break;
                        default:
                            info.Kind = SyntaxKind.MinusToken;
                            break;
                    }
                    break;
                case '%':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.PercentEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.PercentToken;
                    }
                    break;
                case '&':
                    TextWindow.AdvanceChar();
                    if ((c = TextWindow.PeekChar()) == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.AmpersandEqualsToken;
                    }
                    else if (TextWindow.PeekChar() == '&')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.AmpersandAmpersandToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.AmpersandToken;
                    }
                    break;
                case '^':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.CaretEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.CaretToken;
                    }
                    break;
                case '|':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.BarEqualsToken;
                    }
                    else if (TextWindow.PeekChar() == '|')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.BarBarToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.BarToken;
                    }
                    break;
                case '<':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.LessThanEqualsToken;
                    }
                    else if (TextWindow.PeekChar() == '<')
                    {
                        TextWindow.AdvanceChar();
                        if (TextWindow.PeekChar() == '=')
                        {
                            TextWindow.AdvanceChar();
                            info.Kind = SyntaxKind.LessThanLessThanEqualsToken;
                        }
                        else
                        {
                            info.Kind = SyntaxKind.LessThanLessThanToken;
                        }
                    }
                    else
                    {
                        info.Kind = SyntaxKind.LessThanToken;
                    }
                    break;
                case '>':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.GreaterThanEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.GreaterThanToken;
                    }
                    break;
                case '@':
                    if (TextWindow.PeekChar(1) == '"')
                    {
                        ScanVerbatimStringLiteral(ref info);
                    }
                    else if (TextWindow.PeekChar(1) == '$' && TextWindow.PeekChar(2) == '"')
                    {
                        ScanInterpolatedStringLiteral(isVerbatim: true, ref info);
                        CheckFeatureAvailability(MessageID.IDS_FeatureAltInterpolatedVerbatimStrings);
                    }
                    else if (!ScanIdentifierOrKeyword(ref info))
                    {
                        TextWindow.AdvanceChar();
                        info.Text = TextWindow.GetText(intern: true);
                        AddError(ErrorCode.ERR_ExpectedVerbatimLiteral);
                    }
                    break;
                case '$':
                    if (TextWindow.PeekChar(1) == '"')
                    {
                        ScanInterpolatedStringLiteral(isVerbatim: false, ref info);
                        CheckFeatureAvailability(MessageID.IDS_FeatureInterpolatedStrings);
                        break;
                    }
                    if (TextWindow.PeekChar(1) == '@' && TextWindow.PeekChar(2) == '"')
                    {
                        ScanInterpolatedStringLiteral(isVerbatim: true, ref info);
                        CheckFeatureAvailability(MessageID.IDS_FeatureInterpolatedStrings);
                        break;
                    }
                    if (ModeIs(LexerMode.DebuggerSyntax))
                    {
                        goto case 'A';
                    }
                    goto default;
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    ScanIdentifierOrKeyword(ref info);
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ScanNumericLiteral(ref info);
                    break;
                case '\\':
                    c = TextWindow.PeekCharOrUnicodeEscape(out surrogateCharacter);
                    flag = true;
                    if (SyntaxFacts.IsIdentifierStartCharacter(c))
                    {
                        goto case 'A';
                    }
                    goto default;
                case '\uffff':
                    if (TextWindow.IsReallyAtEnd())
                    {
                        if (_directives.HasUnfinishedIf())
                        {
                            AddError(ErrorCode.ERR_EndifDirectiveExpected);
                        }
                        if (_directives.HasUnfinishedRegion())
                        {
                            AddError(ErrorCode.ERR_EndRegionDirectiveExpected);
                        }
                        info.Kind = SyntaxKind.EndOfFileToken;
                        break;
                    }
                    goto default;
                default:
                    if (!SyntaxFacts.IsIdentifierStartCharacter(c))
                    {
                        if (flag)
                        {
                            TextWindow.NextCharOrUnicodeEscape(out surrogateCharacter, out var info2);
                            AddError(info2);
                        }
                        else
                        {
                            TextWindow.AdvanceChar();
                        }
                        if (_badTokenCount++ > 200)
                        {
                            int length = TextWindow.Text.Length;
                            int length2 = length - position;
                            info.Text = TextWindow.Text.ToString(new TextSpan(position, length2));
                            TextWindow.Reset(length);
                        }
                        else
                        {
                            info.Text = TextWindow.GetText(intern: true);
                        }
                        AddError(ErrorCode.ERR_UnexpectedCharacter, info.Text);
                        break;
                    }
                    goto case 'A';
            }
        }

        private void CheckFeatureAvailability(MessageID feature)
        {
            CSDiagnosticInfo featureAvailabilityDiagnosticInfo = feature.GetFeatureAvailabilityDiagnosticInfo(Options);
            if (featureAvailabilityDiagnosticInfo != null)
            {
                AddError(featureAvailabilityDiagnosticInfo.Code, featureAvailabilityDiagnosticInfo.Arguments);
            }
        }

        private bool ScanInteger()
        {
            int position = TextWindow.Position;
            char c;
            while ((c = TextWindow.PeekChar()) >= '0' && c <= '9')
            {
                TextWindow.AdvanceChar();
            }
            return position < TextWindow.Position;
        }

        private void ScanNumericLiteralSingleInteger(ref bool underscoreInWrongPlace, ref bool usedUnderscore, ref bool firstCharWasUnderscore, bool isHex, bool isBinary)
        {
            if (TextWindow.PeekChar() == '_')
            {
                if (isHex || isBinary)
                {
                    firstCharWasUnderscore = true;
                }
                else
                {
                    underscoreInWrongPlace = true;
                }
            }
            bool flag = false;
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c == '_')
                {
                    usedUnderscore = true;
                    flag = true;
                }
                else
                {
                    if (!(isHex ? SyntaxFacts.IsHexDigit(c) : (isBinary ? SyntaxFacts.IsBinaryDigit(c) : SyntaxFacts.IsDecDigit(c))))
                    {
                        break;
                    }
                    _builder.Append(c);
                    flag = false;
                }
                TextWindow.AdvanceChar();
            }
            if (flag)
            {
                underscoreInWrongPlace = true;
            }
        }

        private bool ScanNumericLiteral(ref TokenInfo info)
        {
            int position = TextWindow.Position;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            info.Text = null;
            info.ValueKind = SpecialType.None;
            _builder.Clear();
            bool flag5 = false;
            bool flag6 = false;
            bool underscoreInWrongPlace = false;
            bool usedUnderscore = false;
            bool firstCharWasUnderscore = false;
            char c = TextWindow.PeekChar();
            if (c == '0')
            {
                switch (TextWindow.PeekChar(1))
                {
                    case 'X':
                    case 'x':
                        TextWindow.AdvanceChar(2);
                        flag = true;
                        break;
                    case 'B':
                    case 'b':
                        CheckFeatureAvailability(MessageID.IDS_FeatureBinaryLiteral);
                        TextWindow.AdvanceChar(2);
                        flag2 = true;
                        break;
                }
            }
            if (flag || flag2)
            {
                ScanNumericLiteralSingleInteger(ref underscoreInWrongPlace, ref usedUnderscore, ref firstCharWasUnderscore, flag, flag2);
                if ((c = TextWindow.PeekChar()) == 'L' || c == 'l')
                {
                    if (c == 'l')
                    {
                        AddError(TextWindow.Position, 1, ErrorCode.WRN_LowercaseEllSuffix);
                    }
                    TextWindow.AdvanceChar();
                    flag6 = true;
                    if ((c = TextWindow.PeekChar()) == 'u' || c == 'U')
                    {
                        TextWindow.AdvanceChar();
                        flag5 = true;
                    }
                }
                else if ((c = TextWindow.PeekChar()) == 'u' || c == 'U')
                {
                    TextWindow.AdvanceChar();
                    flag5 = true;
                    if ((c = TextWindow.PeekChar()) == 'L' || c == 'l')
                    {
                        TextWindow.AdvanceChar();
                        flag6 = true;
                    }
                }
            }
            else
            {
                ScanNumericLiteralSingleInteger(ref underscoreInWrongPlace, ref usedUnderscore, ref firstCharWasUnderscore, isHex: false, isBinary: false);
                if (ModeIs(LexerMode.DebuggerSyntax) && TextWindow.PeekChar() == '#')
                {
                    TextWindow.AdvanceChar();
                    info.StringValue = (info.Text = TextWindow.GetText(intern: true));
                    info.Kind = SyntaxKind.IdentifierToken;
                    AddError(AbstractLexer.MakeError(ErrorCode.ERR_LegacyObjectIdSyntax));
                    return true;
                }
                if ((c = TextWindow.PeekChar()) == '.')
                {
                    char c2 = TextWindow.PeekChar(1);
                    if (c2 >= '0' && c2 <= '9')
                    {
                        flag3 = true;
                        _builder.Append(c);
                        TextWindow.AdvanceChar();
                        ScanNumericLiteralSingleInteger(ref underscoreInWrongPlace, ref usedUnderscore, ref firstCharWasUnderscore, isHex: false, isBinary: false);
                    }
                    else if (_builder.Length == 0)
                    {
                        TextWindow.Reset(position);
                        return false;
                    }
                }
                if ((c = TextWindow.PeekChar()) == 'E' || c == 'e')
                {
                    _builder.Append(c);
                    TextWindow.AdvanceChar();
                    flag4 = true;
                    if ((c = TextWindow.PeekChar()) == '-' || c == '+')
                    {
                        _builder.Append(c);
                        TextWindow.AdvanceChar();
                    }
                    if (((c = TextWindow.PeekChar()) < '0' || c > '9') && c != '_')
                    {
                        AddError(AbstractLexer.MakeError(ErrorCode.ERR_InvalidReal));
                        _builder.Append('0');
                    }
                    else
                    {
                        ScanNumericLiteralSingleInteger(ref underscoreInWrongPlace, ref usedUnderscore, ref firstCharWasUnderscore, isHex: false, isBinary: false);
                    }
                }
                if (flag4 || flag3)
                {
                    if ((c = TextWindow.PeekChar()) == 'f' || c == 'F')
                    {
                        TextWindow.AdvanceChar();
                        info.ValueKind = SpecialType.System_Single;
                    }
                    else
                    {
                        switch (c)
                        {
                            case 'D':
                            case 'd':
                                TextWindow.AdvanceChar();
                                info.ValueKind = SpecialType.System_Double;
                                break;
                            case 'M':
                            case 'm':
                                TextWindow.AdvanceChar();
                                info.ValueKind = SpecialType.System_Decimal;
                                break;
                            default:
                                info.ValueKind = SpecialType.System_Double;
                                break;
                        }
                    }
                }
                else if ((c = TextWindow.PeekChar()) == 'f' || c == 'F')
                {
                    TextWindow.AdvanceChar();
                    info.ValueKind = SpecialType.System_Single;
                }
                else
                {
                    switch (c)
                    {
                        case 'D':
                        case 'd':
                            TextWindow.AdvanceChar();
                            info.ValueKind = SpecialType.System_Double;
                            break;
                        case 'M':
                        case 'm':
                            TextWindow.AdvanceChar();
                            info.ValueKind = SpecialType.System_Decimal;
                            break;
                        case 'L':
                        case 'l':
                            if (c == 'l')
                            {
                                AddError(TextWindow.Position, 1, ErrorCode.WRN_LowercaseEllSuffix);
                            }
                            TextWindow.AdvanceChar();
                            flag6 = true;
                            if ((c = TextWindow.PeekChar()) == 'u' || c == 'U')
                            {
                                TextWindow.AdvanceChar();
                                flag5 = true;
                            }
                            break;
                        case 'U':
                        case 'u':
                            flag5 = true;
                            TextWindow.AdvanceChar();
                            if ((c = TextWindow.PeekChar()) == 'L' || c == 'l')
                            {
                                TextWindow.AdvanceChar();
                                flag6 = true;
                            }
                            break;
                    }
                }
            }
            if (underscoreInWrongPlace)
            {
                AddError(MakeError(position, TextWindow.Position - position, ErrorCode.ERR_InvalidNumber));
            }
            else if (firstCharWasUnderscore)
            {
                CheckFeatureAvailability(MessageID.IDS_FeatureLeadingDigitSeparator);
            }
            else if (usedUnderscore)
            {
                CheckFeatureAvailability(MessageID.IDS_FeatureDigitSeparator);
            }
            info.Kind = SyntaxKind.NumericLiteralToken;
            info.Text = TextWindow.GetText(intern: true);
            string text = TextWindow.Intern(_builder);
            switch (info.ValueKind)
            {
                case SpecialType.System_Single:
                    info.FloatValue = GetValueSingle(text);
                    break;
                case SpecialType.System_Double:
                    info.DoubleValue = GetValueDouble(text);
                    break;
                case SpecialType.System_Decimal:
                    info.DecimalValue = GetValueDecimal(text, position, TextWindow.Position);
                    break;
                default:
                    {
                        ulong num;
                        if (string.IsNullOrEmpty(text))
                        {
                            if (!underscoreInWrongPlace)
                            {
                                AddError(AbstractLexer.MakeError(ErrorCode.ERR_InvalidNumber));
                            }
                            num = 0uL;
                        }
                        else
                        {
                            num = GetValueUInt64(text, flag, flag2);
                        }
                        if (!flag5 && !flag6)
                        {
                            if (num <= int.MaxValue)
                            {
                                info.ValueKind = SpecialType.System_Int32;
                                info.IntValue = (int)num;
                            }
                            else if (num <= uint.MaxValue)
                            {
                                info.ValueKind = SpecialType.System_UInt32;
                                info.UintValue = (uint)num;
                            }
                            else if (num <= long.MaxValue)
                            {
                                info.ValueKind = SpecialType.System_Int64;
                                info.LongValue = (long)num;
                            }
                            else
                            {
                                info.ValueKind = SpecialType.System_UInt64;
                                info.UlongValue = num;
                            }
                        }
                        else if (flag5 && !flag6)
                        {
                            if (num <= uint.MaxValue)
                            {
                                info.ValueKind = SpecialType.System_UInt32;
                                info.UintValue = (uint)num;
                            }
                            else
                            {
                                info.ValueKind = SpecialType.System_UInt64;
                                info.UlongValue = num;
                            }
                        }
                        else if (!flag5 && flag6)
                        {
                            if (num <= long.MaxValue)
                            {
                                info.ValueKind = SpecialType.System_Int64;
                                info.LongValue = (long)num;
                            }
                            else
                            {
                                info.ValueKind = SpecialType.System_UInt64;
                                info.UlongValue = num;
                            }
                        }
                        else
                        {
                            info.ValueKind = SpecialType.System_UInt64;
                            info.UlongValue = num;
                        }
                        break;
                    }
            }
            return true;
        }

        private static bool TryParseBinaryUInt64(string text, out ulong value)
        {
            value = 0uL;
            foreach (char c in text)
            {
                if ((value & 0x8000000000000000uL) != 0L)
                {
                    return false;
                }
                ulong num = (ulong)SyntaxFacts.BinaryValue(c);
                value = (value << 1) | num;
            }
            return true;
        }

        private int GetValueInt32(string text, bool isHex)
        {
            if (!int.TryParse(text, isHex ? NumberStyles.AllowHexSpecifier : NumberStyles.None, CultureInfo.InvariantCulture, out var result))
            {
                AddError(AbstractLexer.MakeError(ErrorCode.ERR_IntOverflow));
            }
            return result;
        }

        private ulong GetValueUInt64(string text, bool isHex, bool isBinary)
        {
            ulong value;
            if (isBinary)
            {
                if (!TryParseBinaryUInt64(text, out value))
                {
                    AddError(AbstractLexer.MakeError(ErrorCode.ERR_IntOverflow));
                }
            }
            else if (!ulong.TryParse(text, isHex ? NumberStyles.AllowHexSpecifier : NumberStyles.None, CultureInfo.InvariantCulture, out value))
            {
                AddError(AbstractLexer.MakeError(ErrorCode.ERR_IntOverflow));
            }
            return value;
        }

        private double GetValueDouble(string text)
        {
            if (!RealParser.TryParseDouble(text, out var d))
            {
                AddError(AbstractLexer.MakeError(ErrorCode.ERR_FloatOverflow, "double"));
            }
            return d;
        }

        private float GetValueSingle(string text)
        {
            if (!RealParser.TryParseFloat(text, out var f))
            {
                AddError(AbstractLexer.MakeError(ErrorCode.ERR_FloatOverflow, "float"));
            }
            return f;
        }

        private decimal GetValueDecimal(string text, int start, int end)
        {
            if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var result))
            {
                AddError(MakeError(start, end - start, ErrorCode.ERR_FloatOverflow, "decimal"));
            }
            return result;
        }

        private void ResetIdentBuffer()
        {
            _identLen = 0;
        }

        private void AddIdentChar(char ch)
        {
            if (_identLen >= _identBuffer.Length)
            {
                GrowIdentBuffer();
            }
            _identBuffer[_identLen++] = ch;
        }

        private void GrowIdentBuffer()
        {
            char[] array = new char[_identBuffer.Length * 2];
            Array.Copy(_identBuffer, array, _identBuffer.Length);
            _identBuffer = array;
        }

        private bool ScanIdentifier(ref TokenInfo info)
        {
            if (!ScanIdentifier_FastPath(ref info))
            {
                if (!InXmlCrefOrNameAttributeValue)
                {
                    return ScanIdentifier_SlowPath(ref info);
                }
                return ScanIdentifier_CrefSlowPath(ref info);
            }
            return true;
        }

        private bool ScanIdentifier_FastPath(ref TokenInfo info)
        {
            if ((_mode & LexerMode.MaskLexMode) == LexerMode.DebuggerSyntax)
            {
                return false;
            }
            int i = TextWindow.Offset;
            char[] characterWindow = TextWindow.CharacterWindow;
            int characterWindowCount = TextWindow.CharacterWindowCount;
            int num = i;
            for (; i != characterWindowCount; i++)
            {
                switch (characterWindow[i])
                {
                    case '&':
                        if (InXmlCrefOrNameAttributeValue)
                        {
                            return false;
                        }
                        goto case '\0';
                    case '\0':
                    case '\t':
                    case '\n':
                    case '\r':
                    case ' ':
                    case '!':
                    case '"':
                    case '%':
                    case '\'':
                    case '(':
                    case ')':
                    case '*':
                    case '+':
                    case ',':
                    case '-':
                    case '.':
                    case '/':
                    case ':':
                    case ';':
                    case '<':
                    case '=':
                    case '>':
                    case '?':
                    case '[':
                    case ']':
                    case '^':
                    case '{':
                    case '|':
                    case '}':
                    case '~':
                        {
                            int num2 = i - num;
                            TextWindow.AdvanceChar(num2);
                            info.Text = (info.StringValue = TextWindow.Intern(characterWindow, num, num2));
                            info.IsVerbatim = false;
                            return true;
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (i == num)
                        {
                            return false;
                        }
                        break;
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case '_':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }

        private bool ScanIdentifier_SlowPath(ref TokenInfo info)
        {
            int start = TextWindow.Position;
            this.ResetIdentBuffer();

            info.IsVerbatim = TextWindow.PeekChar() == '@';
            if (info.IsVerbatim)
            {
                TextWindow.AdvanceChar();
            }

            bool isObjectAddress = false;

            while (true)
            {
                char surrogateCharacter = SlidingTextWindow.InvalidCharacter;
                bool isEscaped = false;
                char ch = TextWindow.PeekChar();
            top:
                switch (ch)
                {
                    case '\\':
                        if (!isEscaped && TextWindow.IsUnicodeEscape())
                        {
                            // ^^^^^^^ otherwise \u005Cu1234 looks just like \u1234! (i.e. escape within escape)
                            info.HasIdentifierEscapeSequence = true;
                            isEscaped = true;
                            ch = TextWindow.PeekUnicodeEscape(out surrogateCharacter);
                            goto top;
                        }

                        goto default;
                    case '$':
                        if (!this.ModeIs(LexerMode.DebuggerSyntax) || _identLen > 0)
                        {
                            goto LoopExit;
                        }

                        break;
                    case SlidingTextWindow.InvalidCharacter:
                        if (!TextWindow.IsReallyAtEnd())
                        {
                            goto default;
                        }

                        goto LoopExit;
                    case '_':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        {
                            // Again, these are the 'common' identifier characters...
                            break;
                        }

                    case '0':
                        {
                            if (_identLen == 0)
                            {
                                // Debugger syntax allows @0x[hexdigit]+ for object address identifiers.
                                if (info.IsVerbatim &&
                                    this.ModeIs(LexerMode.DebuggerSyntax) &&
                                    (char.ToLower(TextWindow.PeekChar(1)) == 'x'))
                                {
                                    isObjectAddress = true;
                                }
                                else
                                {
                                    goto LoopExit;
                                }
                            }

                            // Again, these are the 'common' identifier characters...
                            break;
                        }
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            if (_identLen == 0)
                            {
                                goto LoopExit;
                            }

                            // Again, these are the 'common' identifier characters...
                            break;
                        }

                    case ' ':
                    case '\t':
                    case '.':
                    case ';':
                    case '(':
                    case ')':
                    case ',':
                        // ...and these are the 'common' stop characters.
                        goto LoopExit;
                    case '<':
                        if (_identLen == 0 && this.ModeIs(LexerMode.DebuggerSyntax) && TextWindow.PeekChar(1) == '>')
                        {
                            // In DebuggerSyntax mode, identifiers are allowed to begin with <>.
                            TextWindow.AdvanceChar(2);
                            this.AddIdentChar('<');
                            this.AddIdentChar('>');
                            continue;
                        }

                        goto LoopExit;
                    default:
                        {
                            // This is the 'expensive' call
                            if (_identLen == 0 && ch > 127 && SyntaxFacts.IsIdentifierStartCharacter(ch))
                            {
                                break;
                            }
                            else if (_identLen > 0 && ch > 127 && SyntaxFacts.IsIdentifierPartCharacter(ch))
                            {
                                //// BUG 424819 : Handle identifier chars > 0xFFFF via surrogate pairs
                                if (UnicodeCharacterUtilities.IsFormattingChar(ch))
                                {
                                    if (isEscaped)
                                    {
                                        SyntaxDiagnosticInfo error;
                                        TextWindow.NextCharOrUnicodeEscape(out surrogateCharacter, out error);
                                        AddError(error);
                                    }
                                    else
                                    {
                                        TextWindow.AdvanceChar();
                                    }

                                    continue; // Ignore formatting characters
                                }

                                break;
                            }
                            else
                            {
                                // Not a valid identifier character, so bail.
                                goto LoopExit;
                            }
                        }
                }

                if (isEscaped)
                {
                    SyntaxDiagnosticInfo error;
                    TextWindow.NextCharOrUnicodeEscape(out surrogateCharacter, out error);
                    AddError(error);
                }
                else
                {
                    TextWindow.AdvanceChar();
                }

                this.AddIdentChar(ch);
                if (surrogateCharacter != SlidingTextWindow.InvalidCharacter)
                {
                    this.AddIdentChar(surrogateCharacter);
                }
            }

        LoopExit:
            var width = TextWindow.Width; // exact size of input characters
            if (_identLen > 0)
            {
                info.Text = TextWindow.GetInternedText();

                // id buffer is identical to width in input
                if (_identLen == width)
                {
                    info.StringValue = info.Text;
                }
                else
                {
                    info.StringValue = TextWindow.Intern(_identBuffer, 0, _identLen);
                }

                if (isObjectAddress)
                {
                    // @0x[hexdigit]+
                    const int objectAddressOffset = 2;
                    var valueText = TextWindow.Intern(_identBuffer, objectAddressOffset, _identLen - objectAddressOffset);
                    // Verify valid hex value.
                    if ((valueText.Length == 0) || !valueText.All(IsValidHexDigit))
                    {
                        goto Fail;
                    }
                    // Parse hex value to check for overflow.
                    this.GetValueUInt64(valueText, isHex: true, isBinary: false);
                }

                return true;
            }

        Fail:
            info.Text = null;
            info.StringValue = null;
            TextWindow.Reset(start);
            return false;
        }

        private static bool IsValidHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return true;
            }
            c = char.ToLower(c);
            if (c >= 'a')
            {
                return c <= 'f';
            }
            return false;
        }

        /// <summary>
        /// This method is essentially the same as ScanIdentifier_SlowPath,
        /// except that it can handle XML entities.  Since ScanIdentifier
        /// is hot code and since this method does extra work, it seem
        /// worthwhile to separate it from the common case.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool ScanIdentifier_CrefSlowPath(ref TokenInfo info)
        {
            int start = TextWindow.Position;
            this.ResetIdentBuffer();

            if (AdvanceIfMatches('@'))
            {
                // In xml name attribute values, the '@' is part of the value text of the identifier
                // (to match dev11).
                if (InXmlNameAttributeValue)
                {
                    AddIdentChar('@');
                }
                else
                {
                    info.IsVerbatim = true;
                }
            }

            while (true)
            {
                int beforeConsumed = TextWindow.Position;
                char consumedChar;
                char consumedSurrogate;

                if (TextWindow.PeekChar() == '&')
                {
                    if (!TextWindow.TryScanXmlEntity(out consumedChar, out consumedSurrogate))
                    {
                        // If it's not a valid entity, then it's not part of the identifier.
                        TextWindow.Reset(beforeConsumed);
                        goto LoopExit;
                    }
                }
                else
                {
                    consumedChar = TextWindow.NextChar();
                    consumedSurrogate = SlidingTextWindow.InvalidCharacter;
                }

                // NOTE: If the surrogate is non-zero, then consumedChar won't match
                // any of the cases below (UTF-16 guarantees that members of surrogate
                // pairs aren't separately valid).

                bool isEscaped = false;
            top:
                switch (consumedChar)
                {
                    case '\\':
                        // NOTE: For completeness, we should allow xml entities in unicode escape
                        // sequences (DevDiv #16321).  Since it is not currently a priority, we will
                        // try to make the interim behavior sensible: we will only attempt to scan
                        // a unicode escape if NONE of the characters are XML entities (including
                        // the backslash, which we have already consumed).
                        // When we're ready to implement this behavior, we can drop the position
                        // check and use AdvanceIfMatches instead of PeekChar.
                        if (!isEscaped && (TextWindow.Position == beforeConsumed + 1) &&
                            (TextWindow.PeekChar() == 'u' || TextWindow.PeekChar() == 'U'))
                        {
                            info.HasIdentifierEscapeSequence = true;

                            TextWindow.Reset(beforeConsumed);
                            // ^^^^^^^ otherwise \u005Cu1234 looks just like \u1234! (i.e. escape within escape)
                            isEscaped = true;
                            SyntaxDiagnosticInfo error;
                            consumedChar = TextWindow.NextUnicodeEscape(out consumedSurrogate, out error);
                            AddCrefError(error);
                            goto top;
                        }

                        goto default;

                    case '_':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        {
                            // Again, these are the 'common' identifier characters...
                            break;
                        }

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            if (_identLen == 0)
                            {
                                TextWindow.Reset(beforeConsumed);
                                goto LoopExit;
                            }

                            // Again, these are the 'common' identifier characters...
                            break;
                        }

                    case ' ':
                    case '$':
                    case '\t':
                    case '.':
                    case ';':
                    case '(':
                    case ')':
                    case ',':
                    case '<':
                        // ...and these are the 'common' stop characters.
                        TextWindow.Reset(beforeConsumed);
                        goto LoopExit;
                    case SlidingTextWindow.InvalidCharacter:
                        if (!TextWindow.IsReallyAtEnd())
                        {
                            goto default;
                        }

                        TextWindow.Reset(beforeConsumed);
                        goto LoopExit;
                    default:
                        {
                            // This is the 'expensive' call
                            if (_identLen == 0 && consumedChar > 127 && SyntaxFacts.IsIdentifierStartCharacter(consumedChar))
                            {
                                break;
                            }
                            else if (_identLen > 0 && consumedChar > 127 && SyntaxFacts.IsIdentifierPartCharacter(consumedChar))
                            {
                                //// BUG 424819 : Handle identifier chars > 0xFFFF via surrogate pairs
                                if (UnicodeCharacterUtilities.IsFormattingChar(consumedChar))
                                {
                                    continue; // Ignore formatting characters
                                }

                                break;
                            }
                            else
                            {
                                // Not a valid identifier character, so bail.
                                TextWindow.Reset(beforeConsumed);
                                goto LoopExit;
                            }
                        }
                }

                this.AddIdentChar(consumedChar);
                if (consumedSurrogate != SlidingTextWindow.InvalidCharacter)
                {
                    this.AddIdentChar(consumedSurrogate);
                }
            }

        LoopExit:
            if (_identLen > 0)
            {
                // NOTE: If we don't intern the string value, then we won't get a hit
                // in the keyword dictionary!  (It searches for a key using identity.)
                // The text does not have to be interned (and probably shouldn't be
                // if it contains entities (else-case).

                var width = TextWindow.Width; // exact size of input characters

                // id buffer is identical to width in input
                if (_identLen == width)
                {
                    info.StringValue = TextWindow.GetInternedText();
                    info.Text = info.StringValue;
                }
                else
                {
                    info.StringValue = TextWindow.Intern(_identBuffer, 0, _identLen);
                    info.Text = TextWindow.GetText(intern: false);
                }

                return true;
            }
            else
            {
                info.Text = null;
                info.StringValue = null;
                TextWindow.Reset(start);
                return false;
            }
        }

        private bool ScanIdentifierOrKeyword(ref TokenInfo info)
        {
            info.ContextualKind = SyntaxKind.None;
            if (ScanIdentifier(ref info))
            {
                if (!info.IsVerbatim && !info.HasIdentifierEscapeSequence)
                {
                    if (ModeIs(LexerMode.Directive))
                    {
                        SyntaxKind preprocessorKeywordKind = SyntaxFacts.GetPreprocessorKeywordKind(info.Text);
                        if (SyntaxFacts.IsPreprocessorContextualKeyword(preprocessorKeywordKind))
                        {
                            info.Kind = SyntaxKind.IdentifierToken;
                            info.ContextualKind = preprocessorKeywordKind;
                        }
                        else
                        {
                            info.Kind = preprocessorKeywordKind;
                        }
                    }
                    else if (!_cache.TryGetKeywordKind(info.Text, out info.Kind))
                    {
                        info.ContextualKind = (info.Kind = SyntaxKind.IdentifierToken);
                    }
                    else if (SyntaxFacts.IsContextualKeyword(info.Kind))
                    {
                        info.ContextualKind = info.Kind;
                        info.Kind = SyntaxKind.IdentifierToken;
                    }
                    if (info.Kind == SyntaxKind.None)
                    {
                        info.Kind = SyntaxKind.IdentifierToken;
                    }
                }
                else
                {
                    info.ContextualKind = (info.Kind = SyntaxKind.IdentifierToken);
                }
                return true;
            }
            info.Kind = SyntaxKind.None;
            return false;
        }

        private void LexSyntaxTrivia(bool afterFirstToken, bool isTrailing, ref SyntaxListBuilder triviaList)
        {
            bool flag = !isTrailing;
            while (true)
            {
                Start();
                char c = TextWindow.PeekChar();
                if (c == ' ')
                {
                    AddTrivia(ScanWhitespace(), ref triviaList);
                    continue;
                }
                if (c > '\u007f')
                {
                    if (SyntaxFacts.IsWhitespace(c))
                    {
                        c = ' ';
                    }
                    else if (SyntaxFacts.IsNewLine(c))
                    {
                        c = '\n';
                    }
                }
                switch (c)
                {
                    default:
                        return;
                    case '\t':
                    case '\v':
                    case '\f':
                    case '\u001a':
                    case ' ':
                        AddTrivia(ScanWhitespace(), ref triviaList);
                        break;
                    case '/':
                        {
                            if ((c = TextWindow.PeekChar(1)) == '/')
                            {
                                if (!SuppressDocumentationCommentParse && TextWindow.PeekChar(2) == '/' && TextWindow.PeekChar(3) != '/')
                                {
                                    if (isTrailing)
                                    {
                                        return;
                                    }
                                    AddTrivia(LexXmlDocComment(XmlDocCommentStyle.SingleLine), ref triviaList);
                                }
                                else
                                {
                                    ScanToEndOfLine();
                                    string text = TextWindow.GetText(intern: false);
                                    AddTrivia(SyntaxFactory.Comment(text), ref triviaList);
                                    flag = false;
                                }
                                break;
                            }
                            if (c != '*')
                            {
                                return;
                            }
                            if (!SuppressDocumentationCommentParse && TextWindow.PeekChar(2) == '*' && TextWindow.PeekChar(3) != '*' && TextWindow.PeekChar(3) != '/')
                            {
                                if (isTrailing)
                                {
                                    return;
                                }
                                AddTrivia(LexXmlDocComment(XmlDocCommentStyle.Delimited), ref triviaList);
                                break;
                            }
                            ScanMultiLineComment(out var isTerminated);
                            if (!isTerminated)
                            {
                                AddError(ErrorCode.ERR_OpenEndedComment);
                            }
                            string text2 = TextWindow.GetText(intern: false);
                            AddTrivia(SyntaxFactory.Comment(text2), ref triviaList);
                            flag = false;
                            break;
                        }
                    case '\n':
                    case '\r':
                        AddTrivia(ScanEndOfLine(), ref triviaList);
                        if (isTrailing)
                        {
                            return;
                        }
                        flag = true;
                        break;
                    case '#':
                        if (_allowPreprocessorDirectives)
                        {
                            LexDirectiveAndExcludedTrivia(afterFirstToken, isTrailing || !flag, ref triviaList);
                            break;
                        }
                        return;
                    case '<':
                    case '=':
                        if (!isTrailing && IsConflictMarkerTrivia())
                        {
                            LexConflictMarkerTrivia(ref triviaList);
                            break;
                        }
                        return;
                }
            }
        }

        private bool IsConflictMarkerTrivia()
        {
            int position = TextWindow.Position;
            SourceText text = TextWindow.Text;
            if (position == 0 || SyntaxFacts.IsNewLine(text[position - 1]))
            {
                char c = text[position];
                if (position + s_conflictMarkerLength <= text.Length)
                {
                    int i = 0;
                    for (int num = s_conflictMarkerLength; i < num; i++)
                    {
                        if (text[position + i] != c)
                        {
                            return false;
                        }
                    }
                    if (c == '=')
                    {
                        return true;
                    }
                    if (position + s_conflictMarkerLength < text.Length)
                    {
                        return text[position + s_conflictMarkerLength] == ' ';
                    }
                    return false;
                }
            }
            return false;
        }

        private void LexConflictMarkerTrivia(ref SyntaxListBuilder triviaList)
        {
            Start();
            AddError(TextWindow.Position, s_conflictMarkerLength, ErrorCode.ERR_Merge_conflict_marker_encountered);
            char num = TextWindow.PeekChar();
            LexConflictMarkerHeader(ref triviaList);
            LexConflictMarkerEndOfLine(ref triviaList);
            if (num == '=')
            {
                LexConflictMarkerDisabledText(ref triviaList);
            }
        }

        private SyntaxListBuilder LexConflictMarkerDisabledText(ref SyntaxListBuilder triviaList)
        {
            // Consume everything from the start of the mid-conflict marker to the start of the next
            // end-conflict marker.
            this.Start();

            var hitEndConflictMarker = false;
            while (true)
            {
                var ch = this.TextWindow.PeekChar();
                if (ch == SlidingTextWindow.InvalidCharacter)
                {
                    break;
                }

                // If we hit the end-conflict marker, then lex it out at this point.
                if (ch == '>' && IsConflictMarkerTrivia())
                {
                    hitEndConflictMarker = true;
                    break;
                }

                this.TextWindow.AdvanceChar();
            }

            if (this.TextWindow.Width > 0)
            {
                this.AddTrivia(SyntaxFactory.DisabledText(TextWindow.GetText(false)), ref triviaList);
            }

            if (hitEndConflictMarker)
            {
                LexConflictMarkerTrivia(ref triviaList);
            }

            return triviaList;
        }

        private void LexConflictMarkerEndOfLine(ref SyntaxListBuilder triviaList)
        {
            Start();
            while (SyntaxFacts.IsNewLine(TextWindow.PeekChar()))
            {
                TextWindow.AdvanceChar();
            }
            if (TextWindow.Width > 0)
            {
                AddTrivia(SyntaxFactory.EndOfLine(TextWindow.GetText(intern: false)), ref triviaList);
            }
        }

        private void LexConflictMarkerHeader(ref SyntaxListBuilder triviaList)
        {
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c == '\uffff' || SyntaxFacts.IsNewLine(c))
                {
                    break;
                }
                TextWindow.AdvanceChar();
            }
            AddTrivia(SyntaxFactory.ConflictMarker(TextWindow.GetText(intern: false)), ref triviaList);
        }

        private void AddTrivia(CSharpSyntaxNode trivia, ref SyntaxListBuilder list)
        {
            if (base.HasErrors)
            {
                CSharpSyntaxNode node = trivia;
                DiagnosticInfo[] errors = GetErrors(0);
                trivia = node.WithDiagnosticsGreen(errors);
            }
            if (list == null)
            {
                list = new SyntaxListBuilder(8);
            }
            list.Add(trivia);
        }

        private bool ScanMultiLineComment(out bool isTerminated)
        {
            if (TextWindow.PeekChar() == '/' && TextWindow.PeekChar(1) == '*')
            {
                TextWindow.AdvanceChar(2);
                while (true)
                {
                    char c;
                    if ((c = TextWindow.PeekChar()) == '\uffff' && TextWindow.IsReallyAtEnd())
                    {
                        isTerminated = false;
                        break;
                    }
                    if (c == '*' && TextWindow.PeekChar(1) == '/')
                    {
                        TextWindow.AdvanceChar(2);
                        isTerminated = true;
                        break;
                    }
                    TextWindow.AdvanceChar();
                }
                return true;
            }
            isTerminated = false;
            return false;
        }

        private void ScanToEndOfLine()
        {
            char c;
            while (!SyntaxFacts.IsNewLine(c = TextWindow.PeekChar()) && (c != '\uffff' || !TextWindow.IsReallyAtEnd()))
            {
                TextWindow.AdvanceChar();
            }
        }

        private CSharpSyntaxNode ScanEndOfLine()
        {
            char ch;
            switch (ch = TextWindow.PeekChar())
            {
                case '\r':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '\n')
                    {
                        TextWindow.AdvanceChar();
                        return SyntaxFactory.CarriageReturnLineFeed;
                    }
                    return SyntaxFactory.CarriageReturn;
                case '\n':
                    TextWindow.AdvanceChar();
                    return SyntaxFactory.LineFeed;
                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        TextWindow.AdvanceChar();
                        return SyntaxFactory.EndOfLine(ch.ToString());
                    }
                    return null;
            }
        }

        /// <summary>
        /// Scans all of the whitespace (not new-lines) into a trivia node until it runs out.
        /// </summary>
        /// <returns>A trivia node with the whitespace text</returns>
        private SyntaxTrivia ScanWhitespace()
        {
            if (_createWhitespaceTriviaFunction == null)
            {
                _createWhitespaceTriviaFunction = this.CreateWhitespaceTrivia;
            }

            int hashCode = Hash.FnvOffsetBias;  // FNV base
            bool onlySpaces = true;

        top:
            char ch = TextWindow.PeekChar();

            switch (ch)
            {
                case '\t':       // Horizontal tab
                case '\v':       // Vertical Tab
                case '\f':       // Form-feed
                case '\u001A':
                    onlySpaces = false;
                    goto case ' ';

                case ' ':
                    TextWindow.AdvanceChar();
                    hashCode = Hash.CombineFNVHash(hashCode, ch);
                    goto top;

                case '\r':      // Carriage Return
                case '\n':      // Line-feed
                    break;

                default:
                    if (ch > 127 && SyntaxFacts.IsWhitespace(ch))
                    {
                        goto case '\t';
                    }

                    break;
            }

            if (TextWindow.Width == 1 && onlySpaces)
            {
                return SyntaxFactory.Space;
            }
            else
            {
                var width = TextWindow.Width;

                if (width < MaxCachedTokenSize)
                {
                    return _cache.LookupTrivia(
                        TextWindow.CharacterWindow,
                        TextWindow.LexemeRelativeStart,
                        width,
                        hashCode,
                        _createWhitespaceTriviaFunction);
                }
                else
                {
                    return _createWhitespaceTriviaFunction();
                }
            }
        }

        private SyntaxTrivia CreateWhitespaceTrivia()
        {
            return SyntaxFactory.Whitespace(TextWindow.GetText(intern: true));
        }

        private void LexDirectiveAndExcludedTrivia(bool afterFirstToken, bool afterNonWhitespaceOnLine, ref SyntaxListBuilder triviaList)
        {
            if (LexSingleDirective(isActive: true, endIsActive: true, afterFirstToken, afterNonWhitespaceOnLine, ref triviaList) is BranchingDirectiveTriviaSyntax branchingDirectiveTriviaSyntax && !branchingDirectiveTriviaSyntax.BranchTaken)
            {
                LexExcludedDirectivesAndTrivia(endIsActive: true, ref triviaList);
            }
        }

        private void LexExcludedDirectivesAndTrivia(bool endIsActive, ref SyntaxListBuilder triviaList)
        {
            while (true)
            {
                CSharpSyntaxNode cSharpSyntaxNode = LexDisabledText(out bool followedByDirective);
                if (cSharpSyntaxNode != null)
                {
                    AddTrivia(cSharpSyntaxNode, ref triviaList);
                }
                if (followedByDirective)
                {
                    CSharpSyntaxNode cSharpSyntaxNode2 = LexSingleDirective(isActive: false, endIsActive, afterFirstToken: false, afterNonWhitespaceOnLine: false, ref triviaList);
                    BranchingDirectiveTriviaSyntax branchingDirectiveTriviaSyntax = cSharpSyntaxNode2 as BranchingDirectiveTriviaSyntax;
                    if (cSharpSyntaxNode2.Kind != SyntaxKind.EndIfDirectiveTrivia && (branchingDirectiveTriviaSyntax == null || !branchingDirectiveTriviaSyntax.BranchTaken))
                    {
                        if (cSharpSyntaxNode2.Kind == SyntaxKind.IfDirectiveTrivia)
                        {
                            LexExcludedDirectivesAndTrivia(endIsActive: false, ref triviaList);
                        }
                        continue;
                    }
                    break;
                }
                break;
            }
        }

        private CSharpSyntaxNode LexSingleDirective(bool isActive, bool endIsActive, bool afterFirstToken, bool afterNonWhitespaceOnLine, ref SyntaxListBuilder triviaList)
        {
            if (SyntaxFacts.IsWhitespace(TextWindow.PeekChar()))
            {
                Start();
                AddTrivia(ScanWhitespace(), ref triviaList);
            }
            LexerMode mode = _mode;
            CSharpSyntaxNode cSharpSyntaxNode;
            using (DirectiveParser directiveParser = new DirectiveParser(this, _directives))
            {
                cSharpSyntaxNode = directiveParser.ParseDirective(isActive, endIsActive, afterFirstToken, afterNonWhitespaceOnLine);
            }
            AddTrivia(cSharpSyntaxNode, ref triviaList);
            _directives = cSharpSyntaxNode.ApplyDirectives(_directives);
            _mode = mode;
            return cSharpSyntaxNode;
        }

        private CSharpSyntaxNode LexDisabledText(out bool followedByDirective)
        {
            this.Start();

            int lastLineStart = TextWindow.Position;
            int lines = 0;
            bool allWhitespace = true;

            while (true)
            {
                char ch = TextWindow.PeekChar();
                switch (ch)
                {
                    case SlidingTextWindow.InvalidCharacter:
                        if (!TextWindow.IsReallyAtEnd())
                        {
                            goto default;
                        }

                        followedByDirective = false;
                        return TextWindow.Width > 0 ? SyntaxFactory.DisabledText(TextWindow.GetText(false)) : null;
                    case '#':
                        if (!_allowPreprocessorDirectives) goto default;
                        followedByDirective = true;
                        if (lastLineStart < TextWindow.Position && !allWhitespace)
                        {
                            goto default;
                        }

                        TextWindow.Reset(lastLineStart);  // reset so directive parser can consume the starting whitespace on this line
                        return TextWindow.Width > 0 ? SyntaxFactory.DisabledText(TextWindow.GetText(false)) : null;
                    case '\r':
                    case '\n':
                        this.ScanEndOfLine();
                        lastLineStart = TextWindow.Position;
                        allWhitespace = true;
                        lines++;
                        break;
                    default:
                        if (SyntaxFacts.IsNewLine(ch))
                        {
                            goto case '\n';
                        }

                        allWhitespace = allWhitespace && SyntaxFacts.IsWhitespace(ch);
                        TextWindow.AdvanceChar();
                        break;
                }
            }
        }

        private SyntaxToken LexDirectiveToken()
        {
            Start();
            TokenInfo info = default(TokenInfo);
            ScanDirectiveToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(0);
            SyntaxListBuilder trailing = LexDirectiveTrailingTrivia(info.Kind == SyntaxKind.EndOfDirectiveToken);
            return Create(ref info, null, trailing, errors);
        }

        private bool ScanDirectiveToken(ref TokenInfo info)
        {
            bool flag = false;
            char ch;
            char surrogateCharacter;
            switch (ch = TextWindow.PeekChar())
            {
                case '\uffff':
                    if (TextWindow.IsReallyAtEnd())
                    {
                        info.Kind = SyntaxKind.EndOfDirectiveToken;
                        break;
                    }
                    goto default;
                case '\n':
                case '\r':
                    info.Kind = SyntaxKind.EndOfDirectiveToken;
                    break;
                case '#':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.HashToken;
                    break;
                case '(':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.OpenParenToken;
                    break;
                case ')':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CloseParenToken;
                    break;
                case ',':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.CommaToken;
                    break;
                case '!':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.ExclamationEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.ExclamationToken;
                    }
                    break;
                case '=':
                    TextWindow.AdvanceChar();
                    if (TextWindow.PeekChar() == '=')
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.EqualsEqualsToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.EqualsToken;
                    }
                    break;
                case '&':
                    if (TextWindow.PeekChar(1) == '&')
                    {
                        TextWindow.AdvanceChar(2);
                        info.Kind = SyntaxKind.AmpersandAmpersandToken;
                        break;
                    }
                    goto default;
                case '|':
                    if (TextWindow.PeekChar(1) == '|')
                    {
                        TextWindow.AdvanceChar(2);
                        info.Kind = SyntaxKind.BarBarToken;
                        break;
                    }
                    goto default;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ScanInteger();
                    info.Kind = SyntaxKind.NumericLiteralToken;
                    info.Text = TextWindow.GetText(intern: true);
                    info.ValueKind = SpecialType.System_Int32;
                    info.IntValue = GetValueInt32(info.Text, isHex: false);
                    break;
                case '"':
                    ScanStringLiteral(ref info, allowEscapes: false);
                    break;
                case '\\':
                    ch = TextWindow.PeekCharOrUnicodeEscape(out surrogateCharacter);
                    flag = true;
                    if (SyntaxFacts.IsIdentifierStartCharacter(ch))
                    {
                        ScanIdentifierOrKeyword(ref info);
                        break;
                    }
                    goto default;
                default:
                    if (flag || !SyntaxFacts.IsNewLine(ch))
                    {
                        if (SyntaxFacts.IsIdentifierStartCharacter(ch))
                        {
                            ScanIdentifierOrKeyword(ref info);
                            break;
                        }
                        if (flag)
                        {
                            TextWindow.NextCharOrUnicodeEscape(out surrogateCharacter, out var info2);
                            AddError(info2);
                        }
                        else
                        {
                            TextWindow.AdvanceChar();
                        }
                        info.Kind = SyntaxKind.None;
                        info.Text = TextWindow.GetText(intern: true);
                        break;
                    }
                    goto case '\n';
            }
            return info.Kind != SyntaxKind.None;
        }

        private SyntaxListBuilder LexDirectiveTrailingTrivia(bool includeEndOfLine)
        {
            SyntaxListBuilder list = null;
            while (true)
            {
                int position = TextWindow.Position;
                CSharpSyntaxNode cSharpSyntaxNode = LexDirectiveTrivia();
                if (cSharpSyntaxNode == null)
                {
                    break;
                }
                if (cSharpSyntaxNode.Kind == SyntaxKind.EndOfLineTrivia)
                {
                    if (includeEndOfLine)
                    {
                        AddTrivia(cSharpSyntaxNode, ref list);
                    }
                    else
                    {
                        TextWindow.Reset(position);
                    }
                    break;
                }
                AddTrivia(cSharpSyntaxNode, ref list);
            }
            return list;
        }

        private CSharpSyntaxNode LexDirectiveTrivia()
        {
            CSharpSyntaxNode result = null;
            Start();
            char c = TextWindow.PeekChar();
            switch (c)
            {
                case '/':
                    if (TextWindow.PeekChar(1) == '/')
                    {
                        ScanToEndOfLine();
                        result = SyntaxFactory.Comment(TextWindow.GetText(intern: false));
                    }
                    break;
                case '\n':
                case '\r':
                    result = ScanEndOfLine();
                    break;
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    result = ScanWhitespace();
                    break;
                default:
                    if (!SyntaxFacts.IsWhitespace(c))
                    {
                        if (!SyntaxFacts.IsNewLine(c))
                        {
                            break;
                        }
                        goto case '\n';
                    }
                    goto case '\t';
            }
            return result;
        }

        private CSharpSyntaxNode LexXmlDocComment(XmlDocCommentStyle style)
        {
            LexerMode mode = _mode;
            LexerMode modeflags = ((style != 0) ? LexerMode.XmlDocCommentStyleDelimited : LexerMode.XmlDocCommentLocationStart);
            if (_xmlParser == null)
            {
                _xmlParser = new DocumentationCommentParser(this, modeflags);
            }
            else
            {
                _xmlParser.ReInitialize(modeflags);
            }
            DocumentationCommentTriviaSyntax result = _xmlParser.ParseDocumentationComment(out bool isTerminated);
            _mode = mode;
            if (!isTerminated)
            {
                AddError(TextWindow.LexemeStartPosition, TextWindow.Width, ErrorCode.ERR_OpenEndedComment);
            }
            return result;
        }

        private SyntaxToken LexXmlToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTrivia(ref trivia);
            Start();
            ScanXmlToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlToken(ref TokenInfo info)
        {
            char ch;

            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            switch (ch = TextWindow.PeekChar())
            {
                case '&':
                    this.ScanXmlEntity(ref info);
                    info.Kind = SyntaxKind.XmlEntityLiteralToken;
                    break;

                case '<':
                    this.ScanXmlTagStart(ref info);
                    break;

                case '\r':
                case '\n':
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }

                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        goto case '\n';
                    }

                    this.ScanXmlText(ref info);
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    break;
            }

            return info.Kind != SyntaxKind.None;
        }

        private void ScanXmlTextLiteralNewLineToken(ref TokenInfo info)
        {
            ScanEndOfLine();
            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
            info.Kind = SyntaxKind.XmlTextLiteralNewLineToken;
            MutateLocation(XmlDocCommentLocation.Exterior);
        }

        private void ScanXmlTagStart(ref TokenInfo info)
        {
            if (TextWindow.PeekChar(1) == '!')
            {
                if (TextWindow.PeekChar(2) == '-' && TextWindow.PeekChar(3) == '-')
                {
                    TextWindow.AdvanceChar(4);
                    info.Kind = SyntaxKind.XmlCommentStartToken;
                }
                else if (TextWindow.PeekChar(2) == '[' && TextWindow.PeekChar(3) == 'C' && TextWindow.PeekChar(4) == 'D' && TextWindow.PeekChar(5) == 'A' && TextWindow.PeekChar(6) == 'T' && TextWindow.PeekChar(7) == 'A' && TextWindow.PeekChar(8) == '[')
                {
                    TextWindow.AdvanceChar(9);
                    info.Kind = SyntaxKind.XmlCDataStartToken;
                }
                else
                {
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.LessThanToken;
                }
            }
            else if (TextWindow.PeekChar(1) == '/')
            {
                TextWindow.AdvanceChar(2);
                info.Kind = SyntaxKind.LessThanSlashToken;
            }
            else if (TextWindow.PeekChar(1) == '?')
            {
                TextWindow.AdvanceChar(2);
                info.Kind = SyntaxKind.XmlProcessingInstructionStartToken;
            }
            else
            {
                TextWindow.AdvanceChar();
                info.Kind = SyntaxKind.LessThanToken;
            }
        }

        private void ScanXmlEntity(ref TokenInfo info)
        {
            info.StringValue = null;
            TextWindow.AdvanceChar();
            _builder.Clear();
            XmlParseErrorCode? xmlParseErrorCode = null;
            object[] array = null;
            char c;
            if (IsXmlNameStartChar(c = TextWindow.PeekChar()))
            {
                while (IsXmlNameChar(c = TextWindow.PeekChar()))
                {
                    TextWindow.AdvanceChar();
                    _builder.Append(c);
                }
                switch (_builder.ToString())
                {
                    case "lt":
                        info.StringValue = "<";
                        break;
                    case "gt":
                        info.StringValue = ">";
                        break;
                    case "amp":
                        info.StringValue = "&";
                        break;
                    case "apos":
                        info.StringValue = "'";
                        break;
                    case "quot":
                        info.StringValue = "\"";
                        break;
                    default:
                        {
                            xmlParseErrorCode = XmlParseErrorCode.XML_RefUndefinedEntity_1;
                            object[] array2 = new string[1] { _builder.ToString() };
                            array = array2;
                            break;
                        }
                }
            }
            else if (c == '#')
            {
                TextWindow.AdvanceChar();
                bool num = TextWindow.PeekChar() == 'x';
                uint num2 = 0u;
                if (num)
                {
                    TextWindow.AdvanceChar();
                    while (SyntaxFacts.IsHexDigit(c = TextWindow.PeekChar()))
                    {
                        TextWindow.AdvanceChar();
                        if (num2 <= 134217727)
                        {
                            num2 = (num2 << 4) + (uint)SyntaxFacts.HexValue(c);
                        }
                    }
                }
                else
                {
                    while (SyntaxFacts.IsDecDigit(c = TextWindow.PeekChar()))
                    {
                        TextWindow.AdvanceChar();
                        if (num2 <= 134217727)
                        {
                            num2 = (num2 << 3) + (num2 << 1) + (uint)SyntaxFacts.DecValue(c);
                        }
                    }
                }
                if (TextWindow.PeekChar() != ';')
                {
                    xmlParseErrorCode = XmlParseErrorCode.XML_InvalidCharEntity;
                }
                if (MatchesProductionForXmlChar(num2))
                {
                    char charsFromUtf = SlidingTextWindow.GetCharsFromUtf32(num2, out char lowSurrogate);
                    _builder.Append(charsFromUtf);
                    if (lowSurrogate != '\uffff')
                    {
                        _builder.Append(lowSurrogate);
                    }
                    info.StringValue = _builder.ToString();
                }
                else if (!xmlParseErrorCode.HasValue)
                {
                    xmlParseErrorCode = XmlParseErrorCode.XML_InvalidUnicodeChar;
                }
            }
            else if (SyntaxFacts.IsWhitespace(c) || SyntaxFacts.IsNewLine(c))
            {
                if (!xmlParseErrorCode.HasValue)
                {
                    xmlParseErrorCode = XmlParseErrorCode.XML_InvalidWhitespace;
                }
            }
            else if (!xmlParseErrorCode.HasValue)
            {
                xmlParseErrorCode = XmlParseErrorCode.XML_InvalidToken;
                object[] array2 = new string[1] { c.ToString() };
                array = array2;
            }
            c = TextWindow.PeekChar();
            if (c == ';')
            {
                TextWindow.AdvanceChar();
            }
            else if (!xmlParseErrorCode.HasValue)
            {
                xmlParseErrorCode = XmlParseErrorCode.XML_InvalidToken;
                object[] array2 = new string[1] { c.ToString() };
                array = array2;
            }
            info.Text = TextWindow.GetText(intern: true);
            if (info.StringValue == null)
            {
                info.StringValue = info.Text;
            }
            if (xmlParseErrorCode.HasValue)
            {
                AddError(xmlParseErrorCode.Value, array ?? new object[0]);
            }
        }

        private static bool MatchesProductionForXmlChar(uint charValue)
        {
            if (charValue != 9 && charValue != 10 && charValue != 13 && (charValue < 32 || charValue > 55295) && (charValue < 57344 || charValue > 65533))
            {
                if (charValue >= 65536)
                {
                    return charValue <= 1114111;
                }
                return false;
            }
            return true;
        }

        private void ScanXmlText(ref TokenInfo info)
        {
            // Collect "]]>" strings into their own XmlText.
            if (TextWindow.PeekChar() == ']' && TextWindow.PeekChar(1) == ']' && TextWindow.PeekChar(2) == '>')
            {
                TextWindow.AdvanceChar(3);
                info.StringValue = info.Text = TextWindow.GetText(false);
                this.AddError(XmlParseErrorCode.XML_CDataEndTagNotAllowed);
                return;
            }

            while (true)
            {
                var ch = TextWindow.PeekChar();
                switch (ch)
                {
                    case SlidingTextWindow.InvalidCharacter:
                        if (!TextWindow.IsReallyAtEnd())
                        {
                            goto default;
                        }

                        info.StringValue = info.Text = TextWindow.GetText(false);
                        return;
                    case '&':
                    case '<':
                    case '\r':
                    case '\n':
                        info.StringValue = info.Text = TextWindow.GetText(false);
                        return;

                    case '*':
                        if (this.StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                        {
                            // we're at the end of the comment, but don't lex it yet.
                            info.StringValue = info.Text = TextWindow.GetText(false);
                            return;
                        }

                        goto default;

                    case ']':
                        if (TextWindow.PeekChar(1) == ']' && TextWindow.PeekChar(2) == '>')
                        {
                            info.StringValue = info.Text = TextWindow.GetText(false);
                            return;
                        }

                        goto default;

                    default:
                        if (SyntaxFacts.IsNewLine(ch))
                        {
                            goto case '\n';
                        }

                        TextWindow.AdvanceChar();
                        break;
                }
            }
        }

        private SyntaxToken LexXmlElementTagToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTriviaWithWhitespace(ref trivia);
            Start();
            ScanXmlElementTagToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            if (errors == null && info.ContextualKind == SyntaxKind.None && info.Kind == SyntaxKind.IdentifierToken)
            {
                SyntaxToken syntaxToken = DocumentationCommentXmlTokens.LookupToken(info.Text, trivia);
                if (syntaxToken != null)
                {
                    return syntaxToken;
                }
            }
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlElementTagToken(ref TokenInfo info)
        {
            if (LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }
            char ch;
            switch (ch = TextWindow.PeekChar())
            {
                case '<':
                    ScanXmlTagStart(ref info);
                    break;
                case '>':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.GreaterThanToken;
                    break;
                case '/':
                    if (TextWindow.PeekChar(1) == '>')
                    {
                        TextWindow.AdvanceChar(2);
                        info.Kind = SyntaxKind.SlashGreaterThanToken;
                        break;
                    }
                    goto default;
                case '"':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.DoubleQuoteToken;
                    break;
                case '\'':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.SingleQuoteToken;
                    break;
                case '=':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.EqualsToken;
                    break;
                case ':':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.ColonToken;
                    break;
                case '\uffff':
                    if (TextWindow.IsReallyAtEnd())
                    {
                        info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                        break;
                    }
                    goto default;
                case '*':
                    if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                    {
                        break;
                    }
                    goto default;
                default:
                    if (IsXmlNameStartChar(ch))
                    {
                        ScanXmlName(ref info);
                        info.StringValue = info.Text;
                        info.Kind = SyntaxKind.IdentifierToken;
                    }
                    else if (!SyntaxFacts.IsWhitespace(ch) && !SyntaxFacts.IsNewLine(ch))
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.None;
                        info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                    }
                    break;
                case '\n':
                case '\r':
                    break;
            }
            return info.Kind != SyntaxKind.None;
        }

        private void ScanXmlName(ref TokenInfo info)
        {
            int position = TextWindow.Position;
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c == ':' || !IsXmlNameChar(c))
                {
                    break;
                }
                TextWindow.AdvanceChar();
            }
            info.Text = TextWindow.GetText(position, TextWindow.Position - position, intern: true);
        }

        private static bool IsXmlNameStartChar(char ch)
        {
            return XmlCharType.IsStartNCNameCharXml4e(ch);
        }

        private static bool IsXmlNameChar(char ch)
        {
            return XmlCharType.IsNCNameCharXml4e(ch);
        }

        private SyntaxToken LexXmlAttributeTextToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTrivia(ref trivia);
            Start();
            ScanXmlAttributeTextToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlAttributeTextToken(ref TokenInfo info)
        {
            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            char ch;
            switch (ch = TextWindow.PeekChar())
            {
                case '"':
                    if (this.ModeIs(LexerMode.XmlAttributeTextDoubleQuote))
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.DoubleQuoteToken;
                        break;
                    }

                    goto default;

                case '\'':
                    if (this.ModeIs(LexerMode.XmlAttributeTextQuote))
                    {
                        TextWindow.AdvanceChar();
                        info.Kind = SyntaxKind.SingleQuoteToken;
                        break;
                    }

                    goto default;

                case '&':
                    this.ScanXmlEntity(ref info);
                    info.Kind = SyntaxKind.XmlEntityLiteralToken;
                    break;

                case '<':
                    TextWindow.AdvanceChar();
                    info.Kind = SyntaxKind.LessThanToken;
                    break;

                case '\r':
                case '\n':
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }

                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        goto case '\n';
                    }

                    this.ScanXmlAttributeText(ref info);
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    break;
            }

            return info.Kind != SyntaxKind.None;
        }

        private void ScanXmlAttributeText(ref TokenInfo info)
        {
            while (true)
            {
                char c = TextWindow.PeekChar();
                switch (c)
                {
                    case '"':
                        if (ModeIs(LexerMode.XmlAttributeTextDoubleQuote))
                        {
                            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                            return;
                        }
                        goto default;
                    case '\'':
                        if (ModeIs(LexerMode.XmlAttributeTextQuote))
                        {
                            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                            return;
                        }
                        goto default;
                    case '\n':
                    case '\r':
                    case '&':
                    case '<':
                        info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                        return;
                    case '\uffff':
                        if (TextWindow.IsReallyAtEnd())
                        {
                            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                            return;
                        }
                        goto default;
                    case '*':
                        if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                        {
                            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                            return;
                        }
                        goto default;
                    default:
                        if (!SyntaxFacts.IsNewLine(c))
                        {
                            break;
                        }
                        goto case '\n';
                }
                TextWindow.AdvanceChar();
            }
        }

        private SyntaxToken LexXmlCharacter()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTriviaWithWhitespace(ref trivia);
            Start();
            ScanXmlCharacter(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlCharacter(ref TokenInfo info)
        {
            if (LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }
            char c = TextWindow.PeekChar();
            if (c != '&')
            {
                if (c == '\uffff' && TextWindow.IsReallyAtEnd())
                {
                    info.Kind = SyntaxKind.EndOfFileToken;
                }
                else
                {
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    info.Text = (info.StringValue = TextWindow.NextChar().ToString());
                }
            }
            else
            {
                ScanXmlEntity(ref info);
                info.Kind = SyntaxKind.XmlEntityLiteralToken;
            }
            return true;
        }

        private SyntaxToken LexXmlCrefOrNameToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTriviaWithWhitespace(ref trivia);
            Start();
            ScanXmlCrefToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        /// <summary>
        /// Scan a single cref attribute token.  Assumes that leading trivia has already
        /// been consumed.
        /// </summary>
        /// <remarks>
        /// Within this method, characters that are not XML meta-characters can be seamlessly
        /// replaced with the corresponding XML entities.
        /// </remarks>
        private bool ScanXmlCrefToken(ref TokenInfo info)
        {
            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            int beforeConsumed = TextWindow.Position;
            char consumedChar = TextWindow.NextChar();
            char consumedSurrogate = SlidingTextWindow.InvalidCharacter;

            // This first switch is for special characters.  If we see the corresponding
            // XML entities, we DO NOT want to take these actions.
            switch (consumedChar)
            {
                case '"':
                    if (this.ModeIs(LexerMode.XmlCrefDoubleQuote) || this.ModeIs(LexerMode.XmlNameDoubleQuote))
                    {
                        info.Kind = SyntaxKind.DoubleQuoteToken;
                        return true;
                    }

                    break;

                case '\'':
                    if (this.ModeIs(LexerMode.XmlCrefQuote) || this.ModeIs(LexerMode.XmlNameQuote))
                    {
                        info.Kind = SyntaxKind.SingleQuoteToken;
                        return true;
                    }

                    break;

                case '<':
                    info.Text = TextWindow.GetText(intern: false);
                    this.AddError(XmlParseErrorCode.XML_LessThanInAttributeValue, info.Text); //ErrorCode.WRN_XMLParseError
                    return true;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }

                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    return true;

                case '\r':
                case '\n':
                    TextWindow.Reset(beforeConsumed);
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case '&':
                    TextWindow.Reset(beforeConsumed);
                    if (!TextWindow.TryScanXmlEntity(out consumedChar, out consumedSurrogate))
                    {
                        TextWindow.Reset(beforeConsumed);
                        this.ScanXmlEntity(ref info);
                        info.Kind = SyntaxKind.XmlEntityLiteralToken;
                        return true;
                    }

                    // TryScanXmlEntity advances even when it returns false.
                    break;

                case '{':
                    consumedChar = '<';
                    break;

                case '}':
                    consumedChar = '>';
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(consumedChar))
                    {
                        goto case '\n';
                    }

                    break;
            }

            // NOTE: None of these cases will be matched if the surrogate is non-zero (UTF-16 rules)
            // so we don't need to check for that explicitly.

            // NOTE: there's a lot of overlap between this switch and the one in
            // ScanSyntaxToken, but we probably don't want to share code because
            // ScanSyntaxToken is really hot code and this switch does some extra
            // work.
            switch (consumedChar)
            {
                //// Single-Character Punctuation/Operators ////
                case '(':
                    info.Kind = SyntaxKind.OpenParenToken;
                    break;
                case ')':
                    info.Kind = SyntaxKind.CloseParenToken;
                    break;
                case '[':
                    info.Kind = SyntaxKind.OpenBracketToken;
                    break;
                case ']':
                    info.Kind = SyntaxKind.CloseBracketToken;
                    break;
                case ',':
                    info.Kind = SyntaxKind.CommaToken;
                    break;
                case '.':
                    if (AdvanceIfMatches('.'))
                    {
                        if (TextWindow.PeekChar() == '.')
                        {
                            // See documentation in ScanSyntaxToken
                            this.AddCrefError(ErrorCode.ERR_UnexpectedCharacter, ".");
                        }

                        info.Kind = SyntaxKind.DotDotToken;
                    }
                    else
                    {
                        info.Kind = SyntaxKind.DotToken;
                    }
                    break;
                case '?':
                    info.Kind = SyntaxKind.QuestionToken;
                    break;
                case '&':
                    info.Kind = SyntaxKind.AmpersandToken;
                    break;
                case '*':
                    info.Kind = SyntaxKind.AsteriskToken;
                    break;
                case '|':
                    info.Kind = SyntaxKind.BarToken;
                    break;
                case '^':
                    info.Kind = SyntaxKind.CaretToken;
                    break;
                case '%':
                    info.Kind = SyntaxKind.PercentToken;
                    break;
                case '/':
                    info.Kind = SyntaxKind.SlashToken;
                    break;
                case '~':
                    info.Kind = SyntaxKind.TildeToken;
                    break;

                // NOTE: Special case - convert curly brackets into angle brackets.
                case '{':
                    info.Kind = SyntaxKind.LessThanToken;
                    break;
                case '}':
                    info.Kind = SyntaxKind.GreaterThanToken;
                    break;

                //// Multi-Character Punctuation/Operators ////
                case ':':
                    if (AdvanceIfMatches(':')) info.Kind = SyntaxKind.ColonColonToken;
                    else info.Kind = SyntaxKind.ColonToken;
                    break;
                case '=':
                    if (AdvanceIfMatches('=')) info.Kind = SyntaxKind.EqualsEqualsToken;
                    else info.Kind = SyntaxKind.EqualsToken;
                    break;
                case '!':
                    if (AdvanceIfMatches('=')) info.Kind = SyntaxKind.ExclamationEqualsToken;
                    else info.Kind = SyntaxKind.ExclamationToken;
                    break;
                case '>':
                    if (AdvanceIfMatches('=')) info.Kind = SyntaxKind.GreaterThanEqualsToken;
                    // GreaterThanGreaterThanToken is synthesized in the parser since it is ambiguous (with closing nested type parameter lists)
                    // else if (AdvanceIfMatches('>')) info.Kind = SyntaxKind.GreaterThanGreaterThanToken;
                    else info.Kind = SyntaxKind.GreaterThanToken;
                    break;
                case '<':
                    if (AdvanceIfMatches('=')) info.Kind = SyntaxKind.LessThanEqualsToken;
                    else if (AdvanceIfMatches('<')) info.Kind = SyntaxKind.LessThanLessThanToken;
                    else info.Kind = SyntaxKind.LessThanToken;
                    break;
                case '+':
                    if (AdvanceIfMatches('+')) info.Kind = SyntaxKind.PlusPlusToken;
                    else info.Kind = SyntaxKind.PlusToken;
                    break;
                case '-':
                    if (AdvanceIfMatches('-')) info.Kind = SyntaxKind.MinusMinusToken;
                    else info.Kind = SyntaxKind.MinusToken;
                    break;
            }

            if (info.Kind != SyntaxKind.None)
            {
                string valueText = SyntaxFacts.GetText(info.Kind);
                string actualText = TextWindow.GetText(intern: false);
                if (!string.IsNullOrEmpty(valueText) && actualText != valueText)
                {
                    info.RequiresTextForXmlEntity = true;
                    info.Text = actualText;
                    info.StringValue = valueText;
                }
            }
            else
            {
                // If we didn't match any of the above cases, then we either have an
                // identifier or an unexpected character.

                TextWindow.Reset(beforeConsumed);

                if (this.ScanIdentifier(ref info) && info.Text.Length > 0)
                {
                    // ACASEY:  All valid identifier characters should be valid in XML attribute values,
                    // but I don't want to add an assert because XML character classification is expensive.
                    // check to see if it is an actual keyword
                    // NOTE: name attribute values don't respect keywords - everything is an identifier.
                    SyntaxKind keywordKind;
                    if (!InXmlNameAttributeValue && !info.IsVerbatim && !info.HasIdentifierEscapeSequence && _cache.TryGetKeywordKind(info.StringValue, out keywordKind))
                    {
                        if (SyntaxFacts.IsContextualKeyword(keywordKind))
                        {
                            info.Kind = SyntaxKind.IdentifierToken;
                            info.ContextualKind = keywordKind;
                            // Don't need to set any special flags to store the original text of an identifier.
                        }
                        else
                        {
                            info.Kind = keywordKind;
                            info.RequiresTextForXmlEntity = info.Text != info.StringValue;
                        }
                    }
                    else
                    {
                        info.ContextualKind = info.Kind = SyntaxKind.IdentifierToken;
                    }
                }
                else
                {
                    if (consumedChar == '@')
                    {
                        // Saw '@', but it wasn't followed by an identifier (otherwise ScanIdentifier would have succeeded).
                        if (TextWindow.PeekChar() == '@')
                        {
                            TextWindow.NextChar();
                            info.Text = TextWindow.GetText(intern: true);
                            info.StringValue = ""; // Can't be null for an identifier.
                        }
                        else
                        {
                            this.ScanXmlEntity(ref info);
                        }
                        info.Kind = SyntaxKind.IdentifierToken;
                        this.AddError(ErrorCode.ERR_ExpectedVerbatimLiteral);
                    }
                    else if (TextWindow.PeekChar() == '&')
                    {
                        this.ScanXmlEntity(ref info);
                        info.Kind = SyntaxKind.XmlEntityLiteralToken;
                        this.AddCrefError(ErrorCode.ERR_UnexpectedCharacter, info.Text);
                    }
                    else
                    {
                        char bad = TextWindow.NextChar();
                        info.Text = TextWindow.GetText(intern: false);

                        // If it's valid in XML, then it was unexpected in cref mode.
                        // Otherwise, it's just bad XML.
                        if (MatchesProductionForXmlChar((uint)bad))
                        {
                            this.AddCrefError(ErrorCode.ERR_UnexpectedCharacter, info.Text);
                        }
                        else
                        {
                            this.AddError(XmlParseErrorCode.XML_InvalidUnicodeChar);
                        }
                    }
                }
            }

            return info.Kind != SyntaxKind.None;
        }

        private bool AdvanceIfMatches(char ch)
        {
            char c = TextWindow.PeekChar();
            if (c == ch || (c == '{' && ch == '<') || (c == '}' && ch == '>'))
            {
                TextWindow.AdvanceChar();
                return true;
            }
            if (c == '&')
            {
                int position = TextWindow.Position;
                if (TextWindow.TryScanXmlEntity(out var ch2, out var surrogate) && ch2 == ch && surrogate == '\uffff')
                {
                    return true;
                }
                TextWindow.Reset(position);
            }
            return false;
        }

        private void AddCrefError(ErrorCode code, params object[] args)
        {
            AddCrefError(AbstractLexer.MakeError(code, args));
        }

        private void AddCrefError(DiagnosticInfo info)
        {
            if (info != null)
            {
                AddError(ErrorCode.WRN_ErrorOverride, info, info.Code);
            }
        }

        private SyntaxToken LexXmlCDataSectionTextToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTrivia(ref trivia);
            Start();
            ScanXmlCDataSectionTextToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlCDataSectionTextToken(ref TokenInfo info)
        {
            char ch;

            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            switch (ch = TextWindow.PeekChar())
            {
                case ']':
                    if (TextWindow.PeekChar(1) == ']' && TextWindow.PeekChar(2) == '>')
                    {
                        TextWindow.AdvanceChar(3);
                        info.Kind = SyntaxKind.XmlCDataEndToken;
                        break;
                    }

                    goto default;

                case '\r':
                case '\n':
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }

                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        goto case '\n';
                    }

                    this.ScanXmlCDataSectionText(ref info);
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    break;
            }

            return true;
        }

        private void ScanXmlCDataSectionText(ref TokenInfo info)
        {
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c <= 13u)
                {
                    if (c == '\n' || c == '\r')
                    {
                        break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case ']':
                            if (TextWindow.PeekChar(1) == ']' && TextWindow.PeekChar(2) == '>')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '\uffff':
                            if (TextWindow.IsReallyAtEnd())
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '*':
                            if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                    }
                }
                if (SyntaxFacts.IsNewLine(c))
                {
                    break;
                }
                TextWindow.AdvanceChar();
            }
            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
        }

        private SyntaxToken LexXmlCommentTextToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTrivia(ref trivia);
            Start();
            ScanXmlCommentTextToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        private bool ScanXmlCommentTextToken(ref TokenInfo info)
        {
            char ch;

            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            switch (ch = TextWindow.PeekChar())
            {
                case '-':
                    if (TextWindow.PeekChar(1) == '-')
                    {
                        if (TextWindow.PeekChar(2) == '>')
                        {
                            TextWindow.AdvanceChar(3);
                            info.Kind = SyntaxKind.XmlCommentEndToken;
                            break;
                        }
                        else
                        {
                            TextWindow.AdvanceChar(2);
                            info.Kind = SyntaxKind.MinusMinusToken;
                            break;
                        }
                    }

                    goto default;

                case '\r':
                case '\n':
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }
                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        goto case '\n';
                    }

                    this.ScanXmlCommentText(ref info);
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    break;
            }

            return true;
        }

        private void ScanXmlCommentText(ref TokenInfo info)
        {
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c <= 13u)
                {
                    if (c == '\n' || c == '\r')
                    {
                        break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '-':
                            if (TextWindow.PeekChar(1) == '-')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '\uffff':
                            if (TextWindow.IsReallyAtEnd())
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '*':
                            if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                    }
                }
                if (SyntaxFacts.IsNewLine(c))
                {
                    break;
                }
                TextWindow.AdvanceChar();
            }
            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
        }

        private SyntaxToken LexXmlProcessingInstructionTextToken()
        {
            TokenInfo info = default(TokenInfo);
            SyntaxListBuilder trivia = null;
            LexXmlDocCommentLeadingTrivia(ref trivia);
            Start();
            ScanXmlProcessingInstructionTextToken(ref info);
            SyntaxDiagnosticInfo[] errors = GetErrors(GetFullWidth(trivia));
            return Create(ref info, trivia, null, errors);
        }

        // CONSIDER: This could easily be merged with ScanXmlCDataSectionTextToken
        private bool ScanXmlProcessingInstructionTextToken(ref TokenInfo info)
        {
            char ch;

            if (this.LocationIs(XmlDocCommentLocation.End))
            {
                info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                return true;
            }

            switch (ch = TextWindow.PeekChar())
            {
                case '?':
                    if (TextWindow.PeekChar(1) == '>')
                    {
                        TextWindow.AdvanceChar(2);
                        info.Kind = SyntaxKind.XmlProcessingInstructionEndToken;
                        break;
                    }

                    goto default;

                case '\r':
                case '\n':
                    ScanXmlTextLiteralNewLineToken(ref info);
                    break;

                case SlidingTextWindow.InvalidCharacter:
                    if (!TextWindow.IsReallyAtEnd())
                    {
                        goto default;
                    }

                    info.Kind = SyntaxKind.EndOfDocumentationCommentToken;
                    break;

                default:
                    if (SyntaxFacts.IsNewLine(ch))
                    {
                        goto case '\n';
                    }

                    this.ScanXmlProcessingInstructionText(ref info);
                    info.Kind = SyntaxKind.XmlTextLiteralToken;
                    break;
            }

            return true;
        }

        private void ScanXmlProcessingInstructionText(ref TokenInfo info)
        {
            while (true)
            {
                char c = TextWindow.PeekChar();
                if (c <= 13u)
                {
                    if (c == '\n' || c == '\r')
                    {
                        break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '?':
                            if (TextWindow.PeekChar(1) == '>')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '\uffff':
                            if (TextWindow.IsReallyAtEnd())
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                        case '*':
                            if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                            {
                                info.StringValue = (info.Text = TextWindow.GetText(intern: false));
                                return;
                            }
                            break;
                    }
                }
                if (SyntaxFacts.IsNewLine(c))
                {
                    break;
                }
                TextWindow.AdvanceChar();
            }
            info.StringValue = (info.Text = TextWindow.GetText(intern: false));
        }

        /// <summary>
        /// Collects XML doc comment exterior trivia, and therefore is a no op unless we are in the Start or Exterior of an XML doc comment.
        /// </summary>
        /// <param name="trivia">List in which to collect the trivia</param>
        private void LexXmlDocCommentLeadingTrivia(ref SyntaxListBuilder trivia)
        {
            var start = TextWindow.Position;
            this.Start();

            if (this.LocationIs(XmlDocCommentLocation.Start) && this.StyleIs(XmlDocCommentStyle.Delimited))
            {
                // Read the /** that begins an XML doc comment. Since these are recognized only
                // when the trailing character is not a '*', we wind up in the interior of the
                // doc comment at the end.

                if (TextWindow.PeekChar() == '/'
                    && TextWindow.PeekChar(1) == '*'
                    && TextWindow.PeekChar(2) == '*'
                    && TextWindow.PeekChar(3) != '*')
                {
                    TextWindow.AdvanceChar(3);
                    var text = TextWindow.GetText(true);
                    this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia(text), ref trivia);
                    this.MutateLocation(XmlDocCommentLocation.Interior);
                    return;
                }
            }
            else if (this.LocationIs(XmlDocCommentLocation.Start) || this.LocationIs(XmlDocCommentLocation.Exterior))
            {
                // We're in the exterior of an XML doc comment and need to eat the beginnings of
                // lines, for single line and delimited comments. We chew up white space until
                // a non-whitespace character, and then make the right decision depending on
                // what kind of comment we're in.

                while (true)
                {
                    char ch = TextWindow.PeekChar();
                    switch (ch)
                    {
                        case ' ':
                        case '\t':
                        case '\v':
                        case '\f':
                            TextWindow.AdvanceChar();
                            break;

                        case '/':
                            if (this.StyleIs(XmlDocCommentStyle.SingleLine) && TextWindow.PeekChar(1) == '/' && TextWindow.PeekChar(2) == '/' && TextWindow.PeekChar(3) != '/')
                            {
                                TextWindow.AdvanceChar(3);
                                var text = TextWindow.GetText(true);
                                this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia(text), ref trivia);
                                this.MutateLocation(XmlDocCommentLocation.Interior);
                                return;
                            }

                            goto default;

                        case '*':
                            if (this.StyleIs(XmlDocCommentStyle.Delimited))
                            {
                                while (TextWindow.PeekChar() == '*' && TextWindow.PeekChar(1) != '/')
                                {
                                    TextWindow.AdvanceChar();
                                }

                                var text = TextWindow.GetText(true);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia(text), ref trivia);
                                }

                                // This setup ensures that on the final line of a comment, if we have
                                // the string "  */", the "*/" part is separated from the whitespace
                                // and therefore recognizable as the end of the comment.

                                if (TextWindow.PeekChar() == '*' && TextWindow.PeekChar(1) == '/')
                                {
                                    TextWindow.AdvanceChar(2);
                                    this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia("*/"), ref trivia);
                                    this.MutateLocation(XmlDocCommentLocation.End);
                                }
                                else
                                {
                                    this.MutateLocation(XmlDocCommentLocation.Interior);
                                }

                                return;
                            }

                            goto default;

                        default:
                            if (SyntaxFacts.IsWhitespace(ch))
                            {
                                goto case ' ';
                            }

                            // so here we have something else. if this is a single-line xml
                            // doc comment, that means we're on a line that's no longer a doc
                            // comment, so we need to rewind. if we're in a delimited doc comment,
                            // then that means we hit pay dirt and we're back into xml text.

                            if (this.StyleIs(XmlDocCommentStyle.SingleLine))
                            {
                                TextWindow.Reset(start);
                                this.MutateLocation(XmlDocCommentLocation.End);
                            }
                            else // XmlDocCommentStyle.Delimited
                            {
                                var text = TextWindow.GetText(true);
                                if (!String.IsNullOrEmpty(text))
                                    this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia(text), ref trivia);
                                this.MutateLocation(XmlDocCommentLocation.Interior);
                            }

                            return;
                    }
                }
            }
            else if (!this.LocationIs(XmlDocCommentLocation.End) && this.StyleIs(XmlDocCommentStyle.Delimited))
            {
                if (TextWindow.PeekChar() == '*' && TextWindow.PeekChar(1) == '/')
                {
                    TextWindow.AdvanceChar(2);
                    var text = TextWindow.GetText(true);
                    this.AddTrivia(SyntaxFactory.DocumentationCommentExteriorTrivia(text), ref trivia);
                    this.MutateLocation(XmlDocCommentLocation.End);
                }
            }
        }

        private void LexXmlDocCommentLeadingTriviaWithWhitespace(ref SyntaxListBuilder trivia)
        {
            while (true)
            {
                LexXmlDocCommentLeadingTrivia(ref trivia);
                char ch = TextWindow.PeekChar();
                if (LocationIs(XmlDocCommentLocation.Interior) && (SyntaxFacts.IsWhitespace(ch) || SyntaxFacts.IsNewLine(ch)))
                {
                    LexXmlWhitespaceAndNewLineTrivia(ref trivia);
                    continue;
                }
                break;
            }
        }

        private void LexXmlWhitespaceAndNewLineTrivia(ref SyntaxListBuilder trivia)
        {
            Start();
            if (!LocationIs(XmlDocCommentLocation.Interior))
            {
                return;
            }
            char c = TextWindow.PeekChar();
            switch (c)
            {
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    AddTrivia(ScanWhitespace(), ref trivia);
                    break;
                case '\n':
                case '\r':
                    AddTrivia(ScanEndOfLine(), ref trivia);
                    MutateLocation(XmlDocCommentLocation.Exterior);
                    break;
                case '*':
                    if (StyleIs(XmlDocCommentStyle.Delimited) && TextWindow.PeekChar(1) == '/')
                    {
                        break;
                    }
                    goto default;
                default:
                    if (SyntaxFacts.IsWhitespace(c))
                    {
                        goto case '\t';
                    }
                    if (!SyntaxFacts.IsNewLine(c))
                    {
                        break;
                    }
                    goto case '\n';
            }
        }

        private void ScanStringLiteral(ref TokenInfo info, bool allowEscapes = true)
        {
            char c = TextWindow.PeekChar();
            if (c == '\'' || c == '"')
            {
                TextWindow.AdvanceChar();
                _builder.Length = 0;
                while (true)
                {
                    char c2 = TextWindow.PeekChar();
                    if (c2 == '\\' && allowEscapes)
                    {
                        c2 = ScanEscapeSequence(out var surrogateCharacter);
                        _builder.Append(c2);
                        if (surrogateCharacter != '\uffff')
                        {
                            _builder.Append(surrogateCharacter);
                        }
                        continue;
                    }
                    if (c2 == c)
                    {
                        TextWindow.AdvanceChar();
                        break;
                    }
                    if (SyntaxFacts.IsNewLine(c2) || (c2 == '\uffff' && TextWindow.IsReallyAtEnd()))
                    {
                        AddError(ErrorCode.ERR_NewlineInConst);
                        break;
                    }
                    TextWindow.AdvanceChar();
                    _builder.Append(c2);
                }
                info.Text = TextWindow.GetText(intern: true);
                if (c == '\'')
                {
                    info.Kind = SyntaxKind.CharacterLiteralToken;
                    if (_builder.Length != 1)
                    {
                        AddError((_builder.Length != 0) ? ErrorCode.ERR_TooManyCharsInConst : ErrorCode.ERR_EmptyCharConst);
                    }
                    if (_builder.Length > 0)
                    {
                        info.StringValue = TextWindow.Intern(_builder);
                        info.CharValue = info.StringValue[0];
                    }
                    else
                    {
                        info.StringValue = string.Empty;
                        info.CharValue = '\uffff';
                    }
                }
                else
                {
                    info.Kind = SyntaxKind.StringLiteralToken;
                    if (_builder.Length > 0)
                    {
                        info.StringValue = TextWindow.Intern(_builder);
                    }
                    else
                    {
                        info.StringValue = string.Empty;
                    }
                }
            }
            else
            {
                info.Kind = SyntaxKind.None;
                info.Text = null;
            }
        }

        private char ScanEscapeSequence(out char surrogateCharacter)
        {
            int position = TextWindow.Position;
            surrogateCharacter = '\uffff';
            char c = TextWindow.NextChar();
            c = TextWindow.NextChar();
            switch (c)
            {
                case '0':
                    c = '\0';
                    break;
                case 'a':
                    c = '\a';
                    break;
                case 'b':
                    c = '\b';
                    break;
                case 'f':
                    c = '\f';
                    break;
                case 'n':
                    c = '\n';
                    break;
                case 'r':
                    c = '\r';
                    break;
                case 't':
                    c = '\t';
                    break;
                case 'v':
                    c = '\v';
                    break;
                case 'U':
                case 'u':
                case 'x':
                    {
                        TextWindow.Reset(position);
                        c = TextWindow.NextUnicodeEscape(out surrogateCharacter, out var info);
                        AddError(info);
                        break;
                    }
                default:
                    AddError(position, TextWindow.Position - position, ErrorCode.ERR_IllegalEscape);
                    break;
                case '"':
                case '\'':
                case '\\':
                    break;
            }
            return c;
        }

        private void ScanVerbatimStringLiteral(ref TokenInfo info, bool allowNewlines = true)
        {
            _builder.Length = 0;
            if (TextWindow.PeekChar() == '@' && TextWindow.PeekChar(1) == '"')
            {
                TextWindow.AdvanceChar(2);
                bool flag = false;
                _builder.Length = 0;
                while (!flag)
                {
                    char c;
                    switch (c = TextWindow.PeekChar())
                    {
                        case '"':
                            TextWindow.AdvanceChar();
                            if (TextWindow.PeekChar() == '"')
                            {
                                TextWindow.AdvanceChar();
                                _builder.Append(c);
                            }
                            else
                            {
                                flag = true;
                            }
                            continue;
                        case '\uffff':
                            if (TextWindow.IsReallyAtEnd())
                            {
                                AddError(ErrorCode.ERR_UnterminatedStringLit);
                                flag = true;
                                continue;
                            }
                            break;
                    }
                    if (!allowNewlines && SyntaxFacts.IsNewLine(c))
                    {
                        AddError(ErrorCode.ERR_UnterminatedStringLit);
                        flag = true;
                    }
                    else
                    {
                        TextWindow.AdvanceChar();
                        _builder.Append(c);
                    }
                }
                info.Kind = SyntaxKind.StringLiteralToken;
                info.Text = TextWindow.GetText(intern: false);
                info.StringValue = _builder.ToString();
            }
            else
            {
                info.Kind = SyntaxKind.None;
                info.Text = null;
                info.StringValue = null;
            }
        }

        private void ScanInterpolatedStringLiteral(bool isVerbatim, ref TokenInfo info)
        {
            SyntaxDiagnosticInfo error = null;
            ScanInterpolatedStringLiteralTop(null, isVerbatim, ref info, ref error, out var _);
            AddError(error);
        }

        internal void ScanInterpolatedStringLiteralTop(ArrayBuilder<Interpolation> interpolations, bool isVerbatim, ref TokenInfo info, ref SyntaxDiagnosticInfo error, out bool closeQuoteMissing)
        {
            InterpolatedStringScanner interpolatedStringScanner = new InterpolatedStringScanner(this, isVerbatim);
            interpolatedStringScanner.ScanInterpolatedStringLiteralTop(interpolations, ref info, out closeQuoteMissing);
            error = interpolatedStringScanner.error;
            info.Text = TextWindow.GetText(intern: false);
        }

        internal static SyntaxToken RescanInterpolatedString(InterpolatedStringExpressionSyntax interpolatedString)
        {
            string text = interpolatedString.ToString();
            SyntaxKind kind = SyntaxKind.InterpolatedStringToken;
            return SyntaxFactory.Literal(interpolatedString.GetFirstToken().GetLeadingTrivia(), text, kind, text, interpolatedString.GetLastToken().GetTrailingTrivia());
        }

        private SyntaxToken QuickScanSyntaxToken()
        {
            Start();
            QuickScanState quickScanState = QuickScanState.Initial;
            int num = TextWindow.Offset;
            int characterWindowCount = TextWindow.CharacterWindowCount;
            characterWindowCount = Math.Min(characterWindowCount, num + 42);
            int num2 = -2128831035;
            char[] characterWindow = TextWindow.CharacterWindow;
            int num3 = s_charProperties.Length;
            while (true)
            {
                if (num < characterWindowCount)
                {
                    int num4 = characterWindow[num];
                    CharFlags charFlags = (CharFlags)((num4 < num3) ? s_charProperties[num4] : 9);
                    quickScanState = (QuickScanState)s_stateTransitions[(uint)quickScanState, (uint)charFlags];
                    if ((int)quickScanState >= 9)
                    {
                        break;
                    }
                    num2 = (num2 ^ num4) * 16777619;
                    num++;
                    continue;
                }
                quickScanState = QuickScanState.Bad;
                break;
            }
            TextWindow.AdvanceChar(num - TextWindow.Offset);
            if (quickScanState == QuickScanState.Done)
            {
                return _cache.LookupToken(TextWindow.CharacterWindow, TextWindow.LexemeRelativeStart, num - TextWindow.LexemeRelativeStart, num2, _createQuickTokenFunction);
            }
            TextWindow.Reset(TextWindow.LexemeStartPosition);
            return null;
        }

        private SyntaxToken CreateQuickToken()
        {
            TextWindow.Reset(TextWindow.LexemeStartPosition);
            return LexSyntaxToken();
        }
    }
}
