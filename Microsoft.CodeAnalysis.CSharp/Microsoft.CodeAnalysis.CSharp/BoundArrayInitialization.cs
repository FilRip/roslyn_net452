using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundArrayInitialization : BoundExpression
    {
        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Initializers { get; }

        public BoundArrayInitialization(SyntaxNode syntax, ImmutableArray<BoundExpression> initializers, bool hasErrors = false)
            : base(BoundKind.ArrayInitialization, syntax, null, hasErrors || initializers.HasErrors())
        {
            Initializers = initializers;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitArrayInitialization(this);
        }

        public BoundArrayInitialization Update(ImmutableArray<BoundExpression> initializers)
        {
            if (initializers != Initializers)
            {
                BoundArrayInitialization boundArrayInitialization = new BoundArrayInitialization(Syntax, initializers, base.HasErrors);
                boundArrayInitialization.CopyAttributes(this);
                return boundArrayInitialization;
            }
            return this;
        }
    }
}
