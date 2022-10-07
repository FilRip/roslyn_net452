Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DeclareStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
		Friend _libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax

		Friend _aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Public ReadOnly Property AliasKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._aliasKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(8), MyBase.GetChildIndex(8)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property AliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)(Me._aliasName, 9)
			End Get
		End Property

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 11)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property CharsetKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._charsetKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(3), MyBase.GetChildIndex(3)))
				Return syntaxToken
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.SubOrFunctionKeyword
			End Get
		End Property

		Public ReadOnly Property DeclareKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._declareKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._identifier, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.DeclarationKeyword
			End Get
		End Property

		Public ReadOnly Property LibKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._libKeyword, Me.GetChildPosition(6), MyBase.GetChildIndex(6))
			End Get
		End Property

		Public ReadOnly Property LibraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax)(Me._libraryName, 7)
			End Get
		End Property

		Public Shadows ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public Shadows ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 10)
			End Get
		End Property

		Public ReadOnly Property SubOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)._subOrFunctionKeyword, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal declareKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal charsetKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal libKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax, ByVal aliasKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, DirectCast(libraryName.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax), aliasKeyword, If(aliasName IsNot Nothing, DirectCast(aliasName.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax), Nothing), If(parameterList IsNot Nothing, DirectCast(parameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDeclareStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDeclareStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.WithParameterList(If(Me.ParameterList IsNot Nothing, Me.ParameterList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParameterList()).AddParameters(items))
		End Function

		Friend Overrides Function AddParameterListParametersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddParameterListParameters(items)
		End Function

		Friend Overrides Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return Me.AttributeLists
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._attributeLists
			Else
				Select Case num
					Case 7
						syntaxNode = Me._libraryName
						Exit Select
					Case 8
					Label0:
						syntaxNode = Nothing
						Exit Select
					Case 9
						syntaxNode = Me._aliasName
						Exit Select
					Case 10
						syntaxNode = Me._parameterList
						Exit Select
					Case 11
						syntaxNode = Me._asClause
						Exit Select
					Case Else
						GoTo Label0
				End Select
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetModifiersCore() As SyntaxTokenList
			Return Me.Modifiers
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
			Else
				Select Case num
					Case 7
						redAtZero = Me.LibraryName
						Exit Select
					Case 8
					Label0:
						redAtZero = Nothing
						Exit Select
					Case 9
						redAtZero = Me.AliasName
						Exit Select
					Case 10
						redAtZero = Me.ParameterList
						Exit Select
					Case 11
						redAtZero = Me.AsClause
						Exit Select
					Case Else
						GoTo Label0
				End Select
			End If
			Return redAtZero
		End Function

		Friend Overrides Function GetParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.ParameterList
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal declareKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal charsetKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal libKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax, ByVal aliasKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Dim declareStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			If (kind <> MyBase.Kind() OrElse attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse declareKeyword <> Me.DeclareKeyword OrElse charsetKeyword <> Me.CharsetKeyword OrElse subOrFunctionKeyword <> Me.SubOrFunctionKeyword OrElse identifier <> Me.Identifier OrElse libKeyword <> Me.LibKeyword OrElse libraryName <> Me.LibraryName OrElse aliasKeyword <> Me.AliasKeyword OrElse aliasName <> Me.AliasName OrElse parameterList <> Me.ParameterList OrElse asClause <> Me.AsClause) Then
				Dim declareStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DeclareStatement(kind, attributeLists, modifiers, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				declareStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, declareStatementSyntax1, declareStatementSyntax1.WithAnnotations(annotations))
			Else
				declareStatementSyntax = Me
			End If
			Return declareStatementSyntax
		End Function

		Public Function WithAliasKeyword(ByVal aliasKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, aliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithAliasName(ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, aliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, asClause)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), attributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Function WithCharsetKeyword(ByVal charsetKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, charsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithSubOrFunctionKeyword(keyword)
		End Function

		Public Function WithDeclareKeyword(ByVal declareKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, declareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.WithSubOrFunctionKeyword(keyword)
		End Function

		Public Function WithLibKeyword(ByVal libKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, libKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithLibraryName(ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, libraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Shadows Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, parameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterList(parameterList)
		End Function

		Public Function WithSubOrFunctionKeyword(ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DeclareKeyword, Me.CharsetKeyword, subOrFunctionKeyword, Me.Identifier, Me.LibKeyword, Me.LibraryName, Me.AliasKeyword, Me.AliasName, Me.ParameterList, Me.AsClause)
		End Function
	End Class
End Namespace