using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTryStatement : BoundStatement
	{
		private readonly BoundBlock _TryBlock;

		private readonly ImmutableArray<BoundCatchBlock> _CatchBlocks;

		private readonly BoundBlock _FinallyBlockOpt;

		private readonly LabelSymbol _ExitLabelOpt;

		public BoundBlock TryBlock => _TryBlock;

		public ImmutableArray<BoundCatchBlock> CatchBlocks => _CatchBlocks;

		public BoundBlock FinallyBlockOpt => _FinallyBlockOpt;

		public LabelSymbol ExitLabelOpt => _ExitLabelOpt;

		public BoundTryStatement(SyntaxNode syntax, BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock finallyBlockOpt, LabelSymbol exitLabelOpt, bool hasErrors = false)
			: base(BoundKind.TryStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(tryBlock) || BoundNodeExtensions.NonNullAndHasErrors(catchBlocks) || BoundNodeExtensions.NonNullAndHasErrors(finallyBlockOpt))
		{
			_TryBlock = tryBlock;
			_CatchBlocks = catchBlocks;
			_FinallyBlockOpt = finallyBlockOpt;
			_ExitLabelOpt = exitLabelOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTryStatement(this);
		}

		public BoundTryStatement Update(BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock finallyBlockOpt, LabelSymbol exitLabelOpt)
		{
			if (tryBlock != TryBlock || catchBlocks != CatchBlocks || finallyBlockOpt != FinallyBlockOpt || (object)exitLabelOpt != ExitLabelOpt)
			{
				BoundTryStatement boundTryStatement = new BoundTryStatement(base.Syntax, tryBlock, catchBlocks, finallyBlockOpt, exitLabelOpt, base.HasErrors);
				boundTryStatement.CopyAttributes(this);
				return boundTryStatement;
			}
			return this;
		}
	}
}
