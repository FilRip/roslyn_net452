Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DocumentationCommentTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
		Friend _content As SyntaxNode

		Public ReadOnly Property Content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRedAtZero(Me._content))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal content As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax(kind, errors, annotations, If(content IsNot Nothing, content.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDocumentationCommentTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDocumentationCommentTrivia(Me)
		End Sub

		Public Function AddContent(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
			Return Me.WithContent(Me.Content.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._content
			End If
			Return syntaxNode
		End Function

		Friend Function GetInteriorXml() As String
			Return DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax).GetInteriorXml()
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			If (i <> 0) Then
				redAtZero = Nothing
			Else
				redAtZero = MyBase.GetRedAtZero(Me._content)
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
			Dim documentationCommentTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
			If (content = Me.Content) Then
				documentationCommentTriviaSyntax = Me
			Else
				Dim documentationCommentTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DocumentationCommentTrivia(content)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				documentationCommentTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, documentationCommentTriviaSyntax1, documentationCommentTriviaSyntax1.WithAnnotations(annotations))
			End If
			Return documentationCommentTriviaSyntax
		End Function

		Public Function WithContent(ByVal content As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax
			Return Me.Update(content)
		End Function
	End Class
End Namespace