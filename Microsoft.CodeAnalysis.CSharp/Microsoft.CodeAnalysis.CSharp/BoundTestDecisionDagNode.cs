using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTestDecisionDagNode : BoundDecisionDagNode
    {
        public BoundDagTest Test { get; }

        public BoundDecisionDagNode WhenTrue { get; }

        public BoundDecisionDagNode WhenFalse { get; }

        public BoundTestDecisionDagNode(SyntaxNode syntax, BoundDagTest test, BoundDecisionDagNode whenTrue, BoundDecisionDagNode whenFalse, bool hasErrors = false)
            : base(BoundKind.TestDecisionDagNode, syntax, hasErrors || test.HasErrors() || whenTrue.HasErrors() || whenFalse.HasErrors())
        {
            Test = test;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTestDecisionDagNode(this);
        }

        public BoundTestDecisionDagNode Update(BoundDagTest test, BoundDecisionDagNode whenTrue, BoundDecisionDagNode whenFalse)
        {
            if (test != Test || whenTrue != WhenTrue || whenFalse != WhenFalse)
            {
                BoundTestDecisionDagNode boundTestDecisionDagNode = new BoundTestDecisionDagNode(Syntax, test, whenTrue, whenFalse, base.HasErrors);
                boundTestDecisionDagNode.CopyAttributes(this);
                return boundTestDecisionDagNode;
            }
            return this;
        }
    }
}
