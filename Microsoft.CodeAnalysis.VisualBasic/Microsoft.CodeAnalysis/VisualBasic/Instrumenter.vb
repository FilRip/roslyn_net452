Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class Instrumenter
		Public ReadOnly Shared NoOp As Instrumenter

		Shared Sub New()
			Instrumenter.NoOp = New Instrumenter()
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function CreateBlockPrologue(ByVal trueOriginal As BoundBlock, ByVal original As BoundBlock, ByRef synthesizedLocal As LocalSymbol) As BoundStatement
			synthesizedLocal = Nothing
			Return Nothing
		End Function

		Public Overridable Function CreateCatchBlockPrologue(ByVal original As BoundCatchBlock) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateFinallyBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateIfStatementAlternativePrologue(ByVal original As BoundIfStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateSelectStatementPrologue(ByVal original As BoundSelectStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateSyncLockExitDueToExceptionEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateSyncLockExitNormallyEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateSyncLockStatementPrologue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateTryBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateUsingStatementDisposePrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateUsingStatementPrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateWithStatementEpilogue(ByVal original As BoundWithStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function CreateWithStatementPrologue(ByVal original As BoundWithStatement) As BoundStatement
			Return Nothing
		End Function

		Public Overridable Function InstrumentAddHandlerStatement(ByVal original As BoundAddHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentCaseBlockConditionalGoto(ByVal original As BoundCaseBlock, ByVal condGoto As BoundStatement) As BoundStatement
			Return condGoto
		End Function

		Public Overridable Function InstrumentCaseElseBlock(ByVal original As BoundCaseBlock, ByVal rewritten As BoundBlock) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentCatchBlockFilter(ByVal original As BoundCatchBlock, ByVal rewrittenFilter As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenFilter
		End Function

		Public Overridable Function InstrumentContinueStatement(ByVal original As BoundContinueStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentDoLoopEpilogue(ByVal original As BoundDoLoopStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Public Overridable Function InstrumentDoLoopStatementCondition(ByVal original As BoundDoLoopStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenCondition
		End Function

		Public Overridable Function InstrumentDoLoopStatementEntryOrConditionalGotoStart(ByVal original As BoundDoLoopStatement, ByVal ifConditionGotoStartOpt As BoundStatement) As BoundStatement
			Return ifConditionGotoStartOpt
		End Function

		Public Overridable Function InstrumentEndStatement(ByVal original As BoundEndStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentExitStatement(ByVal original As BoundExitStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentExpressionStatement(ByVal original As BoundExpressionStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentFieldOrPropertyInitializer(ByVal original As BoundFieldOrPropertyInitializer, ByVal rewritten As BoundStatement, ByVal symbolIndex As Integer, ByVal createTemporary As Boolean) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentForEachLoopEpilogue(ByVal original As BoundForEachStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Public Overridable Function InstrumentForEachLoopInitialization(ByVal original As BoundForEachStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return initialization
		End Function

		Public Overridable Function InstrumentForEachStatementCondition(ByVal original As BoundForEachStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenCondition
		End Function

		Public Overridable Function InstrumentForEachStatementConditionalGotoStart(ByVal original As BoundForEachStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return ifConditionGotoStart
		End Function

		Public Overridable Function InstrumentForLoopIncrement(ByVal original As BoundForToStatement, ByVal increment As BoundStatement) As BoundStatement
			Return increment
		End Function

		Public Overridable Function InstrumentForLoopInitialization(ByVal original As BoundForToStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return initialization
		End Function

		Public Overridable Function InstrumentGotoStatement(ByVal original As BoundGotoStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentIfStatementAfterIfStatement(ByVal original As BoundIfStatement, ByVal afterIfStatement As BoundStatement) As BoundStatement
			Return afterIfStatement
		End Function

		Public Overridable Function InstrumentIfStatementAlternativeEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Public Overridable Function InstrumentIfStatementCondition(ByVal original As BoundIfStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenCondition
		End Function

		Public Overridable Function InstrumentIfStatementConditionalGoto(ByVal original As BoundIfStatement, ByVal condGoto As BoundStatement) As BoundStatement
			Return condGoto
		End Function

		Public Overridable Function InstrumentIfStatementConsequenceEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Public Overridable Function InstrumentLabelStatement(ByVal original As BoundLabelStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentLocalInitialization(ByVal original As BoundLocalDeclaration, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentObjectForLoopCondition(ByVal original As BoundForToStatement, ByVal rewrittenLoopCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenLoopCondition
		End Function

		Public Overridable Function InstrumentObjectForLoopInitCondition(ByVal original As BoundForToStatement, ByVal rewrittenInitCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenInitCondition
		End Function

		Public Overridable Function InstrumentOnErrorStatement(ByVal original As BoundOnErrorStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentQueryLambdaBody(ByVal original As BoundQueryLambda, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentRaiseEventStatement(ByVal original As BoundRaiseEventStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentRemoveHandlerStatement(ByVal original As BoundRemoveHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentResumeStatement(ByVal original As BoundResumeStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentReturnStatement(ByVal original As BoundReturnStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentSelectStatementCaseCondition(ByVal original As BoundSelectStatement, ByVal rewrittenCaseCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol, ByRef lazyConditionalBranchLocal As LocalSymbol) As BoundExpression
			Return rewrittenCaseCondition
		End Function

		Public Overridable Function InstrumentSelectStatementEpilogue(ByVal original As BoundSelectStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Private Shared Function InstrumentStatement(ByVal original As BoundStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentStopStatement(ByVal original As BoundStopStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentSyncLockObjectCapture(ByVal original As BoundSyncLockStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentThrowStatement(ByVal original As BoundThrowStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Instrumenter.InstrumentStatement(original, rewritten)
		End Function

		Public Overridable Function InstrumentTopLevelExpressionInQuery(ByVal original As BoundExpression, ByVal rewritten As BoundExpression) As BoundExpression
			Return rewritten
		End Function

		Public Overridable Function InstrumentTryStatement(ByVal original As BoundTryStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentUsingStatementResourceCapture(ByVal original As BoundUsingStatement, ByVal resourceIndex As Integer, ByVal rewritten As BoundStatement) As BoundStatement
			Return rewritten
		End Function

		Public Overridable Function InstrumentWhileEpilogue(ByVal original As BoundWhileStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return epilogueOpt
		End Function

		Public Overridable Function InstrumentWhileStatementCondition(ByVal original As BoundWhileStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return rewrittenCondition
		End Function

		Public Overridable Function InstrumentWhileStatementConditionalGotoStart(ByVal original As BoundWhileStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return ifConditionGotoStart
		End Function
	End Class
End Namespace