Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module EmbeddedSymbolExtensions
		<Extension>
		Public Function GetEmbeddedKind(ByVal tree As SyntaxTree) As EmbeddedSymbolKind
			Return EmbeddedSymbolManager.GetEmbeddedKind(tree)
		End Function

		<Extension>
		Public Function IsEmbeddedOrMyTemplateLocation(ByVal location As Microsoft.CodeAnalysis.Location) As Boolean
			If (TypeOf location Is EmbeddedTreeLocation) Then
				Return True
			End If
			Return TypeOf location Is MyTemplateLocation
		End Function

		<Extension>
		Public Function IsEmbeddedOrMyTemplateTree(ByVal tree As SyntaxTree) As Boolean
			Dim visualBasicSyntaxTree As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree = TryCast(tree, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree)
			If (visualBasicSyntaxTree IsNot Nothing AndAlso visualBasicSyntaxTree.IsMyTemplate) Then
				Return True
			End If
			Return visualBasicSyntaxTree.IsEmbeddedSyntaxTree()
		End Function

		<Extension>
		Public Function IsEmbeddedSyntaxTree(ByVal tree As SyntaxTree) As Boolean
			Return EmbeddedSymbolManager.GetEmbeddedKind(tree) <> EmbeddedSymbolKind.None
		End Function
	End Module
End Namespace