Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class WhileBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax

		Friend _statements As SyntaxNode

		Friend _endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endWhileStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property WhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax)(Me._whileStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax, ByVal statements As SyntaxNode, ByVal endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax(kind, errors, annotations, DirectCast(whileStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endWhileStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitWhileBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitWhileBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._whileStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endWhileStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim whileStatement As SyntaxNode
			Select Case i
				Case 0
					whileStatement = Me.WhileStatement
					Exit Select
				Case 1
					whileStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					whileStatement = Me.EndWhileStatement
					Exit Select
				Case Else
					whileStatement = Nothing
					Exit Select
			End Select
			Return whileStatement
		End Function

		Public Function Update(ByVal whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			Dim whileBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			If (whileStatement <> Me.WhileStatement OrElse statements <> Me.Statements OrElse endWhileStatement <> Me.EndWhileStatement) Then
				Dim whileBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.WhileBlock(whileStatement, statements, endWhileStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				whileBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, whileBlockSyntax1, whileBlockSyntax1.WithAnnotations(annotations))
			Else
				whileBlockSyntax = Me
			End If
			Return whileBlockSyntax
		End Function

		Public Function WithEndWhileStatement(ByVal endWhileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			Return Me.Update(Me.WhileStatement, Me.Statements, endWhileStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			Return Me.Update(Me.WhileStatement, statements, Me.EndWhileStatement)
		End Function

		Public Function WithWhileStatement(ByVal whileStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax
			Return Me.Update(whileStatement, Me.Statements, Me.EndWhileStatement)
		End Function
	End Class
End Namespace