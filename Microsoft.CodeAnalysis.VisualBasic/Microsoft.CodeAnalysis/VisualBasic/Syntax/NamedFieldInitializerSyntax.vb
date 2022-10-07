Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NamedFieldInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property DotToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)._dotToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)._equalsToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 4)
			End Get
		End Property

		Public Shadows ReadOnly Property KeyKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax)._keyKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._name, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyKeyword As KeywordSyntax, ByVal dotToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax(kind, errors, annotations, keyKeyword, dotToken, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), equalsToken, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNamedFieldInitializer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNamedFieldInitializer(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				syntaxNode = Me._name
			ElseIf (num = 4) Then
				syntaxNode = Me._expression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetKeyKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.KeyKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				name = Me.Name
			ElseIf (num = 4) Then
				name = Me.Expression
			Else
				name = Nothing
			End If
			Return name
		End Function

		Public Function Update(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Dim namedFieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			If (keyKeyword <> Me.KeyKeyword OrElse dotToken <> Me.DotToken OrElse name <> Me.Name OrElse equalsToken <> Me.EqualsToken OrElse expression <> Me.Expression) Then
				Dim namedFieldInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NamedFieldInitializer(keyKeyword, dotToken, name, equalsToken, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				namedFieldInitializerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, namedFieldInitializerSyntax1, namedFieldInitializerSyntax1.WithAnnotations(annotations))
			Else
				namedFieldInitializerSyntax = Me
			End If
			Return namedFieldInitializerSyntax
		End Function

		Public Function WithDotToken(ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Return Me.Update(Me.KeyKeyword, dotToken, Me.Name, Me.EqualsToken, Me.Expression)
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Return Me.Update(Me.KeyKeyword, Me.DotToken, Me.Name, equalsToken, Me.Expression)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Return Me.Update(Me.KeyKeyword, Me.DotToken, Me.Name, Me.EqualsToken, expression)
		End Function

		Public Shadows Function WithKeyKeyword(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Return Me.Update(keyKeyword, Me.DotToken, Me.Name, Me.EqualsToken, Me.Expression)
		End Function

		Friend Overrides Function WithKeyKeywordCore(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
			Return Me.WithKeyKeyword(keyKeyword)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax
			Return Me.Update(Me.KeyKeyword, Me.DotToken, name, Me.EqualsToken, Me.Expression)
		End Function
	End Class
End Namespace