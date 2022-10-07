Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ArrayCreationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Friend _arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Friend _rankSpecifiers As SyntaxNode

		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax

		Public ReadOnly Property ArrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._arrayBounds, 3)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRed(Me._attributeLists, 1))
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)(Me._initializer, 5)
			End Get
		End Property

		Public Shadows ReadOnly Property NewKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax)._newKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property RankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(MyBase.GetRed(Me._rankSpecifiers, 4))
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal newKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal attributeLists As SyntaxNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal rankSpecifiers As SyntaxNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(kind, errors, annotations, newKeyword, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), If(arrayBounds IsNot Nothing, DirectCast(arrayBounds.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing), If(rankSpecifiers IsNot Nothing, rankSpecifiers.Green, Nothing), DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitArrayCreationExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitArrayCreationExpression(Me)
		End Sub

		Public Function AddArrayBoundsArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.WithArrayBounds(If(Me.ArrayBounds IsNot Nothing, Me.ArrayBounds, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Function AddInitializerInitializers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.WithInitializer(If(Me.Initializer IsNot Nothing, Me.Initializer, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CollectionInitializer()).AddInitializers(items))
		End Function

		Public Function AddRankSpecifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.WithRankSpecifiers(Me.RankSpecifiers.AddRange(items))
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
					syntaxNode = Me._arrayBounds
					Exit Select
				Case 4
					syntaxNode = Me._rankSpecifiers
					Exit Select
				Case 5
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
					red = Me.ArrayBounds
					Exit Select
				Case 4
					red = MyBase.GetRed(Me._rankSpecifiers, 4)
					Exit Select
				Case 5
					red = Me.Initializer
					Exit Select
				Case Else
					red = Nothing
					Exit Select
			End Select
			Return red
		End Function

		Public Function Update(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal rankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax), ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Dim arrayCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			If (newKeyword <> Me.NewKeyword OrElse attributeLists <> Me.AttributeLists OrElse type <> Me.Type OrElse arrayBounds <> Me.ArrayBounds OrElse rankSpecifiers <> Me.RankSpecifiers OrElse initializer <> Me.Initializer) Then
				Dim arrayCreationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArrayCreationExpression(newKeyword, attributeLists, type, arrayBounds, rankSpecifiers, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				arrayCreationExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, arrayCreationExpressionSyntax1, arrayCreationExpressionSyntax1.WithAnnotations(annotations))
			Else
				arrayCreationExpressionSyntax = Me
			End If
			Return arrayCreationExpressionSyntax
		End Function

		Public Function WithArrayBounds(ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, Me.Type, arrayBounds, Me.RankSpecifiers, Me.Initializer)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, attributeLists, Me.Type, Me.ArrayBounds, Me.RankSpecifiers, Me.Initializer)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, Me.Type, Me.ArrayBounds, Me.RankSpecifiers, initializer)
		End Function

		Public Shadows Function WithNewKeyword(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(newKeyword, Me.AttributeLists, Me.Type, Me.ArrayBounds, Me.RankSpecifiers, Me.Initializer)
		End Function

		Friend Overrides Function WithNewKeywordCore(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax
			Return Me.WithNewKeyword(newKeyword)
		End Function

		Public Function WithRankSpecifiers(ByVal rankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, Me.Type, Me.ArrayBounds, rankSpecifiers, Me.Initializer)
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax
			Return Me.Update(Me.NewKeyword, Me.AttributeLists, type, Me.ArrayBounds, Me.RankSpecifiers, Me.Initializer)
		End Function
	End Class
End Namespace