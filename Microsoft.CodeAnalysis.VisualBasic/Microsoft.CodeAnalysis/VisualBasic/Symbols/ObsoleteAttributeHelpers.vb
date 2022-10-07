Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports System
Imports System.Reflection.Metadata
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class ObsoleteAttributeHelpers
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function CreateObsoleteDiagnostic(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim obsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData = symbol.ObsoleteAttributeData
			If (obsoleteAttributeData Is Nothing) Then
				diagnosticInfo = Nothing
			ElseIf (obsoleteAttributeData.Kind = ObsoleteAttributeKind.Experimental) Then
				diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.WRN_Experimental, New [Object]() { New FormattedSymbol(symbol, SymbolDisplayFormat.VisualBasicErrorMessageFormat) })
			ElseIf (Not symbol.IsAccessor() OrElse DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).AssociatedSymbol.Kind <> SymbolKind.[Property]) Then
				diagnosticInfo = If(Not [String].IsNullOrEmpty(obsoleteAttributeData.Message), ErrorFactory.ObsoleteErrorInfo(If(obsoleteAttributeData.IsError, ERRID.ERR_UseOfObsoleteSymbol2, ERRID.WRN_UseOfObsoleteSymbol2), obsoleteAttributeData, New [Object]() { symbol, obsoleteAttributeData.Message }), ErrorFactory.ObsoleteErrorInfo(If(obsoleteAttributeData.IsError, ERRID.ERR_UseOfObsoleteSymbolNoMessage1, ERRID.WRN_UseOfObsoleteSymbolNoMessage1), obsoleteAttributeData, New [Object]() { symbol }))
			Else
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				Dim str As String = If(methodSymbol.MethodKind = MethodKind.PropertyGet, "Get", "Set")
				diagnosticInfo = If(Not [String].IsNullOrEmpty(obsoleteAttributeData.Message), ErrorFactory.ObsoleteErrorInfo(If(obsoleteAttributeData.IsError, ERRID.ERR_UseOfObsoletePropertyAccessor3, ERRID.WRN_UseOfObsoletePropertyAccessor3), obsoleteAttributeData, New [Object]() { str, methodSymbol.AssociatedSymbol, obsoleteAttributeData.Message }), ErrorFactory.ObsoleteErrorInfo(If(obsoleteAttributeData.IsError, ERRID.ERR_UseOfObsoletePropertyAccessor2, ERRID.WRN_UseOfObsoletePropertyAccessor2), obsoleteAttributeData, New [Object]() { str, methodSymbol.AssociatedSymbol }))
			End If
			Return diagnosticInfo
		End Function

		Private Shared Function GetObsoleteContextState(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal forceComplete As Boolean) As Microsoft.CodeAnalysis.ThreeState
			Dim threeState As Microsoft.CodeAnalysis.ThreeState
			While True
				If (symbol IsNot Nothing) Then
					If (forceComplete) Then
						symbol.ForceCompleteObsoleteAttribute()
					End If
					Dim obsoleteState As Microsoft.CodeAnalysis.ThreeState = symbol.ObsoleteState
					If (obsoleteState <> Microsoft.CodeAnalysis.ThreeState.[False]) Then
						threeState = obsoleteState
						Exit While
					ElseIf (Not symbol.IsAccessor()) Then
						symbol = symbol.ContainingSymbol
					Else
						symbol = DirectCast(symbol, MethodSymbol).AssociatedSymbol
					End If
				Else
					threeState = Microsoft.CodeAnalysis.ThreeState.[False]
					Exit While
				End If
			End While
			Return threeState
		End Function

		Friend Shared Function GetObsoleteDataFromMetadata(ByVal token As EntityHandle, ByVal containingModule As PEModuleSymbol) As ObsoleteAttributeData
			Return containingModule.[Module].TryGetDeprecatedOrExperimentalOrObsoleteAttribute(token, New MetadataDecoder(containingModule), False)
		End Function

		Friend Shared Function GetObsoleteDiagnosticKind(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, Optional ByVal forceComplete As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind
			Dim obsoleteDiagnosticKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind
			Select Case symbol.ObsoleteKind
				Case ObsoleteAttributeKind.None
					obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.NotObsolete
					Exit Select
				Case ObsoleteAttributeKind.Uninitialized
					obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.Lazy
					Exit Select
				Case ObsoleteAttributeKind.Obsolete
				Case ObsoleteAttributeKind.Deprecated
				Label0:
					Dim obsoleteContextState As ThreeState = ObsoleteAttributeHelpers.GetObsoleteContextState(context, forceComplete)
					If (obsoleteContextState = ThreeState.[False]) Then
						obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.Diagnostic
						Exit Select
					ElseIf (obsoleteContextState = ThreeState.[True]) Then
						obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.Suppressed
						Exit Select
					Else
						obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.LazyPotentiallySuppressed
						Exit Select
					End If
				Case ObsoleteAttributeKind.Experimental
					obsoleteDiagnosticKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.ObsoleteDiagnosticKind.Diagnostic
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return obsoleteDiagnosticKind
		End Function

		Friend Shared Sub InitializeObsoleteDataFromMetadata(ByRef data As ObsoleteAttributeData, ByVal token As EntityHandle, ByVal containingModule As PEModuleSymbol)
			If (data = ObsoleteAttributeData.Uninitialized) Then
				Dim obsoleteDataFromMetadata As ObsoleteAttributeData = ObsoleteAttributeHelpers.GetObsoleteDataFromMetadata(token, containingModule)
				Interlocked.CompareExchange(Of ObsoleteAttributeData)(data, obsoleteDataFromMetadata, ObsoleteAttributeData.Uninitialized)
			End If
		End Sub
	End Class
End Namespace