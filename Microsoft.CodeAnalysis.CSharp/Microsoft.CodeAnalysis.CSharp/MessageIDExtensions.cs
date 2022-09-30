using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class MessageIDExtensions
    {
        public static LocalizableErrorArgument Localize(this MessageID id)
        {
            return new LocalizableErrorArgument(id);
        }

        internal static string? RequiredFeature(this MessageID feature)
        {
            return null;
        }

        internal static bool CheckFeatureAvailability(this MessageID feature, BindingDiagnosticBag diagnostics, SyntaxNode syntax, Location? location = null)
        {
            CSDiagnosticInfo featureAvailabilityDiagnosticInfo = feature.GetFeatureAvailabilityDiagnosticInfo((CSharpParseOptions)syntax.SyntaxTree.Options);
            if (featureAvailabilityDiagnosticInfo != null)
            {
                diagnostics.Add(featureAvailabilityDiagnosticInfo, location ?? syntax.GetLocation());
                return false;
            }
            return true;
        }

        internal static bool CheckFeatureAvailability(this MessageID feature, BindingDiagnosticBag diagnostics, Compilation compilation, Location location)
        {
            CSDiagnosticInfo featureAvailabilityDiagnosticInfo = feature.GetFeatureAvailabilityDiagnosticInfo((CSharpCompilation)compilation);
            if (featureAvailabilityDiagnosticInfo != null)
            {
                diagnostics.Add(featureAvailabilityDiagnosticInfo, location);
                return false;
            }
            return true;
        }

        internal static CSDiagnosticInfo? GetFeatureAvailabilityDiagnosticInfo(this MessageID feature, CSharpParseOptions options)
        {
            if (!options.IsFeatureEnabled(feature))
            {
                return GetDisabledFeatureDiagnosticInfo(feature, options.LanguageVersion);
            }
            return null;
        }

        internal static CSDiagnosticInfo? GetFeatureAvailabilityDiagnosticInfo(this MessageID feature, CSharpCompilation compilation)
        {
            if (!compilation.IsFeatureEnabled(feature))
            {
                return GetDisabledFeatureDiagnosticInfo(feature, compilation.LanguageVersion);
            }
            return null;
        }

        private static CSDiagnosticInfo GetDisabledFeatureDiagnosticInfo(MessageID feature, LanguageVersion availableVersion)
        {
            string text = feature.RequiredFeature();
            if (text != null)
            {
                return new CSDiagnosticInfo(ErrorCode.ERR_FeatureIsExperimental, feature.Localize(), text);
            }
            LanguageVersion languageVersion = feature.RequiredVersion();
            if (languageVersion != LanguageVersion.Preview.MapSpecifiedToEffectiveVersion())
            {
                return new CSDiagnosticInfo(availableVersion.GetErrorCode(), feature.Localize(), new CSharpRequiredLanguageVersion(languageVersion));
            }
            return new CSDiagnosticInfo(ErrorCode.ERR_FeatureInPreview, feature.Localize());
        }

        internal static LanguageVersion RequiredVersion(this MessageID feature)
        {
            switch (feature)
            {
                case MessageID.IDS_FeatureMixedDeclarationsAndExpressionsInDeconstruction:
                case MessageID.IDS_FeatureSealedToStringInRecord:
                case MessageID.IDS_FeatureRecordStructs:
                case MessageID.IDS_FeatureWithOnStructs:
                case MessageID.IDS_FeaturePositionalFieldsInRecords:
                case MessageID.IDS_FeatureGlobalUsing:
                case MessageID.IDS_FeatureInferredDelegateType:
                case MessageID.IDS_FeatureLambdaAttributes:
                    return LanguageVersion.Preview;
                case MessageID.IDS_FeatureStaticAnonymousFunction:
                case MessageID.IDS_FeatureDefaultTypeParameterConstraint:
                case MessageID.IDS_FeatureLambdaDiscardParameters:
                case MessageID.IDS_FeatureLocalFunctionAttributes:
                case MessageID.IDS_FeatureExternLocalFunctions:
                case MessageID.IDS_FeatureMemberNotNull:
                case MessageID.IDS_FeatureNativeInt:
                case MessageID.IDS_FeatureImplicitObjectCreation:
                case MessageID.IDS_FeatureTypePattern:
                case MessageID.IDS_FeatureParenthesizedPattern:
                case MessageID.IDS_FeatureOrPattern:
                case MessageID.IDS_FeatureAndPattern:
                case MessageID.IDS_FeatureNotPattern:
                case MessageID.IDS_FeatureRelationalPattern:
                case MessageID.IDS_FeatureExtendedPartialMethods:
                case MessageID.IDS_TopLevelStatements:
                case MessageID.IDS_FeatureFunctionPointers:
                case MessageID.IDS_FeatureInitOnlySetters:
                case MessageID.IDS_FeatureRecords:
                case MessageID.IDS_FeatureModuleInitializers:
                case MessageID.IDS_FeatureTargetTypedConditional:
                case MessageID.IDS_FeatureCovariantReturnsForOverrides:
                case MessageID.IDS_FeatureExtensionGetEnumerator:
                case MessageID.IDS_FeatureExtensionGetAsyncEnumerator:
                    return LanguageVersion.CSharp9;
                case MessageID.IDS_FeatureVarianceSafetyForStaticInterfaceMembers:
                case MessageID.IDS_FeatureConstantInterpolatedStrings:
                    return LanguageVersion.Preview;
                case MessageID.IDS_FeatureNullableReferenceTypes:
                case MessageID.IDS_FeatureAltInterpolatedVerbatimStrings:
                case MessageID.IDS_FeatureCoalesceAssignmentExpression:
                case MessageID.IDS_FeatureUnconstrainedTypeParameterInNullCoalescingOperator:
                case MessageID.IDS_FeatureNotNullGenericTypeConstraint:
                case MessageID.IDS_FeatureIndexOperator:
                case MessageID.IDS_FeatureRangeOperator:
                case MessageID.IDS_FeatureAsyncStreams:
                case MessageID.IDS_FeatureRecursivePatterns:
                case MessageID.IDS_FeatureUsingDeclarations:
                case MessageID.IDS_FeatureStaticLocalFunctions:
                case MessageID.IDS_FeatureNameShadowingInNestedFunctions:
                case MessageID.IDS_FeatureUnmanagedConstructedTypes:
                case MessageID.IDS_FeatureObsoleteOnPropertyAccessor:
                case MessageID.IDS_FeatureReadOnlyMembers:
                case MessageID.IDS_DefaultInterfaceImplementation:
                case MessageID.IDS_OverrideWithConstraints:
                case MessageID.IDS_FeatureNestedStackalloc:
                case MessageID.IDS_FeatureSwitchExpression:
                case MessageID.IDS_FeatureAsyncUsing:
                case MessageID.IDS_FeatureNullPointerConstantPattern:
                    return LanguageVersion.CSharp8;
                case MessageID.IDS_FeatureAttributesOnBackingFields:
                case MessageID.IDS_FeatureImprovedOverloadCandidates:
                case MessageID.IDS_FeatureRefReassignment:
                case MessageID.IDS_FeatureRefFor:
                case MessageID.IDS_FeatureRefForEach:
                case MessageID.IDS_FeatureEnumGenericTypeConstraint:
                case MessageID.IDS_FeatureDelegateGenericTypeConstraint:
                case MessageID.IDS_FeatureUnmanagedGenericTypeConstraint:
                case MessageID.IDS_FeatureStackAllocInitializer:
                case MessageID.IDS_FeatureTupleEquality:
                case MessageID.IDS_FeatureExpressionVariablesInQueriesAndInitializers:
                case MessageID.IDS_FeatureExtensibleFixedStatement:
                case MessageID.IDS_FeatureIndexingMovableFixedBuffers:
                    return LanguageVersion.CSharp7_3;
                case MessageID.IDS_FeatureLeadingDigitSeparator:
                case MessageID.IDS_FeatureNonTrailingNamedArguments:
                case MessageID.IDS_FeatureReadOnlyReferences:
                case MessageID.IDS_FeatureRefStructs:
                case MessageID.IDS_FeatureReadOnlyStructs:
                case MessageID.IDS_FeatureRefExtensionMethods:
                case MessageID.IDS_FeaturePrivateProtected:
                case MessageID.IDS_FeatureRefConditional:
                    return LanguageVersion.CSharp7_2;
                case MessageID.IDS_FeatureDefaultLiteral:
                case MessageID.IDS_FeatureInferredTupleNames:
                case MessageID.IDS_FeatureGenericPatternMatching:
                case MessageID.IDS_FeatureAsyncMain:
                    return LanguageVersion.CSharp7_1;
                case MessageID.IDS_FeaturePatternMatching:
                case MessageID.IDS_FeatureThrowExpression:
                case MessageID.IDS_FeatureDiscards:
                case MessageID.IDS_FeatureBinaryLiteral:
                case MessageID.IDS_FeatureDigitSeparator:
                case MessageID.IDS_FeatureLocalFunctions:
                case MessageID.IDS_FeatureRefLocalsReturns:
                case MessageID.IDS_FeatureTuples:
                case MessageID.IDS_FeatureOutVar:
                case MessageID.IDS_FeatureExpressionBodiedAccessor:
                case MessageID.IDS_FeatureExpressionBodiedDeOrConstructor:
                    return LanguageVersion.CSharp7;
                case MessageID.IDS_FeatureExceptionFilter:
                case MessageID.IDS_FeatureAutoPropertyInitializer:
                case MessageID.IDS_FeatureNullPropagatingOperator:
                case MessageID.IDS_FeatureExpressionBodiedMethod:
                case MessageID.IDS_FeatureExpressionBodiedProperty:
                case MessageID.IDS_FeatureExpressionBodiedIndexer:
                case MessageID.IDS_FeatureNameof:
                case MessageID.IDS_FeatureDictionaryInitializer:
                case MessageID.IDS_FeatureUsingStatic:
                case MessageID.IDS_FeatureInterpolatedStrings:
                case MessageID.IDS_AwaitInCatchAndFinally:
                case MessageID.IDS_FeatureReadonlyAutoImplementedProperties:
                    return LanguageVersion.CSharp6;
                case MessageID.IDS_FeatureAsync:
                    return LanguageVersion.CSharp5;
                case MessageID.IDS_FeatureDynamic:
                case MessageID.IDS_FeatureTypeVariance:
                case MessageID.IDS_FeatureNamedArgument:
                case MessageID.IDS_FeatureOptionalParameter:
                    return LanguageVersion.CSharp4;
                case MessageID.IDS_FeatureImplicitArray:
                case MessageID.IDS_FeatureImplicitLocal:
                case MessageID.IDS_FeatureAnonymousTypes:
                case MessageID.IDS_FeatureAutoImplementedProperties:
                case MessageID.IDS_FeatureObjectInitializer:
                case MessageID.IDS_FeatureCollectionInitializer:
                case MessageID.IDS_FeatureLambda:
                case MessageID.IDS_FeatureQueryExpression:
                case MessageID.IDS_FeatureExtensionMethod:
                case MessageID.IDS_FeaturePartialMethod:
                    return LanguageVersion.CSharp3;
                case MessageID.IDS_FeatureGenerics:
                case MessageID.IDS_FeatureAnonDelegates:
                case MessageID.IDS_FeatureGlobalNamespace:
                case MessageID.IDS_FeatureFixedBuffer:
                case MessageID.IDS_FeaturePragma:
                case MessageID.IDS_FeatureStaticClasses:
                case MessageID.IDS_FeaturePartialTypes:
                case MessageID.IDS_FeatureSwitchOnBool:
                case MessageID.IDS_FeaturePropertyAccessorMods:
                case MessageID.IDS_FeatureExternAlias:
                case MessageID.IDS_FeatureIterators:
                case MessageID.IDS_FeatureDefault:
                case MessageID.IDS_FeatureNullable:
                    return LanguageVersion.CSharp2;
                case MessageID.IDS_FeatureModuleAttrLoc:
                    return LanguageVersion.CSharp1;
                default:
                    throw ExceptionUtilities.UnexpectedValue(feature);
            }
        }
    }
}
