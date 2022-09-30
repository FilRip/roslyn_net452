using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class CompoundInstrumenter : Instrumenter
	{
		public Instrumenter Previous { get; }

		public CompoundInstrumenter(Instrumenter previous)
		{
			Previous = previous;
		}

		public override BoundStatement InstrumentExpressionStatement(BoundExpressionStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentExpressionStatement(original, rewritten);
		}

		public override BoundStatement InstrumentStopStatement(BoundStopStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentStopStatement(original, rewritten);
		}

		public override BoundStatement InstrumentEndStatement(BoundEndStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentEndStatement(original, rewritten);
		}

		public override BoundStatement InstrumentContinueStatement(BoundContinueStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentContinueStatement(original, rewritten);
		}

		public override BoundStatement InstrumentExitStatement(BoundExitStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentExitStatement(original, rewritten);
		}

		public override BoundStatement InstrumentGotoStatement(BoundGotoStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentGotoStatement(original, rewritten);
		}

		public override BoundStatement InstrumentLabelStatement(BoundLabelStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentLabelStatement(original, rewritten);
		}

		public override BoundStatement InstrumentRaiseEventStatement(BoundRaiseEventStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentRaiseEventStatement(original, rewritten);
		}

		public override BoundStatement InstrumentReturnStatement(BoundReturnStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentReturnStatement(original, rewritten);
		}

		public override BoundStatement InstrumentThrowStatement(BoundThrowStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentThrowStatement(original, rewritten);
		}

		public override BoundStatement InstrumentOnErrorStatement(BoundOnErrorStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentOnErrorStatement(original, rewritten);
		}

		public override BoundStatement InstrumentResumeStatement(BoundResumeStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentResumeStatement(original, rewritten);
		}

		public override BoundStatement InstrumentAddHandlerStatement(BoundAddHandlerStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentAddHandlerStatement(original, rewritten);
		}

		public override BoundStatement InstrumentRemoveHandlerStatement(BoundRemoveHandlerStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentRemoveHandlerStatement(original, rewritten);
		}

		public override BoundStatement CreateBlockPrologue(BoundBlock trueOriginal, BoundBlock original, ref LocalSymbol synthesizedLocal)
		{
			return Previous.CreateBlockPrologue(trueOriginal, original, ref synthesizedLocal);
		}

		public override BoundExpression InstrumentTopLevelExpressionInQuery(BoundExpression original, BoundExpression rewritten)
		{
			return Previous.InstrumentTopLevelExpressionInQuery(original, rewritten);
		}

		public override BoundStatement InstrumentQueryLambdaBody(BoundQueryLambda original, BoundStatement rewritten)
		{
			return Previous.InstrumentQueryLambdaBody(original, rewritten);
		}

		public override BoundStatement InstrumentDoLoopEpilogue(BoundDoLoopStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentDoLoopEpilogue(original, epilogueOpt);
		}

		public override BoundStatement CreateSyncLockStatementPrologue(BoundSyncLockStatement original)
		{
			return Previous.CreateSyncLockStatementPrologue(original);
		}

		public override BoundStatement InstrumentSyncLockObjectCapture(BoundSyncLockStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentSyncLockObjectCapture(original, rewritten);
		}

		public override BoundStatement CreateSyncLockExitDueToExceptionEpilogue(BoundSyncLockStatement original)
		{
			return Previous.CreateSyncLockExitDueToExceptionEpilogue(original);
		}

		public override BoundStatement CreateSyncLockExitNormallyEpilogue(BoundSyncLockStatement original)
		{
			return Previous.CreateSyncLockExitNormallyEpilogue(original);
		}

		public override BoundStatement InstrumentWhileEpilogue(BoundWhileStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentWhileEpilogue(original, epilogueOpt);
		}

		public override BoundStatement InstrumentWhileStatementConditionalGotoStart(BoundWhileStatement original, BoundStatement ifConditionGotoStart)
		{
			return Previous.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart);
		}

		public override BoundStatement InstrumentDoLoopStatementEntryOrConditionalGotoStart(BoundDoLoopStatement original, BoundStatement ifConditionGotoStartOpt)
		{
			return Previous.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt);
		}

		public override BoundStatement InstrumentForEachStatementConditionalGotoStart(BoundForEachStatement original, BoundStatement ifConditionGotoStart)
		{
			return Previous.InstrumentForEachStatementConditionalGotoStart(original, ifConditionGotoStart);
		}

		public override BoundStatement InstrumentIfStatementConditionalGoto(BoundIfStatement original, BoundStatement condGoto)
		{
			return Previous.InstrumentIfStatementConditionalGoto(original, condGoto);
		}

		public override BoundStatement InstrumentIfStatementAfterIfStatement(BoundIfStatement original, BoundStatement afterIfStatement)
		{
			return Previous.InstrumentIfStatementAfterIfStatement(original, afterIfStatement);
		}

		public override BoundStatement InstrumentIfStatementConsequenceEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentIfStatementConsequenceEpilogue(original, epilogueOpt);
		}

		public override BoundStatement InstrumentIfStatementAlternativeEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentIfStatementAlternativeEpilogue(original, epilogueOpt);
		}

		public override BoundStatement CreateIfStatementAlternativePrologue(BoundIfStatement original)
		{
			return Previous.CreateIfStatementAlternativePrologue(original);
		}

		public override BoundExpression InstrumentDoLoopStatementCondition(BoundDoLoopStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentDoLoopStatementCondition(original, rewrittenCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentWhileStatementCondition(BoundWhileStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentWhileStatementCondition(original, rewrittenCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentForEachStatementCondition(BoundForEachStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentForEachStatementCondition(original, rewrittenCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentObjectForLoopInitCondition(BoundForToStatement original, BoundExpression rewrittenInitCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentObjectForLoopInitCondition(original, rewrittenInitCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentObjectForLoopCondition(BoundForToStatement original, BoundExpression rewrittenLoopCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentObjectForLoopCondition(original, rewrittenLoopCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentIfStatementCondition(BoundIfStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentIfStatementCondition(original, rewrittenCondition, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentCatchBlockFilter(BoundCatchBlock original, BoundExpression rewrittenFilter, MethodSymbol currentMethodOrLambda)
		{
			return Previous.InstrumentCatchBlockFilter(original, rewrittenFilter, currentMethodOrLambda);
		}

		public override BoundStatement CreateCatchBlockPrologue(BoundCatchBlock original)
		{
			return Previous.CreateCatchBlockPrologue(original);
		}

		public override BoundStatement CreateFinallyBlockPrologue(BoundTryStatement original)
		{
			return Previous.CreateFinallyBlockPrologue(original);
		}

		public override BoundStatement CreateTryBlockPrologue(BoundTryStatement original)
		{
			return Previous.CreateTryBlockPrologue(original);
		}

		public override BoundStatement InstrumentTryStatement(BoundTryStatement original, BoundStatement rewritten)
		{
			return Previous.InstrumentTryStatement(original, rewritten);
		}

		public override BoundStatement CreateSelectStatementPrologue(BoundSelectStatement original)
		{
			return Previous.CreateSelectStatementPrologue(original);
		}

		public override BoundExpression InstrumentSelectStatementCaseCondition(BoundSelectStatement original, BoundExpression rewrittenCaseCondition, MethodSymbol currentMethodOrLambda, ref LocalSymbol lazyConditionalBranchLocal)
		{
			return Previous.InstrumentSelectStatementCaseCondition(original, rewrittenCaseCondition, currentMethodOrLambda, ref lazyConditionalBranchLocal);
		}

		public override BoundStatement InstrumentCaseBlockConditionalGoto(BoundCaseBlock original, BoundStatement condGoto)
		{
			return Previous.InstrumentCaseBlockConditionalGoto(original, condGoto);
		}

		public override BoundStatement InstrumentCaseElseBlock(BoundCaseBlock original, BoundBlock rewritten)
		{
			return Previous.InstrumentCaseElseBlock(original, rewritten);
		}

		public override BoundStatement InstrumentSelectStatementEpilogue(BoundSelectStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentSelectStatementEpilogue(original, epilogueOpt);
		}

		public override BoundStatement InstrumentFieldOrPropertyInitializer(BoundFieldOrPropertyInitializer original, BoundStatement rewritten, int symbolIndex, bool createTemporary)
		{
			return Previous.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary);
		}

		public override BoundStatement InstrumentForEachLoopInitialization(BoundForEachStatement original, BoundStatement initialization)
		{
			return Previous.InstrumentForEachLoopInitialization(original, initialization);
		}

		public override BoundStatement InstrumentForEachLoopEpilogue(BoundForEachStatement original, BoundStatement epilogueOpt)
		{
			return Previous.InstrumentForEachLoopEpilogue(original, epilogueOpt);
		}

		public override BoundStatement InstrumentForLoopInitialization(BoundForToStatement original, BoundStatement initialization)
		{
			return Previous.InstrumentForLoopInitialization(original, initialization);
		}

		public override BoundStatement InstrumentForLoopIncrement(BoundForToStatement original, BoundStatement increment)
		{
			return Previous.InstrumentForLoopIncrement(original, increment);
		}

		public override BoundStatement InstrumentLocalInitialization(BoundLocalDeclaration original, BoundStatement rewritten)
		{
			return Previous.InstrumentLocalInitialization(original, rewritten);
		}

		public override BoundStatement CreateUsingStatementPrologue(BoundUsingStatement original)
		{
			return Previous.CreateUsingStatementPrologue(original);
		}

		public override BoundStatement InstrumentUsingStatementResourceCapture(BoundUsingStatement original, int resourceIndex, BoundStatement rewritten)
		{
			return Previous.InstrumentUsingStatementResourceCapture(original, resourceIndex, rewritten);
		}

		public override BoundStatement CreateUsingStatementDisposePrologue(BoundUsingStatement original)
		{
			return Previous.CreateUsingStatementDisposePrologue(original);
		}

		public override BoundStatement CreateWithStatementPrologue(BoundWithStatement original)
		{
			return Previous.CreateWithStatementPrologue(original);
		}

		public override BoundStatement CreateWithStatementEpilogue(BoundWithStatement original)
		{
			return Previous.CreateWithStatementEpilogue(original);
		}
	}
}
