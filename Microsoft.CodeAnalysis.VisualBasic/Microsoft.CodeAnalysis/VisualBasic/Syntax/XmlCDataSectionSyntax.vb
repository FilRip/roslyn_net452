Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlCDataSectionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property BeginCDataToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)._beginCDataToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property EndCDataToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)._endCDataToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property TextTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)._textTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal beginCDataToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal endCDataToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax(kind, errors, annotations, beginCDataToken, textTokens, endCDataToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlCDataSection(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlCDataSection(Me)
		End Sub

		Public Function AddTextTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			Return Me.WithTextTokens(Me.TextTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal beginCDataToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal textTokens As SyntaxTokenList, ByVal endCDataToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			Dim xmlCDataSectionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			If (beginCDataToken <> Me.BeginCDataToken OrElse textTokens <> Me.TextTokens OrElse endCDataToken <> Me.EndCDataToken) Then
				Dim xmlCDataSectionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlCDataSection(beginCDataToken, textTokens, endCDataToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlCDataSectionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlCDataSectionSyntax1, xmlCDataSectionSyntax1.WithAnnotations(annotations))
			Else
				xmlCDataSectionSyntax = Me
			End If
			Return xmlCDataSectionSyntax
		End Function

		Public Function WithBeginCDataToken(ByVal beginCDataToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			Return Me.Update(beginCDataToken, Me.TextTokens, Me.EndCDataToken)
		End Function

		Public Function WithEndCDataToken(ByVal endCDataToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			Return Me.Update(Me.BeginCDataToken, Me.TextTokens, endCDataToken)
		End Function

		Public Function WithTextTokens(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCDataSectionSyntax
			Return Me.Update(Me.BeginCDataToken, textTokens, Me.EndCDataToken)
		End Function
	End Class
End Namespace