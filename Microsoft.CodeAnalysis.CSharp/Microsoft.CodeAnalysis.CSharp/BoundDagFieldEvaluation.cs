using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagFieldEvaluation : BoundDagEvaluation
    {
        public FieldSymbol Field { get; }

        public BoundDagFieldEvaluation(SyntaxNode syntax, FieldSymbol field, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagFieldEvaluation, syntax, input, hasErrors || input.HasErrors())
        {
            Field = field;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagFieldEvaluation(this);
        }

        public BoundDagFieldEvaluation Update(FieldSymbol field, BoundDagTemp input)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, Field) || input != base.Input)
            {
                BoundDagFieldEvaluation boundDagFieldEvaluation = new BoundDagFieldEvaluation(Syntax, field, input, base.HasErrors);
                boundDagFieldEvaluation.CopyAttributes(this);
                return boundDagFieldEvaluation;
            }
            return this;
        }
    }
}
