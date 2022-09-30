using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SpecialConstraintSyntax : ConstraintSyntax
	{
		public SyntaxToken ConstraintKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)base.Green)._constraintKeyword, base.Position, 0);

		internal SpecialConstraintSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SpecialConstraintSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax constraintKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(kind, errors, annotations, constraintKeyword), null, 0)
		{
		}

		public SpecialConstraintSyntax WithConstraintKeyword(SyntaxToken constraintKeyword)
		{
			return Update(Kind(), constraintKeyword);
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
			return visitor.VisitSpecialConstraint(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSpecialConstraint(this);
		}

		public SpecialConstraintSyntax Update(SyntaxKind kind, SyntaxToken constraintKeyword)
		{
			if (kind != Kind() || constraintKeyword != ConstraintKeyword)
			{
				SpecialConstraintSyntax specialConstraintSyntax = SyntaxFactory.SpecialConstraint(kind, constraintKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(specialConstraintSyntax, annotations);
				}
				return specialConstraintSyntax;
			}
			return this;
		}
	}
}
