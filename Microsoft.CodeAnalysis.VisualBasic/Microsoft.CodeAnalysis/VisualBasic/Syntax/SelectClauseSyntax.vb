Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SelectClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _variables As SyntaxNode

		Public ReadOnly Property SelectKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax)._selectKeyword, MyBase.Position, 0)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal selectKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal variables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax(kind, errors, annotations, selectKeyword, If(variables IsNot Nothing, variables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSelectClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSelectClause(Me)
		End Sub

		Public Function AddVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax
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

		Public Function Update(ByVal selectKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax
			Dim selectClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax
			If (selectKeyword <> Me.SelectKeyword OrElse variables <> Me.Variables) Then
				Dim selectClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SelectClause(selectKeyword, variables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				selectClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, selectClauseSyntax1, selectClauseSyntax1.WithAnnotations(annotations))
			Else
				selectClauseSyntax = Me
			End If
			Return selectClauseSyntax
		End Function

		Public Function WithSelectKeyword(ByVal selectKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax
			Return Me.Update(selectKeyword, Me.Variables)
		End Function

		Public Function WithVariables(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax
			Return Me.Update(Me.SelectKeyword, variables)
		End Function
	End Class
End Namespace