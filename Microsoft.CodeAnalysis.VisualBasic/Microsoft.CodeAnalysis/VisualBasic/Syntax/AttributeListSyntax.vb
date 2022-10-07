Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AttributeListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _attributes As SyntaxNode

		Public ReadOnly Property Attributes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
			Get
				Dim attributeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._attributes, 1)
				attributeSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)(red, MyBase.GetChildIndex(1)))
				Return attributeSyntaxes
			End Get
		End Property

		Public ReadOnly Property GreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)._greaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property LessThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)._lessThanToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal attributes As SyntaxNode, ByVal greaterThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax(kind, errors, annotations, lessThanToken, If(attributes IsNot Nothing, attributes.Green, Nothing), greaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAttributeList(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAttributeList(Me)
		End Sub

		Public Function AddAttributes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			Return Me.WithAttributes(Me.Attributes.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._attributes
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._attributes, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal attributes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax), ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			Dim attributeListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			If (lessThanToken <> Me.LessThanToken OrElse attributes <> Me.Attributes OrElse greaterThanToken <> Me.GreaterThanToken) Then
				Dim attributeListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AttributeList(lessThanToken, attributes, greaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				attributeListSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, attributeListSyntax1, attributeListSyntax1.WithAnnotations(annotations))
			Else
				attributeListSyntax = Me
			End If
			Return attributeListSyntax
		End Function

		Public Function WithAttributes(ByVal attributes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			Return Me.Update(Me.LessThanToken, attributes, Me.GreaterThanToken)
		End Function

		Public Function WithGreaterThanToken(ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			Return Me.Update(Me.LessThanToken, Me.Attributes, greaterThanToken)
		End Function

		Public Function WithLessThanToken(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax
			Return Me.Update(lessThanToken, Me.Attributes, Me.GreaterThanToken)
		End Function
	End Class
End Namespace