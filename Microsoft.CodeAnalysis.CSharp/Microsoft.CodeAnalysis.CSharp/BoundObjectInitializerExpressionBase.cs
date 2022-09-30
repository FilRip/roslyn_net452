using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundObjectInitializerExpressionBase : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundObjectOrCollectionValuePlaceholder Placeholder { get; }

        public ImmutableArray<BoundExpression> Initializers { get; }

        protected BoundObjectInitializerExpressionBase(BoundKind kind, SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            Placeholder = placeholder;
            Initializers = initializers;
        }
    }
}
