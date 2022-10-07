Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MyBaseExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InstanceExpressionSyntax
		Public Shadows ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax)._keyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax(kind, errors, annotations, keyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMyBaseExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMyBaseExpression(Me)
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

		Public Function Update(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax
			Dim myBaseExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax
			If (keyword = Me.Keyword) Then
				myBaseExpressionSyntax = Me
			Else
				Dim myBaseExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MyBaseExpression(keyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				myBaseExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, myBaseExpressionSyntax1, myBaseExpressionSyntax1.WithAnnotations(annotations))
			End If
			Return myBaseExpressionSyntax
		End Function

		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax
			Return Me.Update(keyword)
		End Function

		Friend Overrides Function WithKeywordCore(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InstanceExpressionSyntax
			Return Me.WithKeyword(keyword)
		End Function
	End Class
End Namespace