Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NamespaceStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)(Me._name, 1)
			End Get
		End Property

		Public ReadOnly Property NamespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)._namespaceKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal namespaceKeyword As KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax(kind, errors, annotations, namespaceKeyword, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNamespaceStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNamespaceStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._name
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			If (i <> 1) Then
				name = Nothing
			Else
				name = Me.Name
			End If
			Return name
		End Function

		Public Function Update(ByVal namespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax
			If (namespaceKeyword <> Me.NamespaceKeyword OrElse name <> Me.Name) Then
				Dim namespaceStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NamespaceStatement(namespaceKeyword, name)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				namespaceStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, namespaceStatementSyntax1, namespaceStatementSyntax1.WithAnnotations(annotations))
			Else
				namespaceStatementSyntax = Me
			End If
			Return namespaceStatementSyntax
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax
			Return Me.Update(Me.NamespaceKeyword, name)
		End Function

		Public Function WithNamespaceKeyword(ByVal namespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax
			Return Me.Update(namespaceKeyword, Me.Name)
		End Function
	End Class
End Namespace