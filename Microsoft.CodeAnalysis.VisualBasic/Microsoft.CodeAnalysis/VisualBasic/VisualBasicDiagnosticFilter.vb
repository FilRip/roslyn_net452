Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VisualBasicDiagnosticFilter
		Private ReadOnly Shared s_alinkWarnings As ERRID()

		Shared Sub New()
			VisualBasicDiagnosticFilter.s_alinkWarnings = New ERRID() { GetType(<PrivateImplementationDetails>).GetField("554F3604B1038274FA9D7966BBACBE122DDEFE1DE7427F9B110F40CF6161E486").FieldHandle }
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function Filter(ByVal diagnostic As Microsoft.CodeAnalysis.Diagnostic, ByVal generalDiagnosticOption As Microsoft.CodeAnalysis.ReportDiagnostic, ByVal specificDiagnosticOptions As IDictionary(Of String, Microsoft.CodeAnalysis.ReportDiagnostic), ByVal syntaxTreeOptions As SyntaxTreeOptionsProvider, ByVal cancellationToken As System.Threading.CancellationToken) As Microsoft.CodeAnalysis.Diagnostic
			Dim diagnostic1 As Microsoft.CodeAnalysis.Diagnostic
			Dim reportDiagnostic As Microsoft.CodeAnalysis.ReportDiagnostic
			Dim strs As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.ReportDiagnostic) = ImmutableDictionary.Create(Of String, Microsoft.CodeAnalysis.ReportDiagnostic)(CaseInsensitiveComparison.Comparer).AddRange(specificDiagnosticOptions)
			If (CInt(diagnostic.Severity) = -2) Then
				diagnostic1 = Nothing
			ElseIf (Not diagnostic.IsNotConfigurable()) Then
				Dim flag As Boolean = False
				reportDiagnostic = If(Not IReadOnlyListExtensions.Contains(Of ERRID)(VisualBasicDiagnosticFilter.s_alinkWarnings, DirectCast(diagnostic.Code, ERRID), Nothing) OrElse Not EnumerableExtensions.Contains(strs.Keys, MessageProvider.Instance.GetIdForErrorCode(40010)), VisualBasicDiagnosticFilter.GetDiagnosticReport(diagnostic.Severity, diagnostic.IsEnabledByDefault, diagnostic.Id, diagnostic.Location, diagnostic.Category, generalDiagnosticOption, strs, syntaxTreeOptions, cancellationToken, flag), VisualBasicDiagnosticFilter.GetDiagnosticReport(MessageProvider.Instance.GetSeverity(40010), diagnostic.IsEnabledByDefault, MessageProvider.Instance.GetIdForErrorCode(40010), diagnostic.Location, diagnostic.Category, generalDiagnosticOption, strs, syntaxTreeOptions, cancellationToken, flag))
				If (flag) Then
					diagnostic = diagnostic.WithIsSuppressed(True)
				End If
				diagnostic1 = diagnostic.WithReportDiagnostic(reportDiagnostic)
			ElseIf (Not diagnostic.IsEnabledByDefault) Then
				diagnostic1 = Nothing
			Else
				diagnostic1 = diagnostic
			End If
			Return diagnostic1
		End Function

		Friend Shared Function GetDiagnosticReport(ByVal severity As DiagnosticSeverity, ByVal isEnabledByDefault As Boolean, ByVal id As String, ByVal location As Microsoft.CodeAnalysis.Location, ByVal category As String, ByVal generalDiagnosticOption As Microsoft.CodeAnalysis.ReportDiagnostic, ByVal caseInsensitiveSpecificDiagnosticOptions As IDictionary(Of String, Microsoft.CodeAnalysis.ReportDiagnostic), ByVal syntaxTreeOptions As SyntaxTreeOptionsProvider, ByVal cancellationToken As System.Threading.CancellationToken, <Out> ByRef hasDisableDirectiveSuppression As Boolean) As Microsoft.CodeAnalysis.ReportDiagnostic
			Dim reportDiagnostic As Microsoft.CodeAnalysis.ReportDiagnostic
			Dim reportDiagnostic1 As Microsoft.CodeAnalysis.ReportDiagnostic
			Dim reportDiagnostic2 As Microsoft.CodeAnalysis.ReportDiagnostic
			Dim sourceTree As Microsoft.CodeAnalysis.SyntaxTree
			hasDisableDirectiveSuppression = False
			If (location IsNot Nothing) Then
				sourceTree = location.SourceTree
			Else
				sourceTree = Nothing
			End If
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = sourceTree
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			If (caseInsensitiveSpecificDiagnosticOptions.TryGetValue(id, reportDiagnostic1)) Then
				flag = True
				If (reportDiagnostic1 = Microsoft.CodeAnalysis.ReportDiagnostic.[Default]) Then
					flag1 = True
				End If
			End If
			If (syntaxTreeOptions IsNot Nothing AndAlso (Not flag OrElse flag1) AndAlso (syntaxTree IsNot Nothing AndAlso syntaxTreeOptions.TryGetDiagnosticValue(syntaxTree, id, cancellationToken, reportDiagnostic2) OrElse syntaxTreeOptions.TryGetGlobalDiagnosticValue(id, cancellationToken, reportDiagnostic2)) AndAlso (Not flag1 OrElse severity <> DiagnosticSeverity.Warning OrElse reportDiagnostic2 <> Microsoft.CodeAnalysis.ReportDiagnostic.[Error])) Then
				flag = True
				reportDiagnostic1 = reportDiagnostic2
				If (Not flag1 AndAlso reportDiagnostic1 = Microsoft.CodeAnalysis.ReportDiagnostic.Warn AndAlso generalDiagnosticOption = Microsoft.CodeAnalysis.ReportDiagnostic.[Error]) Then
					reportDiagnostic1 = Microsoft.CodeAnalysis.ReportDiagnostic.[Error]
				End If
			End If
			If (Not flag) Then
				reportDiagnostic1 = If(isEnabledByDefault, Microsoft.CodeAnalysis.ReportDiagnostic.[Default], Microsoft.CodeAnalysis.ReportDiagnostic.Suppress)
			End If
			If (reportDiagnostic1 <> Microsoft.CodeAnalysis.ReportDiagnostic.Suppress) Then
				If (location IsNot Nothing AndAlso location.SourceTree IsNot Nothing AndAlso location.SourceTree.GetWarningState(id, location.SourceSpan.Start) = Microsoft.CodeAnalysis.ReportDiagnostic.Suppress) Then
					hasDisableDirectiveSuppression = True
				End If
				If (generalDiagnosticOption <> Microsoft.CodeAnalysis.ReportDiagnostic.Suppress OrElse severity <> DiagnosticSeverity.Warning AndAlso severity <> DiagnosticSeverity.Info) Then
					reportDiagnostic = If(generalDiagnosticOption <> Microsoft.CodeAnalysis.ReportDiagnostic.[Error] OrElse severity <> DiagnosticSeverity.Warning OrElse flag OrElse reportDiagnostic1 <> Microsoft.CodeAnalysis.ReportDiagnostic.[Default], reportDiagnostic1, Microsoft.CodeAnalysis.ReportDiagnostic.[Error])
				Else
					reportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.Suppress
				End If
			Else
				reportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.Suppress
			End If
			Return reportDiagnostic
		End Function
	End Class
End Namespace