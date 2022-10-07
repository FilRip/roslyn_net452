Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module GeneratedExtensionSyntaxFacts
		<Extension>
		Public Function GetText(ByVal kind As SyntaxKind) As String
			Return SyntaxFacts.GetText(kind)
		End Function
	End Module
End Namespace