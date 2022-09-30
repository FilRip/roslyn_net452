using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class XmlWhitespaceChecker : VisualBasicSyntaxRewriter
	{
		[Flags]
		internal enum TriviaCheck
		{
			None = 0,
			ProhibitLeadingTrivia = 1,
			ProhibitTrailingTrivia = 2
		}

		internal struct WhiteSpaceOptions
		{
			internal SyntaxKind _parentKind;

			internal TriviaCheck _triviaCheck;
		}

		private WhiteSpaceOptions _options;

		public override VisualBasicSyntaxNode VisitXmlDeclaration(XmlDeclarationSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)Visit(node.LessThanQuestionToken);
			if (node._lessThanQuestionToken != punctuationSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
			KeywordSyntax keywordSyntax = (KeywordSyntax)Visit(node.XmlKeyword);
			if (node._xmlKeyword != keywordSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, node.Version, node.Encoding, node.Standalone, node.QuestionGreaterThanToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlElementStartTag(XmlElementStartTagSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlElementStartTag;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.LessThanToken);
			if (node.LessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.None;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node.Attributes.Node != syntaxList.Node)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return SyntaxFactory.XmlElementStartTag(punctuationSyntax, xmlNodeSyntax, syntaxList, node.GreaterThanToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlEmptyElement(XmlEmptyElementSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlElementStartTag;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.LessThanToken);
			if (node.LessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.None;
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = VisitList(node.Attributes);
			if (node.Attributes.Node != syntaxList.Node)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return SyntaxFactory.XmlEmptyElement(punctuationSyntax, xmlNodeSyntax, syntaxList, node.SlashGreaterThanToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlElementEndTag(XmlElementEndTagSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlElementStartTag;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.LessThanSlashToken);
			if (node.LessThanSlashToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return SyntaxFactory.XmlElementEndTag(punctuationSyntax, xmlNameSyntax, node.GreaterThanToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.LessThanQuestionToken);
			if (node.LessThanQuestionToken != punctuationSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitSyntaxToken(node.Name);
			if (node.Name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlProcessingInstructionSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, xmlNameTokenSyntax, node.TextTokens.Node, node.QuestionGreaterThanToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlNameAttribute(XmlNameAttributeSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlNameAttribute;
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlNameAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, node.EqualsToken, node.StartQuoteToken, node.Reference, node.EndQuoteToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlCrefAttribute;
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)Visit(node.Name);
			if (node.Name != xmlNameSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlCrefAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, node.EqualsToken, node.StartQuoteToken, node.Reference, node.EndQuoteToken);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlAttribute(XmlAttributeSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlAttribute;
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNodeSyntax, node.EqualsToken, node.Value);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlBracketedName(XmlBracketedNameSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._parentKind = SyntaxKind.XmlBracketedName;
			_options._triviaCheck = TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.LessThanToken);
			if (node.LessThanToken != punctuationSyntax)
			{
				flag = true;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)Visit(node.Name);
			if (node.Name != xmlNodeSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)VisitSyntaxToken(node.GreaterThanToken);
			if (node.GreaterThanToken != punctuationSyntax2)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return SyntaxFactory.XmlBracketedName(punctuationSyntax, (XmlNameSyntax)xmlNodeSyntax, punctuationSyntax2);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlName(XmlNameSyntax node)
		{
			bool flag = false;
			XmlPrefixSyntax xmlPrefixSyntax = (XmlPrefixSyntax)Visit(node.Prefix);
			if (node.Prefix != xmlPrefixSyntax)
			{
				flag = true;
			}
			WhiteSpaceOptions options = _options;
			switch (_options._parentKind)
			{
			case SyntaxKind.XmlAttribute:
			case SyntaxKind.XmlCrefAttribute:
			case SyntaxKind.XmlNameAttribute:
				if (node.Prefix != null)
				{
					_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
				}
				else
				{
					_options._triviaCheck = TriviaCheck.None;
				}
				break;
			case SyntaxKind.XmlBracketedName:
				_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia | TriviaCheck.ProhibitTrailingTrivia;
				break;
			default:
				_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia;
				break;
			}
			XmlNameTokenSyntax xmlNameTokenSyntax;
			if (_options._triviaCheck != 0)
			{
				xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitSyntaxToken(node.LocalName);
				if (node.LocalName != xmlNameTokenSyntax)
				{
					flag = true;
				}
			}
			else
			{
				xmlNameTokenSyntax = node.LocalName;
			}
			_options = options;
			if (flag)
			{
				return new XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, xmlNameTokenSyntax);
			}
			return node;
		}

		public override VisualBasicSyntaxNode VisitXmlPrefix(XmlPrefixSyntax node)
		{
			bool flag = false;
			WhiteSpaceOptions options = _options;
			_options._triviaCheck = ((_options._parentKind == SyntaxKind.XmlAttribute) ? TriviaCheck.ProhibitTrailingTrivia : (TriviaCheck.ProhibitLeadingTrivia | TriviaCheck.ProhibitTrailingTrivia));
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)VisitSyntaxToken(node.Name);
			if (node.Name != xmlNameTokenSyntax)
			{
				flag = true;
			}
			_options._triviaCheck = TriviaCheck.ProhibitLeadingTrivia | TriviaCheck.ProhibitTrailingTrivia;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)VisitSyntaxToken(node.ColonToken);
			if (node.ColonToken != punctuationSyntax)
			{
				flag = true;
			}
			_options = options;
			if (flag)
			{
				return new XmlPrefixSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax);
			}
			return node;
		}

		public override SyntaxToken VisitSyntaxToken(SyntaxToken token)
		{
			if (token == null)
			{
				return null;
			}
			bool flag = false;
			GreenNode greenNode = null;
			GreenNode greenNode2 = null;
			switch (token.Kind)
			{
			case SyntaxKind.XmlKeyword:
			case SyntaxKind.ColonToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanSlashToken:
			case SyntaxKind.LessThanQuestionToken:
			case SyntaxKind.LessThanPercentEqualsToken:
			case SyntaxKind.XmlNameToken:
				greenNode = token.GetLeadingTrivia();
				if ((_options._triviaCheck & TriviaCheck.ProhibitLeadingTrivia) == TriviaCheck.ProhibitLeadingTrivia)
				{
					GreenNode node = VisitList(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(greenNode)).Node;
					if (node != greenNode)
					{
						flag = true;
						greenNode = node;
					}
				}
				greenNode2 = token.GetTrailingTrivia();
				if ((_options._triviaCheck & TriviaCheck.ProhibitTrailingTrivia) == TriviaCheck.ProhibitTrailingTrivia)
				{
					GreenNode node2 = VisitList(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(greenNode2)).Node;
					if (node2 != greenNode2)
					{
						flag = true;
						greenNode2 = node2;
					}
				}
				break;
			}
			if (flag)
			{
				switch (token.Kind)
				{
				case SyntaxKind.XmlNameToken:
				{
					XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)token;
					return new XmlNameTokenSyntax(xmlNameTokenSyntax.Kind, xmlNameTokenSyntax.GetDiagnostics(), xmlNameTokenSyntax.GetAnnotations(), xmlNameTokenSyntax.Text, greenNode, greenNode2, xmlNameTokenSyntax.PossibleKeywordKind);
				}
				case SyntaxKind.XmlKeyword:
				{
					KeywordSyntax keywordSyntax = (KeywordSyntax)token;
					return new KeywordSyntax(keywordSyntax.Kind, keywordSyntax.GetDiagnostics(), keywordSyntax.GetAnnotations(), keywordSyntax.Text, greenNode, greenNode2);
				}
				case SyntaxKind.ColonToken:
				case SyntaxKind.LessThanToken:
				case SyntaxKind.LessThanSlashToken:
				case SyntaxKind.LessThanQuestionToken:
				case SyntaxKind.LessThanPercentEqualsToken:
				{
					PunctuationSyntax punctuationSyntax = (PunctuationSyntax)token;
					return new PunctuationSyntax(punctuationSyntax.Kind, punctuationSyntax.GetDiagnostics(), punctuationSyntax.GetAnnotations(), punctuationSyntax.Text, greenNode, greenNode2);
				}
				}
			}
			return token;
		}

		public override SyntaxTrivia VisitSyntaxTrivia(SyntaxTrivia trivia)
		{
			if (trivia.Kind == SyntaxKind.WhitespaceTrivia || trivia.Kind == SyntaxKind.EndOfLineTrivia)
			{
				return (SyntaxTrivia)trivia.AddError(ErrorFactory.ErrorInfo(ERRID.ERR_IllegalXmlWhiteSpace));
			}
			return trivia;
		}
	}
}
