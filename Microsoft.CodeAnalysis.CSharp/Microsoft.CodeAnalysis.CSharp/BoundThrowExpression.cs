using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundThrowExpression : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Expression);

        public override object Display => MessageID.IDS_ThrowExpression.Localize();

        public BoundExpression Expression { get; }

        public BoundThrowExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ThrowExpression, syntax, type, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitThrowExpression(this);
        }

        public BoundThrowExpression Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != Expression || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundThrowExpression boundThrowExpression = new BoundThrowExpression(Syntax, expression, type, base.HasErrors);
                boundThrowExpression.CopyAttributes(this);
                return boundThrowExpression;
            }
            return this;
        }
    }
}
