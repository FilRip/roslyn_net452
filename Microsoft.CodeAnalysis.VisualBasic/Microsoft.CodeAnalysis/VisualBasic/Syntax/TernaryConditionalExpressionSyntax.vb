Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TernaryConditionalExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)._closeParenToken, Me.GetChildPosition(7), MyBase.GetChildIndex(7))
			End Get
		End Property

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 2)
			End Get
		End Property

		Public ReadOnly Property FirstCommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)._firstCommaToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property IfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)._ifKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property SecondCommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)._secondCommaToken, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public ReadOnly Property WhenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._whenFalse, 6)
			End Get
		End Property

		Public ReadOnly Property WhenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._whenTrue, 4)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(kind, errors, annotations, ifKeyword, openParenToken, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), firstCommaToken, DirectCast(whenTrue.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), secondCommaToken, DirectCast(whenFalse.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTernaryConditionalExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTernaryConditionalExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 2
					syntaxNode = Me._condition
					Exit Select
				Case 3
				Case 5
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 4
					syntaxNode = Me._whenTrue
					Exit Select
				Case 6
					syntaxNode = Me._whenFalse
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			Select Case i
				Case 2
					condition = Me.Condition
					Exit Select
				Case 3
				Case 5
				Label0:
					condition = Nothing
					Exit Select
				Case 4
					condition = Me.WhenTrue
					Exit Select
				Case 6
					condition = Me.WhenFalse
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return condition
		End Function

		Public Function Update(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal firstCommaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal secondCommaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Dim ternaryConditionalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			If (ifKeyword <> Me.IfKeyword OrElse openParenToken <> Me.OpenParenToken OrElse condition <> Me.Condition OrElse firstCommaToken <> Me.FirstCommaToken OrElse whenTrue <> Me.WhenTrue OrElse secondCommaToken <> Me.SecondCommaToken OrElse whenFalse <> Me.WhenFalse OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim ternaryConditionalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TernaryConditionalExpression(ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				ternaryConditionalExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, ternaryConditionalExpressionSyntax1, ternaryConditionalExpressionSyntax1.WithAnnotations(annotations))
			Else
				ternaryConditionalExpressionSyntax = Me
			End If
			Return ternaryConditionalExpressionSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.Condition, Me.FirstCommaToken, Me.WhenTrue, Me.SecondCommaToken, Me.WhenFalse, closeParenToken)
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, condition, Me.FirstCommaToken, Me.WhenTrue, Me.SecondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function

		Public Function WithFirstCommaToken(ByVal firstCommaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.Condition, firstCommaToken, Me.WhenTrue, Me.SecondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function

		Public Function WithIfKeyword(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(ifKeyword, Me.OpenParenToken, Me.Condition, Me.FirstCommaToken, Me.WhenTrue, Me.SecondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, openParenToken, Me.Condition, Me.FirstCommaToken, Me.WhenTrue, Me.SecondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function

		Public Function WithSecondCommaToken(ByVal secondCommaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.Condition, Me.FirstCommaToken, Me.WhenTrue, secondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function

		Public Function WithWhenFalse(ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.Condition, Me.FirstCommaToken, Me.WhenTrue, Me.SecondCommaToken, whenFalse, Me.CloseParenToken)
		End Function

		Public Function WithWhenTrue(ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.Condition, Me.FirstCommaToken, whenTrue, Me.SecondCommaToken, Me.WhenFalse, Me.CloseParenToken)
		End Function
	End Class
End Namespace