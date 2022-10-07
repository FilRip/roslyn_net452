Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class IfDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 3)
			End Get
		End Property

		Public ReadOnly Property ElseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)._elseKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxToken
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property IfOrElseIfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)._ifOrElseIfKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property ThenKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)._thenKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(4), MyBase.GetChildIndex(4)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As KeywordSyntax, ByVal ifOrElseIfKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax(kind, errors, annotations, hashToken, elseKeyword, ifOrElseIfKeyword, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), thenKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitIfDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitIfDirectiveTrivia(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 3) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._condition
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetHashTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.HashToken
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			If (i <> 3) Then
				condition = Nothing
			Else
				condition = Me.Condition
			End If
			Return condition
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal ifOrElseIfKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Dim ifDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			If (kind <> MyBase.Kind() OrElse hashToken <> Me.HashToken OrElse elseKeyword <> Me.ElseKeyword OrElse ifOrElseIfKeyword <> Me.IfOrElseIfKeyword OrElse condition <> Me.Condition OrElse thenKeyword <> Me.ThenKeyword) Then
				Dim ifDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.IfDirectiveTrivia(kind, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				ifDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, ifDirectiveTriviaSyntax1, ifDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				ifDirectiveTriviaSyntax = Me
			End If
			Return ifDirectiveTriviaSyntax
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Return Me.Update(MyBase.Kind(), Me.HashToken, Me.ElseKeyword, Me.IfOrElseIfKeyword, condition, Me.ThenKeyword)
		End Function

		Public Function WithElseKeyword(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Return Me.Update(MyBase.Kind(), Me.HashToken, elseKeyword, Me.IfOrElseIfKeyword, Me.Condition, Me.ThenKeyword)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Return Me.Update(MyBase.Kind(), hashToken, Me.ElseKeyword, Me.IfOrElseIfKeyword, Me.Condition, Me.ThenKeyword)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithIfOrElseIfKeyword(ByVal ifOrElseIfKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Return Me.Update(MyBase.Kind(), Me.HashToken, Me.ElseKeyword, ifOrElseIfKeyword, Me.Condition, Me.ThenKeyword)
		End Function

		Public Function WithThenKeyword(ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax
			Return Me.Update(MyBase.Kind(), Me.HashToken, Me.ElseKeyword, Me.IfOrElseIfKeyword, Me.Condition, thenKeyword)
		End Function
	End Class
End Namespace