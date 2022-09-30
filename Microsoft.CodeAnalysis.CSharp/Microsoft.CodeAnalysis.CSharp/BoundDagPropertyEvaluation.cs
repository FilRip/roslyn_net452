using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDagPropertyEvaluation : BoundDagEvaluation
    {
        public PropertySymbol Property { get; }

        public BoundDagPropertyEvaluation(SyntaxNode syntax, PropertySymbol property, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagPropertyEvaluation, syntax, input, hasErrors || input.HasErrors())
        {
            Property = property;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDagPropertyEvaluation(this);
        }

        public BoundDagPropertyEvaluation Update(PropertySymbol property, BoundDagTemp input)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, Property) || input != base.Input)
            {
                BoundDagPropertyEvaluation boundDagPropertyEvaluation = new BoundDagPropertyEvaluation(Syntax, property, input, base.HasErrors);
                boundDagPropertyEvaluation.CopyAttributes(this);
                return boundDagPropertyEvaluation;
            }
            return this;
        }
    }
}
