using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnconvertedConditionalOperator : BoundExpression
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public override object Display
        {
            get
            {
                if ((object)base.Type != null)
                {
                    return base.Display;
                }
                return MessageID.IDS_FeatureTargetTypedConditional.Localize();
            }
        }

        public BoundExpression Condition { get; }

        public BoundExpression Consequence { get; }

        public BoundExpression Alternative { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public ErrorCode NoCommonTypeError { get; }

        public BoundUnconvertedConditionalOperator(SyntaxNode syntax, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, ErrorCode noCommonTypeError, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedConditionalOperator, syntax, type, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternative.HasErrors())
        {
            Condition = condition;
            Consequence = consequence;
            Alternative = alternative;
            ConstantValueOpt = constantValueOpt;
            NoCommonTypeError = noCommonTypeError;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnconvertedConditionalOperator(this);
        }

        public BoundUnconvertedConditionalOperator Update(BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, ErrorCode noCommonTypeError, TypeSymbol? type)
        {
            if (condition != Condition || consequence != Consequence || alternative != Alternative || constantValueOpt != ConstantValueOpt || noCommonTypeError != NoCommonTypeError || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundUnconvertedConditionalOperator boundUnconvertedConditionalOperator = new BoundUnconvertedConditionalOperator(Syntax, condition, consequence, alternative, constantValueOpt, noCommonTypeError, type, base.HasErrors);
                boundUnconvertedConditionalOperator.CopyAttributes(this);
                return boundUnconvertedConditionalOperator;
            }
            return this;
        }
    }
}
