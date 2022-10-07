Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class VBDiagnostic
		Inherits DiagnosticWithInfo
		Friend Sub New(ByVal info As DiagnosticInfo, ByVal location As Microsoft.CodeAnalysis.Location, Optional ByVal isSuppressed As Boolean = False)
			MyBase.New(info, location, isSuppressed)
		End Sub

		Public Overrides Function ToString() As String
			Return VisualBasicDiagnosticFormatter.Instance.Format(Me, Nothing)
		End Function

		Friend Overrides Function WithIsSuppressed(ByVal isSuppressed As Boolean) As Microsoft.CodeAnalysis.Diagnostic
			Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic
			diagnostic = If(Me.IsSuppressed = isSuppressed, Me, New VBDiagnostic(MyBase.Info, Me.Location, isSuppressed))
			Return diagnostic
		End Function

		Friend Overrides Function WithLocation(ByVal location As Microsoft.CodeAnalysis.Location) As Microsoft.CodeAnalysis.Diagnostic
			Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic
			If (location Is Nothing) Then
				Throw New ArgumentNullException("location")
			End If
			diagnostic = If(CObj(location) = CObj(Me.Location), Me, New VBDiagnostic(MyBase.Info, location, Me.IsSuppressed))
			Return diagnostic
		End Function

		Friend Overrides Function WithSeverity(ByVal severity As DiagnosticSeverity) As Microsoft.CodeAnalysis.Diagnostic
			Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic
			diagnostic = If(Me.Severity = severity, Me, New VBDiagnostic(MyBase.Info.GetInstanceWithSeverity(severity), Me.Location, Me.IsSuppressed))
			Return diagnostic
		End Function
	End Class
End Namespace