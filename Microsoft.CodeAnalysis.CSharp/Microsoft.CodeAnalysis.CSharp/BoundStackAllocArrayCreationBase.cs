using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundStackAllocArrayCreationBase : BoundExpression
    {
        public TypeSymbol ElementType { get; }

        public BoundExpression Count { get; }

        public BoundArrayInitialization? InitializerOpt { get; }

        internal static ImmutableArray<BoundExpression> GetChildInitializers(BoundArrayInitialization? arrayInitializer)
        {
            return arrayInitializer?.Initializers ?? ImmutableArray<BoundExpression>.Empty;
        }

        protected BoundStackAllocArrayCreationBase(BoundKind kind, SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            ElementType = elementType;
            Count = count;
            InitializerOpt = initializerOpt;
        }
    }
}
