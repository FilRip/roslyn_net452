using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class ErrorFacts
	{
		public static bool IsWarning(ERRID code)
		{
			if (code <= ERRID.WRN_UnimplementedCommandLineSwitch)
			{
				switch (code)
				{
				case ERRID.WRN_BadSwitch:
				case ERRID.WRN_NoConfigInResponseFile:
				case ERRID.WRN_IgnoreModuleManifest:
				case ERRID.WRN_BadUILang:
				case ERRID.WRN_UseOfObsoleteSymbol2:
				case ERRID.WRN_InvalidOverrideDueToTupleNames2:
				case ERRID.WRN_MustOverloadBase4:
				case ERRID.WRN_OverrideType5:
				case ERRID.WRN_MustOverride2:
				case ERRID.WRN_DefaultnessShadowed4:
				case ERRID.WRN_UseOfObsoleteSymbolNoMessage1:
				case ERRID.WRN_AssemblyGeneration0:
				case ERRID.WRN_AssemblyGeneration1:
				case ERRID.WRN_ComClassNoMembers1:
				case ERRID.WRN_SynthMemberShadowsMember5:
				case ERRID.WRN_MemberShadowsSynthMember6:
				case ERRID.WRN_SynthMemberShadowsSynthMember7:
				case ERRID.WRN_UseOfObsoletePropertyAccessor3:
				case ERRID.WRN_UseOfObsoletePropertyAccessor2:
				case ERRID.WRN_FieldNotCLSCompliant1:
				case ERRID.WRN_BaseClassNotCLSCompliant2:
				case ERRID.WRN_ProcTypeNotCLSCompliant1:
				case ERRID.WRN_ParamNotCLSCompliant1:
				case ERRID.WRN_InheritedInterfaceNotCLSCompliant2:
				case ERRID.WRN_CLSMemberInNonCLSType3:
				case ERRID.WRN_NameNotCLSCompliant1:
				case ERRID.WRN_EnumUnderlyingTypeNotCLS1:
				case ERRID.WRN_NonCLSMemberInCLSInterface1:
				case ERRID.WRN_NonCLSMustOverrideInCLSType1:
				case ERRID.WRN_ArrayOverloadsNonCLS2:
				case ERRID.WRN_RootNamespaceNotCLSCompliant1:
				case ERRID.WRN_RootNamespaceNotCLSCompliant2:
				case ERRID.WRN_GenericConstraintNotCLSCompliant1:
				case ERRID.WRN_TypeNotCLSCompliant1:
				case ERRID.WRN_OptionalValueNotCLSCompliant1:
				case ERRID.WRN_CLSAttrInvalidOnGetSet:
				case ERRID.WRN_TypeConflictButMerged6:
				case ERRID.WRN_ShadowingGenericParamWithParam1:
				case ERRID.WRN_CannotFindStandardLibrary1:
				case ERRID.WRN_EventDelegateTypeNotCLSCompliant2:
				case ERRID.WRN_DebuggerHiddenIgnoredOnProperties:
				case ERRID.WRN_SelectCaseInvalidRange:
				case ERRID.WRN_CLSEventMethodInNonCLSType3:
				case ERRID.WRN_ExpectedInitComponentCall2:
				case ERRID.WRN_NamespaceCaseMismatch3:
				case ERRID.WRN_UndefinedOrEmptyNamespaceOrClass1:
				case ERRID.WRN_UndefinedOrEmptyProjectNamespaceOrClass1:
				case ERRID.WRN_IndirectRefToLinkedAssembly2:
				case ERRID.WRN_DelaySignButNoKey:
				case ERRID.WRN_UnimplementedCommandLineSwitch:
					break;
				default:
					goto IL_03d4;
				}
			}
			else if (code <= ERRID.WRN_StaticLocalNoInference)
			{
				switch (code)
				{
				case ERRID.WRN_NoNonObsoleteConstructorOnBase3:
				case ERRID.WRN_NoNonObsoleteConstructorOnBase4:
				case ERRID.WRN_RequiredNonObsoleteNewCall3:
				case ERRID.WRN_RequiredNonObsoleteNewCall4:
				case ERRID.WRN_MissingAsClauseinOperator:
				case ERRID.WRN_ConstraintsFailedForInferredArgs2:
				case ERRID.WRN_ConditionalNotValidOnFunction:
				case ERRID.WRN_UseSwitchInsteadOfAttribute:
				case ERRID.WRN_TupleLiteralNameMismatch:
				case ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName:
				case ERRID.WRN_RecursiveAddHandlerCall:
				case ERRID.WRN_ImplicitConversionCopyBack:
				case ERRID.WRN_MustShadowOnMultipleInheritance2:
				case ERRID.WRN_RecursiveOperatorCall:
				case ERRID.WRN_ImplicitConversionSubst1:
				case ERRID.WRN_LateBindingResolution:
				case ERRID.WRN_ObjectMath1:
				case ERRID.WRN_ObjectMath2:
				case ERRID.WRN_ObjectAssumedVar1:
				case ERRID.WRN_ObjectAssumed1:
				case ERRID.WRN_ObjectAssumedProperty1:
				case ERRID.WRN_UnusedLocal:
				case ERRID.WRN_SharedMemberThroughInstance:
				case ERRID.WRN_RecursivePropertyCall:
				case ERRID.WRN_OverlappingCatch:
				case ERRID.WRN_DefAsgUseNullRefByRef:
				case ERRID.WRN_DuplicateCatch:
				case ERRID.WRN_ObjectMath1Not:
				case ERRID.WRN_BadChecksumValExtChecksum:
				case ERRID.WRN_MultipleDeclFileExtChecksum:
				case ERRID.WRN_BadGUIDFormatExtChecksum:
				case ERRID.WRN_ObjectMathSelectCase:
				case ERRID.WRN_EqualToLiteralNothing:
				case ERRID.WRN_NotEqualToLiteralNothing:
				case ERRID.WRN_UnusedLocalConst:
				case ERRID.WRN_ComClassInterfaceShadows5:
				case ERRID.WRN_ComClassPropertySetObject1:
				case ERRID.WRN_DefAsgUseNullRef:
				case ERRID.WRN_DefAsgNoRetValFuncRef1:
				case ERRID.WRN_DefAsgNoRetValOpRef1:
				case ERRID.WRN_DefAsgNoRetValPropRef1:
				case ERRID.WRN_DefAsgUseNullRefByRefStr:
				case ERRID.WRN_DefAsgUseNullRefStr:
				case ERRID.WRN_StaticLocalNoInference:
					break;
				default:
					goto IL_03d4;
				}
			}
			else
			{
				switch (code)
				{
				case ERRID.WRN_InvalidAssemblyName:
				case ERRID.WRN_XMLDocBadXMLLine:
				case ERRID.WRN_XMLDocMoreThanOneCommentBlock:
				case ERRID.WRN_XMLDocNotFirstOnLine:
				case ERRID.WRN_XMLDocInsideMethod:
				case ERRID.WRN_XMLDocParseError1:
				case ERRID.WRN_XMLDocDuplicateXMLNode1:
				case ERRID.WRN_XMLDocIllegalTagOnElement2:
				case ERRID.WRN_XMLDocBadParamTag2:
				case ERRID.WRN_XMLDocParamTagWithoutName:
				case ERRID.WRN_XMLDocCrefAttributeNotFound1:
				case ERRID.WRN_XMLMissingFileOrPathAttribute1:
				case ERRID.WRN_XMLCannotWriteToXMLDocFile2:
				case ERRID.WRN_XMLDocWithoutLanguageElement:
				case ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty:
				case ERRID.WRN_XMLDocOnAPartialType:
				case ERRID.WRN_XMLDocReturnsOnADeclareSub:
				case ERRID.WRN_XMLDocStartTagWithNoEndTag:
				case ERRID.WRN_XMLDocBadGenericParamTag2:
				case ERRID.WRN_XMLDocGenericParamTagWithoutName:
				case ERRID.WRN_XMLDocExceptionTagWithoutCRef:
				case ERRID.WRN_XMLDocInvalidXMLFragment:
				case ERRID.WRN_XMLDocBadFormedXML:
				case ERRID.WRN_InterfaceConversion2:
				case ERRID.WRN_LiftControlVariableLambda:
				case ERRID.WRN_LambdaPassedToRemoveHandler:
				case ERRID.WRN_LiftControlVariableQuery:
				case ERRID.WRN_RelDelegatePassedToRemoveHandler:
				case ERRID.WRN_AmbiguousCastConversion2:
				case ERRID.WRN_VarianceDeclarationAmbiguous3:
				case ERRID.WRN_ArrayInitNoTypeObjectAssumed:
				case ERRID.WRN_TypeInferenceAssumed3:
				case ERRID.WRN_VarianceConversionFailedOut6:
				case ERRID.WRN_VarianceConversionFailedIn6:
				case ERRID.WRN_VarianceIEnumerableSuggestion3:
				case ERRID.WRN_VarianceConversionFailedTryOut4:
				case ERRID.WRN_VarianceConversionFailedTryIn4:
				case ERRID.WRN_IfNoTypeObjectAssumed:
				case ERRID.WRN_IfTooManyTypesObjectAssumed:
				case ERRID.WRN_ArrayInitTooManyTypesObjectAssumed:
				case ERRID.WRN_LambdaNoTypeObjectAssumed:
				case ERRID.WRN_LambdaTooManyTypesObjectAssumed:
				case ERRID.WRN_MissingAsClauseinVarDecl:
				case ERRID.WRN_MissingAsClauseinFunction:
				case ERRID.WRN_MissingAsClauseinProperty:
				case ERRID.WRN_ObsoleteIdentityDirectCastForValueType:
				case ERRID.WRN_ImplicitConversion2:
				case ERRID.WRN_MutableStructureInUsing:
				case ERRID.WRN_MutableGenericStructureInUsing:
				case ERRID.WRN_DefAsgNoRetValFuncVal1:
				case ERRID.WRN_DefAsgNoRetValOpVal1:
				case ERRID.WRN_DefAsgNoRetValPropVal1:
				case ERRID.WRN_AsyncLacksAwaits:
				case ERRID.WRN_AsyncSubCouldBeFunction:
				case ERRID.WRN_UnobservedAwaitableExpression:
				case ERRID.WRN_UnobservedAwaitableDelegate:
				case ERRID.WRN_PrefixAndXmlnsLocalName:
				case ERRID.WRN_UseValueForXmlExpression3:
				case ERRID.WRN_ReturnTypeAttributeOnWriteOnlyProperty:
				case ERRID.WRN_InvalidVersionFormat:
				case ERRID.WRN_MainIgnored:
				case ERRID.WRN_EmptyPrefixAndXmlnsLocalName:
				case ERRID.WRN_DefAsgNoRetValWinRtEventVal1:
				case ERRID.WRN_AssemblyAttributeFromModuleIsOverridden:
				case ERRID.WRN_RefCultureMismatch:
				case ERRID.WRN_ConflictingMachineAssembly:
				case ERRID.WRN_PdbLocalNameTooLong:
				case ERRID.WRN_PdbUsingNameTooLong:
				case ERRID.WRN_XMLDocCrefToTypeParameter:
				case ERRID.WRN_AnalyzerCannotBeCreated:
				case ERRID.WRN_NoAnalyzerInAssembly:
				case ERRID.WRN_UnableToLoadAnalyzer:
				case ERRID.WRN_AttributeIgnoredWhenPublicSigning:
				case ERRID.WRN_Experimental:
				case ERRID.WRN_AttributeNotSupportedInVB:
				case ERRID.WRN_GeneratorFailedDuringInitialization:
				case ERRID.WRN_GeneratorFailedDuringGeneration:
				case ERRID.WRN_AnalyzerReferencesFramework:
					break;
				default:
					goto IL_03d4;
				}
			}
			return true;
			IL_03d4:
			return false;
		}

		public static bool IsFatal(ERRID code)
		{
			if (code == ERRID.FTL_InvalidInputFileName)
			{
				return true;
			}
			return false;
		}

		public static bool IsInfo(ERRID code)
		{
			if (code == ERRID.INF_UnableToLoadSomeTypesInAnalyzer)
			{
				return true;
			}
			return false;
		}

		public static bool IsHidden(ERRID code)
		{
			if ((uint)(code - 50000) <= 1u)
			{
				return true;
			}
			return false;
		}
	}
}
