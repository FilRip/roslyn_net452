using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSwitchStatement : BoundStatement, IBoundSwitchStatement
    {
        BoundNode IBoundSwitchStatement.Value => Expression;

        ImmutableArray<BoundStatementList> IBoundSwitchStatement.Cases => StaticCast<BoundStatementList>.From(SwitchSections);

        public BoundExpression Expression { get; }

        public ImmutableArray<LocalSymbol> InnerLocals { get; }

        public ImmutableArray<LocalFunctionSymbol> InnerLocalFunctions { get; }

        public ImmutableArray<BoundSwitchSection> SwitchSections { get; }

        public BoundDecisionDag DecisionDag { get; }

        public BoundSwitchLabel? DefaultLabel { get; }

        public GeneratedLabelSymbol BreakLabel { get; }

        public BoundSwitchStatement(SyntaxNode syntax, BoundExpression expression, ImmutableArray<LocalSymbol> innerLocals, ImmutableArray<LocalFunctionSymbol> innerLocalFunctions, ImmutableArray<BoundSwitchSection> switchSections, BoundDecisionDag decisionDag, BoundSwitchLabel? defaultLabel, GeneratedLabelSymbol breakLabel, bool hasErrors = false)
            : base(BoundKind.SwitchStatement, syntax, hasErrors || expression.HasErrors() || switchSections.HasErrors() || decisionDag.HasErrors() || defaultLabel.HasErrors())
        {
            Expression = expression;
            InnerLocals = innerLocals;
            InnerLocalFunctions = innerLocalFunctions;
            SwitchSections = switchSections;
            DecisionDag = decisionDag;
            DefaultLabel = defaultLabel;
            BreakLabel = breakLabel;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSwitchStatement(this);
        }

        public BoundSwitchStatement Update(BoundExpression expression, ImmutableArray<LocalSymbol> innerLocals, ImmutableArray<LocalFunctionSymbol> innerLocalFunctions, ImmutableArray<BoundSwitchSection> switchSections, BoundDecisionDag decisionDag, BoundSwitchLabel? defaultLabel, GeneratedLabelSymbol breakLabel)
        {
            if (expression != Expression || innerLocals != InnerLocals || innerLocalFunctions != InnerLocalFunctions || switchSections != SwitchSections || decisionDag != DecisionDag || defaultLabel != DefaultLabel || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, BreakLabel))
            {
                BoundSwitchStatement boundSwitchStatement = new BoundSwitchStatement(Syntax, expression, innerLocals, innerLocalFunctions, switchSections, decisionDag, defaultLabel, breakLabel, base.HasErrors);
                boundSwitchStatement.CopyAttributes(this);
                return boundSwitchStatement;
            }
            return this;
        }
    }
}
