using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUserDefinedConditionalLogicalOperator : BoundBinaryOperatorBase
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol ExpressionSymbol => LogicalOperator;

        public BinaryOperatorKind OperatorKind { get; }

        public MethodSymbol LogicalOperator { get; }

        public MethodSymbol TrueOperator { get; }

        public MethodSymbol FalseOperator { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }

        public BoundUserDefinedConditionalLogicalOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operatorKind, logicalOperator, trueOperator, falseOperator, resultKind, originalUserDefinedOperatorsOpt, left, right, type, hasErrors)
        {
        }

        public BoundUserDefinedConditionalLogicalOperator Update(BinaryOperatorKind operatorKind, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            return Update(operatorKind, logicalOperator, trueOperator, falseOperator, resultKind, OriginalUserDefinedOperatorsOpt, left, right, type);
        }

        public BoundUserDefinedConditionalLogicalOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.UserDefinedConditionalLogicalOperator, syntax, left, right, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            OperatorKind = operatorKind;
            LogicalOperator = logicalOperator;
            TrueOperator = trueOperator;
            FalseOperator = falseOperator;
            _ResultKind = resultKind;
            OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUserDefinedConditionalLogicalOperator(this);
        }

        public BoundUserDefinedConditionalLogicalOperator Update(BinaryOperatorKind operatorKind, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            if (operatorKind != OperatorKind || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(logicalOperator, LogicalOperator) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(trueOperator, TrueOperator) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(falseOperator, FalseOperator) || resultKind != ResultKind || originalUserDefinedOperatorsOpt != OriginalUserDefinedOperatorsOpt || left != base.Left || right != base.Right || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundUserDefinedConditionalLogicalOperator boundUserDefinedConditionalLogicalOperator = new BoundUserDefinedConditionalLogicalOperator(Syntax, operatorKind, logicalOperator, trueOperator, falseOperator, resultKind, originalUserDefinedOperatorsOpt, left, right, type, base.HasErrors);
                boundUserDefinedConditionalLogicalOperator.CopyAttributes(this);
                return boundUserDefinedConditionalLogicalOperator;
            }
            return this;
        }
    }
}
