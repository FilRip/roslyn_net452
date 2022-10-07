Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NextStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _controlVariables As SyntaxNode

		Public ReadOnly Property ControlVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			Get
				Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._controlVariables, 1)
				expressionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(red, MyBase.GetChildIndex(1)))
				Return expressionSyntaxes
			End Get
		End Property

		Public ReadOnly Property NextKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)._nextKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nextKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal controlVariables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax(kind, errors, annotations, nextKeyword, If(controlVariables IsNot Nothing, controlVariables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNextStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNextStatement(Me)
		End Sub

		Public Function AddControlVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Return Me.WithControlVariables(Me.ControlVariables.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._controlVariables
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._controlVariables, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal nextKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal controlVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			If (nextKeyword <> Me.NextKeyword OrElse controlVariables <> Me.ControlVariables) Then
				Dim nextStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NextStatement(nextKeyword, controlVariables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				nextStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, nextStatementSyntax1, nextStatementSyntax1.WithAnnotations(annotations))
			Else
				nextStatementSyntax = Me
			End If
			Return nextStatementSyntax
		End Function

		Public Function WithControlVariables(ByVal controlVariables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Return Me.Update(Me.NextKeyword, controlVariables)
		End Function

		Public Function WithNextKeyword(ByVal nextKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Return Me.Update(nextKeyword, Me.ControlVariables)
		End Function
	End Class
End Namespace