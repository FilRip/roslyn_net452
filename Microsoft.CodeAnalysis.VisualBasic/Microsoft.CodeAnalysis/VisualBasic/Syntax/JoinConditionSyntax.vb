Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class JoinConditionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _left As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _right As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property EqualsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)._equalsKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Left As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._left)
			End Get
		End Property

		Public ReadOnly Property Right As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._right, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal equalsKeyword As KeywordSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax(kind, errors, annotations, DirectCast(left.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), equalsKeyword, DirectCast(right.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitJoinCondition(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitJoinCondition(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._left
			ElseIf (num = 2) Then
				syntaxNode = Me._right
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim left As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				left = Me.Left
			ElseIf (num = 2) Then
				left = Me.Right
			Else
				left = Nothing
			End If
			Return left
		End Function

		Public Function Update(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal equalsKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax
			Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax
			If (left <> Me.Left OrElse equalsKeyword <> Me.EqualsKeyword OrElse right <> Me.Right) Then
				Dim joinConditionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.JoinCondition(left, equalsKeyword, right)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				joinConditionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, joinConditionSyntax1, joinConditionSyntax1.WithAnnotations(annotations))
			Else
				joinConditionSyntax = Me
			End If
			Return joinConditionSyntax
		End Function

		Public Function WithEqualsKeyword(ByVal equalsKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax
			Return Me.Update(Me.Left, equalsKeyword, Me.Right)
		End Function

		Public Function WithLeft(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax
			Return Me.Update(left, Me.EqualsKeyword, Me.Right)
		End Function

		Public Function WithRight(ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.JoinConditionSyntax
			Return Me.Update(Me.Left, Me.EqualsKeyword, right)
		End Function
	End Class
End Namespace