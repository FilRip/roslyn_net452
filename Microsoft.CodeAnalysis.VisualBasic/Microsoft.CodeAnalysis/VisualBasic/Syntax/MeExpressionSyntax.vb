Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MeExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InstanceExpressionSyntax
		Public Shadows ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax)._keyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax(kind, errors, annotations, keyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMeExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMeExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.Keyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax
			Dim meExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax
			If (keyword = Me.Keyword) Then
				meExpressionSyntax = Me
			Else
				Dim meExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MeExpression(keyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				meExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, meExpressionSyntax1, meExpressionSyntax1.WithAnnotations(annotations))
			End If
			Return meExpressionSyntax
		End Function

		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax
			Return Me.Update(keyword)
		End Function

		Friend Overrides Function WithKeywordCore(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InstanceExpressionSyntax
			Return Me.WithKeyword(keyword)
		End Function
	End Class
End Namespace