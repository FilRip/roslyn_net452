Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BasesBeingResolvedBinder
		Inherits Binder
		Public Sub New(ByVal containingBinder As Binder, ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved)
			MyBase.New(containingBinder, basesBeingResolved)
		End Sub

		Public Overrides Function CheckAccessibility(ByVal sym As Symbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal accessThroughType As TypeSymbol = Nothing, Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			Dim enumerator As ConsList(Of TypeSymbol).Enumerator = New ConsList(Of TypeSymbol).Enumerator()
			Dim enumerator1 As ConsList(Of TypeSymbol).Enumerator = New ConsList(Of TypeSymbol).Enumerator()
			Dim basesBeingResolved1 As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = MyBase.BasesBeingResolved()
			Try
				Dim inheritsBeingResolvedOpt As [Object] = basesBeingResolved.InheritsBeingResolvedOpt
				If (inheritsBeingResolvedOpt Is Nothing) Then
					inheritsBeingResolvedOpt = ConsList(Of TypeSymbol).Empty
				End If
				enumerator = DirectCast(inheritsBeingResolvedOpt, ConsList(Of TypeSymbol)).GetEnumerator()
				While enumerator.MoveNext()
					basesBeingResolved1 = basesBeingResolved1.PrependInheritsBeingResolved(enumerator.Current)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Try
				Dim implementsBeingResolvedOpt As [Object] = basesBeingResolved.ImplementsBeingResolvedOpt
				If (implementsBeingResolvedOpt Is Nothing) Then
					implementsBeingResolvedOpt = ConsList(Of TypeSymbol).Empty
				End If
				enumerator1 = DirectCast(implementsBeingResolvedOpt, ConsList(Of TypeSymbol)).GetEnumerator()
				While enumerator1.MoveNext()
					basesBeingResolved1 = basesBeingResolved1.PrependImplementsBeingResolved(enumerator1.Current)
				End While
			Finally
				DirectCast(enumerator1, IDisposable).Dispose()
			End Try
			Return Me.m_containingBinder.CheckAccessibility(sym, useSiteInfo, accessThroughType, basesBeingResolved1)
		End Function
	End Class
End Namespace