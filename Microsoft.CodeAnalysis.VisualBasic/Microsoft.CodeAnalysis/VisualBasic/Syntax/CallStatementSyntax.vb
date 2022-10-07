Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CallStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property CallKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax)._callKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._invocation, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal callKeyword As KeywordSyntax, ByVal invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax(kind, errors, annotations, callKeyword, DirectCast(invocation.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCallStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCallStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._invocation
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim invocation As SyntaxNode
			If (i <> 1) Then
				invocation = Nothing
			Else
				invocation = Me.Invocation
			End If
			Return invocation
		End Function

		Public Function Update(ByVal callKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax
			Dim callStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax
			If (callKeyword <> Me.CallKeyword OrElse invocation <> Me.Invocation) Then
				Dim callStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CallStatement(callKeyword, invocation)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				callStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, callStatementSyntax1, callStatementSyntax1.WithAnnotations(annotations))
			Else
				callStatementSyntax = Me
			End If
			Return callStatementSyntax
		End Function

		Public Function WithCallKeyword(ByVal callKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax
			Return Me.Update(callKeyword, Me.Invocation)
		End Function

		Public Function WithInvocation(ByVal invocation As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax
			Return Me.Update(Me.CallKeyword, invocation)
		End Function
	End Class
End Namespace