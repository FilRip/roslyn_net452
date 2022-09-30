using System.Collections.Immutable;
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

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTryStatement : BoundStatement
    {
        public BoundBlock TryBlock { get; }

        public ImmutableArray<BoundCatchBlock> CatchBlocks { get; }

        public BoundBlock? FinallyBlockOpt { get; }

        public LabelSymbol? FinallyLabelOpt { get; }

        public bool PreferFaultHandler { get; }

        public BoundTryStatement(SyntaxNode syntax, BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlockOpt, LabelSymbol? finallyLabelOpt = null)
            : this(syntax, tryBlock, catchBlocks, finallyBlockOpt, finallyLabelOpt, preferFaultHandler: false)
        {
        }

        public BoundTryStatement(SyntaxNode syntax, BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlockOpt, LabelSymbol? finallyLabelOpt, bool preferFaultHandler, bool hasErrors = false)
            : base(BoundKind.TryStatement, syntax, hasErrors || tryBlock.HasErrors() || catchBlocks.HasErrors() || finallyBlockOpt.HasErrors())
        {
            TryBlock = tryBlock;
            CatchBlocks = catchBlocks;
            FinallyBlockOpt = finallyBlockOpt;
            FinallyLabelOpt = finallyLabelOpt;
            PreferFaultHandler = preferFaultHandler;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTryStatement(this);
        }

        public BoundTryStatement Update(BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlockOpt, LabelSymbol? finallyLabelOpt, bool preferFaultHandler)
        {
            if (tryBlock != TryBlock || catchBlocks != CatchBlocks || finallyBlockOpt != FinallyBlockOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(finallyLabelOpt, FinallyLabelOpt) || preferFaultHandler != PreferFaultHandler)
            {
                BoundTryStatement boundTryStatement = new BoundTryStatement(Syntax, tryBlock, catchBlocks, finallyBlockOpt, finallyLabelOpt, preferFaultHandler, base.HasErrors);
                boundTryStatement.CopyAttributes(this);
                return boundTryStatement;
            }
            return this;
        }
    }
}
