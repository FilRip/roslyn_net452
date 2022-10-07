Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlStringSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property EndQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)._endQuoteToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property StartQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)._startQuoteToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property TextTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)._textTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal startQuoteToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal endQuoteToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax(kind, errors, annotations, startQuoteToken, textTokens, endQuoteToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlString(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlString(Me)
		End Sub

		Public Function AddTextTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Return Me.WithTextTokens(Me.TextTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal startQuoteToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal textTokens As SyntaxTokenList, ByVal endQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			If (startQuoteToken <> Me.StartQuoteToken OrElse textTokens <> Me.TextTokens OrElse endQuoteToken <> Me.EndQuoteToken) Then
				Dim xmlStringSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlString(startQuoteToken, textTokens, endQuoteToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlStringSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlStringSyntax1, xmlStringSyntax1.WithAnnotations(annotations))
			Else
				xmlStringSyntax = Me
			End If
			Return xmlStringSyntax
		End Function

		Public Function WithEndQuoteToken(ByVal endQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Return Me.Update(Me.StartQuoteToken, Me.TextTokens, endQuoteToken)
		End Function

		Public Function WithStartQuoteToken(ByVal startQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Return Me.Update(startQuoteToken, Me.TextTokens, Me.EndQuoteToken)
		End Function

		Public Function WithTextTokens(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlStringSyntax
			Return Me.Update(Me.StartQuoteToken, textTokens, Me.EndQuoteToken)
		End Function
	End Class
End Namespace