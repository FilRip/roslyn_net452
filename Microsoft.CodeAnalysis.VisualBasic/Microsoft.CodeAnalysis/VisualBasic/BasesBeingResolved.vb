Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure BasesBeingResolved
		Public ReadOnly InheritsBeingResolvedOpt As ConsList(Of TypeSymbol)

		Public ReadOnly ImplementsBeingResolvedOpt As ConsList(Of TypeSymbol)

		Public ReadOnly Shared Property Empty As BasesBeingResolved
			Get
				Return New BasesBeingResolved()
			End Get
		End Property

		Public Sub New(ByVal inheritsBeingResolved As ConsList(Of TypeSymbol), ByVal implementsBeingResolved As ConsList(Of TypeSymbol))
			Me = New BasesBeingResolved() With
			{
				.InheritsBeingResolvedOpt = inheritsBeingResolved,
				.ImplementsBeingResolvedOpt = implementsBeingResolved
			}
		End Sub

		Public Function PrependImplementsBeingResolved(ByVal symbol As TypeSymbol) As BasesBeingResolved
			Dim inheritsBeingResolvedOpt As ConsList(Of TypeSymbol) = Me.InheritsBeingResolvedOpt
			Dim implementsBeingResolvedOpt As [Object] = Me.ImplementsBeingResolvedOpt
			If (implementsBeingResolvedOpt Is Nothing) Then
				implementsBeingResolvedOpt = ConsList(Of TypeSymbol).Empty
			End If
			Return New BasesBeingResolved(inheritsBeingResolvedOpt, DirectCast(implementsBeingResolvedOpt, ConsList(Of TypeSymbol)).Prepend(symbol))
		End Function

		Public Function PrependInheritsBeingResolved(ByVal symbol As TypeSymbol) As BasesBeingResolved
			Dim inheritsBeingResolvedOpt As [Object] = Me.InheritsBeingResolvedOpt
			If (inheritsBeingResolvedOpt Is Nothing) Then
				inheritsBeingResolvedOpt = ConsList(Of TypeSymbol).Empty
			End If
			Return New BasesBeingResolved(DirectCast(inheritsBeingResolvedOpt, ConsList(Of TypeSymbol)).Prepend(symbol), Me.ImplementsBeingResolvedOpt)
		End Function
	End Structure
End Namespace