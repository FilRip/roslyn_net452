Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure SingleLookupResult
		Friend ReadOnly Kind As LookupResultKind

		Friend ReadOnly Symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol

		Friend ReadOnly Diagnostic As DiagnosticInfo

		Public ReadOnly Shared Empty As SingleLookupResult

		Public ReadOnly Shared EmptyAndStopLookup As SingleLookupResult

		Public ReadOnly Property HasDiagnostic As Boolean
			Get
				Return Me.Diagnostic IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property IsAmbiguous As Boolean
			Get
				Return Me.Kind = LookupResultKind.Ambiguous
			End Get
		End Property

		Public ReadOnly Property IsGood As Boolean
			Get
				Return Me.Kind = LookupResultKind.Good
			End Get
		End Property

		Public ReadOnly Property IsGoodOrAmbiguous As Boolean
			Get
				If (Me.Kind = LookupResultKind.Good) Then
					Return True
				End If
				Return Me.Kind = LookupResultKind.Ambiguous
			End Get
		End Property

		Public ReadOnly Property StopFurtherLookup As Boolean
			Get
				Return Me.Kind >= LookupResultKind.WrongArityAndStopLookup
			End Get
		End Property

		Shared Sub New()
			SingleLookupResult.Empty = New SingleLookupResult(LookupResultKind.Empty, Nothing, Nothing)
			SingleLookupResult.EmptyAndStopLookup = New SingleLookupResult(LookupResultKind.EmptyAndStopLookup, Nothing, Nothing)
		End Sub

		Friend Sub New(ByVal kind As LookupResultKind, ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagInfo As DiagnosticInfo)
			Me = New SingleLookupResult() With
			{
				.Kind = kind,
				.Symbol = symbol,
				.Diagnostic = diagInfo
			}
		End Sub

		Public Shared Function Ambiguous(ByVal syms As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal generateAmbiguityDiagnostic As Func(Of ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), AmbiguousSymbolDiagnostic)) As SingleLookupResult
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = generateAmbiguityDiagnostic(syms)
			Return New SingleLookupResult(LookupResultKind.Ambiguous, syms.First(), diagnosticInfo)
		End Function

		Public Shared Function Good(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.Good, sym, Nothing)
		End Function

		Public Shared Function Inaccessible(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagInfo As DiagnosticInfo) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.Inaccessible, sym, diagInfo)
		End Function

		Public Shared Function MustBeInstance(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.MustBeInstance, sym, Nothing)
		End Function

		Public Shared Function MustNotBeInstance(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal err As ERRID) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.MustNotBeInstance, sym, New BadSymbolDiagnostic(sym, err))
		End Function

		Friend Shared Function NotAnAttributeType(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal [error] As DiagnosticInfo) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.NotAnAttributeType, sym, [error])
		End Function

		Public Shared Function WrongArity(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagInfo As DiagnosticInfo) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.WrongArity, sym, diagInfo)
		End Function

		Public Shared Function WrongArity(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal err As ERRID) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.WrongArity, sym, New BadSymbolDiagnostic(sym, err))
		End Function

		Public Shared Function WrongArityAndStopLookup(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal err As ERRID) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.WrongArityAndStopLookup, sym, New BadSymbolDiagnostic(sym, err))
		End Function

		Public Shared Function WrongArityAndStopLookup(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagInfo As DiagnosticInfo) As SingleLookupResult
			Return New SingleLookupResult(LookupResultKind.WrongArityAndStopLookup, sym, diagInfo)
		End Function
	End Structure
End Namespace