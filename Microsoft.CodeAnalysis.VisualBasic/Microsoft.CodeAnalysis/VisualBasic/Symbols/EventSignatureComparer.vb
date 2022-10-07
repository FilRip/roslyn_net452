Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class EventSignatureComparer
		Implements IEqualityComparer(Of EventSymbol)
		Public ReadOnly Shared ExplicitEventImplementationComparer As EventSignatureComparer

		Public ReadOnly Shared ExplicitEventImplementationWithTupleNamesComparer As EventSignatureComparer

		Public ReadOnly Shared OverrideSignatureComparer As EventSignatureComparer

		Public ReadOnly Shared RuntimeEventSignatureComparer As EventSignatureComparer

		Public ReadOnly Shared WinRTConflictComparer As EventSignatureComparer

		Private ReadOnly _considerName As Boolean

		Private ReadOnly _considerType As Boolean

		Private ReadOnly _considerCustomModifiers As Boolean

		Private ReadOnly _considerTupleNames As Boolean

		Shared Sub New()
			EventSignatureComparer.ExplicitEventImplementationComparer = New EventSignatureComparer(False, False, False, False)
			EventSignatureComparer.ExplicitEventImplementationWithTupleNamesComparer = New EventSignatureComparer(False, False, False, True)
			EventSignatureComparer.OverrideSignatureComparer = New EventSignatureComparer(True, False, False, False)
			EventSignatureComparer.RuntimeEventSignatureComparer = New EventSignatureComparer(True, True, True, False)
			EventSignatureComparer.WinRTConflictComparer = New EventSignatureComparer(True, False, False, False)
		End Sub

		Private Sub New(ByVal considerName As Boolean, ByVal considerType As Boolean, ByVal considerCustomModifiers As Boolean, ByVal considerTupleNames As Boolean)
			MyBase.New()
			Me._considerName = considerName
			Me._considerType = considerType
			Me._considerCustomModifiers = considerCustomModifiers
			Me._considerTupleNames = considerTupleNames
		End Sub

		Public Function Equals(ByVal event1 As EventSymbol, ByVal event2 As EventSymbol) As Boolean Implements IEqualityComparer(Of EventSymbol).Equals
			Dim flag As Boolean
			If (event1 = event2) Then
				flag = True
			ElseIf (event1 Is Nothing OrElse event2 Is Nothing) Then
				flag = False
			ElseIf (Not Me._considerName OrElse CaseInsensitiveComparison.Equals(event1.Name, event2.Name)) Then
				If (Me._considerType) Then
					Dim typeCompareKind As Microsoft.CodeAnalysis.TypeCompareKind = MethodSignatureComparer.MakeTypeCompareKind(Me._considerCustomModifiers, Me._considerTupleNames)
					If (event1.Type.IsSameType(event2.Type, typeCompareKind)) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = If((event1.DelegateParameters.Length > 0 OrElse event2.DelegateParameters.Length > 0) AndAlso Not MethodSignatureComparer.HaveSameParameterTypes(event1.DelegateParameters, Nothing, event2.DelegateParameters, Nothing, True, Me._considerCustomModifiers, Me._considerTupleNames), False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Function GetHashCode(ByVal [event] As EventSymbol) As Integer Implements IEqualityComparer(Of EventSymbol).GetHashCode
			Dim num As Integer = 1
			If ([event] IsNot Nothing) Then
				If (Me._considerName) Then
					num = Hash.Combine(Of String)([event].Name, num)
				End If
				If (Me._considerType AndAlso Not Me._considerCustomModifiers) Then
					num = Hash.Combine(Of TypeSymbol)([event].Type, num)
				End If
				num = Hash.Combine(num, [event].DelegateParameters.Length)
			End If
			Return num
		End Function
	End Class
End Namespace