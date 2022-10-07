Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module OptionStrictEnumBounds
		<Extension>
		Friend Function IsValid(ByVal value As OptionStrict) As Boolean
			If (value < OptionStrict.Off) Then
				Return False
			End If
			Return value <= OptionStrict.[On]
		End Function
	End Module
End Namespace