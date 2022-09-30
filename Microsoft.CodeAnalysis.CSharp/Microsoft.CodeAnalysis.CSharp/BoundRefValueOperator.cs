using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundRefValueOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Operand);

        public new TypeSymbol Type => base.Type;

        public NullableAnnotation NullableAnnotation { get; }

        public BoundExpression Operand { get; }

        public BoundRefValueOperator(SyntaxNode syntax, NullableAnnotation nullableAnnotation, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RefValueOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            NullableAnnotation = nullableAnnotation;
            Operand = operand;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRefValueOperator(this);
        }

        public BoundRefValueOperator Update(NullableAnnotation nullableAnnotation, BoundExpression operand, TypeSymbol type)
        {
            if (nullableAnnotation != NullableAnnotation || operand != Operand || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundRefValueOperator boundRefValueOperator = new BoundRefValueOperator(Syntax, nullableAnnotation, operand, type, base.HasErrors);
                boundRefValueOperator.CopyAttributes(this);
                return boundRefValueOperator;
            }
            return this;
        }
    }
}
