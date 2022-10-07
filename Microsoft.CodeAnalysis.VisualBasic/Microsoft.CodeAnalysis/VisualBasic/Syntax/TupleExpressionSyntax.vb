Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TupleExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _arguments As SyntaxNode

		Public ReadOnly Property Arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)
			Get
				Dim simpleArgumentSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._arguments, 1)
				simpleArgumentSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)(red, MyBase.GetChildIndex(1)))
				Return simpleArgumentSyntaxes
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal arguments As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(kind, errors, annotations, openParenToken, If(arguments IsNot Nothing, arguments.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTupleExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTupleExpression(Me)
		End Sub

		Public Function AddArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			Return Me.WithArguments(Me.Arguments.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._arguments
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._arguments, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			Dim tupleExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			If (openParenToken <> Me.OpenParenToken OrElse arguments <> Me.Arguments OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim tupleExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TupleExpression(openParenToken, arguments, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				tupleExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, tupleExpressionSyntax1, tupleExpressionSyntax1.WithAnnotations(annotations))
			Else
				tupleExpressionSyntax = Me
			End If
			Return tupleExpressionSyntax
		End Function

		Public Function WithArguments(ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			Return Me.Update(Me.OpenParenToken, arguments, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			Return Me.Update(Me.OpenParenToken, Me.Arguments, closeParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleExpressionSyntax
			Return Me.Update(openParenToken, Me.Arguments, Me.CloseParenToken)
		End Function
	End Class
End Namespace