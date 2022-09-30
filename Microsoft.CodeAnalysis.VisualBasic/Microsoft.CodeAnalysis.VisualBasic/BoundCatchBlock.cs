using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCatchBlock : BoundNode
	{
		private readonly LocalSymbol _LocalOpt;

		private readonly BoundExpression _ExceptionSourceOpt;

		private readonly BoundExpression _ErrorLineNumberOpt;

		private readonly BoundExpression _ExceptionFilterOpt;

		private readonly BoundBlock _Body;

		private readonly bool _IsSynthesizedAsyncCatchAll;

		public LocalSymbol LocalOpt => _LocalOpt;

		public BoundExpression ExceptionSourceOpt => _ExceptionSourceOpt;

		public BoundExpression ErrorLineNumberOpt => _ErrorLineNumberOpt;

		public BoundExpression ExceptionFilterOpt => _ExceptionFilterOpt;

		public BoundBlock Body => _Body;

		public bool IsSynthesizedAsyncCatchAll => _IsSynthesizedAsyncCatchAll;

		public BoundCatchBlock(SyntaxNode syntax, LocalSymbol localOpt, BoundExpression exceptionSourceOpt, BoundExpression errorLineNumberOpt, BoundExpression exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll, bool hasErrors = false)
			: base(BoundKind.CatchBlock, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(exceptionSourceOpt) || BoundNodeExtensions.NonNullAndHasErrors(errorLineNumberOpt) || BoundNodeExtensions.NonNullAndHasErrors(exceptionFilterOpt) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_LocalOpt = localOpt;
			_ExceptionSourceOpt = exceptionSourceOpt;
			_ErrorLineNumberOpt = errorLineNumberOpt;
			_ExceptionFilterOpt = exceptionFilterOpt;
			_Body = body;
			_IsSynthesizedAsyncCatchAll = isSynthesizedAsyncCatchAll;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCatchBlock(this);
		}

		public BoundCatchBlock Update(LocalSymbol localOpt, BoundExpression exceptionSourceOpt, BoundExpression errorLineNumberOpt, BoundExpression exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll)
		{
			if ((object)localOpt != LocalOpt || exceptionSourceOpt != ExceptionSourceOpt || errorLineNumberOpt != ErrorLineNumberOpt || exceptionFilterOpt != ExceptionFilterOpt || body != Body || isSynthesizedAsyncCatchAll != IsSynthesizedAsyncCatchAll)
			{
				BoundCatchBlock boundCatchBlock = new BoundCatchBlock(base.Syntax, localOpt, exceptionSourceOpt, errorLineNumberOpt, exceptionFilterOpt, body, isSynthesizedAsyncCatchAll, base.HasErrors);
				boundCatchBlock.CopyAttributes(this);
				return boundCatchBlock;
			}
			return this;
		}
	}
}
