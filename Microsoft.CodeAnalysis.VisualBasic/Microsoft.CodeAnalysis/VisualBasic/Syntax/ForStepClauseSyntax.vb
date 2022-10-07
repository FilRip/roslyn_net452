Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ForStepClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property StepKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax)._stepKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property StepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._stepValue, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal stepKeyword As KeywordSyntax, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax(kind, errors, annotations, stepKeyword, DirectCast(stepValue.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitForStepClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitForStepClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._stepValue
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim stepValue As SyntaxNode
			If (i <> 1) Then
				stepValue = Nothing
			Else
				stepValue = Me.StepValue
			End If
			Return stepValue
		End Function

		Public Function Update(ByVal stepKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax
			If (stepKeyword <> Me.StepKeyword OrElse stepValue <> Me.StepValue) Then
				Dim forStepClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ForStepClause(stepKeyword, stepValue)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				forStepClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, forStepClauseSyntax1, forStepClauseSyntax1.WithAnnotations(annotations))
			Else
				forStepClauseSyntax = Me
			End If
			Return forStepClauseSyntax
		End Function

		Public Function WithStepKeyword(ByVal stepKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax
			Return Me.Update(stepKeyword, Me.StepValue)
		End Function

		Public Function WithStepValue(ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStepClauseSyntax
			Return Me.Update(Me.StepKeyword, stepValue)
		End Function
	End Class
End Namespace