using System;
using System.Collections.Concurrent;
using System.Globalization;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class CommonMessageProvider
    {
        private static readonly ConcurrentDictionary<(string prefix, int code), string> s_errorIdCache = new ConcurrentDictionary<(string, int), string>();

        public abstract string CodePrefix { get; }

        public abstract Type ErrorCodeType { get; }

        public abstract int ERR_FailedToCreateTempFile { get; }

        public abstract int ERR_MultipleAnalyzerConfigsInSameDir { get; }

        public abstract int ERR_ExpectedSingleScript { get; }

        public abstract int ERR_OpenResponseFile { get; }

        public abstract int ERR_InvalidPathMap { get; }

        public abstract int FTL_InvalidInputFileName { get; }

        public abstract int ERR_FileNotFound { get; }

        public abstract int ERR_NoSourceFile { get; }

        public abstract int ERR_CantOpenFileWrite { get; }

        public abstract int ERR_OutputWriteFailed { get; }

        public abstract int WRN_NoConfigNotOnCommandLine { get; }

        public abstract int ERR_BinaryFile { get; }

        public abstract int WRN_UnableToLoadAnalyzer { get; }

        public abstract int INF_UnableToLoadSomeTypesInAnalyzer { get; }

        public abstract int WRN_AnalyzerCannotBeCreated { get; }

        public abstract int WRN_NoAnalyzerInAssembly { get; }

        public abstract int WRN_AnalyzerReferencesFramework { get; }

        public abstract int ERR_CantReadRulesetFile { get; }

        public abstract int ERR_CompileCancelled { get; }

        public abstract int ERR_BadSourceCodeKind { get; }

        public abstract int ERR_BadDocumentationMode { get; }

        public abstract int ERR_BadCompilationOptionValue { get; }

        public abstract int ERR_MutuallyExclusiveOptions { get; }

        public abstract int ERR_InvalidDebugInformationFormat { get; }

        public abstract int ERR_InvalidFileAlignment { get; }

        public abstract int ERR_InvalidSubsystemVersion { get; }

        public abstract int ERR_InvalidOutputName { get; }

        public abstract int ERR_InvalidInstrumentationKind { get; }

        public abstract int ERR_InvalidHashAlgorithmName { get; }

        public abstract int ERR_MetadataFileNotAssembly { get; }

        public abstract int ERR_MetadataFileNotModule { get; }

        public abstract int ERR_InvalidAssemblyMetadata { get; }

        public abstract int ERR_InvalidModuleMetadata { get; }

        public abstract int ERR_ErrorOpeningAssemblyFile { get; }

        public abstract int ERR_ErrorOpeningModuleFile { get; }

        public abstract int ERR_MetadataFileNotFound { get; }

        public abstract int ERR_MetadataReferencesNotSupported { get; }

        public abstract int ERR_LinkedNetmoduleMetadataMustProvideFullPEImage { get; }

        public abstract int ERR_PublicKeyFileFailure { get; }

        public abstract int ERR_PublicKeyContainerFailure { get; }

        public abstract int ERR_OptionMustBeAbsolutePath { get; }

        public abstract int ERR_CantReadResource { get; }

        public abstract int ERR_CantOpenWin32Resource { get; }

        public abstract int ERR_CantOpenWin32Manifest { get; }

        public abstract int ERR_CantOpenWin32Icon { get; }

        public abstract int ERR_BadWin32Resource { get; }

        public abstract int ERR_ErrorBuildingWin32Resource { get; }

        public abstract int ERR_ResourceNotUnique { get; }

        public abstract int ERR_ResourceFileNameNotUnique { get; }

        public abstract int ERR_ResourceInModule { get; }

        public abstract int ERR_PermissionSetAttributeFileReadError { get; }

        public abstract int ERR_EncodinglessSyntaxTree { get; }

        public abstract int WRN_PdbUsingNameTooLong { get; }

        public abstract int WRN_PdbLocalNameTooLong { get; }

        public abstract int ERR_PdbWritingFailed { get; }

        public abstract int ERR_MetadataNameTooLong { get; }

        public abstract int ERR_EncReferenceToAddedMember { get; }

        public abstract int ERR_TooManyUserStrings { get; }

        public abstract int ERR_PeWritingFailure { get; }

        public abstract int ERR_ModuleEmitFailure { get; }

        public abstract int ERR_EncUpdateFailedMissingAttribute { get; }

        public abstract int ERR_InvalidDebugInfo { get; }

        public abstract int WRN_GeneratorFailedDuringInitialization { get; }

        public abstract int WRN_GeneratorFailedDuringGeneration { get; }

        public abstract int ERR_BadAssemblyName { get; }

        public abstract DiagnosticSeverity GetSeverity(int code);

        public abstract string LoadMessage(int code, CultureInfo? language);

        public abstract LocalizableString GetTitle(int code);

        public abstract LocalizableString GetDescription(int code);

        public abstract LocalizableString GetMessageFormat(int code);

        public abstract string GetHelpLink(int code);

        public abstract string GetCategory(int code);

        public abstract int GetWarningLevel(int code);

        public Diagnostic CreateDiagnostic(int code, Location location)
        {
            return CreateDiagnostic(code, location, new object[0]);
        }

        public abstract Diagnostic CreateDiagnostic(DiagnosticInfo info);

        public abstract Diagnostic CreateDiagnostic(int code, Location location, params object[] args);

        public abstract string GetMessagePrefix(string id, DiagnosticSeverity severity, bool isWarningAsError, CultureInfo? culture);

        public abstract string GetErrorDisplayString(ISymbol symbol);

        public string GetIdForErrorCode(int errorCode)
        {
            return s_errorIdCache.GetOrAdd((CodePrefix, errorCode), ((string prefix, int code) key) => key.prefix + key.code.ToString("0000"));
        }

        public abstract ReportDiagnostic GetDiagnosticReport(DiagnosticInfo diagnosticInfo, CompilationOptions options);

        public DiagnosticInfo? FilterDiagnosticInfo(DiagnosticInfo diagnosticInfo, CompilationOptions options)
        {
            return GetDiagnosticReport(diagnosticInfo, options) switch
            {
                ReportDiagnostic.Error => diagnosticInfo.GetInstanceWithSeverity(DiagnosticSeverity.Error),
                ReportDiagnostic.Warn => diagnosticInfo.GetInstanceWithSeverity(DiagnosticSeverity.Warning),
                ReportDiagnostic.Info => diagnosticInfo.GetInstanceWithSeverity(DiagnosticSeverity.Info),
                ReportDiagnostic.Hidden => diagnosticInfo.GetInstanceWithSeverity(DiagnosticSeverity.Hidden),
                ReportDiagnostic.Suppress => null,
                _ => diagnosticInfo,
            };
        }

        public abstract void ReportDuplicateMetadataReferenceStrong(DiagnosticBag diagnostics, Location location, MetadataReference reference, AssemblyIdentity identity, MetadataReference equivalentReference, AssemblyIdentity equivalentIdentity);

        public abstract void ReportDuplicateMetadataReferenceWeak(DiagnosticBag diagnostics, Location location, MetadataReference reference, AssemblyIdentity identity, MetadataReference equivalentReference, AssemblyIdentity equivalentIdentity);

        public void ReportStreamWriteException(Exception e, string filePath, DiagnosticBag diagnostics)
        {
            diagnostics.Add(CreateDiagnostic(ERR_OutputWriteFailed, Location.None, filePath, e.Message));
        }

        protected abstract void ReportInvalidAttributeArgument(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, AttributeData attribute);

        public void ReportInvalidAttributeArgument(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, AttributeData attribute)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportInvalidAttributeArgument(diagnosticBag, attributeSyntax, parameterIndex, attribute);
            }
        }

        protected abstract void ReportInvalidNamedArgument(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex, ITypeSymbol attributeClass, string parameterName);

        public void ReportInvalidNamedArgument(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex, ITypeSymbol attributeClass, string parameterName)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportInvalidNamedArgument(diagnosticBag, attributeSyntax, namedArgumentIndex, attributeClass, parameterName);
            }
        }

        protected abstract void ReportParameterNotValidForType(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex);

        public void ReportParameterNotValidForType(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportParameterNotValidForType(diagnosticBag, attributeSyntax, namedArgumentIndex);
            }
        }

        protected abstract void ReportMarshalUnmanagedTypeNotValidForFields(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute);

        public void ReportMarshalUnmanagedTypeNotValidForFields(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportMarshalUnmanagedTypeNotValidForFields(diagnosticBag, attributeSyntax, parameterIndex, unmanagedTypeName, attribute);
            }
        }

        protected abstract void ReportMarshalUnmanagedTypeOnlyValidForFields(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute);

        public void ReportMarshalUnmanagedTypeOnlyValidForFields(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportMarshalUnmanagedTypeOnlyValidForFields(diagnosticBag, attributeSyntax, parameterIndex, unmanagedTypeName, attribute);
            }
        }

        protected abstract void ReportAttributeParameterRequired(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName);

        public void ReportAttributeParameterRequired(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportAttributeParameterRequired(diagnosticBag, attributeSyntax, parameterName);
            }
        }

        protected abstract void ReportAttributeParameterRequired(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName1, string parameterName2);

        public void ReportAttributeParameterRequired(BindingDiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName1, string parameterName2)
        {
            DiagnosticBag diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag != null)
            {
                ReportAttributeParameterRequired(diagnosticBag, attributeSyntax, parameterName1, parameterName2);
            }
        }
    }
}
