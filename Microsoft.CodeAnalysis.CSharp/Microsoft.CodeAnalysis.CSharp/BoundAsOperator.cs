using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAsOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public BoundTypeExpression TargetType { get; }

        public Conversion Conversion { get; }

        public BoundAsOperator(SyntaxNode syntax, BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AsOperator, syntax, type, hasErrors || operand.HasErrors() || targetType.HasErrors())
        {
            Operand = operand;
            TargetType = targetType;
            Conversion = conversion;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAsOperator(this);
        }

        public BoundAsOperator Update(BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type)
        {
            if (operand != Operand || targetType != TargetType || conversion != Conversion || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAsOperator boundAsOperator = new BoundAsOperator(Syntax, operand, targetType, conversion, type, base.HasErrors);
                boundAsOperator.CopyAttributes(this);
                return boundAsOperator;
            }
            return this;
        }
    }
}
