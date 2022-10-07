Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class WhileOrUntilClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 1)
			End Get
		End Property

		Public ReadOnly Property WhileOrUntilKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)._whileOrUntilKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal whileOrUntilKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(kind, errors, annotations, whileOrUntilKeyword, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitWhileOrUntilClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitWhileOrUntilClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._condition
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			If (i <> 1) Then
				condition = Nothing
			Else
				condition = Me.Condition
			End If
			Return condition
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal whileOrUntilKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax
			If (kind <> MyBase.Kind() OrElse whileOrUntilKeyword <> Me.WhileOrUntilKeyword OrElse condition <> Me.Condition) Then
				Dim whileOrUntilClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.WhileOrUntilClause(kind, whileOrUntilKeyword, condition)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				whileOrUntilClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, whileOrUntilClauseSyntax1, whileOrUntilClauseSyntax1.WithAnnotations(annotations))
			Else
				whileOrUntilClauseSyntax = Me
			End If
			Return whileOrUntilClauseSyntax
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.WhileOrUntilKeyword, condition)
		End Function

		Public Function WithWhileOrUntilKeyword(ByVal whileOrUntilKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax
			Return Me.Update(MyBase.Kind(), whileOrUntilKeyword, Me.Condition)
		End Function
	End Class
End Namespace