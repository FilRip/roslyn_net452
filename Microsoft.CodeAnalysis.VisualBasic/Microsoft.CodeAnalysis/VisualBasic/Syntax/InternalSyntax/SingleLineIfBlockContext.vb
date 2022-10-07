Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SingleLineIfBlockContext
		Inherits SingleLineIfOrElseBlockContext
		Private _optionalElseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax

		Private _haveElseClause As Boolean

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.SingleLineIfStatement, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.CreateIfBlockSyntax()
		End Function

		Private Function CreateIfBlockSyntax() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			Dim singleLineIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax = MyBase.SyntaxFactory.SingleLineIfStatement(beginStatement.IfKeyword, beginStatement.Condition, beginStatement.ThenKeyword, MyBase.Body(), Me._optionalElseClause)
			MyBase.FreeStatements()
			Return singleLineIfStatementSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim singleLineIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax = Me.CreateIfBlockSyntax()
			Return MyBase.PrevBlock.ProcessSyntax(singleLineIfStatementSyntax)
		End Function

		Friend Overrides Function ProcessStatementTerminator(ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = MyBase.Parser.CurrentToken
			Dim kind As SyntaxKind = currentToken.Kind
			If (kind = SyntaxKind.ColonToken) Then
				MyBase.Parser.ConsumeColonInSingleLineExpression()
				blockContext = Me
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.StatementTerminatorToken) > CUShort(SyntaxKind.List)) Then
					Throw ExceptionUtilities.UnexpectedValue(currentToken.Kind)
				End If
				blockContext = Me.EndBlock(Nothing).ProcessStatementTerminator(lambdaContext)
			End If
			Return blockContext
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim singleLineIfBlockContext As BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (kind > SyntaxKind.ElseStatement) Then
				If (kind <> SyntaxKind.CatchStatement AndAlso kind <> SyntaxKind.FinallyStatement) Then
					singleLineIfBlockContext = MyBase.ProcessSyntax(node)
					Return singleLineIfBlockContext
				End If
				MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, If(node.Kind = SyntaxKind.CatchStatement, ERRID.ERR_CatchNoMatchingTry, ERRID.ERR_FinallyNoMatchingTry)))
				singleLineIfBlockContext = Me.EndBlock(Nothing)
				Return singleLineIfBlockContext
			ElseIf (kind = SyntaxKind.SingleLineElseClause) Then
				Me._optionalElseClause = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)
				singleLineIfBlockContext = Me
				Return singleLineIfBlockContext
			Else
				Select Case kind
					Case SyntaxKind.IfStatement
						Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
						If (ifStatementSyntax.ThenKeyword Is Nothing OrElse SyntaxFacts.IsTerminator(MyBase.Parser.CurrentToken.Kind)) Then
							Exit Select
						End If
						singleLineIfBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfBlockContext(ifStatementSyntax, Me)
						Return singleLineIfBlockContext
					Case SyntaxKind.ElseIfStatement
						MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExpectedEOS))
						singleLineIfBlockContext = Me.EndBlock(Nothing)
						Return singleLineIfBlockContext
					Case SyntaxKind.ElseStatement
						If (Me._haveElseClause) Then
							Throw ExceptionUtilities.Unreachable
						End If
						Me._haveElseClause = True
						singleLineIfBlockContext = New SingleLineElseContext(SyntaxKind.SingleLineElseClause, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Return singleLineIfBlockContext
				End Select
			End If
			singleLineIfBlockContext = MyBase.ProcessSyntax(node)
			Return singleLineIfBlockContext
		End Function

		Friend Overrides Function ResyncAndProcessStatementTerminator(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = MyBase.Parser.CurrentToken.Kind
			If (kind = SyntaxKind.ElseKeyword) Then
				MyBase.Parser.ConsumedStatementTerminator(False)
				blockContext = Me
			ElseIf (kind = SyntaxKind.ColonToken OrElse CUShort(kind) - CUShort(SyntaxKind.StatementTerminatorToken) <= CUShort(SyntaxKind.List)) Then
				blockContext = Me.ProcessStatementTerminator(lambdaContext)
			ElseIf (Me._statements.Count <= 0) Then
				MyBase.Parser.ConsumedStatementTerminator(False)
				blockContext = Me
			Else
				blockContext = If(Not MyBase.TreatOtherAsStatementTerminator, MyBase.ResyncAndProcessStatementTerminator(statement, lambdaContext), MyBase.ProcessOtherAsStatementTerminator())
			End If
			Return blockContext
		End Function
	End Class
End Namespace