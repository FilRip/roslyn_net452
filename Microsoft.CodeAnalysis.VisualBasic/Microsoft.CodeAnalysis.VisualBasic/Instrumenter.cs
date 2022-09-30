using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class Instrumenter
	{
		public static readonly Instrumenter NoOp = new Instrumenter();

		private static BoundStatement InstrumentStatement(BoundStatement original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentExpressionStatement(BoundExpressionStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentStopStatement(BoundStopStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentEndStatement(BoundEndStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentContinueStatement(BoundContinueStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentExitStatement(BoundExitStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentGotoStatement(BoundGotoStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentLabelStatement(BoundLabelStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentRaiseEventStatement(BoundRaiseEventStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentReturnStatement(BoundReturnStatement original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentThrowStatement(BoundThrowStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentOnErrorStatement(BoundOnErrorStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentResumeStatement(BoundResumeStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentAddHandlerStatement(BoundAddHandlerStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement InstrumentRemoveHandlerStatement(BoundRemoveHandlerStatement original, BoundStatement rewritten)
		{
			return InstrumentStatement(original, rewritten);
		}

		public virtual BoundStatement CreateBlockPrologue(BoundBlock trueOriginal, BoundBlock original, ref LocalSymbol synthesizedLocal)
		{
			synthesizedLocal = null;
			return null;
		}

		public virtual BoundExpression InstrumentTopLevelExpressionInQuery(BoundExpression original, BoundExpression rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentQueryLambdaBody(BoundQueryLambda original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentDoLoopEpilogue(BoundDoLoopStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement CreateSyncLockStatementPrologue(BoundSyncLockStatement original)
		{
			return null;
		}

		public virtual BoundStatement InstrumentSyncLockObjectCapture(BoundSyncLockStatement original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement CreateSyncLockExitDueToExceptionEpilogue(BoundSyncLockStatement original)
		{
			return null;
		}

		public virtual BoundStatement CreateSyncLockExitNormallyEpilogue(BoundSyncLockStatement original)
		{
			return null;
		}

		public virtual BoundStatement InstrumentWhileEpilogue(BoundWhileStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement InstrumentWhileStatementConditionalGotoStart(BoundWhileStatement original, BoundStatement ifConditionGotoStart)
		{
			return ifConditionGotoStart;
		}

		public virtual BoundStatement InstrumentDoLoopStatementEntryOrConditionalGotoStart(BoundDoLoopStatement original, BoundStatement ifConditionGotoStartOpt)
		{
			return ifConditionGotoStartOpt;
		}

		public virtual BoundStatement InstrumentForEachStatementConditionalGotoStart(BoundForEachStatement original, BoundStatement ifConditionGotoStart)
		{
			return ifConditionGotoStart;
		}

		public virtual BoundStatement InstrumentIfStatementConditionalGoto(BoundIfStatement original, BoundStatement condGoto)
		{
			return condGoto;
		}

		public virtual BoundStatement InstrumentIfStatementAfterIfStatement(BoundIfStatement original, BoundStatement afterIfStatement)
		{
			return afterIfStatement;
		}

		public virtual BoundStatement InstrumentIfStatementConsequenceEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement InstrumentIfStatementAlternativeEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement CreateIfStatementAlternativePrologue(BoundIfStatement original)
		{
			return null;
		}

		public virtual BoundExpression InstrumentDoLoopStatementCondition(BoundDoLoopStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenCondition;
		}

		public virtual BoundExpression InstrumentWhileStatementCondition(BoundWhileStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenCondition;
		}

		public virtual BoundExpression InstrumentForEachStatementCondition(BoundForEachStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenCondition;
		}

		public virtual BoundExpression InstrumentObjectForLoopInitCondition(BoundForToStatement original, BoundExpression rewrittenInitCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenInitCondition;
		}

		public virtual BoundExpression InstrumentObjectForLoopCondition(BoundForToStatement original, BoundExpression rewrittenLoopCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenLoopCondition;
		}

		public virtual BoundExpression InstrumentIfStatementCondition(BoundIfStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenCondition;
		}

		public virtual BoundExpression InstrumentCatchBlockFilter(BoundCatchBlock original, BoundExpression rewrittenFilter, MethodSymbol currentMethodOrLambda)
		{
			return rewrittenFilter;
		}

		public virtual BoundStatement CreateCatchBlockPrologue(BoundCatchBlock original)
		{
			return null;
		}

		public virtual BoundStatement CreateFinallyBlockPrologue(BoundTryStatement original)
		{
			return null;
		}

		public virtual BoundStatement CreateTryBlockPrologue(BoundTryStatement original)
		{
			return null;
		}

		public virtual BoundStatement InstrumentTryStatement(BoundTryStatement original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement CreateSelectStatementPrologue(BoundSelectStatement original)
		{
			return null;
		}

		public virtual BoundExpression InstrumentSelectStatementCaseCondition(BoundSelectStatement original, BoundExpression rewrittenCaseCondition, MethodSymbol currentMethodOrLambda, ref LocalSymbol lazyConditionalBranchLocal)
		{
			return rewrittenCaseCondition;
		}

		public virtual BoundStatement InstrumentCaseBlockConditionalGoto(BoundCaseBlock original, BoundStatement condGoto)
		{
			return condGoto;
		}

		public virtual BoundStatement InstrumentCaseElseBlock(BoundCaseBlock original, BoundBlock rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentSelectStatementEpilogue(BoundSelectStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement InstrumentFieldOrPropertyInitializer(BoundFieldOrPropertyInitializer original, BoundStatement rewritten, int symbolIndex, bool createTemporary)
		{
			return rewritten;
		}

		public virtual BoundStatement InstrumentForEachLoopInitialization(BoundForEachStatement original, BoundStatement initialization)
		{
			return initialization;
		}

		public virtual BoundStatement InstrumentForEachLoopEpilogue(BoundForEachStatement original, BoundStatement epilogueOpt)
		{
			return epilogueOpt;
		}

		public virtual BoundStatement InstrumentForLoopInitialization(BoundForToStatement original, BoundStatement initialization)
		{
			return initialization;
		}

		public virtual BoundStatement InstrumentForLoopIncrement(BoundForToStatement original, BoundStatement increment)
		{
			return increment;
		}

		public virtual BoundStatement InstrumentLocalInitialization(BoundLocalDeclaration original, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement CreateUsingStatementPrologue(BoundUsingStatement original)
		{
			return null;
		}

		public virtual BoundStatement InstrumentUsingStatementResourceCapture(BoundUsingStatement original, int resourceIndex, BoundStatement rewritten)
		{
			return rewritten;
		}

		public virtual BoundStatement CreateUsingStatementDisposePrologue(BoundUsingStatement original)
		{
			return null;
		}

		public virtual BoundStatement CreateWithStatementPrologue(BoundWithStatement original)
		{
			return null;
		}

		public virtual BoundStatement CreateWithStatementEpilogue(BoundWithStatement original)
		{
			return null;
		}
	}
}
