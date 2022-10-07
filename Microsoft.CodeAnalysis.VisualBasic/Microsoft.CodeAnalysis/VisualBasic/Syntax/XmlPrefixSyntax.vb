Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlPrefixSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property ColonToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)._colonToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)._name, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As XmlNameTokenSyntax, ByVal colonToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(kind, errors, annotations, name, colonToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlPrefix(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlPrefix(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.SyntaxToken, ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax
			If (name <> Me.Name OrElse colonToken <> Me.ColonToken) Then
				Dim xmlPrefixSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlPrefix(name, colonToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlPrefixSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlPrefixSyntax1, xmlPrefixSyntax1.WithAnnotations(annotations))
			Else
				xmlPrefixSyntax = Me
			End If
			Return xmlPrefixSyntax
		End Function

		Public Function WithColonToken(ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax
			Return Me.Update(Me.Name, colonToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixSyntax
			Return Me.Update(name, Me.ColonToken)
		End Function
	End Class
End Namespace