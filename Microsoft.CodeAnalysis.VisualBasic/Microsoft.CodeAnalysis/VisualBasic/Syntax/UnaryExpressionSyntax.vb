Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class UnaryExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._operand, 1)
			End Get
		End Property

		Public ReadOnly Property OperatorToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax)._operatorToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax(kind, errors, annotations, operatorToken, DirectCast(operand.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitUnaryExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitUnaryExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._operand
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim operand As SyntaxNode
			If (i <> 1) Then
				operand = Nothing
			Else
				operand = Me.Operand
			End If
			Return operand
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax
			Dim unaryExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax
			If (kind <> MyBase.Kind() OrElse operatorToken <> Me.OperatorToken OrElse operand <> Me.Operand) Then
				Dim unaryExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.UnaryExpression(kind, operatorToken, operand)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				unaryExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, unaryExpressionSyntax1, unaryExpressionSyntax1.WithAnnotations(annotations))
			Else
				unaryExpressionSyntax = Me
			End If
			Return unaryExpressionSyntax
		End Function

		Public Function WithOperand(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.OperatorToken, operand)
		End Function

		Public Function WithOperatorToken(ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax
			Return Me.Update(MyBase.Kind(), operatorToken, Me.Operand)
		End Function
	End Class
End Namespace