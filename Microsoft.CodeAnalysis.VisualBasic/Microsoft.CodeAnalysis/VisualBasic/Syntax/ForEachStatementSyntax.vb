Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ForEachStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public Shadows ReadOnly Property ControlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._controlVariable, 2)
			End Get
		End Property

		Public ReadOnly Property EachKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)._eachKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 4)
			End Get
		End Property

		Public Shadows ReadOnly Property ForKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)._forKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property InKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)._inKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(kind, errors, annotations, forKeyword, eachKeyword, DirectCast(controlVariable.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), inKeyword, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitForEachStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitForEachStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				syntaxNode = Me._controlVariable
			ElseIf (num = 4) Then
				syntaxNode = Me._expression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetControlVariableCore() As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return Me.ControlVariable
		End Function

		Friend Overrides Function GetForKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.ForKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim controlVariable As SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				controlVariable = Me.ControlVariable
			ElseIf (num = 4) Then
				controlVariable = Me.Expression
			Else
				controlVariable = Nothing
			End If
			Return controlVariable
		End Function

		Public Function Update(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal eachKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal inKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Dim forEachStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			If (forKeyword <> Me.ForKeyword OrElse eachKeyword <> Me.EachKeyword OrElse controlVariable <> Me.ControlVariable OrElse inKeyword <> Me.InKeyword OrElse expression <> Me.Expression) Then
				Dim forEachStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ForEachStatement(forKeyword, eachKeyword, controlVariable, inKeyword, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				forEachStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, forEachStatementSyntax1, forEachStatementSyntax1.WithAnnotations(annotations))
			Else
				forEachStatementSyntax = Me
			End If
			Return forEachStatementSyntax
		End Function

		Public Shadows Function WithControlVariable(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.EachKeyword, controlVariable, Me.InKeyword, Me.Expression)
		End Function

		Friend Overrides Function WithControlVariableCore(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithControlVariable(controlVariable)
		End Function

		Public Function WithEachKeyword(ByVal eachKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Return Me.Update(Me.ForKeyword, eachKeyword, Me.ControlVariable, Me.InKeyword, Me.Expression)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.EachKeyword, Me.ControlVariable, Me.InKeyword, expression)
		End Function

		Public Shadows Function WithForKeyword(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Return Me.Update(forKeyword, Me.EachKeyword, Me.ControlVariable, Me.InKeyword, Me.Expression)
		End Function

		Friend Overrides Function WithForKeywordCore(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithForKeyword(forKeyword)
		End Function

		Public Function WithInKeyword(ByVal inKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Return Me.Update(Me.ForKeyword, Me.EachKeyword, Me.ControlVariable, inKeyword, Me.Expression)
		End Function
	End Class
End Namespace