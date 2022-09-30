using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPassByCopy : BoundExpression
    {
        public override ConstantValue? ConstantValue => null;

        public override Symbol? ExpressionSymbol => Expression.ExpressionSymbol;

        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Expression);

        public override object Display => Expression.Display;

        public BoundExpression Expression { get; }

        public BoundPassByCopy(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.PassByCopy, syntax, type, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPassByCopy(this);
        }

        public BoundPassByCopy Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != Expression || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPassByCopy boundPassByCopy = new BoundPassByCopy(Syntax, expression, type, base.HasErrors);
                boundPassByCopy.CopyAttributes(this);
                return boundPassByCopy;
            }
            return this;
        }
    }
}
