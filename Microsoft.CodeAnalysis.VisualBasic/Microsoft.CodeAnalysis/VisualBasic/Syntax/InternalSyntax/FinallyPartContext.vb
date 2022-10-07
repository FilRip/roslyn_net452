Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class FinallyPartContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.FinallyBlock, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax = MyBase.SyntaxFactory.FinallyBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax), MyBase.Body())
			MyBase.FreeStatements()
			Return finallyBlockSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Return MyBase.PrevBlock.ProcessSyntax(Me.CreateBlockSyntax(Nothing)).EndBlock(statement)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (kind = SyntaxKind.CatchStatement) Then
				MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_CatchAfterFinally))
				blockContext = Me
			ElseIf (kind = SyntaxKind.FinallyStatement) Then
				MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_FinallyAfterFinally))
				blockContext = Me
			Else
				blockContext = MyBase.ProcessSyntax(node)
			End If
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			linkResult = If(kind = SyntaxKind.CatchStatement OrElse kind = SyntaxKind.FinallyStatement, MyBase.UseSyntax(node, newContext, False), MyBase.TryLinkSyntax(node, newContext))
			Return linkResult
		End Function
	End Class
End Namespace