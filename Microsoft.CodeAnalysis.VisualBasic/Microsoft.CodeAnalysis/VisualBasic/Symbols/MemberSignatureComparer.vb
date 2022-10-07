Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MemberSignatureComparer
		Implements IEqualityComparer(Of Symbol)
		Public ReadOnly Shared WinRTComparer As MemberSignatureComparer

		Private ReadOnly _methodComparer As MethodSignatureComparer

		Private ReadOnly _propertyComparer As PropertySignatureComparer

		Private ReadOnly _eventComparer As EventSignatureComparer

		Shared Sub New()
			MemberSignatureComparer.WinRTComparer = New MemberSignatureComparer(MethodSignatureComparer.WinRTConflictComparer, PropertySignatureComparer.WinRTConflictComparer, EventSignatureComparer.WinRTConflictComparer)
		End Sub

		Private Sub New(ByVal methodComparer As MethodSignatureComparer, ByVal propertyComparer As PropertySignatureComparer, ByVal eventComparer As EventSignatureComparer)
			MyBase.New()
			Me._methodComparer = methodComparer
			Me._propertyComparer = propertyComparer
			Me._eventComparer = eventComparer
		End Sub

		<Conditional("DEBUG")>
		Private Sub CheckSymbolKind(ByVal sym As Symbol)
			Dim kind As SymbolKind = sym.Kind
			If (kind <> SymbolKind.[Event] AndAlso kind <> SymbolKind.Method) Then
			End If
		End Sub

		Public Function Equals(ByVal sym1 As Symbol, ByVal sym2 As Symbol) As Boolean Implements IEqualityComparer(Of Symbol).Equals
			Dim flag As Boolean
			If (sym1.Kind = sym2.Kind) Then
				Dim kind As SymbolKind = sym1.Kind
				If (kind = SymbolKind.[Event]) Then
					flag = Me._eventComparer.Equals(DirectCast(sym1, EventSymbol), DirectCast(sym2, EventSymbol))
				ElseIf (kind = SymbolKind.Method) Then
					flag = Me._methodComparer.Equals(DirectCast(sym1, MethodSymbol), DirectCast(sym2, MethodSymbol))
				Else
					flag = If(kind = SymbolKind.[Property], Me._propertyComparer.Equals(DirectCast(sym1, PropertySymbol), DirectCast(sym2, PropertySymbol)), False)
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Function GetHashCode(ByVal sym As Symbol) As Integer Implements IEqualityComparer(Of Symbol).GetHashCode
			Dim hashCode As Integer
			Dim kind As SymbolKind = sym.Kind
			If (kind = SymbolKind.[Event]) Then
				hashCode = Me._eventComparer.GetHashCode(DirectCast(sym, EventSymbol))
			ElseIf (kind = SymbolKind.Method) Then
				hashCode = Me._methodComparer.GetHashCode(DirectCast(sym, MethodSymbol))
			Else
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(sym.Kind)
				End If
				hashCode = Me._propertyComparer.GetHashCode(DirectCast(sym, PropertySymbol))
			End If
			Return hashCode
		End Function
	End Class
End Namespace