Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class FunctionAggregationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax
		Friend _argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._argument, 2)
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)._closeParenToken
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(3), MyBase.GetChildIndex(3)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property FunctionName As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)._functionName, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)._openParenToken
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal functionName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax(kind, errors, annotations, functionName, openParenToken, If(argument IsNot Nothing, DirectCast(argument.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitFunctionAggregation(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitFunctionAggregation(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._argument
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim argument As SyntaxNode
			If (i <> 2) Then
				argument = Nothing
			Else
				argument = Me.Argument
			End If
			Return argument
		End Function

		Public Function Update(ByVal functionName As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			Dim functionAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			If (functionName <> Me.FunctionName OrElse openParenToken <> Me.OpenParenToken OrElse argument <> Me.Argument OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim functionAggregationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FunctionAggregation(functionName, openParenToken, argument, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				functionAggregationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, functionAggregationSyntax1, functionAggregationSyntax1.WithAnnotations(annotations))
			Else
				functionAggregationSyntax = Me
			End If
			Return functionAggregationSyntax
		End Function

		Public Function WithArgument(ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			Return Me.Update(Me.FunctionName, Me.OpenParenToken, argument, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			Return Me.Update(Me.FunctionName, Me.OpenParenToken, Me.Argument, closeParenToken)
		End Function

		Public Function WithFunctionName(ByVal functionName As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			Return Me.Update(functionName, Me.OpenParenToken, Me.Argument, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax
			Return Me.Update(Me.FunctionName, openParenToken, Me.Argument, Me.CloseParenToken)
		End Function
	End Class
End Namespace