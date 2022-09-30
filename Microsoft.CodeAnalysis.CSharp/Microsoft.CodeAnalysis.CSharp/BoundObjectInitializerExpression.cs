using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundObjectInitializerExpression : BoundObjectInitializerExpressionBase
    {
        public BoundObjectInitializerExpression(SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ObjectInitializerExpression, syntax, placeholder, initializers, type, hasErrors || placeholder.HasErrors() || initializers.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitObjectInitializerExpression(this);
        }

        public BoundObjectInitializerExpression Update(BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
        {
            if (placeholder != base.Placeholder || initializers != base.Initializers || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundObjectInitializerExpression boundObjectInitializerExpression = new BoundObjectInitializerExpression(Syntax, placeholder, initializers, type, base.HasErrors);
                boundObjectInitializerExpression.CopyAttributes(this);
                return boundObjectInitializerExpression;
            }
            return this;
        }
    }
}
