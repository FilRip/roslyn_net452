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

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIncrementOperator : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol? ExpressionSymbol => MethodOpt;

        public new TypeSymbol Type => base.Type;

        public UnaryOperatorKind OperatorKind { get; }

        public BoundExpression Operand { get; }

        public MethodSymbol? MethodOpt { get; }

        public Conversion OperandConversion { get; }

        public Conversion ResultConversion { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }

        public BoundIncrementOperator(CSharpSyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : this(syntax, operatorKind, operand, methodOpt, operandConversion, resultConversion, resultKind, default(ImmutableArray<MethodSymbol>), type, hasErrors)
        {
        }

        public BoundIncrementOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, TypeSymbol type)
        {
            return Update(operatorKind, operand, methodOpt, operandConversion, resultConversion, resultKind, OriginalUserDefinedOperatorsOpt, type);
        }

        public BoundIncrementOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IncrementOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            OperatorKind = operatorKind;
            Operand = operand;
            MethodOpt = methodOpt;
            OperandConversion = operandConversion;
            ResultConversion = resultConversion;
            _ResultKind = resultKind;
            OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIncrementOperator(this);
        }

        public BoundIncrementOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (operatorKind != OperatorKind || operand != Operand || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || operandConversion != OperandConversion || resultConversion != ResultConversion || resultKind != ResultKind || originalUserDefinedOperatorsOpt != OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundIncrementOperator boundIncrementOperator = new BoundIncrementOperator(Syntax, operatorKind, operand, methodOpt, operandConversion, resultConversion, resultKind, originalUserDefinedOperatorsOpt, type, base.HasErrors);
                boundIncrementOperator.CopyAttributes(this);
                return boundIncrementOperator;
            }
            return this;
        }
    }
}
