using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMakeRefOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Operand);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public BoundMakeRefOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.MakeRefOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMakeRefOperator(this);
        }

        public BoundMakeRefOperator Update(BoundExpression operand, TypeSymbol type)
        {
            if (operand != Operand || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundMakeRefOperator boundMakeRefOperator = new BoundMakeRefOperator(Syntax, operand, type, base.HasErrors);
                boundMakeRefOperator.CopyAttributes(this);
                return boundMakeRefOperator;
            }
            return this;
        }
    }
}
