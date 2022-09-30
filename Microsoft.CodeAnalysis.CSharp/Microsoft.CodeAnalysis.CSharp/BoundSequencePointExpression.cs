using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSequencePointExpression : BoundExpression
    {
        public BoundExpression Expression { get; }

        public BoundSequencePointExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.SequencePointExpression, syntax, type, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSequencePointExpression(this);
        }

        public BoundSequencePointExpression Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != Expression || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundSequencePointExpression boundSequencePointExpression = new BoundSequencePointExpression(Syntax, expression, type, base.HasErrors);
                boundSequencePointExpression.CopyAttributes(this);
                return boundSequencePointExpression;
            }
            return this;
        }
    }
}
