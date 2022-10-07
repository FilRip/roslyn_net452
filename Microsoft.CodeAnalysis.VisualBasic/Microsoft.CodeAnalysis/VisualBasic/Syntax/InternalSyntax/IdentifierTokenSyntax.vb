Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class IdentifierTokenSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
		Friend MustOverride ReadOnly Property IdentifierText As String

		Friend MustOverride ReadOnly Property IsBracketed As Boolean

		Friend MustOverride ReadOnly Property PossibleKeywordKind As SyntaxKind

		Public Overrides ReadOnly Property RawContextualKind As Integer
			Get
				Return CInt(Me.PossibleKeywordKind)
			End Get
		End Property

		Friend MustOverride ReadOnly Property TypeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter

		Friend Overrides ReadOnly Property ValueText As String
			Get
				Return Me.IdentifierText
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode)
			MyBase.New(kind, errors, annotations, text, precedingTrivia, followingTrivia)
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
		End Sub
	End Class
End Namespace