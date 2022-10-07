Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LookupSymbolsInfo
		Inherits AbstractLookupSymbolsInfo(Of Symbol)
		Private Const s_poolSize As Integer = 64

		Private ReadOnly Shared s_pool As ObjectPool(Of LookupSymbolsInfo)

		Shared Sub New()
			LookupSymbolsInfo.s_pool = New ObjectPool(Of LookupSymbolsInfo)(Function() New LookupSymbolsInfo(), 64)
		End Sub

		Private Sub New()
			MyBase.New(CaseInsensitiveComparison.Comparer)
		End Sub

		Public Sub Free()
			MyBase.Clear()
			LookupSymbolsInfo.s_pool.Free(Me)
		End Sub

		Public Shared Function GetInstance() As LookupSymbolsInfo
			Return LookupSymbolsInfo.s_pool.Allocate()
		End Function
	End Class
End Namespace