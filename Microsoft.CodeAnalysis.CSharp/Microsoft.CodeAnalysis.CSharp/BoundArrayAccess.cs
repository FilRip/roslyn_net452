using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundArrayAccess : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Expression { get; }

        public ImmutableArray<BoundExpression> Indices { get; }

        public BoundArrayAccess(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundExpression> indices, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ArrayAccess, syntax, type, hasErrors || expression.HasErrors() || indices.HasErrors())
        {
            Expression = expression;
            Indices = indices;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitArrayAccess(this);
        }

        public BoundArrayAccess Update(BoundExpression expression, ImmutableArray<BoundExpression> indices, TypeSymbol type)
        {
            if (expression != Expression || indices != Indices || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundArrayAccess boundArrayAccess = new BoundArrayAccess(Syntax, expression, indices, type, base.HasErrors);
                boundArrayAccess.CopyAttributes(this);
                return boundArrayAccess;
            }
            return this;
        }
    }
}
