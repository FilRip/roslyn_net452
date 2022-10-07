Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum SourceParameterFlags As Byte
		[ByVal] = 1
		[ByRef] = 2
		[Optional] = 4
		[ParamArray] = 8
	End Enum
End Namespace