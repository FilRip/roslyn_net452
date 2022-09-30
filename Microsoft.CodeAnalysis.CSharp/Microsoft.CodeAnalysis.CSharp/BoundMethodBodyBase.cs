#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundMethodBodyBase : BoundNode
    {
        public BoundBlock? BlockBody { get; }

        public BoundBlock? ExpressionBody { get; }

        protected BoundMethodBodyBase(BoundKind kind, SyntaxNode syntax, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            BlockBody = blockBody;
            ExpressionBody = expressionBody;
        }
    }
}
