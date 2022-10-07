Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class RangeCaseClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax
		Friend _lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property LowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._lowerBound)
			End Get
		End Property

		Public ReadOnly Property ToKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax)._toKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property UpperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._upperBound, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal toKeyword As KeywordSyntax, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax(kind, errors, annotations, DirectCast(lowerBound.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), toKeyword, DirectCast(upperBound.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitRangeCaseClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitRangeCaseClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._lowerBound
			ElseIf (num = 2) Then
				syntaxNode = Me._upperBound
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim lowerBound As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				lowerBound = Me.LowerBound
			ElseIf (num = 2) Then
				lowerBound = Me.UpperBound
			Else
				lowerBound = Nothing
			End If
			Return lowerBound
		End Function

		Public Function Update(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax
			Dim rangeCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax
			If (lowerBound <> Me.LowerBound OrElse toKeyword <> Me.ToKeyword OrElse upperBound <> Me.UpperBound) Then
				Dim rangeCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.RangeCaseClause(lowerBound, toKeyword, upperBound)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				rangeCaseClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, rangeCaseClauseSyntax1, rangeCaseClauseSyntax1.WithAnnotations(annotations))
			Else
				rangeCaseClauseSyntax = Me
			End If
			Return rangeCaseClauseSyntax
		End Function

		Public Function WithLowerBound(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax
			Return Me.Update(lowerBound, Me.ToKeyword, Me.UpperBound)
		End Function

		Public Function WithToKeyword(ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax
			Return Me.Update(Me.LowerBound, toKeyword, Me.UpperBound)
		End Function

		Public Function WithUpperBound(ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeCaseClauseSyntax
			Return Me.Update(Me.LowerBound, Me.ToKeyword, upperBound)
		End Function
	End Class
End Namespace