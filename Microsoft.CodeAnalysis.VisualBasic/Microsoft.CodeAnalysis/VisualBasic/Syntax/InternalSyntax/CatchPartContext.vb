Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CatchPartContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.CatchBlock, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim catchBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax = MyBase.SyntaxFactory.CatchBlock(DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax), MyBase.Body())
			MyBase.FreeStatements()
			Return catchBlockSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Return MyBase.PrevBlock.ProcessSyntax(Me.CreateBlockSyntax(Nothing)).EndBlock(statement)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			blockContext = If(kind = SyntaxKind.CatchStatement OrElse kind = SyntaxKind.FinallyStatement, MyBase.PrevBlock.ProcessSyntax(Me.CreateBlockSyntax(Nothing)).ProcessSyntax(node), MyBase.ProcessSyntax(node))
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