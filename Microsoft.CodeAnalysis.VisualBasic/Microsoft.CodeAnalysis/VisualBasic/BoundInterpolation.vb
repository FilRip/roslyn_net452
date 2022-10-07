Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundInterpolation
		Inherits BoundNode
		Private ReadOnly _Expression As BoundExpression

		Private ReadOnly _AlignmentOpt As BoundExpression

		Private ReadOnly _FormatStringOpt As BoundLiteral

		Public ReadOnly Property AlignmentOpt As BoundExpression
			Get
				Return Me._AlignmentOpt
			End Get
		End Property

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public ReadOnly Property FormatStringOpt As BoundLiteral
			Get
				Return Me._FormatStringOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal alignmentOpt As BoundExpression, ByVal formatStringOpt As BoundLiteral, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Interpolation, syntax, If(hasErrors OrElse expression.NonNullAndHasErrors() OrElse alignmentOpt.NonNullAndHasErrors(), True, formatStringOpt.NonNullAndHasErrors()))
			Me._Expression = expression
			Me._AlignmentOpt = alignmentOpt
			Me._FormatStringOpt = formatStringOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitInterpolation(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal alignmentOpt As BoundExpression, ByVal formatStringOpt As BoundLiteral) As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation
			Dim boundInterpolation As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation
			If (expression <> Me.Expression OrElse alignmentOpt <> Me.AlignmentOpt OrElse formatStringOpt <> Me.FormatStringOpt) Then
				Dim boundInterpolation1 As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation = New Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation(MyBase.Syntax, expression, alignmentOpt, formatStringOpt, MyBase.HasErrors)
				boundInterpolation1.CopyAttributes(Me)
				boundInterpolation = boundInterpolation1
			Else
				boundInterpolation = Me
			End If
			Return boundInterpolation
		End Function
	End Class
End Namespace