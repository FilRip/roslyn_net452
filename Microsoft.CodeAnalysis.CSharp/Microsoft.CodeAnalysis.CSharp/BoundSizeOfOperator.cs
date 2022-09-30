using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSizeOfOperator : BoundExpression
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public new TypeSymbol Type => base.Type;

        public BoundTypeExpression SourceType { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public BoundSizeOfOperator(SyntaxNode syntax, BoundTypeExpression sourceType, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.SizeOfOperator, syntax, type, hasErrors || sourceType.HasErrors())
        {
            SourceType = sourceType;
            ConstantValueOpt = constantValueOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSizeOfOperator(this);
        }

        public BoundSizeOfOperator Update(BoundTypeExpression sourceType, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            if (sourceType != SourceType || constantValueOpt != ConstantValueOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundSizeOfOperator boundSizeOfOperator = new BoundSizeOfOperator(Syntax, sourceType, constantValueOpt, type, base.HasErrors);
                boundSizeOfOperator.CopyAttributes(this);
                return boundSizeOfOperator;
            }
            return this;
        }
    }
}
