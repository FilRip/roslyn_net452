Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class RelationalCaseClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax
		Friend _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property IsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)._isKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property OperatorToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)._operatorToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._value, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal isKeyword As KeywordSyntax, ByVal operatorToken As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(kind, errors, annotations, isKeyword, operatorToken, DirectCast(value.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitRelationalCaseClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitRelationalCaseClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._value
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim value As SyntaxNode
			If (i <> 2) Then
				value = Nothing
			Else
				value = Me.Value
			End If
			Return value
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal isKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax
			Dim relationalCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax
			If (kind <> MyBase.Kind() OrElse isKeyword <> Me.IsKeyword OrElse operatorToken <> Me.OperatorToken OrElse value <> Me.Value) Then
				Dim relationalCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.RelationalCaseClause(kind, isKeyword, operatorToken, value)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				relationalCaseClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, relationalCaseClauseSyntax1, relationalCaseClauseSyntax1.WithAnnotations(annotations))
			Else
				relationalCaseClauseSyntax = Me
			End If
			Return relationalCaseClauseSyntax
		End Function

		Public Function WithIsKeyword(ByVal isKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax
			Return Me.Update(MyBase.Kind(), isKeyword, Me.OperatorToken, Me.Value)
		End Function

		Public Function WithOperatorToken(ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.IsKeyword, operatorToken, Me.Value)
		End Function

		Public Function WithValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.IsKeyword, Me.OperatorToken, value)
		End Function
	End Class
End Namespace