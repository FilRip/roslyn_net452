Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PropertyStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax

		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax

		Friend _implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)(Me._asClause, 5)
			End Get
		End Property

		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.PropertyKeyword
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)._identifier, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property ImplementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)(Me._implementsClause, 7)
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)(Me._initializer, 6)
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
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public Shadows ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 4)
			End Get
		End Property

		Public ReadOnly Property PropertyKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)._propertyKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal propertyKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, propertyKeyword, identifier, If(parameterList IsNot Nothing, DirectCast(parameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax), Nothing), If(initializer IsNot Nothing, DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax), Nothing), If(implementsClause IsNot Nothing, DirectCast(implementsClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPropertyStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPropertyStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Function AddImplementsClauseInterfaceMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.WithImplementsClause(If(Me.ImplementsClause IsNot Nothing, Me.ImplementsClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImplementsClause(New Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax(-1) {})).AddInterfaceMembers(items))
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
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
			Select Case i
				Case 0
					syntaxNode = Me._attributeLists
					Exit Select
				Case 1
				Case 2
				Case 3
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 4
					syntaxNode = Me._parameterList
					Exit Select
				Case 5
					syntaxNode = Me._asClause
					Exit Select
				Case 6
					syntaxNode = Me._initializer
					Exit Select
				Case 7
					syntaxNode = Me._implementsClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetModifiersCore() As SyntaxTokenList
			Return Me.Modifiers
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Select Case i
				Case 0
					redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
					Exit Select
				Case 1
				Case 2
				Case 3
				Label0:
					redAtZero = Nothing
					Exit Select
				Case 4
					redAtZero = Me.ParameterList
					Exit Select
				Case 5
					redAtZero = Me.AsClause
					Exit Select
				Case 6
					redAtZero = Me.Initializer
					Exit Select
				Case 7
					redAtZero = Me.ImplementsClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return redAtZero
		End Function

		Friend Overrides Function GetParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.ParameterList
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal propertyKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse propertyKeyword <> Me.PropertyKeyword OrElse identifier <> Me.Identifier OrElse parameterList <> Me.ParameterList OrElse asClause <> Me.AsClause OrElse initializer <> Me.Initializer OrElse implementsClause <> Me.ImplementsClause) Then
				Dim propertyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PropertyStatement(attributeLists, modifiers, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				propertyStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, propertyStatementSyntax1, propertyStatementSyntax1.WithAnnotations(annotations))
			Else
				propertyStatementSyntax = Me
			End If
			Return propertyStatementSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.PropertyKeyword, Me.Identifier, Me.ParameterList, asClause, Me.Initializer, Me.ImplementsClause)
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.PropertyKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.Initializer, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithPropertyKeyword(keyword)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.PropertyKeyword, identifier, Me.ParameterList, Me.AsClause, Me.Initializer, Me.ImplementsClause)
		End Function

		Public Function WithImplementsClause(ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.PropertyKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.Initializer, implementsClause)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.PropertyKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, initializer, Me.ImplementsClause)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.WithPropertyKeyword(keyword)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.PropertyKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.Initializer, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Shadows Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.PropertyKeyword, Me.Identifier, parameterList, Me.AsClause, Me.Initializer, Me.ImplementsClause)
		End Function

		Friend Overrides Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterList(parameterList)
		End Function

		Public Function WithPropertyKeyword(ByVal propertyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, propertyKeyword, Me.Identifier, Me.ParameterList, Me.AsClause, Me.Initializer, Me.ImplementsClause)
		End Function
	End Class
End Namespace