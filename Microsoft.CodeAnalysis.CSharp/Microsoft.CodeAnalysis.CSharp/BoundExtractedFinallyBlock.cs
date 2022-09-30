using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundExtractedFinallyBlock : BoundStatement
    {
        public BoundBlock FinallyBlock { get; }

        public BoundExtractedFinallyBlock(SyntaxNode syntax, BoundBlock finallyBlock, bool hasErrors = false)
            : base(BoundKind.ExtractedFinallyBlock, syntax, hasErrors || finallyBlock.HasErrors())
        {
            FinallyBlock = finallyBlock;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitExtractedFinallyBlock(this);
        }

        public BoundExtractedFinallyBlock Update(BoundBlock finallyBlock)
        {
            if (finallyBlock != FinallyBlock)
            {
                BoundExtractedFinallyBlock boundExtractedFinallyBlock = new BoundExtractedFinallyBlock(Syntax, finallyBlock, base.HasErrors);
                boundExtractedFinallyBlock.CopyAttributes(this);
                return boundExtractedFinallyBlock;
            }
            return this;
        }
    }
}
