Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class IncompleteMemberSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property MissingIdentifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As IdentifierTokenSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)._missingIdentifier
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As GreenNode, ByVal missingIdentifier As IdentifierTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, missingIdentifier), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitIncompleteMember(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitIncompleteMember(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._attributeLists
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			If (i <> 0) Then
				redAtZero = Nothing
			Else
				redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal missingIdentifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Dim incompleteMemberSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse missingIdentifier <> Me.MissingIdentifier) Then
				Dim incompleteMemberSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.IncompleteMember(attributeLists, modifiers, missingIdentifier)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				incompleteMemberSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, incompleteMemberSyntax1, incompleteMemberSyntax1.WithAnnotations(annotations))
			Else
				incompleteMemberSyntax = Me
			End If
			Return incompleteMemberSyntax
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.MissingIdentifier)
		End Function

		Public Function WithMissingIdentifier(ByVal missingIdentifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, missingIdentifier)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.MissingIdentifier)
		End Function
	End Class
End Namespace