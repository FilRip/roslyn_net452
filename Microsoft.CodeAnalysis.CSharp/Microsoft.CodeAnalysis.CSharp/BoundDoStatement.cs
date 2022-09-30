using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDoStatement : BoundConditionalLoopStatement
    {
        public BoundDoStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.DoStatement, syntax, locals, condition, body, breakLabel, continueLabel, hasErrors || condition.HasErrors() || body.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDoStatement(this);
        }

        public BoundDoStatement Update(ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (locals != base.Locals || condition != base.Condition || body != base.Body || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, base.BreakLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, base.ContinueLabel))
            {
                BoundDoStatement boundDoStatement = new BoundDoStatement(Syntax, locals, condition, body, breakLabel, continueLabel, base.HasErrors);
                boundDoStatement.CopyAttributes(this);
                return boundDoStatement;
            }
            return this;
        }
    }
}
