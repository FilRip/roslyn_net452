using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagExplicitNullTest : BoundDagTest
    {
        public BoundDagExplicitNullTest(SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagExplicitNullTest, syntax, input, hasErrors || input.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagExplicitNullTest(this);
        }

        public BoundDagExplicitNullTest Update(BoundDagTemp input)
        {
            if (input != base.Input)
            {
                BoundDagExplicitNullTest boundDagExplicitNullTest = new BoundDagExplicitNullTest(Syntax, input, base.HasErrors);
                boundDagExplicitNullTest.CopyAttributes(this);
                return boundDagExplicitNullTest;
            }
            return this;
        }
    }
}
