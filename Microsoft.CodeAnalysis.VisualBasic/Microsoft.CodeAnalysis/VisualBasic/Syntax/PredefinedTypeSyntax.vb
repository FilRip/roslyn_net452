Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PredefinedTypeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
		Public ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)._keyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax(kind, errors, annotations, keyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPredefinedType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPredefinedType(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax
			Dim predefinedTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax
			If (keyword = Me.Keyword) Then
				predefinedTypeSyntax = Me
			Else
				Dim predefinedTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PredefinedType(keyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				predefinedTypeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, predefinedTypeSyntax1, predefinedTypeSyntax1.WithAnnotations(annotations))
			End If
			Return predefinedTypeSyntax
		End Function

		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedTypeSyntax
			Return Me.Update(keyword)
		End Function
	End Class
End Namespace