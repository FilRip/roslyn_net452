using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundExpressionStatement : BoundStatement
	{
		private readonly BoundExpression _Expression;

		public BoundExpression Expression => _Expression;

		public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
			: base(BoundKind.ExpressionStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitExpressionStatement(this);
		}

		public BoundExpressionStatement Update(BoundExpression expression)
		{
			if (expression != Expression)
			{
				BoundExpressionStatement boundExpressionStatement = new BoundExpressionStatement(base.Syntax, expression, base.HasErrors);
				boundExpressionStatement.CopyAttributes(this);
				return boundExpressionStatement;
			}
			return this;
		}
	}
}
