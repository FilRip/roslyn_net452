Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CatchBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax

		Friend _statements As SyntaxNode

		Public ReadOnly Property CatchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax)(Me._catchStatement)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax(kind, errors, annotations, DirectCast(catchStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCatchBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCatchBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._catchStatement
			ElseIf (num = 1) Then
				syntaxNode = Me._statements
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim catchStatement As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				catchStatement = Me.CatchStatement
			ElseIf (num = 1) Then
				catchStatement = MyBase.GetRed(Me._statements, 1)
			Else
				catchStatement = Nothing
			End If
			Return catchStatement
		End Function

		Public Function Update(ByVal catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax
			Dim catchBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax
			If (catchStatement <> Me.CatchStatement OrElse statements <> Me.Statements) Then
				Dim catchBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CatchBlock(catchStatement, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				catchBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, catchBlockSyntax1, catchBlockSyntax1.WithAnnotations(annotations))
			Else
				catchBlockSyntax = Me
			End If
			Return catchBlockSyntax
		End Function

		Public Function WithCatchStatement(ByVal catchStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax
			Return Me.Update(catchStatement, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax
			Return Me.Update(Me.CatchStatement, statements)
		End Function
	End Class
End Namespace