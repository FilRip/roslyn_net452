using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagRelationalTest : BoundDagTest
    {
        public BinaryOperatorKind Relation => OperatorKind.Operator();

        public BinaryOperatorKind OperatorKind { get; }

        public ConstantValue Value { get; }

        public BoundDagRelationalTest(SyntaxNode syntax, BinaryOperatorKind operatorKind, ConstantValue value, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagRelationalTest, syntax, input, hasErrors || input.HasErrors())
        {
            OperatorKind = operatorKind;
            Value = value;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagRelationalTest(this);
        }

        public BoundDagRelationalTest Update(BinaryOperatorKind operatorKind, ConstantValue value, BoundDagTemp input)
        {
            if (operatorKind != OperatorKind || value != Value || input != base.Input)
            {
                BoundDagRelationalTest boundDagRelationalTest = new BoundDagRelationalTest(Syntax, operatorKind, value, input, base.HasErrors);
                boundDagRelationalTest.CopyAttributes(this);
                return boundDagRelationalTest;
            }
            return this;
        }
    }
}
