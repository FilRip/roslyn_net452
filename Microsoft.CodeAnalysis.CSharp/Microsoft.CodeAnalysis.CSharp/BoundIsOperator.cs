using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIsOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public BoundTypeExpression TargetType { get; }

        public Conversion Conversion { get; }

        public BoundIsOperator(SyntaxNode syntax, BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IsOperator, syntax, type, hasErrors || operand.HasErrors() || targetType.HasErrors())
        {
            Operand = operand;
            TargetType = targetType;
            Conversion = conversion;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIsOperator(this);
        }

        public BoundIsOperator Update(BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type)
        {
            if (operand != Operand || targetType != TargetType || conversion != Conversion || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundIsOperator boundIsOperator = new BoundIsOperator(Syntax, operand, targetType, conversion, type, base.HasErrors);
                boundIsOperator.CopyAttributes(this);
                return boundIsOperator;
            }
            return this;
        }
    }
}
