Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlEmptyElementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Friend _attributes As SyntaxNode

		Public ReadOnly Property Attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRed(Me._attributes, 2))
			End Get
		End Property

		Public ReadOnly Property LessThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax)._lessThanToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._name, 1)
			End Get
		End Property

		Public ReadOnly Property SlashGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax)._slashGreaterThanToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal attributes As SyntaxNode, ByVal slashGreaterThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax(kind, errors, annotations, lessThanToken, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax), If(attributes IsNot Nothing, attributes.Green, Nothing), slashGreaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlEmptyElement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlEmptyElement(Me)
		End Sub

		Public Function AddAttributes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
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

		Public Function Update(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax), ByVal slashGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			Dim xmlEmptyElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			If (lessThanToken <> Me.LessThanToken OrElse name <> Me.Name OrElse attributes <> Me.Attributes OrElse slashGreaterThanToken <> Me.SlashGreaterThanToken) Then
				Dim xmlEmptyElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlEmptyElement(lessThanToken, name, attributes, slashGreaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlEmptyElementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlEmptyElementSyntax1, xmlEmptyElementSyntax1.WithAnnotations(annotations))
			Else
				xmlEmptyElementSyntax = Me
			End If
			Return xmlEmptyElementSyntax
		End Function

		Public Function WithAttributes(ByVal attributes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			Return Me.Update(Me.LessThanToken, Me.Name, attributes, Me.SlashGreaterThanToken)
		End Function

		Public Function WithLessThanToken(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			Return Me.Update(lessThanToken, Me.Name, Me.Attributes, Me.SlashGreaterThanToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			Return Me.Update(Me.LessThanToken, name, Me.Attributes, Me.SlashGreaterThanToken)
		End Function

		Public Function WithSlashGreaterThanToken(ByVal slashGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmptyElementSyntax
			Return Me.Update(Me.LessThanToken, Me.Name, Me.Attributes, slashGreaterThanToken)
		End Function
	End Class
End Namespace