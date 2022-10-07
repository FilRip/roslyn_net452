Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ElseIfBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax

		Friend _statements As SyntaxNode

		Public ReadOnly Property ElseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax)(Me._elseIfStatement)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(kind, errors, annotations, DirectCast(elseIfStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitElseIfBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitElseIfBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._elseIfStatement
			ElseIf (num = 1) Then
				syntaxNode = Me._statements
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim elseIfStatement As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				elseIfStatement = Me.ElseIfStatement
			ElseIf (num = 1) Then
				elseIfStatement = MyBase.GetRed(Me._statements, 1)
			Else
				elseIfStatement = Nothing
			End If
			Return elseIfStatement
		End Function

		Public Function Update(ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax
			Dim elseIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax
			If (elseIfStatement <> Me.ElseIfStatement OrElse statements <> Me.Statements) Then
				Dim elseIfBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ElseIfBlock(elseIfStatement, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				elseIfBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, elseIfBlockSyntax1, elseIfBlockSyntax1.WithAnnotations(annotations))
			Else
				elseIfBlockSyntax = Me
			End If
			Return elseIfBlockSyntax
		End Function

		Public Function WithElseIfStatement(ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax
			Return Me.Update(elseIfStatement, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax
			Return Me.Update(Me.ElseIfStatement, statements)
		End Function
	End Class
End Namespace