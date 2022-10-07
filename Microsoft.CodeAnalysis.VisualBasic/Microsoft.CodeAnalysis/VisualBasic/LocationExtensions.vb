Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module LocationExtensions
		<Extension>
		Public Function EmbeddedKind(ByVal location As Microsoft.CodeAnalysis.Location) As EmbeddedSymbolKind
			Dim vBLocation As Microsoft.CodeAnalysis.VisualBasic.VBLocation = TryCast(location, Microsoft.CodeAnalysis.VisualBasic.VBLocation)
			Return If(vBLocation Is Nothing, EmbeddedSymbolKind.None, vBLocation.EmbeddedKind)
		End Function

		<Extension>
		Public Function PossiblyEmbeddedOrMySourceSpan(ByVal location As Microsoft.CodeAnalysis.Location) As TextSpan
			Dim vBLocation As Microsoft.CodeAnalysis.VisualBasic.VBLocation = TryCast(location, Microsoft.CodeAnalysis.VisualBasic.VBLocation)
			Return If(vBLocation Is Nothing, location.SourceSpan, vBLocation.PossiblyEmbeddedOrMySourceSpan)
		End Function

		<Extension>
		Public Function PossiblyEmbeddedOrMySourceTree(ByVal location As Microsoft.CodeAnalysis.Location) As SyntaxTree
			Dim sourceTree As SyntaxTree
			Dim vBLocation As Microsoft.CodeAnalysis.VisualBasic.VBLocation = TryCast(location, Microsoft.CodeAnalysis.VisualBasic.VBLocation)
			If (vBLocation Is Nothing) Then
				sourceTree = DirectCast(location.SourceTree, VisualBasicSyntaxTree)
			Else
				sourceTree = vBLocation.PossiblyEmbeddedOrMySourceTree
			End If
			Return sourceTree
		End Function
	End Module
End Namespace