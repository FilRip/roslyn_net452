Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlAttributeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Friend _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)._equalsToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._name)
			End Get
		End Property

		Public ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._value, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal equalsToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(kind, errors, annotations, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax), equalsToken, DirectCast(value.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlAttribute(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlAttribute(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._name
			ElseIf (num = 2) Then
				syntaxNode = Me._value
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				name = Me.Name
			ElseIf (num = 2) Then
				name = Me.Value
			Else
				name = Nothing
			End If
			Return name
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			If (name <> Me.Name OrElse equalsToken <> Me.EqualsToken OrElse value <> Me.Value) Then
				Dim xmlAttributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlAttribute(name, equalsToken, value)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlAttributeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlAttributeSyntax1, xmlAttributeSyntax1.WithAnnotations(annotations))
			Else
				xmlAttributeSyntax = Me
			End If
			Return xmlAttributeSyntax
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			Return Me.Update(Me.Name, equalsToken, Me.Value)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			Return Me.Update(name, Me.EqualsToken, Me.Value)
		End Function

		Public Function WithValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			Return Me.Update(Me.Name, Me.EqualsToken, value)
		End Function
	End Class
End Namespace