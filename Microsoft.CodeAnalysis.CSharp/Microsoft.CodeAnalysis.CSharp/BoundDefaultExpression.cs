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

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDefaultExpression : BoundExpression
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public new TypeSymbol Type => base.Type;

        public BoundTypeExpression? TargetType { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public BoundDefaultExpression(SyntaxNode syntax, TypeSymbol type, bool hasErrors = false)
            : this(syntax, null, type.GetDefaultValue(), type, hasErrors)
        {
        }

        public BoundDefaultExpression(SyntaxNode syntax, BoundTypeExpression? targetType, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DefaultExpression, syntax, type, hasErrors || targetType.HasErrors())
        {
            TargetType = targetType;
            ConstantValueOpt = constantValueOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDefaultExpression(this);
        }

        public BoundDefaultExpression Update(BoundTypeExpression? targetType, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            if (targetType != TargetType || constantValueOpt != ConstantValueOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDefaultExpression boundDefaultExpression = new BoundDefaultExpression(Syntax, targetType, constantValueOpt, type, base.HasErrors);
                boundDefaultExpression.CopyAttributes(this);
                return boundDefaultExpression;
            }
            return this;
        }
    }
}
