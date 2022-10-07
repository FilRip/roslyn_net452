Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SingleLineLambdaContext
		Inherits MethodBlockContext
		Friend Overrides ReadOnly Property IsLambda As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsSingleLine As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(If(statement.Kind = SyntaxKind.FunctionLambdaHeader, SyntaxKind.SingleLineFunctionLambdaExpression, SyntaxKind.SingleLineSubLambdaExpression), statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = MyBase.Body()
			If (syntaxList.Count <> 0) Then
				item = syntaxList(0)
				flag = If(item.ContainsDiagnostics, False, Not SingleLineLambdaContext.IsSingleStatement(item))
			Else
				item = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingEmptyToken)
				flag = True
			End If
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)
			Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax = MyBase.SyntaxFactory.SingleLineLambdaExpression(MyBase.BlockKind, beginStatement, item)
			If (flag) Then
				singleLineLambdaExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)(singleLineLambdaExpressionSyntax, ERRID.ERR_SubRequiresSingleStatement)
			ElseIf (beginStatement.Kind = SyntaxKind.FunctionLambdaHeader AndAlso beginStatement.Modifiers.Any(632)) Then
				singleLineLambdaExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax)(singleLineLambdaExpressionSyntax, ERRID.ERR_BadIteratorExpressionLambda)
			End If
			MyBase.FreeStatements()
			Return singleLineLambdaExpressionSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Return MyBase.PrevBlock
		End Function

		Private Shared Function IsSingleStatement(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			Dim kind As SyntaxKind = statement.Kind
			If (kind <= SyntaxKind.MultiLineIfBlock) Then
				If (kind > SyntaxKind.WhileBlock) Then
					If (CUShort(kind) - CUShort(SyntaxKind.UsingBlock) <= CUShort(SyntaxKind.EmptyStatement) OrElse kind = SyntaxKind.MultiLineIfBlock) Then
						flag = False
						Return flag
					End If
					flag = True
					Return flag
				Else
					If (kind = SyntaxKind.EmptyStatement OrElse kind = SyntaxKind.WhileBlock) Then
						flag = False
						Return flag
					End If
					flag = True
					Return flag
				End If
			ElseIf (kind <= SyntaxKind.SelectBlock) Then
				If (kind = SyntaxKind.TryBlock OrElse kind = SyntaxKind.SelectBlock) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			ElseIf (CUShort(kind) - CUShort(SyntaxKind.ForBlock) > CUShort(SyntaxKind.List) AndAlso CUShort(kind) - CUShort(SyntaxKind.SimpleDoLoopBlock) > 4) Then
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Friend Overrides Function ProcessStatementTerminator(ByVal lambdaContext As BlockContext) As BlockContext
			Dim prevBlock As BlockContext
			Dim kind As SyntaxKind = MyBase.Parser.CurrentToken.Kind
			If (kind = SyntaxKind.ColonToken) Then
				If (Me._statements.Count > 0) Then
					Me._statements(0) = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements(0), ERRID.ERR_SubRequiresSingleStatement)
				End If
				prevBlock = Me.EndLambda()
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.StatementTerminatorToken) > CUShort(SyntaxKind.List)) Then
				End If
				prevBlock = MyBase.PrevBlock
			End If
			Return prevBlock
		End Function

		Friend Overrides Function ResyncAndProcessStatementTerminator(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal lambdaContext As BlockContext) As BlockContext
			Return Me.ProcessStatementTerminator(lambdaContext)
		End Function
	End Class
End Namespace