Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SymbolInfoFactory
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function Create(ByVal symbols As ImmutableArray(Of Symbol), ByVal resultKind As LookupResultKind) As SymbolInfo
			Return SymbolInfoFactory.Create(StaticCast(Of ISymbol).From(Of Symbol)(symbols), resultKind)
		End Function

		Friend Shared Function Create(ByVal symbols As ImmutableArray(Of ISymbol), ByVal resultKind As LookupResultKind) As SymbolInfo
			Return SymbolInfoFactory.Create(symbols, If(resultKind = LookupResultKind.Good, CandidateReason.None, resultKind.ToCandidateReason()))
		End Function

		Friend Shared Function Create(ByVal symbols As ImmutableArray(Of ISymbol), ByVal reason As CandidateReason) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			symbols = ImmutableArrayExtensions.NullToEmpty(Of ISymbol)(symbols)
			If (symbols.IsEmpty AndAlso reason <> CandidateReason.None AndAlso reason <> CandidateReason.LateBound) Then
				reason = CandidateReason.None
			End If
			symbolInfo = If(symbols.Length <> 1 OrElse reason <> CandidateReason.None AndAlso reason <> CandidateReason.LateBound, New Microsoft.CodeAnalysis.SymbolInfo(symbols, reason), New Microsoft.CodeAnalysis.SymbolInfo(symbols(0), reason))
			Return symbolInfo
		End Function
	End Class
End Namespace