Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SelectBlockContext
		Inherits ExecutableStatementContext
		Private ReadOnly _caseBlocks As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.SelectBlock, statement, prevContext)
			Me._caseBlocks = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)()
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax = Nothing
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(selectStatementSyntax, endBlockStatementSyntax)
			Dim selectBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax = MyBase.SyntaxFactory.SelectBlock(selectStatementSyntax, Me._caseBlocks.ToList(), endBlockStatementSyntax)
			Me._parser._pool.Free(Me._caseBlocks)
			MyBase.FreeStatements()
			Return selectBlockSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim caseBlockContext As BlockContext
			Select Case node.Kind
				Case SyntaxKind.CaseBlock
				Case SyntaxKind.CaseElseBlock
					Me._caseBlocks.Add(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax))
					caseBlockContext = Me
					Exit Select
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock
				Case SyntaxKind.List Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock
				Label0:
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExpectedCase)
					Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax = MyBase.SyntaxFactory.CaseStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.CaseKeyword), New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax)())
					caseBlockContext = (New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockContext(SyntaxKind.CaseBlock, caseStatementSyntax, Me)).ProcessSyntax(node)
					Exit Select
				Case SyntaxKind.CaseStatement
					caseBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockContext(SyntaxKind.CaseBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Exit Select
				Case SyntaxKind.CaseElseStatement
					caseBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockContext(SyntaxKind.CaseElseBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return caseBlockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			If (Not Me.KindEndsBlock(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (kind = SyntaxKind.CaseBlock) Then
					linkResult = MyBase.UseSyntax(node, newContext, False) Or BlockContext.LinkResult.SkipTerminator
				Else
					linkResult = If(CUShort(kind) - CUShort(SyntaxKind.CaseStatement) > CUShort(SyntaxKind.List), BlockContext.LinkResult.NotUsed, MyBase.UseSyntax(node, newContext, False))
				End If
			Else
				linkResult = MyBase.UseSyntax(node, newContext, False)
			End If
			Return linkResult
		End Function
	End Class
End Namespace