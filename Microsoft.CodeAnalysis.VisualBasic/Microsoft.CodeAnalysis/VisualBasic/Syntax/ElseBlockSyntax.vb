Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ElseBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax

		Friend _statements As SyntaxNode

		Public ReadOnly Property ElseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax)(Me._elseStatement)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax(kind, errors, annotations, DirectCast(elseStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitElseBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitElseBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._elseStatement
			ElseIf (num = 1) Then
				syntaxNode = Me._statements
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim elseStatement As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				elseStatement = Me.ElseStatement
			ElseIf (num = 1) Then
				elseStatement = MyBase.GetRed(Me._statements, 1)
			Else
				elseStatement = Nothing
			End If
			Return elseStatement
		End Function

		Public Function Update(ByVal elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			If (elseStatement <> Me.ElseStatement OrElse statements <> Me.Statements) Then
				Dim elseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ElseBlock(elseStatement, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				elseBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, elseBlockSyntax1, elseBlockSyntax1.WithAnnotations(annotations))
			Else
				elseBlockSyntax = Me
			End If
			Return elseBlockSyntax
		End Function

		Public Function WithElseStatement(ByVal elseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			Return Me.Update(elseStatement, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			Return Me.Update(Me.ElseStatement, statements)
		End Function
	End Class
End Namespace