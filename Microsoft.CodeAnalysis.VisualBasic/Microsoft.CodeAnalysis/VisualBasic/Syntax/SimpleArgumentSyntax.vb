Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SimpleArgumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax
		Friend _nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax

		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNamed As Boolean
			Get
				Return Me.NameColonEquals IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property NameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax)(Me._nameColonEquals)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax(kind, errors, annotations, If(nameColonEquals IsNot Nothing, DirectCast(nameColonEquals.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax), Nothing), DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSimpleArgument(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSimpleArgument(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._nameColonEquals
			ElseIf (num = 1) Then
				syntaxNode = Me._expression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public NotOverridable Overrides Function GetExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Return Me.Expression
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim nameColonEquals As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				nameColonEquals = Me.NameColonEquals
			ElseIf (num = 1) Then
				nameColonEquals = Me.Expression
			Else
				nameColonEquals = Nothing
			End If
			Return nameColonEquals
		End Function

		Public Function Update(ByVal nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax
			Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax
			If (nameColonEquals <> Me.NameColonEquals OrElse expression <> Me.Expression) Then
				Dim simpleArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SimpleArgument(nameColonEquals, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				simpleArgumentSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, simpleArgumentSyntax1, simpleArgumentSyntax1.WithAnnotations(annotations))
			Else
				simpleArgumentSyntax = Me
			End If
			Return simpleArgumentSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax
			Return Me.Update(Me.NameColonEquals, expression)
		End Function

		Public Function WithNameColonEquals(ByVal nameColonEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax
			Return Me.Update(nameColonEquals, Me.Expression)
		End Function
	End Class
End Namespace