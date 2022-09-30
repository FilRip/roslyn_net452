using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ArrayRankSpecifierSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)base.Green)._openParenToken, base.Position, 0);

		public SyntaxTokenList CommaTokens
		{
			get
			{
				GreenNode commaTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)base.Green)._commaTokens;
				return (commaTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, commaTokens, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		public int Rank => CommaTokens.Count + 1;

		internal ArrayRankSpecifierSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ArrayRankSpecifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, GreenNode commaTokens, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax(kind, errors, annotations, openParenToken, commaTokens, closeParenToken), null, 0)
		{
		}

		public ArrayRankSpecifierSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, CommaTokens, CloseParenToken);
		}

		public ArrayRankSpecifierSyntax WithCommaTokens(SyntaxTokenList commaTokens)
		{
			return Update(OpenParenToken, commaTokens, CloseParenToken);
		}

		public ArrayRankSpecifierSyntax AddCommaTokens(params SyntaxToken[] items)
		{
			return WithCommaTokens(CommaTokens.AddRange(items));
		}

		public ArrayRankSpecifierSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, CommaTokens, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitArrayRankSpecifier(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitArrayRankSpecifier(this);
		}

		public ArrayRankSpecifierSyntax Update(SyntaxToken openParenToken, SyntaxTokenList commaTokens, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || commaTokens != CommaTokens || closeParenToken != CloseParenToken)
			{
				ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = SyntaxFactory.ArrayRankSpecifier(openParenToken, commaTokens, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(arrayRankSpecifierSyntax, annotations);
				}
				return arrayRankSpecifierSyntax;
			}
			return this;
		}
	}
}
