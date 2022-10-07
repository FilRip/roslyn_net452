Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class TypeStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Friend _typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax

		Public ReadOnly Property Arity As Integer
			Get
				If (Me.TypeParameterList Is Nothing) Then
					Return 0
				End If
				Return Me.TypeParameterList.Parameters.Count
			End Get
		End Property

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return Me.GetAttributeListsCore()
			End Get
		End Property

		Public MustOverride ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetIdentifierCore()
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. ClassKeyword) instead.", True)>
		Public ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.DeclarationKeyword
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Return Me.GetModifiersCore()
			End Get
		End Property

		Public ReadOnly Property TypeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Get
				Return Me.GetTypeParameterListCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddAttributeListsCore(items)
		End Function

		Friend MustOverride Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddModifiersCore(items)
		End Function

		Friend MustOverride Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Public Function AddTypeParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddTypeParameterListParametersCore(items)
		End Function

		Friend MustOverride Function AddTypeParameterListParametersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Friend Overridable Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
		End Function

		Friend Overridable Function GetIdentifierCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax)._identifier, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
		End Function

		Friend Overridable Function GetModifiersCore() As SyntaxTokenList
			Dim syntaxTokenLists As SyntaxTokenList
			Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax)._modifiers
			syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
			Return syntaxTokenLists
		End Function

		Friend Overridable Function GetTypeParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)(Me._typeParameterList, 3)
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithAttributeListsCore(attributeLists)
		End Function

		Friend MustOverride Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Public MustOverride Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithIdentifierCore(identifier)
		End Function

		Friend MustOverride Function WithIdentifierCore(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. WithClassKeyword) instead.", True)>
		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithDeclarationKeyword(keyword)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithModifiersCore(modifiers)
		End Function

		Friend MustOverride Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax

		Public Function WithTypeParameterList(ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithTypeParameterListCore(typeParameterList)
		End Function

		Friend MustOverride Function WithTypeParameterListCore(ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
	End Class
End Namespace