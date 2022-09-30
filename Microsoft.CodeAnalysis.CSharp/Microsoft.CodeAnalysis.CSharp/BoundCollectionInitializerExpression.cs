using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundCollectionInitializerExpression : BoundObjectInitializerExpressionBase
    {
        public BoundCollectionInitializerExpression(SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CollectionInitializerExpression, syntax, placeholder, initializers, type, hasErrors || placeholder.HasErrors() || initializers.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitCollectionInitializerExpression(this);
        }

        public BoundCollectionInitializerExpression Update(BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
        {
            if (placeholder != base.Placeholder || initializers != base.Initializers || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundCollectionInitializerExpression boundCollectionInitializerExpression = new BoundCollectionInitializerExpression(Syntax, placeholder, initializers, type, base.HasErrors);
                boundCollectionInitializerExpression.CopyAttributes(this);
                return boundCollectionInitializerExpression;
            }
            return this;
        }
    }
}
