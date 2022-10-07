Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class StructuredTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me.Initialize()
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind)
			MyBase.New(kind)
			Me.Initialize()
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			Me.Initialize()
			MyBase.SetFactoryContext(context)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation())
			MyBase.New(kind, errors, annotations)
			Me.Initialize()
		End Sub

		Private Sub Initialize()
			MyBase.SetFlags(GreenNode.NodeFlags.ContainsStructuredTrivia)
			If (MyBase.Kind = SyntaxKind.SkippedTokensTrivia) Then
				MyBase.SetFlags(GreenNode.NodeFlags.ContainsSkippedText)
			End If
		End Sub
	End Class
End Namespace