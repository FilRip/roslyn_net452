Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class IntegerLiteralTokenSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
		Friend ReadOnly _base As LiteralBase

		Friend ReadOnly _typeSuffix As TypeCharacter

		Friend ReadOnly Property Base As LiteralBase
			Get
				Return Me._base
			End Get
		End Property

		Friend ReadOnly Property TypeSuffix As TypeCharacter
			Get
				Return Me._typeSuffix
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal base As LiteralBase, ByVal typeSuffix As TypeCharacter)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			Me._base = base
			Me._typeSuffix = typeSuffix
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal base As LiteralBase, ByVal typeSuffix As TypeCharacter)
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
			Me._base = base
			Me._typeSuffix = typeSuffix
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._base = reader.ReadByte()
			Me._typeSuffix = reader.ReadByte()
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteByte(CByte(Me._base))
			writer.WriteByte(CByte(Me._typeSuffix))
		End Sub
	End Class
End Namespace