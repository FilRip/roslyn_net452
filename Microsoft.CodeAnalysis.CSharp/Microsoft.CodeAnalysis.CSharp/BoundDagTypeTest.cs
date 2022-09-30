using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagTypeTest : BoundDagTest
    {
        public TypeSymbol Type { get; }

        public BoundDagTypeTest(SyntaxNode syntax, TypeSymbol type, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagTypeTest, syntax, input, hasErrors || input.HasErrors())
        {
            Type = type;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagTypeTest(this);
        }

        public BoundDagTypeTest Update(TypeSymbol type, BoundDagTemp input)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything) || input != base.Input)
            {
                BoundDagTypeTest boundDagTypeTest = new BoundDagTypeTest(Syntax, type, input, base.HasErrors);
                boundDagTypeTest.CopyAttributes(this);
                return boundDagTypeTest;
            }
            return this;
        }
    }
}
