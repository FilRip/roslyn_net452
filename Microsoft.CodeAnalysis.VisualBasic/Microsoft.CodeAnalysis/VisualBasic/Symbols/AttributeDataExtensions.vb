Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module AttributeDataExtensions
		<Extension>
		Public Function IndexOfAttribute(ByVal attributes As ImmutableArray(Of VisualBasicAttributeData), ByVal targetSymbol As Symbol, ByVal description As AttributeDescription) As Integer
			Dim num As Integer
			Dim length As Integer = attributes.Length - 1
			Dim num1 As Integer = 0
			While True
				If (num1 > length) Then
					num = -1
					Exit While
				ElseIf (Not attributes(num1).IsTargetAttribute(targetSymbol, description)) Then
					num1 = num1 + 1
				Else
					num = num1
					Exit While
				End If
			End While
			Return num
		End Function
	End Module
End Namespace