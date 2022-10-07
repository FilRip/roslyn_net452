Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class RangeArgumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax
		Friend _lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public NotOverridable Overrides ReadOnly Property IsNamed As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property LowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._lowerBound)
			End Get
		End Property

		Public ReadOnly Property ToKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax)._toKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
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
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax(kind, errors, annotations, DirectCast(lowerBound.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), toKeyword, DirectCast(upperBound.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitRangeArgument(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitRangeArgument(Me)
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

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public NotOverridable Overrides Function GetExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Return Me.UpperBound
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

		Public Function Update(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax
			Dim rangeArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax
			If (lowerBound <> Me.LowerBound OrElse toKeyword <> Me.ToKeyword OrElse upperBound <> Me.UpperBound) Then
				Dim rangeArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.RangeArgument(lowerBound, toKeyword, upperBound)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				rangeArgumentSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, rangeArgumentSyntax1, rangeArgumentSyntax1.WithAnnotations(annotations))
			Else
				rangeArgumentSyntax = Me
			End If
			Return rangeArgumentSyntax
		End Function

		Public Function WithLowerBound(ByVal lowerBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax
			Return Me.Update(lowerBound, Me.ToKeyword, Me.UpperBound)
		End Function

		Public Function WithToKeyword(ByVal toKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax
			Return Me.Update(Me.LowerBound, toKeyword, Me.UpperBound)
		End Function

		Public Function WithUpperBound(ByVal upperBound As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RangeArgumentSyntax
			Return Me.Update(Me.LowerBound, Me.ToKeyword, upperBound)
		End Function
	End Class
End Namespace