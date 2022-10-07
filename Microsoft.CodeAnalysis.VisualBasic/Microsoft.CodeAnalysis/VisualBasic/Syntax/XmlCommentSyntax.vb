Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlCommentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property LessThanExclamationMinusMinusToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)._lessThanExclamationMinusMinusToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property MinusMinusGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)._minusMinusGreaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property TextTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)._textTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal minusMinusGreaterThanToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(kind, errors, annotations, lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlComment(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlComment(Me)
		End Sub

		Public Function AddTextTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			Return Me.WithTextTokens(Me.TextTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal lessThanExclamationMinusMinusToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal textTokens As SyntaxTokenList, ByVal minusMinusGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			Dim xmlCommentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			If (lessThanExclamationMinusMinusToken <> Me.LessThanExclamationMinusMinusToken OrElse textTokens <> Me.TextTokens OrElse minusMinusGreaterThanToken <> Me.MinusMinusGreaterThanToken) Then
				Dim xmlCommentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlCommentSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlCommentSyntax1, xmlCommentSyntax1.WithAnnotations(annotations))
			Else
				xmlCommentSyntax = Me
			End If
			Return xmlCommentSyntax
		End Function

		Public Function WithLessThanExclamationMinusMinusToken(ByVal lessThanExclamationMinusMinusToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			Return Me.Update(lessThanExclamationMinusMinusToken, Me.TextTokens, Me.MinusMinusGreaterThanToken)
		End Function

		Public Function WithMinusMinusGreaterThanToken(ByVal minusMinusGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			Return Me.Update(Me.LessThanExclamationMinusMinusToken, Me.TextTokens, minusMinusGreaterThanToken)
		End Function

		Public Function WithTextTokens(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax
			Return Me.Update(Me.LessThanExclamationMinusMinusToken, textTokens, Me.MinusMinusGreaterThanToken)
		End Function
	End Class
End Namespace