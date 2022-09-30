using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAddressOfOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public bool IsManaged { get; }

        public BoundAddressOfOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operand, isManaged: false, type, hasErrors)
        {
        }

        public BoundAddressOfOperator(SyntaxNode syntax, BoundExpression operand, bool isManaged, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AddressOfOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
            IsManaged = isManaged;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAddressOfOperator(this);
        }

        public BoundAddressOfOperator Update(BoundExpression operand, bool isManaged, TypeSymbol type)
        {
            if (operand != Operand || isManaged != IsManaged || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAddressOfOperator boundAddressOfOperator = new BoundAddressOfOperator(Syntax, operand, isManaged, type, base.HasErrors);
                boundAddressOfOperator.CopyAttributes(this);
                return boundAddressOfOperator;
            }
            return this;
        }
    }
}
