using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundInterpolation : BoundNode
	{
		private readonly BoundExpression _Expression;

		private readonly BoundExpression _AlignmentOpt;

		private readonly BoundLiteral _FormatStringOpt;

		public BoundExpression Expression => _Expression;

		public BoundExpression AlignmentOpt => _AlignmentOpt;

		public BoundLiteral FormatStringOpt => _FormatStringOpt;

		public BoundInterpolation(SyntaxNode syntax, BoundExpression expression, BoundExpression alignmentOpt, BoundLiteral formatStringOpt, bool hasErrors = false)
			: base(BoundKind.Interpolation, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression) || BoundNodeExtensions.NonNullAndHasErrors(alignmentOpt) || BoundNodeExtensions.NonNullAndHasErrors(formatStringOpt))
		{
			_Expression = expression;
			_AlignmentOpt = alignmentOpt;
			_FormatStringOpt = formatStringOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitInterpolation(this);
		}

		public BoundInterpolation Update(BoundExpression expression, BoundExpression alignmentOpt, BoundLiteral formatStringOpt)
		{
			if (expression != Expression || alignmentOpt != AlignmentOpt || formatStringOpt != FormatStringOpt)
			{
				BoundInterpolation boundInterpolation = new BoundInterpolation(base.Syntax, expression, alignmentOpt, formatStringOpt, base.HasErrors);
				boundInterpolation.CopyAttributes(this);
				return boundInterpolation;
			}
			return this;
		}
	}
}
