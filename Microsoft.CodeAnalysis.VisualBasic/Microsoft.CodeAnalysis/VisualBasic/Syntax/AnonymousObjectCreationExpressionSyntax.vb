Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AnonymousObjectCreationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRed(Me._attributeLists, 1))
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax)(Me._initializer, 2)
			End Get
		End Property

		Public Shadows ReadOnly Property NewKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)._newKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal newKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal attributeLists As SyntaxNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(kind, errors, annotations, newKeyword, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAnonymousObjectCreationExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAnonymousObjectCreationExpression(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Function AddInitializerInitializers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Return Me.WithInitializer(If(Me.Initializer IsNot Nothing, Me.Initializer, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ObjectMemberInitializer(New Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax(-1) {})).AddInitializers(items))
		End Function

		Friend Overrides Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return Me.AttributeLists
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 2) Then
				syntaxNode = Me._initializer
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNewKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.NewKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				red = MyBase.GetRed(Me._attributeLists, 1)
			ElseIf (num = 2) Then
				red = Me.Initializer
			Else
				red = Nothing
			End If
			Return red
		End Function

		Public Function Update(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Dim anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			If (newKeyword <> Me.NewKeyword OrElse attributeLists <> Me.AttributeLists OrElse initializer <> Me.Initializer) Then
				Dim anonymousObjectCreationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AnonymousObjectCreationExpression(newKeyword, attributeLists, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				anonymousObjectCreationExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, anonymousObjectCreationExpressionSyntax1, anonymousObjectCreationExpressionSyntax1.WithAnnotations(annotations))
			Else
				anonymousObjectCreationExpressionSyntax = Me
			End If
			Return anonymousObjectCreationExpressionSyntax
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, attributeLists, Me.Initializer)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, initializer)
		End Function

		Public Shadows Function WithNewKeyword(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax
			Return Me.Update(newKeyword, Me.AttributeLists, Me.Initializer)
		End Function

		Friend Overrides Function WithNewKeywordCore(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithNewKeyword(newKeyword)
		End Function
	End Class
End Namespace