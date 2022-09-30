using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagValueTest : BoundDagTest
    {
        public ConstantValue Value { get; }

        public BoundDagValueTest(SyntaxNode syntax, ConstantValue value, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagValueTest, syntax, input, hasErrors || input.HasErrors())
        {
            Value = value;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagValueTest(this);
        }

        public BoundDagValueTest Update(ConstantValue value, BoundDagTemp input)
        {
            if (value != Value || input != base.Input)
            {
                BoundDagValueTest boundDagValueTest = new BoundDagValueTest(Syntax, value, input, base.HasErrors);
                boundDagValueTest.CopyAttributes(this);
                return boundDagValueTest;
            }
            return this;
        }
    }
}
