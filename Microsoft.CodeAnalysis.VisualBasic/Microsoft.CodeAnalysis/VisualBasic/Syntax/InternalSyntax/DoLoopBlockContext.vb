Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class DoLoopBlockContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(If(DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax).WhileOrUntilClause Is Nothing, SyntaxKind.SimpleDoLoopBlock, SyntaxKind.DoWhileLoopBlock), statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = Nothing
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)(doStatementSyntax, loopStatementSyntax)
			Dim blockKind As SyntaxKind = MyBase.BlockKind
			If (blockKind = SyntaxKind.DoWhileLoopBlock AndAlso loopStatementSyntax.WhileOrUntilClause IsNot Nothing) Then
				Dim whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = loopStatementSyntax.WhileOrUntilClause
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(whileOrUntilClause.WhileOrUntilKeyword, ERRID.ERR_LoopDoubleCondition)
				Dim diagnostics As DiagnosticInfo() = whileOrUntilClause.GetDiagnostics()
				whileOrUntilClause = MyBase.SyntaxFactory.WhileOrUntilClause(whileOrUntilClause.Kind, keywordSyntax, whileOrUntilClause.Condition)
				If (diagnostics IsNot Nothing) Then
					whileOrUntilClause = DirectCast(whileOrUntilClause.SetDiagnostics(diagnostics), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
				End If
				loopStatementSyntax = MyBase.SyntaxFactory.LoopStatement(loopStatementSyntax.Kind, loopStatementSyntax.LoopKeyword, whileOrUntilClause)
			End If
			If (blockKind = SyntaxKind.SimpleDoLoopBlock AndAlso loopStatementSyntax.WhileOrUntilClause IsNot Nothing) Then
				blockKind = If(loopStatementSyntax.Kind = SyntaxKind.LoopWhileStatement, SyntaxKind.DoLoopWhileBlock, SyntaxKind.DoLoopUntilBlock)
			ElseIf (doStatementSyntax.WhileOrUntilClause IsNot Nothing) Then
				blockKind = If(doStatementSyntax.Kind = SyntaxKind.DoWhileStatement, SyntaxKind.DoWhileLoopBlock, SyntaxKind.DoUntilLoopBlock)
			End If
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax = MyBase.SyntaxFactory.DoLoopBlock(blockKind, doStatementSyntax, MyBase.Body(), loopStatementSyntax)
			MyBase.FreeStatements()
			Return doLoopBlockSyntax
		End Function

		Friend Overrides Function KindEndsBlock(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SimpleLoopStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function
	End Class
End Namespace