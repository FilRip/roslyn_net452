using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	internal class SyntaxNormalizer : VisualBasicSyntaxRewriter
	{
		private readonly TextSpan _consideredSpan;

		private readonly string _indentWhitespace;

		private readonly SyntaxTrivia _eolTrivia;

		private readonly bool _useElasticTrivia;

		private readonly bool _useDefaultCasing;

		private bool _isInStructuredTrivia;

		private SyntaxToken _previousToken;

		private bool _afterLineBreak;

		private bool _afterIndentation;

		private readonly Dictionary<SyntaxToken, int> _lineBreaksAfterToken;

		private readonly HashSet<SyntaxNode> _lastStatementsInBlocks;

		private int _indentationDepth;

		private ArrayBuilder<SyntaxTrivia> _indentations;

		private SyntaxNormalizer(TextSpan consideredSpan, string indentWhitespace, string eolWhitespace, bool useElasticTrivia, bool useDefaultCasing)
			: base(visitIntoStructuredTrivia: true)
		{
			_lineBreaksAfterToken = new Dictionary<SyntaxToken, int>();
			_lastStatementsInBlocks = new HashSet<SyntaxNode>();
			_consideredSpan = consideredSpan;
			_indentWhitespace = indentWhitespace;
			_useElasticTrivia = useElasticTrivia;
			_eolTrivia = (useElasticTrivia ? SyntaxFactory.ElasticEndOfLine(eolWhitespace) : SyntaxFactory.EndOfLine(eolWhitespace));
			_useDefaultCasing = useDefaultCasing;
			_afterLineBreak = true;
		}

		internal static SyntaxNode Normalize<TNode>(TNode node, string indentWhitespace, string eolWhitespace, bool useElasticTrivia, bool useDefaultCasing) where TNode : SyntaxNode
		{
			SyntaxNormalizer syntaxNormalizer = new SyntaxNormalizer(node.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing);
			TNode val = (TNode)syntaxNormalizer.Visit(node);
			syntaxNormalizer.Free();
			return val;
		}

		internal static SyntaxToken Normalize(SyntaxToken token, string indentWhitespace, string eolWhitespace, bool useElasticTrivia, bool useDefaultCasing)
		{
			SyntaxNormalizer syntaxNormalizer = new SyntaxNormalizer(token.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing);
			SyntaxToken result = syntaxNormalizer.VisitToken(token);
			syntaxNormalizer.Free();
			return result;
		}

		internal static SyntaxTriviaList Normalize(SyntaxTriviaList trivia, string indentWhitespace, string eolWhitespace, bool useElasticTrivia, bool useDefaultCasing)
		{
			SyntaxNormalizer syntaxNormalizer = new SyntaxNormalizer(trivia.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing);
			SyntaxTriviaList result = syntaxNormalizer.RewriteTrivia(trivia, syntaxNormalizer.GetIndentationDepth(), isTrailing: false, mustBeIndented: false, mustHaveSeparator: false, 0, 0);
			syntaxNormalizer.Free();
			return result;
		}

		private void Free()
		{
			if (_indentations != null)
			{
				_indentations.Free();
			}
		}

		private SyntaxTrivia GetIndentation(int count)
		{
			int capacity = count + 1;
			if (_indentations == null)
			{
				_indentations = ArrayBuilder<SyntaxTrivia>.GetInstance(capacity);
			}
			else
			{
				_indentations.EnsureCapacity(capacity);
			}
			for (int i = _indentations.Count; i <= count; i++)
			{
				string text = ((i == 0) ? "" : (_indentations[i - 1].ToString() + _indentWhitespace));
				_indentations.Add(_useElasticTrivia ? SyntaxFactory.ElasticWhitespace(text) : SyntaxFactory.Whitespace(text));
			}
			return _indentations[count];
		}

		public override SyntaxToken VisitToken(SyntaxToken token)
		{
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.None)
			{
				return token;
			}
			try
			{
				SyntaxToken syntaxToken = ((!_useDefaultCasing || !VisualBasicExtensions.IsKeyword(token)) ? token : SyntaxFactory.Token(VisualBasicExtensions.Kind(token)));
				int indentationDepth = GetIndentationDepth();
				int num = LineBreaksBetween(_previousToken, token);
				bool mustBeIndented = num > 0;
				if (num > 0 && IsLastTokenOnLine(_previousToken))
				{
					num--;
				}
				syntaxToken = syntaxToken.WithLeadingTrivia(RewriteTrivia(token.LeadingTrivia, indentationDepth, isTrailing: false, mustBeIndented, mustHaveSeparator: false, 0, num));
				SyntaxToken nextRelevantToken = GetNextRelevantToken(token);
				_afterIndentation = false;
				int num2 = ((LineBreaksBetween(token, nextRelevantToken) > 0) ? 1 : 0);
				bool mustHaveSeparator = num2 <= 0 && NeedsSeparator(token, nextRelevantToken);
				syntaxToken = syntaxToken.WithTrailingTrivia(RewriteTrivia(token.TrailingTrivia, 0, isTrailing: true, mustBeIndented: false, mustHaveSeparator, num2, 0));
				if (VisualBasicExtensions.Kind(syntaxToken) == SyntaxKind.DocumentationCommentLineBreakToken)
				{
					_afterLineBreak = true;
				}
				else if (VisualBasicExtensions.Kind(syntaxToken) == SyntaxKind.XmlTextLiteralToken && syntaxToken.TrailingTrivia.Count == 0 && IsNewLineChar(syntaxToken.ValueText.Last()))
				{
					_afterLineBreak = true;
				}
				return syntaxToken;
			}
			finally
			{
				_previousToken = token;
			}
		}

		private SyntaxTriviaList RewriteTrivia(SyntaxTriviaList triviaList, int depth, bool isTrailing, bool mustBeIndented, bool mustHaveSeparator, int lineBreaksAfter, int lineBreaksBefore)
		{
			ArrayBuilder<SyntaxTrivia> instance = ArrayBuilder<SyntaxTrivia>.GetInstance();
			try
			{
				for (int i = 1; i <= lineBreaksBefore; i++)
				{
					instance.Add(GetEndOfLine());
					_afterLineBreak = true;
					_afterIndentation = false;
				}
				SyntaxTriviaList.Enumerator enumerator = triviaList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia syntaxTrivia = enumerator.Current;
					if (VisualBasicExtensions.Kind(syntaxTrivia) == SyntaxKind.WhitespaceTrivia || VisualBasicExtensions.Kind(syntaxTrivia) == SyntaxKind.EndOfLineTrivia || VisualBasicExtensions.Kind(syntaxTrivia) == SyntaxKind.LineContinuationTrivia || syntaxTrivia.FullWidth == 0)
					{
						continue;
					}
					SyntaxNode parent = syntaxTrivia.Token.Parent;
					bool flag = (VisualBasicExtensions.Kind(syntaxTrivia) != SyntaxKind.ColonTrivia || parent == null || VisualBasicExtensions.Kind(parent) != SyntaxKind.LabelStatement) && (parent == null || parent.Parent == null || VisualBasicExtensions.Kind(parent.Parent) != SyntaxKind.CrefReference) && ((instance.Count > 0 && NeedsSeparatorBetween(instance.Last()) && !EndsInLineBreak(instance.Last())) || (instance.Count == 0 && isTrailing));
					if ((NeedsLineBreakBefore(syntaxTrivia) || (instance.Count > 0 && NeedsLineBreakBetween(instance.Last(), syntaxTrivia, isTrailing))) && !_afterLineBreak)
					{
						instance.Add(GetEndOfLine());
						_afterLineBreak = true;
						_afterIndentation = false;
					}
					if (_afterLineBreak && !isTrailing)
					{
						if (!_afterIndentation && NeedsIndentAfterLineBreak(syntaxTrivia))
						{
							instance.Add(GetIndentation(GetIndentationDepth(syntaxTrivia)));
							_afterIndentation = true;
						}
					}
					else if (flag)
					{
						instance.Add(GetSpace());
						_afterLineBreak = false;
						_afterIndentation = false;
					}
					if (syntaxTrivia.HasStructure)
					{
						SyntaxTrivia item = VisitStructuredTrivia(syntaxTrivia);
						instance.Add(item);
					}
					else
					{
						if (VisualBasicExtensions.Kind(syntaxTrivia) == SyntaxKind.DocumentationCommentExteriorTrivia)
						{
							syntaxTrivia = SyntaxFactory.DocumentationCommentExteriorTrivia(SyntaxFacts.GetText(SyntaxKind.DocumentationCommentExteriorTrivia));
						}
						instance.Add(syntaxTrivia);
					}
					if (NeedsLineBreakAfter(syntaxTrivia))
					{
						if (!isTrailing)
						{
							instance.Add(GetEndOfLine());
							_afterLineBreak = true;
							_afterIndentation = false;
						}
					}
					else
					{
						_afterLineBreak = EndsInLineBreak(syntaxTrivia);
					}
				}
				if (lineBreaksAfter > 0)
				{
					if (instance.Count > 0 && EndsInLineBreak(instance.Last()))
					{
						lineBreaksAfter--;
					}
					int num = lineBreaksAfter - 1;
					for (int j = 0; j <= num; j++)
					{
						instance.Add(GetEndOfLine());
						_afterLineBreak = true;
						_afterIndentation = false;
					}
				}
				else if (mustHaveSeparator)
				{
					instance.Add(GetSpace());
					_afterLineBreak = false;
					_afterIndentation = false;
				}
				if (mustBeIndented)
				{
					instance.Add(GetIndentation(depth));
					_afterIndentation = true;
					_afterLineBreak = false;
				}
				if (instance.Count == 0)
				{
					if (_useElasticTrivia)
					{
						return SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker);
					}
					return default(SyntaxTriviaList);
				}
				if (instance.Count == 1)
				{
					return SyntaxFactory.TriviaList(instance.First());
				}
				return SyntaxFactory.TriviaList(instance);
			}
			finally
			{
				instance.Free();
			}
		}

		private bool IsLastTokenOnLine(SyntaxToken token)
		{
			if (!token.HasTrailingTrivia || VisualBasicExtensions.Kind(token.TrailingTrivia.Last()) != SyntaxKind.ColonTrivia)
			{
				if (token.Parent != null)
				{
					return token.Parent!.GetLastToken() == token;
				}
				return false;
			}
			return true;
		}

		private int LineBreaksBetween(SyntaxToken currentToken, SyntaxToken nextToken)
		{
			if (VisualBasicExtensions.Kind(currentToken) == SyntaxKind.None || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.None)
			{
				return 0;
			}
			int value = 0;
			if (_lineBreaksAfterToken.TryGetValue(currentToken, out value))
			{
				return value;
			}
			return 0;
		}

		private int GetIndentationDepth()
		{
			return _indentationDepth;
		}

		private int GetIndentationDepth(SyntaxTrivia trivia)
		{
			if (SyntaxFacts.IsPreprocessorDirective(VisualBasicExtensions.Kind(trivia)))
			{
				return 0;
			}
			return GetIndentationDepth();
		}

		private SyntaxTrivia GetSpace()
		{
			if (!_useElasticTrivia)
			{
				return SyntaxFactory.Space;
			}
			return SyntaxFactory.ElasticSpace;
		}

		private SyntaxTrivia GetEndOfLine()
		{
			return _eolTrivia;
		}

		private bool NeedsSeparatorBetween(SyntaxTrivia trivia)
		{
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(trivia);
			if (syntaxKind == SyntaxKind.None || syntaxKind == SyntaxKind.WhitespaceTrivia || syntaxKind - 733 <= SyntaxKind.List)
			{
				return false;
			}
			return !SyntaxFacts.IsPreprocessorDirective(VisualBasicExtensions.Kind(trivia));
		}

		private bool NeedsLineBreakBetween(SyntaxTrivia trivia, SyntaxTrivia nextTrivia, bool isTrailingTrivia)
		{
			if (EndsInLineBreak(trivia))
			{
				return false;
			}
			switch (VisualBasicExtensions.Kind(nextTrivia))
			{
			case SyntaxKind.EmptyStatement:
			case SyntaxKind.CommentTrivia:
			case SyntaxKind.DocumentationCommentExteriorTrivia:
			case SyntaxKind.ConstDirectiveTrivia:
			case SyntaxKind.IfDirectiveTrivia:
			case SyntaxKind.ElseIfDirectiveTrivia:
			case SyntaxKind.ElseDirectiveTrivia:
			case SyntaxKind.EndIfDirectiveTrivia:
			case SyntaxKind.RegionDirectiveTrivia:
			case SyntaxKind.EndRegionDirectiveTrivia:
			case SyntaxKind.ExternalSourceDirectiveTrivia:
			case SyntaxKind.EndExternalSourceDirectiveTrivia:
			case SyntaxKind.ExternalChecksumDirectiveTrivia:
			case SyntaxKind.EnableWarningDirectiveTrivia:
			case SyntaxKind.DisableWarningDirectiveTrivia:
			case SyntaxKind.ReferenceDirectiveTrivia:
			case SyntaxKind.BadDirectiveTrivia:
				return !isTrailingTrivia;
			default:
				return false;
			}
		}

		private bool NeedsLineBreakAfter(SyntaxTrivia trivia)
		{
			return VisualBasicExtensions.Kind(trivia) == SyntaxKind.CommentTrivia;
		}

		private bool NeedsLineBreakBefore(SyntaxTrivia trivia)
		{
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(trivia);
			if (syntaxKind == SyntaxKind.DocumentationCommentExteriorTrivia)
			{
				return true;
			}
			return false;
		}

		private bool NeedsIndentAfterLineBreak(SyntaxTrivia trivia)
		{
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(trivia);
			if (syntaxKind == SyntaxKind.DocumentationCommentTrivia || syntaxKind == SyntaxKind.CommentTrivia || syntaxKind == SyntaxKind.DocumentationCommentExteriorTrivia)
			{
				return true;
			}
			return false;
		}

		private bool NeedsSeparator(SyntaxToken token, SyntaxToken nextToken)
		{
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.EndOfFileToken)
			{
				return false;
			}
			if (token.Parent == null || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.None)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.SingleLineFunctionLambdaExpression)
			{
				return true;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.EndOfFileToken)
			{
				return false;
			}
			if (token.Parent is UnaryExpressionSyntax && VisualBasicExtensions.Kind(token) != SyntaxKind.NotKeyword && VisualBasicExtensions.Kind(token) != SyntaxKind.AddressOfKeyword)
			{
				return false;
			}
			if (token.Parent is BinaryExpressionSyntax || nextToken.Parent is BinaryExpressionSyntax)
			{
				return true;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.OpenParenToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.CloseParenToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) != SyntaxKind.CommaToken && VisualBasicExtensions.Kind(nextToken) == SyntaxKind.OpenParenToken)
			{
				return false;
			}
			if ((VisualBasicExtensions.Kind(token) == SyntaxKind.CommaToken && (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.EmptyToken || VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.InterpolationAlignmentClause)) || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.CommaToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.DotToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.DotToken && VisualBasicExtensions.Kind(nextToken.Parent) != SyntaxKind.NamedFieldInitializer)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.ColonToken)
			{
				if (VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.LabelStatement)
				{
					return false;
				}
				if (VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.InterpolationFormatClause)
				{
					return false;
				}
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.OpenBraceToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.CloseBraceToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.ColonEqualsToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.ColonEqualsToken)
			{
				return false;
			}
			if (SyntaxFacts.IsRelationalCaseClause(VisualBasicExtensions.Kind(token.Parent)) || SyntaxFacts.IsRelationalCaseClause(VisualBasicExtensions.Kind(nextToken.Parent)))
			{
				return true;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.GreaterThanToken && VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.AttributeList)
			{
				return true;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.LessThanToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.GreaterThanToken || VisualBasicExtensions.Kind(token) == SyntaxKind.LessThanSlashToken || VisualBasicExtensions.Kind(token) == SyntaxKind.GreaterThanToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.LessThanSlashToken)
			{
				return false;
			}
			if ((VisualBasicExtensions.Kind(token) == SyntaxKind.ColonToken && VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlPrefix) || (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.ColonToken && VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.XmlPrefix))
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.SlashGreaterThanToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.LessThanExclamationMinusMinusToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.MinusMinusGreaterThanToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.LessThanQuestionToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.BeginCDataToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.EndCDataToken)
			{
				return false;
			}
			if ((VisualBasicExtensions.Kind(token) == SyntaxKind.ColonToken && VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.AttributeTarget) || (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.ColonToken && VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.AttributeTarget))
			{
				return false;
			}
			if ((VisualBasicExtensions.Kind(token) == SyntaxKind.EqualsToken && (VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlAttribute || VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlCrefAttribute || VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlNameAttribute || VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlDeclaration)) || (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.EqualsToken && (VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.XmlAttribute || VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.XmlCrefAttribute || VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.XmlNameAttribute || VisualBasicExtensions.Kind(nextToken.Parent) == SyntaxKind.XmlDeclaration)))
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.DoubleQuoteToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.DoubleQuoteToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.AtToken && VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.XmlAttributeAccessExpression)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.SingleQuoteToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.SingleQuoteToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(nextToken) == SyntaxKind.QuestionToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.HashToken && token.Parent is DirectiveTriviaSyntax)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token.Parent) == SyntaxKind.RegionDirectiveTrivia && VisualBasicExtensions.Kind(nextToken) == SyntaxKind.StringLiteralToken && string.IsNullOrEmpty(nextToken.ValueText))
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.XmlTextLiteralToken || VisualBasicExtensions.Kind(token) == SyntaxKind.DocumentationCommentLineBreakToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.DollarSignDoubleQuoteToken)
			{
				return false;
			}
			if (VisualBasicExtensions.Kind(token) == SyntaxKind.InterpolatedStringTextToken || VisualBasicExtensions.Kind(nextToken) == SyntaxKind.InterpolatedStringTextToken)
			{
				return false;
			}
			return true;
		}

		private bool EndsInLineBreak(SyntaxTrivia trivia)
		{
			if (VisualBasicExtensions.Kind(trivia) == SyntaxKind.EndOfLineTrivia)
			{
				return true;
			}
			if (VisualBasicExtensions.Kind(trivia) == SyntaxKind.DisabledTextTrivia)
			{
				string text = trivia.ToFullString();
				return text.Length > 0 && IsNewLineChar(text.Last());
			}
			if (trivia.HasStructure && trivia.GetStructure()!.GetLastToken().HasTrailingTrivia && VisualBasicExtensions.Kind(trivia.GetStructure()!.GetLastToken().TrailingTrivia.Last()) == SyntaxKind.EndOfLineTrivia)
			{
				return true;
			}
			return false;
		}

		private static bool IsNewLineChar(char ch)
		{
			if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\n", TextCompare: false) != 0 && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\r", TextCompare: false) != 0 && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\\u0085", TextCompare: false) != 0 && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\\u2028", TextCompare: false) != 0)
			{
				return EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\\u2029", TextCompare: false) == 0;
			}
			return true;
		}

		private SyntaxTrivia VisitStructuredTrivia(SyntaxTrivia trivia)
		{
			bool isInStructuredTrivia = _isInStructuredTrivia;
			_isInStructuredTrivia = true;
			SyntaxToken previousToken = _previousToken;
			_previousToken = default(SyntaxToken);
			SyntaxTrivia result = VisitTrivia(trivia);
			_isInStructuredTrivia = isInStructuredTrivia;
			_previousToken = previousToken;
			return result;
		}

		private SyntaxToken GetNextRelevantToken(SyntaxToken token)
		{
			SyntaxToken nextToken = token.GetNextToken((SyntaxToken t) => VisualBasicExtensions.Kind(t) != SyntaxKind.None, (SyntaxTrivia t) => false);
			TextSpan consideredSpan = _consideredSpan;
			if (consideredSpan.Contains(nextToken.FullSpan))
			{
				return nextToken;
			}
			return default(SyntaxToken);
		}

		private void AddLinebreaksAfterElementsIfNeeded<TNode>(SyntaxList<TNode> list, int linebreaksBetweenElements, int linebreaksAfterLastElement) where TNode : SyntaxNode
		{
			int num = list.Count - 1;
			int num2 = num;
			for (int i = 0; i <= num2; i++)
			{
				TNode val = list[i];
				if (VisualBasicExtensions.Kind(val) == SyntaxKind.LabelStatement)
				{
					_lineBreaksAfterToken[val.GetLastToken()] = 1;
				}
				else
				{
					AddLinebreaksAfterTokenIfNeeded(val.GetLastToken(), (i == num) ? linebreaksAfterLastElement : linebreaksBetweenElements);
				}
			}
		}

		private void AddLinebreaksAfterTokenIfNeeded(SyntaxToken node, int linebreaksAfterToken)
		{
			if (!EndsWithColonSeparator(node))
			{
				_lineBreaksAfterToken[node] = linebreaksAfterToken;
			}
		}

		private bool EndsWithColonSeparator(SyntaxToken node)
		{
			if (node.HasTrailingTrivia)
			{
				return VisualBasicExtensions.Kind(node.TrailingTrivia.Last()) == SyntaxKind.ColonTrivia;
			}
			return false;
		}

		private void MarkLastStatementIfNeeded<TNode>(SyntaxList<TNode> list) where TNode : SyntaxNode
		{
			if (list.Any())
			{
				_lastStatementsInBlocks.Add(list.Last());
			}
		}

		public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
		{
			bool num = node.Imports.Any();
			bool flag = node.Members.Any();
			bool flag2 = node.Attributes.Any();
			if (num || flag2 || flag)
			{
				AddLinebreaksAfterElementsIfNeeded(node.Options, 1, 2);
			}
			else
			{
				AddLinebreaksAfterElementsIfNeeded(node.Options, 1, 1);
			}
			if (flag2 || flag)
			{
				AddLinebreaksAfterElementsIfNeeded(node.Imports, 1, 2);
			}
			else
			{
				AddLinebreaksAfterElementsIfNeeded(node.Imports, 1, 1);
			}
			if (flag)
			{
				AddLinebreaksAfterElementsIfNeeded(node.Attributes, 1, 2);
			}
			else
			{
				AddLinebreaksAfterElementsIfNeeded(node.Attributes, 1, 1);
			}
			AddLinebreaksAfterElementsIfNeeded(node.Members, 2, 1);
			return base.VisitCompilationUnit(node);
		}

		public override SyntaxNode VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			if (node.Members.Count > 0)
			{
				if (node.Members[0].Kind() != SyntaxKind.NamespaceBlock)
				{
					AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(), 2);
				}
				else
				{
					AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(), 1);
				}
				AddLinebreaksAfterElementsIfNeeded(node.Members, 2, 1);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(), 1);
			}
			return base.VisitNamespaceBlock(node);
		}

		public override SyntaxNode VisitModuleBlock(ModuleBlockSyntax node)
		{
			VisitTypeBlockSyntax(node);
			return base.VisitModuleBlock(node);
		}

		public override SyntaxNode VisitClassBlock(ClassBlockSyntax node)
		{
			VisitTypeBlockSyntax(node);
			return base.VisitClassBlock(node);
		}

		public override SyntaxNode VisitStructureBlock(StructureBlockSyntax node)
		{
			VisitTypeBlockSyntax(node);
			return base.VisitStructureBlock(node);
		}

		public override SyntaxNode VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			VisitTypeBlockSyntax(node);
			return base.VisitInterfaceBlock(node);
		}

		private void VisitTypeBlockSyntax(TypeBlockSyntax node)
		{
			bool flag = node.Implements.Count > 0;
			if (node.Inherits.Count <= 0 && !flag && node.Members.Count > 0)
			{
				AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 2);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 1);
			}
			if (flag)
			{
				AddLinebreaksAfterElementsIfNeeded(node.Inherits, 1, 1);
			}
			else
			{
				AddLinebreaksAfterElementsIfNeeded(node.Inherits, 1, 2);
			}
			AddLinebreaksAfterElementsIfNeeded(node.Implements, 1, 2);
			if (node.Kind() == SyntaxKind.InterfaceBlock)
			{
				AddLinebreaksAfterElementsIfNeeded(node.Members, 2, 2);
			}
			else
			{
				AddLinebreaksAfterElementsIfNeeded(node.Members, 2, 1);
			}
		}

		public override SyntaxNode VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.IfStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			VisualBasicSyntaxNode visualBasicSyntaxNode = ((!node.Statements.Any()) ? node.IfStatement : node.Statements.Last());
			SyntaxList<ElseIfBlockSyntax>.Enumerator enumerator = node.ElseIfBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ElseIfBlockSyntax current = enumerator.Current;
				AddLinebreaksAfterTokenIfNeeded(visualBasicSyntaxNode.GetLastToken(), 1);
				visualBasicSyntaxNode = current;
			}
			if (node.ElseBlock != null)
			{
				AddLinebreaksAfterTokenIfNeeded(visualBasicSyntaxNode.GetLastToken(), 1);
			}
			if (!_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndIfStatement.GetLastToken(), 2);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndIfStatement.GetLastToken(), 1);
			}
			return base.VisitMultiLineIfBlock(node);
		}

		public override SyntaxNode VisitEventBlock(EventBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.EventStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Accessors, 2, 1);
			return base.VisitEventBlock(node);
		}

		public override SyntaxNode VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.DoStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			if (_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.LoopStatement.GetLastToken(), 1);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.LoopStatement.GetLastToken(), 2);
			}
			return base.VisitDoLoopBlock(node);
		}

		public override SyntaxNode VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.SyncLockStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			if (_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndSyncLockStatement.GetLastToken(), 1);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndSyncLockStatement.GetLastToken(), 2);
			}
			return base.VisitSyncLockBlock(node);
		}

		public override SyntaxNode VisitMethodBlock(MethodBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitMethodBlock(node);
		}

		public override SyntaxNode VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitConstructorBlock(node);
		}

		public override SyntaxNode VisitOperatorBlock(OperatorBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitOperatorBlock(node);
		}

		public override SyntaxNode VisitAccessorBlock(AccessorBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitAccessorBlock(node);
		}

		public override SyntaxNode VisitEnumBlock(EnumBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.EnumStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Members, 1, 1);
			return base.VisitEnumBlock(node);
		}

		public override SyntaxNode VisitWhileBlock(WhileBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.WhileStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			if (!_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndWhileStatement.GetLastToken(), 2);
			}
			return base.VisitWhileBlock(node);
		}

		public override SyntaxNode VisitForBlock(ForBlockSyntax node)
		{
			VisitForOrForEachBlock(node);
			return base.VisitForBlock(node);
		}

		public override SyntaxNode VisitForEachBlock(ForEachBlockSyntax node)
		{
			VisitForOrForEachBlock(node);
			return base.VisitForEachBlock(node);
		}

		private void VisitForOrForEachBlock(ForOrForEachBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.ForOrForEachStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			if (node.NextStatement != null)
			{
				if (!_lastStatementsInBlocks.Contains(node))
				{
					AddLinebreaksAfterTokenIfNeeded(node.NextStatement.GetLastToken(), 2);
				}
				else
				{
					AddLinebreaksAfterTokenIfNeeded(node.NextStatement.GetLastToken(), 1);
				}
			}
		}

		public override SyntaxNode VisitUsingBlock(UsingBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.UsingStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			if (!_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndUsingStatement.GetLastToken(), 2);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndUsingStatement.GetLastToken(), 1);
			}
			return base.VisitUsingBlock(node);
		}

		public override SyntaxNode VisitWithBlock(WithBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.WithStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitWithBlock(node);
		}

		public override SyntaxNode VisitSelectBlock(SelectBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.SelectStatement.GetLastToken(), 1);
			if (!_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndSelectStatement.GetLastToken(), 2);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndSelectStatement.GetLastToken(), 1);
			}
			return base.VisitSelectBlock(node);
		}

		public override SyntaxNode VisitCaseBlock(CaseBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.CaseStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			SyntaxNode result = base.VisitCaseBlock(node);
			_indentationDepth--;
			return result;
		}

		public override SyntaxNode VisitTryBlock(TryBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.TryStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			if (!_lastStatementsInBlocks.Contains(node))
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndTryStatement.GetLastToken(), 2);
			}
			else
			{
				AddLinebreaksAfterTokenIfNeeded(node.EndTryStatement.GetLastToken(), 1);
			}
			return base.VisitTryBlock(node);
		}

		public override SyntaxNode VisitCatchBlock(CatchBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.CatchStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			return base.VisitCatchBlock(node);
		}

		public override SyntaxNode VisitFinallyBlock(FinallyBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.FinallyStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitFinallyBlock(node);
		}

		public override SyntaxNode VisitPropertyBlock(PropertyBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.PropertyStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Accessors, 2, 1);
			return base.VisitPropertyBlock(node);
		}

		public override SyntaxNode VisitElseBlock(ElseBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.ElseStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitElseBlock(node);
		}

		public override SyntaxNode VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.ElseIfStatement.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			return base.VisitElseIfBlock(node);
		}

		public override SyntaxNode VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.SubOrFunctionHeader.GetLastToken(), 1);
			AddLinebreaksAfterElementsIfNeeded(node.Statements, 1, 1);
			MarkLastStatementIfNeeded(node.Statements);
			_indentationDepth++;
			return base.VisitMultiLineLambdaExpression(node);
		}

		public override SyntaxNode VisitConstDirectiveTrivia(ConstDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitConstDirectiveTrivia(node);
		}

		public override SyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitIfDirectiveTrivia(node);
		}

		public override SyntaxNode VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitElseDirectiveTrivia(node);
		}

		public override SyntaxNode VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitEndIfDirectiveTrivia(node);
		}

		public override SyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitRegionDirectiveTrivia(node);
		}

		public override SyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitEndRegionDirectiveTrivia(node);
		}

		public override SyntaxNode VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitExternalSourceDirectiveTrivia(node);
		}

		public override SyntaxNode VisitEndExternalSourceDirectiveTrivia(EndExternalSourceDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitEndExternalSourceDirectiveTrivia(node);
		}

		public override SyntaxNode VisitExternalChecksumDirectiveTrivia(ExternalChecksumDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitExternalChecksumDirectiveTrivia(node);
		}

		public override SyntaxNode VisitEnableWarningDirectiveTrivia(EnableWarningDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitEnableWarningDirectiveTrivia(node);
		}

		public override SyntaxNode VisitDisableWarningDirectiveTrivia(DisableWarningDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitDisableWarningDirectiveTrivia(node);
		}

		public override SyntaxNode VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitReferenceDirectiveTrivia(node);
		}

		public override SyntaxNode VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
		{
			AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			return base.VisitBadDirectiveTrivia(node);
		}

		public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
		{
			if (node.Parent == null || (node.Parent.Kind() != SyntaxKind.Parameter && node.Parent.Kind() != SyntaxKind.SimpleAsClause))
			{
				AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(), 1);
			}
			return base.VisitAttributeList(node);
		}

		public override SyntaxNode VisitEndBlockStatement(EndBlockStatementSyntax node)
		{
			_indentationDepth--;
			return base.VisitEndBlockStatement(node);
		}

		public override SyntaxNode VisitElseStatement(ElseStatementSyntax node)
		{
			_indentationDepth--;
			SyntaxNode result = base.VisitElseStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitElseIfStatement(ElseIfStatementSyntax node)
		{
			_indentationDepth--;
			SyntaxNode result = base.VisitElseIfStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
		{
			SyntaxNode result = base.VisitIfStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitWithStatement(WithStatementSyntax node)
		{
			SyntaxNode result = base.VisitWithStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitSyncLockStatement(SyncLockStatementSyntax node)
		{
			SyntaxNode result = base.VisitSyncLockStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitModuleStatement(ModuleStatementSyntax node)
		{
			SyntaxNode result = base.VisitModuleStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitNamespaceStatement(NamespaceStatementSyntax node)
		{
			SyntaxNode result = base.VisitNamespaceStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitInterfaceStatement(InterfaceStatementSyntax node)
		{
			SyntaxNode result = base.VisitInterfaceStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitStructureStatement(StructureStatementSyntax node)
		{
			SyntaxNode result = base.VisitStructureStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitEnumStatement(EnumStatementSyntax node)
		{
			SyntaxNode result = base.VisitEnumStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitClassStatement(ClassStatementSyntax node)
		{
			SyntaxNode result = base.VisitClassStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
		{
			SyntaxNode result = base.VisitWhileStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
		{
			SyntaxNode result = base.VisitDoStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitSelectStatement(SelectStatementSyntax node)
		{
			SyntaxNode result = base.VisitSelectStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitCaseStatement(CaseStatementSyntax node)
		{
			SyntaxNode result = base.VisitCaseStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitLoopStatement(LoopStatementSyntax node)
		{
			_indentationDepth--;
			return base.VisitLoopStatement(node);
		}

		public override SyntaxNode VisitForStatement(ForStatementSyntax node)
		{
			SyntaxNode result = base.VisitForStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
		{
			SyntaxNode result = base.VisitForEachStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitTryStatement(TryStatementSyntax node)
		{
			SyntaxNode result = base.VisitTryStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitCatchStatement(CatchStatementSyntax node)
		{
			_indentationDepth--;
			SyntaxNode result = base.VisitCatchStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitFinallyStatement(FinallyStatementSyntax node)
		{
			_indentationDepth--;
			SyntaxNode result = base.VisitFinallyStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
		{
			SyntaxNode result = base.VisitUsingStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitMethodStatement(MethodStatementSyntax node)
		{
			SyntaxNode result = base.VisitMethodStatement(node);
			if (node.Parent != null && (node.Parent.Kind() == SyntaxKind.SubBlock || node.Parent.Kind() == SyntaxKind.FunctionBlock))
			{
				_indentationDepth++;
			}
			return result;
		}

		public override SyntaxNode VisitSubNewStatement(SubNewStatementSyntax node)
		{
			SyntaxNode result = base.VisitSubNewStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitAccessorStatement(AccessorStatementSyntax node)
		{
			SyntaxNode result = base.VisitAccessorStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitOperatorStatement(OperatorStatementSyntax node)
		{
			SyntaxNode result = base.VisitOperatorStatement(node);
			_indentationDepth++;
			return result;
		}

		public override SyntaxNode VisitEventStatement(EventStatementSyntax node)
		{
			SyntaxNode result = base.VisitEventStatement(node);
			if (node.Parent != null && node.Parent.Kind() == SyntaxKind.EventBlock)
			{
				_indentationDepth++;
			}
			return result;
		}

		public override SyntaxNode VisitPropertyStatement(PropertyStatementSyntax node)
		{
			SyntaxNode result = base.VisitPropertyStatement(node);
			if (node.Parent != null && node.Parent.Kind() == SyntaxKind.PropertyBlock)
			{
				_indentationDepth++;
			}
			return result;
		}

		public override SyntaxNode VisitLabelStatement(LabelStatementSyntax node)
		{
			int indentationDepth = _indentationDepth;
			_indentationDepth = 0;
			SyntaxNode result = base.VisitLabelStatement(node);
			_indentationDepth = indentationDepth;
			return result;
		}

		public override SyntaxNode VisitNextStatement(NextStatementSyntax node)
		{
			int num = node.ControlVariables.Count;
			if (num == 0)
			{
				num = 1;
			}
			_indentationDepth -= num;
			return base.VisitNextStatement(node);
		}
	}
}
