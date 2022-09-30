using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EndIfDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken EndKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)base.Green)._endKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)base.Green)._ifKeyword, GetChildPosition(2), GetChildIndex(2));

		internal EndIfDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EndIfDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax(kind, errors, annotations, hashToken, endKeyword, ifKeyword), null, 0)
		{
		}

		internal override SyntaxToken GetHashTokenCore()
		{
			return HashToken;
		}

		internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
		{
			return WithHashToken(hashToken);
		}

		public new EndIfDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, EndKeyword, IfKeyword);
		}

		public EndIfDirectiveTriviaSyntax WithEndKeyword(SyntaxToken endKeyword)
		{
			return Update(HashToken, endKeyword, IfKeyword);
		}

		public EndIfDirectiveTriviaSyntax WithIfKeyword(SyntaxToken ifKeyword)
		{
			return Update(HashToken, EndKeyword, ifKeyword);
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
			return visitor.VisitEndIfDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEndIfDirectiveTrivia(this);
		}

		public EndIfDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endKeyword, SyntaxToken ifKeyword)
		{
			if (hashToken != HashToken || endKeyword != EndKeyword || ifKeyword != IfKeyword)
			{
				EndIfDirectiveTriviaSyntax endIfDirectiveTriviaSyntax = SyntaxFactory.EndIfDirectiveTrivia(hashToken, endKeyword, ifKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(endIfDirectiveTriviaSyntax, annotations);
				}
				return endIfDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
