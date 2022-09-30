using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnaryOperator : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public override Symbol? ExpressionSymbol => MethodOpt;

        public new TypeSymbol Type => base.Type;

        public UnaryOperatorKind OperatorKind { get; }

        public BoundExpression Operand { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public MethodSymbol? MethodOpt { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }

        public BoundUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operatorKind, operand, constantValueOpt, methodOpt, resultKind, default(ImmutableArray<MethodSymbol>), type, hasErrors)
        {
        }

        public BoundUnaryOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, TypeSymbol type)
        {
            return Update(operatorKind, operand, constantValueOpt, methodOpt, resultKind, OriginalUserDefinedOperatorsOpt, type);
        }

        public BoundUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.UnaryOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            OperatorKind = operatorKind;
            Operand = operand;
            ConstantValueOpt = constantValueOpt;
            MethodOpt = methodOpt;
            _ResultKind = resultKind;
            OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnaryOperator(this);
        }

        public BoundUnaryOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (operatorKind != OperatorKind || operand != Operand || constantValueOpt != ConstantValueOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || resultKind != ResultKind || originalUserDefinedOperatorsOpt != OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundUnaryOperator boundUnaryOperator = new BoundUnaryOperator(Syntax, operatorKind, operand, constantValueOpt, methodOpt, resultKind, originalUserDefinedOperatorsOpt, type, base.HasErrors);
                boundUnaryOperator.CopyAttributes(this);
                return boundUnaryOperator;
            }
            return this;
        }
    }
}
