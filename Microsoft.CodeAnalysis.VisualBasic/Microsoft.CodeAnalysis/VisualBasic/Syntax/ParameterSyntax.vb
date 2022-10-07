Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ParameterSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _attributeLists As SyntaxNode

		Friend _identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Friend _default As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 3)
			End Get
		End Property

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property [Default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)(Me._default, 4)
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(Me._identifier, 2)
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, DirectCast(identifier.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing), If([default] IsNot Nothing, DirectCast([default].Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitParameter(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitParameter(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._attributeLists
					Exit Select
				Case 1
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 2
					syntaxNode = Me._identifier
					Exit Select
				Case 3
					syntaxNode = Me._asClause
					Exit Select
				Case 4
					syntaxNode = Me._default
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Select Case i
				Case 0
					redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
					Exit Select
				Case 1
				Label0:
					redAtZero = Nothing
					Exit Select
				Case 2
					redAtZero = Me.Identifier
					Exit Select
				Case 3
					redAtZero = Me.AsClause
					Exit Select
				Case 4
					redAtZero = Me.[Default]
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return redAtZero
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse identifier <> Me.Identifier OrElse asClause <> Me.AsClause OrElse [default] <> Me.[Default]) Then
				Dim parameterSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Parameter(attributeLists, modifiers, identifier, asClause, [default])
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				parameterSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, parameterSyntax1, parameterSyntax1.WithAnnotations(annotations))
			Else
				parameterSyntax = Me
			End If
			Return parameterSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.Identifier, asClause, Me.[Default])
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.Identifier, Me.AsClause, Me.[Default])
		End Function

		Public Function WithDefault(ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, Me.Identifier, Me.AsClause, [default])
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, identifier, Me.AsClause, Me.[Default])
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.Identifier, Me.AsClause, Me.[Default])
		End Function
	End Class
End Namespace