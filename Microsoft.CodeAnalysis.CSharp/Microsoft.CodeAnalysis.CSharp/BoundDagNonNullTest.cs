using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagNonNullTest : BoundDagTest
    {
        public bool IsExplicitTest { get; }

        public BoundDagNonNullTest(SyntaxNode syntax, bool isExplicitTest, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagNonNullTest, syntax, input, hasErrors || input.HasErrors())
        {
            IsExplicitTest = isExplicitTest;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagNonNullTest(this);
        }

        public BoundDagNonNullTest Update(bool isExplicitTest, BoundDagTemp input)
        {
            if (isExplicitTest != IsExplicitTest || input != base.Input)
            {
                BoundDagNonNullTest boundDagNonNullTest = new BoundDagNonNullTest(Syntax, isExplicitTest, input, base.HasErrors);
                boundDagNonNullTest.CopyAttributes(this);
                return boundDagNonNullTest;
            }
            return this;
        }
    }
}
