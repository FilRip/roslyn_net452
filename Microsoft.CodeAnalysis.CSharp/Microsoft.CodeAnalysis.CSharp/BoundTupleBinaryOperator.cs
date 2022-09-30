using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTupleBinaryOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        public BinaryOperatorKind OperatorKind { get; }

        public TupleBinaryOperatorInfo.Multiple Operators { get; }

        public BoundTupleBinaryOperator(SyntaxNode syntax, BoundExpression left, BoundExpression right, BinaryOperatorKind operatorKind, TupleBinaryOperatorInfo.Multiple operators, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TupleBinaryOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            Left = left;
            Right = right;
            OperatorKind = operatorKind;
            Operators = operators;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTupleBinaryOperator(this);
        }

        public BoundTupleBinaryOperator Update(BoundExpression left, BoundExpression right, BinaryOperatorKind operatorKind, TupleBinaryOperatorInfo.Multiple operators, TypeSymbol type)
        {
            if (left != Left || right != Right || operatorKind != OperatorKind || operators != Operators || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTupleBinaryOperator boundTupleBinaryOperator = new BoundTupleBinaryOperator(Syntax, left, right, operatorKind, operators, type, base.HasErrors);
                boundTupleBinaryOperator.CopyAttributes(this);
                return boundTupleBinaryOperator;
            }
            return this;
        }
    }
}
