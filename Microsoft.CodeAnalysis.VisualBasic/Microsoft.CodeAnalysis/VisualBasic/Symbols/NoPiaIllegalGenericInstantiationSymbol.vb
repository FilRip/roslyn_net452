Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class NoPiaIllegalGenericInstantiationSymbol
		Inherits ErrorTypeSymbol
		Private ReadOnly _underlyingSymbol As NamedTypeSymbol

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
				Dim objArray As [Object]()
				If (Me._underlyingSymbol.IsErrorType()) Then
					Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = DirectCast(Me._underlyingSymbol, ErrorTypeSymbol).ErrorInfo
					If (diagnosticInfo1 Is Nothing) Then
						objArray = New [Object]() { Me._underlyingSymbol }
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_CannotUseGenericTypeAcrossAssemblyBoundaries, objArray)
						Return diagnosticInfo
					End If
					diagnosticInfo = diagnosticInfo1
					Return diagnosticInfo
				End If
				objArray = New [Object]() { Me._underlyingSymbol }
				diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_CannotUseGenericTypeAcrossAssemblyBoundaries, objArray)
				Return diagnosticInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property UnderlyingSymbol As NamedTypeSymbol
			Get
				Return Me._underlyingSymbol
			End Get
		End Property

		Public Sub New(ByVal underlyingSymbol As NamedTypeSymbol)
			MyBase.New()
			Me._underlyingSymbol = underlyingSymbol
		End Sub

		Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return obj = Me
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return RuntimeHelpers.GetHashCode(Me)
		End Function
	End Class
End Namespace