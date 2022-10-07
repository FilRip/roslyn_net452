Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class WithBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax

		Friend _statements As SyntaxNode

		Friend _endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endWithStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property WithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax)(Me._withStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax, ByVal statements As SyntaxNode, ByVal endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax(kind, errors, annotations, DirectCast(withStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endWithStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitWithBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitWithBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._withStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endWithStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim withStatement As SyntaxNode
			Select Case i
				Case 0
					withStatement = Me.WithStatement
					Exit Select
				Case 1
					withStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					withStatement = Me.EndWithStatement
					Exit Select
				Case Else
					withStatement = Nothing
					Exit Select
			End Select
			Return withStatement
		End Function

		Public Function Update(ByVal withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			Dim withBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			If (withStatement <> Me.WithStatement OrElse statements <> Me.Statements OrElse endWithStatement <> Me.EndWithStatement) Then
				Dim withBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.WithBlock(withStatement, statements, endWithStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				withBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, withBlockSyntax1, withBlockSyntax1.WithAnnotations(annotations))
			Else
				withBlockSyntax = Me
			End If
			Return withBlockSyntax
		End Function

		Public Function WithEndWithStatement(ByVal endWithStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			Return Me.Update(Me.WithStatement, Me.Statements, endWithStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			Return Me.Update(Me.WithStatement, statements, Me.EndWithStatement)
		End Function

		Public Function WithWithStatement(ByVal withStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax
			Return Me.Update(withStatement, Me.Statements, Me.EndWithStatement)
		End Function
	End Class
End Namespace