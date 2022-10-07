Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class StructureStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.StructureKeyword
			End Get
		End Property

		Public Shadows ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)._identifier, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.DeclarationKeyword
			End Get
		End Property

		Public Shadows ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public ReadOnly Property StructureKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)._structureKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public Shadows ReadOnly Property TypeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)(Me._typeParameterList, 4)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal structureKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, structureKeyword, identifier, If(typeParameterList IsNot Nothing, DirectCast(typeParameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitStructureStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitStructureStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddTypeParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.WithTypeParameterList(If(Me.TypeParameterList IsNot Nothing, Me.TypeParameterList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameterList(New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(-1) {})).AddParameters(items))
		End Function

		Friend Overrides Function AddTypeParameterListParametersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.AddTypeParameterListParameters(items)
		End Function

		Friend Overrides Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return Me.AttributeLists
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 4) Then
				syntaxNode = Me._typeParameterList
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetIdentifierCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.Identifier
		End Function

		Friend Overrides Function GetModifiersCore() As SyntaxTokenList
			Return Me.Modifiers
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
			ElseIf (num = 4) Then
				redAtZero = Me.TypeParameterList
			Else
				redAtZero = Nothing
			End If
			Return redAtZero
		End Function

		Friend Overrides Function GetTypeParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.TypeParameterList
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal structureKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Dim structureStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse structureKeyword <> Me.StructureKeyword OrElse identifier <> Me.Identifier OrElse typeParameterList <> Me.TypeParameterList) Then
				Dim structureStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.StructureStatement(attributeLists, modifiers, structureKeyword, identifier, typeParameterList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				structureStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, structureStatementSyntax1, structureStatementSyntax1.WithAnnotations(annotations))
			Else
				structureStatementSyntax = Me
			End If
			Return structureStatementSyntax
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.StructureKeyword, Me.Identifier, Me.TypeParameterList)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithStructureKeyword(keyword)
		End Function

		Public Shadows Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.StructureKeyword, identifier, Me.TypeParameterList)
		End Function

		Friend Overrides Function WithIdentifierCore(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithIdentifier(identifier)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.WithStructureKeyword(keyword)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.StructureKeyword, Me.Identifier, Me.TypeParameterList)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Function WithStructureKeyword(ByVal structureKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, structureKeyword, Me.Identifier, Me.TypeParameterList)
		End Function

		Public Shadows Function WithTypeParameterList(ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.StructureKeyword, Me.Identifier, typeParameterList)
		End Function

		Friend Overrides Function WithTypeParameterListCore(ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax
			Return Me.WithTypeParameterList(typeParameterList)
		End Function
	End Class
End Namespace