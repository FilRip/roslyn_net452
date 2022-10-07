Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
	Friend Class Optimizer
		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function Optimize(ByVal container As Symbol, ByVal src As BoundStatement, ByVal debugFriendly As Boolean, <Out> ByRef stackLocals As HashSet(Of LocalSymbol)) As BoundStatement
			Return StackScheduler.OptimizeLocalsOut(container, src, debugFriendly, stackLocals)
		End Function
	End Class
End Namespace