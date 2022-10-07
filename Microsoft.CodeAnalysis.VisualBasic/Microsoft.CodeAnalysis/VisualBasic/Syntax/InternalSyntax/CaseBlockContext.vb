Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CaseBlockContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal contextKind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(contextKind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			visualBasicSyntaxNode = If(MyBase.BlockKind <> SyntaxKind.CaseBlock, MyBase.SyntaxFactory.CaseElseBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax), MyBase.Body()), MyBase.SyntaxFactory.CaseBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax), MyBase.Body()))
			MyBase.FreeStatements()
			Return visualBasicSyntaxNode
		End Function

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.CreateBlockSyntax(Nothing)
			MyBase.PrevBlock.ProcessSyntax(visualBasicSyntaxNode)
			Return MyBase.PrevBlock.EndBlock(endStmt)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			If (CUShort(node.Kind) - CUShort(SyntaxKind.CaseStatement) > CUShort(SyntaxKind.List)) Then
				blockContext = MyBase.ProcessSyntax(node)
			Else
				If (MyBase.BlockKind = SyntaxKind.CaseElseBlock) Then
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_CaseAfterCaseElse)
				End If
				blockContext = MyBase.PrevBlock.ProcessSyntax(Me.CreateBlockSyntax(Nothing)).ProcessSyntax(node)
			End If
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			linkResult = If(CUShort(node.Kind) - CUShort(SyntaxKind.CaseStatement) > CUShort(SyntaxKind.List), MyBase.TryLinkSyntax(node, newContext), MyBase.UseSyntax(node, newContext, False))
			Return linkResult
		End Function
	End Class
End Namespace