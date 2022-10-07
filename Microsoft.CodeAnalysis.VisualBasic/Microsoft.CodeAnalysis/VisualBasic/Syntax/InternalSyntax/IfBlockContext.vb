Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class IfBlockContext
		Inherits ExecutableStatementContext
		Private ReadOnly _elseIfBlocks As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)

		Private _optionalElseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.MultiLineIfBlock, statement, prevContext)
			Me._elseIfBlocks = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)()
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = MyBase.BeginStatement
			If (endStmt Is Nothing) Then
				beginStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(beginStatement, ERRID.ERR_ExpectedEndIf)
				endStmt = MyBase.SyntaxFactory.EndIfStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword))
			End If
			Dim syntaxFactory As ContextAwareSyntaxFactory = MyBase.SyntaxFactory
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(beginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = MyBase.Body()
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax) = Me._elseIfBlocks
			Dim multiLineIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax = syntaxFactory.MultiLineIfBlock(ifStatementSyntax, syntaxList, syntaxListBuilder.ToList(), Me._optionalElseBlock, DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax))
			Me._parser._pool.Free(Me._elseIfBlocks)
			MyBase.FreeStatements()
			Return multiLineIfBlockSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.CreateBlockSyntax(statement)
			Return MyBase.PrevBlock.ProcessSyntax(visualBasicSyntaxNode)
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim ifPartContext As BlockContext
			Select Case node.Kind
				Case SyntaxKind.ElseIfBlock
					Me._elseIfBlocks.Add(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax))
					Exit Select
				Case SyntaxKind.ElseBlock
					Me._optionalElseBlock = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)
					Exit Select
				Case SyntaxKind.IfStatement
					ifPartContext = MyBase.ProcessSyntax(node)
					Return ifPartContext
				Case SyntaxKind.ElseIfStatement
					ifPartContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfPartContext(SyntaxKind.ElseIfBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Return ifPartContext
				Case SyntaxKind.ElseStatement
					ifPartContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfPartContext(SyntaxKind.ElseBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Return ifPartContext
				Case Else
					ifPartContext = MyBase.ProcessSyntax(node)
					Return ifPartContext
			End Select
			ifPartContext = Me
			Return ifPartContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (CUShort(kind) - CUShort(SyntaxKind.ElseIfBlock) <= CUShort(SyntaxKind.List)) Then
				linkResult = MyBase.UseSyntax(node, newContext, False) Or BlockContext.LinkResult.SkipTerminator
			Else
				linkResult = If(CUShort(kind) - CUShort(SyntaxKind.ElseIfStatement) > CUShort(SyntaxKind.List), MyBase.TryLinkSyntax(node, newContext), MyBase.UseSyntax(node, newContext, False))
			End If
			Return linkResult
		End Function
	End Class
End Namespace