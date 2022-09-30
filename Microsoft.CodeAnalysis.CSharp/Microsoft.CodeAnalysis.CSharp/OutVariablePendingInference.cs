using System.Diagnostics;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class OutVariablePendingInference : VariablePendingInference
    {
        public override object Display => string.Empty;

        protected override ErrorCode InferenceFailedError => ErrorCode.ERR_TypeInferenceFailedForImplicitlyTypedOutVariable;

        public OutVariablePendingInference(SyntaxNode syntax, Symbol variableSymbol, BoundExpression? receiverOpt, bool hasErrors = false)
            : base(BoundKind.OutVariablePendingInference, syntax, variableSymbol, receiverOpt, hasErrors || receiverOpt.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitOutVariablePendingInference(this);
        }

        public OutVariablePendingInference Update(Symbol variableSymbol, BoundExpression? receiverOpt)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variableSymbol, base.VariableSymbol) || receiverOpt != base.ReceiverOpt)
            {
                OutVariablePendingInference outVariablePendingInference = new OutVariablePendingInference(Syntax, variableSymbol, receiverOpt, base.HasErrors);
                outVariablePendingInference.CopyAttributes(this);
                return outVariablePendingInference;
            }
            return this;
        }
    }
}
