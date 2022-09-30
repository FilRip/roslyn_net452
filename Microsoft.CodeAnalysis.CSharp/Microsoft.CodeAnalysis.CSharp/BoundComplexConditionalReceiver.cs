using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundComplexConditionalReceiver : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression ValueTypeReceiver { get; }

        public BoundExpression ReferenceTypeReceiver { get; }

        public BoundComplexConditionalReceiver(SyntaxNode syntax, BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ComplexConditionalReceiver, syntax, type, hasErrors || valueTypeReceiver.HasErrors() || referenceTypeReceiver.HasErrors())
        {
            ValueTypeReceiver = valueTypeReceiver;
            ReferenceTypeReceiver = referenceTypeReceiver;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitComplexConditionalReceiver(this);
        }

        public BoundComplexConditionalReceiver Update(BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type)
        {
            if (valueTypeReceiver != ValueTypeReceiver || referenceTypeReceiver != ReferenceTypeReceiver || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundComplexConditionalReceiver boundComplexConditionalReceiver = new BoundComplexConditionalReceiver(Syntax, valueTypeReceiver, referenceTypeReceiver, type, base.HasErrors);
                boundComplexConditionalReceiver.CopyAttributes(this);
                return boundComplexConditionalReceiver;
            }
            return this;
        }
    }
}
