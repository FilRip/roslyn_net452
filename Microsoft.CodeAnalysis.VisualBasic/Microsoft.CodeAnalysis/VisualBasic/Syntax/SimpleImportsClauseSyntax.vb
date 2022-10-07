Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SimpleImportsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax
		Friend _alias As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax

		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax

		Public ReadOnly Property [Alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax)(Me._alias)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)(Me._name, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal [alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax(kind, errors, annotations, If([alias] IsNot Nothing, DirectCast([alias].Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax), Nothing), DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSimpleImportsClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSimpleImportsClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._alias
			ElseIf (num = 1) Then
				syntaxNode = Me._name
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim [alias] As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				[alias] = Me.[Alias]
			ElseIf (num = 1) Then
				[alias] = Me.Name
			Else
				[alias] = Nothing
			End If
			Return [alias]
		End Function

		Public Function Update(ByVal [alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax
			Dim simpleImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax
			If ([alias] <> Me.[Alias] OrElse name <> Me.Name) Then
				Dim simpleImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SimpleImportsClause([alias], name)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				simpleImportsClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, simpleImportsClauseSyntax1, simpleImportsClauseSyntax1.WithAnnotations(annotations))
			Else
				simpleImportsClauseSyntax = Me
			End If
			Return simpleImportsClauseSyntax
		End Function

		Public Function WithAlias(ByVal [alias] As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax
			Return Me.Update([alias], Me.Name)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax
			Return Me.Update(Me.[Alias], name)
		End Function
	End Class
End Namespace