Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SimpleAsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
		Friend _attributeLists As SyntaxNode

		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public Shadows ReadOnly Property AsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)._asKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRed(Me._attributeLists, 1))
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal attributeLists As SyntaxNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(kind, errors, annotations, asKeyword, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSimpleAsClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSimpleAsClause(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function GetAsKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.AsKeyword
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 2) Then
				syntaxNode = Me._type
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				red = MyBase.GetRed(Me._attributeLists, 1)
			ElseIf (num = 2) Then
				red = Me.Type
			Else
				red = Nothing
			End If
			Return red
		End Function

		Public Function Update(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			If (asKeyword <> Me.AsKeyword OrElse attributeLists <> Me.AttributeLists OrElse type <> Me.Type) Then
				Dim simpleAsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SimpleAsClause(asKeyword, attributeLists, type)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				simpleAsClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, simpleAsClauseSyntax1, simpleAsClauseSyntax1.WithAnnotations(annotations))
			Else
				simpleAsClauseSyntax = Me
			End If
			Return simpleAsClauseSyntax
		End Function

		Public Shadows Function WithAsKeyword(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Return Me.Update(asKeyword, Me.AttributeLists, Me.Type)
		End Function

		Friend Overrides Function WithAsKeywordCore(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Return Me.WithAsKeyword(asKeyword)
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Return Me.Update(Me.AsKeyword, attributeLists, Me.Type)
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Return Me.Update(Me.AsKeyword, Me.AttributeLists, type)
		End Function
	End Class
End Namespace