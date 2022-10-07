Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TryBlockContext
		Inherits ExecutableStatementContext
		Private ReadOnly _catchParts As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)

		Private _optionalFinallyPart As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.TryBlock, statement, prevContext)
			Me._catchParts = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)()
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)
			If (endStmt Is Nothing) Then
				beginStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)(beginStatement, ERRID.ERR_ExpectedEndTry)
				endStmt = MyBase.SyntaxFactory.EndTryStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.TryKeyword))
			End If
			Dim syntaxFactory As ContextAwareSyntaxFactory = MyBase.SyntaxFactory
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = MyBase.Body()
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax) = Me._catchParts
			Dim tryBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax = syntaxFactory.TryBlock(beginStatement, syntaxList, syntaxListBuilder.ToList(), Me._optionalFinallyPart, DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax))
			Me._parser._pool.Free(Me._catchParts)
			MyBase.FreeStatements()
			Return tryBlockSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.CreateBlockSyntax(statement)
			Return MyBase.PrevBlock.ProcessSyntax(visualBasicSyntaxNode)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim catchPartContext As BlockContext
			Dim kind As SyntaxKind = node.Kind
			Select Case kind
				Case SyntaxKind.CatchBlock
					Me._catchParts.Add(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax))
					Exit Select
				Case SyntaxKind.FinallyBlock
					Me._optionalFinallyPart = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)
					Exit Select
				Case SyntaxKind.TryStatement
					catchPartContext = MyBase.ProcessSyntax(node)
					Return catchPartContext
				Case SyntaxKind.CatchStatement
					catchPartContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchPartContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Return catchPartContext
				Case Else
					If (kind = SyntaxKind.FinallyStatement) Then
						catchPartContext = New FinallyPartContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Return catchPartContext
					Else
						catchPartContext = MyBase.ProcessSyntax(node)
						Return catchPartContext
					End If
			End Select
			catchPartContext = Me
			Return catchPartContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (CUShort(kind) - CUShort(SyntaxKind.CatchBlock) <= CUShort(SyntaxKind.List)) Then
				linkResult = MyBase.UseSyntax(node, newContext, False) Or BlockContext.LinkResult.SkipTerminator
			Else
				linkResult = If(kind = SyntaxKind.CatchStatement OrElse kind = SyntaxKind.FinallyStatement, MyBase.UseSyntax(node, newContext, False), MyBase.TryLinkSyntax(node, newContext))
			End If
			Return linkResult
		End Function
	End Class
End Namespace