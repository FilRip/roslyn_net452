using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundParenthesized : BoundExpression
	{
		private readonly BoundExpression _Expression;

		public override ConstantValue ConstantValueOpt => Expression.ConstantValueOpt;

		public BoundExpression Expression => _Expression;

		public BoundParenthesized(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Parenthesized, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitParenthesized(this);
		}

		public BoundParenthesized Update(BoundExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundParenthesized boundParenthesized = new BoundParenthesized(base.Syntax, expression, type, base.HasErrors);
				boundParenthesized.CopyAttributes(this);
				return boundParenthesized;
			}
			return this;
		}
	}
}
