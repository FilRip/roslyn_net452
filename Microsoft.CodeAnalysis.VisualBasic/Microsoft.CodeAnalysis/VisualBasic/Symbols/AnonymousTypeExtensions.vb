Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module AnonymousTypeExtensions
		<Extension>
		Friend Function IsSubDescription(ByVal fields As ImmutableArray(Of AnonymousTypeField)) As Boolean
			Return CObj(fields.Last().Name) = CObj("Sub")
		End Function
	End Module
End Namespace