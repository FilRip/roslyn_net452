Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DelegateStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
		Friend _typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 7)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.SubOrFunctionKeyword
			End Get
		End Property

		Public ReadOnly Property DelegateKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)._delegateKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)._identifier, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
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
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public Shadows ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 6)
			End Get
		End Property

		Public ReadOnly Property SubOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)._subOrFunctionKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property TypeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax)(Me._typeParameterList, 5)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal delegateKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, delegateKeyword, subOrFunctionKeyword, identifier, If(typeParameterList IsNot Nothing, DirectCast(typeParameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax), Nothing), If(parameterList IsNot Nothing, DirectCast(parameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDelegateStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDelegateStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.WithParameterList(If(Me.ParameterList IsNot Nothing, Me.ParameterList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParameterList()).AddParameters(items))
		End Function

		Friend Overrides Function AddParameterListParametersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddParameterListParameters(items)
		End Function

		Public Function AddTypeParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.WithTypeParameterList(If(Me.TypeParameterList IsNot Nothing, Me.TypeParameterList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameterList(New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(-1) {})).AddParameters(items))
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
					Case 5
						syntaxNode = Me._typeParameterList
						Exit Select
					Case 6
						syntaxNode = Me._parameterList
						Exit Select
					Case 7
						syntaxNode = Me._asClause
						Exit Select
					Case Else
						syntaxNode = Nothing
						Exit Select
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
					Case 5
						redAtZero = Me.TypeParameterList
						Exit Select
					Case 6
						redAtZero = Me.ParameterList
						Exit Select
					Case 7
						redAtZero = Me.AsClause
						Exit Select
					Case Else
						redAtZero = Nothing
						Exit Select
				End Select
			End If
			Return redAtZero
		End Function

		Friend Overrides Function GetParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.ParameterList
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal delegateKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Dim delegateStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			If (kind <> MyBase.Kind() OrElse attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse delegateKeyword <> Me.DelegateKeyword OrElse subOrFunctionKeyword <> Me.SubOrFunctionKeyword OrElse identifier <> Me.Identifier OrElse typeParameterList <> Me.TypeParameterList OrElse parameterList <> Me.ParameterList OrElse asClause <> Me.AsClause) Then
				Dim delegateStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DelegateStatement(kind, attributeLists, modifiers, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				delegateStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, delegateStatementSyntax1, delegateStatementSyntax1.WithAnnotations(annotations))
			Else
				delegateStatementSyntax = Me
			End If
			Return delegateStatementSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, Me.ParameterList, asClause)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), attributeLists, Me.Modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, Me.ParameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithSubOrFunctionKeyword(keyword)
		End Function

		Public Function WithDelegateKeyword(ByVal delegateKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, delegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, identifier, Me.TypeParameterList, Me.ParameterList, Me.AsClause)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.WithSubOrFunctionKeyword(keyword)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, Me.ParameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Shadows Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, parameterList, Me.AsClause)
		End Function

		Friend Overrides Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterList(parameterList)
		End Function

		Public Function WithSubOrFunctionKeyword(ByVal subOrFunctionKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DelegateKeyword, subOrFunctionKeyword, Me.Identifier, Me.TypeParameterList, Me.ParameterList, Me.AsClause)
		End Function

		Public Function WithTypeParameterList(ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.AttributeLists, Me.Modifiers, Me.DelegateKeyword, Me.SubOrFunctionKeyword, Me.Identifier, typeParameterList, Me.ParameterList, Me.AsClause)
		End Function
	End Class
End Namespace