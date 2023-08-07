// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static partial class ErrorFacts
    {
        private const string s_titleSuffix = "_Title";
        private const string s_descriptionSuffix = "_Description";
        private static readonly Lazy<ImmutableDictionary<ErrorCode, string>> s_categoriesMap = new(CreateCategoriesMap);
        public static readonly ImmutableHashSet<string> NullableWarnings;

        static ErrorFacts()
        {
            ImmutableHashSet<string>.Builder nullableWarnings = ImmutableHashSet.CreateBuilder<string>();

            nullableWarnings.Add(GetId(ErrorCode.WRN_NullReferenceAssignment));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullReferenceReceiver));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullReferenceReturn));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullReferenceArgument));
            nullableWarnings.Add(GetId(ErrorCode.WRN_UninitializedNonNullableField));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInAssignment));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInArgument));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInArgumentForOutput));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullAsNonNullable));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullableValueTypeMayBeNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterConstraint));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeParameterNotNullConstraint));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ThrowPossibleNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_UnboxPossibleNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen));

            nullableWarnings.Add(GetId(ErrorCode.WRN_ConvertingNullableToNonNullable));
            nullableWarnings.Add(GetId(ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ParameterConditionallyDisallowsNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ShouldNotReturn));

            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnOverride));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInTypeOnExplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInConstraintsOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase));
            nullableWarnings.Add(GetId(ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_NullReferenceInitializer));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ShouldNotReturn));
            nullableWarnings.Add(GetId(ErrorCode.WRN_DoesNotReturnMismatch));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation));
            nullableWarnings.Add(GetId(ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride));
            nullableWarnings.Add(GetId(ErrorCode.WRN_MemberNotNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_MemberNotNullBadMember));
            nullableWarnings.Add(GetId(ErrorCode.WRN_MemberNotNullWhen));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ParameterDisallowsNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ParameterNotNullIfNotNull));
            nullableWarnings.Add(GetId(ErrorCode.WRN_ReturnNotNullIfNotNull));

            NullableWarnings = nullableWarnings.ToImmutable();
        }

        private static string GetId(ErrorCode errorCode)
        {
            return MessageProvider.Instance.GetIdForErrorCode((int)errorCode);
        }

        private static ImmutableDictionary<ErrorCode, string> CreateCategoriesMap()
        {
            var map = new Dictionary<ErrorCode, string>()
            {
                // { ERROR_CODE,    CATEGORY }
            };

            return map.ToImmutableDictionary();
        }

        internal static DiagnosticSeverity GetSeverity(ErrorCode code)
        {
            if (code == ErrorCode.Void)
            {
                return InternalDiagnosticSeverity.Void;
            }
            else if (code == ErrorCode.Unknown)
            {
                return InternalDiagnosticSeverity.Unknown;
            }
            else if (IsWarning(code))
            {
                return DiagnosticSeverity.Warning;
            }
            else if (IsInfo(code))
            {
                return DiagnosticSeverity.Info;
            }
            else if (IsHidden(code))
            {
                return DiagnosticSeverity.Hidden;
            }
            else
            {
                return DiagnosticSeverity.Error;
            }
        }

        /// <remarks>Don't call this during a parse--it loads resources</remarks>
        public static string GetMessage(MessageID code, CultureInfo culture)
        {
            string message = ResourceManager.GetString(code.ToString(), culture);
            return message;
        }

        /// <remarks>Don't call this during a parse--it loads resources</remarks>
        public static string GetMessage(ErrorCode code, CultureInfo culture)
        {
            string message = ResourceManager.GetString(code.ToString(), culture);
            return message;
        }

        public static LocalizableResourceString GetMessageFormat(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString(), ResourceManager, typeof(ErrorFacts));
        }

        public static LocalizableResourceString GetTitle(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString() + s_titleSuffix, ResourceManager, typeof(ErrorFacts));
        }

        public static LocalizableResourceString GetDescription(ErrorCode code)
        {
            return new LocalizableResourceString(code.ToString() + s_descriptionSuffix, ResourceManager, typeof(ErrorFacts));
        }

        public static string GetHelpLink(ErrorCode code)
        {
            return $"https://msdn.microsoft.com/query/roslyn.query?appId=roslyn&k=k({GetId(code)})";
        }

        public static string GetCategory(ErrorCode code)
        {
            if (s_categoriesMap.Value.TryGetValue(code, out string category))
            {
                return category;
            }

            return Diagnostic.CompilerDiagnosticCategory;
        }

        /// <remarks>Don't call this during a parse--it loads resources</remarks>
        public static string GetMessage(XmlParseErrorCode id, CultureInfo culture)
        {
            return ResourceManager.GetString(id.ToString(), culture);
        }

        private static System.Resources.ResourceManager s_resourceManager;
        private static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                s_resourceManager ??= new System.Resources.ResourceManager(typeof(Properties.Resources).FullName, typeof(ErrorCode).GetTypeInfo().Assembly);

                return s_resourceManager;
            }
        }

        internal static int GetWarningLevel(ErrorCode code)
        {
            if (IsInfo(code) || IsHidden(code))
            {
                // Info and hidden diagnostics should always be produced because some analyzers depend on them.
                return Diagnostic.InfoAndHiddenWarningLevel;
            }

            return code switch
            {
                ErrorCode.WRN_NubExprIsConstBool2 or ErrorCode.WRN_StaticInAsOrIs or ErrorCode.WRN_PrecedenceInversion or ErrorCode.WRN_UnassignedThisAutoProperty or ErrorCode.WRN_UnassignedThis or ErrorCode.WRN_ParamUnassigned or ErrorCode.WRN_UseDefViolationProperty or ErrorCode.WRN_UseDefViolationField or ErrorCode.WRN_UseDefViolationThis or ErrorCode.WRN_UseDefViolationOut or ErrorCode.WRN_UseDefViolation or ErrorCode.WRN_SyncAndAsyncEntryPoints or ErrorCode.WRN_ParameterIsStaticClass or ErrorCode.WRN_ReturnTypeIsStaticClass => 5,// Warning level 5 is exclusively for warnings introduced in the compiler
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            // shipped with dotnet 5 (C# 9) and that can be reported for pre-existing code.
                ErrorCode.WRN_InvalidMainSig or ErrorCode.WRN_LowercaseEllSuffix or ErrorCode.WRN_NewNotRequired or ErrorCode.WRN_MainCantBeGeneric or ErrorCode.WRN_ProtectedInSealed or ErrorCode.WRN_UnassignedInternalField or ErrorCode.WRN_MissingParamTag or ErrorCode.WRN_MissingXMLComment or ErrorCode.WRN_MissingTypeParamTag or ErrorCode.WRN_InvalidVersionFormat => 4,
                ErrorCode.WRN_UnreferencedEvent or ErrorCode.WRN_DuplicateUsing or ErrorCode.WRN_UnreferencedVar or ErrorCode.WRN_UnreferencedField or ErrorCode.WRN_UnreferencedVarAssg or ErrorCode.WRN_UnreferencedLocalFunction or ErrorCode.WRN_SequentialOnPartialClass or ErrorCode.WRN_UnreferencedFieldAssg or ErrorCode.WRN_AmbiguousXMLReference or ErrorCode.WRN_PossibleMistakenNullStatement or ErrorCode.WRN_EqualsWithoutGetHashCode or ErrorCode.WRN_EqualityOpWithoutEquals or ErrorCode.WRN_EqualityOpWithoutGetHashCode or ErrorCode.WRN_IncorrectBooleanAssg or ErrorCode.WRN_BitwiseOrSignExtend or ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter or ErrorCode.WRN_InvalidAssemblyName or ErrorCode.WRN_UnifyReferenceBldRev or ErrorCode.WRN_AssignmentToSelf or ErrorCode.WRN_ComparisonToSelf or ErrorCode.WRN_IsDynamicIsConfusing or ErrorCode.WRN_DebugFullNameTooLong or ErrorCode.WRN_PdbLocalNameTooLong or ErrorCode.WRN_RecordEqualsWithoutGetHashCode => 3,
                ErrorCode.WRN_NewRequired or ErrorCode.WRN_NewOrOverrideExpected or ErrorCode.WRN_UnreachableCode or ErrorCode.WRN_UnreferencedLabel or ErrorCode.WRN_NegativeArrayIndex or ErrorCode.WRN_BadRefCompareLeft or ErrorCode.WRN_BadRefCompareRight or ErrorCode.WRN_PatternIsAmbiguous or ErrorCode.WRN_PatternNotPublicOrNotInstance or ErrorCode.WRN_PatternBadSignature or ErrorCode.WRN_SameFullNameThisNsAgg or ErrorCode.WRN_SameFullNameThisAggAgg or ErrorCode.WRN_SameFullNameThisAggNs or ErrorCode.WRN_GlobalAliasDefn or ErrorCode.WRN_AlwaysNull or ErrorCode.WRN_CmpAlwaysFalse or ErrorCode.WRN_GotoCaseShouldConvert or ErrorCode.WRN_NubExprIsConstBool or ErrorCode.WRN_ExplicitImplCollision or ErrorCode.WRN_DeprecatedSymbolStr or ErrorCode.WRN_VacuousIntegralComp or ErrorCode.WRN_AssignmentToLockOrDispose or ErrorCode.WRN_DeprecatedCollectionInitAddStr or ErrorCode.WRN_DeprecatedCollectionInitAdd or ErrorCode.WRN_DuplicateParamTag or ErrorCode.WRN_UnmatchedParamTag or ErrorCode.WRN_UnprocessedXMLComment or ErrorCode.WRN_InvalidSearchPathDir or ErrorCode.WRN_UnifyReferenceMajMin or ErrorCode.WRN_DuplicateTypeParamTag or ErrorCode.WRN_UnmatchedTypeParamTag or ErrorCode.WRN_UnmatchedParamRefTag or ErrorCode.WRN_UnmatchedTypeParamRefTag or ErrorCode.WRN_CantHaveManifestForModule or ErrorCode.WRN_DynamicDispatchToConditionalMethod or ErrorCode.WRN_NoSources or ErrorCode.WRN_CLS_MeaninglessOnPrivateType or ErrorCode.WRN_CLS_AssemblyNotCLS2 or ErrorCode.WRN_MainIgnored or ErrorCode.WRN_UnqualifiedNestedTypeInCref or ErrorCode.WRN_NoRuntimeMetadataVersion => 2,
                ErrorCode.WRN_IsAlwaysTrue or ErrorCode.WRN_IsAlwaysFalse or ErrorCode.WRN_ByRefNonAgileField or ErrorCode.WRN_VolatileByRef or ErrorCode.WRN_FinalizeMethod or ErrorCode.WRN_DeprecatedSymbol or ErrorCode.WRN_ExternMethodNoImplementation or ErrorCode.WRN_AttributeLocationOnBadDeclaration or ErrorCode.WRN_InvalidAttributeLocation or ErrorCode.WRN_NonObsoleteOverridingObsolete or ErrorCode.WRN_CoClassWithoutComImport or ErrorCode.WRN_ObsoleteOverridingNonObsolete or ErrorCode.WRN_ExternCtorNoImplementation or ErrorCode.WRN_WarningDirective or ErrorCode.WRN_UnreachableGeneralCatch or ErrorCode.WRN_DefaultValueForUnconsumedLocation or ErrorCode.WRN_EmptySwitch or ErrorCode.WRN_XMLParseError or ErrorCode.WRN_BadXMLRef or ErrorCode.WRN_BadXMLRefParamType or ErrorCode.WRN_BadXMLRefReturnType or ErrorCode.WRN_BadXMLRefSyntax or ErrorCode.WRN_FailedInclude or ErrorCode.WRN_InvalidInclude or ErrorCode.WRN_XMLParseIncludeError or ErrorCode.WRN_ALinkWarn or ErrorCode.WRN_AssemblyAttributeFromModuleIsOverridden or ErrorCode.WRN_CmdOptionConflictsSource or ErrorCode.WRN_IllegalPragma or ErrorCode.WRN_IllegalPPWarning or ErrorCode.WRN_BadRestoreNumber or ErrorCode.WRN_NonECMAFeature or ErrorCode.WRN_ErrorOverride or ErrorCode.WRN_MultiplePredefTypes or ErrorCode.WRN_TooManyLinesForDebugger or ErrorCode.WRN_CallOnNonAgileField or ErrorCode.WRN_InvalidNumber or ErrorCode.WRN_IllegalPPChecksum or ErrorCode.WRN_EndOfPPLineExpected or ErrorCode.WRN_ConflictingChecksum or ErrorCode.WRN_DotOnDefault or ErrorCode.WRN_BadXMLRefTypeVar or ErrorCode.WRN_ReferencedAssemblyReferencesLinkedPIA or ErrorCode.WRN_MultipleRuntimeImplementationMatches or ErrorCode.WRN_MultipleRuntimeOverrideMatches or ErrorCode.WRN_FileAlreadyIncluded or ErrorCode.WRN_NoConfigNotOnCommandLine or ErrorCode.WRN_AnalyzerCannotBeCreated or ErrorCode.WRN_NoAnalyzerInAssembly or ErrorCode.WRN_UnableToLoadAnalyzer or ErrorCode.WRN_DefineIdentifierRequired or ErrorCode.WRN_CLS_NoVarArgs or ErrorCode.WRN_CLS_BadArgType or ErrorCode.WRN_CLS_BadReturnType or ErrorCode.WRN_CLS_BadFieldPropType or ErrorCode.WRN_CLS_BadIdentifierCase or ErrorCode.WRN_CLS_OverloadRefOut or ErrorCode.WRN_CLS_OverloadUnnamed or ErrorCode.WRN_CLS_BadIdentifier or ErrorCode.WRN_CLS_BadBase or ErrorCode.WRN_CLS_BadInterfaceMember or ErrorCode.WRN_CLS_NoAbstractMembers or ErrorCode.WRN_CLS_NotOnModules or ErrorCode.WRN_CLS_ModuleMissingCLS or ErrorCode.WRN_CLS_AssemblyNotCLS or ErrorCode.WRN_CLS_BadAttributeType or ErrorCode.WRN_CLS_ArrayArgumentToAttribute or ErrorCode.WRN_CLS_NotOnModules2 or ErrorCode.WRN_CLS_IllegalTrueInFalse or ErrorCode.WRN_CLS_MeaninglessOnParam or ErrorCode.WRN_CLS_MeaninglessOnReturn or ErrorCode.WRN_CLS_BadTypeVar or ErrorCode.WRN_CLS_VolatileField or ErrorCode.WRN_CLS_BadInterface or ErrorCode.WRN_UnobservedAwaitableExpression or ErrorCode.WRN_CallerLineNumberParamForUnconsumedLocation or ErrorCode.WRN_CallerFilePathParamForUnconsumedLocation or ErrorCode.WRN_CallerMemberNameParamForUnconsumedLocation or ErrorCode.WRN_CallerFilePathPreferredOverCallerMemberName or ErrorCode.WRN_CallerLineNumberPreferredOverCallerMemberName or ErrorCode.WRN_CallerLineNumberPreferredOverCallerFilePath or ErrorCode.WRN_DelaySignButNoKey or ErrorCode.WRN_UnimplementedCommandLineSwitch or ErrorCode.WRN_AsyncLacksAwaits or ErrorCode.WRN_BadUILang or ErrorCode.WRN_RefCultureMismatch or ErrorCode.WRN_ConflictingMachineAssembly or ErrorCode.WRN_FilterIsConstantTrue or ErrorCode.WRN_FilterIsConstantFalse or ErrorCode.WRN_FilterIsConstantFalseRedundantTryCatch or ErrorCode.WRN_IdentifierOrNumericLiteralExpected or ErrorCode.WRN_ReferencedAssemblyDoesNotHaveStrongName or ErrorCode.WRN_AlignmentMagnitude or ErrorCode.WRN_AttributeIgnoredWhenPublicSigning or ErrorCode.WRN_TupleLiteralNameMismatch or ErrorCode.WRN_Experimental or ErrorCode.WRN_AttributesOnBackingFieldsNotAvailable or ErrorCode.WRN_TupleBinopLiteralNameMismatch or ErrorCode.WRN_TypeParameterSameAsOuterMethodTypeParameter or ErrorCode.WRN_ConvertingNullableToNonNullable or ErrorCode.WRN_NullReferenceAssignment or ErrorCode.WRN_NullReferenceReceiver or ErrorCode.WRN_NullReferenceReturn or ErrorCode.WRN_NullReferenceArgument or ErrorCode.WRN_NullabilityMismatchInTypeOnOverride or ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride or ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial or ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride or ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial or ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation or ErrorCode.WRN_NullabilityMismatchInTypeOnImplicitImplementation or ErrorCode.WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation or ErrorCode.WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation or ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList or ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase or ErrorCode.WRN_NullabilityMismatchInExplicitlyImplementedInterface or ErrorCode.WRN_NullabilityMismatchInTypeOnExplicitImplementation or ErrorCode.WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation or ErrorCode.WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation or ErrorCode.WRN_UninitializedNonNullableField or ErrorCode.WRN_NullabilityMismatchInAssignment or ErrorCode.WRN_NullabilityMismatchInArgument or ErrorCode.WRN_NullabilityMismatchInArgumentForOutput or ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate or ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate or ErrorCode.WRN_NullAsNonNullable or ErrorCode.WRN_NullableValueTypeMayBeNull or ErrorCode.WRN_NullabilityMismatchInTypeParameterConstraint or ErrorCode.WRN_MissingNonNullTypesContextForAnnotation or ErrorCode.WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode or ErrorCode.WRN_NullabilityMismatchInConstraintsOnImplicitImplementation or ErrorCode.WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint or ErrorCode.WRN_SwitchExpressionNotExhaustive or ErrorCode.WRN_IsTypeNamedUnderscore or ErrorCode.WRN_GivenExpressionNeverMatchesPattern or ErrorCode.WRN_GivenExpressionAlwaysMatchesConstant or ErrorCode.WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue or ErrorCode.WRN_CaseConstantNamedUnderscore or ErrorCode.WRN_ThrowPossibleNull or ErrorCode.WRN_UnboxPossibleNull or ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull or ErrorCode.WRN_ImplicitCopyInReadOnlyMember or ErrorCode.WRN_UnconsumedEnumeratorCancellationAttributeUsage or ErrorCode.WRN_UndecoratedCancellationTokenParameter or ErrorCode.WRN_NullabilityMismatchInTypeParameterNotNullConstraint or ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment or ErrorCode.WRN_ParameterConditionallyDisallowsNull or ErrorCode.WRN_NullReferenceInitializer or ErrorCode.WRN_ShouldNotReturn or ErrorCode.WRN_DoesNotReturnMismatch or ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride or ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride or ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation or ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation or ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation or ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation or ErrorCode.WRN_ConstOutOfRangeChecked or ErrorCode.WRN_MemberNotNull or ErrorCode.WRN_MemberNotNullBadMember or ErrorCode.WRN_MemberNotNullWhen or ErrorCode.WRN_GeneratorFailedDuringInitialization or ErrorCode.WRN_GeneratorFailedDuringGeneration or ErrorCode.WRN_ParameterDisallowsNull or ErrorCode.WRN_GivenExpressionAlwaysMatchesPattern or ErrorCode.WRN_IsPatternAlways or ErrorCode.WRN_SwitchExpressionNotExhaustiveWithWhen or ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen or ErrorCode.WRN_RecordNamedDisallowed or ErrorCode.WRN_ParameterNotNullIfNotNull or ErrorCode.WRN_ReturnNotNullIfNotNull or ErrorCode.WRN_AnalyzerReferencesFramework or ErrorCode.WRN_UnreadRecordParameter or ErrorCode.WRN_DoNotCompareFunctionPointers => 1,
                _ => 0,
            };
            // Note: when adding a warning here, consider whether it should be registered as a nullability warning too
        }

        /// <summary>
        /// When converting an anonymous function to a delegate type, there are some diagnostics
        /// that will occur regardless of the delegate type - particularly those that do not
        /// depend on the substituted types (e.g. name uniqueness).  Even though we need to
        /// produce a diagnostic in such cases, we do not need to abandon overload resolution -
        /// we can choose the overload that is best without regard to such diagnostics.
        /// </summary>
        /// <returns>True if seeing the ErrorCode should prevent a delegate conversion
        /// from completing successfully.</returns>
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

            return code switch
            {
                ErrorCode.ERR_DuplicateParamName or ErrorCode.ERR_LocalDuplicate or ErrorCode.ERR_LocalIllegallyOverrides or ErrorCode.ERR_LocalSameNameAsTypeParam or ErrorCode.ERR_QueryRangeVariableOverrides or ErrorCode.ERR_QueryRangeVariableSameAsTypeParam or ErrorCode.ERR_DeprecatedCollectionInitAddStr or ErrorCode.ERR_DeprecatedSymbolStr => false,
                _ => true,
            };
        }

        /// <remarks>
        /// WARNING: will resolve lazy diagnostics - do not call this before the member lists are completed
        /// or you could trigger infinite recursion.
        /// </remarks>
        internal static bool PreventsSuccessfulDelegateConversion(DiagnosticBag diagnostics)
        {
            foreach (Diagnostic diag in diagnostics.AsEnumerable()) // Checking the code would have resolved them anyway.
            {
                if (ErrorFacts.PreventsSuccessfulDelegateConversion((ErrorCode)diag.Code))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool PreventsSuccessfulDelegateConversion(ImmutableArray<Diagnostic> diagnostics)
        {
            foreach (var diag in diagnostics)
            {
                if (ErrorFacts.PreventsSuccessfulDelegateConversion((ErrorCode)diag.Code))
                {
                    return true;
                }
            }

            return false;
        }

        internal static ErrorCode GetStaticClassParameterCode(bool useWarning)
            => useWarning ? ErrorCode.WRN_ParameterIsStaticClass : ErrorCode.ERR_ParameterIsStaticClass;

        internal static ErrorCode GetStaticClassReturnCode(bool useWarning)
            => useWarning ? ErrorCode.WRN_ReturnTypeIsStaticClass : ErrorCode.ERR_ReturnTypeIsStaticClass;
    }
}
