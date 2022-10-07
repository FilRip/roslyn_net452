Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Class VisualBasicDiagnosticFormatter
		Inherits DiagnosticFormatter
		Public ReadOnly Shared Property Instance As VisualBasicDiagnosticFormatter

		Shared Sub New()
			VisualBasicDiagnosticFormatter.Instance = New VisualBasicDiagnosticFormatter()
		End Sub

		Protected Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function FormatSourceSpan(ByVal span As LinePositionSpan, ByVal formatter As IFormatProvider) As String
			Dim line As Integer = span.Start.Line + 1
			Return [String].Concat("(", line.ToString(), ") ")
		End Function
	End Class
End Namespace