Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Resources
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ErrorFactory
		Private Const s_titleSuffix As String = "_Title"

		Private Const s_descriptionSuffix As String = "_Description"

		Private ReadOnly Shared s_categoriesMap As Lazy(Of ImmutableDictionary(Of ERRID, String))

		Public ReadOnly Shared VoidDiagnosticInfo As DiagnosticInfo

		Public ReadOnly Shared GetErrorInfo_ERR_WithEventsRequiresClass As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_ERR_StrictDisallowImplicitObject As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_WRN_ObjectAssumedVar1_WRN_StaticLocalNoInference As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_ERR_StrictDisallowsImplicitProc As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_ERR_StrictDisallowsImplicitArgs As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinOperator As Func(Of DiagnosticInfo)

		Public ReadOnly Shared GetErrorInfo_WRN_ObjectAssumedProperty1_WRN_MissingAsClauseinProperty As Func(Of DiagnosticInfo)

		Private Shared s_resourceManager As System.Resources.ResourceManager

		Friend ReadOnly Shared Property ResourceManager As System.Resources.ResourceManager
			Get
				If (ErrorFactory.s_resourceManager Is Nothing) Then
					ErrorFactory.s_resourceManager = New System.Resources.ResourceManager(GetType(VBResources).FullName, GetType(ERRID).GetTypeInfo().Assembly)
				End If
				Return ErrorFactory.s_resourceManager
			End Get
		End Property

		Shared Sub New()
			ErrorFactory.s_categoriesMap = New Lazy(Of ImmutableDictionary(Of ERRID, String))(New Func(Of ImmutableDictionary(Of ERRID, String))(AddressOf ErrorFactory.CreateCategoriesMap))
			ErrorFactory.VoidDiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.Void)
			ErrorFactory.GetErrorInfo_ERR_WithEventsRequiresClass = Function() ErrorFactory.ErrorInfo(ERRID.ERR_WithEventsRequiresClass)
			ErrorFactory.GetErrorInfo_ERR_StrictDisallowImplicitObject = Function() ErrorFactory.ErrorInfo(ERRID.ERR_StrictDisallowImplicitObject)
			ErrorFactory.GetErrorInfo_WRN_ObjectAssumedVar1_WRN_StaticLocalNoInference = Function() ErrorFactory.ErrorInfo(ERRID.WRN_ObjectAssumedVar1, New [Object]() { ErrorFactory.ErrorInfo(ERRID.WRN_StaticLocalNoInference) })
			ErrorFactory.GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl = Function() ErrorFactory.ErrorInfo(ERRID.WRN_ObjectAssumedVar1, New [Object]() { ErrorFactory.ErrorInfo(ERRID.WRN_MissingAsClauseinVarDecl) })
			ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc = Function() ErrorFactory.ErrorInfo(ERRID.ERR_StrictDisallowsImplicitProc)
			ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitArgs = Function() ErrorFactory.ErrorInfo(ERRID.ERR_StrictDisallowsImplicitArgs)
			ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction = Function() ErrorFactory.ErrorInfo(ERRID.WRN_ObjectAssumed1, New [Object]() { ErrorFactory.ErrorInfo(ERRID.WRN_MissingAsClauseinFunction) })
			ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinOperator = Function() ErrorFactory.ErrorInfo(ERRID.WRN_ObjectAssumed1, New [Object]() { ErrorFactory.ErrorInfo(ERRID.WRN_MissingAsClauseinOperator) })
			ErrorFactory.GetErrorInfo_WRN_ObjectAssumedProperty1_WRN_MissingAsClauseinProperty = Function() ErrorFactory.ErrorInfo(ERRID.WRN_ObjectAssumedProperty1, New [Object]() { ErrorFactory.ErrorInfo(ERRID.WRN_MissingAsClauseinProperty) })
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Function CreateCategoriesMap() As ImmutableDictionary(Of ERRID, String)
			Return (New Dictionary(Of ERRID, String)()).ToImmutableDictionary()
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID) As DiagnosticInfo
			Return New DiagnosticInfo(MessageProvider.Instance, CInt(id))
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID, ByVal ParamArray arguments As Object()) As DiagnosticInfo
			Return New DiagnosticInfo(MessageProvider.Instance, CInt(id), arguments)
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID, ByRef syntaxToken As Microsoft.CodeAnalysis.SyntaxToken) As DiagnosticInfo
			Return ErrorFactory.ErrorInfo(id, New [Object]() { SyntaxFacts.GetText(syntaxToken.Kind()) })
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID, ByRef syntaxTokenKind As SyntaxKind) As DiagnosticInfo
			Return ErrorFactory.ErrorInfo(id, New [Object]() { SyntaxFacts.GetText(DirectCast(CUShort(syntaxTokenKind), SyntaxKind)) })
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID, ByRef syntaxToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal type As TypeSymbol) As DiagnosticInfo
			Return ErrorFactory.ErrorInfo(id, New [Object]() { SyntaxFacts.GetText(syntaxToken.Kind()), type })
		End Function

		Public Shared Function ErrorInfo(ByVal id As ERRID, ByRef syntaxToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal type1 As TypeSymbol, ByVal type2 As TypeSymbol) As DiagnosticInfo
			Return ErrorFactory.ErrorInfo(id, New [Object]() { SyntaxFacts.GetText(syntaxToken.Kind()), type1, type2 })
		End Function

		Public Shared Function GetCategory(ByVal id As ERRID) As String
			Dim str As String
			Dim str1 As String = Nothing
			str = If(Not ErrorFactory.s_categoriesMap.Value.TryGetValue(id, str1), "Compiler", str1)
			Return str
		End Function

		Public Shared Function GetDescription(ByVal id As ERRID) As LocalizableResourceString
			Return New LocalizableResourceString([String].Concat(id.ToString(), "_Description"), ErrorFactory.ResourceManager, GetType(ErrorFactory))
		End Function

		Public Shared Function GetHelpLink(ByVal id As ERRID) As String
			Return [String].Format("https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k({0})", MessageProvider.Instance.GetIdForErrorCode(CInt(id)))
		End Function

		Public Shared Function GetMessageFormat(ByVal id As ERRID) As LocalizableResourceString
			Return New LocalizableResourceString(id.ToString(), ErrorFactory.ResourceManager, GetType(ErrorFactory))
		End Function

		Public Shared Function GetTitle(ByVal id As ERRID) As LocalizableResourceString
			Return New LocalizableResourceString([String].Concat(id.ToString(), "_Title"), ErrorFactory.ResourceManager, GetType(ErrorFactory))
		End Function

		Friend Shared Function IdToString(ByVal id As ERRID) As String
			Return ErrorFactory.IdToString(id, CultureInfo.CurrentUICulture)
		End Function

		Public Shared Function IdToString(ByVal id As ERRID, ByVal language As CultureInfo) As String
			Return ErrorFactory.ResourceManager.GetString(id.ToString(), language)
		End Function

		Public Shared Function ObsoleteErrorInfo(ByVal id As ERRID, ByVal data As ObsoleteAttributeData, ByVal ParamArray arguments As Object()) As CustomObsoleteDiagnosticInfo
			Return New CustomObsoleteDiagnosticInfo(MessageProvider.Instance, CInt(id), data, arguments)
		End Function
	End Class
End Namespace