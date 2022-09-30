using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPointerIndirectionOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Operand);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public BoundPointerIndirectionOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PointerIndirectionOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPointerIndirectionOperator(this);
        }

        public BoundPointerIndirectionOperator Update(BoundExpression operand, TypeSymbol type)
        {
            if (operand != Operand || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPointerIndirectionOperator boundPointerIndirectionOperator = new BoundPointerIndirectionOperator(Syntax, operand, type, base.HasErrors);
                boundPointerIndirectionOperator.CopyAttributes(this);
                return boundPointerIndirectionOperator;
            }
            return this;
        }
    }
}
