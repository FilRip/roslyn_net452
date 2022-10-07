Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ImportAliasClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)._equalsToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax, ByVal equalsToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax(kind, errors, annotations, identifier, equalsToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitImportAliasClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitImportAliasClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax
			Dim importAliasClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax
			If (identifier <> Me.Identifier OrElse equalsToken <> Me.EqualsToken) Then
				Dim importAliasClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImportAliasClause(identifier, equalsToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				importAliasClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, importAliasClauseSyntax1, importAliasClauseSyntax1.WithAnnotations(annotations))
			Else
				importAliasClauseSyntax = Me
			End If
			Return importAliasClauseSyntax
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax
			Return Me.Update(Me.Identifier, equalsToken)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax
			Return Me.Update(identifier, Me.EqualsToken)
		End Function
	End Class
End Namespace