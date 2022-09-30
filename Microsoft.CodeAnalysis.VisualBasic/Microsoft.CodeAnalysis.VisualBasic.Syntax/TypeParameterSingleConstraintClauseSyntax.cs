using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeParameterSingleConstraintClauseSyntax : TypeParameterConstraintClauseSyntax
	{
		internal ConstraintSyntax _constraint;

		public SyntaxToken AsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax)base.Green)._asKeyword, base.Position, 0);

		public ConstraintSyntax Constraint => GetRed(ref _constraint, 1);

		internal TypeParameterSingleConstraintClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeParameterSingleConstraintClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, ConstraintSyntax constraint)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(kind, errors, annotations, asKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)constraint.Green), null, 0)
		{
		}

		public TypeParameterSingleConstraintClauseSyntax WithAsKeyword(SyntaxToken asKeyword)
		{
			return Update(asKeyword, Constraint);
		}

		public TypeParameterSingleConstraintClauseSyntax WithConstraint(ConstraintSyntax constraint)
		{
			return Update(AsKeyword, constraint);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _constraint;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Constraint;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameterSingleConstraintClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeParameterSingleConstraintClause(this);
		}

		public TypeParameterSingleConstraintClauseSyntax Update(SyntaxToken asKeyword, ConstraintSyntax constraint)
		{
			if (asKeyword != AsKeyword || constraint != Constraint)
			{
				TypeParameterSingleConstraintClauseSyntax typeParameterSingleConstraintClauseSyntax = SyntaxFactory.TypeParameterSingleConstraintClause(asKeyword, constraint);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeParameterSingleConstraintClauseSyntax, annotations);
				}
				return typeParameterSingleConstraintClauseSyntax;
			}
			return this;
		}
	}
}
