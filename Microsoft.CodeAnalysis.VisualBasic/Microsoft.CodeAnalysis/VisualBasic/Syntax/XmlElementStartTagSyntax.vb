Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlElementStartTagSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Friend _attributes As SyntaxNode

		Public ReadOnly Property Attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRed(Me._attributes, 2))
			End Get
		End Property

		Public ReadOnly Property GreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)._greaterThanToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property LessThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)._lessThanToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._name, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal attributes As SyntaxNode, ByVal greaterThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(kind, errors, annotations, lessThanToken, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax), If(attributes IsNot Nothing, attributes.Green, Nothing), greaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlElementStartTag(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlElementStartTag(Me)
		End Sub

		Public Function AddAttributes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Return Me.WithAttributes(Me.Attributes.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._name
			ElseIf (num = 2) Then
				syntaxNode = Me._attributes
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				name = Me.Name
			ElseIf (num = 2) Then
				name = MyBase.GetRed(Me._attributes, 2)
			Else
				name = Nothing
			End If
			Return name
		End Function

		Public Function Update(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax), ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			If (lessThanToken <> Me.LessThanToken OrElse name <> Me.Name OrElse attributes <> Me.Attributes OrElse greaterThanToken <> Me.GreaterThanToken) Then
				Dim xmlElementStartTagSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlElementStartTag(lessThanToken, name, attributes, greaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlElementStartTagSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlElementStartTagSyntax1, xmlElementStartTagSyntax1.WithAnnotations(annotations))
			Else
				xmlElementStartTagSyntax = Me
			End If
			Return xmlElementStartTagSyntax
		End Function

		Public Function WithAttributes(ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Return Me.Update(Me.LessThanToken, Me.Name, attributes, Me.GreaterThanToken)
		End Function

		Public Function WithGreaterThanToken(ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Return Me.Update(Me.LessThanToken, Me.Name, Me.Attributes, greaterThanToken)
		End Function

		Public Function WithLessThanToken(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Return Me.Update(lessThanToken, Me.Name, Me.Attributes, Me.GreaterThanToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax
			Return Me.Update(Me.LessThanToken, name, Me.Attributes, Me.GreaterThanToken)
		End Function
	End Class
End Namespace