Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class OverloadResolutionResult(Of TMember As Symbol)
		Private ReadOnly _validResult As Nullable(Of MemberResolutionResult(Of TMember))

		Private ReadOnly _bestResult As Nullable(Of MemberResolutionResult(Of TMember))

		Private _results As ImmutableArray(Of MemberResolutionResult(Of TMember))

		Public ReadOnly Property BestResult As Nullable(Of MemberResolutionResult(Of TMember))
			Get
				Return Me._bestResult
			End Get
		End Property

		Public ReadOnly Property Results As ImmutableArray(Of MemberResolutionResult(Of TMember))
			Get
				Return Me._results
			End Get
		End Property

		Public ReadOnly Property Succeeded As Boolean
			Get
				Return Me.ValidResult.HasValue
			End Get
		End Property

		Public ReadOnly Property ValidResult As Nullable(Of MemberResolutionResult(Of TMember))
			Get
				Return Me._validResult
			End Get
		End Property

		Friend Sub New(ByVal results As ImmutableArray(Of MemberResolutionResult(Of TMember)), ByVal validResult As Nullable(Of MemberResolutionResult(Of TMember)), ByVal bestResult As Nullable(Of MemberResolutionResult(Of TMember)))
			MyBase.New()
			Me._results = results
			Me._validResult = validResult
			Me._bestResult = bestResult
		End Sub
	End Class
End Namespace