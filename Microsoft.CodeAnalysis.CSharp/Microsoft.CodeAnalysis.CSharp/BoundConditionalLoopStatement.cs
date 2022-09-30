using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundConditionalLoopStatement : BoundLoopStatement
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpression Condition { get; }

        public BoundStatement Body { get; }

        protected BoundConditionalLoopStatement(BoundKind kind, SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(kind, syntax, breakLabel, continueLabel, hasErrors)
        {
            Locals = locals;
            Condition = condition;
            Body = body;
        }
    }
}
