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

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundBinaryOperator : BoundBinaryOperatorBase
    {
        private readonly LookupResultKind _ResultKind;

        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public override Symbol? ExpressionSymbol => MethodOpt;

        public BinaryOperatorKind OperatorKind { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public MethodSymbol? MethodOpt { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }

        public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operatorKind, constantValueOpt, methodOpt, resultKind, originalUserDefinedOperatorsOpt, left, right, type, hasErrors)
        {
        }

        public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operatorKind, constantValueOpt, methodOpt, resultKind, default(ImmutableArray<MethodSymbol>), left, right, type, hasErrors)
        {
        }

        public BoundBinaryOperator Update(BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            return Update(operatorKind, constantValueOpt, methodOpt, resultKind, OriginalUserDefinedOperatorsOpt, left, right, type);
        }

        public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.BinaryOperator, syntax, left, right, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            OperatorKind = operatorKind;
            ConstantValueOpt = constantValueOpt;
            MethodOpt = methodOpt;
            _ResultKind = resultKind;
            OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitBinaryOperator(this);
        }

        public BoundBinaryOperator Update(BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            if (operatorKind != OperatorKind || constantValueOpt != ConstantValueOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || resultKind != ResultKind || originalUserDefinedOperatorsOpt != OriginalUserDefinedOperatorsOpt || left != base.Left || right != base.Right || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundBinaryOperator boundBinaryOperator = new BoundBinaryOperator(Syntax, operatorKind, constantValueOpt, methodOpt, resultKind, originalUserDefinedOperatorsOpt, left, right, type, base.HasErrors);
                boundBinaryOperator.CopyAttributes(this);
                return boundBinaryOperator;
            }
            return this;
        }
    }
}
