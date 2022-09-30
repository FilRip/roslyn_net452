using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnconvertedAddressOfOperator : BoundExpression
    {
        public override object Display => $"&{Operand.Display}";

        public BoundMethodGroup Operand { get; }

        public new TypeSymbol? Type => base.Type;

        public BoundUnconvertedAddressOfOperator(SyntaxNode syntax, BoundMethodGroup operand, bool hasErrors = false)
            : base(BoundKind.UnconvertedAddressOfOperator, syntax, null, hasErrors || operand.HasErrors())
        {
            Operand = operand;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnconvertedAddressOfOperator(this);
        }

        public BoundUnconvertedAddressOfOperator Update(BoundMethodGroup operand)
        {
            if (operand != Operand)
            {
                BoundUnconvertedAddressOfOperator boundUnconvertedAddressOfOperator = new BoundUnconvertedAddressOfOperator(Syntax, operand, base.HasErrors);
                boundUnconvertedAddressOfOperator.CopyAttributes(this);
                return boundUnconvertedAddressOfOperator;
            }
            return this;
        }
    }
}
