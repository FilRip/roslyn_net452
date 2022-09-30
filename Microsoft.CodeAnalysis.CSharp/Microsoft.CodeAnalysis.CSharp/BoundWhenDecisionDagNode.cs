using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundWhenDecisionDagNode : BoundDecisionDagNode
    {
        public ImmutableArray<BoundPatternBinding> Bindings { get; }

        public BoundExpression? WhenExpression { get; }

        public BoundDecisionDagNode WhenTrue { get; }

        public BoundDecisionDagNode? WhenFalse { get; }

        public BoundWhenDecisionDagNode(SyntaxNode syntax, ImmutableArray<BoundPatternBinding> bindings, BoundExpression? whenExpression, BoundDecisionDagNode whenTrue, BoundDecisionDagNode? whenFalse, bool hasErrors = false)
            : base(BoundKind.WhenDecisionDagNode, syntax, hasErrors || whenExpression.HasErrors() || whenTrue.HasErrors() || whenFalse.HasErrors())
        {
            Bindings = bindings;
            WhenExpression = whenExpression;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitWhenDecisionDagNode(this);
        }

        public BoundWhenDecisionDagNode Update(ImmutableArray<BoundPatternBinding> bindings, BoundExpression? whenExpression, BoundDecisionDagNode whenTrue, BoundDecisionDagNode? whenFalse)
        {
            if (bindings != Bindings || whenExpression != WhenExpression || whenTrue != WhenTrue || whenFalse != WhenFalse)
            {
                BoundWhenDecisionDagNode boundWhenDecisionDagNode = new BoundWhenDecisionDagNode(Syntax, bindings, whenExpression, whenTrue, whenFalse, base.HasErrors);
                boundWhenDecisionDagNode.CopyAttributes(this);
                return boundWhenDecisionDagNode;
            }
            return this;
        }
    }
}
