using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTypeOrInstanceInitializers : BoundStatementList
    {
        public BoundTypeOrInstanceInitializers(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.TypeOrInstanceInitializers, syntax, statements, hasErrors || statements.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTypeOrInstanceInitializers(this);
        }

        public new BoundTypeOrInstanceInitializers Update(ImmutableArray<BoundStatement> statements)
        {
            if (statements != base.Statements)
            {
                BoundTypeOrInstanceInitializers boundTypeOrInstanceInitializers = new BoundTypeOrInstanceInitializers(Syntax, statements, base.HasErrors);
                boundTypeOrInstanceInitializers.CopyAttributes(this);
                return boundTypeOrInstanceInitializers;
            }
            return this;
        }
    }
}
