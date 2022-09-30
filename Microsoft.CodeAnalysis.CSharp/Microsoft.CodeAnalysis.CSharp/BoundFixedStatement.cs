using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFixedStatement : BoundStatement
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create(Declarations, (BoundNode)Body);

        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundMultipleLocalDeclarations Declarations { get; }

        public BoundStatement Body { get; }

        public BoundFixedStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations declarations, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.FixedStatement, syntax, hasErrors || declarations.HasErrors() || body.HasErrors())
        {
            Locals = locals;
            Declarations = declarations;
            Body = body;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFixedStatement(this);
        }

        public BoundFixedStatement Update(ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations declarations, BoundStatement body)
        {
            if (locals != Locals || declarations != Declarations || body != Body)
            {
                BoundFixedStatement boundFixedStatement = new BoundFixedStatement(Syntax, locals, declarations, body, base.HasErrors);
                boundFixedStatement.CopyAttributes(this);
                return boundFixedStatement;
            }
            return this;
        }
    }
}
