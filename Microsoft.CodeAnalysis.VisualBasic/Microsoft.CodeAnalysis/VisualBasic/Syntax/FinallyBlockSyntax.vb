Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class FinallyBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax

		Friend _statements As SyntaxNode

		Public ReadOnly Property FinallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax)(Me._finallyStatement)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(kind, errors, annotations, DirectCast(finallyStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitFinallyBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitFinallyBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._finallyStatement
			ElseIf (num = 1) Then
				syntaxNode = Me._statements
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim finallyStatement As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				finallyStatement = Me.FinallyStatement
			ElseIf (num = 1) Then
				finallyStatement = MyBase.GetRed(Me._statements, 1)
			Else
				finallyStatement = Nothing
			End If
			Return finallyStatement
		End Function

		Public Function Update(ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			If (finallyStatement <> Me.FinallyStatement OrElse statements <> Me.Statements) Then
				Dim finallyBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FinallyBlock(finallyStatement, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				finallyBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, finallyBlockSyntax1, finallyBlockSyntax1.WithAnnotations(annotations))
			Else
				finallyBlockSyntax = Me
			End If
			Return finallyBlockSyntax
		End Function

		Public Function WithFinallyStatement(ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			Return Me.Update(finallyStatement, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			Return Me.Update(Me.FinallyStatement, statements)
		End Function
	End Class
End Namespace