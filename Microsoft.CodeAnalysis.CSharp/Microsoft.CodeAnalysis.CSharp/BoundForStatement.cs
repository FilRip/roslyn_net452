using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundForStatement : BoundLoopStatement
    {
        public ImmutableArray<LocalSymbol> OuterLocals { get; }

        public BoundStatement? Initializer { get; }

        public ImmutableArray<LocalSymbol> InnerLocals { get; }

        public BoundExpression? Condition { get; }

        public BoundStatement? Increment { get; }

        public BoundStatement Body { get; }

        public BoundForStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> outerLocals, BoundStatement? initializer, ImmutableArray<LocalSymbol> innerLocals, BoundExpression? condition, BoundStatement? increment, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.ForStatement, syntax, breakLabel, continueLabel, hasErrors || initializer.HasErrors() || condition.HasErrors() || increment.HasErrors() || body.HasErrors())
        {
            OuterLocals = outerLocals;
            Initializer = initializer;
            InnerLocals = innerLocals;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitForStatement(this);
        }

        public BoundForStatement Update(ImmutableArray<LocalSymbol> outerLocals, BoundStatement? initializer, ImmutableArray<LocalSymbol> innerLocals, BoundExpression? condition, BoundStatement? increment, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (outerLocals != OuterLocals || initializer != Initializer || innerLocals != InnerLocals || condition != Condition || increment != Increment || body != Body || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, base.BreakLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, base.ContinueLabel))
            {
                BoundForStatement boundForStatement = new BoundForStatement(Syntax, outerLocals, initializer, innerLocals, condition, increment, body, breakLabel, continueLabel, base.HasErrors);
                boundForStatement.CopyAttributes(this);
                return boundForStatement;
            }
            return this;
        }
    }
}
