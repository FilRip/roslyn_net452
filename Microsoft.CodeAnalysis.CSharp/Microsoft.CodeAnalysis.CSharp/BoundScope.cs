using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundScope : BoundStatementList
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundScope(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.Scope, syntax, statements, hasErrors || statements.HasErrors())
        {
            Locals = locals;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitScope(this);
        }

        public BoundScope Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements)
        {
            if (locals != Locals || statements != base.Statements)
            {
                BoundScope boundScope = new BoundScope(Syntax, locals, statements, base.HasErrors);
                boundScope.CopyAttributes(this);
                return boundScope;
            }
            return this;
        }
    }
}
