Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class FieldDeclarationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Friend _declarators As SyntaxNode

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property Declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
			Get
				Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._declarators, 2)
				variableDeclaratorSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(red, MyBase.GetChildIndex(2)))
				Return variableDeclaratorSyntaxes
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal declarators As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), modifiers, If(declarators IsNot Nothing, declarators.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitFieldDeclaration(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitFieldDeclaration(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Public Function AddDeclarators(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.WithDeclarators(Me.Declarators.AddRange(items))
		End Function

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 2) Then
				syntaxNode = Me._declarators
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				redAtZero = MyBase.GetRedAtZero(Me._attributeLists)
			ElseIf (num = 2) Then
				redAtZero = MyBase.GetRed(Me._declarators, 2)
			Else
				redAtZero = Nothing
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			If (attributeLists <> Me.AttributeLists OrElse modifiers <> Me.Modifiers OrElse declarators <> Me.Declarators) Then
				Dim fieldDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FieldDeclaration(attributeLists, modifiers, declarators)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				fieldDeclarationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, fieldDeclarationSyntax1, fieldDeclarationSyntax1.WithAnnotations(annotations))
			Else
				fieldDeclarationSyntax = Me
			End If
			Return fieldDeclarationSyntax
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.Update(attributeLists, Me.Modifiers, Me.Declarators)
		End Function

		Public Function WithDeclarators(ByVal declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.Update(Me.AttributeLists, Me.Modifiers, declarators)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax
			Return Me.Update(Me.AttributeLists, modifiers, Me.Declarators)
		End Function
	End Class
End Namespace