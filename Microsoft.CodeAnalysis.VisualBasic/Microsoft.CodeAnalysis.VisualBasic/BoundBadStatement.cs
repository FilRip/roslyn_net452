using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBadStatement : BoundStatement, IBoundInvalidNode
	{
		private readonly ImmutableArray<BoundNode> _ChildBoundNodes;

		protected override ImmutableArray<BoundNode> Children => ChildBoundNodes;

		private ImmutableArray<BoundNode> IBoundInvalidNode_InvalidNodeChildren => ChildBoundNodes;

		public ImmutableArray<BoundNode> ChildBoundNodes => _ChildBoundNodes;

		public BoundBadStatement(SyntaxNode syntax, ImmutableArray<BoundNode> childBoundNodes, bool hasErrors = false)
			: base(BoundKind.BadStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(childBoundNodes))
		{
			_ChildBoundNodes = childBoundNodes;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBadStatement(this);
		}

		public BoundBadStatement Update(ImmutableArray<BoundNode> childBoundNodes)
		{
			if (childBoundNodes != ChildBoundNodes)
			{
				BoundBadStatement boundBadStatement = new BoundBadStatement(base.Syntax, childBoundNodes, base.HasErrors);
				boundBadStatement.CopyAttributes(this);
				return boundBadStatement;
			}
			return this;
		}
	}
}
