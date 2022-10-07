Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class QueryExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _clauses As SyntaxNode

		Public ReadOnly Property Clauses As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)(MyBase.GetRedAtZero(Me._clauses))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal clauses As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax(kind, errors, annotations, If(clauses IsNot Nothing, clauses.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitQueryExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitQueryExpression(Me)
		End Sub

		Public Function AddClauses(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax
			Return Me.WithClauses(Me.Clauses.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._clauses
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			If (i <> 0) Then
				redAtZero = Nothing
			Else
				redAtZero = MyBase.GetRedAtZero(Me._clauses)
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal clauses As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax
			Dim queryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax
			If (clauses = Me.Clauses) Then
				queryExpressionSyntax = Me
			Else
				Dim queryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.QueryExpression(clauses)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				queryExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, queryExpressionSyntax1, queryExpressionSyntax1.WithAnnotations(annotations))
			End If
			Return queryExpressionSyntax
		End Function

		Public Function WithClauses(ByVal clauses As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax
			Return Me.Update(clauses)
		End Function
	End Class
End Namespace