Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ObjectCreationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Friend _argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax

		Public ReadOnly Property ArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._argumentList, 3)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRed(Me._attributeLists, 1))
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax)(Me._initializer, 4)
			End Get
		End Property

		Public Shadows ReadOnly Property NewKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax)._newKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal newKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal attributeLists As SyntaxNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(kind, errors, annotations, newKeyword, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), If(argumentList IsNot Nothing, DirectCast(argumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing), If(initializer IsNot Nothing, DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitObjectCreationExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitObjectCreationExpression(Me)
		End Sub

		Public Function AddArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.WithArgumentList(If(Me.ArgumentList IsNot Nothing, Me.ArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Friend Overrides Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return Me.AttributeLists
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._attributeLists
					Exit Select
				Case 2
					syntaxNode = Me._type
					Exit Select
				Case 3
					syntaxNode = Me._argumentList
					Exit Select
				Case 4
					syntaxNode = Me._initializer
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNewKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.NewKeyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			Select Case i
				Case 1
					red = MyBase.GetRed(Me._attributeLists, 1)
					Exit Select
				Case 2
					red = Me.Type
					Exit Select
				Case 3
					red = Me.ArgumentList
					Exit Select
				Case 4
					red = Me.Initializer
					Exit Select
				Case Else
					red = Nothing
					Exit Select
			End Select
			Return red
		End Function

		Public Function Update(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Dim objectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			If (newKeyword <> Me.NewKeyword OrElse attributeLists <> Me.AttributeLists OrElse type <> Me.Type OrElse argumentList <> Me.ArgumentList OrElse initializer <> Me.Initializer) Then
				Dim objectCreationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ObjectCreationExpression(newKeyword, attributeLists, type, argumentList, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				objectCreationExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, objectCreationExpressionSyntax1, objectCreationExpressionSyntax1.WithAnnotations(annotations))
			Else
				objectCreationExpressionSyntax = Me
			End If
			Return objectCreationExpressionSyntax
		End Function

		Public Function WithArgumentList(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, Me.Type, argumentList, Me.Initializer)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, attributeLists, Me.Type, Me.ArgumentList, Me.Initializer)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, Me.Type, Me.ArgumentList, initializer)
		End Function

		Public Shadows Function WithNewKeyword(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.Update(newKeyword, Me.AttributeLists, Me.Type, Me.ArgumentList, Me.Initializer)
		End Function

		Friend Overrides Function WithNewKeywordCore(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithNewKeyword(newKeyword)
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, type, Me.ArgumentList, Me.Initializer)
		End Function
	End Class
End Namespace