using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class RegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken RegionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green)._regionKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green)._name, GetChildPosition(2), GetChildIndex(2));

		internal RegionDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal RegionDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax(kind, errors, annotations, hashToken, regionKeyword, name), null, 0)
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

		public new RegionDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, RegionKeyword, Name);
		}

		public RegionDirectiveTriviaSyntax WithRegionKeyword(SyntaxToken regionKeyword)
		{
			return Update(HashToken, regionKeyword, Name);
		}

		public RegionDirectiveTriviaSyntax WithName(SyntaxToken name)
		{
			return Update(HashToken, RegionKeyword, name);
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
			return visitor.VisitRegionDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitRegionDirectiveTrivia(this);
		}

		public RegionDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken regionKeyword, SyntaxToken name)
		{
			if (hashToken != HashToken || regionKeyword != RegionKeyword || name != Name)
			{
				RegionDirectiveTriviaSyntax regionDirectiveTriviaSyntax = SyntaxFactory.RegionDirectiveTrivia(hashToken, regionKeyword, name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(regionDirectiveTriviaSyntax, annotations);
				}
				return regionDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
