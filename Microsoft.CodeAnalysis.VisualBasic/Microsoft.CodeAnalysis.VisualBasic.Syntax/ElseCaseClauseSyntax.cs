using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ElseCaseClauseSyntax : CaseClauseSyntax
	{
		public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax)base.Green)._elseKeyword, base.Position, 0);

		internal ElseCaseClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ElseCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax(kind, errors, annotations, elseKeyword), null, 0)
		{
		}

		public ElseCaseClauseSyntax WithElseKeyword(SyntaxToken elseKeyword)
		{
			return Update(elseKeyword);
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
			return visitor.VisitElseCaseClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitElseCaseClause(this);
		}

		public ElseCaseClauseSyntax Update(SyntaxToken elseKeyword)
		{
			if (elseKeyword != ElseKeyword)
			{
				ElseCaseClauseSyntax elseCaseClauseSyntax = SyntaxFactory.ElseCaseClause(elseKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(elseCaseClauseSyntax, annotations);
				}
				return elseCaseClauseSyntax;
			}
			return this;
		}
	}
}
