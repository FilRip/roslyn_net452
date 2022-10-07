Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LazyObsoleteDiagnosticInfo
		Inherits DiagnosticInfo
		Private _lazyActualObsoleteDiagnostic As DiagnosticInfo

		Private ReadOnly _symbol As Symbol

		Private ReadOnly _containingSymbol As Symbol

		Friend Sub New(ByVal sym As Symbol, ByVal containingSymbol As Symbol)
			MyBase.New(MessageProvider.Instance, -1)
			Me._symbol = sym
			Me._containingSymbol = containingSymbol
		End Sub

		Friend Overrides Function GetResolvedInfo() As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			If (Me._lazyActualObsoleteDiagnostic Is Nothing) Then
				Me._symbol.ForceCompleteObsoleteAttribute()
				If (ObsoleteAttributeHelpers.GetObsoleteDiagnosticKind(Me._containingSymbol, Me._symbol, True) = ObsoleteDiagnosticKind.Diagnostic) Then
					diagnosticInfo = ObsoleteAttributeHelpers.CreateObsoleteDiagnostic(Me._symbol)
				Else
					diagnosticInfo = Nothing
				End If
				Dim voidDiagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = If(diagnosticInfo, ErrorFactory.VoidDiagnosticInfo)
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.DiagnosticInfo)(Me._lazyActualObsoleteDiagnostic, voidDiagnosticInfo, Nothing)
			End If
			Return Me._lazyActualObsoleteDiagnostic
		End Function
	End Class
End Namespace