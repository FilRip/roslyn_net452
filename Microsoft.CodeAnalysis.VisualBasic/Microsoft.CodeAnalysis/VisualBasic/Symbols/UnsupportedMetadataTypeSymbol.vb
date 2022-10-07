Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class UnsupportedMetadataTypeSymbol
		Inherits ErrorTypeSymbol
		Private ReadOnly _mrEx As BadImageFormatException

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Return ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, New [Object]() { [String].Empty })
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Sub New(Optional ByVal mrEx As BadImageFormatException = Nothing)
			MyBase.New()
			Me._mrEx = mrEx
		End Sub

		Public Sub New(ByVal explanation As String)
			MyBase.New()
		End Sub
	End Class
End Namespace