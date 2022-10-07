Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class EqualsIgnoringComparer
		Inherits EqualityComparer(Of TypeSymbol)
		Private ReadOnly _comparison As TypeCompareKind

		Public ReadOnly Shared Property InstanceCLRSignatureCompare As EqualsIgnoringComparer

		Public ReadOnly Shared Property InstanceIgnoringTupleNames As EqualsIgnoringComparer

		Shared Sub New()
			EqualsIgnoringComparer.InstanceIgnoringTupleNames = New EqualsIgnoringComparer(TypeCompareKind.IgnoreTupleNames)
			EqualsIgnoringComparer.InstanceCLRSignatureCompare = New EqualsIgnoringComparer(TypeCompareKind.IgnoreTupleNames)
		End Sub

		Public Sub New(ByVal comparison As TypeCompareKind)
			MyBase.New()
			Me._comparison = comparison
		End Sub

		Public Overrides Function Equals(ByVal type1 As TypeSymbol, ByVal type2 As TypeSymbol) As Boolean
			If (type1 Is Nothing) Then
				Return type2 Is Nothing
			End If
			Return type1.IsSameType(type2, Me._comparison)
		End Function

		Public Overrides Function GetHashCode(ByVal obj As TypeSymbol) As Integer
			If (obj Is Nothing) Then
				Return 0
			End If
			Return obj.GetHashCode()
		End Function
	End Class
End Namespace