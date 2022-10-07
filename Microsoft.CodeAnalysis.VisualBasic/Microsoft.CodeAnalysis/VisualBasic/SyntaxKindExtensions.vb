Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module SyntaxKindExtensions
		<Extension>
		Public Function Contains(ByVal kinds As SyntaxKind(), ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKindArray As SyntaxKind() = kinds
			Dim num As Integer = 0
			While True
				If (num >= CInt(syntaxKindArray.Length)) Then
					flag = False
					Exit While
				ElseIf (syntaxKindArray(num) <> kind) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function
	End Module
End Namespace