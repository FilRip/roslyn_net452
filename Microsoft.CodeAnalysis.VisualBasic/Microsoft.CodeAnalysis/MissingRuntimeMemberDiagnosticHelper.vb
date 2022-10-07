Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis
	Friend Module MissingRuntimeMemberDiagnosticHelper
		Friend Const MyVBNamespace As String = "My"

		Private ReadOnly s_metadataNames As Dictionary(Of String, String)

		Sub New()
			MissingRuntimeMemberDiagnosticHelper.s_metadataNames = New Dictionary(Of String, String)() From
			{
				{ "Microsoft.VisualBasic.CompilerServices.Operators", "Late binding" },
				{ "Microsoft.VisualBasic.CompilerServices.NewLateBinding", "Late binding" },
				{ "Microsoft.VisualBasic.CompilerServices.LikeOperator", "Like operator" },
				{ "Microsoft.VisualBasic.CompilerServices.ProjectData", "Unstructured exception handling" },
				{ "Microsoft.VisualBasic.CompilerServices.ProjectData.CreateProjectError", "Unstructured exception handling" }
			}
		End Sub

		Friend Function GetDiagnosticForMissingRuntimeHelper(ByVal typename As String, ByVal membername As String, ByVal embedVBCoreRuntime As Boolean) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim str As String = ""
			MissingRuntimeMemberDiagnosticHelper.s_metadataNames.TryGetValue(typename, str)
			diagnosticInfo = If(Not embedVBCoreRuntime OrElse [String].IsNullOrEmpty(str), ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, New [Object]() { [String].Concat(typename, ".", membername) }), ErrorFactory.ErrorInfo(ERRID.ERR_PlatformDoesntSupport, New [Object]() { str }))
			Return diagnosticInfo
		End Function
	End Module
End Namespace