Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AsNewClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
		Friend _newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax

		Public Shadows ReadOnly Property AsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax)._asKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property NewExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax)(Me._newExpression, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax, ByVal newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax(kind, errors, annotations, asKeyword, DirectCast(newExpression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAsNewClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAsNewClause(Me)
		End Sub

		Friend Overrides Function GetAsKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.AsKeyword
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._newExpression
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim newExpression As SyntaxNode
			If (i <> 1) Then
				newExpression = Nothing
			Else
				newExpression = Me.NewExpression
			End If
			Return newExpression
		End Function

		Public Function Update(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax
			Dim asNewClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax
			If (asKeyword <> Me.AsKeyword OrElse newExpression <> Me.NewExpression) Then
				Dim asNewClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AsNewClause(asKeyword, newExpression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				asNewClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, asNewClauseSyntax1, asNewClauseSyntax1.WithAnnotations(annotations))
			Else
				asNewClauseSyntax = Me
			End If
			Return asNewClauseSyntax
		End Function

		Public Shadows Function WithAsKeyword(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax
			Return Me.Update(asKeyword, Me.NewExpression)
		End Function

		Friend Overrides Function WithAsKeywordCore(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Return Me.WithAsKeyword(asKeyword)
		End Function

		Public Function WithNewExpression(ByVal newExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax
			Return Me.Update(Me.AsKeyword, newExpression)
		End Function
	End Class
End Namespace