using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class BadDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		internal BadDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal BadDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax(kind, errors, annotations, hashToken), null, 0)
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

		public new BadDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken);
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
			return visitor.VisitBadDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitBadDirectiveTrivia(this);
		}

		public BadDirectiveTriviaSyntax Update(SyntaxToken hashToken)
		{
			if (hashToken != HashToken)
			{
				BadDirectiveTriviaSyntax badDirectiveTriviaSyntax = SyntaxFactory.BadDirectiveTrivia(hashToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(badDirectiveTriviaSyntax, annotations);
				}
				return badDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
