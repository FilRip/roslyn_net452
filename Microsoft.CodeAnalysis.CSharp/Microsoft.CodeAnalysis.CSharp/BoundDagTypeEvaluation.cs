using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagTypeEvaluation : BoundDagEvaluation
    {
        public TypeSymbol Type { get; }

        public BoundDagTypeEvaluation(SyntaxNode syntax, TypeSymbol type, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagTypeEvaluation, syntax, input, hasErrors || input.HasErrors())
        {
            Type = type;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagTypeEvaluation(this);
        }

        public BoundDagTypeEvaluation Update(TypeSymbol type, BoundDagTemp input)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything) || input != base.Input)
            {
                BoundDagTypeEvaluation boundDagTypeEvaluation = new BoundDagTypeEvaluation(Syntax, type, input, base.HasErrors);
                boundDagTypeEvaluation.CopyAttributes(this);
                return boundDagTypeEvaluation;
            }
            return this;
        }
    }
}
