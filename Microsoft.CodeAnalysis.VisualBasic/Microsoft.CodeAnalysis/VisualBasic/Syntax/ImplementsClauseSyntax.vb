Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ImplementsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _interfaceMembers As SyntaxNode

		Public ReadOnly Property ImplementsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)._implementsKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property InterfaceMembers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
			Get
				Dim qualifiedNameSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._interfaceMembers, 1)
				qualifiedNameSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)(red, MyBase.GetChildIndex(1)))
				Return qualifiedNameSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal implementsKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal interfaceMembers As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax(kind, errors, annotations, implementsKeyword, If(interfaceMembers IsNot Nothing, interfaceMembers.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitImplementsClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitImplementsClause(Me)
		End Sub

		Public Function AddInterfaceMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Return Me.WithInterfaceMembers(Me.InterfaceMembers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._interfaceMembers
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._interfaceMembers, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal implementsKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal interfaceMembers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			If (implementsKeyword <> Me.ImplementsKeyword OrElse interfaceMembers <> Me.InterfaceMembers) Then
				Dim implementsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImplementsClause(implementsKeyword, interfaceMembers)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				implementsClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, implementsClauseSyntax1, implementsClauseSyntax1.WithAnnotations(annotations))
			Else
				implementsClauseSyntax = Me
			End If
			Return implementsClauseSyntax
		End Function

		Public Function WithImplementsKeyword(ByVal implementsKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Return Me.Update(implementsKeyword, Me.InterfaceMembers)
		End Function

		Public Function WithInterfaceMembers(ByVal interfaceMembers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax
			Return Me.Update(Me.ImplementsKeyword, interfaceMembers)
		End Function
	End Class
End Namespace