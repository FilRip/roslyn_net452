Imports Microsoft.CodeAnalysis.PooledObjects
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Interface IDiagnosticInfoWithSymbols
		Sub GetAssociatedSymbols(ByVal builder As ArrayBuilder(Of Symbol))
	End Interface
End Namespace