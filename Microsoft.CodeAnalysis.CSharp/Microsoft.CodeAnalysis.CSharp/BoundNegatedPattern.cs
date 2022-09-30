using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNegatedPattern : BoundPattern
    {
        public BoundPattern Negated { get; }

        public BoundNegatedPattern(SyntaxNode syntax, BoundPattern negated, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.NegatedPattern, syntax, inputType, narrowedType, hasErrors || negated.HasErrors())
        {
            Negated = negated;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNegatedPattern(this);
        }

        public BoundNegatedPattern Update(BoundPattern negated, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (negated != Negated || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundNegatedPattern boundNegatedPattern = new BoundNegatedPattern(Syntax, negated, inputType, narrowedType, base.HasErrors);
                boundNegatedPattern.CopyAttributes(this);
                return boundNegatedPattern;
            }
            return this;
        }
    }
}
