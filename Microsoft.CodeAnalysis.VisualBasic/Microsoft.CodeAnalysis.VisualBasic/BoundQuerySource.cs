using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundQuerySource : BoundQueryPart
	{
		private readonly BoundExpression _Expression;

		public override Symbol ExpressionSymbol => Expression.ExpressionSymbol;

		public override LookupResultKind ResultKind => Expression.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Expression);

		public BoundExpression Expression => _Expression;

		public BoundQuerySource(BoundExpression source)
			: this(source.Syntax, source, source.Type)
		{
		}

		public BoundQuerySource(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.QuerySource, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_Expression = expression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitQuerySource(this);
		}

		public BoundQuerySource Update(BoundExpression expression, TypeSymbol type)
		{
			if (expression != Expression || (object)type != base.Type)
			{
				BoundQuerySource boundQuerySource = new BoundQuerySource(base.Syntax, expression, type, base.HasErrors);
				boundQuerySource.CopyAttributes(this);
				return boundQuerySource;
			}
			return this;
		}
	}
}
