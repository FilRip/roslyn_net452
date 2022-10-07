Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class IdentifierNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
		Public Shadows ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax(kind, errors, annotations, identifier), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitIdentifierName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitIdentifierName(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetIdentifierCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.Identifier
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			If (identifier = Me.Identifier) Then
				identifierNameSyntax = Me
			Else
				Dim identifierNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.IdentifierName(identifier)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				identifierNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, identifierNameSyntax1, identifierNameSyntax1.WithAnnotations(annotations))
			End If
			Return identifierNameSyntax
		End Function

		Public Shadows Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Return Me.Update(identifier)
		End Function

		Friend Overrides Function WithIdentifierCore(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
			Return Me.WithIdentifier(identifier)
		End Function
	End Class
End Namespace