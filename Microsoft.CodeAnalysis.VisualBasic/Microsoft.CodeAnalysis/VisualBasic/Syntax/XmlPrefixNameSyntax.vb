Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlPrefixNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property Name As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)._name, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As XmlNameTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(kind, errors, annotations, name), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlPrefixName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlPrefixName(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax
			If (name = Me.Name) Then
				xmlPrefixNameSyntax = Me
			Else
				Dim xmlPrefixNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlPrefixName(name)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlPrefixNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlPrefixNameSyntax1, xmlPrefixNameSyntax1.WithAnnotations(annotations))
			End If
			Return xmlPrefixNameSyntax
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax
			Return Me.Update(name)
		End Function
	End Class
End Namespace