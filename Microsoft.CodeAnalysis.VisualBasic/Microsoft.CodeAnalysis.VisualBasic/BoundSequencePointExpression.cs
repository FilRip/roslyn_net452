using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSequencePointExpression : BoundExpression
	{
		private readonly BoundExpression _Expression;

		public override bool IsLValue => Expression.IsLValue;

		public BoundExpression Expression => _Expression;

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundSequencePointExpression MakeRValue()
		{
			if (Expression.IsLValue)
			{
				return Update(Expression.MakeRValue(), base.Type);
			}
			return this;
		}

		public BoundSequencePointExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.SequencePointExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSequencePointExpression(this);
		}

		public BoundSequencePointExpression Update(BoundExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundSequencePointExpression boundSequencePointExpression = new BoundSequencePointExpression(base.Syntax, expression, type, base.HasErrors);
				boundSequencePointExpression.CopyAttributes(this);
				return boundSequencePointExpression;
			}
			return this;
		}
	}
}
