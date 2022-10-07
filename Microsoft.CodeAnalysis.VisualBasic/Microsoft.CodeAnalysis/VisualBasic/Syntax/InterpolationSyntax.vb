Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InterpolationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax

		Friend _formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax

		Public ReadOnly Property AlignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax)(Me._alignmentClause, 2)
			End Get
		End Property

		Public ReadOnly Property CloseBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax)._closeBraceToken, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property FormatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax)(Me._formatClause, 3)
			End Get
		End Property

		Public ReadOnly Property OpenBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax)._openBraceToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax(kind, errors, annotations, openBraceToken, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), If(alignmentClause IsNot Nothing, DirectCast(alignmentClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax), Nothing), If(formatClause IsNot Nothing, DirectCast(formatClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax), Nothing), closeBraceToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInterpolation(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInterpolation(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._expression
					Exit Select
				Case 2
					syntaxNode = Me._alignmentClause
					Exit Select
				Case 3
					syntaxNode = Me._formatClause
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Select Case i
				Case 1
					expression = Me.Expression
					Exit Select
				Case 2
					expression = Me.AlignmentClause
					Exit Select
				Case 3
					expression = Me.FormatClause
					Exit Select
				Case Else
					expression = Nothing
					Exit Select
			End Select
			Return expression
		End Function

		Public Function Update(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax, ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax, ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Dim interpolationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			If (openBraceToken <> Me.OpenBraceToken OrElse expression <> Me.Expression OrElse alignmentClause <> Me.AlignmentClause OrElse formatClause <> Me.FormatClause OrElse closeBraceToken <> Me.CloseBraceToken) Then
				Dim interpolationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Interpolation(openBraceToken, expression, alignmentClause, formatClause, closeBraceToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				interpolationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, interpolationSyntax1, interpolationSyntax1.WithAnnotations(annotations))
			Else
				interpolationSyntax = Me
			End If
			Return interpolationSyntax
		End Function

		Public Function WithAlignmentClause(ByVal alignmentClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationAlignmentClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Return Me.Update(Me.OpenBraceToken, Me.Expression, alignmentClause, Me.FormatClause, Me.CloseBraceToken)
		End Function

		Public Function WithCloseBraceToken(ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Return Me.Update(Me.OpenBraceToken, Me.Expression, Me.AlignmentClause, Me.FormatClause, closeBraceToken)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Return Me.Update(Me.OpenBraceToken, expression, Me.AlignmentClause, Me.FormatClause, Me.CloseBraceToken)
		End Function

		Public Function WithFormatClause(ByVal formatClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Return Me.Update(Me.OpenBraceToken, Me.Expression, Me.AlignmentClause, formatClause, Me.CloseBraceToken)
		End Function

		Public Function WithOpenBraceToken(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationSyntax
			Return Me.Update(openBraceToken, Me.Expression, Me.AlignmentClause, Me.FormatClause, Me.CloseBraceToken)
		End Function
	End Class
End Namespace