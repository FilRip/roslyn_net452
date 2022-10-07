Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class UsingBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax

		Friend _statements As SyntaxNode

		Friend _endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endUsingStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property UsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax)(Me._usingStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax, ByVal statements As SyntaxNode, ByVal endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax(kind, errors, annotations, DirectCast(usingStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endUsingStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitUsingBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitUsingBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._usingStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endUsingStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim usingStatement As SyntaxNode
			Select Case i
				Case 0
					usingStatement = Me.UsingStatement
					Exit Select
				Case 1
					usingStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					usingStatement = Me.EndUsingStatement
					Exit Select
				Case Else
					usingStatement = Nothing
					Exit Select
			End Select
			Return usingStatement
		End Function

		Public Function Update(ByVal usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			Dim usingBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			If (usingStatement <> Me.UsingStatement OrElse statements <> Me.Statements OrElse endUsingStatement <> Me.EndUsingStatement) Then
				Dim usingBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.UsingBlock(usingStatement, statements, endUsingStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				usingBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, usingBlockSyntax1, usingBlockSyntax1.WithAnnotations(annotations))
			Else
				usingBlockSyntax = Me
			End If
			Return usingBlockSyntax
		End Function

		Public Function WithEndUsingStatement(ByVal endUsingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			Return Me.Update(Me.UsingStatement, Me.Statements, endUsingStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			Return Me.Update(Me.UsingStatement, statements, Me.EndUsingStatement)
		End Function

		Public Function WithUsingStatement(ByVal usingStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax
			Return Me.Update(usingStatement, Me.Statements, Me.EndUsingStatement)
		End Function
	End Class
End Namespace