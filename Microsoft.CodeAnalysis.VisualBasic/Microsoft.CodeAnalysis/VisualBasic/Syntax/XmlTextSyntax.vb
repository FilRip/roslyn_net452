Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlTextSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property TextTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax)._textTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, MyBase.Position, 0))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal textTokens As GreenNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax(kind, errors, annotations, textTokens), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlText(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlText(Me)
		End Sub

		Public Function AddTextTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax
			Return Me.WithTextTokens(Me.TextTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax
			Dim xmlTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax
			If (textTokens = Me.TextTokens) Then
				xmlTextSyntax = Me
			Else
				Dim xmlTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlText(textTokens)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlTextSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlTextSyntax1, xmlTextSyntax1.WithAnnotations(annotations))
			End If
			Return xmlTextSyntax
		End Function

		Public Function WithTextTokens(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlTextSyntax
			Return Me.Update(textTokens)
		End Function
	End Class
End Namespace