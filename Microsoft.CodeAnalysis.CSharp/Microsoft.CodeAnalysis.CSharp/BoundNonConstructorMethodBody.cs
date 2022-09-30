using System.Diagnostics;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNonConstructorMethodBody : BoundMethodBodyBase
    {
        public BoundNonConstructorMethodBody(SyntaxNode syntax, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.NonConstructorMethodBody, syntax, blockBody, expressionBody, hasErrors || blockBody.HasErrors() || expressionBody.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNonConstructorMethodBody(this);
        }

        public BoundNonConstructorMethodBody Update(BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (blockBody != base.BlockBody || expressionBody != base.ExpressionBody)
            {
                BoundNonConstructorMethodBody boundNonConstructorMethodBody = new BoundNonConstructorMethodBody(Syntax, blockBody, expressionBody, base.HasErrors);
                boundNonConstructorMethodBody.CopyAttributes(this);
                return boundNonConstructorMethodBody;
            }
            return this;
        }
    }
}
