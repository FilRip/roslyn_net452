using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ForStepClauseSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _stepValue;

		public SyntaxToken StepKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)base.Green)._stepKeyword, base.Position, 0);

		public ExpressionSyntax StepValue => GetRed(ref _stepValue, 1);

		internal ForStepClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ForStepClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax stepKeyword, ExpressionSyntax stepValue)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax(kind, errors, annotations, stepKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)stepValue.Green), null, 0)
		{
		}

		public ForStepClauseSyntax WithStepKeyword(SyntaxToken stepKeyword)
		{
			return Update(stepKeyword, StepValue);
		}

		public ForStepClauseSyntax WithStepValue(ExpressionSyntax stepValue)
		{
			return Update(StepKeyword, stepValue);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _stepValue;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return StepValue;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitForStepClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitForStepClause(this);
		}

		public ForStepClauseSyntax Update(SyntaxToken stepKeyword, ExpressionSyntax stepValue)
		{
			if (stepKeyword != StepKeyword || stepValue != StepValue)
			{
				ForStepClauseSyntax forStepClauseSyntax = SyntaxFactory.ForStepClause(stepKeyword, stepValue);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(forStepClauseSyntax, annotations);
				}
				return forStepClauseSyntax;
			}
			return this;
		}
	}
}
