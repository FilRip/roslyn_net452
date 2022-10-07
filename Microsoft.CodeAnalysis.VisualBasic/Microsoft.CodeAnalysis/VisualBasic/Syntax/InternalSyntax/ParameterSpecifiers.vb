Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	<Flags>
	Friend Enum ParameterSpecifiers
		[ByRef] = 1
		[ByVal] = 2
		[Optional] = 4
		[ParamArray] = 8
	End Enum
End Namespace