Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EraseStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _expressions As SyntaxNode

		Public ReadOnly Property EraseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax)._eraseKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Expressions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			Get
				Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._expressions, 1)
				expressionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(red, MyBase.GetChildIndex(1)))
				Return expressionSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal eraseKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal expressions As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax(kind, errors, annotations, eraseKeyword, If(expressions IsNot Nothing, expressions.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEraseStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEraseStatement(Me)
		End Sub

		Public Function AddExpressions(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax
			Return Me.WithExpressions(Me.Expressions.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._expressions
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._expressions, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal eraseKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expressions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax
			Dim eraseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax
			If (eraseKeyword <> Me.EraseKeyword OrElse expressions <> Me.Expressions) Then
				Dim eraseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EraseStatement(eraseKeyword, expressions)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				eraseStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, eraseStatementSyntax1, eraseStatementSyntax1.WithAnnotations(annotations))
			Else
				eraseStatementSyntax = Me
			End If
			Return eraseStatementSyntax
		End Function

		Public Function WithEraseKeyword(ByVal eraseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax
			Return Me.Update(eraseKeyword, Me.Expressions)
		End Function

		Public Function WithExpressions(ByVal expressions As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax
			Return Me.Update(Me.EraseKeyword, expressions)
		End Function
	End Class
End Namespace