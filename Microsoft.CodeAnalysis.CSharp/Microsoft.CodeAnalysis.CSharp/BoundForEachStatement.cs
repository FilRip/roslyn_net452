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

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundForEachStatement : BoundLoopStatement
    {
        public ForEachEnumeratorInfo? EnumeratorInfoOpt { get; }

        public Conversion ElementConversion { get; }

        public BoundTypeExpression IterationVariableType { get; }

        public ImmutableArray<LocalSymbol> IterationVariables { get; }

        public BoundExpression? IterationErrorExpressionOpt { get; }

        public BoundExpression Expression { get; }

        public BoundForEachDeconstructStep? DeconstructionOpt { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }

        public BoundStatement Body { get; }

        public bool Checked { get; }

        public BoundForEachStatement(SyntaxNode syntax, ForEachEnumeratorInfo? enumeratorInfoOpt, Conversion elementConversion, BoundTypeExpression iterationVariableType, ImmutableArray<LocalSymbol> iterationVariables, BoundExpression? iterationErrorExpressionOpt, BoundExpression expression, BoundForEachDeconstructStep? deconstructionOpt, BoundAwaitableInfo? awaitOpt, BoundStatement body, bool @checked, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.ForEachStatement, syntax, breakLabel, continueLabel, hasErrors || iterationVariableType.HasErrors() || iterationErrorExpressionOpt.HasErrors() || expression.HasErrors() || deconstructionOpt.HasErrors() || awaitOpt.HasErrors() || body.HasErrors())
        {
            EnumeratorInfoOpt = enumeratorInfoOpt;
            ElementConversion = elementConversion;
            IterationVariableType = iterationVariableType;
            IterationVariables = iterationVariables;
            IterationErrorExpressionOpt = iterationErrorExpressionOpt;
            Expression = expression;
            DeconstructionOpt = deconstructionOpt;
            AwaitOpt = awaitOpt;
            Body = body;
            Checked = @checked;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitForEachStatement(this);
        }

        public BoundForEachStatement Update(ForEachEnumeratorInfo? enumeratorInfoOpt, Conversion elementConversion, BoundTypeExpression iterationVariableType, ImmutableArray<LocalSymbol> iterationVariables, BoundExpression? iterationErrorExpressionOpt, BoundExpression expression, BoundForEachDeconstructStep? deconstructionOpt, BoundAwaitableInfo? awaitOpt, BoundStatement body, bool @checked, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (enumeratorInfoOpt != EnumeratorInfoOpt || elementConversion != ElementConversion || iterationVariableType != IterationVariableType || iterationVariables != IterationVariables || iterationErrorExpressionOpt != IterationErrorExpressionOpt || expression != Expression || deconstructionOpt != DeconstructionOpt || awaitOpt != AwaitOpt || body != Body || @checked != Checked || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, base.BreakLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, base.ContinueLabel))
            {
                BoundForEachStatement boundForEachStatement = new BoundForEachStatement(Syntax, enumeratorInfoOpt, elementConversion, iterationVariableType, iterationVariables, iterationErrorExpressionOpt, expression, deconstructionOpt, awaitOpt, body, @checked, breakLabel, continueLabel, base.HasErrors);
                boundForEachStatement.CopyAttributes(this);
                return boundForEachStatement;
            }
            return this;
        }
    }
}
