using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundRelationalPattern : BoundPattern
    {
        public BinaryOperatorKind Relation { get; }

        public BoundExpression Value { get; }

        public ConstantValue ConstantValue { get; }

        public BoundRelationalPattern(SyntaxNode syntax, BinaryOperatorKind relation, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.RelationalPattern, syntax, inputType, narrowedType, hasErrors || value.HasErrors())
        {
            Relation = relation;
            Value = value;
            ConstantValue = constantValue;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRelationalPattern(this);
        }

        public BoundRelationalPattern Update(BinaryOperatorKind relation, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (relation != Relation || value != Value || constantValue != ConstantValue || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundRelationalPattern boundRelationalPattern = new BoundRelationalPattern(Syntax, relation, value, constantValue, inputType, narrowedType, base.HasErrors);
                boundRelationalPattern.CopyAttributes(this);
                return boundRelationalPattern;
            }
            return this;
        }
    }
}
