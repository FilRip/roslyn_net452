Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module SourceMemberFlagsExtensions
		<Extension>
		Friend Function ToMethodKind(ByVal flags As SourceMemberFlags) As MethodKind
			Return DirectCast((CInt(flags) >> CInt(SourceMemberFlags.MethodKindShift) And CInt(SourceMemberFlags.MethodKindMask)), MethodKind)
		End Function
	End Module
End Namespace