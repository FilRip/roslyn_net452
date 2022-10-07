Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SubNewStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
		Public Shadows ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public Overrides ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.SubKeyword
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
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Public ReadOnly Property NewKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)._newKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public Shadows ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 4)
			End Get
		End Property

		Public ReadOnly Property SubKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)._subKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal subKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal newKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, subKeyword, newKeyword, If(parameterList IsNot Nothing, DirectCast(parameterList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSubNewStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSubNewStatement(Me)
		End Sub

		Public Shadows Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeLists(items)
		End Function

		Public Shadows Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiers(items)
		End Function

		Public Shadows Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
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
			ElseIf (num = 4) Then
				syntaxNode = Me._parameterList
			Else
				syntaxNode = Nothing
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
			ElseIf (num = 4) Then
				redAtZero = Me.ParameterList
			Else
				redAtZero = Nothing
			End If
			Return redAtZero
		End Function

		Friend Overrides Function GetParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.ParameterList
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal subKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Dim subNewStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse subKeyword <> Me.SubKeyword OrElse newKeyword <> Me.NewKeyword OrElse parameterList <> Me.ParameterList) Then
				Dim subNewStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SubNewStatement(attributeLists, modifiers, subKeyword, newKeyword, parameterList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				subNewStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, subNewStatementSyntax1, subNewStatementSyntax1.WithAnnotations(annotations))
			Else
				subNewStatementSyntax = Me
			End If
			Return subNewStatementSyntax
		End Function

		Public Shadows Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.SubKeyword, Me.NewKeyword, Me.ParameterList)
		End Function

		Friend Overrides Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeLists(attributeLists)
		End Function

		Public Overrides Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithSubKeyword(keyword)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.WithSubKeyword(keyword)
		End Function

		Public Shadows Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.SubKeyword, Me.NewKeyword, Me.ParameterList)
		End Function

		Friend Overrides Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiers(modifiers)
		End Function

		Public Function WithNewKeyword(ByVal newKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.SubKeyword, newKeyword, Me.ParameterList)
		End Function

		Public Shadows Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.SubKeyword, Me.NewKeyword, parameterList)
		End Function

		Friend Overrides Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterList(parameterList)
		End Function

		Public Function WithSubKeyword(ByVal subKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, subKeyword, Me.NewKeyword, Me.ParameterList)
		End Function
	End Class
End Namespace