Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlDeclarationOptionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax

		Public ReadOnly Property Equals As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)._equals, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)._name, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax)(Me._value, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(kind, errors, annotations, name, equals, DirectCast(value.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlDeclarationOption(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlDeclarationOption(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._value
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim value As SyntaxNode
			If (i <> 2) Then
				value = Nothing
			Else
				value = Me.Value
			End If
			Return value
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.SyntaxToken, ByVal equals As Microsoft.CodeAnalysis.SyntaxToken, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			If (name <> Me.Name OrElse equals <> Me.Equals OrElse value <> Me.Value) Then
				Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlDeclarationOption(name, equals, value)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlDeclarationOptionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlDeclarationOptionSyntax1, xmlDeclarationOptionSyntax1.WithAnnotations(annotations))
			Else
				xmlDeclarationOptionSyntax = Me
			End If
			Return xmlDeclarationOptionSyntax
		End Function

		Public Function WithEquals(ByVal equals As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Return Me.Update(Me.Name, equals, Me.Value)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Return Me.Update(name, Me.Equals, Me.Value)
		End Function

		Public Function WithValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax
			Return Me.Update(Me.Name, Me.Equals, value)
		End Function
	End Class
End Namespace