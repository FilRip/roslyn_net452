using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundRangeExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression? LeftOperandOpt { get; }

        public BoundExpression? RightOperandOpt { get; }

        public MethodSymbol? MethodOpt { get; }

        public BoundRangeExpression(SyntaxNode syntax, BoundExpression? leftOperandOpt, BoundExpression? rightOperandOpt, MethodSymbol? methodOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RangeExpression, syntax, type, hasErrors || leftOperandOpt.HasErrors() || rightOperandOpt.HasErrors())
        {
            LeftOperandOpt = leftOperandOpt;
            RightOperandOpt = rightOperandOpt;
            MethodOpt = methodOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRangeExpression(this);
        }

        public BoundRangeExpression Update(BoundExpression? leftOperandOpt, BoundExpression? rightOperandOpt, MethodSymbol? methodOpt, TypeSymbol type)
        {
            if (leftOperandOpt != LeftOperandOpt || rightOperandOpt != RightOperandOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundRangeExpression boundRangeExpression = new BoundRangeExpression(Syntax, leftOperandOpt, rightOperandOpt, methodOpt, type, base.HasErrors);
                boundRangeExpression.CopyAttributes(this);
                return boundRangeExpression;
            }
            return this;
        }
    }
}
