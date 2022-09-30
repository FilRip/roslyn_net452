using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundArrayLength : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Expression { get; }

        public BoundArrayLength(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ArrayLength, syntax, type, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitArrayLength(this);
        }

        public BoundArrayLength Update(BoundExpression expression, TypeSymbol type)
        {
            if (expression != Expression || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundArrayLength boundArrayLength = new BoundArrayLength(Syntax, expression, type, base.HasErrors);
                boundArrayLength.CopyAttributes(this);
                return boundArrayLength;
            }
            return this;
        }
    }
}
