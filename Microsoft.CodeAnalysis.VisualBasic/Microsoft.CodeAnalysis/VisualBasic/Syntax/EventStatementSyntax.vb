Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EventStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Friend _implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 6)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property CustomKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)._customKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxToken
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.EventKeyword
			End Get
		End Property

		Public ReadOnly Property EventKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)._eventKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)._identifier, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property ImplementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)(Me._implementsClause, 7)
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
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public Shadows ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 5)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal customKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal eventKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, customKeyword, eventKeyword, identifier, If(parameterList IsNot Nothing, DirectCast(parameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing), If(implementsClause IsNot Nothing, DirectCast(implementsClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEventStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEventStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Function AddImplementsClauseInterfaceMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.WithImplementsClause(If(Me.ImplementsClause IsNot Nothing, Me.ImplementsClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImplementsClause(New Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax(-1) {})).AddInterfaceMembers(items))
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
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
					Case 5
						syntaxNode = Me._parameterList
						Exit Select
					Case 6
						syntaxNode = Me._asClause
						Exit Select
					Case 7
						syntaxNode = Me._implementsClause
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
						redAtZero = Me.ParameterList
						Exit Select
					Case 6
						redAtZero = Me.AsClause
						Exit Select
					Case 7
						redAtZero = Me.ImplementsClause
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

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal customKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal eventKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse customKeyword <> Me.CustomKeyword OrElse eventKeyword <> Me.EventKeyword OrElse identifier <> Me.Identifier OrElse parameterList <> Me.ParameterList OrElse asClause <> Me.AsClause OrElse implementsClause <> Me.ImplementsClause) Then
				Dim eventStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EventStatement(attributeLists, modifiers, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				eventStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, eventStatementSyntax1, eventStatementSyntax1.WithAnnotations(annotations))
			Else
				eventStatementSyntax = Me
			End If
			Return eventStatementSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.CustomKeyword, Me.EventKeyword, Me.Identifier, Me.ParameterList, asClause, Me.ImplementsClause)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.CustomKeyword, Me.EventKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Function WithCustomKeyword(ByVal customKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, customKeyword, Me.EventKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithEventKeyword(keyword)
		End Function

		Public Function WithEventKeyword(ByVal eventKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.CustomKeyword, eventKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.CustomKeyword, Me.EventKeyword, identifier, Me.ParameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Public Function WithImplementsClause(ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.CustomKeyword, Me.EventKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, implementsClause)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.WithEventKeyword(keyword)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.CustomKeyword, Me.EventKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Shadows Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.CustomKeyword, Me.EventKeyword, Me.Identifier, parameterList, Me.AsClause, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterList(parameterList)
		End Function
	End Class
End Namespace