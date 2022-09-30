using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundBinaryOperatorBase : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        protected BoundBinaryOperatorBase(BoundKind kind, SyntaxNode syntax, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            Left = left;
            Right = right;
        }
    }
}
