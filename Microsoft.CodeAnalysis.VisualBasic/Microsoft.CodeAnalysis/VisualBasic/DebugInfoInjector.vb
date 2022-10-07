Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DebugInfoInjector
		Inherits CompoundInstrumenter
		Public ReadOnly Shared Singleton As DebugInfoInjector

		Shared Sub New()
			DebugInfoInjector.Singleton = New DebugInfoInjector(Instrumenter.NoOp)
		End Sub

		Public Sub New(ByVal previous As Instrumenter)
			MyBase.New(previous)
		End Sub

		Friend Shared Function AddConditionSequencePoint(ByVal condition As BoundExpression, ByVal containingCatchWithFilter As BoundCatchBlock, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
			Return DebugInfoInjector.AddConditionSequencePoint(condition, containingCatchWithFilter.ExceptionFilterOpt.Syntax.Parent, currentMethodOrLambda, localSymbol, False)
		End Function

		Friend Shared Function AddConditionSequencePoint(ByVal condition As BoundExpression, ByVal containingStatement As BoundStatement, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
			Return DebugInfoInjector.AddConditionSequencePoint(condition, containingStatement.Syntax, currentMethodOrLambda, localSymbol, False)
		End Function

		Friend Shared Function AddConditionSequencePoint(ByVal condition As BoundExpression, ByVal containingStatement As BoundStatement, ByVal currentMethodOrLambda As MethodSymbol, ByRef lazyConditionalBranchLocal As LocalSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(condition, containingStatement.Syntax, currentMethodOrLambda, lazyConditionalBranchLocal, True)
		End Function

		Private Shared Function AddConditionSequencePoint(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal synthesizedVariableSyntax As Microsoft.CodeAnalysis.SyntaxNode, ByVal currentMethodOrLambda As MethodSymbol, ByRef lazyConditionalBranchLocal As LocalSymbol, ByVal shareLocal As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequencePointExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim empty As ImmutableArray(Of LocalSymbol)
			If (currentMethodOrLambda.DeclaringCompilation.Options.EnableEditAndContinue) Then
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = condition.Syntax
				If (lazyConditionalBranchLocal Is Nothing) Then
					lazyConditionalBranchLocal = New SynthesizedLocal(currentMethodOrLambda, condition.Type, SynthesizedLocalKind.ConditionalBranchDiscriminator, synthesizedVariableSyntax, False)
				End If
				If (condition.ConstantValueOpt Is Nothing) Then
					boundSequencePointExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression(Nothing, DebugInfoInjector.MakeLocalRead(syntax, lazyConditionalBranchLocal), condition.Type, False)
				Else
					boundSequencePointExpression = condition
				End If
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundSequencePointExpression
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
				If (shareLocal) Then
					empty = ImmutableArray(Of LocalSymbol).Empty
				Else
					empty = ImmutableArray.Create(Of LocalSymbol)(lazyConditionalBranchLocal)
				End If
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntaxNode, empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(DebugInfoInjector.MakeAssignmentExpression(syntax, DebugInfoInjector.MakeLocalWrite(syntax, lazyConditionalBranchLocal), condition)), boundExpression, condition.Type, False)
			Else
				boundSequence = condition
			End If
			Return boundSequence
		End Function

		Public Overrides Function CreateBlockPrologue(ByVal trueOriginal As BoundBlock, ByVal original As BoundBlock, ByRef synthesizedLocal As LocalSymbol) As BoundStatement
			Return DebugInfoInjector.CreateBlockPrologue(original, MyBase.CreateBlockPrologue(trueOriginal, original, synthesizedLocal))
		End Function

		Public Shared Function CreateBlockPrologue(ByVal node As BoundBlock, ByVal previousPrologue As BoundStatement) As BoundStatement
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntax As MethodBlockBaseSyntax = TryCast(node.Syntax, MethodBlockBaseSyntax)
			If (syntax Is Nothing) Then
				Dim lambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax = TryCast(node.Syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax)
				If (lambdaExpressionSyntax IsNot Nothing) Then
					previousPrologue = New BoundSequencePoint(lambdaExpressionSyntax.SubOrFunctionHeader, previousPrologue, False)
				End If
			Else
				Dim blockStatement As MethodBaseSyntax = syntax.BlockStatement
				syntaxToken = If(blockStatement.Modifiers.Count <= 0, blockStatement.DeclarationKeyword, blockStatement.Modifiers(0))
				Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(syntaxToken.SpanStart, blockStatement.Span.[End])
				previousPrologue = New BoundSequencePointWithSpan(blockStatement, previousPrologue, textSpan, False)
			End If
			Return previousPrologue
		End Function

		Public Overrides Function CreateCatchBlockPrologue(ByVal original As BoundCatchBlock) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, CatchBlockSyntax).CatchStatement, MyBase.CreateCatchBlockPrologue(original), False)
		End Function

		Public Overrides Function CreateFinallyBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.FinallyBlockOpt.Syntax, FinallyBlockSyntax).FinallyStatement, MyBase.CreateFinallyBlockPrologue(original), False)
		End Function

		Public Overrides Function CreateIfStatementAlternativePrologue(ByVal original As BoundIfStatement) As BoundStatement
			Dim boundSequencePointWithSpan As BoundStatement = MyBase.CreateIfStatementAlternativePrologue(original)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = original.AlternativeOpt.Syntax.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause) Then
				Dim syntax As SyntaxNode = original.AlternativeOpt.Syntax
				Dim elseKeyword As SyntaxToken = DirectCast(original.AlternativeOpt.Syntax, SingleLineElseClauseSyntax).ElseKeyword
				boundSequencePointWithSpan = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan(syntax, boundSequencePointWithSpan, elseKeyword.Span, False)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock) Then
				boundSequencePointWithSpan = New BoundSequencePoint(DirectCast(original.AlternativeOpt.Syntax, ElseBlockSyntax).ElseStatement, boundSequencePointWithSpan, False)
			End If
			Return boundSequencePointWithSpan
		End Function

		Public Overrides Function CreateSelectStatementPrologue(ByVal original As BoundSelectStatement) As BoundStatement
			Return New BoundSequencePoint(original.ExpressionStatement.Syntax, MyBase.CreateSelectStatementPrologue(original), False)
		End Function

		Public Overrides Function CreateSyncLockExitDueToExceptionEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, SyncLockBlockSyntax).EndSyncLockStatement, MyBase.CreateSyncLockExitDueToExceptionEpilogue(original), False)
		End Function

		Public Overrides Function CreateSyncLockExitNormallyEpilogue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, SyncLockBlockSyntax).EndSyncLockStatement, MyBase.CreateSyncLockExitNormallyEpilogue(original), False)
		End Function

		Public Overrides Function CreateSyncLockStatementPrologue(ByVal original As BoundSyncLockStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, SyncLockBlockSyntax).SyncLockStatement, MyBase.CreateSyncLockStatementPrologue(original), False)
		End Function

		Public Overrides Function CreateTryBlockPrologue(ByVal original As BoundTryStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, TryBlockSyntax).TryStatement, MyBase.CreateTryBlockPrologue(original), False)
		End Function

		Public Overrides Function CreateUsingStatementDisposePrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, UsingBlockSyntax).EndUsingStatement, MyBase.CreateUsingStatementDisposePrologue(original), False)
		End Function

		Public Overrides Function CreateUsingStatementPrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return New BoundSequencePoint(original.UsingInfo.UsingStatementSyntax.UsingStatement, MyBase.CreateUsingStatementPrologue(original), False)
		End Function

		Public Overrides Function CreateWithStatementEpilogue(ByVal original As BoundWithStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, WithBlockSyntax).EndWithStatement, MyBase.CreateWithStatementEpilogue(original), False)
		End Function

		Public Overrides Function CreateWithStatementPrologue(ByVal original As BoundWithStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, WithBlockSyntax).WithStatement, MyBase.CreateWithStatementPrologue(original), False)
		End Function

		Public Overrides Function InstrumentAddHandlerStatement(ByVal original As BoundAddHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentAddHandlerStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentCaseBlockConditionalGoto(ByVal original As BoundCaseBlock, ByVal condGoto As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(original.CaseStatement.Syntax, MyBase.InstrumentCaseBlockConditionalGoto(original, condGoto), False)
		End Function

		Public Overrides Function InstrumentCaseElseBlock(ByVal original As BoundCaseBlock, ByVal rewritten As BoundBlock) As BoundStatement
			Return New BoundSequencePoint(original.CaseStatement.Syntax, MyBase.InstrumentCaseElseBlock(original, rewritten), False)
		End Function

		Public Overrides Function InstrumentCatchBlockFilter(ByVal original As BoundCatchBlock, ByVal rewrittenFilter As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			rewrittenFilter = MyBase.InstrumentCatchBlockFilter(original, rewrittenFilter, currentMethodOrLambda)
			rewrittenFilter = New BoundSequencePointExpression(DirectCast(original.Syntax, CatchBlockSyntax).CatchStatement, rewrittenFilter, rewrittenFilter.Type, False)
			Return DebugInfoInjector.AddConditionSequencePoint(rewrittenFilter, original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentContinueStatement(ByVal original As BoundContinueStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentContinueStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentDoLoopEpilogue(ByVal original As BoundDoLoopStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, DoLoopBlockSyntax).LoopStatement, MyBase.InstrumentDoLoopEpilogue(original, epilogueOpt), False)
		End Function

		Public Overrides Function InstrumentDoLoopStatementCondition(ByVal original As BoundDoLoopStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentDoLoopStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentDoLoopStatementEntryOrConditionalGotoStart(ByVal original As BoundDoLoopStatement, ByVal ifConditionGotoStartOpt As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, DoLoopBlockSyntax).DoStatement, MyBase.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt), False)
		End Function

		Public Overrides Function InstrumentEndStatement(ByVal original As BoundEndStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentEndStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentExitStatement(ByVal original As BoundExitStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentExitStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentExpressionStatement(ByVal original As BoundExpressionStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentExpressionStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentFieldOrPropertyInitializer(ByVal original As BoundFieldOrPropertyInitializer, ByVal rewritten As BoundStatement, ByVal symbolIndex As Integer, ByVal createTemporary As Boolean) As BoundStatement
			rewritten = MyBase.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary)
			If (createTemporary) Then
				rewritten = DebugInfoInjector.MarkInitializerSequencePoint(rewritten, original.Syntax, symbolIndex)
			End If
			Return rewritten
		End Function

		Public Overrides Function InstrumentForEachLoopEpilogue(ByVal original As BoundForEachStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			epilogueOpt = MyBase.InstrumentForEachLoopEpilogue(original, epilogueOpt)
			If (DirectCast(original.Syntax, ForEachBlockSyntax).NextStatement IsNot Nothing) Then
				epilogueOpt = New BoundSequencePoint(DirectCast(original.Syntax, ForEachBlockSyntax).NextStatement, epilogueOpt, False)
			End If
			Return epilogueOpt
		End Function

		Public Overrides Function InstrumentForEachLoopInitialization(ByVal original As BoundForEachStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, ForEachBlockSyntax).ForEachStatement, MyBase.InstrumentForEachLoopInitialization(original, initialization), False)
		End Function

		Public Overrides Function InstrumentForEachStatementCondition(ByVal original As BoundForEachStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentForEachStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentForEachStatementConditionalGotoStart(ByVal original As BoundForEachStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(Nothing, MyBase.InstrumentForEachStatementConditionalGotoStart(original, ifConditionGotoStart), False)
		End Function

		Public Overrides Function InstrumentForLoopIncrement(ByVal original As BoundForToStatement, ByVal increment As BoundStatement) As BoundStatement
			increment = MyBase.InstrumentForLoopIncrement(original, increment)
			If (DirectCast(original.Syntax, ForBlockSyntax).NextStatement IsNot Nothing) Then
				increment = New BoundSequencePoint(DirectCast(original.Syntax, ForBlockSyntax).NextStatement, increment, False)
			End If
			Return increment
		End Function

		Public Overrides Function InstrumentForLoopInitialization(ByVal original As BoundForToStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, ForBlockSyntax).ForStatement, MyBase.InstrumentForLoopInitialization(original, initialization), False)
		End Function

		Public Overrides Function InstrumentGotoStatement(ByVal original As BoundGotoStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentGotoStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentIfStatementAfterIfStatement(ByVal original As BoundIfStatement, ByVal afterIfStatement As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, MultiLineIfBlockSyntax).EndIfStatement, MyBase.InstrumentIfStatementAfterIfStatement(original, afterIfStatement), False)
		End Function

		Public Overrides Function InstrumentIfStatementAlternativeEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.AlternativeOpt.Syntax.Parent, MultiLineIfBlockSyntax).EndIfStatement, MyBase.InstrumentIfStatementAlternativeEpilogue(original, epilogueOpt), False)
		End Function

		Public Overrides Function InstrumentIfStatementCondition(ByVal original As BoundIfStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentIfStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentIfStatementConditionalGoto(ByVal original As BoundIfStatement, ByVal condGoto As BoundStatement) As BoundStatement
			condGoto = MyBase.InstrumentIfStatementConditionalGoto(original, condGoto)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = original.Syntax.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
				Dim syntax As SingleLineIfStatementSyntax = DirectCast(original.Syntax, SingleLineIfStatementSyntax)
				Dim spanStart As Integer = syntax.IfKeyword.SpanStart
				Dim thenKeyword As SyntaxToken = syntax.ThenKeyword
				condGoto = New BoundSequencePointWithSpan(syntax, condGoto, TextSpan.FromBounds(spanStart, thenKeyword.EndPosition - 1), False)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
				condGoto = New BoundSequencePoint(DirectCast(original.Syntax, MultiLineIfBlockSyntax).IfStatement, condGoto, False)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
				condGoto = New BoundSequencePoint(DirectCast(original.Syntax, ElseIfBlockSyntax).ElseIfStatement, condGoto, False)
			End If
			Return condGoto
		End Function

		Public Overrides Function InstrumentIfStatementConsequenceEpilogue(ByVal original As BoundIfStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Dim boundSequencePoint As BoundStatement
			epilogueOpt = MyBase.InstrumentIfStatementConsequenceEpilogue(original, epilogueOpt)
			Dim endIfStatement As VisualBasicSyntaxNode = Nothing
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = original.Syntax.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
				boundSequencePoint = epilogueOpt
			Else
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
					endIfStatement = DirectCast(original.Syntax, MultiLineIfBlockSyntax).EndIfStatement
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
					endIfStatement = DirectCast(original.Syntax.Parent, MultiLineIfBlockSyntax).EndIfStatement
				End If
				boundSequencePoint = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint(endIfStatement, epilogueOpt, False)
			End If
			Return boundSequencePoint
		End Function

		Public Overrides Function InstrumentLabelStatement(ByVal original As BoundLabelStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentLabelStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentLocalInitialization(ByVal original As BoundLocalDeclaration, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkInitializerSequencePoint(MyBase.InstrumentLocalInitialization(original, rewritten), original.Syntax)
		End Function

		Public Overrides Function InstrumentObjectForLoopCondition(ByVal original As BoundForToStatement, ByVal rewrittenLoopCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentObjectForLoopCondition(original, rewrittenLoopCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentObjectForLoopInitCondition(ByVal original As BoundForToStatement, ByVal rewrittenInitCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentObjectForLoopInitCondition(original, rewrittenInitCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentOnErrorStatement(ByVal original As BoundOnErrorStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentOnErrorStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentQueryLambdaBody(ByVal original As BoundQueryLambda, ByVal rewritten As BoundStatement) As BoundStatement
			Dim span As Microsoft.CodeAnalysis.Text.TextSpan = New Microsoft.CodeAnalysis.Text.TextSpan()
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan
			rewritten = MyBase.InstrumentQueryLambdaBody(original, rewritten)
			Dim syntax As SyntaxNode = Nothing
			Dim synthesizedKind As SynthesizedLambdaKind = original.LambdaSymbol.SynthesizedKind
			If (synthesizedKind = SynthesizedLambdaKind.AggregateQueryLambda) Then
				Dim parent As AggregateClauseSyntax = DirectCast(original.Syntax.Parent.Parent, AggregateClauseSyntax)
				If (parent.AggregationVariables.Count <> 1) Then
					syntax = parent
					If (parent.AdditionalQueryOperators.Count <> 0) Then
						Dim spanStart As Integer = parent.SpanStart
						textSpan = parent.AdditionalQueryOperators.Last().Span
						span = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(spanStart, textSpan.[End])
					Else
						Dim num As Integer = parent.SpanStart
						textSpan = parent.Variables.Last().Span
						span = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(num, textSpan.[End])
					End If
				Else
					syntax = parent
					span = parent.Span
				End If
			ElseIf (synthesizedKind = SynthesizedLambdaKind.LetVariableQueryLambda) Then
				syntax = original.Syntax
				Dim spanStart1 As Integer = original.Syntax.SpanStart
				textSpan = original.Syntax.Span
				span = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(spanStart1, textSpan.[End])
			End If
			If (syntax IsNot Nothing) Then
				rewritten = New BoundSequencePointWithSpan(syntax, rewritten, span, False)
			End If
			Return rewritten
		End Function

		Public Overrides Function InstrumentRaiseEventStatement(ByVal original As BoundRaiseEventStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentRaiseEventStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentRemoveHandlerStatement(ByVal original As BoundRemoveHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentRemoveHandlerStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentResumeStatement(ByVal original As BoundResumeStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentResumeStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentReturnStatement(ByVal original As BoundReturnStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentReturnStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentSelectStatementCaseCondition(ByVal original As BoundSelectStatement, ByVal rewrittenCaseCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol, ByRef lazyConditionalBranchLocal As LocalSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentSelectStatementCaseCondition(original, rewrittenCaseCondition, currentMethodOrLambda, lazyConditionalBranchLocal), original, currentMethodOrLambda, lazyConditionalBranchLocal)
		End Function

		Public Overrides Function InstrumentSelectStatementEpilogue(ByVal original As BoundSelectStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, SelectBlockSyntax).EndSelectStatement, MyBase.InstrumentSelectStatementEpilogue(original, epilogueOpt), False)
		End Function

		Public Overrides Function InstrumentStopStatement(ByVal original As BoundStopStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentStopStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentSyncLockObjectCapture(ByVal original As BoundSyncLockStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(original.LockExpression.Syntax, MyBase.InstrumentSyncLockObjectCapture(original, rewritten), False)
		End Function

		Public Overrides Function InstrumentThrowStatement(ByVal original As BoundThrowStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return DebugInfoInjector.MarkStatementWithSequencePoint(original, MyBase.InstrumentThrowStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentTopLevelExpressionInQuery(ByVal original As BoundExpression, ByVal rewritten As BoundExpression) As BoundExpression
			rewritten = MyBase.InstrumentTopLevelExpressionInQuery(original, rewritten)
			Return New BoundSequencePointExpression(original.Syntax, rewritten, rewritten.Type, False)
		End Function

		Public Overrides Function InstrumentTryStatement(ByVal original As BoundTryStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return New BoundStatementList(original.Syntax, ImmutableArray.Create(Of BoundStatement)(MyBase.InstrumentTryStatement(original, rewritten), New BoundSequencePoint(DirectCast(original.Syntax, TryBlockSyntax).EndTryStatement, Nothing, False)), False)
		End Function

		Public Overrides Function InstrumentUsingStatementResourceCapture(ByVal original As BoundUsingStatement, ByVal resourceIndex As Integer, ByVal rewritten As BoundStatement) As BoundStatement
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			rewritten = MyBase.InstrumentUsingStatementResourceCapture(original, resourceIndex, rewritten)
			If (Not original.ResourceList.IsDefault AndAlso original.ResourceList.Length > 1) Then
				Dim item As BoundLocalDeclarationBase = original.ResourceList(resourceIndex)
				syntaxNode = If(item.Kind <> BoundKind.LocalDeclaration, item.Syntax, item.Syntax.Parent)
				rewritten = New BoundSequencePoint(syntaxNode, rewritten, False)
			End If
			Return rewritten
		End Function

		Public Overrides Function InstrumentWhileEpilogue(ByVal original As BoundWhileStatement, ByVal epilogueOpt As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, WhileBlockSyntax).EndWhileStatement, MyBase.InstrumentWhileEpilogue(original, epilogueOpt), False)
		End Function

		Public Overrides Function InstrumentWhileStatementCondition(ByVal original As BoundWhileStatement, ByVal rewrittenCondition As BoundExpression, ByVal currentMethodOrLambda As MethodSymbol) As BoundExpression
			Return DebugInfoInjector.AddConditionSequencePoint(MyBase.InstrumentWhileStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda)
		End Function

		Public Overrides Function InstrumentWhileStatementConditionalGotoStart(ByVal original As BoundWhileStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(DirectCast(original.Syntax, WhileBlockSyntax).WhileStatement, MyBase.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart), False)
		End Function

		Private Shared Function MakeAssignmentExpression(ByVal syntax As SyntaxNode, ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundExpression
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, left, right, True, False)
			boundAssignmentOperator.SetWasCompilerGenerated()
			Return boundAssignmentOperator
		End Function

		Private Shared Function MakeLocalRead(ByVal syntax As SyntaxNode, ByVal localSym As LocalSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localSym, False, localSym.Type)
			boundLocal.SetWasCompilerGenerated()
			Return boundLocal
		End Function

		Private Shared Function MakeLocalWrite(ByVal syntax As SyntaxNode, ByVal localSym As LocalSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localSym, True, localSym.Type)
			boundLocal.SetWasCompilerGenerated()
			Return boundLocal
		End Function

		Private Shared Function MarkInitializerSequencePoint(ByVal rewrittenStatement As BoundStatement, ByVal syntax As SyntaxNode, ByVal nameIndex As Integer) As BoundStatement
			Dim boundSequencePointWithSpan As BoundStatement
			Dim span As Microsoft.CodeAnalysis.Text.TextSpan
			Dim [end] As Integer
			If (syntax.Parent.IsKind(SyntaxKind.PropertyStatement)) Then
				Dim parent As PropertyStatementSyntax = DirectCast(syntax.Parent, PropertyStatementSyntax)
				Dim spanStart As Integer = parent.Identifier.SpanStart
				If (parent.Initializer Is Nothing) Then
					span = parent.AsClause.Span
					[end] = span.[End]
				Else
					span = parent.Initializer.Span
					[end] = span.[End]
				End If
				Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(spanStart, [end])
				boundSequencePointWithSpan = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointWithSpan(syntax, rewrittenStatement, textSpan, False)
			ElseIf (Not syntax.IsKind(SyntaxKind.AsNewClause)) Then
				boundSequencePointWithSpan = If(Not syntax.IsKind(SyntaxKind.ModifiedIdentifier), New BoundSequencePoint(syntax.Parent, rewrittenStatement, False), New BoundSequencePoint(syntax, rewrittenStatement, False))
			Else
				Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = DirectCast(syntax.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
				If (variableDeclaratorSyntax.Names.Count <= 1) Then
					boundSequencePointWithSpan = New BoundSequencePoint(syntax.Parent, rewrittenStatement, False)
				Else
					Dim names As SeparatedSyntaxList(Of ModifiedIdentifierSyntax) = variableDeclaratorSyntax.Names
					boundSequencePointWithSpan = New BoundSequencePoint(names(nameIndex), rewrittenStatement, False)
				End If
			End If
			Return boundSequencePointWithSpan
		End Function

		Private Shared Function MarkInitializerSequencePoint(ByVal rewrittenStatement As BoundStatement, ByVal syntax As SyntaxNode) As BoundStatement
			Dim boundSequencePoint As BoundStatement
			If (DirectCast(syntax, ModifiedIdentifierSyntax).ArrayBounds Is Nothing) Then
				Dim parent As VariableDeclaratorSyntax = DirectCast(syntax.Parent, VariableDeclaratorSyntax)
				boundSequencePoint = If(parent.Names.Count <= 1, New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint(parent, rewrittenStatement, False), New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint(syntax, rewrittenStatement, False))
			Else
				boundSequencePoint = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePoint(syntax, rewrittenStatement, False)
			End If
			Return boundSequencePoint
		End Function

		Private Shared Function MarkStatementWithSequencePoint(ByVal original As BoundStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return New BoundSequencePoint(original.Syntax, rewritten, False)
		End Function
	End Class
End Namespace