using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConstantPattern : BoundPattern
    {
        public BoundExpression Value { get; }

        public ConstantValue ConstantValue { get; }

        public BoundConstantPattern(SyntaxNode syntax, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.ConstantPattern, syntax, inputType, narrowedType, hasErrors || value.HasErrors())
        {
            Value = value;
            ConstantValue = constantValue;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConstantPattern(this);
        }

        public BoundConstantPattern Update(BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (value != Value || constantValue != ConstantValue || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundConstantPattern boundConstantPattern = new BoundConstantPattern(Syntax, value, constantValue, inputType, narrowedType, base.HasErrors);
                boundConstantPattern.CopyAttributes(this);
                return boundConstantPattern;
            }
            return this;
        }
    }
}
