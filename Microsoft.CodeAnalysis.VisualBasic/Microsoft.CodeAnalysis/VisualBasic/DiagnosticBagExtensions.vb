Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module DiagnosticBagExtensions
		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal code As ERRID, ByVal location As Microsoft.CodeAnalysis.Location) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(code)
			diagnostics.Add(New VBDiagnostic(diagnosticInfo, location, False))
			Return diagnosticInfo
		End Function

		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal code As ERRID, ByVal location As Microsoft.CodeAnalysis.Location, ByVal ParamArray args As Object()) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(code, args)
			diagnostics.Add(New VBDiagnostic(diagnosticInfo, location, False))
			Return diagnosticInfo
		End Function

		<Extension>
		Friend Sub Add(ByVal diagnostics As DiagnosticBag, ByVal info As DiagnosticInfo, ByVal location As Microsoft.CodeAnalysis.Location)
			diagnostics.Add(New VBDiagnostic(info, location, False))
		End Sub

		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal node As VisualBasicSyntaxNode, ByVal useSiteDiagnostics As IReadOnlyCollection(Of DiagnosticInfo)) As Boolean
			If (useSiteDiagnostics.IsNullOrEmpty()) Then
				Return False
			End If
			Return diagnostics.Add(node.GetLocation(), useSiteDiagnostics)
		End Function

		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal node As BoundNode, ByVal useSiteDiagnostics As IReadOnlyCollection(Of DiagnosticInfo)) As Boolean
			If (useSiteDiagnostics.IsNullOrEmpty()) Then
				Return False
			End If
			Return diagnostics.Add(node.Syntax.GetLocation(), useSiteDiagnostics)
		End Function

		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal node As SyntaxNodeOrToken, ByVal useSiteDiagnostics As IReadOnlyCollection(Of DiagnosticInfo)) As Boolean
			If (useSiteDiagnostics.IsNullOrEmpty()) Then
				Return False
			End If
			Return diagnostics.Add(node.GetLocation(), useSiteDiagnostics)
		End Function

		<Extension>
		Friend Function Add(ByVal diagnostics As DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal useSiteDiagnostics As IReadOnlyCollection(Of DiagnosticInfo)) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of DiagnosticInfo) = Nothing
			If (Not useSiteDiagnostics.IsNullOrEmpty()) Then
				Using enumerator
					enumerator = useSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						diagnostics.Add(New VBDiagnostic(enumerator.Current, location, False))
					End While
				End Using
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function
	End Module
End Namespace