using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundBinaryPattern : BoundPattern
    {
        public bool Disjunction { get; }

        public BoundPattern Left { get; }

        public BoundPattern Right { get; }

        public BoundBinaryPattern(SyntaxNode syntax, bool disjunction, BoundPattern left, BoundPattern right, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.BinaryPattern, syntax, inputType, narrowedType, hasErrors || left.HasErrors() || right.HasErrors())
        {
            Disjunction = disjunction;
            Left = left;
            Right = right;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitBinaryPattern(this);
        }

        public BoundBinaryPattern Update(bool disjunction, BoundPattern left, BoundPattern right, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (disjunction != Disjunction || left != Left || right != Right || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundBinaryPattern boundBinaryPattern = new BoundBinaryPattern(Syntax, disjunction, left, right, inputType, narrowedType, base.HasErrors);
                boundBinaryPattern.CopyAttributes(this);
                return boundBinaryPattern;
            }
            return this;
        }
    }
}
