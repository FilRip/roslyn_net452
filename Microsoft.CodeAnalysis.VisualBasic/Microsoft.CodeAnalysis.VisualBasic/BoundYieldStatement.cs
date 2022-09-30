using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundYieldStatement : BoundStatement
	{
		private readonly BoundExpression _Expression;

		public BoundExpression Expression => _Expression;

		internal BoundYieldStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors, bool returnTypeIsBeingInferred)
			: base(BoundKind.YieldStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		public BoundYieldStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
			: base(BoundKind.YieldStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitYieldStatement(this);
		}

		public BoundYieldStatement Update(BoundExpression expression)
		{
			if (expression != Expression)
			{
				BoundYieldStatement boundYieldStatement = new BoundYieldStatement(base.Syntax, expression, base.HasErrors);
				boundYieldStatement.CopyAttributes(this);
				return boundYieldStatement;
			}
			return this;
		}
	}
}
