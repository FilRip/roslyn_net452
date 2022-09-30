using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EndRegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken EndKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)base.Green)._endKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken RegionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)base.Green)._regionKeyword, GetChildPosition(2), GetChildIndex(2));

		internal EndRegionDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EndRegionDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax(kind, errors, annotations, hashToken, endKeyword, regionKeyword), null, 0)
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

		public new EndRegionDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, EndKeyword, RegionKeyword);
		}

		public EndRegionDirectiveTriviaSyntax WithEndKeyword(SyntaxToken endKeyword)
		{
			return Update(HashToken, endKeyword, RegionKeyword);
		}

		public EndRegionDirectiveTriviaSyntax WithRegionKeyword(SyntaxToken regionKeyword)
		{
			return Update(HashToken, EndKeyword, regionKeyword);
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
			return visitor.VisitEndRegionDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEndRegionDirectiveTrivia(this);
		}

		public EndRegionDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endKeyword, SyntaxToken regionKeyword)
		{
			if (hashToken != HashToken || endKeyword != EndKeyword || regionKeyword != RegionKeyword)
			{
				EndRegionDirectiveTriviaSyntax endRegionDirectiveTriviaSyntax = SyntaxFactory.EndRegionDirectiveTrivia(hashToken, endKeyword, regionKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(endRegionDirectiveTriviaSyntax, annotations);
				}
				return endRegionDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
