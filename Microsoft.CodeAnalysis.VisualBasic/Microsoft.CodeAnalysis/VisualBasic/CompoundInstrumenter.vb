Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class CompoundInstrumenter
		Inherits Instrumenter
		Public ReadOnly Property Previous As Instrumenter

		Public Sub New(ByVal previous As Instrumenter)
			MyBase.New()
			Me.Previous = previous
		End Sub

		Public Overrides Function CreateBlockPrologue(ByVal trueOriginal As BoundBlock, ByVal original As BoundBlock, ByRef synthesizedLocal As LocalSymbol) As BoundStatement
			Return Me.Previous.CreateBlockPrologue(trueOriginal, original, synthesizedLocal)
		End Function

		Public Overrides Function CreateCatchBlockPrologue(ByVal original As BoundCatchBlock) As BoundStatement
			Return Me.Previous.CreateCatchBlockPrologue(original)
		End Function

		Public Overrides Function CreateFinallyBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return Me.Previous.CreateFinallyBlockPrologue(original)
		End Function

		Public Overrides Function CreateIfStatementAlternativePrologue(ByVal original As BoundIfStatement) As BoundStatement
			Return Me.Previous.CreateIfStatementAlternativePrologue(original)
		End Function

		Public Overrides Function CreateSelectStatementPrologue(ByVal original As BoundSelectStatement) As BoundStatement
			Return Me.Previous.CreateSelectStatementPrologue(original)
		End Function

		Public Overrides Function CreateSyncLockExitDueToExceptionEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Me.Previous.CreateSyncLockExitDueToExceptionEpilogue(original)
		End Function

		Public Overrides Function CreateSyncLockExitNormallyEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Me.Previous.CreateSyncLockExitNormallyEpilogue(original)
		End Function

		Public Overrides Function CreateSyncLockStatementPrologue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Me.Previous.CreateSyncLockStatementPrologue(original)
		End Function

		Public Overrides Function CreateTryBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return Me.Previous.CreateTryBlockPrologue(original)
		End Function

		Public Overrides Function CreateUsingStatementDisposePrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return Me.Previous.CreateUsingStatementDisposePrologue(original)
		End Function

		Public Overrides Function CreateUsingStatementPrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return Me.Previous.CreateUsingStatementPrologue(original)
		End Function

		Public Overrides Function CreateWithStatementEpilogue(ByVal original As BoundWithStatement) As BoundStatement
			Return Me.Previous.CreateWithStatementEpilogue(original)
		End Function

		Public Overrides Function CreateWithStatementPrologue(ByVal original As BoundWithStatement) As BoundStatement
			Return Me.Previous.CreateWithStatementPrologue(original)
		End Function

		Public Overrides Function InstrumentAddHandlerStatement(ByVal original As BoundAddHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentAddHandlerStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentCaseBlockConditionalGoto(ByVal original As BoundCaseBlock, ByVal condGoto As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentCaseBlockConditionalGoto(original, condGoto)
		End Function

		Public Overrides Function InstrumentCaseElseBlock(ByVal original As BoundCaseBlock, ByVal rewritten As BoundBlock) As BoundStatement
			Return Me.Previous.InstrumentCaseElseBlock(original, rewritten)
		End Function

		Public Overrides Function InstrumentCatchBlockFilter(ByVal original As BoundCatchBlock, ByVal rewrittenFilter As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentCatchBlockFilter(original, rewrittenFilter, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentContinueStatement(ByVal original As BoundContinueStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentContinueStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentDoLoopEpilogue(ByVal original As BoundDoLoopStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentDoLoopEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentDoLoopStatementCondition(ByVal original As BoundDoLoopStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentDoLoopStatementCondition(original, rewrittenCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentDoLoopStatementEntryOrConditionalGotoStart(ByVal original As BoundDoLoopStatement, ByVal ifConditionGotoStartOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt)
		End Function

		Public Overrides Function InstrumentEndStatement(ByVal original As BoundEndStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentEndStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentExitStatement(ByVal original As BoundExitStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentExitStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentExpressionStatement(ByVal original As BoundExpressionStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentExpressionStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentFieldOrPropertyInitializer(ByVal original As BoundFieldOrPropertyInitializer, ByVal rewritten As BoundStatement, ByVal symbolIndex As Integer, ByVal createTemporary As Boolean) As BoundStatement
			Return Me.Previous.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary)
		End Function

		Public Overrides Function InstrumentForEachLoopEpilogue(ByVal original As BoundForEachStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentForEachLoopEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentForEachLoopInitialization(ByVal original As BoundForEachStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentForEachLoopInitialization(original, initialization)
		End Function

		Public Overrides Function InstrumentForEachStatementCondition(ByVal original As BoundForEachStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentForEachStatementCondition(original, rewrittenCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentForEachStatementConditionalGotoStart(ByVal original As BoundForEachStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentForEachStatementConditionalGotoStart(original, ifConditionGotoStart)
		End Function

		Public Overrides Function InstrumentForLoopIncrement(ByVal original As BoundForToStatement, ByVal increment As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentForLoopIncrement(original, increment)
		End Function

		Public Overrides Function InstrumentForLoopInitialization(ByVal original As BoundForToStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentForLoopInitialization(original, initialization)
		End Function

		Public Overrides Function InstrumentGotoStatement(ByVal original As BoundGotoStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentGotoStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentIfStatementAfterIfStatement(ByVal original As BoundIfStatement, ByVal afterIfStatement As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentIfStatementAfterIfStatement(original, afterIfStatement)
		End Function

		Public Overrides Function InstrumentIfStatementAlternativeEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentIfStatementAlternativeEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentIfStatementCondition(ByVal original As BoundIfStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentIfStatementCondition(original, rewrittenCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentIfStatementConditionalGoto(ByVal original As BoundIfStatement, ByVal condGoto As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentIfStatementConditionalGoto(original, condGoto)
		End Function

		Public Overrides Function InstrumentIfStatementConsequenceEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentIfStatementConsequenceEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentLabelStatement(ByVal original As BoundLabelStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentLabelStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentLocalInitialization(ByVal original As BoundLocalDeclaration, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentLocalInitialization(original, rewritten)
		End Function

		Public Overrides Function InstrumentObjectForLoopCondition(ByVal original As BoundForToStatement, ByVal rewrittenLoopCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentObjectForLoopCondition(original, rewrittenLoopCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentObjectForLoopInitCondition(ByVal original As BoundForToStatement, ByVal rewrittenInitCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentObjectForLoopInitCondition(original, rewrittenInitCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentOnErrorStatement(ByVal original As BoundOnErrorStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentOnErrorStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentQueryLambdaBody(ByVal original As BoundQueryLambda, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentQueryLambdaBody(original, rewritten)
		End Function

		Public Overrides Function InstrumentRaiseEventStatement(ByVal original As BoundRaiseEventStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentRaiseEventStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentRemoveHandlerStatement(ByVal original As BoundRemoveHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentRemoveHandlerStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentResumeStatement(ByVal original As BoundResumeStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentResumeStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentReturnStatement(ByVal original As BoundReturnStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentReturnStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentSelectStatementCaseCondition(ByVal original As BoundSelectStatement, ByVal rewrittenCaseCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol, ByRef lazyConditionalBranchLocal As LocalSymbol) As BoundExpression
			Return Me.Previous.InstrumentSelectStatementCaseCondition(original, rewrittenCaseCondition, currentMethodOrLambda, lazyConditionalBranchLocal)
		End Function

		Public Overrides Function InstrumentSelectStatementEpilogue(ByVal original As BoundSelectStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentSelectStatementEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentStopStatement(ByVal original As BoundStopStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentStopStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentSyncLockObjectCapture(ByVal original As BoundSyncLockStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentSyncLockObjectCapture(original, rewritten)
		End Function

		Public Overrides Function InstrumentThrowStatement(ByVal original As BoundThrowStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentThrowStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentTopLevelExpressionInQuery(ByVal original As BoundExpression, ByVal rewritten As BoundExpression) As BoundExpression
			Return Me.Previous.InstrumentTopLevelExpressionInQuery(original, rewritten)
		End Function

		Public Overrides Function InstrumentTryStatement(ByVal original As BoundTryStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentTryStatement(original, rewritten)
		End Function

		Public Overrides Function InstrumentUsingStatementResourceCapture(ByVal original As BoundUsingStatement, ByVal resourceIndex As Integer, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentUsingStatementResourceCapture(original, resourceIndex, rewritten)
		End Function

		Public Overrides Function InstrumentWhileEpilogue(ByVal original As BoundWhileStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentWhileEpilogue(original, epilogueOpt)
		End Function

		Public Overrides Function InstrumentWhileStatementCondition(ByVal original As BoundWhileStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return Me.Previous.InstrumentWhileStatementCondition(original, rewrittenCondition, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentWhileStatementConditionalGotoStart(ByVal original As BoundWhileStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return Me.Previous.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart)
		End Function
	End Class
End Namespace