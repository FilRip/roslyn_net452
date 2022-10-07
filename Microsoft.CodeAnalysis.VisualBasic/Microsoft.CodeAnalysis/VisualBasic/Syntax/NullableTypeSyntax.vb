Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NullableTypeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
		Friend _elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public ReadOnly Property ElementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._elementType)
			End Get
		End Property

		Public ReadOnly Property QuestionMarkToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax)._questionMarkToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal questionMarkToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax(kind, errors, annotations, DirectCast(elementType.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), questionMarkToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNullableType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNullableType(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._elementType
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim elementType As SyntaxNode
			If (i <> 0) Then
				elementType = Nothing
			Else
				elementType = Me.ElementType
			End If
			Return elementType
		End Function

		Public Function Update(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal questionMarkToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax
			Dim nullableTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax
			If (elementType <> Me.ElementType OrElse questionMarkToken <> Me.QuestionMarkToken) Then
				Dim nullableTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NullableType(elementType, questionMarkToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				nullableTypeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, nullableTypeSyntax1, nullableTypeSyntax1.WithAnnotations(annotations))
			Else
				nullableTypeSyntax = Me
			End If
			Return nullableTypeSyntax
		End Function

		Public Function WithElementType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax
			Return Me.Update(elementType, Me.QuestionMarkToken)
		End Function

		Public Function WithQuestionMarkToken(ByVal questionMarkToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax
			Return Me.Update(Me.ElementType, questionMarkToken)
		End Function
	End Class
End Namespace