Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class LetClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _variables As SyntaxNode

		Public ReadOnly Property LetKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax)._letKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
			Get
				Dim expressionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._variables, 1)
				expressionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return expressionRangeVariableSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal letKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal variables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax(kind, errors, annotations, letKeyword, If(variables IsNot Nothing, variables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitLetClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitLetClause(Me)
		End Sub

		Public Function AddVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax
			Return Me.WithVariables(Me.Variables.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._variables
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._variables, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal letKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax
			Dim letClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax
			If (letKeyword <> Me.LetKeyword OrElse variables <> Me.Variables) Then
				Dim letClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.LetClause(letKeyword, variables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				letClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, letClauseSyntax1, letClauseSyntax1.WithAnnotations(annotations))
			Else
				letClauseSyntax = Me
			End If
			Return letClauseSyntax
		End Function

		Public Function WithLetKeyword(ByVal letKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax
			Return Me.Update(letKeyword, Me.Variables)
		End Function

		Public Function WithVariables(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax
			Return Me.Update(Me.LetKeyword, variables)
		End Function
	End Class
End Namespace