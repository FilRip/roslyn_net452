Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SimpleCaseClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax
		Friend _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._value)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax(kind, errors, annotations, DirectCast(value.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSimpleCaseClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSimpleCaseClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._value
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim value As SyntaxNode
			If (i <> 0) Then
				value = Nothing
			Else
				value = Me.Value
			End If
			Return value
		End Function

		Public Function Update(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax
			Dim simpleCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax
			If (value = Me.Value) Then
				simpleCaseClauseSyntax = Me
			Else
				Dim simpleCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SimpleCaseClause(value)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				simpleCaseClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, simpleCaseClauseSyntax1, simpleCaseClauseSyntax1.WithAnnotations(annotations))
			End If
			Return simpleCaseClauseSyntax
		End Function

		Public Function WithValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax
			Return Me.Update(value)
		End Function
	End Class
End Namespace