Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MessageProvider
		Inherits CommonMessageProvider
		Implements IObjectWritable
		Public ReadOnly Shared Instance As MessageProvider

		Public Overrides ReadOnly Property CodePrefix As String
			Get
				Return "BC"
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BadAssemblyName As Integer
			Get
				Return 37283
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BadCompilationOptionValue As Integer
			Get
				Return 2014
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BadDocumentationMode As Integer
			Get
				Return 37286
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BadSourceCodeKind As Integer
			Get
				Return 37285
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BadWin32Resource As Integer
			Get
				Return 30136
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_BinaryFile As Integer
			Get
				Return 2015
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantOpenFileWrite As Integer
			Get
				Return 2012
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantOpenWin32Icon As Integer
			Get
				Return 31509
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantOpenWin32Manifest As Integer
			Get
				Return 31192
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantOpenWin32Resource As Integer
			Get
				Return 31509
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantReadResource As Integer
			Get
				Return 31509
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CantReadRulesetFile As Integer
			Get
				Return 37232
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_CompileCancelled As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_EncodinglessSyntaxTree As Integer
			Get
				Return 37236
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_EncReferenceToAddedMember As Integer
			Get
				Return 37248
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_EncUpdateFailedMissingAttribute As Integer
			Get
				Return 36983
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ErrorBuildingWin32Resource As Integer
			Get
				Return 30136
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ErrorOpeningAssemblyFile As Integer
			Get
				Return 31011
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ErrorOpeningModuleFile As Integer
			Get
				Return 31007
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ExpectedSingleScript As Integer
			Get
				Return 36963
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_FailedToCreateTempFile As Integer
			Get
				Return 30138
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_FileNotFound As Integer
			Get
				Return 2001
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidAssemblyMetadata As Integer
			Get
				Return 31519
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidDebugInfo As Integer
			Get
				Return 37290
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidDebugInformationFormat As Integer
			Get
				Return 31453
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidFileAlignment As Integer
			Get
				Return 31452
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidHashAlgorithmName As Integer
			Get
				Return 31219
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidInstrumentationKind As Integer
			Get
				Return 37266
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidModuleMetadata As Integer
			Get
				Return 31007
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidOutputName As Integer
			Get
				Return 31451
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidPathMap As Integer
			Get
				Return 37253
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_InvalidSubsystemVersion As Integer
			Get
				Return 31391
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_LinkedNetmoduleMetadataMustProvideFullPEImage As Integer
			Get
				Return 37231
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MetadataFileNotAssembly As Integer
			Get
				Return 31076
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MetadataFileNotFound As Integer
			Get
				Return 2017
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MetadataFileNotModule As Integer
			Get
				Return 31077
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MetadataNameTooLong As Integer
			Get
				Return 37220
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MetadataReferencesNotSupported As Integer
			Get
				Return 37233
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ModuleEmitFailure As Integer
			Get
				Return 36970
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MultipleAnalyzerConfigsInSameDir As Integer
			Get
				Return 42500
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_MutuallyExclusiveOptions As Integer
			Get
				Return 2046
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_NoSourceFile As Integer
			Get
				Return 31007
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_OpenResponseFile As Integer
			Get
				Return 2011
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_OptionMustBeAbsolutePath As Integer
			Get
				Return 37257
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_OutputWriteFailed As Integer
			Get
				Return 2012
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_PdbWritingFailed As Integer
			Get
				Return 37225
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_PermissionSetAttributeFileReadError As Integer
			Get
				Return 31217
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_PeWritingFailure As Integer
			Get
				Return 37256
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_PublicKeyContainerFailure As Integer
			Get
				Return 36981
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_PublicKeyFileFailure As Integer
			Get
				Return 36980
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ResourceFileNameNotUnique As Integer
			Get
				Return 35003
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ResourceInModule As Integer
			Get
				Return 37227
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_ResourceNotUnique As Integer
			Get
				Return 31502
			End Get
		End Property

		Public Overrides ReadOnly Property ERR_TooManyUserStrings As Integer
			Get
				Return 37255
			End Get
		End Property

		Public Overrides ReadOnly Property ErrorCodeType As Type
			Get
				Return GetType(ERRID)
			End Get
		End Property

		Public Overrides ReadOnly Property FTL_InvalidInputFileName As Integer
			Get
				Return 2032
			End Get
		End Property

		Public Overrides ReadOnly Property INF_UnableToLoadSomeTypesInAnalyzer As Integer
			Get
				Return 50002
			End Get
		End Property

		ReadOnly Property IObjectWritable_ShouldReuseInSerialization As Boolean Implements IObjectWritable.ShouldReuseInSerialization
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_AnalyzerCannotBeCreated As Integer
			Get
				Return 42376
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_AnalyzerReferencesFramework As Integer
			Get
				Return 42503
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_GeneratorFailedDuringGeneration As Integer
			Get
				Return 42502
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_GeneratorFailedDuringInitialization As Integer
			Get
				Return 42501
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_NoAnalyzerInAssembly As Integer
			Get
				Return 42377
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_NoConfigNotOnCommandLine As Integer
			Get
				Return 2025
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_PdbLocalNameTooLong As Integer
			Get
				Return 42373
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_PdbUsingNameTooLong As Integer
			Get
				Return 42374
			End Get
		End Property

		Public Overrides ReadOnly Property WRN_UnableToLoadAnalyzer As Integer
			Get
				Return 42378
			End Get
		End Property

		Shared Sub New()
			MessageProvider.Instance = New MessageProvider()
			ObjectBinder.RegisterTypeReader(GetType(MessageProvider), Function(r As ObjectReader) MessageProvider.Instance)
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function CreateDiagnostic(ByVal code As Integer, ByVal location As Microsoft.CodeAnalysis.Location, ByVal ParamArray args As Object()) As Diagnostic
			Return New VBDiagnostic(ErrorFactory.ErrorInfo(DirectCast(code, ERRID), args), location, False)
		End Function

		Public Overrides Function CreateDiagnostic(ByVal info As DiagnosticInfo) As Diagnostic
			Return New VBDiagnostic(info, Location.None, False)
		End Function

		Public Overrides Function GetCategory(ByVal code As Integer) As String
			Return ErrorFactory.GetCategory(DirectCast(code, ERRID))
		End Function

		Public Overrides Function GetDescription(ByVal code As Integer) As LocalizableString
			Return ErrorFactory.GetDescription(DirectCast(code, ERRID))
		End Function

		Public Overrides Function GetDiagnosticReport(ByVal diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo, ByVal options As CompilationOptions) As ReportDiagnostic
			Dim flag As Boolean = False
			Return VisualBasicDiagnosticFilter.GetDiagnosticReport(diagnosticInfo.Severity, True, diagnosticInfo.MessageIdentifier, Location.None, diagnosticInfo.Category, options.GeneralDiagnosticOption, options.SpecificDiagnosticOptions, options.SyntaxTreeOptionsProvider, CancellationToken.None, flag)
		End Function

		Public Overrides Function GetErrorDisplayString(ByVal symbol As ISymbol) As String
			Dim str As String
			str = If(symbol.Kind = SymbolKind.Assembly OrElse symbol.Kind = SymbolKind.[Namespace], symbol.ToString(), Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayString(symbol, SymbolDisplayFormat.VisualBasicShortErrorMessageFormat))
			Return str
		End Function

		Public Overrides Function GetHelpLink(ByVal code As Integer) As String
			Return ErrorFactory.GetHelpLink(DirectCast(code, ERRID))
		End Function

		Public Overrides Function GetMessageFormat(ByVal code As Integer) As LocalizableString
			Return ErrorFactory.GetMessageFormat(DirectCast(code, ERRID))
		End Function

		Public Overrides Function GetMessagePrefix(ByVal id As String, ByVal severity As DiagnosticSeverity, ByVal isWarningAsError As Boolean, ByVal culture As CultureInfo) As String
			Return [String].Format(culture, "{0} {1}", If(severity = DiagnosticSeverity.[Error] OrElse isWarningAsError, "error", "warning"), id)
		End Function

		Public Overrides Function GetSeverity(ByVal code As Integer) As Microsoft.CodeAnalysis.DiagnosticSeverity
			Dim diagnosticSeverity As Microsoft.CodeAnalysis.DiagnosticSeverity
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = code
			If (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.Void) Then
				diagnosticSeverity = -2
			ElseIf (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.Unknown) Then
				diagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity.Info Or Microsoft.CodeAnalysis.DiagnosticSeverity.Warning Or Microsoft.CodeAnalysis.DiagnosticSeverity.[Error]
			ElseIf (ErrorFacts.IsWarning(eRRID)) Then
				diagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning
			ElseIf (Not ErrorFacts.IsInfo(eRRID)) Then
				diagnosticSeverity = If(Not ErrorFacts.IsHidden(eRRID), Microsoft.CodeAnalysis.DiagnosticSeverity.[Error], Microsoft.CodeAnalysis.DiagnosticSeverity.Hidden)
			Else
				diagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity.Info
			End If
			Return diagnosticSeverity
		End Function

		Public Overrides Function GetTitle(ByVal code As Integer) As LocalizableString
			Return ErrorFactory.GetTitle(DirectCast(code, ERRID))
		End Function

		Public Overrides Function GetWarningLevel(ByVal code As Integer) As Integer
			Dim num As Integer
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = code
			If (Not ErrorFacts.IsWarning(eRRID) OrElse code = 2007 OrElse code = 2025 OrElse code = 2034) Then
				num = If(ErrorFacts.IsInfo(eRRID) OrElse ErrorFacts.IsHidden(eRRID), 1, 0)
			Else
				num = 1
			End If
			Return num
		End Function

		Public Overrides Function LoadMessage(ByVal code As Integer, ByVal language As CultureInfo) As String
			Return ErrorFactory.IdToString(DirectCast(code, ERRID), language)
		End Function

		Protected Overrides Sub ReportAttributeParameterRequired(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal parameterName As String)
			diagnostics.Add(ERRID.ERR_AttributeParameterRequired1, DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).Name.GetLocation(), New [Object]() { parameterName })
		End Sub

		Protected Overrides Sub ReportAttributeParameterRequired(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal parameterName1 As String, ByVal parameterName2 As String)
			diagnostics.Add(ERRID.ERR_AttributeParameterRequired2, DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).Name.GetLocation(), New [Object]() { parameterName1, parameterName2 })
		End Sub

		Public Overrides Sub ReportDuplicateMetadataReferenceStrong(ByVal diagnostics As DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal reference As MetadataReference, ByVal identity As AssemblyIdentity, ByVal equivalentReference As MetadataReference, ByVal equivalentIdentity As AssemblyIdentity)
			diagnostics.Add(ERRID.ERR_DuplicateReferenceStrong, location, New [Object]() { If(reference.Display, identity.GetDisplayName(False)), If(equivalentReference.Display, equivalentIdentity.GetDisplayName(False)) })
		End Sub

		Public Overrides Sub ReportDuplicateMetadataReferenceWeak(ByVal diagnostics As Microsoft.CodeAnalysis.DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal reference As MetadataReference, ByVal identity As AssemblyIdentity, ByVal equivalentReference As MetadataReference, ByVal equivalentIdentity As AssemblyIdentity)
			Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = diagnostics
			Dim location1 As Microsoft.CodeAnalysis.Location = location
			Dim name() As [Object] = { identity.Name, Nothing }
			name(1) = If(equivalentReference.Display, equivalentIdentity.GetDisplayName(False))
			diagnosticBag.Add(ERRID.ERR_DuplicateReference2, location1, name)
		End Sub

		Protected Overrides Sub ReportInvalidAttributeArgument(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal parameterIndex As Integer, ByVal attribute As AttributeData)
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).ArgumentList.Arguments
			diagnostics.Add(ERRID.ERR_BadAttribute1, arguments(parameterIndex).GetLocation(), New [Object]() { attribute.AttributeClass })
		End Sub

		Protected Overrides Sub ReportInvalidNamedArgument(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal namedArgumentIndex As Integer, ByVal attributeClass As ITypeSymbol, ByVal parameterName As String)
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).ArgumentList.Arguments
			diagnostics.Add(ERRID.ERR_BadAttribute1, arguments(namedArgumentIndex).GetLocation(), New [Object]() { attributeClass })
		End Sub

		Protected Overrides Sub ReportMarshalUnmanagedTypeNotValidForFields(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal parameterIndex As Integer, ByVal unmanagedTypeName As String, ByVal attribute As AttributeData)
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).ArgumentList.Arguments
			diagnostics.Add(ERRID.ERR_MarshalUnmanagedTypeNotValidForFields, arguments(parameterIndex).GetLocation(), New [Object]() { unmanagedTypeName })
		End Sub

		Protected Overrides Sub ReportMarshalUnmanagedTypeOnlyValidForFields(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal parameterIndex As Integer, ByVal unmanagedTypeName As String, ByVal attribute As AttributeData)
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).ArgumentList.Arguments
			diagnostics.Add(ERRID.ERR_MarshalUnmanagedTypeOnlyValidForFields, arguments(parameterIndex).GetLocation(), New [Object]() { unmanagedTypeName })
		End Sub

		Protected Overrides Sub ReportParameterNotValidForType(ByVal diagnostics As DiagnosticBag, ByVal attributeSyntax As SyntaxNode, ByVal namedArgumentIndex As Integer)
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = DirectCast(attributeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax).ArgumentList.Arguments
			diagnostics.Add(ERRID.ERR_ParameterNotValidForType, arguments(namedArgumentIndex).GetLocation())
		End Sub

		Private Sub WriteTo(ByVal writer As ObjectWriter) Implements IObjectWritable.WriteTo
		End Sub
	End Class
End Namespace