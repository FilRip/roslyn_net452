Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InferredFieldInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public Shadows ReadOnly Property KeyKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax)._keyKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax(kind, errors, annotations, keyKeyword, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInferredFieldInitializer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInferredFieldInitializer(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._expression
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetKeyKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.KeyKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			If (i <> 1) Then
				expression = Nothing
			Else
				expression = Me.Expression
			End If
			Return expression
		End Function

		Public Function Update(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax
			Dim inferredFieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax
			If (keyKeyword <> Me.KeyKeyword OrElse expression <> Me.Expression) Then
				Dim inferredFieldInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InferredFieldInitializer(keyKeyword, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				inferredFieldInitializerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, inferredFieldInitializerSyntax1, inferredFieldInitializerSyntax1.WithAnnotations(annotations))
			Else
				inferredFieldInitializerSyntax = Me
			End If
			Return inferredFieldInitializerSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax
			Return Me.Update(Me.KeyKeyword, expression)
		End Function

		Public Shadows Function WithKeyKeyword(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax
			Return Me.Update(keyKeyword, Me.Expression)
		End Function

		Friend Overrides Function WithKeyKeywordCore(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
			Return Me.WithKeyKeyword(keyKeyword)
		End Function
	End Class
End Namespace