using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class ErrorFacts
    {
        private const string s_titleSuffix = "_Title";

        private const string s_descriptionSuffix = "_Description";

        private static readonly Lazy<ImmutableDictionary<ErrorCode, string>> s_categoriesMap;

        public static readonly ImmutableHashSet<string> NullableWarnings;

        private static ResourceManager s_resourceManager;

        private static ResourceManager ResourceManager
        {
            get
            {
                if (s_resourceManager == null)
                {
                    s_resourceManager = new ResourceManager(typeof(Properties.Resources).FullName, typeof(ErrorCode).GetTypeInfo().Assembly);
                }
                return s_resourceManager;
            }
        }

        static ErrorFacts()
        {
            s_categoriesMap = new Lazy<ImmutableDictionary<ErrorCode, string>>(CreateCategoriesMap);
            ImmutableHashSet<string>.Builder builder = ImmutableHashSet.CreateBuilder<string>();
            builder.Add(GetId(ErrorCode.WRN_NullReferenceAssignment));
            builder.Add(GetId(ErrorCode.WRN_NullReferenceReceiver));
            builder.Add(GetId(ErrorCode.WRN_NullReferenceReturn));
            builder.Add(GetId(ErrorCode.WRN_NullReferenceArgument));
            builder.Add(GetId(ErrorCode.WRN_UninitializedNonNullableField));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInAssignment));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInArgument));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInArgumentForOutput));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate));
            builder.Add(GetId(ErrorCode.WRN_NullAsNonNullable));
            builder.Add(GetId(ErrorCode.WRN_NullableValueTypeMayBeNull));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterConstraint));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterNotNullConstraint));
            builder.Add(GetId(ErrorCode.WRN_ThrowPossibleNull));
            builder.Add(GetId(ErrorCode.WRN_UnboxPossibleNull));
            builder.Add(GetId(ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull));
            builder.Add(GetId(ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen));
            builder.Add(GetId(ErrorCode.WRN_ConvertingNullableToNonNullable));
            builder.Add(GetId(ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment));
            builder.Add(GetId(ErrorCode.WRN_ParameterConditionallyDisallowsNull));
            builder.Add(GetId(ErrorCode.WRN_ShouldNotReturn));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnOverride));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnExplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInConstraintsOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase));
            builder.Add(GetId(ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList));
            builder.Add(GetId(ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation));
            builder.Add(GetId(ErrorCode.WRN_NullReferenceInitializer));
            builder.Add(GetId(ErrorCode.WRN_ShouldNotReturn));
            builder.Add(GetId(ErrorCode.WRN_DoesNotReturnMismatch));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation));
            builder.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride));
            builder.Add(GetId(ErrorCode.WRN_MemberNotNull));
            builder.Add(GetId(ErrorCode.WRN_MemberNotNullBadMember));
            builder.Add(GetId(ErrorCode.WRN_MemberNotNullWhen));
            builder.Add(GetId(ErrorCode.WRN_ParameterDisallowsNull));
            builder.Add(GetId(ErrorCode.WRN_ParameterNotNullIfNotNull));
            builder.Add(GetId(ErrorCode.WRN_ReturnNotNullIfNotNull));
            NullableWarnings = builder.ToImmutable();
        }

        private static string GetId(ErrorCode errorCode)
        {
            return MessageProvider.Instance.GetIdForErrorCode((int)errorCode);
        }

        private static ImmutableDictionary<ErrorCode, string> CreateCategoriesMap()
        {
            return new Dictionary<ErrorCode, string>().ToImmutableDictionary();
        }

        internal static DiagnosticSeverity GetSeverity(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.Void:
                    return (DiagnosticSeverity)(-2);
                case ErrorCode.Unknown:
                    return (DiagnosticSeverity)(-1);
                default:
                    if (IsWarning(code))
                    {
                        return DiagnosticSeverity.Warning;
                    }
                    if (IsInfo(code))
                    {
                        return DiagnosticSeverity.Info;
                    }
                    if (IsHidden(code))
                    {
                        return DiagnosticSeverity.Hidden;
                    }
                    return DiagnosticSeverity.Error;
            }
        }

        public static string GetMessage(MessageID code, CultureInfo culture)
        {
            return ResourceManager.GetString(code.ToString("G"), culture);
        }

        public static string GetMessage(ErrorCode code, CultureInfo culture)
        {
            return ResourceManager.GetString(code.ToString(), culture);
        }

        public static LocalizableResourceString GetMessageFormat(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString(), ResourceManager, typeof(ErrorFacts));
        }

        public static LocalizableResourceString GetTitle(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString() + "_Title", ResourceManager, typeof(ErrorFacts));
        }

        public static LocalizableResourceString GetDescription(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString() + "_Description", ResourceManager, typeof(ErrorFacts));
        }

        public static string GetHelpLink(ErrorCode code)
        {
            return "https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k(" + GetId(code) + ")";
        }

        public static string GetCategory(ErrorCode code)
        {
            if (s_categoriesMap.Value.TryGetValue(code, out var value))
            {
                return value;
            }
            return "Compiler";
        }

        public static string GetMessage(XmlParseErrorCode id, CultureInfo culture)
        {
            return ResourceManager.GetString(id.ToString(), culture);
        }

        internal static int GetWarningLevel(ErrorCode code)
        {
            if (IsInfo(code) || IsHidden(code))
            {
                return 1;
            }
            if (code <= ErrorCode.WRN_DotOnDefault)
            {
                switch (code)
                {
                    case ErrorCode.WRN_InvalidMainSig:
                    case ErrorCode.WRN_LowercaseEllSuffix:
                    case ErrorCode.WRN_NewNotRequired:
                    case ErrorCode.WRN_MainCantBeGeneric:
                    case ErrorCode.WRN_ProtectedInSealed:
                    case ErrorCode.WRN_UnassignedInternalField:
                    case ErrorCode.WRN_MissingParamTag:
                    case ErrorCode.WRN_MissingXMLComment:
                    case ErrorCode.WRN_MissingTypeParamTag:
                        break;
                    case ErrorCode.WRN_UnreferencedEvent:
                    case ErrorCode.WRN_DuplicateUsing:
                    case ErrorCode.WRN_UnreferencedVar:
                    case ErrorCode.WRN_UnreferencedField:
                    case ErrorCode.WRN_UnreferencedVarAssg:
                    case ErrorCode.WRN_SequentialOnPartialClass:
                    case ErrorCode.WRN_UnreferencedFieldAssg:
                    case ErrorCode.WRN_AmbiguousXMLReference:
                    case ErrorCode.WRN_PossibleMistakenNullStatement:
                    case ErrorCode.WRN_EqualsWithoutGetHashCode:
                    case ErrorCode.WRN_EqualityOpWithoutEquals:
                    case ErrorCode.WRN_EqualityOpWithoutGetHashCode:
                    case ErrorCode.WRN_IncorrectBooleanAssg:
                    case ErrorCode.WRN_BitwiseOrSignExtend:
                    case ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter:
                    case ErrorCode.WRN_DebugFullNameTooLong:
                    case ErrorCode.WRN_InvalidAssemblyName:
                    case ErrorCode.WRN_UnifyReferenceBldRev:
                    case ErrorCode.WRN_AssignmentToSelf:
                    case ErrorCode.WRN_ComparisonToSelf:
                        goto IL_0ae7;
                    case ErrorCode.WRN_NewRequired:
                    case ErrorCode.WRN_NewOrOverrideExpected:
                    case ErrorCode.WRN_UnreachableCode:
                    case ErrorCode.WRN_UnreferencedLabel:
                    case ErrorCode.WRN_NegativeArrayIndex:
                    case ErrorCode.WRN_BadRefCompareLeft:
                    case ErrorCode.WRN_BadRefCompareRight:
                    case ErrorCode.WRN_PatternIsAmbiguous:
                    case ErrorCode.WRN_PatternNotPublicOrNotInstance:
                    case ErrorCode.WRN_PatternBadSignature:
                    case ErrorCode.WRN_SameFullNameThisNsAgg:
                    case ErrorCode.WRN_SameFullNameThisAggAgg:
                    case ErrorCode.WRN_SameFullNameThisAggNs:
                    case ErrorCode.WRN_GlobalAliasDefn:
                    case ErrorCode.WRN_AlwaysNull:
                    case ErrorCode.WRN_CmpAlwaysFalse:
                    case ErrorCode.WRN_GotoCaseShouldConvert:
                    case ErrorCode.WRN_NubExprIsConstBool:
                    case ErrorCode.WRN_ExplicitImplCollision:
                    case ErrorCode.WRN_DeprecatedSymbolStr:
                    case ErrorCode.WRN_VacuousIntegralComp:
                    case ErrorCode.WRN_AssignmentToLockOrDispose:
                    case ErrorCode.WRN_DeprecatedCollectionInitAddStr:
                    case ErrorCode.WRN_DeprecatedCollectionInitAdd:
                    case ErrorCode.WRN_DuplicateParamTag:
                    case ErrorCode.WRN_UnmatchedParamTag:
                    case ErrorCode.WRN_UnprocessedXMLComment:
                    case ErrorCode.WRN_InvalidSearchPathDir:
                    case ErrorCode.WRN_UnifyReferenceMajMin:
                    case ErrorCode.WRN_DuplicateTypeParamTag:
                    case ErrorCode.WRN_UnmatchedTypeParamTag:
                        goto IL_0ae9;
                    case ErrorCode.WRN_IsAlwaysTrue:
                    case ErrorCode.WRN_IsAlwaysFalse:
                    case ErrorCode.WRN_ByRefNonAgileField:
                    case ErrorCode.WRN_VolatileByRef:
                    case ErrorCode.WRN_FinalizeMethod:
                    case ErrorCode.WRN_DeprecatedSymbol:
                    case ErrorCode.WRN_ExternMethodNoImplementation:
                    case ErrorCode.WRN_AttributeLocationOnBadDeclaration:
                    case ErrorCode.WRN_InvalidAttributeLocation:
                    case ErrorCode.WRN_NonObsoleteOverridingObsolete:
                    case ErrorCode.WRN_CoClassWithoutComImport:
                    case ErrorCode.WRN_ObsoleteOverridingNonObsolete:
                    case ErrorCode.WRN_ExternCtorNoImplementation:
                    case ErrorCode.WRN_WarningDirective:
                    case ErrorCode.WRN_UnreachableGeneralCatch:
                    case ErrorCode.WRN_DefaultValueForUnconsumedLocation:
                    case ErrorCode.WRN_IdentifierOrNumericLiteralExpected:
                    case ErrorCode.WRN_EmptySwitch:
                    case ErrorCode.WRN_XMLParseError:
                    case ErrorCode.WRN_BadXMLRef:
                    case ErrorCode.WRN_BadXMLRefParamType:
                    case ErrorCode.WRN_BadXMLRefReturnType:
                    case ErrorCode.WRN_BadXMLRefSyntax:
                    case ErrorCode.WRN_FailedInclude:
                    case ErrorCode.WRN_InvalidInclude:
                    case ErrorCode.WRN_XMLParseIncludeError:
                    case ErrorCode.WRN_ALinkWarn:
                    case ErrorCode.WRN_CmdOptionConflictsSource:
                    case ErrorCode.WRN_IllegalPragma:
                    case ErrorCode.WRN_IllegalPPWarning:
                    case ErrorCode.WRN_BadRestoreNumber:
                    case ErrorCode.WRN_NonECMAFeature:
                    case ErrorCode.WRN_ErrorOverride:
                    case ErrorCode.WRN_MultiplePredefTypes:
                    case ErrorCode.WRN_TooManyLinesForDebugger:
                    case ErrorCode.WRN_CallOnNonAgileField:
                    case ErrorCode.WRN_InvalidNumber:
                    case ErrorCode.WRN_IllegalPPChecksum:
                    case ErrorCode.WRN_EndOfPPLineExpected:
                    case ErrorCode.WRN_ConflictingChecksum:
                    case ErrorCode.WRN_DotOnDefault:
                        goto IL_0aeb;
                    default:
                        goto IL_0aed;
                }
                goto IL_0ae5;
            }
            if (code <= ErrorCode.WRN_UnqualifiedNestedTypeInCref)
            {
                switch (code)
                {
                    case ErrorCode.WRN_StaticInAsOrIs:
                        break;
                    case ErrorCode.WRN_InvalidVersionFormat:
                        goto IL_0ae5;
                    case ErrorCode.WRN_IsDynamicIsConfusing:
                        goto IL_0ae7;
                    case ErrorCode.WRN_UnmatchedParamRefTag:
                    case ErrorCode.WRN_UnmatchedTypeParamRefTag:
                    case ErrorCode.WRN_CantHaveManifestForModule:
                    case ErrorCode.WRN_DynamicDispatchToConditionalMethod:
                    case ErrorCode.WRN_NoSources:
                    case ErrorCode.WRN_CLS_MeaninglessOnPrivateType:
                    case ErrorCode.WRN_CLS_AssemblyNotCLS2:
                    case ErrorCode.WRN_MainIgnored:
                    case ErrorCode.WRN_UnqualifiedNestedTypeInCref:
                        goto IL_0ae9;
                    case ErrorCode.WRN_BadXMLRefTypeVar:
                    case ErrorCode.WRN_ReferencedAssemblyReferencesLinkedPIA:
                    case ErrorCode.WRN_MultipleRuntimeImplementationMatches:
                    case ErrorCode.WRN_MultipleRuntimeOverrideMatches:
                    case ErrorCode.WRN_AsyncLacksAwaits:
                    case ErrorCode.WRN_FileAlreadyIncluded:
                    case ErrorCode.WRN_NoConfigNotOnCommandLine:
                    case ErrorCode.WRN_DefineIdentifierRequired:
                    case ErrorCode.WRN_BadUILang:
                    case ErrorCode.WRN_CLS_NoVarArgs:
                    case ErrorCode.WRN_CLS_BadArgType:
                    case ErrorCode.WRN_CLS_BadReturnType:
                    case ErrorCode.WRN_CLS_BadFieldPropType:
                    case ErrorCode.WRN_CLS_BadIdentifierCase:
                    case ErrorCode.WRN_CLS_OverloadRefOut:
                    case ErrorCode.WRN_CLS_OverloadUnnamed:
                    case ErrorCode.WRN_CLS_BadIdentifier:
                    case ErrorCode.WRN_CLS_BadBase:
                    case ErrorCode.WRN_CLS_BadInterfaceMember:
                    case ErrorCode.WRN_CLS_NoAbstractMembers:
                    case ErrorCode.WRN_CLS_NotOnModules:
                    case ErrorCode.WRN_CLS_ModuleMissingCLS:
                    case ErrorCode.WRN_CLS_AssemblyNotCLS:
                    case ErrorCode.WRN_CLS_BadAttributeType:
                    case ErrorCode.WRN_CLS_ArrayArgumentToAttribute:
                    case ErrorCode.WRN_CLS_NotOnModules2:
                    case ErrorCode.WRN_CLS_IllegalTrueInFalse:
                    case ErrorCode.WRN_CLS_MeaninglessOnParam:
                    case ErrorCode.WRN_CLS_MeaninglessOnReturn:
                    case ErrorCode.WRN_CLS_BadTypeVar:
                    case ErrorCode.WRN_CLS_VolatileField:
                    case ErrorCode.WRN_CLS_BadInterface:
                    case ErrorCode.WRN_UnobservedAwaitableExpression:
                    case ErrorCode.WRN_CallerLineNumberParamForUnconsumedLocation:
                    case ErrorCode.WRN_CallerFilePathParamForUnconsumedLocation:
                    case ErrorCode.WRN_CallerMemberNameParamForUnconsumedLocation:
                    case ErrorCode.WRN_DelaySignButNoKey:
                    case ErrorCode.WRN_CallerFilePathPreferredOverCallerMemberName:
                    case ErrorCode.WRN_CallerLineNumberPreferredOverCallerMemberName:
                    case ErrorCode.WRN_CallerLineNumberPreferredOverCallerFilePath:
                    case ErrorCode.WRN_AssemblyAttributeFromModuleIsOverridden:
                    case ErrorCode.WRN_FilterIsConstantTrue:
                    case ErrorCode.WRN_UnimplementedCommandLineSwitch:
                    case ErrorCode.WRN_ReferencedAssemblyDoesNotHaveStrongName:
                    case ErrorCode.WRN_RefCultureMismatch:
                    case ErrorCode.WRN_ConflictingMachineAssembly:
                        goto IL_0aeb;
                    default:
                        goto IL_0aed;
                }
            }
            else
            {
                switch (code)
                {
                    case ErrorCode.WRN_NubExprIsConstBool2:
                    case ErrorCode.WRN_PrecedenceInversion:
                    case ErrorCode.WRN_UnassignedThisAutoProperty:
                    case ErrorCode.WRN_UnassignedThis:
                    case ErrorCode.WRN_ParamUnassigned:
                    case ErrorCode.WRN_UseDefViolationProperty:
                    case ErrorCode.WRN_UseDefViolationField:
                    case ErrorCode.WRN_UseDefViolationThis:
                    case ErrorCode.WRN_UseDefViolationOut:
                    case ErrorCode.WRN_UseDefViolation:
                    case ErrorCode.WRN_SyncAndAsyncEntryPoints:
                    case ErrorCode.WRN_ParameterIsStaticClass:
                    case ErrorCode.WRN_ReturnTypeIsStaticClass:
                        break;
                    case ErrorCode.WRN_PdbLocalNameTooLong:
                    case ErrorCode.WRN_UnreferencedLocalFunction:
                    case ErrorCode.WRN_RecordEqualsWithoutGetHashCode:
                        goto IL_0ae7;
                    case ErrorCode.WRN_NoRuntimeMetadataVersion:
                        goto IL_0ae9;
                    case ErrorCode.WRN_AnalyzerCannotBeCreated:
                    case ErrorCode.WRN_NoAnalyzerInAssembly:
                    case ErrorCode.WRN_UnableToLoadAnalyzer:
                    case ErrorCode.WRN_AlignmentMagnitude:
                    case ErrorCode.WRN_AttributeIgnoredWhenPublicSigning:
                    case ErrorCode.WRN_TupleLiteralNameMismatch:
                    case ErrorCode.WRN_Experimental:
                    case ErrorCode.WRN_FilterIsConstantFalse:
                    case ErrorCode.WRN_FilterIsConstantFalseRedundantTryCatch:
                    case ErrorCode.WRN_AttributesOnBackingFieldsNotAvailable:
                    case ErrorCode.WRN_TupleBinopLiteralNameMismatch:
                    case ErrorCode.WRN_TypeParameterSameAsOuterMethodTypeParameter:
                    case ErrorCode.WRN_UnconsumedEnumeratorCancellationAttributeUsage:
                    case ErrorCode.WRN_UndecoratedCancellationTokenParameter:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustive:
                    case ErrorCode.WRN_CaseConstantNamedUnderscore:
                    case ErrorCode.WRN_IsTypeNamedUnderscore:
                    case ErrorCode.WRN_GivenExpressionNeverMatchesPattern:
                    case ErrorCode.WRN_GivenExpressionAlwaysMatchesConstant:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue:
                    case ErrorCode.WRN_ThrowPossibleNull:
                    case ErrorCode.WRN_ConvertingNullableToNonNullable:
                    case ErrorCode.WRN_NullReferenceAssignment:
                    case ErrorCode.WRN_NullReferenceReceiver:
                    case ErrorCode.WRN_NullReferenceReturn:
                    case ErrorCode.WRN_NullReferenceArgument:
                    case ErrorCode.WRN_UnboxPossibleNull:
                    case ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnExplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation:
                    case ErrorCode.WRN_UninitializedNonNullableField:
                    case ErrorCode.WRN_NullabilityMismatchInAssignment:
                    case ErrorCode.WRN_NullabilityMismatchInArgument:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate:
                    case ErrorCode.WRN_NullabilityMismatchInArgumentForOutput:
                    case ErrorCode.WRN_NullAsNonNullable:
                    case ErrorCode.WRN_NullableValueTypeMayBeNull:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterConstraint:
                    case ErrorCode.WRN_MissingNonNullTypesContextForAnnotation:
                    case ErrorCode.WRN_NullabilityMismatchInConstraintsOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint:
                    case ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface:
                    case ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase:
                    case ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull:
                    case ErrorCode.WRN_ImplicitCopyInReadOnlyMember:
                    case ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation:
                    case ErrorCode.WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode:
                    case ErrorCode.WRN_NullReferenceInitializer:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterNotNullConstraint:
                    case ErrorCode.WRN_ParameterConditionallyDisallowsNull:
                    case ErrorCode.WRN_ShouldNotReturn:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation:
                    case ErrorCode.WRN_DoesNotReturnMismatch:
                    case ErrorCode.WRN_MemberNotNull:
                    case ErrorCode.WRN_MemberNotNullWhen:
                    case ErrorCode.WRN_MemberNotNullBadMember:
                    case ErrorCode.WRN_ParameterDisallowsNull:
                    case ErrorCode.WRN_ConstOutOfRangeChecked:
                    case ErrorCode.WRN_GeneratorFailedDuringInitialization:
                    case ErrorCode.WRN_GeneratorFailedDuringGeneration:
                    case ErrorCode.WRN_GivenExpressionAlwaysMatchesPattern:
                    case ErrorCode.WRN_IsPatternAlways:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial:
                    case ErrorCode.WRN_ParameterNotNullIfNotNull:
                    case ErrorCode.WRN_ReturnNotNullIfNotNull:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveWithWhen:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen:
                    case ErrorCode.WRN_AnalyzerReferencesFramework:
                    case ErrorCode.WRN_RecordNamedDisallowed:
                    case ErrorCode.WRN_UnreadRecordParameter:
                    case ErrorCode.WRN_DoNotCompareFunctionPointers:
                        goto IL_0aeb;
                    default:
                        goto IL_0aed;
                }
            }
            return 5;
        IL_0ae5:
            return 4;
        IL_0ae9:
            return 2;
        IL_0ae7:
            return 3;
        IL_0aed:
            return 0;
        IL_0aeb:
            return 1;
        }

        internal static bool PreventsSuccessfulDelegateConversion(ErrorCode code)
        {
            if (code == ErrorCode.Void || code == ErrorCode.Unknown)
            {
                return false;
            }
            if (IsWarning(code))
            {
                return false;
            }
            switch (code)
            {
                case ErrorCode.ERR_DuplicateParamName:
                case ErrorCode.ERR_LocalDuplicate:
                case ErrorCode.ERR_LocalIllegallyOverrides:
                case ErrorCode.ERR_LocalSameNameAsTypeParam:
                case ErrorCode.ERR_DeprecatedSymbolStr:
                case ErrorCode.ERR_DeprecatedCollectionInitAddStr:
                case ErrorCode.ERR_QueryRangeVariableOverrides:
                case ErrorCode.ERR_QueryRangeVariableSameAsTypeParam:
                    return false;
                default:
                    return true;
            }
        }

        internal static bool PreventsSuccessfulDelegateConversion(DiagnosticBag diagnostics)
        {
            foreach (Diagnostic item in diagnostics.AsEnumerable())
            {
                if (PreventsSuccessfulDelegateConversion((ErrorCode)item.Code))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool PreventsSuccessfulDelegateConversion(ImmutableArray<Diagnostic> diagnostics)
        {
            ImmutableArray<Diagnostic>.Enumerator enumerator = diagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (PreventsSuccessfulDelegateConversion((ErrorCode)enumerator.Current.Code))
                {
                    return true;
                }
            }
            return false;
        }

        internal static ErrorCode GetStaticClassParameterCode(bool useWarning)
        {
            if (!useWarning)
            {
                return ErrorCode.ERR_ParameterIsStaticClass;
            }
            return ErrorCode.WRN_ParameterIsStaticClass;
        }

        internal static ErrorCode GetStaticClassReturnCode(bool useWarning)
        {
            if (!useWarning)
            {
                return ErrorCode.ERR_ReturnTypeIsStaticClass;
            }
            return ErrorCode.WRN_ReturnTypeIsStaticClass;
        }

        public static bool IsWarning(ErrorCode code)
        {
            if (code <= ErrorCode.WRN_BadXMLRefTypeVar)
            {
                if (code <= ErrorCode.WRN_ExternMethodNoImplementation)
                {
                    if (code <= ErrorCode.WRN_BadRefCompareRight)
                    {
                        if (code <= ErrorCode.WRN_NewOrOverrideExpected)
                        {
                            if (code <= ErrorCode.WRN_LowercaseEllSuffix)
                            {
                                if (code == ErrorCode.WRN_InvalidMainSig || code == ErrorCode.WRN_UnreferencedEvent || code == ErrorCode.WRN_LowercaseEllSuffix)
                                {
                                    goto IL_0a83;
                                }
                            }
                            else if (code == ErrorCode.WRN_DuplicateUsing || (uint)(code - 108) <= 1u || code == ErrorCode.WRN_NewOrOverrideExpected)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if (code <= ErrorCode.WRN_UnreferencedField)
                        {
                            if (code == ErrorCode.WRN_UnreachableCode || code == ErrorCode.WRN_UnreferencedLabel || (uint)(code - 168) <= 1u)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if (code <= ErrorCode.WRN_ByRefNonAgileField)
                        {
                            if ((uint)(code - 183) <= 1u || code == ErrorCode.WRN_ByRefNonAgileField)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if (code == ErrorCode.WRN_UnreferencedVarAssg || (uint)(code - 251) <= 2u)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code <= ErrorCode.WRN_GlobalAliasDefn)
                    {
                        if (code <= ErrorCode.WRN_MainCantBeGeneric)
                        {
                            if ((uint)(code - 278) <= 2u || code == ErrorCode.WRN_SequentialOnPartialClass || code == ErrorCode.WRN_MainCantBeGeneric)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if (code <= ErrorCode.WRN_VolatileByRef)
                        {
                            if (code == ErrorCode.WRN_UnreferencedFieldAssg || (uint)(code - 419) <= 1u)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if ((uint)(code - 435) <= 2u || code == ErrorCode.WRN_GlobalAliasDefn)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code <= ErrorCode.WRN_GotoCaseShouldConvert)
                    {
                        if (code == ErrorCode.WRN_AlwaysNull || (uint)(code - 464) <= 1u || code == ErrorCode.WRN_GotoCaseShouldConvert)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code <= ErrorCode.WRN_DeprecatedSymbol)
                    {
                        if ((uint)(code - 472) <= 1u || code == ErrorCode.WRN_DeprecatedSymbol)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code == ErrorCode.WRN_DeprecatedSymbolStr || code == ErrorCode.WRN_ExternMethodNoImplementation)
                    {
                        goto IL_0a83;
                    }
                    goto IL_0a85;
                }
                switch (code)
                {
                    case ErrorCode.WRN_ProtectedInSealed:
                    case ErrorCode.WRN_PossibleMistakenNullStatement:
                    case ErrorCode.WRN_UnassignedInternalField:
                    case ErrorCode.WRN_VacuousIntegralComp:
                    case ErrorCode.WRN_AttributeLocationOnBadDeclaration:
                    case ErrorCode.WRN_InvalidAttributeLocation:
                    case ErrorCode.WRN_EqualsWithoutGetHashCode:
                    case ErrorCode.WRN_EqualityOpWithoutEquals:
                    case ErrorCode.WRN_EqualityOpWithoutGetHashCode:
                    case ErrorCode.WRN_IncorrectBooleanAssg:
                    case ErrorCode.WRN_NonObsoleteOverridingObsolete:
                    case ErrorCode.WRN_BitwiseOrSignExtend:
                    case ErrorCode.WRN_CoClassWithoutComImport:
                    case ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter:
                    case ErrorCode.WRN_AssignmentToLockOrDispose:
                    case ErrorCode.WRN_ObsoleteOverridingNonObsolete:
                    case ErrorCode.WRN_DebugFullNameTooLong:
                    case ErrorCode.WRN_ExternCtorNoImplementation:
                    case ErrorCode.WRN_WarningDirective:
                    case ErrorCode.WRN_UnreachableGeneralCatch:
                    case ErrorCode.WRN_DeprecatedCollectionInitAddStr:
                    case ErrorCode.WRN_DeprecatedCollectionInitAdd:
                    case ErrorCode.WRN_DefaultValueForUnconsumedLocation:
                    case ErrorCode.WRN_IdentifierOrNumericLiteralExpected:
                    case ErrorCode.WRN_EmptySwitch:
                    case ErrorCode.WRN_XMLParseError:
                    case ErrorCode.WRN_DuplicateParamTag:
                    case ErrorCode.WRN_UnmatchedParamTag:
                    case ErrorCode.WRN_MissingParamTag:
                    case ErrorCode.WRN_BadXMLRef:
                    case ErrorCode.WRN_BadXMLRefParamType:
                    case ErrorCode.WRN_BadXMLRefReturnType:
                    case ErrorCode.WRN_BadXMLRefSyntax:
                    case ErrorCode.WRN_UnprocessedXMLComment:
                    case ErrorCode.WRN_FailedInclude:
                    case ErrorCode.WRN_InvalidInclude:
                    case ErrorCode.WRN_MissingXMLComment:
                    case ErrorCode.WRN_XMLParseIncludeError:
                    case ErrorCode.WRN_ALinkWarn:
                    case ErrorCode.WRN_CmdOptionConflictsSource:
                    case ErrorCode.WRN_IllegalPragma:
                    case ErrorCode.WRN_IllegalPPWarning:
                    case ErrorCode.WRN_BadRestoreNumber:
                    case ErrorCode.WRN_NonECMAFeature:
                    case ErrorCode.WRN_ErrorOverride:
                    case ErrorCode.WRN_InvalidSearchPathDir:
                    case ErrorCode.WRN_MultiplePredefTypes:
                    case ErrorCode.WRN_TooManyLinesForDebugger:
                    case ErrorCode.WRN_CallOnNonAgileField:
                    case ErrorCode.WRN_InvalidNumber:
                    case ErrorCode.WRN_IllegalPPChecksum:
                    case ErrorCode.WRN_EndOfPPLineExpected:
                    case ErrorCode.WRN_ConflictingChecksum:
                    case ErrorCode.WRN_InvalidAssemblyName:
                    case ErrorCode.WRN_UnifyReferenceMajMin:
                    case ErrorCode.WRN_UnifyReferenceBldRev:
                    case ErrorCode.WRN_DuplicateTypeParamTag:
                    case ErrorCode.WRN_UnmatchedTypeParamTag:
                    case ErrorCode.WRN_MissingTypeParamTag:
                    case ErrorCode.WRN_AssignmentToSelf:
                    case ErrorCode.WRN_ComparisonToSelf:
                    case ErrorCode.WRN_DotOnDefault:
                    case ErrorCode.WRN_BadXMLRefTypeVar:
                        break;
                    default:
                        goto IL_0a85;
                }
            }
            else if (code <= ErrorCode.WRN_PdbLocalNameTooLong)
            {
                switch (code)
                {
                    case ErrorCode.WRN_UnmatchedParamRefTag:
                    case ErrorCode.WRN_UnmatchedTypeParamRefTag:
                    case ErrorCode.WRN_ReferencedAssemblyReferencesLinkedPIA:
                    case ErrorCode.WRN_CantHaveManifestForModule:
                    case ErrorCode.WRN_MultipleRuntimeImplementationMatches:
                    case ErrorCode.WRN_MultipleRuntimeOverrideMatches:
                    case ErrorCode.WRN_DynamicDispatchToConditionalMethod:
                    case ErrorCode.WRN_IsDynamicIsConfusing:
                    case ErrorCode.WRN_AsyncLacksAwaits:
                    case ErrorCode.WRN_FileAlreadyIncluded:
                    case ErrorCode.WRN_NoSources:
                    case ErrorCode.WRN_NoConfigNotOnCommandLine:
                    case ErrorCode.WRN_DefineIdentifierRequired:
                    case ErrorCode.WRN_BadUILang:
                    case ErrorCode.WRN_CLS_NoVarArgs:
                    case ErrorCode.WRN_CLS_BadArgType:
                    case ErrorCode.WRN_CLS_BadReturnType:
                    case ErrorCode.WRN_CLS_BadFieldPropType:
                    case ErrorCode.WRN_CLS_BadIdentifierCase:
                    case ErrorCode.WRN_CLS_OverloadRefOut:
                    case ErrorCode.WRN_CLS_OverloadUnnamed:
                    case ErrorCode.WRN_CLS_BadIdentifier:
                    case ErrorCode.WRN_CLS_BadBase:
                    case ErrorCode.WRN_CLS_BadInterfaceMember:
                    case ErrorCode.WRN_CLS_NoAbstractMembers:
                    case ErrorCode.WRN_CLS_NotOnModules:
                    case ErrorCode.WRN_CLS_ModuleMissingCLS:
                    case ErrorCode.WRN_CLS_AssemblyNotCLS:
                    case ErrorCode.WRN_CLS_BadAttributeType:
                    case ErrorCode.WRN_CLS_ArrayArgumentToAttribute:
                    case ErrorCode.WRN_CLS_NotOnModules2:
                    case ErrorCode.WRN_CLS_IllegalTrueInFalse:
                    case ErrorCode.WRN_CLS_MeaninglessOnPrivateType:
                    case ErrorCode.WRN_CLS_AssemblyNotCLS2:
                    case ErrorCode.WRN_CLS_MeaninglessOnParam:
                    case ErrorCode.WRN_CLS_MeaninglessOnReturn:
                    case ErrorCode.WRN_CLS_BadTypeVar:
                    case ErrorCode.WRN_CLS_VolatileField:
                    case ErrorCode.WRN_CLS_BadInterface:
                    case ErrorCode.WRN_UnobservedAwaitableExpression:
                    case ErrorCode.WRN_CallerLineNumberParamForUnconsumedLocation:
                    case ErrorCode.WRN_CallerFilePathParamForUnconsumedLocation:
                    case ErrorCode.WRN_CallerMemberNameParamForUnconsumedLocation:
                    case ErrorCode.WRN_MainIgnored:
                    case ErrorCode.WRN_StaticInAsOrIs:
                    case ErrorCode.WRN_DelaySignButNoKey:
                    case ErrorCode.WRN_InvalidVersionFormat:
                    case ErrorCode.WRN_CallerFilePathPreferredOverCallerMemberName:
                    case ErrorCode.WRN_CallerLineNumberPreferredOverCallerMemberName:
                    case ErrorCode.WRN_CallerLineNumberPreferredOverCallerFilePath:
                    case ErrorCode.WRN_AssemblyAttributeFromModuleIsOverridden:
                    case ErrorCode.WRN_FilterIsConstantTrue:
                    case ErrorCode.WRN_UnimplementedCommandLineSwitch:
                    case ErrorCode.WRN_ReferencedAssemblyDoesNotHaveStrongName:
                    case ErrorCode.WRN_RefCultureMismatch:
                    case ErrorCode.WRN_ConflictingMachineAssembly:
                    case ErrorCode.WRN_UnqualifiedNestedTypeInCref:
                    case ErrorCode.WRN_NoRuntimeMetadataVersion:
                    case ErrorCode.WRN_PdbLocalNameTooLong:
                        break;
                    default:
                        goto IL_0a85;
                }
            }
            else
            {
                if (code <= ErrorCode.WRN_SwitchExpressionNotExhaustive)
                {
                    if (code <= ErrorCode.WRN_Experimental)
                    {
                        if (code <= ErrorCode.WRN_AlignmentMagnitude)
                        {
                            if ((uint)(code - 8032) <= 2u || code == ErrorCode.WRN_NubExprIsConstBool2 || code == ErrorCode.WRN_AlignmentMagnitude)
                            {
                                goto IL_0a83;
                            }
                        }
                        else if (code == ErrorCode.WRN_AttributeIgnoredWhenPublicSigning || code == ErrorCode.WRN_TupleLiteralNameMismatch || code == ErrorCode.WRN_Experimental)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code <= ErrorCode.WRN_AttributesOnBackingFieldsNotAvailable)
                    {
                        if (code == ErrorCode.WRN_UnreferencedLocalFunction || (uint)(code - 8359) <= 1u || code == ErrorCode.WRN_AttributesOnBackingFieldsNotAvailable)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if (code <= ErrorCode.WRN_TypeParameterSameAsOuterMethodTypeParameter)
                    {
                        if (code == ErrorCode.WRN_TupleBinopLiteralNameMismatch || code == ErrorCode.WRN_TypeParameterSameAsOuterMethodTypeParameter)
                        {
                            goto IL_0a83;
                        }
                    }
                    else if ((uint)(code - 8424) <= 1u || code == ErrorCode.WRN_SwitchExpressionNotExhaustive)
                    {
                        goto IL_0a83;
                    }
                    goto IL_0a85;
                }
                switch (code)
                {
                    case ErrorCode.WRN_CaseConstantNamedUnderscore:
                    case ErrorCode.WRN_IsTypeNamedUnderscore:
                    case ErrorCode.WRN_GivenExpressionNeverMatchesPattern:
                    case ErrorCode.WRN_GivenExpressionAlwaysMatchesConstant:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue:
                    case ErrorCode.WRN_ThrowPossibleNull:
                    case ErrorCode.WRN_ConvertingNullableToNonNullable:
                    case ErrorCode.WRN_NullReferenceAssignment:
                    case ErrorCode.WRN_NullReferenceReceiver:
                    case ErrorCode.WRN_NullReferenceReturn:
                    case ErrorCode.WRN_NullReferenceArgument:
                    case ErrorCode.WRN_UnboxPossibleNull:
                    case ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInTypeOnExplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation:
                    case ErrorCode.WRN_UninitializedNonNullableField:
                    case ErrorCode.WRN_NullabilityMismatchInAssignment:
                    case ErrorCode.WRN_NullabilityMismatchInArgument:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate:
                    case ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate:
                    case ErrorCode.WRN_NullabilityMismatchInArgumentForOutput:
                    case ErrorCode.WRN_NullAsNonNullable:
                    case ErrorCode.WRN_NullableValueTypeMayBeNull:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterConstraint:
                    case ErrorCode.WRN_MissingNonNullTypesContextForAnnotation:
                    case ErrorCode.WRN_NullabilityMismatchInConstraintsOnImplicitImplementation:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint:
                    case ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface:
                    case ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase:
                    case ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull:
                    case ErrorCode.WRN_ImplicitCopyInReadOnlyMember:
                    case ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation:
                    case ErrorCode.WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode:
                    case ErrorCode.WRN_NullReferenceInitializer:
                    case ErrorCode.WRN_NullabilityMismatchInTypeParameterNotNullConstraint:
                    case ErrorCode.WRN_ParameterConditionallyDisallowsNull:
                    case ErrorCode.WRN_ShouldNotReturn:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation:
                    case ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation:
                    case ErrorCode.WRN_DoesNotReturnMismatch:
                    case ErrorCode.WRN_MemberNotNull:
                    case ErrorCode.WRN_MemberNotNullWhen:
                    case ErrorCode.WRN_MemberNotNullBadMember:
                    case ErrorCode.WRN_ParameterDisallowsNull:
                    case ErrorCode.WRN_ConstOutOfRangeChecked:
                    case ErrorCode.WRN_GeneratorFailedDuringInitialization:
                    case ErrorCode.WRN_GeneratorFailedDuringGeneration:
                    case ErrorCode.WRN_GivenExpressionAlwaysMatchesPattern:
                    case ErrorCode.WRN_IsPatternAlways:
                    case ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial:
                    case ErrorCode.WRN_ParameterNotNullIfNotNull:
                    case ErrorCode.WRN_ReturnNotNullIfNotNull:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveWithWhen:
                    case ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen:
                    case ErrorCode.WRN_PrecedenceInversion:
                    case ErrorCode.WRN_AnalyzerReferencesFramework:
                    case ErrorCode.WRN_RecordEqualsWithoutGetHashCode:
                    case ErrorCode.WRN_RecordNamedDisallowed:
                    case ErrorCode.WRN_UnassignedThisAutoProperty:
                    case ErrorCode.WRN_UnassignedThis:
                    case ErrorCode.WRN_ParamUnassigned:
                    case ErrorCode.WRN_UseDefViolationProperty:
                    case ErrorCode.WRN_UseDefViolationField:
                    case ErrorCode.WRN_UseDefViolationThis:
                    case ErrorCode.WRN_UseDefViolationOut:
                    case ErrorCode.WRN_UseDefViolation:
                    case ErrorCode.WRN_SyncAndAsyncEntryPoints:
                    case ErrorCode.WRN_ParameterIsStaticClass:
                    case ErrorCode.WRN_ReturnTypeIsStaticClass:
                    case ErrorCode.WRN_UnreadRecordParameter:
                    case ErrorCode.WRN_DoNotCompareFunctionPointers:
                        break;
                    default:
                        goto IL_0a85;
                }
            }
            goto IL_0a83;
        IL_0a85:
            return false;
        IL_0a83:
            return true;
        }

        public static bool IsFatal(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.FTL_MetadataCantOpenFile:
                case ErrorCode.FTL_DebugEmitFailure:
                case ErrorCode.FTL_BadCodepage:
                case ErrorCode.FTL_InvalidTarget:
                case ErrorCode.FTL_InvalidInputFileName:
                case ErrorCode.FTL_OutputFileExists:
                case ErrorCode.FTL_BadChecksumAlgorithm:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInfo(ErrorCode code)
        {
            if (code == ErrorCode.INF_UnableToLoadSomeTypesInAnalyzer)
            {
                return true;
            }
            return false;
        }

        public static bool IsHidden(ErrorCode code)
        {
            if ((uint)(code - 8019) <= 1u)
            {
                return true;
            }
            return false;
        }
    }
}
