Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EnumStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Friend _underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property EnumKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)._enumKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)._identifier, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public ReadOnly Property UnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)(Me._underlyingType, 4)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal enumKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, enumKeyword, identifier, If(underlyingType IsNot Nothing, DirectCast(underlyingType.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEnumStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEnumStatement(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 4) Then
				syntaxNode = Me._underlyingType
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
			ElseIf (num = 4) Then
				redAtZero = Me.UnderlyingType
			Else
				redAtZero = Nothing
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal enumKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Dim enumStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse enumKeyword <> Me.EnumKeyword OrElse identifier <> Me.Identifier OrElse underlyingType <> Me.UnderlyingType) Then
				Dim enumStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EnumStatement(attributeLists, modifiers, enumKeyword, identifier, underlyingType)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				enumStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, enumStatementSyntax1, enumStatementSyntax1.WithAnnotations(annotations))
			Else
				enumStatementSyntax = Me
			End If
			Return enumStatementSyntax
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.EnumKeyword, Me.Identifier, Me.UnderlyingType)
		End Function

		Public Function WithEnumKeyword(ByVal enumKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, enumKeyword, Me.Identifier, Me.UnderlyingType)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.EnumKeyword, identifier, Me.UnderlyingType)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.EnumKeyword, Me.Identifier, Me.UnderlyingType)
		End Function

		Public Function WithUnderlyingType(ByVal underlyingType As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.EnumKeyword, Me.Identifier, underlyingType)
		End Function
	End Class
End Namespace