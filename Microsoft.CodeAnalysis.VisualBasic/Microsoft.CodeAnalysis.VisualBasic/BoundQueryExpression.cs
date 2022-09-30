using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundQueryExpression : BoundExpression
	{
		private readonly BoundQueryClauseBase _LastOperator;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)LastOperator);

		public BoundQueryClauseBase LastOperator => _LastOperator;

		public BoundQueryExpression(SyntaxNode syntax, BoundQueryClauseBase lastOperator, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.QueryExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(lastOperator))
		{
			_LastOperator = lastOperator;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitQueryExpression(this);
		}

		public BoundQueryExpression Update(BoundQueryClauseBase lastOperator, TypeSymbol type)
		{
			if (lastOperator != LastOperator || (object)type != base.Type)
			{
				BoundQueryExpression boundQueryExpression = new BoundQueryExpression(base.Syntax, lastOperator, type, base.HasErrors);
				boundQueryExpression.CopyAttributes(this);
				return boundQueryExpression;
			}
			return this;
		}
	}
}
