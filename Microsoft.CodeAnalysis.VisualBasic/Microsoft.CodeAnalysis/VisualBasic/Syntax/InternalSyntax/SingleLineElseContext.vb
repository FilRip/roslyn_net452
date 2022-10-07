Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SingleLineElseContext
		Inherits SingleLineIfOrElseBlockContext
		Private ReadOnly Property TreatElseAsStatementTerminator As Boolean
			Get
				Dim flag As Boolean
				Dim prevBlock As BlockContext = MyBase.PrevBlock.PrevBlock
				While True
					If (prevBlock.BlockKind <> SyntaxKind.SingleLineIfStatement) Then
						Dim blockKind As SyntaxKind = prevBlock.BlockKind
						If (blockKind = SyntaxKind.SingleLineElseClause) Then
							prevBlock = prevBlock.PrevBlock.PrevBlock
						ElseIf (blockKind = SyntaxKind.SingleLineSubLambdaExpression) Then
							flag = True
							Exit While
						Else
							flag = False
							Exit While
						End If
					Else
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(kind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.CreateElseBlockSyntax()
		End Function

		Private Function CreateElseBlockSyntax() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax = MyBase.SyntaxFactory.SingleLineElseClause(beginStatement.ElseKeyword, MyBase.OptionalBody())
			MyBase.FreeStatements()
			Return singleLineElseClauseSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Return MyBase.PrevBlock.ProcessSyntax(Me.CreateElseBlockSyntax()).EndBlock(Nothing)
		End Function

		Private Function ProcessElseAsStatementTerminator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim prevBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.EndBlock(Nothing)
			While True
				If (blockContext.BlockKind <> SyntaxKind.SingleLineIfStatement) Then
					Dim blockKind As SyntaxKind = blockContext.BlockKind
					If (blockKind = SyntaxKind.SingleLineElseClause) Then
						blockContext = blockContext.EndBlock(Nothing)
					Else
						If (blockKind <> SyntaxKind.SingleLineSubLambdaExpression) Then
							Throw ExceptionUtilities.UnexpectedValue(blockContext.BlockKind)
						End If
						prevBlock = blockContext.PrevBlock
						Exit While
					End If
				Else
					prevBlock = blockContext
					Exit While
				End If
			End While
			Return prevBlock
		End Function

		Friend Overrides Function ProcessStatementTerminator(ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = MyBase.Parser.CurrentToken
			Dim kind As SyntaxKind = currentToken.Kind
			If (kind = SyntaxKind.ColonToken) Then
				If (Me._statements.Count <= 0) Then
					blockContext = Me.EndBlock(Nothing).ProcessStatementTerminator(lambdaContext)
					Return blockContext
				End If
				MyBase.Parser.ConsumeColonInSingleLineExpression()
				blockContext = Me
				Return blockContext
			ElseIf (CUShort(kind) - CUShort(SyntaxKind.StatementTerminatorToken) > CUShort(SyntaxKind.List)) Then
				Throw ExceptionUtilities.UnexpectedValue(currentToken.Kind)
			End If
			blockContext = Me.EndBlock(Nothing).ProcessStatementTerminator(lambdaContext)
			Return blockContext
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (kind > SyntaxKind.ElseIfStatement) Then
				If (kind <> SyntaxKind.CatchStatement AndAlso kind <> SyntaxKind.FinallyStatement) Then
					blockContext = MyBase.ProcessSyntax(node)
					Return blockContext
				End If
				MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, If(node.Kind = SyntaxKind.CatchStatement, ERRID.ERR_CatchNoMatchingTry, ERRID.ERR_FinallyNoMatchingTry)))
				blockContext = Me.EndBlock(Nothing)
				Return blockContext
			ElseIf (kind = SyntaxKind.IfStatement) Then
				If (DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax).ThenKeyword Is Nothing) Then
					blockContext = MyBase.ProcessSyntax(node)
					Return blockContext
				End If
				SyntaxFacts.IsTerminator(MyBase.Parser.CurrentToken.Kind)
				blockContext = MyBase.ProcessSyntax(node)
				Return blockContext
			Else
				If (kind <> SyntaxKind.ElseIfStatement) Then
					blockContext = MyBase.ProcessSyntax(node)
					Return blockContext
				End If
				blockContext = Me.EndBlock(Nothing).ProcessSyntax(node)
				Return blockContext
			End If
			blockContext = MyBase.ProcessSyntax(node)
			Return blockContext
		End Function

		Friend Overrides Function ResyncAndProcessStatementTerminator(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = MyBase.Parser.CurrentToken.Kind
			If (kind = SyntaxKind.ElseKeyword) Then
				If (Not Me.TreatElseAsStatementTerminator) Then
					blockContext = MyBase.ResyncAndProcessStatementTerminator(statement, lambdaContext)
				Else
					MyBase.Parser.ConsumedStatementTerminator(False)
					blockContext = Me.ProcessElseAsStatementTerminator()
				End If
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