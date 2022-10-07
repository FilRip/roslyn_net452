Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CatchFilterClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._filter, 1)
			End Get
		End Property

		Public ReadOnly Property WhenKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)._whenKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal whenKeyword As KeywordSyntax, ByVal filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax(kind, errors, annotations, whenKeyword, DirectCast(filter.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCatchFilterClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCatchFilterClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._filter
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim filter As SyntaxNode
			If (i <> 1) Then
				filter = Nothing
			Else
				filter = Me.Filter
			End If
			Return filter
		End Function

		Public Function Update(ByVal whenKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax
			If (whenKeyword <> Me.WhenKeyword OrElse filter <> Me.Filter) Then
				Dim catchFilterClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CatchFilterClause(whenKeyword, filter)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				catchFilterClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, catchFilterClauseSyntax1, catchFilterClauseSyntax1.WithAnnotations(annotations))
			Else
				catchFilterClauseSyntax = Me
			End If
			Return catchFilterClauseSyntax
		End Function

		Public Function WithFilter(ByVal filter As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax
			Return Me.Update(Me.WhenKeyword, filter)
		End Function

		Public Function WithWhenKeyword(ByVal whenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax
			Return Me.Update(whenKeyword, Me.Filter)
		End Function
	End Class
End Namespace