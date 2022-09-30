using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterpolationSyntax : InterpolatedStringContentSyntax
	{
		internal ExpressionSyntax _expression;

		internal InterpolationAlignmentClauseSyntax _alignmentClause;

		internal InterpolationFormatClauseSyntax _formatClause;

		public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax)base.Green)._openBraceToken, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public InterpolationAlignmentClauseSyntax AlignmentClause => GetRed(ref _alignmentClause, 2);

		public InterpolationFormatClauseSyntax FormatClause => GetRed(ref _formatClause, 3);

		public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax)base.Green)._closeBraceToken, GetChildPosition(4), GetChildIndex(4));

		internal InterpolationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterpolationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(kind, errors, annotations, openBraceToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (alignmentClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)alignmentClause.Green) : null, (formatClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)formatClause.Green) : null, closeBraceToken), null, 0)
		{
		}

		public InterpolationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
		{
			return Update(openBraceToken, Expression, AlignmentClause, FormatClause, CloseBraceToken);
		}

		public InterpolationSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(OpenBraceToken, expression, AlignmentClause, FormatClause, CloseBraceToken);
		}

		public InterpolationSyntax WithAlignmentClause(InterpolationAlignmentClauseSyntax alignmentClause)
		{
			return Update(OpenBraceToken, Expression, alignmentClause, FormatClause, CloseBraceToken);
		}

		public InterpolationSyntax WithFormatClause(InterpolationFormatClauseSyntax formatClause)
		{
			return Update(OpenBraceToken, Expression, AlignmentClause, formatClause, CloseBraceToken);
		}

		public InterpolationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
		{
			return Update(OpenBraceToken, Expression, AlignmentClause, FormatClause, closeBraceToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _expression, 
				2 => _alignmentClause, 
				3 => _formatClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Expression, 
				2 => AlignmentClause, 
				3 => FormatClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInterpolation(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterpolation(this);
		}

		public InterpolationSyntax Update(SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, SyntaxToken closeBraceToken)
		{
			if (openBraceToken != OpenBraceToken || expression != Expression || alignmentClause != AlignmentClause || formatClause != FormatClause || closeBraceToken != CloseBraceToken)
			{
				InterpolationSyntax interpolationSyntax = SyntaxFactory.Interpolation(openBraceToken, expression, alignmentClause, formatClause, closeBraceToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interpolationSyntax, annotations);
				}
				return interpolationSyntax;
			}
			return this;
		}
	}
}
