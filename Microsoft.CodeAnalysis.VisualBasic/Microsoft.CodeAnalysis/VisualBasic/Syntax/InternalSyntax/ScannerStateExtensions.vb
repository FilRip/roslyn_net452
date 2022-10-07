Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module ScannerStateExtensions
		<Extension>
		Friend Function IsVBState(ByVal state As ScannerState) As Boolean
			Return state <= ScannerState.VBAllowLeadingMultilineTrivia
		End Function
	End Module
End Namespace