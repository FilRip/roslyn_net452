Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InterpolatedStringExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _contents As SyntaxNode

		Public ReadOnly Property Contents As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax)(MyBase.GetRed(Me._contents, 1))
			End Get
		End Property

		Public ReadOnly Property DollarSignDoubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)._dollarSignDoubleQuoteToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property DoubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)._doubleQuoteToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal dollarSignDoubleQuoteToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal contents As SyntaxNode, ByVal doubleQuoteToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(kind, errors, annotations, dollarSignDoubleQuoteToken, If(contents IsNot Nothing, contents.Green, Nothing), doubleQuoteToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInterpolatedStringExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInterpolatedStringExpression(Me)
		End Sub

		Public Function AddContents(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			Return Me.WithContents(Me.Contents.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._contents
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._contents, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal dollarSignDoubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal contents As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax), ByVal doubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			Dim interpolatedStringExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			If (dollarSignDoubleQuoteToken <> Me.DollarSignDoubleQuoteToken OrElse contents <> Me.Contents OrElse doubleQuoteToken <> Me.DoubleQuoteToken) Then
				Dim interpolatedStringExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InterpolatedStringExpression(dollarSignDoubleQuoteToken, contents, doubleQuoteToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				interpolatedStringExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, interpolatedStringExpressionSyntax1, interpolatedStringExpressionSyntax1.WithAnnotations(annotations))
			Else
				interpolatedStringExpressionSyntax = Me
			End If
			Return interpolatedStringExpressionSyntax
		End Function

		Public Function WithContents(ByVal contents As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			Return Me.Update(Me.DollarSignDoubleQuoteToken, contents, Me.DoubleQuoteToken)
		End Function

		Public Function WithDollarSignDoubleQuoteToken(ByVal dollarSignDoubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			Return Me.Update(dollarSignDoubleQuoteToken, Me.Contents, Me.DoubleQuoteToken)
		End Function

		Public Function WithDoubleQuoteToken(ByVal doubleQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringExpressionSyntax
			Return Me.Update(Me.DollarSignDoubleQuoteToken, Me.Contents, doubleQuoteToken)
		End Function
	End Class
End Namespace