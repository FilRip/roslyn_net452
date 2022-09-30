using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagDeconstructEvaluation : BoundDagEvaluation
    {
        public MethodSymbol DeconstructMethod { get; }

        public BoundDagDeconstructEvaluation(SyntaxNode syntax, MethodSymbol deconstructMethod, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagDeconstructEvaluation, syntax, input, hasErrors || input.HasErrors())
        {
            DeconstructMethod = deconstructMethod;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagDeconstructEvaluation(this);
        }

        public BoundDagDeconstructEvaluation Update(MethodSymbol deconstructMethod, BoundDagTemp input)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(deconstructMethod, DeconstructMethod) || input != base.Input)
            {
                BoundDagDeconstructEvaluation boundDagDeconstructEvaluation = new BoundDagDeconstructEvaluation(Syntax, deconstructMethod, input, base.HasErrors);
                boundDagDeconstructEvaluation.CopyAttributes(this);
                return boundDagDeconstructEvaluation;
            }
            return this;
        }
    }
}
