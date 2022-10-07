Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EqualsValueSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)._equalsToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._value, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax(kind, errors, annotations, equalsToken, DirectCast(value.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEqualsValue(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEqualsValue(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._value
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim value As SyntaxNode
			If (i <> 1) Then
				value = Nothing
			Else
				value = Me.Value
			End If
			Return value
		End Function

		Public Function Update(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			If (equalsToken <> Me.EqualsToken OrElse value <> Me.Value) Then
				Dim equalsValueSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EqualsValue(equalsToken, value)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				equalsValueSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, equalsValueSyntax1, equalsValueSyntax1.WithAnnotations(annotations))
			Else
				equalsValueSyntax = Me
			End If
			Return equalsValueSyntax
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Return Me.Update(equalsToken, Me.Value)
		End Function

		Public Function WithValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Return Me.Update(Me.EqualsToken, value)
		End Function
	End Class
End Namespace