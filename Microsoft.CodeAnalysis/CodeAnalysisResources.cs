using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis
{
    public static class CodeAnalysisResources
    {
        internal static CultureInfo Culture { get; set; }

        internal static string OutputKindNotSupported => GetResourceString("OutputKindNotSupported");

        internal static string PathReturnedByResolveMetadataFileMustBeAbsolute => GetResourceString("PathReturnedByResolveMetadataFileMustBeAbsolute");

        internal static string AssemblyMustHaveAtLeastOneModule => GetResourceString("AssemblyMustHaveAtLeastOneModule");

        internal static string ModuleCopyCannotBeUsedToCreateAssemblyMetadata => GetResourceString("ModuleCopyCannotBeUsedToCreateAssemblyMetadata");

        internal static string Unresolved => GetResourceString("Unresolved");

        internal static string Assembly => GetResourceString("Assembly");

        internal static string Class1 => GetResourceString("Class1");

        internal static string Constructor => GetResourceString("Constructor");

        internal static string Delegate1 => GetResourceString("Delegate1");

        internal static string Enum1 => GetResourceString("Enum1");

        internal static string Event1 => GetResourceString("Event1");

        internal static string Field => GetResourceString("Field");

        internal static string TypeParameter => GetResourceString("TypeParameter");

        internal static string Interface1 => GetResourceString("Interface1");

        internal static string Method => GetResourceString("Method");

        internal static string Module => GetResourceString("Module");

        internal static string Parameter => GetResourceString("Parameter");

        internal static string Property => GetResourceString("Property");

        internal static string Return1 => GetResourceString("Return1");

        internal static string Struct1 => GetResourceString("Struct1");

        internal static string CannotCreateReferenceToSubmission => GetResourceString("CannotCreateReferenceToSubmission");

        internal static string CannotCreateReferenceToModule => GetResourceString("CannotCreateReferenceToModule");

        internal static string InMemoryAssembly => GetResourceString("InMemoryAssembly");

        internal static string InMemoryModule => GetResourceString("InMemoryModule");

        internal static string SizeHasToBePositive => GetResourceString("SizeHasToBePositive");

        internal static string AssemblyFileNotFound => GetResourceString("AssemblyFileNotFound");

        internal static string CannotEmbedInteropTypesFromModule => GetResourceString("CannotEmbedInteropTypesFromModule");

        internal static string CannotAliasModule => GetResourceString("CannotAliasModule");

        internal static string InvalidAlias => GetResourceString("InvalidAlias");

        internal static string GetMetadataMustReturnInstance => GetResourceString("GetMetadataMustReturnInstance");

        internal static string Value_too_large_to_be_represented_as_a_30_bit_unsigned_integer => GetResourceString("Value_too_large_to_be_represented_as_a_30_bit_unsigned_integer");

        internal static string Arrays_with_more_than_one_dimension_cannot_be_serialized => GetResourceString("Arrays_with_more_than_one_dimension_cannot_be_serialized");

        internal static string InvalidAssemblyName => GetResourceString("InvalidAssemblyName");

        internal static string AbsolutePathExpected => GetResourceString("AbsolutePathExpected");

        internal static string EmptyKeyInPathMap => GetResourceString("EmptyKeyInPathMap");

        internal static string NullValueInPathMap => GetResourceString("NullValueInPathMap");

        internal static string CompilationOptionsMustNotHaveErrors => GetResourceString("CompilationOptionsMustNotHaveErrors");

        internal static string ReturnTypeCannotBeValuePointerbyRefOrOpen => GetResourceString("ReturnTypeCannotBeValuePointerbyRefOrOpen");

        internal static string ReturnTypeCannotBeVoidByRefOrOpen => GetResourceString("ReturnTypeCannotBeVoidByRefOrOpen");

        internal static string TypeMustBeSameAsHostObjectTypeOfPreviousSubmission => GetResourceString("TypeMustBeSameAsHostObjectTypeOfPreviousSubmission");

        internal static string PreviousSubmissionHasErrors => GetResourceString("PreviousSubmissionHasErrors");

        internal static string InvalidOutputKindForSubmission => GetResourceString("InvalidOutputKindForSubmission");

        internal static string InvalidCompilationOptions => GetResourceString("InvalidCompilationOptions");

        internal static string ResourceStreamProviderShouldReturnNonNullStream => GetResourceString("ResourceStreamProviderShouldReturnNonNullStream");

        internal static string ReferenceResolverShouldReturnReadableNonNullStream => GetResourceString("ReferenceResolverShouldReturnReadableNonNullStream");

        internal static string EmptyOrInvalidResourceName => GetResourceString("EmptyOrInvalidResourceName");

        internal static string EmptyOrInvalidFileName => GetResourceString("EmptyOrInvalidFileName");

        internal static string ResourceDataProviderShouldReturnNonNullStream => GetResourceString("ResourceDataProviderShouldReturnNonNullStream");

        public static string FileNotFound => GetResourceString("FileNotFound");

        internal static string PathReturnedByResolveStrongNameKeyFileMustBeAbsolute => GetResourceString("PathReturnedByResolveStrongNameKeyFileMustBeAbsolute");

        internal static string TypeMustBeASubclassOfSyntaxAnnotation => GetResourceString("TypeMustBeASubclassOfSyntaxAnnotation");

        internal static string InvalidModuleName => GetResourceString("InvalidModuleName");

        internal static string FileSizeExceedsMaximumAllowed => GetResourceString("FileSizeExceedsMaximumAllowed");

        internal static string NameCannotBeNull => GetResourceString("NameCannotBeNull");

        internal static string NameCannotBeEmpty => GetResourceString("NameCannotBeEmpty");

        internal static string NameCannotStartWithWhitespace => GetResourceString("NameCannotStartWithWhitespace");

        internal static string NameContainsInvalidCharacter => GetResourceString("NameContainsInvalidCharacter");

        internal static string SpanDoesNotIncludeStartOfLine => GetResourceString("SpanDoesNotIncludeStartOfLine");

        internal static string SpanDoesNotIncludeEndOfLine => GetResourceString("SpanDoesNotIncludeEndOfLine");

        internal static string StartMustNotBeNegative => GetResourceString("StartMustNotBeNegative");

        internal static string EndMustNotBeLessThanStart => GetResourceString("EndMustNotBeLessThanStart");

        internal static string InvalidContentType => GetResourceString("InvalidContentType");

        internal static string ExpectedNonEmptyPublicKey => GetResourceString("ExpectedNonEmptyPublicKey");

        internal static string InvalidSizeOfPublicKeyToken => GetResourceString("InvalidSizeOfPublicKeyToken");

        public static string InvalidCharactersInAssemblyName => GetResourceString("InvalidCharactersInAssemblyName");

        internal static string InvalidCharactersInAssemblyCultureName => GetResourceString("InvalidCharactersInAssemblyCultureName");

        internal static string StreamMustSupportReadAndSeek => GetResourceString("StreamMustSupportReadAndSeek");

        internal static string StreamMustSupportRead => GetResourceString("StreamMustSupportRead");

        internal static string StreamMustSupportWrite => GetResourceString("StreamMustSupportWrite");

        internal static string PdbStreamUnexpectedWhenEmbedding => GetResourceString("PdbStreamUnexpectedWhenEmbedding");

        internal static string PdbStreamUnexpectedWhenEmittingMetadataOnly => GetResourceString("PdbStreamUnexpectedWhenEmittingMetadataOnly");

        internal static string MetadataPeStreamUnexpectedWhenEmittingMetadataOnly => GetResourceString("MetadataPeStreamUnexpectedWhenEmittingMetadataOnly");

        internal static string IncludingPrivateMembersUnexpectedWhenEmittingToMetadataPeStream => GetResourceString("IncludingPrivateMembersUnexpectedWhenEmittingToMetadataPeStream");

        internal static string MustIncludePrivateMembersUnlessRefAssembly => GetResourceString("MustIncludePrivateMembersUnlessRefAssembly");

        internal static string EmbeddingPdbUnexpectedWhenEmittingMetadata => GetResourceString("EmbeddingPdbUnexpectedWhenEmittingMetadata");

        internal static string CannotTargetNetModuleWhenEmittingRefAssembly => GetResourceString("CannotTargetNetModuleWhenEmittingRefAssembly");

        internal static string InvalidHash => GetResourceString("InvalidHash");

        internal static string UnsupportedHashAlgorithm => GetResourceString("UnsupportedHashAlgorithm");

        public static string InconsistentLanguageVersions => GetResourceString("InconsistentLanguageVersions");

        internal static string CoffResourceInvalidRelocation => GetResourceString("CoffResourceInvalidRelocation");

        internal static string CoffResourceInvalidSectionSize => GetResourceString("CoffResourceInvalidSectionSize");

        internal static string CoffResourceInvalidSymbol => GetResourceString("CoffResourceInvalidSymbol");

        internal static string CoffResourceMissingSection => GetResourceString("CoffResourceMissingSection");

        internal static string IconStreamUnexpectedFormat => GetResourceString("IconStreamUnexpectedFormat");

        internal static string InvalidCultureName => GetResourceString("InvalidCultureName");

        internal static string WinRTIdentityCantBeRetargetable => GetResourceString("WinRTIdentityCantBeRetargetable");

        internal static string PEImageNotAvailable => GetResourceString("PEImageNotAvailable");

        internal static string AssemblySigningNotSupported => GetResourceString("AssemblySigningNotSupported");

        public static string XmlReferencesNotSupported => GetResourceString("XmlReferencesNotSupported");

        internal static string FailedToResolveRuleSetName => GetResourceString("FailedToResolveRuleSetName");

        internal static string InvalidRuleSetInclude => GetResourceString("InvalidRuleSetInclude");

        internal static string CompilerAnalyzerFailure => GetResourceString("CompilerAnalyzerFailure");

        internal static string CompilerAnalyzerThrows => GetResourceString("CompilerAnalyzerThrows");

        internal static string CompilerAnalyzerThrowsDescription => GetResourceString("CompilerAnalyzerThrowsDescription");

        internal static string AnalyzerDriverFailure => GetResourceString("AnalyzerDriverFailure");

        internal static string AnalyzerDriverThrows => GetResourceString("AnalyzerDriverThrows");

        internal static string AnalyzerDriverThrowsDescription => GetResourceString("AnalyzerDriverThrowsDescription");

        internal static string PEImageDoesntContainManagedMetadata => GetResourceString("PEImageDoesntContainManagedMetadata");

        internal static string ChangesMustNotOverlap => GetResourceString("ChangesMustNotOverlap");

        internal static string DiagnosticIdCantBeNullOrWhitespace => GetResourceString("DiagnosticIdCantBeNullOrWhitespace");

        internal static string SuppressionIdCantBeNullOrWhitespace => GetResourceString("SuppressionIdCantBeNullOrWhitespace");

        internal static string RuleSetHasDuplicateRules => GetResourceString("RuleSetHasDuplicateRules");

        internal static string CantCreateModuleReferenceToAssembly => GetResourceString("CantCreateModuleReferenceToAssembly");

        internal static string CantCreateReferenceToDynamicAssembly => GetResourceString("CantCreateReferenceToDynamicAssembly");

        internal static string CantCreateReferenceToAssemblyWithoutLocation => GetResourceString("CantCreateReferenceToAssemblyWithoutLocation");

        internal static string ArgumentCannotBeEmpty => GetResourceString("ArgumentCannotBeEmpty");

        internal static string ArgumentElementCannotBeNull => GetResourceString("ArgumentElementCannotBeNull");

        internal static string UnsupportedDiagnosticReported => GetResourceString("UnsupportedDiagnosticReported");

        internal static string UnsupportedSuppressionReported => GetResourceString("UnsupportedSuppressionReported");

        internal static string InvalidDiagnosticSuppressionReported => GetResourceString("InvalidDiagnosticSuppressionReported");

        internal static string NonReportedDiagnosticCannotBeSuppressed => GetResourceString("NonReportedDiagnosticCannotBeSuppressed");

        internal static string InvalidDiagnosticIdReported => GetResourceString("InvalidDiagnosticIdReported");

        internal static string InvalidDiagnosticLocationReported => GetResourceString("InvalidDiagnosticLocationReported");

        internal static string SupportedDiagnosticsHasNullDescriptor => GetResourceString("SupportedDiagnosticsHasNullDescriptor");

        internal static string SupportedSuppressionsHasNullDescriptor => GetResourceString("SupportedSuppressionsHasNullDescriptor");

        internal static string The_type_0_is_not_understood_by_the_serialization_binder => GetResourceString("The_type_0_is_not_understood_by_the_serialization_binder");

        internal static string Cannot_deserialize_type_0 => GetResourceString("Cannot_deserialize_type_0");

        internal static string Cannot_serialize_type_0 => GetResourceString("Cannot_serialize_type_0");

        internal static string InvalidNodeToTrack => GetResourceString("InvalidNodeToTrack");

        public static string NodeOrTokenOutOfSequence => GetResourceString("NodeOrTokenOutOfSequence");

        public static string UnexpectedTypeOfNodeInList => GetResourceString("UnexpectedTypeOfNodeInList");

        public static string MissingListItem => GetResourceString("MissingListItem");

        internal static string InvalidPublicKey => GetResourceString("InvalidPublicKey");

        internal static string InvalidPublicKeyToken => GetResourceString("InvalidPublicKeyToken");

        internal static string InvalidDataAtOffset => GetResourceString("InvalidDataAtOffset");

        internal static string SymWriterNotDeterministic => GetResourceString("SymWriterNotDeterministic");

        internal static string SymWriterOlderVersionThanRequired => GetResourceString("SymWriterOlderVersionThanRequired");

        internal static string SymWriterDoesNotSupportSourceLink => GetResourceString("SymWriterDoesNotSupportSourceLink");

        internal static string RuleSetBadAttributeValue => GetResourceString("RuleSetBadAttributeValue");

        internal static string RuleSetMissingAttribute => GetResourceString("RuleSetMissingAttribute");

        internal static string KeepAliveIsNotAnInteger => GetResourceString("KeepAliveIsNotAnInteger");

        internal static string KeepAliveIsTooSmall => GetResourceString("KeepAliveIsTooSmall");

        internal static string KeepAliveWithoutShared => GetResourceString("KeepAliveWithoutShared");

        internal static string MismatchedVersion => GetResourceString("MismatchedVersion");

        internal static string MissingKeepAlive => GetResourceString("MissingKeepAlive");

        internal static string AnalyzerTotalExecutionTime => GetResourceString("AnalyzerTotalExecutionTime");

        internal static string MultithreadedAnalyzerExecutionNote => GetResourceString("MultithreadedAnalyzerExecutionNote");

        internal static string AnalyzerExecutionTimeColumnHeader => GetResourceString("AnalyzerExecutionTimeColumnHeader");

        internal static string AnalyzerNameColumnHeader => GetResourceString("AnalyzerNameColumnHeader");

        internal static string NoAnalyzersFound => GetResourceString("NoAnalyzersFound");

        internal static string DuplicateAnalyzerInstances => GetResourceString("DuplicateAnalyzerInstances");

        internal static string UnsupportedAnalyzerInstance => GetResourceString("UnsupportedAnalyzerInstance");

        internal static string InvalidTree => GetResourceString("InvalidTree");

        internal static string InvalidAdditionalFile => GetResourceString("InvalidAdditionalFile");

        internal static string ResourceStreamEndedUnexpectedly => GetResourceString("ResourceStreamEndedUnexpectedly");

        internal static string SharedArgumentMissing => GetResourceString("SharedArgumentMissing");

        internal static string ExceptionContext => GetResourceString("ExceptionContext");

        internal static string AnonymousTypeMemberAndNamesCountMismatch2 => GetResourceString("AnonymousTypeMemberAndNamesCountMismatch2");

        internal static string AnonymousTypeArgumentCountMismatch2 => GetResourceString("AnonymousTypeArgumentCountMismatch2");

        internal static string InconsistentSyntaxTreeFeature => GetResourceString("InconsistentSyntaxTreeFeature");

        internal static string ReferenceOfTypeIsInvalid1 => GetResourceString("ReferenceOfTypeIsInvalid1");

        internal static string MetadataRefNotFoundToRemove1 => GetResourceString("MetadataRefNotFoundToRemove1");

        internal static string TupleElementNameCountMismatch => GetResourceString("TupleElementNameCountMismatch");

        internal static string TupleElementNameEmpty => GetResourceString("TupleElementNameEmpty");

        internal static string TupleElementLocationCountMismatch => GetResourceString("TupleElementLocationCountMismatch");

        internal static string TupleElementNullableAnnotationCountMismatch => GetResourceString("TupleElementNullableAnnotationCountMismatch");

        internal static string TuplesNeedAtLeastTwoElements => GetResourceString("TuplesNeedAtLeastTwoElements");

        internal static string CompilationReferencesAssembliesWithDifferentAutoGeneratedVersion => GetResourceString("CompilationReferencesAssembliesWithDifferentAutoGeneratedVersion");

        public static string TupleUnderlyingTypeMustBeTupleCompatible => GetResourceString("TupleUnderlyingTypeMustBeTupleCompatible");

        internal static string UnrecognizedResourceFileFormat => GetResourceString("UnrecognizedResourceFileFormat");

        internal static string SourceTextCannotBeEmbedded => GetResourceString("SourceTextCannotBeEmbedded");

        internal static string StreamIsTooLong => GetResourceString("StreamIsTooLong");

        internal static string EmbeddedTextsRequirePdb => GetResourceString("EmbeddedTextsRequirePdb");

        internal static string TheStreamCannotBeWrittenTo => GetResourceString("TheStreamCannotBeWrittenTo");

        public static string ElementIsExpected => GetResourceString("ElementIsExpected");

        public static string SeparatorIsExpected => GetResourceString("SeparatorIsExpected");

        public static string TheStreamCannotBeReadFrom => GetResourceString("TheStreamCannotBeReadFrom");

        internal static string Deserialization_reader_for_0_read_incorrect_number_of_values => GetResourceString("Deserialization_reader_for_0_read_incorrect_number_of_values");

        public static string Stream_contains_invalid_data => GetResourceString("Stream_contains_invalid_data");

        internal static string InvalidDiagnosticSpanReported => GetResourceString("InvalidDiagnosticSpanReported");

        public static string ExceptionEnablingMulticoreJit => GetResourceString("ExceptionEnablingMulticoreJit");

        internal static string NotARootOperation => GetResourceString("NotARootOperation");

        internal static string OperationHasNullSemanticModel => GetResourceString("OperationHasNullSemanticModel");

        internal static string InvalidOperationBlockForAnalysisContext => GetResourceString("InvalidOperationBlockForAnalysisContext");

        internal static string IsSymbolAccessibleBadWithin => GetResourceString("IsSymbolAccessibleBadWithin");

        internal static string IsSymbolAccessibleWrongAssembly => GetResourceString("IsSymbolAccessibleWrongAssembly");

        internal static string OperationMustNotBeControlFlowGraphPart => GetResourceString("OperationMustNotBeControlFlowGraphPart");

        internal static string A_language_name_cannot_be_specified_for_this_option => GetResourceString("A_language_name_cannot_be_specified_for_this_option");

        internal static string A_language_name_must_be_specified_for_this_option => GetResourceString("A_language_name_must_be_specified_for_this_option");

        internal static string WRN_InvalidSeverityInAnalyzerConfig => GetResourceString("WRN_InvalidSeverityInAnalyzerConfig");

        internal static string WRN_InvalidSeverityInAnalyzerConfig_Title => GetResourceString("WRN_InvalidSeverityInAnalyzerConfig_Title");

        internal static string SuppressionDiagnosticDescriptorTitle => GetResourceString("SuppressionDiagnosticDescriptorTitle");

        internal static string SuppressionDiagnosticDescriptorMessage => GetResourceString("SuppressionDiagnosticDescriptorMessage");

        public static string ModuleHasInvalidAttributes => GetResourceString("ModuleHasInvalidAttributes");

        public static string UnableToDetermineSpecificCauseOfFailure => GetResourceString("UnableToDetermineSpecificCauseOfFailure");

        public static string ChangingVersionOfAssemblyReferenceIsNotAllowedDuringDebugging => GetResourceString("ChangingVersionOfAssemblyReferenceIsNotAllowedDuringDebugging");

        internal static string DisableAnalyzerDiagnosticsMessage => GetResourceString("DisableAnalyzerDiagnosticsMessage");

        internal static string Single_type_per_generator_0 => GetResourceString("Single_type_per_generator_0");

        internal static string WRN_MultipleGlobalAnalyzerKeys => GetResourceString("WRN_MultipleGlobalAnalyzerKeys");

        internal static string WRN_MultipleGlobalAnalyzerKeys_Title => GetResourceString("WRN_MultipleGlobalAnalyzerKeys_Title");

        internal static string HintNameUniquePerGenerator => GetResourceString("HintNameUniquePerGenerator");

        internal static string HintNameInvalidChar => GetResourceString("HintNameInvalidChar");

        internal static string SourceTextRequiresEncoding => GetResourceString("SourceTextRequiresEncoding");

        internal static string AssemblyReferencesNetFramework => GetResourceString("AssemblyReferencesNetFramework");

        internal static string WRN_InvalidGlobalSectionName => GetResourceString("WRN_InvalidGlobalSectionName");

        internal static string WRN_InvalidGlobalSectionName_Title => GetResourceString("WRN_InvalidGlobalSectionName_Title");

        internal static string ChangesMustBeWithinBoundsOfSourceText => GetResourceString("ChangesMustBeWithinBoundsOfSourceText");

        public static System.Resources.ResourceManager ResourceManager
        {
            get { return Properties.Resources.ResourceManager; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetResourceString(string resourceKey, string defaultValue = null)
        {
            return Properties.Resources.ResourceManager.GetString(resourceKey, Culture) ?? defaultValue;
        }
    }
}
