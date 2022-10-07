Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Roslyn.Utilities
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class MethodBaseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Friend _parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax

		Friend ReadOnly Property AsClauseInternal As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Get
				Dim asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = MyBase.Kind()
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax).AsClause
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
						asClause = Nothing
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax).AsClause
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax).AsClause
						Exit Select
					Case 100
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Throw ExceptionUtilities.UnexpectedValue(MyBase.Kind())
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax).AsClause
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax).AsClause
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
						asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax).AsClause
						Exit Select
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							asClause = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax).AsClause
							Exit Select
						Else
							Throw ExceptionUtilities.UnexpectedValue(MyBase.Kind())
						End If
				End Select
				Return asClause
			End Get
		End Property

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return Me.GetAttributeListsCore()
			End Get
		End Property

		Public MustOverride ReadOnly Property DeclarationKeyword As Microsoft.CodeAnalysis.SyntaxToken

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. SubOrFunctionKeyword) instead.", True)>
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

		Public ReadOnly Property ParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Get
				Return Me.GetParameterListCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddAttributeListsCore(items)
		End Function

		Friend MustOverride Function AddAttributeListsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddModifiersCore(items)
		End Function

		Friend MustOverride Function AddModifiersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		Public Function AddParameterListParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.AddParameterListParametersCore(items)
		End Function

		Friend MustOverride Function AddParameterListParametersCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		Friend Overridable Function GetAttributeListsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
		End Function

		Friend Overridable Function GetModifiersCore() As SyntaxTokenList
			Dim syntaxTokenLists As SyntaxTokenList
			Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax)._modifiers
			syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
			Return syntaxTokenLists
		End Function

		Friend Overridable Function GetParameterListCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax)(Me._parameterList, 2)
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithAttributeListsCore(attributeLists)
		End Function

		Friend MustOverride Function WithAttributeListsCore(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		Public MustOverride Function WithDeclarationKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. WithSubOrFunctionKeyword) instead.", True)>
		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithDeclarationKeyword(keyword)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithModifiersCore(modifiers)
		End Function

		Friend MustOverride Function WithModifiersCore(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax

		Public Function WithParameterList(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Return Me.WithParameterListCore(parameterList)
		End Function

		Friend MustOverride Function WithParameterListCore(ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
	End Class
End Namespace