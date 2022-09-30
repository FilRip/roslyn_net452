using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class Parser : ISyntaxFactoryContext, IDisposable
	{
		private enum PossibleFirstStatementKind
		{
			No,
			Yes,
			IfPrecededByLineBreak
		}

		private class TriviaChecker
		{
			private TriviaChecker()
			{
			}

			public static bool HasInvalidTrivia(GreenNode node)
			{
				return SyntaxNodeOrTokenHasInvalidTrivia(node);
			}

			private static bool SyntaxNodeOrTokenHasInvalidTrivia(GreenNode node)
			{
				if (node != null)
				{
					if (node is SyntaxToken syntaxToken)
					{
						if (IsInvalidTrivia(syntaxToken.GetLeadingTrivia()) || IsInvalidTrivia(syntaxToken.GetTrailingTrivia()))
						{
							return true;
						}
					}
					else if (SyntaxNodeHasInvalidTrivia(node))
					{
						return true;
					}
				}
				return false;
			}

			private static bool SyntaxNodeHasInvalidTrivia(GreenNode node)
			{
				int num = node.SlotCount - 1;
				for (int i = 0; i <= num; i++)
				{
					if (SyntaxNodeOrTokenHasInvalidTrivia(node.GetSlot(i)))
					{
						return true;
					}
				}
				return false;
			}

			private static bool IsInvalidTrivia(GreenNode node)
			{
				if (node != null)
				{
					switch (node.RawKind)
					{
					case 1:
					{
						int num = node.SlotCount - 1;
						for (int i = 0; i <= num; i++)
						{
							if (IsInvalidTrivia(node.GetSlot(i)))
							{
								return true;
							}
						}
						break;
					}
					case 729:
					{
						string text = ((SyntaxTrivia)node).Text;
						foreach (char c in text)
						{
							if (c != ' ' && c != '\t')
							{
								return true;
							}
						}
						break;
					}
					case 709:
						if (SyntaxNodeOrTokenHasInvalidTrivia(((SkippedTokensTriviaSyntax)node).Tokens.Node))
						{
							return true;
						}
						break;
					default:
						return true;
					}
				}
				return false;
			}
		}

		private bool _allowLeadingMultilineTrivia;

		private bool _hadImplicitLineContinuation;

		private bool _hadLineContinuationComment;

		private PossibleFirstStatementKind _possibleFirstStatementOnLine;

		private int _recursionDepth;

		private bool _evaluatingConditionCompilationExpression;

		private readonly Scanner _scanner;

		private readonly CancellationToken _cancellationToken;

		internal readonly SyntaxListPool _pool;

		private readonly ContextAwareSyntaxFactory _syntaxFactory;

		private readonly bool _disposeScanner;

		private BlockContext _context;

		private bool _isInMethodDeclarationHeader;

		private bool _isInAsyncMethodDeclarationHeader;

		private bool _isInIteratorMethodDeclarationHeader;

		private SyntaxToken _currentToken;

		private static readonly Func<SyntaxToken, SyntaxKind[], bool> s_isTokenOrKeywordFunc = IsTokenOrKeyword;

		internal bool IsScript => _scanner.Options.Kind == SourceCodeKind.Script;

		public bool IsWithinAsyncMethodOrLambda
		{
			get
			{
				if (_isInMethodDeclarationHeader)
				{
					return _isInAsyncMethodDeclarationHeader;
				}
				return Context.IsWithinAsyncMethodOrLambda;
			}
		}

		public bool IsWithinIteratorContext
		{
			get
			{
				if (_isInMethodDeclarationHeader)
				{
					return _isInIteratorMethodDeclarationHeader;
				}
				return Context.IsWithinIteratorMethodOrLambdaOrProperty;
			}
		}

		internal BlockContext Context => _context;

		internal ContextAwareSyntaxFactory SyntaxFactory => _syntaxFactory;

		private SyntaxToken PrevToken => _scanner.PrevToken;

		internal SyntaxToken CurrentToken
		{
			get
			{
				SyntaxToken currentToken = _currentToken;
				if (currentToken == null)
				{
					currentToken = _scanner.GetCurrentToken();
					_allowLeadingMultilineTrivia = false;
					_currentToken = currentToken;
				}
				return currentToken;
			}
		}

		internal DirectiveTriviaSyntax ParseConditionalCompilationStatement()
		{
			if (CurrentToken.Kind == SyntaxKind.DateLiteralToken || CurrentToken.Kind == SyntaxKind.BadToken)
			{
				PunctuationSyntax hashToken = SyntaxNodeExtensions.AddLeadingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.HashToken), new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(CurrentToken));
				GetNextToken();
				return ParseBadDirective(hashToken);
			}
			DirectiveTriviaSyntax directiveTriviaSyntax = null;
			PunctuationSyntax hashToken2 = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.ElseKeyword:
				directiveTriviaSyntax = ParseElseDirective(hashToken2);
				break;
			case SyntaxKind.IfKeyword:
				directiveTriviaSyntax = ParseIfDirective(hashToken2, null);
				break;
			case SyntaxKind.ElseIfKeyword:
				directiveTriviaSyntax = ParseElseIfDirective(hashToken2);
				break;
			case SyntaxKind.EndKeyword:
				directiveTriviaSyntax = ParseEndDirective(hashToken2);
				break;
			case SyntaxKind.EndIfKeyword:
				directiveTriviaSyntax = ParseAnachronisticEndIfDirective(hashToken2);
				break;
			case SyntaxKind.ConstKeyword:
				directiveTriviaSyntax = ParseConstDirective(hashToken2);
				break;
			case SyntaxKind.IdentifierToken:
				switch (((IdentifierTokenSyntax)CurrentToken).PossibleKeywordKind)
				{
				case SyntaxKind.ExternalSourceKeyword:
					directiveTriviaSyntax = ParseExternalSourceDirective(hashToken2);
					break;
				case SyntaxKind.ExternalChecksumKeyword:
					directiveTriviaSyntax = ParseExternalChecksumDirective(hashToken2);
					break;
				case SyntaxKind.RegionKeyword:
					directiveTriviaSyntax = ParseRegionDirective(hashToken2);
					break;
				case SyntaxKind.DisableKeyword:
				case SyntaxKind.EnableKeyword:
					directiveTriviaSyntax = ParseWarningDirective(hashToken2);
					break;
				case SyntaxKind.ReferenceKeyword:
					directiveTriviaSyntax = ParseReferenceDirective(hashToken2);
					break;
				default:
					directiveTriviaSyntax = ParseBadDirective(hashToken2);
					break;
				}
				break;
			default:
				directiveTriviaSyntax = ParseBadDirective(hashToken2);
				break;
			}
			return directiveTriviaSyntax;
		}

		internal ExpressionSyntax ParseConditionalCompilationExpression()
		{
			bool evaluatingConditionCompilationExpression = _evaluatingConditionCompilationExpression;
			_evaluatingConditionCompilationExpression = true;
			ExpressionSyntax result = ParseExpressionCore();
			_evaluatingConditionCompilationExpression = evaluatingConditionCompilationExpression;
			return result;
		}

		private DirectiveTriviaSyntax ParseElseDirective(PunctuationSyntax hashToken)
		{
			KeywordSyntax elseKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			if (CurrentToken.Kind != SyntaxKind.IfKeyword)
			{
				return SyntaxFactory.ElseDirectiveTrivia(hashToken, elseKeyword);
			}
			return ParseIfDirective(hashToken, elseKeyword);
		}

		private DirectiveTriviaSyntax ParseElseIfDirective(PunctuationSyntax hashToken)
		{
			return ParseIfDirective(hashToken, null);
		}

		private IfDirectiveTriviaSyntax ParseIfDirective(PunctuationSyntax hashToken, KeywordSyntax elseKeyword)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseConditionalCompilationExpression();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			KeywordSyntax thenKeyword = null;
			if (CurrentToken.Kind == SyntaxKind.ThenKeyword)
			{
				thenKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
			}
			if (keywordSyntax.Kind == SyntaxKind.IfKeyword && elseKeyword == null)
			{
				return SyntaxFactory.IfDirectiveTrivia(hashToken, elseKeyword, keywordSyntax, expressionSyntax, thenKeyword);
			}
			return SyntaxFactory.ElseIfDirectiveTrivia(hashToken, elseKeyword, keywordSyntax, expressionSyntax, thenKeyword);
		}

		private DirectiveTriviaSyntax ParseEndDirective(PunctuationSyntax hashToken)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			DirectiveTriviaSyntax directiveTriviaSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.IfKeyword)
			{
				KeywordSyntax ifKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				directiveTriviaSyntax = SyntaxFactory.EndIfDirectiveTrivia(hashToken, keywordSyntax, ifKeyword);
			}
			else if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
			{
				IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)CurrentToken;
				if (identifierTokenSyntax.PossibleKeywordKind == SyntaxKind.RegionKeyword)
				{
					KeywordSyntax regionKeyword = _scanner.MakeKeyword((IdentifierTokenSyntax)CurrentToken);
					GetNextToken();
					directiveTriviaSyntax = SyntaxFactory.EndRegionDirectiveTrivia(hashToken, keywordSyntax, regionKeyword);
				}
				else if (identifierTokenSyntax.PossibleKeywordKind == SyntaxKind.ExternalSourceKeyword)
				{
					KeywordSyntax externalSourceKeyword = _scanner.MakeKeyword((IdentifierTokenSyntax)CurrentToken);
					GetNextToken();
					directiveTriviaSyntax = SyntaxFactory.EndExternalSourceDirectiveTrivia(hashToken, keywordSyntax, externalSourceKeyword);
				}
			}
			if (directiveTriviaSyntax == null)
			{
				hashToken = SyntaxNodeExtensions.AddTrailingSyntax(hashToken, keywordSyntax, ERRID.ERR_Syntax);
				directiveTriviaSyntax = SyntaxFactory.BadDirectiveTrivia(hashToken);
			}
			return directiveTriviaSyntax;
		}

		private DirectiveTriviaSyntax ParseAnachronisticEndIfDirective(PunctuationSyntax hashToken)
		{
			KeywordSyntax unexpected = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword);
			node = SyntaxNodeExtensions.AddLeadingSyntax(node, unexpected, ERRID.ERR_ObsoleteEndIf);
			return SyntaxFactory.EndIfDirectiveTrivia(hashToken, node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword));
		}

		private ConstDirectiveTriviaSyntax ParseConstDirective(PunctuationSyntax hashToken)
		{
			KeywordSyntax constKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			if (identifierTokenSyntax.ContainsDiagnostics)
			{
				syntaxList = ResyncAt(new SyntaxKind[1] { SyntaxKind.EqualsToken });
			}
			PunctuationSyntax token = null;
			VerifyExpectedToken(SyntaxKind.EqualsToken, ref token);
			if (syntaxList.Node != null)
			{
				token = SyntaxNodeExtensions.AddLeadingSyntax(token, syntaxList, ERRID.ERR_Syntax);
			}
			ExpressionSyntax expressionSyntax = ParseConditionalCompilationExpression();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.ConstDirectiveTrivia(hashToken, constKeyword, identifierTokenSyntax, token, expressionSyntax);
		}

		private RegionDirectiveTriviaSyntax ParseRegionDirective(PunctuationSyntax hashToken)
		{
			IdentifierTokenSyntax identifier = (IdentifierTokenSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax regionKeyword = _scanner.MakeKeyword(identifier);
			StringLiteralTokenSyntax token = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token);
			return SyntaxFactory.RegionDirectiveTrivia(hashToken, regionKeyword, token);
		}

		private ExternalSourceDirectiveTriviaSyntax ParseExternalSourceDirective(PunctuationSyntax hashToken)
		{
			IdentifierTokenSyntax identifier = (IdentifierTokenSyntax)CurrentToken;
			KeywordSyntax externalSourceKeyword = _scanner.MakeKeyword(identifier);
			GetNextToken();
			PunctuationSyntax token = null;
			PunctuationSyntax token2 = null;
			VerifyExpectedToken(SyntaxKind.OpenParenToken, ref token);
			StringLiteralTokenSyntax token3 = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token3);
			PunctuationSyntax token4 = null;
			VerifyExpectedToken(SyntaxKind.CommaToken, ref token4);
			IntegerLiteralTokenSyntax token5 = null;
			VerifyExpectedToken(SyntaxKind.IntegerLiteralToken, ref token5);
			VerifyExpectedToken(SyntaxKind.CloseParenToken, ref token2);
			return SyntaxFactory.ExternalSourceDirectiveTrivia(hashToken, externalSourceKeyword, token, token3, token4, token5, token2);
		}

		private ExternalChecksumDirectiveTriviaSyntax ParseExternalChecksumDirective(PunctuationSyntax hashToken)
		{
			IdentifierTokenSyntax identifier = (IdentifierTokenSyntax)CurrentToken;
			KeywordSyntax externalChecksumKeyword = _scanner.MakeKeyword(identifier);
			GetNextToken();
			PunctuationSyntax token = null;
			PunctuationSyntax token2 = null;
			VerifyExpectedToken(SyntaxKind.OpenParenToken, ref token);
			StringLiteralTokenSyntax token3 = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token3);
			PunctuationSyntax token4 = null;
			VerifyExpectedToken(SyntaxKind.CommaToken, ref token4);
			StringLiteralTokenSyntax token5 = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token5);
			if (!token5.IsMissing && !Guid.TryParse(token5.ValueText, out var _))
			{
				token5 = SyntaxExtensions.WithDiagnostics(token5, ErrorFactory.ErrorInfo(ERRID.WRN_BadGUIDFormatExtChecksum));
			}
			PunctuationSyntax token6 = null;
			VerifyExpectedToken(SyntaxKind.CommaToken, ref token6);
			StringLiteralTokenSyntax token7 = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token7);
			if (!token7.IsMissing)
			{
				string valueText = token7.ValueText;
				if (valueText.Length % 2 != 0)
				{
					token7 = SyntaxExtensions.WithDiagnostics(token7, ErrorFactory.ErrorInfo(ERRID.WRN_BadChecksumValExtChecksum));
				}
				else
				{
					string text = valueText;
					for (int i = 0; i < text.Length; i = checked(i + 1))
					{
						if (!SyntaxFacts.IsHexDigit(text[i]))
						{
							token7 = SyntaxExtensions.WithDiagnostics(token7, ErrorFactory.ErrorInfo(ERRID.WRN_BadChecksumValExtChecksum));
							break;
						}
					}
				}
			}
			VerifyExpectedToken(SyntaxKind.CloseParenToken, ref token2);
			return SyntaxFactory.ExternalChecksumDirectiveTrivia(hashToken, externalChecksumKeyword, token, token3, token4, token5, token6, token7, token2);
		}

		private DirectiveTriviaSyntax ParseWarningDirective(PunctuationSyntax hashToken)
		{
			IdentifierTokenSyntax identifier = (IdentifierTokenSyntax)CurrentToken;
			KeywordSyntax keywordSyntax = _scanner.MakeKeyword(identifier);
			GetNextToken();
			KeywordSyntax keyword = null;
			TryGetContextualKeyword(SyntaxKind.WarningKeyword, ref keyword, createIfMissing: true);
			if (keyword.ContainsDiagnostics)
			{
				keyword = ResyncAt(keyword);
			}
			SeparatedSyntaxListBuilder<IdentifierNameSyntax> item = _pool.AllocateSeparated<IdentifierNameSyntax>();
			if (!SyntaxFacts.IsTerminator(CurrentToken.Kind))
			{
				while (true)
				{
					IdentifierNameSyntax identifierNameSyntax = SyntaxFactory.IdentifierName(ParseIdentifier());
					if (identifierNameSyntax.ContainsDiagnostics)
					{
						identifierNameSyntax = ResyncAt(identifierNameSyntax, SyntaxKind.CommaToken);
					}
					else if (identifierNameSyntax.Identifier.TypeCharacter != 0)
					{
						identifierNameSyntax = ReportSyntaxError(identifierNameSyntax, ERRID.ERR_TypecharNotallowed);
					}
					item.Add(identifierNameSyntax);
					PunctuationSyntax token = null;
					if (!TryGetToken(SyntaxKind.CommaToken, ref token))
					{
						if (SyntaxFacts.IsTerminator(CurrentToken.Kind))
						{
							break;
						}
						token = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CommaToken);
						token = ReportSyntaxError(token, ERRID.ERR_ExpectedComma);
					}
					if (token.ContainsDiagnostics)
					{
						token = ResyncAt(token);
					}
					item.AddSeparator(token);
				}
			}
			DirectiveTriviaSyntax directiveTriviaSyntax = null;
			if (keywordSyntax.Kind == SyntaxKind.EnableKeyword)
			{
				directiveTriviaSyntax = SyntaxFactory.EnableWarningDirectiveTrivia(hashToken, keywordSyntax, keyword, item.ToList());
			}
			else if (keywordSyntax.Kind == SyntaxKind.DisableKeyword)
			{
				directiveTriviaSyntax = SyntaxFactory.DisableWarningDirectiveTrivia(hashToken, keywordSyntax, keyword, item.ToList());
			}
			if (directiveTriviaSyntax != null)
			{
				directiveTriviaSyntax = CheckFeatureAvailability(Feature.WarningDirectives, directiveTriviaSyntax);
			}
			_pool.Free(in item);
			return directiveTriviaSyntax;
		}

		private DirectiveTriviaSyntax ParseReferenceDirective(PunctuationSyntax hashToken)
		{
			IdentifierTokenSyntax identifier = (IdentifierTokenSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax keywordSyntax = _scanner.MakeKeyword(identifier);
			if (!IsScript)
			{
				keywordSyntax = SyntaxNodeExtensions.AddError(keywordSyntax, ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts);
			}
			StringLiteralTokenSyntax token = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token);
			return SyntaxFactory.ReferenceDirectiveTrivia(hashToken, keywordSyntax, token);
		}

		private static BadDirectiveTriviaSyntax ParseBadDirective(PunctuationSyntax hashToken)
		{
			BadDirectiveTriviaSyntax badDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.BadDirectiveTrivia(hashToken);
			if (!badDirectiveTriviaSyntax.ContainsDiagnostics)
			{
				badDirectiveTriviaSyntax = ReportSyntaxError(badDirectiveTriviaSyntax, ERRID.ERR_ExpectedConditionalDirective);
			}
			return badDirectiveTriviaSyntax;
		}

		internal ExpressionSyntax ParseExpression(OperatorPrecedence pendingPrecedence = OperatorPrecedence.PrecedenceNone, bool bailIfFirstTokenRejected = false)
		{
			return ParseWithStackGuard(() => ParseExpressionCore(pendingPrecedence, bailIfFirstTokenRejected), () => Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression());
		}

		private ExpressionSyntax ParseExpressionCore(OperatorPrecedence pendingPrecedence = OperatorPrecedence.PrecedenceNone, bool bailIfFirstTokenRejected = false)
		{
			try
			{
				_recursionDepth++;
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				ExpressionSyntax expressionSyntax = null;
				SyntaxToken currentToken = CurrentToken;
				if (_evaluatingConditionCompilationExpression && !StartsValidConditionalCompilationExpr(currentToken))
				{
					if (bailIfFirstTokenRejected)
					{
						return null;
					}
					expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), currentToken, ERRID.ERR_BadCCExpression);
					GetNextToken();
					return expressionSyntax;
				}
				switch (currentToken.Kind)
				{
				case SyntaxKind.MinusToken:
				{
					GetNextToken();
					ExpressionSyntax operand = ParseExpressionCore(OperatorPrecedence.PrecedenceNegate);
					expressionSyntax = SyntaxFactory.UnaryMinusExpression(currentToken, operand);
					break;
				}
				case SyntaxKind.NotKeyword:
				{
					GetNextToken();
					ExpressionSyntax operand4 = ParseExpressionCore(OperatorPrecedence.PrecedenceNot);
					expressionSyntax = SyntaxFactory.NotExpression(currentToken, operand4);
					break;
				}
				case SyntaxKind.PlusToken:
				{
					GetNextToken();
					ExpressionSyntax operand3 = ParseExpressionCore(OperatorPrecedence.PrecedenceNegate);
					expressionSyntax = SyntaxFactory.UnaryPlusExpression(currentToken, operand3);
					break;
				}
				case SyntaxKind.AddressOfKeyword:
				{
					GetNextToken();
					ExpressionSyntax operand2 = ParseExpressionCore(OperatorPrecedence.PrecedenceNegate);
					expressionSyntax = SyntaxFactory.AddressOfExpression(currentToken, operand2);
					break;
				}
				default:
					expressionSyntax = ParseTerm(bailIfFirstTokenRejected);
					if (expressionSyntax == null)
					{
						return null;
					}
					break;
				}
				if (SyntaxKind.CollectionInitializer != expressionSyntax.Kind)
				{
					while (CurrentToken.IsBinaryOperator())
					{
						if (_evaluatingConditionCompilationExpression && !IsValidOperatorForConditionalCompilationExpr(CurrentToken))
						{
							expressionSyntax = ReportSyntaxError(expressionSyntax, ERRID.ERR_BadCCExpression);
							break;
						}
						OperatorPrecedence operatorPrecedence = KeywordTable.TokenOpPrec(CurrentToken.Kind);
						if (operatorPrecedence <= pendingPrecedence)
						{
							break;
						}
						SyntaxToken syntaxToken = ParseBinaryOperator();
						ExpressionSyntax right = ParseExpressionCore(operatorPrecedence);
						expressionSyntax = SyntaxFactory.BinaryExpression(GetBinaryOperatorHelper(syntaxToken), expressionSyntax, syntaxToken, right);
					}
				}
				return expressionSyntax;
			}
			finally
			{
				_recursionDepth--;
			}
		}

		private ExpressionSyntax ParseTerm(bool BailIfFirstTokenRejected = false, bool RedimOrNewParent = false)
		{
			ExpressionSyntax expression = null;
			SyntaxToken currentToken = CurrentToken;
			switch (currentToken.Kind)
			{
			case SyntaxKind.IdentifierToken:
			{
				KeywordSyntax k = null;
				if (TryIdentifierAsContextualKeyword(currentToken, ref k))
				{
					if (k.Kind == SyntaxKind.FromKeyword || k.Kind == SyntaxKind.AggregateKeyword)
					{
						expression = ParsePotentialQuery(k);
						if (expression != null)
						{
							break;
						}
					}
					else if (k.Kind == SyntaxKind.AsyncKeyword || k.Kind == SyntaxKind.IteratorKeyword)
					{
						SyntaxToken syntaxToken = PeekToken(1);
						if (syntaxToken.Kind == SyntaxKind.IdentifierToken)
						{
							KeywordSyntax k2 = null;
							if (TryTokenAsContextualKeyword(syntaxToken, ref k2) && k2.Kind != k.Kind && (k2.Kind == SyntaxKind.AsyncKeyword || k2.Kind == SyntaxKind.IteratorKeyword))
							{
								syntaxToken = PeekToken(2);
							}
						}
						if (syntaxToken.Kind == SyntaxKind.SubKeyword || syntaxToken.Kind == SyntaxKind.FunctionKeyword)
						{
							expression = ParseLambda(parseModifiers: true);
							break;
						}
					}
					else if (Context.IsWithinAsyncMethodOrLambda && k.Kind == SyntaxKind.AwaitKeyword)
					{
						expression = ParseAwaitExpression(k);
						break;
					}
				}
				expression = ParseSimpleNameExpressionAllowingKeywordAndTypeArguments();
				break;
			}
			case SyntaxKind.ExclamationToken:
				expression = ParseQualifiedExpr(null);
				break;
			case SyntaxKind.DotToken:
				expression = ParseQualifiedExpr(null);
				break;
			case SyntaxKind.GlobalKeyword:
				expression = SyntaxFactory.GlobalName((KeywordSyntax)currentToken);
				GetNextToken();
				if (CurrentToken.Kind != SyntaxKind.DotToken)
				{
					expression = ReportSyntaxError(expression, ERRID.ERR_ExpectedDotAfterGlobalNameSpace);
				}
				break;
			case SyntaxKind.MyBaseKeyword:
				expression = SyntaxFactory.MyBaseExpression((KeywordSyntax)currentToken);
				GetNextToken();
				if (CurrentToken.Kind != SyntaxKind.DotToken)
				{
					expression = ReportSyntaxError(expression, ERRID.ERR_ExpectedDotAfterMyBase);
				}
				break;
			case SyntaxKind.MyClassKeyword:
				expression = SyntaxFactory.MyClassExpression((KeywordSyntax)currentToken);
				GetNextToken();
				if (CurrentToken.Kind != SyntaxKind.DotToken)
				{
					expression = ReportSyntaxError(expression, ERRID.ERR_ExpectedDotAfterMyClass);
				}
				break;
			case SyntaxKind.MeKeyword:
				expression = SyntaxFactory.MeExpression((KeywordSyntax)currentToken);
				GetNextToken();
				break;
			case SyntaxKind.OpenParenToken:
				expression = ParseParenthesizedExpressionOrTupleLiteral();
				break;
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.LessThanSlashToken:
			case SyntaxKind.LessThanExclamationMinusMinusToken:
			case SyntaxKind.LessThanQuestionToken:
			case SyntaxKind.BeginCDataToken:
				if (TokenContainsFullWidthChars(currentToken))
				{
					if (BailIfFirstTokenRejected)
					{
						return null;
					}
					expression = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression();
					expression = SyntaxNodeExtensions.AddTrailingSyntax(expression, CurrentToken, ERRID.ERR_FullWidthAsXmlDelimiter);
					GetNextToken();
					return expression;
				}
				expression = ParseXmlExpression();
				break;
			case SyntaxKind.IntegerLiteralToken:
				expression = ParseIntLiteral();
				break;
			case SyntaxKind.CharacterLiteralToken:
				expression = ParseCharLiteral();
				break;
			case SyntaxKind.DecimalLiteralToken:
				expression = ParseDecLiteral();
				break;
			case SyntaxKind.FloatingLiteralToken:
				expression = ParseFltLiteral();
				break;
			case SyntaxKind.DateLiteralToken:
				expression = ParseDateLiteral();
				break;
			case SyntaxKind.StringLiteralToken:
				expression = ParseStringLiteral();
				break;
			case SyntaxKind.TrueKeyword:
				expression = SyntaxFactory.TrueLiteralExpression(CurrentToken);
				GetNextToken();
				break;
			case SyntaxKind.FalseKeyword:
				expression = SyntaxFactory.FalseLiteralExpression(CurrentToken);
				GetNextToken();
				break;
			case SyntaxKind.NothingKeyword:
				expression = SyntaxFactory.NothingLiteralExpression(CurrentToken);
				GetNextToken();
				break;
			case SyntaxKind.TypeOfKeyword:
				expression = ParseTypeOf();
				break;
			case SyntaxKind.GetTypeKeyword:
				expression = ParseGetType();
				break;
			case SyntaxKind.NameOfKeyword:
				expression = ParseNameOf();
				break;
			case SyntaxKind.GetXmlNamespaceKeyword:
				expression = ParseGetXmlNamespace();
				break;
			case SyntaxKind.NewKeyword:
				expression = ParseNewExpression();
				break;
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
				expression = ParseCastExpression();
				break;
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.TryCastKeyword:
				expression = ParseCast();
				break;
			case SyntaxKind.IfKeyword:
				expression = ParseIfExpression();
				break;
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
			case SyntaxKind.VariantKeyword:
			{
				bool allowedEmptyGenericArguments = false;
				expression = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				break;
			}
			case SyntaxKind.OpenBraceToken:
				expression = ParseCollectionInitializer();
				break;
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.SubKeyword:
				expression = ParseLambda(parseModifiers: false);
				break;
			case SyntaxKind.DollarSignDoubleQuoteToken:
				expression = ParseInterpolatedStringExpression();
				break;
			default:
				if (currentToken.Kind == SyntaxKind.QuestionToken && CanStartConsequenceExpression(PeekToken(1).Kind, qualified: false))
				{
					PunctuationSyntax node = (PunctuationSyntax)currentToken;
					node = CheckFeatureAvailability(Feature.NullPropagatingOperator, node);
					GetNextToken();
					expression = SyntaxFactory.ConditionalAccessExpression(expression, node, ParsePostFixExpression(RedimOrNewParent, null));
					break;
				}
				if (BailIfFirstTokenRejected)
				{
					return null;
				}
				expression = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression();
				expression = ReportSyntaxError(expression, ERRID.ERR_ExpectedExpression);
				break;
			}
			if (!_evaluatingConditionCompilationExpression)
			{
				expression = ParsePostFixExpression(RedimOrNewParent, expression);
			}
			if (CurrentToken != null && CurrentToken.Kind == SyntaxKind.QuestionToken)
			{
				expression = SyntaxNodeExtensions.AddTrailingSyntax(expression, CurrentToken, ERRID.ERR_NullableCharNotSupported);
				GetNextToken();
			}
			return expression;
		}

		private ExpressionSyntax ParsePostFixExpression(bool RedimOrNewParent, ExpressionSyntax term)
		{
			while (true)
			{
				SyntaxToken currentToken = CurrentToken;
				bool flag = term != null && term.Kind == SyntaxKind.SingleLineSubLambdaExpression;
				if (currentToken.Kind == SyntaxKind.DotToken)
				{
					if (flag)
					{
						term = ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesDot);
					}
					term = ParseQualifiedExpr(term);
					continue;
				}
				if (currentToken.Kind == SyntaxKind.ExclamationToken)
				{
					if (flag)
					{
						term = ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesBang);
					}
					term = ParseQualifiedExpr(term);
					continue;
				}
				if (currentToken.Kind == SyntaxKind.OpenParenToken)
				{
					if (flag)
					{
						term = ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesLParen);
					}
					term = ParseParenthesizedQualifier(term, RedimOrNewParent);
					continue;
				}
				if (currentToken.Kind != SyntaxKind.QuestionToken || !CanStartConsequenceExpression(PeekToken(1).Kind, qualified: true))
				{
					break;
				}
				PunctuationSyntax node = (PunctuationSyntax)currentToken;
				node = CheckFeatureAvailability(Feature.NullPropagatingOperator, node);
				GetNextToken();
				if (flag)
				{
					term = CurrentToken.Kind switch
					{
						SyntaxKind.DotToken => ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesDot), 
						SyntaxKind.ExclamationToken => ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesBang), 
						SyntaxKind.OpenParenToken => ReportSyntaxError(term, ERRID.ERR_SubRequiresParenthesesLParen), 
						_ => throw ExceptionUtilities.Unreachable, 
					};
				}
				term = SyntaxFactory.ConditionalAccessExpression(term, node, ParsePostFixExpression(RedimOrNewParent, null));
			}
			return term;
		}

		private bool CanStartConsequenceExpression(SyntaxKind kind, bool qualified)
		{
			if (kind != SyntaxKind.DotToken && kind != SyntaxKind.ExclamationToken)
			{
				if (qualified)
				{
					return kind == SyntaxKind.OpenParenToken;
				}
				return false;
			}
			return true;
		}

		private static bool TokenContainsFullWidthChars(SyntaxToken tk)
		{
			string text = tk.Text;
			for (int i = 0; i < text.Length; i = checked(i + 1))
			{
				if (SyntaxFacts.IsFullWidth(text[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static ExpressionSyntax GetArgumentAsExpression(ArgumentSyntax arg)
		{
			SyntaxKind kind = arg.Kind;
			if (kind == SyntaxKind.SimpleArgument)
			{
				SimpleArgumentSyntax simpleArgumentSyntax = (SimpleArgumentSyntax)arg;
				ExpressionSyntax expressionSyntax = simpleArgumentSyntax.Expression;
				if (simpleArgumentSyntax.NameColonEquals != null)
				{
					expressionSyntax = SyntaxNodeExtensions.AddLeadingSyntax(expressionSyntax, SyntaxList.List(simpleArgumentSyntax.NameColonEquals.Name, simpleArgumentSyntax.NameColonEquals.ColonEqualsToken), ERRID.ERR_IllegalOperandInIIFName);
				}
				return expressionSyntax;
			}
			return SyntaxNodeExtensions.AddLeadingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), arg, ERRID.ERR_ExpectedExpression);
		}

		private ExpressionSyntax ParseIfExpression()
		{
			KeywordSyntax ifKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				ArgumentListSyntax argumentListSyntax = ParseParenthesizedArguments();
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> arguments = argumentListSyntax.Arguments;
				switch (arguments.Count)
				{
				case 0:
					return SyntaxFactory.BinaryConditionalExpression(ifKeyword, argumentListSyntax.OpenParenToken, SyntaxExtensions.WithDiagnostics(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ErrorFactory.ErrorInfo(ERRID.ERR_IllegalOperandInIIFCount)), (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), argumentListSyntax.CloseParenToken);
				case 1:
					return SyntaxFactory.BinaryConditionalExpression(ifKeyword, argumentListSyntax.OpenParenToken, GetArgumentAsExpression(arguments[0]), (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), SyntaxExtensions.WithDiagnostics(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ErrorFactory.ErrorInfo(ERRID.ERR_IllegalOperandInIIFCount)), argumentListSyntax.CloseParenToken);
				case 2:
					return SyntaxFactory.BinaryConditionalExpression(ifKeyword, argumentListSyntax.OpenParenToken, GetArgumentAsExpression(arguments[0]), (PunctuationSyntax)arguments.GetWithSeparators()[1], GetArgumentAsExpression(arguments[1]), argumentListSyntax.CloseParenToken);
				case 3:
					return SyntaxFactory.TernaryConditionalExpression(ifKeyword, argumentListSyntax.OpenParenToken, GetArgumentAsExpression(arguments[0]), (PunctuationSyntax)arguments.GetWithSeparators()[1], GetArgumentAsExpression(arguments[1]), (PunctuationSyntax)arguments.GetWithSeparators()[3], GetArgumentAsExpression(arguments[2]), argumentListSyntax.CloseParenToken);
				default:
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = arguments.GetWithSeparators();
					VisualBasicSyntaxNode[] array = new VisualBasicSyntaxNode[syntaxList.Count - 5 - 1 + 1];
					int num = syntaxList.Count - 1;
					for (int i = 5; i <= num; i++)
					{
						array[i - 5] = syntaxList[i];
					}
					return SyntaxFactory.TernaryConditionalExpression(ifKeyword, argumentListSyntax.OpenParenToken, GetArgumentAsExpression(arguments[0]), (PunctuationSyntax)syntaxList[1], GetArgumentAsExpression(arguments[1]), (PunctuationSyntax)syntaxList[3], GetArgumentAsExpression(arguments[2]), SyntaxNodeExtensions.AddLeadingSyntax(argumentListSyntax.CloseParenToken, SyntaxList.List(ArrayElement<GreenNode>.MakeElementArray(array)), ERRID.ERR_IllegalOperandInIIFCount));
				}
				}
			}
			return SyntaxFactory.BinaryConditionalExpression(ifKeyword, (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken));
		}

		private GetTypeExpressionSyntax ParseGetType()
		{
			KeywordSyntax getTypeKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token, createIfMissing: true);
			TypeSyntax type = ParseGeneralType(allowEmptyGenericArguments: true);
			PunctuationSyntax token2 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
			return SyntaxFactory.GetTypeExpression(getTypeKeyword, token, type, token2);
		}

		private NameOfExpressionSyntax ParseNameOf()
		{
			KeywordSyntax node = (KeywordSyntax)CurrentToken;
			node = CheckFeatureAvailability(Feature.NameOfExpressions, node);
			GetNextToken();
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token, createIfMissing: true);
			ExpressionSyntax argument = ValidateNameOfArgument(ParseExpressionCore(), isTopLevel: true);
			PunctuationSyntax token2 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
			return SyntaxFactory.NameOfExpression(node, token, argument, token2);
		}

		private ExpressionSyntax ValidateNameOfArgument(ExpressionSyntax argument, bool isTopLevel)
		{
			switch (argument.Kind)
			{
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GenericName:
				return argument;
			case SyntaxKind.MeExpression:
			case SyntaxKind.MyBaseExpression:
			case SyntaxKind.MyClassExpression:
			case SyntaxKind.NullableType:
			case SyntaxKind.PredefinedType:
			case SyntaxKind.GlobalName:
				if (isTopLevel)
				{
					return ReportSyntaxError(argument, ERRID.ERR_ExpressionDoesntHaveName);
				}
				return argument;
			case SyntaxKind.SimpleMemberAccessExpression:
			{
				MemberAccessExpressionSyntax memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)argument;
				if (memberAccessExpressionSyntax.Expression != null)
				{
					ExpressionSyntax expressionSyntax = ValidateNameOfArgument(memberAccessExpressionSyntax.Expression, isTopLevel: false);
					if (expressionSyntax != memberAccessExpressionSyntax.Expression)
					{
						memberAccessExpressionSyntax = SyntaxFactory.SimpleMemberAccessExpression(expressionSyntax, memberAccessExpressionSyntax.OperatorToken, memberAccessExpressionSyntax.Name);
					}
				}
				return memberAccessExpressionSyntax;
			}
			default:
				return ReportSyntaxError(argument, isTopLevel ? ERRID.ERR_ExpressionDoesntHaveName : ERRID.ERR_InvalidNameOfSubExpression);
			}
		}

		private ExpressionSyntax ParseGetXmlNamespace()
		{
			KeywordSyntax getXmlNamespaceKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				ResetCurrentToken(ScannerState.Element);
				PunctuationSyntax token = null;
				VerifyExpectedToken(SyntaxKind.OpenParenToken, ref token, ScannerState.Element);
				XmlPrefixNameSyntax name = null;
				if (CurrentToken.Kind == SyntaxKind.XmlNameToken)
				{
					name = SyntaxFactory.XmlPrefixName((XmlNameTokenSyntax)CurrentToken);
					GetNextToken(ScannerState.Element);
				}
				PunctuationSyntax token2 = null;
				VerifyExpectedToken(SyntaxKind.CloseParenToken, ref token2);
				GetXmlNamespaceExpressionSyntax xmlNamespaceExpression = SyntaxFactory.GetXmlNamespaceExpression(getXmlNamespaceKeyword, token, name, token2);
				xmlNamespaceExpression = AdjustTriviaForMissingTokens(xmlNamespaceExpression);
				return TransitionFromXmlToVB(xmlNamespaceExpression);
			}
			PunctuationSyntax openParenToken = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.OpenParenToken);
			PunctuationSyntax closeParenToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken);
			return SyntaxFactory.GetXmlNamespaceExpression(getXmlNamespaceKeyword, openParenToken, null, closeParenToken);
		}

		private ExpressionSyntax ParseCastExpression()
		{
			KeywordSyntax keyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token, createIfMissing: true);
			ExpressionSyntax expression = ParseExpressionCore();
			PunctuationSyntax token2 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
			return SyntaxFactory.PredefinedCastExpression(keyword, token, expression, token2);
		}

		private ExpressionSyntax ParseNewExpression()
		{
			KeywordSyntax newKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			if (CurrentToken.Kind == SyntaxKind.WithKeyword)
			{
				ObjectMemberInitializerSyntax initializer = ParseObjectInitializerList(anonymousTypeInitializer: true);
				return SyntaxFactory.AnonymousObjectCreationExpression(newKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), initializer);
			}
			bool allowedEmptyGenericArguments = false;
			TypeSyntax typeSyntax = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
			if (typeSyntax.ContainsDiagnostics)
			{
				typeSyntax = ResyncAt(typeSyntax, SyntaxKind.OpenParenToken);
			}
			ArgumentListSyntax argumentListSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				argumentListSyntax = ParseParenthesizedArguments(RedimOrNewParent: true);
				bool flag = false;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> arrayModifiers = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>);
				if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
				{
					arrayModifiers = ParseArrayRankSpecifiers(ERRID.ERR_NoConstituentArraySizes);
					flag = true;
				}
				else if (CurrentToken.Kind == SyntaxKind.OpenBraceToken)
				{
					if (TryReinterpretAsArraySpecifier(argumentListSyntax, ref arrayModifiers))
					{
						argumentListSyntax = null;
					}
					flag = true;
				}
				if (flag)
				{
					CollectionInitializerSyntax initializer2 = ParseCollectionInitializer();
					return SyntaxFactory.ArrayCreationExpression(newKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax, argumentListSyntax, arrayModifiers, initializer2);
				}
			}
			KeywordSyntax k = null;
			if (TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.FromKeyword, ref k) && (PeekToken(1).Kind == SyntaxKind.OpenBraceToken || PeekToken(1).Kind == SyntaxKind.StatementTerminatorToken))
			{
				GetNextToken();
				ObjectCollectionInitializerSyntax objectCollectionInitializerSyntax = ParseObjectCollectionInitializer(k);
				if (CurrentToken.Kind == SyntaxKind.WithKeyword)
				{
					objectCollectionInitializerSyntax = ReportSyntaxError(objectCollectionInitializerSyntax, ERRID.ERR_CantCombineInitializers);
				}
				return SyntaxFactory.ObjectCreationExpression(newKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax, argumentListSyntax, objectCollectionInitializerSyntax);
			}
			if (CurrentToken.Kind == SyntaxKind.WithKeyword)
			{
				ObjectMemberInitializerSyntax objectMemberInitializerSyntax = ParseObjectInitializerList();
				if (CurrentToken.Kind == SyntaxKind.WithKeyword)
				{
					objectMemberInitializerSyntax = ReportSyntaxError(objectMemberInitializerSyntax, ERRID.ERR_CantCombineInitializers);
				}
				if (TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.FromKeyword, ref k) && PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
				{
					objectMemberInitializerSyntax = ReportSyntaxError(objectMemberInitializerSyntax, ERRID.ERR_CantCombineInitializers);
				}
				return SyntaxFactory.ObjectCreationExpression(newKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax, argumentListSyntax, objectMemberInitializerSyntax);
			}
			return SyntaxFactory.ObjectCreationExpression(newKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax, argumentListSyntax, null);
		}

		private TypeOfExpressionSyntax ParseTypeOf()
		{
			KeywordSyntax typeOfKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore(OperatorPrecedence.PrecedenceRelational);
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.IsKeyword, SyntaxKind.IsNotKeyword);
			}
			KeywordSyntax keywordSyntax = null;
			SyntaxToken currentToken = CurrentToken;
			if (currentToken.Kind == SyntaxKind.IsKeyword || currentToken.Kind == SyntaxKind.IsNotKeyword)
			{
				keywordSyntax = (KeywordSyntax)currentToken;
				if (keywordSyntax.Kind == SyntaxKind.IsNotKeyword)
				{
					keywordSyntax = CheckFeatureAvailability(Feature.TypeOfIsNot, keywordSyntax);
				}
				GetNextToken();
				TryEatNewLine();
			}
			else
			{
				keywordSyntax = (KeywordSyntax)HandleUnexpectedToken(SyntaxKind.IsKeyword);
			}
			TypeSyntax type = ParseGeneralType();
			SyntaxKind kind = ((keywordSyntax.Kind == SyntaxKind.IsNotKeyword) ? SyntaxKind.TypeOfIsNotExpression : SyntaxKind.TypeOfIsExpression);
			return SyntaxFactory.TypeOfExpression(kind, typeOfKeyword, expressionSyntax, keywordSyntax, type);
		}

		private ExpressionSyntax ParseVariable()
		{
			return ParseExpressionCore(OperatorPrecedence.PrecedenceRelational);
		}

		private ExpressionSyntax ParseQualifiedExpr(ExpressionSyntax Term)
		{
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
			SyntaxToken prevToken = PrevToken;
			GetNextToken();
			if (punctuationSyntax.Kind == SyntaxKind.ExclamationToken)
			{
				IdentifierNameSyntax name = ParseIdentifierNameAllowingKeyword();
				return SyntaxFactory.DictionaryAccessExpression(Term, punctuationSyntax, name);
			}
			if (CurrentToken.IsEndOfLine && !CurrentToken.IsEndOfParse)
			{
				if (prevToken == null || prevToken.Kind == SyntaxKind.StatementTerminatorToken)
				{
					IdentifierTokenSyntax identifier = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), ERRID.ERR_ExpectedIdentifier);
					return SyntaxFactory.SimpleMemberAccessExpression(Term, punctuationSyntax, SyntaxFactory.IdentifierName(identifier));
				}
				if (!NextLineStartsWithStatementTerminator())
				{
					TryEatNewLineIfNotFollowedBy(SyntaxKind.DotToken);
				}
			}
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.AtToken:
			{
				PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
				XmlNodeSyntax xmlNodeSyntax;
				if (punctuationSyntax2.HasTrailingTrivia)
				{
					GetNextToken();
					punctuationSyntax2 = ReportSyntaxError(punctuationSyntax2, ERRID.ERR_ExpectedXmlName);
					punctuationSyntax2 = SyntaxNodeExtensions.AddTrailingSyntax(punctuationSyntax2, ResyncAt());
					xmlNodeSyntax = SyntaxFactory.XmlName(null, (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken));
				}
				else if (PeekNextToken().Kind == SyntaxKind.LessThanToken)
				{
					GetNextToken(ScannerState.Element);
					xmlNodeSyntax = ParseBracketedXmlQualifiedName();
				}
				else
				{
					GetNextToken();
					xmlNodeSyntax = ParseXmlQualifiedNameVB();
					if (xmlNodeSyntax.HasLeadingTrivia)
					{
						punctuationSyntax2 = ReportSyntaxError(punctuationSyntax2, ERRID.ERR_ExpectedXmlName);
						SyntaxNodeExtensions.AddTrailingSyntax(punctuationSyntax2, xmlNodeSyntax);
						SyntaxNodeExtensions.AddTrailingSyntax(punctuationSyntax2, ResyncAt());
						xmlNodeSyntax = SyntaxFactory.XmlName(null, (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken));
					}
				}
				return SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlAttributeAccessExpression, Term, punctuationSyntax, punctuationSyntax2, null, xmlNodeSyntax);
			}
			case SyntaxKind.LessThanToken:
			{
				XmlBracketedNameSyntax name4 = ParseBracketedXmlQualifiedName();
				return SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlElementAccessExpression, Term, punctuationSyntax, null, null, name4);
			}
			case SyntaxKind.DotToken:
			{
				if (PeekToken(1).Kind == SyntaxKind.DotToken)
				{
					PunctuationSyntax token = (PunctuationSyntax)CurrentToken;
					GetNextToken();
					PunctuationSyntax token2 = null;
					TryGetToken(SyntaxKind.DotToken, ref token2);
					TryEatNewLineIfFollowedBy(SyntaxKind.LessThanToken);
					XmlBracketedNameSyntax name3 = ((CurrentToken.Kind != SyntaxKind.LessThanToken) ? ReportExpectedXmlBracketedName() : ParseBracketedXmlQualifiedName());
					return SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlDescendantAccessExpression, Term, punctuationSyntax, token, token2, name3);
				}
				ExpressionSyntax result;
				if (CurrentToken.Kind == SyntaxKind.AtToken)
				{
					XmlNameTokenSyntax localName = (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken);
					result = SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlAttributeAccessExpression, Term, punctuationSyntax, null, null, ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlName(null, localName), ERRID.ERR_ExpectedXmlName));
				}
				else
				{
					result = SyntaxFactory.SimpleMemberAccessExpression(Term, punctuationSyntax, ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()), ERRID.ERR_ExpectedIdentifier));
				}
				return result;
			}
			default:
			{
				SimpleNameSyntax name2 = ParseSimpleNameExpressionAllowingKeywordAndTypeArguments();
				return SyntaxFactory.SimpleMemberAccessExpression(Term, punctuationSyntax, name2);
			}
			}
		}

		private void RescanTrailingColonAsToken(ref SyntaxToken prevToken, ref SyntaxToken currentToken)
		{
			_scanner.RescanTrailingColonAsToken(ref prevToken, ref currentToken);
			_currentToken = currentToken;
		}

		private T TransitionFromXmlToVB<T>(T node) where T : VisualBasicSyntaxNode
		{
			node = LastTokenReplacer.Replace(node, delegate(SyntaxToken token)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(token.GetTrailingTrivia());
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toRemove = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toAdd = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
				_scanner.TransitionFromXmlToVB(syntaxList, ref toRemove, ref toAdd);
				token = (SyntaxToken)token.WithTrailingTrivia(SyntaxNodeExtensions.GetStartOfTrivia(syntaxList, syntaxList.Count - toRemove.Count).Node);
				token = SyntaxToken.AddTrailingTrivia(token, toAdd);
				return token;
			});
			_currentToken = null;
			return node;
		}

		private T TransitionFromVBToXml<T>(ScannerState state, T node) where T : VisualBasicSyntaxNode
		{
			node = LastTokenReplacer.Replace(node, delegate(SyntaxToken token)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(token.GetTrailingTrivia());
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toRemove = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> toAdd = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
				_scanner.TransitionFromVBToXml(state, syntaxList, ref toRemove, ref toAdd);
				token = (SyntaxToken)token.WithTrailingTrivia(SyntaxNodeExtensions.GetStartOfTrivia(syntaxList, syntaxList.Count - toRemove.Count).Node);
				token = SyntaxToken.AddTrailingTrivia(token, toAdd);
				return token;
			});
			_currentToken = null;
			return node;
		}

		private XmlBracketedNameSyntax ParseBracketedXmlQualifiedName()
		{
			PunctuationSyntax token = null;
			ResetCurrentToken(ScannerState.Content);
			if (TryGetToken(SyntaxKind.LessThanToken, ref token))
			{
				ResetCurrentToken(ScannerState.Element);
				XmlNodeSyntax xmlNodeSyntax = ParseXmlQualifiedName(requireLeadingWhitespace: false, allowExpr: false, ScannerState.Element, ScannerState.Element);
				PunctuationSyntax token2 = null;
				VerifyExpectedToken(SyntaxKind.GreaterThanToken, ref token2);
				XmlBracketedNameSyntax node = SyntaxFactory.XmlBracketedName(token, (XmlNameSyntax)xmlNodeSyntax, token2);
				node = AdjustTriviaForMissingTokens(node);
				node = TransitionFromXmlToVB(node);
				return (XmlBracketedNameSyntax)new XmlWhitespaceChecker().Visit(node);
			}
			ResetCurrentToken(ScannerState.VB);
			return ReportExpectedXmlBracketedName();
		}

		private XmlBracketedNameSyntax ReportExpectedXmlBracketedName()
		{
			PunctuationSyntax lessThanToken = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.LessThanToken);
			XmlNameSyntax name = ReportExpectedXmlName();
			PunctuationSyntax greaterThanToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanToken);
			return SyntaxFactory.XmlBracketedName(lessThanToken, name, greaterThanToken);
		}

		private XmlNameSyntax ReportExpectedXmlName()
		{
			return ReportSyntaxError(SyntaxFactory.XmlName(null, SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, null, null)), ERRID.ERR_ExpectedXmlName);
		}

		private ExpressionSyntax ParseParenthesizedExpressionOrTupleLiteral()
		{
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.ColonEqualsToken)
			{
				IdentifierNameSyntax name = ParseIdentifierNameAllowingKeyword();
				PunctuationSyntax token2 = null;
				TryGetTokenAndEatNewLine(SyntaxKind.ColonEqualsToken, ref token2);
				NameColonEqualsSyntax nameColonEquals = SyntaxFactory.NameColonEquals(name, token2);
				SimpleArgumentSyntax firstArgument = SyntaxFactory.SimpleArgument(nameColonEquals, ParseExpressionCore());
				return ParseTheRestOfTupleLiteral(token, firstArgument);
			}
			ExpressionSyntax expression = ParseExpressionCore();
			if (CurrentToken.Kind == SyntaxKind.CommaToken)
			{
				SimpleArgumentSyntax firstArgument2 = SyntaxFactory.SimpleArgument(null, expression);
				return ParseTheRestOfTupleLiteral(token, firstArgument2);
			}
			PunctuationSyntax token3 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3, createIfMissing: true);
			return SyntaxFactory.ParenthesizedExpression(token, expression, token3);
		}

		private TupleExpressionSyntax ParseTheRestOfTupleLiteral(PunctuationSyntax openParen, SimpleArgumentSyntax firstArgument)
		{
			SeparatedSyntaxListBuilder<SimpleArgumentSyntax> item = _pool.AllocateSeparated<SimpleArgumentSyntax>();
			item.Add(firstArgument);
			while (CurrentToken.Kind == SyntaxKind.CommaToken)
			{
				PunctuationSyntax token = null;
				TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token);
				item.AddSeparator(token);
				NameColonEqualsSyntax nameColonEquals = null;
				if (CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.ColonEqualsToken)
				{
					IdentifierNameSyntax name = ParseIdentifierNameAllowingKeyword();
					PunctuationSyntax token2 = null;
					TryGetTokenAndEatNewLine(SyntaxKind.ColonEqualsToken, ref token2);
					nameColonEquals = SyntaxFactory.NameColonEquals(name, token2);
				}
				SimpleArgumentSyntax node = SyntaxFactory.SimpleArgument(nameColonEquals, ParseExpressionCore());
				item.Add(node);
			}
			PunctuationSyntax token3 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3, createIfMissing: true);
			if (item.Count < 2)
			{
				item.AddSeparator(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
				IdentifierNameSyntax syntax = SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
				syntax = ReportSyntaxError(syntax, ERRID.ERR_TupleTooFewElements);
				item.Add(SyntaxFactory.SimpleArgument(null, syntax));
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SimpleArgumentSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			TupleExpressionSyntax node2 = SyntaxFactory.TupleExpression(openParen, separatedSyntaxList, token3);
			return CheckFeatureAvailability(Feature.Tuples, node2);
		}

		internal ArgumentListSyntax ParseParenthesizedArguments(bool RedimOrNewParent = false, bool attributeListParent = false)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>);
			PunctuationSyntax token = null;
			PunctuationSyntax token2 = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
			GreenNode unexpected = null;
			separatedSyntaxList = ParseArguments(ref unexpected, RedimOrNewParent, attributeListParent);
			if (!TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2))
			{
				if (PeekAheadFor(SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken) == SyntaxKind.CloseParenToken)
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected2 = ResyncAt(new SyntaxKind[1] { SyntaxKind.CloseParenToken });
					token2 = (PunctuationSyntax)CurrentToken;
					token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, unexpected2);
					GetNextToken();
				}
				else
				{
					token2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken);
					token2 = ReportSyntaxError(token2, ERRID.ERR_ExpectedRparen);
				}
			}
			if (unexpected != null)
			{
				token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, unexpected);
			}
			return SyntaxFactory.ArgumentList(token, separatedSyntaxList, token2);
		}

		private ExpressionSyntax ParseParenthesizedQualifier(ExpressionSyntax Term, bool RedimOrNewParent = false)
		{
			ArgumentListSyntax argumentList = ParseParenthesizedArguments(RedimOrNewParent);
			return SyntaxFactory.InvocationExpression(Term, argumentList);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> ParseArguments(ref GreenNode unexpected, bool RedimOrNewParent = false, bool attributeListParent = false)
		{
			SeparatedSyntaxListBuilder<ArgumentSyntax> item = _pool.AllocateSeparated<ArgumentSyntax>();
			bool allowNonTrailingNamedArguments = LanguageVersionFacts.AllowNonTrailingNamedArguments(_scanner.Options.LanguageVersion);
			bool seenNames = false;
			while (true)
			{
				bool flag = false;
				if ((CurrentToken.Kind == SyntaxKind.IdentifierToken || CurrentToken.IsKeyword) && PeekToken(1).Kind == SyntaxKind.ColonEqualsToken)
				{
					seenNames = true;
					flag = true;
				}
				PunctuationSyntax token = null;
				if (flag)
				{
					if (attributeListParent)
					{
						ParseNamedArguments(item);
						break;
					}
					IdentifierNameSyntax name = ParseIdentifierNameAllowingKeyword();
					PunctuationSyntax token2 = null;
					TryGetTokenAndEatNewLine(SyntaxKind.ColonEqualsToken, ref token2);
					SimpleArgumentSyntax node = SyntaxFactory.SimpleArgument(SyntaxFactory.NameColonEquals(name, token2), ParseExpressionCore());
					item.Add(node);
				}
				else
				{
					if (CurrentToken.Kind == SyntaxKind.CommaToken)
					{
						TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token);
						ArgumentSyntax node2 = ReportNonTrailingNamedArgumentIfNeeded(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OmittedArgument(), seenNames, allowNonTrailingNamedArguments);
						item.Add(node2);
						item.AddSeparator(token);
						continue;
					}
					if (CurrentToken.Kind == SyntaxKind.CloseParenToken)
					{
						if (item.Count > 0)
						{
							ArgumentSyntax node3 = ReportNonTrailingNamedArgumentIfNeeded(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OmittedArgument(), seenNames, allowNonTrailingNamedArguments);
							item.Add(node3);
						}
						break;
					}
					ArgumentSyntax argument = ParseArgument(RedimOrNewParent);
					argument = ReportNonTrailingNamedArgumentIfNeeded(argument, seenNames, allowNonTrailingNamedArguments);
					item.Add(argument);
				}
				if (TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					item.AddSeparator(token);
					continue;
				}
				if (CurrentToken.Kind == SyntaxKind.CloseParenToken || MustEndStatement(CurrentToken))
				{
					break;
				}
				GreenNode greenNode = ResyncAt(new SyntaxKind[2]
				{
					SyntaxKind.CommaToken,
					SyntaxKind.CloseParenToken
				}).Node;
				if (greenNode != null)
				{
					greenNode = ReportSyntaxError(greenNode, ERRID.ERR_ArgumentSyntax);
				}
				if (CurrentToken.Kind == SyntaxKind.CommaToken)
				{
					token = (PunctuationSyntax)CurrentToken;
					token = SyntaxNodeExtensions.AddLeadingSyntax(token, greenNode);
					item.AddSeparator(token);
					GetNextToken();
					continue;
				}
				unexpected = greenNode;
				break;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private static ArgumentSyntax ReportNonTrailingNamedArgumentIfNeeded(ArgumentSyntax argument, bool seenNames, bool allowNonTrailingNamedArguments)
		{
			if (!seenNames || allowNonTrailingNamedArguments)
			{
				return argument;
			}
			return ReportSyntaxError(argument, ERRID.ERR_ExpectedNamedArgument, new VisualBasicRequiredLanguageVersion(FeatureExtensions.GetLanguageVersion(Feature.NonTrailingNamedArguments)));
		}

		private void ParseNamedArguments(SeparatedSyntaxListBuilder<ArgumentSyntax> arguments)
		{
			SimpleArgumentSyntax simpleArgumentSyntax;
			while (true)
			{
				PunctuationSyntax token = null;
				bool flag = false;
				IdentifierNameSyntax identifierNameSyntax;
				if ((CurrentToken.Kind == SyntaxKind.IdentifierToken || CurrentToken.IsKeyword) && PeekToken(1).Kind == SyntaxKind.ColonEqualsToken)
				{
					identifierNameSyntax = ParseIdentifierNameAllowingKeyword();
					TryGetTokenAndEatNewLine(SyntaxKind.ColonEqualsToken, ref token);
				}
				else
				{
					identifierNameSyntax = SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
					token = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonEqualsToken);
					flag = true;
				}
				if (flag)
				{
					identifierNameSyntax = ReportSyntaxError(identifierNameSyntax, ERRID.ERR_ExpectedNamedArgumentInAttributeList);
				}
				simpleArgumentSyntax = SyntaxFactory.SimpleArgument(SyntaxFactory.NameColonEquals(identifierNameSyntax, token), ParseExpressionCore());
				if (CurrentToken.Kind != SyntaxKind.CommaToken)
				{
					if (CurrentToken.Kind == SyntaxKind.CloseParenToken || MustEndStatement(CurrentToken))
					{
						arguments.Add(simpleArgumentSyntax);
						return;
					}
					simpleArgumentSyntax = ReportSyntaxError(simpleArgumentSyntax, ERRID.ERR_ArgumentSyntax);
					simpleArgumentSyntax = ResyncAt(simpleArgumentSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
					if (CurrentToken.Kind != SyntaxKind.CommaToken)
					{
						break;
					}
				}
				PunctuationSyntax token2 = null;
				TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2);
				arguments.Add(simpleArgumentSyntax);
				arguments.AddSeparator(token2);
			}
			arguments.Add(simpleArgumentSyntax);
		}

		private ArgumentSyntax ParseArgument(bool RedimOrNewParent = false)
		{
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
			}
			if (RedimOrNewParent && CurrentToken.Kind == SyntaxKind.ToKeyword)
			{
				KeywordSyntax toKeyword = (KeywordSyntax)CurrentToken;
				ExpressionSyntax lowerBound = expressionSyntax;
				GetNextToken();
				expressionSyntax = ParseExpressionCore();
				return SyntaxFactory.RangeArgument(lowerBound, toKeyword, expressionSyntax);
			}
			return SyntaxFactory.SimpleArgument(null, expressionSyntax);
		}

		private CastExpressionSyntax ParseCast()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			SyntaxKind kind = keywordSyntax.Kind;
			GetNextToken();
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token, createIfMissing: true);
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
			}
			PunctuationSyntax token2 = null;
			if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
			{
				token2 = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CommaToken), ERRID.ERR_SyntaxInCastOp);
			}
			TypeSyntax type = ParseGeneralType();
			PunctuationSyntax token3 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3, createIfMissing: true);
			CastExpressionSyntax castExpressionSyntax = null;
			return kind switch
			{
				SyntaxKind.CTypeKeyword => SyntaxFactory.CTypeExpression(keywordSyntax, token, expressionSyntax, token2, type, token3), 
				SyntaxKind.DirectCastKeyword => SyntaxFactory.DirectCastExpression(keywordSyntax, token, expressionSyntax, token2, type, token3), 
				SyntaxKind.TryCastKeyword => SyntaxFactory.TryCastExpression(keywordSyntax, token, expressionSyntax, token2, type, token3), 
				_ => throw ExceptionUtilities.UnexpectedValue(kind), 
			};
		}

		private LambdaHeaderSyntax ParseFunctionOrSubLambdaHeader(out bool isMultiLine, bool parseModifiers = false)
		{
			bool isInMethodDeclarationHeader = _isInMethodDeclarationHeader;
			_isInMethodDeclarationHeader = true;
			bool isInAsyncMethodDeclarationHeader = _isInAsyncMethodDeclarationHeader;
			bool isInIteratorMethodDeclarationHeader = _isInIteratorMethodDeclarationHeader;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList;
			if (parseModifiers)
			{
				syntaxList = ParseSpecifiers();
				_isInAsyncMethodDeclarationHeader = syntaxList.Any(630);
				_isInIteratorMethodDeclarationHeader = syntaxList.Any(632);
			}
			else
			{
				syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>);
				_isInAsyncMethodDeclarationHeader = false;
				_isInIteratorMethodDeclarationHeader = false;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			TypeParameterListSyntax genericParams = null;
			PunctuationSyntax openParen = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>);
			PunctuationSyntax closeParen = null;
			isMultiLine = false;
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				TryRejectGenericParametersForMemberDecl(ref genericParams);
			}
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
			}
			else
			{
				openParen = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.OpenParenToken);
				closeParen = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.CloseParenToken);
			}
			if (genericParams != null)
			{
				openParen = SyntaxNodeExtensions.AddLeadingSyntax(openParen, genericParams, ERRID.ERR_GenericParamsOnInvalidMember);
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList2 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
			KeywordSyntax keywordSyntax2 = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				if (CurrentToken.Kind == SyntaxKind.LessThanToken)
				{
					keywordSyntax2 = SyntaxNodeExtensions.AddTrailingSyntax(keywordSyntax2, ParseAttributeLists(allowFileLevelAttributes: false).Node, ERRID.ERR_AttributeOnLambdaReturnType);
				}
				TypeSyntax typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(keywordSyntax2, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
				isMultiLine = true;
			}
			if (keywordSyntax.Kind != SyntaxKind.FunctionKeyword && simpleAsClauseSyntax != null)
			{
				closeParen = SyntaxNodeExtensions.AddTrailingSyntax(closeParen, simpleAsClauseSyntax, ERRID.ERR_ExpectedEOS);
				simpleAsClauseSyntax = null;
			}
			isMultiLine = isMultiLine || CurrentToken.Kind == SyntaxKind.StatementTerminatorToken;
			_isInMethodDeclarationHeader = isInMethodDeclarationHeader;
			_isInAsyncMethodDeclarationHeader = isInAsyncMethodDeclarationHeader;
			_isInIteratorMethodDeclarationHeader = isInIteratorMethodDeclarationHeader;
			return SyntaxFactory.LambdaHeader((keywordSyntax.Kind == SyntaxKind.FunctionKeyword) ? SyntaxKind.FunctionLambdaHeader : SyntaxKind.SubLambdaHeader, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), syntaxList, keywordSyntax, SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen), simpleAsClauseSyntax);
		}

		private ExpressionSyntax ParseLambda(bool parseModifiers)
		{
			bool isMultiLine = false;
			LambdaHeaderSyntax node = ParseFunctionOrSubLambdaHeader(out isMultiLine, parseModifiers);
			node = AdjustTriviaForMissingTokens(node);
			ExpressionSyntax node2;
			if (node.Kind == SyntaxKind.FunctionLambdaHeader && !isMultiLine)
			{
				SingleLineLambdaContext singleLineLambdaContext = (SingleLineLambdaContext)(_context = new SingleLineLambdaContext(node, _context));
				node2 = SyntaxFactory.SingleLineLambdaExpression(SyntaxKind.SingleLineFunctionLambdaExpression, node, ParseExpressionCore());
				node2 = AdjustTriviaForMissingTokens(node2);
				if (node.Modifiers.Any(632))
				{
					node2 = ReportSyntaxError(node2, ERRID.ERR_BadIteratorExpressionLambda);
				}
				_context = singleLineLambdaContext.PrevBlock;
			}
			else
			{
				BlockContext context = _context;
				MethodBlockContext methodBlockContext = (MethodBlockContext)(_context = ((!isMultiLine) ? ((MethodBlockContext)new SingleLineLambdaContext(node, context)) : ((MethodBlockContext)new LambdaContext(node, context))));
				if (isMultiLine || CurrentToken.Kind == SyntaxKind.ColonToken)
				{
					_context = _context.ResyncAndProcessStatementTerminator(node, methodBlockContext);
				}
				StatementSyntax statementSyntax = null;
				while (_context.Level >= methodBlockContext.Level)
				{
					if (CurrentToken.IsEndOfParse)
					{
						_context = BlockContextExtensions.EndLambda(_context);
						break;
					}
					statementSyntax = _context.Parse();
					statementSyntax = AdjustTriviaForMissingTokens(statementSyntax);
					bool flag = IsDeclarationStatement(statementSyntax.Kind);
					if (flag)
					{
						_context.Add(ReportSyntaxError(statementSyntax, ERRID.ERR_InvInsideEndsProc));
					}
					else
					{
						_context = _context.LinkSyntax(statementSyntax);
						if (_context.Level < methodBlockContext.Level)
						{
							break;
						}
					}
					_context = _context.ResyncAndProcessStatementTerminator(statementSyntax, methodBlockContext);
					statementSyntax = null;
					if (flag)
					{
						if (_context.Level >= methodBlockContext.Level)
						{
							_context = BlockContextExtensions.EndLambda(_context);
						}
						break;
					}
				}
				node2 = (ExpressionSyntax)methodBlockContext.CreateBlockSyntax(statementSyntax);
			}
			if (isMultiLine)
			{
				node2 = CheckFeatureAvailability(Feature.StatementLambdas, node2);
			}
			return node2;
		}

		internal static bool IsDeclarationStatement(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.NamespaceStatement:
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.StructureStatement:
			case SyntaxKind.InterfaceStatement:
			case SyntaxKind.ClassStatement:
			case SyntaxKind.EnumStatement:
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
				return true;
			default:
				return false;
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> ParseVariableList()
		{
			SeparatedSyntaxListBuilder<ExpressionSyntax> item = _pool.AllocateSeparated<ExpressionSyntax>();
			while (true)
			{
				item.Add(ParseVariable());
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private AwaitExpressionSyntax ParseAwaitExpression(KeywordSyntax awaitKeyword = null)
		{
			if (awaitKeyword == null)
			{
				TryIdentifierAsContextualKeyword(CurrentToken, ref awaitKeyword);
			}
			GetNextToken();
			ExpressionSyntax expression = ParseTerm();
			return SyntaxFactory.AwaitExpression(awaitKeyword, expression);
		}

		private InterpolatedStringExpressionSyntax ParseInterpolatedStringExpression()
		{
			ResetCurrentToken(ScannerState.InterpolatedStringPunctuation);
			PunctuationSyntax dollarSignDoubleQuoteToken = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.InterpolatedStringContent);
			SyntaxListBuilder<InterpolatedStringContentSyntax> item = _pool.Allocate<InterpolatedStringContentSyntax>();
			SyntaxListBuilder<SyntaxToken> item2 = default(SyntaxListBuilder<SyntaxToken>);
			PunctuationSyntax punctuationSyntax;
			while (true)
			{
				SyntaxKind kind = CurrentToken.Kind;
				InterpolatedStringContentSyntax node;
				if (kind <= SyntaxKind.CloseBraceToken)
				{
					if (kind != SyntaxKind.OpenBraceToken)
					{
						if (kind != SyntaxKind.CloseBraceToken)
						{
							goto IL_0106;
						}
						if (item2.IsNull)
						{
							item2 = _pool.Allocate<SyntaxToken>();
						}
						item2.Add(CurrentToken);
						GetNextToken(ScannerState.InterpolatedStringContent);
						continue;
					}
					node = ParseInterpolatedStringInterpolation();
				}
				else
				{
					if (kind == SyntaxKind.DoubleQuoteToken)
					{
						punctuationSyntax = (PunctuationSyntax)CurrentToken;
						GetNextToken();
						break;
					}
					if (kind != SyntaxKind.InterpolatedStringTextToken)
					{
						if (kind != SyntaxKind.EndOfInterpolatedStringToken)
						{
							goto IL_0106;
						}
						punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken);
						GetNextToken();
						break;
					}
					InterpolatedStringTextTokenSyntax textToken = (InterpolatedStringTextTokenSyntax)CurrentToken;
					GetNextToken(ScannerState.InterpolatedStringPunctuation);
					node = SyntaxFactory.InterpolatedStringText(textToken);
				}
				if (!item2.IsNull)
				{
					node = SyntaxNodeExtensions.AddLeadingSyntax(node, _pool.ToListAndFree(item2), ERRID.ERR_Syntax);
					item2 = default(SyntaxListBuilder<SyntaxToken>);
				}
				item.Add(node);
				continue;
				IL_0106:
				punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken);
				break;
			}
			if (!item2.IsNull)
			{
				punctuationSyntax = SyntaxNodeExtensions.AddLeadingSyntax(punctuationSyntax, _pool.ToListAndFree(item2), ERRID.ERR_Syntax);
				item2 = default(SyntaxListBuilder<SyntaxToken>);
			}
			InterpolatedStringExpressionSyntax node2 = SyntaxFactory.InterpolatedStringExpression(dollarSignDoubleQuoteToken, _pool.ToListAndFree(item), punctuationSyntax);
			return CheckFeatureAvailability(Feature.InterpolatedStrings, node2);
		}

		private InterpolationSyntax ParseInterpolatedStringInterpolation()
		{
			PunctuationSyntax colonToken = null;
			string excessText = null;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax;
			if (CurrentToken.Kind == SyntaxKind.ColonToken)
			{
				punctuationSyntax = (PunctuationSyntax)RemoveTrailingColonTriviaAndConvertToColonToken(punctuationSyntax, out colonToken, out excessText);
				expressionSyntax = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ERRID.ERR_ExpectedExpression);
			}
			else
			{
				expressionSyntax = ParseExpressionCore();
				if (CurrentToken.Kind == SyntaxKind.ColonToken)
				{
					expressionSyntax = (ExpressionSyntax)RemoveTrailingColonTriviaAndConvertToColonToken(expressionSyntax, out colonToken, out excessText);
				}
			}
			InterpolationAlignmentClauseSyntax alignmentClause;
			if (CurrentToken.Kind == SyntaxKind.CommaToken)
			{
				PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				if (CurrentToken.Kind == SyntaxKind.ColonToken)
				{
					punctuationSyntax2 = (PunctuationSyntax)RemoveTrailingColonTriviaAndConvertToColonToken(punctuationSyntax2, out colonToken, out excessText);
				}
				PunctuationSyntax punctuationSyntax3;
				if (CurrentToken.Kind == SyntaxKind.MinusToken || CurrentToken.Kind == SyntaxKind.PlusToken)
				{
					punctuationSyntax3 = (PunctuationSyntax)CurrentToken;
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.ColonToken)
					{
						punctuationSyntax3 = (PunctuationSyntax)RemoveTrailingColonTriviaAndConvertToColonToken(punctuationSyntax3, out colonToken, out excessText);
					}
				}
				else
				{
					punctuationSyntax3 = null;
				}
				IntegerLiteralTokenSyntax token;
				if (CurrentToken.Kind == SyntaxKind.IntegerLiteralToken)
				{
					token = (IntegerLiteralTokenSyntax)CurrentToken;
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.ColonToken)
					{
						token = (IntegerLiteralTokenSyntax)RemoveTrailingColonTriviaAndConvertToColonToken(token, out colonToken, out excessText);
					}
				}
				else
				{
					token = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIntegerLiteralToken(), ERRID.ERR_ExpectedIntLiteral);
				}
				ExpressionSyntax expressionSyntax2 = SyntaxFactory.NumericLiteralExpression(token);
				if (punctuationSyntax3 != null)
				{
					expressionSyntax2 = SyntaxFactory.UnaryExpression((punctuationSyntax3.Kind == SyntaxKind.PlusToken) ? SyntaxKind.UnaryPlusExpression : SyntaxKind.UnaryMinusExpression, punctuationSyntax3, expressionSyntax2);
				}
				alignmentClause = SyntaxFactory.InterpolationAlignmentClause(punctuationSyntax2, expressionSyntax2);
			}
			else
			{
				alignmentClause = null;
			}
			InterpolationFormatClauseSyntax formatClause;
			if (CurrentToken.Kind == SyntaxKind.ColonToken && colonToken != null)
			{
				GetNextToken(ScannerState.InterpolatedStringFormatString);
				InterpolatedStringTextTokenSyntax interpolatedStringTextTokenSyntax;
				if (CurrentToken.Kind != SyntaxKind.InterpolatedStringTextToken)
				{
					interpolatedStringTextTokenSyntax = ((excessText == null) ? null : Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken(excessText, excessText, null, null));
				}
				else
				{
					interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)CurrentToken;
					GetNextToken(ScannerState.InterpolatedStringPunctuation);
					if (excessText != null)
					{
						interpolatedStringTextTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken(excessText + interpolatedStringTextTokenSyntax.Text, excessText + interpolatedStringTextTokenSyntax.Value, interpolatedStringTextTokenSyntax.GetLeadingTrivia(), interpolatedStringTextTokenSyntax.GetTrailingTrivia());
					}
				}
				if (interpolatedStringTextTokenSyntax == null)
				{
					interpolatedStringTextTokenSyntax = (InterpolatedStringTextTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.InterpolatedStringTextToken);
					interpolatedStringTextTokenSyntax = ReportSyntaxError(interpolatedStringTextTokenSyntax, ERRID.ERR_Syntax);
				}
				else if (interpolatedStringTextTokenSyntax.GetTrailingTrivia() != null)
				{
					interpolatedStringTextTokenSyntax = ReportSyntaxError(interpolatedStringTextTokenSyntax, ERRID.ERR_InterpolationFormatWhitespace);
				}
				formatClause = SyntaxFactory.InterpolationFormatClause(colonToken, interpolatedStringTextTokenSyntax);
			}
			else
			{
				formatClause = null;
				if (CurrentToken.Kind == SyntaxKind.ColonToken)
				{
					GetNextToken(ScannerState.InterpolatedStringFormatString);
				}
			}
			PunctuationSyntax closeBraceToken;
			if (CurrentToken.Kind == SyntaxKind.CloseBraceToken)
			{
				ResetCurrentToken(ScannerState.InterpolatedStringPunctuation);
				closeBraceToken = (PunctuationSyntax)CurrentToken;
				GetNextToken(ScannerState.InterpolatedStringContent);
			}
			else if (CurrentToken.Kind == SyntaxKind.EndOfInterpolatedStringToken)
			{
				GetNextToken();
				closeBraceToken = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.CloseBraceToken);
			}
			else
			{
				if (!IsValidStatementTerminator(CurrentToken))
				{
					ResetCurrentToken(ScannerState.InterpolatedStringFormatString);
				}
				closeBraceToken = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.CloseBraceToken);
				if (CurrentToken.Kind == SyntaxKind.InterpolatedStringTextToken)
				{
					ResetCurrentToken(ScannerState.InterpolatedStringContent);
					GetNextToken(ScannerState.InterpolatedStringContent);
				}
			}
			return SyntaxFactory.Interpolation(punctuationSyntax, expressionSyntax, alignmentClause, formatClause, closeBraceToken);
		}

		private static SyntaxToken RemoveTrailingColonTriviaAndConvertToColonToken(SyntaxToken token, out PunctuationSyntax colonToken, out string excessText)
		{
			if (!token.HasTrailingTrivia)
			{
				colonToken = null;
				excessText = null;
				return token;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(token.GetTrailingTrivia());
			int num = -1;
			GreenNode trivia2;
			if (trivia.Count == 1)
			{
				num = 0;
				trivia2 = null;
				excessText = null;
			}
			else if (trivia[0]!.Kind == SyntaxKind.ColonTrivia)
			{
				num = 0;
				trivia2 = null;
				excessText = SyntaxNodeExtensions.GetEndOfTrivia(trivia, 1).Node!.ToFullString();
			}
			else
			{
				int num2 = trivia.Count - 1;
				for (int i = 0; i <= num2; i++)
				{
					if (trivia[i]!.Kind == SyntaxKind.ColonTrivia)
					{
						num = i;
						break;
					}
				}
				trivia2 = SyntaxNodeExtensions.GetStartOfTrivia(trivia, num).Node;
				if (num == trivia.Count - 1)
				{
					excessText = null;
				}
				else
				{
					excessText = SyntaxNodeExtensions.GetEndOfTrivia(trivia, num + 1).Node!.ToFullString();
				}
			}
			SyntaxTrivia syntaxTrivia = (SyntaxTrivia)trivia[num];
			colonToken = new PunctuationSyntax(SyntaxKind.ColonToken, syntaxTrivia.Text, null, null);
			return (SyntaxToken)token.WithTrailingTrivia(trivia2);
		}

		private VisualBasicSyntaxNode RemoveTrailingColonTriviaAndConvertToColonToken(VisualBasicSyntaxNode node, out PunctuationSyntax colonToken, out string excessText)
		{
			SyntaxToken lastToken = node.GetLastToken();
			SyntaxToken syntaxToken = RemoveTrailingColonTriviaAndConvertToColonToken(lastToken, out colonToken, out excessText);
			VisualBasicSyntaxNode visualBasicSyntaxNode = LastTokenReplacer.Replace(node, (SyntaxToken t) => (t != lastToken) ? t : syntaxToken);
			if (visualBasicSyntaxNode == node)
			{
				colonToken = null;
				excessText = null;
			}
			return visualBasicSyntaxNode;
		}

		private ExpressionRangeVariableSyntax ParseSelectListInitializer()
		{
			VariableNameEqualsSyntax nameEquals = null;
			if (((CurrentToken.Kind == SyntaxKind.IdentifierToken || CurrentToken.IsKeyword) && PeekToken(1).Kind == SyntaxKind.EqualsToken) || (PeekToken(1).Kind == SyntaxKind.QuestionToken && PeekToken(2).Kind == SyntaxKind.EqualsToken))
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = null;
				PunctuationSyntax punctuationSyntax = null;
				modifiedIdentifierSyntax = ParseSimpleIdentifierAsModifiedIdentifier();
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				nameEquals = SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, null, punctuationSyntax);
			}
			ExpressionSyntax expression = ParseExpressionCore();
			return SyntaxFactory.ExpressionRangeVariable(nameEquals, expression);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> ParseSelectList()
		{
			SeparatedSyntaxListBuilder<ExpressionRangeVariableSyntax> item = _pool.AllocateSeparated<ExpressionRangeVariableSyntax>();
			while (true)
			{
				ExpressionRangeVariableSyntax expressionRangeVariableSyntax = ParseSelectListInitializer();
				if (expressionRangeVariableSyntax.ContainsDiagnostics)
				{
					expressionRangeVariableSyntax = ResyncAt(expressionRangeVariableSyntax, SyntaxKind.CommaToken, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.FromKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				}
				item.Add(expressionRangeVariableSyntax);
				if (CurrentToken.Kind != SyntaxKind.CommaToken)
				{
					break;
				}
				PunctuationSyntax separatorToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				item.AddSeparator(separatorToken);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private AggregationSyntax ParseAggregationExpression()
		{
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
			{
				IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)CurrentToken;
				if (identifierTokenSyntax.PossibleKeywordKind == SyntaxKind.GroupKeyword)
				{
					identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_InvalidUseOfKeyword);
					GetNextToken();
					return SyntaxFactory.FunctionAggregation(identifierTokenSyntax, null, null, null);
				}
			}
			IdentifierTokenSyntax identifierTokenSyntax2 = ParseIdentifier();
			FunctionAggregationSyntax functionAggregationSyntax = null;
			if (!identifierTokenSyntax2.ContainsDiagnostics && CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				PunctuationSyntax openParenToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				ExpressionSyntax syntax = null;
				if (CurrentToken.Kind != SyntaxKind.CloseParenToken)
				{
					syntax = ParseExpressionCore();
				}
				PunctuationSyntax token = null;
				if (TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token, createIfMissing: true) && syntax != null)
				{
					CheckForEndOfExpression(ref syntax);
				}
				functionAggregationSyntax = SyntaxFactory.FunctionAggregation(identifierTokenSyntax2, openParenToken, syntax, token);
			}
			else
			{
				functionAggregationSyntax = SyntaxFactory.FunctionAggregation(identifierTokenSyntax2, null, null, null);
				CheckForEndOfExpression(ref functionAggregationSyntax);
			}
			return functionAggregationSyntax;
		}

		private bool CheckForEndOfExpression<T>(ref T syntax) where T : VisualBasicSyntaxNode
		{
			if (!CurrentToken.IsBinaryOperator() && CurrentToken.Kind != SyntaxKind.DotToken)
			{
				return true;
			}
			syntax = ReportSyntaxError(syntax, ERRID.ERR_ExpectedEndOfExpression);
			return false;
		}

		private ModifiedIdentifierSyntax ParseSimpleIdentifierAsModifiedIdentifier()
		{
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (CurrentToken.Kind == SyntaxKind.QuestionToken)
			{
				SyntaxToken currentToken = CurrentToken;
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ReportSyntaxError(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported));
				GetNextToken();
			}
			return SyntaxFactory.ModifiedIdentifier(identifierTokenSyntax, null, null, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private AggregationRangeVariableSyntax ParseAggregateListInitializer(bool AllowGroupName)
		{
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = null;
			PunctuationSyntax punctuationSyntax = null;
			if (((CurrentToken.Kind == SyntaxKind.IdentifierToken || CurrentToken.IsKeyword) && PeekToken(1).Kind == SyntaxKind.EqualsToken) || (PeekToken(1).Kind == SyntaxKind.QuestionToken && PeekToken(2).Kind == SyntaxKind.EqualsToken))
			{
				modifiedIdentifierSyntax = ParseSimpleIdentifierAsModifiedIdentifier();
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
			}
			AggregationSyntax aggregationSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken || CurrentToken.IsKeyword)
			{
				KeywordSyntax k = null;
				if (TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.GroupKeyword, ref k) && PeekToken(1).Kind != SyntaxKind.OpenParenToken)
				{
					if (!AllowGroupName)
					{
						k = ReportSyntaxError(k, ERRID.ERR_UnexpectedGroup);
					}
					aggregationSyntax = SyntaxFactory.GroupAggregation(k);
					CheckForEndOfExpression(ref aggregationSyntax);
					GetNextToken();
				}
				else
				{
					aggregationSyntax = ParseAggregationExpression();
				}
			}
			else
			{
				IdentifierTokenSyntax syntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				syntax = ((!AllowGroupName) ? ReportSyntaxError(syntax, ERRID.ERR_ExpectedIdentifier) : ReportSyntaxError(syntax, ERRID.ERR_ExpectedIdentifierOrGroup));
				aggregationSyntax = SyntaxFactory.FunctionAggregation(syntax, null, null, null);
			}
			VariableNameEqualsSyntax nameEquals = null;
			if (modifiedIdentifierSyntax != null && punctuationSyntax != null)
			{
				nameEquals = SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, null, punctuationSyntax);
			}
			return SyntaxFactory.AggregationRangeVariable(nameEquals, aggregationSyntax);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> ParseAggregateList(bool AllowGroupName, bool IsGroupJoinProjection)
		{
			SeparatedSyntaxListBuilder<AggregationRangeVariableSyntax> item = _pool.AllocateSeparated<AggregationRangeVariableSyntax>();
			while (true)
			{
				AggregationRangeVariableSyntax aggregationRangeVariableSyntax = ParseAggregateListInitializer(AllowGroupName);
				if (aggregationRangeVariableSyntax.ContainsDiagnostics)
				{
					aggregationRangeVariableSyntax = ((!IsGroupJoinProjection) ? ResyncAt(aggregationRangeVariableSyntax, SyntaxKind.CommaToken, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.FromKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword) : ResyncAt(aggregationRangeVariableSyntax, SyntaxKind.CommaToken, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.FromKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.OnKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword));
				}
				item.Add(aggregationRangeVariableSyntax);
				if (CurrentToken.Kind != SyntaxKind.CommaToken)
				{
					break;
				}
				PunctuationSyntax separatorToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				item.AddSeparator(separatorToken);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> ParseLetList()
		{
			SeparatedSyntaxListBuilder<ExpressionRangeVariableSyntax> item = _pool.AllocateSeparated<ExpressionRangeVariableSyntax>();
			while (true)
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseNullableModifiedIdentifier();
				if (modifiedIdentifierSyntax.ContainsDiagnostics)
				{
					SyntaxKind syntaxKind = PeekAheadFor(SyntaxKind.AsKeyword, SyntaxKind.InKeyword, SyntaxKind.CommaToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
					if (syntaxKind == SyntaxKind.AsKeyword || syntaxKind == SyntaxKind.InKeyword || syntaxKind == SyntaxKind.CommaToken)
					{
						modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind }));
					}
				}
				if (CurrentToken.Kind == SyntaxKind.QuestionToken && (PeekToken(1).Kind == SyntaxKind.InKeyword || PeekToken(1).Kind == SyntaxKind.EqualsToken))
				{
					SyntaxToken currentToken = CurrentToken;
					modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ReportSyntaxError(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported));
					GetNextToken();
				}
				SimpleAsClauseSyntax simpleAsClauseSyntax = null;
				bool flag = false;
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.InKeyword)
					{
						flag = true;
					}
					TypeSyntax typeSyntax = ParseGeneralType();
					simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(asKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
					if (typeSyntax.ContainsDiagnostics)
					{
						SyntaxKind syntaxKind2 = PeekAheadFor(SyntaxKind.InKeyword, SyntaxKind.CommaToken, SyntaxKind.EqualsToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
						if (syntaxKind2 == SyntaxKind.AsKeyword || syntaxKind2 == SyntaxKind.InKeyword || syntaxKind2 == SyntaxKind.CommaToken)
						{
							simpleAsClauseSyntax = SyntaxNodeExtensions.AddTrailingSyntax(simpleAsClauseSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind2 }));
						}
					}
				}
				PunctuationSyntax token = null;
				ExpressionSyntax expressionSyntax = null;
				if (!TryGetToken(SyntaxKind.EqualsToken, ref token))
				{
					token = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken);
					token = ReportSyntaxError(token, ERRID.ERR_ExpectedAssignmentOperator);
				}
				else
				{
					if (!flag)
					{
						TryEatNewLine();
					}
					expressionSyntax = ParseExpressionCore();
				}
				if (expressionSyntax == null || expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = expressionSyntax ?? Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression();
					SyntaxKind syntaxKind3 = PeekAheadFor(SyntaxKind.CommaToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
					if (syntaxKind3 == SyntaxKind.CommaToken)
					{
						expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(expressionSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind3 }));
					}
				}
				ExpressionRangeVariableSyntax node = SyntaxFactory.ExpressionRangeVariable(SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, simpleAsClauseSyntax, token), expressionSyntax);
				item.Add(node);
				PunctuationSyntax token2 = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
				{
					break;
				}
				item.AddSeparator(token2);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> ParseFromControlVars()
		{
			SeparatedSyntaxListBuilder<CollectionRangeVariableSyntax> item = _pool.AllocateSeparated<CollectionRangeVariableSyntax>();
			while (true)
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseNullableModifiedIdentifier();
				if (modifiedIdentifierSyntax.ContainsDiagnostics)
				{
					SyntaxKind syntaxKind = PeekAheadFor(SyntaxKind.AsKeyword, SyntaxKind.InKeyword, SyntaxKind.CommaToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
					if (syntaxKind == SyntaxKind.AsKeyword || syntaxKind == SyntaxKind.InKeyword || syntaxKind == SyntaxKind.CommaToken)
					{
						modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind }));
					}
				}
				if (CurrentToken.Kind == SyntaxKind.QuestionToken && (PeekToken(1).Kind == SyntaxKind.InKeyword || PeekToken(1).Kind == SyntaxKind.EqualsToken))
				{
					SyntaxToken currentToken = CurrentToken;
					modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ReportSyntaxError(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported));
					GetNextToken();
				}
				SimpleAsClauseSyntax simpleAsClauseSyntax = null;
				bool flag = false;
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.InKeyword)
					{
						flag = true;
					}
					TypeSyntax typeSyntax = ParseGeneralType();
					simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(asKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
					if (typeSyntax.ContainsDiagnostics)
					{
						SyntaxKind syntaxKind2 = PeekAheadFor(SyntaxKind.InKeyword, SyntaxKind.CommaToken, SyntaxKind.EqualsToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
						if (syntaxKind2 == SyntaxKind.AsKeyword || syntaxKind2 == SyntaxKind.InKeyword || syntaxKind2 == SyntaxKind.CommaToken)
						{
							simpleAsClauseSyntax = SyntaxNodeExtensions.AddTrailingSyntax(simpleAsClauseSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind2 }));
						}
					}
				}
				KeywordSyntax token = null;
				ExpressionSyntax expressionSyntax = null;
				if (TryEatNewLineAndGetToken(SyntaxKind.InKeyword, ref token, createIfMissing: true))
				{
					if (!flag)
					{
						TryEatNewLine();
					}
					expressionSyntax = ParseExpressionCore();
				}
				if (expressionSyntax == null || expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = expressionSyntax ?? Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression();
					SyntaxKind syntaxKind3 = PeekAheadFor(SyntaxKind.CommaToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
					if (syntaxKind3 == SyntaxKind.CommaToken)
					{
						expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(expressionSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind3 }));
					}
				}
				CollectionRangeVariableSyntax node = SyntaxFactory.CollectionRangeVariable(modifiedIdentifierSyntax, simpleAsClauseSyntax, token, expressionSyntax);
				item.Add(node);
				PunctuationSyntax token2 = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
				{
					break;
				}
				item.AddSeparator(token2);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private ExpressionSyntax ParsePotentialQuery(KeywordSyntax contextualKeyword)
		{
			bool flag = false;
			int num = 1;
			SyntaxToken syntaxToken = PeekToken(num);
			if (syntaxToken != null && syntaxToken.IsEndOfLine && !NextLineStartsWithStatementTerminator(1))
			{
				num++;
				syntaxToken = PeekToken(num);
				flag = true;
			}
			if (syntaxToken == null || (syntaxToken.Kind != SyntaxKind.IdentifierToken && !syntaxToken.IsKeyword))
			{
				return null;
			}
			KeywordSyntax k = null;
			if (flag || syntaxToken.IsKeyword || (syntaxToken.Kind == SyntaxKind.IdentifierToken && TryTokenAsContextualKeyword(syntaxToken, ref k)))
			{
				num++;
				syntaxToken = PeekToken(num);
				if (syntaxToken != null)
				{
					if (syntaxToken.Kind == SyntaxKind.StatementTerminatorToken)
					{
						syntaxToken = PeekToken(num + 1);
						if (syntaxToken == null || syntaxToken.Kind != SyntaxKind.InKeyword)
						{
							return null;
						}
					}
					else if (syntaxToken.Kind == SyntaxKind.QuestionToken)
					{
						num++;
						syntaxToken = PeekToken(num);
					}
				}
				if (syntaxToken == null)
				{
					return null;
				}
				if (syntaxToken.Kind != SyntaxKind.InKeyword && syntaxToken.Kind != SyntaxKind.AsKeyword && (flag || syntaxToken.Kind != SyntaxKind.EqualsToken))
				{
					return null;
				}
			}
			if (contextualKeyword.Kind == SyntaxKind.FromKeyword)
			{
				GetNextToken();
				return ParseFromQueryExpression(contextualKeyword);
			}
			GetNextToken();
			return ParseAggregateQueryExpression(contextualKeyword);
		}

		private GroupByClauseSyntax ParseGroupByExpression(KeywordSyntax groupKw)
		{
			KeywordSyntax keyword = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>);
			if (!TryEatNewLineAndGetContextualKeyword(SyntaxKind.ByKeyword, ref keyword))
			{
				TryEatNewLine();
				separatedSyntaxList = ParseSelectList();
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList2 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>);
			if (keyword != null || TryEatNewLineAndGetContextualKeyword(SyntaxKind.ByKeyword, ref keyword, createIfMissing: true))
			{
				TryEatNewLine();
				separatedSyntaxList2 = ParseSelectList();
			}
			else
			{
				SeparatedSyntaxListBuilder<ExpressionRangeVariableSyntax> item = _pool.AllocateSeparated<ExpressionRangeVariableSyntax>();
				item.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ExpressionRangeVariable(null, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()));
				separatedSyntaxList2 = item.ToList();
				_pool.Free(in item);
			}
			KeywordSyntax keyword2 = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList3 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>);
			if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, ref keyword2, createIfMissing: true))
			{
				TryEatNewLine();
				separatedSyntaxList3 = ParseAggregateList(AllowGroupName: true, IsGroupJoinProjection: false);
			}
			else
			{
				separatedSyntaxList3 = MissingAggregationRangeVariables();
			}
			return SyntaxFactory.GroupByClause(groupKw, separatedSyntaxList, keyword, separatedSyntaxList2, keyword2, separatedSyntaxList3);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> MissingAggregationRangeVariables()
		{
			SeparatedSyntaxListBuilder<AggregationRangeVariableSyntax> item = _pool.AllocateSeparated<AggregationRangeVariableSyntax>();
			item.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.AggregationRangeVariable(null, SyntaxFactory.FunctionAggregation(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), null, null, null)));
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private JoinClauseSyntax ParseInnerJoinOrGroupJoinExpression(KeywordSyntax groupKw, KeywordSyntax joinKw)
		{
			TryEatNewLine();
			CollectionRangeVariableSyntax collectionRangeVariableSyntax = ParseJoinControlVar();
			SyntaxListBuilder<JoinClauseSyntax> syntaxListBuilder = _pool.Allocate<JoinClauseSyntax>();
			while (true)
			{
				JoinClauseSyntax joinClauseSyntax = ParseOptionalJoinOperator();
				if (joinClauseSyntax == null)
				{
					break;
				}
				syntaxListBuilder.Add(joinClauseSyntax);
			}
			KeywordSyntax token = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax>);
			if (TryEatNewLineAndGetToken(SyntaxKind.OnKeyword, ref token, createIfMissing: true))
			{
				TryEatNewLine();
				separatedSyntaxList = ParseJoinPredicateExpression();
			}
			else
			{
				JoinConditionSyntax joinConditionSyntax = SyntaxFactory.JoinCondition(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EqualsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression());
				separatedSyntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax>(joinConditionSyntax);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList2 = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax>(collectionRangeVariableSyntax);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinClauseSyntax> syntaxList = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			if (groupKw == null)
			{
				return SyntaxFactory.SimpleJoinClause(joinKw, separatedSyntaxList2, syntaxList, token, separatedSyntaxList);
			}
			KeywordSyntax keyword = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList3 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>);
			if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, ref keyword, createIfMissing: true))
			{
				TryEatNewLine();
				separatedSyntaxList3 = ParseAggregateList(AllowGroupName: true, IsGroupJoinProjection: true);
			}
			else
			{
				separatedSyntaxList3 = MissingAggregationRangeVariables();
			}
			return SyntaxFactory.GroupJoinClause(groupKw, joinKw, separatedSyntaxList2, syntaxList, token, separatedSyntaxList, keyword, separatedSyntaxList3);
		}

		private JoinClauseSyntax ParseOptionalJoinOperator()
		{
			KeywordSyntax keyword = null;
			if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.JoinKeyword, ref keyword))
			{
				return ParseInnerJoinOrGroupJoinExpression(null, keyword);
			}
			KeywordSyntax keyword2 = null;
			if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.GroupKeyword, ref keyword2))
			{
				TryGetContextualKeyword(SyntaxKind.JoinKeyword, ref keyword, createIfMissing: true);
				return ParseInnerJoinOrGroupJoinExpression(keyword2, keyword);
			}
			return null;
		}

		private CollectionRangeVariableSyntax ParseJoinControlVar()
		{
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseNullableModifiedIdentifier();
			if (modifiedIdentifierSyntax.ContainsDiagnostics)
			{
				SyntaxKind syntaxKind = PeekAheadFor(SyntaxKind.AsKeyword, SyntaxKind.InKeyword, SyntaxKind.OnKeyword, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				switch (syntaxKind)
				{
				case SyntaxKind.AsKeyword:
				case SyntaxKind.InKeyword:
				case SyntaxKind.OnKeyword:
				case SyntaxKind.GroupKeyword:
				case SyntaxKind.JoinKeyword:
					modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind }));
					break;
				}
			}
			if (CurrentToken.Kind == SyntaxKind.QuestionToken && PeekToken(1).Kind == SyntaxKind.InKeyword)
			{
				SyntaxToken currentToken = CurrentToken;
				modifiedIdentifierSyntax = SyntaxNodeExtensions.AddTrailingSyntax(modifiedIdentifierSyntax, ReportSyntaxError(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported));
				GetNextToken();
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				TypeSyntax typeSyntax = ParseGeneralType();
				simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(asKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
				if (typeSyntax.ContainsDiagnostics)
				{
					SyntaxKind syntaxKind2 = PeekAheadFor(SyntaxKind.InKeyword, SyntaxKind.OnKeyword, SyntaxKind.EqualsToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
					switch (syntaxKind2)
					{
					case SyntaxKind.InKeyword:
					case SyntaxKind.OnKeyword:
					case SyntaxKind.GroupKeyword:
					case SyntaxKind.JoinKeyword:
					case SyntaxKind.EqualsToken:
						simpleAsClauseSyntax = SyntaxNodeExtensions.AddTrailingSyntax(simpleAsClauseSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind2 }));
						break;
					}
				}
			}
			KeywordSyntax token = null;
			ExpressionSyntax expressionSyntax = null;
			if (TryEatNewLineAndGetToken(SyntaxKind.InKeyword, ref token, createIfMissing: true))
			{
				TryEatNewLine();
				expressionSyntax = ParseExpressionCore();
			}
			if (expressionSyntax == null || expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = expressionSyntax ?? Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression();
				SyntaxKind syntaxKind3 = PeekAheadFor(SyntaxKind.CommaToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				if (syntaxKind3 == SyntaxKind.CommaToken)
				{
					expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(expressionSyntax, ResyncAt(new SyntaxKind[1] { syntaxKind3 }));
				}
			}
			return SyntaxFactory.CollectionRangeVariable(modifiedIdentifierSyntax, simpleAsClauseSyntax, token, expressionSyntax);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> ParseJoinPredicateExpression()
		{
			SeparatedSyntaxListBuilder<JoinConditionSyntax> item = _pool.AllocateSeparated<JoinConditionSyntax>();
			KeywordSyntax token = null;
			do
			{
				JoinConditionSyntax joinConditionSyntax = null;
				if (CurrentToken.Kind != SyntaxKind.StatementTerminatorToken)
				{
					ExpressionSyntax expressionSyntax = ParseExpressionCore(OperatorPrecedence.PrecedenceRelational);
					if (expressionSyntax.ContainsDiagnostics)
					{
						expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.EqualsToken, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.OnKeyword, SyntaxKind.AndKeyword, SyntaxKind.AndAlsoKeyword, SyntaxKind.OrKeyword, SyntaxKind.OrElseKeyword, SyntaxKind.SkipKeyword, SyntaxKind.SkipKeyword, SyntaxKind.LetKeyword);
					}
					KeywordSyntax keyword = null;
					ExpressionSyntax expressionSyntax2 = null;
					expressionSyntax2 = ((!TryGetContextualKeywordAndEatNewLine(SyntaxKind.EqualsKeyword, ref keyword, createIfMissing: true)) ? Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression() : ParseExpressionCore(OperatorPrecedence.PrecedenceRelational));
					joinConditionSyntax = SyntaxFactory.JoinCondition(expressionSyntax, keyword, expressionSyntax2);
				}
				else
				{
					joinConditionSyntax = SyntaxFactory.JoinCondition(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EqualsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression());
					joinConditionSyntax = ReportSyntaxError(joinConditionSyntax, ERRID.ERR_ExpectedExpression);
				}
				if (joinConditionSyntax.ContainsDiagnostics)
				{
					joinConditionSyntax = ResyncAt(joinConditionSyntax, SyntaxKind.AndKeyword, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.OnKeyword, SyntaxKind.AndAlsoKeyword, SyntaxKind.OrKeyword, SyntaxKind.OrElseKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				}
				if (item.Count > 0)
				{
					item.AddSeparator(token);
				}
				item.Add(joinConditionSyntax);
			}
			while (TryGetTokenAndEatNewLine(SyntaxKind.AndKeyword, ref token));
			if (CurrentToken.Kind == SyntaxKind.AndAlsoKeyword || CurrentToken.Kind == SyntaxKind.OrKeyword || CurrentToken.Kind == SyntaxKind.OrElseKeyword)
			{
				item[item.Count - 1] = SyntaxNodeExtensions.AddTrailingSyntax(item[item.Count - 1], (GreenNode)CurrentToken, ERRID.ERR_ExpectedAnd);
				GetNextToken();
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax> result = item.ToList();
			_pool.Free(in item);
			if (result.Node!.ContainsDiagnostics)
			{
				GreenNode node = result.Node;
				node = ResyncAt(node, SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.OnKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				result = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<JoinConditionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<JoinConditionSyntax>(node));
			}
			return result;
		}

		private FromClauseSyntax ParseFromOperator(KeywordSyntax FromKw)
		{
			TryEatNewLine();
			return SyntaxFactory.FromClause(FromKw, ParseFromControlVars());
		}

		private LetClauseSyntax ParseLetOperator(KeywordSyntax LetKw)
		{
			TryEatNewLine();
			return SyntaxFactory.LetClause(LetKw, ParseLetList());
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> ParseOrderByList()
		{
			SeparatedSyntaxListBuilder<OrderingSyntax> item = _pool.AllocateSeparated<OrderingSyntax>();
			while (true)
			{
				ExpressionSyntax expressionSyntax = ParseExpressionCore();
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken, SyntaxKind.AscendingKeyword, SyntaxKind.DescendingKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.FromKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				}
				OrderingSyntax orderingSyntax = null;
				KeywordSyntax keyword = null;
				if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.DescendingKeyword, ref keyword))
				{
					orderingSyntax = SyntaxFactory.DescendingOrdering(expressionSyntax, keyword);
				}
				else
				{
					TryEatNewLineAndGetContextualKeyword(SyntaxKind.AscendingKeyword, ref keyword);
					orderingSyntax = SyntaxFactory.AscendingOrdering(expressionSyntax, keyword);
				}
				item.Add(orderingSyntax);
				PunctuationSyntax token = null;
				if (!TryEatNewLineAndGetToken(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				TryEatNewLine();
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private void ParseMoreQueryOperators(ref SyntaxListBuilder<QueryClauseSyntax> operators)
		{
			while (true)
			{
				if (operators.Count > 0 && operators[operators.Count - 1]!.ContainsDiagnostics)
				{
					operators[operators.Count - 1] = ResyncAt(operators[operators.Count - 1], SyntaxKind.FromKeyword, SyntaxKind.WhereKeyword, SyntaxKind.GroupKeyword, SyntaxKind.SelectKeyword, SyntaxKind.OrderKeyword, SyntaxKind.JoinKeyword, SyntaxKind.DistinctKeyword, SyntaxKind.AggregateKeyword, SyntaxKind.IntoKeyword, SyntaxKind.SkipKeyword, SyntaxKind.TakeKeyword, SyntaxKind.LetKeyword);
				}
				QueryClauseSyntax queryClauseSyntax = ParseNextQueryOperator();
				if (queryClauseSyntax != null)
				{
					operators.Add(queryClauseSyntax);
					continue;
				}
				break;
			}
		}

		private QueryClauseSyntax ParseNextQueryOperator()
		{
			SyntaxToken currentToken = CurrentToken;
			if (currentToken.Kind == SyntaxKind.StatementTerminatorToken)
			{
				if (NextLineStartsWithStatementTerminator() || !IsContinuableQueryOperator(PeekToken(1)))
				{
					return null;
				}
				TryEatNewLine();
				currentToken = CurrentToken;
			}
			switch (currentToken.Kind)
			{
			case SyntaxKind.SelectKeyword:
			{
				GetNextToken();
				TryEatNewLine();
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> separatedSyntaxList2 = ParseSelectList();
				return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SelectClause((KeywordSyntax)currentToken, separatedSyntaxList2);
			}
			case SyntaxKind.LetKeyword:
				GetNextToken();
				return ParseLetOperator((KeywordSyntax)currentToken);
			case SyntaxKind.IdentifierToken:
			{
				KeywordSyntax k = null;
				if (!TryTokenAsContextualKeyword(currentToken, ref k))
				{
					return null;
				}
				switch (k.Kind)
				{
				case SyntaxKind.WhereKeyword:
					GetNextToken();
					TryEatNewLine();
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhereClause(k, ParseExpressionCore());
				case SyntaxKind.SkipKeyword:
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.WhileKeyword)
					{
						KeywordSyntax whileKeyword2 = (KeywordSyntax)CurrentToken;
						GetNextToken();
						TryEatNewLine();
						return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SkipWhileClause(k, whileKeyword2, ParseExpressionCore());
					}
					TryEatNewLineIfNotFollowedBy(SyntaxKind.WhileKeyword);
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SkipClause(k, ParseExpressionCore());
				case SyntaxKind.TakeKeyword:
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.WhileKeyword)
					{
						KeywordSyntax whileKeyword = (KeywordSyntax)CurrentToken;
						GetNextToken();
						TryEatNewLine();
						return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TakeWhileClause(k, whileKeyword, ParseExpressionCore());
					}
					TryEatNewLineIfNotFollowedBy(SyntaxKind.WhileKeyword);
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TakeClause(k, ParseExpressionCore());
				case SyntaxKind.GroupKeyword:
				{
					GetNextToken();
					KeywordSyntax keyword2 = null;
					if (TryGetContextualKeyword(SyntaxKind.JoinKeyword, ref keyword2))
					{
						return ParseInnerJoinOrGroupJoinExpression(k, keyword2);
					}
					return ParseGroupByExpression(k);
				}
				case SyntaxKind.AggregateKeyword:
					GetNextToken();
					return ParseAggregateClause(k);
				case SyntaxKind.OrderKeyword:
				{
					GetNextToken();
					KeywordSyntax keyword = null;
					TryGetContextualKeywordAndEatNewLine(SyntaxKind.ByKeyword, ref keyword, createIfMissing: true);
					TryEatNewLine();
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> separatedSyntaxList = ParseOrderByList();
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OrderByClause(k, keyword, separatedSyntaxList);
				}
				case SyntaxKind.DistinctKeyword:
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken)
					{
						SyntaxToken syntaxToken = PeekToken(1);
						switch (syntaxToken.Kind)
						{
						case SyntaxKind.ExclamationToken:
						case SyntaxKind.OpenParenToken:
						case SyntaxKind.DotToken:
						case SyntaxKind.QuestionToken:
							TryEatNewLine();
							break;
						default:
							if (syntaxToken.IsBinaryOperator())
							{
								TryEatNewLine();
							}
							break;
						}
					}
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DistinctClause(k);
				case SyntaxKind.JoinKeyword:
					GetNextToken();
					return ParseInnerJoinOrGroupJoinExpression(null, k);
				case SyntaxKind.FromKeyword:
					GetNextToken();
					return ParseFromOperator(k);
				}
				break;
			}
			}
			return null;
		}

		private QueryExpressionSyntax ParseFromQueryExpression(KeywordSyntax fromKw)
		{
			SyntaxListBuilder<QueryClauseSyntax> operators = _pool.Allocate<QueryClauseSyntax>();
			operators.Add(ParseFromOperator(fromKw));
			ParseMoreQueryOperators(ref operators);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> syntaxList = operators.ToList();
			_pool.Free(operators);
			return SyntaxFactory.QueryExpression(syntaxList);
		}

		private QueryExpressionSyntax ParseAggregateQueryExpression(KeywordSyntax AggregateKw)
		{
			SyntaxListBuilder<QueryClauseSyntax> syntaxListBuilder = _pool.Allocate<QueryClauseSyntax>();
			syntaxListBuilder.Add(ParseAggregateClause(AggregateKw));
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> syntaxList = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return SyntaxFactory.QueryExpression(syntaxList);
		}

		private AggregateClauseSyntax ParseAggregateClause(KeywordSyntax AggregateKw)
		{
			TryEatNewLine();
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> separatedSyntaxList = ParseFromControlVars();
			SyntaxListBuilder<QueryClauseSyntax> operators = _pool.Allocate<QueryClauseSyntax>();
			ParseMoreQueryOperators(ref operators);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> syntaxList = operators.ToList();
			_pool.Free(operators);
			KeywordSyntax keyword = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> separatedSyntaxList2 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>);
			if (TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, ref keyword, createIfMissing: true))
			{
				TryEatNewLine();
				separatedSyntaxList2 = ParseAggregateList(AllowGroupName: false, IsGroupJoinProjection: false);
			}
			else
			{
				separatedSyntaxList2 = MissingAggregationRangeVariables();
			}
			return SyntaxFactory.AggregateClause(AggregateKw, separatedSyntaxList, syntaxList, keyword, separatedSyntaxList2);
		}

		private bool IsContinuableQueryOperator(SyntaxToken pToken)
		{
			SyntaxKind kind = SyntaxKind.None;
			if (!TryTokenAsKeyword(pToken, ref kind))
			{
				return false;
			}
			bool flag = KeywordTable.IsQueryClause(kind);
			if (flag && kind == SyntaxKind.SelectKeyword && PeekToken(2).Kind == SyntaxKind.CaseKeyword)
			{
				flag = false;
			}
			return flag;
		}

		internal Parser(SourceText text, VisualBasicParseOptions options, CancellationToken cancellationToken = default(CancellationToken))
			: this(new Scanner(text, options))
		{
			_disposeScanner = true;
			_cancellationToken = cancellationToken;
		}

		internal Parser(Scanner scanner)
		{
			_allowLeadingMultilineTrivia = true;
			_hadImplicitLineContinuation = false;
			_hadLineContinuationComment = false;
			_possibleFirstStatementOnLine = PossibleFirstStatementKind.Yes;
			_pool = new SyntaxListPool();
			_context = null;
			_scanner = scanner;
			_context = new CompilationUnitContext(this);
			_syntaxFactory = new ContextAwareSyntaxFactory(this);
		}

		internal void Dispose()
		{
			if (_disposeScanner)
			{
				_scanner.Dispose();
			}
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in Dispose
			this.Dispose();
		}

		private SimpleNameSyntax ParseSimpleName(bool allowGenericArguments, bool allowGenericsWithoutOf, bool disallowGenericArgumentsOnLastQualifiedName, bool nonArrayName, bool allowKeyword, ref bool allowEmptyGenericArguments, ref bool allowNonEmptyGenericArguments)
		{
			IdentifierTokenSyntax identifierTokenSyntax = (allowKeyword ? ParseIdentifierAllowingKeyword() : ParseIdentifier());
			TypeArgumentListSyntax typeArgumentListSyntax = null;
			if (allowGenericArguments && BeginsGeneric(nonArrayName, allowGenericsWithoutOf))
			{
				typeArgumentListSyntax = ParseGenericArguments(ref allowEmptyGenericArguments, ref allowNonEmptyGenericArguments);
			}
			if (typeArgumentListSyntax == null)
			{
				return SyntaxFactory.IdentifierName(identifierTokenSyntax);
			}
			if (disallowGenericArgumentsOnLastQualifiedName && CurrentToken.Kind != SyntaxKind.DotToken && !typeArgumentListSyntax.ContainsDiagnostics)
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, typeArgumentListSyntax, ERRID.ERR_TypeArgsUnexpected);
				return SyntaxFactory.IdentifierName(identifierTokenSyntax);
			}
			return SyntaxFactory.GenericName(identifierTokenSyntax, typeArgumentListSyntax);
		}

		internal NameSyntax ParseName(bool requireQualification, bool allowGlobalNameSpace, bool allowGenericArguments, bool allowGenericsWithoutOf, bool nonArrayName = false, bool disallowGenericArgumentsOnLastQualifiedName = false, bool allowEmptyGenericArguments = false, ref bool allowedEmptyGenericArguments = false, bool isNameInNamespaceDeclaration = false)
		{
			bool allowNonEmptyGenericArguments = true;
			NameSyntax nameSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.GlobalKeyword)
			{
				nameSyntax = SyntaxFactory.GlobalName((KeywordSyntax)CurrentToken);
				if (isNameInNamespaceDeclaration)
				{
					nameSyntax = CheckFeatureAvailability(Feature.GlobalNamespace, nameSyntax);
				}
				GetNextToken();
				if (!allowGlobalNameSpace)
				{
					nameSyntax = ReportSyntaxError(nameSyntax, ERRID.ERR_NoGlobalExpectedIdentifier);
				}
			}
			else
			{
				nameSyntax = ParseSimpleName(allowGenericArguments, allowGenericsWithoutOf, disallowGenericArgumentsOnLastQualifiedName, nonArrayName, allowKeyword: false, ref allowEmptyGenericArguments, ref allowNonEmptyGenericArguments);
			}
			PunctuationSyntax token = null;
			while (TryGetTokenAndEatNewLine(SyntaxKind.DotToken, ref token))
			{
				nameSyntax = SyntaxFactory.QualifiedName(nameSyntax, token, ParseSimpleName(allowGenericArguments, allowGenericsWithoutOf, disallowGenericArgumentsOnLastQualifiedName, nonArrayName, allowKeyword: true, ref allowEmptyGenericArguments, ref allowNonEmptyGenericArguments));
			}
			if (requireQualification && token == null)
			{
				nameSyntax = SyntaxFactory.QualifiedName(nameSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DotToken), SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()));
				nameSyntax = ReportSyntaxError(nameSyntax, ERRID.ERR_ExpectedDot);
			}
			allowedEmptyGenericArguments = !allowNonEmptyGenericArguments;
			return nameSyntax;
		}

		private static Microsoft.CodeAnalysis.SyntaxToken GetLastNZWToken(SyntaxNode node)
		{
			SyntaxNodeOrToken current;
			do
			{
				ChildSyntaxList.Reversed.Enumerator enumerator = node.ChildNodesAndTokens().Reverse().GetEnumerator();
				do
				{
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						continue;
					}
					throw ExceptionUtilities.Unreachable;
				}
				while (current.FullWidth == 0);
				node = current.AsNode();
			}
			while (node != null);
			return current.AsToken();
		}

		private static Microsoft.CodeAnalysis.SyntaxToken GetLastToken(SyntaxNode node)
		{
			SyntaxNodeOrToken syntaxNodeOrToken;
			do
			{
				syntaxNodeOrToken = node.ChildNodesAndTokens().Last();
				node = syntaxNodeOrToken.AsNode();
			}
			while (node != null);
			return syntaxNodeOrToken.AsToken();
		}

		private static T AdjustTriviaForMissingTokens<T>(T node) where T : VisualBasicSyntaxNode
		{
			if (!node.ContainsDiagnostics)
			{
				return node;
			}
			if (node.GetLastTerminal()!.FullWidth != 0)
			{
				return node;
			}
			return AdjustTriviaForMissingTokensCore(node);
		}

		private static T AdjustTriviaForMissingTokensCore<T>(T node) where T : VisualBasicSyntaxNode
		{
			_Closure_0024__111_002D0<T> arg = default(_Closure_0024__111_002D0<T>);
			_Closure_0024__111_002D0<T> CS_0024_003C_003E8__locals0 = new _Closure_0024__111_002D0<T>(arg);
			SyntaxNode syntaxNode = node.CreateRed(null, 0);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_lastNonZeroWidthToken = GetLastNZWToken(syntaxNode);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_lastZeroWidthToken = GetLastToken(syntaxNode);
			SyntaxTriviaList trailingTrivia = CS_0024_003C_003E8__locals0._0024VB_0024Local_lastNonZeroWidthToken.TrailingTrivia;
			int num = trailingTrivia.Count;
			SyntaxTriviaList.Enumerator enumerator = trailingTrivia.GetEnumerator();
			while (enumerator.MoveNext() && VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.WhitespaceTrivia)
			{
				num--;
			}
			if (num == 0)
			{
				return node;
			}
			Microsoft.CodeAnalysis.SyntaxTrivia[] array = new Microsoft.CodeAnalysis.SyntaxTrivia[trailingTrivia.Count - num - 1 + 1];
			trailingTrivia.CopyTo(0, array, 0, array.Length);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_nonZwTokenReplacement = CS_0024_003C_003E8__locals0._0024VB_0024Local_lastNonZeroWidthToken.WithTrailingTrivia(array);
			SyntaxTriviaList trailingTrivia2 = CS_0024_003C_003E8__locals0._0024VB_0024Local_lastZeroWidthToken.TrailingTrivia;
			Microsoft.CodeAnalysis.SyntaxTrivia[] array2 = new Microsoft.CodeAnalysis.SyntaxTrivia[num + trailingTrivia2.Count - 1 + 1];
			trailingTrivia.CopyTo(trailingTrivia.Count - num, array2, 0, num);
			trailingTrivia2.CopyTo(0, array2, num, trailingTrivia2.Count);
			CS_0024_003C_003E8__locals0._0024VB_0024Local_lastTokenReplacement = CS_0024_003C_003E8__locals0._0024VB_0024Local_lastZeroWidthToken.WithTrailingTrivia(array2);
			syntaxNode = syntaxNode.ReplaceTokens(new Microsoft.CodeAnalysis.SyntaxToken[2] { CS_0024_003C_003E8__locals0._0024VB_0024Local_lastNonZeroWidthToken, CS_0024_003C_003E8__locals0._0024VB_0024Local_lastZeroWidthToken }, delegate(Microsoft.CodeAnalysis.SyntaxToken oldToken, Microsoft.CodeAnalysis.SyntaxToken newToken)
			{
				if (oldToken == CS_0024_003C_003E8__locals0._0024VB_0024Local_lastNonZeroWidthToken)
				{
					return CS_0024_003C_003E8__locals0._0024VB_0024Local_nonZwTokenReplacement;
				}
				return (oldToken == CS_0024_003C_003E8__locals0._0024VB_0024Local_lastZeroWidthToken) ? CS_0024_003C_003E8__locals0._0024VB_0024Local_lastTokenReplacement : newToken;
			});
			node = (T)syntaxNode.Green;
			return node;
		}

		private static string MergeTokenText(SyntaxToken firstToken, SyntaxToken secondToken)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringWriter writer = new StringWriter(instance);
			firstToken.WriteTo(writer);
			secondToken.WriteTo(writer);
			int leadingTriviaWidth = firstToken.GetLeadingTriviaWidth();
			int trailingTriviaWidth = secondToken.GetTrailingTriviaWidth();
			int num = firstToken.FullWidth + secondToken.FullWidth;
			return instance.ToStringAndFree(leadingTriviaWidth, num - leadingTriviaWidth - trailingTriviaWidth);
		}

		private static string MergeTokenText(SyntaxToken firstToken, SyntaxToken secondToken, SyntaxToken thirdToken)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringWriter writer = new StringWriter(instance);
			firstToken.WriteTo(writer);
			secondToken.WriteTo(writer);
			thirdToken.WriteTo(writer);
			int leadingTriviaWidth = firstToken.GetLeadingTriviaWidth();
			int trailingTriviaWidth = thirdToken.GetTrailingTriviaWidth();
			int num = firstToken.FullWidth + secondToken.FullWidth + thirdToken.FullWidth;
			return instance.ToStringAndFree(leadingTriviaWidth, num - leadingTriviaWidth - trailingTriviaWidth);
		}

		private BlockContext GetCurrentSyntaxNodeIfApplicable(out VisualBasicSyntaxNode curSyntaxNode)
		{
			BlockContext newContext = _context;
			BlockContext.LinkResult linkResult;
			do
			{
				curSyntaxNode = _scanner.GetCurrentSyntaxNode();
				linkResult = ((curSyntaxNode != null) ? ((!(curSyntaxNode is DirectiveTriviaSyntax) && curSyntaxNode.Kind != SyntaxKind.DocumentationCommentTrivia) ? newContext.TryLinkSyntax(curSyntaxNode, ref newContext) : BlockContext.LinkResult.NotUsed) : BlockContext.LinkResult.NotUsed);
			}
			while (linkResult == BlockContext.LinkResult.Crumble && _scanner.TryCrumbleOnce());
			if ((linkResult & BlockContext.LinkResult.Used) == BlockContext.LinkResult.Used)
			{
				return newContext;
			}
			return null;
		}

		internal CompilationUnitSyntax ParseCompilationUnit()
		{
			return ParseWithStackGuard(ParseCompilationUnitCore, () => SyntaxFactory.CompilationUnit(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfFileToken()));
		}

		internal CompilationUnitSyntax ParseCompilationUnitCore()
		{
			CompilationUnitContext compilationUnitContext = (CompilationUnitContext)_context;
			GetNextToken();
			while (true)
			{
				VisualBasicSyntaxNode curSyntaxNode = null;
				BlockContext currentSyntaxNodeIfApplicable = GetCurrentSyntaxNodeIfApplicable(out curSyntaxNode);
				if (currentSyntaxNodeIfApplicable != null)
				{
					_context = currentSyntaxNodeIfApplicable;
					VisualBasicSyntaxNode visualBasicSyntaxNode = SyntaxExtensions.LastTriviaIfAny(curSyntaxNode);
					if (visualBasicSyntaxNode != null)
					{
						if (visualBasicSyntaxNode.Kind == SyntaxKind.EndOfLineTrivia)
						{
							ConsumedStatementTerminator(allowLeadingMultilineTrivia: true);
							ResetCurrentToken(ScannerState.VBAllowLeadingMultilineTrivia);
						}
						else if (visualBasicSyntaxNode.Kind == SyntaxKind.ColonTrivia)
						{
							ConsumedStatementTerminator(allowLeadingMultilineTrivia: true, PossibleFirstStatementKind.IfPrecededByLineBreak);
							ResetCurrentToken(_allowLeadingMultilineTrivia ? ScannerState.VBAllowLeadingMultilineTrivia : ScannerState.VB);
						}
					}
					else if (curSyntaxNode is LabelStatementSyntax labelStatementSyntax && labelStatementSyntax.ColonToken.Kind == SyntaxKind.ColonToken)
					{
						ConsumedStatementTerminator(allowLeadingMultilineTrivia: true, PossibleFirstStatementKind.IfPrecededByLineBreak);
					}
				}
				else
				{
					ResetCurrentToken(_allowLeadingMultilineTrivia ? ScannerState.VBAllowLeadingMultilineTrivia : ScannerState.VB);
					if (CurrentToken.IsEndOfParse)
					{
						break;
					}
					StatementSyntax statementSyntax = AdjustTriviaForMissingTokens(_context.Parse());
					_context = _context.LinkSyntax(statementSyntax);
					_context = _context.ResyncAndProcessStatementTerminator(statementSyntax, null);
				}
			}
			BlockContextExtensions.RecoverFromMissingEnd(_context, compilationUnitContext);
			PunctuationSyntax eof = (PunctuationSyntax)CurrentToken;
			ArrayBuilder<IfDirectiveTriviaSyntax> notClosedIfDirectives = null;
			ArrayBuilder<RegionDirectiveTriviaSyntax> notClosedRegionDirectives = null;
			bool haveRegionDirectives = false;
			ExternalSourceDirectiveTriviaSyntax notClosedExternalSourceDirective = null;
			eof = _scanner.RecoverFromMissingConditionalEnds(eof, out notClosedIfDirectives, out notClosedRegionDirectives, out haveRegionDirectives, out notClosedExternalSourceDirective);
			return compilationUnitContext.CreateCompilationUnit(eof, notClosedIfDirectives, notClosedRegionDirectives, haveRegionDirectives, notClosedExternalSourceDirective);
		}

		private TNode ParseWithStackGuard<TNode>(Func<TNode> parseFunc, Func<TNode> defaultFunc) where TNode : VisualBasicSyntaxNode
		{
			Scanner.RestorePoint restorePoint = _scanner.CreateRestorePoint();
			try
			{
				return parseFunc();
			}
			catch (InsufficientExecutionStackException ex)
			{
				ProjectData.SetProjectError(ex);
				InsufficientExecutionStackException ex2 = ex;
				TNode result = CreateForInsufficientStack(ref restorePoint, defaultFunc());
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private TNode CreateForInsufficientStack<TNode>(ref Scanner.RestorePoint restorePoint, TNode result) where TNode : VisualBasicSyntaxNode
		{
			restorePoint.Restore();
			GetNextToken();
			SyntaxListBuilder syntaxListBuilder = new SyntaxListBuilder(4);
			while (CurrentToken.Kind != SyntaxKind.EndOfFileToken)
			{
				syntaxListBuilder.Add(CurrentToken);
				GetNextToken();
			}
			return SyntaxNodeExtensions.AddLeadingSyntax(result, syntaxListBuilder.ToList<SyntaxToken>(), ERRID.ERR_TooLongOrComplexExpression);
		}

		internal StatementSyntax ParseExecutableStatement()
		{
			return ParseWithStackGuard(ParseExecutableStatementCore, () => Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement());
		}

		private StatementSyntax ParseExecutableStatementCore()
		{
			CompilationUnitContext compilationUnitContext = new CompilationUnitContext(this);
			MethodStatementSyntax statement = SyntaxFactory.SubStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), null, null, null, null, null);
			MethodBlockContext methodBlockContext = new MethodBlockContext(SyntaxKind.SubBlock, statement, compilationUnitContext);
			GetNextToken();
			_context = methodBlockContext;
			do
			{
				StatementSyntax statementSyntax = _context.Parse();
				_context = _context.LinkSyntax(statementSyntax);
				_context = _context.ResyncAndProcessStatementTerminator(statementSyntax, null);
			}
			while (_context.Level > methodBlockContext.Level && !CurrentToken.IsEndOfParse);
			BlockContextExtensions.RecoverFromMissingEnd(_context, methodBlockContext);
			if (methodBlockContext.Statements.Count > 0)
			{
				return methodBlockContext.Statements[0];
			}
			MethodBlockBaseSyntax methodBlockBaseSyntax = (MethodBlockBaseSyntax)compilationUnitContext.Statements[0];
			if (methodBlockBaseSyntax.Statements.Any())
			{
				return methodBlockBaseSyntax.Statements[0];
			}
			return ReportSyntaxError(methodBlockBaseSyntax.End, ERRID.ERR_InvInsideEndsProc);
		}

		private SyntaxToken ParseBinaryOperator()
		{
			SyntaxToken syntaxToken = CurrentToken;
			SyntaxToken syntaxToken2 = null;
			if (CurrentToken.Kind == SyntaxKind.GreaterThanToken && PeekToken(1).Kind == SyntaxKind.LessThanToken)
			{
				syntaxToken2 = PeekToken(1);
				syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.LessThanGreaterThanToken);
			}
			else if (CurrentToken.Kind == SyntaxKind.EqualsToken)
			{
				if (PeekToken(1).Kind == SyntaxKind.GreaterThanToken)
				{
					syntaxToken2 = PeekToken(1);
					syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanEqualsToken);
				}
				else if (PeekToken(1).Kind == SyntaxKind.LessThanToken)
				{
					syntaxToken2 = PeekToken(1);
					syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.LessThanEqualsToken);
				}
			}
			if (syntaxToken2 != null)
			{
				syntaxToken = SyntaxNodeExtensions.AddLeadingSyntax(syntaxToken, SyntaxList.List(CurrentToken, syntaxToken2), ERRID.ERR_ExpectedRelational);
				GetNextToken();
			}
			GetNextToken();
			TryEatNewLine();
			return syntaxToken;
		}

		internal StatementSyntax ParseDeclarationStatement()
		{
			bool hadImplicitLineContinuation = _hadImplicitLineContinuation;
			bool hadLineContinuationComment = _hadLineContinuationComment;
			try
			{
				_hadImplicitLineContinuation = false;
				_hadLineContinuationComment = false;
				StatementSyntax statementSyntax = ParseDeclarationStatementInternal();
				if (_hadImplicitLineContinuation)
				{
					StatementSyntax statementSyntax2 = statementSyntax;
					statementSyntax = CheckFeatureAvailability(Feature.LineContinuation, statementSyntax);
					if (statementSyntax2 == statementSyntax && _hadLineContinuationComment)
					{
						statementSyntax = CheckFeatureAvailability(Feature.LineContinuationComments, statementSyntax);
					}
				}
				return statementSyntax;
			}
			finally
			{
				_hadImplicitLineContinuation = hadImplicitLineContinuation;
				_hadLineContinuationComment = hadLineContinuationComment;
			}
		}

		internal StatementSyntax ParseDeclarationStatementInternal()
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.LessThanToken:
			{
				SyntaxToken t = PeekToken(1);
				if (IsContinuableEOL(1))
				{
					t = PeekToken(2);
				}
				SyntaxKind kind2 = SyntaxKind.None;
				if (TryTokenAsKeyword(t, ref kind2) && (kind2 == SyntaxKind.AssemblyKeyword || kind2 == SyntaxKind.ModuleKeyword))
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = ParseAttributeLists(allowFileLevelAttributes: true);
					return SyntaxFactory.AttributesStatement(syntaxList);
				}
				return ParseSpecifierDeclaration();
			}
			case SyntaxKind.LessThanGreaterThanToken:
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = ParseEmptyAttributeLists();
				return ParseSpecifierDeclaration(attributes);
			}
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.DefaultKeyword:
			case SyntaxKind.DimKeyword:
			case SyntaxKind.FriendKeyword:
			case SyntaxKind.MustInheritKeyword:
			case SyntaxKind.MustOverrideKeyword:
			case SyntaxKind.NarrowingKeyword:
			case SyntaxKind.NotInheritableKeyword:
			case SyntaxKind.NotOverridableKeyword:
			case SyntaxKind.OverloadsKeyword:
			case SyntaxKind.OverridableKeyword:
			case SyntaxKind.OverridesKeyword:
			case SyntaxKind.PartialKeyword:
			case SyntaxKind.PrivateKeyword:
			case SyntaxKind.ProtectedKeyword:
			case SyntaxKind.PublicKeyword:
			case SyntaxKind.ReadOnlyKeyword:
			case SyntaxKind.ShadowsKeyword:
			case SyntaxKind.SharedKeyword:
			case SyntaxKind.StaticKeyword:
			case SyntaxKind.WideningKeyword:
			case SyntaxKind.WithEventsKeyword:
			case SyntaxKind.WriteOnlyKeyword:
				return ParseSpecifierDeclaration();
			case SyntaxKind.EnumKeyword:
				return ParseEnumStatement();
			case SyntaxKind.ImplementsKeyword:
			case SyntaxKind.InheritsKeyword:
				return ParseInheritsImplementsStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.ImportsKeyword:
				return ParseImportsStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.NamespaceKeyword:
				return ParseNamespaceStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.StructureKeyword:
				return ParseTypeStatement();
			case SyntaxKind.DeclareKeyword:
				return ParseProcDeclareStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.EventKeyword:
				return ParseEventDefinition(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.DelegateKeyword:
				return ParseDelegateStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.SubKeyword:
				return ParseSubStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.FunctionKeyword:
				return ParseFunctionStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.OperatorKeyword:
				return ParseOperatorStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.PropertyKeyword:
				return ParsePropertyDefinition(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.EmptyToken:
				return ParseEmptyStatement();
			case SyntaxKind.ColonToken:
			case SyntaxKind.StatementTerminatorToken:
				return ParseStatementInMethodBodyInternal();
			case SyntaxKind.IntegerLiteralToken:
				if (IsFirstStatementOnLine(CurrentToken))
				{
					return ParseLabel();
				}
				return ReportUnrecognizedStatementError(ERRID.ERR_Syntax);
			case SyntaxKind.IdentifierToken:
			{
				if (Context.BlockKind == SyntaxKind.EnumBlock)
				{
					return ParseEnumMemberOrLabel(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>));
				}
				SyntaxKind kind = SyntaxKind.None;
				if (TryIdentifierAsContextualKeyword(CurrentToken, ref kind))
				{
					switch (kind)
					{
					case SyntaxKind.CustomKeyword:
						return ParseCustomEventDefinition(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
					case SyntaxKind.TypeKeyword:
						return ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteStructureNotType);
					case SyntaxKind.AsyncKeyword:
					case SyntaxKind.IteratorKeyword:
						return ParseSpecifierDeclaration();
					}
				}
				StatementSyntax statementSyntax2 = ParsePossibleDeclarationStatement();
				if (statementSyntax2 != null)
				{
					return statementSyntax2;
				}
				if (Context.BlockKind == SyntaxKind.CompilationUnit)
				{
					return ParseStatementInMethodBodyInternal();
				}
				if (ShouldParseAsLabel())
				{
					return ParseLabel();
				}
				return ReportUnrecognizedStatementError(ERRID.ERR_ExpectedDeclaration);
			}
			case SyntaxKind.EndKeyword:
				return ParseGroupEndStatement();
			case SyntaxKind.OptionKeyword:
				return ParseOptionStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.AddHandlerKeyword:
				return ParsePropertyOrEventAccessor(SyntaxKind.AddHandlerAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.RemoveHandlerKeyword:
				return ParsePropertyOrEventAccessor(SyntaxKind.RemoveHandlerAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.RaiseEventKeyword:
				return ParsePropertyOrEventAccessor(SyntaxKind.RaiseEventAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.GetKeyword:
				return ParsePropertyOrEventAccessor(SyntaxKind.GetAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.SetKeyword:
				return ParsePropertyOrEventAccessor(SyntaxKind.SetAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			case SyntaxKind.GlobalKeyword:
			{
				StatementSyntax statementSyntax = ParsePossibleDeclarationStatement();
				if (statementSyntax != null)
				{
					return statementSyntax;
				}
				return ParseStatementInMethodBodyInternal();
			}
			default:
				return ParseStatementInMethodBodyInternal();
			}
		}

		private StatementSyntax ParsePossibleDeclarationStatement()
		{
			SyntaxKind kind = PeekToken(1).Kind;
			if (SyntaxFacts.CanStartSpecifierDeclaration(kind) || SyntaxFacts.IsSpecifier(kind))
			{
				SyntaxToken currentToken = CurrentToken;
				GetNextToken();
				return SyntaxNodeExtensions.AddLeadingSyntax(ParseSpecifierDeclaration(), currentToken, ERRID.ERR_ExpectedDeclaration);
			}
			return null;
		}

		internal StatementSyntax ParseStatementInMethodBody()
		{
			bool hadImplicitLineContinuation = _hadImplicitLineContinuation;
			bool hadLineContinuationComment = _hadLineContinuationComment;
			try
			{
				_hadImplicitLineContinuation = false;
				_hadLineContinuationComment = false;
				StatementSyntax statementSyntax = ParseStatementInMethodBodyInternal();
				if (_hadImplicitLineContinuation)
				{
					StatementSyntax statementSyntax2 = statementSyntax;
					statementSyntax = CheckFeatureAvailability(Feature.LineContinuation, statementSyntax);
					if (statementSyntax2 == statementSyntax && _hadLineContinuationComment)
					{
						statementSyntax = CheckFeatureAvailability(Feature.LineContinuationComments, statementSyntax);
					}
				}
				return statementSyntax;
			}
			finally
			{
				_hadImplicitLineContinuation = hadImplicitLineContinuation;
				_hadLineContinuationComment = hadLineContinuationComment;
			}
		}

		internal StatementSyntax ParseStatementInMethodBodyInternal()
		{
			try
			{
				_recursionDepth++;
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				return ParseStatementInMethodBodyCore();
			}
			finally
			{
				_recursionDepth--;
			}
		}

		private StatementSyntax ParseStatementInMethodBodyCore()
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.GoToKeyword:
				return ParseGotoStatement();
			case SyntaxKind.CaseKeyword:
				return ParseCaseStatement();
			case SyntaxKind.SelectKeyword:
				return ParseSelectStatement();
			case SyntaxKind.WhileKeyword:
			case SyntaxKind.WithKeyword:
				return ParseExpressionBlockStatement();
			case SyntaxKind.UsingKeyword:
				return ParseUsingStatement();
			case SyntaxKind.SyncLockKeyword:
				return ParseExpressionBlockStatement();
			case SyntaxKind.TryKeyword:
				return ParseTry();
			case SyntaxKind.CatchKeyword:
				return ParseCatch();
			case SyntaxKind.FinallyKeyword:
				return ParseFinally();
			case SyntaxKind.IfKeyword:
				return ParseIfStatement();
			case SyntaxKind.ElseKeyword:
				if (PeekToken(1).Kind == SyntaxKind.IfKeyword)
				{
					return ParseElseIfStatement();
				}
				return ParseElseStatement();
			case SyntaxKind.ElseIfKeyword:
				return ParseElseIfStatement();
			case SyntaxKind.DoKeyword:
				return ParseDoStatement();
			case SyntaxKind.LoopKeyword:
				return ParseLoopStatement();
			case SyntaxKind.ForKeyword:
				return ParseForStatement();
			case SyntaxKind.NextKeyword:
				return ParseNextStatement();
			case SyntaxKind.EndIfKeyword:
			case SyntaxKind.WendKeyword:
				return ParseAnachronisticStatement();
			case SyntaxKind.EndKeyword:
				return ParseEndStatement();
			case SyntaxKind.ReturnKeyword:
				return ParseReturnStatement();
			case SyntaxKind.StopKeyword:
				return ParseStopOrEndStatement();
			case SyntaxKind.ContinueKeyword:
				return ParseContinueStatement();
			case SyntaxKind.ExitKeyword:
				return ParseExitStatement();
			case SyntaxKind.OnKeyword:
				return ParseOnErrorStatement();
			case SyntaxKind.ResumeKeyword:
				return ParseResumeStatement();
			case SyntaxKind.CallKeyword:
				return ParseCallStatement();
			case SyntaxKind.RaiseEventKeyword:
				return ParseRaiseEventStatement();
			case SyntaxKind.ReDimKeyword:
				return ParseRedimStatement();
			case SyntaxKind.AddHandlerKeyword:
			case SyntaxKind.RemoveHandlerKeyword:
				return ParseHandlerStatement();
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.DefaultKeyword:
			case SyntaxKind.DimKeyword:
			case SyntaxKind.FriendKeyword:
			case SyntaxKind.MustInheritKeyword:
			case SyntaxKind.MustOverrideKeyword:
			case SyntaxKind.NarrowingKeyword:
			case SyntaxKind.NotOverridableKeyword:
			case SyntaxKind.OverloadsKeyword:
			case SyntaxKind.OverridableKeyword:
			case SyntaxKind.OverridesKeyword:
			case SyntaxKind.PartialKeyword:
			case SyntaxKind.PrivateKeyword:
			case SyntaxKind.ProtectedKeyword:
			case SyntaxKind.PublicKeyword:
			case SyntaxKind.ReadOnlyKeyword:
			case SyntaxKind.ShadowsKeyword:
			case SyntaxKind.SharedKeyword:
			case SyntaxKind.StaticKeyword:
			case SyntaxKind.WideningKeyword:
			case SyntaxKind.WithEventsKeyword:
			case SyntaxKind.WriteOnlyKeyword:
			case SyntaxKind.LessThanToken:
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
				if (CurrentToken.Kind == SyntaxKind.LessThanToken)
				{
					attributes = ParseAttributeLists(allowFileLevelAttributes: false);
				}
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> syntaxList = ParseSpecifiers();
				if (!ParserExtensions.Any(syntaxList, SyntaxKind.DimKeyword, SyntaxKind.ConstKeyword))
				{
					switch (CurrentToken.Kind)
					{
					case SyntaxKind.ClassKeyword:
					case SyntaxKind.EnumKeyword:
					case SyntaxKind.EventKeyword:
					case SyntaxKind.FunctionKeyword:
					case SyntaxKind.InterfaceKeyword:
					case SyntaxKind.OperatorKeyword:
					case SyntaxKind.PropertyKeyword:
					case SyntaxKind.StructureKeyword:
					case SyntaxKind.SubKeyword:
						return ParseSpecifierDeclaration(attributes, syntaxList);
					case SyntaxKind.IdentifierToken:
					{
						SyntaxKind kind2 = SyntaxKind.None;
						if (TryIdentifierAsContextualKeyword(CurrentToken, ref kind2) && kind2 == SyntaxKind.CustomKeyword && PeekToken(1).Kind == SyntaxKind.EventKeyword)
						{
							return ParseSpecifierDeclaration(attributes, syntaxList);
						}
						break;
					}
					}
				}
				return ParseVarDeclStatement(attributes, syntaxList);
			}
			case SyntaxKind.LetKeyword:
			case SyntaxKind.SetKeyword:
				return ParseAssignmentStatement();
			case SyntaxKind.ErrorKeyword:
				return ParseError();
			case SyntaxKind.ThrowKeyword:
				return ParseThrowStatement();
			case SyntaxKind.IntegerLiteralToken:
				if (IsFirstStatementOnLine(CurrentToken))
				{
					return ParseLabel();
				}
				break;
			case SyntaxKind.IdentifierToken:
			{
				if (ShouldParseAsLabel())
				{
					return ParseLabel();
				}
				SyntaxKind kind = SyntaxKind.None;
				if (TryIdentifierAsContextualKeyword(CurrentToken, ref kind))
				{
					if (kind == SyntaxKind.MidKeyword)
					{
						if (PeekToken(1).Kind == SyntaxKind.OpenParenToken)
						{
							return ParseMid();
						}
					}
					else
					{
						if (kind == SyntaxKind.CustomKeyword && PeekToken(1).Kind == SyntaxKind.EventKeyword)
						{
							return ParseSpecifierDeclaration();
						}
						if (kind == SyntaxKind.AsyncKeyword || kind == SyntaxKind.IteratorKeyword)
						{
							SyntaxToken syntaxToken = PeekToken(1);
							if (SyntaxFacts.IsSpecifier(syntaxToken.Kind) || SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken.Kind))
							{
								return ParseSpecifierDeclaration();
							}
						}
						else
						{
							if (kind == SyntaxKind.AwaitKeyword && Context.IsWithinAsyncMethodOrLambda)
							{
								return ParseAwaitStatement();
							}
							if (kind == SyntaxKind.YieldKeyword && Context.IsWithinIteratorMethodOrLambdaOrProperty)
							{
								return ParseYieldStatement();
							}
						}
					}
				}
				return ParseAssignmentOrInvocationStatement();
			}
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.GetTypeKeyword:
			case SyntaxKind.GetXmlNamespaceKeyword:
			case SyntaxKind.GlobalKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.MeKeyword:
			case SyntaxKind.MyBaseKeyword:
			case SyntaxKind.MyClassKeyword:
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.TryCastKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
			case SyntaxKind.VariantKeyword:
			case SyntaxKind.ExclamationToken:
			case SyntaxKind.DotToken:
				return ParseAssignmentOrInvocationStatement();
			case SyntaxKind.EmptyToken:
				return ParseEmptyStatement();
			case SyntaxKind.EraseKeyword:
				return ParseErase();
			case SyntaxKind.GetKeyword:
				if ((IsValidStatementTerminator(PeekToken(1)) || PeekToken(1).Kind == SyntaxKind.OpenParenToken) && BlockContextExtensions.IsWithin(Context, SyntaxKind.SetAccessorBlock, SyntaxKind.GetAccessorBlock))
				{
					return ParsePropertyOrEventAccessor(SyntaxKind.GetAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
				}
				return ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteGetStatement);
			case SyntaxKind.GosubKeyword:
				return ParseAnachronisticStatement();
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.DeclareKeyword:
			case SyntaxKind.DelegateKeyword:
			case SyntaxKind.EnumKeyword:
			case SyntaxKind.EventKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.ImplementsKeyword:
			case SyntaxKind.ImportsKeyword:
			case SyntaxKind.InheritsKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.NamespaceKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.OptionKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.StructureKeyword:
			case SyntaxKind.SubKeyword:
				return ParseDeclarationStatementInternal();
			case SyntaxKind.QuestionToken:
				if (CanStartConsequenceExpression(PeekToken(1).Kind, qualified: false))
				{
					return ParseAssignmentOrInvocationStatement();
				}
				return ParsePrintStatement();
			default:
				if (CanFollowStatement(CurrentToken))
				{
					return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement();
				}
				break;
			case SyntaxKind.ColonToken:
			case SyntaxKind.StatementTerminatorToken:
				break;
			}
			return ReportUnrecognizedStatementError(ERRID.ERR_Syntax);
		}

		private EmptyStatementSyntax ParseEmptyStatement()
		{
			PunctuationSyntax empty = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(empty);
		}

		private StatementSyntax ParseSpecifierDeclaration()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
			if (CurrentToken.Kind == SyntaxKind.LessThanToken)
			{
				attributes = ParseAttributeLists(allowFileLevelAttributes: false);
			}
			return ParseSpecifierDeclaration(attributes);
		}

		private StatementSyntax ParseSpecifierDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = ParseSpecifiers();
			return ParseSpecifierDeclaration(attributes, modifiers);
		}

		private StatementSyntax ParseSpecifierDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			StatementSyntax statementSyntax = null;
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.PropertyKeyword:
				statementSyntax = ParsePropertyDefinition(attributes, modifiers);
				break;
			case SyntaxKind.IdentifierToken:
			{
				if (Context.BlockKind == SyntaxKind.EnumBlock && !modifiers.Any())
				{
					statementSyntax = ParseEnumMemberOrLabel(attributes);
					break;
				}
				KeywordSyntax k = null;
				if (TryIdentifierAsContextualKeyword(CurrentToken, ref k))
				{
					if (k.Kind == SyntaxKind.CustomKeyword)
					{
						return ParseCustomEventDefinition(attributes, modifiers);
					}
					if (k.Kind == SyntaxKind.TypeKeyword && PeekToken(1).Kind == SyntaxKind.IdentifierToken && IsValidStatementTerminator(PeekToken(2)) && ParserExtensions.AnyAndOnly(modifiers, SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.FriendKeyword, SyntaxKind.PrivateKeyword))
					{
						statementSyntax = ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteStructureNotType, attributes, modifiers);
						break;
					}
				}
				statementSyntax = ParseVarDeclStatement(attributes, modifiers);
				break;
			}
			case SyntaxKind.EnumKeyword:
				statementSyntax = ParseEnumStatement(attributes, modifiers);
				break;
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.StructureKeyword:
				statementSyntax = ParseTypeStatement(attributes, modifiers);
				break;
			case SyntaxKind.DeclareKeyword:
				statementSyntax = ParseProcDeclareStatement(attributes, modifiers);
				break;
			case SyntaxKind.EventKeyword:
				statementSyntax = ParseEventDefinition(attributes, modifiers);
				break;
			case SyntaxKind.SubKeyword:
				statementSyntax = ParseSubStatement(attributes, modifiers);
				break;
			case SyntaxKind.FunctionKeyword:
				statementSyntax = ParseFunctionStatement(attributes, modifiers);
				break;
			case SyntaxKind.OperatorKeyword:
				statementSyntax = ParseOperatorStatement(attributes, modifiers);
				break;
			case SyntaxKind.DelegateKeyword:
				statementSyntax = ParseDelegateStatement(attributes, modifiers);
				break;
			case SyntaxKind.AddHandlerKeyword:
				statementSyntax = ParsePropertyOrEventAccessor(SyntaxKind.AddHandlerAccessorStatement, attributes, modifiers);
				break;
			case SyntaxKind.RemoveHandlerKeyword:
				statementSyntax = ParsePropertyOrEventAccessor(SyntaxKind.RemoveHandlerAccessorStatement, attributes, modifiers);
				break;
			case SyntaxKind.RaiseEventKeyword:
				statementSyntax = ParsePropertyOrEventAccessor(SyntaxKind.RaiseEventAccessorStatement, attributes, modifiers);
				break;
			case SyntaxKind.GetKeyword:
				statementSyntax = ParsePropertyOrEventAccessor(SyntaxKind.GetAccessorStatement, attributes, modifiers);
				break;
			case SyntaxKind.SetKeyword:
				statementSyntax = ParsePropertyOrEventAccessor(SyntaxKind.SetAccessorStatement, attributes, modifiers);
				break;
			case SyntaxKind.ImplementsKeyword:
			case SyntaxKind.InheritsKeyword:
				statementSyntax = ParseInheritsImplementsStatement(attributes, modifiers);
				break;
			case SyntaxKind.ImportsKeyword:
				statementSyntax = ParseImportsStatement(attributes, modifiers);
				break;
			case SyntaxKind.NamespaceKeyword:
				statementSyntax = ParseNamespaceStatement(attributes, modifiers);
				break;
			case SyntaxKind.OptionKeyword:
				statementSyntax = ParseOptionStatement(attributes, modifiers);
				break;
			default:
				switch (Context.BlockKind)
				{
				case SyntaxKind.CompilationUnit:
				case SyntaxKind.NamespaceBlock:
				case SyntaxKind.ModuleBlock:
				case SyntaxKind.StructureBlock:
				case SyntaxKind.InterfaceBlock:
				case SyntaxKind.ClassBlock:
				case SyntaxKind.EnumBlock:
				case SyntaxKind.PropertyBlock:
					statementSyntax = ((attributes.Any() && !modifiers.Any()) ? ReportUnrecognizedStatementError(ERRID.ERR_StandaloneAttribute, attributes, modifiers) : ((!modifiers.Any() || !CurrentToken.IsKeyword) ? ReportUnrecognizedStatementError(ERRID.ERR_ExpectedIdentifier, attributes, modifiers, createMissingIdentifier: true) : ReportUnrecognizedStatementError(ERRID.ERR_InvalidUseOfKeyword, attributes, modifiers, createMissingIdentifier: false, forceErrorOnFirstToken: true)));
					break;
				default:
					statementSyntax = ParseVarDeclStatement(attributes, modifiers);
					break;
				}
				break;
			}
			return statementSyntax;
		}

		private EnumStatementSyntax ParseEnumStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>))
		{
			KeywordSyntax enumKeyword = (KeywordSyntax)CurrentToken;
			AsClauseSyntax underlyingType = null;
			GetNextToken();
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				TypeParameterListSyntax unexpected = ReportSyntaxError(ParseGenericParameters(), ERRID.ERR_GenericParamsOnInvalidMember);
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, unexpected);
			}
			if (identifierTokenSyntax.ContainsDiagnostics)
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ResyncAt(new SyntaxKind[1] { SyntaxKind.AsKeyword }));
			}
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				bool allowedEmptyGenericArguments = false;
				TypeSyntax typeSyntax = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = SyntaxNodeExtensions.AddTrailingSyntax(typeSyntax, ResyncAt());
				}
				underlyingType = SyntaxFactory.SimpleAsClause(asKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
			}
			return SyntaxFactory.EnumStatement(attributes, modifiers, enumKeyword, identifierTokenSyntax, underlyingType);
		}

		private StatementSyntax ParseEnumMemberOrLabel(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
		{
			if (!attributes.Any() && ShouldParseAsLabel())
			{
				return ParseLabel();
			}
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (identifierTokenSyntax.ContainsDiagnostics)
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ResyncAt(new SyntaxKind[1] { SyntaxKind.EqualsToken }));
			}
			EqualsValueSyntax initializer = null;
			PunctuationSyntax token = null;
			ExpressionSyntax expressionSyntax = null;
			if (TryGetTokenAndEatNewLine(SyntaxKind.EqualsToken, ref token))
			{
				expressionSyntax = ParseExpressionCore();
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax);
				}
				initializer = SyntaxFactory.EqualsValue(token, expressionSyntax);
			}
			return SyntaxFactory.EnumMemberDeclaration(attributes, identifierTokenSyntax, initializer);
		}

		private TypeStatementSyntax ParseTypeStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>))
		{
			TypeParameterListSyntax typeParameterList = null;
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			SyntaxKind syntaxKind = keywordSyntax.Kind switch
			{
				SyntaxKind.ModuleKeyword => SyntaxKind.ModuleStatement, 
				SyntaxKind.ClassKeyword => SyntaxKind.ClassStatement, 
				SyntaxKind.StructureKeyword => SyntaxKind.StructureStatement, 
				SyntaxKind.InterfaceKeyword => SyntaxKind.InterfaceStatement, 
				_ => throw ExceptionUtilities.UnexpectedValue(keywordSyntax.Kind), 
			};
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (identifierTokenSyntax.ContainsDiagnostics)
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ResyncAt(new SyntaxKind[2]
				{
					SyntaxKind.OfKeyword,
					SyntaxKind.OpenParenToken
				}));
			}
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				if (syntaxKind == SyntaxKind.ModuleStatement)
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ReportGenericParamsDisallowedError(ERRID.ERR_ModulesCannotBeGeneric));
				}
				else
				{
					typeParameterList = ParseGenericParameters();
				}
			}
			TypeStatementSyntax typeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TypeStatement(syntaxKind, attributes, modifiers, keywordSyntax, identifierTokenSyntax, typeParameterList);
			if ((syntaxKind == SyntaxKind.ModuleStatement || syntaxKind == SyntaxKind.InterfaceStatement) && typeStatementSyntax.Modifiers.Any(530))
			{
				typeStatementSyntax = CheckFeatureAvailability((syntaxKind == SyntaxKind.ModuleStatement) ? Feature.PartialModules : Feature.PartialInterfaces, typeStatementSyntax);
			}
			return typeStatementSyntax;
		}

		private TypeParameterListSyntax ReportGenericParamsDisallowedError(ERRID errid)
		{
			TypeParameterListSyntax typeParameterListSyntax = ParseGenericParameters();
			if (typeParameterListSyntax.CloseParenToken.IsMissing)
			{
				typeParameterListSyntax = ResyncAt(typeParameterListSyntax);
			}
			typeParameterListSyntax = ReportSyntaxError(typeParameterListSyntax, errid);
			return AdjustTriviaForMissingTokens(typeParameterListSyntax);
		}

		private TypeArgumentListSyntax ReportGenericArgumentsDisallowedError(ERRID errid)
		{
			bool allowEmptyGenericArguments = true;
			bool AllowNonEmptyGenericArguments = true;
			TypeArgumentListSyntax typeArgumentListSyntax = ParseGenericArguments(ref allowEmptyGenericArguments, ref AllowNonEmptyGenericArguments);
			if (typeArgumentListSyntax.CloseParenToken.IsMissing)
			{
				typeArgumentListSyntax = ResyncAt(typeArgumentListSyntax);
			}
			return ReportSyntaxError(typeArgumentListSyntax, errid);
		}

		private bool TryRejectGenericParametersForMemberDecl(ref TypeParameterListSyntax genericParams)
		{
			if (!BeginsGeneric())
			{
				genericParams = null;
				return false;
			}
			genericParams = ReportGenericParamsDisallowedError(ERRID.ERR_GenericParamsOnInvalidMember);
			return true;
		}

		private NamespaceStatementSyntax ParseNamespaceStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Specifiers)
		{
			KeywordSyntax keywordSyntax = ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvalidOnInheritsImplOpt, attributes, Specifiers, (KeywordSyntax)CurrentToken);
			if (IsScript)
			{
				keywordSyntax = SyntaxNodeExtensions.AddError(keywordSyntax, ERRID.ERR_NamespaceNotAllowedInScript);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			GetNextToken();
			bool allowedEmptyGenericArguments = false;
			NameSyntax nameSyntax = ParseName(requireQualification: false, allowGlobalNameSpace: true, allowGenericArguments: false, allowGenericsWithoutOf: true, nonArrayName: false, disallowGenericArgumentsOnLastQualifiedName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments, isNameInNamespaceDeclaration: true);
			if (nameSyntax.ContainsDiagnostics)
			{
				unexpected = ResyncAt();
			}
			NamespaceStatementSyntax namespaceStatementSyntax = SyntaxFactory.NamespaceStatement(keywordSyntax, nameSyntax);
			if (unexpected.Node != null)
			{
				namespaceStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(namespaceStatementSyntax, unexpected);
			}
			return namespaceStatementSyntax;
		}

		private StatementSyntax ParseEndStatement()
		{
			SyntaxToken nextToken = PeekToken(1);
			if (CanFollowStatementButIsNotSelectFollowingExpression(nextToken))
			{
				return ParseStopOrEndStatement();
			}
			return ParseGroupEndStatement();
		}

		private StatementSyntax ParseGroupEndStatement()
		{
			KeywordSyntax endKeyword = (KeywordSyntax)CurrentToken;
			SyntaxToken syntaxToken = PeekToken(1);
			SyntaxToken syntaxToken2 = (IsValidStatementTerminator(syntaxToken) ? null : syntaxToken);
			SyntaxKind endStatementKindFromKeyword = GetEndStatementKindFromKeyword(syntaxToken.Kind);
			if (endStatementKindFromKeyword == SyntaxKind.None)
			{
				return ReportSyntaxError(ParseStopOrEndStatement(), ERRID.ERR_UnrecognizedEnd);
			}
			GetNextToken();
			GetNextToken();
			return SyntaxFactory.EndBlockStatement(endStatementKindFromKeyword, endKeyword, (KeywordSyntax)syntaxToken2);
		}

		private SyntaxKind PeekEndStatement(int i)
		{
			return PeekToken(i).Kind switch
			{
				SyntaxKind.LoopKeyword => SyntaxKind.SimpleLoopStatement, 
				SyntaxKind.NextKeyword => SyntaxKind.NextStatement, 
				SyntaxKind.EndKeyword => GetEndStatementKindFromKeyword(PeekToken(i + 1).Kind), 
				SyntaxKind.EndIfKeyword => SyntaxKind.EndIfStatement, 
				SyntaxKind.WendKeyword => SyntaxKind.EndWhileStatement, 
				_ => SyntaxKind.None, 
			};
		}

		private static SyntaxKind GetEndStatementKindFromKeyword(SyntaxKind kind)
		{
			return kind switch
			{
				SyntaxKind.IfKeyword => SyntaxKind.EndIfStatement, 
				SyntaxKind.UsingKeyword => SyntaxKind.EndUsingStatement, 
				SyntaxKind.WithKeyword => SyntaxKind.EndWithStatement, 
				SyntaxKind.StructureKeyword => SyntaxKind.EndStructureStatement, 
				SyntaxKind.EnumKeyword => SyntaxKind.EndEnumStatement, 
				SyntaxKind.InterfaceKeyword => SyntaxKind.EndInterfaceStatement, 
				SyntaxKind.SubKeyword => SyntaxKind.EndSubStatement, 
				SyntaxKind.FunctionKeyword => SyntaxKind.EndFunctionStatement, 
				SyntaxKind.OperatorKeyword => SyntaxKind.EndOperatorStatement, 
				SyntaxKind.SelectKeyword => SyntaxKind.EndSelectStatement, 
				SyntaxKind.TryKeyword => SyntaxKind.EndTryStatement, 
				SyntaxKind.GetKeyword => SyntaxKind.EndGetStatement, 
				SyntaxKind.SetKeyword => SyntaxKind.EndSetStatement, 
				SyntaxKind.PropertyKeyword => SyntaxKind.EndPropertyStatement, 
				SyntaxKind.AddHandlerKeyword => SyntaxKind.EndAddHandlerStatement, 
				SyntaxKind.RemoveHandlerKeyword => SyntaxKind.EndRemoveHandlerStatement, 
				SyntaxKind.RaiseEventKeyword => SyntaxKind.EndRaiseEventStatement, 
				SyntaxKind.EventKeyword => SyntaxKind.EndEventStatement, 
				SyntaxKind.ClassKeyword => SyntaxKind.EndClassStatement, 
				SyntaxKind.ModuleKeyword => SyntaxKind.EndModuleStatement, 
				SyntaxKind.NamespaceKeyword => SyntaxKind.EndNamespaceStatement, 
				SyntaxKind.SyncLockKeyword => SyntaxKind.EndSyncLockStatement, 
				SyntaxKind.WhileKeyword => SyntaxKind.EndWhileStatement, 
				_ => SyntaxKind.None, 
			};
		}

		private bool PeekDeclarationStatement(int i)
		{
			while (true)
			{
				SyntaxToken syntaxToken = PeekToken(i);
				switch (syntaxToken.Kind)
				{
				case SyntaxKind.IdentifierToken:
				{
					SyntaxKind possibleKeywordKind = ((IdentifierTokenSyntax)syntaxToken).PossibleKeywordKind;
					if (possibleKeywordKind != SyntaxKind.CustomKeyword && possibleKeywordKind != SyntaxKind.AsyncKeyword && possibleKeywordKind != SyntaxKind.IteratorKeyword)
					{
						return false;
					}
					break;
				}
				case SyntaxKind.ClassKeyword:
				case SyntaxKind.DeclareKeyword:
				case SyntaxKind.DelegateKeyword:
				case SyntaxKind.EnumKeyword:
				case SyntaxKind.EventKeyword:
				case SyntaxKind.FunctionKeyword:
				case SyntaxKind.GetKeyword:
				case SyntaxKind.InterfaceKeyword:
				case SyntaxKind.ModuleKeyword:
				case SyntaxKind.NamespaceKeyword:
				case SyntaxKind.OperatorKeyword:
				case SyntaxKind.PropertyKeyword:
				case SyntaxKind.SetKeyword:
				case SyntaxKind.StructureKeyword:
				case SyntaxKind.SubKeyword:
					return true;
				default:
					return false;
				case SyntaxKind.DefaultKeyword:
				case SyntaxKind.FriendKeyword:
				case SyntaxKind.MustInheritKeyword:
				case SyntaxKind.MustOverrideKeyword:
				case SyntaxKind.NarrowingKeyword:
				case SyntaxKind.NotInheritableKeyword:
				case SyntaxKind.NotOverridableKeyword:
				case SyntaxKind.OverloadsKeyword:
				case SyntaxKind.OverridableKeyword:
				case SyntaxKind.OverridesKeyword:
				case SyntaxKind.PartialKeyword:
				case SyntaxKind.PrivateKeyword:
				case SyntaxKind.ProtectedKeyword:
				case SyntaxKind.PublicKeyword:
				case SyntaxKind.ReadOnlyKeyword:
				case SyntaxKind.ShadowsKeyword:
				case SyntaxKind.SharedKeyword:
				case SyntaxKind.StaticKeyword:
				case SyntaxKind.WideningKeyword:
				case SyntaxKind.WithEventsKeyword:
				case SyntaxKind.WriteOnlyKeyword:
				case SyntaxKind.CustomKeyword:
				case SyntaxKind.AsyncKeyword:
				case SyntaxKind.IteratorKeyword:
					break;
				}
				i++;
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> ParseSpecifiers()
		{
			SyntaxListBuilder<KeywordSyntax> syntaxListBuilder = _pool.Allocate<KeywordSyntax>();
			while (true)
			{
				ERRID eRRID = ERRID.ERR_None;
				SyntaxToken syntaxToken = CurrentToken;
				KeywordSyntax keywordSyntax;
				switch (syntaxToken.Kind)
				{
				case SyntaxKind.IdentifierToken:
				{
					KeywordSyntax k = null;
					if (!TryTokenAsContextualKeyword(CurrentToken, ref k))
					{
						break;
					}
					if (k.Kind == SyntaxKind.CustomKeyword)
					{
						SyntaxToken syntaxToken2 = PeekToken(1);
						if (syntaxToken2.Kind == SyntaxKind.EventKeyword || (!SyntaxFacts.IsSpecifier(syntaxToken2.Kind) && !SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken2.Kind)))
						{
							break;
						}
						syntaxToken = ReportSyntaxError(k, ERRID.ERR_InvalidUseOfCustomModifier);
					}
					else
					{
						if (k.Kind != SyntaxKind.AsyncKeyword && k.Kind != SyntaxKind.IteratorKeyword)
						{
							break;
						}
						SyntaxToken syntaxToken3 = PeekToken(1);
						if (!SyntaxFacts.IsSpecifier(syntaxToken3.Kind) && !SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken3.Kind))
						{
							break;
						}
						syntaxToken = k;
						syntaxToken = CheckFeatureAvailability((k.Kind == SyntaxKind.AsyncKeyword) ? Feature.AsyncExpressions : Feature.Iterators, syntaxToken);
					}
					goto case SyntaxKind.ConstKeyword;
				}
				case SyntaxKind.ConstKeyword:
				case SyntaxKind.DefaultKeyword:
				case SyntaxKind.DimKeyword:
				case SyntaxKind.FriendKeyword:
				case SyntaxKind.MustInheritKeyword:
				case SyntaxKind.MustOverrideKeyword:
				case SyntaxKind.NarrowingKeyword:
				case SyntaxKind.NotInheritableKeyword:
				case SyntaxKind.NotOverridableKeyword:
				case SyntaxKind.OverloadsKeyword:
				case SyntaxKind.OverridableKeyword:
				case SyntaxKind.OverridesKeyword:
				case SyntaxKind.PartialKeyword:
				case SyntaxKind.PrivateKeyword:
				case SyntaxKind.ProtectedKeyword:
				case SyntaxKind.PublicKeyword:
				case SyntaxKind.ReadOnlyKeyword:
				case SyntaxKind.ShadowsKeyword:
				case SyntaxKind.SharedKeyword:
				case SyntaxKind.StaticKeyword:
				case SyntaxKind.WideningKeyword:
				case SyntaxKind.WithEventsKeyword:
				case SyntaxKind.WriteOnlyKeyword:
					keywordSyntax = (KeywordSyntax)syntaxToken;
					if (eRRID != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, eRRID);
					}
					goto IL_0255;
				}
				break;
				IL_0255:
				syntaxListBuilder.Add(keywordSyntax);
				GetNextToken();
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private StatementSyntax ParseVarDeclStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			bool flag = false;
			switch (Context.BlockKind)
			{
			case SyntaxKind.CompilationUnit:
			case SyntaxKind.NamespaceBlock:
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
			case SyntaxKind.EnumBlock:
			case SyntaxKind.PropertyBlock:
				flag = true;
				break;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> separatedSyntaxList = ParseVariableDeclaration(!flag);
			StatementSyntax statementSyntax;
			if (flag)
			{
				statementSyntax = SyntaxFactory.FieldDeclaration(attributes, modifiers, separatedSyntaxList);
			}
			else
			{
				statementSyntax = SyntaxFactory.LocalDeclarationStatement(modifiers, separatedSyntaxList);
				if (attributes.Any())
				{
					statementSyntax = ((!modifiers.Any(551)) ? SyntaxNodeExtensions.AddLeadingSyntax(statementSyntax, attributes.Node, ERRID.ERR_LocalsCannotHaveAttributes) : SyntaxNodeExtensions.AddLeadingSyntax(statementSyntax, attributes.Node));
				}
			}
			if (!modifiers.Any())
			{
				statementSyntax = ReportSyntaxError(statementSyntax, attributes.Any() ? ERRID.ERR_StandaloneAttribute : ERRID.ERR_ExpectedSpecifier);
			}
			return statementSyntax;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> ParseVariableDeclaration(bool allowAsNewWith)
		{
			SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> item = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
			bool checkForCustom = true;
			SeparatedSyntaxListBuilder<ModifiedIdentifierSyntax> item2 = _pool.AllocateSeparated<ModifiedIdentifierSyntax>();
			while (true)
			{
				item2.Clear();
				PunctuationSyntax token;
				while (true)
				{
					ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseModifiedIdentifier(AllowExplicitArraySizes: true, checkForCustom);
					checkForCustom = false;
					if (modifiedIdentifierSyntax.ContainsDiagnostics)
					{
						modifiedIdentifierSyntax = ResyncAt(modifiedIdentifierSyntax, SyntaxKind.AsKeyword, SyntaxKind.CommaToken, SyntaxKind.NewKeyword, SyntaxKind.EqualsToken);
					}
					item2.Add(modifiedIdentifierSyntax);
					token = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
					{
						break;
					}
					item2.AddSeparator(token);
				}
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ModifiedIdentifierSyntax> separatedSyntaxList = item2.ToList();
				AsClauseSyntax optionalAsClause = null;
				EqualsValueSyntax optionalInitializer = null;
				ParseFieldOrPropertyAsClauseAndInitializer(isProperty: false, allowAsNewWith, ref optionalAsClause, ref optionalInitializer);
				VariableDeclaratorSyntax node = SyntaxFactory.VariableDeclarator(separatedSyntaxList, optionalAsClause, optionalInitializer);
				item.Add(node);
				token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			_pool.Free(in item2);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private void ParseFieldOrPropertyAsClauseAndInitializer(bool isProperty, bool allowAsNewWith, ref AsClauseSyntax optionalAsClause, ref EqualsValueSyntax optionalInitializer)
		{
			KeywordSyntax asKeyword = null;
			KeywordSyntax keywordSyntax = null;
			ArgumentListSyntax argumentList = null;
			TypeSyntax typeSyntax = null;
			KeywordSyntax k = null;
			GreenNode greenNode = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				asKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				ObjectCollectionInitializerSyntax initializer = null;
				if (CurrentToken.Kind == SyntaxKind.NewKeyword)
				{
					keywordSyntax = (KeywordSyntax)CurrentToken;
					GetNextToken();
					if (isProperty && CurrentToken.Kind == SyntaxKind.LessThanToken)
					{
						greenNode = ParseAttributeLists(allowFileLevelAttributes: false).Node;
					}
					if (CurrentToken.Kind == SyntaxKind.WithKeyword)
					{
						optionalAsClause = null;
					}
					else
					{
						bool allowedEmptyGenericArguments = false;
						typeSyntax = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
						if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
						{
							argumentList = ParseParenthesizedArguments();
						}
						if (isProperty)
						{
							TryEatNewLineIfFollowedBy(SyntaxKind.FromKeyword);
						}
						if (TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.FromKeyword, ref k))
						{
							GetNextToken();
							initializer = ParseObjectCollectionInitializer(k);
						}
						optionalAsClause = SyntaxFactory.AsNewClause(asKeyword, new ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, keywordSyntax, greenNode, typeSyntax, argumentList, initializer));
					}
				}
				else
				{
					if (isProperty && CurrentToken.Kind == SyntaxKind.LessThanToken)
					{
						greenNode = ParseAttributeLists(allowFileLevelAttributes: false).Node;
					}
					typeSyntax = ParseGeneralType();
					if (typeSyntax.ContainsDiagnostics)
					{
						typeSyntax = ResyncAt(typeSyntax, SyntaxKind.CommaToken, SyntaxKind.EqualsToken);
					}
					optionalAsClause = SyntaxFactory.SimpleAsClause(asKeyword, greenNode, typeSyntax);
				}
			}
			PunctuationSyntax token = null;
			if (keywordSyntax == null)
			{
				if (TryGetTokenAndEatNewLine(SyntaxKind.EqualsToken, ref token))
				{
					ExpressionSyntax value = ParseExpressionCore();
					optionalInitializer = SyntaxFactory.EqualsValue(token, value);
					if (optionalInitializer.ContainsDiagnostics)
					{
						optionalInitializer = ResyncAt(optionalInitializer, SyntaxKind.CommaToken);
					}
				}
				return;
			}
			ObjectMemberInitializerSyntax objectMemberInitializerSyntax = null;
			if (CurrentToken.Kind != SyntaxKind.WithKeyword)
			{
				return;
			}
			KeywordSyntax syntax = (KeywordSyntax)CurrentToken;
			if (k != null)
			{
				syntax = ReportSyntaxError(syntax, ERRID.ERR_CantCombineInitializers);
				optionalAsClause = SyntaxNodeExtensions.AddTrailingSyntax(optionalAsClause, syntax);
				GetNextToken();
				return;
			}
			objectMemberInitializerSyntax = ParseObjectInitializerList(typeSyntax == null, allowAsNewWith);
			KeywordSyntax k2 = null;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken && TryIdentifierAsContextualKeyword(CurrentToken, ref k2) && k2.Kind == SyntaxKind.FromKeyword)
			{
				objectMemberInitializerSyntax = SyntaxNodeExtensions.AddTrailingSyntax(objectMemberInitializerSyntax, k2, ERRID.ERR_CantCombineInitializers);
				GetNextToken();
			}
			NewExpressionSyntax newExpressionSyntax = null;
			if (typeSyntax == null)
			{
				if (!allowAsNewWith)
				{
					syntax = ReportSyntaxError(syntax, ERRID.ERR_UnrecognizedTypeKeyword);
				}
				newExpressionSyntax = new AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, keywordSyntax, null, objectMemberInitializerSyntax);
			}
			else
			{
				newExpressionSyntax = new ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, keywordSyntax, greenNode, typeSyntax, argumentList, objectMemberInitializerSyntax);
			}
			optionalAsClause = SyntaxFactory.AsNewClause(asKeyword, newExpressionSyntax);
		}

		private CollectionInitializerSyntax ParseCollectionInitializer()
		{
			PunctuationSyntax token = null;
			if (!TryGetTokenAndEatNewLine(SyntaxKind.OpenBraceToken, ref token, createIfMissing: true))
			{
				return SyntaxFactory.CollectionInitializer(token, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseBraceToken));
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>);
			if (CurrentToken.Kind != SyntaxKind.CloseBraceToken)
			{
				SeparatedSyntaxListBuilder<ExpressionSyntax> item = _pool.AllocateSeparated<ExpressionSyntax>();
				while (true)
				{
					ExpressionSyntax expressionSyntax = ParseExpressionCore();
					if (expressionSyntax.ContainsDiagnostics)
					{
						expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken);
					}
					item.Add(expressionSyntax);
					PunctuationSyntax token2 = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
					{
						break;
					}
					item.AddSeparator(token2);
				}
				separatedSyntaxList = item.ToList();
				_pool.Free(in item);
			}
			PunctuationSyntax closingRightBrace = GetClosingRightBrace();
			return SyntaxFactory.CollectionInitializer(token, separatedSyntaxList, closingRightBrace);
		}

		private PunctuationSyntax GetClosingRightBrace()
		{
			PunctuationSyntax token = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			if (CurrentToken.Kind != SyntaxKind.CloseBraceToken)
			{
				syntaxList = ResyncAt(new SyntaxKind[1] { SyntaxKind.CloseBraceToken });
			}
			TryEatNewLineAndGetToken(SyntaxKind.CloseBraceToken, ref token, createIfMissing: true);
			if (syntaxList.Node != null)
			{
				token = SyntaxNodeExtensions.AddLeadingSyntax(token, syntaxList, ERRID.ERR_ExpectedRbrace);
			}
			return token;
		}

		private ObjectMemberInitializerSyntax ParseObjectInitializerList(bool anonymousTypeInitializer = false, bool anonymousTypesAllowedHere = true)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			if (anonymousTypeInitializer && !anonymousTypesAllowedHere)
			{
				keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_UnrecognizedTypeKeyword);
			}
			GetNextToken();
			if (PeekPastStatementTerminator().Kind == SyntaxKind.OpenBraceToken)
			{
				TryEatNewLine();
			}
			PunctuationSyntax token = null;
			if (!TryGetTokenAndEatNewLine(SyntaxKind.OpenBraceToken, ref token, createIfMissing: true))
			{
				return SyntaxFactory.ObjectMemberInitializer(keywordSyntax, token, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseBraceToken));
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FieldInitializerSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FieldInitializerSyntax>);
			if (CurrentToken.Kind != SyntaxKind.CloseBraceToken && CurrentToken.Kind != SyntaxKind.StatementTerminatorToken && CurrentToken.Kind != SyntaxKind.ColonToken)
			{
				SeparatedSyntaxListBuilder<FieldInitializerSyntax> item = _pool.AllocateSeparated<FieldInitializerSyntax>();
				while (true)
				{
					FieldInitializerSyntax fieldInitializerSyntax = ParseAssignmentInitializer(anonymousTypeInitializer);
					if (fieldInitializerSyntax.ContainsDiagnostics)
					{
						fieldInitializerSyntax = ResyncAt(fieldInitializerSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken);
					}
					item.Add(fieldInitializerSyntax);
					PunctuationSyntax token2 = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
					{
						break;
					}
					item.AddSeparator(token2);
				}
				separatedSyntaxList = item.ToList();
				_pool.Free(in item);
			}
			else
			{
				token = ReportSyntaxError(token, anonymousTypeInitializer ? ERRID.ERR_AnonymousTypeNeedField : ERRID.ERR_InitializerExpected);
			}
			PunctuationSyntax closingRightBrace = GetClosingRightBrace();
			return SyntaxFactory.ObjectMemberInitializer(keywordSyntax, token, separatedSyntaxList, closingRightBrace);
		}

		private ObjectCollectionInitializerSyntax ParseObjectCollectionInitializer(KeywordSyntax fromKeyword)
		{
			fromKeyword = CheckFeatureAvailability(Feature.CollectionInitializers, fromKeyword);
			if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken && PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
			{
				TryEatNewLine();
			}
			CollectionInitializerSyntax initializer = ParseCollectionInitializer();
			return SyntaxFactory.ObjectCollectionInitializer(fromKeyword, initializer);
		}

		private FieldInitializerSyntax ParseAssignmentInitializer(bool anonymousTypeInitializer)
		{
			KeywordSyntax k = null;
			PunctuationSyntax punctuationSyntax = null;
			IdentifierTokenSyntax identifierTokenSyntax = null;
			PunctuationSyntax punctuationSyntax2 = null;
			if (anonymousTypeInitializer && TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.KeyKeyword, ref k))
			{
				GetNextToken();
			}
			ExpressionSyntax expressionSyntax;
			if (CurrentToken.Kind == SyntaxKind.DotToken)
			{
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				identifierTokenSyntax = ParseIdentifierAllowingKeyword();
				if (SyntaxKind.QuestionToken == CurrentToken.Kind)
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, CurrentToken);
					identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_NullableTypeInferenceNotSupported);
					GetNextToken();
				}
				if (CurrentToken.Kind == SyntaxKind.EqualsToken)
				{
					punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
					GetNextToken();
					TryEatNewLine();
				}
				else
				{
					punctuationSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken);
					punctuationSyntax2 = ReportSyntaxError(punctuationSyntax2, ERRID.ERR_ExpectedAssignmentOperatorInInit);
				}
			}
			else
			{
				if (anonymousTypeInitializer)
				{
					expressionSyntax = ParseExpressionCore();
					bool isNameDictionaryAccess = false;
					bool isRejectedXmlName = false;
					SyntaxToken syntaxToken = SyntaxNodeExtensions.ExtractAnonymousTypeMemberName(expressionSyntax, ref isNameDictionaryAccess, ref isRejectedXmlName);
					if (syntaxToken == null || syntaxToken.IsMissing)
					{
						SyntaxKind kind = expressionSyntax.Kind;
						if (kind == SyntaxKind.CharacterLiteralExpression || kind - 275 <= SyntaxKind.List || kind == SyntaxKind.StringLiteralExpression)
						{
							expressionSyntax = ReportSyntaxError(expressionSyntax, ERRID.ERR_AnonymousTypeExpectedIdentifier);
						}
						else if (expressionSyntax.Kind == SyntaxKind.EqualsExpression && ((BinaryExpressionSyntax)expressionSyntax).Left.Kind == SyntaxKind.IdentifierName)
						{
							expressionSyntax = ReportSyntaxError(expressionSyntax, ERRID.ERR_AnonymousTypeNameWithoutPeriod);
						}
						else
						{
							Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = ResyncAt(new SyntaxKind[2]
							{
								SyntaxKind.CommaToken,
								SyntaxKind.CloseBraceToken
							});
							expressionSyntax = ((!isRejectedXmlName) ? ReportSyntaxError(expressionSyntax, ERRID.ERR_AnonymousTypeFieldNameInference) : ReportSyntaxError(expressionSyntax, ERRID.ERR_AnonTypeFieldXMLNameInference));
							expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(expressionSyntax, unexpected);
						}
					}
					return SyntaxFactory.InferredFieldInitializer(k, expressionSyntax);
				}
				punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DotToken);
				identifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_ExpectedQualifiedNameInInit);
				punctuationSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken);
			}
			expressionSyntax = ParseExpressionCore();
			return SyntaxFactory.NamedFieldInitializer(k, punctuationSyntax, SyntaxFactory.IdentifierName(identifierTokenSyntax), punctuationSyntax2, expressionSyntax);
		}

		private ModifiedIdentifierSyntax ParseModifiedIdentifier(bool AllowExplicitArraySizes, bool checkForCustom)
		{
			SyntaxToken prevToken = PrevToken;
			SyntaxToken currentToken = CurrentToken;
			PunctuationSyntax optionalNullable = null;
			bool flag = false;
			if (checkForCustom)
			{
				KeywordSyntax k = null;
				if (TryTokenAsContextualKeyword(currentToken, SyntaxKind.CustomKeyword, ref k))
				{
					SyntaxToken syntaxToken = PeekToken(1);
					flag = SyntaxFacts.IsSpecifier(syntaxToken.Kind) || SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken.Kind);
				}
			}
			IdentifierTokenSyntax syntax;
			if (SyntaxFacts.IsSpecifier(currentToken.Kind))
			{
				if (prevToken != null && prevToken.IsEndOfLine)
				{
					syntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
					syntax = ReportSyntaxError(syntax, ERRID.ERR_ExpectedIdentifier);
					return SyntaxFactory.ModifiedIdentifier(syntax, null, null, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
				}
				syntax = SyntaxNodeExtensions.AddLeadingSyntax(unexpected: ParseSpecifiers().Node, node: ParseNullableIdentifier(ref optionalNullable), errorId: ERRID.ERR_ExtraSpecifiers);
			}
			else
			{
				syntax = ParseNullableIdentifier(ref optionalNullable);
				if (flag)
				{
					syntax = ReportSyntaxError(syntax, ERRID.ERR_InvalidUseOfCustomModifier);
				}
			}
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				return ParseArrayModifiedIdentifier(syntax, optionalNullable, AllowExplicitArraySizes);
			}
			return SyntaxFactory.ModifiedIdentifier(syntax, optionalNullable, null, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private ModifiedIdentifierSyntax ParseNullableModifiedIdentifier()
		{
			PunctuationSyntax optionalNullable = null;
			IdentifierTokenSyntax identifier = ParseNullableIdentifier(ref optionalNullable);
			return SyntaxFactory.ModifiedIdentifier(identifier, optionalNullable, null, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>));
		}

		private static bool CanTokenStartTypeName(SyntaxToken Token)
		{
			if (SyntaxFacts.IsPredefinedTypeOrVariant(Token.Kind))
			{
				return true;
			}
			SyntaxKind kind = Token.Kind;
			if (kind == SyntaxKind.GlobalKeyword || kind == SyntaxKind.IdentifierToken)
			{
				return true;
			}
			return false;
		}

		internal TypeSyntax ParseTypeName(bool nonArrayName = false, bool allowEmptyGenericArguments = false, ref bool allowedEmptyGenericArguments = false)
		{
			SyntaxToken currentToken = CurrentToken;
			SyntaxToken prevToken = PrevToken;
			TypeSyntax typeSyntax = null;
			NameSyntax nameSyntax = null;
			if (SyntaxFacts.IsPredefinedTypeKeyword(currentToken.Kind))
			{
				typeSyntax = SyntaxFactory.PredefinedType((KeywordSyntax)currentToken);
				goto IL_015b;
			}
			SyntaxKind kind = currentToken.Kind;
			if (kind <= SyntaxKind.VariantKeyword)
			{
				if (kind == SyntaxKind.GlobalKeyword)
				{
					goto IL_00a1;
				}
				if (kind == SyntaxKind.VariantKeyword)
				{
					nameSyntax = SyntaxFactory.IdentifierName(_scanner.MakeIdentifier((KeywordSyntax)currentToken));
					nameSyntax = ReportSyntaxError(nameSyntax, ERRID.ERR_ObsoleteObjectNotVariant);
					goto IL_015b;
				}
			}
			else
			{
				if (kind == SyntaxKind.OpenParenToken)
				{
					PunctuationSyntax token = null;
					TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
					typeSyntax = ParseTupleType(token);
					goto IL_0162;
				}
				if (kind == SyntaxKind.IdentifierToken)
				{
					goto IL_00a1;
				}
			}
			return ReportSyntaxError(ErrorId: (currentToken.Kind == SyntaxKind.NewKeyword && PeekToken(1).Kind == SyntaxKind.IdentifierToken) ? ERRID.ERR_InvalidNewInType : ((currentToken.Kind == SyntaxKind.OpenBraceToken && prevToken != null && prevToken.Kind == SyntaxKind.NewKeyword) ? ERRID.ERR_UnrecognizedTypeOrWith : ((!currentToken.IsKeyword) ? ERRID.ERR_UnrecognizedType : ERRID.ERR_UnrecognizedTypeKeyword)), syntax: SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()));
			IL_015b:
			GetNextToken();
			goto IL_0162;
			IL_00a1:
			nameSyntax = ParseName(requireQualification: false, allowGlobalNameSpace: true, allowGenericArguments: true, allowGenericsWithoutOf: true, nonArrayName, disallowGenericArgumentsOnLastQualifiedName: false, allowEmptyGenericArguments, ref allowedEmptyGenericArguments);
			goto IL_0162;
			IL_0162:
			if (typeSyntax == null)
			{
				typeSyntax = nameSyntax;
			}
			if (SyntaxKind.QuestionToken == CurrentToken.Kind)
			{
				if (_evaluatingConditionCompilationExpression)
				{
					typeSyntax = SyntaxNodeExtensions.AddTrailingSyntax(typeSyntax, CurrentToken, ERRID.ERR_BadNullTypeInCCExpression);
					GetNextToken();
					return typeSyntax;
				}
				if (allowedEmptyGenericArguments)
				{
					typeSyntax = ReportUnrecognizedTypeInGeneric(typeSyntax);
				}
				PunctuationSyntax questionMarkToken = (PunctuationSyntax)CurrentToken;
				NullableTypeSyntax nullableTypeSyntax = SyntaxFactory.NullableType(typeSyntax, questionMarkToken);
				GetNextToken();
				typeSyntax = nullableTypeSyntax;
			}
			return typeSyntax;
		}

		private TypeSyntax ParseTupleType(PunctuationSyntax openParen)
		{
			SeparatedSyntaxListBuilder<TupleElementSyntax> item = _pool.AllocateSeparated<TupleElementSyntax>();
			GreenNode greenNode = null;
			while (true)
			{
				IdentifierTokenSyntax identifierTokenSyntax = null;
				KeywordSyntax token = null;
				if (CurrentToken.Kind == SyntaxKind.IdentifierToken && (((IdentifierTokenSyntax)CurrentToken).TypeCharacter != 0 || PeekNextToken().Kind == SyntaxKind.AsKeyword))
				{
					identifierTokenSyntax = ParseIdentifier();
					TryGetToken(SyntaxKind.AsKeyword, ref token);
				}
				TypeSyntax type = null;
				if (token != null || identifierTokenSyntax == null)
				{
					type = ParseGeneralType();
				}
				TupleElementSyntax tupleElementSyntax;
				if (identifierTokenSyntax != null)
				{
					SimpleAsClauseSyntax asClause = null;
					if (token != null)
					{
						asClause = SyntaxFactory.SimpleAsClause(token, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), type);
					}
					tupleElementSyntax = SyntaxFactory.NamedTupleElement(identifierTokenSyntax, asClause);
				}
				else
				{
					tupleElementSyntax = SyntaxFactory.TypedTupleElement(type);
				}
				item.Add(tupleElementSyntax);
				PunctuationSyntax token2 = null;
				if (TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
				{
					item.AddSeparator(token2);
					continue;
				}
				if (CurrentToken.Kind == SyntaxKind.CloseParenToken || MustEndStatement(CurrentToken))
				{
					break;
				}
				GreenNode greenNode2 = ResyncAt(new SyntaxKind[2]
				{
					SyntaxKind.CommaToken,
					SyntaxKind.CloseParenToken
				}).Node;
				if (greenNode2 != null && !tupleElementSyntax.ContainsDiagnostics)
				{
					greenNode2 = ReportSyntaxError(greenNode2, ERRID.ERR_ArgumentSyntax);
				}
				if (CurrentToken.Kind == SyntaxKind.CommaToken)
				{
					token2 = (PunctuationSyntax)CurrentToken;
					token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, greenNode2);
					item.AddSeparator(token2);
					GetNextToken();
					continue;
				}
				greenNode = greenNode2;
				break;
			}
			PunctuationSyntax token3 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3, createIfMissing: true);
			if (greenNode != null)
			{
				token3 = SyntaxNodeExtensions.AddLeadingSyntax(token3, greenNode);
			}
			if (item.Count < 2)
			{
				item.AddSeparator(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
				IdentifierNameSyntax syntax = SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
				syntax = ReportSyntaxError(syntax, ERRID.ERR_TupleTooFewElements);
				item.Add(_syntaxFactory.TypedTupleElement(syntax));
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TupleElementSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			TupleTypeSyntax node = SyntaxFactory.TupleType(openParen, separatedSyntaxList, token3);
			return CheckFeatureAvailability(Feature.Tuples, node);
		}

		private TypeSyntax ReportUnrecognizedTypeInGeneric(TypeSyntax typeName)
		{
			switch (typeName.Kind)
			{
			case SyntaxKind.QualifiedName:
			{
				QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)typeName;
				if (qualifiedNameSyntax.Right is GenericNameSyntax genericName)
				{
					GenericNameSyntax right = ReportUnrecognizedTypeInGeneric(genericName);
					typeName = SyntaxFactory.QualifiedName(qualifiedNameSyntax.Left, qualifiedNameSyntax.DotToken, right);
				}
				else
				{
					NameSyntax left = (NameSyntax)ReportUnrecognizedTypeInGeneric(qualifiedNameSyntax.Left);
					typeName = SyntaxFactory.QualifiedName(left, qualifiedNameSyntax.DotToken, qualifiedNameSyntax.Right);
				}
				break;
			}
			case SyntaxKind.GenericName:
				typeName = ReportUnrecognizedTypeInGeneric((GenericNameSyntax)typeName);
				break;
			}
			return typeName;
		}

		private GenericNameSyntax ReportUnrecognizedTypeInGeneric(GenericNameSyntax genericName)
		{
			TypeArgumentListSyntax typeArgumentList = genericName.TypeArgumentList;
			typeArgumentList = SyntaxFactory.TypeArgumentList(typeArgumentList.OpenParenToken, typeArgumentList.OfKeyword, typeArgumentList.Arguments, ReportSyntaxError(typeArgumentList.CloseParenToken, ERRID.ERR_UnrecognizedType));
			genericName = SyntaxFactory.GenericName(genericName.Identifier, typeArgumentList);
			return genericName;
		}

		internal TypeSyntax ParseGeneralType(bool allowEmptyGenericArguments = false)
		{
			SyntaxToken currentToken = CurrentToken;
			TypeSyntax result;
			if (_evaluatingConditionCompilationExpression && !SyntaxFacts.IsPredefinedTypeOrVariant(currentToken.Kind))
			{
				IdentifierTokenSyntax node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				node = SyntaxNodeExtensions.AddTrailingSyntax(node, currentToken, ERRID.ERR_BadTypeInCCExpression);
				result = SyntaxFactory.IdentifierName(node);
				GetNextToken();
				return result;
			}
			bool allowedEmptyGenericArguments = false;
			result = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments, ref allowedEmptyGenericArguments);
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				TypeSyntax elementType = result;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> syntaxList = ParseArrayRankSpecifiers();
				if (allowedEmptyGenericArguments)
				{
					syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>(ReportSyntaxError(syntaxList.Node, ERRID.ERR_ArrayOfRawGenericInvalid));
				}
				result = SyntaxFactory.ArrayType(elementType, syntaxList);
			}
			return result;
		}

		private TypeArgumentListSyntax ParseGenericArguments(ref bool allowEmptyGenericArguments, ref bool AllowNonEmptyGenericArguments)
		{
			KeywordSyntax token = null;
			PunctuationSyntax token2 = null;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			TryEatNewLine();
			TryGetTokenAndEatNewLine(SyntaxKind.OfKeyword, ref token, createIfMissing: true);
			SeparatedSyntaxListBuilder<TypeSyntax> item = _pool.AllocateSeparated<TypeSyntax>();
			while (true)
			{
				TypeSyntax typeSyntax = null;
				if (CurrentToken.Kind == SyntaxKind.CommaToken || CurrentToken.Kind == SyntaxKind.CloseParenToken)
				{
					if (allowEmptyGenericArguments)
					{
						typeSyntax = SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
						AllowNonEmptyGenericArguments = false;
					}
					else
					{
						typeSyntax = ParseGeneralType();
					}
				}
				else
				{
					typeSyntax = ParseGeneralType();
					if (AllowNonEmptyGenericArguments)
					{
						allowEmptyGenericArguments = false;
					}
					else
					{
						typeSyntax = ReportSyntaxError(typeSyntax, ERRID.ERR_TypeParamMissingCommaOrRParen);
					}
				}
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax, SyntaxKind.CloseParenToken, SyntaxKind.CommaToken);
				}
				item.Add(typeSyntax);
				PunctuationSyntax token3 = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token3))
				{
					break;
				}
				item.AddSeparator(token3);
			}
			if (punctuationSyntax != null)
			{
				TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.TypeArgumentList(punctuationSyntax, token, separatedSyntaxList, token2);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> ParseArrayRankSpecifiers(ERRID errorForExplicitArraySizes = ERRID.ERR_NoExplicitArraySizes)
		{
			SyntaxListBuilder<ArrayRankSpecifierSyntax> syntaxListBuilder = default(SyntaxListBuilder<ArrayRankSpecifierSyntax>);
			do
			{
				PunctuationSyntax token = null;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax>);
				PunctuationSyntax token2 = null;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>);
				TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
				if (CurrentToken.Kind == SyntaxKind.CommaToken)
				{
					syntaxList = ParseSeparators(SyntaxKind.CommaToken);
				}
				else if (CurrentToken.Kind != SyntaxKind.CloseParenToken)
				{
					separatedSyntaxList = ParseArgumentList();
				}
				TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
				if (syntaxListBuilder.IsNull)
				{
					syntaxListBuilder = _pool.Allocate<ArrayRankSpecifierSyntax>();
				}
				if (separatedSyntaxList.Count != 0)
				{
					token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, separatedSyntaxList.Node, errorForExplicitArraySizes);
				}
				ArrayRankSpecifierSyntax node = SyntaxFactory.ArrayRankSpecifier(token, syntaxList, token2);
				syntaxListBuilder.Add(node);
			}
			while (CurrentToken.Kind == SyntaxKind.OpenParenToken);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private ModifiedIdentifierSyntax ParseArrayModifiedIdentifier(IdentifierTokenSyntax elementType, PunctuationSyntax optionalNullable, bool allowExplicitSizes)
		{
			ArgumentListSyntax argumentListSyntax = null;
			SyntaxListBuilder<ArrayRankSpecifierSyntax> syntaxListBuilder = default(SyntaxListBuilder<ArrayRankSpecifierSyntax>);
			PunctuationSyntax token = null;
			bool flag = false;
			do
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax>);
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>);
				PunctuationSyntax token2 = null;
				TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
				if (CurrentToken.Kind == SyntaxKind.CommaToken)
				{
					syntaxList = ParseSeparators(SyntaxKind.CommaToken);
				}
				else if (CurrentToken.Kind != SyntaxKind.CloseParenToken)
				{
					separatedSyntaxList = ParseArgumentList();
				}
				TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token2, createIfMissing: true);
				if (syntaxListBuilder.IsNull)
				{
					syntaxListBuilder = _pool.Allocate<ArrayRankSpecifierSyntax>();
				}
				if (separatedSyntaxList.Count != 0)
				{
					if (!flag)
					{
						argumentListSyntax = SyntaxFactory.ArgumentList(token, separatedSyntaxList, token2);
						if (!allowExplicitSizes)
						{
							argumentListSyntax = ReportSyntaxError(argumentListSyntax, ERRID.ERR_NoExplicitArraySizes);
						}
					}
					else
					{
						token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, separatedSyntaxList.Node, ERRID.ERR_NoConstituentArraySizes);
						syntaxListBuilder.Add(SyntaxFactory.ArrayRankSpecifier(token, syntaxList, token2));
					}
				}
				else
				{
					syntaxListBuilder.Add(SyntaxFactory.ArrayRankSpecifier(token, syntaxList, token2));
				}
				flag = true;
			}
			while (CurrentToken.Kind == SyntaxKind.OpenParenToken);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> syntaxList2 = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return SyntaxFactory.ModifiedIdentifier(elementType, optionalNullable, argumentListSyntax, syntaxList2);
		}

		private bool TryReinterpretAsArraySpecifier(ArgumentListSyntax argumentList, ref Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> arrayModifiers)
		{
			SyntaxListBuilder<PunctuationSyntax> syntaxListBuilder = _pool.Allocate<PunctuationSyntax>();
			bool flag = true;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
			int num = arguments.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (arguments[i]!.Kind != SyntaxKind.OmittedArgument)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> withSeparators = arguments.GetWithSeparators();
				int num2 = arguments.SeparatorCount - 1;
				for (int j = 0; j <= num2; j++)
				{
					syntaxListBuilder.Add((PunctuationSyntax)withSeparators[2 * j + 1]);
				}
				arrayModifiers = SyntaxFactory.ArrayRankSpecifier(argumentList.OpenParenToken, syntaxListBuilder.ToList(), argumentList.CloseParenToken);
			}
			_pool.Free(syntaxListBuilder);
			return flag;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> ParseSeparators(SyntaxKind kind)
		{
			SyntaxListBuilder<PunctuationSyntax> syntaxListBuilder = _pool.Allocate<PunctuationSyntax>();
			while (CurrentToken.Kind == kind)
			{
				PunctuationSyntax node = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				syntaxListBuilder.Add(node);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> ParseArgumentList()
		{
			SeparatedSyntaxListBuilder<ArgumentSyntax> item = _pool.AllocateSeparated<ArgumentSyntax>();
			while (true)
			{
				ExpressionSyntax expressionSyntax = null;
				KeywordSyntax keywordSyntax = null;
				ExpressionSyntax expressionSyntax2 = ParseExpressionCore();
				if (expressionSyntax2.ContainsDiagnostics)
				{
					expressionSyntax2 = ResyncAt(expressionSyntax2, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken, SyntaxKind.AsKeyword);
				}
				else if (CurrentToken.Kind == SyntaxKind.ToKeyword)
				{
					keywordSyntax = (KeywordSyntax)CurrentToken;
					expressionSyntax = expressionSyntax2;
					GetNextToken();
					expressionSyntax2 = ParseExpressionCore();
				}
				if (expressionSyntax2.ContainsDiagnostics || (keywordSyntax != null && expressionSyntax.ContainsDiagnostics))
				{
					expressionSyntax2 = ResyncAt(expressionSyntax2, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken, SyntaxKind.AsKeyword);
				}
				ArgumentSyntax node = ((keywordSyntax != null) ? ((ArgumentSyntax)SyntaxFactory.RangeArgument(expressionSyntax, keywordSyntax, expressionSyntax2)) : ((ArgumentSyntax)SyntaxFactory.SimpleArgument(null, expressionSyntax2)));
				item.Add(node);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private AccessorStatementSyntax ParsePropertyOrEventAccessor(SyntaxKind accessorKind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			if (!IsFirstStatementOnLine(CurrentToken))
			{
				keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_MethodMustBeFirstStatementOnLine);
			}
			GetNextToken();
			TypeParameterListSyntax genericParams = null;
			ParameterListSyntax parameterList = null;
			PunctuationSyntax openParen = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>);
			PunctuationSyntax closeParen = null;
			TryRejectGenericParametersForMemberDecl(ref genericParams);
			if (genericParams != null)
			{
				keywordSyntax = SyntaxNodeExtensions.AddTrailingSyntax(keywordSyntax, genericParams);
			}
			if (keywordSyntax.Kind != SyntaxKind.GetKeyword && CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
				parameterList = SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen);
			}
			if (modifiers.Any() && (keywordSyntax.Kind == SyntaxKind.AddHandlerKeyword || keywordSyntax.Kind == SyntaxKind.RemoveHandlerKeyword || keywordSyntax.Kind == SyntaxKind.RaiseEventKeyword))
			{
				keywordSyntax = ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvOnEventMethod, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), modifiers, keywordSyntax);
				modifiers = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>);
			}
			return SyntaxFactory.AccessorStatement(accessorKind, attributes, modifiers, keywordSyntax, parameterList);
		}

		private ImplementsClauseSyntax ParseImplementsList()
		{
			KeywordSyntax implementsKeyword = (KeywordSyntax)CurrentToken;
			SeparatedSyntaxListBuilder<QualifiedNameSyntax> item = _pool.AllocateSeparated<QualifiedNameSyntax>();
			GetNextToken();
			while (true)
			{
				bool allowedEmptyGenericArguments = false;
				QualifiedNameSyntax node = (QualifiedNameSyntax)ParseName(requireQualification: true, allowGlobalNameSpace: true, allowGenericArguments: true, allowGenericsWithoutOf: true, nonArrayName: true, disallowGenericArgumentsOnLastQualifiedName: true, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				item.Add(node);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<QualifiedNameSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.ImplementsClause(implementsKeyword, separatedSyntaxList);
		}

		private HandlesClauseSyntax ParseHandlesList()
		{
			KeywordSyntax handlesKeyword = (KeywordSyntax)CurrentToken;
			SeparatedSyntaxListBuilder<HandlesClauseItemSyntax> item = _pool.AllocateSeparated<HandlesClauseItemSyntax>();
			GetNextToken();
			while (true)
			{
				EventContainerSyntax eventContainerSyntax;
				if (CurrentToken.Kind == SyntaxKind.MyBaseKeyword || CurrentToken.Kind == SyntaxKind.MyClassKeyword || CurrentToken.Kind == SyntaxKind.MeKeyword)
				{
					eventContainerSyntax = SyntaxFactory.KeywordEventContainer((KeywordSyntax)CurrentToken);
					GetNextToken();
				}
				else if (CurrentToken.Kind == SyntaxKind.GlobalKeyword)
				{
					eventContainerSyntax = SyntaxFactory.WithEventsEventContainer(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
					eventContainerSyntax = ReportSyntaxError(eventContainerSyntax, ERRID.ERR_NoGlobalInHandles);
				}
				else
				{
					eventContainerSyntax = SyntaxFactory.WithEventsEventContainer(ParseIdentifier());
				}
				PunctuationSyntax token = null;
				IdentifierNameSyntax identifierNameSyntax;
				if (TryGetTokenAndEatNewLine(SyntaxKind.DotToken, ref token, createIfMissing: true))
				{
					identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(ParseIdentifierAllowingKeyword());
					WithEventsEventContainerSyntax withEventsEventContainerSyntax = eventContainerSyntax as WithEventsEventContainerSyntax;
					PunctuationSyntax token2 = null;
					if (withEventsEventContainerSyntax != null && TryGetTokenAndEatNewLine(SyntaxKind.DotToken, ref token2, createIfMissing: true))
					{
						eventContainerSyntax = SyntaxFactory.WithEventsPropertyEventContainer(withEventsEventContainerSyntax, token, identifierNameSyntax);
						token = token2;
						identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(ParseIdentifierAllowingKeyword());
					}
				}
				else
				{
					identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier());
				}
				HandlesClauseItemSyntax handlesClauseItemSyntax = SyntaxFactory.HandlesClauseItem(eventContainerSyntax, token, identifierNameSyntax);
				if ((eventContainerSyntax.ContainsDiagnostics || token.ContainsDiagnostics || identifierNameSyntax.ContainsDiagnostics) && CurrentToken.Kind != SyntaxKind.CommaToken)
				{
					handlesClauseItemSyntax = ResyncAt(handlesClauseItemSyntax, SyntaxKind.CommaToken);
				}
				item.Add(handlesClauseItemSyntax);
				PunctuationSyntax token3 = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token3))
				{
					break;
				}
				item.AddSeparator(token3);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<HandlesClauseItemSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.HandlesClause(handlesKeyword, separatedSyntaxList);
		}

		private MethodBaseSyntax ParseSubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			bool isInMethodDeclarationHeader = _isInMethodDeclarationHeader;
			_isInMethodDeclarationHeader = true;
			bool isInAsyncMethodDeclarationHeader = _isInAsyncMethodDeclarationHeader;
			bool isInIteratorMethodDeclarationHeader = _isInIteratorMethodDeclarationHeader;
			_isInAsyncMethodDeclarationHeader = modifiers.Any(630);
			_isInIteratorMethodDeclarationHeader = modifiers.Any(632);
			KeywordSyntax keywordSyntax2 = null;
			IdentifierTokenSyntax ident = null;
			TypeParameterListSyntax optionalGenericParams = null;
			ParameterListSyntax optionalParameters = null;
			HandlesClauseSyntax handlesClause = null;
			ImplementsClauseSyntax implementsClause = null;
			if (CurrentToken.Kind == SyntaxKind.NewKeyword)
			{
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
			}
			ParseSubOrDelegateStatement((keywordSyntax2 == null) ? SyntaxKind.SubStatement : SyntaxKind.SubNewStatement, ref ident, ref optionalGenericParams, ref optionalParameters, ref handlesClause, ref implementsClause);
			_isInMethodDeclarationHeader = isInMethodDeclarationHeader;
			_isInAsyncMethodDeclarationHeader = isInAsyncMethodDeclarationHeader;
			_isInIteratorMethodDeclarationHeader = isInIteratorMethodDeclarationHeader;
			if (keywordSyntax2 == null)
			{
				return SyntaxFactory.SubStatement(attributes, modifiers, keywordSyntax, ident, optionalGenericParams, optionalParameters, null, handlesClause, implementsClause);
			}
			if (handlesClause != null)
			{
				keywordSyntax2 = SyntaxNodeExtensions.AddError(keywordSyntax2, ERRID.ERR_NewCannotHandleEvents);
			}
			if (implementsClause != null)
			{
				keywordSyntax2 = SyntaxNodeExtensions.AddError(keywordSyntax2, ERRID.ERR_ImplementsOnNew);
			}
			if (optionalGenericParams != null)
			{
				keywordSyntax2 = SyntaxNodeExtensions.AddTrailingSyntax(keywordSyntax2, optionalGenericParams);
			}
			return SyntaxNodeExtensions.AddTrailingSyntax(SyntaxNodeExtensions.AddTrailingSyntax(SyntaxFactory.SubNewStatement(attributes, modifiers, keywordSyntax, keywordSyntax2, optionalParameters), handlesClause), implementsClause);
		}

		private void ParseSubOrDelegateStatement(SyntaxKind kind, ref IdentifierTokenSyntax ident, ref TypeParameterListSyntax optionalGenericParams, ref ParameterListSyntax optionalParameters, ref HandlesClauseSyntax handlesClause, ref ImplementsClauseSyntax implementsClause)
		{
			if (kind != SyntaxKind.SubNewStatement)
			{
				ident = ParseIdentifier();
				if (ident.ContainsDiagnostics)
				{
					ident = SyntaxNodeExtensions.AddTrailingSyntax(ident, ResyncAt(new SyntaxKind[2]
					{
						SyntaxKind.OpenParenToken,
						SyntaxKind.OfKeyword
					}));
				}
			}
			if (BeginsGeneric())
			{
				if (kind == SyntaxKind.SubNewStatement)
				{
					optionalGenericParams = ReportGenericParamsDisallowedError(ERRID.ERR_GenericParamsOnInvalidMember);
				}
				else
				{
					optionalGenericParams = ParseGenericParameters();
				}
			}
			optionalParameters = ParseParameterList();
			if (CurrentToken.Kind == SyntaxKind.HandlesKeyword)
			{
				handlesClause = ParseHandlesList();
				if (kind == SyntaxKind.DelegateSubStatement)
				{
					handlesClause = ReportSyntaxError(handlesClause, ERRID.ERR_DelegateCantHandleEvents);
				}
			}
			else if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				implementsClause = ParseImplementsList();
				if (kind == SyntaxKind.DelegateSubStatement)
				{
					implementsClause = ReportSyntaxError(implementsClause, ERRID.ERR_DelegateCantImplement);
				}
			}
		}

		internal ParameterListSyntax ParseParameterList()
		{
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				PunctuationSyntax openParen = null;
				PunctuationSyntax closeParen = null;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
				return SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen);
			}
			return null;
		}

		private MethodStatementSyntax ParseFunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax subOrFunctionKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			bool isInMethodDeclarationHeader = _isInMethodDeclarationHeader;
			_isInMethodDeclarationHeader = true;
			bool isInAsyncMethodDeclarationHeader = _isInAsyncMethodDeclarationHeader;
			bool isInIteratorMethodDeclarationHeader = _isInIteratorMethodDeclarationHeader;
			_isInAsyncMethodDeclarationHeader = modifiers.Any(630);
			_isInIteratorMethodDeclarationHeader = modifiers.Any(632);
			IdentifierTokenSyntax ident = null;
			TypeParameterListSyntax optionalGenericParams = null;
			ParameterListSyntax optionalParameters = null;
			SimpleAsClauseSyntax asClause = null;
			HandlesClauseSyntax handlesClause = null;
			ImplementsClauseSyntax implementsClause = null;
			ParseFunctionOrDelegateStatement(SyntaxKind.FunctionStatement, ref ident, ref optionalGenericParams, ref optionalParameters, ref asClause, ref handlesClause, ref implementsClause);
			_isInMethodDeclarationHeader = isInMethodDeclarationHeader;
			_isInAsyncMethodDeclarationHeader = isInAsyncMethodDeclarationHeader;
			_isInIteratorMethodDeclarationHeader = isInIteratorMethodDeclarationHeader;
			return SyntaxFactory.FunctionStatement(attributes, modifiers, subOrFunctionKeyword, ident, optionalGenericParams, optionalParameters, asClause, handlesClause, implementsClause);
		}

		private void ParseFunctionOrDelegateStatement(SyntaxKind kind, ref IdentifierTokenSyntax ident, ref TypeParameterListSyntax optionalGenericParams, ref ParameterListSyntax optionalParameters, ref SimpleAsClauseSyntax asClause, ref HandlesClauseSyntax handlesClause, ref ImplementsClauseSyntax implementsClause)
		{
			if (CurrentToken.Kind == SyntaxKind.NewKeyword)
			{
				ident = ParseIdentifierAllowingKeyword();
				ident = ReportSyntaxError(ident, ERRID.ERR_ConstructorFunction);
			}
			else
			{
				ident = ParseIdentifier();
				if (ident.ContainsDiagnostics)
				{
					ident = SyntaxNodeExtensions.AddTrailingSyntax(ident, ResyncAt(new SyntaxKind[2]
					{
						SyntaxKind.OpenParenToken,
						SyntaxKind.AsKeyword
					}));
				}
			}
			if (BeginsGeneric())
			{
				optionalGenericParams = ParseGenericParameters();
			}
			optionalParameters = ParseParameterList();
			TypeSyntax typeSyntax = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
			KeywordSyntax keywordSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				if (CurrentToken.Kind == SyntaxKind.LessThanToken)
				{
					syntaxList = ParseAttributeLists(allowFileLevelAttributes: false);
				}
				typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				asClause = SyntaxFactory.SimpleAsClause(keywordSyntax, syntaxList, typeSyntax);
			}
			if (CurrentToken.Kind == SyntaxKind.HandlesKeyword)
			{
				handlesClause = ParseHandlesList();
				if (kind == SyntaxKind.DelegateFunctionStatement)
				{
					handlesClause = ReportSyntaxError(handlesClause, ERRID.ERR_DelegateCantHandleEvents);
				}
			}
			else if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				implementsClause = ParseImplementsList();
				if (kind == SyntaxKind.DelegateFunctionStatement)
				{
					implementsClause = ReportSyntaxError(implementsClause, ERRID.ERR_DelegateCantImplement);
				}
			}
		}

		private OperatorStatementSyntax ParseOperatorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax operatorKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax k = null;
			SyntaxToken syntaxToken = ((!TryTokenAsContextualKeyword(CurrentToken, ref k)) ? CurrentToken : k);
			SyntaxKind kind = syntaxToken.Kind;
			if (SyntaxFacts.IsOperatorStatementOperatorToken(kind))
			{
				GetNextToken();
			}
			else
			{
				SyntaxToken syntaxToken2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.PlusToken);
				if (SyntaxFacts.IsOperator(kind))
				{
					syntaxToken = SyntaxNodeExtensions.AddTrailingSyntax(syntaxToken2, syntaxToken, ERRID.ERR_OperatorNotOverloadable);
					GetNextToken();
				}
				else if (kind != SyntaxKind.OpenParenToken && !IsValidStatementTerminator(syntaxToken))
				{
					syntaxToken = SyntaxNodeExtensions.AddTrailingSyntax(syntaxToken2, syntaxToken, ERRID.ERR_UnknownOperator);
					GetNextToken();
				}
				else
				{
					syntaxToken = ReportSyntaxError(syntaxToken2, ERRID.ERR_UnknownOperator);
				}
			}
			TypeParameterListSyntax genericParams = null;
			if (TryRejectGenericParametersForMemberDecl(ref genericParams))
			{
				syntaxToken = SyntaxNodeExtensions.AddTrailingSyntax(syntaxToken, genericParams);
			}
			ParameterListSyntax parameterList = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>);
			bool flag = false;
			PunctuationSyntax openParen = null;
			PunctuationSyntax closeParen = null;
			if (CurrentToken.Kind != SyntaxKind.OpenParenToken)
			{
				flag = true;
				syntaxToken = SyntaxNodeExtensions.AddTrailingSyntax(syntaxToken, ResyncAt(new SyntaxKind[2]
				{
					SyntaxKind.OpenParenToken,
					SyntaxKind.AsKeyword
				}));
			}
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
			}
			if (flag)
			{
				openParen = ((openParen != null) ? ReportSyntaxError(openParen, ERRID.ERR_ExpectedLparen) : ((PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.OpenParenToken)));
				if (closeParen == null)
				{
					closeParen = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.CloseParenToken);
				}
			}
			if (openParen != null)
			{
				parameterList = SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen);
			}
			TypeSyntax typeSyntax = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
			SimpleAsClauseSyntax asClause = null;
			KeywordSyntax keywordSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				if (CurrentToken.Kind == SyntaxKind.LessThanToken)
				{
					syntaxList = ParseAttributeLists(allowFileLevelAttributes: false);
				}
				typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				asClause = SyntaxFactory.SimpleAsClause(keywordSyntax, syntaxList, typeSyntax);
			}
			OperatorStatementSyntax operatorStatementSyntax = SyntaxFactory.OperatorStatement(attributes, modifiers, operatorKeyword, syntaxToken, parameterList, asClause);
			SyntaxToken syntaxToken3 = null;
			ERRID errorId = ERRID.ERR_None;
			if (CurrentToken.Kind == SyntaxKind.HandlesKeyword)
			{
				syntaxToken3 = CurrentToken;
				GetNextToken();
				errorId = ERRID.ERR_InvalidHandles;
			}
			else if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				syntaxToken3 = CurrentToken;
				GetNextToken();
				errorId = ERRID.ERR_InvalidImplements;
			}
			if (syntaxToken3 != null)
			{
				operatorStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(operatorStatementSyntax, syntaxToken3, errorId);
			}
			return operatorStatementSyntax;
		}

		private PropertyStatementSyntax ParsePropertyDefinition(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax propertyKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierTokenSyntax identifierTokenSyntax;
			if (CurrentToken.Kind == SyntaxKind.GetKeyword || CurrentToken.Kind == SyntaxKind.SetKeyword || CurrentToken.Kind == SyntaxKind.LetKeyword)
			{
				identifierTokenSyntax = ReportSyntaxError(ParseIdentifierAllowingKeyword(), ERRID.ERR_ObsoletePropertyGetLetSet);
				if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ParseIdentifier());
				}
			}
			else
			{
				identifierTokenSyntax = ParseIdentifier();
			}
			TypeParameterListSyntax genericParams = null;
			if (TryRejectGenericParametersForMemberDecl(ref genericParams))
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, genericParams);
			}
			PunctuationSyntax openParen = null;
			PunctuationSyntax closeParen = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>);
			ParameterListSyntax parameterList = null;
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
				parameterList = SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen);
			}
			else if (identifierTokenSyntax.ContainsDiagnostics)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = ResyncAt(new SyntaxKind[3]
				{
					SyntaxKind.AsKeyword,
					SyntaxKind.ImplementsKeyword,
					SyntaxKind.EqualsToken
				});
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, unexpected);
			}
			AsClauseSyntax optionalAsClause = null;
			EqualsValueSyntax optionalInitializer = null;
			ParseFieldOrPropertyAsClauseAndInitializer(isProperty: true, allowAsNewWith: false, ref optionalAsClause, ref optionalInitializer);
			ImplementsClauseSyntax implementsClause = null;
			if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				implementsClause = ParseImplementsList();
			}
			PropertyStatementSyntax propertyStatementSyntax = SyntaxFactory.PropertyStatement(attributes, modifiers, propertyKeyword, identifierTokenSyntax, parameterList, optionalAsClause, optionalInitializer, implementsClause);
			if (CurrentToken.Kind != SyntaxKind.EndOfFileToken)
			{
				SyntaxToken syntaxToken = PeekToken(1);
				if (syntaxToken.Kind != SyntaxKind.GetKeyword && syntaxToken.Kind != SyntaxKind.SetKeyword && Context.BlockKind != SyntaxKind.InterfaceBlock && !propertyStatementSyntax.Modifiers.Any(505))
				{
					PropertyStatementSyntax propertyStatementSyntax2 = propertyStatementSyntax;
					propertyStatementSyntax = CheckFeatureAvailability(Feature.AutoProperties, propertyStatementSyntax);
					if (propertyStatementSyntax == propertyStatementSyntax2 && propertyStatementSyntax.Modifiers.Any(538))
					{
						propertyStatementSyntax = CheckFeatureAvailability(Feature.ReadonlyAutoProperties, propertyStatementSyntax);
					}
				}
			}
			return propertyStatementSyntax;
		}

		private DelegateStatementSyntax ParseDelegateStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax delegateKeyword = (KeywordSyntax)CurrentToken;
			KeywordSyntax keywordSyntax = null;
			IdentifierTokenSyntax ident = null;
			TypeParameterListSyntax optionalGenericParams = null;
			ParameterListSyntax optionalParameters = null;
			SimpleAsClauseSyntax asClause = null;
			HandlesClauseSyntax handlesClause = null;
			ImplementsClauseSyntax implementsClause = null;
			GetNextToken();
			SyntaxKind kind;
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.SubKeyword:
				kind = SyntaxKind.DelegateSubStatement;
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				ParseSubOrDelegateStatement(SyntaxKind.DelegateSubStatement, ref ident, ref optionalGenericParams, ref optionalParameters, ref handlesClause, ref implementsClause);
				break;
			case SyntaxKind.FunctionKeyword:
				kind = SyntaxKind.DelegateFunctionStatement;
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				ParseFunctionOrDelegateStatement(SyntaxKind.DelegateFunctionStatement, ref ident, ref optionalGenericParams, ref optionalParameters, ref asClause, ref handlesClause, ref implementsClause);
				break;
			default:
				kind = SyntaxKind.DelegateSubStatement;
				keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword);
				keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_ExpectedSubOrFunction);
				ParseSubOrDelegateStatement(SyntaxKind.DelegateSubStatement, ref ident, ref optionalGenericParams, ref optionalParameters, ref handlesClause, ref implementsClause);
				break;
			}
			DelegateStatementSyntax delegateStatementSyntax = SyntaxFactory.DelegateStatement(kind, attributes, modifiers, delegateKeyword, keywordSyntax, ident, optionalGenericParams, optionalParameters, asClause);
			if (handlesClause != null)
			{
				delegateStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(delegateStatementSyntax, handlesClause);
			}
			if (implementsClause != null)
			{
				delegateStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(delegateStatementSyntax, implementsClause);
			}
			return delegateStatementSyntax;
		}

		private TypeParameterListSyntax ParseGenericParameters()
		{
			PunctuationSyntax token = null;
			KeywordSyntax token2 = null;
			PunctuationSyntax token3 = null;
			PunctuationSyntax punctuationSyntax = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
			TryGetTokenAndEatNewLine(SyntaxKind.OfKeyword, ref token2, createIfMissing: true);
			SeparatedSyntaxListBuilder<TypeParameterSyntax> item = _pool.AllocateSeparated<TypeParameterSyntax>();
			KeywordSyntax keywordSyntax;
			while (true)
			{
				IdentifierTokenSyntax identifierTokenSyntax = null;
				KeywordSyntax varianceKeyword = null;
				if (CurrentToken.Kind == SyntaxKind.InKeyword)
				{
					varianceKeyword = (KeywordSyntax)CurrentToken;
					varianceKeyword = CheckFeatureAvailability(Feature.CoContraVariance, varianceKeyword);
					GetNextToken();
				}
				else
				{
					KeywordSyntax k = null;
					if (TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.OutKeyword, ref k))
					{
						IdentifierTokenSyntax identifierTokenSyntax2 = (IdentifierTokenSyntax)CurrentToken;
						GetNextToken();
						TryEatNewLineIfFollowedBy(SyntaxKind.CloseParenToken);
						if (CurrentToken.Kind == SyntaxKind.CloseParenToken || CurrentToken.Kind == SyntaxKind.CommaToken || CurrentToken.Kind == SyntaxKind.AsKeyword)
						{
							identifierTokenSyntax = identifierTokenSyntax2;
							varianceKeyword = null;
						}
						else
						{
							k = CheckFeatureAvailability(Feature.CoContraVariance, k);
							varianceKeyword = k;
						}
					}
				}
				if (identifierTokenSyntax == null)
				{
					identifierTokenSyntax = ParseIdentifier();
				}
				TypeParameterConstraintClauseSyntax typeParameterConstraintClause = null;
				keywordSyntax = null;
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					keywordSyntax = (KeywordSyntax)CurrentToken;
					GetNextToken();
					PunctuationSyntax token4 = null;
					if (TryGetTokenAndEatNewLine(SyntaxKind.OpenBraceToken, ref token4))
					{
						SeparatedSyntaxListBuilder<ConstraintSyntax> item2 = _pool.AllocateSeparated<ConstraintSyntax>();
						while (true)
						{
							ConstraintSyntax constraintSyntax = ParseConstraintSyntax();
							if (constraintSyntax.ContainsDiagnostics)
							{
								constraintSyntax = ResyncAt(constraintSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken, SyntaxKind.CloseParenToken);
							}
							item2.Add(constraintSyntax);
							punctuationSyntax = null;
							if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref punctuationSyntax))
							{
								break;
							}
							item2.AddSeparator(punctuationSyntax);
						}
						PunctuationSyntax token5 = null;
						TryEatNewLineAndGetToken(SyntaxKind.CloseBraceToken, ref token5, createIfMissing: true);
						Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ConstraintSyntax> separatedSyntaxList = item2.ToList();
						_pool.Free(in item2);
						typeParameterConstraintClause = SyntaxFactory.TypeParameterMultipleConstraintClause(keywordSyntax, token4, separatedSyntaxList, token5);
					}
					else
					{
						ConstraintSyntax constraintSyntax2 = ParseConstraintSyntax();
						if (constraintSyntax2.ContainsDiagnostics)
						{
							constraintSyntax2 = ResyncAt(constraintSyntax2, SyntaxKind.CloseParenToken);
						}
						typeParameterConstraintClause = SyntaxFactory.TypeParameterSingleConstraintClause(keywordSyntax, constraintSyntax2);
					}
				}
				TypeParameterSyntax node = SyntaxFactory.TypeParameter(varianceKeyword, identifierTokenSyntax, typeParameterConstraintClause);
				item.Add(node);
				punctuationSyntax = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref punctuationSyntax))
				{
					break;
				}
				item.AddSeparator(punctuationSyntax);
			}
			if (token != null && !TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3))
			{
				token3 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken);
				token3 = ReportSyntaxError(token3, (keywordSyntax == null) ? ERRID.ERR_TypeParamMissingAsCommaOrRParen : ERRID.ERR_TypeParamMissingCommaOrRParen);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax> separatedSyntaxList2 = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.TypeParameterList(token, token2, separatedSyntaxList2, token3);
		}

		private ConstraintSyntax ParseConstraintSyntax()
		{
			ConstraintSyntax constraintSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.NewKeyword)
			{
				KeywordSyntax constraintKeyword = (KeywordSyntax)CurrentToken;
				constraintSyntax = SyntaxFactory.NewConstraint(constraintKeyword);
				GetNextToken();
			}
			else if (CurrentToken.Kind == SyntaxKind.ClassKeyword)
			{
				KeywordSyntax constraintKeyword = (KeywordSyntax)CurrentToken;
				constraintSyntax = SyntaxFactory.ClassConstraint(constraintKeyword);
				GetNextToken();
			}
			else if (CurrentToken.Kind == SyntaxKind.StructureKeyword)
			{
				KeywordSyntax constraintKeyword = (KeywordSyntax)CurrentToken;
				constraintSyntax = SyntaxFactory.StructureConstraint(constraintKeyword);
				GetNextToken();
			}
			else
			{
				DiagnosticInfo diagnosticInfo = null;
				if (!CanTokenStartTypeName(CurrentToken))
				{
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_BadConstraintSyntax);
				}
				TypeSyntax typeSyntax = ParseGeneralType();
				if (diagnosticInfo != null)
				{
					typeSyntax = (TypeSyntax)typeSyntax.AddError(diagnosticInfo);
				}
				constraintSyntax = SyntaxFactory.TypeConstraint(typeSyntax);
			}
			return constraintSyntax;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> ParseParameters(ref PunctuationSyntax openParen, ref PunctuationSyntax closeParen)
		{
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref openParen);
			SeparatedSyntaxListBuilder<ParameterSyntax> item = _pool.AllocateSeparated<ParameterSyntax>();
			if (CurrentToken.Kind != SyntaxKind.CloseParenToken)
			{
				while (true)
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
					if (CurrentToken.Kind == SyntaxKind.LessThanToken)
					{
						attributes = ParseAttributeLists(allowFileLevelAttributes: false);
					}
					ParameterSpecifiers specifiers = (ParameterSpecifiers)0;
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = ParseParameterSpecifiers(ref specifiers);
					ParameterSyntax parameterSyntax = ParseParameter(attributes, modifiers);
					if (parameterSyntax.ContainsDiagnostics)
					{
						parameterSyntax = SyntaxNodeExtensions.AddTrailingSyntax(parameterSyntax, ResyncAt(new SyntaxKind[2]
						{
							SyntaxKind.CommaToken,
							SyntaxKind.CloseParenToken
						}));
					}
					PunctuationSyntax token = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
					{
						if (CurrentToken.Kind == SyntaxKind.CloseParenToken || MustEndStatement(CurrentToken))
						{
							item.Add(parameterSyntax);
							break;
						}
						if (IsContinuableEOL() && PeekToken(1).Kind == SyntaxKind.CloseParenToken)
						{
							item.Add(parameterSyntax);
							break;
						}
						parameterSyntax = SyntaxNodeExtensions.AddTrailingSyntax(parameterSyntax, ResyncAt(new SyntaxKind[2]
						{
							SyntaxKind.CommaToken,
							SyntaxKind.CloseParenToken
						}), ERRID.ERR_InvalidParameterSyntax);
						if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
						{
							item.Add(parameterSyntax);
							break;
						}
					}
					item.Add(parameterSyntax);
					item.AddSeparator(token);
				}
			}
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref closeParen, createIfMissing: true);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> result = item.ToList();
			_pool.Free(in item);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> ParseParameterSpecifiers(ref ParameterSpecifiers specifiers)
		{
			SyntaxListBuilder<KeywordSyntax> syntaxListBuilder = _pool.Allocate<KeywordSyntax>();
			specifiers = (ParameterSpecifiers)0;
			while (true)
			{
				KeywordSyntax keywordSyntax;
				ParameterSpecifiers parameterSpecifiers;
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.ByValKeyword:
					keywordSyntax = (KeywordSyntax)CurrentToken;
					if ((specifiers & ParameterSpecifiers.ByRef) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_MultipleParameterSpecifiers);
					}
					parameterSpecifiers = ParameterSpecifiers.ByVal;
					break;
				case SyntaxKind.ByRefKeyword:
					keywordSyntax = (KeywordSyntax)CurrentToken;
					if ((specifiers & ParameterSpecifiers.ByVal) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_MultipleParameterSpecifiers);
					}
					else if ((specifiers & ParameterSpecifiers.ParamArray) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_ParamArrayMustBeByVal);
					}
					parameterSpecifiers = ParameterSpecifiers.ByRef;
					break;
				case SyntaxKind.OptionalKeyword:
					keywordSyntax = (KeywordSyntax)CurrentToken;
					if ((specifiers & ParameterSpecifiers.ParamArray) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_MultipleOptionalParameterSpecifiers);
					}
					parameterSpecifiers = ParameterSpecifiers.Optional;
					break;
				case SyntaxKind.ParamArrayKeyword:
					keywordSyntax = (KeywordSyntax)CurrentToken;
					if ((specifiers & ParameterSpecifiers.Optional) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_MultipleOptionalParameterSpecifiers);
					}
					else if ((specifiers & ParameterSpecifiers.ByRef) != 0)
					{
						keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_ParamArrayMustBeByVal);
					}
					parameterSpecifiers = ParameterSpecifiers.ParamArray;
					break;
				default:
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> result = syntaxListBuilder.ToList();
					_pool.Free(syntaxListBuilder);
					return result;
				}
				}
				if ((specifiers & parameterSpecifiers) != 0)
				{
					keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_DuplicateParameterSpecifier);
				}
				else
				{
					specifiers |= parameterSpecifiers;
				}
				syntaxListBuilder.Add(keywordSyntax);
				GetNextToken();
			}
		}

		private ParameterSyntax ParseParameter(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseModifiedIdentifier(AllowExplicitArraySizes: false, checkForCustom: false);
			if (modifiedIdentifierSyntax.ContainsDiagnostics && PeekAheadFor(SyntaxKind.AsKeyword, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken) == SyntaxKind.AsKeyword)
			{
				modifiedIdentifierSyntax = ResyncAt(modifiedIdentifierSyntax, SyntaxKind.AsKeyword);
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = null;
			KeywordSyntax token = null;
			if (TryGetToken(SyntaxKind.AsKeyword, ref token))
			{
				TypeSyntax type = ParseGeneralType();
				simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(token, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), type);
				if (simpleAsClauseSyntax.ContainsDiagnostics)
				{
					simpleAsClauseSyntax = ResyncAt(simpleAsClauseSyntax, SyntaxKind.EqualsToken, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
				}
			}
			PunctuationSyntax token2 = null;
			ExpressionSyntax expressionSyntax = null;
			if (TryGetTokenAndEatNewLine(SyntaxKind.EqualsToken, ref token2))
			{
				if (!modifiers.Any() || !modifiers.Any(523))
				{
					token2 = ReportSyntaxError(token2, ERRID.ERR_DefaultValueForNonOptionalParam);
				}
				expressionSyntax = ParseExpressionCore();
			}
			else if (modifiers.Any() && modifiers.Any(523))
			{
				token2 = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), ERRID.ERR_ObsoleteOptionalWithoutValue);
				expressionSyntax = ParseExpressionCore();
			}
			EqualsValueSyntax @default = null;
			if (expressionSyntax != null)
			{
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
				}
				@default = SyntaxFactory.EqualsValue(token2, expressionSyntax);
			}
			return SyntaxFactory.Parameter(attributes, modifiers, modifiedIdentifierSyntax, simpleAsClauseSyntax, @default);
		}

		private ImportsStatementSyntax ParseImportsStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> Attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Specifiers)
		{
			KeywordSyntax importsKeyword = ReportModifiersOnStatementError(Attributes, Specifiers, (KeywordSyntax)CurrentToken);
			SeparatedSyntaxListBuilder<ImportsClauseSyntax> item = _pool.AllocateSeparated<ImportsClauseSyntax>();
			GetNextToken();
			while (true)
			{
				ImportsClauseSyntax node = ParseOneImportsDirective();
				item.Add(node);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ImportsClauseSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.ImportsStatement(importsKeyword, separatedSyntaxList);
		}

		private ImportsClauseSyntax ParseOneImportsDirective()
		{
			ImportsClauseSyntax importsClauseSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.LessThanToken)
			{
				ResetCurrentToken(ScannerState.Element);
				PunctuationSyntax token = null;
				XmlAttributeSyntax xmlAttributeSyntax;
				if (VerifyExpectedToken(SyntaxKind.LessThanToken, ref token, ScannerState.Element))
				{
					xmlAttributeSyntax = ((CurrentToken.Kind != SyntaxKind.XmlNameToken || EmbeddedOperators.CompareString(CurrentToken.ToFullString(), "xmlns", TextCompare: false) != 0 || token.HasTrailingTrivia) ? ReportSyntaxError(CreateMissingXmlAttribute(), ERRID.ERR_ExpectedXmlns) : ((XmlAttributeSyntax)ParseXmlAttribute(requireLeadingWhitespace: false, AllowNameAsExpression: false, null)));
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = ResyncAt(ScannerState.Element, new SyntaxKind[1] { SyntaxKind.GreaterThanToken });
					if (unexpected.Any())
					{
						xmlAttributeSyntax = SyntaxNodeExtensions.AddTrailingSyntax(xmlAttributeSyntax, unexpected, ERRID.ERR_ExpectedGreater);
					}
				}
				else
				{
					xmlAttributeSyntax = CreateMissingXmlAttribute();
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected2 = ResyncAt(ScannerState.Element, new SyntaxKind[1] { SyntaxKind.GreaterThanToken });
					xmlAttributeSyntax = SyntaxNodeExtensions.AddTrailingSyntax(xmlAttributeSyntax, unexpected2);
				}
				PunctuationSyntax token2 = null;
				VerifyExpectedToken(SyntaxKind.GreaterThanToken, ref token2, ScannerState.Element);
				importsClauseSyntax = SyntaxFactory.XmlNamespaceImportsClause(token, xmlAttributeSyntax, token2);
				importsClauseSyntax = AdjustTriviaForMissingTokens(importsClauseSyntax);
				importsClauseSyntax = TransitionFromXmlToVB(importsClauseSyntax);
			}
			else if ((CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.EqualsToken) || CurrentToken.Kind == SyntaxKind.EqualsToken)
			{
				IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
				if (identifierTokenSyntax.TypeCharacter != 0)
				{
					identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_NoTypecharInAlias);
				}
				PunctuationSyntax equalsToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				bool allowedEmptyGenericArguments = false;
				NameSyntax name = ParseName(requireQualification: false, allowGlobalNameSpace: false, allowGenericArguments: true, allowGenericsWithoutOf: true, nonArrayName: false, disallowGenericArgumentsOnLastQualifiedName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				importsClauseSyntax = SyntaxFactory.SimpleImportsClause(SyntaxFactory.ImportAliasClause(identifierTokenSyntax, equalsToken), name);
			}
			else
			{
				bool allowedEmptyGenericArguments = false;
				NameSyntax name2 = ParseName(requireQualification: false, allowGlobalNameSpace: false, allowGenericArguments: true, allowGenericsWithoutOf: true, nonArrayName: false, disallowGenericArgumentsOnLastQualifiedName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				importsClauseSyntax = SyntaxFactory.SimpleImportsClause(null, name2);
			}
			if (importsClauseSyntax.ContainsDiagnostics && CurrentToken.Kind != SyntaxKind.CommaToken)
			{
				importsClauseSyntax = SyntaxNodeExtensions.AddTrailingSyntax(importsClauseSyntax, ResyncAt(new SyntaxKind[1] { SyntaxKind.CommaToken }));
			}
			return importsClauseSyntax;
		}

		private XmlStringSyntax CreateMissingXmlString()
		{
			PunctuationSyntax punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken);
			return SyntaxFactory.XmlString(punctuationSyntax, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), punctuationSyntax);
		}

		private XmlAttributeSyntax CreateMissingXmlAttribute()
		{
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken);
			PunctuationSyntax colonToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken);
			PunctuationSyntax equalsToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken);
			return SyntaxFactory.XmlAttribute(SyntaxFactory.XmlName(SyntaxFactory.XmlPrefix(xmlNameTokenSyntax, colonToken), xmlNameTokenSyntax), equalsToken, CreateMissingXmlString());
		}

		private InheritsOrImplementsStatementSyntax ParseInheritsImplementsStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> Attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Specifiers)
		{
			KeywordSyntax keywordSyntax = ReportModifiersOnStatementError(Attributes, Specifiers, (KeywordSyntax)CurrentToken);
			SeparatedSyntaxListBuilder<TypeSyntax> item = _pool.AllocateSeparated<TypeSyntax>();
			GetNextToken();
			while (true)
			{
				bool allowedEmptyGenericArguments = false;
				TypeSyntax typeSyntax = ParseTypeName(nonArrayName: true, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax, SyntaxKind.CommaToken);
				}
				item.Add(typeSyntax);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			InheritsOrImplementsStatementSyntax inheritsOrImplementsStatementSyntax = null;
			return keywordSyntax.Kind switch
			{
				SyntaxKind.InheritsKeyword => SyntaxFactory.InheritsStatement(keywordSyntax, separatedSyntaxList), 
				SyntaxKind.ImplementsKeyword => SyntaxFactory.ImplementsStatement(keywordSyntax, separatedSyntaxList), 
				_ => throw ExceptionUtilities.UnexpectedValue(keywordSyntax.Kind), 
			};
		}

		private StatementSyntax ParseOptionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> Attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Specifiers)
		{
			ERRID eRRID = ERRID.ERR_None;
			KeywordSyntax k = null;
			KeywordSyntax k2 = null;
			KeywordSyntax optionKeyword = ReportModifiersOnStatementError(Attributes, Specifiers, (KeywordSyntax)CurrentToken);
			GetNextToken();
			if (TryTokenAsContextualKeyword(CurrentToken, ref k))
			{
				switch (k.Kind)
				{
				case SyntaxKind.CompareKeyword:
					GetNextToken();
					if (TryTokenAsContextualKeyword(CurrentToken, ref k2))
					{
						if (k2.Kind == SyntaxKind.TextKeyword)
						{
							GetNextToken();
							break;
						}
						if (k2.Kind == SyntaxKind.BinaryKeyword)
						{
							GetNextToken();
							break;
						}
						k2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.BinaryKeyword);
						eRRID = ERRID.ERR_InvalidOptionCompare;
					}
					else
					{
						k2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.BinaryKeyword);
						eRRID = ERRID.ERR_InvalidOptionCompare;
					}
					break;
				case SyntaxKind.ExplicitKeyword:
				case SyntaxKind.InferKeyword:
				case SyntaxKind.StrictKeyword:
					GetNextToken();
					if (CurrentToken.Kind == SyntaxKind.OnKeyword)
					{
						k2 = (KeywordSyntax)CurrentToken;
						GetNextToken();
					}
					else if (TryTokenAsContextualKeyword(CurrentToken, ref k2) && k2.Kind == SyntaxKind.OffKeyword)
					{
						GetNextToken();
					}
					else if (!IsValidStatementTerminator(CurrentToken))
					{
						eRRID = ((k.Kind == SyntaxKind.StrictKeyword) ? ((k2 == null || k2.Kind != SyntaxKind.CustomKeyword) ? ERRID.ERR_InvalidOptionStrict : ERRID.ERR_InvalidOptionStrictCustom) : ((k.Kind != SyntaxKind.ExplicitKeyword) ? ERRID.ERR_InvalidOptionInfer : ERRID.ERR_InvalidOptionExplicit));
						k2 = null;
					}
					break;
				case SyntaxKind.BinaryKeyword:
				case SyntaxKind.TextKeyword:
					k = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.CompareKeyword);
					eRRID = ERRID.ERR_ExpectedOptionCompare;
					break;
				default:
					k = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StrictKeyword);
					eRRID = ERRID.ERR_ExpectedForOptionStmt;
					break;
				}
			}
			else
			{
				k = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StrictKeyword);
				eRRID = ERRID.ERR_ExpectedForOptionStmt;
			}
			OptionStatementSyntax optionStatementSyntax = SyntaxFactory.OptionStatement(optionKeyword, k, k2);
			if (eRRID != 0)
			{
				optionStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(optionStatementSyntax, ResyncAt(), eRRID);
			}
			return optionStatementSyntax;
		}

		private DeclareStatementSyntax ParseProcDeclareStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax declareKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax k = null;
			KeywordSyntax charsetKeyword = null;
			if (TryTokenAsContextualKeyword(CurrentToken, ref k))
			{
				SyntaxKind kind = k.Kind;
				if (kind == SyntaxKind.AnsiKeyword || kind == SyntaxKind.AutoKeyword || kind == SyntaxKind.UnicodeKeyword)
				{
					charsetKeyword = k;
					GetNextToken();
				}
			}
			KeywordSyntax keywordSyntax;
			SyntaxKind kind2;
			if (CurrentToken.Kind == SyntaxKind.SubKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				kind2 = SyntaxKind.DeclareSubStatement;
			}
			else if (CurrentToken.Kind == SyntaxKind.FunctionKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				kind2 = SyntaxKind.DeclareFunctionStatement;
			}
			else
			{
				keywordSyntax = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword), ERRID.ERR_ExpectedSubFunction);
				kind2 = SyntaxKind.DeclareSubStatement;
			}
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (identifierTokenSyntax.ContainsDiagnostics)
			{
				identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ResyncAt(new SyntaxKind[2]
				{
					SyntaxKind.LibKeyword,
					SyntaxKind.OpenParenToken
				}));
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			bool flag = false;
			if (CurrentToken.Kind != SyntaxKind.LibKeyword)
			{
				if (PeekAheadFor(SyntaxKind.LibKeyword) == SyntaxKind.LibKeyword)
				{
					syntaxList = ResyncAt(new SyntaxKind[1] { SyntaxKind.LibKeyword });
				}
				else
				{
					syntaxList = ResyncAt(new SyntaxKind[2]
					{
						SyntaxKind.AliasKeyword,
						SyntaxKind.OpenParenToken
					});
					flag = true;
				}
			}
			KeywordSyntax libKeyword = null;
			LiteralExpressionSyntax libraryName = null;
			KeywordSyntax optionalAliasKeyword = null;
			LiteralExpressionSyntax optionalAliasName = null;
			ParseDeclareLibClause(ref libKeyword, ref libraryName, ref optionalAliasKeyword, ref optionalAliasName);
			if (syntaxList.Node != null)
			{
				libKeyword = ((!flag) ? SyntaxNodeExtensions.AddLeadingSyntax(libKeyword, syntaxList, ERRID.ERR_MissingLibInDeclare) : SyntaxNodeExtensions.AddLeadingSyntax(libKeyword, syntaxList));
			}
			TypeParameterListSyntax genericParams = null;
			if (TryRejectGenericParametersForMemberDecl(ref genericParams))
			{
				if (optionalAliasName != null)
				{
					optionalAliasName = SyntaxNodeExtensions.AddTrailingSyntax(optionalAliasName, genericParams);
				}
				else
				{
					libraryName = SyntaxNodeExtensions.AddTrailingSyntax(libraryName, genericParams);
				}
			}
			ParameterListSyntax parameterListSyntax = null;
			parameterListSyntax = ParseParameterList();
			SimpleAsClauseSyntax asClause = null;
			if (keywordSyntax.Kind == SyntaxKind.FunctionKeyword && CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList2 = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
				if (CurrentToken.Kind == SyntaxKind.LessThanToken)
				{
					syntaxList2 = ParseAttributeLists(allowFileLevelAttributes: false);
				}
				TypeSyntax typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				asClause = SyntaxFactory.SimpleAsClause(asKeyword, syntaxList2, typeSyntax);
			}
			return SyntaxFactory.DeclareStatement(kind2, attributes, modifiers, declareKeyword, charsetKeyword, keywordSyntax, identifierTokenSyntax, libKeyword, libraryName, optionalAliasKeyword, optionalAliasName, parameterListSyntax, asClause);
		}

		private void ParseDeclareLibClause(ref KeywordSyntax libKeyword, ref LiteralExpressionSyntax libraryName, ref KeywordSyntax optionalAliasKeyword, ref LiteralExpressionSyntax optionalAliasName)
		{
			libKeyword = null;
			optionalAliasKeyword = null;
			if (VerifyExpectedToken(SyntaxKind.LibKeyword, ref libKeyword))
			{
				libraryName = ParseStringLiteral();
				if (libraryName.ContainsDiagnostics)
				{
					libraryName = ResyncAt(libraryName, SyntaxKind.AliasKeyword, SyntaxKind.OpenParenToken);
				}
			}
			else
			{
				libraryName = SyntaxFactory.StringLiteralExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingStringLiteral());
			}
			if (CurrentToken.Kind == SyntaxKind.AliasKeyword)
			{
				optionalAliasKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
				optionalAliasName = ParseStringLiteral();
				if (optionalAliasName.ContainsDiagnostics)
				{
					optionalAliasName = ResyncAt(optionalAliasName, SyntaxKind.OpenParenToken);
				}
			}
		}

		private StatementSyntax ParseCustomEventDefinition(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax keywordSyntax = null;
			if (PeekToken(1).Kind != SyntaxKind.EventKeyword)
			{
				return ParseVarDeclStatement(attributes, modifiers);
			}
			keywordSyntax = _scanner.MakeKeyword((IdentifierTokenSyntax)CurrentToken);
			GetNextToken();
			KeywordSyntax eventKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			KeywordSyntax keywordSyntax2 = null;
			TypeSyntax typeSyntax = null;
			SimpleAsClauseSyntax simpleAsClauseSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(keywordSyntax2, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
			}
			else
			{
				TypeParameterListSyntax genericParams = null;
				if (TryRejectGenericParametersForMemberDecl(ref genericParams))
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, genericParams);
				}
				if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
				{
					PunctuationSyntax openParen = null;
					PunctuationSyntax closeParen = null;
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen));
				}
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					keywordSyntax2 = (KeywordSyntax)CurrentToken;
					keywordSyntax2 = ReportSyntaxError(keywordSyntax2, ERRID.ERR_EventsCantBeFunctions);
					GetNextToken();
					keywordSyntax2 = SyntaxNodeExtensions.AddTrailingSyntax(keywordSyntax2, ResyncAt(new SyntaxKind[1] { SyntaxKind.ImplementsKeyword }));
				}
				else
				{
					keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.AsKeyword);
				}
				simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(keywordSyntax2, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()));
			}
			ImplementsClauseSyntax implementsClause = null;
			if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				implementsClause = ParseImplementsList();
			}
			return SyntaxFactory.EventStatement(attributes, modifiers, keywordSyntax, eventKeyword, identifierTokenSyntax, null, simpleAsClauseSyntax, implementsClause);
		}

		private EventStatementSyntax ParseEventDefinition(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers)
		{
			KeywordSyntax eventKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			ParameterListSyntax parameterList = null;
			PunctuationSyntax openParen = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>);
			PunctuationSyntax closeParen = null;
			KeywordSyntax keywordSyntax = null;
			TypeSyntax typeSyntax = null;
			SimpleAsClauseSyntax asClause = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				typeSyntax = ParseGeneralType();
				if (typeSyntax.ContainsDiagnostics)
				{
					typeSyntax = ResyncAt(typeSyntax);
				}
				asClause = SyntaxFactory.SimpleAsClause(keywordSyntax, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
			}
			else
			{
				TypeParameterListSyntax genericParams = null;
				if (TryRejectGenericParametersForMemberDecl(ref genericParams))
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, genericParams);
				}
				if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
				{
					separatedSyntaxList = ParseParameters(ref openParen, ref closeParen);
				}
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					if (closeParen != null)
					{
						closeParen = SyntaxNodeExtensions.AddTrailingSyntax(closeParen, ResyncAt(new SyntaxKind[1] { SyntaxKind.ImplementsKeyword }), ERRID.ERR_EventsCantBeFunctions);
					}
					else
					{
						identifierTokenSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierTokenSyntax, ResyncAt(new SyntaxKind[1] { SyntaxKind.ImplementsKeyword }), ERRID.ERR_EventsCantBeFunctions);
					}
				}
			}
			if (openParen != null)
			{
				parameterList = SyntaxFactory.ParameterList(openParen, separatedSyntaxList, closeParen);
			}
			ImplementsClauseSyntax implementsClause = null;
			if (CurrentToken.Kind == SyntaxKind.ImplementsKeyword)
			{
				implementsClause = ParseImplementsList();
			}
			return SyntaxFactory.EventStatement(attributes, modifiers, null, eventKeyword, identifierTokenSyntax, parameterList, asClause, implementsClause);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> ParseEmptyAttributeLists()
		{
			SyntaxToken currentToken = CurrentToken;
			string text = currentToken.Text;
			int length = currentToken.Text.Length;
			string spelling = text.Substring(0, 1);
			string spelling2 = text.Substring(length - 1, 1);
			SyntaxTrivia syntaxTrivia = ((length > 2) ? _scanner.MakeWhiteSpaceTrivia(text.Substring(1, length - 2)) : null);
			PunctuationSyntax lessThanToken = _scanner.MakePunctuationToken(SyntaxKind.LessThanToken, spelling, currentToken.GetLeadingTrivia(), syntaxTrivia);
			PunctuationSyntax greaterThanToken = _scanner.MakePunctuationToken(SyntaxKind.GreaterThanToken, spelling2, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), currentToken.GetTrailingTrivia());
			GetNextToken();
			SyntaxListBuilder<AttributeListSyntax> syntaxListBuilder = _pool.Allocate<AttributeListSyntax>();
			SeparatedSyntaxListBuilder<AttributeSyntax> item = _pool.AllocateSeparated<AttributeSyntax>();
			IdentifierNameSyntax name = SyntaxFactory.IdentifierName(ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), ERRID.ERR_ExpectedIdentifier));
			AttributeSyntax node = SyntaxFactory.Attribute(null, name, null);
			item.Add(node);
			syntaxListBuilder.Add(SyntaxFactory.AttributeList(lessThanToken, item.ToList(), greaterThanToken));
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(in item);
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> ParseAttributeLists(bool allowFileLevelAttributes)
		{
			SyntaxListBuilder<AttributeListSyntax> syntaxListBuilder = _pool.Allocate<AttributeListSyntax>();
			SeparatedSyntaxListBuilder<AttributeSyntax> item = _pool.AllocateSeparated<AttributeSyntax>();
			do
			{
				PunctuationSyntax token = null;
				TryGetTokenAndEatNewLine(SyntaxKind.LessThanToken, ref token);
				while (true)
				{
					AttributeTargetSyntax target = null;
					ArgumentListSyntax argumentList = null;
					if (allowFileLevelAttributes)
					{
						KeywordSyntax keywordSyntax = GetTokenAsAssemblyOrModuleKeyword(CurrentToken);
						PunctuationSyntax colonToken;
						if (keywordSyntax == null)
						{
							keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.AssemblyKeyword);
							keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_FileAttributeNotAssemblyOrModule);
							colonToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken);
						}
						else
						{
							GetNextToken();
							if (CurrentToken.Kind == SyntaxKind.ColonToken)
							{
								SyntaxToken prevToken = null;
								SyntaxToken currentToken = null;
								RescanTrailingColonAsToken(ref prevToken, ref currentToken);
								GetNextToken();
								keywordSyntax = GetTokenAsAssemblyOrModuleKeyword(prevToken);
								colonToken = (PunctuationSyntax)currentToken;
							}
							else
							{
								colonToken = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.ColonToken);
							}
						}
						target = SyntaxFactory.AttributeTarget(keywordSyntax, colonToken);
					}
					ResetCurrentToken(ScannerState.VB);
					bool allowedEmptyGenericArguments = false;
					NameSyntax nameSyntax = ParseName(requireQualification: false, allowGlobalNameSpace: true, allowGenericArguments: false, allowGenericsWithoutOf: true, nonArrayName: false, disallowGenericArgumentsOnLastQualifiedName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
					if (BeginsGeneric())
					{
						nameSyntax = ReportSyntaxError(nameSyntax, ERRID.ERR_GenericArgsOnAttributeSpecifier);
						nameSyntax = ResyncAt(nameSyntax, SyntaxKind.GreaterThanToken);
					}
					else if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
					{
						argumentList = ParseParenthesizedArguments(RedimOrNewParent: false, attributeListParent: true);
					}
					AttributeSyntax node = SyntaxFactory.Attribute(target, nameSyntax, argumentList);
					item.Add(node);
					PunctuationSyntax token2 = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
					{
						break;
					}
					item.AddSeparator(token2);
				}
				ResetCurrentToken(ScannerState.VB);
				PunctuationSyntax token3 = null;
				if (TryEatNewLineAndGetToken(SyntaxKind.GreaterThanToken, ref token3, createIfMissing: true) && !allowFileLevelAttributes && IsContinuableEOL())
				{
					TryEatNewLine();
				}
				syntaxListBuilder.Add(SyntaxFactory.AttributeList(token, item.ToList(), token3));
				item.Clear();
			}
			while (CurrentToken.Kind == SyntaxKind.LessThanToken);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(in item);
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private KeywordSyntax GetTokenAsAssemblyOrModuleKeyword(SyntaxToken token)
		{
			if (token.Kind == SyntaxKind.ModuleKeyword)
			{
				return (KeywordSyntax)token;
			}
			KeywordSyntax k = null;
			TryTokenAsContextualKeyword(token, SyntaxKind.AssemblyKeyword, ref k);
			return k;
		}

		internal static SyntaxKind GetBinaryOperatorHelper(SyntaxToken t)
		{
			return SyntaxFacts.GetBinaryExpression(t.Kind);
		}

		private static bool StartsValidConditionalCompilationExpr(SyntaxToken t)
		{
			switch (t.Kind)
			{
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.FalseKeyword:
			case SyntaxKind.IfKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.NothingKeyword:
			case SyntaxKind.TrueKeyword:
			case SyntaxKind.TryCastKeyword:
			case SyntaxKind.OpenParenToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.StatementTerminatorToken:
			case SyntaxKind.IdentifierToken:
			case SyntaxKind.IntegerLiteralToken:
			case SyntaxKind.FloatingLiteralToken:
			case SyntaxKind.DecimalLiteralToken:
			case SyntaxKind.DateLiteralToken:
			case SyntaxKind.StringLiteralToken:
			case SyntaxKind.CharacterLiteralToken:
				return true;
			default:
				return false;
			}
		}

		private static bool IsValidOperatorForConditionalCompilationExpr(SyntaxToken t)
		{
			switch (t.Kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				return true;
			default:
				return false;
			}
		}

		internal bool IsFirstStatementOnLine(VisualBasicSyntaxNode node)
		{
			if (_possibleFirstStatementOnLine == PossibleFirstStatementKind.No)
			{
				return false;
			}
			if (node.HasLeadingTrivia)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(node.GetLeadingTrivia());
				for (int i = syntaxList.Count - 1; i >= 0; i += -1)
				{
					switch (syntaxList[i]!.Kind)
					{
					case SyntaxKind.DocumentationCommentTrivia:
					case SyntaxKind.EndOfLineTrivia:
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
						return true;
					default:
						return false;
					case SyntaxKind.WhitespaceTrivia:
					case SyntaxKind.LineContinuationTrivia:
						break;
					}
				}
			}
			return _possibleFirstStatementOnLine == PossibleFirstStatementKind.Yes;
		}

		internal DirectiveTriviaSyntax ConsumeStatementTerminatorAfterDirective(ref DirectiveTriviaSyntax stmt)
		{
			if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken && !CurrentToken.HasLeadingTrivia)
			{
				GetNextToken();
			}
			else
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = ResyncAndConsumeStatementTerminator();
				if (unexpected.Node != null)
				{
					if (stmt.Kind != SyntaxKind.BadDirectiveTrivia)
					{
						stmt = SyntaxNodeExtensions.AddTrailingSyntax(stmt, unexpected, ERRID.ERR_ExpectedEOS);
					}
					else
					{
						stmt = SyntaxNodeExtensions.AddTrailingSyntax(stmt, unexpected);
					}
				}
			}
			return stmt;
		}

		internal void ConsumedStatementTerminator(bool allowLeadingMultilineTrivia)
		{
			ConsumedStatementTerminator(allowLeadingMultilineTrivia, allowLeadingMultilineTrivia ? PossibleFirstStatementKind.Yes : PossibleFirstStatementKind.No);
		}

		private void ConsumedStatementTerminator(bool allowLeadingMultilineTrivia, PossibleFirstStatementKind possibleFirstStatementOnLine)
		{
			_allowLeadingMultilineTrivia = allowLeadingMultilineTrivia;
			_possibleFirstStatementOnLine = possibleFirstStatementOnLine;
		}

		internal void ConsumeColonInSingleLineExpression()
		{
			ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
			GetNextToken();
		}

		internal void ConsumeStatementTerminator(bool colonAsSeparator)
		{
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.EndOfFileToken:
				ConsumedStatementTerminator(allowLeadingMultilineTrivia: true);
				break;
			case SyntaxKind.StatementTerminatorToken:
				ConsumedStatementTerminator(allowLeadingMultilineTrivia: true);
				GetNextToken();
				break;
			case SyntaxKind.ColonToken:
				if (colonAsSeparator)
				{
					ConsumedStatementTerminator(allowLeadingMultilineTrivia: false);
					GetNextToken();
				}
				else
				{
					ConsumedStatementTerminator(allowLeadingMultilineTrivia: true, PossibleFirstStatementKind.IfPrecededByLineBreak);
					GetNextToken();
				}
				break;
			}
		}

		internal bool IsNextStatementInsideLambda(BlockContext context, BlockContext lambdaContext, bool allowLeadingMultilineTrivia)
		{
			_allowLeadingMultilineTrivia = allowLeadingMultilineTrivia;
			SyntaxKind syntaxKind = PeekEndStatement(1);
			if (syntaxKind != 0)
			{
				BlockContext blockContext = BlockContextExtensions.FindNearest(context, (BlockContext c) => c.KindEndsBlock(syntaxKind));
				if (blockContext != null && blockContext.Level < lambdaContext.Level)
				{
					return false;
				}
			}
			else
			{
				if (PeekDeclarationStatement(1))
				{
					return false;
				}
				switch (PeekToken(1).Kind)
				{
				case SyntaxKind.LessThanToken:
					return false;
				case SyntaxKind.CatchKeyword:
				case SyntaxKind.FinallyKeyword:
				{
					BlockContext blockContext3 = BlockContextExtensions.FindNearest(context, SyntaxKind.TryBlock, SyntaxKind.CatchBlock);
					return blockContext3 == null || blockContext3.Level >= lambdaContext.Level;
				}
				case SyntaxKind.ElseKeyword:
				case SyntaxKind.ElseIfKeyword:
				{
					BlockContext blockContext2 = BlockContextExtensions.FindNearest(context, SyntaxKind.SingleLineIfStatement, SyntaxKind.MultiLineIfBlock);
					return blockContext2 == null || blockContext2.Level >= lambdaContext.Level;
				}
				}
			}
			return true;
		}

		private bool TryGetToken<T>(SyntaxKind kind, ref T token) where T : SyntaxToken
		{
			if (CurrentToken.Kind == kind)
			{
				token = (T)CurrentToken;
				GetNextToken();
				return true;
			}
			return false;
		}

		private bool TryGetContextualKeyword(SyntaxKind kind, ref KeywordSyntax keyword, bool createIfMissing = false)
		{
			if (TryTokenAsContextualKeyword(CurrentToken, kind, ref keyword))
			{
				GetNextToken();
				return true;
			}
			if (createIfMissing)
			{
				keyword = HandleUnexpectedKeyword(kind);
			}
			return false;
		}

		private bool TryGetContextualKeywordAndEatNewLine(SyntaxKind kind, ref KeywordSyntax keyword, bool createIfMissing = false)
		{
			bool num = TryGetContextualKeyword(kind, ref keyword, createIfMissing);
			if (num)
			{
				TryEatNewLine();
			}
			return num;
		}

		private bool TryEatNewLineAndGetContextualKeyword(SyntaxKind kind, ref KeywordSyntax keyword, bool createIfMissing = false)
		{
			if (TryGetContextualKeyword(kind, ref keyword, createIfMissing))
			{
				return true;
			}
			if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken && TryTokenAsContextualKeyword(PeekToken(1), kind, ref keyword))
			{
				TryEatNewLine();
				GetNextToken();
				return true;
			}
			if (createIfMissing)
			{
				keyword = HandleUnexpectedKeyword(kind);
			}
			return false;
		}

		private bool TryGetTokenAndEatNewLine<T>(SyntaxKind kind, ref T token, bool createIfMissing = false, ScannerState state = ScannerState.VB) where T : SyntaxToken
		{
			if (CurrentToken.Kind == kind)
			{
				token = (T)CurrentToken;
				GetNextToken(state);
				if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken)
				{
					TryEatNewLine(state);
				}
				return true;
			}
			if (createIfMissing)
			{
				token = (T)HandleUnexpectedToken(kind);
			}
			return false;
		}

		private bool TryEatNewLineAndGetToken<T>(SyntaxKind kind, ref T token, bool createIfMissing = false, ScannerState state = ScannerState.VB) where T : SyntaxToken
		{
			if (CurrentToken.Kind == kind)
			{
				token = (T)CurrentToken;
				GetNextToken(state);
				return true;
			}
			if (TryEatNewLineIfFollowedBy(kind))
			{
				token = (T)CurrentToken;
				GetNextToken(state);
				return true;
			}
			if (createIfMissing)
			{
				token = (T)HandleUnexpectedToken(kind);
			}
			return false;
		}

		private SyntaxToken PeekToken(int offset)
		{
			ScannerState state = (_allowLeadingMultilineTrivia ? ScannerState.VBAllowLeadingMultilineTrivia : ScannerState.VB);
			return _scanner.PeekToken(offset, state);
		}

		internal SyntaxToken PeekNextToken(ScannerState state = ScannerState.VB)
		{
			if (_allowLeadingMultilineTrivia && state == ScannerState.VB)
			{
				state = ScannerState.VBAllowLeadingMultilineTrivia;
			}
			return _scanner.PeekNextToken(state);
		}

		private void ResetCurrentToken(ScannerState state)
		{
			_scanner.ResetCurrentToken(state);
			_currentToken = null;
		}

		internal void GetNextToken(ScannerState state = ScannerState.VB)
		{
			if (_allowLeadingMultilineTrivia && state == ScannerState.VB)
			{
				state = ScannerState.VBAllowLeadingMultilineTrivia;
			}
			_scanner.GetNextTokenInState(state);
			_currentToken = null;
		}

		internal void GetNextSyntaxNode()
		{
			_scanner.MoveToNextSyntaxNode();
			_currentToken = null;
		}

		private static bool TryIdentifierAsContextualKeyword(SyntaxToken id, ref SyntaxKind kind)
		{
			return Scanner.TryIdentifierAsContextualKeyword((IdentifierTokenSyntax)id, ref kind);
		}

		private bool TryIdentifierAsContextualKeyword(SyntaxToken id, ref KeywordSyntax k)
		{
			return _scanner.TryIdentifierAsContextualKeyword((IdentifierTokenSyntax)id, ref k);
		}

		private bool TryTokenAsContextualKeyword(SyntaxToken t, SyntaxKind kind, ref KeywordSyntax k)
		{
			KeywordSyntax k2 = null;
			if (_scanner.TryTokenAsContextualKeyword(t, ref k2) && k2.Kind == kind)
			{
				k = k2;
				return true;
			}
			return false;
		}

		private bool TryTokenAsContextualKeyword(SyntaxToken t, ref KeywordSyntax k)
		{
			return _scanner.TryTokenAsContextualKeyword(t, ref k);
		}

		private static bool TryTokenAsKeyword(SyntaxToken t, ref SyntaxKind kind)
		{
			return Scanner.TryTokenAsKeyword(t, ref kind);
		}

		private static bool IsTokenOrKeyword(SyntaxToken token, SyntaxKind[] kinds)
		{
			if (token.Kind == SyntaxKind.IdentifierToken)
			{
				return Scanner.IsContextualKeyword(token, kinds);
			}
			return IsToken(token, kinds);
		}

		private static bool IsToken(SyntaxToken token, params SyntaxKind[] kinds)
		{
			return SyntaxKindExtensions.Contains(kinds, token.Kind);
		}

		internal TNode ConsumeUnexpectedTokens<TNode>(TNode node) where TNode : VisualBasicSyntaxNode
		{
			if (CurrentToken.Kind == SyntaxKind.EndOfFileToken)
			{
				return node;
			}
			SyntaxListBuilder<SyntaxToken> syntaxListBuilder = SyntaxListBuilder<SyntaxToken>.Create();
			while (CurrentToken.Kind != SyntaxKind.EndOfFileToken)
			{
				syntaxListBuilder.Add(CurrentToken);
				GetNextToken();
			}
			return SyntaxNodeExtensions.AddTrailingSyntax(node, syntaxListBuilder.ToList(), ERRID.ERR_Syntax);
		}

		private TNode CheckFeatureAvailability<TNode>(Feature feature, TNode node) where TNode : VisualBasicSyntaxNode
		{
			return CheckFeatureAvailability(feature, node, _scanner.Options.LanguageVersion);
		}

		internal static TNode CheckFeatureAvailability<TNode>(Feature feature, TNode node, LanguageVersion languageVersion) where TNode : VisualBasicSyntaxNode
		{
			if (CheckFeatureAvailability(languageVersion, feature))
			{
				return node;
			}
			return ReportFeatureUnavailable(feature, node, languageVersion);
		}

		private static TNode ReportFeatureUnavailable<TNode>(Feature feature, TNode node, LanguageVersion languageVersion) where TNode : VisualBasicSyntaxNode
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(FeatureExtensions.GetResourceId(feature));
			VisualBasicRequiredLanguageVersion visualBasicRequiredLanguageVersion = new VisualBasicRequiredLanguageVersion(FeatureExtensions.GetLanguageVersion(feature));
			return ReportSyntaxError(node, ERRID.ERR_LanguageVersion, LanguageVersionEnumBounds.GetErrorName(languageVersion), diagnosticInfo, visualBasicRequiredLanguageVersion);
		}

		internal TNode ReportFeatureUnavailable<TNode>(Feature feature, TNode node) where TNode : VisualBasicSyntaxNode
		{
			return ReportFeatureUnavailable(feature, node, _scanner.Options.LanguageVersion);
		}

		internal bool CheckFeatureAvailability(Feature feature)
		{
			return CheckFeatureAvailability(_scanner.Options.LanguageVersion, feature);
		}

		internal static bool CheckFeatureAvailability(LanguageVersion languageVersion, Feature feature)
		{
			return FeatureExtensions.GetLanguageVersion(feature) <= languageVersion;
		}

		internal static bool CheckFeatureAvailability(DiagnosticBag diagnosticsOpt, Location location, LanguageVersion languageVersion, Feature feature)
		{
			if (!CheckFeatureAvailability(languageVersion, feature))
			{
				if (diagnosticsOpt != null)
				{
					DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(FeatureExtensions.GetResourceId(feature));
					VisualBasicRequiredLanguageVersion visualBasicRequiredLanguageVersion = new VisualBasicRequiredLanguageVersion(FeatureExtensions.GetLanguageVersion(feature));
					DiagnosticBagExtensions.Add(diagnosticsOpt, ERRID.ERR_LanguageVersion, location, LanguageVersionEnumBounds.GetErrorName(languageVersion), diagnosticInfo, visualBasicRequiredLanguageVersion);
				}
				return false;
			}
			return true;
		}

		internal static bool CheckFeatureAvailability(BindingDiagnosticBag diagnostics, Location location, LanguageVersion languageVersion, Feature feature)
		{
			return CheckFeatureAvailability(diagnostics.DiagnosticBag, location, languageVersion, feature);
		}

		internal static T ReportSyntaxError<T>(T syntax, ERRID ErrorId) where T : GreenNode
		{
			return (T)syntax.AddError(ErrorFactory.ErrorInfo(ErrorId));
		}

		internal static T ReportSyntaxError<T>(T syntax, ERRID ErrorId, params object[] args) where T : VisualBasicSyntaxNode
		{
			return (T)syntax.AddError(ErrorFactory.ErrorInfo(ErrorId, args));
		}

		private StatementSyntax ReportUnrecognizedStatementError(ERRID ErrorId)
		{
			return ReportUnrecognizedStatementError(ErrorId, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
		}

		private StatementSyntax ReportUnrecognizedStatementError(ERRID ErrorId, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers, bool createMissingIdentifier = false, bool forceErrorOnFirstToken = false)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = ResyncAt();
			IdentifierTokenSyntax missingIdentifier = null;
			if (createMissingIdentifier)
			{
				missingIdentifier = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				missingIdentifier = ReportSyntaxError(missingIdentifier, ErrorId);
			}
			StatementSyntax node;
			if (modifiers.Any() || attributes.Any())
			{
				node = SyntaxFactory.IncompleteMember(attributes, modifiers, missingIdentifier);
				node = ((!forceErrorOnFirstToken) ? SyntaxNodeExtensions.AddTrailingSyntax(node, syntaxList) : SyntaxNodeExtensions.AddTrailingSyntax(node, syntaxList, ErrorId));
			}
			else
			{
				node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement();
				node = (ParserExtensions.ContainsDiagnostics(syntaxList) ? SyntaxNodeExtensions.AddTrailingSyntax(node, syntaxList) : SyntaxNodeExtensions.AddTrailingSyntax(node, syntaxList, ErrorId));
			}
			if (!node.ContainsDiagnostics)
			{
				node = SyntaxNodeExtensions.AddError(node, ErrorId);
			}
			return node;
		}

		private KeywordSyntax ReportModifiersOnStatementError(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers, KeywordSyntax keyword)
		{
			return ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvalidOnInheritsImplOpt, attributes, modifiers, keyword);
		}

		private KeywordSyntax ReportModifiersOnStatementError(ERRID errorId, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers, KeywordSyntax keyword)
		{
			if (modifiers.Any())
			{
				keyword = SyntaxNodeExtensions.AddLeadingSyntax(keyword, modifiers.Node, errorId);
			}
			if (attributes.Any())
			{
				keyword = SyntaxNodeExtensions.AddLeadingSyntax(keyword, attributes.Node, errorId);
			}
			return keyword;
		}

		private void ReportSyntaxErrorForLanguageFeature(ERRID Errid, SyntaxToken Start, uint Feature, string wszVersion)
		{
		}

		private bool IsContinuableEOL(int i = 0)
		{
			if (PeekToken(i).Kind == SyntaxKind.StatementTerminatorToken && PeekToken(i + 1).Kind != SyntaxKind.EmptyToken)
			{
				return true;
			}
			return false;
		}

		private SyntaxToken PeekPastStatementTerminator()
		{
			SyntaxToken syntaxToken = PeekToken(1);
			if (syntaxToken.Kind == SyntaxKind.StatementTerminatorToken)
			{
				SyntaxToken syntaxToken2 = PeekToken(2);
				if (syntaxToken2.Kind != SyntaxKind.EmptyToken)
				{
					return syntaxToken2;
				}
			}
			return syntaxToken;
		}

		internal bool IsValidStatementTerminator(SyntaxToken t)
		{
			return SyntaxFacts.IsTerminator(t.Kind);
		}

		private bool CanFollowStatement(SyntaxToken T)
		{
			if (!(Context.IsWithinSingleLineLambda ? CanFollowExpression(T) : IsValidStatementTerminator(T)))
			{
				return T.Kind == SyntaxKind.ElseKeyword;
			}
			return true;
		}

		internal bool CanFollowStatementButIsNotSelectFollowingExpression(SyntaxToken nextToken)
		{
			bool num;
			if (!Context.IsWithinSingleLineLambda)
			{
				num = IsValidStatementTerminator(nextToken);
			}
			else
			{
				if (!CanFollowExpression(nextToken))
				{
					goto IL_0034;
				}
				num = nextToken.Kind != SyntaxKind.SelectKeyword;
			}
			if (num)
			{
				return true;
			}
			goto IL_0034;
			IL_0034:
			if (Context.IsLineIf)
			{
				return nextToken.Kind == SyntaxKind.ElseKeyword;
			}
			return false;
		}

		private bool CanEndExecutableStatement(SyntaxToken t)
		{
			if (!CanFollowStatement(t))
			{
				return t.Kind == SyntaxKind.ElseKeyword;
			}
			return true;
		}

		private bool CanFollowExpression(SyntaxToken t)
		{
			SyntaxKind kind = SyntaxKind.None;
			if (t.Kind == SyntaxKind.IdentifierToken && TryIdentifierAsContextualKeyword(t, ref kind))
			{
				return KeywordTable.CanFollowExpression(kind);
			}
			return KeywordTable.CanFollowExpression(t.Kind) || IsValidStatementTerminator(t);
		}

		private bool BeginsGeneric(bool nonArrayName = false, bool allowGenericsWithoutOf = false)
		{
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				if (nonArrayName)
				{
					return true;
				}
				SyntaxToken syntaxToken = PeekPastStatementTerminator();
				if (syntaxToken.Kind == SyntaxKind.OfKeyword)
				{
					return true;
				}
				if (allowGenericsWithoutOf && SyntaxFacts.IsPredefinedTypeOrVariant(syntaxToken.Kind))
				{
					SyntaxKind kind = PeekToken(2).Kind;
					if (kind == SyntaxKind.CommaToken || kind == SyntaxKind.CloseParenToken)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool MustEndStatement(SyntaxToken t)
		{
			return IsValidStatementTerminator(t);
		}

		private SyntaxKind PeekAheadFor(params SyntaxKind[] kinds)
		{
			SyntaxToken token = null;
			PeekAheadFor(s_isTokenOrKeywordFunc, kinds, out token);
			return token?.Kind ?? SyntaxKind.None;
		}

		private int PeekAheadForToken(params SyntaxKind[] kinds)
		{
			SyntaxToken token = null;
			return PeekAheadFor(s_isTokenOrKeywordFunc, kinds, out token);
		}

		private int PeekAheadFor<TArg>(Func<SyntaxToken, TArg, bool> predicate, TArg arg, out SyntaxToken token)
		{
			SyntaxToken syntaxToken = CurrentToken;
			int num = 0;
			while (!IsValidStatementTerminator(syntaxToken))
			{
				if (predicate(syntaxToken, arg))
				{
					token = syntaxToken;
					return num;
				}
				num++;
				syntaxToken = PeekToken(num);
			}
			token = null;
			return 0;
		}

		private void ResyncAt(SyntaxListBuilder<SyntaxToken> skippedTokens, ScannerState state, SyntaxKind[] resyncTokens)
		{
			while (CurrentToken.Kind != SyntaxKind.EndOfFileToken)
			{
				if (ScannerStateExtensions.IsVBState(state))
				{
					if (IsValidStatementTerminator(CurrentToken) || CurrentToken.Kind == SyntaxKind.EmptyToken)
					{
						break;
					}
				}
				else if (CurrentToken.Kind == SyntaxKind.EndOfXmlToken || CurrentToken.Kind == SyntaxKind.EndOfInterpolatedStringToken)
				{
					break;
				}
				if (!IsTokenOrKeyword(CurrentToken, resyncTokens))
				{
					skippedTokens.Add(CurrentToken);
					GetNextToken(state);
					continue;
				}
				break;
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> ResyncAt(ScannerState state, SyntaxKind[] resyncTokens)
		{
			SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
			ResyncAt(syntaxListBuilder, state, resyncTokens);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> ResyncAndConsumeStatementTerminator()
		{
			SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
			while (CurrentToken.Kind != SyntaxKind.EndOfFileToken && CurrentToken.Kind != SyntaxKind.StatementTerminatorToken)
			{
				syntaxListBuilder.Add(CurrentToken);
				GetNextToken();
			}
			if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken)
			{
				if (CurrentToken.HasLeadingTrivia)
				{
					syntaxListBuilder.Add(CurrentToken);
				}
				GetNextToken();
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> ResyncAt()
		{
			return ResyncAt(ScannerState.VB, Array.Empty<SyntaxKind>());
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> ResyncAt(SyntaxKind[] resyncTokens)
		{
			return ResyncAt(ScannerState.VB, resyncTokens);
		}

		private T ResyncAt<T>(T syntax, ScannerState state, params SyntaxKind[] resyncTokens) where T : VisualBasicSyntaxNode
		{
			return SyntaxNodeExtensions.AddTrailingSyntax(syntax, ResyncAt(state, resyncTokens));
		}

		private T ResyncAt<T>(T syntax) where T : VisualBasicSyntaxNode
		{
			return SyntaxNodeExtensions.AddTrailingSyntax(syntax, ResyncAt());
		}

		private T ResyncAt<T>(T syntax, params SyntaxKind[] resyncTokens) where T : GreenNode
		{
			return SyntaxNodeExtensions.AddTrailingSyntax(syntax, ResyncAt(resyncTokens).Node);
		}

		private bool TryEatNewLine(ScannerState state = ScannerState.VB)
		{
			if (CurrentToken.Kind == SyntaxKind.StatementTerminatorToken && PeekEndStatement(1) == SyntaxKind.None && !_evaluatingConditionCompilationExpression && !NextLineStartsWithStatementTerminator())
			{
				_hadImplicitLineContinuation = true;
				if (SyntaxNodeExtensions.ContainsCommentTrivia(PrevToken.GetTrailingTrivia()))
				{
					_hadLineContinuationComment = true;
				}
				GetNextToken(state);
				return true;
			}
			return false;
		}

		private bool TryEatNewLineIfFollowedBy(SyntaxKind kind)
		{
			if (NextLineStartsWith(kind))
			{
				return TryEatNewLine();
			}
			return false;
		}

		private bool TryEatNewLineIfNotFollowedBy(SyntaxKind kind)
		{
			if (!NextLineStartsWith(kind))
			{
				return TryEatNewLine();
			}
			return false;
		}

		private bool NextLineStartsWith(SyntaxKind kind)
		{
			if (CurrentToken.IsEndOfLine)
			{
				SyntaxToken syntaxToken = PeekToken(1);
				if (syntaxToken.Kind == kind)
				{
					return true;
				}
				if (syntaxToken.Kind == SyntaxKind.IdentifierToken)
				{
					SyntaxKind kind2 = SyntaxKind.None;
					if (TryIdentifierAsContextualKeyword(syntaxToken, ref kind2) && kind2 == kind)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool NextLineStartsWithStatementTerminator(int offset = 0)
		{
			SyntaxKind kind = PeekToken(offset + 1).Kind;
			if (kind != SyntaxKind.EmptyToken)
			{
				return kind == SyntaxKind.EndOfFileToken;
			}
			return true;
		}

		private static bool CanUseInTryGetToken(SyntaxKind kind)
		{
			if (!SyntaxFacts.IsTerminator(kind))
			{
				return kind != SyntaxKind.EmptyToken;
			}
			return false;
		}

		private ContinueStatementSyntax ParseContinueStatement()
		{
			KeywordSyntax continueKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax keywordSyntax = null;
			SyntaxKind kind = default(SyntaxKind);
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.DoKeyword:
				kind = SyntaxKind.ContinueDoStatement;
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.ForKeyword:
				kind = SyntaxKind.ContinueForStatement;
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.WhileKeyword:
				kind = SyntaxKind.ContinueWhileStatement;
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			default:
			{
				BlockContext blockContext = BlockContextExtensions.FindNearest(Context, SyntaxFacts.SupportsContinueStatement);
				if (blockContext != null)
				{
					switch (blockContext.BlockKind)
					{
					case SyntaxKind.SimpleDoLoopBlock:
					case SyntaxKind.DoWhileLoopBlock:
						kind = SyntaxKind.ContinueDoStatement;
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.DoKeyword);
						break;
					case SyntaxKind.ForBlock:
					case SyntaxKind.ForEachBlock:
						kind = SyntaxKind.ContinueForStatement;
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ForKeyword);
						break;
					case SyntaxKind.WhileBlock:
						kind = SyntaxKind.ContinueWhileStatement;
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword);
						break;
					}
				}
				if (keywordSyntax == null)
				{
					kind = SyntaxKind.ContinueDoStatement;
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.DoKeyword);
				}
				keywordSyntax = ReportSyntaxError(keywordSyntax, ERRID.ERR_ExpectedContinueKind);
				break;
			}
			}
			return SyntaxFactory.ContinueStatement(kind, continueKeyword, keywordSyntax);
		}

		private StatementSyntax ParseExitStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax keywordSyntax2 = null;
			SyntaxKind kind = default(SyntaxKind);
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.DoKeyword:
				kind = SyntaxKind.ExitDoStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.ForKeyword:
				kind = SyntaxKind.ExitForStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.WhileKeyword:
				kind = SyntaxKind.ExitWhileStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.SelectKeyword:
				kind = SyntaxKind.ExitSelectStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.SubKeyword:
				kind = SyntaxKind.ExitSubStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.FunctionKeyword:
				kind = SyntaxKind.ExitFunctionStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.PropertyKeyword:
				kind = SyntaxKind.ExitPropertyStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			case SyntaxKind.TryKeyword:
				kind = SyntaxKind.ExitTryStatement;
				keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				break;
			default:
			{
				BlockContext blockContext = BlockContextExtensions.FindNearest(Context, SyntaxFacts.SupportsExitStatement);
				if (blockContext != null)
				{
					switch (blockContext.BlockKind)
					{
					case SyntaxKind.SimpleDoLoopBlock:
					case SyntaxKind.DoWhileLoopBlock:
						kind = SyntaxKind.ExitDoStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.DoKeyword);
						break;
					case SyntaxKind.ForBlock:
					case SyntaxKind.ForEachBlock:
						kind = SyntaxKind.ExitForStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ForKeyword);
						break;
					case SyntaxKind.WhileBlock:
						kind = SyntaxKind.ExitWhileStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword);
						break;
					case SyntaxKind.SelectBlock:
						kind = SyntaxKind.ExitSelectStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SelectKeyword);
						break;
					case SyntaxKind.SubBlock:
					case SyntaxKind.ConstructorBlock:
						kind = SyntaxKind.ExitSubStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword);
						break;
					case SyntaxKind.FunctionBlock:
						kind = SyntaxKind.ExitFunctionStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.FunctionKeyword);
						break;
					case SyntaxKind.PropertyBlock:
						kind = SyntaxKind.ExitPropertyStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.PropertyKeyword);
						break;
					case SyntaxKind.TryBlock:
						kind = SyntaxKind.ExitTryStatement;
						keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.TryKeyword);
						break;
					}
				}
				ERRID eRRID = ERRID.ERR_None;
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.OperatorKeyword:
					eRRID = ERRID.ERR_ExitOperatorNotValid;
					break;
				case SyntaxKind.AddHandlerKeyword:
				case SyntaxKind.RaiseEventKeyword:
				case SyntaxKind.RemoveHandlerKeyword:
					eRRID = ERRID.ERR_ExitEventMemberNotInvalid;
					break;
				}
				if (eRRID != 0)
				{
					StatementSyntax result = SyntaxNodeExtensions.AddLeadingSyntax((StatementSyntax)SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ReturnKeyword), null), (GreenNode)SyntaxList.List(keywordSyntax, CurrentToken), eRRID);
					GetNextToken();
					return result;
				}
				if (keywordSyntax2 == null)
				{
					kind = SyntaxKind.ExitSubStatement;
					keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword);
				}
				keywordSyntax2 = ReportSyntaxError(keywordSyntax2, ERRID.ERR_ExpectedExitKind);
				break;
			}
			}
			return SyntaxFactory.ExitStatement(kind, keywordSyntax, keywordSyntax2);
		}

		private CaseStatementSyntax ParseCaseStatement()
		{
			KeywordSyntax caseKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			SeparatedSyntaxListBuilder<CaseClauseSyntax> item = _pool.AllocateSeparated<CaseClauseSyntax>();
			KeywordSyntax keywordSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.ElseKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				ElseCaseClauseSyntax node = SyntaxFactory.ElseCaseClause(keywordSyntax);
				item.Add(node);
			}
			else
			{
				while (true)
				{
					SyntaxKind kind = CurrentToken.Kind;
					CaseClauseSyntax node2;
					if (kind == SyntaxKind.IsKeyword || SyntaxFacts.IsRelationalOperator(kind))
					{
						KeywordSyntax token = null;
						TryGetTokenAndEatNewLine(SyntaxKind.IsKeyword, ref token);
						if (SyntaxFacts.IsRelationalOperator(CurrentToken.Kind))
						{
							PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
							GetNextToken();
							TryEatNewLine();
							ExpressionSyntax expressionSyntax = ParseExpressionCore();
							if (expressionSyntax.ContainsDiagnostics)
							{
								expressionSyntax = ResyncAt(expressionSyntax);
							}
							node2 = SyntaxFactory.RelationalCaseClause(RelationalOperatorKindToCaseKind(punctuationSyntax.Kind), token, punctuationSyntax, expressionSyntax);
						}
						else
						{
							PunctuationSyntax operatorToken = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), ERRID.ERR_ExpectedRelational);
							node2 = ResyncAt(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.RelationalCaseClause(SyntaxKind.CaseEqualsClause, token, operatorToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()));
						}
					}
					else
					{
						ExpressionSyntax expressionSyntax2 = ParseExpressionCore();
						if (expressionSyntax2.ContainsDiagnostics)
						{
							expressionSyntax2 = ResyncAt(expressionSyntax2, SyntaxKind.ToKeyword);
						}
						KeywordSyntax token2 = null;
						if (TryGetToken(SyntaxKind.ToKeyword, ref token2))
						{
							ExpressionSyntax expressionSyntax3 = ParseExpressionCore();
							if (expressionSyntax3.ContainsDiagnostics)
							{
								expressionSyntax3 = ResyncAt(expressionSyntax3);
							}
							node2 = SyntaxFactory.RangeCaseClause(expressionSyntax2, token2, expressionSyntax3);
						}
						else
						{
							node2 = SyntaxFactory.SimpleCaseClause(expressionSyntax2);
						}
					}
					item.Add(node2);
					PunctuationSyntax token3 = null;
					if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token3))
					{
						break;
					}
					item.AddSeparator(token3);
				}
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CaseClauseSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			if (keywordSyntax == null)
			{
				return SyntaxFactory.CaseStatement(caseKeyword, separatedSyntaxList);
			}
			return SyntaxFactory.CaseElseStatement(caseKeyword, separatedSyntaxList);
		}

		private static SyntaxKind RelationalOperatorKindToCaseKind(SyntaxKind kind)
		{
			return kind switch
			{
				SyntaxKind.LessThanToken => SyntaxKind.CaseLessThanClause, 
				SyntaxKind.LessThanEqualsToken => SyntaxKind.CaseLessThanOrEqualClause, 
				SyntaxKind.EqualsToken => SyntaxKind.CaseEqualsClause, 
				SyntaxKind.LessThanGreaterThanToken => SyntaxKind.CaseNotEqualsClause, 
				SyntaxKind.GreaterThanToken => SyntaxKind.CaseGreaterThanClause, 
				SyntaxKind.GreaterThanEqualsToken => SyntaxKind.CaseGreaterThanOrEqualClause, 
				_ => SyntaxKind.None, 
			};
		}

		private SelectStatementSyntax ParseSelectStatement()
		{
			KeywordSyntax selectKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax token = null;
			TryGetToken(SyntaxKind.CaseKeyword, ref token);
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.SelectStatement(selectKeyword, token, expressionSyntax);
		}

		private IfStatementSyntax ParseIfStatement()
		{
			KeywordSyntax ifKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.ThenKeyword);
			}
			KeywordSyntax token = null;
			TryGetToken(SyntaxKind.ThenKeyword, ref token);
			return SyntaxFactory.IfStatement(ifKeyword, expressionSyntax, token);
		}

		private ElseStatementSyntax ParseElseStatement()
		{
			KeywordSyntax elseKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			return SyntaxFactory.ElseStatement(elseKeyword);
		}

		private StatementSyntax ParseElseIfStatement()
		{
			KeywordSyntax elseIfKeyword = null;
			if (CurrentToken.Kind == SyntaxKind.ElseIfKeyword)
			{
				elseIfKeyword = (KeywordSyntax)CurrentToken;
				GetNextToken();
			}
			else if (CurrentToken.Kind == SyntaxKind.ElseKeyword)
			{
				KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				if (Context.IsSingleLine)
				{
					return SyntaxFactory.ElseStatement(keywordSyntax);
				}
				KeywordSyntax keywordSyntax2 = (KeywordSyntax)CurrentToken;
				GetNextToken();
				elseIfKeyword = new KeywordSyntax(SyntaxKind.ElseIfKeyword, MergeTokenText(keywordSyntax, keywordSyntax2), keywordSyntax.GetLeadingTrivia(), keywordSyntax2.GetTrailingTrivia());
			}
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.ThenKeyword);
			}
			KeywordSyntax token = null;
			TryGetToken(SyntaxKind.ThenKeyword, ref token);
			return SyntaxFactory.ElseIfStatement(elseIfKeyword, expressionSyntax, token);
		}

		private StatementSyntax ParseAnachronisticStatement()
		{
			KeywordSyntax obj = (KeywordSyntax)CurrentToken;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = ResyncAt();
			KeywordSyntax endKeyword = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword);
			StatementSyntax node = null;
			ERRID errorId = default(ERRID);
			switch (obj.Kind)
			{
			case SyntaxKind.EndIfKeyword:
				node = SyntaxFactory.EndIfStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword));
				errorId = ERRID.ERR_ObsoleteEndIf;
				break;
			case SyntaxKind.WendKeyword:
				node = SyntaxFactory.EndWhileStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword));
				errorId = ERRID.ERR_ObsoleteWhileWend;
				break;
			case SyntaxKind.GosubKeyword:
				node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement();
				errorId = ERRID.ERR_ObsoleteGosub;
				break;
			}
			return SyntaxNodeExtensions.AddTrailingSyntax(node, unexpected, errorId);
		}

		private DoStatementSyntax ParseDoStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			WhileOrUntilClauseSyntax optionalWhileOrUntilClause = null;
			TryParseOptionalWhileOrUntilClause(keywordSyntax, ref optionalWhileOrUntilClause);
			SyntaxKind kind = ((optionalWhileOrUntilClause == null) ? SyntaxKind.SimpleDoStatement : ((optionalWhileOrUntilClause.Kind != SyntaxKind.WhileClause) ? SyntaxKind.DoUntilStatement : SyntaxKind.DoWhileStatement));
			return SyntaxFactory.DoStatement(kind, keywordSyntax, optionalWhileOrUntilClause);
		}

		private LoopStatementSyntax ParseLoopStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			WhileOrUntilClauseSyntax optionalWhileOrUntilClause = null;
			TryParseOptionalWhileOrUntilClause(keywordSyntax, ref optionalWhileOrUntilClause);
			SyntaxKind kind = ((optionalWhileOrUntilClause == null) ? SyntaxKind.SimpleLoopStatement : ((optionalWhileOrUntilClause.Kind != SyntaxKind.WhileClause) ? SyntaxKind.LoopUntilStatement : SyntaxKind.LoopWhileStatement));
			return SyntaxFactory.LoopStatement(kind, keywordSyntax, optionalWhileOrUntilClause);
		}

		private StatementSyntax ParseForStatement()
		{
			KeywordSyntax forKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax token = null;
			if (TryGetToken(SyntaxKind.EachKeyword, ref token))
			{
				return ParseForEachStatement(forKeyword, token);
			}
			return ParseForStatement(forKeyword);
		}

		private ForEachStatementSyntax ParseForEachStatement(KeywordSyntax forKeyword, KeywordSyntax eachKeyword)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = ParseForLoopControlVariable();
			ExpressionSyntax expressionSyntax = null;
			if (visualBasicSyntaxNode.ContainsDiagnostics)
			{
				visualBasicSyntaxNode = ResyncAt(visualBasicSyntaxNode, SyntaxKind.InKeyword);
			}
			TryEatNewLineIfFollowedBy(SyntaxKind.InKeyword);
			KeywordSyntax token = null;
			if (TryGetTokenAndEatNewLine(SyntaxKind.InKeyword, ref token))
			{
				expressionSyntax = ParseExpressionCore();
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax);
				}
			}
			else
			{
				token = (KeywordSyntax)HandleUnexpectedToken(SyntaxKind.InKeyword);
				expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ResyncAt(new SyntaxKind[1] { SyntaxKind.ToKeyword }), ERRID.ERR_Syntax);
			}
			return SyntaxFactory.ForEachStatement(forKeyword, eachKeyword, visualBasicSyntaxNode, token, expressionSyntax);
		}

		private ForStatementSyntax ParseForStatement(KeywordSyntax forKeyword)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = ParseForLoopControlVariable();
			ExpressionSyntax expressionSyntax = null;
			ExpressionSyntax expressionSyntax2 = null;
			if (visualBasicSyntaxNode.ContainsDiagnostics)
			{
				visualBasicSyntaxNode = ResyncAt(visualBasicSyntaxNode, SyntaxKind.EqualsToken, SyntaxKind.ToKeyword);
			}
			PunctuationSyntax token = null;
			if (TryGetTokenAndEatNewLine(SyntaxKind.EqualsToken, ref token))
			{
				expressionSyntax = ParseExpressionCore();
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.ToKeyword);
				}
			}
			else
			{
				token = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.EqualsToken);
				expressionSyntax = SyntaxNodeExtensions.AddTrailingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ResyncAt(new SyntaxKind[1] { SyntaxKind.ToKeyword }), ERRID.ERR_Syntax);
			}
			KeywordSyntax token2 = null;
			if (TryGetToken(SyntaxKind.ToKeyword, ref token2))
			{
				expressionSyntax2 = ParseExpressionCore();
				if (expressionSyntax2.ContainsDiagnostics)
				{
					expressionSyntax2 = ResyncAt(expressionSyntax2, SyntaxKind.StepKeyword);
				}
			}
			else
			{
				token2 = (KeywordSyntax)HandleUnexpectedToken(SyntaxKind.ToKeyword);
				expressionSyntax2 = SyntaxNodeExtensions.AddTrailingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ResyncAt(new SyntaxKind[1] { SyntaxKind.ToKeyword }));
			}
			ForStepClauseSyntax stepClause = null;
			KeywordSyntax token3 = null;
			ExpressionSyntax expressionSyntax3 = null;
			if (TryGetToken(SyntaxKind.StepKeyword, ref token3))
			{
				expressionSyntax3 = ParseExpressionCore();
				if (expressionSyntax3.ContainsDiagnostics)
				{
					expressionSyntax3 = ResyncAt(expressionSyntax3);
				}
				stepClause = SyntaxFactory.ForStepClause(token3, expressionSyntax3);
			}
			return SyntaxFactory.ForStatement(forKeyword, visualBasicSyntaxNode, token, expressionSyntax, token2, expressionSyntax2, stepClause);
		}

		private NextStatementSyntax ParseNextStatement()
		{
			KeywordSyntax nextKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			if (CanFollowStatement(CurrentToken))
			{
				return SyntaxFactory.NextStatement(nextKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>));
			}
			SeparatedSyntaxListBuilder<ExpressionSyntax> item = _pool.AllocateSeparated<ExpressionSyntax>();
			BlockContext blockContext = Context;
			while (true)
			{
				ExpressionSyntax expressionSyntax = ParseVariable();
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax);
				}
				if (blockContext != null && blockContext.BlockKind != SyntaxKind.ForBlock && blockContext.BlockKind != SyntaxKind.ForEachBlock)
				{
					expressionSyntax = ReportSyntaxError(expressionSyntax, ERRID.ERR_ExtraNextVariable);
					blockContext = null;
				}
				item.Add(expressionSyntax);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
				if (blockContext != null)
				{
					blockContext = blockContext.PrevBlock;
				}
			}
			NextStatementSyntax result = SyntaxFactory.NextStatement(nextKeyword, item.ToList());
			_pool.Free(in item);
			return result;
		}

		private VisualBasicSyntaxNode ParseForLoopControlVariable()
		{
			SyntaxKind kind = CurrentToken.Kind;
			if (kind == SyntaxKind.IdentifierToken)
			{
				switch (PeekToken(1).Kind)
				{
				case SyntaxKind.AsKeyword:
				case SyntaxKind.QuestionToken:
					return ParseForLoopVariableDeclaration();
				case SyntaxKind.OpenParenToken:
				{
					SyntaxToken token = null;
					int num = PeekAheadFor(s_isTokenOrKeywordFunc, new SyntaxKind[3]
					{
						SyntaxKind.AsKeyword,
						SyntaxKind.InKeyword,
						SyntaxKind.EqualsToken
					}, out token);
					if (token != null && token.Kind == SyntaxKind.AsKeyword && PeekToken(num - 1).Kind == SyntaxKind.CloseParenToken)
					{
						return ParseForLoopVariableDeclaration();
					}
					break;
				}
				}
				return ParseVariable();
			}
			return ParseVariable();
		}

		private VariableDeclaratorSyntax ParseForLoopVariableDeclaration()
		{
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = ParseModifiedIdentifier(AllowExplicitArraySizes: true, checkForCustom: false);
			if (modifiedIdentifierSyntax.ContainsDiagnostics && PeekAheadFor(SyntaxKind.AsKeyword, SyntaxKind.InKeyword, SyntaxKind.EqualsToken) == SyntaxKind.AsKeyword)
			{
				modifiedIdentifierSyntax = ResyncAt(modifiedIdentifierSyntax, SyntaxKind.AsKeyword);
			}
			KeywordSyntax keywordSyntax = null;
			TypeSyntax typeSyntax = null;
			AsClauseSyntax asClause = null;
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
				typeSyntax = ParseGeneralType();
				asClause = SyntaxFactory.SimpleAsClause(keywordSyntax, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
			}
			SeparatedSyntaxListBuilder<ModifiedIdentifierSyntax> item = _pool.AllocateSeparated<ModifiedIdentifierSyntax>();
			item.Add(modifiedIdentifierSyntax);
			VariableDeclaratorSyntax result = SyntaxFactory.VariableDeclarator(item.ToList(), asClause, null);
			_pool.Free(in item);
			return result;
		}

		private SyntaxToken ParseLabelReference()
		{
			SyntaxToken currentToken = CurrentToken;
			if (currentToken.Kind == SyntaxKind.IdentifierToken)
			{
				if (((IdentifierTokenSyntax)currentToken).TypeCharacter != 0)
				{
					currentToken = ReportSyntaxError(currentToken, ERRID.ERR_NoTypecharInLabel);
					GetNextToken();
					return currentToken;
				}
				return ParseIdentifier();
			}
			if (currentToken.Kind == SyntaxKind.IntegerLiteralToken)
			{
				IntegerLiteralTokenSyntax integerLiteralTokenSyntax = (IntegerLiteralTokenSyntax)currentToken;
				if (!integerLiteralTokenSyntax.ContainsDiagnostics)
				{
					if (integerLiteralTokenSyntax.TypeSuffix == TypeCharacter.None)
					{
						ulong num = Microsoft.VisualBasic.CompilerServices.Conversions.ToULong(integerLiteralTokenSyntax.ObjectValue);
						if (integerLiteralTokenSyntax is IntegerLiteralTokenSyntax<int>)
						{
							num = (uint)num;
						}
						integerLiteralTokenSyntax = new IntegerLiteralTokenSyntax<ulong>(SyntaxKind.IntegerLiteralToken, integerLiteralTokenSyntax.ToString(), integerLiteralTokenSyntax.GetLeadingTrivia(), integerLiteralTokenSyntax.GetTrailingTrivia(), integerLiteralTokenSyntax.Base, TypeCharacter.None, num);
					}
					else
					{
						integerLiteralTokenSyntax = ReportSyntaxError(integerLiteralTokenSyntax, ERRID.ERR_Syntax);
					}
				}
				GetNextToken();
				return integerLiteralTokenSyntax;
			}
			currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
			return ReportSyntaxError(currentToken, ERRID.ERR_ExpectedIdentifier);
		}

		private StatementSyntax ParseGotoStatement()
		{
			KeywordSyntax goToKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			SyntaxToken labelName = ParseLabelReference();
			return SyntaxFactory.GoToStatement(goToKeyword, GetLabelSyntaxForIdentifierOrLineNumber(labelName));
		}

		private LabelSyntax GetLabelSyntaxForIdentifierOrLineNumber(SyntaxToken labelName)
		{
			if (labelName.Kind != SyntaxKind.IntegerLiteralToken)
			{
				return SyntaxFactory.IdentifierLabel(labelName);
			}
			return SyntaxFactory.NumericLabel(labelName);
		}

		private StatementSyntax ParseOnErrorStatement()
		{
			KeywordSyntax onKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax keywordSyntax;
			if (CurrentToken.Kind == SyntaxKind.ErrorKeyword)
			{
				keywordSyntax = (KeywordSyntax)CurrentToken;
				GetNextToken();
			}
			else
			{
				keywordSyntax = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ErrorKeyword), ERRID.ERR_ObsoleteOnGotoGosub);
				keywordSyntax = ResyncAt(keywordSyntax, SyntaxKind.GoToKeyword, SyntaxKind.ResumeKeyword);
			}
			if (CurrentToken.Kind == SyntaxKind.ResumeKeyword)
			{
				return ParseOnErrorResumeNext(onKeyword, keywordSyntax);
			}
			if (CurrentToken.Kind == SyntaxKind.GoToKeyword)
			{
				return ParseOnErrorGoto(onKeyword, keywordSyntax);
			}
			KeywordSyntax keywordSyntax2 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.GoToKeyword);
			if (!keywordSyntax.ContainsDiagnostics)
			{
				keywordSyntax2 = ReportSyntaxError(keywordSyntax2, ERRID.ERR_ExpectedResumeOrGoto);
			}
			return SyntaxFactory.OnErrorGoToStatement(SyntaxKind.OnErrorGoToLabelStatement, onKeyword, keywordSyntax, keywordSyntax2, null, SyntaxFactory.IdentifierLabel(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()));
		}

		private OnErrorResumeNextStatementSyntax ParseOnErrorResumeNext(KeywordSyntax onKeyword, KeywordSyntax errorKeyword)
		{
			KeywordSyntax resumeKeyword = (KeywordSyntax)CurrentToken;
			KeywordSyntax token = null;
			GetNextToken();
			VerifyExpectedToken(SyntaxKind.NextKeyword, ref token);
			return SyntaxFactory.OnErrorResumeNextStatement(onKeyword, errorKeyword, resumeKeyword, token);
		}

		private OnErrorGoToStatementSyntax ParseOnErrorGoto(KeywordSyntax onKeyword, KeywordSyntax errorKeyword)
		{
			KeywordSyntax goToKeyword = (KeywordSyntax)CurrentToken;
			PunctuationSyntax minus = null;
			SyntaxToken syntaxToken = PeekToken(1);
			SyntaxKind kind;
			LabelSyntax label;
			if (syntaxToken.Kind == SyntaxKind.IntegerLiteralToken && EmbeddedOperators.CompareString(syntaxToken.ValueText, "0", TextCompare: false) == 0)
			{
				kind = SyntaxKind.OnErrorGoToZeroStatement;
				label = SyntaxFactory.NumericLabel(syntaxToken);
				GetNextToken();
				GetNextToken();
			}
			else if (syntaxToken.Kind == SyntaxKind.MinusToken && PeekToken(2).Kind == SyntaxKind.IntegerLiteralToken && EmbeddedOperators.CompareString(PeekToken(2).ValueText, "1", TextCompare: false) == 0)
			{
				kind = SyntaxKind.OnErrorGoToMinusOneStatement;
				minus = (PunctuationSyntax)syntaxToken;
				label = SyntaxFactory.NumericLabel(PeekToken(2));
				GetNextToken();
				GetNextToken();
				GetNextToken();
			}
			else
			{
				GetNextToken();
				kind = SyntaxKind.OnErrorGoToLabelStatement;
				label = GetLabelSyntaxForIdentifierOrLineNumber(ParseLabelReference());
			}
			return SyntaxFactory.OnErrorGoToStatement(kind, onKeyword, errorKeyword, goToKeyword, minus, label);
		}

		private ResumeStatementSyntax ParseResumeStatement()
		{
			KeywordSyntax resumeKeyword = (KeywordSyntax)CurrentToken;
			SyntaxToken syntaxToken = null;
			GetNextToken();
			if (!IsValidStatementTerminator(CurrentToken))
			{
				if (CurrentToken.Kind == SyntaxKind.NextKeyword)
				{
					KeywordSyntax labelToken = (KeywordSyntax)CurrentToken;
					GetNextToken();
					return SyntaxFactory.ResumeNextStatement(resumeKeyword, SyntaxFactory.NextLabel(labelToken));
				}
				syntaxToken = ParseLabelReference();
				return SyntaxFactory.ResumeLabelStatement(resumeKeyword, GetLabelSyntaxForIdentifierOrLineNumber(syntaxToken));
			}
			return SyntaxFactory.ResumeStatement(resumeKeyword, null);
		}

		private StatementSyntax ParseAssignmentOrInvocationStatement()
		{
			ExpressionSyntax expressionSyntax = ParseTerm();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.EqualsToken);
			}
			if (SyntaxFacts.IsAssignmentStatementOperatorToken(CurrentToken.Kind))
			{
				PunctuationSyntax operatorToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				TryEatNewLine();
				ExpressionSyntax expressionSyntax2 = ParseExpressionCore();
				if (expressionSyntax2.ContainsDiagnostics)
				{
					expressionSyntax2 = ResyncAt(expressionSyntax2);
				}
				return MakeAssignmentStatement(expressionSyntax, operatorToken, expressionSyntax2);
			}
			return SyntaxFactory.ExpressionStatement(MakeInvocationExpression(expressionSyntax));
		}

		private ExpressionSyntax MakeInvocationExpression(ExpressionSyntax target)
		{
			if (target.Kind == SyntaxKind.ConditionalAccessExpression)
			{
				ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = (ConditionalAccessExpressionSyntax)target;
				ExpressionSyntax expressionSyntax = MakeInvocationExpression(conditionalAccessExpressionSyntax.WhenNotNull);
				if (conditionalAccessExpressionSyntax.WhenNotNull != expressionSyntax)
				{
					target = SyntaxFactory.ConditionalAccessExpression(conditionalAccessExpressionSyntax.Expression, conditionalAccessExpressionSyntax.QuestionMarkToken, expressionSyntax);
				}
			}
			else if (target.Kind != SyntaxKind.InvocationExpression)
			{
				if (!CanEndExecutableStatement(CurrentToken) && CurrentToken.Kind != SyntaxKind.BadToken && target.Kind != SyntaxKind.PredefinedCastExpression)
				{
					GreenNode unexpected = null;
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = ParseArguments(ref unexpected);
					PunctuationSyntax punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken);
					if (unexpected != null)
					{
						punctuationSyntax = SyntaxNodeExtensions.AddLeadingSyntax(punctuationSyntax, unexpected);
					}
					ArgumentListSyntax syntax = SyntaxFactory.ArgumentList(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.OpenParenToken), separatedSyntaxList, punctuationSyntax);
					target = SyntaxFactory.InvocationExpression(target, ReportSyntaxError(syntax, ERRID.ERR_ObsoleteArgumentsNeedParens));
				}
				else
				{
					target = SyntaxFactory.InvocationExpression(target, null);
				}
			}
			return target;
		}

		private AssignmentStatementSyntax MakeAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			return operatorToken.Kind switch
			{
				SyntaxKind.EqualsToken => SyntaxFactory.SimpleAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.PlusEqualsToken => SyntaxFactory.AddAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.MinusEqualsToken => SyntaxFactory.SubtractAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.AsteriskEqualsToken => SyntaxFactory.MultiplyAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.SlashEqualsToken => SyntaxFactory.DivideAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.BackslashEqualsToken => SyntaxFactory.IntegerDivideAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.CaretEqualsToken => SyntaxFactory.ExponentiateAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.LessThanLessThanEqualsToken => SyntaxFactory.LeftShiftAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.GreaterThanGreaterThanEqualsToken => SyntaxFactory.RightShiftAssignmentStatement(left, operatorToken, right), 
				SyntaxKind.AmpersandEqualsToken => SyntaxFactory.ConcatenateAssignmentStatement(left, operatorToken, right), 
				_ => throw ExceptionUtilities.UnexpectedValue(operatorToken.Kind), 
			};
		}

		private CallStatementSyntax ParseCallStatement()
		{
			KeywordSyntax callKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = MakeCallStatementExpression(ParseVariable());
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.CallStatement(callKeyword, expressionSyntax);
		}

		private ExpressionSyntax MakeCallStatementExpression(ExpressionSyntax expr)
		{
			if (expr.Kind == SyntaxKind.ConditionalAccessExpression)
			{
				ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = (ConditionalAccessExpressionSyntax)expr;
				ExpressionSyntax expressionSyntax = MakeCallStatementExpression(conditionalAccessExpressionSyntax.WhenNotNull);
				if (conditionalAccessExpressionSyntax.WhenNotNull != expressionSyntax)
				{
					expr = SyntaxFactory.ConditionalAccessExpression(conditionalAccessExpressionSyntax.Expression, conditionalAccessExpressionSyntax.QuestionMarkToken, expressionSyntax);
				}
			}
			else if (expr.Kind != SyntaxKind.InvocationExpression)
			{
				expr = SyntaxFactory.InvocationExpression(expr, null);
			}
			return expr;
		}

		private RaiseEventStatementSyntax ParseRaiseEventStatement()
		{
			KeywordSyntax raiseEventKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierNameSyntax identifierNameSyntax = ParseIdentifierNameAllowingKeyword();
			if (identifierNameSyntax.ContainsDiagnostics)
			{
				identifierNameSyntax = SyntaxNodeExtensions.AddTrailingSyntax(identifierNameSyntax, ResyncAt());
			}
			ArgumentListSyntax argumentList = null;
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken)
			{
				argumentList = ParseParenthesizedArguments();
			}
			return SyntaxFactory.RaiseEventStatement(raiseEventKeyword, identifierNameSyntax, argumentList);
		}

		private StatementSyntax ParseRedimStatement()
		{
			KeywordSyntax reDimKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax k = null;
			KeywordSyntax keywordSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken && TryIdentifierAsContextualKeyword(CurrentToken, ref k) && k.Kind == SyntaxKind.PreserveKeyword)
			{
				keywordSyntax = k;
				GetNextToken();
			}
			SeparatedSyntaxListBuilder<RedimClauseSyntax> item = _pool.AllocateSeparated<RedimClauseSyntax>();
			while (true)
			{
				ExpressionSyntax expressionSyntax = ParseTerm(BailIfFirstTokenRejected: false, RedimOrNewParent: true);
				if (expressionSyntax.ContainsDiagnostics)
				{
					expressionSyntax = ResyncAt(expressionSyntax);
				}
				RedimClauseSyntax node;
				if (expressionSyntax.Kind == SyntaxKind.InvocationExpression)
				{
					InvocationExpressionSyntax invocationExpressionSyntax = (InvocationExpressionSyntax)expressionSyntax;
					node = SyntaxFactory.RedimClause(invocationExpressionSyntax.Expression, invocationExpressionSyntax.ArgumentList);
					DiagnosticInfo[] diagnostics = invocationExpressionSyntax.GetDiagnostics();
					if (diagnostics != null && diagnostics.Length > 0)
					{
						node = SyntaxExtensions.WithDiagnostics(node, diagnostics);
					}
				}
				else
				{
					node = SyntaxFactory.RedimClause(expressionSyntax, SyntaxFactory.ArgumentList(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.OpenParenToken), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)));
				}
				item.Add(node);
				PunctuationSyntax token = null;
				if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token))
				{
					break;
				}
				item.AddSeparator(token);
			}
			ReDimStatementSyntax reDimStatementSyntax = ((keywordSyntax == null) ? SyntaxFactory.ReDimStatement(reDimKeyword, keywordSyntax, item.ToList()) : SyntaxFactory.ReDimPreserveStatement(reDimKeyword, keywordSyntax, item.ToList()));
			_pool.Free(in item);
			if (CurrentToken.Kind == SyntaxKind.AsKeyword)
			{
				reDimStatementSyntax = SyntaxNodeExtensions.AddTrailingSyntax(reDimStatementSyntax, CurrentToken, ERRID.ERR_ObsoleteRedimAs);
				GetNextToken();
			}
			return reDimStatementSyntax;
		}

		private AddRemoveHandlerStatementSyntax ParseHandlerStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			SyntaxKind kind = ((keywordSyntax.Kind == SyntaxKind.AddHandlerKeyword) ? SyntaxKind.AddHandlerStatement : SyntaxKind.RemoveHandlerStatement);
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax, SyntaxKind.CommaToken);
			}
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token, createIfMissing: true);
			ExpressionSyntax expressionSyntax2 = ParseExpressionCore();
			if (expressionSyntax2.ContainsDiagnostics)
			{
				expressionSyntax2 = ResyncAt(expressionSyntax2);
			}
			return SyntaxFactory.AddRemoveHandlerStatement(kind, keywordSyntax, expressionSyntax, token, expressionSyntax2);
		}

		private StatementSyntax ParseExpressionBlockStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			StatementSyntax result = null;
			switch (keywordSyntax.Kind)
			{
			case SyntaxKind.WhileKeyword:
				result = SyntaxFactory.WhileStatement(keywordSyntax, expressionSyntax);
				break;
			case SyntaxKind.WithKeyword:
				result = SyntaxFactory.WithStatement(keywordSyntax, expressionSyntax);
				break;
			case SyntaxKind.SyncLockKeyword:
				result = SyntaxFactory.SyncLockStatement(keywordSyntax, expressionSyntax);
				break;
			}
			return result;
		}

		private StatementSyntax ParseAssignmentStatement()
		{
			if (CurrentToken.Kind == SyntaxKind.SetKeyword && (IsValidStatementTerminator(PeekToken(1)) || PeekToken(1).Kind == SyntaxKind.OpenParenToken) && BlockContextExtensions.IsWithin(Context, SyntaxKind.SetAccessorBlock, SyntaxKind.GetAccessorBlock))
			{
				return ParsePropertyOrEventAccessor(SyntaxKind.SetAccessorStatement, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax>));
			}
			SyntaxToken currentToken = CurrentToken;
			GetNextToken();
			return SyntaxNodeExtensions.AddTrailingSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(), currentToken, ERRID.ERR_ObsoleteLetSetNotNeeded);
		}

		private TryStatementSyntax ParseTry()
		{
			KeywordSyntax tryKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			return SyntaxFactory.TryStatement(tryKeyword);
		}

		private CatchStatementSyntax ParseCatch()
		{
			KeywordSyntax catchKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			IdentifierNameSyntax identifierName = null;
			SimpleAsClauseSyntax asClause = null;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
			{
				IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
				if (identifierTokenSyntax.Kind != 0)
				{
					identifierName = SyntaxFactory.IdentifierName(identifierTokenSyntax);
				}
				KeywordSyntax token = null;
				TypeSyntax typeSyntax = null;
				if (TryGetToken(SyntaxKind.AsKeyword, ref token))
				{
					bool allowedEmptyGenericArguments = false;
					typeSyntax = ParseTypeName(nonArrayName: false, allowEmptyGenericArguments: false, ref allowedEmptyGenericArguments);
					if (typeSyntax.ContainsDiagnostics)
					{
						typeSyntax = ResyncAt(typeSyntax, SyntaxKind.WhenKeyword);
					}
					asClause = SyntaxFactory.SimpleAsClause(token, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), typeSyntax);
				}
			}
			CatchFilterClauseSyntax whenClause = null;
			KeywordSyntax token2 = null;
			ExpressionSyntax expressionSyntax = null;
			if (TryGetToken(SyntaxKind.WhenKeyword, ref token2))
			{
				expressionSyntax = ParseExpressionCore();
				whenClause = SyntaxFactory.CatchFilterClause(token2, expressionSyntax);
			}
			return SyntaxFactory.CatchStatement(catchKeyword, identifierName, asClause, whenClause);
		}

		private FinallyStatementSyntax ParseFinally()
		{
			KeywordSyntax finallyKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			return SyntaxFactory.FinallyStatement(finallyKeyword);
		}

		private ThrowStatementSyntax ParseThrowStatement()
		{
			KeywordSyntax throwKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore(OperatorPrecedence.PrecedenceNone, bailIfFirstTokenRejected: true);
			if (expressionSyntax != null && expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.ThrowStatement(throwKeyword, expressionSyntax);
		}

		private ErrorStatementSyntax ParseError()
		{
			KeywordSyntax errorKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.ErrorStatement(errorKeyword, expressionSyntax);
		}

		private EraseStatementSyntax ParseErase()
		{
			KeywordSyntax eraseKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> separatedSyntaxList = ParseVariableList();
			return SyntaxFactory.EraseStatement(eraseKeyword, separatedSyntaxList);
		}

		private bool ShouldParseAsLabel()
		{
			if (IsFirstStatementOnLine(CurrentToken))
			{
				return PeekToken(1).Kind == SyntaxKind.ColonToken;
			}
			return false;
		}

		private LabelStatementSyntax ParseLabel()
		{
			SyntaxToken syntaxToken = ParseLabelReference();
			if (syntaxToken.Kind == SyntaxKind.IntegerLiteralToken && CurrentToken.Kind != SyntaxKind.ColonToken)
			{
				return ReportSyntaxError(SyntaxFactory.LabelStatement(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken)), ERRID.ERR_ObsoleteLineNumbersAreLabels);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(syntaxToken.GetTrailingTrivia());
			int num = -1;
			int num2 = trivia.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				if (trivia[i]!.Kind == SyntaxKind.ColonTrivia)
				{
					num = i;
					break;
				}
			}
			syntaxToken = (SyntaxToken)syntaxToken.WithTrailingTrivia(SyntaxNodeExtensions.GetStartOfTrivia(trivia, num).Node);
			SyntaxTrivia syntaxTrivia = (SyntaxTrivia)trivia[num];
			PunctuationSyntax colonToken = new PunctuationSyntax(trailingTrivia: SyntaxNodeExtensions.GetEndOfTrivia(trivia, num + 1).Node, kind: SyntaxKind.ColonToken, text: syntaxTrivia.Text, leadingTrivia: null);
			return SyntaxFactory.LabelStatement(syntaxToken, colonToken);
		}

		private AssignmentStatementSyntax ParseMid()
		{
			IdentifierTokenSyntax mid = (IdentifierTokenSyntax)CurrentToken;
			GetNextToken();
			PunctuationSyntax token = null;
			TryGetTokenAndEatNewLine(SyntaxKind.OpenParenToken, ref token);
			SeparatedSyntaxListBuilder<ArgumentSyntax> item = _pool.AllocateSeparated<ArgumentSyntax>();
			PunctuationSyntax token2 = null;
			item.Add(ParseArgument());
			if (!TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
			{
				VerifyExpectedToken(SyntaxKind.CommaToken, ref token2);
			}
			item.AddSeparator(token2);
			item.Add(ParseArgument());
			if (TryGetTokenAndEatNewLine(SyntaxKind.CommaToken, ref token2))
			{
				item.AddSeparator(token2);
				item.Add(ParseArgument());
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			PunctuationSyntax token3 = null;
			TryEatNewLineAndGetToken(SyntaxKind.CloseParenToken, ref token3, createIfMissing: true);
			PunctuationSyntax token4 = null;
			VerifyExpectedToken(SyntaxKind.EqualsToken, ref token4);
			ExpressionSyntax expressionSyntax = ParseExpressionCore();
			if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.MidAssignmentStatement(SyntaxFactory.MidExpression(mid, SyntaxFactory.ArgumentList(token, separatedSyntaxList, token3)), token4, expressionSyntax);
		}

		private bool TryParseOptionalWhileOrUntilClause(KeywordSyntax precedingKeyword, ref WhileOrUntilClauseSyntax optionalWhileOrUntilClause)
		{
			if (!CanFollowStatement(CurrentToken))
			{
				KeywordSyntax k = null;
				if (CurrentToken.Kind == SyntaxKind.WhileKeyword)
				{
					k = (KeywordSyntax)CurrentToken;
				}
				else
				{
					TryTokenAsContextualKeyword(CurrentToken, SyntaxKind.UntilKeyword, ref k);
				}
				if (k != null)
				{
					GetNextToken();
					ExpressionSyntax expressionSyntax = ParseExpressionCore();
					if (expressionSyntax.ContainsDiagnostics)
					{
						expressionSyntax = ResyncAt(expressionSyntax);
					}
					SyntaxKind kind = ((k.Kind != SyntaxKind.WhileKeyword) ? SyntaxKind.UntilClause : SyntaxKind.WhileClause);
					optionalWhileOrUntilClause = SyntaxFactory.WhileOrUntilClause(kind, k, expressionSyntax);
					return true;
				}
				SyntaxKind kind2;
				if (precedingKeyword.Kind == SyntaxKind.DoKeyword)
				{
					kind2 = SyntaxKind.UntilClause;
					k = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.UntilKeyword);
				}
				else
				{
					kind2 = SyntaxKind.WhileClause;
					k = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword);
				}
				WhileOrUntilClauseSyntax syntax = SyntaxFactory.WhileOrUntilClause(kind2, k, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression());
				optionalWhileOrUntilClause = ReportSyntaxError(ResyncAt(syntax), ERRID.ERR_Syntax);
				return true;
			}
			return false;
		}

		private ReturnStatementSyntax ParseReturnStatement()
		{
			KeywordSyntax returnKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			_ = CurrentToken;
			ExpressionSyntax expressionSyntax = ParseExpressionCore(OperatorPrecedence.PrecedenceNone, bailIfFirstTokenRejected: true);
			if (expressionSyntax == null)
			{
				if (!CanFollowStatement(CurrentToken))
				{
					expressionSyntax = ParseExpressionCore();
				}
			}
			else if (expressionSyntax.ContainsDiagnostics)
			{
				expressionSyntax = ResyncAt(expressionSyntax);
			}
			return SyntaxFactory.ReturnStatement(returnKeyword, expressionSyntax);
		}

		private StopOrEndStatementSyntax ParseStopOrEndStatement()
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)CurrentToken;
			GetNextToken();
			SyntaxKind kind = ((keywordSyntax.Kind == SyntaxKind.StopKeyword) ? SyntaxKind.StopStatement : SyntaxKind.EndStatement);
			return SyntaxFactory.StopOrEndStatement(kind, keywordSyntax);
		}

		private UsingStatementSyntax ParseUsingStatement()
		{
			KeywordSyntax usingKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expression = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> separatedSyntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax>);
			SyntaxToken syntaxToken = PeekToken(1);
			if (syntaxToken.Kind == SyntaxKind.AsKeyword || syntaxToken.Kind == SyntaxKind.EqualsToken || syntaxToken.Kind == SyntaxKind.CommaToken || syntaxToken.Kind == SyntaxKind.QuestionToken)
			{
				separatedSyntaxList = ParseVariableDeclaration(allowAsNewWith: true);
			}
			else
			{
				expression = ParseExpressionCore();
			}
			return SyntaxFactory.UsingStatement(usingKeyword, expression, separatedSyntaxList);
		}

		private ExpressionStatementSyntax ParseAwaitStatement()
		{
			AwaitExpressionSyntax awaitExpressionSyntax = ParseAwaitExpression();
			if (awaitExpressionSyntax.ContainsDiagnostics)
			{
				awaitExpressionSyntax = ResyncAt(awaitExpressionSyntax);
			}
			return SyntaxFactory.ExpressionStatement(awaitExpressionSyntax);
		}

		private YieldStatementSyntax ParseYieldStatement()
		{
			KeywordSyntax k = null;
			TryIdentifierAsContextualKeyword(CurrentToken, ref k);
			k = CheckFeatureAvailability(Feature.Iterators, k);
			GetNextToken();
			ExpressionSyntax expression = ParseExpressionCore();
			return SyntaxFactory.YieldStatement(k, expression);
		}

		private PrintStatementSyntax ParsePrintStatement()
		{
			PunctuationSyntax questionToken = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			ExpressionSyntax expression = ParseExpressionCore();
			PrintStatementSyntax printStatementSyntax = SyntaxFactory.PrintStatement(questionToken, expression);
			if (PeekToken(1).Kind != SyntaxKind.EndOfFileToken || _scanner.Options.Kind == SourceCodeKind.Regular)
			{
				printStatementSyntax = SyntaxNodeExtensions.AddError(printStatementSyntax, ERRID.ERR_UnexpectedExpressionStatement);
			}
			return printStatementSyntax;
		}

		private IdentifierTokenSyntax ParseIdentifier()
		{
			IdentifierTokenSyntax identifierTokenSyntax;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
			{
				identifierTokenSyntax = (IdentifierTokenSyntax)CurrentToken;
				if ((identifierTokenSyntax.ContextualKind == SyntaxKind.AwaitKeyword && IsWithinAsyncMethodOrLambda) || (identifierTokenSyntax.ContextualKind == SyntaxKind.YieldKeyword && IsWithinIteratorContext))
				{
					identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_InvalidUseOfKeyword);
				}
				GetNextToken();
			}
			else if (CurrentToken.IsKeyword)
			{
				KeywordSyntax keyword = (KeywordSyntax)CurrentToken;
				identifierTokenSyntax = _scanner.MakeIdentifier(keyword);
				identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_InvalidUseOfKeyword);
				GetNextToken();
			}
			else
			{
				identifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				if (CurrentToken.Kind == SyntaxKind.BadToken && EmbeddedOperators.CompareString(CurrentToken.Text, "_", TextCompare: false) == 0)
				{
					identifierTokenSyntax = SyntaxNodeExtensions.AddLeadingSyntax(identifierTokenSyntax, CurrentToken, ERRID.ERR_ExpectedIdentifier);
					GetNextToken();
				}
				else
				{
					identifierTokenSyntax = ReportSyntaxError(identifierTokenSyntax, ERRID.ERR_ExpectedIdentifier);
				}
			}
			return identifierTokenSyntax;
		}

		private IdentifierTokenSyntax ParseNullableIdentifier(ref PunctuationSyntax optionalNullable)
		{
			IdentifierTokenSyntax identifierTokenSyntax = ParseIdentifier();
			if (SyntaxKind.QuestionToken == CurrentToken.Kind && !identifierTokenSyntax.ContainsDiagnostics)
			{
				optionalNullable = (PunctuationSyntax)CurrentToken;
				GetNextToken();
			}
			return identifierTokenSyntax;
		}

		private IdentifierTokenSyntax ParseIdentifierAllowingKeyword()
		{
			IdentifierTokenSyntax result;
			if (CurrentToken.Kind == SyntaxKind.IdentifierToken)
			{
				result = (IdentifierTokenSyntax)CurrentToken;
				GetNextToken();
			}
			else if (CurrentToken.IsKeyword)
			{
				KeywordSyntax keyword = (KeywordSyntax)CurrentToken;
				result = _scanner.MakeIdentifier(keyword);
				GetNextToken();
			}
			else
			{
				result = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier();
				result = ReportSyntaxError(result, ERRID.ERR_ExpectedIdentifier);
			}
			return result;
		}

		private IdentifierNameSyntax ParseIdentifierNameAllowingKeyword()
		{
			IdentifierTokenSyntax identifier = ParseIdentifierAllowingKeyword();
			return SyntaxFactory.IdentifierName(identifier);
		}

		private SimpleNameSyntax ParseSimpleNameExpressionAllowingKeywordAndTypeArguments()
		{
			IdentifierTokenSyntax identifier = ParseIdentifierAllowingKeyword();
			if (!_evaluatingConditionCompilationExpression && BeginsGeneric(nonArrayName: false, allowGenericsWithoutOf: true))
			{
				bool allowEmptyGenericArguments = false;
				bool AllowNonEmptyGenericArguments = true;
				TypeArgumentListSyntax typeArgumentList = ParseGenericArguments(ref allowEmptyGenericArguments, ref AllowNonEmptyGenericArguments);
				return SyntaxFactory.GenericName(identifier, typeArgumentList);
			}
			return SyntaxFactory.IdentifierName(identifier);
		}

		private LiteralExpressionSyntax ParseIntLiteral()
		{
			LiteralExpressionSyntax result = SyntaxFactory.NumericLiteralExpression(CurrentToken);
			GetNextToken();
			return result;
		}

		private LiteralExpressionSyntax ParseCharLiteral()
		{
			LiteralExpressionSyntax result = SyntaxFactory.CharacterLiteralExpression(CurrentToken);
			GetNextToken();
			return result;
		}

		private LiteralExpressionSyntax ParseDecLiteral()
		{
			LiteralExpressionSyntax result = SyntaxFactory.NumericLiteralExpression(CurrentToken);
			GetNextToken();
			return result;
		}

		private LiteralExpressionSyntax ParseStringLiteral()
		{
			SyntaxToken token = null;
			VerifyExpectedToken(SyntaxKind.StringLiteralToken, ref token);
			return SyntaxFactory.StringLiteralExpression(token);
		}

		private LiteralExpressionSyntax ParseFltLiteral()
		{
			LiteralExpressionSyntax result = SyntaxFactory.NumericLiteralExpression(CurrentToken);
			GetNextToken();
			return result;
		}

		private LiteralExpressionSyntax ParseDateLiteral()
		{
			LiteralExpressionSyntax result = SyntaxFactory.DateLiteralExpression(CurrentToken);
			GetNextToken();
			return result;
		}

		private static SyntaxToken HandleUnexpectedToken(SyntaxKind kind)
		{
			ERRID unexpectedTokenErrorId = GetUnexpectedTokenErrorId(kind);
			return ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(kind), unexpectedTokenErrorId);
		}

		private static KeywordSyntax HandleUnexpectedKeyword(SyntaxKind kind)
		{
			ERRID unexpectedTokenErrorId = GetUnexpectedTokenErrorId(kind);
			return ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(kind), unexpectedTokenErrorId);
		}

		private static ERRID GetUnexpectedTokenErrorId(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AsKeyword:
				return ERRID.ERR_ExpectedAs;
			case SyntaxKind.ByKeyword:
				return ERRID.ERR_ExpectedBy;
			case SyntaxKind.CloseBraceToken:
				return ERRID.ERR_ExpectedRbrace;
			case SyntaxKind.CloseParenToken:
				return ERRID.ERR_ExpectedRparen;
			case SyntaxKind.CommaToken:
				return ERRID.ERR_ExpectedComma;
			case SyntaxKind.DoubleQuoteToken:
				return ERRID.ERR_ExpectedQuote;
			case SyntaxKind.DotToken:
				return ERRID.ERR_ExpectedDot;
			case SyntaxKind.EndCDataToken:
				return ERRID.ERR_ExpectedXmlEndCData;
			case SyntaxKind.EqualsKeyword:
				return ERRID.ERR_ExpectedEquals;
			case SyntaxKind.EqualsToken:
				return ERRID.ERR_ExpectedEQ;
			case SyntaxKind.GreaterThanToken:
				return ERRID.ERR_ExpectedGreater;
			case SyntaxKind.IdentifierToken:
				return ERRID.ERR_ExpectedIdentifier;
			case SyntaxKind.IntegerLiteralToken:
				return ERRID.ERR_ExpectedIntLiteral;
			case SyntaxKind.InKeyword:
				return ERRID.ERR_ExpectedIn;
			case SyntaxKind.IntoKeyword:
				return ERRID.ERR_ExpectedInto;
			case SyntaxKind.IsKeyword:
				return ERRID.ERR_MissingIsInTypeOf;
			case SyntaxKind.JoinKeyword:
				return ERRID.ERR_ExpectedJoin;
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanSlashToken:
				return ERRID.ERR_ExpectedLT;
			case SyntaxKind.LessThanPercentEqualsToken:
				return ERRID.ERR_ExpectedXmlBeginEmbedded;
			case SyntaxKind.LibKeyword:
				return ERRID.ERR_MissingLibInDeclare;
			case SyntaxKind.MinusToken:
				return ERRID.ERR_ExpectedMinus;
			case SyntaxKind.MinusMinusGreaterThanToken:
				return ERRID.ERR_ExpectedXmlEndComment;
			case SyntaxKind.NextKeyword:
				return ERRID.ERR_MissingNext;
			case SyntaxKind.OfKeyword:
				return ERRID.ERR_OfExpected;
			case SyntaxKind.OnKeyword:
				return ERRID.ERR_ExpectedOn;
			case SyntaxKind.OpenBraceToken:
				return ERRID.ERR_ExpectedLbrace;
			case SyntaxKind.OpenParenToken:
				return ERRID.ERR_ExpectedLparen;
			case SyntaxKind.PercentGreaterThanToken:
				return ERRID.ERR_ExpectedXmlEndEmbedded;
			case SyntaxKind.QuestionGreaterThanToken:
				return ERRID.ERR_ExpectedXmlEndPI;
			case SyntaxKind.SemicolonToken:
				return ERRID.ERR_ExpectedSColon;
			case SyntaxKind.SingleQuoteToken:
				return ERRID.ERR_ExpectedSQuote;
			case SyntaxKind.SlashToken:
				return ERRID.ERR_ExpectedDiv;
			case SyntaxKind.StringLiteralToken:
				return ERRID.ERR_ExpectedStringLiteral;
			case SyntaxKind.XmlNameToken:
				return ERRID.ERR_ExpectedXmlName;
			case SyntaxKind.WarningKeyword:
				return ERRID.ERR_ExpectedWarningKeyword;
			default:
				return ERRID.ERR_Syntax;
			}
		}

		private bool VerifyExpectedToken<T>(SyntaxKind kind, ref T token, ScannerState state = ScannerState.VB) where T : SyntaxToken
		{
			SyntaxToken currentToken = CurrentToken;
			if (currentToken.Kind == kind)
			{
				token = (T)currentToken;
				GetNextToken(state);
				return true;
			}
			token = (T)HandleUnexpectedToken(kind);
			return false;
		}

		private XmlNodeSyntax ParseXmlExpression()
		{
			ResetCurrentToken(ScannerState.Content);
			XmlNodeSyntax xmlNodeSyntax = null;
			xmlNodeSyntax = ((CurrentToken.Kind != SyntaxKind.LessThanQuestionToken) ? ParseXmlElement(ScannerState.VB) : ParseXmlDocument());
			xmlNodeSyntax = AdjustTriviaForMissingTokens(xmlNodeSyntax);
			return TransitionFromXmlToVB(xmlNodeSyntax);
		}

		private XmlNodeSyntax ParseXmlDocument()
		{
			XmlWhitespaceChecker xmlWhitespaceChecker = new XmlWhitespaceChecker();
			SyntaxToken syntaxToken = PeekNextToken(ScannerState.Element);
			if (syntaxToken.Kind == SyntaxKind.XmlNameToken && ((XmlNameTokenSyntax)syntaxToken).PossibleKeywordKind == SyntaxKind.XmlKeyword)
			{
				XmlDeclarationSyntax node = ParseXmlDeclaration();
				node = (XmlDeclarationSyntax)xmlWhitespaceChecker.Visit(node);
				VisualBasicSyntaxNode outerNode = node;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = ParseXmlMisc(IsProlog: true, xmlWhitespaceChecker, ref outerNode);
				node = (XmlDeclarationSyntax)outerNode;
				outerNode = CurrentToken.Kind switch
				{
					SyntaxKind.LessThanToken => ParseXmlElement(ScannerState.Misc), 
					SyntaxKind.LessThanPercentEqualsToken => ParseXmlEmbedded(ScannerState.Misc), 
					_ => SyntaxFactory.XmlEmptyElement((PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.LessThanToken), SyntaxFactory.XmlName(null, (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken)), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.SlashGreaterThanToken)), 
				};
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList2 = ParseXmlMisc(IsProlog: false, xmlWhitespaceChecker, ref outerNode);
				XmlNodeSyntax root = (XmlNodeSyntax)outerNode;
				return SyntaxFactory.XmlDocument(node, syntaxList, root, syntaxList2);
			}
			return ParseXmlProcessingInstruction(ScannerState.VB, xmlWhitespaceChecker);
		}

		private XmlDeclarationSyntax ParseXmlDeclaration()
		{
			PunctuationSyntax lessThanQuestionToken = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.Element);
			XmlNameTokenSyntax token = null;
			VerifyExpectedToken(SyntaxKind.XmlNameToken, ref token, ScannerState.Element);
			int num = 0;
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			VisualBasicSyntaxNode[] array = new VisualBasicSyntaxNode[4];
			int num3 = 0;
			array[num3] = _scanner.MakeKeyword(token);
			num3++;
			while (true)
			{
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.XmlNameToken:
				{
					XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)CurrentToken;
					string left = xmlNameTokenSyntax.ToString();
					XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax;
					if (EmbeddedOperators.CompareString(left, "version", TextCompare: false) != 0)
					{
						if (EmbeddedOperators.CompareString(left, "encoding", TextCompare: false) != 0)
						{
							if (EmbeddedOperators.CompareString(left, "standalone", TextCompare: false) == 0)
							{
								xmlDeclarationOptionSyntax = ParseXmlDeclarationOption();
								if (flag3)
								{
									xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, xmlNameTokenSyntax.ToString());
									array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
									break;
								}
								flag3 = true;
								if (!flag)
								{
									array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
									break;
								}
								string left2 = ((xmlDeclarationOptionSyntax.Value.TextTokens.Node != null) ? xmlDeclarationOptionSyntax.Value.TextTokens.Node!.ToFullString() : "");
								if (EmbeddedOperators.CompareString(left2, "yes", TextCompare: false) != 0 && EmbeddedOperators.CompareString(left2, "no", TextCompare: false) != 0)
								{
									xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_InvalidAttributeValue2, "yes", "no");
								}
								num2 = num3;
								array[num3] = xmlDeclarationOptionSyntax;
								num3++;
							}
							else
							{
								xmlDeclarationOptionSyntax = ParseXmlDeclarationOption();
								xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_IllegalAttributeInXmlDecl, "", "", xmlDeclarationOptionSyntax.Name.ToString());
								array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
							}
							break;
						}
						xmlDeclarationOptionSyntax = ParseXmlDeclarationOption();
						if (flag2)
						{
							xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, xmlNameTokenSyntax.ToString());
							array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
							break;
						}
						flag2 = true;
						if (flag3)
						{
							xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_AttributeOrder, "encoding", "standalone");
							array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
						}
						else if (!flag)
						{
							array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
						}
						else
						{
							num = num3;
							array[num3] = xmlDeclarationOptionSyntax;
							num3++;
						}
						break;
					}
					xmlDeclarationOptionSyntax = ParseXmlDeclarationOption();
					if (flag)
					{
						xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, xmlNameTokenSyntax.ToString());
						array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
						break;
					}
					flag = true;
					if (flag2 || flag3)
					{
						xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_VersionMustBeFirstInXmlDecl, "", "", xmlNameTokenSyntax.ToString());
					}
					if (xmlDeclarationOptionSyntax.Value.TextTokens.Node == null || EmbeddedOperators.CompareString(xmlDeclarationOptionSyntax.Value.TextTokens.Node!.ToFullString(), "1.0", TextCompare: false) != 0)
					{
						xmlDeclarationOptionSyntax = ReportSyntaxError(xmlDeclarationOptionSyntax, ERRID.ERR_InvalidAttributeValue1, "1.0");
					}
					array[num3] = xmlDeclarationOptionSyntax;
					num3++;
					break;
				}
				case SyntaxKind.LessThanPercentEqualsToken:
				{
					XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = ParseXmlDeclarationOption();
					array[num3 - 1] = SyntaxNodeExtensions.AddTrailingSyntax(array[num3 - 1], xmlDeclarationOptionSyntax);
					break;
				}
				default:
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
					if (CurrentToken.Kind != SyntaxKind.QuestionGreaterThanToken)
					{
						syntaxList = ResyncAt(ScannerState.Element, new SyntaxKind[5]
						{
							SyntaxKind.EndOfXmlToken,
							SyntaxKind.QuestionGreaterThanToken,
							SyntaxKind.LessThanToken,
							SyntaxKind.LessThanPercentEqualsToken,
							SyntaxKind.LessThanExclamationMinusMinusToken
						});
					}
					PunctuationSyntax token2 = null;
					VerifyExpectedToken(SyntaxKind.QuestionGreaterThanToken, ref token2, ScannerState.Content);
					if (syntaxList.Node != null)
					{
						token2 = SyntaxNodeExtensions.AddLeadingSyntax(token2, syntaxList, ERRID.ERR_ExpectedXmlName);
					}
					if (array[1] == null)
					{
						XmlDeclarationOptionSyntax syntax = SyntaxFactory.XmlDeclarationOption((XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), CreateMissingXmlString());
						array[1] = ReportSyntaxError(syntax, ERRID.ERR_MissingVersionInXmlDecl);
					}
					return SyntaxFactory.XmlDeclaration(lessThanQuestionToken, array[0] as KeywordSyntax, array[1] as XmlDeclarationOptionSyntax, (num == 0) ? null : (array[num] as XmlDeclarationOptionSyntax), (num2 == 0) ? null : (array[num2] as XmlDeclarationOptionSyntax), token2);
				}
				}
			}
		}

		private XmlDeclarationOptionSyntax ParseXmlDeclarationOption()
		{
			XmlNameTokenSyntax token = null;
			PunctuationSyntax token2 = null;
			XmlStringSyntax xmlStringSyntax = null;
			bool num = SyntaxNodeExtensions.ContainsWhitespaceTrivia(PrevToken.GetTrailingTrivia()) || SyntaxNodeExtensions.ContainsWhitespaceTrivia(CurrentToken.GetLeadingTrivia());
			VerifyExpectedToken(SyntaxKind.XmlNameToken, ref token, ScannerState.Element);
			if (!num)
			{
				token = ReportSyntaxError(token, ERRID.ERR_ExpectedXmlWhiteSpace);
			}
			if (CurrentToken.Kind == SyntaxKind.LessThanPercentEqualsToken)
			{
				XmlEmbeddedExpressionSyntax unexpected = ParseXmlEmbedded(ScannerState.Element);
				token = SyntaxNodeExtensions.AddTrailingSyntax(token, unexpected, ERRID.ERR_EmbeddedExpression);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			if (!VerifyExpectedToken(SyntaxKind.EqualsToken, ref token2, ScannerState.Element))
			{
				syntaxList = ResyncAt(ScannerState.Element, new SyntaxKind[5]
				{
					SyntaxKind.SingleQuoteToken,
					SyntaxKind.DoubleQuoteToken,
					SyntaxKind.LessThanPercentEqualsToken,
					SyntaxKind.QuestionGreaterThanToken,
					SyntaxKind.EndOfXmlToken
				});
				token2 = SyntaxNodeExtensions.AddTrailingSyntax(token2, syntaxList);
			}
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.SingleQuoteToken:
			case SyntaxKind.DoubleQuoteToken:
				xmlStringSyntax = ParseXmlString(ScannerState.Element);
				break;
			case SyntaxKind.LessThanPercentEqualsToken:
			{
				XmlEmbeddedExpressionSyntax unexpected2 = ParseXmlEmbedded(ScannerState.Element);
				xmlStringSyntax = SyntaxNodeExtensions.AddLeadingSyntax(CreateMissingXmlString(), unexpected2, ERRID.ERR_EmbeddedExpression);
				break;
			}
			default:
				xmlStringSyntax = CreateMissingXmlString();
				break;
			}
			return SyntaxFactory.XmlDeclarationOption(token, token2, xmlStringSyntax);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> ParseXmlMisc(bool IsProlog, XmlWhitespaceChecker whitespaceChecker, ref VisualBasicSyntaxNode outerNode)
		{
			SyntaxListBuilder<XmlNodeSyntax> syntaxListBuilder = _pool.Allocate<XmlNodeSyntax>();
			while (true)
			{
				XmlNodeSyntax xmlNodeSyntax = null;
				SyntaxKind kind = CurrentToken.Kind;
				if (kind != SyntaxKind.LessThanExclamationMinusMinusToken)
				{
					if (kind != SyntaxKind.LessThanQuestionToken)
					{
						if (kind != SyntaxKind.BadToken)
						{
							break;
						}
						BadTokenSyntax badTokenSyntax = (BadTokenSyntax)CurrentToken;
						GreenNode unexpected;
						if (badTokenSyntax.SubKind == SyntaxSubKind.BeginDocTypeToken)
						{
							unexpected = ParseXmlDocType(ScannerState.Misc);
						}
						else
						{
							unexpected = badTokenSyntax;
							GetNextToken(ScannerState.Misc);
						}
						int count = syntaxListBuilder.Count;
						if (count > 0)
						{
							syntaxListBuilder[count - 1] = SyntaxNodeExtensions.AddTrailingSyntax(syntaxListBuilder[count - 1], unexpected, ERRID.ERR_DTDNotSupported);
						}
						else
						{
							outerNode = SyntaxNodeExtensions.AddTrailingSyntax(outerNode, unexpected, ERRID.ERR_DTDNotSupported);
						}
					}
					else
					{
						xmlNodeSyntax = ParseXmlProcessingInstruction(ScannerState.Misc, whitespaceChecker);
					}
				}
				else
				{
					xmlNodeSyntax = ParseXmlComment(ScannerState.Misc);
				}
				if (xmlNodeSyntax != null)
				{
					syntaxListBuilder.Add(xmlNodeSyntax);
				}
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private GreenNode ParseXmlDocType(ScannerState enclosingState)
		{
			SyntaxListBuilder<GreenNode> builder = SyntaxListBuilder<GreenNode>.Create();
			BadTokenSyntax node = (BadTokenSyntax)CurrentToken;
			builder.Add(node);
			XmlNameTokenSyntax token = null;
			GetNextToken(ScannerState.DocType);
			VerifyExpectedToken(SyntaxKind.XmlNameToken, ref token, ScannerState.DocType);
			builder.Add(token);
			ParseExternalID(builder);
			ParseInternalSubSet(builder);
			PunctuationSyntax token2 = null;
			VerifyExpectedToken(SyntaxKind.GreaterThanToken, ref token2, enclosingState);
			builder.Add(token2);
			return builder.ToList().Node;
		}

		private void ParseExternalID(SyntaxListBuilder<GreenNode> builder)
		{
			if (CurrentToken.Kind != SyntaxKind.XmlNameToken)
			{
				return;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)CurrentToken;
			string left = xmlNameTokenSyntax.ToString();
			if (EmbeddedOperators.CompareString(left, "SYSTEM", TextCompare: false) != 0)
			{
				if (EmbeddedOperators.CompareString(left, "PUBLIC", TextCompare: false) == 0)
				{
					builder.Add(xmlNameTokenSyntax);
					GetNextToken(ScannerState.DocType);
					XmlStringSyntax node = ParseXmlString(ScannerState.DocType);
					builder.Add(node);
					XmlStringSyntax node2 = ParseXmlString(ScannerState.DocType);
					builder.Add(node2);
				}
			}
			else
			{
				builder.Add(xmlNameTokenSyntax);
				GetNextToken(ScannerState.DocType);
				XmlStringSyntax node3 = ParseXmlString(ScannerState.DocType);
				builder.Add(node3);
			}
		}

		private void ParseInternalSubSet(SyntaxListBuilder<GreenNode> builder)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			if (CurrentToken.Kind != SyntaxKind.BadToken || ((BadTokenSyntax)CurrentToken).SubKind != SyntaxSubKind.OpenBracketToken)
			{
				syntaxList = ResyncAt(ScannerState.DocType, new SyntaxKind[7]
				{
					SyntaxKind.BadToken,
					SyntaxKind.GreaterThanToken,
					SyntaxKind.LessThanToken,
					SyntaxKind.LessThanExclamationMinusMinusToken,
					SyntaxKind.BeginCDataToken,
					SyntaxKind.LessThanPercentEqualsToken,
					SyntaxKind.EndOfXmlToken
				});
				if (syntaxList.Node != null)
				{
					builder.Add(syntaxList.Node);
				}
			}
			if (CurrentToken.Kind != SyntaxKind.BadToken || ((BadTokenSyntax)CurrentToken).SubKind != SyntaxSubKind.OpenBracketToken)
			{
				return;
			}
			builder.Add(CurrentToken);
			GetNextToken(ScannerState.DocType);
			if (CurrentToken.Kind == SyntaxKind.BadToken && ((BadTokenSyntax)CurrentToken).SubKind == SyntaxSubKind.LessThanExclamationToken)
			{
				builder.Add(CurrentToken);
				GetNextToken(ScannerState.DocType);
				ParseXmlMarkupDecl(builder);
			}
			if (CurrentToken.Kind != SyntaxKind.BadToken || ((BadTokenSyntax)CurrentToken).SubKind != SyntaxSubKind.CloseBracketToken)
			{
				syntaxList = ResyncAt(ScannerState.DocType, new SyntaxKind[7]
				{
					SyntaxKind.BadToken,
					SyntaxKind.GreaterThanToken,
					SyntaxKind.LessThanToken,
					SyntaxKind.LessThanExclamationMinusMinusToken,
					SyntaxKind.BeginCDataToken,
					SyntaxKind.LessThanPercentEqualsToken,
					SyntaxKind.EndOfXmlToken
				});
				if (syntaxList.Node != null)
				{
					builder.Add(syntaxList.Node);
				}
			}
			builder.Add(CurrentToken);
			GetNextToken(ScannerState.DocType);
		}

		private void ParseXmlMarkupDecl(SyntaxListBuilder<GreenNode> builder)
		{
			while (true)
			{
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.BadToken:
				{
					builder.Add(CurrentToken);
					BadTokenSyntax obj = (BadTokenSyntax)CurrentToken;
					GetNextToken(ScannerState.DocType);
					if (obj.SubKind == SyntaxSubKind.LessThanExclamationToken)
					{
						ParseXmlMarkupDecl(builder);
					}
					break;
				}
				case SyntaxKind.LessThanQuestionToken:
				{
					XmlProcessingInstructionSyntax node2 = ParseXmlProcessingInstruction(ScannerState.DocType, null);
					builder.Add(node2);
					break;
				}
				case SyntaxKind.LessThanExclamationMinusMinusToken:
				{
					XmlNodeSyntax node = ParseXmlComment(ScannerState.DocType);
					builder.Add(node);
					break;
				}
				case SyntaxKind.GreaterThanToken:
					builder.Add(CurrentToken);
					GetNextToken(ScannerState.DocType);
					return;
				default:
					builder.Add(CurrentToken);
					GetNextToken(ScannerState.DocType);
					break;
				case SyntaxKind.EndOfFileToken:
				case SyntaxKind.EndOfXmlToken:
					return;
				}
			}
		}

		private XmlNodeSyntax ParseXmlElementStartTag(ScannerState enclosingState)
		{
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.Element);
			XmlNodeSyntax xmlNodeSyntax = ParseXmlQualifiedName(requireLeadingWhitespace: false, allowExpr: true, ScannerState.Element, ScannerState.Element);
			bool hasTrailingTrivia = xmlNodeSyntax.HasTrailingTrivia;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = ParseXmlAttributes(!hasTrailingTrivia, xmlNodeSyntax);
			PunctuationSyntax punctuationSyntax2 = null;
			PunctuationSyntax punctuationSyntax3 = null;
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.GreaterThanToken:
				punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
				GetNextToken(ScannerState.Content);
				return SyntaxFactory.XmlElementStartTag(punctuationSyntax, xmlNodeSyntax, syntaxList, punctuationSyntax2);
			case SyntaxKind.SlashGreaterThanToken:
				punctuationSyntax3 = (PunctuationSyntax)CurrentToken;
				GetNextToken(enclosingState);
				return SyntaxFactory.XmlEmptyElement(punctuationSyntax, xmlNodeSyntax, syntaxList, punctuationSyntax3);
			case SyntaxKind.SlashToken:
				if (PeekNextToken(ScannerState.Element).Kind == SyntaxKind.GreaterThanToken)
				{
					SyntaxToken currentToken = CurrentToken;
					GetNextToken(ScannerState.Element);
					punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
					GetNextToken(enclosingState);
					punctuationSyntax3 = SyntaxNodeExtensions.AddLeadingSyntax(unexpected: new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(SyntaxList.List(currentToken, punctuationSyntax2)), node: new PunctuationSyntax(SyntaxKind.SlashGreaterThanToken, "", null, null), errorId: ERRID.ERR_IllegalXmlWhiteSpace);
					return SyntaxFactory.XmlEmptyElement(punctuationSyntax, xmlNodeSyntax, syntaxList, punctuationSyntax3);
				}
				return ResyncXmlElement(enclosingState, punctuationSyntax, xmlNodeSyntax, syntaxList);
			default:
				return ResyncXmlElement(enclosingState, punctuationSyntax, xmlNodeSyntax, syntaxList);
			}
		}

		private XmlNodeSyntax ParseXmlElement(ScannerState enclosingState)
		{
			XmlNodeSyntax xmlNodeSyntax = null;
			List<XmlContext> list = new List<XmlContext>();
			ScannerState scannerState = enclosingState;
			XmlWhitespaceChecker xmlWhitespaceChecker = new XmlWhitespaceChecker();
			while (true)
			{
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.LessThanToken:
					if (PeekNextToken(ScannerState.Element).Kind == SyntaxKind.SlashToken)
					{
						goto case SyntaxKind.LessThanSlashToken;
					}
					xmlNodeSyntax = ParseXmlElementStartTag(scannerState);
					xmlNodeSyntax = (XmlNodeSyntax)xmlWhitespaceChecker.Visit(xmlNodeSyntax);
					if (xmlNodeSyntax.Kind == SyntaxKind.XmlElementStartTag)
					{
						XmlElementStartTagSyntax start = (XmlElementStartTagSyntax)xmlNodeSyntax;
						XmlContextExtensions.Push(list, new XmlContext(_pool, start));
						scannerState = ScannerState.Content;
						continue;
					}
					goto IL_02b2;
				case SyntaxKind.LessThanSlashToken:
				{
					XmlElementEndTagSyntax node = ParseXmlElementEndTag(scannerState);
					node = (XmlElementEndTagSyntax)xmlWhitespaceChecker.Visit(node);
					if (list.Count > 0)
					{
						xmlNodeSyntax = CreateXmlElement(list, node);
					}
					else
					{
						PunctuationSyntax lessThanToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.LessThanToken);
						XmlNameTokenSyntax localName = (XmlNameTokenSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken);
						XmlNameSyntax name = SyntaxFactory.XmlName(null, localName);
						PunctuationSyntax greaterThanToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanToken);
						XmlElementStartTagSyntax start2 = SyntaxFactory.XmlElementStartTag(lessThanToken, name, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), greaterThanToken);
						XmlContextExtensions.Push(list, new XmlContext(_pool, start2));
						xmlNodeSyntax = XmlContextExtensions.Peek(list).CreateElement(node);
						xmlNodeSyntax = ReportSyntaxError(xmlNodeSyntax, ERRID.ERR_XmlEndElementNoMatchingStart);
						XmlContextExtensions.Pop(list);
					}
					goto IL_02b2;
				}
				case SyntaxKind.LessThanExclamationMinusMinusToken:
					xmlNodeSyntax = ParseXmlComment(scannerState);
					goto IL_02b2;
				case SyntaxKind.LessThanQuestionToken:
					xmlNodeSyntax = ParseXmlProcessingInstruction(scannerState, xmlWhitespaceChecker);
					xmlNodeSyntax = (XmlProcessingInstructionSyntax)xmlWhitespaceChecker.Visit(xmlNodeSyntax);
					goto IL_02b2;
				case SyntaxKind.BeginCDataToken:
					xmlNodeSyntax = ParseXmlCData(scannerState);
					goto IL_02b2;
				case SyntaxKind.LessThanPercentEqualsToken:
					xmlNodeSyntax = ParseXmlEmbedded(scannerState);
					if (list.Count == 0)
					{
						xmlNodeSyntax = ReportSyntaxError(xmlNodeSyntax, ERRID.ERR_EmbeddedExpression);
					}
					goto IL_02b2;
				case SyntaxKind.XmlTextLiteralToken:
				case SyntaxKind.XmlEntityLiteralToken:
				case SyntaxKind.DocumentationCommentLineBreakToken:
				{
					SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder = _pool.Allocate<XmlTextTokenSyntax>();
					SyntaxKind kind;
					do
					{
						syntaxListBuilder.Add((XmlTextTokenSyntax)CurrentToken);
						GetNextToken(scannerState);
						kind = CurrentToken.Kind;
					}
					while (kind == SyntaxKind.XmlTextLiteralToken || kind == SyntaxKind.XmlEntityLiteralToken || kind == SyntaxKind.DocumentationCommentLineBreakToken);
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = syntaxListBuilder.ToList();
					_pool.Free(syntaxListBuilder);
					xmlNodeSyntax = SyntaxFactory.XmlText(syntaxList);
					goto IL_02b2;
				}
				case SyntaxKind.BadToken:
					{
						if (((BadTokenSyntax)CurrentToken).SubKind != SyntaxSubKind.BeginDocTypeToken)
						{
							break;
						}
						GreenNode unexpected = ParseXmlDocType(ScannerState.Element);
						xmlNodeSyntax = SyntaxFactory.XmlText(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlTextLiteralToken));
						xmlNodeSyntax = SyntaxNodeExtensions.AddLeadingSyntax(xmlNodeSyntax, unexpected, ERRID.ERR_DTDNotSupported);
						goto IL_02b2;
					}
					IL_02b2:
					if (list.Count > 0)
					{
						XmlContextExtensions.Peek(list).Add(xmlNodeSyntax);
						continue;
					}
					break;
				}
				break;
			}
			if (list.Count > 0)
			{
				while (true)
				{
					XmlElementEndTagSyntax node = ParseXmlElementEndTag(scannerState);
					xmlNodeSyntax = CreateXmlElement(list, node);
					if (list.Count <= 0)
					{
						break;
					}
					XmlContextExtensions.Peek(list).Add(xmlNodeSyntax);
				}
			}
			ResetCurrentToken(enclosingState);
			return xmlNodeSyntax;
		}

		private XmlNodeSyntax CreateXmlElement(List<XmlContext> contexts, XmlElementEndTagSyntax endElement)
		{
			int num = XmlContextExtensions.MatchEndElement(contexts, endElement.Name);
			XmlNodeSyntax result;
			if (num >= 0)
			{
				for (int num2 = contexts.Count - 1; num2 > num; num2--)
				{
					XmlElementEndTagSyntax endElement2 = SyntaxFactory.XmlElementEndTag((PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.LessThanSlashToken), ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlName(null, SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, null, null)), ERRID.ERR_ExpectedXmlName), (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.GreaterThanToken));
					XmlNodeSyntax xml = XmlContextExtensions.Peek(contexts).CreateElement(endElement2, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag));
					XmlContextExtensions.Pop(contexts);
					if (contexts.Count <= 0)
					{
						break;
					}
					XmlContextExtensions.Peek(contexts).Add(xml);
				}
				result = ((!endElement.IsMissing) ? XmlContextExtensions.Peek(contexts).CreateElement(endElement) : XmlContextExtensions.Peek(contexts).CreateElement(endElement, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag)));
			}
			else
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				XmlNodeSyntax name = XmlContextExtensions.Peek(contexts).StartElement.Name;
				if (name.Kind == SyntaxKind.XmlName)
				{
					XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)name;
					if (xmlNameSyntax.Prefix != null)
					{
						text = xmlNameSyntax.Prefix.Name.Text;
						text2 = ":";
					}
					text3 = xmlNameSyntax.LocalName.Text;
				}
				endElement = ReportSyntaxError(endElement, ERRID.ERR_MismatchedXmlEndTag, text, text2, text3);
				result = XmlContextExtensions.Peek(contexts).CreateElement(endElement, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag));
			}
			XmlContextExtensions.Pop(contexts);
			return result;
		}

		private XmlNodeSyntax ResyncXmlElement(ScannerState state, PunctuationSyntax lessThan, XmlNodeSyntax Name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> attributes)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = ResyncAt(ScannerState.Element, new SyntaxKind[9]
			{
				SyntaxKind.SlashGreaterThanToken,
				SyntaxKind.GreaterThanToken,
				SyntaxKind.LessThanToken,
				SyntaxKind.LessThanSlashToken,
				SyntaxKind.LessThanPercentEqualsToken,
				SyntaxKind.BeginCDataToken,
				SyntaxKind.LessThanExclamationMinusMinusToken,
				SyntaxKind.LessThanQuestionToken,
				SyntaxKind.EndOfXmlToken
			});
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.SlashGreaterThanToken:
			{
				PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)CurrentToken;
				if (syntaxList.Node != null)
				{
					punctuationSyntax2 = SyntaxNodeExtensions.AddLeadingSyntax(punctuationSyntax2, syntaxList, ERRID.ERR_ExpectedGreater);
				}
				GetNextToken(state);
				return SyntaxFactory.XmlEmptyElement(lessThan, Name, attributes, punctuationSyntax2);
			}
			case SyntaxKind.GreaterThanToken:
			{
				PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken(ScannerState.Content);
				if (syntaxList.Node != null)
				{
					punctuationSyntax = SyntaxNodeExtensions.AddLeadingSyntax(punctuationSyntax, syntaxList, ERRID.ERR_ExpectedGreater);
				}
				return SyntaxFactory.XmlElementStartTag(lessThan, Name, attributes, punctuationSyntax);
			}
			default:
			{
				PunctuationSyntax punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanToken);
				if (syntaxList.Node == null)
				{
					if (attributes.Node == null || !attributes.Node!.ContainsDiagnostics)
					{
						punctuationSyntax = ReportSyntaxError(punctuationSyntax, ERRID.ERR_ExpectedGreater);
					}
				}
				else
				{
					punctuationSyntax = SyntaxNodeExtensions.AddLeadingSyntax(punctuationSyntax, syntaxList, ERRID.ERR_Syntax);
				}
				return SyntaxFactory.XmlElementStartTag(lessThan, Name, attributes, punctuationSyntax);
			}
			}
		}

		private XmlNodeSyntax ResyncXmlContent()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = ResyncAt(ScannerState.Content, new SyntaxKind[9]
			{
				SyntaxKind.LessThanToken,
				SyntaxKind.LessThanSlashToken,
				SyntaxKind.LessThanPercentEqualsToken,
				SyntaxKind.BeginCDataToken,
				SyntaxKind.LessThanExclamationMinusMinusToken,
				SyntaxKind.LessThanQuestionToken,
				SyntaxKind.EndOfXmlToken,
				SyntaxKind.XmlTextLiteralToken,
				SyntaxKind.XmlEntityLiteralToken
			});
			SyntaxKind kind = CurrentToken.Kind;
			XmlTextSyntax xmlTextSyntax;
			if (kind == SyntaxKind.XmlTextLiteralToken || kind == SyntaxKind.DocumentationCommentLineBreakToken || kind == SyntaxKind.XmlEntityLiteralToken)
			{
				xmlTextSyntax = SyntaxFactory.XmlText(CurrentToken);
				GetNextToken(ScannerState.Content);
			}
			else
			{
				xmlTextSyntax = SyntaxFactory.XmlText(HandleUnexpectedToken(SyntaxKind.XmlTextLiteralToken));
			}
			if (xmlTextSyntax.ContainsDiagnostics)
			{
				return SyntaxNodeExtensions.AddLeadingSyntax(xmlTextSyntax, syntaxList);
			}
			return SyntaxNodeExtensions.AddLeadingSyntax(xmlTextSyntax, syntaxList, ERRID.ERR_Syntax);
		}

		private XmlElementEndTagSyntax ParseXmlElementEndTag(ScannerState nextState)
		{
			PunctuationSyntax token = null;
			XmlNameSyntax name = null;
			PunctuationSyntax token2 = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>);
			if (CurrentToken.Kind != SyntaxKind.LessThanSlashToken)
			{
				syntaxList = ResyncAt(ScannerState.Content, new SyntaxKind[3]
				{
					SyntaxKind.LessThanToken,
					SyntaxKind.LessThanSlashToken,
					SyntaxKind.EndOfXmlToken
				});
			}
			if (!VerifyExpectedToken(SyntaxKind.LessThanSlashToken, ref token, ScannerState.EndElement) && CurrentToken.Kind == SyntaxKind.LessThanToken)
			{
				PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
				SyntaxToken syntaxToken = PeekNextToken(ScannerState.EndElement);
				if (syntaxToken.Kind == SyntaxKind.SlashToken)
				{
					token = ((!(punctuationSyntax.HasTrailingTrivia | syntaxToken.HasLeadingTrivia)) ? ((PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(punctuationSyntax.GetLeadingTrivia(), SyntaxKind.LessThanSlashToken, syntaxToken.GetTrailingTrivia(), punctuationSyntax.Text + syntaxToken.Text)) : SyntaxNodeExtensions.AddLeadingSyntax(token, SyntaxList.List(punctuationSyntax, syntaxToken), ERRID.ERR_IllegalXmlWhiteSpace));
					GetNextToken(ScannerState.EndElement);
					GetNextToken(ScannerState.EndElement);
				}
			}
			if (syntaxList.Node != null)
			{
				token = ((!syntaxList.Node!.ContainsDiagnostics) ? SyntaxNodeExtensions.AddLeadingSyntax(token, syntaxList, ERRID.ERR_ExpectedLT) : SyntaxNodeExtensions.AddLeadingSyntax(token, syntaxList));
			}
			if (CurrentToken.Kind == SyntaxKind.XmlNameToken)
			{
				name = (XmlNameSyntax)ParseXmlQualifiedName(requireLeadingWhitespace: false, allowExpr: false, ScannerState.EndElement, ScannerState.EndElement);
			}
			VerifyExpectedToken(SyntaxKind.GreaterThanToken, ref token2, nextState);
			return SyntaxFactory.XmlElementEndTag(token, name, token2);
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> ParseXmlAttributes(bool requireLeadingWhitespace, XmlNodeSyntax xmlElementName)
		{
			SyntaxListBuilder<XmlNodeSyntax> syntaxListBuilder = _pool.Allocate<XmlNodeSyntax>();
			while (true)
			{
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.SingleQuoteToken:
				case SyntaxKind.EqualsToken:
				case SyntaxKind.DoubleQuoteToken:
				case SyntaxKind.LessThanPercentEqualsToken:
				case SyntaxKind.XmlNameToken:
					break;
				default:
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> result = syntaxListBuilder.ToList();
					_pool.Free(syntaxListBuilder);
					return result;
				}
				}
				XmlNodeSyntax xmlNodeSyntax = ParseXmlAttribute(requireLeadingWhitespace, AllowNameAsExpression: true, xmlElementName);
				requireLeadingWhitespace = !xmlNodeSyntax.HasTrailingTrivia;
				syntaxListBuilder.Add(xmlNodeSyntax);
			}
		}

		internal XmlNodeSyntax ParseXmlAttribute(bool requireLeadingWhitespace, bool AllowNameAsExpression, XmlNodeSyntax xmlElementName)
		{
			XmlNodeSyntax crefAttribute = null;
			if (CurrentToken.Kind == SyntaxKind.XmlNameToken || (AllowNameAsExpression && CurrentToken.Kind == SyntaxKind.LessThanPercentEqualsToken) || CurrentToken.Kind == SyntaxKind.EqualsToken || CurrentToken.Kind == SyntaxKind.SingleQuoteToken || CurrentToken.Kind == SyntaxKind.DoubleQuoteToken)
			{
				XmlNodeSyntax xmlNodeSyntax = ParseXmlQualifiedName(requireLeadingWhitespace, allowExpr: true, ScannerState.Element, ScannerState.Element);
				if (CurrentToken.Kind == SyntaxKind.EqualsToken)
				{
					PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
					GetNextToken(ScannerState.Element);
					XmlNodeSyntax xmlNodeSyntax2 = null;
					if (CurrentToken.Kind == SyntaxKind.LessThanPercentEqualsToken)
					{
						xmlNodeSyntax2 = ParseXmlEmbedded(ScannerState.Element);
						crefAttribute = SyntaxFactory.XmlAttribute(xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax2);
					}
					else if (!_scanner.IsScanningXmlDoc || (!TryParseXmlCrefAttributeValue(xmlNodeSyntax, punctuationSyntax, out crefAttribute) && !TryParseXmlNameAttributeValue(xmlNodeSyntax, punctuationSyntax, out crefAttribute, xmlElementName)))
					{
						xmlNodeSyntax2 = ParseXmlString(ScannerState.Element);
						crefAttribute = SyntaxFactory.XmlAttribute(xmlNodeSyntax, punctuationSyntax, xmlNodeSyntax2);
					}
				}
				else if (xmlNodeSyntax.Kind == SyntaxKind.XmlEmbeddedExpression)
				{
					crefAttribute = xmlNodeSyntax;
				}
				else
				{
					XmlNodeSyntax value;
					if (CurrentToken.Kind != SyntaxKind.SingleQuoteToken && CurrentToken.Kind != SyntaxKind.DoubleQuoteToken)
					{
						PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken);
						value = SyntaxFactory.XmlString(punctuationSyntax2, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), punctuationSyntax2);
					}
					else
					{
						value = ParseXmlString(ScannerState.Element);
					}
					crefAttribute = SyntaxFactory.XmlAttribute(xmlNodeSyntax, (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.EqualsToken), value);
				}
			}
			return crefAttribute;
		}

		private bool ElementNameIsOneFromTheList(XmlNodeSyntax xmlElementName, params string[] names)
		{
			if (xmlElementName == null || xmlElementName.Kind != SyntaxKind.XmlName)
			{
				return false;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)xmlElementName;
			if (xmlNameSyntax.Prefix != null)
			{
				return false;
			}
			foreach (string name in names)
			{
				if (DocumentationCommentXmlNames.ElementEquals(xmlNameSyntax.LocalName.Text, name, fromVb: true))
				{
					return true;
				}
			}
			return false;
		}

		private bool TryParseXmlCrefAttributeValue(XmlNodeSyntax name, PunctuationSyntax equals, out XmlNodeSyntax crefAttribute)
		{
			if (name.Kind != SyntaxKind.XmlName)
			{
				return false;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)name;
			if (xmlNameSyntax.Prefix != null || !DocumentationCommentXmlNames.AttributeEquals(xmlNameSyntax.LocalName.Text, "cref"))
			{
				return false;
			}
			ScannerState state;
			PunctuationSyntax punctuationSyntax;
			if (CurrentToken.Kind == SyntaxKind.SingleQuoteToken)
			{
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "'", TextCompare: false) == 0) ? ScannerState.SingleQuotedString : ScannerState.SmartSingleQuotedString);
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
			}
			else
			{
				if (CurrentToken.Kind != SyntaxKind.DoubleQuoteToken)
				{
					return false;
				}
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "\"", TextCompare: false) == 0) ? ScannerState.QuotedString : ScannerState.SmartQuotedString);
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
			}
			Scanner.RestorePoint restorePoint = _scanner.CreateRestorePoint();
			SyntaxToken syntaxToken = PeekNextToken(state);
			if (syntaxToken.Kind == SyntaxKind.XmlTextLiteralToken || syntaxToken.Kind == SyntaxKind.XmlEntityLiteralToken)
			{
				string text = syntaxToken.Text.Trim();
				if (text.Length < 2 || EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(text[0]), ":", TextCompare: false) == 0 || EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(text[1]), ":", TextCompare: false) != 0)
				{
					GetNextToken();
					CrefReferenceSyntax crefReferenceSyntax;
					if (SyntaxFacts.IsPredefinedTypeKeyword(CurrentToken.Kind))
					{
						PredefinedTypeSyntax name2 = SyntaxFactory.PredefinedType((KeywordSyntax)CurrentToken);
						GetNextToken();
						crefReferenceSyntax = SyntaxFactory.CrefReference(name2, null, null);
					}
					else
					{
						crefReferenceSyntax = TryParseCrefReference();
					}
					if (crefReferenceSyntax != null)
					{
						ResetCurrentToken(state);
						while (true)
						{
							SyntaxToken currentToken;
							switch (CurrentToken.Kind)
							{
							case SyntaxKind.SingleQuoteToken:
							case SyntaxKind.DoubleQuoteToken:
							{
								PunctuationSyntax endQuoteToken2 = (PunctuationSyntax)CurrentToken;
								GetNextToken(ScannerState.Element);
								crefAttribute = SyntaxFactory.XmlCrefAttribute(xmlNameSyntax, equals, punctuationSyntax, crefReferenceSyntax, endQuoteToken2);
								return true;
							}
							case SyntaxKind.XmlTextLiteralToken:
							case SyntaxKind.XmlEntityLiteralToken:
								currentToken = CurrentToken;
								if (!TriviaChecker.HasInvalidTrivia(currentToken))
								{
									goto IL_0234;
								}
								break;
							case SyntaxKind.EndOfFileToken:
							case SyntaxKind.EndOfXmlToken:
							{
								PunctuationSyntax endQuoteToken = (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(punctuationSyntax.Kind);
								crefAttribute = SyntaxFactory.XmlCrefAttribute(xmlNameSyntax, equals, punctuationSyntax, crefReferenceSyntax, endQuoteToken);
								return true;
							}
							}
							break;
							IL_0234:
							crefReferenceSyntax = SyntaxNodeExtensions.AddTrailingSyntax(crefReferenceSyntax, currentToken);
							crefReferenceSyntax.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics);
							GetNextToken(state);
						}
					}
				}
			}
			restorePoint.Restore();
			ResetCurrentToken(ScannerState.Element);
			return false;
		}

		internal CrefReferenceSyntax TryParseCrefReference()
		{
			TypeSyntax typeSyntax = TryParseCrefOptionallyQualifiedName();
			CrefSignatureSyntax signature = null;
			SimpleAsClauseSyntax asClause = null;
			if (CurrentToken.Kind == SyntaxKind.OpenParenToken && typeSyntax.Kind != SyntaxKind.PredefinedType)
			{
				signature = TryParseCrefReferenceSignature();
				if (CurrentToken.Kind == SyntaxKind.AsKeyword)
				{
					KeywordSyntax asKeyword = (KeywordSyntax)CurrentToken;
					GetNextToken();
					TypeSyntax type = ParseGeneralType();
					asClause = SyntaxFactory.SimpleAsClause(asKeyword, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>), type);
				}
			}
			CrefReferenceSyntax crefReferenceSyntax = SyntaxFactory.CrefReference(typeSyntax, signature, asClause);
			if (crefReferenceSyntax.ContainsDiagnostics)
			{
				crefReferenceSyntax.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics);
			}
			return crefReferenceSyntax;
		}

		internal CrefSignatureSyntax TryParseCrefReferenceSignature()
		{
			PunctuationSyntax openParenToken = (PunctuationSyntax)CurrentToken;
			GetNextToken();
			SeparatedSyntaxListBuilder<CrefSignaturePartSyntax> item = _pool.AllocateSeparated<CrefSignaturePartSyntax>();
			bool flag = true;
			SyntaxToken syntaxToken;
			while (true)
			{
				syntaxToken = CurrentToken;
				if (syntaxToken.Kind != SyntaxKind.CloseParenToken && syntaxToken.Kind != SyntaxKind.CommaToken && !flag)
				{
					syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken);
				}
				if (syntaxToken.Kind == SyntaxKind.CloseParenToken)
				{
					break;
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					item.AddSeparator(CurrentToken);
					GetNextToken();
				}
				KeywordSyntax keywordSyntax = null;
				while (CurrentToken.Kind == SyntaxKind.ByValKeyword || CurrentToken.Kind == SyntaxKind.ByRefKeyword)
				{
					keywordSyntax = ((keywordSyntax != null) ? SyntaxNodeExtensions.AddTrailingSyntax(keywordSyntax, CurrentToken, ERRID.ERR_InvalidParameterSyntax) : ((KeywordSyntax)CurrentToken));
					GetNextToken();
				}
				TypeSyntax type = ParseGeneralType();
				item.Add(SyntaxFactory.CrefSignaturePart(keywordSyntax, type));
			}
			PunctuationSyntax closeParenToken = (PunctuationSyntax)syntaxToken;
			if (!syntaxToken.IsMissing)
			{
				GetNextToken();
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefSignaturePartSyntax> separatedSyntaxList = item.ToList();
			_pool.Free(in item);
			return SyntaxFactory.CrefSignature(openParenToken, separatedSyntaxList, closeParenToken);
		}

		internal CrefOperatorReferenceSyntax TryParseCrefOperatorName()
		{
			KeywordSyntax operatorKeyword = (KeywordSyntax)CurrentToken;
			GetNextToken();
			KeywordSyntax k = null;
			SyntaxToken syntaxToken = ((!TryTokenAsContextualKeyword(CurrentToken, ref k)) ? CurrentToken : k);
			if (SyntaxFacts.IsOperatorStatementOperatorToken(syntaxToken.Kind))
			{
				GetNextToken();
			}
			else
			{
				syntaxToken = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.PlusToken), ERRID.ERR_UnknownOperator);
			}
			return SyntaxFactory.CrefOperatorReference(operatorKeyword, syntaxToken);
		}

		internal TypeSyntax TryParseCrefOptionallyQualifiedName()
		{
			NameSyntax nameSyntax = null;
			if (CurrentToken.Kind == SyntaxKind.GlobalKeyword)
			{
				nameSyntax = SyntaxFactory.GlobalName((KeywordSyntax)CurrentToken);
				GetNextToken();
			}
			else if (CurrentToken.Kind == SyntaxKind.ObjectKeyword)
			{
				nameSyntax = SyntaxFactory.IdentifierName(_scanner.MakeIdentifier((KeywordSyntax)CurrentToken));
				GetNextToken();
			}
			else
			{
				if (CurrentToken.Kind == SyntaxKind.OperatorKeyword)
				{
					return TryParseCrefOperatorName();
				}
				bool allowNonEmptyGenericArguments;
				bool allowEmptyGenericArguments;
				if (CurrentToken.Kind == SyntaxKind.NewKeyword)
				{
					allowEmptyGenericArguments = false;
					allowNonEmptyGenericArguments = true;
					return ParseSimpleName(allowGenericArguments: false, allowGenericsWithoutOf: false, disallowGenericArgumentsOnLastQualifiedName: true, nonArrayName: false, allowKeyword: true, ref allowEmptyGenericArguments, ref allowNonEmptyGenericArguments);
				}
				allowNonEmptyGenericArguments = false;
				allowEmptyGenericArguments = true;
				nameSyntax = ParseSimpleName(allowGenericArguments: true, allowGenericsWithoutOf: false, disallowGenericArgumentsOnLastQualifiedName: false, nonArrayName: false, allowKeyword: false, ref allowNonEmptyGenericArguments, ref allowEmptyGenericArguments);
			}
			while (CurrentToken.Kind == SyntaxKind.DotToken)
			{
				PunctuationSyntax dotToken = (PunctuationSyntax)CurrentToken;
				GetNextToken();
				if (CurrentToken.Kind == SyntaxKind.OperatorKeyword)
				{
					CrefOperatorReferenceSyntax right = TryParseCrefOperatorName();
					return SyntaxFactory.QualifiedCrefOperatorReference(nameSyntax, dotToken, right);
				}
				bool allowEmptyGenericArguments = false;
				bool allowNonEmptyGenericArguments = true;
				SimpleNameSyntax right2 = ParseSimpleName(allowGenericArguments: true, allowGenericsWithoutOf: false, disallowGenericArgumentsOnLastQualifiedName: false, nonArrayName: false, allowKeyword: true, ref allowEmptyGenericArguments, ref allowNonEmptyGenericArguments);
				nameSyntax = SyntaxFactory.QualifiedName(nameSyntax, dotToken, right2);
			}
			return nameSyntax;
		}

		private bool TryParseXmlNameAttributeValue(XmlNodeSyntax name, PunctuationSyntax equals, out XmlNodeSyntax nameAttribute, XmlNodeSyntax xmlElementName)
		{
			if (name.Kind != SyntaxKind.XmlName)
			{
				return false;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)name;
			if (xmlNameSyntax.Prefix != null || !DocumentationCommentXmlNames.AttributeEquals(xmlNameSyntax.LocalName.Text, "name"))
			{
				return false;
			}
			if (!ElementNameIsOneFromTheList(xmlElementName, "param", "paramref", "typeparam", "typeparamref"))
			{
				return false;
			}
			ScannerState state;
			PunctuationSyntax startQuoteToken;
			if (CurrentToken.Kind == SyntaxKind.SingleQuoteToken)
			{
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "'", TextCompare: false) == 0) ? ScannerState.SingleQuotedString : ScannerState.SmartSingleQuotedString);
				startQuoteToken = (PunctuationSyntax)CurrentToken;
			}
			else
			{
				if (CurrentToken.Kind != SyntaxKind.DoubleQuoteToken)
				{
					return false;
				}
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "\"", TextCompare: false) == 0) ? ScannerState.QuotedString : ScannerState.SmartQuotedString);
				startQuoteToken = (PunctuationSyntax)CurrentToken;
			}
			Scanner.RestorePoint restorePoint = _scanner.CreateRestorePoint();
			GetNextToken();
			SyntaxToken syntaxToken = CurrentToken;
			if (syntaxToken.Kind != SyntaxKind.IdentifierToken)
			{
				if (!syntaxToken.IsKeyword)
				{
					goto IL_01c1;
				}
				syntaxToken = _scanner.MakeIdentifier((KeywordSyntax)CurrentToken);
			}
			if (!syntaxToken.ContainsDiagnostics && !TriviaChecker.HasInvalidTrivia(syntaxToken))
			{
				GetNextToken(state);
				SyntaxToken currentToken = CurrentToken;
				if (currentToken.Kind == SyntaxKind.SingleQuoteToken || currentToken.Kind == SyntaxKind.DoubleQuoteToken)
				{
					PunctuationSyntax endQuoteToken = (PunctuationSyntax)CurrentToken;
					GetNextToken(ScannerState.Element);
					nameAttribute = SyntaxFactory.XmlNameAttribute(xmlNameSyntax, equals, startQuoteToken, SyntaxFactory.IdentifierName((IdentifierTokenSyntax)syntaxToken), endQuoteToken);
					return true;
				}
			}
			goto IL_01c1;
			IL_01c1:
			restorePoint.Restore();
			ResetCurrentToken(ScannerState.Element);
			return false;
		}

		private XmlNodeSyntax ParseXmlQualifiedName(bool requireLeadingWhitespace, bool allowExpr, ScannerState stateForName, ScannerState nextState)
		{
			switch (CurrentToken.Kind)
			{
			case SyntaxKind.XmlNameToken:
				return ParseXmlQualifiedName(requireLeadingWhitespace, stateForName, nextState);
			case SyntaxKind.LessThanPercentEqualsToken:
				if (allowExpr)
				{
					return ParseXmlEmbedded(nextState);
				}
				break;
			}
			ResetCurrentToken(nextState);
			return ReportExpectedXmlName();
		}

		private XmlNodeSyntax ParseXmlQualifiedName(bool requireLeadingWhitespace, ScannerState stateForName, ScannerState nextState)
		{
			bool flag = requireLeadingWhitespace && (SyntaxNodeExtensions.ContainsWhitespaceTrivia(PrevToken.GetTrailingTrivia()) || SyntaxNodeExtensions.ContainsWhitespaceTrivia(CurrentToken.GetLeadingTrivia()));
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)CurrentToken;
			GetNextToken(stateForName);
			if (requireLeadingWhitespace && !flag)
			{
				xmlNameTokenSyntax = ReportSyntaxError(xmlNameTokenSyntax, ERRID.ERR_ExpectedXmlWhiteSpace);
			}
			XmlPrefixSyntax prefix = null;
			if (CurrentToken.Kind == SyntaxKind.ColonToken)
			{
				PunctuationSyntax punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken(stateForName);
				prefix = SyntaxFactory.XmlPrefix(xmlNameTokenSyntax, punctuationSyntax);
				if (CurrentToken.Kind == SyntaxKind.XmlNameToken)
				{
					xmlNameTokenSyntax = (XmlNameTokenSyntax)CurrentToken;
					GetNextToken(stateForName);
					if (punctuationSyntax.HasTrailingTrivia || xmlNameTokenSyntax.HasLeadingTrivia)
					{
						xmlNameTokenSyntax = ReportSyntaxError(xmlNameTokenSyntax, ERRID.ERR_ExpectedXmlName);
					}
				}
				else
				{
					xmlNameTokenSyntax = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, null, null), ERRID.ERR_ExpectedXmlName);
				}
			}
			XmlNameSyntax result = SyntaxFactory.XmlName(prefix, xmlNameTokenSyntax);
			ResetCurrentToken(nextState);
			return result;
		}

		private static bool IsAsciiColonTrivia(VisualBasicSyntaxNode node)
		{
			if (node.Kind == SyntaxKind.ColonTrivia)
			{
				return EmbeddedOperators.CompareString(node.ToString(), ":", TextCompare: false) == 0;
			}
			return false;
		}

		private XmlNameSyntax ParseXmlQualifiedNameVB()
		{
			if (!IsValidXmlQualifiedNameToken(CurrentToken))
			{
				return ReportExpectedXmlName();
			}
			XmlNameTokenSyntax xmlNameTokenSyntax = ToXmlNameToken(CurrentToken);
			GetNextToken();
			XmlPrefixSyntax prefix = null;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(xmlNameTokenSyntax.GetTrailingTrivia());
			if (trivia.Count > 0 && IsAsciiColonTrivia(trivia[0]))
			{
				trivia = SyntaxNodeExtensions.GetStartOfTrivia(trivia, trivia.Count - 1);
				xmlNameTokenSyntax = SyntaxExtensions.WithDiagnostics(SyntaxFactory.XmlNameToken(xmlNameTokenSyntax.Text, xmlNameTokenSyntax.PossibleKeywordKind, xmlNameTokenSyntax.GetLeadingTrivia(), trivia.Node), xmlNameTokenSyntax.GetDiagnostics());
				ResetCurrentToken(ScannerState.Element);
				PunctuationSyntax node = (PunctuationSyntax)CurrentToken;
				GetNextToken(ScannerState.Element);
				node = TransitionFromXmlToVB(node);
				prefix = SyntaxFactory.XmlPrefix(xmlNameTokenSyntax, node);
				xmlNameTokenSyntax = null;
				if (trivia.Count == 0 && !node.HasTrailingTrivia && IsValidXmlQualifiedNameToken(CurrentToken))
				{
					xmlNameTokenSyntax = ToXmlNameToken(CurrentToken);
					GetNextToken();
				}
				if (xmlNameTokenSyntax == null)
				{
					xmlNameTokenSyntax = ReportSyntaxError(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, null, null), ERRID.ERR_ExpectedXmlName);
				}
			}
			return SyntaxFactory.XmlName(prefix, xmlNameTokenSyntax);
		}

		private static bool IsValidXmlQualifiedNameToken(SyntaxToken token)
		{
			if (token.Kind != SyntaxKind.IdentifierToken)
			{
				return token is KeywordSyntax;
			}
			return true;
		}

		private XmlNameTokenSyntax ToXmlNameToken(SyntaxToken token)
		{
			if (token.Kind == SyntaxKind.IdentifierToken)
			{
				IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)token;
				XmlNameTokenSyntax xmlNameTokenSyntax = SyntaxFactory.XmlNameToken(identifierTokenSyntax.Text, identifierTokenSyntax.PossibleKeywordKind, token.GetLeadingTrivia(), token.GetTrailingTrivia());
				return (!identifierTokenSyntax.IsBracketed) ? VerifyXmlNameToken(xmlNameTokenSyntax) : ReportSyntaxError(xmlNameTokenSyntax, ERRID.ERR_ExpectedXmlName);
			}
			return SyntaxFactory.XmlNameToken(token.Text, token.Kind, token.GetLeadingTrivia(), token.GetTrailingTrivia());
		}

		private static XmlNameTokenSyntax VerifyXmlNameToken(XmlNameTokenSyntax tk)
		{
			string valueText = tk.ValueText;
			if (!string.IsNullOrEmpty(valueText))
			{
				if (!XmlCharacterGlobalHelpers.isStartNameChar(valueText[0]))
				{
					char c = valueText[0];
					return ReportSyntaxError(tk, ERRID.ERR_IllegalXmlStartNameChar, c, "0x" + Convert.ToInt32(c).ToString("X4"));
				}
				string text = valueText;
				foreach (char c2 in text)
				{
					if (!XmlCharacterGlobalHelpers.isNameChar(c2))
					{
						return ReportSyntaxError(tk, ERRID.ERR_IllegalXmlNameChar, c2, "0x" + Convert.ToInt32(c2).ToString("X4"));
					}
				}
			}
			return tk;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> ParseRestOfDocCommentContent(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> nodesSoFar)
		{
			SyntaxListBuilder<XmlNodeSyntax> syntaxListBuilder = _pool.Allocate<XmlNodeSyntax>();
			GreenNode[] nodes = nodesSoFar.Nodes;
			foreach (GreenNode greenNode in nodes)
			{
				syntaxListBuilder.Add((XmlNodeSyntax)greenNode);
			}
			if (CurrentToken.Kind == SyntaxKind.EndOfXmlToken)
			{
				GetNextToken(ScannerState.Content);
				if (CurrentToken.Kind == SyntaxKind.DocumentationCommentLineBreakToken)
				{
					XmlNodeSyntax[] nodes2 = ParseXmlContent(ScannerState.Content).Nodes;
					foreach (XmlNodeSyntax node in nodes2)
					{
						syntaxListBuilder.Add(node);
					}
				}
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> ParseXmlContent(ScannerState state)
		{
			SyntaxListBuilder<XmlNodeSyntax> syntaxListBuilder = _pool.Allocate<XmlNodeSyntax>();
			XmlWhitespaceChecker whitespaceChecker = new XmlWhitespaceChecker();
			while (true)
			{
				XmlNodeSyntax node;
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.LessThanToken:
					node = ParseXmlElement(ScannerState.Content);
					goto IL_01b9;
				case SyntaxKind.LessThanSlashToken:
					node = ReportSyntaxError(ParseXmlElementEndTag(ScannerState.Content), ERRID.ERR_XmlEndElementNoMatchingStart);
					goto IL_01b9;
				case SyntaxKind.LessThanExclamationMinusMinusToken:
					node = ParseXmlComment(ScannerState.Content);
					goto IL_01b9;
				case SyntaxKind.LessThanQuestionToken:
					node = ParseXmlProcessingInstruction(ScannerState.Content, whitespaceChecker);
					goto IL_01b9;
				case SyntaxKind.BeginCDataToken:
					node = ParseXmlCData(ScannerState.Content);
					goto IL_01b9;
				case SyntaxKind.LessThanPercentEqualsToken:
					node = ParseXmlEmbedded(ScannerState.Content);
					goto IL_01b9;
				case SyntaxKind.XmlTextLiteralToken:
				case SyntaxKind.XmlEntityLiteralToken:
				case SyntaxKind.DocumentationCommentLineBreakToken:
				{
					SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder2 = _pool.Allocate<XmlTextTokenSyntax>();
					SyntaxKind kind;
					do
					{
						syntaxListBuilder2.Add((XmlTextTokenSyntax)CurrentToken);
						GetNextToken(ScannerState.Content);
						kind = CurrentToken.Kind;
					}
					while (kind == SyntaxKind.XmlTextLiteralToken || kind == SyntaxKind.XmlEntityLiteralToken || kind == SyntaxKind.DocumentationCommentLineBreakToken);
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = syntaxListBuilder2.ToList();
					_pool.Free(syntaxListBuilder2);
					node = SyntaxFactory.XmlText(syntaxList);
					goto IL_01b9;
				}
				case SyntaxKind.BadToken:
					if (((BadTokenSyntax)CurrentToken).SubKind == SyntaxSubKind.BeginDocTypeToken)
					{
						GreenNode unexpected = ParseXmlDocType(ScannerState.Element);
						node = SyntaxFactory.XmlText(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlTextLiteralToken));
						node = SyntaxNodeExtensions.AddLeadingSyntax(node, unexpected, ERRID.ERR_DTDNotSupported);
						goto IL_01b9;
					}
					goto default;
				default:
					if (state == ScannerState.Content)
					{
						node = ResyncXmlContent();
						goto IL_01b9;
					}
					break;
				case SyntaxKind.EndOfFileToken:
				case SyntaxKind.EndOfXmlToken:
					break;
				}
				break;
				IL_01b9:
				syntaxListBuilder.Add(node);
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> result = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return result;
		}

		private XmlProcessingInstructionSyntax ParseXmlProcessingInstruction(ScannerState nextState, XmlWhitespaceChecker whitespaceChecker)
		{
			PunctuationSyntax lessThanQuestionToken = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.Element);
			XmlNameTokenSyntax token = null;
			if (!VerifyExpectedToken(SyntaxKind.XmlNameToken, ref token, ScannerState.StartProcessingInstruction))
			{
				ResetCurrentToken(ScannerState.StartProcessingInstruction);
			}
			if (token.Text.Length == 3 && string.Equals(token.Text, "xml", StringComparison.OrdinalIgnoreCase))
			{
				token = ReportSyntaxError(token, ERRID.ERR_IllegalProcessingInstructionName, token.Text);
			}
			XmlTextTokenSyntax xmlTextTokenSyntax = null;
			SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder = _pool.Allocate<XmlTextTokenSyntax>();
			if (CurrentToken.Kind == SyntaxKind.XmlTextLiteralToken || CurrentToken.Kind == SyntaxKind.DocumentationCommentLineBreakToken)
			{
				xmlTextTokenSyntax = (XmlTextTokenSyntax)CurrentToken;
				if (!token.IsMissing && !SyntaxNodeExtensions.ContainsWhitespaceTrivia(token.GetTrailingTrivia()) && !SyntaxNodeExtensions.ContainsWhitespaceTrivia(xmlTextTokenSyntax.GetLeadingTrivia()))
				{
					xmlTextTokenSyntax = ReportSyntaxError(xmlTextTokenSyntax, ERRID.ERR_ExpectedXmlWhiteSpace);
				}
				while (true)
				{
					syntaxListBuilder.Add(xmlTextTokenSyntax);
					GetNextToken(ScannerState.ProcessingInstruction);
					if (CurrentToken.Kind != SyntaxKind.XmlTextLiteralToken && CurrentToken.Kind != SyntaxKind.DocumentationCommentLineBreakToken)
					{
						break;
					}
					xmlTextTokenSyntax = (XmlTextTokenSyntax)CurrentToken;
				}
			}
			PunctuationSyntax token2 = null;
			VerifyExpectedToken(SyntaxKind.QuestionGreaterThanToken, ref token2, nextState);
			XmlProcessingInstructionSyntax node = SyntaxFactory.XmlProcessingInstruction(lessThanQuestionToken, token, syntaxListBuilder.ToList(), token2);
			node = (XmlProcessingInstructionSyntax)whitespaceChecker.Visit(node);
			_pool.Free(syntaxListBuilder);
			return node;
		}

		private XmlCDataSectionSyntax ParseXmlCData(ScannerState nextState)
		{
			PunctuationSyntax beginCDataToken = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.CData);
			SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder = _pool.Allocate<XmlTextTokenSyntax>();
			while (CurrentToken.Kind == SyntaxKind.XmlTextLiteralToken || CurrentToken.Kind == SyntaxKind.DocumentationCommentLineBreakToken)
			{
				syntaxListBuilder.Add((XmlTextTokenSyntax)CurrentToken);
				GetNextToken(ScannerState.CData);
			}
			PunctuationSyntax token = null;
			VerifyExpectedToken(SyntaxKind.EndCDataToken, ref token, nextState);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return SyntaxFactory.XmlCDataSection(beginCDataToken, syntaxList, token);
		}

		private XmlNodeSyntax ParseXmlComment(ScannerState nextState)
		{
			PunctuationSyntax lessThanExclamationMinusMinusToken = (PunctuationSyntax)CurrentToken;
			GetNextToken(ScannerState.Comment);
			SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder = _pool.Allocate<XmlTextTokenSyntax>();
			while (CurrentToken.Kind == SyntaxKind.XmlTextLiteralToken || CurrentToken.Kind == SyntaxKind.DocumentationCommentLineBreakToken)
			{
				XmlTextTokenSyntax xmlTextTokenSyntax = (XmlTextTokenSyntax)CurrentToken;
				if (xmlTextTokenSyntax.Text.Length == 2 && EmbeddedOperators.CompareString(xmlTextTokenSyntax.Text, "--", TextCompare: false) == 0)
				{
					xmlTextTokenSyntax = ReportSyntaxError(xmlTextTokenSyntax, ERRID.ERR_IllegalXmlCommentChar);
				}
				syntaxListBuilder.Add(xmlTextTokenSyntax);
				GetNextToken(ScannerState.Comment);
			}
			PunctuationSyntax token = null;
			VerifyExpectedToken(SyntaxKind.MinusMinusGreaterThanToken, ref token, nextState);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlTextTokenSyntax> syntaxList = syntaxListBuilder.ToList();
			_pool.Free(syntaxListBuilder);
			return SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, syntaxList, token);
		}

		internal XmlStringSyntax ParseXmlString(ScannerState nextState)
		{
			ScannerState state;
			PunctuationSyntax punctuationSyntax;
			if (CurrentToken.Kind == SyntaxKind.SingleQuoteToken)
			{
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "'", TextCompare: false) == 0) ? ScannerState.SingleQuotedString : ScannerState.SmartSingleQuotedString);
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken(state);
			}
			else if (CurrentToken.Kind == SyntaxKind.DoubleQuoteToken)
			{
				state = ((EmbeddedOperators.CompareString(CurrentToken.Text, "\"", TextCompare: false) == 0) ? ScannerState.QuotedString : ScannerState.SmartQuotedString);
				punctuationSyntax = (PunctuationSyntax)CurrentToken;
				GetNextToken(state);
			}
			else
			{
				state = ScannerState.UnQuotedString;
				punctuationSyntax = (PunctuationSyntax)Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken);
				punctuationSyntax = ReportSyntaxError(punctuationSyntax, ERRID.ERR_StartAttributeValue);
				ResetCurrentToken(state);
			}
			SyntaxListBuilder<XmlTextTokenSyntax> syntaxListBuilder = _pool.Allocate<XmlTextTokenSyntax>();
			while (true)
			{
				switch (CurrentToken.Kind)
				{
				case SyntaxKind.SingleQuoteToken:
				case SyntaxKind.DoubleQuoteToken:
				{
					PunctuationSyntax endQuoteToken = (PunctuationSyntax)CurrentToken;
					GetNextToken(nextState);
					XmlStringSyntax result2 = SyntaxFactory.XmlString(punctuationSyntax, syntaxListBuilder.ToList(), endQuoteToken);
					_pool.Free(syntaxListBuilder);
					return result2;
				}
				case SyntaxKind.XmlTextLiteralToken:
				case SyntaxKind.XmlEntityLiteralToken:
				case SyntaxKind.DocumentationCommentLineBreakToken:
					break;
				default:
				{
					SyntaxToken syntaxToken = HandleUnexpectedToken(punctuationSyntax.Kind);
					XmlStringSyntax result = SyntaxFactory.XmlString(punctuationSyntax, syntaxListBuilder.ToList(), (PunctuationSyntax)syntaxToken);
					_pool.Free(syntaxListBuilder);
					return result;
				}
				}
				syntaxListBuilder.Add((XmlTextTokenSyntax)CurrentToken);
				GetNextToken(state);
			}
		}

		private XmlEmbeddedExpressionSyntax ParseXmlEmbedded(ScannerState enclosingState)
		{
			PunctuationSyntax node = (PunctuationSyntax)CurrentToken;
			GetNextToken(enclosingState);
			node = TransitionFromXmlToVB(node);
			TryEatNewLine();
			ExpressionSyntax expression = ParseExpressionCore();
			PunctuationSyntax token = null;
			if (!TryEatNewLineAndGetToken(SyntaxKind.PercentGreaterThanToken, ref token, createIfMissing: false, enclosingState))
			{
				SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
				ResyncAt(syntaxListBuilder, ScannerState.VB, new SyntaxKind[8]
				{
					SyntaxKind.PercentGreaterThanToken,
					SyntaxKind.GreaterThanToken,
					SyntaxKind.LessThanToken,
					SyntaxKind.LessThanPercentEqualsToken,
					SyntaxKind.LessThanQuestionToken,
					SyntaxKind.BeginCDataToken,
					SyntaxKind.LessThanExclamationMinusMinusToken,
					SyntaxKind.LessThanSlashToken
				});
				if (CurrentToken.Kind == SyntaxKind.PercentGreaterThanToken)
				{
					token = (PunctuationSyntax)CurrentToken;
					GetNextToken(enclosingState);
				}
				else
				{
					token = (PunctuationSyntax)HandleUnexpectedToken(SyntaxKind.PercentGreaterThanToken);
				}
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = syntaxListBuilder.ToList();
				_pool.Free(syntaxListBuilder);
				if (syntaxList.Node != null)
				{
					token = SyntaxNodeExtensions.AddLeadingSyntax(token, syntaxList, ERRID.ERR_Syntax);
				}
			}
			XmlEmbeddedExpressionSyntax node2 = SyntaxFactory.XmlEmbeddedExpression(node, expression, token);
			node2 = AdjustTriviaForMissingTokens(node2);
			return TransitionFromVBToXml(enclosingState, node2);
		}
	}
}
