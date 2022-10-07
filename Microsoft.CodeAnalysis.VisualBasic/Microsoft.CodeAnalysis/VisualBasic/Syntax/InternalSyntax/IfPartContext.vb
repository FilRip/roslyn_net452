Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class IfPartContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(kind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (MyBase.BeginStatement.Kind <> SyntaxKind.ElseStatement) Then
				visualBasicSyntaxNode = MyBase.SyntaxFactory.ElseIfBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax), MyBase.Body())
			Else
				visualBasicSyntaxNode = MyBase.SyntaxFactory.ElseBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax), MyBase.Body())
			End If
			MyBase.FreeStatements()
			Return visualBasicSyntaxNode
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.CreateBlockSyntax(Nothing)
			Return MyBase.PrevBlock.ProcessSyntax(visualBasicSyntaxNode).EndBlock(statement)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			blockContext = If(CUShort(node.Kind) - CUShort(SyntaxKind.ElseIfStatement) > CUShort(SyntaxKind.List) OrElse MyBase.BlockKind <> SyntaxKind.ElseIfBlock, MyBase.ProcessSyntax(node), MyBase.PrevBlock.ProcessSyntax(Me.CreateBlockSyntax(Nothing)).ProcessSyntax(node))
			Return blockContext
		End Function

		Friend Overrides Function ResyncAndProcessStatementTerminator(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			If (statement.Kind <> SyntaxKind.ElseStatement OrElse SyntaxFacts.IsTerminator(MyBase.Parser.CurrentToken.Kind)) Then
				blockContext = MyBase.ResyncAndProcessStatementTerminator(statement, lambdaContext)
			Else
				MyBase.Parser.ConsumedStatementTerminator(False)
				blockContext = Me
			End If
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			linkResult = If(CUShort(node.Kind) - CUShort(SyntaxKind.ElseIfStatement) > CUShort(SyntaxKind.List), MyBase.TryLinkSyntax(node, newContext), MyBase.UseSyntax(node, newContext, False))
			Return linkResult
		End Function
	End Class
End Namespace