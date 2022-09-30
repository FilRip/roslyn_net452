using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPointerElementAccess : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create(Expression, (BoundNode)Index);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Expression { get; }

        public BoundExpression Index { get; }

        public bool Checked { get; }

        public BoundPointerElementAccess(SyntaxNode syntax, BoundExpression expression, BoundExpression index, bool @checked, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PointerElementAccess, syntax, type, hasErrors || expression.HasErrors() || index.HasErrors())
        {
            Expression = expression;
            Index = index;
            Checked = @checked;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPointerElementAccess(this);
        }

        public BoundPointerElementAccess Update(BoundExpression expression, BoundExpression index, bool @checked, TypeSymbol type)
        {
            if (expression != Expression || index != Index || @checked != Checked || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPointerElementAccess boundPointerElementAccess = new BoundPointerElementAccess(Syntax, expression, index, @checked, type, base.HasErrors);
                boundPointerElementAccess.CopyAttributes(this);
                return boundPointerElementAccess;
            }
            return this;
        }
    }
}
