using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public class BoundStatementList : BoundStatement
    {
        protected override ImmutableArray<BoundNode?> Children
        {
            get
            {
                if (base.Kind != BoundKind.StatementList && base.Kind != BoundKind.Scope)
                {
                    return ImmutableArray<BoundNode>.Empty;
                }
                return StaticCast<BoundNode>.From(Statements);
            }
        }

        public ImmutableArray<BoundStatement> Statements { get; }

        public static BoundStatementList Synthesized(SyntaxNode syntax, params BoundStatement[] statements)
        {
            return Synthesized(syntax, hasErrors: false, statements.AsImmutableOrNull());
        }

        public static BoundStatementList Synthesized(SyntaxNode syntax, bool hasErrors, params BoundStatement[] statements)
        {
            return Synthesized(syntax, hasErrors, statements.AsImmutableOrNull());
        }

        public static BoundStatementList Synthesized(SyntaxNode syntax, ImmutableArray<BoundStatement> statements)
        {
            return Synthesized(syntax, hasErrors: false, statements);
        }

        public static BoundStatementList Synthesized(SyntaxNode syntax, bool hasErrors, ImmutableArray<BoundStatement> statements)
        {
            return new BoundStatementList(syntax, statements, hasErrors)
            {
                WasCompilerGenerated = true
            };
        }

        protected BoundStatementList(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            Statements = statements;
        }

        public BoundStatementList(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.StatementList, syntax, hasErrors || statements.HasErrors())
        {
            Statements = statements;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitStatementList(this);
        }

        public BoundStatementList Update(ImmutableArray<BoundStatement> statements)
        {
            if (statements != Statements)
            {
                BoundStatementList boundStatementList = new BoundStatementList(Syntax, statements, base.HasErrors);
                boundStatementList.CopyAttributes(this);
                return boundStatementList;
            }
            return this;
        }
    }
}
