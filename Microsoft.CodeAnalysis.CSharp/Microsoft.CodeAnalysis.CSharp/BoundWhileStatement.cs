using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundWhileStatement : BoundConditionalLoopStatement
    {
        public BoundWhileStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.WhileStatement, syntax, locals, condition, body, breakLabel, continueLabel, hasErrors || condition.HasErrors() || body.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitWhileStatement(this);
        }

        public BoundWhileStatement Update(ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (locals != base.Locals || condition != base.Condition || body != base.Body || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, base.BreakLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, base.ContinueLabel))
            {
                BoundWhileStatement boundWhileStatement = new BoundWhileStatement(Syntax, locals, condition, body, breakLabel, continueLabel, base.HasErrors);
                boundWhileStatement.CopyAttributes(this);
                return boundWhileStatement;
            }
            return this;
        }
    }
}
