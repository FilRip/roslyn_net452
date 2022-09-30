using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundExpressionWithNullability : BoundExpression
    {
        public BoundExpression Expression { get; }

        public new TypeSymbol? Type => base.Type;

        public NullableAnnotation NullableAnnotation { get; }

        public BoundExpressionWithNullability(SyntaxNode syntax, BoundExpression expression, NullableAnnotation nullableAnnotation, TypeSymbol? type)
            : this(syntax, expression, nullableAnnotation, type, hasErrors: false)
        {
            base.IsSuppressed = expression.IsSuppressed;
        }

        public BoundExpressionWithNullability(SyntaxNode syntax, BoundExpression expression, NullableAnnotation nullableAnnotation, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ExpressionWithNullability, syntax, type, hasErrors || expression.HasErrors())
        {
            Expression = expression;
            NullableAnnotation = nullableAnnotation;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitExpressionWithNullability(this);
        }

        public BoundExpressionWithNullability Update(BoundExpression expression, NullableAnnotation nullableAnnotation, TypeSymbol? type)
        {
            if (expression != Expression || nullableAnnotation != NullableAnnotation || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundExpressionWithNullability boundExpressionWithNullability = new BoundExpressionWithNullability(Syntax, expression, nullableAnnotation, type, base.HasErrors);
                boundExpressionWithNullability.CopyAttributes(this);
                return boundExpressionWithNullability;
            }
            return this;
        }
    }
}
