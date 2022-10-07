Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ForBlockContext
		Inherits ExecutableStatementContext
		Private ReadOnly Shared s_emptyNextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax

		Shared Sub New()
			ForBlockContext.s_emptyNextStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.NextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.NextKeyword), New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)())
		End Sub

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(If(statement.Kind = SyntaxKind.ForStatement, SyntaxKind.ForBlock, SyntaxKind.ForEachBlock), statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Nothing
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)(statementSyntax, nextStatementSyntax)
			If (endStmt = ForBlockContext.s_emptyNextStatement) Then
				nextStatementSyntax = Nothing
			End If
			If (MyBase.BlockKind <> SyntaxKind.ForBlock) Then
				visualBasicSyntaxNode = MyBase.SyntaxFactory.ForEachBlock(DirectCast(statementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax), MyBase.Body(), nextStatementSyntax)
			Else
				visualBasicSyntaxNode = MyBase.SyntaxFactory.ForBlock(DirectCast(statementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax), MyBase.Body(), nextStatementSyntax)
			End If
			MyBase.FreeStatements()
			Return visualBasicSyntaxNode
		End Function

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim prevBlock As BlockContext = Me
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = prevBlock.CreateBlockSyntax(endStmt)
			prevBlock = prevBlock.PrevBlock
			prevBlock = prevBlock.ProcessSyntax(visualBasicSyntaxNode)
			If (endStmt IsNot Nothing) Then
				Dim count As Integer = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax).ControlVariables.Count
				For i As Integer = 2 To (prevBlock.BlockKind = SyntaxKind.ForBlock OrElse prevBlock.BlockKind = SyntaxKind.ForEachBlock)
					visualBasicSyntaxNode = prevBlock.CreateBlockSyntax(ForBlockContext.s_emptyNextStatement)
					prevBlock = prevBlock.PrevBlock
					prevBlock = prevBlock.ProcessSyntax(visualBasicSyntaxNode)
				Next

			End If
			Return prevBlock
		End Function
	End Class
End Namespace