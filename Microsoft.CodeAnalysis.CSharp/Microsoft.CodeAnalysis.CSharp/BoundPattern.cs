using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundPattern : BoundNode
    {
        public TypeSymbol InputType { get; }

        public TypeSymbol NarrowedType { get; }

        internal bool IsNegated(out BoundPattern innerPattern)
        {
            innerPattern = this;
            bool flag = false;
            while (innerPattern is BoundNegatedPattern boundNegatedPattern)
            {
                flag = !flag;
                innerPattern = boundNegatedPattern.Negated;
            }
            return flag;
        }

        protected BoundPattern(BoundKind kind, SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
            InputType = inputType;
            NarrowedType = narrowedType;
        }

        protected BoundPattern(BoundKind kind, SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType)
            : base(kind, syntax)
        {
            InputType = inputType;
            NarrowedType = narrowedType;
        }
    }
}
