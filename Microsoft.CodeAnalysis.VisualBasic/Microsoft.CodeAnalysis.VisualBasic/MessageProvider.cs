using System;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MessageProvider : CommonMessageProvider, IObjectWritable
	{
		public static readonly MessageProvider Instance;

		private bool IObjectWritable_ShouldReuseInSerialization => true;

		public override string CodePrefix => "BC";

		public override Type ErrorCodeType => typeof(ERRID);

		public override int ERR_FailedToCreateTempFile => 30138;

		public override int ERR_MultipleAnalyzerConfigsInSameDir => 42500;

		public override int ERR_ExpectedSingleScript => 36963;

		public override int ERR_OpenResponseFile => 2011;

		public override int ERR_InvalidPathMap => 37253;

		public override int FTL_InvalidInputFileName => 2032;

		public override int ERR_FileNotFound => 2001;

		public override int ERR_NoSourceFile => 31007;

		public override int ERR_CantOpenFileWrite => 2012;

		public override int ERR_OutputWriteFailed => 2012;

		public override int WRN_NoConfigNotOnCommandLine => 2025;

		public override int ERR_BinaryFile => 2015;

		public override int WRN_AnalyzerCannotBeCreated => 42376;

		public override int WRN_NoAnalyzerInAssembly => 42377;

		public override int WRN_UnableToLoadAnalyzer => 42378;

		public override int WRN_AnalyzerReferencesFramework => 42503;

		public override int INF_UnableToLoadSomeTypesInAnalyzer => 50002;

		public override int ERR_CantReadRulesetFile => 37232;

		public override int ERR_CompileCancelled => 0;

		public override int ERR_BadSourceCodeKind => 37285;

		public override int ERR_BadDocumentationMode => 37286;

		public override int ERR_BadCompilationOptionValue => 2014;

		public override int ERR_MutuallyExclusiveOptions => 2046;

		public override int ERR_InvalidDebugInformationFormat => 31453;

		public override int ERR_InvalidOutputName => 31451;

		public override int ERR_InvalidFileAlignment => 31452;

		public override int ERR_InvalidSubsystemVersion => 31391;

		public override int ERR_InvalidHashAlgorithmName => 31219;

		public override int ERR_InvalidInstrumentationKind => 37266;

		public override int ERR_MetadataFileNotAssembly => 31076;

		public override int ERR_MetadataFileNotModule => 31077;

		public override int ERR_InvalidAssemblyMetadata => 31519;

		public override int ERR_InvalidModuleMetadata => 31007;

		public override int ERR_ErrorOpeningAssemblyFile => 31011;

		public override int ERR_ErrorOpeningModuleFile => 31007;

		public override int ERR_MetadataFileNotFound => 2017;

		public override int ERR_MetadataReferencesNotSupported => 37233;

		public override int ERR_LinkedNetmoduleMetadataMustProvideFullPEImage => 37231;

		public override int ERR_CantReadResource => 31509;

		public override int ERR_PublicKeyFileFailure => 36980;

		public override int ERR_PublicKeyContainerFailure => 36981;

		public override int ERR_OptionMustBeAbsolutePath => 37257;

		public override int ERR_CantOpenWin32Resource => 31509;

		public override int ERR_CantOpenWin32Manifest => 31192;

		public override int ERR_CantOpenWin32Icon => 31509;

		public override int ERR_ErrorBuildingWin32Resource => 30136;

		public override int ERR_BadWin32Resource => 30136;

		public override int ERR_ResourceFileNameNotUnique => 35003;

		public override int ERR_ResourceNotUnique => 31502;

		public override int ERR_ResourceInModule => 37227;

		public override int ERR_PermissionSetAttributeFileReadError => 31217;

		public override int ERR_EncodinglessSyntaxTree => 37236;

		public override int WRN_PdbUsingNameTooLong => 42374;

		public override int WRN_PdbLocalNameTooLong => 42373;

		public override int ERR_PdbWritingFailed => 37225;

		public override int ERR_MetadataNameTooLong => 37220;

		public override int ERR_EncReferenceToAddedMember => 37248;

		public override int ERR_TooManyUserStrings => 37255;

		public override int ERR_PeWritingFailure => 37256;

		public override int ERR_ModuleEmitFailure => 36970;

		public override int ERR_EncUpdateFailedMissingAttribute => 36983;

		public override int ERR_BadAssemblyName => 37283;

		public override int ERR_InvalidDebugInfo => 37290;

		public override int WRN_GeneratorFailedDuringInitialization => 42501;

		public override int WRN_GeneratorFailedDuringGeneration => 42502;

		static MessageProvider()
		{
			Instance = new MessageProvider();
			ObjectBinder.RegisterTypeReader(typeof(MessageProvider), (ObjectReader r) => Instance);
		}

		private MessageProvider()
		{
		}

		private void WriteTo(ObjectWriter writer)
		{
		}

		void IObjectWritable.WriteTo(ObjectWriter writer)
		{
			//ILSpy generated this explicit interface implementation from .override directive in WriteTo
			this.WriteTo(writer);
		}

		public override string LoadMessage(int code, CultureInfo language)
		{
			return ErrorFactory.IdToString((ERRID)code, language);
		}

		public override LocalizableString GetMessageFormat(int code)
		{
			return ErrorFactory.GetMessageFormat((ERRID)code);
		}

		public override LocalizableString GetDescription(int code)
		{
			return ErrorFactory.GetDescription((ERRID)code);
		}

		public override LocalizableString GetTitle(int code)
		{
			return ErrorFactory.GetTitle((ERRID)code);
		}

		public override string GetHelpLink(int code)
		{
			return ErrorFactory.GetHelpLink((ERRID)code);
		}

		public override string GetCategory(int code)
		{
			return ErrorFactory.GetCategory((ERRID)code);
		}

		public override DiagnosticSeverity GetSeverity(int code)
		{
			switch (code)
			{
			case -2:
				return (DiagnosticSeverity)(-2);
			case -1:
				return (DiagnosticSeverity)(-1);
			default:
				if (ErrorFacts.IsWarning((ERRID)code))
				{
					return DiagnosticSeverity.Warning;
				}
				if (ErrorFacts.IsInfo((ERRID)code))
				{
					return DiagnosticSeverity.Info;
				}
				if (ErrorFacts.IsHidden((ERRID)code))
				{
					return DiagnosticSeverity.Hidden;
				}
				return DiagnosticSeverity.Error;
			}
		}

		public override int GetWarningLevel(int code)
		{
			if (ErrorFacts.IsWarning((ERRID)code) && code != 2007 && code != 2025 && code != 2034)
			{
				return 1;
			}
			if (ErrorFacts.IsInfo((ERRID)code) || ErrorFacts.IsHidden((ERRID)code))
			{
				return 1;
			}
			return 0;
		}

		public override Diagnostic CreateDiagnostic(int code, Location location, params object[] args)
		{
			return new VBDiagnostic(ErrorFactory.ErrorInfo((ERRID)code, args), location);
		}

		public override Diagnostic CreateDiagnostic(DiagnosticInfo info)
		{
			return new VBDiagnostic(info, Location.None);
		}

		public override string GetErrorDisplayString(ISymbol symbol)
		{
			if (symbol.Kind == SymbolKind.Assembly || symbol.Kind == SymbolKind.Namespace)
			{
				return symbol.ToString();
			}
			return SymbolDisplay.ToDisplayString(symbol, SymbolDisplayFormat.VisualBasicShortErrorMessageFormat);
		}

		public override string GetMessagePrefix(string id, DiagnosticSeverity severity, bool isWarningAsError, CultureInfo culture)
		{
			return string.Format(culture, "{0} {1}", (severity == DiagnosticSeverity.Error || isWarningAsError) ? "error" : "warning", id);
		}

		public override ReportDiagnostic GetDiagnosticReport(DiagnosticInfo diagnosticInfo, CompilationOptions options)
		{
			bool hasDisableDirectiveSuppression = false;
			return VisualBasicDiagnosticFilter.GetDiagnosticReport(diagnosticInfo.Severity, isEnabledByDefault: true, diagnosticInfo.MessageIdentifier, Location.None, diagnosticInfo.Category, options.GeneralDiagnosticOption, options.SpecificDiagnosticOptions, options.SyntaxTreeOptionsProvider, CancellationToken.None, out hasDisableDirectiveSuppression);
		}

		public override void ReportDuplicateMetadataReferenceStrong(DiagnosticBag diagnostics, Location location, MetadataReference reference, AssemblyIdentity identity, MetadataReference equivalentReference, AssemblyIdentity equivalentIdentity)
		{
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicateReferenceStrong, location, reference.Display ?? identity.GetDisplayName(), equivalentReference.Display ?? equivalentIdentity.GetDisplayName());
		}

		public override void ReportDuplicateMetadataReferenceWeak(DiagnosticBag diagnostics, Location location, MetadataReference reference, AssemblyIdentity identity, MetadataReference equivalentReference, AssemblyIdentity equivalentIdentity)
		{
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicateReference2, location, identity.Name, equivalentReference.Display ?? equivalentIdentity.GetDisplayName());
		}

		protected override void ReportInvalidAttributeArgument(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, AttributeData attribute)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_BadAttribute1, attributeSyntax2.ArgumentList.Arguments[parameterIndex].GetLocation(), attribute.AttributeClass);
		}

		protected override void ReportInvalidNamedArgument(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex, ITypeSymbol attributeClass, string parameterName)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_BadAttribute1, attributeSyntax2.ArgumentList.Arguments[namedArgumentIndex].GetLocation(), attributeClass);
		}

		protected override void ReportParameterNotValidForType(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int namedArgumentIndex)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ParameterNotValidForType, attributeSyntax2.ArgumentList.Arguments[namedArgumentIndex].GetLocation());
		}

		protected override void ReportMarshalUnmanagedTypeNotValidForFields(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_MarshalUnmanagedTypeNotValidForFields, attributeSyntax2.ArgumentList.Arguments[parameterIndex].GetLocation(), unmanagedTypeName);
		}

		protected override void ReportMarshalUnmanagedTypeOnlyValidForFields(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, int parameterIndex, string unmanagedTypeName, AttributeData attribute)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_MarshalUnmanagedTypeOnlyValidForFields, attributeSyntax2.ArgumentList.Arguments[parameterIndex].GetLocation(), unmanagedTypeName);
		}

		protected override void ReportAttributeParameterRequired(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_AttributeParameterRequired1, attributeSyntax2.Name.GetLocation(), parameterName);
		}

		protected override void ReportAttributeParameterRequired(DiagnosticBag diagnostics, SyntaxNode attributeSyntax, string parameterName1, string parameterName2)
		{
			AttributeSyntax attributeSyntax2 = (AttributeSyntax)attributeSyntax;
			DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_AttributeParameterRequired2, attributeSyntax2.Name.GetLocation(), parameterName1, parameterName2);
		}
	}
}
