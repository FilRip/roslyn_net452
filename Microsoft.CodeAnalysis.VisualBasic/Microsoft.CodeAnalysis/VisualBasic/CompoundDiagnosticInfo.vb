Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class CompoundDiagnosticInfo
		Inherits DiagnosticInfo
		Friend Sub New(ByVal arguments As DiagnosticInfo())
			MyBase.New(MessageProvider.Instance, 0, arguments)
		End Sub

		Public Overrides Function GetMessage(Optional ByVal formatProvider As IFormatProvider = Nothing) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			If (MyBase.Arguments IsNot Nothing) Then
				Dim arguments As Object() = MyBase.Arguments
				For i As Integer = 0 To CInt(arguments.Length) Step 1
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = DirectCast(arguments(i), Microsoft.CodeAnalysis.DiagnosticInfo)
					instance.Builder.Append(diagnosticInfo.GetMessage(formatProvider))
				Next

			End If
			Dim str As String = instance.Builder.ToString()
			instance.Free()
			Return str
		End Function
	End Class
End Namespace