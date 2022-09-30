using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class Scanner : IDisposable
	{
		internal class ConditionalState
		{
			public enum BranchTakenState : byte
			{
				NotTaken,
				Taken,
				AlreadyTaken
			}

			private readonly BranchTakenState _branchTaken;

			private readonly bool _elseSeen;

			private readonly IfDirectiveTriviaSyntax _ifDirective;

			internal BranchTakenState BranchTaken => _branchTaken;

			internal bool ElseSeen => _elseSeen;

			internal IfDirectiveTriviaSyntax IfDirective => _ifDirective;

			internal ConditionalState(BranchTakenState branchTaken, bool elseSeen, IfDirectiveTriviaSyntax ifDirective)
			{
				_branchTaken = branchTaken;
				_elseSeen = elseSeen;
				_ifDirective = ifDirective;
			}
		}

		internal sealed class PreprocessorState
		{
			private readonly ImmutableDictionary<string, CConst> _symbols;

			private readonly ImmutableStack<ConditionalState> _conditionals;

			private readonly ImmutableStack<RegionDirectiveTriviaSyntax> _regionDirectives;

			private readonly bool _haveSeenRegionDirectives;

			private readonly ExternalSourceDirectiveTriviaSyntax _externalSourceDirective;

			internal ImmutableDictionary<string, CConst> SymbolsMap => _symbols;

			internal ImmutableStack<ConditionalState> ConditionalStack => _conditionals;

			internal ImmutableStack<RegionDirectiveTriviaSyntax> RegionDirectiveStack => _regionDirectives;

			internal bool HaveSeenRegionDirectives => _haveSeenRegionDirectives;

			internal ExternalSourceDirectiveTriviaSyntax ExternalSourceDirective => _externalSourceDirective;

			internal PreprocessorState(ImmutableDictionary<string, CConst> symbols)
			{
				_symbols = symbols;
				_conditionals = ImmutableStack.Create<ConditionalState>();
				_regionDirectives = ImmutableStack.Create<RegionDirectiveTriviaSyntax>();
			}

			private PreprocessorState(ImmutableDictionary<string, CConst> symbols, ImmutableStack<ConditionalState> conditionals, ImmutableStack<RegionDirectiveTriviaSyntax> regionDirectives, bool haveSeenRegionDirectives, ExternalSourceDirectiveTriviaSyntax externalSourceDirective)
			{
				_symbols = symbols;
				_conditionals = conditionals;
				_regionDirectives = regionDirectives;
				_haveSeenRegionDirectives = haveSeenRegionDirectives;
				_externalSourceDirective = externalSourceDirective;
			}

			private PreprocessorState SetSymbol(string name, CConst value)
			{
				ImmutableDictionary<string, CConst> symbols = _symbols;
				symbols = symbols.SetItem(name, value);
				return new PreprocessorState(symbols, _conditionals, _regionDirectives, _haveSeenRegionDirectives, _externalSourceDirective);
			}

			private PreprocessorState WithConditionals(ImmutableStack<ConditionalState> conditionals)
			{
				return new PreprocessorState(_symbols, conditionals, _regionDirectives, _haveSeenRegionDirectives, _externalSourceDirective);
			}

			private PreprocessorState WithRegions(ImmutableStack<RegionDirectiveTriviaSyntax> regions)
			{
				return new PreprocessorState(_symbols, _conditionals, regions, _haveSeenRegionDirectives || regions.Count() > 0, _externalSourceDirective);
			}

			private PreprocessorState WithExternalSource(ExternalSourceDirectiveTriviaSyntax externalSource)
			{
				return new PreprocessorState(_symbols, _conditionals, _regionDirectives, _haveSeenRegionDirectives, externalSource);
			}

			internal PreprocessorState InterpretConstDirective(ref DirectiveTriviaSyntax statement)
			{
				ConstDirectiveTriviaSyntax constDirectiveTriviaSyntax = (ConstDirectiveTriviaSyntax)statement;
				CConst cConst = ExpressionEvaluator.EvaluateExpression(constDirectiveTriviaSyntax.Value, _symbols);
				ERRID errorId = cConst.ErrorId;
				if (errorId != 0)
				{
					statement = Parser.ReportSyntaxError(statement, errorId, cConst.ErrorArgs);
				}
				return SetSymbol(constDirectiveTriviaSyntax.Name.IdentifierText, cConst);
			}

			internal PreprocessorState InterpretExternalSourceDirective(ref DirectiveTriviaSyntax statement)
			{
				ExternalSourceDirectiveTriviaSyntax externalSource = (ExternalSourceDirectiveTriviaSyntax)statement;
				if (_externalSourceDirective != null)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_NestedExternalSource);
					return this;
				}
				return WithExternalSource(externalSource);
			}

			internal PreprocessorState InterpretEndExternalSourceDirective(ref DirectiveTriviaSyntax statement)
			{
				if (_externalSourceDirective == null)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_EndExternalSource);
					return this;
				}
				return WithExternalSource(null);
			}

			internal PreprocessorState InterpretRegionDirective(ref DirectiveTriviaSyntax statement)
			{
				RegionDirectiveTriviaSyntax value = (RegionDirectiveTriviaSyntax)statement;
				return WithRegions(_regionDirectives.Push(value));
			}

			internal PreprocessorState InterpretEndRegionDirective(ref DirectiveTriviaSyntax statement)
			{
				if (_regionDirectives.Count() == 0)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_EndRegionNoRegion);
					return this;
				}
				return WithRegions(_regionDirectives.Pop());
			}

			internal PreprocessorState InterpretIfDirective(ref DirectiveTriviaSyntax statement)
			{
				CConst cConst = ExpressionEvaluator.EvaluateCondition(((IfDirectiveTriviaSyntax)statement).Condition, _symbols);
				ERRID errorId = cConst.ErrorId;
				if (errorId != 0)
				{
					statement = Parser.ReportSyntaxError(statement, errorId, cConst.ErrorArgs);
				}
				ConditionalState.BranchTakenState branchTaken = ((cConst.IsBad || cConst.IsBooleanTrue) ? ConditionalState.BranchTakenState.Taken : ConditionalState.BranchTakenState.NotTaken);
				return WithConditionals(_conditionals.Push(new ConditionalState(branchTaken, elseSeen: false, (IfDirectiveTriviaSyntax)statement)));
			}

			internal PreprocessorState InterpretElseIfDirective(ref DirectiveTriviaSyntax statement)
			{
				ImmutableStack<ConditionalState> immutableStack = _conditionals;
				ConditionalState conditionalState;
				if (immutableStack.Count() == 0)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_LbBadElseif);
					conditionalState = new ConditionalState(ConditionalState.BranchTakenState.NotTaken, elseSeen: false, null);
				}
				else
				{
					conditionalState = immutableStack.Peek();
					immutableStack = immutableStack.Pop();
					if (conditionalState.ElseSeen)
					{
						statement = Parser.ReportSyntaxError(statement, ERRID.ERR_LbElseifAfterElse);
					}
				}
				CConst cConst = ExpressionEvaluator.EvaluateCondition(((IfDirectiveTriviaSyntax)statement).Condition, _symbols);
				ERRID errorId = cConst.ErrorId;
				if (errorId != 0)
				{
					statement = Parser.ReportSyntaxError(statement, errorId, cConst.ErrorArgs);
				}
				ConditionalState.BranchTakenState branchTakenState = conditionalState.BranchTaken;
				switch (branchTakenState)
				{
				case ConditionalState.BranchTakenState.Taken:
					branchTakenState = ConditionalState.BranchTakenState.AlreadyTaken;
					break;
				case ConditionalState.BranchTakenState.NotTaken:
					if (!cConst.IsBad && cConst.IsBooleanTrue)
					{
						branchTakenState = ConditionalState.BranchTakenState.Taken;
					}
					break;
				}
				conditionalState = new ConditionalState(branchTakenState, conditionalState.ElseSeen, (IfDirectiveTriviaSyntax)statement);
				return WithConditionals(immutableStack.Push(conditionalState));
			}

			internal PreprocessorState InterpretElseDirective(ref DirectiveTriviaSyntax statement)
			{
				ImmutableStack<ConditionalState> conditionals = _conditionals;
				if (conditionals.Count() == 0)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_LbElseNoMatchingIf);
					return WithConditionals(conditionals.Push(new ConditionalState(ConditionalState.BranchTakenState.Taken, elseSeen: true, null)));
				}
				ConditionalState conditionalState = conditionals.Peek();
				conditionals = conditionals.Pop();
				if (conditionalState.ElseSeen)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_LbElseNoMatchingIf);
				}
				ConditionalState.BranchTakenState branchTakenState = conditionalState.BranchTaken;
				switch (branchTakenState)
				{
				case ConditionalState.BranchTakenState.Taken:
					branchTakenState = ConditionalState.BranchTakenState.AlreadyTaken;
					break;
				case ConditionalState.BranchTakenState.NotTaken:
					branchTakenState = ConditionalState.BranchTakenState.Taken;
					break;
				}
				conditionalState = new ConditionalState(branchTakenState, elseSeen: true, conditionalState.IfDirective);
				return WithConditionals(conditionals.Push(conditionalState));
			}

			internal PreprocessorState InterpretEndIfDirective(ref DirectiveTriviaSyntax statement)
			{
				if (_conditionals.Count() == 0)
				{
					statement = Parser.ReportSyntaxError(statement, ERRID.ERR_LbNoMatchingIf);
					return this;
				}
				return WithConditionals(_conditionals.Pop());
			}

			internal bool IsEquivalentTo(PreprocessorState other)
			{
				if (_conditionals.Count() > 0 || _symbols.Count > 0 || _externalSourceDirective != null || other._conditionals.Count() > 0 || other._symbols.Count > 0 || other._externalSourceDirective != null)
				{
					return false;
				}
				if (_regionDirectives.Count() != other._regionDirectives.Count())
				{
					return false;
				}
				if (_haveSeenRegionDirectives != other._haveSeenRegionDirectives)
				{
					return false;
				}
				return true;
			}
		}

		private enum AccumulatorState
		{
			Initial,
			InitialAllowLeadingMultilineTrivia,
			Ident,
			TypeChar,
			FollowingWhite,
			Punctuation,
			CompoundPunctStart,
			CR,
			Done,
			Bad
		}

		[Flags]
		private enum CharFlags : ushort
		{
			White = 1,
			Letter = 2,
			IdentOnly = 4,
			TypeChar = 8,
			Punct = 0x10,
			CompoundPunctStart = 0x20,
			CR = 0x40,
			LF = 0x80,
			Digit = 0x100,
			Complex = 0x200
		}

		public struct QuickScanResult
		{
			public readonly char[] Chars;

			public readonly int Start;

			public readonly int Length;

			public readonly int HashCode;

			public readonly byte TerminatorLength;

			public bool Succeeded => Length > 0;

			public QuickScanResult(int start, int length, char[] chars, int hashCode, byte terminatorLength)
			{
				this = default(QuickScanResult);
				Start = start;
				Length = length;
				Chars = chars;
				HashCode = hashCode;
				TerminatorLength = terminatorLength;
			}
		}

		private delegate Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanTriviaFunc();

		private enum NumericLiteralKind
		{
			Integral,
			Float,
			Decimal
		}

		private class Page
		{
			internal int _pageStart;

			internal readonly char[] _arr;

			private readonly ObjectPool<Page> _pool;

			private static readonly ObjectPool<Page> s_poolInstance = CreatePool();

			private Page(ObjectPool<Page> pool)
			{
				_pageStart = -1;
				_arr = new char[2048];
				_pool = pool;
			}

			internal void Free()
			{
				_pageStart = -1;
				_pool.Free(this);
			}

			private static ObjectPool<Page> CreatePool()
			{
				ObjectPool<Page> objectPool = null;
				objectPool = new ObjectPool<Page>(() => new Page(objectPool), 128);
				return objectPool;
			}

			internal static Page GetInstance()
			{
				return s_poolInstance.Allocate();
			}
		}

		internal struct XmlCharResult
		{
			internal readonly int Length;

			internal readonly char Char1;

			internal readonly char Char2;

			internal XmlCharResult(char ch)
			{
				this = default(XmlCharResult);
				Length = 1;
				Char1 = ch;
			}

			internal XmlCharResult(char ch1, char ch2)
			{
				this = default(XmlCharResult);
				Length = 2;
				Char1 = ch1;
				Char2 = ch2;
			}

			internal void AppendTo(StringBuilder list)
			{
				list.Append(Char1);
				if (Length == 2)
				{
					list.Append(Char2);
				}
			}
		}

		private struct TriviaKey
		{
			public readonly string spelling;

			public readonly SyntaxKind kind;

			public TriviaKey(string spelling, SyntaxKind kind)
			{
				this = default(TriviaKey);
				this.spelling = spelling;
				this.kind = kind;
			}
		}

		private struct TokenParts
		{
			internal readonly string spelling;

			internal readonly GreenNode pTrivia;

			internal readonly GreenNode fTrivia;

			internal TokenParts(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> pTrivia, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia, string spelling)
			{
				this = default(TokenParts);
				this.spelling = spelling;
				this.pTrivia = pTrivia.Node;
				this.fTrivia = fTrivia.Node;
			}
		}

		internal struct RestorePoint
		{
			private readonly Scanner _scanner;

			private readonly ScannerToken _currentToken;

			private readonly ScannerToken _prevToken;

			private readonly ScannerToken[] _tokens;

			private readonly int _lineBufferOffset;

			private readonly int _endOfTerminatorTrivia;

			private readonly PreprocessorState _scannerPreprocessorState;

			internal RestorePoint(Scanner scanner)
			{
				this = default(RestorePoint);
				_scanner = scanner;
				_currentToken = scanner._currentToken;
				_prevToken = scanner._prevToken;
				_tokens = scanner.SaveAndClearTokens();
				_lineBufferOffset = scanner._lineBufferOffset;
				_endOfTerminatorTrivia = scanner._endOfTerminatorTrivia;
				_scannerPreprocessorState = scanner._scannerPreprocessorState;
			}

			internal void RestoreTokens(bool includeLookAhead)
			{
				_scanner._currentToken = _currentToken;
				_scanner._prevToken = _prevToken;
				_scanner.RestoreTokens(includeLookAhead ? _tokens : null);
			}

			internal void Restore()
			{
				_scanner._currentToken = _currentToken;
				_scanner._prevToken = _prevToken;
				_scanner.RestoreTokens(_tokens);
				_scanner._lineBufferOffset = _lineBufferOffset;
				_scanner._endOfTerminatorTrivia = _endOfTerminatorTrivia;
				_scanner._scannerPreprocessorState = _scannerPreprocessorState;
			}
		}

		private struct LineBufferAndEndOfTerminatorOffsets
		{
			private readonly Scanner _scanner;

			private readonly int _lineBufferOffset;

			private readonly int _endOfTerminatorTrivia;

			public LineBufferAndEndOfTerminatorOffsets(Scanner scanner)
			{
				this = default(LineBufferAndEndOfTerminatorOffsets);
				_scanner = scanner;
				_lineBufferOffset = scanner._lineBufferOffset;
				_endOfTerminatorTrivia = scanner._endOfTerminatorTrivia;
			}

			public void Restore()
			{
				_scanner._lineBufferOffset = _lineBufferOffset;
				_scanner._endOfTerminatorTrivia = _endOfTerminatorTrivia;
			}
		}

		protected struct ScannerToken
		{
			public readonly SyntaxToken InnerTokenObject;

			public readonly int Position;

			public readonly int EndOfTerminatorTrivia;

			public readonly ScannerState State;

			public readonly PreprocessorState PreprocessorState;

			internal ScannerToken(PreprocessorState preprocessorState, int lineBufferOffset, int endOfTerminatorTrivia, SyntaxToken token, ScannerState state)
			{
				this = default(ScannerToken);
				PreprocessorState = preprocessorState;
				Position = lineBufferOffset;
				EndOfTerminatorTrivia = endOfTerminatorTrivia;
				InnerTokenObject = token;
				State = state;
			}

			internal ScannerToken With(ScannerState state, SyntaxToken token)
			{
				return new ScannerToken(PreprocessorState, Position, EndOfTerminatorTrivia, token, state);
			}

			internal ScannerToken With(PreprocessorState preprocessorState)
			{
				return new ScannerToken(preprocessorState, Position, EndOfTerminatorTrivia, InnerTokenObject, State);
			}
		}

		private bool _isScanningDirective;

		protected PreprocessorState _scannerPreprocessorState;

		private static readonly ushort[] s_charProperties;

		private const int s_CHARPROP_LENGTH = 384;

		internal const int MAX_CACHED_TOKENSIZE = 42;

		private static readonly ScanTriviaFunc s_scanNoTriviaFunc;

		private readonly ScanTriviaFunc _scanSingleLineTriviaFunc;

		protected int _lineBufferOffset;

		private int _endOfTerminatorTrivia;

		internal const int BadTokenCountLimit = 200;

		private int _badTokenCount;

		private readonly PooledStringBuilder _sbPooled;

		private readonly StringBuilder _sb;

		private readonly SyntaxListPool _triviaListPool;

		private readonly VisualBasicParseOptions _options;

		private readonly StringTable _stringTable;

		private readonly TextKeyedCache<SyntaxToken> _quickTokenTable;

		public const int TABLE_LIMIT = 512;

		private static readonly Func<string, SyntaxKind> s_keywordKindFactory;

		private static readonly ObjectPool<CachingIdentityFactory<string, SyntaxKind>> s_keywordsObjsPool;

		private readonly CachingIdentityFactory<string, SyntaxKind> _KeywordsObjs;

		private static readonly ObjectPool<CachingFactory<TokenParts, IdentifierTokenSyntax>> s_idTablePool;

		private readonly CachingFactory<TokenParts, IdentifierTokenSyntax> _idTable;

		private static readonly ObjectPool<CachingFactory<TokenParts, KeywordSyntax>> s_kwTablePool;

		private readonly CachingFactory<TokenParts, KeywordSyntax> _kwTable;

		private static readonly ObjectPool<CachingFactory<TokenParts, PunctuationSyntax>> s_punctTablePool;

		private readonly CachingFactory<TokenParts, PunctuationSyntax> _punctTable;

		private static readonly ObjectPool<CachingFactory<TokenParts, SyntaxToken>> s_literalTablePool;

		private readonly CachingFactory<TokenParts, SyntaxToken> _literalTable;

		private static readonly ObjectPool<CachingFactory<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>>> s_wslTablePool;

		private readonly CachingFactory<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>> _wslTable;

		private static readonly ObjectPool<CachingFactory<TriviaKey, SyntaxTrivia>> s_wsTablePool;

		private readonly CachingFactory<TriviaKey, SyntaxTrivia> _wsTable;

		private readonly bool _isScanningForExpressionCompiler;

		private bool _isDisposed;

		private static readonly int s_conflictMarkerLength;

		private Page _curPage;

		private readonly Page[] _pages;

		private const int s_PAGE_NUM_SHIFT = 2;

		private const int s_PAGE_NUM = 4;

		private const int s_PAGE_NUM_MASK = 3;

		private const int s_PAGE_SHIFT = 11;

		private const int s_PAGE_SIZE = 2048;

		private const int s_PAGE_MASK = 2047;

		private const int s_NOT_PAGE_MASK = -2048;

		private readonly SourceText _buffer;

		private readonly int _bufferLen;

		private StringBuilder _builder;

		private static readonly Func<TriviaKey, int> s_triviaKeyHasher;

		private static readonly Func<TriviaKey, SyntaxTrivia, bool> s_triviaKeyEquality;

		private static readonly SyntaxTrivia s_singleSpaceWhitespaceTrivia;

		private static readonly SyntaxTrivia s_fourSpacesWhitespaceTrivia;

		private static readonly SyntaxTrivia s_eightSpacesWhitespaceTrivia;

		private static readonly SyntaxTrivia s_twelveSpacesWhitespaceTrivia;

		private static readonly SyntaxTrivia s_sixteenSpacesWhitespaceTrivia;

		private static readonly Func<SyntaxListBuilder, int> s_wsListKeyHasher;

		private static readonly Func<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>, bool> s_wsListKeyEquality;

		private static readonly Func<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>> s_wsListFactory;

		private static readonly Func<TokenParts, int> s_tokenKeyHasher;

		private static readonly Func<TokenParts, SyntaxToken, bool> s_tokenKeyEquality;

		private static readonly SyntaxTrivia s_crLfTrivia;

		private readonly SyntaxToken _simpleEof;

		public const int MaxTokensLookAheadBeyondEOL = 4;

		public const int MaxCharsLookBehind = 1;

		private ScannerToken _prevToken;

		protected ScannerToken _currentToken;

		private readonly List<ScannerToken> _tokens;

		private bool _IsScanningXmlDoc;

		private bool _endOfXmlInsteadOfLastDocCommentLineBreak;

		private bool _isStartingFirstXmlDocLine;

		private bool _doNotRequireXmlDocCommentPrefix;

		private static readonly XmlTextTokenSyntax s_xmlAmpToken;

		private static readonly XmlTextTokenSyntax s_xmlAposToken;

		private static readonly XmlTextTokenSyntax s_xmlGtToken;

		private static readonly XmlTextTokenSyntax s_xmlLtToken;

		private static readonly XmlTextTokenSyntax s_xmlQuotToken;

		private static readonly XmlTextTokenSyntax s_docCommentCrLfToken;

		internal VisualBasicParseOptions Options => _options;

		internal SyntaxToken LastToken
		{
			get
			{
				int count = _tokens.Count;
				if (count > 0)
				{
					return _tokens[count - 1].InnerTokenObject;
				}
				if (_currentToken.InnerTokenObject != null)
				{
					return _currentToken.InnerTokenObject;
				}
				return _prevToken.InnerTokenObject;
			}
		}

		internal SyntaxToken PrevToken => _prevToken.InnerTokenObject;

		internal bool IsScanningXmlDoc
		{
			get
			{
				return _IsScanningXmlDoc;
			}
			private set
			{
				_IsScanningXmlDoc = value;
			}
		}

		private bool ShouldReportXmlError
		{
			get
			{
				if (_IsScanningXmlDoc)
				{
					return _options.DocumentationMode == DocumentationMode.Diagnose;
				}
				return true;
			}
		}

		private bool TryScanDirective(SyntaxListBuilder tList)
		{
			if (CanGet() && SyntaxFacts.IsWhitespace(Peek()))
			{
				VisualBasicSyntaxNode item = ScanWhitespace();
				tList.Add(item);
			}
			RestorePoint restorePoint = CreateRestorePoint();
			_isScanningDirective = true;
			GetNextTokenInState(ScannerState.VB);
			DirectiveTriviaSyntax directiveTriviaSyntax = GetCurrentSyntaxNode() as DirectiveTriviaSyntax;
			if (directiveTriviaSyntax != null)
			{
				MoveToNextSyntaxNodeInTrivia();
				GetNextTokenInState(ScannerState.VB);
			}
			else
			{
				Parser parser = new Parser(this);
				directiveTriviaSyntax = parser.ParseConditionalCompilationStatement();
				directiveTriviaSyntax = parser.ConsumeStatementTerminatorAfterDirective(ref directiveTriviaSyntax);
			}
			ProcessDirective(directiveTriviaSyntax, tList);
			ResetLineBufferOffset();
			restorePoint.RestoreTokens(includeLookAhead: true);
			_isScanningDirective = false;
			return true;
		}

		private void ProcessDirective(DirectiveTriviaSyntax directiveTrivia, SyntaxListBuilder tList)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> list = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			DirectiveTriviaSyntax statement = directiveTrivia;
			ImmutableStack<ConditionalState> conditionalStack = (_scannerPreprocessorState = ApplyDirective(_scannerPreprocessorState, ref statement)).ConditionalStack;
			if (conditionalStack.Count() != 0 && conditionalStack.Peek().BranchTaken != ConditionalState.BranchTakenState.Taken)
			{
				list = SkipConditionalCompilationSection();
			}
			if (statement != directiveTrivia)
			{
				directiveTrivia = statement;
			}
			tList.Add(directiveTrivia);
			if (list.Node != null)
			{
				tList.AddRange(list);
			}
		}

		protected static PreprocessorState ApplyDirectives(PreprocessorState preprocessorState, VisualBasicSyntaxNode node)
		{
			if (node.ContainsDirectives)
			{
				preprocessorState = ApplyDirectivesRecursive(preprocessorState, node);
			}
			return preprocessorState;
		}

		private static PreprocessorState ApplyDirectivesRecursive(PreprocessorState preprocessorState, GreenNode node)
		{
			if (node is DirectiveTriviaSyntax directiveTriviaSyntax)
			{
				DirectiveTriviaSyntax statement = directiveTriviaSyntax;
				preprocessorState = ApplyDirective(preprocessorState, ref statement);
				return preprocessorState;
			}
			int slotCount = node.SlotCount;
			if (slotCount > 0)
			{
				int num = slotCount - 1;
				for (int i = 0; i <= num; i++)
				{
					GreenNode slot = node.GetSlot(i);
					if (slot != null && slot.ContainsDirectives)
					{
						preprocessorState = ApplyDirectivesRecursive(preprocessorState, slot);
					}
				}
				return preprocessorState;
			}
			SyntaxToken obj = (SyntaxToken)node;
			GreenNode leadingTrivia = obj.GetLeadingTrivia();
			if (leadingTrivia != null && leadingTrivia.ContainsDirectives)
			{
				preprocessorState = ApplyDirectivesRecursive(preprocessorState, leadingTrivia);
			}
			leadingTrivia = obj.GetTrailingTrivia();
			if (leadingTrivia != null && leadingTrivia.ContainsDirectives)
			{
				preprocessorState = ApplyDirectivesRecursive(preprocessorState, leadingTrivia);
			}
			return preprocessorState;
		}

		internal static PreprocessorState ApplyDirective(PreprocessorState preprocessorState, ref DirectiveTriviaSyntax statement)
		{
			switch (statement.Kind)
			{
			case SyntaxKind.ConstDirectiveTrivia:
			{
				ImmutableStack<ConditionalState> conditionalStack = preprocessorState.ConditionalStack;
				if (conditionalStack.Count() == 0 || conditionalStack.Peek().BranchTaken == ConditionalState.BranchTakenState.Taken)
				{
					preprocessorState = preprocessorState.InterpretConstDirective(ref statement);
				}
				break;
			}
			case SyntaxKind.IfDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretIfDirective(ref statement);
				break;
			case SyntaxKind.ElseIfDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretElseIfDirective(ref statement);
				break;
			case SyntaxKind.ElseDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretElseDirective(ref statement);
				break;
			case SyntaxKind.EndIfDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretEndIfDirective(ref statement);
				break;
			case SyntaxKind.RegionDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretRegionDirective(ref statement);
				break;
			case SyntaxKind.EndRegionDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretEndRegionDirective(ref statement);
				break;
			case SyntaxKind.ExternalSourceDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretExternalSourceDirective(ref statement);
				break;
			case SyntaxKind.EndExternalSourceDirectiveTrivia:
				preprocessorState = preprocessorState.InterpretEndExternalSourceDirective(ref statement);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(statement.Kind);
			case SyntaxKind.ExternalChecksumDirectiveTrivia:
			case SyntaxKind.EnableWarningDirectiveTrivia:
			case SyntaxKind.DisableWarningDirectiveTrivia:
			case SyntaxKind.ReferenceDirectiveTrivia:
			case SyntaxKind.BadDirectiveTrivia:
				break;
			}
			return preprocessorState;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> SkipConditionalCompilationSection()
		{
			int num = 0;
			int num2 = -1;
			int num3 = 0;
			while (true)
			{
				TextSpan textSpan = SkipToNextConditionalLine();
				if (num2 < 0)
				{
					num2 = textSpan.Start;
				}
				num3 += textSpan.Length;
				SyntaxToken currentToken = GetCurrentToken();
				switch (currentToken.Kind)
				{
				case SyntaxKind.HashToken:
				{
					SyntaxKind kind2 = PeekToken(1, ScannerState.VB).Kind;
					SyntaxToken t = PeekToken(2, ScannerState.VB);
					if (num == 0 && ((kind2 == SyntaxKind.EndKeyword && !IsContextualKeyword(t, SyntaxKind.ExternalSourceKeyword, SyntaxKind.RegionKeyword)) || kind2 == SyntaxKind.EndIfKeyword || kind2 == SyntaxKind.ElseIfKeyword || kind2 == SyntaxKind.ElseKeyword))
					{
						break;
					}
					if (kind2 == SyntaxKind.EndIfKeyword || (kind2 == SyntaxKind.EndKeyword && !IsContextualKeyword(t, SyntaxKind.ExternalSourceKeyword, SyntaxKind.RegionKeyword)))
					{
						num--;
					}
					else if (kind2 == SyntaxKind.IfKeyword)
					{
						num++;
					}
					if (num >= 0)
					{
						num3 += GetCurrentToken().FullWidth;
						GetNextTokenInState(ScannerState.VB);
						if (kind2 == SyntaxKind.StatementTerminatorToken || kind2 == SyntaxKind.ColonToken)
						{
							GetNextTokenInState(ScannerState.VB);
						}
						continue;
					}
					break;
				}
				case SyntaxKind.BadToken:
				case SyntaxKind.DateLiteralToken:
				{
					num3 += GetCurrentToken().FullWidth;
					GetNextTokenInState(ScannerState.VB);
					SyntaxKind kind = GetCurrentToken().Kind;
					if (kind == SyntaxKind.StatementTerminatorToken || kind == SyntaxKind.ColonToken)
					{
						GetNextTokenInState(ScannerState.VB);
					}
					continue;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(currentToken.Kind);
				case SyntaxKind.EndOfFileToken:
					break;
				}
				break;
			}
			return (num3 <= 0) ? new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(null) : new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(GetDisabledTextAt(new TextSpan(num2, num3)));
		}

		internal PunctuationSyntax RecoverFromMissingConditionalEnds(PunctuationSyntax eof, out ArrayBuilder<IfDirectiveTriviaSyntax> notClosedIfDirectives, out ArrayBuilder<RegionDirectiveTriviaSyntax> notClosedRegionDirectives, out bool haveRegionDirectives, out ExternalSourceDirectiveTriviaSyntax notClosedExternalSourceDirective)
		{
			notClosedIfDirectives = null;
			notClosedRegionDirectives = null;
			if (_scannerPreprocessorState.ConditionalStack.Count() > 0)
			{
				ImmutableStack<ConditionalState>.Enumerator enumerator = _scannerPreprocessorState.ConditionalStack.GetEnumerator();
				while (enumerator.MoveNext())
				{
					IfDirectiveTriviaSyntax ifDirective = enumerator.Current.IfDirective;
					if (ifDirective != null)
					{
						if (notClosedIfDirectives == null)
						{
							notClosedIfDirectives = ArrayBuilder<IfDirectiveTriviaSyntax>.GetInstance();
						}
						notClosedIfDirectives.Add(ifDirective);
					}
				}
				if (notClosedIfDirectives == null)
				{
					eof = Parser.ReportSyntaxError(eof, ERRID.ERR_LbExpectedEndIf);
				}
			}
			if (_scannerPreprocessorState.RegionDirectiveStack.Count() > 0)
			{
				notClosedRegionDirectives = ArrayBuilder<RegionDirectiveTriviaSyntax>.GetInstance();
				notClosedRegionDirectives.AddRange(_scannerPreprocessorState.RegionDirectiveStack);
			}
			haveRegionDirectives = _scannerPreprocessorState.HaveSeenRegionDirectives;
			notClosedExternalSourceDirective = _scannerPreprocessorState.ExternalSourceDirective;
			return eof;
		}

		static Scanner()
		{
			s_charProperties = new ushort[384]
			{
				512, 512, 512, 512, 512, 512, 512, 512, 512, 1,
				128, 512, 512, 64, 512, 512, 512, 512, 512, 512,
				512, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				512, 512, 1, 512, 512, 512, 8, 8, 8, 512,
				16, 16, 32, 32, 16, 32, 16, 32, 256, 256,
				256, 256, 256, 256, 256, 256, 256, 256, 512, 512,
				512, 512, 512, 16, 8, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 4, 2, 2, 2, 2, 2, 2, 2,
				2, 512, 32, 512, 32, 4, 512, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 4, 2, 2, 2, 2, 2,
				2, 2, 2, 16, 512, 16, 512, 512, 512, 512,
				512, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				512, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				512, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				512, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				2, 512, 512, 512, 512, 512, 512, 512, 512, 512,
				512, 2, 512, 512, 512, 512, 2, 512, 512, 512,
				512, 512, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 512, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 512, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 2, 2, 2
			};
			s_scanNoTriviaFunc = () => default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			s_keywordKindFactory = (string spelling) => KeywordTable.TokenOfString(spelling);
			s_keywordsObjsPool = CachingIdentityFactory<string, SyntaxKind>.CreatePool(512, s_keywordKindFactory);
			s_idTablePool = new ObjectPool<CachingFactory<TokenParts, IdentifierTokenSyntax>>(() => new CachingFactory<TokenParts, IdentifierTokenSyntax>(512, null, s_tokenKeyHasher, s_tokenKeyEquality));
			s_kwTablePool = new ObjectPool<CachingFactory<TokenParts, KeywordSyntax>>(() => new CachingFactory<TokenParts, KeywordSyntax>(512, null, s_tokenKeyHasher, s_tokenKeyEquality));
			s_punctTablePool = new ObjectPool<CachingFactory<TokenParts, PunctuationSyntax>>(() => new CachingFactory<TokenParts, PunctuationSyntax>(512, null, s_tokenKeyHasher, s_tokenKeyEquality));
			s_literalTablePool = new ObjectPool<CachingFactory<TokenParts, SyntaxToken>>(() => new CachingFactory<TokenParts, SyntaxToken>(512, null, s_tokenKeyHasher, s_tokenKeyEquality));
			s_wslTablePool = new ObjectPool<CachingFactory<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>>>(() => new CachingFactory<SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>>(512, s_wsListFactory, s_wsListKeyHasher, s_wsListKeyEquality));
			s_wsTablePool = new ObjectPool<CachingFactory<TriviaKey, SyntaxTrivia>>(() => CreateWsTable());
			s_conflictMarkerLength = "<<<<<<<".Length;
			s_triviaKeyHasher = (TriviaKey key) => RuntimeHelpers.GetHashCode(key.spelling) ^ (int)key.kind;
			s_triviaKeyEquality = (TriviaKey key, SyntaxTrivia value) => (object)key.spelling == value.Text && key.kind == value.Kind;
			s_singleSpaceWhitespaceTrivia = SyntaxFactory.WhitespaceTrivia(" ");
			s_fourSpacesWhitespaceTrivia = SyntaxFactory.WhitespaceTrivia("    ");
			s_eightSpacesWhitespaceTrivia = SyntaxFactory.WhitespaceTrivia("        ");
			s_twelveSpacesWhitespaceTrivia = SyntaxFactory.WhitespaceTrivia("            ");
			s_sixteenSpacesWhitespaceTrivia = SyntaxFactory.WhitespaceTrivia("                ");
			s_wsListKeyHasher = delegate(SyntaxListBuilder builder)
			{
				int num3 = 0;
				int num4 = builder.Count - 1;
				for (int j = 0; j <= num4; j++)
				{
					GreenNode o = builder[j];
					num3 = (num3 << 1) ^ RuntimeHelpers.GetHashCode(o);
				}
				return num3;
			};
			s_wsListKeyEquality = delegate(SyntaxListBuilder builder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> list)
			{
				if (builder.Count != list.Count)
				{
					return false;
				}
				int num2 = builder.Count - 1;
				for (int i = 0; i <= num2; i++)
				{
					if (builder[i] != list.ItemUntyped(i))
					{
						return false;
					}
				}
				return true;
			};
			s_wsListFactory = (SyntaxListBuilder builder) => builder.ToList<VisualBasicSyntaxNode>();
			s_tokenKeyHasher = delegate(TokenParts key)
			{
				int num = RuntimeHelpers.GetHashCode(key.spelling);
				GreenNode pTrivia = key.pTrivia;
				if (pTrivia != null)
				{
					num ^= RuntimeHelpers.GetHashCode(pTrivia) << 1;
				}
				pTrivia = key.fTrivia;
				if (pTrivia != null)
				{
					num ^= RuntimeHelpers.GetHashCode(pTrivia);
				}
				return num;
			};
			s_tokenKeyEquality = (TokenParts x, SyntaxToken y) => (y != null && (object)x.spelling == y.Text && x.fTrivia == y.GetTrailingTrivia() && x.pTrivia == y.GetLeadingTrivia()) ? true : false;
			s_crLfTrivia = SyntaxFactory.EndOfLineTrivia("\r\n");
			s_xmlAmpToken = SyntaxFactory.XmlEntityLiteralToken("&amp;", "&", null, null);
			s_xmlAposToken = SyntaxFactory.XmlEntityLiteralToken("&apos;", "'", null, null);
			s_xmlGtToken = SyntaxFactory.XmlEntityLiteralToken("&gt;", ">", null, null);
			s_xmlLtToken = SyntaxFactory.XmlEntityLiteralToken("&lt;", "<", null, null);
			s_xmlQuotToken = SyntaxFactory.XmlEntityLiteralToken("&quot;", "\"", null, null);
			s_docCommentCrLfToken = SyntaxFactory.DocumentationCommentLineBreakToken("\r\n", "\n", null, null);
		}

		public QuickScanResult QuickScanToken(bool allowLeadingMultilineTrivia)
		{
			AccumulatorState accumulatorState = (allowLeadingMultilineTrivia ? AccumulatorState.InitialAllowLeadingMultilineTrivia : AccumulatorState.Initial);
			int lineBufferOffset = _lineBufferOffset;
			Page page = _curPage;
			if (page == null || page._pageStart != (lineBufferOffset & -2048))
			{
				page = GetPage(lineBufferOffset);
			}
			char[] arr = page._arr;
			char[] chars = arr;
			int i = _lineBufferOffset & 0x7FF;
			int num = i;
			int val = i + Math.Min(42, _bufferLen - lineBufferOffset);
			val = Math.Min(val, arr.Length);
			int num2 = -2128831035;
			byte b = 0;
			int num3;
			for (num3 = 0; i < val; i++, num2 = (num2 ^ num3) * 16777619)
			{
				num3 = arr[i];
				if (num3 >= 384)
				{
					break;
				}
				ushort num4 = s_charProperties[num3];
				switch (accumulatorState)
				{
				case AccumulatorState.InitialAllowLeadingMultilineTrivia:
					switch (num4)
					{
					case 2:
						accumulatorState = AccumulatorState.Ident;
						continue;
					case 16:
						accumulatorState = AccumulatorState.Punctuation;
						continue;
					case 32:
						accumulatorState = AccumulatorState.CompoundPunctStart;
						continue;
					}
					if ((num4 & 0xC1u) != 0)
					{
						continue;
					}
					accumulatorState = AccumulatorState.Bad;
					break;
				case AccumulatorState.Initial:
					switch (num4)
					{
					case 2:
						accumulatorState = AccumulatorState.Ident;
						continue;
					case 16:
						accumulatorState = AccumulatorState.Punctuation;
						continue;
					case 32:
						accumulatorState = AccumulatorState.CompoundPunctStart;
						continue;
					case 1:
						continue;
					}
					accumulatorState = AccumulatorState.Bad;
					break;
				case AccumulatorState.Ident:
					if ((num4 & 0x106u) != 0)
					{
						continue;
					}
					switch (num4)
					{
					case 1:
						accumulatorState = AccumulatorState.FollowingWhite;
						continue;
					case 64:
						accumulatorState = AccumulatorState.CR;
						continue;
					case 128:
						b = 1;
						accumulatorState = AccumulatorState.Done;
						break;
					case 8:
						accumulatorState = AccumulatorState.TypeChar;
						continue;
					case 16:
						accumulatorState = AccumulatorState.Done;
						break;
					default:
						accumulatorState = AccumulatorState.Bad;
						break;
					}
					break;
				case AccumulatorState.TypeChar:
					switch (num4)
					{
					case 1:
						accumulatorState = AccumulatorState.FollowingWhite;
						continue;
					case 64:
						accumulatorState = AccumulatorState.CR;
						continue;
					case 128:
						b = 1;
						accumulatorState = AccumulatorState.Done;
						break;
					default:
						accumulatorState = (((num4 & 0x118) == 0) ? AccumulatorState.Bad : AccumulatorState.Done);
						break;
					}
					break;
				case AccumulatorState.FollowingWhite:
					switch (num4)
					{
					case 64:
						accumulatorState = AccumulatorState.CR;
						continue;
					case 128:
						b = 1;
						accumulatorState = AccumulatorState.Done;
						break;
					default:
						accumulatorState = (((num4 & 0x204) == 0) ? AccumulatorState.Done : AccumulatorState.Bad);
						break;
					case 1:
						continue;
					}
					break;
				case AccumulatorState.Punctuation:
					switch (num4)
					{
					case 1:
						accumulatorState = AccumulatorState.FollowingWhite;
						continue;
					case 64:
						accumulatorState = AccumulatorState.CR;
						continue;
					case 128:
						b = 1;
						accumulatorState = AccumulatorState.Done;
						break;
					default:
						accumulatorState = (((num4 & 0x12) == 0) ? AccumulatorState.Bad : AccumulatorState.Done);
						break;
					}
					break;
				case AccumulatorState.CompoundPunctStart:
					if (num4 == 1)
					{
						continue;
					}
					accumulatorState = (((num4 & 0x102) == 0) ? AccumulatorState.Bad : AccumulatorState.Done);
					break;
				case AccumulatorState.CR:
					if (num4 == 128)
					{
						b = 2;
						accumulatorState = AccumulatorState.Done;
					}
					else
					{
						accumulatorState = AccumulatorState.Bad;
					}
					break;
				default:
					continue;
				}
				break;
			}
			QuickScanResult result;
			if (accumulatorState == AccumulatorState.Done && (b == 0 || !_IsScanningXmlDoc))
			{
				if (b != 0)
				{
					i++;
					num2 = (num2 ^ num3) * 16777619;
				}
				result = new QuickScanResult(num, i - num, chars, num2, b);
			}
			else
			{
				result = default(QuickScanResult);
			}
			return result;
		}

		private StringBuilder GetScratch()
		{
			return _sb;
		}

		internal Scanner(SourceText textToScan, VisualBasicParseOptions options, bool isScanningForExpressionCompiler = false)
		{
			_isScanningDirective = false;
			_scanSingleLineTriviaFunc = ScanSingleLineTrivia;
			_sbPooled = PooledStringBuilder.GetInstance();
			_sb = _sbPooled.Builder;
			_triviaListPool = new SyntaxListPool();
			_stringTable = StringTable.GetInstance();
			_quickTokenTable = TextKeyedCache<SyntaxToken>.GetInstance();
			_KeywordsObjs = s_keywordsObjsPool.Allocate();
			_idTable = s_idTablePool.Allocate();
			_kwTable = s_kwTablePool.Allocate();
			_punctTable = s_punctTablePool.Allocate();
			_literalTable = s_literalTablePool.Allocate();
			_wslTable = s_wslTablePool.Allocate();
			_wsTable = s_wsTablePool.Allocate();
			_pages = new Page[4];
			_simpleEof = SyntaxFactory.Token(null, SyntaxKind.EndOfFileToken, null, string.Empty);
			_tokens = new List<ScannerToken>();
			_IsScanningXmlDoc = false;
			_isStartingFirstXmlDocLine = false;
			_doNotRequireXmlDocCommentPrefix = false;
			_lineBufferOffset = 0;
			_buffer = textToScan;
			_bufferLen = textToScan.Length;
			_curPage = GetPage(0);
			_options = options;
			_scannerPreprocessorState = new PreprocessorState(GetPreprocessorConstants(options));
			_isScanningForExpressionCompiler = isScanningForExpressionCompiler;
		}

		internal void Dispose()
		{
			if (!_isDisposed)
			{
				_isDisposed = true;
				_KeywordsObjs.Free();
				_quickTokenTable.Free();
				_stringTable.Free();
				_sbPooled.Free();
				s_idTablePool.Free(_idTable);
				s_kwTablePool.Free(_kwTable);
				s_punctTablePool.Free(_punctTable);
				s_literalTablePool.Free(_literalTable);
				s_wslTablePool.Free(_wslTable);
				s_wsTablePool.Free(_wsTable);
				Page[] pages = _pages;
				for (int i = 0; i < pages.Length; i = checked(i + 1))
				{
					pages[i]?.Free();
				}
				Array.Clear(_pages, 0, _pages.Length);
			}
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in Dispose
			this.Dispose();
		}

		internal static ImmutableDictionary<string, CConst> GetPreprocessorConstants(VisualBasicParseOptions options)
		{
			if (options.PreprocessorSymbols.IsDefaultOrEmpty)
			{
				return ImmutableDictionary<string, CConst>.Empty;
			}
			ImmutableDictionary<string, CConst>.Builder builder = ImmutableDictionary.CreateBuilder<string, CConst>(CaseInsensitiveComparison.Comparer);
			ImmutableArray<KeyValuePair<string, object>>.Enumerator enumerator = options.PreprocessorSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, object> current = enumerator.Current;
				builder[current.Key] = CConst.CreateChecked(RuntimeHelpers.GetObjectValue(current.Value));
			}
			return builder.ToImmutable();
		}

		private SyntaxToken GetNextToken(bool allowLeadingMultilineTrivia = false)
		{
			QuickScanResult quickScanResult = QuickScanToken(allowLeadingMultilineTrivia);
			if (quickScanResult.Succeeded)
			{
				SyntaxToken syntaxToken = _quickTokenTable.FindItem(quickScanResult.Chars, quickScanResult.Start, quickScanResult.Length, quickScanResult.HashCode);
				if (syntaxToken != null)
				{
					AdvanceChar(quickScanResult.Length);
					if (quickScanResult.TerminatorLength != 0)
					{
						_endOfTerminatorTrivia = _lineBufferOffset;
						_lineBufferOffset -= quickScanResult.TerminatorLength;
					}
					return syntaxToken;
				}
			}
			SyntaxToken syntaxToken2 = ScanNextToken(allowLeadingMultilineTrivia);
			if (quickScanResult.Succeeded)
			{
				_quickTokenTable.AddItem(quickScanResult.Chars, quickScanResult.Start, quickScanResult.Length, quickScanResult.HashCode, syntaxToken2);
			}
			return syntaxToken2;
		}

		private SyntaxToken ScanNextToken(bool allowLeadingMultilineTrivia)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList;
			if (allowLeadingMultilineTrivia)
			{
				syntaxList = ScanMultilineTrivia();
			}
			else
			{
				syntaxList = ScanLeadingTrivia();
				if (PeekStartComment(0) > 0)
				{
					return MakeEmptyToken(syntaxList);
				}
			}
			SyntaxToken syntaxToken = TryScanToken(syntaxList);
			if (syntaxToken == null)
			{
				syntaxToken = ScanNextCharAsToken(syntaxList);
			}
			if (_lineBufferOffset > _endOfTerminatorTrivia)
			{
				_endOfTerminatorTrivia = _lineBufferOffset;
			}
			return syntaxToken;
		}

		private SyntaxToken ScanNextCharAsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> leadingTrivia)
		{
			if (!CanGet())
			{
				return MakeEofToken(leadingTrivia);
			}
			_badTokenCount++;
			if (_badTokenCount < 200)
			{
				int length = ((!SyntaxFacts.IsHighSurrogate(Peek()) || !CanGet(1) || !SyntaxFacts.IsLowSurrogate(Peek(1))) ? 1 : 2);
				return MakeBadToken(leadingTrivia, length, ERRID.ERR_IllegalChar);
			}
			return MakeBadToken(leadingTrivia, RemainingLength(), ERRID.ERR_IllegalChar);
		}

		public TextSpan SkipToNextConditionalLine()
		{
			ResetLineBufferOffset();
			int lineBufferOffset = _lineBufferOffset;
			_ = PrevToken;
			if (!IsAtNewLine() || (PrevToken != null && SyntaxExtensions.EndsWithEndOfLineOrColonTrivia(PrevToken)))
			{
				EatThroughLine();
			}
			int lineBufferOffset2 = _lineBufferOffset;
			while (CanGet())
			{
				char c = Peek();
				switch (c)
				{
				case '\n':
				case '\r':
					EatThroughLineBreak(c);
					lineBufferOffset2 = _lineBufferOffset;
					continue;
				case '\t':
				case ' ':
					EatWhitespace();
					continue;
				case '\'':
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
					EatThroughLine();
					lineBufferOffset2 = _lineBufferOffset;
					continue;
				default:
					if (SyntaxFacts.IsWhitespace(c))
					{
						EatWhitespace();
					}
					else if (SyntaxFacts.IsNewLine(c))
					{
						EatThroughLineBreak(c);
						lineBufferOffset2 = _lineBufferOffset;
					}
					else
					{
						EatThroughLine();
						lineBufferOffset2 = _lineBufferOffset;
					}
					continue;
				case '#':
				case '＃':
					break;
				}
				break;
			}
			_lineBufferOffset = lineBufferOffset2;
			ResetTokens();
			return TextSpan.FromBounds(lineBufferOffset, lineBufferOffset2);
		}

		private void EatThroughLine()
		{
			while (CanGet())
			{
				char c = Peek();
				if (SyntaxFacts.IsNewLine(c))
				{
					EatThroughLineBreak(c);
					break;
				}
				AdvanceChar();
			}
		}

		internal SyntaxTrivia GetDisabledTextAt(TextSpan span)
		{
			if (span.Start >= 0 && span.End <= _bufferLen)
			{
				return SyntaxFactory.DisabledTextTrivia(GetTextNotInterned(span.Start, span.Length));
			}
			throw new ArgumentOutOfRangeException("span");
		}

		internal string GetScratchTextInterned(StringBuilder sb)
		{
			string result = _stringTable.Add(sb);
			sb.Clear();
			return result;
		}

		internal static string GetScratchText(StringBuilder sb)
		{
			string result = ((sb.Length != 1 || sb[0] != ' ') ? sb.ToString() : " ");
			sb.Clear();
			return result;
		}

		private static string GetScratchText(StringBuilder sb, string text)
		{
			string result = ((!StringTable.TextEquals(text, sb)) ? sb.ToString() : text);
			sb.Clear();
			return result;
		}

		internal string Intern(string s, int start, int length)
		{
			return _stringTable.Add(s, start, length);
		}

		internal string Intern(char[] s, int start, int length)
		{
			return _stringTable.Add(s, start, length);
		}

		internal string Intern(char ch)
		{
			return _stringTable.Add(ch);
		}

		internal string Intern(char[] arr)
		{
			return _stringTable.Add(new string(arr));
		}

		private bool NextAre(string chars)
		{
			return NextAre(0, chars);
		}

		private bool NextAre(int offset, string chars)
		{
			int length = chars.Length;
			if (!CanGet(offset + length - 1))
			{
				return false;
			}
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (chars[i] != Peek(offset + i))
				{
					return false;
				}
			}
			return true;
		}

		private bool NextIs(int offset, char c)
		{
			if (CanGet(offset))
			{
				return Peek(offset) == c;
			}
			return false;
		}

		private bool CanGet()
		{
			return _lineBufferOffset < _bufferLen;
		}

		private bool CanGet(int num)
		{
			return _lineBufferOffset + num < _bufferLen;
		}

		private int RemainingLength()
		{
			return _bufferLen - _lineBufferOffset;
		}

		private string GetText(int length)
		{
			if (length == 1)
			{
				return GetNextChar();
			}
			string text = GetText(_lineBufferOffset, length);
			AdvanceChar(length);
			return text;
		}

		private string GetTextNotInterned(int length)
		{
			if (length == 1)
			{
				return GetNextChar();
			}
			string textNotInterned = GetTextNotInterned(_lineBufferOffset, length);
			AdvanceChar(length);
			return textNotInterned;
		}

		private void AdvanceChar(int howFar = 1)
		{
			_lineBufferOffset += howFar;
		}

		private string GetNextChar()
		{
			string @char = GetChar();
			_lineBufferOffset++;
			return @char;
		}

		private void EatThroughLineBreak(char StartCharacter)
		{
			AdvanceChar(LengthOfLineBreak(StartCharacter));
		}

		private int SkipLineBreak(char StartCharacter, int index)
		{
			return index + LengthOfLineBreak(StartCharacter, index);
		}

		private int LengthOfLineBreak(char StartCharacter, int here = 0)
		{
			if (StartCharacter == '\r' && NextIs(here + 1, '\n'))
			{
				return 2;
			}
			return 1;
		}

		private SyntaxToken ScanNewlineAsStatementTerminator(char startCharacter, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			if (_lineBufferOffset < _endOfTerminatorTrivia)
			{
				int width = LengthOfLineBreak(startCharacter);
				return MakeStatementTerminatorToken(precedingTrivia, width);
			}
			return MakeEmptyToken(precedingTrivia);
		}

		private SyntaxToken ScanColonAsStatementTerminator(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			if (_lineBufferOffset < _endOfTerminatorTrivia)
			{
				return MakeColonToken(precedingTrivia, charIsFullWidth);
			}
			return MakeEmptyToken(precedingTrivia);
		}

		private SyntaxTrivia ScanNewlineAsTrivia(char StartCharacter)
		{
			if (LengthOfLineBreak(StartCharacter) == 2)
			{
				return MakeEndOfLineTriviaCRLF();
			}
			return MakeEndOfLineTrivia(GetNextChar());
		}

		private bool TryGet(int num, ref char ch)
		{
			if (CanGet(num))
			{
				ch = Peek(num);
				return true;
			}
			return false;
		}

		private bool ScanLineContinuation(SyntaxListBuilder tList)
		{
			char ch = '\0';
			if (!TryGet(0, ref ch))
			{
				return false;
			}
			if (!IsAfterWhitespace())
			{
				return false;
			}
			if (!SyntaxFacts.IsUnderscore(ch))
			{
				return false;
			}
			int whitespaceLength = GetWhitespaceLength(1);
			TryGet(whitespaceLength, ref ch);
			bool flag = SyntaxFacts.IsSingleQuote(ch);
			bool flag2 = SyntaxFacts.IsNewLine(ch);
			if (!flag && !flag2 && CanGet(whitespaceLength))
			{
				return false;
			}
			tList.Add(MakeLineContinuationTrivia(GetText(1)));
			if (whitespaceLength > 1)
			{
				tList.Add(MakeWhiteSpaceTrivia(GetText(whitespaceLength - 1)));
			}
			if (flag)
			{
				SyntaxTrivia syntaxTrivia = ScanComment();
				if (!CheckFeatureAvailability(Feature.CommentsAfterLineContinuation))
				{
					syntaxTrivia = SyntaxExtensions.WithDiagnostics(syntaxTrivia, ErrorFactory.ErrorInfo(ERRID.ERR_CommentsAfterLineContinuationNotAvailable1, new VisualBasicRequiredLanguageVersion(FeatureExtensions.GetLanguageVersion(Feature.CommentsAfterLineContinuation))));
				}
				tList.Add(syntaxTrivia);
				if (CanGet())
				{
					ch = Peek();
					flag2 = SyntaxFacts.IsNewLine(ch);
				}
			}
			if (flag2)
			{
				int num = SkipLineBreak(ch, 0);
				whitespaceLength = GetWhitespaceLength(num);
				int num2 = whitespaceLength - num;
				if (PeekStartComment(whitespaceLength) == 0 && CanGet(whitespaceLength) && !SyntaxFacts.IsNewLine(Peek(whitespaceLength)))
				{
					tList.Add(MakeEndOfLineTrivia(GetText(num)));
					if (num2 > 0)
					{
						tList.Add(MakeWhiteSpaceTrivia(GetText(num2)));
					}
				}
			}
			return true;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanMultilineTrivia()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> result;
			if (!CanGet())
			{
				result = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			}
			else
			{
				char c = Peek();
				if (c <= ':' || c > '~' || c == '\'' || c == '_' || c == 'R' || c == 'r' || c == '<' || c == '=' || c == '>')
				{
					SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
					while (TryScanSinglePieceOfMultilineTrivia(syntaxListBuilder))
					{
					}
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> result2 = MakeTriviaArray(syntaxListBuilder);
					_triviaListPool.Free(syntaxListBuilder);
					return result2;
				}
				result = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			}
			return result;
		}

		private bool TryScanSinglePieceOfMultilineTrivia(SyntaxListBuilder tList)
		{
			if (CanGet())
			{
				bool flag = IsAtNewLine();
				if (flag)
				{
					if (StartsXmlDoc(0))
					{
						return TryScanXmlDocComment(tList);
					}
					if (StartsDirective(0))
					{
						return TryScanDirective(tList);
					}
					if (IsConflictMarkerTrivia())
					{
						ScanConflictMarker(tList);
						return true;
					}
				}
				char c = Peek();
				if (SyntaxFacts.IsWhitespace(c))
				{
					int whitespaceLength = GetWhitespaceLength(1);
					if (flag)
					{
						if (StartsXmlDoc(whitespaceLength))
						{
							return TryScanXmlDocComment(tList);
						}
						if (StartsDirective(whitespaceLength))
						{
							return TryScanDirective(tList);
						}
					}
					tList.Add(MakeWhiteSpaceTrivia(GetText(whitespaceLength)));
					return true;
				}
				if (SyntaxFacts.IsNewLine(c))
				{
					tList.Add(ScanNewlineAsTrivia(c));
					return true;
				}
				if (SyntaxFacts.IsUnderscore(c))
				{
					return ScanLineContinuation(tList);
				}
				if (IsColonAndNotColonEquals(c, 0))
				{
					tList.Add(ScanColonAsTrivia());
					return true;
				}
				return ScanCommentIfAny(tList);
			}
			return false;
		}

		private bool IsConflictMarkerTrivia()
		{
			if (CanGet())
			{
				char c = Peek();
				if (c == '<' || c == '>' || c == '=')
				{
					int lineBufferOffset = _lineBufferOffset;
					SourceText buffer = _buffer;
					if (lineBufferOffset == 0 || SyntaxFacts.IsNewLine(buffer[lineBufferOffset - 1]))
					{
						char c2 = _buffer[lineBufferOffset];
						if (lineBufferOffset + s_conflictMarkerLength <= buffer.Length)
						{
							int num = s_conflictMarkerLength - 1;
							for (int i = 0; i <= num; i++)
							{
								if (buffer[lineBufferOffset + i] != c2)
								{
									return false;
								}
							}
							if (c2 == '=')
							{
								return true;
							}
							return lineBufferOffset + s_conflictMarkerLength < buffer.Length && buffer[lineBufferOffset + s_conflictMarkerLength] == ' ';
						}
					}
				}
			}
			return false;
		}

		private void ScanConflictMarker(SyntaxListBuilder tList)
		{
			char num = Peek();
			ScanConflictMarkerHeader(tList);
			ScanConflictMarkerEndOfLine(tList);
			if (num == '=')
			{
				ScanConflictMarkerDisabledText(tList);
			}
		}

		private void ScanConflictMarkerDisabledText(SyntaxListBuilder tList)
		{
			int lineBufferOffset = _lineBufferOffset;
			while (CanGet() && (Peek() != '>' || !IsConflictMarkerTrivia()))
			{
				AdvanceChar();
			}
			int num = _lineBufferOffset - lineBufferOffset;
			if (num > 0)
			{
				tList.Add(SyntaxFactory.DisabledTextTrivia(GetText(lineBufferOffset, num)));
			}
		}

		private void ScanConflictMarkerEndOfLine(SyntaxListBuilder tList)
		{
			int lineBufferOffset = _lineBufferOffset;
			while (CanGet() && SyntaxFacts.IsNewLine(Peek()))
			{
				AdvanceChar();
			}
			int num = _lineBufferOffset - lineBufferOffset;
			if (num > 0)
			{
				tList.Add(SyntaxFactory.EndOfLineTrivia(GetText(lineBufferOffset, num)));
			}
		}

		private void ScanConflictMarkerHeader(SyntaxListBuilder tList)
		{
			int lineBufferOffset = _lineBufferOffset;
			while (CanGet() && !SyntaxFacts.IsNewLine(Peek()))
			{
				AdvanceChar();
			}
			SyntaxTrivia syntaxTrivia = SyntaxFactory.ConflictMarkerTrivia(GetText(lineBufferOffset, _lineBufferOffset - lineBufferOffset));
			syntaxTrivia = (SyntaxTrivia)syntaxTrivia.SetDiagnostics(new DiagnosticInfo[1] { ErrorFactory.ErrorInfo(ERRID.ERR_Merge_conflict_marker_encountered) });
			tList.Add(syntaxTrivia);
		}

		private bool StartsXmlDoc(int here)
		{
			if (_options.DocumentationMode >= DocumentationMode.Parse && CanGet(here + 3) && SyntaxFacts.IsSingleQuote(Peek(here)) && SyntaxFacts.IsSingleQuote(Peek(here + 1)) && SyntaxFacts.IsSingleQuote(Peek(here + 2)))
			{
				return !SyntaxFacts.IsSingleQuote(Peek(here + 3));
			}
			return false;
		}

		private bool StartsDirective(int here)
		{
			if (CanGet(here))
			{
				return SyntaxFacts.IsHash(Peek(here));
			}
			return false;
		}

		private bool IsAtNewLine()
		{
			if (_lineBufferOffset != 0)
			{
				return SyntaxFacts.IsNewLine(Peek(-1));
			}
			return true;
		}

		private bool IsAfterWhitespace()
		{
			if (_lineBufferOffset == 0)
			{
				return true;
			}
			return SyntaxFacts.IsWhitespace(Peek(-1));
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanSingleLineTrivia()
		{
			SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
			ScanSingleLineTrivia(syntaxListBuilder);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> result = MakeTriviaArray(syntaxListBuilder);
			_triviaListPool.Free(syntaxListBuilder);
			return result;
		}

		private void ScanSingleLineTrivia(SyntaxListBuilder tList)
		{
			if (IsScanningXmlDoc)
			{
				ScanSingleLineTriviaInXmlDoc(tList);
				return;
			}
			ScanWhitespaceAndLineContinuations(tList);
			ScanCommentIfAny(tList);
			ScanTerminatorTrivia(tList);
		}

		private void ScanSingleLineTriviaInXmlDoc(SyntaxListBuilder tList)
		{
			if (!CanGet())
			{
				return;
			}
			char c = Peek();
			switch (c)
			{
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			{
				LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
				SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = _triviaListPool.Allocate<VisualBasicSyntaxNode>();
				if (!ScanXmlTriviaInXmlDoc(c, syntaxListBuilder))
				{
					_triviaListPool.Free(syntaxListBuilder);
					lineBufferAndEndOfTerminatorOffsets.Restore();
					break;
				}
				int num = syntaxListBuilder.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					tList.Add(syntaxListBuilder[i]);
				}
				_triviaListPool.Free(syntaxListBuilder);
				break;
			}
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanLeadingTrivia()
		{
			SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
			ScanWhitespaceAndLineContinuations(syntaxListBuilder);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> result = MakeTriviaArray(syntaxListBuilder);
			_triviaListPool.Free(syntaxListBuilder);
			return result;
		}

		private void ScanWhitespaceAndLineContinuations(SyntaxListBuilder tList)
		{
			if (CanGet() && SyntaxFacts.IsWhitespace(Peek()))
			{
				tList.Add(ScanWhitespace(1));
				while (ScanLineContinuation(tList))
				{
				}
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanSingleLineTrivia(bool includeFollowingBlankLines)
		{
			SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
			ScanSingleLineTrivia(syntaxListBuilder);
			if (includeFollowingBlankLines && IsBlankLine(syntaxListBuilder))
			{
				SyntaxListBuilder syntaxListBuilder2 = _triviaListPool.Allocate();
				LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets;
				while (true)
				{
					lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
					_lineBufferOffset = _endOfTerminatorTrivia;
					ScanSingleLineTrivia(syntaxListBuilder2);
					if (!IsBlankLine(syntaxListBuilder2))
					{
						break;
					}
					int num = syntaxListBuilder2.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						syntaxListBuilder.Add(syntaxListBuilder2[i]);
					}
					syntaxListBuilder2.Clear();
				}
				lineBufferAndEndOfTerminatorOffsets.Restore();
				_triviaListPool.Free(syntaxListBuilder2);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> syntaxList = syntaxListBuilder.ToList();
			_triviaListPool.Free(syntaxListBuilder);
			return syntaxList;
		}

		private static bool IsBlankLine(SyntaxListBuilder tList)
		{
			int count = tList.Count;
			if (count == 0 || tList[count - 1]!.RawKind != 730)
			{
				return false;
			}
			int num = count - 2;
			for (int i = 0; i <= num; i++)
			{
				if (tList[i]!.RawKind != 729)
				{
					return false;
				}
			}
			return true;
		}

		private void ScanTerminatorTrivia(SyntaxListBuilder tList)
		{
			if (!CanGet())
			{
				return;
			}
			char c = Peek();
			int lineBufferOffset = _lineBufferOffset;
			if (SyntaxFacts.IsNewLine(c))
			{
				tList.Add(ScanNewlineAsTrivia(c));
			}
			else if (IsColonAndNotColonEquals(c, 0))
			{
				tList.Add(ScanColonAsTrivia());
				while (true)
				{
					int whitespaceLength = GetWhitespaceLength(0);
					if (!CanGet(whitespaceLength))
					{
						break;
					}
					c = Peek(whitespaceLength);
					if (!IsColonAndNotColonEquals(c, whitespaceLength))
					{
						break;
					}
					if (whitespaceLength > 0)
					{
						tList.Add(MakeWhiteSpaceTrivia(GetText(whitespaceLength)));
					}
					lineBufferOffset = _lineBufferOffset;
					tList.Add(ScanColonAsTrivia());
				}
			}
			_endOfTerminatorTrivia = _lineBufferOffset;
			_lineBufferOffset = lineBufferOffset;
		}

		private bool ScanCommentIfAny(SyntaxListBuilder tList)
		{
			if (CanGet())
			{
				SyntaxTrivia syntaxTrivia = ScanComment();
				if (syntaxTrivia != null)
				{
					tList.Add(syntaxTrivia);
					return true;
				}
			}
			return false;
		}

		private int GetWhitespaceLength(int len)
		{
			while (CanGet(len) && SyntaxFacts.IsWhitespace(Peek(len)))
			{
				len++;
			}
			return len;
		}

		private int GetXmlWhitespaceLength(int len)
		{
			while (CanGet(len) && SyntaxFacts.IsXmlWhitespace(Peek(len)))
			{
				len++;
			}
			return len;
		}

		private VisualBasicSyntaxNode ScanWhitespace(int len = 0)
		{
			len = GetWhitespaceLength(len);
			if (len > 0)
			{
				return MakeWhiteSpaceTrivia(GetText(len));
			}
			return null;
		}

		private VisualBasicSyntaxNode ScanXmlWhitespace(int len = 0)
		{
			len = GetXmlWhitespaceLength(len);
			if (len > 0)
			{
				return MakeWhiteSpaceTrivia(GetText(len));
			}
			return null;
		}

		private void EatWhitespace()
		{
			AdvanceChar();
			while (CanGet() && SyntaxFacts.IsWhitespace(Peek()))
			{
				AdvanceChar();
			}
		}

		private int PeekStartComment(int i)
		{
			if (CanGet(i))
			{
				char c = Peek(i);
				if (SyntaxFacts.IsSingleQuote(c))
				{
					return 1;
				}
				if (SyntaxFacts.MatchOneOrAnotherOrFullwidth(c, 'R', 'r') && CanGet(i + 2) && SyntaxFacts.MatchOneOrAnotherOrFullwidth(Peek(i + 1), 'E', 'e') && SyntaxFacts.MatchOneOrAnotherOrFullwidth(Peek(i + 2), 'M', 'm'))
				{
					if (!CanGet(i + 3) || SyntaxFacts.IsNewLine(Peek(i + 3)))
					{
						return 3;
					}
					if (!SyntaxFacts.IsIdentifierPartCharacter(Peek(i + 3)))
					{
						return 4;
					}
				}
			}
			return 0;
		}

		private SyntaxTrivia ScanComment()
		{
			int i = PeekStartComment(0);
			if (i > 0)
			{
				bool flag = StartsXmlDoc(0);
				for (; CanGet(i) && !SyntaxFacts.IsNewLine(Peek(i)); i++)
				{
				}
				SyntaxTrivia syntaxTrivia = MakeCommentTrivia(GetTextNotInterned(i));
				if (flag && _options.DocumentationMode >= DocumentationMode.Diagnose)
				{
					syntaxTrivia = SyntaxExtensions.WithDiagnostics(syntaxTrivia, ErrorFactory.ErrorInfo(ERRID.WRN_XMLDocNotFirstOnLine));
				}
				return syntaxTrivia;
			}
			return null;
		}

		private bool IsColonAndNotColonEquals(char ch, int offset)
		{
			if (SyntaxFacts.IsColon(ch))
			{
				int index = offset + 1;
				return !TrySkipFollowingEquals(ref index);
			}
			return false;
		}

		private SyntaxTrivia ScanColonAsTrivia()
		{
			return MakeColonTrivia(GetText(1));
		}

		private SyntaxToken ScanTokenCommon(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, char ch, bool fullWidth)
		{
			int index = 1;
			switch (ch)
			{
			case '\n':
			case '\r':
				return ScanNewlineAsStatementTerminator(ch, precedingTrivia);
			case '\u0085':
			case '\u2028':
			case '\u2029':
				if (!fullWidth)
				{
					return ScanNewlineAsStatementTerminator(ch, precedingTrivia);
				}
				break;
			case '\t':
			case ' ':
			case '\'':
				return null;
			case '@':
				return MakeAtToken(precedingTrivia, fullWidth);
			case '(':
				return MakeOpenParenToken(precedingTrivia, fullWidth);
			case ')':
				return MakeCloseParenToken(precedingTrivia, fullWidth);
			case '{':
				return MakeOpenBraceToken(precedingTrivia, fullWidth);
			case '}':
				return MakeCloseBraceToken(precedingTrivia, fullWidth);
			case ',':
				return MakeCommaToken(precedingTrivia, fullWidth);
			case '#':
				return ScanDateLiteral(precedingTrivia) ?? MakeHashToken(precedingTrivia, fullWidth);
			case '&':
				if (CanGet(1) && SyntaxFacts.BeginsBaseLiteral(Peek(1)))
				{
					return ScanNumericLiteral(precedingTrivia);
				}
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeAmpersandEqualsToken(precedingTrivia, index);
				}
				return MakeAmpersandToken(precedingTrivia, fullWidth);
			case '=':
				return MakeEqualsToken(precedingTrivia, fullWidth);
			case '<':
				return ScanLeftAngleBracket(precedingTrivia, fullWidth, _scanSingleLineTriviaFunc);
			case '>':
				return ScanRightAngleBracket(precedingTrivia, fullWidth);
			case ':':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeColonEqualsToken(precedingTrivia, index);
				}
				return ScanColonAsStatementTerminator(precedingTrivia, fullWidth);
			case '+':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakePlusEqualsToken(precedingTrivia, index);
				}
				return MakePlusToken(precedingTrivia, fullWidth);
			case '-':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeMinusEqualsToken(precedingTrivia, index);
				}
				return MakeMinusToken(precedingTrivia, fullWidth);
			case '*':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeAsteriskEqualsToken(precedingTrivia, index);
				}
				return MakeAsteriskToken(precedingTrivia, fullWidth);
			case '/':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeSlashEqualsToken(precedingTrivia, index);
				}
				return MakeSlashToken(precedingTrivia, fullWidth);
			case '\\':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeBackSlashEqualsToken(precedingTrivia, index);
				}
				return MakeBackslashToken(precedingTrivia, fullWidth);
			case '^':
				if (TrySkipFollowingEquals(ref index))
				{
					return MakeCaretEqualsToken(precedingTrivia, index);
				}
				return MakeCaretToken(precedingTrivia, fullWidth);
			case '!':
				return MakeExclamationToken(precedingTrivia, fullWidth);
			case '.':
				if (CanGet(1) && SyntaxFacts.IsDecimalDigit(Peek(1)))
				{
					return ScanNumericLiteral(precedingTrivia);
				}
				return MakeDotToken(precedingTrivia, fullWidth);
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
				return ScanNumericLiteral(precedingTrivia);
			case '"':
				return ScanStringLiteral(precedingTrivia);
			case 'A':
				if (NextAre(1, "s "))
				{
					AdvanceChar(2);
					return MakeKeyword(SyntaxKind.AsKeyword, "As", precedingTrivia);
				}
				return ScanIdentifierOrKeyword(precedingTrivia);
			case 'E':
				if (NextAre(1, "nd "))
				{
					AdvanceChar(3);
					return MakeKeyword(SyntaxKind.EndKeyword, "End", precedingTrivia);
				}
				return ScanIdentifierOrKeyword(precedingTrivia);
			case 'I':
				if (NextAre(1, "f "))
				{
					AdvanceChar(2);
					return MakeKeyword(SyntaxKind.IfKeyword, "If", precedingTrivia);
				}
				return ScanIdentifierOrKeyword(precedingTrivia);
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
				return ScanIdentifierOrKeyword(precedingTrivia);
			case 'B':
			case 'C':
			case 'D':
			case 'F':
			case 'G':
			case 'H':
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
				return ScanIdentifierOrKeyword(precedingTrivia);
			case '_':
			{
				if (CanGet(1) && SyntaxFacts.IsIdentifierPartCharacter(Peek(1)))
				{
					return ScanIdentifierOrKeyword(precedingTrivia);
				}
				ERRID errId = ERRID.ERR_ExpectedIdentifier;
				int whitespaceLength = GetWhitespaceLength(1);
				if (!CanGet(whitespaceLength) || SyntaxFacts.IsNewLine(Peek(whitespaceLength)) || PeekStartComment(whitespaceLength) > 0)
				{
					errId = ERRID.ERR_LineContWithCommentOrNoPrecSpace;
				}
				return MakeBadToken(precedingTrivia, 1, errId);
			}
			case '[':
				return ScanBracketedIdentifier(precedingTrivia);
			case '?':
				return MakeQuestionToken(precedingTrivia, fullWidth);
			case '%':
				if (NextIs(1, '>'))
				{
					return XmlMakeEndEmbeddedToken(precedingTrivia, _scanSingleLineTriviaFunc);
				}
				break;
			case '$':
			case '＄':
				if (!fullWidth && CanGet(1) && SyntaxFacts.IsDoubleQuote(Peek(1)))
				{
					return MakePunctuationToken(precedingTrivia, 2, SyntaxKind.DollarSignDoubleQuoteToken);
				}
				break;
			}
			if (IsIdentifierStartCharacter(ch))
			{
				return ScanIdentifierOrKeyword(precedingTrivia);
			}
			if (fullWidth)
			{
				return null;
			}
			if (SyntaxFacts.IsDoubleQuote(ch))
			{
				return ScanStringLiteral(precedingTrivia);
			}
			if (SyntaxFacts.IsFullWidth(ch))
			{
				ch = SyntaxFacts.MakeHalfWidth(ch);
				return ScanTokenFullWidth(precedingTrivia, ch);
			}
			return null;
		}

		private SyntaxToken TryScanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			if (CanGet())
			{
				return ScanTokenCommon(precedingTrivia, Peek(), fullWidth: false);
			}
			return MakeEofToken(precedingTrivia);
		}

		private SyntaxToken ScanTokenFullWidth(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, char ch)
		{
			return ScanTokenCommon(precedingTrivia, ch, fullWidth: true);
		}

		private bool TrySkipFollowingEquals(ref int index)
		{
			int num = index;
			while (CanGet(num))
			{
				char c = Peek(num);
				num++;
				if (!SyntaxFacts.IsWhitespace(c))
				{
					if (c == '=' || c == '＝')
					{
						index = num;
						return true;
					}
					return false;
				}
			}
			return false;
		}

		private SyntaxToken ScanRightAngleBracket(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			int len = 1;
			len = GetWhitespaceLength(len);
			if (CanGet(len))
			{
				switch (Peek(len))
				{
				case '=':
				case '＝':
					len++;
					return MakeGreaterThanEqualsToken(precedingTrivia, len);
				case '>':
				case '＞':
					len++;
					if (TrySkipFollowingEquals(ref len))
					{
						return MakeGreaterThanGreaterThanEqualsToken(precedingTrivia, len);
					}
					return MakeGreaterThanGreaterThanToken(precedingTrivia, len);
				}
			}
			return MakeGreaterThanToken(precedingTrivia, charIsFullWidth);
		}

		private SyntaxToken ScanLeftAngleBracket(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth, ScanTriviaFunc scanTrailingTrivia)
		{
			int num = 1;
			if (!charIsFullWidth && CanGet(num))
			{
				switch (Peek(num))
				{
				case '!':
					if (!CanGet(num + 2))
					{
						break;
					}
					switch (Peek(num + 1))
					{
					case '-':
						if (CanGet(num + 3) && Peek(num + 2) == '-')
						{
							return XmlMakeBeginCommentToken(precedingTrivia, scanTrailingTrivia);
						}
						break;
					case '[':
						if (NextAre(num + 2, "CDATA["))
						{
							return XmlMakeBeginCDataToken(precedingTrivia, scanTrailingTrivia);
						}
						break;
					}
					break;
				case '?':
					return XmlMakeBeginProcessingInstructionToken(precedingTrivia, scanTrailingTrivia);
				case '/':
					return XmlMakeBeginEndElementToken(precedingTrivia, _scanSingleLineTriviaFunc);
				}
			}
			num = GetWhitespaceLength(num);
			if (CanGet(num))
			{
				switch (Peek(num))
				{
				case '=':
				case '＝':
					num++;
					return MakeLessThanEqualsToken(precedingTrivia, num);
				case '>':
				case '＞':
					num++;
					return MakeLessThanGreaterThanToken(precedingTrivia, num);
				case '<':
				case '＜':
				{
					num++;
					if (!CanGet(num))
					{
						break;
					}
					char c = Peek(num);
					if (c != '%' && c != '％')
					{
						if (TrySkipFollowingEquals(ref num))
						{
							return MakeLessThanLessThanEqualsToken(precedingTrivia, num);
						}
						return MakeLessThanLessThanToken(precedingTrivia, num);
					}
					break;
				}
				}
			}
			return MakeLessThanToken(precedingTrivia, charIsFullWidth);
		}

		internal static bool IsIdentifier(string spelling)
		{
			int length = spelling.Length;
			if (length == 0)
			{
				return false;
			}
			char c = spelling[0];
			if (SyntaxFacts.IsIdentifierStartCharacter(c))
			{
				if (SyntaxFacts.IsConnectorPunctuation(c) && length == 1)
				{
					return false;
				}
				int num = length - 1;
				for (int i = 1; i <= num; i++)
				{
					if (!SyntaxFacts.IsIdentifierPartCharacter(spelling[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		private SyntaxToken ScanIdentifierOrKeyword(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			char c = Peek();
			if (CanGet(1))
			{
				char c2 = Peek(1);
				if (SyntaxFacts.IsConnectorPunctuation(c) && !SyntaxFacts.IsIdentifierPartCharacter(c2))
				{
					return MakeBadToken(precedingTrivia, 1, ERRID.ERR_ExpectedIdentifier);
				}
			}
			int i;
			for (i = 1; CanGet(i); i++)
			{
				c = Peek(i);
				ushort num = Convert.ToUInt16(c);
				if (((uint)num >= 128u || !SyntaxFacts.IsNarrowIdentifierCharacter(num)) && !SyntaxFacts.IsWideIdentifierCharacter(c))
				{
					break;
				}
			}
			TypeCharacter typeCharacter = TypeCharacter.None;
			if (CanGet(i))
			{
				c = Peek(i);
				while (true)
				{
					switch (c)
					{
					case '!':
						if (CanGet(i + 1))
						{
							char c3 = Peek(i + 1);
							if (IsIdentifierStartCharacter(c3) || SyntaxFacts.MatchOneOrAnotherOrFullwidth(c3, '[', ']'))
							{
								break;
							}
						}
						typeCharacter = TypeCharacter.Single;
						i++;
						break;
					case '#':
						typeCharacter = TypeCharacter.Double;
						i++;
						break;
					case '$':
						typeCharacter = TypeCharacter.String;
						i++;
						break;
					case '%':
						typeCharacter = TypeCharacter.Integer;
						i++;
						break;
					case '&':
						typeCharacter = TypeCharacter.Long;
						i++;
						break;
					case '@':
						typeCharacter = TypeCharacter.Decimal;
						i++;
						break;
					default:
						if (SyntaxFacts.IsFullWidth(c))
						{
							goto IL_011b;
						}
						break;
					}
					break;
					IL_011b:
					c = SyntaxFacts.MakeHalfWidth(c);
				}
			}
			SyntaxKind syntaxKind = SyntaxKind.IdentifierToken;
			SyntaxKind contextualKind = SyntaxKind.IdentifierToken;
			string text = GetText(i);
			string text2 = ((typeCharacter == TypeCharacter.None) ? text : Intern(text, 0, i - 1));
			if (typeCharacter == TypeCharacter.None)
			{
				syntaxKind = TokenOfStringCached(text);
				if (SyntaxFacts.IsContextualKeyword(syntaxKind))
				{
					contextualKind = syntaxKind;
					syntaxKind = SyntaxKind.IdentifierToken;
				}
			}
			else if (TokenOfStringCached(text2) == SyntaxKind.MidKeyword)
			{
				contextualKind = SyntaxKind.MidKeyword;
				syntaxKind = SyntaxKind.IdentifierToken;
			}
			if (syntaxKind != SyntaxKind.IdentifierToken)
			{
				return MakeKeyword(syntaxKind, text, precedingTrivia);
			}
			return MakeIdentifier(text, contextualKind, isBracketed: false, text2, typeCharacter, precedingTrivia);
		}

		private SyntaxKind TokenOfStringCached(string spelling)
		{
			if (spelling.Length > 16)
			{
				return SyntaxKind.IdentifierToken;
			}
			return _KeywordsObjs.GetOrMakeValue(spelling);
		}

		private SyntaxToken ScanBracketedIdentifier(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			int num = 1;
			int i = num;
			bool flag = false;
			if (!CanGet(i))
			{
				return MakeBadToken(precedingTrivia, i, ERRID.ERR_MissingEndBrack);
			}
			char c = Peek(i);
			if (!IsIdentifierStartCharacter(c) || (SyntaxFacts.IsConnectorPunctuation(c) && (!CanGet(i + 1) || !SyntaxFacts.IsIdentifierPartCharacter(Peek(i + 1)))))
			{
				flag = true;
			}
			for (; CanGet(i); i++)
			{
				char c2 = Peek(i);
				if (c2 == ']' || c2 == '］')
				{
					int num2 = i - num;
					if (num2 > 0 && !flag)
					{
						string text = GetText(num2 + 2);
						string baseSpelling = text.Substring(1, num2);
						return MakeIdentifier(text, SyntaxKind.IdentifierToken, isBracketed: true, baseSpelling, TypeCharacter.None, precedingTrivia);
					}
					return MakeBadToken(precedingTrivia, i + 1, ERRID.ERR_ExpectedIdentifier);
				}
				if (SyntaxFacts.IsNewLine(c2))
				{
					break;
				}
				if (!SyntaxFacts.IsIdentifierPartCharacter(c2))
				{
					flag = true;
					break;
				}
			}
			if (i > 1)
			{
				return MakeBadToken(precedingTrivia, i, ERRID.ERR_MissingEndBrack);
			}
			return MakeBadToken(precedingTrivia, i, ERRID.ERR_ExpectedIdentifier);
		}

		private SyntaxToken ScanNumericLiteral(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			int i = 0;
			bool flag = false;
			bool flag2 = false;
			LiteralBase literalBase = LiteralBase.Decimal;
			NumericLiteralKind numericLiteralKind = NumericLiteralKind.Integral;
			char c = Peek();
			int num;
			bool flag3 = default(bool);
			if (c == '&' || c == '＆')
			{
				i++;
				c = (CanGet(i) ? Peek(i) : '\0');
				while (true)
				{
					switch (c)
					{
					case 'H':
					case 'h':
						i++;
						num = i;
						literalBase = LiteralBase.Hexadecimal;
						if (CanGet(i) && Peek(i) == '_')
						{
							flag2 = true;
						}
						for (; CanGet(i); i++)
						{
							c = Peek(i);
							if (!SyntaxFacts.IsHexDigit(c) && c != '_')
							{
								break;
							}
							if (c == '_')
							{
								flag = true;
							}
						}
						flag3 |= Peek(i - 1) == '_';
						break;
					case 'B':
					case 'b':
						i++;
						num = i;
						literalBase = LiteralBase.Binary;
						if (CanGet(i) && Peek(i) == '_')
						{
							flag2 = true;
						}
						for (; CanGet(i); i++)
						{
							c = Peek(i);
							if (!SyntaxFacts.IsBinaryDigit(c) && c != '_')
							{
								break;
							}
							if (c == '_')
							{
								flag = true;
							}
						}
						flag3 |= Peek(i - 1) == '_';
						break;
					case 'O':
					case 'o':
						i++;
						num = i;
						literalBase = LiteralBase.Octal;
						if (CanGet(i) && Peek(i) == '_')
						{
							flag2 = true;
						}
						for (; CanGet(i); i++)
						{
							c = Peek(i);
							if (!SyntaxFacts.IsOctalDigit(c) && c != '_')
							{
								break;
							}
							if (c == '_')
							{
								flag = true;
							}
						}
						flag3 |= Peek(i - 1) == '_';
						break;
					default:
						if (SyntaxFacts.IsFullWidth(c))
						{
							goto IL_01b6;
						}
						throw ExceptionUtilities.UnexpectedValue(c);
					}
					break;
					IL_01b6:
					c = SyntaxFacts.MakeHalfWidth(c);
				}
			}
			else
			{
				num = i;
				flag3 = CanGet(i) && Peek(i) == '_';
				for (; CanGet(i); i++)
				{
					c = Peek(i);
					if (!SyntaxFacts.IsDecimalDigit(c) && c != '_')
					{
						break;
					}
					if (c == '_')
					{
						flag = true;
					}
				}
				if (i != num)
				{
					flag3 |= Peek(i - 1) == '_';
				}
			}
			int num2 = i;
			if (literalBase == LiteralBase.Decimal && CanGet(i))
			{
				c = Peek(i);
				if ((c == '.' || c == '．') && CanGet(i + 1) && SyntaxFacts.IsDecimalDigit(Peek(i + 1)))
				{
					for (i += 2; CanGet(i); i++)
					{
						c = Peek(i);
						if (!SyntaxFacts.IsDecimalDigit(c) && c != '_')
						{
							break;
						}
					}
					flag3 |= Peek(i - 1) == '_';
					numericLiteralKind = NumericLiteralKind.Float;
				}
				if (CanGet(i) && SyntaxFacts.BeginsExponent(Peek(i)))
				{
					i++;
					if (CanGet(i))
					{
						c = Peek(i);
						if (SyntaxFacts.MatchOneOrAnotherOrFullwidth(c, '+', '-'))
						{
							i++;
						}
					}
					if (!CanGet(i) || !SyntaxFacts.IsDecimalDigit(Peek(i)))
					{
						return MakeBadToken(precedingTrivia, i, ERRID.ERR_InvalidLiteralExponent);
					}
					for (i++; CanGet(i); i++)
					{
						c = Peek(i);
						if (!SyntaxFacts.IsDecimalDigit(c) && c != '_')
						{
							break;
						}
					}
					flag3 |= Peek(i - 1) == '_';
					numericLiteralKind = NumericLiteralKind.Float;
				}
			}
			int num3 = i;
			TypeCharacter typeCharacter = TypeCharacter.None;
			if (CanGet(i))
			{
				c = Peek(i);
				while (true)
				{
					switch (c)
					{
					case '!':
						if (literalBase == LiteralBase.Decimal)
						{
							typeCharacter = TypeCharacter.Single;
							numericLiteralKind = NumericLiteralKind.Float;
							i++;
						}
						break;
					case 'F':
					case 'f':
						if (literalBase == LiteralBase.Decimal)
						{
							typeCharacter = TypeCharacter.SingleLiteral;
							numericLiteralKind = NumericLiteralKind.Float;
							i++;
						}
						break;
					case '#':
						if (literalBase == LiteralBase.Decimal)
						{
							typeCharacter = TypeCharacter.Double;
							numericLiteralKind = NumericLiteralKind.Float;
							i++;
						}
						break;
					case 'R':
					case 'r':
						if (literalBase == LiteralBase.Decimal)
						{
							typeCharacter = TypeCharacter.DoubleLiteral;
							numericLiteralKind = NumericLiteralKind.Float;
							i++;
						}
						break;
					case 'S':
					case 's':
						if (numericLiteralKind != NumericLiteralKind.Float)
						{
							typeCharacter = TypeCharacter.ShortLiteral;
							i++;
						}
						break;
					case '%':
						if (numericLiteralKind != NumericLiteralKind.Float)
						{
							typeCharacter = TypeCharacter.Integer;
							i++;
						}
						break;
					case 'I':
					case 'i':
						if (numericLiteralKind != NumericLiteralKind.Float)
						{
							typeCharacter = TypeCharacter.IntegerLiteral;
							i++;
						}
						break;
					case '&':
						if (numericLiteralKind != NumericLiteralKind.Float)
						{
							typeCharacter = TypeCharacter.Long;
							i++;
						}
						break;
					case 'L':
					case 'l':
						if (numericLiteralKind != NumericLiteralKind.Float)
						{
							typeCharacter = TypeCharacter.LongLiteral;
							i++;
						}
						break;
					case '@':
						if (literalBase == LiteralBase.Decimal)
						{
							typeCharacter = TypeCharacter.Decimal;
							numericLiteralKind = NumericLiteralKind.Decimal;
							i++;
						}
						break;
					case 'D':
					case 'd':
						if (literalBase != 0)
						{
							break;
						}
						typeCharacter = TypeCharacter.DecimalLiteral;
						numericLiteralKind = NumericLiteralKind.Decimal;
						if (CanGet(i + 1))
						{
							c = Peek(i + 1);
							if (SyntaxFacts.IsDecimalDigit(c) || SyntaxFacts.MatchOneOrAnotherOrFullwidth(c, '+', '-'))
							{
								return MakeBadToken(precedingTrivia, i, ERRID.ERR_ObsoleteExponent);
							}
						}
						i++;
						break;
					case 'U':
					case 'u':
						if (numericLiteralKind != NumericLiteralKind.Float && CanGet(i + 1))
						{
							char ch = Peek(i + 1);
							if (SyntaxFacts.MatchOneOrAnotherOrFullwidth(ch, 'S', 's'))
							{
								typeCharacter = TypeCharacter.UShortLiteral;
								i += 2;
							}
							else if (SyntaxFacts.MatchOneOrAnotherOrFullwidth(ch, 'I', 'i'))
							{
								typeCharacter = TypeCharacter.UIntegerLiteral;
								i += 2;
							}
							else if (SyntaxFacts.MatchOneOrAnotherOrFullwidth(ch, 'L', 'l'))
							{
								typeCharacter = TypeCharacter.ULongLiteral;
								i += 2;
							}
						}
						break;
					default:
						if (SyntaxFacts.IsFullWidth(c))
						{
							goto IL_05dd;
						}
						break;
					}
					break;
					IL_05dd:
					c = SyntaxFacts.MakeHalfWidth(c);
				}
			}
			bool flag4 = false;
			ulong num4 = default(ulong);
			decimal value = default(decimal);
			double d = default(double);
			if (numericLiteralKind == NumericLiteralKind.Integral)
			{
				if (num == num2)
				{
					return MakeBadToken(precedingTrivia, i, ERRID.ERR_Syntax);
				}
				num4 = 0uL;
				if (literalBase == LiteralBase.Decimal)
				{
					int num5 = num;
					int num6 = num2 - 1;
					for (int j = num5; j <= num6; j++)
					{
						char c2 = Peek(j);
						if (c2 != '_')
						{
							uint num7 = SyntaxFacts.IntegralLiteralCharacterValue(c2);
							if (num4 >= 1844674407370955161L && (num4 != 1844674407370955161L || num7 > 5))
							{
								flag4 = true;
								break;
							}
							num4 = num4 * 10 + num7;
						}
					}
					if (typeCharacter != TypeCharacter.ULongLiteral && decimal.Compare(new decimal(num4), 9223372036854775807m) > 0)
					{
						flag4 = true;
					}
				}
				else
				{
					int num8 = literalBase switch
					{
						LiteralBase.Octal => 3, 
						LiteralBase.Hexadecimal => 4, 
						_ => 1, 
					};
					ulong num9 = literalBase switch
					{
						LiteralBase.Octal => 16140901064495857664uL, 
						LiteralBase.Hexadecimal => 17293822569102704640uL, 
						_ => 9223372036854775808uL, 
					};
					int num10 = num;
					int num11 = num2 - 1;
					for (int k = num10; k <= num11; k++)
					{
						char c3 = Peek(k);
						if (c3 != '_')
						{
							if (decimal.Compare(new decimal(num4 & num9), 0m) != 0)
							{
								flag4 = true;
							}
							num4 = (num4 << num8) + SyntaxFacts.IntegralLiteralCharacterValue(c3);
						}
					}
				}
				switch (typeCharacter)
				{
				case TypeCharacter.Integer:
				case TypeCharacter.IntegerLiteral:
					if ((literalBase == LiteralBase.Decimal && decimal.Compare(new decimal(num4), new decimal(2147483647L)) > 0) || num4 > uint.MaxValue)
					{
						flag4 = true;
					}
					break;
				case TypeCharacter.UIntegerLiteral:
					if (num4 > uint.MaxValue)
					{
						flag4 = true;
					}
					break;
				case TypeCharacter.ShortLiteral:
					if ((literalBase == LiteralBase.Decimal && decimal.Compare(new decimal(num4), new decimal(32767L)) > 0) || decimal.Compare(new decimal(num4), new decimal(65535L)) > 0)
					{
						flag4 = true;
					}
					break;
				case TypeCharacter.UShortLiteral:
					if (decimal.Compare(new decimal(num4), new decimal(65535L)) > 0)
					{
						flag4 = true;
					}
					break;
				}
			}
			else
			{
				StringBuilder scratch = GetScratch();
				int num12 = num3 - 1;
				for (int l = 0; l <= num12; l++)
				{
					char c4 = Peek(l);
					if (c4 != '_')
					{
						scratch.Append(SyntaxFacts.IsFullWidth(c4) ? SyntaxFacts.MakeHalfWidth(c4) : c4);
					}
				}
				string scratchTextInterned = GetScratchTextInterned(scratch);
				if (numericLiteralKind == NumericLiteralKind.Decimal)
				{
					flag4 = !GetDecimalValue(scratchTextInterned, out value);
				}
				else if (typeCharacter == TypeCharacter.Single || typeCharacter == TypeCharacter.SingleLiteral)
				{
					if (!RealParser.TryParseFloat(scratchTextInterned, out var f))
					{
						flag4 = true;
					}
					else
					{
						d = f;
					}
				}
				else if (!RealParser.TryParseDouble(scratchTextInterned, out d))
				{
					flag4 = true;
				}
			}
			SyntaxToken syntaxToken = numericLiteralKind switch
			{
				NumericLiteralKind.Integral => MakeIntegerLiteralToken(precedingTrivia, literalBase, typeCharacter, (flag4 || flag3) ? 0 : num4, i), 
				NumericLiteralKind.Float => MakeFloatingLiteralToken(precedingTrivia, typeCharacter, (flag4 || flag3) ? 0.0 : d, i), 
				NumericLiteralKind.Decimal => MakeDecimalLiteralToken(precedingTrivia, typeCharacter, (flag4 || flag3) ? 0m : value, i), 
				_ => throw ExceptionUtilities.UnexpectedValue(numericLiteralKind), 
			};
			if (flag4)
			{
				syntaxToken = (SyntaxToken)syntaxToken.AddError(ErrorFactory.ErrorInfo(ERRID.ERR_Overflow));
			}
			if (flag3)
			{
				syntaxToken = (SyntaxToken)syntaxToken.AddError(ErrorFactory.ErrorInfo(ERRID.ERR_Syntax));
			}
			else if (flag2)
			{
				syntaxToken = CheckFeatureAvailability(syntaxToken, Feature.LeadingDigitSeparator);
			}
			else if (flag)
			{
				syntaxToken = CheckFeatureAvailability(syntaxToken, Feature.DigitSeparators);
			}
			if (literalBase == LiteralBase.Binary)
			{
				syntaxToken = CheckFeatureAvailability(syntaxToken, Feature.BinaryLiterals);
			}
			return syntaxToken;
		}

		private static bool GetDecimalValue(string text, out decimal value)
		{
			return decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out value);
		}

		private bool ScanIntLiteral(ref int ReturnValue, ref int here)
		{
			if (!CanGet(here))
			{
				return false;
			}
			char c = Peek(here);
			if (!SyntaxFacts.IsDecimalDigit(c))
			{
				return false;
			}
			int num = SyntaxFacts.IntegralLiteralCharacterValue(c);
			here++;
			while (CanGet(here))
			{
				c = Peek(here);
				if (!SyntaxFacts.IsDecimalDigit(c))
				{
					break;
				}
				byte b = SyntaxFacts.IntegralLiteralCharacterValue(c);
				if (num < 214748364 || (num == 214748364 && b < 8))
				{
					num = num * 10 + b;
					here++;
					continue;
				}
				return false;
			}
			ReturnValue = num;
			return true;
		}

		private SyntaxToken ScanDateLiteral(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			int len = 1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			int[] array = null;
			bool flag10 = false;
			len = GetWhitespaceLength(len);
			int num = len;
			int ReturnValue = default(int);
			if (!ScanIntLiteral(ref ReturnValue, ref len))
			{
				return null;
			}
			if (!CanGet(len) || !SyntaxFacts.IsDateSeparatorCharacter(Peek(len)))
			{
				goto IL_0134;
			}
			int skip = len;
			flag = true;
			len++;
			int ReturnValue2 = default(int);
			int ReturnValue3 = default(int);
			int ReturnValue4 = default(int);
			if (len - num == 5)
			{
				flag2 = true;
				flag10 = true;
				ReturnValue2 = ReturnValue;
				if (ScanIntLiteral(ref ReturnValue3, ref len))
				{
					if (!CanGet(len) || !SyntaxFacts.IsDateSeparatorCharacter(Peek(len)))
					{
						goto IL_012c;
					}
					if (Peek(len) == Peek(skip))
					{
						len++;
						if (ScanIntLiteral(ref ReturnValue4, ref len))
						{
							goto IL_012c;
						}
					}
				}
			}
			else
			{
				ReturnValue3 = ReturnValue;
				if (ScanIntLiteral(ref ReturnValue4, ref len))
				{
					if (!CanGet(len) || !SyntaxFacts.IsDateSeparatorCharacter(Peek(len)))
					{
						goto IL_012c;
					}
					if (Peek(len) == Peek(skip))
					{
						flag2 = true;
						len++;
						int num2 = len;
						if (ScanIntLiteral(ref ReturnValue2, ref len))
						{
							if (len - num2 == 2)
							{
								flag9 = true;
							}
							goto IL_012c;
						}
					}
				}
			}
			goto IL_0430;
			IL_012c:
			len = GetWhitespaceLength(len);
			goto IL_0134;
			IL_0134:
			int ReturnValue5 = default(int);
			if (!flag)
			{
				flag3 = true;
				ReturnValue5 = ReturnValue;
			}
			else if (ScanIntLiteral(ref ReturnValue5, ref len))
			{
				flag3 = true;
			}
			int ReturnValue6 = default(int);
			int ReturnValue7 = default(int);
			if (flag3)
			{
				if (CanGet(len) && SyntaxFacts.IsColon(Peek(len)))
				{
					len++;
					if (!ScanIntLiteral(ref ReturnValue6, ref len))
					{
						goto IL_0430;
					}
					flag4 = true;
					if (CanGet(len) && SyntaxFacts.IsColon(Peek(len)))
					{
						flag5 = true;
						len++;
						if (!ScanIntLiteral(ref ReturnValue7, ref len))
						{
							goto IL_0430;
						}
					}
				}
				len = GetWhitespaceLength(len);
				if (CanGet(len))
				{
					if (Peek(len) == 'A' || Peek(len) == 'Ａ' || Peek(len) == 'a' || Peek(len) == 'ａ')
					{
						flag6 = true;
						len++;
					}
					else if (Peek(len) == 'P' || Peek(len) == 'Ｐ' || Peek(len) == 'p' || Peek(len) == 'ｐ')
					{
						flag7 = true;
						len++;
					}
					if (CanGet(len) && (flag6 || flag7))
					{
						if (Peek(len) != 'M' && Peek(len) != 'Ｍ' && Peek(len) != 'm' && Peek(len) != 'ｍ')
						{
							goto IL_0430;
						}
						len = GetWhitespaceLength(len + 1);
					}
				}
				if (!flag4 && !flag6 && !flag7)
				{
					goto IL_0430;
				}
			}
			if (CanGet(len) && SyntaxFacts.IsHash(Peek(len)))
			{
				len++;
				if (flag)
				{
					if (ReturnValue3 < 1 || ReturnValue3 > 12)
					{
						flag8 = true;
					}
					if (!flag2)
					{
						flag8 = true;
						ReturnValue2 = 1;
					}
					array = ((ReturnValue2 % 4 == 0 && (ReturnValue2 % 100 != 0 || ReturnValue2 % 400 == 0)) ? SyntaxFacts.DaysToMonth366 : SyntaxFacts.DaysToMonth365);
					if (ReturnValue4 < 1 || (!flag8 && ReturnValue4 > array[ReturnValue3] - array[ReturnValue3 - 1]))
					{
						flag8 = true;
					}
					if (flag9)
					{
						flag8 = true;
					}
					if (ReturnValue2 < 1 || ReturnValue2 > 9999)
					{
						flag8 = true;
					}
				}
				else
				{
					ReturnValue3 = 1;
					ReturnValue4 = 1;
					ReturnValue2 = 1;
					array = SyntaxFacts.DaysToMonth365;
				}
				if (flag3)
				{
					if (flag6 || flag7)
					{
						if (ReturnValue5 < 1 || ReturnValue5 > 12)
						{
							flag8 = true;
						}
						if (flag6)
						{
							ReturnValue5 %= 12;
						}
						else if (flag7)
						{
							ReturnValue5 += 12;
							if (ReturnValue5 == 24)
							{
								ReturnValue5 = 12;
							}
						}
					}
					else if (ReturnValue5 < 0 || ReturnValue5 > 23)
					{
						flag8 = true;
					}
					if (flag4)
					{
						if (ReturnValue6 < 0 || ReturnValue6 > 59)
						{
							flag8 = true;
						}
					}
					else
					{
						ReturnValue6 = 0;
					}
					if (flag5)
					{
						if (ReturnValue7 < 0 || ReturnValue7 > 59)
						{
							flag8 = true;
						}
					}
					else
					{
						ReturnValue7 = 0;
					}
				}
				else
				{
					ReturnValue5 = 0;
					ReturnValue6 = 0;
					ReturnValue7 = 0;
				}
				if (!flag8)
				{
					DateTime value = new DateTime(ReturnValue2, ReturnValue3, ReturnValue4, ReturnValue5, ReturnValue6, ReturnValue7);
					SyntaxToken syntaxToken = MakeDateLiteralToken(precedingTrivia, value, len);
					if (flag10)
					{
						syntaxToken = Parser.CheckFeatureAvailability(Feature.YearFirstDateLiterals, syntaxToken, Options.LanguageVersion);
					}
					return syntaxToken;
				}
				return MakeBadToken(precedingTrivia, len, ERRID.ERR_InvalidDate);
			}
			goto IL_0430;
			IL_0430:
			for (; CanGet(len); len++)
			{
				char c = Peek(len);
				if (SyntaxFacts.IsHash(c) || SyntaxFacts.IsNewLine(c))
				{
					break;
				}
			}
			if (!CanGet(len) || SyntaxFacts.IsNewLine(Peek(len)))
			{
				return null;
			}
			len++;
			return MakeBadToken(precedingTrivia, len, ERRID.ERR_InvalidDate);
		}

		private SyntaxToken ScanStringLiteral(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			int num = 1;
			if (CanGet(3) && SyntaxFacts.IsDoubleQuote(Peek(2)))
			{
				if (SyntaxFacts.IsDoubleQuote(Peek(1)))
				{
					if (SyntaxFacts.IsDoubleQuote(Peek(3)) && CanGet(4) && SyntaxFacts.IsLetterC(Peek(4)))
					{
						return MakeCharacterLiteralToken(precedingTrivia, '"', 5);
					}
				}
				else if (SyntaxFacts.IsLetterC(Peek(3)))
				{
					return MakeCharacterLiteralToken(precedingTrivia, Peek(1), 4);
				}
			}
			if (CanGet(2) && SyntaxFacts.IsDoubleQuote(Peek(1)) && SyntaxFacts.IsLetterC(Peek(2)))
			{
				return MakeBadToken(precedingTrivia, 3, ERRID.ERR_IllegalCharConstant);
			}
			bool flag = false;
			StringBuilder scratch = GetScratch();
			while (CanGet(num))
			{
				char c = Peek(num);
				if (SyntaxFacts.IsDoubleQuote(c))
				{
					if (CanGet(num + 1))
					{
						c = Peek(num + 1);
						if (SyntaxFacts.IsDoubleQuote(c))
						{
							scratch.Append('"');
							num += 2;
							continue;
						}
						if (SyntaxFacts.IsLetterC(c))
						{
							scratch.Clear();
							return MakeBadToken(precedingTrivia, num + 2, ERRID.ERR_IllegalCharConstant);
						}
					}
					num++;
					SyntaxToken syntaxToken = SyntaxFactory.StringLiteralToken(GetTextNotInterned(num), trailingTrivia: ScanSingleLineTrivia().Node, value: GetScratchText(scratch), leadingTrivia: precedingTrivia.Node);
					if (flag)
					{
						syntaxToken = Parser.CheckFeatureAvailability(Feature.MultilineStringLiterals, syntaxToken, Options.LanguageVersion);
					}
					return syntaxToken;
				}
				if (SyntaxFacts.IsNewLine(c))
				{
					if (_isScanningDirective)
					{
						break;
					}
					flag = true;
				}
				scratch.Append(c);
				num++;
			}
			return (SyntaxToken)SyntaxFactory.StringLiteralToken(GetTextNotInterned(num), trailingTrivia: ScanSingleLineTrivia().Node, value: GetScratchText(scratch), leadingTrivia: precedingTrivia.Node).SetDiagnostics(new DiagnosticInfo[1] { ErrorFactory.ErrorInfo(ERRID.ERR_UnterminatedStringLiteral) });
		}

		internal static bool TryIdentifierAsContextualKeyword(IdentifierTokenSyntax id, ref SyntaxKind k)
		{
			if (id.PossibleKeywordKind != SyntaxKind.IdentifierToken)
			{
				k = id.PossibleKeywordKind;
				return true;
			}
			return false;
		}

		internal bool TryIdentifierAsContextualKeyword(IdentifierTokenSyntax id, ref KeywordSyntax k)
		{
			SyntaxKind k2 = SyntaxKind.IdentifierToken;
			if (TryIdentifierAsContextualKeyword(id, ref k2))
			{
				k = MakeKeyword(id);
				return true;
			}
			return false;
		}

		internal bool TryTokenAsContextualKeyword(SyntaxToken t, ref KeywordSyntax k)
		{
			if (t == null)
			{
				return false;
			}
			if (t.Kind == SyntaxKind.IdentifierToken)
			{
				return TryIdentifierAsContextualKeyword((IdentifierTokenSyntax)t, ref k);
			}
			return false;
		}

		internal static bool TryTokenAsKeyword(SyntaxToken t, ref SyntaxKind kind)
		{
			if (t == null)
			{
				return false;
			}
			if (t.IsKeyword)
			{
				kind = t.Kind;
				return true;
			}
			if (t.Kind == SyntaxKind.IdentifierToken)
			{
				return TryIdentifierAsContextualKeyword((IdentifierTokenSyntax)t, ref kind);
			}
			return false;
		}

		internal static bool IsContextualKeyword(SyntaxToken t, params SyntaxKind[] kinds)
		{
			SyntaxKind kind = SyntaxKind.None;
			if (TryTokenAsKeyword(t, ref kind))
			{
				return Array.IndexOf(kinds, kind) >= 0;
			}
			return false;
		}

		private bool IsIdentifierStartCharacter(char c)
		{
			if (!_isScanningForExpressionCompiler || c != '$')
			{
				return SyntaxFacts.IsIdentifierStartCharacter(c);
			}
			return true;
		}

		private SyntaxToken CheckFeatureAvailability(SyntaxToken token, Feature feature)
		{
			if (CheckFeatureAvailability(feature))
			{
				return token;
			}
			VisualBasicRequiredLanguageVersion visualBasicRequiredLanguageVersion = new VisualBasicRequiredLanguageVersion(FeatureExtensions.GetLanguageVersion(feature));
			DiagnosticInfo err = ErrorFactory.ErrorInfo(ERRID.ERR_LanguageVersion, LanguageVersionEnumBounds.GetErrorName(_options.LanguageVersion), ErrorFactory.ErrorInfo(FeatureExtensions.GetResourceId(feature)), visualBasicRequiredLanguageVersion);
			return (SyntaxToken)token.AddError(err);
		}

		internal bool CheckFeatureAvailability(Feature feature)
		{
			return CheckFeatureAvailability(Options, feature);
		}

		private static bool CheckFeatureAvailability(VisualBasicParseOptions parseOptions, Feature feature)
		{
			string featureFlag = FeatureExtensions.GetFeatureFlag(feature);
			if (featureFlag != null)
			{
				return parseOptions.Features.ContainsKey(featureFlag);
			}
			LanguageVersion languageVersion = FeatureExtensions.GetLanguageVersion(feature);
			LanguageVersion languageVersion2 = parseOptions.LanguageVersion;
			return languageVersion <= languageVersion2;
		}

		private Page GetPage(int position)
		{
			int num = (position >> 11) & 3;
			Page page = _pages[num];
			int num2 = position & -2048;
			if (page == null)
			{
				page = Page.GetInstance();
				_pages[num] = page;
			}
			if (page._pageStart != num2)
			{
				_buffer.CopyTo(num2, page._arr, 0, Math.Min(_bufferLen - num2, 2048));
				page._pageStart = num2;
			}
			_curPage = page;
			return page;
		}

		private char Peek(int skip)
		{
			int lineBufferOffset = _lineBufferOffset;
			Page curPage = _curPage;
			lineBufferOffset += skip;
			char result = curPage._arr[lineBufferOffset & 0x7FF];
			int pageStart = curPage._pageStart;
			int num = lineBufferOffset & -2048;
			if (pageStart != num)
			{
				result = GetPage(lineBufferOffset)._arr[lineBufferOffset & 0x7FF];
			}
			return result;
		}

		internal char Peek()
		{
			Page curPage = _curPage;
			int lineBufferOffset = _lineBufferOffset;
			char result = curPage._arr[lineBufferOffset & 0x7FF];
			int pageStart = curPage._pageStart;
			int num = lineBufferOffset & -2048;
			if (pageStart != num)
			{
				result = GetPage(lineBufferOffset)._arr[lineBufferOffset & 0x7FF];
			}
			return result;
		}

		internal string GetChar()
		{
			return Intern(Peek());
		}

		internal string GetText(int start, int length)
		{
			Page curPage = _curPage;
			int num = start & 0x7FF;
			if (curPage._pageStart == (start & -2048) && num + length < 2048)
			{
				return Intern(curPage._arr, num, length);
			}
			return GetTextSlow(start, length);
		}

		internal string GetTextNotInterned(int start, int length)
		{
			Page curPage = _curPage;
			int num = start & 0x7FF;
			if (curPage._pageStart == (start & -2048) && num + length < 2048)
			{
				char[] arr = curPage._arr;
				if (length == 2 && arr[num] == '\r' && arr[num + 1] == '\n')
				{
					return "\r\n";
				}
				return new string(arr, num, length);
			}
			return GetTextSlow(start, length, suppressInterning: true);
		}

		private string GetTextSlow(int start, int length, bool suppressInterning = false)
		{
			int num = start & 0x7FF;
			Page page = GetPage(start);
			if (num + length < 2048)
			{
				if (suppressInterning)
				{
					return new string(page._arr, num, length);
				}
				return Intern(page._arr, num, length);
			}
			if (_builder == null)
			{
				_builder = new StringBuilder(Math.Min(length, 1024));
			}
			int num2 = Math.Min(length, 2048 - num);
			_builder.Append(page._arr, num, num2);
			int num3 = num2;
			length -= num2;
			start += num2;
			do
			{
				page = GetPage(start);
				num2 = Math.Min(length, 2048);
				_builder.Append(page._arr, 0, num2);
				num3 += num2;
				length -= num2;
				start += num2;
			}
			while (length > 0);
			string text = ((!suppressInterning) ? _stringTable.Add(_builder) : _builder.ToString());
			if (text.Length < 1024)
			{
				_builder.Clear();
			}
			else
			{
				_builder = null;
			}
			return text;
		}

		private SyntaxToken ScanInterpolatedStringPunctuation()
		{
			if (!CanGet())
			{
				return MakeEndOfInterpolatedStringToken();
			}
			int whitespaceLength = GetWhitespaceLength(0);
			int num = whitespaceLength;
			if (!CanGet(num))
			{
				return MakeEndOfInterpolatedStringToken();
			}
			char c = Peek(num);
			SyntaxKind kind;
			int length;
			bool flag;
			switch (c)
			{
			case '$':
			case '＄':
				if (CanGet(num + 1) && SyntaxFacts.IsDoubleQuote(Peek(num + 1)))
				{
					kind = SyntaxKind.DollarSignDoubleQuoteToken;
					length = 2;
					flag = false;
					break;
				}
				throw ExceptionUtilities.Unreachable;
			case '{':
			case '｛':
				kind = SyntaxKind.OpenBraceToken;
				length = 1;
				flag = true;
				break;
			case ',':
			case '，':
				kind = SyntaxKind.CommaToken;
				length = 1;
				flag = true;
				break;
			case ':':
			case '：':
				kind = SyntaxKind.ColonToken;
				length = 1;
				flag = false;
				break;
			case '}':
			case '｝':
				kind = SyntaxKind.CloseBraceToken;
				length = 1;
				flag = false;
				break;
			default:
				if (SyntaxFacts.IsDoubleQuote(c))
				{
					kind = SyntaxKind.DoubleQuoteToken;
					length = 1;
					flag = true;
					break;
				}
				return MakeEndOfInterpolatedStringToken();
			}
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanWhitespace(whitespaceLength);
			string text = GetText(length);
			return MakePunctuationToken(followingTrivia: (flag ? ScanSingleLineTrivia() : default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>)).Node, kind: kind, spelling: text, precedingTrivia: visualBasicSyntaxNode);
		}

		private SyntaxToken ScanInterpolatedStringContent()
		{
			if (IsInterpolatedStringPunctuation())
			{
				return ScanInterpolatedStringPunctuation();
			}
			return ScanInterpolatedStringText(scanTrailingWhitespaceAsTrivia: false);
		}

		private SyntaxToken ScanInterpolatedStringFormatString()
		{
			if (IsInterpolatedStringPunctuation())
			{
				return ScanInterpolatedStringPunctuation();
			}
			return ScanInterpolatedStringText(scanTrailingWhitespaceAsTrivia: true);
		}

		private bool IsInterpolatedStringPunctuation(int offset = 0)
		{
			if (!CanGet(offset))
			{
				return false;
			}
			char c = Peek(offset);
			if (SyntaxFacts.IsLeftCurlyBracket(c))
			{
				return !CanGet(offset + 1) || !SyntaxFacts.IsLeftCurlyBracket(Peek(offset + 1));
			}
			if (SyntaxFacts.IsRightCurlyBracket(c))
			{
				return !CanGet(offset + 1) || !SyntaxFacts.IsRightCurlyBracket(Peek(offset + 1));
			}
			if (SyntaxFacts.IsDoubleQuote(c))
			{
				return !CanGet(offset + 1) || !SyntaxFacts.IsDoubleQuote(Peek(offset + 1));
			}
			return false;
		}

		private SyntaxToken ScanInterpolatedStringText(bool scanTrailingWhitespaceAsTrivia)
		{
			if (!CanGet())
			{
				return MakeEndOfInterpolatedStringToken();
			}
			int num = 0;
			int num2 = 0;
			StringBuilder scratch = GetScratch();
			while (CanGet(num))
			{
				char c = Peek(num);
				if (SyntaxFacts.IsLeftCurlyBracket(c))
				{
					if (!CanGet(num + 1) || !SyntaxFacts.IsLeftCurlyBracket(Peek(num + 1)))
					{
						break;
					}
					scratch.Append("{{");
					num += 2;
					num2 = 0;
					continue;
				}
				if (SyntaxFacts.IsRightCurlyBracket(c))
				{
					if (!CanGet(num + 1) || !SyntaxFacts.IsRightCurlyBracket(Peek(num + 1)))
					{
						break;
					}
					scratch.Append("}}");
					num += 2;
					num2 = 0;
					continue;
				}
				if (SyntaxFacts.IsDoubleQuote(c))
				{
					if (!CanGet(num + 1) || !SyntaxFacts.IsDoubleQuote(Peek(num + 1)))
					{
						break;
					}
					scratch.Append('"');
					num += 2;
					num2 = 0;
					continue;
				}
				if (SyntaxFacts.IsNewLine(c) && scanTrailingWhitespaceAsTrivia)
				{
					break;
				}
				if (SyntaxFacts.IsWhitespace(c) && scanTrailingWhitespaceAsTrivia)
				{
					scratch.Append(c);
					num++;
					num2++;
				}
				else
				{
					scratch.Append(c);
					num++;
					num2 = 0;
				}
			}
			if (num2 > 0)
			{
				num -= num2;
				scratch.Length -= num2;
			}
			string text = ((num > 0) ? GetTextNotInterned(num) : string.Empty);
			string scratchText = GetScratchText(scratch, text);
			return SyntaxFactory.InterpolatedStringTextToken(text, scratchText, null, ScanWhitespace(num2));
		}

		private SyntaxToken MakeEndOfInterpolatedStringToken()
		{
			return SyntaxFactory.Token(null, SyntaxKind.EndOfInterpolatedStringToken, null, string.Empty);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> ScanXmlTrivia(char c)
		{
			SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
			int num = 0;
			while (true)
			{
				if (c == ' ' || c == '\t')
				{
					num++;
				}
				else
				{
					if (c != '\r' && c != '\n')
					{
						break;
					}
					if (num > 0)
					{
						syntaxListBuilder.Add(MakeWhiteSpaceTrivia(GetText(num)));
						num = 0;
					}
					syntaxListBuilder.Add(ScanNewlineAsTrivia(c));
				}
				if (!CanGet(num))
				{
					break;
				}
				c = Peek(num);
			}
			if (num > 0)
			{
				syntaxListBuilder.Add(MakeWhiteSpaceTrivia(GetText(num)));
				num = 0;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> syntaxList = syntaxListBuilder.ToList();
			_triviaListPool.Free(syntaxListBuilder);
			return syntaxList;
		}

		internal SyntaxToken ScanXmlElement(ScannerState state = ScannerState.Element)
		{
			if (IsScanningXmlDoc)
			{
				return ScanXmlElementInXmlDoc(state);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			while (CanGet())
			{
				char c = Peek();
				switch (c)
				{
				case '\n':
				case '\r':
				{
					LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
					precedingTrivia = ScanXmlTrivia(c);
					if (ScanXmlForPossibleStatement(state))
					{
						lineBufferAndEndOfTerminatorOffsets.Restore();
						return SyntaxFactory.Token(null, SyntaxKind.EndOfXmlToken, null, string.Empty);
					}
					break;
				}
				case '\t':
				case ' ':
					precedingTrivia = ScanXmlTrivia(c);
					break;
				case '/':
					if (CanGet(1) && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(Peek(1)), ">", TextCompare: false) == 0)
					{
						return XmlMakeEndEmptyElementToken(precedingTrivia);
					}
					return XmlMakeDivToken(precedingTrivia);
				case '>':
					return XmlMakeGreaterToken(precedingTrivia);
				case '=':
					return XmlMakeEqualsToken(precedingTrivia);
				case '\'':
				case '‘':
				case '’':
					return XmlMakeSingleQuoteToken(precedingTrivia, c, isOpening: true);
				case '"':
				case '“':
				case '”':
					return XmlMakeDoubleQuoteToken(precedingTrivia, c, isOpening: true);
				case '<':
					if (CanGet(1))
					{
						switch (Peek(1))
						{
						case '!':
							if (CanGet(2))
							{
								switch (Peek(2))
								{
								case '-':
									if (NextIs(3, '-'))
									{
										return XmlMakeBeginCommentToken(precedingTrivia, s_scanNoTriviaFunc);
									}
									break;
								case '[':
									if (NextAre(3, "CDATA["))
									{
										return XmlMakeBeginCDataToken(precedingTrivia, s_scanNoTriviaFunc);
									}
									break;
								case 'D':
									if (NextAre(3, "OCTYPE"))
									{
										return XmlMakeBeginDTDToken(precedingTrivia);
									}
									break;
								}
							}
							return XmlLessThanExclamationToken(state, precedingTrivia);
						case '%':
							if (NextIs(2, '='))
							{
								return XmlMakeBeginEmbeddedToken(precedingTrivia);
							}
							break;
						case '?':
							return XmlMakeBeginProcessingInstructionToken(precedingTrivia, s_scanNoTriviaFunc);
						case '/':
							return XmlMakeBeginEndElementToken(precedingTrivia, s_scanNoTriviaFunc);
						}
					}
					return XmlMakeLessToken(precedingTrivia);
				case '?':
					if (NextIs(1, '>'))
					{
						return XmlMakeEndProcessingInstructionToken(precedingTrivia);
					}
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalXmlNameChar);
				case '(':
					return XmlMakeLeftParenToken(precedingTrivia);
				case ')':
					return XmlMakeRightParenToken(precedingTrivia);
				case '!':
				case '#':
				case ',':
				case ';':
				case '}':
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalXmlNameChar);
				case ':':
					return XmlMakeColonToken(precedingTrivia);
				case '[':
					return XmlMakeOpenBracketToken(state, precedingTrivia);
				case ']':
					return XmlMakeCloseBracketToken(state, precedingTrivia);
				default:
					return ScanXmlNcName(precedingTrivia);
				}
			}
			return MakeEofToken(precedingTrivia);
		}

		private bool ScanXmlForPossibleStatement(ScannerState state)
		{
			if (!CanGet())
			{
				return false;
			}
			bool result = false;
			LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
			char c = Peek();
			switch (c)
			{
			case '#':
			case '＃':
			{
				AdvanceChar();
				SyntaxToken syntaxToken = ScanNextToken(allowLeadingMultilineTrivia: false);
				result = syntaxToken.IsKeyword;
				break;
			}
			case '<':
			case '＜':
			{
				AdvanceChar();
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia2 = ScanSingleLineTrivia();
				SyntaxToken syntaxToken = ScanXmlNcName(precedingTrivia2);
				if (syntaxToken is XmlNameTokenSyntax xmlNameTokenSyntax2 && !xmlNameTokenSyntax2.IsMissing && xmlNameTokenSyntax2.PossibleKeywordKind != SyntaxKind.XmlNameToken)
				{
					precedingTrivia2 = ScanSingleLineTrivia();
					c = Peek();
					result = c == '(' || c == '（';
				}
				break;
			}
			default:
			{
				if (SyntaxFacts.IsSingleQuote(c) && LastToken.Kind != SyntaxKind.EqualsToken)
				{
					result = true;
					break;
				}
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = ScanSingleLineTrivia();
				SyntaxToken syntaxToken = ScanXmlNcName(precedingTrivia);
				if (syntaxToken is XmlNameTokenSyntax xmlNameTokenSyntax && !syntaxToken.IsMissing)
				{
					if (state == ScannerState.EndElement)
					{
						result = syntaxToken.Kind == SyntaxKind.XmlNameToken || LastToken.Kind == SyntaxKind.XmlNameToken;
						break;
					}
					syntaxToken = ScanNextToken(allowLeadingMultilineTrivia: false);
					result = ((xmlNameTokenSyntax.PossibleKeywordKind != SyntaxKind.XmlNameToken) ? (syntaxToken.Kind == SyntaxKind.IdentifierToken || syntaxToken.IsKeyword) : (syntaxToken.Kind == SyntaxKind.OpenParenToken));
				}
				break;
			}
			}
			lineBufferAndEndOfTerminatorOffsets.Restore();
			return result;
		}

		internal SyntaxToken ScanXmlContent()
		{
			if (IsScanningXmlDoc)
			{
				return ScanXmlContentInXmlDoc();
			}
			int num = 0;
			bool flag = true;
			if (_lineBufferOffset > 0)
			{
				char c = Peek(-1);
				if (c != '>' && !XmlCharType.IsWhiteSpace(c))
				{
					flag = false;
				}
			}
			StringBuilder scratch = GetScratch();
			while (CanGet(num))
			{
				char c2 = Peek(num);
				switch (c2)
				{
				case '\n':
				case '\r':
					num = SkipLineBreak(c2, num);
					scratch.Append('\n');
					continue;
				case '\t':
				case ' ':
					scratch.Append(c2);
					num++;
					continue;
				case '&':
					if (num != 0)
					{
						return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), num, scratch);
					}
					return ScanXmlReference(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>));
				case '<':
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
					if (num != 0)
					{
						if (!flag)
						{
							return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), num, scratch);
						}
						scratch.Clear();
						num = 0;
						precedingTrivia = ScanXmlTrivia(Peek());
					}
					if (CanGet(1))
					{
						switch (Peek(1))
						{
						case '!':
							if (!CanGet(2))
							{
								break;
							}
							switch (Peek(2))
							{
							case '-':
								if (NextIs(3, '-'))
								{
									return XmlMakeBeginCommentToken(precedingTrivia, s_scanNoTriviaFunc);
								}
								break;
							case '[':
								if (NextAre(3, "CDATA["))
								{
									return XmlMakeBeginCDataToken(precedingTrivia, s_scanNoTriviaFunc);
								}
								break;
							case 'D':
								if (NextAre(3, "OCTYPE"))
								{
									return XmlMakeBeginDTDToken(precedingTrivia);
								}
								break;
							}
							break;
						case '%':
							if (NextIs(2, '='))
							{
								return XmlMakeBeginEmbeddedToken(precedingTrivia);
							}
							break;
						case '?':
							return XmlMakeBeginProcessingInstructionToken(precedingTrivia, s_scanNoTriviaFunc);
						case '/':
							return XmlMakeBeginEndElementToken(precedingTrivia, s_scanNoTriviaFunc);
						}
					}
					return XmlMakeLessToken(precedingTrivia);
				}
				case ']':
					if (NextAre(num + 1, "]>"))
					{
						if (num != 0)
						{
							return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), num, scratch);
						}
						return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), 3, ERRID.ERR_XmlEndCDataNotAllowedInContent);
					}
					break;
				}
				flag = false;
				XmlCharResult xmlCharResult = ScanXmlChar(num);
				if (xmlCharResult.Length == 0)
				{
					if (num > 0)
					{
						return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), num, scratch);
					}
					return XmlMakeBadToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), 1, ERRID.ERR_IllegalChar);
				}
				xmlCharResult.AppendTo(scratch);
				num += xmlCharResult.Length;
			}
			if (num > 0)
			{
				return XmlMakeTextLiteralToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), num, scratch);
			}
			return MakeEofToken();
		}

		internal SyntaxToken ScanXmlComment()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			if (IsScanningXmlDoc && IsAtNewLine())
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				if (visualBasicSyntaxNode == null)
				{
					return MakeEofToken();
				}
				precedingTrivia = visualBasicSyntaxNode;
			}
			int i;
			XmlCharResult xmlCharResult;
			for (i = 0; CanGet(i); i += xmlCharResult.Length)
			{
				char c = Peek(i);
				switch (c)
				{
				case '\n':
				case '\r':
					return XmlMakeCommentToken(precedingTrivia, i + LengthOfLineBreak(c, i));
				case '-':
					if (!NextIs(i + 1, '-'))
					{
						break;
					}
					if (i > 0)
					{
						return XmlMakeCommentToken(precedingTrivia, i);
					}
					if (CanGet(i + 2))
					{
						c = Peek(i + 2);
						i += 2;
						if (c != '>')
						{
							return XmlMakeCommentToken(precedingTrivia, 2);
						}
						return XmlMakeEndCommentToken(precedingTrivia);
					}
					break;
				}
				xmlCharResult = ScanXmlChar(i);
				if (xmlCharResult.Length == 0)
				{
					if (i > 0)
					{
						return XmlMakeCommentToken(precedingTrivia, i);
					}
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalChar);
				}
			}
			if (i > 0)
			{
				return XmlMakeCommentToken(precedingTrivia, i);
			}
			return MakeEofToken(precedingTrivia);
		}

		internal SyntaxToken ScanXmlCData()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			if (IsScanningXmlDoc && IsAtNewLine())
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				if (visualBasicSyntaxNode == null)
				{
					return MakeEofToken();
				}
				precedingTrivia = visualBasicSyntaxNode;
			}
			StringBuilder scratch = GetScratch();
			int i;
			XmlCharResult xmlCharResult;
			for (i = 0; CanGet(i); i += xmlCharResult.Length)
			{
				char c = Peek(i);
				switch (c)
				{
				case '\n':
				case '\r':
					i = SkipLineBreak(c, i);
					scratch.Append('\n');
					return XmlMakeCDataToken(precedingTrivia, i, scratch);
				case ']':
					if (NextAre(i + 1, "]>"))
					{
						if (i != 0)
						{
							return XmlMakeCDataToken(precedingTrivia, i, scratch);
						}
						return XmlMakeEndCDataToken(precedingTrivia);
					}
					break;
				}
				xmlCharResult = ScanXmlChar(i);
				if (xmlCharResult.Length == 0)
				{
					if (i > 0)
					{
						return XmlMakeCDataToken(precedingTrivia, i, scratch);
					}
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalChar);
				}
				xmlCharResult.AppendTo(scratch);
			}
			if (i > 0)
			{
				return XmlMakeCDataToken(precedingTrivia, i, scratch);
			}
			return MakeEofToken(precedingTrivia);
		}

		internal SyntaxToken ScanXmlPIData(ScannerState state)
		{
			if (IsScanningXmlDoc)
			{
				return ScanXmlPIDataInXmlDoc(state);
			}
			SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = _triviaListPool.Allocate<VisualBasicSyntaxNode>();
			if (state == ScannerState.StartProcessingInstruction && CanGet())
			{
				char c = Peek();
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> nodes = ScanXmlTrivia(c);
					syntaxListBuilder.AddRange(nodes);
					break;
				}
				}
			}
			int num = 0;
			SyntaxToken result;
			while (true)
			{
				if (CanGet(num))
				{
					char c2 = Peek(num);
					if (c2 != '\n' && c2 != '\r')
					{
						if (c2 == '?' && NextIs(num + 1, '>'))
						{
							result = ((num == 0) ? ((SyntaxToken)XmlMakeEndProcessingInstructionToken(syntaxListBuilder.ToList())) : ((SyntaxToken)XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num)));
							break;
						}
						XmlCharResult xmlCharResult = ScanXmlChar(num);
						if (xmlCharResult.Length > 0)
						{
							num += xmlCharResult.Length;
							continue;
						}
						result = ((num == 0) ? ((SyntaxToken)XmlMakeBadToken(syntaxListBuilder.ToList(), 1, ERRID.ERR_IllegalChar)) : ((SyntaxToken)XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num)));
						break;
					}
					result = XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num + LengthOfLineBreak(c2, num));
					break;
				}
				result = ((num <= 0) ? MakeEofToken(syntaxListBuilder.ToList()) : XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num));
				break;
			}
			_triviaListPool.Free(syntaxListBuilder);
			return result;
		}

		internal SyntaxToken ScanXmlMisc()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			while (CanGet())
			{
				char c = Peek();
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					break;
				case '<':
					if (CanGet(1))
					{
						switch (Peek(1))
						{
						case '!':
							if (NextAre(2, "--"))
							{
								return XmlMakeBeginCommentToken(precedingTrivia, s_scanNoTriviaFunc);
							}
							if (NextAre(2, "DOCTYPE"))
							{
								return XmlMakeBeginDTDToken(precedingTrivia);
							}
							break;
						case '%':
							if (NextIs(2, '='))
							{
								return XmlMakeBeginEmbeddedToken(precedingTrivia);
							}
							break;
						case '?':
							return XmlMakeBeginProcessingInstructionToken(precedingTrivia, s_scanNoTriviaFunc);
						}
					}
					return XmlMakeLessToken(precedingTrivia);
				default:
					return SyntaxFactory.Token(precedingTrivia.Node, SyntaxKind.EndOfXmlToken, null, string.Empty);
				}
				precedingTrivia = ScanXmlTrivia(c);
			}
			return MakeEofToken(precedingTrivia);
		}

		internal SyntaxToken ScanXmlStringUnQuoted()
		{
			if (!CanGet())
			{
				return MakeEofToken();
			}
			int i = 0;
			StringBuilder scratch = GetScratch();
			XmlCharResult xmlCharResult;
			for (; CanGet(i); i += xmlCharResult.Length)
			{
				switch (Peek(i))
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					if (i > 0)
					{
						return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
					}
					return MakeMissingToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), SyntaxKind.SingleQuoteToken);
				case '<':
				case '>':
				case '?':
					if (i != 0)
					{
						return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
					}
					return MakeMissingToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), SyntaxKind.SingleQuoteToken);
				case '&':
					if (i > 0)
					{
						return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
					}
					return ScanXmlReference(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>));
				case '/':
					if (NextIs(i + 1, '>'))
					{
						if (i != 0)
						{
							return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
						}
						return MakeMissingToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), SyntaxKind.SingleQuoteToken);
					}
					break;
				}
				xmlCharResult = ScanXmlChar(i);
				if (xmlCharResult.Length == 0)
				{
					if (i > 0)
					{
						return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
					}
					return XmlMakeBadToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), 1, ERRID.ERR_IllegalChar);
				}
				xmlCharResult.AppendTo(scratch);
			}
			return XmlMakeAttributeDataToken(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), i, scratch);
		}

		internal SyntaxToken ScanXmlStringSingle()
		{
			return ScanXmlString('\'', '\'', isSingle: true);
		}

		internal SyntaxToken ScanXmlStringDouble()
		{
			return ScanXmlString('"', '"', isSingle: false);
		}

		internal SyntaxToken ScanXmlStringSmartSingle()
		{
			return ScanXmlString('’', '‘', isSingle: true);
		}

		internal SyntaxToken ScanXmlStringSmartDouble()
		{
			return ScanXmlString('”', '“', isSingle: false);
		}

		internal SyntaxToken ScanXmlString(char terminatingChar, char altTerminatingChar, bool isSingle)
		{
			SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = _triviaListPool.Allocate<VisualBasicSyntaxNode>();
			SyntaxToken result;
			if (IsScanningXmlDoc && IsAtNewLine())
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				if (visualBasicSyntaxNode == null)
				{
					result = MakeEofToken();
					goto IL_027a;
				}
				syntaxListBuilder.Add(visualBasicSyntaxNode);
			}
			int num = 0;
			StringBuilder scratch = GetScratch();
			while (true)
			{
				if (!CanGet(num))
				{
					result = ((num <= 0) ? MakeEofToken(syntaxListBuilder) : XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch));
					break;
				}
				char c = Peek(num);
				if (c == terminatingChar || c == altTerminatingChar)
				{
					result = ((num <= 0) ? ((SyntaxToken)((!isSingle) ? XmlMakeDoubleQuoteToken(syntaxListBuilder, c, isOpening: false) : XmlMakeSingleQuoteToken(syntaxListBuilder, c, isOpening: false))) : ((SyntaxToken)XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch)));
					break;
				}
				switch (c)
				{
				case '\n':
				case '\r':
					num = SkipLineBreak(c, num);
					scratch.Append(' ');
					result = XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch);
					break;
				case '\t':
					scratch.Append(' ');
					num++;
					continue;
				case '<':
				{
					if (num > 0)
					{
						result = XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch);
						break;
					}
					if (NextAre(1, "%="))
					{
						XmlTextTokenSyntax xmlTextTokenSyntax = XmlMakeAttributeDataToken(syntaxListBuilder, 3, "<%=");
						DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_QuotedEmbeddedExpression);
						result = (SyntaxToken)xmlTextTokenSyntax.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo });
						break;
					}
					SyntaxToken syntaxToken = SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken);
					if (syntaxListBuilder.Count > 0)
					{
						syntaxToken = (SyntaxToken)syntaxToken.WithLeadingTrivia(syntaxListBuilder.ToList().Node);
					}
					DiagnosticInfo diagnosticInfo2 = ErrorFactory.ErrorInfo(isSingle ? ERRID.ERR_ExpectedSQuote : ERRID.ERR_ExpectedQuote);
					result = (SyntaxToken)syntaxToken.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo2 });
					break;
				}
				case '&':
					result = ((num <= 0) ? ScanXmlReference(syntaxListBuilder) : XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch));
					break;
				default:
				{
					XmlCharResult xmlCharResult = ScanXmlChar(num);
					if (xmlCharResult.Length != 0)
					{
						xmlCharResult.AppendTo(scratch);
						num += xmlCharResult.Length;
						continue;
					}
					result = ((num <= 0) ? ((SyntaxToken)XmlMakeBadToken(syntaxListBuilder, 1, ERRID.ERR_IllegalChar)) : ((SyntaxToken)XmlMakeAttributeDataToken(syntaxListBuilder, num, scratch)));
					break;
				}
				}
				break;
			}
			goto IL_027a;
			IL_027a:
			_triviaListPool.Free(syntaxListBuilder);
			return result;
		}

		private XmlCharResult ScanSurrogatePair(char c1, int Here)
		{
			XmlCharResult result;
			if (SyntaxFacts.IsHighSurrogate(c1) && CanGet(Here + 1))
			{
				char c2 = Peek(Here + 1);
				if (SyntaxFacts.IsLowSurrogate(c2))
				{
					result = new XmlCharResult(c1, c2);
					goto IL_0038;
				}
			}
			result = default(XmlCharResult);
			goto IL_0038;
			IL_0038:
			return result;
		}

		private XmlCharResult ScanXmlChar(int Here)
		{
			char c = Peek(Here);
			XmlCharResult result;
			if (!XmlCharacterGlobalHelpers.isValidUtf16(c))
			{
				result = default(XmlCharResult);
			}
			else
			{
				if (SyntaxFacts.IsSurrogate(c))
				{
					return ScanSurrogatePair(c, Here);
				}
				result = new XmlCharResult(c);
			}
			return result;
		}

		private SyntaxToken ScanXmlNcName(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			int i = 0;
			bool flag = false;
			bool flag2 = true;
			ERRID eRRID = ERRID.ERR_None;
			int num = 0;
			string text = null;
			XmlCharResult ch;
			for (; CanGet(i); i += ch.Length)
			{
				switch (Peek(i))
				{
				default:
					ch = ScanXmlChar(i);
					if (ch.Length == 0)
					{
						flag = true;
						break;
					}
					if (eRRID != 0)
					{
						continue;
					}
					if (ch.Length == 1)
					{
						if (flag2)
						{
							eRRID = ((!XmlCharacterGlobalHelpers.isStartNameChar(ch.Char1)) ? ERRID.ERR_IllegalXmlStartNameChar : ERRID.ERR_None);
							flag2 = false;
						}
						else
						{
							eRRID = ((!XmlCharacterGlobalHelpers.isNameChar(ch.Char1)) ? ERRID.ERR_IllegalXmlNameChar : ERRID.ERR_None);
						}
						if (eRRID != 0)
						{
							text = Convert.ToString(ch.Char1);
							num = Convert.ToInt32(ch.Char1);
						}
					}
					else
					{
						int num2 = XmlCharacterGlobalHelpers.UTF16ToUnicode(ch);
						if (num2 < 65536 || num2 > 983039)
						{
							eRRID = ERRID.ERR_IllegalXmlNameChar;
							text = new string(new char[2] { ch.Char1, ch.Char2 });
							num = num2;
						}
					}
					continue;
				case '\t':
				case '\n':
				case '\r':
				case ' ':
				case '"':
				case '\'':
				case '(':
				case ')':
				case ',':
				case '/':
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '}':
					break;
				}
				break;
			}
			if (i != 0)
			{
				XmlNameTokenSyntax xmlNameTokenSyntax = XmlMakeXmlNCNameToken(precedingTrivia, i);
				if (eRRID != 0)
				{
					xmlNameTokenSyntax = SyntaxExtensions.WithDiagnostics(xmlNameTokenSyntax, ErrorFactory.ErrorInfo(eRRID, text, $"&H{num:X}"));
				}
				return xmlNameTokenSyntax;
			}
			if (flag)
			{
				return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalChar);
			}
			return MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken);
		}

		private XmlTextTokenSyntax ScanXmlReference(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			if (CanGet(1))
			{
				switch (Peek(1))
				{
				case '#':
				{
					int index = 2;
					XmlCharResult xmlCharResult = ScanXmlCharRef(ref index);
					if (xmlCharResult.Length != 0)
					{
						string value = null;
						if (xmlCharResult.Length == 1)
						{
							value = Intern(xmlCharResult.Char1);
						}
						else if (xmlCharResult.Length == 2)
						{
							value = Intern(new char[2] { xmlCharResult.Char1, xmlCharResult.Char2 });
						}
						if (CanGet(index) && Peek(index) == ';')
						{
							return XmlMakeEntityLiteralToken(precedingTrivia, index + 1, value);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax3 = XmlMakeEntityLiteralToken(precedingTrivia, index, value);
						DiagnosticInfo diagnosticInfo3 = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax3.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo3 });
					}
					break;
				}
				case 'a':
					if (CanGet(4) && NextAre(2, "mp"))
					{
						if (Peek(4) == ';')
						{
							return XmlMakeAmpLiteralToken(precedingTrivia);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax5 = XmlMakeEntityLiteralToken(precedingTrivia, 4, "&");
						DiagnosticInfo diagnosticInfo5 = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax5.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo5 });
					}
					if (CanGet(5) && NextAre(2, "pos"))
					{
						if (Peek(5) == ';')
						{
							return XmlMakeAposLiteralToken(precedingTrivia);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax6 = XmlMakeEntityLiteralToken(precedingTrivia, 5, "'");
						DiagnosticInfo diagnosticInfo6 = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax6.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo6 });
					}
					break;
				case 'l':
					if (CanGet(3) && NextIs(2, 't'))
					{
						if (Peek(3) == ';')
						{
							return XmlMakeLtLiteralToken(precedingTrivia);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax4 = XmlMakeEntityLiteralToken(precedingTrivia, 3, "<");
						DiagnosticInfo diagnosticInfo4 = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax4.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo4 });
					}
					break;
				case 'g':
					if (CanGet(3) && NextIs(2, 't'))
					{
						if (Peek(3) == ';')
						{
							return XmlMakeGtLiteralToken(precedingTrivia);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax2 = XmlMakeEntityLiteralToken(precedingTrivia, 3, ">");
						DiagnosticInfo diagnosticInfo2 = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax2.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo2 });
					}
					break;
				case 'q':
					if (CanGet(5) && NextAre(2, "uot"))
					{
						if (Peek(5) == ';')
						{
							return XmlMakeQuotLiteralToken(precedingTrivia);
						}
						XmlTextTokenSyntax xmlTextTokenSyntax = XmlMakeEntityLiteralToken(precedingTrivia, 5, "\"");
						DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon);
						return (XmlTextTokenSyntax)xmlTextTokenSyntax.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo });
					}
					break;
				}
			}
			XmlTextTokenSyntax xmlTextTokenSyntax7 = XmlMakeEntityLiteralToken(precedingTrivia, 1, "");
			DiagnosticInfo diagnosticInfo7 = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference);
			return (XmlTextTokenSyntax)xmlTextTokenSyntax7.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo7 });
		}

		private XmlCharResult ScanXmlCharRef(ref int index)
		{
			XmlCharResult result;
			if (!CanGet(index))
			{
				result = default(XmlCharResult);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				int i = index;
				char c = Peek(i);
				if (c == 'x')
				{
					for (i++; CanGet(i); i++)
					{
						c = Peek(i);
						if (!XmlCharType.IsHexDigit(c))
						{
							break;
						}
						stringBuilder.Append(c);
					}
					if (stringBuilder.Length > 0)
					{
						XmlCharResult result2 = XmlCharacterGlobalHelpers.HexToUTF16(stringBuilder);
						if (result2.Length != 0)
						{
							index = i;
						}
						return result2;
					}
				}
				else
				{
					for (; CanGet(i); i++)
					{
						c = Peek(i);
						if (!XmlCharType.IsDigit(c))
						{
							break;
						}
						stringBuilder.Append(c);
					}
					if (stringBuilder.Length > 0)
					{
						XmlCharResult result3 = XmlCharacterGlobalHelpers.DecToUTF16(stringBuilder);
						if (result3.Length != 0)
						{
							index = i;
						}
						return result3;
					}
				}
				result = default(XmlCharResult);
			}
			return result;
		}

		private static CachingFactory<TriviaKey, SyntaxTrivia> CreateWsTable()
		{
			CachingFactory<TriviaKey, SyntaxTrivia> cachingFactory = new CachingFactory<TriviaKey, SyntaxTrivia>(512, null, s_triviaKeyHasher, s_triviaKeyEquality);
			cachingFactory.Add(new TriviaKey(" ", SyntaxKind.WhitespaceTrivia), s_singleSpaceWhitespaceTrivia);
			cachingFactory.Add(new TriviaKey("    ", SyntaxKind.WhitespaceTrivia), s_fourSpacesWhitespaceTrivia);
			cachingFactory.Add(new TriviaKey("        ", SyntaxKind.WhitespaceTrivia), s_eightSpacesWhitespaceTrivia);
			cachingFactory.Add(new TriviaKey("            ", SyntaxKind.WhitespaceTrivia), s_twelveSpacesWhitespaceTrivia);
			cachingFactory.Add(new TriviaKey("                ", SyntaxKind.WhitespaceTrivia), s_sixteenSpacesWhitespaceTrivia);
			return cachingFactory;
		}

		private static bool CanCache(SyntaxListBuilder trivia)
		{
			int num = trivia.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				int rawKind = trivia[i]!.RawKind;
				if ((uint)(rawKind - 729) > 1u && (uint)(rawKind - 733) > 1u)
				{
					return false;
				}
			}
			return true;
		}

		internal SyntaxTrivia MakeWhiteSpaceTrivia(string text)
		{
			SyntaxTrivia value = null;
			TriviaKey key = new TriviaKey(text, SyntaxKind.WhitespaceTrivia);
			if (!_wsTable.TryGetValue(key, out value))
			{
				value = SyntaxFactory.WhitespaceTrivia(text);
				_wsTable.Add(key, value);
			}
			return value;
		}

		internal SyntaxTrivia MakeEndOfLineTrivia(string text)
		{
			SyntaxTrivia value = null;
			TriviaKey key = new TriviaKey(text, SyntaxKind.EndOfLineTrivia);
			if (!_wsTable.TryGetValue(key, out value))
			{
				value = SyntaxFactory.EndOfLineTrivia(text);
				_wsTable.Add(key, value);
			}
			return value;
		}

		internal SyntaxTrivia MakeColonTrivia(string text)
		{
			SyntaxTrivia value = null;
			TriviaKey key = new TriviaKey(text, SyntaxKind.ColonTrivia);
			if (!_wsTable.TryGetValue(key, out value))
			{
				value = SyntaxFactory.ColonTrivia(text);
				_wsTable.Add(key, value);
			}
			return value;
		}

		internal SyntaxTrivia MakeEndOfLineTriviaCRLF()
		{
			AdvanceChar(2);
			return s_crLfTrivia;
		}

		internal SyntaxTrivia MakeLineContinuationTrivia(string text)
		{
			SyntaxTrivia value = null;
			TriviaKey key = new TriviaKey(text, SyntaxKind.LineContinuationTrivia);
			if (!_wsTable.TryGetValue(key, out value))
			{
				value = SyntaxFactory.LineContinuationTrivia(text);
				_wsTable.Add(key, value);
			}
			return value;
		}

		internal SyntaxTrivia MakeDocumentationCommentExteriorTrivia(string text)
		{
			SyntaxTrivia value = null;
			TriviaKey key = new TriviaKey(text, SyntaxKind.DocumentationCommentExteriorTrivia);
			if (!_wsTable.TryGetValue(key, out value))
			{
				value = SyntaxFactory.DocumentationCommentExteriorTrivia(text);
				_wsTable.Add(key, value);
			}
			return value;
		}

		internal static SyntaxTrivia MakeCommentTrivia(string text)
		{
			return SyntaxFactory.SyntaxTrivia(SyntaxKind.CommentTrivia, text);
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> MakeTriviaArray(SyntaxListBuilder builder)
		{
			if (builder.Count == 0)
			{
				return default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			}
			if (CanCache(builder))
			{
				return _wslTable.GetOrMakeValue(builder);
			}
			return builder.ToList();
		}

		private IdentifierTokenSyntax MakeIdentifier(string spelling, SyntaxKind contextualKind, bool isBracketed, string BaseSpelling, TypeCharacter TypeCharacter, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> leadingTrivia)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = ScanSingleLineTrivia();
			return MakeIdentifier(spelling, contextualKind, isBracketed, BaseSpelling, TypeCharacter, leadingTrivia, syntaxList);
		}

		internal IdentifierTokenSyntax MakeIdentifier(KeywordSyntax keyword)
		{
			return MakeIdentifier(keyword.Text, keyword.Kind, isBracketed: false, keyword.Text, TypeCharacter.None, keyword.GetLeadingTrivia(), keyword.GetTrailingTrivia());
		}

		private IdentifierTokenSyntax MakeIdentifier(string spelling, SyntaxKind contextualKind, bool isBracketed, string BaseSpelling, TypeCharacter TypeCharacter, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> precedingTrivia, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> followingTrivia)
		{
			TokenParts key = new TokenParts(precedingTrivia, followingTrivia, spelling);
			IdentifierTokenSyntax value = null;
			if (_idTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = ((contextualKind == SyntaxKind.IdentifierToken && !isBracketed && TypeCharacter == TypeCharacter.None) ? SyntaxFactory.Identifier(spelling, precedingTrivia.Node, followingTrivia.Node) : SyntaxFactory.Identifier(spelling, contextualKind, isBracketed, BaseSpelling, TypeCharacter, precedingTrivia.Node, followingTrivia.Node));
			_idTable.Add(key, value);
			return value;
		}

		private KeywordSyntax MakeKeyword(SyntaxKind tokenType, string spelling, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = ScanSingleLineTrivia();
			return MakeKeyword(tokenType, spelling, precedingTrivia, syntaxList);
		}

		internal KeywordSyntax MakeKeyword(IdentifierTokenSyntax identifier)
		{
			return MakeKeyword(identifier.PossibleKeywordKind, identifier.Text, identifier.GetLeadingTrivia(), identifier.GetTrailingTrivia());
		}

		internal KeywordSyntax MakeKeyword(XmlNameTokenSyntax xmlName)
		{
			return MakeKeyword(xmlName.PossibleKeywordKind, xmlName.Text, xmlName.GetLeadingTrivia(), xmlName.GetTrailingTrivia());
		}

		private KeywordSyntax MakeKeyword(SyntaxKind tokenType, string spelling, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> precedingTrivia, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> followingTrivia)
		{
			TokenParts key = new TokenParts(precedingTrivia, followingTrivia, spelling);
			KeywordSyntax value = null;
			if (_kwTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = new KeywordSyntax(tokenType, spelling, precedingTrivia.Node, followingTrivia.Node);
			_kwTable.Add(key, value);
			return value;
		}

		internal PunctuationSyntax MakePunctuationToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, string spelling, SyntaxKind kind)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = ScanSingleLineTrivia();
			return MakePunctuationToken(kind, spelling, precedingTrivia, syntaxList);
		}

		private PunctuationSyntax MakePunctuationToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length, SyntaxKind kind)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = ScanSingleLineTrivia();
			return MakePunctuationToken(kind, text, precedingTrivia, syntaxList);
		}

		internal PunctuationSyntax MakePunctuationToken(SyntaxKind kind, string spelling, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> precedingTrivia, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> followingTrivia)
		{
			TokenParts key = new TokenParts(precedingTrivia, followingTrivia, spelling);
			PunctuationSyntax value = null;
			if (_punctTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = new PunctuationSyntax(kind, spelling, precedingTrivia.Node, followingTrivia.Node);
			_punctTable.Add(key, value);
			return value;
		}

		private PunctuationSyntax MakeOpenParenToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "（" : "(");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.OpenParenToken);
		}

		private PunctuationSyntax MakeCloseParenToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "）" : ")");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.CloseParenToken);
		}

		private PunctuationSyntax MakeDotToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "．" : ".");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.DotToken);
		}

		private PunctuationSyntax MakeCommaToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "，" : ",");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.CommaToken);
		}

		private PunctuationSyntax MakeEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＝" : "=");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.EqualsToken);
		}

		private PunctuationSyntax MakeHashToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＃" : "#");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.HashToken);
		}

		private PunctuationSyntax MakeAmpersandToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＆" : "&");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.AmpersandToken);
		}

		private PunctuationSyntax MakeOpenBraceToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "｛" : "{");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.OpenBraceToken);
		}

		private PunctuationSyntax MakeCloseBraceToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "｝" : "}");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.CloseBraceToken);
		}

		private PunctuationSyntax MakeColonToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			int howFar = _endOfTerminatorTrivia - _lineBufferOffset;
			AdvanceChar(howFar);
			return SyntaxFactory.ColonToken;
		}

		private PunctuationSyntax MakeEmptyToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return MakePunctuationToken(precedingTrivia, "", SyntaxKind.EmptyToken);
		}

		private PunctuationSyntax MakePlusToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＋" : "+");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.PlusToken);
		}

		private PunctuationSyntax MakeMinusToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "－" : "-");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.MinusToken);
		}

		private PunctuationSyntax MakeAsteriskToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＊" : "*");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.AsteriskToken);
		}

		private PunctuationSyntax MakeSlashToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "／" : "/");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.SlashToken);
		}

		private PunctuationSyntax MakeBackslashToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＼" : "\\");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.BackslashToken);
		}

		private PunctuationSyntax MakeCaretToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "\uff3e" : "^");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.CaretToken);
		}

		private PunctuationSyntax MakeExclamationToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "！" : "!");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.ExclamationToken);
		}

		private PunctuationSyntax MakeQuestionToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "？" : "?");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.QuestionToken);
		}

		private PunctuationSyntax MakeGreaterThanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＞" : ">");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.GreaterThanToken);
		}

		private PunctuationSyntax MakeLessThanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＜" : "<");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.LessThanToken);
		}

		private PunctuationSyntax MakeStatementTerminatorToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int width)
		{
			AdvanceChar(width);
			return SyntaxFactory.StatementTerminatorToken;
		}

		private PunctuationSyntax MakeAmpersandEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.AmpersandEqualsToken);
		}

		private PunctuationSyntax MakeColonEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.ColonEqualsToken);
		}

		private PunctuationSyntax MakePlusEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.PlusEqualsToken);
		}

		private PunctuationSyntax MakeMinusEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.MinusEqualsToken);
		}

		private PunctuationSyntax MakeAsteriskEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.AsteriskEqualsToken);
		}

		private PunctuationSyntax MakeSlashEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.SlashEqualsToken);
		}

		private PunctuationSyntax MakeBackSlashEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.BackslashEqualsToken);
		}

		private PunctuationSyntax MakeCaretEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.CaretEqualsToken);
		}

		private PunctuationSyntax MakeGreaterThanEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanEqualsToken);
		}

		private PunctuationSyntax MakeLessThanEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanEqualsToken);
		}

		private PunctuationSyntax MakeLessThanGreaterThanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanGreaterThanToken);
		}

		private PunctuationSyntax MakeLessThanLessThanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanLessThanToken);
		}

		private PunctuationSyntax MakeGreaterThanGreaterThanToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanGreaterThanToken);
		}

		private PunctuationSyntax MakeLessThanLessThanEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanLessThanEqualsToken);
		}

		private PunctuationSyntax MakeGreaterThanGreaterThanEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length)
		{
			return MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanGreaterThanEqualsToken);
		}

		private PunctuationSyntax MakeAtToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, bool charIsFullWidth)
		{
			string spelling = (charIsFullWidth ? "＠" : "@");
			AdvanceChar();
			return MakePunctuationToken(precedingTrivia, spelling, SyntaxKind.AtToken);
		}

		private SyntaxToken MakeIntegerLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, LiteralBase @base, TypeCharacter typeCharacter, ulong integralValue, int length)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia = ScanSingleLineTrivia();
			TokenParts key = new TokenParts(precedingTrivia, fTrivia, text);
			SyntaxToken value = null;
			if (_literalTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = SyntaxFactory.IntegerLiteralToken(text, @base, typeCharacter, integralValue, precedingTrivia.Node, fTrivia.Node);
			_literalTable.Add(key, value);
			return value;
		}

		private SyntaxToken MakeCharacterLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, char value, int length)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia = ScanSingleLineTrivia();
			TokenParts key = new TokenParts(precedingTrivia, fTrivia, text);
			SyntaxToken value2 = null;
			if (_literalTable.TryGetValue(key, out value2))
			{
				return value2;
			}
			value2 = SyntaxFactory.CharacterLiteralToken(text, value, precedingTrivia.Node, fTrivia.Node);
			_literalTable.Add(key, value2);
			return value2;
		}

		private SyntaxToken MakeDateLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, DateTime value, int length)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia = ScanSingleLineTrivia();
			TokenParts key = new TokenParts(precedingTrivia, fTrivia, text);
			SyntaxToken value2 = null;
			if (_literalTable.TryGetValue(key, out value2))
			{
				return value2;
			}
			value2 = SyntaxFactory.DateLiteralToken(text, value, precedingTrivia.Node, fTrivia.Node);
			_literalTable.Add(key, value2);
			return value2;
		}

		private SyntaxToken MakeFloatingLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, TypeCharacter typeCharacter, double floatingValue, int length)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia = ScanSingleLineTrivia();
			TokenParts key = new TokenParts(precedingTrivia, fTrivia, text);
			SyntaxToken value = null;
			if (_literalTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = SyntaxFactory.FloatingLiteralToken(text, typeCharacter, floatingValue, precedingTrivia.Node, fTrivia.Node);
			_literalTable.Add(key, value);
			return value;
		}

		private SyntaxToken MakeDecimalLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, TypeCharacter typeCharacter, decimal decimalValue, int length)
		{
			string text = GetText(length);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> fTrivia = ScanSingleLineTrivia();
			TokenParts key = new TokenParts(precedingTrivia, fTrivia, text);
			SyntaxToken value = null;
			if (_literalTable.TryGetValue(key, out value))
			{
				return value;
			}
			value = SyntaxFactory.DecimalLiteralToken(text, typeCharacter, decimalValue, precedingTrivia.Node, fTrivia.Node);
			_literalTable.Add(key, value);
			return value;
		}

		private SyntaxToken MakeBadToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length, ERRID errId)
		{
			string textNotInterned = GetTextNotInterned(length);
			return (SyntaxToken)SyntaxFactory.BadToken(followingTrivia: ScanSingleLineTrivia().Node, SubKind: SyntaxSubKind.None, text: textNotInterned, precedingTrivia: precedingTrivia.Node).AddError(ErrorFactory.ErrorInfo(errId));
		}

		private static SyntaxToken MakeEofToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return SyntaxFactory.Token(precedingTrivia.Node, SyntaxKind.EndOfFileToken, null, string.Empty);
		}

		private SyntaxToken MakeEofToken()
		{
			return _simpleEof;
		}

		internal virtual bool TryCrumbleOnce()
		{
			return false;
		}

		internal virtual VisualBasicSyntaxNode GetCurrentSyntaxNode()
		{
			return null;
		}

		internal virtual void MoveToNextSyntaxNode()
		{
			_prevToken = default(ScannerToken);
			_scannerPreprocessorState = _currentToken.PreprocessorState;
			ResetTokens();
		}

		internal virtual void MoveToNextSyntaxNodeInTrivia()
		{
			_prevToken = default(ScannerToken);
		}

		internal SyntaxToken GetCurrentToken()
		{
			SyntaxToken syntaxToken = _currentToken.InnerTokenObject;
			if (syntaxToken == null)
			{
				ScannerState state = _currentToken.State;
				syntaxToken = GetScannerToken(state);
				_currentToken = _currentToken.With(state, syntaxToken);
			}
			return syntaxToken;
		}

		internal void ResetCurrentToken(ScannerState state)
		{
			if (state != _currentToken.State)
			{
				if (_currentToken.State == ScannerState.VB && state == ScannerState.Content)
				{
					SyntaxToken currentToken = GetCurrentToken();
					AbandonAllTokens();
					int num = (_lineBufferOffset = _currentToken.Position + currentToken.GetLeadingTriviaWidth());
					SyntaxToken scannerToken = GetScannerToken(state);
					scannerToken = SyntaxToken.AddLeadingTrivia(scannerToken, currentToken.GetLeadingTrivia());
					_currentToken = _currentToken.With(state, scannerToken);
				}
				else
				{
					AbandonAllTokens();
					_currentToken = _currentToken.With(state, null);
				}
			}
		}

		internal void RescanTrailingColonAsToken(ref SyntaxToken prevToken, ref SyntaxToken currentToken)
		{
			SyntaxToken innerTokenObject = _prevToken.InnerTokenObject;
			AbandonAllTokens();
			RevertState(_prevToken);
			ScannerState state = ScannerState.VB;
			innerTokenObject = (SyntaxToken)innerTokenObject.WithTrailingTrivia(null);
			int fullWidth = innerTokenObject.FullWidth;
			_lineBufferOffset += fullWidth;
			_endOfTerminatorTrivia = _lineBufferOffset;
			_prevToken = _prevToken.With(state, innerTokenObject);
			prevToken = innerTokenObject;
			_currentToken = new ScannerToken(_scannerPreprocessorState, _lineBufferOffset, _endOfTerminatorTrivia, null, state);
			SyntaxListBuilder syntaxListBuilder = _triviaListPool.Allocate();
			ScanSingleLineTrivia(syntaxListBuilder);
			SyntaxTrivia syntaxTrivia = (SyntaxTrivia)syntaxListBuilder[syntaxListBuilder.Count - 1];
			syntaxListBuilder.RemoveLast();
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = MakeTriviaArray(syntaxListBuilder);
			_triviaListPool.Free(syntaxListBuilder);
			_lineBufferOffset = _endOfTerminatorTrivia;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList2 = ScanSingleLineTrivia();
			innerTokenObject = MakePunctuationToken(SyntaxKind.ColonToken, syntaxTrivia.Text, syntaxList, syntaxList2);
			_currentToken = _currentToken.With(state, innerTokenObject);
			currentToken = innerTokenObject;
		}

		internal void TransitionFromXmlToVB(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toCompare, ref Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toRemove, ref Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toAdd)
		{
			SyntaxToken innerTokenObject = _prevToken.InnerTokenObject;
			bool includeFollowingBlankLines = _currentToken.InnerTokenObject != null && _currentToken.InnerTokenObject.Kind == SyntaxKind.EndOfXmlToken;
			AbandonAllTokens();
			RevertState(_prevToken);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(innerTokenObject.GetTrailingTrivia());
			int indexOfEnd = syntaxList.Count - SyntaxNodeExtensions.GetLengthOfCommonEnd(syntaxList, toCompare);
			toRemove = SyntaxNodeExtensions.GetEndOfTrivia(syntaxList, indexOfEnd);
			innerTokenObject = (SyntaxToken)innerTokenObject.WithTrailingTrivia(SyntaxNodeExtensions.GetStartOfTrivia(syntaxList, indexOfEnd).Node);
			int fullWidth = GetFullWidth(_prevToken, innerTokenObject);
			_lineBufferOffset += fullWidth;
			_endOfTerminatorTrivia = _lineBufferOffset;
			toAdd = ScanSingleLineTrivia(includeFollowingBlankLines);
			ScannerState state = ScannerState.VB;
			innerTokenObject = SyntaxToken.AddTrailingTrivia(innerTokenObject, toAdd.Node);
			_prevToken = _prevToken.With(state, innerTokenObject);
			_currentToken = new ScannerToken(_scannerPreprocessorState, _lineBufferOffset, _endOfTerminatorTrivia, null, state);
		}

		internal void TransitionFromVBToXml(ScannerState state, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toCompare, ref Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toRemove, ref Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toAdd)
		{
			SyntaxToken innerTokenObject = _prevToken.InnerTokenObject;
			AbandonAllTokens();
			RevertState(_prevToken);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(innerTokenObject.GetTrailingTrivia());
			int indexOfEnd = syntaxList.Count - SyntaxNodeExtensions.GetLengthOfCommonEnd(syntaxList, toCompare);
			toRemove = SyntaxNodeExtensions.GetEndOfTrivia(syntaxList, indexOfEnd);
			innerTokenObject = (SyntaxToken)innerTokenObject.WithTrailingTrivia(SyntaxNodeExtensions.GetStartOfTrivia(syntaxList, indexOfEnd).Node);
			int fullWidth = GetFullWidth(_prevToken, innerTokenObject);
			_lineBufferOffset += fullWidth;
			_endOfTerminatorTrivia = _lineBufferOffset;
			toAdd = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			_prevToken = _prevToken.With(_prevToken.State, innerTokenObject);
			_currentToken = new ScannerToken(_scannerPreprocessorState, _lineBufferOffset, _endOfTerminatorTrivia, null, state);
		}

		private static int GetFullWidth(ScannerToken token, SyntaxToken tk)
		{
			if (tk.Width == 0 && SyntaxFacts.IsTerminator(tk.Kind))
			{
				return token.EndOfTerminatorTrivia - token.Position;
			}
			return tk.FullWidth;
		}

		internal void GetNextTokenInState(ScannerState state)
		{
			_prevToken = _currentToken;
			if (_tokens.Count == 0)
			{
				_currentToken = new ScannerToken(_scannerPreprocessorState, _lineBufferOffset, _endOfTerminatorTrivia, null, state);
				return;
			}
			_currentToken = _tokens[0];
			_tokens.RemoveAt(0);
			ResetCurrentToken(state);
		}

		internal SyntaxToken PeekNextToken(ScannerState state)
		{
			if (_tokens.Count > 0)
			{
				ScannerToken scannerToken = _tokens[0];
				if (scannerToken.State == state)
				{
					return scannerToken.InnerTokenObject;
				}
				AbandonPeekedTokens();
			}
			GetCurrentToken();
			return GetTokenAndAddToQueue(state);
		}

		internal SyntaxToken PeekToken(int tokenOffset, ScannerState state)
		{
			switch (tokenOffset)
			{
			case 0:
				return GetCurrentToken();
			case 1:
				return PeekNextToken(state);
			default:
			{
				int num = tokenOffset - 1;
				if (num == _tokens.Count)
				{
					return GetTokenAndAddToQueue(state);
				}
				if (num < _tokens.Count && _tokens[num].State == state)
				{
					return _tokens[num].InnerTokenObject;
				}
				RevertState(_tokens[num]);
				_tokens.RemoveRange(num, _tokens.Count - num);
				return GetTokenAndAddToQueue(state);
			}
			}
		}

		private SyntaxToken GetTokenAndAddToQueue(ScannerState state)
		{
			int lineBufferOffset = _lineBufferOffset;
			int endOfTerminatorTrivia = _endOfTerminatorTrivia;
			PreprocessorState scannerPreprocessorState = _scannerPreprocessorState;
			SyntaxToken scannerToken = GetScannerToken(state);
			_tokens.Add(new ScannerToken(scannerPreprocessorState, lineBufferOffset, endOfTerminatorTrivia, scannerToken, state));
			return scannerToken;
		}

		private void AbandonAllTokens()
		{
			RevertState(_currentToken);
			_tokens.Clear();
			_currentToken = _currentToken.With(ScannerState.VB, null);
		}

		private void ResetTokens()
		{
			_tokens.Clear();
			_currentToken = new ScannerToken(_scannerPreprocessorState, _lineBufferOffset, _endOfTerminatorTrivia, null, ScannerState.VB);
		}

		private void AbandonPeekedTokens()
		{
			if (_tokens.Count != 0)
			{
				RevertState(_tokens[0]);
				_tokens.Clear();
			}
		}

		internal RestorePoint CreateRestorePoint()
		{
			return new RestorePoint(this);
		}

		private ScannerToken[] SaveAndClearTokens()
		{
			if (_tokens.Count == 0)
			{
				return null;
			}
			ScannerToken[] result = _tokens.ToArray();
			_tokens.Clear();
			return result;
		}

		private void RestoreTokens(ScannerToken[] tokens)
		{
			_tokens.Clear();
			if (tokens != null)
			{
				_tokens.AddRange(tokens);
			}
		}

		private LineBufferAndEndOfTerminatorOffsets CreateOffsetRestorePoint()
		{
			return new LineBufferAndEndOfTerminatorOffsets(this);
		}

		private void ResetLineBufferOffset()
		{
			_lineBufferOffset = _currentToken.Position;
			_endOfTerminatorTrivia = _lineBufferOffset;
		}

		private void RevertState(ScannerToken revertTo)
		{
			_lineBufferOffset = revertTo.Position;
			_endOfTerminatorTrivia = revertTo.EndOfTerminatorTrivia;
			_scannerPreprocessorState = revertTo.PreprocessorState;
		}

		private SyntaxToken GetScannerToken(ScannerState state)
		{
			SyntaxToken syntaxToken = null;
			switch (state)
			{
			case ScannerState.VB:
				return GetNextToken();
			case ScannerState.VBAllowLeadingMultilineTrivia:
				return GetNextToken(!_isScanningDirective);
			case ScannerState.Misc:
				return ScanXmlMisc();
			case ScannerState.DocType:
			case ScannerState.Element:
			case ScannerState.EndElement:
				return ScanXmlElement(state);
			case ScannerState.Content:
				return ScanXmlContent();
			case ScannerState.CData:
				return ScanXmlCData();
			case ScannerState.StartProcessingInstruction:
			case ScannerState.ProcessingInstruction:
				return ScanXmlPIData(state);
			case ScannerState.Comment:
				return ScanXmlComment();
			case ScannerState.SingleQuotedString:
				return ScanXmlStringSingle();
			case ScannerState.SmartSingleQuotedString:
				return ScanXmlStringSmartSingle();
			case ScannerState.QuotedString:
				return ScanXmlStringDouble();
			case ScannerState.SmartQuotedString:
				return ScanXmlStringSmartDouble();
			case ScannerState.UnQuotedString:
				return ScanXmlStringUnQuoted();
			case ScannerState.InterpolatedStringPunctuation:
				return ScanInterpolatedStringPunctuation();
			case ScannerState.InterpolatedStringContent:
				return ScanInterpolatedStringContent();
			case ScannerState.InterpolatedStringFormatString:
				return ScanInterpolatedStringFormatString();
			default:
				throw ExceptionUtilities.UnexpectedValue(state);
			}
		}

		internal void ForceScanningXmlDocMode()
		{
			IsScanningXmlDoc = true;
			_isStartingFirstXmlDocLine = false;
			_doNotRequireXmlDocCommentPrefix = true;
		}

		private bool TryScanXmlDocComment(SyntaxListBuilder tList)
		{
			if (CanGet() && SyntaxFacts.IsWhitespace(Peek()))
			{
				VisualBasicSyntaxNode item = ScanWhitespace();
				tList.Add(item);
			}
			RestorePoint restorePoint = CreateRestorePoint();
			GetNextTokenInState(ScannerState.Content);
			DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = GetCurrentSyntaxNode() as DocumentationCommentTriviaSyntax;
			if (documentationCommentTriviaSyntax != null)
			{
				MoveToNextSyntaxNodeInTrivia();
			}
			else
			{
				Parser parser = new Parser(this);
				IsScanningXmlDoc = true;
				_isStartingFirstXmlDocLine = true;
				_endOfXmlInsteadOfLastDocCommentLineBreak = true;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = parser.ParseXmlContent(ScannerState.Content);
				_endOfXmlInsteadOfLastDocCommentLineBreak = false;
				if (syntaxList.Count == 0 && parser.CurrentToken.Kind == SyntaxKind.EndOfXmlToken)
				{
					ResetLineBufferOffset();
					restorePoint.RestoreTokens(includeLookAhead: false);
					_isStartingFirstXmlDocLine = true;
				}
				syntaxList = parser.ParseRestOfDocCommentContent(syntaxList);
				IsScanningXmlDoc = false;
				ResetLineBufferOffset();
				documentationCommentTriviaSyntax = SyntaxFactory.DocumentationCommentTrivia(syntaxList);
				if (Options.DocumentationMode < DocumentationMode.Diagnose)
				{
					documentationCommentTriviaSyntax.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics);
				}
			}
			restorePoint.RestoreTokens(includeLookAhead: true);
			tList.Add(documentationCommentTriviaSyntax);
			return true;
		}

		private bool TrySkipXmlDocMarker(ref int len)
		{
			int i;
			for (i = len; CanGet(i) && SyntaxFacts.IsWhitespace(Peek(i)); i++)
			{
			}
			if (StartsXmlDoc(i))
			{
				len = i + 3;
				return true;
			}
			return false;
		}

		private VisualBasicSyntaxNode ScanXmlDocTrivia()
		{
			int len = 0;
			if (TrySkipXmlDocMarker(ref len))
			{
				return MakeDocumentationCommentExteriorTrivia(GetText(len));
			}
			return null;
		}

		private bool ScanXmlTriviaInXmlDoc(char c, SyntaxListBuilder<VisualBasicSyntaxNode> triviaList)
		{
			int num = 0;
			while (true)
			{
				if (c == ' ' || c == '\t')
				{
					num++;
				}
				else
				{
					if (!SyntaxFacts.IsNewLine(c))
					{
						break;
					}
					if (num > 0)
					{
						triviaList.Add(MakeWhiteSpaceTrivia(GetText(num)));
						num = 0;
					}
					LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
					SyntaxTrivia node = ScanNewlineAsTrivia(c);
					int len = GetXmlWhitespaceLength(0);
					if (!TrySkipXmlDocMarker(ref len))
					{
						lineBufferAndEndOfTerminatorOffsets.Restore();
						return false;
					}
					triviaList.Add(node);
					triviaList.Add(MakeDocumentationCommentExteriorTrivia(GetText(len)));
				}
				c = Peek(num);
			}
			if (num > 0)
			{
				triviaList.Add(MakeWhiteSpaceTrivia(GetText(num)));
			}
			return true;
		}

		private SyntaxToken ScanXmlContentInXmlDoc()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			if (IsAtNewLine() || _isStartingFirstXmlDocLine)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				_isStartingFirstXmlDocLine = false;
				if (visualBasicSyntaxNode == null)
				{
					return MakeEofToken();
				}
				precedingTrivia = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(visualBasicSyntaxNode);
			}
			int num = 0;
			StringBuilder scratch = GetScratch();
			while (CanGet(num))
			{
				char c = Peek(num);
				switch (c)
				{
				case '\n':
				case '\r':
					if (num != 0)
					{
						return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
					}
					num = SkipLineBreak(c, num);
					if (_endOfXmlInsteadOfLastDocCommentLineBreak)
					{
						int len = num;
						if (!TrySkipXmlDocMarker(ref len))
						{
							ResetLineBufferOffset();
							return SyntaxFactory.Token(null, SyntaxKind.EndOfXmlToken, null, string.Empty);
						}
					}
					return MakeDocCommentLineBreakToken(precedingTrivia, num);
				case '\t':
				case ' ':
					scratch.Append(c);
					num++;
					continue;
				case '&':
					if (num != 0)
					{
						return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
					}
					return ScanXmlReference(precedingTrivia);
				case '<':
					if (num != 0)
					{
						return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
					}
					if (CanGet(1))
					{
						switch (Peek(1))
						{
						case '!':
							if (!CanGet(2))
							{
								break;
							}
							switch (Peek(2))
							{
							case '-':
								if (NextIs(3, '-'))
								{
									return XmlMakeBeginCommentToken(precedingTrivia, s_scanNoTriviaFunc);
								}
								break;
							case '[':
								if (NextAre(3, "CDATA["))
								{
									return XmlMakeBeginCDataToken(precedingTrivia, s_scanNoTriviaFunc);
								}
								break;
							case 'D':
								if (NextAre(3, "OCTYPE"))
								{
									return XmlMakeBeginDTDToken(precedingTrivia);
								}
								break;
							}
							break;
						case '?':
							return XmlMakeBeginProcessingInstructionToken(precedingTrivia, s_scanNoTriviaFunc);
						case '/':
							return XmlMakeBeginEndElementToken(precedingTrivia, s_scanNoTriviaFunc);
						}
					}
					return XmlMakeLessToken(precedingTrivia);
				case ']':
					if (NextAre(num + 1, "]>"))
					{
						if (num != 0)
						{
							return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
						}
						return XmlMakeTextLiteralToken(precedingTrivia, 3, ERRID.ERR_XmlEndCDataNotAllowedInContent);
					}
					break;
				}
				XmlCharResult xmlCharResult = ScanXmlChar(num);
				if (xmlCharResult.Length == 0)
				{
					if (num > 0)
					{
						return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
					}
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalChar);
				}
				xmlCharResult.AppendTo(scratch);
				num += xmlCharResult.Length;
			}
			if (num > 0)
			{
				return XmlMakeTextLiteralToken(precedingTrivia, num, scratch);
			}
			return MakeEofToken(precedingTrivia);
		}

		internal SyntaxToken ScanXmlPIDataInXmlDoc(ScannerState state)
		{
			SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = _triviaListPool.Allocate<VisualBasicSyntaxNode>();
			if (IsAtNewLine())
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				if (visualBasicSyntaxNode == null)
				{
					return MakeEofToken();
				}
				syntaxListBuilder.Add(visualBasicSyntaxNode);
			}
			SyntaxToken result;
			if (state == ScannerState.StartProcessingInstruction && CanGet())
			{
				char c = Peek();
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					break;
				default:
					goto IL_00af;
				}
				LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
				if (!ScanXmlTriviaInXmlDoc(c, syntaxListBuilder))
				{
					lineBufferAndEndOfTerminatorOffsets.Restore();
					result = SyntaxFactory.Token(syntaxListBuilder.ToList().Node, SyntaxKind.EndOfXmlToken, null, string.Empty);
					goto IL_019c;
				}
			}
			goto IL_00af;
			IL_019c:
			_triviaListPool.Free(syntaxListBuilder);
			return result;
			IL_00af:
			int num = 0;
			while (true)
			{
				if (CanGet(num))
				{
					char c2 = Peek(num);
					if (c2 != '\n' && c2 != '\r')
					{
						if (c2 == '?' && NextIs(num + 1, '>'))
						{
							result = ((num == 0) ? ((SyntaxToken)XmlMakeEndProcessingInstructionToken(syntaxListBuilder.ToList())) : ((SyntaxToken)XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num)));
							break;
						}
						XmlCharResult xmlCharResult = ScanXmlChar(num);
						if (xmlCharResult.Length > 0)
						{
							num += xmlCharResult.Length;
							continue;
						}
						result = ((num == 0) ? ((SyntaxToken)XmlMakeBadToken(syntaxListBuilder.ToList(), 1, ERRID.ERR_IllegalChar)) : ((SyntaxToken)XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num)));
						break;
					}
					result = XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num + LengthOfLineBreak(c2, num));
					break;
				}
				result = ((num <= 0) ? MakeEofToken(syntaxListBuilder.ToList()) : XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), num));
				break;
			}
			goto IL_019c;
		}

		private SyntaxToken ScanXmlElementInXmlDoc(ScannerState state)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			if (IsAtNewLine() && !_doNotRequireXmlDocCommentPrefix)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlDocTrivia();
				if (visualBasicSyntaxNode == null)
				{
					return MakeEofToken();
				}
				precedingTrivia = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(visualBasicSyntaxNode);
			}
			while (CanGet())
			{
				if (!precedingTrivia.Any() && IsAtNewLine() && !_doNotRequireXmlDocCommentPrefix)
				{
					return MakeEofToken(precedingTrivia);
				}
				char c = Peek();
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					break;
				case '/':
					if (NextIs(1, '>'))
					{
						return XmlMakeEndEmptyElementToken(precedingTrivia);
					}
					return XmlMakeDivToken(precedingTrivia);
				case '>':
					return XmlMakeGreaterToken(precedingTrivia);
				case '=':
					return XmlMakeEqualsToken(precedingTrivia);
				case '\'':
				case '‘':
				case '’':
					return XmlMakeSingleQuoteToken(precedingTrivia, c, isOpening: true);
				case '"':
				case '“':
				case '”':
					return XmlMakeDoubleQuoteToken(precedingTrivia, c, isOpening: true);
				case '<':
					if (CanGet(1))
					{
						switch (Peek(1))
						{
						case '!':
							if (CanGet(2))
							{
								switch (Peek(2))
								{
								case '-':
									if (NextIs(3, '-'))
									{
										return XmlMakeBeginCommentToken(precedingTrivia, s_scanNoTriviaFunc);
									}
									break;
								case '[':
									if (NextAre(3, "CDATA["))
									{
										return XmlMakeBeginCDataToken(precedingTrivia, s_scanNoTriviaFunc);
									}
									break;
								case 'D':
									if (NextAre(3, "OCTYPE"))
									{
										return XmlMakeBeginDTDToken(precedingTrivia);
									}
									break;
								}
							}
							return XmlLessThanExclamationToken(state, precedingTrivia);
						case '?':
							return XmlMakeBeginProcessingInstructionToken(precedingTrivia, s_scanNoTriviaFunc);
						case '/':
							return XmlMakeBeginEndElementToken(precedingTrivia, s_scanNoTriviaFunc);
						}
					}
					return XmlMakeLessToken(precedingTrivia);
				case '?':
					if (NextIs(1, '>'))
					{
						return XmlMakeEndProcessingInstructionToken(precedingTrivia);
					}
					return MakeQuestionToken(precedingTrivia, charIsFullWidth: false);
				case '(':
					return XmlMakeLeftParenToken(precedingTrivia);
				case ')':
					return XmlMakeRightParenToken(precedingTrivia);
				case '!':
				case '#':
				case ',':
				case ';':
				case '}':
					return XmlMakeBadToken(precedingTrivia, 1, ERRID.ERR_IllegalXmlNameChar);
				case ':':
					return XmlMakeColonToken(precedingTrivia);
				case '[':
					return XmlMakeOpenBracketToken(state, precedingTrivia);
				case ']':
					return XmlMakeCloseBracketToken(state, precedingTrivia);
				default:
					return ScanXmlNcName(precedingTrivia);
				}
				LineBufferAndEndOfTerminatorOffsets lineBufferAndEndOfTerminatorOffsets = CreateOffsetRestorePoint();
				SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = _triviaListPool.Allocate<VisualBasicSyntaxNode>();
				bool num = ScanXmlTriviaInXmlDoc(c, syntaxListBuilder);
				precedingTrivia = syntaxListBuilder.ToList();
				_triviaListPool.Free(syntaxListBuilder);
				if (!num)
				{
					lineBufferAndEndOfTerminatorOffsets.Restore();
					return SyntaxFactory.Token(precedingTrivia.Node, SyntaxKind.EndOfXmlToken, null, string.Empty);
				}
			}
			return MakeEofToken(precedingTrivia);
		}

		private static SyntaxToken MakeMissingToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, SyntaxKind kind)
		{
			SyntaxToken syntaxToken = SyntaxFactory.MissingToken(kind);
			if (precedingTrivia.Any())
			{
				syntaxToken = (SyntaxToken)syntaxToken.WithLeadingTrivia(precedingTrivia.Node);
			}
			return syntaxToken;
		}

		private PunctuationSyntax XmlMakeLeftParenToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.OpenParenToken, "(", precedingTrivia, visualBasicSyntaxNode);
		}

		private PunctuationSyntax XmlMakeRightParenToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.CloseParenToken, ")", precedingTrivia, visualBasicSyntaxNode);
		}

		private PunctuationSyntax XmlMakeEqualsToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.EqualsToken, "=", precedingTrivia, visualBasicSyntaxNode);
		}

		private PunctuationSyntax XmlMakeDivToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.SlashToken, "/", precedingTrivia, visualBasicSyntaxNode);
		}

		private PunctuationSyntax XmlMakeColonToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.ColonToken, ":", precedingTrivia, visualBasicSyntaxNode);
		}

		private PunctuationSyntax XmlMakeGreaterToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			return MakePunctuationToken(SyntaxKind.GreaterThanToken, ">", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private PunctuationSyntax XmlMakeLessToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar();
			VisualBasicSyntaxNode visualBasicSyntaxNode = ScanXmlWhitespace();
			return MakePunctuationToken(SyntaxKind.LessThanToken, "<", precedingTrivia, visualBasicSyntaxNode);
		}

		private BadTokenSyntax XmlMakeBadToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length, ERRID id)
		{
			return XmlMakeBadToken(SyntaxSubKind.None, precedingTrivia, length, id);
		}

		private BadTokenSyntax XmlMakeBadToken(SyntaxSubKind subkind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int length, ERRID id)
		{
			string textNotInterned = GetTextNotInterned(length);
			VisualBasicSyntaxNode followingTrivia = ScanXmlWhitespace();
			BadTokenSyntax badTokenSyntax = SyntaxFactory.BadToken(subkind, textNotInterned, precedingTrivia.Node, followingTrivia);
			ERRID eRRID = id;
			DiagnosticInfo diagnosticInfo;
			if ((uint)(eRRID - 31169) <= 1u)
			{
				if (id == ERRID.ERR_IllegalXmlNameChar && (precedingTrivia.Any() || PrevToken == null || PrevToken.HasTrailingTrivia || PrevToken.Kind == SyntaxKind.LessThanToken || PrevToken.Kind == SyntaxKind.LessThanSlashToken || PrevToken.Kind == SyntaxKind.LessThanQuestionToken))
				{
					id = ERRID.ERR_IllegalXmlStartNameChar;
				}
				char c = textNotInterned[0];
				int num = XmlCharacterGlobalHelpers.UTF16ToUnicode(new XmlCharResult(c));
				diagnosticInfo = ErrorFactory.ErrorInfo(id, c, $"&H{num:X}");
			}
			else
			{
				diagnosticInfo = ErrorFactory.ErrorInfo(id);
			}
			return (BadTokenSyntax)badTokenSyntax.SetDiagnostics(new DiagnosticInfo[1] { diagnosticInfo });
		}

		private PunctuationSyntax XmlMakeSingleQuoteToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, char spelling, bool isOpening)
		{
			AdvanceChar();
			GreenNode greenNode = null;
			if (!isOpening)
			{
				greenNode = ScanXmlWhitespace();
			}
			return MakePunctuationToken(SyntaxKind.SingleQuoteToken, Intern(spelling), precedingTrivia, greenNode);
		}

		private PunctuationSyntax XmlMakeDoubleQuoteToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, char spelling, bool isOpening)
		{
			AdvanceChar();
			GreenNode greenNode = null;
			if (!isOpening)
			{
				greenNode = ScanXmlWhitespace();
			}
			return MakePunctuationToken(SyntaxKind.DoubleQuoteToken, Intern(spelling), precedingTrivia, greenNode);
		}

		private XmlNameTokenSyntax XmlMakeXmlNCNameToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth)
		{
			string text = GetText(TokenWidth);
			SyntaxKind syntaxKind = SyntaxKind.XmlNameToken;
			int length = text.Length;
			if (length == 3 && string.Equals(text, "xml", StringComparison.Ordinal))
			{
				syntaxKind = SyntaxKind.XmlKeyword;
			}
			if (syntaxKind == SyntaxKind.XmlNameToken)
			{
				syntaxKind = TokenOfStringCached(text);
				if (syntaxKind == SyntaxKind.IdentifierToken)
				{
					syntaxKind = SyntaxKind.XmlNameToken;
				}
			}
			VisualBasicSyntaxNode trailingTrivia = ScanXmlWhitespace();
			return SyntaxFactory.XmlNameToken(text, syntaxKind, precedingTrivia.Node, trailingTrivia);
		}

		private XmlTextTokenSyntax XmlMakeAttributeDataToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, string Value)
		{
			return SyntaxFactory.XmlTextLiteralToken(GetTextNotInterned(TokenWidth), Value, precedingTrivia.Node, null);
		}

		private XmlTextTokenSyntax XmlMakeAttributeDataToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, StringBuilder Scratch)
		{
			return XmlMakeTextLiteralToken(precedingTrivia, TokenWidth, Scratch);
		}

		private XmlTextTokenSyntax XmlMakeEntityLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, string Value)
		{
			return SyntaxFactory.XmlEntityLiteralToken(GetText(TokenWidth), Value, precedingTrivia.Node, null);
		}

		private XmlTextTokenSyntax XmlMakeAmpLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(5);
			if (precedingTrivia.Node != null)
			{
				return SyntaxFactory.XmlEntityLiteralToken("&amp;", "&", precedingTrivia.Node, null);
			}
			return s_xmlAmpToken;
		}

		private XmlTextTokenSyntax XmlMakeAposLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(6);
			if (precedingTrivia.Node != null)
			{
				return SyntaxFactory.XmlEntityLiteralToken("&apos;", "'", precedingTrivia.Node, null);
			}
			return s_xmlAposToken;
		}

		private XmlTextTokenSyntax XmlMakeGtLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(4);
			if (precedingTrivia.Node != null)
			{
				return SyntaxFactory.XmlEntityLiteralToken("&gt;", "&", precedingTrivia.Node, null);
			}
			return s_xmlGtToken;
		}

		private XmlTextTokenSyntax XmlMakeLtLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(4);
			if (precedingTrivia.Node != null)
			{
				return SyntaxFactory.XmlEntityLiteralToken("&lt;", "<", precedingTrivia.Node, null);
			}
			return s_xmlLtToken;
		}

		private XmlTextTokenSyntax XmlMakeQuotLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(6);
			if (precedingTrivia.Node != null)
			{
				return SyntaxFactory.XmlEntityLiteralToken("&quot;", "\"", precedingTrivia.Node, null);
			}
			return s_xmlQuotToken;
		}

		private XmlTextTokenSyntax XmlMakeTextLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, StringBuilder Scratch)
		{
			string textNotInterned = GetTextNotInterned(TokenWidth);
			string scratchText = GetScratchText(Scratch, textNotInterned);
			return SyntaxFactory.XmlTextLiteralToken(textNotInterned, scratchText, precedingTrivia.Node, null);
		}

		private XmlTextTokenSyntax MakeDocCommentLineBreakToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth)
		{
			string text = GetText(TokenWidth);
			if (precedingTrivia.Node == null && EmbeddedOperators.CompareString(text, "\r\n", TextCompare: false) == 0)
			{
				return s_docCommentCrLfToken;
			}
			return SyntaxFactory.DocumentationCommentLineBreakToken(text, "\n", precedingTrivia.Node, null);
		}

		private XmlTextTokenSyntax XmlMakeTextLiteralToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, ERRID err)
		{
			string textNotInterned = GetTextNotInterned(TokenWidth);
			return (XmlTextTokenSyntax)SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, null).SetDiagnostics(new DiagnosticInfo[1] { ErrorFactory.ErrorInfo(err) });
		}

		private PunctuationSyntax XmlMakeBeginEndElementToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, ScanTriviaFunc scanTrailingTrivia)
		{
			AdvanceChar(2);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = scanTrailingTrivia();
			return MakePunctuationToken(SyntaxKind.LessThanSlashToken, "</", precedingTrivia, syntaxList);
		}

		private PunctuationSyntax XmlMakeEndEmptyElementToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(2);
			return MakePunctuationToken(SyntaxKind.SlashGreaterThanToken, "/>", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private PunctuationSyntax XmlMakeBeginEmbeddedToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(3);
			return MakePunctuationToken(SyntaxKind.LessThanPercentEqualsToken, "<%=", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private PunctuationSyntax XmlMakeEndEmbeddedToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, ScanTriviaFunc scanTrailingTrivia)
		{
			string spelling;
			if (Peek() == '%')
			{
				AdvanceChar(2);
				spelling = "%>";
			}
			else
			{
				spelling = GetText(2);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = scanTrailingTrivia();
			return MakePunctuationToken(SyntaxKind.PercentGreaterThanToken, spelling, precedingTrivia, syntaxList);
		}

		private BadTokenSyntax XmlMakeBeginDTDToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return XmlMakeBadToken(SyntaxSubKind.BeginDocTypeToken, precedingTrivia, 9, ERRID.ERR_DTDNotSupported);
		}

		private BadTokenSyntax XmlLessThanExclamationToken(ScannerState state, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return XmlMakeBadToken(SyntaxSubKind.LessThanExclamationToken, precedingTrivia, 2, (state == ScannerState.DocType) ? ERRID.ERR_DTDNotSupported : ERRID.ERR_Syntax);
		}

		private BadTokenSyntax XmlMakeOpenBracketToken(ScannerState state, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return XmlMakeBadToken(SyntaxSubKind.OpenBracketToken, precedingTrivia, 1, (state == ScannerState.DocType) ? ERRID.ERR_DTDNotSupported : ERRID.ERR_IllegalXmlNameChar);
		}

		private BadTokenSyntax XmlMakeCloseBracketToken(ScannerState state, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			return XmlMakeBadToken(SyntaxSubKind.CloseBracketToken, precedingTrivia, 1, (state == ScannerState.DocType) ? ERRID.ERR_DTDNotSupported : ERRID.ERR_IllegalXmlNameChar);
		}

		private PunctuationSyntax XmlMakeBeginProcessingInstructionToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, ScanTriviaFunc scanTrailingTrivia)
		{
			AdvanceChar(2);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = scanTrailingTrivia();
			return MakePunctuationToken(SyntaxKind.LessThanQuestionToken, "<?", precedingTrivia, syntaxList);
		}

		private XmlTextTokenSyntax XmlMakeProcessingInstructionToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth)
		{
			string textNotInterned = GetTextNotInterned(TokenWidth);
			return SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, null);
		}

		private PunctuationSyntax XmlMakeEndProcessingInstructionToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(2);
			return MakePunctuationToken(SyntaxKind.QuestionGreaterThanToken, "?>", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private PunctuationSyntax XmlMakeBeginCommentToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, ScanTriviaFunc scanTrailingTrivia)
		{
			AdvanceChar(4);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = scanTrailingTrivia();
			return MakePunctuationToken(SyntaxKind.LessThanExclamationMinusMinusToken, "<!--", precedingTrivia, syntaxList);
		}

		private XmlTextTokenSyntax XmlMakeCommentToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth)
		{
			string textNotInterned = GetTextNotInterned(TokenWidth);
			return SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, null);
		}

		private PunctuationSyntax XmlMakeEndCommentToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(3);
			return MakePunctuationToken(SyntaxKind.MinusMinusGreaterThanToken, "-->", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private PunctuationSyntax XmlMakeBeginCDataToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, ScanTriviaFunc scanTrailingTrivia)
		{
			AdvanceChar(9);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = scanTrailingTrivia();
			return MakePunctuationToken(SyntaxKind.BeginCDataToken, "<![CDATA[", precedingTrivia, syntaxList);
		}

		private XmlTextTokenSyntax XmlMakeCDataToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia, int TokenWidth, StringBuilder scratch)
		{
			return XmlMakeTextLiteralToken(precedingTrivia, TokenWidth, scratch);
		}

		private PunctuationSyntax XmlMakeEndCDataToken(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> precedingTrivia)
		{
			AdvanceChar(3);
			return MakePunctuationToken(SyntaxKind.EndCDataToken, "]]>", precedingTrivia, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}
	}
}
