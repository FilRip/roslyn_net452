Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ArrayRankSpecifierSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property CommaTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)._commaTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Rank As Integer
			Get
				Return Me.CommaTokens.Count + 1
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As PunctuationSyntax, ByVal commaTokens As GreenNode, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax(kind, errors, annotations, openParenToken, commaTokens, closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitArrayRankSpecifier(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitArrayRankSpecifier(Me)
		End Sub

		Public Function AddCommaTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			Return Me.WithCommaTokens(Me.CommaTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal commaTokens As SyntaxTokenList, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			Dim arrayRankSpecifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			If (openParenToken <> Me.OpenParenToken OrElse commaTokens <> Me.CommaTokens OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim arrayRankSpecifierSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArrayRankSpecifier(openParenToken, commaTokens, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				arrayRankSpecifierSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, arrayRankSpecifierSyntax1, arrayRankSpecifierSyntax1.WithAnnotations(annotations))
			Else
				arrayRankSpecifierSyntax = Me
			End If
			Return arrayRankSpecifierSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			Return Me.Update(Me.OpenParenToken, Me.CommaTokens, closeParenToken)
		End Function

		Public Function WithCommaTokens(ByVal commaTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			Return Me.Update(Me.OpenParenToken, commaTokens, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax
			Return Me.Update(openParenToken, Me.CommaTokens, Me.CloseParenToken)
		End Function
	End Class
End Namespace