using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EndExternalSourceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken EndKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax)base.Green)._endKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken ExternalSourceKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax)base.Green)._externalSourceKeyword, GetChildPosition(2), GetChildIndex(2));

		internal EndExternalSourceDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EndExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax(kind, errors, annotations, hashToken, endKeyword, externalSourceKeyword), null, 0)
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

		public new EndExternalSourceDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, EndKeyword, ExternalSourceKeyword);
		}

		public EndExternalSourceDirectiveTriviaSyntax WithEndKeyword(SyntaxToken endKeyword)
		{
			return Update(HashToken, endKeyword, ExternalSourceKeyword);
		}

		public EndExternalSourceDirectiveTriviaSyntax WithExternalSourceKeyword(SyntaxToken externalSourceKeyword)
		{
			return Update(HashToken, EndKeyword, externalSourceKeyword);
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
			return visitor.VisitEndExternalSourceDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEndExternalSourceDirectiveTrivia(this);
		}

		public EndExternalSourceDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endKeyword, SyntaxToken externalSourceKeyword)
		{
			if (hashToken != HashToken || endKeyword != EndKeyword || externalSourceKeyword != ExternalSourceKeyword)
			{
				EndExternalSourceDirectiveTriviaSyntax endExternalSourceDirectiveTriviaSyntax = SyntaxFactory.EndExternalSourceDirectiveTrivia(hashToken, endKeyword, externalSourceKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(endExternalSourceDirectiveTriviaSyntax, annotations);
				}
				return endExternalSourceDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
