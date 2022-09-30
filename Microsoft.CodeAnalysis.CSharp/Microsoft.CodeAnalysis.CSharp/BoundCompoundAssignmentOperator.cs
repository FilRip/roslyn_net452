using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundCompoundAssignmentOperator : BoundExpression
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol? ExpressionSymbol => Operator.Method;

        public new TypeSymbol Type => base.Type;

        public BinaryOperatorSignature Operator { get; }

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        public Conversion LeftConversion { get; }

        public Conversion FinalConversion { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }

        public BoundCompoundAssignmentOperator(SyntaxNode syntax, BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : this(syntax, @operator, left, right, leftConversion, finalConversion, resultKind, default(ImmutableArray<MethodSymbol>), type, hasErrors)
        {
        }

        public BoundCompoundAssignmentOperator Update(BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, TypeSymbol type)
        {
            return Update(@operator, left, right, leftConversion, finalConversion, resultKind, OriginalUserDefinedOperatorsOpt, type);
        }

        public BoundCompoundAssignmentOperator(SyntaxNode syntax, BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CompoundAssignmentOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            Operator = @operator;
            Left = left;
            Right = right;
            LeftConversion = leftConversion;
            FinalConversion = finalConversion;
            _ResultKind = resultKind;
            OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitCompoundAssignmentOperator(this);
        }

        public BoundCompoundAssignmentOperator Update(BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (@operator != Operator || left != Left || right != Right || leftConversion != LeftConversion || finalConversion != FinalConversion || resultKind != ResultKind || originalUserDefinedOperatorsOpt != OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundCompoundAssignmentOperator boundCompoundAssignmentOperator = new BoundCompoundAssignmentOperator(Syntax, @operator, left, right, leftConversion, finalConversion, resultKind, originalUserDefinedOperatorsOpt, type, base.HasErrors);
                boundCompoundAssignmentOperator.CopyAttributes(this);
                return boundCompoundAssignmentOperator;
            }
            return this;
        }
    }
}
