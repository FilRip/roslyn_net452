Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax

		Public ReadOnly Property LocalName As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)._localName, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax)(Me._prefix)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(kind, errors, annotations, If(prefix IsNot Nothing, DirectCast(prefix.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax), Nothing), localName), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlName(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._prefix
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim prefix As SyntaxNode
			If (i <> 0) Then
				prefix = Nothing
			Else
				prefix = Me.Prefix
			End If
			Return prefix
		End Function

		Public Function Update(ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax, ByVal localName As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			If (prefix <> Me.Prefix OrElse localName <> Me.LocalName) Then
				Dim xmlNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlName(prefix, localName)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlNameSyntax1, xmlNameSyntax1.WithAnnotations(annotations))
			Else
				xmlNameSyntax = Me
			End If
			Return xmlNameSyntax
		End Function

		Public Function WithLocalName(ByVal localName As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			Return Me.Update(Me.Prefix, localName)
		End Function

		Public Function WithPrefix(ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			Return Me.Update(prefix, Me.LocalName)
		End Function
	End Class
End Namespace