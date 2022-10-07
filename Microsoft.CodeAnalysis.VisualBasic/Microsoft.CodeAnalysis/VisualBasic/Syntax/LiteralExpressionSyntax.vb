Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class LiteralExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Public ReadOnly Property Token As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)._token, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(kind, errors, annotations, token), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitLiteralExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitLiteralExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax
			If (kind <> MyBase.Kind() OrElse token <> Me.Token) Then
				Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.LiteralExpression(kind, token)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				literalExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, literalExpressionSyntax1, literalExpressionSyntax1.WithAnnotations(annotations))
			Else
				literalExpressionSyntax = Me
			End If
			Return literalExpressionSyntax
		End Function

		Public Function WithToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax
			Return Me.Update(MyBase.Kind(), token)
		End Function
	End Class
End Namespace