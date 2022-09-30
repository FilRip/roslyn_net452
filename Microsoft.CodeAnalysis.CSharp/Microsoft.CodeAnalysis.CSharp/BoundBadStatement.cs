using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundBadStatement : BoundStatement, IBoundInvalidNode
    {
        protected override ImmutableArray<BoundNode?> Children => ChildBoundNodes;

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => ChildBoundNodes;

        public ImmutableArray<BoundNode> ChildBoundNodes { get; }

        public BoundBadStatement(SyntaxNode syntax, ImmutableArray<BoundNode> childBoundNodes, bool hasErrors = false)
            : base(BoundKind.BadStatement, syntax, hasErrors || childBoundNodes.HasErrors())
        {
            ChildBoundNodes = childBoundNodes;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitBadStatement(this);
        }

        public BoundBadStatement Update(ImmutableArray<BoundNode> childBoundNodes)
        {
            if (childBoundNodes != ChildBoundNodes)
            {
                BoundBadStatement boundBadStatement = new BoundBadStatement(Syntax, childBoundNodes, base.HasErrors);
                boundBadStatement.CopyAttributes(this);
                return boundBadStatement;
            }
            return this;
        }
    }
}
