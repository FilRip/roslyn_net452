using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeParameterMultipleConstraintClauseSyntax : TypeParameterConstraintClauseSyntax
	{
		internal SyntaxNode _constraints;

		public SyntaxToken AsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)base.Green)._asKeyword, base.Position, 0);

		public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)base.Green)._openBraceToken, GetChildPosition(1), GetChildIndex(1));

		public SeparatedSyntaxList<ConstraintSyntax> Constraints
		{
			get
			{
				SyntaxNode red = GetRed(ref _constraints, 2);
				return (red == null) ? default(SeparatedSyntaxList<ConstraintSyntax>) : new SeparatedSyntaxList<ConstraintSyntax>(red, GetChildIndex(2));
			}
		}

		public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)base.Green)._closeBraceToken, GetChildPosition(3), GetChildIndex(3));

		internal TypeParameterMultipleConstraintClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeParameterMultipleConstraintClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, SyntaxNode constraints, PunctuationSyntax closeBraceToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(kind, errors, annotations, asKeyword, openBraceToken, constraints?.Green, closeBraceToken), null, 0)
		{
		}

		public TypeParameterMultipleConstraintClauseSyntax WithAsKeyword(SyntaxToken asKeyword)
		{
			return Update(asKeyword, OpenBraceToken, Constraints, CloseBraceToken);
		}

		public TypeParameterMultipleConstraintClauseSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
		{
			return Update(AsKeyword, openBraceToken, Constraints, CloseBraceToken);
		}

		public TypeParameterMultipleConstraintClauseSyntax WithConstraints(SeparatedSyntaxList<ConstraintSyntax> constraints)
		{
			return Update(AsKeyword, OpenBraceToken, constraints, CloseBraceToken);
		}

		public TypeParameterMultipleConstraintClauseSyntax AddConstraints(params ConstraintSyntax[] items)
		{
			return WithConstraints(Constraints.AddRange(items));
		}

		public TypeParameterMultipleConstraintClauseSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
		{
			return Update(AsKeyword, OpenBraceToken, Constraints, closeBraceToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _constraints;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _constraints, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameterMultipleConstraintClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeParameterMultipleConstraintClause(this);
		}

		public TypeParameterMultipleConstraintClauseSyntax Update(SyntaxToken asKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<ConstraintSyntax> constraints, SyntaxToken closeBraceToken)
		{
			if (asKeyword != AsKeyword || openBraceToken != OpenBraceToken || constraints != Constraints || closeBraceToken != CloseBraceToken)
			{
				TypeParameterMultipleConstraintClauseSyntax typeParameterMultipleConstraintClauseSyntax = SyntaxFactory.TypeParameterMultipleConstraintClause(asKeyword, openBraceToken, constraints, closeBraceToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeParameterMultipleConstraintClauseSyntax, annotations);
				}
				return typeParameterMultipleConstraintClauseSyntax;
			}
			return this;
		}
	}
}
