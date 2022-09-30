using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundThrowStatement : BoundStatement
	{
		private readonly BoundExpression _ExpressionOpt;

		public BoundExpression ExpressionOpt => _ExpressionOpt;

		public BoundThrowStatement(SyntaxNode syntax, BoundExpression expressionOpt, bool hasErrors = false)
			: base(BoundKind.ThrowStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expressionOpt))
		{
			_ExpressionOpt = expressionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitThrowStatement(this);
		}

		public BoundThrowStatement Update(BoundExpression expressionOpt)
		{
			if (expressionOpt != ExpressionOpt)
			{
				BoundThrowStatement boundThrowStatement = new BoundThrowStatement(base.Syntax, expressionOpt, base.HasErrors);
				boundThrowStatement.CopyAttributes(this);
				return boundThrowStatement;
			}
			return this;
		}
	}
}
