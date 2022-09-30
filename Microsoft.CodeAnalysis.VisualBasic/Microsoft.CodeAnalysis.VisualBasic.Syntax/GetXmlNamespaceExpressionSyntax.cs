using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GetXmlNamespaceExpressionSyntax : ExpressionSyntax
	{
		internal XmlPrefixNameSyntax _name;

		public SyntaxToken GetXmlNamespaceKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)base.Green)._getXmlNamespaceKeyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public XmlPrefixNameSyntax Name => GetRed(ref _name, 2);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal GetXmlNamespaceExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GetXmlNamespaceExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(kind, errors, annotations, getXmlNamespaceKeyword, openParenToken, (name != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)name.Green) : null, closeParenToken), null, 0)
		{
		}

		public GetXmlNamespaceExpressionSyntax WithGetXmlNamespaceKeyword(SyntaxToken getXmlNamespaceKeyword)
		{
			return Update(getXmlNamespaceKeyword, OpenParenToken, Name, CloseParenToken);
		}

		public GetXmlNamespaceExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(GetXmlNamespaceKeyword, openParenToken, Name, CloseParenToken);
		}

		public GetXmlNamespaceExpressionSyntax WithName(XmlPrefixNameSyntax name)
		{
			return Update(GetXmlNamespaceKeyword, OpenParenToken, name, CloseParenToken);
		}

		public GetXmlNamespaceExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(GetXmlNamespaceKeyword, OpenParenToken, Name, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _name;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Name;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitGetXmlNamespaceExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGetXmlNamespaceExpression(this);
		}

		public GetXmlNamespaceExpressionSyntax Update(SyntaxToken getXmlNamespaceKeyword, SyntaxToken openParenToken, XmlPrefixNameSyntax name, SyntaxToken closeParenToken)
		{
			if (getXmlNamespaceKeyword != GetXmlNamespaceKeyword || openParenToken != OpenParenToken || name != Name || closeParenToken != CloseParenToken)
			{
				GetXmlNamespaceExpressionSyntax xmlNamespaceExpression = SyntaxFactory.GetXmlNamespaceExpression(getXmlNamespaceKeyword, openParenToken, name, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlNamespaceExpression, annotations);
				}
				return xmlNamespaceExpression;
			}
			return this;
		}
	}
}
