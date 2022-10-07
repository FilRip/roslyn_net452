Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AddRemoveHandlerStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property AddHandlerOrRemoveHandlerKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax)._addHandlerOrRemoveHandlerKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property CommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax)._commaToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property DelegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._delegateExpression, 3)
			End Get
		End Property

		Public ReadOnly Property EventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._eventExpression, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(kind, errors, annotations, addHandlerOrRemoveHandlerKeyword, DirectCast(eventExpression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), commaToken, DirectCast(delegateExpression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAddRemoveHandlerStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAddRemoveHandlerStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._eventExpression
			ElseIf (num = 3) Then
				syntaxNode = Me._delegateExpression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim eventExpression As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				eventExpression = Me.EventExpression
			ElseIf (num = 3) Then
				eventExpression = Me.DelegateExpression
			Else
				eventExpression = Nothing
			End If
			Return eventExpression
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal addHandlerOrRemoveHandlerKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			Dim addRemoveHandlerStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			If (kind <> MyBase.Kind() OrElse addHandlerOrRemoveHandlerKeyword <> Me.AddHandlerOrRemoveHandlerKeyword OrElse eventExpression <> Me.EventExpression OrElse commaToken <> Me.CommaToken OrElse delegateExpression <> Me.DelegateExpression) Then
				Dim addRemoveHandlerStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AddRemoveHandlerStatement(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				addRemoveHandlerStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, addRemoveHandlerStatementSyntax1, addRemoveHandlerStatementSyntax1.WithAnnotations(annotations))
			Else
				addRemoveHandlerStatementSyntax = Me
			End If
			Return addRemoveHandlerStatementSyntax
		End Function

		Public Function WithAddHandlerOrRemoveHandlerKeyword(ByVal addHandlerOrRemoveHandlerKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			Return Me.Update(MyBase.Kind(), addHandlerOrRemoveHandlerKeyword, Me.EventExpression, Me.CommaToken, Me.DelegateExpression)
		End Function

		Public Function WithCommaToken(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AddHandlerOrRemoveHandlerKeyword, Me.EventExpression, commaToken, Me.DelegateExpression)
		End Function

		Public Function WithDelegateExpression(ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AddHandlerOrRemoveHandlerKeyword, Me.EventExpression, Me.CommaToken, delegateExpression)
		End Function

		Public Function WithEventExpression(ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AddHandlerOrRemoveHandlerKeyword, eventExpression, Me.CommaToken, Me.DelegateExpression)
		End Function
	End Class
End Namespace