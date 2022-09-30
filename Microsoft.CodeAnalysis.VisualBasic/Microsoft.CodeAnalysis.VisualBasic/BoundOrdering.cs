using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundOrdering : BoundQueryPart
	{
		private readonly BoundExpression _UnderlyingExpression;

		public override Symbol ExpressionSymbol => UnderlyingExpression.ExpressionSymbol;

		public override LookupResultKind ResultKind => UnderlyingExpression.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)UnderlyingExpression);

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public BoundOrdering(SyntaxNode syntax, BoundExpression underlyingExpression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Ordering, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_UnderlyingExpression = underlyingExpression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitOrdering(this);
		}

		public BoundOrdering Update(BoundExpression underlyingExpression, TypeSymbol type)
		{
			if (underlyingExpression != UnderlyingExpression || (object)type != base.Type)
			{
				BoundOrdering boundOrdering = new BoundOrdering(base.Syntax, underlyingExpression, type, base.HasErrors);
				boundOrdering.CopyAttributes(this);
				return boundOrdering;
			}
			return this;
		}
	}
}
