Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class KeywordEventContainerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax
		Public ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax)._keyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax(kind, errors, annotations, keyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitKeywordEventContainer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitKeywordEventContainer(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax
			Dim keywordEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax
			If (keyword = Me.Keyword) Then
				keywordEventContainerSyntax = Me
			Else
				Dim keywordEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.KeywordEventContainer(keyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				keywordEventContainerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, keywordEventContainerSyntax1, keywordEventContainerSyntax1.WithAnnotations(annotations))
			End If
			Return keywordEventContainerSyntax
		End Function

		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax
			Return Me.Update(keyword)
		End Function
	End Class
End Namespace