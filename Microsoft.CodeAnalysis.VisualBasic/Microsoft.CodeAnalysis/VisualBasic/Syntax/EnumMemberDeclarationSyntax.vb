Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EnumMemberDeclarationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _attributeLists As SyntaxNode

		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax

		Public ReadOnly Property AttributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)(MyBase.GetRedAtZero(Me._attributeLists))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax)._identifier, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)(Me._initializer, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As SyntaxNode, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax(kind, errors, annotations, If(attributeLists IsNot Nothing, attributeLists.Green, Nothing), identifier, If(initializer IsNot Nothing, DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEnumMemberDeclaration(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEnumMemberDeclaration(Me)
		End Sub

		Public Function AddAttributeLists(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			Return Me.WithAttributeLists(Me.AttributeLists.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._attributeLists
			ElseIf (num = 2) Then
				syntaxNode = Me._initializer
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
				redAtZero = Me.Initializer
			Else
				redAtZero = Nothing
			End If
			Return redAtZero
		End Function

		Public Function Update(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax), ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			If (attributeLists <> Me.AttributeLists OrElse identifier <> Me.Identifier OrElse initializer <> Me.Initializer) Then
				Dim enumMemberDeclarationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EnumMemberDeclaration(attributeLists, identifier, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				enumMemberDeclarationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, enumMemberDeclarationSyntax1, enumMemberDeclarationSyntax1.WithAnnotations(annotations))
			Else
				enumMemberDeclarationSyntax = Me
			End If
			Return enumMemberDeclarationSyntax
		End Function

		Public Function WithAttributeLists(ByVal attributeLists As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			Return Me.Update(attributeLists, Me.Identifier, Me.Initializer)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			Return Me.Update(Me.AttributeLists, identifier, Me.Initializer)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax
			Return Me.Update(Me.AttributeLists, Me.Identifier, initializer)
		End Function
	End Class
End Namespace