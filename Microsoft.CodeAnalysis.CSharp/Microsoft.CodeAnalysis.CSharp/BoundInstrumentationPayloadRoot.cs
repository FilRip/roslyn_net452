using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundInstrumentationPayloadRoot : BoundExpression
    {
        public int AnalysisKind { get; }

        public new TypeSymbol Type => base.Type;

        public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, TypeSymbol type, bool hasErrors)
            : base(BoundKind.InstrumentationPayloadRoot, syntax, type, hasErrors)
        {
            AnalysisKind = analysisKind;
        }

        public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, TypeSymbol type)
            : base(BoundKind.InstrumentationPayloadRoot, syntax, type)
        {
            AnalysisKind = analysisKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitInstrumentationPayloadRoot(this);
        }

        public BoundInstrumentationPayloadRoot Update(int analysisKind, TypeSymbol type)
        {
            if (analysisKind != AnalysisKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundInstrumentationPayloadRoot boundInstrumentationPayloadRoot = new BoundInstrumentationPayloadRoot(Syntax, analysisKind, type, base.HasErrors);
                boundInstrumentationPayloadRoot.CopyAttributes(this);
                return boundInstrumentationPayloadRoot;
            }
            return this;
        }
    }
}
