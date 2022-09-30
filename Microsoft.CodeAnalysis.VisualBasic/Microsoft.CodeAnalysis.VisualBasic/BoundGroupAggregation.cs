using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundGroupAggregation : BoundQueryPart
	{
		private readonly BoundExpression _Group;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Group);

		public BoundExpression Group => _Group;

		public BoundGroupAggregation(SyntaxNode syntax, BoundExpression group, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.GroupAggregation, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(group))
		{
			_Group = group;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitGroupAggregation(this);
		}

		public BoundGroupAggregation Update(BoundExpression group, TypeSymbol type)
		{
			if (group != Group || (object)type != base.Type)
			{
				BoundGroupAggregation boundGroupAggregation = new BoundGroupAggregation(base.Syntax, group, type, base.HasErrors);
				boundGroupAggregation.CopyAttributes(this);
				return boundGroupAggregation;
			}
			return this;
		}
	}
}
