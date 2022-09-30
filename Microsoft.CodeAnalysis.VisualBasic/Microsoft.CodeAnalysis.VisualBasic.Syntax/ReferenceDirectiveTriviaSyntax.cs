using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ReferenceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		public new SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)base.Green)._hashToken, base.Position, 0);

		public SyntaxToken ReferenceKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)base.Green)._referenceKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken File => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)base.Green)._file, GetChildPosition(2), GetChildIndex(2));

		internal ReferenceDirectiveTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ReferenceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax(kind, errors, annotations, hashToken, referenceKeyword, file), null, 0)
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

		public new ReferenceDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
		{
			return Update(hashToken, ReferenceKeyword, File);
		}

		public ReferenceDirectiveTriviaSyntax WithReferenceKeyword(SyntaxToken referenceKeyword)
		{
			return Update(HashToken, referenceKeyword, File);
		}

		public ReferenceDirectiveTriviaSyntax WithFile(SyntaxToken file)
		{
			return Update(HashToken, ReferenceKeyword, file);
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
			return visitor.VisitReferenceDirectiveTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitReferenceDirectiveTrivia(this);
		}

		public ReferenceDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken referenceKeyword, SyntaxToken file)
		{
			if (hashToken != HashToken || referenceKeyword != ReferenceKeyword || file != File)
			{
				ReferenceDirectiveTriviaSyntax referenceDirectiveTriviaSyntax = SyntaxFactory.ReferenceDirectiveTrivia(hashToken, referenceKeyword, file);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(referenceDirectiveTriviaSyntax, annotations);
				}
				return referenceDirectiveTriviaSyntax;
			}
			return this;
		}
	}
}
