using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VBResources
	{
		private static ResourceManager s_resourceManager;

		internal static ResourceManager ResourceManager
		{
			get
			{
				if (s_resourceManager == null)
				{
					s_resourceManager = new ResourceManager(typeof(VBResources));
				}
				return s_resourceManager;
			}
		}

		internal static CultureInfo Culture { get; set; }

		internal static string Trees0 => GetResourceString("Trees0");

		internal static string TreesMustHaveRootNode => GetResourceString("TreesMustHaveRootNode");

		internal static string CannotAddCompilerSpecialTree => GetResourceString("CannotAddCompilerSpecialTree");

		internal static string SyntaxTreeAlreadyPresent => GetResourceString("SyntaxTreeAlreadyPresent");

		internal static string SubmissionCanHaveAtMostOneSyntaxTree => GetResourceString("SubmissionCanHaveAtMostOneSyntaxTree");

		internal static string CannotRemoveCompilerSpecialTree => GetResourceString("CannotRemoveCompilerSpecialTree");

		internal static string SyntaxTreeNotFoundToRemove => GetResourceString("SyntaxTreeNotFoundToRemove");

		internal static string TreeMustHaveARootNodeWithCompilationUnit => GetResourceString("TreeMustHaveARootNodeWithCompilationUnit");

		internal static string CompilationVisualBasic => GetResourceString("CompilationVisualBasic");

		internal static string NodeIsNotWithinSyntaxTree => GetResourceString("NodeIsNotWithinSyntaxTree");

		internal static string CantReferenceCompilationFromTypes => GetResourceString("CantReferenceCompilationFromTypes");

		internal static string PositionOfTypeParameterTooLarge => GetResourceString("PositionOfTypeParameterTooLarge");

		internal static string AssociatedTypeDoesNotHaveTypeParameters => GetResourceString("AssociatedTypeDoesNotHaveTypeParameters");

		internal static string IDS_FunctionReturnType => GetResourceString("IDS_FunctionReturnType");

		internal static string TypeArgumentCannotBeNothing => GetResourceString("TypeArgumentCannotBeNothing");

		internal static string WrongNumberOfTypeArguments => GetResourceString("WrongNumberOfTypeArguments");

		internal static string ERR_FileNotFound => GetResourceString("ERR_FileNotFound");

		internal static string ERR_NoResponseFile => GetResourceString("ERR_NoResponseFile");

		internal static string ERR_ArgumentRequired => GetResourceString("ERR_ArgumentRequired");

		internal static string ERR_SwitchNeedsBool => GetResourceString("ERR_SwitchNeedsBool");

		internal static string ERR_InvalidSwitchValue => GetResourceString("ERR_InvalidSwitchValue");

		internal static string ERR_MutuallyExclusiveOptions => GetResourceString("ERR_MutuallyExclusiveOptions");

		internal static string WRN_BadUILang => GetResourceString("WRN_BadUILang");

		internal static string WRN_BadUILang_Title => GetResourceString("WRN_BadUILang_Title");

		internal static string ERR_VBCoreNetModuleConflict => GetResourceString("ERR_VBCoreNetModuleConflict");

		internal static string ERR_InvalidFormatForGuidForOption => GetResourceString("ERR_InvalidFormatForGuidForOption");

		internal static string ERR_MissingGuidForOption => GetResourceString("ERR_MissingGuidForOption");

		internal static string ERR_BadChecksumAlgorithm => GetResourceString("ERR_BadChecksumAlgorithm");

		internal static string WRN_BadSwitch => GetResourceString("WRN_BadSwitch");

		internal static string WRN_BadSwitch_Title => GetResourceString("WRN_BadSwitch_Title");

		internal static string ERR_NoSources => GetResourceString("ERR_NoSources");

		internal static string WRN_FileAlreadyIncluded => GetResourceString("WRN_FileAlreadyIncluded");

		internal static string WRN_FileAlreadyIncluded_Title => GetResourceString("WRN_FileAlreadyIncluded_Title");

		internal static string ERR_CantOpenFileWrite => GetResourceString("ERR_CantOpenFileWrite");

		internal static string ERR_BadCodepage => GetResourceString("ERR_BadCodepage");

		internal static string ERR_BinaryFile => GetResourceString("ERR_BinaryFile");

		internal static string ERR_LibNotFound => GetResourceString("ERR_LibNotFound");

		internal static string ERR_MetadataReferencesNotSupported => GetResourceString("ERR_MetadataReferencesNotSupported");

		internal static string ERR_IconFileAndWin32ResFile => GetResourceString("ERR_IconFileAndWin32ResFile");

		internal static string WRN_NoConfigInResponseFile => GetResourceString("WRN_NoConfigInResponseFile");

		internal static string WRN_NoConfigInResponseFile_Title => GetResourceString("WRN_NoConfigInResponseFile_Title");

		internal static string WRN_InvalidWarningId => GetResourceString("WRN_InvalidWarningId");

		internal static string WRN_InvalidWarningId_Title => GetResourceString("WRN_InvalidWarningId_Title");

		internal static string ERR_NoSourcesOut => GetResourceString("ERR_NoSourcesOut");

		internal static string ERR_NeedModule => GetResourceString("ERR_NeedModule");

		internal static string ERR_InvalidAssemblyName => GetResourceString("ERR_InvalidAssemblyName");

		internal static string ERR_ConflictingManifestSwitches => GetResourceString("ERR_ConflictingManifestSwitches");

		internal static string WRN_IgnoreModuleManifest => GetResourceString("WRN_IgnoreModuleManifest");

		internal static string WRN_IgnoreModuleManifest_Title => GetResourceString("WRN_IgnoreModuleManifest_Title");

		internal static string FTL_InvalidInputFileName => GetResourceString("FTL_InvalidInputFileName");

		internal static string ERR_InvalidInNamespace => GetResourceString("ERR_InvalidInNamespace");

		internal static string ERR_UndefinedType1 => GetResourceString("ERR_UndefinedType1");

		internal static string ERR_MissingNext => GetResourceString("ERR_MissingNext");

		internal static string ERR_IllegalCharConstant => GetResourceString("ERR_IllegalCharConstant");

		internal static string ERR_UnreferencedAssemblyEvent3 => GetResourceString("ERR_UnreferencedAssemblyEvent3");

		internal static string ERR_UnreferencedModuleEvent3 => GetResourceString("ERR_UnreferencedModuleEvent3");

		internal static string ERR_LbExpectedEndIf => GetResourceString("ERR_LbExpectedEndIf");

		internal static string ERR_LbNoMatchingIf => GetResourceString("ERR_LbNoMatchingIf");

		internal static string ERR_LbBadElseif => GetResourceString("ERR_LbBadElseif");

		internal static string ERR_InheritsFromRestrictedType1 => GetResourceString("ERR_InheritsFromRestrictedType1");

		internal static string ERR_InvOutsideProc => GetResourceString("ERR_InvOutsideProc");

		internal static string ERR_DelegateCantImplement => GetResourceString("ERR_DelegateCantImplement");

		internal static string ERR_DelegateCantHandleEvents => GetResourceString("ERR_DelegateCantHandleEvents");

		internal static string ERR_IsOperatorRequiresReferenceTypes1 => GetResourceString("ERR_IsOperatorRequiresReferenceTypes1");

		internal static string ERR_TypeOfRequiresReferenceType1 => GetResourceString("ERR_TypeOfRequiresReferenceType1");

		internal static string ERR_ReadOnlyHasSet => GetResourceString("ERR_ReadOnlyHasSet");

		internal static string ERR_WriteOnlyHasGet => GetResourceString("ERR_WriteOnlyHasGet");

		internal static string ERR_InvInsideProc => GetResourceString("ERR_InvInsideProc");

		internal static string ERR_InvInsideBlock => GetResourceString("ERR_InvInsideBlock");

		internal static string ERR_UnexpectedExpressionStatement => GetResourceString("ERR_UnexpectedExpressionStatement");

		internal static string ERR_EndProp => GetResourceString("ERR_EndProp");

		internal static string ERR_EndSubExpected => GetResourceString("ERR_EndSubExpected");

		internal static string ERR_EndFunctionExpected => GetResourceString("ERR_EndFunctionExpected");

		internal static string ERR_LbElseNoMatchingIf => GetResourceString("ERR_LbElseNoMatchingIf");

		internal static string ERR_CantRaiseBaseEvent => GetResourceString("ERR_CantRaiseBaseEvent");

		internal static string ERR_TryWithoutCatchOrFinally => GetResourceString("ERR_TryWithoutCatchOrFinally");

		internal static string ERR_EventsCantBeFunctions => GetResourceString("ERR_EventsCantBeFunctions");

		internal static string ERR_MissingEndBrack => GetResourceString("ERR_MissingEndBrack");

		internal static string ERR_Syntax => GetResourceString("ERR_Syntax");

		internal static string ERR_Overflow => GetResourceString("ERR_Overflow");

		internal static string ERR_IllegalChar => GetResourceString("ERR_IllegalChar");

		internal static string ERR_StrictDisallowsObjectOperand1 => GetResourceString("ERR_StrictDisallowsObjectOperand1");

		internal static string ERR_LoopControlMustNotBeProperty => GetResourceString("ERR_LoopControlMustNotBeProperty");

		internal static string ERR_MethodBodyNotAtLineStart => GetResourceString("ERR_MethodBodyNotAtLineStart");

		internal static string ERR_MaximumNumberOfErrors => GetResourceString("ERR_MaximumNumberOfErrors");

		internal static string ERR_UseOfKeywordNotInInstanceMethod1 => GetResourceString("ERR_UseOfKeywordNotInInstanceMethod1");

		internal static string ERR_UseOfKeywordFromStructure1 => GetResourceString("ERR_UseOfKeywordFromStructure1");

		internal static string ERR_BadAttributeConstructor1 => GetResourceString("ERR_BadAttributeConstructor1");

		internal static string ERR_ParamArrayWithOptArgs => GetResourceString("ERR_ParamArrayWithOptArgs");

		internal static string ERR_ExpectedArray1 => GetResourceString("ERR_ExpectedArray1");

		internal static string ERR_ParamArrayNotArray => GetResourceString("ERR_ParamArrayNotArray");

		internal static string ERR_ParamArrayRank => GetResourceString("ERR_ParamArrayRank");

		internal static string ERR_ArrayRankLimit => GetResourceString("ERR_ArrayRankLimit");

		internal static string ERR_AsNewArray => GetResourceString("ERR_AsNewArray");

		internal static string ERR_TooManyArgs1 => GetResourceString("ERR_TooManyArgs1");

		internal static string ERR_ExpectedCase => GetResourceString("ERR_ExpectedCase");

		internal static string ERR_RequiredConstExpr => GetResourceString("ERR_RequiredConstExpr");

		internal static string ERR_RequiredConstConversion2 => GetResourceString("ERR_RequiredConstConversion2");

		internal static string ERR_InvalidMe => GetResourceString("ERR_InvalidMe");

		internal static string ERR_ReadOnlyAssignment => GetResourceString("ERR_ReadOnlyAssignment");

		internal static string ERR_ExitSubOfFunc => GetResourceString("ERR_ExitSubOfFunc");

		internal static string ERR_ExitPropNot => GetResourceString("ERR_ExitPropNot");

		internal static string ERR_ExitFuncOfSub => GetResourceString("ERR_ExitFuncOfSub");

		internal static string ERR_LValueRequired => GetResourceString("ERR_LValueRequired");

		internal static string ERR_ForIndexInUse1 => GetResourceString("ERR_ForIndexInUse1");

		internal static string ERR_NextForMismatch1 => GetResourceString("ERR_NextForMismatch1");

		internal static string ERR_CaseElseNoSelect => GetResourceString("ERR_CaseElseNoSelect");

		internal static string ERR_CaseNoSelect => GetResourceString("ERR_CaseNoSelect");

		internal static string ERR_CantAssignToConst => GetResourceString("ERR_CantAssignToConst");

		internal static string ERR_NamedSubscript => GetResourceString("ERR_NamedSubscript");

		internal static string ERR_ExpectedEndIf => GetResourceString("ERR_ExpectedEndIf");

		internal static string ERR_ExpectedEndWhile => GetResourceString("ERR_ExpectedEndWhile");

		internal static string ERR_ExpectedLoop => GetResourceString("ERR_ExpectedLoop");

		internal static string ERR_ExpectedNext => GetResourceString("ERR_ExpectedNext");

		internal static string ERR_ExpectedEndWith => GetResourceString("ERR_ExpectedEndWith");

		internal static string ERR_ElseNoMatchingIf => GetResourceString("ERR_ElseNoMatchingIf");

		internal static string ERR_EndIfNoMatchingIf => GetResourceString("ERR_EndIfNoMatchingIf");

		internal static string ERR_EndSelectNoSelect => GetResourceString("ERR_EndSelectNoSelect");

		internal static string ERR_ExitDoNotWithinDo => GetResourceString("ERR_ExitDoNotWithinDo");

		internal static string ERR_EndWhileNoWhile => GetResourceString("ERR_EndWhileNoWhile");

		internal static string ERR_LoopNoMatchingDo => GetResourceString("ERR_LoopNoMatchingDo");

		internal static string ERR_NextNoMatchingFor => GetResourceString("ERR_NextNoMatchingFor");

		internal static string ERR_EndWithWithoutWith => GetResourceString("ERR_EndWithWithoutWith");

		internal static string ERR_MultiplyDefined1 => GetResourceString("ERR_MultiplyDefined1");

		internal static string ERR_ExpectedEndSelect => GetResourceString("ERR_ExpectedEndSelect");

		internal static string ERR_ExitForNotWithinFor => GetResourceString("ERR_ExitForNotWithinFor");

		internal static string ERR_ExitWhileNotWithinWhile => GetResourceString("ERR_ExitWhileNotWithinWhile");

		internal static string ERR_ReadOnlyProperty1 => GetResourceString("ERR_ReadOnlyProperty1");

		internal static string ERR_ExitSelectNotWithinSelect => GetResourceString("ERR_ExitSelectNotWithinSelect");

		internal static string ERR_BranchOutOfFinally => GetResourceString("ERR_BranchOutOfFinally");

		internal static string ERR_QualNotObjectRecord1 => GetResourceString("ERR_QualNotObjectRecord1");

		internal static string ERR_TooFewIndices => GetResourceString("ERR_TooFewIndices");

		internal static string ERR_TooManyIndices => GetResourceString("ERR_TooManyIndices");

		internal static string ERR_EnumNotExpression1 => GetResourceString("ERR_EnumNotExpression1");

		internal static string ERR_TypeNotExpression1 => GetResourceString("ERR_TypeNotExpression1");

		internal static string ERR_ClassNotExpression1 => GetResourceString("ERR_ClassNotExpression1");

		internal static string ERR_StructureNotExpression1 => GetResourceString("ERR_StructureNotExpression1");

		internal static string ERR_InterfaceNotExpression1 => GetResourceString("ERR_InterfaceNotExpression1");

		internal static string ERR_NamespaceNotExpression1 => GetResourceString("ERR_NamespaceNotExpression1");

		internal static string ERR_BadNamespaceName1 => GetResourceString("ERR_BadNamespaceName1");

		internal static string ERR_XmlPrefixNotExpression => GetResourceString("ERR_XmlPrefixNotExpression");

		internal static string ERR_MultipleExtends => GetResourceString("ERR_MultipleExtends");

		internal static string ERR_PropMustHaveGetSet => GetResourceString("ERR_PropMustHaveGetSet");

		internal static string ERR_WriteOnlyHasNoWrite => GetResourceString("ERR_WriteOnlyHasNoWrite");

		internal static string ERR_ReadOnlyHasNoGet => GetResourceString("ERR_ReadOnlyHasNoGet");

		internal static string ERR_BadAttribute1 => GetResourceString("ERR_BadAttribute1");

		internal static string ERR_LabelNotDefined1 => GetResourceString("ERR_LabelNotDefined1");

		internal static string ERR_ErrorCreatingWin32ResourceFile => GetResourceString("ERR_ErrorCreatingWin32ResourceFile");

		internal static string ERR_UnableToCreateTempFile => GetResourceString("ERR_UnableToCreateTempFile");

		internal static string ERR_RequiredNewCall2 => GetResourceString("ERR_RequiredNewCall2");

		internal static string ERR_UnimplementedMember3 => GetResourceString("ERR_UnimplementedMember3");

		internal static string ERR_BadWithRef => GetResourceString("ERR_BadWithRef");

		internal static string ERR_DuplicateAccessCategoryUsed => GetResourceString("ERR_DuplicateAccessCategoryUsed");

		internal static string ERR_DuplicateModifierCategoryUsed => GetResourceString("ERR_DuplicateModifierCategoryUsed");

		internal static string ERR_DuplicateSpecifier => GetResourceString("ERR_DuplicateSpecifier");

		internal static string ERR_TypeConflict6 => GetResourceString("ERR_TypeConflict6");

		internal static string ERR_UnrecognizedTypeKeyword => GetResourceString("ERR_UnrecognizedTypeKeyword");

		internal static string ERR_ExtraSpecifiers => GetResourceString("ERR_ExtraSpecifiers");

		internal static string ERR_UnrecognizedType => GetResourceString("ERR_UnrecognizedType");

		internal static string ERR_InvalidUseOfKeyword => GetResourceString("ERR_InvalidUseOfKeyword");

		internal static string ERR_InvalidEndEnum => GetResourceString("ERR_InvalidEndEnum");

		internal static string ERR_MissingEndEnum => GetResourceString("ERR_MissingEndEnum");

		internal static string ERR_ExpectedDeclaration => GetResourceString("ERR_ExpectedDeclaration");

		internal static string ERR_ParamArrayMustBeLast => GetResourceString("ERR_ParamArrayMustBeLast");

		internal static string ERR_SpecifiersInvalidOnInheritsImplOpt => GetResourceString("ERR_SpecifiersInvalidOnInheritsImplOpt");

		internal static string ERR_ExpectedSpecifier => GetResourceString("ERR_ExpectedSpecifier");

		internal static string ERR_ExpectedComma => GetResourceString("ERR_ExpectedComma");

		internal static string ERR_ExpectedAs => GetResourceString("ERR_ExpectedAs");

		internal static string ERR_ExpectedRparen => GetResourceString("ERR_ExpectedRparen");

		internal static string ERR_ExpectedLparen => GetResourceString("ERR_ExpectedLparen");

		internal static string ERR_InvalidNewInType => GetResourceString("ERR_InvalidNewInType");

		internal static string ERR_ExpectedExpression => GetResourceString("ERR_ExpectedExpression");

		internal static string ERR_ExpectedOptional => GetResourceString("ERR_ExpectedOptional");

		internal static string ERR_ExpectedIdentifier => GetResourceString("ERR_ExpectedIdentifier");

		internal static string ERR_ExpectedIntLiteral => GetResourceString("ERR_ExpectedIntLiteral");

		internal static string ERR_ExpectedEOS => GetResourceString("ERR_ExpectedEOS");

		internal static string ERR_ExpectedForOptionStmt => GetResourceString("ERR_ExpectedForOptionStmt");

		internal static string ERR_InvalidOptionCompare => GetResourceString("ERR_InvalidOptionCompare");

		internal static string ERR_ExpectedOptionCompare => GetResourceString("ERR_ExpectedOptionCompare");

		internal static string ERR_StrictDisallowImplicitObject => GetResourceString("ERR_StrictDisallowImplicitObject");

		internal static string ERR_StrictDisallowsImplicitProc => GetResourceString("ERR_StrictDisallowsImplicitProc");

		internal static string ERR_StrictDisallowsImplicitArgs => GetResourceString("ERR_StrictDisallowsImplicitArgs");

		internal static string ERR_InvalidParameterSyntax => GetResourceString("ERR_InvalidParameterSyntax");

		internal static string ERR_ExpectedSubFunction => GetResourceString("ERR_ExpectedSubFunction");

		internal static string ERR_ExpectedStringLiteral => GetResourceString("ERR_ExpectedStringLiteral");

		internal static string ERR_MissingLibInDeclare => GetResourceString("ERR_MissingLibInDeclare");

		internal static string ERR_DelegateNoInvoke1 => GetResourceString("ERR_DelegateNoInvoke1");

		internal static string ERR_MissingIsInTypeOf => GetResourceString("ERR_MissingIsInTypeOf");

		internal static string ERR_DuplicateOption1 => GetResourceString("ERR_DuplicateOption1");

		internal static string ERR_ModuleCantInherit => GetResourceString("ERR_ModuleCantInherit");

		internal static string ERR_ModuleCantImplement => GetResourceString("ERR_ModuleCantImplement");

		internal static string ERR_BadImplementsType => GetResourceString("ERR_BadImplementsType");

		internal static string ERR_BadConstFlags1 => GetResourceString("ERR_BadConstFlags1");

		internal static string ERR_BadWithEventsFlags1 => GetResourceString("ERR_BadWithEventsFlags1");

		internal static string ERR_BadDimFlags1 => GetResourceString("ERR_BadDimFlags1");

		internal static string ERR_DuplicateParamName1 => GetResourceString("ERR_DuplicateParamName1");

		internal static string ERR_LoopDoubleCondition => GetResourceString("ERR_LoopDoubleCondition");

		internal static string ERR_ExpectedRelational => GetResourceString("ERR_ExpectedRelational");

		internal static string ERR_ExpectedExitKind => GetResourceString("ERR_ExpectedExitKind");

		internal static string ERR_ExpectedNamedArgumentInAttributeList => GetResourceString("ERR_ExpectedNamedArgumentInAttributeList");

		internal static string ERR_NamedArgumentSpecificationBeforeFixedArgumentInLateboundInvocation => GetResourceString("ERR_NamedArgumentSpecificationBeforeFixedArgumentInLateboundInvocation");

		internal static string ERR_ExpectedNamedArgument => GetResourceString("ERR_ExpectedNamedArgument");

		internal static string ERR_BadMethodFlags1 => GetResourceString("ERR_BadMethodFlags1");

		internal static string ERR_BadEventFlags1 => GetResourceString("ERR_BadEventFlags1");

		internal static string ERR_BadDeclareFlags1 => GetResourceString("ERR_BadDeclareFlags1");

		internal static string ERR_BadLocalConstFlags1 => GetResourceString("ERR_BadLocalConstFlags1");

		internal static string ERR_BadLocalDimFlags1 => GetResourceString("ERR_BadLocalDimFlags1");

		internal static string ERR_ExpectedConditionalDirective => GetResourceString("ERR_ExpectedConditionalDirective");

		internal static string ERR_ExpectedEQ => GetResourceString("ERR_ExpectedEQ");

		internal static string ERR_ConstructorNotFound1 => GetResourceString("ERR_ConstructorNotFound1");

		internal static string ERR_InvalidEndInterface => GetResourceString("ERR_InvalidEndInterface");

		internal static string ERR_MissingEndInterface => GetResourceString("ERR_MissingEndInterface");

		internal static string ERR_InheritsFrom2 => GetResourceString("ERR_InheritsFrom2");

		internal static string ERR_IsNestedIn2 => GetResourceString("ERR_IsNestedIn2");

		internal static string ERR_InheritanceCycle1 => GetResourceString("ERR_InheritanceCycle1");

		internal static string ERR_InheritsFromNonClass => GetResourceString("ERR_InheritsFromNonClass");

		internal static string ERR_MultiplyDefinedType3 => GetResourceString("ERR_MultiplyDefinedType3");

		internal static string ERR_BadOverrideAccess2 => GetResourceString("ERR_BadOverrideAccess2");

		internal static string ERR_CantOverrideNotOverridable2 => GetResourceString("ERR_CantOverrideNotOverridable2");

		internal static string ERR_DuplicateProcDef1 => GetResourceString("ERR_DuplicateProcDef1");

		internal static string ERR_DuplicateProcDefWithDifferentTupleNames2 => GetResourceString("ERR_DuplicateProcDefWithDifferentTupleNames2");

		internal static string ERR_BadInterfaceMethodFlags1 => GetResourceString("ERR_BadInterfaceMethodFlags1");

		internal static string ERR_NamedParamNotFound2 => GetResourceString("ERR_NamedParamNotFound2");

		internal static string ERR_BadInterfacePropertyFlags1 => GetResourceString("ERR_BadInterfacePropertyFlags1");

		internal static string ERR_NamedArgUsedTwice2 => GetResourceString("ERR_NamedArgUsedTwice2");

		internal static string ERR_InterfaceCantUseEventSpecifier1 => GetResourceString("ERR_InterfaceCantUseEventSpecifier1");

		internal static string ERR_TypecharNoMatch2 => GetResourceString("ERR_TypecharNoMatch2");

		internal static string ERR_ExpectedSubOrFunction => GetResourceString("ERR_ExpectedSubOrFunction");

		internal static string ERR_BadEmptyEnum1 => GetResourceString("ERR_BadEmptyEnum1");

		internal static string ERR_InvalidConstructorCall => GetResourceString("ERR_InvalidConstructorCall");

		internal static string ERR_CantOverrideConstructor => GetResourceString("ERR_CantOverrideConstructor");

		internal static string ERR_ConstructorCannotBeDeclaredPartial => GetResourceString("ERR_ConstructorCannotBeDeclaredPartial");

		internal static string ERR_ModuleEmitFailure => GetResourceString("ERR_ModuleEmitFailure");

		internal static string ERR_EncUpdateFailedMissingAttribute => GetResourceString("ERR_EncUpdateFailedMissingAttribute");

		internal static string ERR_OverrideNotNeeded3 => GetResourceString("ERR_OverrideNotNeeded3");

		internal static string ERR_ExpectedDot => GetResourceString("ERR_ExpectedDot");

		internal static string ERR_DuplicateLocals1 => GetResourceString("ERR_DuplicateLocals1");

		internal static string ERR_InvInsideEndsProc => GetResourceString("ERR_InvInsideEndsProc");

		internal static string ERR_LocalSameAsFunc => GetResourceString("ERR_LocalSameAsFunc");

		internal static string ERR_RecordEmbeds2 => GetResourceString("ERR_RecordEmbeds2");

		internal static string ERR_RecordCycle2 => GetResourceString("ERR_RecordCycle2");

		internal static string ERR_InterfaceCycle1 => GetResourceString("ERR_InterfaceCycle1");

		internal static string ERR_SubNewCycle2 => GetResourceString("ERR_SubNewCycle2");

		internal static string ERR_SubNewCycle1 => GetResourceString("ERR_SubNewCycle1");

		internal static string ERR_InheritsFromCantInherit3 => GetResourceString("ERR_InheritsFromCantInherit3");

		internal static string ERR_OverloadWithOptional2 => GetResourceString("ERR_OverloadWithOptional2");

		internal static string ERR_OverloadWithReturnType2 => GetResourceString("ERR_OverloadWithReturnType2");

		internal static string ERR_TypeCharWithType1 => GetResourceString("ERR_TypeCharWithType1");

		internal static string ERR_TypeCharOnSub => GetResourceString("ERR_TypeCharOnSub");

		internal static string ERR_OverloadWithDefault2 => GetResourceString("ERR_OverloadWithDefault2");

		internal static string ERR_MissingSubscript => GetResourceString("ERR_MissingSubscript");

		internal static string ERR_OverrideWithDefault2 => GetResourceString("ERR_OverrideWithDefault2");

		internal static string ERR_OverrideWithOptional2 => GetResourceString("ERR_OverrideWithOptional2");

		internal static string ERR_FieldOfValueFieldOfMarshalByRef3 => GetResourceString("ERR_FieldOfValueFieldOfMarshalByRef3");

		internal static string ERR_TypeMismatch2 => GetResourceString("ERR_TypeMismatch2");

		internal static string ERR_CaseAfterCaseElse => GetResourceString("ERR_CaseAfterCaseElse");

		internal static string ERR_ConvertArrayMismatch4 => GetResourceString("ERR_ConvertArrayMismatch4");

		internal static string ERR_ConvertObjectArrayMismatch3 => GetResourceString("ERR_ConvertObjectArrayMismatch3");

		internal static string ERR_ForLoopType1 => GetResourceString("ERR_ForLoopType1");

		internal static string ERR_OverloadWithByref2 => GetResourceString("ERR_OverloadWithByref2");

		internal static string ERR_InheritsFromNonInterface => GetResourceString("ERR_InheritsFromNonInterface");

		internal static string ERR_BadInterfaceOrderOnInherits => GetResourceString("ERR_BadInterfaceOrderOnInherits");

		internal static string ERR_DuplicateDefaultProps1 => GetResourceString("ERR_DuplicateDefaultProps1");

		internal static string ERR_DefaultMissingFromProperty2 => GetResourceString("ERR_DefaultMissingFromProperty2");

		internal static string ERR_OverridingPropertyKind2 => GetResourceString("ERR_OverridingPropertyKind2");

		internal static string ERR_NewInInterface => GetResourceString("ERR_NewInInterface");

		internal static string ERR_BadFlagsOnNew1 => GetResourceString("ERR_BadFlagsOnNew1");

		internal static string ERR_OverloadingPropertyKind2 => GetResourceString("ERR_OverloadingPropertyKind2");

		internal static string ERR_NoDefaultNotExtend1 => GetResourceString("ERR_NoDefaultNotExtend1");

		internal static string ERR_OverloadWithArrayVsParamArray2 => GetResourceString("ERR_OverloadWithArrayVsParamArray2");

		internal static string ERR_BadInstanceMemberAccess => GetResourceString("ERR_BadInstanceMemberAccess");

		internal static string ERR_ExpectedRbrace => GetResourceString("ERR_ExpectedRbrace");

		internal static string ERR_ModuleAsType1 => GetResourceString("ERR_ModuleAsType1");

		internal static string ERR_NewIfNullOnNonClass => GetResourceString("ERR_NewIfNullOnNonClass");

		internal static string ERR_CatchAfterFinally => GetResourceString("ERR_CatchAfterFinally");

		internal static string ERR_CatchNoMatchingTry => GetResourceString("ERR_CatchNoMatchingTry");

		internal static string ERR_FinallyAfterFinally => GetResourceString("ERR_FinallyAfterFinally");

		internal static string ERR_FinallyNoMatchingTry => GetResourceString("ERR_FinallyNoMatchingTry");

		internal static string ERR_EndTryNoTry => GetResourceString("ERR_EndTryNoTry");

		internal static string ERR_ExpectedEndTry => GetResourceString("ERR_ExpectedEndTry");

		internal static string ERR_BadDelegateFlags1 => GetResourceString("ERR_BadDelegateFlags1");

		internal static string ERR_NoConstructorOnBase2 => GetResourceString("ERR_NoConstructorOnBase2");

		internal static string ERR_InaccessibleSymbol2 => GetResourceString("ERR_InaccessibleSymbol2");

		internal static string ERR_InaccessibleMember3 => GetResourceString("ERR_InaccessibleMember3");

		internal static string ERR_CatchNotException1 => GetResourceString("ERR_CatchNotException1");

		internal static string ERR_ExitTryNotWithinTry => GetResourceString("ERR_ExitTryNotWithinTry");

		internal static string ERR_BadRecordFlags1 => GetResourceString("ERR_BadRecordFlags1");

		internal static string ERR_BadEnumFlags1 => GetResourceString("ERR_BadEnumFlags1");

		internal static string ERR_BadInterfaceFlags1 => GetResourceString("ERR_BadInterfaceFlags1");

		internal static string ERR_OverrideWithByref2 => GetResourceString("ERR_OverrideWithByref2");

		internal static string ERR_MyBaseAbstractCall1 => GetResourceString("ERR_MyBaseAbstractCall1");

		internal static string ERR_IdentNotMemberOfInterface4 => GetResourceString("ERR_IdentNotMemberOfInterface4");

		internal static string ERR_ImplementingInterfaceWithDifferentTupleNames5 => GetResourceString("ERR_ImplementingInterfaceWithDifferentTupleNames5");

		internal static string ERR_WithEventsRequiresClass => GetResourceString("ERR_WithEventsRequiresClass");

		internal static string ERR_WithEventsAsStruct => GetResourceString("ERR_WithEventsAsStruct");

		internal static string ERR_ConvertArrayRankMismatch2 => GetResourceString("ERR_ConvertArrayRankMismatch2");

		internal static string ERR_RedimRankMismatch => GetResourceString("ERR_RedimRankMismatch");

		internal static string ERR_StartupCodeNotFound1 => GetResourceString("ERR_StartupCodeNotFound1");

		internal static string ERR_ConstAsNonConstant => GetResourceString("ERR_ConstAsNonConstant");

		internal static string ERR_InvalidEndSub => GetResourceString("ERR_InvalidEndSub");

		internal static string ERR_InvalidEndFunction => GetResourceString("ERR_InvalidEndFunction");

		internal static string ERR_InvalidEndProperty => GetResourceString("ERR_InvalidEndProperty");

		internal static string ERR_ModuleCantUseMethodSpecifier1 => GetResourceString("ERR_ModuleCantUseMethodSpecifier1");

		internal static string ERR_ModuleCantUseEventSpecifier1 => GetResourceString("ERR_ModuleCantUseEventSpecifier1");

		internal static string ERR_StructCantUseVarSpecifier1 => GetResourceString("ERR_StructCantUseVarSpecifier1");

		internal static string ERR_InvalidOverrideDueToReturn2 => GetResourceString("ERR_InvalidOverrideDueToReturn2");

		internal static string WRN_InvalidOverrideDueToTupleNames2 => GetResourceString("WRN_InvalidOverrideDueToTupleNames2");

		internal static string WRN_InvalidOverrideDueToTupleNames2_Title => GetResourceString("WRN_InvalidOverrideDueToTupleNames2_Title");

		internal static string ERR_ConstantWithNoValue => GetResourceString("ERR_ConstantWithNoValue");

		internal static string ERR_ExpressionOverflow1 => GetResourceString("ERR_ExpressionOverflow1");

		internal static string ERR_DuplicatePropertyGet => GetResourceString("ERR_DuplicatePropertyGet");

		internal static string ERR_DuplicatePropertySet => GetResourceString("ERR_DuplicatePropertySet");

		internal static string ERR_NameNotDeclared1 => GetResourceString("ERR_NameNotDeclared1");

		internal static string ERR_BinaryOperands3 => GetResourceString("ERR_BinaryOperands3");

		internal static string ERR_ExpectedProcedure => GetResourceString("ERR_ExpectedProcedure");

		internal static string ERR_OmittedArgument2 => GetResourceString("ERR_OmittedArgument2");

		internal static string ERR_NameNotMember2 => GetResourceString("ERR_NameNotMember2");

		internal static string ERR_EndClassNoClass => GetResourceString("ERR_EndClassNoClass");

		internal static string ERR_BadClassFlags1 => GetResourceString("ERR_BadClassFlags1");

		internal static string ERR_ImportsMustBeFirst => GetResourceString("ERR_ImportsMustBeFirst");

		internal static string ERR_NonNamespaceOrClassOnImport2 => GetResourceString("ERR_NonNamespaceOrClassOnImport2");

		internal static string ERR_TypecharNotallowed => GetResourceString("ERR_TypecharNotallowed");

		internal static string ERR_ObjectReferenceNotSupplied => GetResourceString("ERR_ObjectReferenceNotSupplied");

		internal static string ERR_MyClassNotInClass => GetResourceString("ERR_MyClassNotInClass");

		internal static string ERR_IndexedNotArrayOrProc => GetResourceString("ERR_IndexedNotArrayOrProc");

		internal static string ERR_EventSourceIsArray => GetResourceString("ERR_EventSourceIsArray");

		internal static string ERR_SharedConstructorWithParams => GetResourceString("ERR_SharedConstructorWithParams");

		internal static string ERR_SharedConstructorIllegalSpec1 => GetResourceString("ERR_SharedConstructorIllegalSpec1");

		internal static string ERR_ExpectedEndClass => GetResourceString("ERR_ExpectedEndClass");

		internal static string ERR_UnaryOperand2 => GetResourceString("ERR_UnaryOperand2");

		internal static string ERR_BadFlagsWithDefault1 => GetResourceString("ERR_BadFlagsWithDefault1");

		internal static string ERR_VoidValue => GetResourceString("ERR_VoidValue");

		internal static string ERR_ConstructorFunction => GetResourceString("ERR_ConstructorFunction");

		internal static string ERR_InvalidLiteralExponent => GetResourceString("ERR_InvalidLiteralExponent");

		internal static string ERR_NewCannotHandleEvents => GetResourceString("ERR_NewCannotHandleEvents");

		internal static string ERR_CircularEvaluation1 => GetResourceString("ERR_CircularEvaluation1");

		internal static string ERR_BadFlagsOnSharedMeth1 => GetResourceString("ERR_BadFlagsOnSharedMeth1");

		internal static string ERR_BadFlagsOnSharedProperty1 => GetResourceString("ERR_BadFlagsOnSharedProperty1");

		internal static string ERR_BadFlagsOnStdModuleProperty1 => GetResourceString("ERR_BadFlagsOnStdModuleProperty1");

		internal static string ERR_SharedOnProcThatImpl => GetResourceString("ERR_SharedOnProcThatImpl");

		internal static string ERR_NoWithEventsVarOnHandlesList => GetResourceString("ERR_NoWithEventsVarOnHandlesList");

		internal static string ERR_InheritanceAccessMismatch5 => GetResourceString("ERR_InheritanceAccessMismatch5");

		internal static string ERR_NarrowingConversionDisallowed2 => GetResourceString("ERR_NarrowingConversionDisallowed2");

		internal static string ERR_NoArgumentCountOverloadCandidates1 => GetResourceString("ERR_NoArgumentCountOverloadCandidates1");

		internal static string ERR_NoViableOverloadCandidates1 => GetResourceString("ERR_NoViableOverloadCandidates1");

		internal static string ERR_NoCallableOverloadCandidates2 => GetResourceString("ERR_NoCallableOverloadCandidates2");

		internal static string ERR_BadOverloadCandidates2 => GetResourceString("ERR_BadOverloadCandidates2");

		internal static string ERR_NoNonNarrowingOverloadCandidates2 => GetResourceString("ERR_NoNonNarrowingOverloadCandidates2");

		internal static string ERR_ArgumentNarrowing3 => GetResourceString("ERR_ArgumentNarrowing3");

		internal static string ERR_NoMostSpecificOverload2 => GetResourceString("ERR_NoMostSpecificOverload2");

		internal static string ERR_NotMostSpecificOverload => GetResourceString("ERR_NotMostSpecificOverload");

		internal static string ERR_OverloadCandidate2 => GetResourceString("ERR_OverloadCandidate2");

		internal static string ERR_NoGetProperty1 => GetResourceString("ERR_NoGetProperty1");

		internal static string ERR_NoSetProperty1 => GetResourceString("ERR_NoSetProperty1");

		internal static string ERR_ParamTypingInconsistency => GetResourceString("ERR_ParamTypingInconsistency");

		internal static string ERR_ParamNameFunctionNameCollision => GetResourceString("ERR_ParamNameFunctionNameCollision");

		internal static string ERR_DateToDoubleConversion => GetResourceString("ERR_DateToDoubleConversion");

		internal static string ERR_DoubleToDateConversion => GetResourceString("ERR_DoubleToDateConversion");

		internal static string ERR_ZeroDivide => GetResourceString("ERR_ZeroDivide");

		internal static string ERR_TryAndOnErrorDoNotMix => GetResourceString("ERR_TryAndOnErrorDoNotMix");

		internal static string ERR_PropertyAccessIgnored => GetResourceString("ERR_PropertyAccessIgnored");

		internal static string ERR_InterfaceNoDefault1 => GetResourceString("ERR_InterfaceNoDefault1");

		internal static string ERR_InvalidAssemblyAttribute1 => GetResourceString("ERR_InvalidAssemblyAttribute1");

		internal static string ERR_InvalidModuleAttribute1 => GetResourceString("ERR_InvalidModuleAttribute1");

		internal static string ERR_AmbiguousInUnnamedNamespace1 => GetResourceString("ERR_AmbiguousInUnnamedNamespace1");

		internal static string ERR_DefaultMemberNotProperty1 => GetResourceString("ERR_DefaultMemberNotProperty1");

		internal static string ERR_AmbiguousInNamespace2 => GetResourceString("ERR_AmbiguousInNamespace2");

		internal static string ERR_AmbiguousInImports2 => GetResourceString("ERR_AmbiguousInImports2");

		internal static string ERR_AmbiguousInModules2 => GetResourceString("ERR_AmbiguousInModules2");

		internal static string ERR_AmbiguousInNamespaces2 => GetResourceString("ERR_AmbiguousInNamespaces2");

		internal static string ERR_ArrayInitializerTooFewDimensions => GetResourceString("ERR_ArrayInitializerTooFewDimensions");

		internal static string ERR_ArrayInitializerTooManyDimensions => GetResourceString("ERR_ArrayInitializerTooManyDimensions");

		internal static string ERR_InitializerTooFewElements1 => GetResourceString("ERR_InitializerTooFewElements1");

		internal static string ERR_InitializerTooManyElements1 => GetResourceString("ERR_InitializerTooManyElements1");

		internal static string ERR_NewOnAbstractClass => GetResourceString("ERR_NewOnAbstractClass");

		internal static string ERR_DuplicateNamedImportAlias1 => GetResourceString("ERR_DuplicateNamedImportAlias1");

		internal static string ERR_DuplicatePrefix => GetResourceString("ERR_DuplicatePrefix");

		internal static string ERR_StdInOptionProvidedButConsoleInputIsNotRedirected => GetResourceString("ERR_StdInOptionProvidedButConsoleInputIsNotRedirected");

		internal static string ERR_StrictDisallowsLateBinding => GetResourceString("ERR_StrictDisallowsLateBinding");

		internal static string ERR_AddressOfOperandNotMethod => GetResourceString("ERR_AddressOfOperandNotMethod");

		internal static string ERR_EndExternalSource => GetResourceString("ERR_EndExternalSource");

		internal static string ERR_ExpectedEndExternalSource => GetResourceString("ERR_ExpectedEndExternalSource");

		internal static string ERR_NestedExternalSource => GetResourceString("ERR_NestedExternalSource");

		internal static string ERR_AddressOfNotDelegate1 => GetResourceString("ERR_AddressOfNotDelegate1");

		internal static string ERR_SyncLockRequiresReferenceType1 => GetResourceString("ERR_SyncLockRequiresReferenceType1");

		internal static string ERR_MethodAlreadyImplemented2 => GetResourceString("ERR_MethodAlreadyImplemented2");

		internal static string ERR_DuplicateInInherits1 => GetResourceString("ERR_DuplicateInInherits1");

		internal static string ERR_NamedParamArrayArgument => GetResourceString("ERR_NamedParamArrayArgument");

		internal static string ERR_OmittedParamArrayArgument => GetResourceString("ERR_OmittedParamArrayArgument");

		internal static string ERR_ParamArrayArgumentMismatch => GetResourceString("ERR_ParamArrayArgumentMismatch");

		internal static string ERR_EventNotFound1 => GetResourceString("ERR_EventNotFound1");

		internal static string ERR_ModuleCantUseVariableSpecifier1 => GetResourceString("ERR_ModuleCantUseVariableSpecifier1");

		internal static string ERR_SharedEventNeedsSharedHandler => GetResourceString("ERR_SharedEventNeedsSharedHandler");

		internal static string ERR_SharedEventNeedsHandlerInTheSameType => GetResourceString("ERR_SharedEventNeedsHandlerInTheSameType");

		internal static string ERR_ExpectedMinus => GetResourceString("ERR_ExpectedMinus");

		internal static string ERR_InterfaceMemberSyntax => GetResourceString("ERR_InterfaceMemberSyntax");

		internal static string ERR_InvInsideInterface => GetResourceString("ERR_InvInsideInterface");

		internal static string ERR_InvInsideEndsInterface => GetResourceString("ERR_InvInsideEndsInterface");

		internal static string ERR_BadFlagsInNotInheritableClass1 => GetResourceString("ERR_BadFlagsInNotInheritableClass1");

		internal static string ERR_BaseOnlyClassesMustBeExplicit2 => GetResourceString("ERR_BaseOnlyClassesMustBeExplicit2");

		internal static string ERR_MustInheritEventNotOverridden => GetResourceString("ERR_MustInheritEventNotOverridden");

		internal static string ERR_NegativeArraySize => GetResourceString("ERR_NegativeArraySize");

		internal static string ERR_MyClassAbstractCall1 => GetResourceString("ERR_MyClassAbstractCall1");

		internal static string ERR_EndDisallowedInDllProjects => GetResourceString("ERR_EndDisallowedInDllProjects");

		internal static string ERR_BlockLocalShadowing1 => GetResourceString("ERR_BlockLocalShadowing1");

		internal static string ERR_ModuleNotAtNamespace => GetResourceString("ERR_ModuleNotAtNamespace");

		internal static string ERR_NamespaceNotAtNamespace => GetResourceString("ERR_NamespaceNotAtNamespace");

		internal static string ERR_InvInsideEnum => GetResourceString("ERR_InvInsideEnum");

		internal static string ERR_InvInsideEndsEnum => GetResourceString("ERR_InvInsideEndsEnum");

		internal static string ERR_InvalidOptionStrict => GetResourceString("ERR_InvalidOptionStrict");

		internal static string ERR_EndStructureNoStructure => GetResourceString("ERR_EndStructureNoStructure");

		internal static string ERR_EndModuleNoModule => GetResourceString("ERR_EndModuleNoModule");

		internal static string ERR_EndNamespaceNoNamespace => GetResourceString("ERR_EndNamespaceNoNamespace");

		internal static string ERR_ExpectedEndStructure => GetResourceString("ERR_ExpectedEndStructure");

		internal static string ERR_ExpectedEndModule => GetResourceString("ERR_ExpectedEndModule");

		internal static string ERR_ExpectedEndNamespace => GetResourceString("ERR_ExpectedEndNamespace");

		internal static string ERR_OptionStmtWrongOrder => GetResourceString("ERR_OptionStmtWrongOrder");

		internal static string ERR_StructCantInherit => GetResourceString("ERR_StructCantInherit");

		internal static string ERR_NewInStruct => GetResourceString("ERR_NewInStruct");

		internal static string ERR_InvalidEndGet => GetResourceString("ERR_InvalidEndGet");

		internal static string ERR_MissingEndGet => GetResourceString("ERR_MissingEndGet");

		internal static string ERR_InvalidEndSet => GetResourceString("ERR_InvalidEndSet");

		internal static string ERR_MissingEndSet => GetResourceString("ERR_MissingEndSet");

		internal static string ERR_InvInsideEndsProperty => GetResourceString("ERR_InvInsideEndsProperty");

		internal static string ERR_DuplicateWriteabilityCategoryUsed => GetResourceString("ERR_DuplicateWriteabilityCategoryUsed");

		internal static string ERR_ExpectedGreater => GetResourceString("ERR_ExpectedGreater");

		internal static string ERR_AttributeStmtWrongOrder => GetResourceString("ERR_AttributeStmtWrongOrder");

		internal static string ERR_NoExplicitArraySizes => GetResourceString("ERR_NoExplicitArraySizes");

		internal static string ERR_BadPropertyFlags1 => GetResourceString("ERR_BadPropertyFlags1");

		internal static string ERR_InvalidOptionExplicit => GetResourceString("ERR_InvalidOptionExplicit");

		internal static string ERR_MultipleParameterSpecifiers => GetResourceString("ERR_MultipleParameterSpecifiers");

		internal static string ERR_MultipleOptionalParameterSpecifiers => GetResourceString("ERR_MultipleOptionalParameterSpecifiers");

		internal static string ERR_UnsupportedProperty1 => GetResourceString("ERR_UnsupportedProperty1");

		internal static string ERR_InvalidOptionalParameterUsage1 => GetResourceString("ERR_InvalidOptionalParameterUsage1");

		internal static string ERR_ReturnFromNonFunction => GetResourceString("ERR_ReturnFromNonFunction");

		internal static string ERR_UnterminatedStringLiteral => GetResourceString("ERR_UnterminatedStringLiteral");

		internal static string ERR_UnsupportedType1 => GetResourceString("ERR_UnsupportedType1");

		internal static string ERR_InvalidEnumBase => GetResourceString("ERR_InvalidEnumBase");

		internal static string ERR_ByRefIllegal1 => GetResourceString("ERR_ByRefIllegal1");

		internal static string ERR_UnreferencedAssembly3 => GetResourceString("ERR_UnreferencedAssembly3");

		internal static string ERR_UnreferencedModule3 => GetResourceString("ERR_UnreferencedModule3");

		internal static string ERR_ReturnWithoutValue => GetResourceString("ERR_ReturnWithoutValue");

		internal static string ERR_UnsupportedField1 => GetResourceString("ERR_UnsupportedField1");

		internal static string ERR_UnsupportedMethod1 => GetResourceString("ERR_UnsupportedMethod1");

		internal static string ERR_NoNonIndexProperty1 => GetResourceString("ERR_NoNonIndexProperty1");

		internal static string ERR_BadAttributePropertyType1 => GetResourceString("ERR_BadAttributePropertyType1");

		internal static string ERR_LocalsCannotHaveAttributes => GetResourceString("ERR_LocalsCannotHaveAttributes");

		internal static string ERR_PropertyOrFieldNotDefined1 => GetResourceString("ERR_PropertyOrFieldNotDefined1");

		internal static string ERR_InvalidAttributeUsage2 => GetResourceString("ERR_InvalidAttributeUsage2");

		internal static string ERR_InvalidAttributeUsageOnAccessor => GetResourceString("ERR_InvalidAttributeUsageOnAccessor");

		internal static string ERR_NestedTypeInInheritsClause2 => GetResourceString("ERR_NestedTypeInInheritsClause2");

		internal static string ERR_TypeInItsInheritsClause1 => GetResourceString("ERR_TypeInItsInheritsClause1");

		internal static string ERR_BaseTypeReferences2 => GetResourceString("ERR_BaseTypeReferences2");

		internal static string ERR_IllegalBaseTypeReferences3 => GetResourceString("ERR_IllegalBaseTypeReferences3");

		internal static string ERR_InvalidMultipleAttributeUsage1 => GetResourceString("ERR_InvalidMultipleAttributeUsage1");

		internal static string ERR_InvalidMultipleAttributeUsageInNetModule2 => GetResourceString("ERR_InvalidMultipleAttributeUsageInNetModule2");

		internal static string ERR_CantThrowNonException => GetResourceString("ERR_CantThrowNonException");

		internal static string ERR_MustBeInCatchToRethrow => GetResourceString("ERR_MustBeInCatchToRethrow");

		internal static string ERR_ParamArrayMustBeByVal => GetResourceString("ERR_ParamArrayMustBeByVal");

		internal static string ERR_UseOfObsoleteSymbol2 => GetResourceString("ERR_UseOfObsoleteSymbol2");

		internal static string ERR_RedimNoSizes => GetResourceString("ERR_RedimNoSizes");

		internal static string ERR_InitWithMultipleDeclarators => GetResourceString("ERR_InitWithMultipleDeclarators");

		internal static string ERR_InitWithExplicitArraySizes => GetResourceString("ERR_InitWithExplicitArraySizes");

		internal static string ERR_EndSyncLockNoSyncLock => GetResourceString("ERR_EndSyncLockNoSyncLock");

		internal static string ERR_ExpectedEndSyncLock => GetResourceString("ERR_ExpectedEndSyncLock");

		internal static string ERR_NameNotEvent2 => GetResourceString("ERR_NameNotEvent2");

		internal static string ERR_AddOrRemoveHandlerEvent => GetResourceString("ERR_AddOrRemoveHandlerEvent");

		internal static string ERR_UnrecognizedEnd => GetResourceString("ERR_UnrecognizedEnd");

		internal static string ERR_ArrayInitForNonArray2 => GetResourceString("ERR_ArrayInitForNonArray2");

		internal static string ERR_EndRegionNoRegion => GetResourceString("ERR_EndRegionNoRegion");

		internal static string ERR_ExpectedEndRegion => GetResourceString("ERR_ExpectedEndRegion");

		internal static string ERR_InheritsStmtWrongOrder => GetResourceString("ERR_InheritsStmtWrongOrder");

		internal static string ERR_AmbiguousAcrossInterfaces3 => GetResourceString("ERR_AmbiguousAcrossInterfaces3");

		internal static string ERR_DefaultPropertyAmbiguousAcrossInterfaces4 => GetResourceString("ERR_DefaultPropertyAmbiguousAcrossInterfaces4");

		internal static string ERR_InterfaceEventCantUse1 => GetResourceString("ERR_InterfaceEventCantUse1");

		internal static string ERR_ExecutableAsDeclaration => GetResourceString("ERR_ExecutableAsDeclaration");

		internal static string ERR_StructureNoDefault1 => GetResourceString("ERR_StructureNoDefault1");

		internal static string ERR_MustShadow2 => GetResourceString("ERR_MustShadow2");

		internal static string ERR_OverrideWithOptionalTypes2 => GetResourceString("ERR_OverrideWithOptionalTypes2");

		internal static string ERR_ExpectedEndOfExpression => GetResourceString("ERR_ExpectedEndOfExpression");

		internal static string ERR_StructsCannotHandleEvents => GetResourceString("ERR_StructsCannotHandleEvents");

		internal static string ERR_OverridesImpliesOverridable => GetResourceString("ERR_OverridesImpliesOverridable");

		internal static string ERR_LocalNamedSameAsParam1 => GetResourceString("ERR_LocalNamedSameAsParam1");

		internal static string ERR_LocalNamedSameAsParamInLambda1 => GetResourceString("ERR_LocalNamedSameAsParamInLambda1");

		internal static string ERR_ModuleCantUseTypeSpecifier1 => GetResourceString("ERR_ModuleCantUseTypeSpecifier1");

		internal static string ERR_InValidSubMainsFound1 => GetResourceString("ERR_InValidSubMainsFound1");

		internal static string ERR_MoreThanOneValidMainWasFound2 => GetResourceString("ERR_MoreThanOneValidMainWasFound2");

		internal static string ERR_CannotConvertValue2 => GetResourceString("ERR_CannotConvertValue2");

		internal static string ERR_OnErrorInSyncLock => GetResourceString("ERR_OnErrorInSyncLock");

		internal static string ERR_NarrowingConversionCollection2 => GetResourceString("ERR_NarrowingConversionCollection2");

		internal static string ERR_GotoIntoTryHandler => GetResourceString("ERR_GotoIntoTryHandler");

		internal static string ERR_GotoIntoSyncLock => GetResourceString("ERR_GotoIntoSyncLock");

		internal static string ERR_GotoIntoWith => GetResourceString("ERR_GotoIntoWith");

		internal static string ERR_GotoIntoFor => GetResourceString("ERR_GotoIntoFor");

		internal static string ERR_BadAttributeNonPublicConstructor => GetResourceString("ERR_BadAttributeNonPublicConstructor");

		internal static string ERR_DefaultEventNotFound1 => GetResourceString("ERR_DefaultEventNotFound1");

		internal static string ERR_InvalidNonSerializedUsage => GetResourceString("ERR_InvalidNonSerializedUsage");

		internal static string ERR_ExpectedContinueKind => GetResourceString("ERR_ExpectedContinueKind");

		internal static string ERR_ContinueDoNotWithinDo => GetResourceString("ERR_ContinueDoNotWithinDo");

		internal static string ERR_ContinueForNotWithinFor => GetResourceString("ERR_ContinueForNotWithinFor");

		internal static string ERR_ContinueWhileNotWithinWhile => GetResourceString("ERR_ContinueWhileNotWithinWhile");

		internal static string ERR_DuplicateParameterSpecifier => GetResourceString("ERR_DuplicateParameterSpecifier");

		internal static string ERR_ModuleCantUseDLLDeclareSpecifier1 => GetResourceString("ERR_ModuleCantUseDLLDeclareSpecifier1");

		internal static string ERR_StructCantUseDLLDeclareSpecifier1 => GetResourceString("ERR_StructCantUseDLLDeclareSpecifier1");

		internal static string ERR_TryCastOfValueType1 => GetResourceString("ERR_TryCastOfValueType1");

		internal static string ERR_TryCastOfUnconstrainedTypeParam1 => GetResourceString("ERR_TryCastOfUnconstrainedTypeParam1");

		internal static string ERR_AmbiguousDelegateBinding2 => GetResourceString("ERR_AmbiguousDelegateBinding2");

		internal static string ERR_SharedStructMemberCannotSpecifyNew => GetResourceString("ERR_SharedStructMemberCannotSpecifyNew");

		internal static string ERR_GenericSubMainsFound1 => GetResourceString("ERR_GenericSubMainsFound1");

		internal static string ERR_GeneralProjectImportsError3 => GetResourceString("ERR_GeneralProjectImportsError3");

		internal static string ERR_InvalidTypeForAliasesImport2 => GetResourceString("ERR_InvalidTypeForAliasesImport2");

		internal static string ERR_UnsupportedConstant2 => GetResourceString("ERR_UnsupportedConstant2");

		internal static string ERR_ObsoleteArgumentsNeedParens => GetResourceString("ERR_ObsoleteArgumentsNeedParens");

		internal static string ERR_ObsoleteLineNumbersAreLabels => GetResourceString("ERR_ObsoleteLineNumbersAreLabels");

		internal static string ERR_ObsoleteStructureNotType => GetResourceString("ERR_ObsoleteStructureNotType");

		internal static string ERR_ObsoleteObjectNotVariant => GetResourceString("ERR_ObsoleteObjectNotVariant");

		internal static string ERR_ObsoleteLetSetNotNeeded => GetResourceString("ERR_ObsoleteLetSetNotNeeded");

		internal static string ERR_ObsoletePropertyGetLetSet => GetResourceString("ERR_ObsoletePropertyGetLetSet");

		internal static string ERR_ObsoleteWhileWend => GetResourceString("ERR_ObsoleteWhileWend");

		internal static string ERR_ObsoleteRedimAs => GetResourceString("ERR_ObsoleteRedimAs");

		internal static string ERR_ObsoleteOptionalWithoutValue => GetResourceString("ERR_ObsoleteOptionalWithoutValue");

		internal static string ERR_ObsoleteGosub => GetResourceString("ERR_ObsoleteGosub");

		internal static string ERR_ObsoleteOnGotoGosub => GetResourceString("ERR_ObsoleteOnGotoGosub");

		internal static string ERR_ObsoleteEndIf => GetResourceString("ERR_ObsoleteEndIf");

		internal static string ERR_ObsoleteExponent => GetResourceString("ERR_ObsoleteExponent");

		internal static string ERR_ObsoleteAsAny => GetResourceString("ERR_ObsoleteAsAny");

		internal static string ERR_ObsoleteGetStatement => GetResourceString("ERR_ObsoleteGetStatement");

		internal static string ERR_OverrideWithArrayVsParamArray2 => GetResourceString("ERR_OverrideWithArrayVsParamArray2");

		internal static string ERR_CircularBaseDependencies4 => GetResourceString("ERR_CircularBaseDependencies4");

		internal static string ERR_NestedBase2 => GetResourceString("ERR_NestedBase2");

		internal static string ERR_AccessMismatchOutsideAssembly4 => GetResourceString("ERR_AccessMismatchOutsideAssembly4");

		internal static string ERR_InheritanceAccessMismatchOutside3 => GetResourceString("ERR_InheritanceAccessMismatchOutside3");

		internal static string ERR_UseOfObsoletePropertyAccessor3 => GetResourceString("ERR_UseOfObsoletePropertyAccessor3");

		internal static string ERR_UseOfObsoletePropertyAccessor2 => GetResourceString("ERR_UseOfObsoletePropertyAccessor2");

		internal static string ERR_AccessMismatchImplementedEvent6 => GetResourceString("ERR_AccessMismatchImplementedEvent6");

		internal static string ERR_AccessMismatchImplementedEvent4 => GetResourceString("ERR_AccessMismatchImplementedEvent4");

		internal static string ERR_InheritanceCycleInImportedType1 => GetResourceString("ERR_InheritanceCycleInImportedType1");

		internal static string ERR_NoNonObsoleteConstructorOnBase3 => GetResourceString("ERR_NoNonObsoleteConstructorOnBase3");

		internal static string ERR_NoNonObsoleteConstructorOnBase4 => GetResourceString("ERR_NoNonObsoleteConstructorOnBase4");

		internal static string ERR_RequiredNonObsoleteNewCall3 => GetResourceString("ERR_RequiredNonObsoleteNewCall3");

		internal static string ERR_RequiredNonObsoleteNewCall4 => GetResourceString("ERR_RequiredNonObsoleteNewCall4");

		internal static string ERR_InheritsTypeArgAccessMismatch7 => GetResourceString("ERR_InheritsTypeArgAccessMismatch7");

		internal static string ERR_InheritsTypeArgAccessMismatchOutside5 => GetResourceString("ERR_InheritsTypeArgAccessMismatchOutside5");

		internal static string ERR_PartialTypeAccessMismatch3 => GetResourceString("ERR_PartialTypeAccessMismatch3");

		internal static string ERR_PartialTypeBadMustInherit1 => GetResourceString("ERR_PartialTypeBadMustInherit1");

		internal static string ERR_MustOverOnNotInheritPartClsMem1 => GetResourceString("ERR_MustOverOnNotInheritPartClsMem1");

		internal static string ERR_BaseMismatchForPartialClass3 => GetResourceString("ERR_BaseMismatchForPartialClass3");

		internal static string ERR_PartialTypeTypeParamNameMismatch3 => GetResourceString("ERR_PartialTypeTypeParamNameMismatch3");

		internal static string ERR_PartialTypeConstraintMismatch1 => GetResourceString("ERR_PartialTypeConstraintMismatch1");

		internal static string ERR_LateBoundOverloadInterfaceCall1 => GetResourceString("ERR_LateBoundOverloadInterfaceCall1");

		internal static string ERR_RequiredAttributeConstConversion2 => GetResourceString("ERR_RequiredAttributeConstConversion2");

		internal static string ERR_AmbiguousOverrides3 => GetResourceString("ERR_AmbiguousOverrides3");

		internal static string ERR_OverriddenCandidate1 => GetResourceString("ERR_OverriddenCandidate1");

		internal static string ERR_AmbiguousImplements3 => GetResourceString("ERR_AmbiguousImplements3");

		internal static string ERR_AddressOfNotCreatableDelegate1 => GetResourceString("ERR_AddressOfNotCreatableDelegate1");

		internal static string ERR_ComClassGenericMethod => GetResourceString("ERR_ComClassGenericMethod");

		internal static string ERR_SyntaxInCastOp => GetResourceString("ERR_SyntaxInCastOp");

		internal static string ERR_ArrayInitializerForNonConstDim => GetResourceString("ERR_ArrayInitializerForNonConstDim");

		internal static string ERR_DelegateBindingFailure3 => GetResourceString("ERR_DelegateBindingFailure3");

		internal static string ERR_StructLayoutAttributeNotAllowed => GetResourceString("ERR_StructLayoutAttributeNotAllowed");

		internal static string ERR_IterationVariableShadowLocal1 => GetResourceString("ERR_IterationVariableShadowLocal1");

		internal static string ERR_InvalidOptionInfer => GetResourceString("ERR_InvalidOptionInfer");

		internal static string ERR_CircularInference1 => GetResourceString("ERR_CircularInference1");

		internal static string ERR_InAccessibleOverridingMethod5 => GetResourceString("ERR_InAccessibleOverridingMethod5");

		internal static string ERR_NoSuitableWidestType1 => GetResourceString("ERR_NoSuitableWidestType1");

		internal static string ERR_AmbiguousWidestType3 => GetResourceString("ERR_AmbiguousWidestType3");

		internal static string ERR_ExpectedAssignmentOperatorInInit => GetResourceString("ERR_ExpectedAssignmentOperatorInInit");

		internal static string ERR_ExpectedQualifiedNameInInit => GetResourceString("ERR_ExpectedQualifiedNameInInit");

		internal static string ERR_ExpectedLbrace => GetResourceString("ERR_ExpectedLbrace");

		internal static string ERR_UnrecognizedTypeOrWith => GetResourceString("ERR_UnrecognizedTypeOrWith");

		internal static string ERR_DuplicateAggrMemberInit1 => GetResourceString("ERR_DuplicateAggrMemberInit1");

		internal static string ERR_NonFieldPropertyAggrMemberInit1 => GetResourceString("ERR_NonFieldPropertyAggrMemberInit1");

		internal static string ERR_SharedMemberAggrMemberInit1 => GetResourceString("ERR_SharedMemberAggrMemberInit1");

		internal static string ERR_ParameterizedPropertyInAggrInit1 => GetResourceString("ERR_ParameterizedPropertyInAggrInit1");

		internal static string ERR_NoZeroCountArgumentInitCandidates1 => GetResourceString("ERR_NoZeroCountArgumentInitCandidates1");

		internal static string ERR_AggrInitInvalidForObject => GetResourceString("ERR_AggrInitInvalidForObject");

		internal static string ERR_InitializerExpected => GetResourceString("ERR_InitializerExpected");

		internal static string ERR_LineContWithCommentOrNoPrecSpace => GetResourceString("ERR_LineContWithCommentOrNoPrecSpace");

		internal static string ERR_CommentsAfterLineContinuationNotAvailable1 => GetResourceString("ERR_CommentsAfterLineContinuationNotAvailable1");

		internal static string ERR_BadModuleFile1 => GetResourceString("ERR_BadModuleFile1");

		internal static string ERR_BadRefLib1 => GetResourceString("ERR_BadRefLib1");

		internal static string ERR_EventHandlerSignatureIncompatible2 => GetResourceString("ERR_EventHandlerSignatureIncompatible2");

		internal static string ERR_ConditionalCompilationConstantNotValid => GetResourceString("ERR_ConditionalCompilationConstantNotValid");

		internal static string ERR_InterfaceImplementedTwice1 => GetResourceString("ERR_InterfaceImplementedTwice1");

		internal static string ERR_InterfaceImplementedTwiceWithDifferentTupleNames2 => GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames2");

		internal static string ERR_InterfaceImplementedTwiceWithDifferentTupleNames3 => GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames3");

		internal static string ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3 => GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3");

		internal static string ERR_InterfaceImplementedTwiceWithDifferentTupleNames4 => GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames4");

		internal static string ERR_InterfaceInheritedTwiceWithDifferentTupleNames2 => GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames2");

		internal static string ERR_InterfaceInheritedTwiceWithDifferentTupleNames3 => GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames3");

		internal static string ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3 => GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3");

		internal static string ERR_InterfaceInheritedTwiceWithDifferentTupleNames4 => GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames4");

		internal static string ERR_InterfaceNotImplemented1 => GetResourceString("ERR_InterfaceNotImplemented1");

		internal static string ERR_AmbiguousImplementsMember3 => GetResourceString("ERR_AmbiguousImplementsMember3");

		internal static string ERR_ImplementsOnNew => GetResourceString("ERR_ImplementsOnNew");

		internal static string ERR_ArrayInitInStruct => GetResourceString("ERR_ArrayInitInStruct");

		internal static string ERR_EventTypeNotDelegate => GetResourceString("ERR_EventTypeNotDelegate");

		internal static string ERR_ProtectedTypeOutsideClass => GetResourceString("ERR_ProtectedTypeOutsideClass");

		internal static string ERR_DefaultPropertyWithNoParams => GetResourceString("ERR_DefaultPropertyWithNoParams");

		internal static string ERR_InitializerInStruct => GetResourceString("ERR_InitializerInStruct");

		internal static string ERR_DuplicateImport1 => GetResourceString("ERR_DuplicateImport1");

		internal static string ERR_BadModuleFlags1 => GetResourceString("ERR_BadModuleFlags1");

		internal static string ERR_ImplementsStmtWrongOrder => GetResourceString("ERR_ImplementsStmtWrongOrder");

		internal static string ERR_SynthMemberClashesWithSynth7 => GetResourceString("ERR_SynthMemberClashesWithSynth7");

		internal static string ERR_SynthMemberClashesWithMember5 => GetResourceString("ERR_SynthMemberClashesWithMember5");

		internal static string ERR_MemberClashesWithSynth6 => GetResourceString("ERR_MemberClashesWithSynth6");

		internal static string ERR_TypeClashesWithVbCoreType4 => GetResourceString("ERR_TypeClashesWithVbCoreType4");

		internal static string ERR_SecurityAttributeMissingAction => GetResourceString("ERR_SecurityAttributeMissingAction");

		internal static string ERR_SecurityAttributeInvalidAction => GetResourceString("ERR_SecurityAttributeInvalidAction");

		internal static string ERR_SecurityAttributeInvalidActionAssembly => GetResourceString("ERR_SecurityAttributeInvalidActionAssembly");

		internal static string ERR_SecurityAttributeInvalidActionTypeOrMethod => GetResourceString("ERR_SecurityAttributeInvalidActionTypeOrMethod");

		internal static string ERR_PrincipalPermissionInvalidAction => GetResourceString("ERR_PrincipalPermissionInvalidAction");

		internal static string ERR_PermissionSetAttributeInvalidFile => GetResourceString("ERR_PermissionSetAttributeInvalidFile");

		internal static string ERR_PermissionSetAttributeFileReadError => GetResourceString("ERR_PermissionSetAttributeFileReadError");

		internal static string ERR_SetHasOnlyOneParam => GetResourceString("ERR_SetHasOnlyOneParam");

		internal static string ERR_SetValueNotPropertyType => GetResourceString("ERR_SetValueNotPropertyType");

		internal static string ERR_SetHasToBeByVal1 => GetResourceString("ERR_SetHasToBeByVal1");

		internal static string ERR_StructureCantUseProtected => GetResourceString("ERR_StructureCantUseProtected");

		internal static string ERR_BadInterfaceDelegateSpecifier1 => GetResourceString("ERR_BadInterfaceDelegateSpecifier1");

		internal static string ERR_BadInterfaceEnumSpecifier1 => GetResourceString("ERR_BadInterfaceEnumSpecifier1");

		internal static string ERR_BadInterfaceClassSpecifier1 => GetResourceString("ERR_BadInterfaceClassSpecifier1");

		internal static string ERR_BadInterfaceStructSpecifier1 => GetResourceString("ERR_BadInterfaceStructSpecifier1");

		internal static string ERR_BadInterfaceInterfaceSpecifier1 => GetResourceString("ERR_BadInterfaceInterfaceSpecifier1");

		internal static string ERR_UseOfObsoleteSymbolNoMessage1 => GetResourceString("ERR_UseOfObsoleteSymbolNoMessage1");

		internal static string ERR_MetaDataIsNotAssembly => GetResourceString("ERR_MetaDataIsNotAssembly");

		internal static string ERR_MetaDataIsNotModule => GetResourceString("ERR_MetaDataIsNotModule");

		internal static string ERR_ReferenceComparison3 => GetResourceString("ERR_ReferenceComparison3");

		internal static string ERR_CatchVariableNotLocal1 => GetResourceString("ERR_CatchVariableNotLocal1");

		internal static string ERR_ModuleMemberCantImplement => GetResourceString("ERR_ModuleMemberCantImplement");

		internal static string ERR_EventDelegatesCantBeFunctions => GetResourceString("ERR_EventDelegatesCantBeFunctions");

		internal static string ERR_InvalidDate => GetResourceString("ERR_InvalidDate");

		internal static string ERR_CantOverride4 => GetResourceString("ERR_CantOverride4");

		internal static string ERR_CantSpecifyArraysOnBoth => GetResourceString("ERR_CantSpecifyArraysOnBoth");

		internal static string ERR_NotOverridableRequiresOverrides => GetResourceString("ERR_NotOverridableRequiresOverrides");

		internal static string ERR_PrivateTypeOutsideType => GetResourceString("ERR_PrivateTypeOutsideType");

		internal static string ERR_TypeRefResolutionError3 => GetResourceString("ERR_TypeRefResolutionError3");

		internal static string ERR_ValueTupleTypeRefResolutionError1 => GetResourceString("ERR_ValueTupleTypeRefResolutionError1");

		internal static string ERR_ParamArrayWrongType => GetResourceString("ERR_ParamArrayWrongType");

		internal static string ERR_CoClassMissing2 => GetResourceString("ERR_CoClassMissing2");

		internal static string ERR_InvalidCoClass1 => GetResourceString("ERR_InvalidCoClass1");

		internal static string ERR_InvalidMeReference => GetResourceString("ERR_InvalidMeReference");

		internal static string ERR_InvalidImplicitMeReference => GetResourceString("ERR_InvalidImplicitMeReference");

		internal static string ERR_RuntimeMemberNotFound2 => GetResourceString("ERR_RuntimeMemberNotFound2");

		internal static string ERR_BadPropertyAccessorFlags => GetResourceString("ERR_BadPropertyAccessorFlags");

		internal static string ERR_BadPropertyAccessorFlagsRestrict => GetResourceString("ERR_BadPropertyAccessorFlagsRestrict");

		internal static string ERR_OnlyOneAccessorForGetSet => GetResourceString("ERR_OnlyOneAccessorForGetSet");

		internal static string ERR_NoAccessibleSet => GetResourceString("ERR_NoAccessibleSet");

		internal static string ERR_NoAccessibleGet => GetResourceString("ERR_NoAccessibleGet");

		internal static string ERR_WriteOnlyNoAccessorFlag => GetResourceString("ERR_WriteOnlyNoAccessorFlag");

		internal static string ERR_ReadOnlyNoAccessorFlag => GetResourceString("ERR_ReadOnlyNoAccessorFlag");

		internal static string ERR_BadPropertyAccessorFlags1 => GetResourceString("ERR_BadPropertyAccessorFlags1");

		internal static string ERR_BadPropertyAccessorFlags2 => GetResourceString("ERR_BadPropertyAccessorFlags2");

		internal static string ERR_BadPropertyAccessorFlags3 => GetResourceString("ERR_BadPropertyAccessorFlags3");

		internal static string ERR_InAccessibleCoClass3 => GetResourceString("ERR_InAccessibleCoClass3");

		internal static string ERR_MissingValuesForArraysInApplAttrs => GetResourceString("ERR_MissingValuesForArraysInApplAttrs");

		internal static string ERR_ExitEventMemberNotInvalid => GetResourceString("ERR_ExitEventMemberNotInvalid");

		internal static string ERR_InvInsideEndsEvent => GetResourceString("ERR_InvInsideEndsEvent");

		internal static string ERR_MissingEndEvent => GetResourceString("ERR_MissingEndEvent");

		internal static string ERR_MissingEndAddHandler => GetResourceString("ERR_MissingEndAddHandler");

		internal static string ERR_MissingEndRemoveHandler => GetResourceString("ERR_MissingEndRemoveHandler");

		internal static string ERR_MissingEndRaiseEvent => GetResourceString("ERR_MissingEndRaiseEvent");

		internal static string ERR_CustomEventInvInInterface => GetResourceString("ERR_CustomEventInvInInterface");

		internal static string ERR_CustomEventRequiresAs => GetResourceString("ERR_CustomEventRequiresAs");

		internal static string ERR_InvalidEndEvent => GetResourceString("ERR_InvalidEndEvent");

		internal static string ERR_InvalidEndAddHandler => GetResourceString("ERR_InvalidEndAddHandler");

		internal static string ERR_InvalidEndRemoveHandler => GetResourceString("ERR_InvalidEndRemoveHandler");

		internal static string ERR_InvalidEndRaiseEvent => GetResourceString("ERR_InvalidEndRaiseEvent");

		internal static string ERR_DuplicateAddHandlerDef => GetResourceString("ERR_DuplicateAddHandlerDef");

		internal static string ERR_DuplicateRemoveHandlerDef => GetResourceString("ERR_DuplicateRemoveHandlerDef");

		internal static string ERR_DuplicateRaiseEventDef => GetResourceString("ERR_DuplicateRaiseEventDef");

		internal static string ERR_MissingAddHandlerDef1 => GetResourceString("ERR_MissingAddHandlerDef1");

		internal static string ERR_MissingRemoveHandlerDef1 => GetResourceString("ERR_MissingRemoveHandlerDef1");

		internal static string ERR_MissingRaiseEventDef1 => GetResourceString("ERR_MissingRaiseEventDef1");

		internal static string ERR_EventAddRemoveHasOnlyOneParam => GetResourceString("ERR_EventAddRemoveHasOnlyOneParam");

		internal static string ERR_EventAddRemoveByrefParamIllegal => GetResourceString("ERR_EventAddRemoveByrefParamIllegal");

		internal static string ERR_SpecifiersInvOnEventMethod => GetResourceString("ERR_SpecifiersInvOnEventMethod");

		internal static string ERR_AddRemoveParamNotEventType => GetResourceString("ERR_AddRemoveParamNotEventType");

		internal static string ERR_RaiseEventShapeMismatch1 => GetResourceString("ERR_RaiseEventShapeMismatch1");

		internal static string ERR_EventMethodOptionalParamIllegal1 => GetResourceString("ERR_EventMethodOptionalParamIllegal1");

		internal static string ERR_CantReferToMyGroupInsideGroupType1 => GetResourceString("ERR_CantReferToMyGroupInsideGroupType1");

		internal static string ERR_InvalidUseOfCustomModifier => GetResourceString("ERR_InvalidUseOfCustomModifier");

		internal static string ERR_InvalidOptionStrictCustom => GetResourceString("ERR_InvalidOptionStrictCustom");

		internal static string ERR_ObsoleteInvalidOnEventMember => GetResourceString("ERR_ObsoleteInvalidOnEventMember");

		internal static string ERR_DelegateBindingIncompatible2 => GetResourceString("ERR_DelegateBindingIncompatible2");

		internal static string ERR_ExpectedXmlName => GetResourceString("ERR_ExpectedXmlName");

		internal static string ERR_UndefinedXmlPrefix => GetResourceString("ERR_UndefinedXmlPrefix");

		internal static string ERR_DuplicateXmlAttribute => GetResourceString("ERR_DuplicateXmlAttribute");

		internal static string ERR_MismatchedXmlEndTag => GetResourceString("ERR_MismatchedXmlEndTag");

		internal static string ERR_MissingXmlEndTag => GetResourceString("ERR_MissingXmlEndTag");

		internal static string ERR_ReservedXmlPrefix => GetResourceString("ERR_ReservedXmlPrefix");

		internal static string ERR_MissingVersionInXmlDecl => GetResourceString("ERR_MissingVersionInXmlDecl");

		internal static string ERR_IllegalAttributeInXmlDecl => GetResourceString("ERR_IllegalAttributeInXmlDecl");

		internal static string ERR_QuotedEmbeddedExpression => GetResourceString("ERR_QuotedEmbeddedExpression");

		internal static string ERR_VersionMustBeFirstInXmlDecl => GetResourceString("ERR_VersionMustBeFirstInXmlDecl");

		internal static string ERR_AttributeOrder => GetResourceString("ERR_AttributeOrder");

		internal static string ERR_ExpectedXmlEndEmbedded => GetResourceString("ERR_ExpectedXmlEndEmbedded");

		internal static string ERR_ExpectedXmlEndPI => GetResourceString("ERR_ExpectedXmlEndPI");

		internal static string ERR_ExpectedXmlEndComment => GetResourceString("ERR_ExpectedXmlEndComment");

		internal static string ERR_ExpectedXmlEndCData => GetResourceString("ERR_ExpectedXmlEndCData");

		internal static string ERR_ExpectedSQuote => GetResourceString("ERR_ExpectedSQuote");

		internal static string ERR_ExpectedQuote => GetResourceString("ERR_ExpectedQuote");

		internal static string ERR_ExpectedLT => GetResourceString("ERR_ExpectedLT");

		internal static string ERR_StartAttributeValue => GetResourceString("ERR_StartAttributeValue");

		internal static string ERR_ExpectedDiv => GetResourceString("ERR_ExpectedDiv");

		internal static string ERR_NoXmlAxesLateBinding => GetResourceString("ERR_NoXmlAxesLateBinding");

		internal static string ERR_IllegalXmlStartNameChar => GetResourceString("ERR_IllegalXmlStartNameChar");

		internal static string ERR_IllegalXmlNameChar => GetResourceString("ERR_IllegalXmlNameChar");

		internal static string ERR_IllegalXmlCommentChar => GetResourceString("ERR_IllegalXmlCommentChar");

		internal static string ERR_EmbeddedExpression => GetResourceString("ERR_EmbeddedExpression");

		internal static string ERR_ExpectedXmlWhiteSpace => GetResourceString("ERR_ExpectedXmlWhiteSpace");

		internal static string ERR_IllegalProcessingInstructionName => GetResourceString("ERR_IllegalProcessingInstructionName");

		internal static string ERR_DTDNotSupported => GetResourceString("ERR_DTDNotSupported");

		internal static string ERR_IllegalXmlWhiteSpace => GetResourceString("ERR_IllegalXmlWhiteSpace");

		internal static string ERR_ExpectedSColon => GetResourceString("ERR_ExpectedSColon");

		internal static string ERR_ExpectedXmlBeginEmbedded => GetResourceString("ERR_ExpectedXmlBeginEmbedded");

		internal static string ERR_XmlEntityReference => GetResourceString("ERR_XmlEntityReference");

		internal static string ERR_InvalidAttributeValue1 => GetResourceString("ERR_InvalidAttributeValue1");

		internal static string ERR_InvalidAttributeValue2 => GetResourceString("ERR_InvalidAttributeValue2");

		internal static string ERR_ReservedXmlNamespace => GetResourceString("ERR_ReservedXmlNamespace");

		internal static string ERR_IllegalDefaultNamespace => GetResourceString("ERR_IllegalDefaultNamespace");

		internal static string ERR_QualifiedNameNotAllowed => GetResourceString("ERR_QualifiedNameNotAllowed");

		internal static string ERR_ExpectedXmlns => GetResourceString("ERR_ExpectedXmlns");

		internal static string ERR_IllegalXmlnsPrefix => GetResourceString("ERR_IllegalXmlnsPrefix");

		internal static string ERR_XmlFeaturesNotAvailable => GetResourceString("ERR_XmlFeaturesNotAvailable");

		internal static string ERR_UnableToReadUacManifest2 => GetResourceString("ERR_UnableToReadUacManifest2");

		internal static string WRN_UseValueForXmlExpression3 => GetResourceString("WRN_UseValueForXmlExpression3");

		internal static string WRN_UseValueForXmlExpression3_Title => GetResourceString("WRN_UseValueForXmlExpression3_Title");

		internal static string ERR_TypeMismatchForXml3 => GetResourceString("ERR_TypeMismatchForXml3");

		internal static string ERR_BinaryOperandsForXml4 => GetResourceString("ERR_BinaryOperandsForXml4");

		internal static string ERR_FullWidthAsXmlDelimiter => GetResourceString("ERR_FullWidthAsXmlDelimiter");

		internal static string ERR_InvalidSubsystemVersion => GetResourceString("ERR_InvalidSubsystemVersion");

		internal static string ERR_InvalidFileAlignment => GetResourceString("ERR_InvalidFileAlignment");

		internal static string ERR_InvalidOutputName => GetResourceString("ERR_InvalidOutputName");

		internal static string ERR_InvalidDebugInformationFormat => GetResourceString("ERR_InvalidDebugInformationFormat");

		internal static string ERR_LibAnycpu32bitPreferredConflict => GetResourceString("ERR_LibAnycpu32bitPreferredConflict");

		internal static string ERR_RestrictedAccess => GetResourceString("ERR_RestrictedAccess");

		internal static string ERR_RestrictedConversion1 => GetResourceString("ERR_RestrictedConversion1");

		internal static string ERR_NoTypecharInLabel => GetResourceString("ERR_NoTypecharInLabel");

		internal static string ERR_RestrictedType1 => GetResourceString("ERR_RestrictedType1");

		internal static string ERR_NoTypecharInAlias => GetResourceString("ERR_NoTypecharInAlias");

		internal static string ERR_NoAccessibleConstructorOnBase => GetResourceString("ERR_NoAccessibleConstructorOnBase");

		internal static string ERR_BadStaticLocalInStruct => GetResourceString("ERR_BadStaticLocalInStruct");

		internal static string ERR_DuplicateLocalStatic1 => GetResourceString("ERR_DuplicateLocalStatic1");

		internal static string ERR_ImportAliasConflictsWithType2 => GetResourceString("ERR_ImportAliasConflictsWithType2");

		internal static string ERR_CantShadowAMustOverride1 => GetResourceString("ERR_CantShadowAMustOverride1");

		internal static string ERR_MultipleEventImplMismatch3 => GetResourceString("ERR_MultipleEventImplMismatch3");

		internal static string ERR_BadSpecifierCombo2 => GetResourceString("ERR_BadSpecifierCombo2");

		internal static string ERR_MustBeOverloads2 => GetResourceString("ERR_MustBeOverloads2");

		internal static string ERR_MustOverridesInClass1 => GetResourceString("ERR_MustOverridesInClass1");

		internal static string ERR_HandlesSyntaxInClass => GetResourceString("ERR_HandlesSyntaxInClass");

		internal static string ERR_SynthMemberShadowsMustOverride5 => GetResourceString("ERR_SynthMemberShadowsMustOverride5");

		internal static string ERR_CannotOverrideInAccessibleMember => GetResourceString("ERR_CannotOverrideInAccessibleMember");

		internal static string ERR_HandlesSyntaxInModule => GetResourceString("ERR_HandlesSyntaxInModule");

		internal static string ERR_IsNotOpRequiresReferenceTypes1 => GetResourceString("ERR_IsNotOpRequiresReferenceTypes1");

		internal static string ERR_ClashWithReservedEnumMember1 => GetResourceString("ERR_ClashWithReservedEnumMember1");

		internal static string ERR_MultiplyDefinedEnumMember2 => GetResourceString("ERR_MultiplyDefinedEnumMember2");

		internal static string ERR_BadUseOfVoid => GetResourceString("ERR_BadUseOfVoid");

		internal static string ERR_EventImplMismatch5 => GetResourceString("ERR_EventImplMismatch5");

		internal static string ERR_ForwardedTypeUnavailable3 => GetResourceString("ERR_ForwardedTypeUnavailable3");

		internal static string ERR_TypeFwdCycle2 => GetResourceString("ERR_TypeFwdCycle2");

		internal static string ERR_BadTypeInCCExpression => GetResourceString("ERR_BadTypeInCCExpression");

		internal static string ERR_BadCCExpression => GetResourceString("ERR_BadCCExpression");

		internal static string ERR_VoidArrayDisallowed => GetResourceString("ERR_VoidArrayDisallowed");

		internal static string ERR_MetadataMembersAmbiguous3 => GetResourceString("ERR_MetadataMembersAmbiguous3");

		internal static string ERR_TypeOfExprAlwaysFalse2 => GetResourceString("ERR_TypeOfExprAlwaysFalse2");

		internal static string ERR_OnlyPrivatePartialMethods1 => GetResourceString("ERR_OnlyPrivatePartialMethods1");

		internal static string ERR_PartialMethodsMustBePrivate => GetResourceString("ERR_PartialMethodsMustBePrivate");

		internal static string ERR_OnlyOnePartialMethodAllowed2 => GetResourceString("ERR_OnlyOnePartialMethodAllowed2");

		internal static string ERR_OnlyOneImplementingMethodAllowed3 => GetResourceString("ERR_OnlyOneImplementingMethodAllowed3");

		internal static string ERR_PartialMethodMustBeEmpty => GetResourceString("ERR_PartialMethodMustBeEmpty");

		internal static string ERR_PartialMethodsMustBeSub1 => GetResourceString("ERR_PartialMethodsMustBeSub1");

		internal static string ERR_PartialMethodGenericConstraints2 => GetResourceString("ERR_PartialMethodGenericConstraints2");

		internal static string ERR_PartialDeclarationImplements1 => GetResourceString("ERR_PartialDeclarationImplements1");

		internal static string ERR_NoPartialMethodInAddressOf1 => GetResourceString("ERR_NoPartialMethodInAddressOf1");

		internal static string ERR_ImplementationMustBePrivate2 => GetResourceString("ERR_ImplementationMustBePrivate2");

		internal static string ERR_PartialMethodParamNamesMustMatch3 => GetResourceString("ERR_PartialMethodParamNamesMustMatch3");

		internal static string ERR_PartialMethodTypeParamNameMismatch3 => GetResourceString("ERR_PartialMethodTypeParamNameMismatch3");

		internal static string ERR_BadAttributeSharedProperty1 => GetResourceString("ERR_BadAttributeSharedProperty1");

		internal static string ERR_BadAttributeReadOnlyProperty1 => GetResourceString("ERR_BadAttributeReadOnlyProperty1");

		internal static string ERR_DuplicateResourceName1 => GetResourceString("ERR_DuplicateResourceName1");

		internal static string ERR_DuplicateResourceFileName1 => GetResourceString("ERR_DuplicateResourceFileName1");

		internal static string ERR_AttributeMustBeClassNotStruct1 => GetResourceString("ERR_AttributeMustBeClassNotStruct1");

		internal static string ERR_AttributeMustInheritSysAttr => GetResourceString("ERR_AttributeMustInheritSysAttr");

		internal static string ERR_AttributeCannotBeAbstract => GetResourceString("ERR_AttributeCannotBeAbstract");

		internal static string ERR_UnableToOpenResourceFile1 => GetResourceString("ERR_UnableToOpenResourceFile1");

		internal static string ERR_BadAttributeNonPublicProperty1 => GetResourceString("ERR_BadAttributeNonPublicProperty1");

		internal static string ERR_STAThreadAndMTAThread0 => GetResourceString("ERR_STAThreadAndMTAThread0");

		internal static string ERR_IndirectUnreferencedAssembly4 => GetResourceString("ERR_IndirectUnreferencedAssembly4");

		internal static string ERR_BadAttributeNonPublicType1 => GetResourceString("ERR_BadAttributeNonPublicType1");

		internal static string ERR_BadAttributeNonPublicContType2 => GetResourceString("ERR_BadAttributeNonPublicContType2");

		internal static string ERR_DllImportOnNonEmptySubOrFunction => GetResourceString("ERR_DllImportOnNonEmptySubOrFunction");

		internal static string ERR_DllImportNotLegalOnDeclare => GetResourceString("ERR_DllImportNotLegalOnDeclare");

		internal static string ERR_DllImportNotLegalOnGetOrSet => GetResourceString("ERR_DllImportNotLegalOnGetOrSet");

		internal static string ERR_DllImportOnGenericSubOrFunction => GetResourceString("ERR_DllImportOnGenericSubOrFunction");

		internal static string ERR_ComClassOnGeneric => GetResourceString("ERR_ComClassOnGeneric");

		internal static string ERR_DllImportOnInstanceMethod => GetResourceString("ERR_DllImportOnInstanceMethod");

		internal static string ERR_DllImportOnInterfaceMethod => GetResourceString("ERR_DllImportOnInterfaceMethod");

		internal static string ERR_DllImportNotLegalOnEventMethod => GetResourceString("ERR_DllImportNotLegalOnEventMethod");

		internal static string ERR_FriendAssemblyBadArguments => GetResourceString("ERR_FriendAssemblyBadArguments");

		internal static string ERR_FriendAssemblyStrongNameRequired => GetResourceString("ERR_FriendAssemblyStrongNameRequired");

		internal static string ERR_FriendAssemblyNameInvalid => GetResourceString("ERR_FriendAssemblyNameInvalid");

		internal static string ERR_FriendAssemblyBadAccessOverride2 => GetResourceString("ERR_FriendAssemblyBadAccessOverride2");

		internal static string ERR_UseOfLocalBeforeDeclaration1 => GetResourceString("ERR_UseOfLocalBeforeDeclaration1");

		internal static string ERR_UseOfKeywordFromModule1 => GetResourceString("ERR_UseOfKeywordFromModule1");

		internal static string ERR_BogusWithinLineIf => GetResourceString("ERR_BogusWithinLineIf");

		internal static string ERR_CharToIntegralTypeMismatch1 => GetResourceString("ERR_CharToIntegralTypeMismatch1");

		internal static string ERR_IntegralToCharTypeMismatch1 => GetResourceString("ERR_IntegralToCharTypeMismatch1");

		internal static string ERR_NoDirectDelegateConstruction1 => GetResourceString("ERR_NoDirectDelegateConstruction1");

		internal static string ERR_MethodMustBeFirstStatementOnLine => GetResourceString("ERR_MethodMustBeFirstStatementOnLine");

		internal static string ERR_AttrAssignmentNotFieldOrProp1 => GetResourceString("ERR_AttrAssignmentNotFieldOrProp1");

		internal static string ERR_StrictDisallowsObjectComparison1 => GetResourceString("ERR_StrictDisallowsObjectComparison1");

		internal static string ERR_NoConstituentArraySizes => GetResourceString("ERR_NoConstituentArraySizes");

		internal static string ERR_FileAttributeNotAssemblyOrModule => GetResourceString("ERR_FileAttributeNotAssemblyOrModule");

		internal static string ERR_FunctionResultCannotBeIndexed1 => GetResourceString("ERR_FunctionResultCannotBeIndexed1");

		internal static string ERR_ArgumentSyntax => GetResourceString("ERR_ArgumentSyntax");

		internal static string ERR_ExpectedResumeOrGoto => GetResourceString("ERR_ExpectedResumeOrGoto");

		internal static string ERR_ExpectedAssignmentOperator => GetResourceString("ERR_ExpectedAssignmentOperator");

		internal static string ERR_NamedArgAlsoOmitted2 => GetResourceString("ERR_NamedArgAlsoOmitted2");

		internal static string ERR_CannotCallEvent1 => GetResourceString("ERR_CannotCallEvent1");

		internal static string ERR_ForEachCollectionDesignPattern1 => GetResourceString("ERR_ForEachCollectionDesignPattern1");

		internal static string ERR_DefaultValueForNonOptionalParam => GetResourceString("ERR_DefaultValueForNonOptionalParam");

		internal static string ERR_ExpectedDotAfterMyBase => GetResourceString("ERR_ExpectedDotAfterMyBase");

		internal static string ERR_ExpectedDotAfterMyClass => GetResourceString("ERR_ExpectedDotAfterMyClass");

		internal static string ERR_StrictArgumentCopyBackNarrowing3 => GetResourceString("ERR_StrictArgumentCopyBackNarrowing3");

		internal static string ERR_LbElseifAfterElse => GetResourceString("ERR_LbElseifAfterElse");

		internal static string ERR_StandaloneAttribute => GetResourceString("ERR_StandaloneAttribute");

		internal static string ERR_NoUniqueConstructorOnBase2 => GetResourceString("ERR_NoUniqueConstructorOnBase2");

		internal static string ERR_ExtraNextVariable => GetResourceString("ERR_ExtraNextVariable");

		internal static string ERR_RequiredNewCallTooMany2 => GetResourceString("ERR_RequiredNewCallTooMany2");

		internal static string ERR_ForCtlVarArraySizesSpecified => GetResourceString("ERR_ForCtlVarArraySizesSpecified");

		internal static string ERR_BadFlagsOnNewOverloads => GetResourceString("ERR_BadFlagsOnNewOverloads");

		internal static string ERR_TypeCharOnGenericParam => GetResourceString("ERR_TypeCharOnGenericParam");

		internal static string ERR_TooFewGenericArguments1 => GetResourceString("ERR_TooFewGenericArguments1");

		internal static string ERR_TooManyGenericArguments1 => GetResourceString("ERR_TooManyGenericArguments1");

		internal static string ERR_GenericConstraintNotSatisfied2 => GetResourceString("ERR_GenericConstraintNotSatisfied2");

		internal static string ERR_TypeOrMemberNotGeneric1 => GetResourceString("ERR_TypeOrMemberNotGeneric1");

		internal static string ERR_NewIfNullOnGenericParam => GetResourceString("ERR_NewIfNullOnGenericParam");

		internal static string ERR_MultipleClassConstraints1 => GetResourceString("ERR_MultipleClassConstraints1");

		internal static string ERR_ConstNotClassInterfaceOrTypeParam1 => GetResourceString("ERR_ConstNotClassInterfaceOrTypeParam1");

		internal static string ERR_DuplicateTypeParamName1 => GetResourceString("ERR_DuplicateTypeParamName1");

		internal static string ERR_UnboundTypeParam2 => GetResourceString("ERR_UnboundTypeParam2");

		internal static string ERR_IsOperatorGenericParam1 => GetResourceString("ERR_IsOperatorGenericParam1");

		internal static string ERR_ArgumentCopyBackNarrowing3 => GetResourceString("ERR_ArgumentCopyBackNarrowing3");

		internal static string ERR_ShadowingGenericParamWithMember1 => GetResourceString("ERR_ShadowingGenericParamWithMember1");

		internal static string ERR_GenericParamBase2 => GetResourceString("ERR_GenericParamBase2");

		internal static string ERR_ImplementsGenericParam => GetResourceString("ERR_ImplementsGenericParam");

		internal static string ERR_OnlyNullLowerBound => GetResourceString("ERR_OnlyNullLowerBound");

		internal static string ERR_ClassConstraintNotInheritable1 => GetResourceString("ERR_ClassConstraintNotInheritable1");

		internal static string ERR_ConstraintIsRestrictedType1 => GetResourceString("ERR_ConstraintIsRestrictedType1");

		internal static string ERR_GenericParamsOnInvalidMember => GetResourceString("ERR_GenericParamsOnInvalidMember");

		internal static string ERR_GenericArgsOnAttributeSpecifier => GetResourceString("ERR_GenericArgsOnAttributeSpecifier");

		internal static string ERR_AttrCannotBeGenerics => GetResourceString("ERR_AttrCannotBeGenerics");

		internal static string ERR_BadStaticLocalInGenericMethod => GetResourceString("ERR_BadStaticLocalInGenericMethod");

		internal static string ERR_SyntMemberShadowsGenericParam3 => GetResourceString("ERR_SyntMemberShadowsGenericParam3");

		internal static string ERR_ConstraintAlreadyExists1 => GetResourceString("ERR_ConstraintAlreadyExists1");

		internal static string ERR_InterfacePossiblyImplTwice2 => GetResourceString("ERR_InterfacePossiblyImplTwice2");

		internal static string ERR_ModulesCannotBeGeneric => GetResourceString("ERR_ModulesCannotBeGeneric");

		internal static string ERR_GenericClassCannotInheritAttr => GetResourceString("ERR_GenericClassCannotInheritAttr");

		internal static string ERR_DeclaresCantBeInGeneric => GetResourceString("ERR_DeclaresCantBeInGeneric");

		internal static string ERR_OverrideWithConstraintMismatch2 => GetResourceString("ERR_OverrideWithConstraintMismatch2");

		internal static string ERR_ImplementsWithConstraintMismatch3 => GetResourceString("ERR_ImplementsWithConstraintMismatch3");

		internal static string ERR_OpenTypeDisallowed => GetResourceString("ERR_OpenTypeDisallowed");

		internal static string ERR_HandlesInvalidOnGenericMethod => GetResourceString("ERR_HandlesInvalidOnGenericMethod");

		internal static string ERR_MultipleNewConstraints => GetResourceString("ERR_MultipleNewConstraints");

		internal static string ERR_MustInheritForNewConstraint2 => GetResourceString("ERR_MustInheritForNewConstraint2");

		internal static string ERR_NoSuitableNewForNewConstraint2 => GetResourceString("ERR_NoSuitableNewForNewConstraint2");

		internal static string ERR_BadGenericParamForNewConstraint2 => GetResourceString("ERR_BadGenericParamForNewConstraint2");

		internal static string ERR_NewArgsDisallowedForTypeParam => GetResourceString("ERR_NewArgsDisallowedForTypeParam");

		internal static string ERR_DuplicateRawGenericTypeImport1 => GetResourceString("ERR_DuplicateRawGenericTypeImport1");

		internal static string ERR_NoTypeArgumentCountOverloadCand1 => GetResourceString("ERR_NoTypeArgumentCountOverloadCand1");

		internal static string ERR_TypeArgsUnexpected => GetResourceString("ERR_TypeArgsUnexpected");

		internal static string ERR_NameSameAsMethodTypeParam1 => GetResourceString("ERR_NameSameAsMethodTypeParam1");

		internal static string ERR_TypeParamNameFunctionNameCollision => GetResourceString("ERR_TypeParamNameFunctionNameCollision");

		internal static string ERR_BadConstraintSyntax => GetResourceString("ERR_BadConstraintSyntax");

		internal static string ERR_OfExpected => GetResourceString("ERR_OfExpected");

		internal static string ERR_ArrayOfRawGenericInvalid => GetResourceString("ERR_ArrayOfRawGenericInvalid");

		internal static string ERR_ForEachAmbiguousIEnumerable1 => GetResourceString("ERR_ForEachAmbiguousIEnumerable1");

		internal static string ERR_IsNotOperatorGenericParam1 => GetResourceString("ERR_IsNotOperatorGenericParam1");

		internal static string ERR_TypeParamQualifierDisallowed => GetResourceString("ERR_TypeParamQualifierDisallowed");

		internal static string ERR_TypeParamMissingCommaOrRParen => GetResourceString("ERR_TypeParamMissingCommaOrRParen");

		internal static string ERR_TypeParamMissingAsCommaOrRParen => GetResourceString("ERR_TypeParamMissingAsCommaOrRParen");

		internal static string ERR_MultipleReferenceConstraints => GetResourceString("ERR_MultipleReferenceConstraints");

		internal static string ERR_MultipleValueConstraints => GetResourceString("ERR_MultipleValueConstraints");

		internal static string ERR_NewAndValueConstraintsCombined => GetResourceString("ERR_NewAndValueConstraintsCombined");

		internal static string ERR_RefAndValueConstraintsCombined => GetResourceString("ERR_RefAndValueConstraintsCombined");

		internal static string ERR_BadTypeArgForStructConstraint2 => GetResourceString("ERR_BadTypeArgForStructConstraint2");

		internal static string ERR_BadTypeArgForRefConstraint2 => GetResourceString("ERR_BadTypeArgForRefConstraint2");

		internal static string ERR_RefAndClassTypeConstrCombined => GetResourceString("ERR_RefAndClassTypeConstrCombined");

		internal static string ERR_ValueAndClassTypeConstrCombined => GetResourceString("ERR_ValueAndClassTypeConstrCombined");

		internal static string ERR_ConstraintClashIndirectIndirect4 => GetResourceString("ERR_ConstraintClashIndirectIndirect4");

		internal static string ERR_ConstraintClashDirectIndirect3 => GetResourceString("ERR_ConstraintClashDirectIndirect3");

		internal static string ERR_ConstraintClashIndirectDirect3 => GetResourceString("ERR_ConstraintClashIndirectDirect3");

		internal static string ERR_ConstraintCycleLink2 => GetResourceString("ERR_ConstraintCycleLink2");

		internal static string ERR_ConstraintCycle2 => GetResourceString("ERR_ConstraintCycle2");

		internal static string ERR_TypeParamWithStructConstAsConst => GetResourceString("ERR_TypeParamWithStructConstAsConst");

		internal static string ERR_NullableDisallowedForStructConstr1 => GetResourceString("ERR_NullableDisallowedForStructConstr1");

		internal static string ERR_ConflictingDirectConstraints3 => GetResourceString("ERR_ConflictingDirectConstraints3");

		internal static string ERR_InterfaceUnifiesWithInterface2 => GetResourceString("ERR_InterfaceUnifiesWithInterface2");

		internal static string ERR_BaseUnifiesWithInterfaces3 => GetResourceString("ERR_BaseUnifiesWithInterfaces3");

		internal static string ERR_InterfaceBaseUnifiesWithBase4 => GetResourceString("ERR_InterfaceBaseUnifiesWithBase4");

		internal static string ERR_InterfaceUnifiesWithBase3 => GetResourceString("ERR_InterfaceUnifiesWithBase3");

		internal static string ERR_ClassInheritsBaseUnifiesWithInterfaces3 => GetResourceString("ERR_ClassInheritsBaseUnifiesWithInterfaces3");

		internal static string ERR_ClassInheritsInterfaceBaseUnifiesWithBase4 => GetResourceString("ERR_ClassInheritsInterfaceBaseUnifiesWithBase4");

		internal static string ERR_ClassInheritsInterfaceUnifiesWithBase3 => GetResourceString("ERR_ClassInheritsInterfaceUnifiesWithBase3");

		internal static string ERR_OptionalsCantBeStructGenericParams => GetResourceString("ERR_OptionalsCantBeStructGenericParams");

		internal static string ERR_AddressOfNullableMethod => GetResourceString("ERR_AddressOfNullableMethod");

		internal static string ERR_IsOperatorNullable1 => GetResourceString("ERR_IsOperatorNullable1");

		internal static string ERR_IsNotOperatorNullable1 => GetResourceString("ERR_IsNotOperatorNullable1");

		internal static string ERR_ShadowingTypeOutsideClass1 => GetResourceString("ERR_ShadowingTypeOutsideClass1");

		internal static string ERR_PropertySetParamCollisionWithValue => GetResourceString("ERR_PropertySetParamCollisionWithValue");

		internal static string ERR_SxSIndirectRefHigherThanDirectRef3 => GetResourceString("ERR_SxSIndirectRefHigherThanDirectRef3");

		internal static string ERR_DuplicateReferenceStrong => GetResourceString("ERR_DuplicateReferenceStrong");

		internal static string ERR_DuplicateReference2 => GetResourceString("ERR_DuplicateReference2");

		internal static string ERR_IllegalCallOrIndex => GetResourceString("ERR_IllegalCallOrIndex");

		internal static string ERR_ConflictDefaultPropertyAttribute => GetResourceString("ERR_ConflictDefaultPropertyAttribute");

		internal static string ERR_BadAttributeUuid2 => GetResourceString("ERR_BadAttributeUuid2");

		internal static string ERR_ComClassAndReservedAttribute1 => GetResourceString("ERR_ComClassAndReservedAttribute1");

		internal static string ERR_ComClassRequiresPublicClass2 => GetResourceString("ERR_ComClassRequiresPublicClass2");

		internal static string ERR_ComClassReservedDispIdZero1 => GetResourceString("ERR_ComClassReservedDispIdZero1");

		internal static string ERR_ComClassReservedDispId1 => GetResourceString("ERR_ComClassReservedDispId1");

		internal static string ERR_ComClassDuplicateGuids1 => GetResourceString("ERR_ComClassDuplicateGuids1");

		internal static string ERR_ComClassCantBeAbstract0 => GetResourceString("ERR_ComClassCantBeAbstract0");

		internal static string ERR_ComClassRequiresPublicClass1 => GetResourceString("ERR_ComClassRequiresPublicClass1");

		internal static string ERR_UnknownOperator => GetResourceString("ERR_UnknownOperator");

		internal static string ERR_DuplicateConversionCategoryUsed => GetResourceString("ERR_DuplicateConversionCategoryUsed");

		internal static string ERR_OperatorNotOverloadable => GetResourceString("ERR_OperatorNotOverloadable");

		internal static string ERR_InvalidHandles => GetResourceString("ERR_InvalidHandles");

		internal static string ERR_InvalidImplements => GetResourceString("ERR_InvalidImplements");

		internal static string ERR_EndOperatorExpected => GetResourceString("ERR_EndOperatorExpected");

		internal static string ERR_EndOperatorNotAtLineStart => GetResourceString("ERR_EndOperatorNotAtLineStart");

		internal static string ERR_InvalidEndOperator => GetResourceString("ERR_InvalidEndOperator");

		internal static string ERR_ExitOperatorNotValid => GetResourceString("ERR_ExitOperatorNotValid");

		internal static string ERR_ParamArrayIllegal1 => GetResourceString("ERR_ParamArrayIllegal1");

		internal static string ERR_OptionalIllegal1 => GetResourceString("ERR_OptionalIllegal1");

		internal static string ERR_OperatorMustBePublic => GetResourceString("ERR_OperatorMustBePublic");

		internal static string ERR_OperatorMustBeShared => GetResourceString("ERR_OperatorMustBeShared");

		internal static string ERR_BadOperatorFlags1 => GetResourceString("ERR_BadOperatorFlags1");

		internal static string ERR_OneParameterRequired1 => GetResourceString("ERR_OneParameterRequired1");

		internal static string ERR_TwoParametersRequired1 => GetResourceString("ERR_TwoParametersRequired1");

		internal static string ERR_OneOrTwoParametersRequired1 => GetResourceString("ERR_OneOrTwoParametersRequired1");

		internal static string ERR_ConvMustBeWideningOrNarrowing => GetResourceString("ERR_ConvMustBeWideningOrNarrowing");

		internal static string ERR_OperatorDeclaredInModule => GetResourceString("ERR_OperatorDeclaredInModule");

		internal static string ERR_InvalidSpecifierOnNonConversion1 => GetResourceString("ERR_InvalidSpecifierOnNonConversion1");

		internal static string ERR_UnaryParamMustBeContainingType1 => GetResourceString("ERR_UnaryParamMustBeContainingType1");

		internal static string ERR_BinaryParamMustBeContainingType1 => GetResourceString("ERR_BinaryParamMustBeContainingType1");

		internal static string ERR_ConvParamMustBeContainingType1 => GetResourceString("ERR_ConvParamMustBeContainingType1");

		internal static string ERR_OperatorRequiresBoolReturnType1 => GetResourceString("ERR_OperatorRequiresBoolReturnType1");

		internal static string ERR_ConversionToSameType => GetResourceString("ERR_ConversionToSameType");

		internal static string ERR_ConversionToInterfaceType => GetResourceString("ERR_ConversionToInterfaceType");

		internal static string ERR_ConversionToBaseType => GetResourceString("ERR_ConversionToBaseType");

		internal static string ERR_ConversionToDerivedType => GetResourceString("ERR_ConversionToDerivedType");

		internal static string ERR_ConversionToObject => GetResourceString("ERR_ConversionToObject");

		internal static string ERR_ConversionFromInterfaceType => GetResourceString("ERR_ConversionFromInterfaceType");

		internal static string ERR_ConversionFromBaseType => GetResourceString("ERR_ConversionFromBaseType");

		internal static string ERR_ConversionFromDerivedType => GetResourceString("ERR_ConversionFromDerivedType");

		internal static string ERR_ConversionFromObject => GetResourceString("ERR_ConversionFromObject");

		internal static string ERR_MatchingOperatorExpected2 => GetResourceString("ERR_MatchingOperatorExpected2");

		internal static string ERR_UnacceptableLogicalOperator3 => GetResourceString("ERR_UnacceptableLogicalOperator3");

		internal static string ERR_ConditionOperatorRequired3 => GetResourceString("ERR_ConditionOperatorRequired3");

		internal static string ERR_CopyBackTypeMismatch3 => GetResourceString("ERR_CopyBackTypeMismatch3");

		internal static string ERR_ForLoopOperatorRequired2 => GetResourceString("ERR_ForLoopOperatorRequired2");

		internal static string ERR_UnacceptableForLoopOperator2 => GetResourceString("ERR_UnacceptableForLoopOperator2");

		internal static string ERR_UnacceptableForLoopRelOperator2 => GetResourceString("ERR_UnacceptableForLoopRelOperator2");

		internal static string ERR_OperatorRequiresIntegerParameter1 => GetResourceString("ERR_OperatorRequiresIntegerParameter1");

		internal static string ERR_CantSpecifyNullableOnBoth => GetResourceString("ERR_CantSpecifyNullableOnBoth");

		internal static string ERR_BadTypeArgForStructConstraintNull => GetResourceString("ERR_BadTypeArgForStructConstraintNull");

		internal static string ERR_CantSpecifyArrayAndNullableOnBoth => GetResourceString("ERR_CantSpecifyArrayAndNullableOnBoth");

		internal static string ERR_CantSpecifyTypeCharacterOnIIF => GetResourceString("ERR_CantSpecifyTypeCharacterOnIIF");

		internal static string ERR_IllegalOperandInIIFName => GetResourceString("ERR_IllegalOperandInIIFName");

		internal static string ERR_IllegalOperandInIIFConversion => GetResourceString("ERR_IllegalOperandInIIFConversion");

		internal static string ERR_IllegalCondTypeInIIF => GetResourceString("ERR_IllegalCondTypeInIIF");

		internal static string ERR_CantCallIIF => GetResourceString("ERR_CantCallIIF");

		internal static string ERR_CantSpecifyAsNewAndNullable => GetResourceString("ERR_CantSpecifyAsNewAndNullable");

		internal static string ERR_IllegalOperandInIIFConversion2 => GetResourceString("ERR_IllegalOperandInIIFConversion2");

		internal static string ERR_BadNullTypeInCCExpression => GetResourceString("ERR_BadNullTypeInCCExpression");

		internal static string ERR_NullableImplicit => GetResourceString("ERR_NullableImplicit");

		internal static string ERR_MissingRuntimeHelper => GetResourceString("ERR_MissingRuntimeHelper");

		internal static string ERR_ExpectedDotAfterGlobalNameSpace => GetResourceString("ERR_ExpectedDotAfterGlobalNameSpace");

		internal static string ERR_NoGlobalExpectedIdentifier => GetResourceString("ERR_NoGlobalExpectedIdentifier");

		internal static string ERR_NoGlobalInHandles => GetResourceString("ERR_NoGlobalInHandles");

		internal static string ERR_ElseIfNoMatchingIf => GetResourceString("ERR_ElseIfNoMatchingIf");

		internal static string ERR_BadAttributeConstructor2 => GetResourceString("ERR_BadAttributeConstructor2");

		internal static string ERR_EndUsingWithoutUsing => GetResourceString("ERR_EndUsingWithoutUsing");

		internal static string ERR_ExpectedEndUsing => GetResourceString("ERR_ExpectedEndUsing");

		internal static string ERR_GotoIntoUsing => GetResourceString("ERR_GotoIntoUsing");

		internal static string ERR_UsingRequiresDisposePattern => GetResourceString("ERR_UsingRequiresDisposePattern");

		internal static string ERR_UsingResourceVarNeedsInitializer => GetResourceString("ERR_UsingResourceVarNeedsInitializer");

		internal static string ERR_UsingResourceVarCantBeArray => GetResourceString("ERR_UsingResourceVarCantBeArray");

		internal static string ERR_OnErrorInUsing => GetResourceString("ERR_OnErrorInUsing");

		internal static string ERR_PropertyNameConflictInMyCollection => GetResourceString("ERR_PropertyNameConflictInMyCollection");

		internal static string ERR_InvalidImplicitVar => GetResourceString("ERR_InvalidImplicitVar");

		internal static string ERR_ObjectInitializerRequiresFieldName => GetResourceString("ERR_ObjectInitializerRequiresFieldName");

		internal static string ERR_ExpectedFrom => GetResourceString("ERR_ExpectedFrom");

		internal static string ERR_LambdaBindingMismatch1 => GetResourceString("ERR_LambdaBindingMismatch1");

		internal static string ERR_LambdaBindingMismatch2 => GetResourceString("ERR_LambdaBindingMismatch2");

		internal static string ERR_CannotLiftByRefParamQuery1 => GetResourceString("ERR_CannotLiftByRefParamQuery1");

		internal static string ERR_ExpressionTreeNotSupported => GetResourceString("ERR_ExpressionTreeNotSupported");

		internal static string ERR_CannotLiftStructureMeQuery => GetResourceString("ERR_CannotLiftStructureMeQuery");

		internal static string ERR_InferringNonArrayType1 => GetResourceString("ERR_InferringNonArrayType1");

		internal static string ERR_ByRefParamInExpressionTree => GetResourceString("ERR_ByRefParamInExpressionTree");

		internal static string ERR_DuplicateAnonTypeMemberName1 => GetResourceString("ERR_DuplicateAnonTypeMemberName1");

		internal static string ERR_BadAnonymousTypeForExprTree => GetResourceString("ERR_BadAnonymousTypeForExprTree");

		internal static string ERR_CannotLiftAnonymousType1 => GetResourceString("ERR_CannotLiftAnonymousType1");

		internal static string ERR_ExtensionOnlyAllowedOnModuleSubOrFunction => GetResourceString("ERR_ExtensionOnlyAllowedOnModuleSubOrFunction");

		internal static string ERR_ExtensionMethodNotInModule => GetResourceString("ERR_ExtensionMethodNotInModule");

		internal static string ERR_ExtensionMethodNoParams => GetResourceString("ERR_ExtensionMethodNoParams");

		internal static string ERR_ExtensionMethodOptionalFirstArg => GetResourceString("ERR_ExtensionMethodOptionalFirstArg");

		internal static string ERR_ExtensionMethodParamArrayFirstArg => GetResourceString("ERR_ExtensionMethodParamArrayFirstArg");

		internal static string ERR_AnonymousTypeFieldNameInference => GetResourceString("ERR_AnonymousTypeFieldNameInference");

		internal static string ERR_NameNotMemberOfAnonymousType2 => GetResourceString("ERR_NameNotMemberOfAnonymousType2");

		internal static string ERR_ExtensionAttributeInvalid => GetResourceString("ERR_ExtensionAttributeInvalid");

		internal static string ERR_AnonymousTypePropertyOutOfOrder1 => GetResourceString("ERR_AnonymousTypePropertyOutOfOrder1");

		internal static string ERR_AnonymousTypeDisallowsTypeChar => GetResourceString("ERR_AnonymousTypeDisallowsTypeChar");

		internal static string ERR_TupleLiteralDisallowsTypeChar => GetResourceString("ERR_TupleLiteralDisallowsTypeChar");

		internal static string ERR_NewWithTupleTypeSyntax => GetResourceString("ERR_NewWithTupleTypeSyntax");

		internal static string ERR_PredefinedValueTupleTypeMustBeStruct => GetResourceString("ERR_PredefinedValueTupleTypeMustBeStruct");

		internal static string ERR_ExtensionMethodUncallable1 => GetResourceString("ERR_ExtensionMethodUncallable1");

		internal static string ERR_ExtensionMethodOverloadCandidate3 => GetResourceString("ERR_ExtensionMethodOverloadCandidate3");

		internal static string ERR_DelegateBindingMismatch => GetResourceString("ERR_DelegateBindingMismatch");

		internal static string ERR_DelegateBindingTypeInferenceFails => GetResourceString("ERR_DelegateBindingTypeInferenceFails");

		internal static string ERR_TooManyArgs => GetResourceString("ERR_TooManyArgs");

		internal static string ERR_NamedArgAlsoOmitted1 => GetResourceString("ERR_NamedArgAlsoOmitted1");

		internal static string ERR_NamedArgUsedTwice1 => GetResourceString("ERR_NamedArgUsedTwice1");

		internal static string ERR_NamedParamNotFound1 => GetResourceString("ERR_NamedParamNotFound1");

		internal static string ERR_OmittedArgument1 => GetResourceString("ERR_OmittedArgument1");

		internal static string ERR_UnboundTypeParam1 => GetResourceString("ERR_UnboundTypeParam1");

		internal static string ERR_ExtensionMethodOverloadCandidate2 => GetResourceString("ERR_ExtensionMethodOverloadCandidate2");

		internal static string ERR_AnonymousTypeNeedField => GetResourceString("ERR_AnonymousTypeNeedField");

		internal static string ERR_AnonymousTypeNameWithoutPeriod => GetResourceString("ERR_AnonymousTypeNameWithoutPeriod");

		internal static string ERR_AnonymousTypeExpectedIdentifier => GetResourceString("ERR_AnonymousTypeExpectedIdentifier");

		internal static string ERR_TooManyArgs2 => GetResourceString("ERR_TooManyArgs2");

		internal static string ERR_NamedArgAlsoOmitted3 => GetResourceString("ERR_NamedArgAlsoOmitted3");

		internal static string ERR_NamedArgUsedTwice3 => GetResourceString("ERR_NamedArgUsedTwice3");

		internal static string ERR_NamedParamNotFound3 => GetResourceString("ERR_NamedParamNotFound3");

		internal static string ERR_OmittedArgument3 => GetResourceString("ERR_OmittedArgument3");

		internal static string ERR_UnboundTypeParam3 => GetResourceString("ERR_UnboundTypeParam3");

		internal static string ERR_TooFewGenericArguments2 => GetResourceString("ERR_TooFewGenericArguments2");

		internal static string ERR_TooManyGenericArguments2 => GetResourceString("ERR_TooManyGenericArguments2");

		internal static string ERR_ExpectedInOrEq => GetResourceString("ERR_ExpectedInOrEq");

		internal static string ERR_ExpectedQueryableSource => GetResourceString("ERR_ExpectedQueryableSource");

		internal static string ERR_QueryOperatorNotFound => GetResourceString("ERR_QueryOperatorNotFound");

		internal static string ERR_CannotUseOnErrorGotoWithClosure => GetResourceString("ERR_CannotUseOnErrorGotoWithClosure");

		internal static string ERR_CannotGotoNonScopeBlocksWithClosure => GetResourceString("ERR_CannotGotoNonScopeBlocksWithClosure");

		internal static string ERR_CannotLiftRestrictedTypeQuery => GetResourceString("ERR_CannotLiftRestrictedTypeQuery");

		internal static string ERR_QueryAnonymousTypeFieldNameInference => GetResourceString("ERR_QueryAnonymousTypeFieldNameInference");

		internal static string ERR_QueryDuplicateAnonTypeMemberName1 => GetResourceString("ERR_QueryDuplicateAnonTypeMemberName1");

		internal static string ERR_QueryAnonymousTypeDisallowsTypeChar => GetResourceString("ERR_QueryAnonymousTypeDisallowsTypeChar");

		internal static string ERR_ReadOnlyInClosure => GetResourceString("ERR_ReadOnlyInClosure");

		internal static string ERR_ExprTreeNoMultiDimArrayCreation => GetResourceString("ERR_ExprTreeNoMultiDimArrayCreation");

		internal static string ERR_ExprTreeNoLateBind => GetResourceString("ERR_ExprTreeNoLateBind");

		internal static string ERR_ExpectedBy => GetResourceString("ERR_ExpectedBy");

		internal static string ERR_QueryInvalidControlVariableName1 => GetResourceString("ERR_QueryInvalidControlVariableName1");

		internal static string ERR_ExpectedIn => GetResourceString("ERR_ExpectedIn");

		internal static string ERR_QueryNameNotDeclared => GetResourceString("ERR_QueryNameNotDeclared");

		internal static string ERR_NestedFunctionArgumentNarrowing3 => GetResourceString("ERR_NestedFunctionArgumentNarrowing3");

		internal static string ERR_AnonTypeFieldXMLNameInference => GetResourceString("ERR_AnonTypeFieldXMLNameInference");

		internal static string ERR_QueryAnonTypeFieldXMLNameInference => GetResourceString("ERR_QueryAnonTypeFieldXMLNameInference");

		internal static string ERR_ExpectedInto => GetResourceString("ERR_ExpectedInto");

		internal static string ERR_TypeCharOnAggregation => GetResourceString("ERR_TypeCharOnAggregation");

		internal static string ERR_ExpectedOn => GetResourceString("ERR_ExpectedOn");

		internal static string ERR_ExpectedEquals => GetResourceString("ERR_ExpectedEquals");

		internal static string ERR_ExpectedAnd => GetResourceString("ERR_ExpectedAnd");

		internal static string ERR_EqualsTypeMismatch => GetResourceString("ERR_EqualsTypeMismatch");

		internal static string ERR_EqualsOperandIsBad => GetResourceString("ERR_EqualsOperandIsBad");

		internal static string ERR_LambdaNotDelegate1 => GetResourceString("ERR_LambdaNotDelegate1");

		internal static string ERR_LambdaNotCreatableDelegate1 => GetResourceString("ERR_LambdaNotCreatableDelegate1");

		internal static string ERR_CannotInferNullableForVariable1 => GetResourceString("ERR_CannotInferNullableForVariable1");

		internal static string ERR_NullableTypeInferenceNotSupported => GetResourceString("ERR_NullableTypeInferenceNotSupported");

		internal static string ERR_ExpectedJoin => GetResourceString("ERR_ExpectedJoin");

		internal static string ERR_NullableParameterMustSpecifyType => GetResourceString("ERR_NullableParameterMustSpecifyType");

		internal static string ERR_IterationVariableShadowLocal2 => GetResourceString("ERR_IterationVariableShadowLocal2");

		internal static string ERR_LambdasCannotHaveAttributes => GetResourceString("ERR_LambdasCannotHaveAttributes");

		internal static string ERR_LambdaInSelectCaseExpr => GetResourceString("ERR_LambdaInSelectCaseExpr");

		internal static string ERR_AddressOfInSelectCaseExpr => GetResourceString("ERR_AddressOfInSelectCaseExpr");

		internal static string ERR_NullableCharNotSupported => GetResourceString("ERR_NullableCharNotSupported");

		internal static string ERR_CannotLiftStructureMeLambda => GetResourceString("ERR_CannotLiftStructureMeLambda");

		internal static string ERR_CannotLiftByRefParamLambda1 => GetResourceString("ERR_CannotLiftByRefParamLambda1");

		internal static string ERR_CannotLiftRestrictedTypeLambda => GetResourceString("ERR_CannotLiftRestrictedTypeLambda");

		internal static string ERR_LambdaParamShadowLocal1 => GetResourceString("ERR_LambdaParamShadowLocal1");

		internal static string ERR_StrictDisallowImplicitObjectLambda => GetResourceString("ERR_StrictDisallowImplicitObjectLambda");

		internal static string ERR_CantSpecifyParamsOnLambdaParamNoType => GetResourceString("ERR_CantSpecifyParamsOnLambdaParamNoType");

		internal static string ERR_TypeInferenceFailure1 => GetResourceString("ERR_TypeInferenceFailure1");

		internal static string ERR_TypeInferenceFailure2 => GetResourceString("ERR_TypeInferenceFailure2");

		internal static string ERR_TypeInferenceFailure3 => GetResourceString("ERR_TypeInferenceFailure3");

		internal static string ERR_TypeInferenceFailureNoExplicit1 => GetResourceString("ERR_TypeInferenceFailureNoExplicit1");

		internal static string ERR_TypeInferenceFailureNoExplicit2 => GetResourceString("ERR_TypeInferenceFailureNoExplicit2");

		internal static string ERR_TypeInferenceFailureNoExplicit3 => GetResourceString("ERR_TypeInferenceFailureNoExplicit3");

		internal static string ERR_TypeInferenceFailureAmbiguous1 => GetResourceString("ERR_TypeInferenceFailureAmbiguous1");

		internal static string ERR_TypeInferenceFailureAmbiguous2 => GetResourceString("ERR_TypeInferenceFailureAmbiguous2");

		internal static string ERR_TypeInferenceFailureAmbiguous3 => GetResourceString("ERR_TypeInferenceFailureAmbiguous3");

		internal static string ERR_TypeInferenceFailureNoExplicitAmbiguous1 => GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous1");

		internal static string ERR_TypeInferenceFailureNoExplicitAmbiguous2 => GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous2");

		internal static string ERR_TypeInferenceFailureNoExplicitAmbiguous3 => GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous3");

		internal static string ERR_TypeInferenceFailureNoBest1 => GetResourceString("ERR_TypeInferenceFailureNoBest1");

		internal static string ERR_TypeInferenceFailureNoBest2 => GetResourceString("ERR_TypeInferenceFailureNoBest2");

		internal static string ERR_TypeInferenceFailureNoBest3 => GetResourceString("ERR_TypeInferenceFailureNoBest3");

		internal static string ERR_TypeInferenceFailureNoExplicitNoBest1 => GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest1");

		internal static string ERR_TypeInferenceFailureNoExplicitNoBest2 => GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest2");

		internal static string ERR_TypeInferenceFailureNoExplicitNoBest3 => GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest3");

		internal static string ERR_DelegateBindingMismatchStrictOff2 => GetResourceString("ERR_DelegateBindingMismatchStrictOff2");

		internal static string ERR_InaccessibleReturnTypeOfMember2 => GetResourceString("ERR_InaccessibleReturnTypeOfMember2");

		internal static string ERR_ExpectedIdentifierOrGroup => GetResourceString("ERR_ExpectedIdentifierOrGroup");

		internal static string ERR_UnexpectedGroup => GetResourceString("ERR_UnexpectedGroup");

		internal static string ERR_DelegateBindingMismatchStrictOff3 => GetResourceString("ERR_DelegateBindingMismatchStrictOff3");

		internal static string ERR_DelegateBindingIncompatible3 => GetResourceString("ERR_DelegateBindingIncompatible3");

		internal static string ERR_ArgumentNarrowing2 => GetResourceString("ERR_ArgumentNarrowing2");

		internal static string ERR_OverloadCandidate1 => GetResourceString("ERR_OverloadCandidate1");

		internal static string ERR_AutoPropertyInitializedInStructure => GetResourceString("ERR_AutoPropertyInitializedInStructure");

		internal static string ERR_TypeDisallowsElements => GetResourceString("ERR_TypeDisallowsElements");

		internal static string ERR_TypeDisallowsAttributes => GetResourceString("ERR_TypeDisallowsAttributes");

		internal static string ERR_TypeDisallowsDescendants => GetResourceString("ERR_TypeDisallowsDescendants");

		internal static string ERR_TypeOrMemberNotGeneric2 => GetResourceString("ERR_TypeOrMemberNotGeneric2");

		internal static string ERR_ExtensionMethodCannotBeLateBound => GetResourceString("ERR_ExtensionMethodCannotBeLateBound");

		internal static string ERR_TypeInferenceArrayRankMismatch1 => GetResourceString("ERR_TypeInferenceArrayRankMismatch1");

		internal static string ERR_QueryStrictDisallowImplicitObject => GetResourceString("ERR_QueryStrictDisallowImplicitObject");

		internal static string ERR_CannotEmbedInterfaceWithGeneric => GetResourceString("ERR_CannotEmbedInterfaceWithGeneric");

		internal static string ERR_CannotUseGenericTypeAcrossAssemblyBoundaries => GetResourceString("ERR_CannotUseGenericTypeAcrossAssemblyBoundaries");

		internal static string WRN_UseOfObsoleteSymbol2 => GetResourceString("WRN_UseOfObsoleteSymbol2");

		internal static string WRN_UseOfObsoleteSymbol2_Title => GetResourceString("WRN_UseOfObsoleteSymbol2_Title");

		internal static string WRN_MustOverloadBase4 => GetResourceString("WRN_MustOverloadBase4");

		internal static string WRN_MustOverloadBase4_Title => GetResourceString("WRN_MustOverloadBase4_Title");

		internal static string WRN_OverrideType5 => GetResourceString("WRN_OverrideType5");

		internal static string WRN_OverrideType5_Title => GetResourceString("WRN_OverrideType5_Title");

		internal static string WRN_MustOverride2 => GetResourceString("WRN_MustOverride2");

		internal static string WRN_MustOverride2_Title => GetResourceString("WRN_MustOverride2_Title");

		internal static string WRN_DefaultnessShadowed4 => GetResourceString("WRN_DefaultnessShadowed4");

		internal static string WRN_DefaultnessShadowed4_Title => GetResourceString("WRN_DefaultnessShadowed4_Title");

		internal static string WRN_UseOfObsoleteSymbolNoMessage1 => GetResourceString("WRN_UseOfObsoleteSymbolNoMessage1");

		internal static string WRN_UseOfObsoleteSymbolNoMessage1_Title => GetResourceString("WRN_UseOfObsoleteSymbolNoMessage1_Title");

		internal static string WRN_AssemblyGeneration0 => GetResourceString("WRN_AssemblyGeneration0");

		internal static string WRN_AssemblyGeneration0_Title => GetResourceString("WRN_AssemblyGeneration0_Title");

		internal static string WRN_AssemblyGeneration1 => GetResourceString("WRN_AssemblyGeneration1");

		internal static string WRN_AssemblyGeneration1_Title => GetResourceString("WRN_AssemblyGeneration1_Title");

		internal static string WRN_ComClassNoMembers1 => GetResourceString("WRN_ComClassNoMembers1");

		internal static string WRN_ComClassNoMembers1_Title => GetResourceString("WRN_ComClassNoMembers1_Title");

		internal static string WRN_SynthMemberShadowsMember5 => GetResourceString("WRN_SynthMemberShadowsMember5");

		internal static string WRN_SynthMemberShadowsMember5_Title => GetResourceString("WRN_SynthMemberShadowsMember5_Title");

		internal static string WRN_MemberShadowsSynthMember6 => GetResourceString("WRN_MemberShadowsSynthMember6");

		internal static string WRN_MemberShadowsSynthMember6_Title => GetResourceString("WRN_MemberShadowsSynthMember6_Title");

		internal static string WRN_SynthMemberShadowsSynthMember7 => GetResourceString("WRN_SynthMemberShadowsSynthMember7");

		internal static string WRN_SynthMemberShadowsSynthMember7_Title => GetResourceString("WRN_SynthMemberShadowsSynthMember7_Title");

		internal static string WRN_UseOfObsoletePropertyAccessor3 => GetResourceString("WRN_UseOfObsoletePropertyAccessor3");

		internal static string WRN_UseOfObsoletePropertyAccessor3_Title => GetResourceString("WRN_UseOfObsoletePropertyAccessor3_Title");

		internal static string WRN_UseOfObsoletePropertyAccessor2 => GetResourceString("WRN_UseOfObsoletePropertyAccessor2");

		internal static string WRN_UseOfObsoletePropertyAccessor2_Title => GetResourceString("WRN_UseOfObsoletePropertyAccessor2_Title");

		internal static string WRN_FieldNotCLSCompliant1 => GetResourceString("WRN_FieldNotCLSCompliant1");

		internal static string WRN_FieldNotCLSCompliant1_Title => GetResourceString("WRN_FieldNotCLSCompliant1_Title");

		internal static string WRN_BaseClassNotCLSCompliant2 => GetResourceString("WRN_BaseClassNotCLSCompliant2");

		internal static string WRN_BaseClassNotCLSCompliant2_Title => GetResourceString("WRN_BaseClassNotCLSCompliant2_Title");

		internal static string WRN_ProcTypeNotCLSCompliant1 => GetResourceString("WRN_ProcTypeNotCLSCompliant1");

		internal static string WRN_ProcTypeNotCLSCompliant1_Title => GetResourceString("WRN_ProcTypeNotCLSCompliant1_Title");

		internal static string WRN_ParamNotCLSCompliant1 => GetResourceString("WRN_ParamNotCLSCompliant1");

		internal static string WRN_ParamNotCLSCompliant1_Title => GetResourceString("WRN_ParamNotCLSCompliant1_Title");

		internal static string WRN_InheritedInterfaceNotCLSCompliant2 => GetResourceString("WRN_InheritedInterfaceNotCLSCompliant2");

		internal static string WRN_InheritedInterfaceNotCLSCompliant2_Title => GetResourceString("WRN_InheritedInterfaceNotCLSCompliant2_Title");

		internal static string WRN_CLSMemberInNonCLSType3 => GetResourceString("WRN_CLSMemberInNonCLSType3");

		internal static string WRN_CLSMemberInNonCLSType3_Title => GetResourceString("WRN_CLSMemberInNonCLSType3_Title");

		internal static string WRN_NameNotCLSCompliant1 => GetResourceString("WRN_NameNotCLSCompliant1");

		internal static string WRN_NameNotCLSCompliant1_Title => GetResourceString("WRN_NameNotCLSCompliant1_Title");

		internal static string WRN_EnumUnderlyingTypeNotCLS1 => GetResourceString("WRN_EnumUnderlyingTypeNotCLS1");

		internal static string WRN_EnumUnderlyingTypeNotCLS1_Title => GetResourceString("WRN_EnumUnderlyingTypeNotCLS1_Title");

		internal static string WRN_NonCLSMemberInCLSInterface1 => GetResourceString("WRN_NonCLSMemberInCLSInterface1");

		internal static string WRN_NonCLSMemberInCLSInterface1_Title => GetResourceString("WRN_NonCLSMemberInCLSInterface1_Title");

		internal static string WRN_NonCLSMustOverrideInCLSType1 => GetResourceString("WRN_NonCLSMustOverrideInCLSType1");

		internal static string WRN_NonCLSMustOverrideInCLSType1_Title => GetResourceString("WRN_NonCLSMustOverrideInCLSType1_Title");

		internal static string WRN_ArrayOverloadsNonCLS2 => GetResourceString("WRN_ArrayOverloadsNonCLS2");

		internal static string WRN_ArrayOverloadsNonCLS2_Title => GetResourceString("WRN_ArrayOverloadsNonCLS2_Title");

		internal static string WRN_RootNamespaceNotCLSCompliant1 => GetResourceString("WRN_RootNamespaceNotCLSCompliant1");

		internal static string WRN_RootNamespaceNotCLSCompliant1_Title => GetResourceString("WRN_RootNamespaceNotCLSCompliant1_Title");

		internal static string WRN_RootNamespaceNotCLSCompliant2 => GetResourceString("WRN_RootNamespaceNotCLSCompliant2");

		internal static string WRN_RootNamespaceNotCLSCompliant2_Title => GetResourceString("WRN_RootNamespaceNotCLSCompliant2_Title");

		internal static string WRN_GenericConstraintNotCLSCompliant1 => GetResourceString("WRN_GenericConstraintNotCLSCompliant1");

		internal static string WRN_GenericConstraintNotCLSCompliant1_Title => GetResourceString("WRN_GenericConstraintNotCLSCompliant1_Title");

		internal static string WRN_TypeNotCLSCompliant1 => GetResourceString("WRN_TypeNotCLSCompliant1");

		internal static string WRN_TypeNotCLSCompliant1_Title => GetResourceString("WRN_TypeNotCLSCompliant1_Title");

		internal static string WRN_OptionalValueNotCLSCompliant1 => GetResourceString("WRN_OptionalValueNotCLSCompliant1");

		internal static string WRN_OptionalValueNotCLSCompliant1_Title => GetResourceString("WRN_OptionalValueNotCLSCompliant1_Title");

		internal static string WRN_CLSAttrInvalidOnGetSet => GetResourceString("WRN_CLSAttrInvalidOnGetSet");

		internal static string WRN_CLSAttrInvalidOnGetSet_Title => GetResourceString("WRN_CLSAttrInvalidOnGetSet_Title");

		internal static string WRN_TypeConflictButMerged6 => GetResourceString("WRN_TypeConflictButMerged6");

		internal static string WRN_TypeConflictButMerged6_Title => GetResourceString("WRN_TypeConflictButMerged6_Title");

		internal static string WRN_ShadowingGenericParamWithParam1 => GetResourceString("WRN_ShadowingGenericParamWithParam1");

		internal static string WRN_ShadowingGenericParamWithParam1_Title => GetResourceString("WRN_ShadowingGenericParamWithParam1_Title");

		internal static string WRN_CannotFindStandardLibrary1 => GetResourceString("WRN_CannotFindStandardLibrary1");

		internal static string WRN_CannotFindStandardLibrary1_Title => GetResourceString("WRN_CannotFindStandardLibrary1_Title");

		internal static string WRN_EventDelegateTypeNotCLSCompliant2 => GetResourceString("WRN_EventDelegateTypeNotCLSCompliant2");

		internal static string WRN_EventDelegateTypeNotCLSCompliant2_Title => GetResourceString("WRN_EventDelegateTypeNotCLSCompliant2_Title");

		internal static string WRN_DebuggerHiddenIgnoredOnProperties => GetResourceString("WRN_DebuggerHiddenIgnoredOnProperties");

		internal static string WRN_DebuggerHiddenIgnoredOnProperties_Title => GetResourceString("WRN_DebuggerHiddenIgnoredOnProperties_Title");

		internal static string WRN_SelectCaseInvalidRange => GetResourceString("WRN_SelectCaseInvalidRange");

		internal static string WRN_SelectCaseInvalidRange_Title => GetResourceString("WRN_SelectCaseInvalidRange_Title");

		internal static string WRN_CLSEventMethodInNonCLSType3 => GetResourceString("WRN_CLSEventMethodInNonCLSType3");

		internal static string WRN_CLSEventMethodInNonCLSType3_Title => GetResourceString("WRN_CLSEventMethodInNonCLSType3_Title");

		internal static string WRN_ExpectedInitComponentCall2 => GetResourceString("WRN_ExpectedInitComponentCall2");

		internal static string WRN_ExpectedInitComponentCall2_Title => GetResourceString("WRN_ExpectedInitComponentCall2_Title");

		internal static string WRN_NamespaceCaseMismatch3 => GetResourceString("WRN_NamespaceCaseMismatch3");

		internal static string WRN_NamespaceCaseMismatch3_Title => GetResourceString("WRN_NamespaceCaseMismatch3_Title");

		internal static string WRN_UndefinedOrEmptyNamespaceOrClass1 => GetResourceString("WRN_UndefinedOrEmptyNamespaceOrClass1");

		internal static string WRN_UndefinedOrEmptyNamespaceOrClass1_Title => GetResourceString("WRN_UndefinedOrEmptyNamespaceOrClass1_Title");

		internal static string WRN_UndefinedOrEmptyProjectNamespaceOrClass1 => GetResourceString("WRN_UndefinedOrEmptyProjectNamespaceOrClass1");

		internal static string WRN_UndefinedOrEmptyProjectNamespaceOrClass1_Title => GetResourceString("WRN_UndefinedOrEmptyProjectNamespaceOrClass1_Title");

		internal static string WRN_IndirectRefToLinkedAssembly2 => GetResourceString("WRN_IndirectRefToLinkedAssembly2");

		internal static string WRN_IndirectRefToLinkedAssembly2_Title => GetResourceString("WRN_IndirectRefToLinkedAssembly2_Title");

		internal static string WRN_NoNonObsoleteConstructorOnBase3 => GetResourceString("WRN_NoNonObsoleteConstructorOnBase3");

		internal static string WRN_NoNonObsoleteConstructorOnBase3_Title => GetResourceString("WRN_NoNonObsoleteConstructorOnBase3_Title");

		internal static string WRN_NoNonObsoleteConstructorOnBase4 => GetResourceString("WRN_NoNonObsoleteConstructorOnBase4");

		internal static string WRN_NoNonObsoleteConstructorOnBase4_Title => GetResourceString("WRN_NoNonObsoleteConstructorOnBase4_Title");

		internal static string WRN_RequiredNonObsoleteNewCall3 => GetResourceString("WRN_RequiredNonObsoleteNewCall3");

		internal static string WRN_RequiredNonObsoleteNewCall3_Title => GetResourceString("WRN_RequiredNonObsoleteNewCall3_Title");

		internal static string WRN_RequiredNonObsoleteNewCall4 => GetResourceString("WRN_RequiredNonObsoleteNewCall4");

		internal static string WRN_RequiredNonObsoleteNewCall4_Title => GetResourceString("WRN_RequiredNonObsoleteNewCall4_Title");

		internal static string WRN_MissingAsClauseinOperator => GetResourceString("WRN_MissingAsClauseinOperator");

		internal static string WRN_MissingAsClauseinOperator_Title => GetResourceString("WRN_MissingAsClauseinOperator_Title");

		internal static string WRN_ConstraintsFailedForInferredArgs2 => GetResourceString("WRN_ConstraintsFailedForInferredArgs2");

		internal static string WRN_ConstraintsFailedForInferredArgs2_Title => GetResourceString("WRN_ConstraintsFailedForInferredArgs2_Title");

		internal static string WRN_ConditionalNotValidOnFunction => GetResourceString("WRN_ConditionalNotValidOnFunction");

		internal static string WRN_ConditionalNotValidOnFunction_Title => GetResourceString("WRN_ConditionalNotValidOnFunction_Title");

		internal static string WRN_UseSwitchInsteadOfAttribute => GetResourceString("WRN_UseSwitchInsteadOfAttribute");

		internal static string WRN_UseSwitchInsteadOfAttribute_Title => GetResourceString("WRN_UseSwitchInsteadOfAttribute_Title");

		internal static string WRN_RecursiveAddHandlerCall => GetResourceString("WRN_RecursiveAddHandlerCall");

		internal static string WRN_RecursiveAddHandlerCall_Title => GetResourceString("WRN_RecursiveAddHandlerCall_Title");

		internal static string WRN_ImplicitConversionCopyBack => GetResourceString("WRN_ImplicitConversionCopyBack");

		internal static string WRN_ImplicitConversionCopyBack_Title => GetResourceString("WRN_ImplicitConversionCopyBack_Title");

		internal static string WRN_MustShadowOnMultipleInheritance2 => GetResourceString("WRN_MustShadowOnMultipleInheritance2");

		internal static string WRN_MustShadowOnMultipleInheritance2_Title => GetResourceString("WRN_MustShadowOnMultipleInheritance2_Title");

		internal static string WRN_RecursiveOperatorCall => GetResourceString("WRN_RecursiveOperatorCall");

		internal static string WRN_RecursiveOperatorCall_Title => GetResourceString("WRN_RecursiveOperatorCall_Title");

		internal static string WRN_ImplicitConversion2 => GetResourceString("WRN_ImplicitConversion2");

		internal static string WRN_ImplicitConversion2_Title => GetResourceString("WRN_ImplicitConversion2_Title");

		internal static string WRN_MutableStructureInUsing => GetResourceString("WRN_MutableStructureInUsing");

		internal static string WRN_MutableStructureInUsing_Title => GetResourceString("WRN_MutableStructureInUsing_Title");

		internal static string WRN_MutableGenericStructureInUsing => GetResourceString("WRN_MutableGenericStructureInUsing");

		internal static string WRN_MutableGenericStructureInUsing_Title => GetResourceString("WRN_MutableGenericStructureInUsing_Title");

		internal static string WRN_ImplicitConversionSubst1 => GetResourceString("WRN_ImplicitConversionSubst1");

		internal static string WRN_ImplicitConversionSubst1_Title => GetResourceString("WRN_ImplicitConversionSubst1_Title");

		internal static string WRN_LateBindingResolution => GetResourceString("WRN_LateBindingResolution");

		internal static string WRN_LateBindingResolution_Title => GetResourceString("WRN_LateBindingResolution_Title");

		internal static string WRN_ObjectMath1 => GetResourceString("WRN_ObjectMath1");

		internal static string WRN_ObjectMath1_Title => GetResourceString("WRN_ObjectMath1_Title");

		internal static string WRN_ObjectMath2 => GetResourceString("WRN_ObjectMath2");

		internal static string WRN_ObjectMath2_Title => GetResourceString("WRN_ObjectMath2_Title");

		internal static string WRN_ObjectAssumedVar1 => GetResourceString("WRN_ObjectAssumedVar1");

		internal static string WRN_ObjectAssumedVar1_Title => GetResourceString("WRN_ObjectAssumedVar1_Title");

		internal static string WRN_ObjectAssumed1 => GetResourceString("WRN_ObjectAssumed1");

		internal static string WRN_ObjectAssumed1_Title => GetResourceString("WRN_ObjectAssumed1_Title");

		internal static string WRN_ObjectAssumedProperty1 => GetResourceString("WRN_ObjectAssumedProperty1");

		internal static string WRN_ObjectAssumedProperty1_Title => GetResourceString("WRN_ObjectAssumedProperty1_Title");

		internal static string WRN_MissingAsClauseinVarDecl => GetResourceString("WRN_MissingAsClauseinVarDecl");

		internal static string WRN_MissingAsClauseinVarDecl_Title => GetResourceString("WRN_MissingAsClauseinVarDecl_Title");

		internal static string WRN_MissingAsClauseinFunction => GetResourceString("WRN_MissingAsClauseinFunction");

		internal static string WRN_MissingAsClauseinFunction_Title => GetResourceString("WRN_MissingAsClauseinFunction_Title");

		internal static string WRN_MissingAsClauseinProperty => GetResourceString("WRN_MissingAsClauseinProperty");

		internal static string WRN_MissingAsClauseinProperty_Title => GetResourceString("WRN_MissingAsClauseinProperty_Title");

		internal static string WRN_UnusedLocal => GetResourceString("WRN_UnusedLocal");

		internal static string WRN_UnusedLocal_Title => GetResourceString("WRN_UnusedLocal_Title");

		internal static string WRN_SharedMemberThroughInstance => GetResourceString("WRN_SharedMemberThroughInstance");

		internal static string WRN_SharedMemberThroughInstance_Title => GetResourceString("WRN_SharedMemberThroughInstance_Title");

		internal static string WRN_RecursivePropertyCall => GetResourceString("WRN_RecursivePropertyCall");

		internal static string WRN_RecursivePropertyCall_Title => GetResourceString("WRN_RecursivePropertyCall_Title");

		internal static string WRN_OverlappingCatch => GetResourceString("WRN_OverlappingCatch");

		internal static string WRN_OverlappingCatch_Title => GetResourceString("WRN_OverlappingCatch_Title");

		internal static string WRN_DefAsgUseNullRefByRef => GetResourceString("WRN_DefAsgUseNullRefByRef");

		internal static string WRN_DefAsgUseNullRefByRef_Title => GetResourceString("WRN_DefAsgUseNullRefByRef_Title");

		internal static string WRN_DuplicateCatch => GetResourceString("WRN_DuplicateCatch");

		internal static string WRN_DuplicateCatch_Title => GetResourceString("WRN_DuplicateCatch_Title");

		internal static string WRN_ObjectMath1Not => GetResourceString("WRN_ObjectMath1Not");

		internal static string WRN_ObjectMath1Not_Title => GetResourceString("WRN_ObjectMath1Not_Title");

		internal static string WRN_BadChecksumValExtChecksum => GetResourceString("WRN_BadChecksumValExtChecksum");

		internal static string WRN_BadChecksumValExtChecksum_Title => GetResourceString("WRN_BadChecksumValExtChecksum_Title");

		internal static string WRN_MultipleDeclFileExtChecksum => GetResourceString("WRN_MultipleDeclFileExtChecksum");

		internal static string WRN_MultipleDeclFileExtChecksum_Title => GetResourceString("WRN_MultipleDeclFileExtChecksum_Title");

		internal static string WRN_BadGUIDFormatExtChecksum => GetResourceString("WRN_BadGUIDFormatExtChecksum");

		internal static string WRN_BadGUIDFormatExtChecksum_Title => GetResourceString("WRN_BadGUIDFormatExtChecksum_Title");

		internal static string WRN_ObjectMathSelectCase => GetResourceString("WRN_ObjectMathSelectCase");

		internal static string WRN_ObjectMathSelectCase_Title => GetResourceString("WRN_ObjectMathSelectCase_Title");

		internal static string WRN_EqualToLiteralNothing => GetResourceString("WRN_EqualToLiteralNothing");

		internal static string WRN_EqualToLiteralNothing_Title => GetResourceString("WRN_EqualToLiteralNothing_Title");

		internal static string WRN_NotEqualToLiteralNothing => GetResourceString("WRN_NotEqualToLiteralNothing");

		internal static string WRN_NotEqualToLiteralNothing_Title => GetResourceString("WRN_NotEqualToLiteralNothing_Title");

		internal static string WRN_UnusedLocalConst => GetResourceString("WRN_UnusedLocalConst");

		internal static string WRN_UnusedLocalConst_Title => GetResourceString("WRN_UnusedLocalConst_Title");

		internal static string WRN_ComClassInterfaceShadows5 => GetResourceString("WRN_ComClassInterfaceShadows5");

		internal static string WRN_ComClassInterfaceShadows5_Title => GetResourceString("WRN_ComClassInterfaceShadows5_Title");

		internal static string WRN_ComClassPropertySetObject1 => GetResourceString("WRN_ComClassPropertySetObject1");

		internal static string WRN_ComClassPropertySetObject1_Title => GetResourceString("WRN_ComClassPropertySetObject1_Title");

		internal static string WRN_DefAsgUseNullRef => GetResourceString("WRN_DefAsgUseNullRef");

		internal static string WRN_DefAsgUseNullRef_Title => GetResourceString("WRN_DefAsgUseNullRef_Title");

		internal static string WRN_DefAsgNoRetValFuncRef1 => GetResourceString("WRN_DefAsgNoRetValFuncRef1");

		internal static string WRN_DefAsgNoRetValFuncRef1_Title => GetResourceString("WRN_DefAsgNoRetValFuncRef1_Title");

		internal static string WRN_DefAsgNoRetValOpRef1 => GetResourceString("WRN_DefAsgNoRetValOpRef1");

		internal static string WRN_DefAsgNoRetValOpRef1_Title => GetResourceString("WRN_DefAsgNoRetValOpRef1_Title");

		internal static string WRN_DefAsgNoRetValPropRef1 => GetResourceString("WRN_DefAsgNoRetValPropRef1");

		internal static string WRN_DefAsgNoRetValPropRef1_Title => GetResourceString("WRN_DefAsgNoRetValPropRef1_Title");

		internal static string WRN_DefAsgUseNullRefByRefStr => GetResourceString("WRN_DefAsgUseNullRefByRefStr");

		internal static string WRN_DefAsgUseNullRefByRefStr_Title => GetResourceString("WRN_DefAsgUseNullRefByRefStr_Title");

		internal static string WRN_DefAsgUseNullRefStr => GetResourceString("WRN_DefAsgUseNullRefStr");

		internal static string WRN_DefAsgUseNullRefStr_Title => GetResourceString("WRN_DefAsgUseNullRefStr_Title");

		internal static string WRN_StaticLocalNoInference => GetResourceString("WRN_StaticLocalNoInference");

		internal static string WRN_StaticLocalNoInference_Title => GetResourceString("WRN_StaticLocalNoInference_Title");

		internal static string WRN_InvalidAssemblyName => GetResourceString("WRN_InvalidAssemblyName");

		internal static string WRN_InvalidAssemblyName_Title => GetResourceString("WRN_InvalidAssemblyName_Title");

		internal static string WRN_XMLDocBadXMLLine => GetResourceString("WRN_XMLDocBadXMLLine");

		internal static string WRN_XMLDocBadXMLLine_Title => GetResourceString("WRN_XMLDocBadXMLLine_Title");

		internal static string WRN_XMLDocMoreThanOneCommentBlock => GetResourceString("WRN_XMLDocMoreThanOneCommentBlock");

		internal static string WRN_XMLDocMoreThanOneCommentBlock_Title => GetResourceString("WRN_XMLDocMoreThanOneCommentBlock_Title");

		internal static string WRN_XMLDocNotFirstOnLine => GetResourceString("WRN_XMLDocNotFirstOnLine");

		internal static string WRN_XMLDocNotFirstOnLine_Title => GetResourceString("WRN_XMLDocNotFirstOnLine_Title");

		internal static string WRN_XMLDocInsideMethod => GetResourceString("WRN_XMLDocInsideMethod");

		internal static string WRN_XMLDocInsideMethod_Title => GetResourceString("WRN_XMLDocInsideMethod_Title");

		internal static string WRN_XMLDocParseError1 => GetResourceString("WRN_XMLDocParseError1");

		internal static string WRN_XMLDocParseError1_Title => GetResourceString("WRN_XMLDocParseError1_Title");

		internal static string WRN_XMLDocDuplicateXMLNode1 => GetResourceString("WRN_XMLDocDuplicateXMLNode1");

		internal static string WRN_XMLDocDuplicateXMLNode1_Title => GetResourceString("WRN_XMLDocDuplicateXMLNode1_Title");

		internal static string WRN_XMLDocIllegalTagOnElement2 => GetResourceString("WRN_XMLDocIllegalTagOnElement2");

		internal static string WRN_XMLDocIllegalTagOnElement2_Title => GetResourceString("WRN_XMLDocIllegalTagOnElement2_Title");

		internal static string WRN_XMLDocBadParamTag2 => GetResourceString("WRN_XMLDocBadParamTag2");

		internal static string WRN_XMLDocBadParamTag2_Title => GetResourceString("WRN_XMLDocBadParamTag2_Title");

		internal static string WRN_XMLDocParamTagWithoutName => GetResourceString("WRN_XMLDocParamTagWithoutName");

		internal static string WRN_XMLDocParamTagWithoutName_Title => GetResourceString("WRN_XMLDocParamTagWithoutName_Title");

		internal static string WRN_XMLDocCrefAttributeNotFound1 => GetResourceString("WRN_XMLDocCrefAttributeNotFound1");

		internal static string WRN_XMLDocCrefAttributeNotFound1_Title => GetResourceString("WRN_XMLDocCrefAttributeNotFound1_Title");

		internal static string WRN_XMLMissingFileOrPathAttribute1 => GetResourceString("WRN_XMLMissingFileOrPathAttribute1");

		internal static string WRN_XMLMissingFileOrPathAttribute1_Title => GetResourceString("WRN_XMLMissingFileOrPathAttribute1_Title");

		internal static string WRN_XMLCannotWriteToXMLDocFile2 => GetResourceString("WRN_XMLCannotWriteToXMLDocFile2");

		internal static string WRN_XMLCannotWriteToXMLDocFile2_Title => GetResourceString("WRN_XMLCannotWriteToXMLDocFile2_Title");

		internal static string WRN_XMLDocWithoutLanguageElement => GetResourceString("WRN_XMLDocWithoutLanguageElement");

		internal static string WRN_XMLDocWithoutLanguageElement_Title => GetResourceString("WRN_XMLDocWithoutLanguageElement_Title");

		internal static string WRN_XMLDocReturnsOnWriteOnlyProperty => GetResourceString("WRN_XMLDocReturnsOnWriteOnlyProperty");

		internal static string WRN_XMLDocReturnsOnWriteOnlyProperty_Title => GetResourceString("WRN_XMLDocReturnsOnWriteOnlyProperty_Title");

		internal static string WRN_XMLDocOnAPartialType => GetResourceString("WRN_XMLDocOnAPartialType");

		internal static string WRN_XMLDocOnAPartialType_Title => GetResourceString("WRN_XMLDocOnAPartialType_Title");

		internal static string WRN_XMLDocReturnsOnADeclareSub => GetResourceString("WRN_XMLDocReturnsOnADeclareSub");

		internal static string WRN_XMLDocReturnsOnADeclareSub_Title => GetResourceString("WRN_XMLDocReturnsOnADeclareSub_Title");

		internal static string WRN_XMLDocStartTagWithNoEndTag => GetResourceString("WRN_XMLDocStartTagWithNoEndTag");

		internal static string WRN_XMLDocStartTagWithNoEndTag_Title => GetResourceString("WRN_XMLDocStartTagWithNoEndTag_Title");

		internal static string WRN_XMLDocBadGenericParamTag2 => GetResourceString("WRN_XMLDocBadGenericParamTag2");

		internal static string WRN_XMLDocBadGenericParamTag2_Title => GetResourceString("WRN_XMLDocBadGenericParamTag2_Title");

		internal static string WRN_XMLDocGenericParamTagWithoutName => GetResourceString("WRN_XMLDocGenericParamTagWithoutName");

		internal static string WRN_XMLDocGenericParamTagWithoutName_Title => GetResourceString("WRN_XMLDocGenericParamTagWithoutName_Title");

		internal static string WRN_XMLDocExceptionTagWithoutCRef => GetResourceString("WRN_XMLDocExceptionTagWithoutCRef");

		internal static string WRN_XMLDocExceptionTagWithoutCRef_Title => GetResourceString("WRN_XMLDocExceptionTagWithoutCRef_Title");

		internal static string WRN_XMLDocInvalidXMLFragment => GetResourceString("WRN_XMLDocInvalidXMLFragment");

		internal static string WRN_XMLDocInvalidXMLFragment_Title => GetResourceString("WRN_XMLDocInvalidXMLFragment_Title");

		internal static string WRN_XMLDocBadFormedXML => GetResourceString("WRN_XMLDocBadFormedXML");

		internal static string WRN_XMLDocBadFormedXML_Title => GetResourceString("WRN_XMLDocBadFormedXML_Title");

		internal static string WRN_InterfaceConversion2 => GetResourceString("WRN_InterfaceConversion2");

		internal static string WRN_InterfaceConversion2_Title => GetResourceString("WRN_InterfaceConversion2_Title");

		internal static string WRN_LiftControlVariableLambda => GetResourceString("WRN_LiftControlVariableLambda");

		internal static string WRN_LiftControlVariableLambda_Title => GetResourceString("WRN_LiftControlVariableLambda_Title");

		internal static string WRN_LambdaPassedToRemoveHandler => GetResourceString("WRN_LambdaPassedToRemoveHandler");

		internal static string WRN_LambdaPassedToRemoveHandler_Title => GetResourceString("WRN_LambdaPassedToRemoveHandler_Title");

		internal static string WRN_LiftControlVariableQuery => GetResourceString("WRN_LiftControlVariableQuery");

		internal static string WRN_LiftControlVariableQuery_Title => GetResourceString("WRN_LiftControlVariableQuery_Title");

		internal static string WRN_RelDelegatePassedToRemoveHandler => GetResourceString("WRN_RelDelegatePassedToRemoveHandler");

		internal static string WRN_RelDelegatePassedToRemoveHandler_Title => GetResourceString("WRN_RelDelegatePassedToRemoveHandler_Title");

		internal static string WRN_QueryMissingAsClauseinVarDecl => GetResourceString("WRN_QueryMissingAsClauseinVarDecl");

		internal static string WRN_QueryMissingAsClauseinVarDecl_Title => GetResourceString("WRN_QueryMissingAsClauseinVarDecl_Title");

		internal static string ERR_MultilineLambdaMissingFunction => GetResourceString("ERR_MultilineLambdaMissingFunction");

		internal static string ERR_MultilineLambdaMissingSub => GetResourceString("ERR_MultilineLambdaMissingSub");

		internal static string ERR_AttributeOnLambdaReturnType => GetResourceString("ERR_AttributeOnLambdaReturnType");

		internal static string ERR_SubDisallowsStatement => GetResourceString("ERR_SubDisallowsStatement");

		internal static string ERR_SubRequiresParenthesesBang => GetResourceString("ERR_SubRequiresParenthesesBang");

		internal static string ERR_SubRequiresParenthesesDot => GetResourceString("ERR_SubRequiresParenthesesDot");

		internal static string ERR_SubRequiresParenthesesLParen => GetResourceString("ERR_SubRequiresParenthesesLParen");

		internal static string ERR_SubRequiresSingleStatement => GetResourceString("ERR_SubRequiresSingleStatement");

		internal static string ERR_StaticInLambda => GetResourceString("ERR_StaticInLambda");

		internal static string ERR_InitializedExpandedProperty => GetResourceString("ERR_InitializedExpandedProperty");

		internal static string ERR_AutoPropertyCantHaveParams => GetResourceString("ERR_AutoPropertyCantHaveParams");

		internal static string ERR_AutoPropertyCantBeWriteOnly => GetResourceString("ERR_AutoPropertyCantBeWriteOnly");

		internal static string ERR_IllegalOperandInIIFCount => GetResourceString("ERR_IllegalOperandInIIFCount");

		internal static string ERR_NotACollection1 => GetResourceString("ERR_NotACollection1");

		internal static string ERR_NoAddMethod1 => GetResourceString("ERR_NoAddMethod1");

		internal static string ERR_CantCombineInitializers => GetResourceString("ERR_CantCombineInitializers");

		internal static string ERR_EmptyAggregateInitializer => GetResourceString("ERR_EmptyAggregateInitializer");

		internal static string ERR_XmlEndElementNoMatchingStart => GetResourceString("ERR_XmlEndElementNoMatchingStart");

		internal static string ERR_MultilineLambdasCannotContainOnError => GetResourceString("ERR_MultilineLambdasCannotContainOnError");

		internal static string ERR_VarianceDisallowedHere => GetResourceString("ERR_VarianceDisallowedHere");

		internal static string ERR_XmlEndCDataNotAllowedInContent => GetResourceString("ERR_XmlEndCDataNotAllowedInContent");

		internal static string ERR_OverloadsModifierInModule => GetResourceString("ERR_OverloadsModifierInModule");

		internal static string ERR_UndefinedTypeOrNamespace1 => GetResourceString("ERR_UndefinedTypeOrNamespace1");

		internal static string ERR_IdentityDirectCastForFloat => GetResourceString("ERR_IdentityDirectCastForFloat");

		internal static string WRN_ObsoleteIdentityDirectCastForValueType => GetResourceString("WRN_ObsoleteIdentityDirectCastForValueType");

		internal static string WRN_ObsoleteIdentityDirectCastForValueType_Title => GetResourceString("WRN_ObsoleteIdentityDirectCastForValueType_Title");

		internal static string WRN_UnreachableCode => GetResourceString("WRN_UnreachableCode");

		internal static string WRN_UnreachableCode_Title => GetResourceString("WRN_UnreachableCode_Title");

		internal static string WRN_DefAsgNoRetValFuncVal1 => GetResourceString("WRN_DefAsgNoRetValFuncVal1");

		internal static string WRN_DefAsgNoRetValFuncVal1_Title => GetResourceString("WRN_DefAsgNoRetValFuncVal1_Title");

		internal static string WRN_DefAsgNoRetValOpVal1 => GetResourceString("WRN_DefAsgNoRetValOpVal1");

		internal static string WRN_DefAsgNoRetValOpVal1_Title => GetResourceString("WRN_DefAsgNoRetValOpVal1_Title");

		internal static string WRN_DefAsgNoRetValPropVal1 => GetResourceString("WRN_DefAsgNoRetValPropVal1");

		internal static string WRN_DefAsgNoRetValPropVal1_Title => GetResourceString("WRN_DefAsgNoRetValPropVal1_Title");

		internal static string ERR_NestedGlobalNamespace => GetResourceString("ERR_NestedGlobalNamespace");

		internal static string ERR_AccessMismatch6 => GetResourceString("ERR_AccessMismatch6");

		internal static string ERR_BadMetaDataReference1 => GetResourceString("ERR_BadMetaDataReference1");

		internal static string ERR_PropertyDoesntImplementAllAccessors => GetResourceString("ERR_PropertyDoesntImplementAllAccessors");

		internal static string ERR_UnimplementedMustOverride => GetResourceString("ERR_UnimplementedMustOverride");

		internal static string ERR_IfTooManyTypesObjectDisallowed => GetResourceString("ERR_IfTooManyTypesObjectDisallowed");

		internal static string WRN_IfTooManyTypesObjectAssumed => GetResourceString("WRN_IfTooManyTypesObjectAssumed");

		internal static string WRN_IfTooManyTypesObjectAssumed_Title => GetResourceString("WRN_IfTooManyTypesObjectAssumed_Title");

		internal static string ERR_IfNoTypeObjectDisallowed => GetResourceString("ERR_IfNoTypeObjectDisallowed");

		internal static string WRN_IfNoTypeObjectAssumed => GetResourceString("WRN_IfNoTypeObjectAssumed");

		internal static string WRN_IfNoTypeObjectAssumed_Title => GetResourceString("WRN_IfNoTypeObjectAssumed_Title");

		internal static string ERR_IfNoType => GetResourceString("ERR_IfNoType");

		internal static string ERR_PublicKeyFileFailure => GetResourceString("ERR_PublicKeyFileFailure");

		internal static string ERR_PublicKeyContainerFailure => GetResourceString("ERR_PublicKeyContainerFailure");

		internal static string ERR_FriendRefNotEqualToThis => GetResourceString("ERR_FriendRefNotEqualToThis");

		internal static string ERR_FriendRefSigningMismatch => GetResourceString("ERR_FriendRefSigningMismatch");

		internal static string ERR_PublicSignNoKey => GetResourceString("ERR_PublicSignNoKey");

		internal static string ERR_PublicSignNetModule => GetResourceString("ERR_PublicSignNetModule");

		internal static string WRN_AttributeIgnoredWhenPublicSigning => GetResourceString("WRN_AttributeIgnoredWhenPublicSigning");

		internal static string WRN_AttributeIgnoredWhenPublicSigning_Title => GetResourceString("WRN_AttributeIgnoredWhenPublicSigning_Title");

		internal static string WRN_DelaySignButNoKey => GetResourceString("WRN_DelaySignButNoKey");

		internal static string WRN_DelaySignButNoKey_Title => GetResourceString("WRN_DelaySignButNoKey_Title");

		internal static string ERR_SignButNoPrivateKey => GetResourceString("ERR_SignButNoPrivateKey");

		internal static string ERR_FailureSigningAssembly => GetResourceString("ERR_FailureSigningAssembly");

		internal static string ERR_InvalidVersionFormat => GetResourceString("ERR_InvalidVersionFormat");

		internal static string WRN_InvalidVersionFormat => GetResourceString("WRN_InvalidVersionFormat");

		internal static string WRN_InvalidVersionFormat_Title => GetResourceString("WRN_InvalidVersionFormat_Title");

		internal static string ERR_InvalidVersionFormat2 => GetResourceString("ERR_InvalidVersionFormat2");

		internal static string ERR_InvalidAssemblyCultureForExe => GetResourceString("ERR_InvalidAssemblyCultureForExe");

		internal static string WRN_MainIgnored => GetResourceString("WRN_MainIgnored");

		internal static string WRN_MainIgnored_Title => GetResourceString("WRN_MainIgnored_Title");

		internal static string WRN_EmptyPrefixAndXmlnsLocalName => GetResourceString("WRN_EmptyPrefixAndXmlnsLocalName");

		internal static string WRN_EmptyPrefixAndXmlnsLocalName_Title => GetResourceString("WRN_EmptyPrefixAndXmlnsLocalName_Title");

		internal static string WRN_PrefixAndXmlnsLocalName => GetResourceString("WRN_PrefixAndXmlnsLocalName");

		internal static string WRN_PrefixAndXmlnsLocalName_Title => GetResourceString("WRN_PrefixAndXmlnsLocalName_Title");

		internal static string ERR_ExpectedSingleScript => GetResourceString("ERR_ExpectedSingleScript");

		internal static string ERR_ReservedAssemblyName => GetResourceString("ERR_ReservedAssemblyName");

		internal static string ERR_ReferenceDirectiveOnlyAllowedInScripts => GetResourceString("ERR_ReferenceDirectiveOnlyAllowedInScripts");

		internal static string ERR_NamespaceNotAllowedInScript => GetResourceString("ERR_NamespaceNotAllowedInScript");

		internal static string ERR_KeywordNotAllowedInScript => GetResourceString("ERR_KeywordNotAllowedInScript");

		internal static string ERR_LambdaNoType => GetResourceString("ERR_LambdaNoType");

		internal static string WRN_LambdaNoTypeObjectAssumed => GetResourceString("WRN_LambdaNoTypeObjectAssumed");

		internal static string WRN_LambdaNoTypeObjectAssumed_Title => GetResourceString("WRN_LambdaNoTypeObjectAssumed_Title");

		internal static string WRN_LambdaTooManyTypesObjectAssumed => GetResourceString("WRN_LambdaTooManyTypesObjectAssumed");

		internal static string WRN_LambdaTooManyTypesObjectAssumed_Title => GetResourceString("WRN_LambdaTooManyTypesObjectAssumed_Title");

		internal static string ERR_LambdaNoTypeObjectDisallowed => GetResourceString("ERR_LambdaNoTypeObjectDisallowed");

		internal static string ERR_LambdaTooManyTypesObjectDisallowed => GetResourceString("ERR_LambdaTooManyTypesObjectDisallowed");

		internal static string WRN_UnimplementedCommandLineSwitch => GetResourceString("WRN_UnimplementedCommandLineSwitch");

		internal static string WRN_UnimplementedCommandLineSwitch_Title => GetResourceString("WRN_UnimplementedCommandLineSwitch_Title");

		internal static string ERR_ArrayInitNoTypeObjectDisallowed => GetResourceString("ERR_ArrayInitNoTypeObjectDisallowed");

		internal static string ERR_ArrayInitNoType => GetResourceString("ERR_ArrayInitNoType");

		internal static string ERR_ArrayInitTooManyTypesObjectDisallowed => GetResourceString("ERR_ArrayInitTooManyTypesObjectDisallowed");

		internal static string WRN_ArrayInitNoTypeObjectAssumed => GetResourceString("WRN_ArrayInitNoTypeObjectAssumed");

		internal static string WRN_ArrayInitNoTypeObjectAssumed_Title => GetResourceString("WRN_ArrayInitNoTypeObjectAssumed_Title");

		internal static string WRN_ArrayInitTooManyTypesObjectAssumed => GetResourceString("WRN_ArrayInitTooManyTypesObjectAssumed");

		internal static string WRN_ArrayInitTooManyTypesObjectAssumed_Title => GetResourceString("WRN_ArrayInitTooManyTypesObjectAssumed_Title");

		internal static string WRN_TypeInferenceAssumed3 => GetResourceString("WRN_TypeInferenceAssumed3");

		internal static string WRN_TypeInferenceAssumed3_Title => GetResourceString("WRN_TypeInferenceAssumed3_Title");

		internal static string ERR_AmbiguousCastConversion2 => GetResourceString("ERR_AmbiguousCastConversion2");

		internal static string WRN_AmbiguousCastConversion2 => GetResourceString("WRN_AmbiguousCastConversion2");

		internal static string WRN_AmbiguousCastConversion2_Title => GetResourceString("WRN_AmbiguousCastConversion2_Title");

		internal static string ERR_VarianceIEnumerableSuggestion3 => GetResourceString("ERR_VarianceIEnumerableSuggestion3");

		internal static string WRN_VarianceIEnumerableSuggestion3 => GetResourceString("WRN_VarianceIEnumerableSuggestion3");

		internal static string WRN_VarianceIEnumerableSuggestion3_Title => GetResourceString("WRN_VarianceIEnumerableSuggestion3_Title");

		internal static string ERR_VarianceConversionFailedIn6 => GetResourceString("ERR_VarianceConversionFailedIn6");

		internal static string ERR_VarianceConversionFailedOut6 => GetResourceString("ERR_VarianceConversionFailedOut6");

		internal static string WRN_VarianceConversionFailedIn6 => GetResourceString("WRN_VarianceConversionFailedIn6");

		internal static string WRN_VarianceConversionFailedIn6_Title => GetResourceString("WRN_VarianceConversionFailedIn6_Title");

		internal static string WRN_VarianceConversionFailedOut6 => GetResourceString("WRN_VarianceConversionFailedOut6");

		internal static string WRN_VarianceConversionFailedOut6_Title => GetResourceString("WRN_VarianceConversionFailedOut6_Title");

		internal static string ERR_VarianceConversionFailedTryIn4 => GetResourceString("ERR_VarianceConversionFailedTryIn4");

		internal static string ERR_VarianceConversionFailedTryOut4 => GetResourceString("ERR_VarianceConversionFailedTryOut4");

		internal static string WRN_VarianceConversionFailedTryIn4 => GetResourceString("WRN_VarianceConversionFailedTryIn4");

		internal static string WRN_VarianceConversionFailedTryIn4_Title => GetResourceString("WRN_VarianceConversionFailedTryIn4_Title");

		internal static string WRN_VarianceConversionFailedTryOut4 => GetResourceString("WRN_VarianceConversionFailedTryOut4");

		internal static string WRN_VarianceConversionFailedTryOut4_Title => GetResourceString("WRN_VarianceConversionFailedTryOut4_Title");

		internal static string WRN_VarianceDeclarationAmbiguous3 => GetResourceString("WRN_VarianceDeclarationAmbiguous3");

		internal static string WRN_VarianceDeclarationAmbiguous3_Title => GetResourceString("WRN_VarianceDeclarationAmbiguous3_Title");

		internal static string ERR_VarianceInterfaceNesting => GetResourceString("ERR_VarianceInterfaceNesting");

		internal static string ERR_VariancePreventsSynthesizedEvents2 => GetResourceString("ERR_VariancePreventsSynthesizedEvents2");

		internal static string ERR_VarianceInByRefDisallowed1 => GetResourceString("ERR_VarianceInByRefDisallowed1");

		internal static string ERR_VarianceInNullableDisallowed2 => GetResourceString("ERR_VarianceInNullableDisallowed2");

		internal static string ERR_VarianceInParamDisallowed1 => GetResourceString("ERR_VarianceInParamDisallowed1");

		internal static string ERR_VarianceInParamDisallowedForGeneric3 => GetResourceString("ERR_VarianceInParamDisallowedForGeneric3");

		internal static string ERR_VarianceInParamDisallowedHere2 => GetResourceString("ERR_VarianceInParamDisallowedHere2");

		internal static string ERR_VarianceInParamDisallowedHereForGeneric4 => GetResourceString("ERR_VarianceInParamDisallowedHereForGeneric4");

		internal static string ERR_VarianceInPropertyDisallowed1 => GetResourceString("ERR_VarianceInPropertyDisallowed1");

		internal static string ERR_VarianceInReadOnlyPropertyDisallowed1 => GetResourceString("ERR_VarianceInReadOnlyPropertyDisallowed1");

		internal static string ERR_VarianceInReturnDisallowed1 => GetResourceString("ERR_VarianceInReturnDisallowed1");

		internal static string ERR_VarianceOutByRefDisallowed1 => GetResourceString("ERR_VarianceOutByRefDisallowed1");

		internal static string ERR_VarianceOutByValDisallowed1 => GetResourceString("ERR_VarianceOutByValDisallowed1");

		internal static string ERR_VarianceOutConstraintDisallowed1 => GetResourceString("ERR_VarianceOutConstraintDisallowed1");

		internal static string ERR_VarianceOutNullableDisallowed2 => GetResourceString("ERR_VarianceOutNullableDisallowed2");

		internal static string ERR_VarianceOutParamDisallowed1 => GetResourceString("ERR_VarianceOutParamDisallowed1");

		internal static string ERR_VarianceOutParamDisallowedForGeneric3 => GetResourceString("ERR_VarianceOutParamDisallowedForGeneric3");

		internal static string ERR_VarianceOutParamDisallowedHere2 => GetResourceString("ERR_VarianceOutParamDisallowedHere2");

		internal static string ERR_VarianceOutParamDisallowedHereForGeneric4 => GetResourceString("ERR_VarianceOutParamDisallowedHereForGeneric4");

		internal static string ERR_VarianceOutPropertyDisallowed1 => GetResourceString("ERR_VarianceOutPropertyDisallowed1");

		internal static string ERR_VarianceOutWriteOnlyPropertyDisallowed1 => GetResourceString("ERR_VarianceOutWriteOnlyPropertyDisallowed1");

		internal static string ERR_VarianceTypeDisallowed2 => GetResourceString("ERR_VarianceTypeDisallowed2");

		internal static string ERR_VarianceTypeDisallowedForGeneric4 => GetResourceString("ERR_VarianceTypeDisallowedForGeneric4");

		internal static string ERR_VarianceTypeDisallowedHere3 => GetResourceString("ERR_VarianceTypeDisallowedHere3");

		internal static string ERR_VarianceTypeDisallowedHereForGeneric5 => GetResourceString("ERR_VarianceTypeDisallowedHereForGeneric5");

		internal static string ERR_ParameterNotValidForType => GetResourceString("ERR_ParameterNotValidForType");

		internal static string ERR_MarshalUnmanagedTypeNotValidForFields => GetResourceString("ERR_MarshalUnmanagedTypeNotValidForFields");

		internal static string ERR_MarshalUnmanagedTypeOnlyValidForFields => GetResourceString("ERR_MarshalUnmanagedTypeOnlyValidForFields");

		internal static string ERR_AttributeParameterRequired1 => GetResourceString("ERR_AttributeParameterRequired1");

		internal static string ERR_AttributeParameterRequired2 => GetResourceString("ERR_AttributeParameterRequired2");

		internal static string ERR_MemberConflictWithSynth4 => GetResourceString("ERR_MemberConflictWithSynth4");

		internal static string IDS_ProjectSettingsLocationName => GetResourceString("IDS_ProjectSettingsLocationName");

		internal static string WRN_ReturnTypeAttributeOnWriteOnlyProperty => GetResourceString("WRN_ReturnTypeAttributeOnWriteOnlyProperty");

		internal static string WRN_ReturnTypeAttributeOnWriteOnlyProperty_Title => GetResourceString("WRN_ReturnTypeAttributeOnWriteOnlyProperty_Title");

		internal static string ERR_SecurityAttributeInvalidTarget => GetResourceString("ERR_SecurityAttributeInvalidTarget");

		internal static string ERR_AbsentReferenceToPIA1 => GetResourceString("ERR_AbsentReferenceToPIA1");

		internal static string ERR_CannotLinkClassWithNoPIA1 => GetResourceString("ERR_CannotLinkClassWithNoPIA1");

		internal static string ERR_InvalidStructMemberNoPIA1 => GetResourceString("ERR_InvalidStructMemberNoPIA1");

		internal static string ERR_NoPIAAttributeMissing2 => GetResourceString("ERR_NoPIAAttributeMissing2");

		internal static string ERR_PIAHasNoAssemblyGuid1 => GetResourceString("ERR_PIAHasNoAssemblyGuid1");

		internal static string ERR_DuplicateLocalTypes3 => GetResourceString("ERR_DuplicateLocalTypes3");

		internal static string ERR_PIAHasNoTypeLibAttribute1 => GetResourceString("ERR_PIAHasNoTypeLibAttribute1");

		internal static string ERR_SourceInterfaceMustBeInterface => GetResourceString("ERR_SourceInterfaceMustBeInterface");

		internal static string ERR_EventNoPIANoBackingMember => GetResourceString("ERR_EventNoPIANoBackingMember");

		internal static string ERR_NestedInteropType => GetResourceString("ERR_NestedInteropType");

		internal static string ERR_LocalTypeNameClash2 => GetResourceString("ERR_LocalTypeNameClash2");

		internal static string ERR_InteropMethodWithBody1 => GetResourceString("ERR_InteropMethodWithBody1");

		internal static string ERR_BadAsyncInQuery => GetResourceString("ERR_BadAsyncInQuery");

		internal static string ERR_BadGetAwaiterMethod1 => GetResourceString("ERR_BadGetAwaiterMethod1");

		internal static string ERR_BadIsCompletedOnCompletedGetResult2 => GetResourceString("ERR_BadIsCompletedOnCompletedGetResult2");

		internal static string ERR_DoesntImplementAwaitInterface2 => GetResourceString("ERR_DoesntImplementAwaitInterface2");

		internal static string ERR_BadAwaitNothing => GetResourceString("ERR_BadAwaitNothing");

		internal static string ERR_BadAsyncByRefParam => GetResourceString("ERR_BadAsyncByRefParam");

		internal static string ERR_InvalidAsyncIteratorModifiers => GetResourceString("ERR_InvalidAsyncIteratorModifiers");

		internal static string ERR_BadResumableAccessReturnVariable => GetResourceString("ERR_BadResumableAccessReturnVariable");

		internal static string ERR_ReturnFromNonGenericTaskAsync => GetResourceString("ERR_ReturnFromNonGenericTaskAsync");

		internal static string ERR_BadAsyncReturnOperand1 => GetResourceString("ERR_BadAsyncReturnOperand1");

		internal static string ERR_BadAsyncReturn => GetResourceString("ERR_BadAsyncReturn");

		internal static string ERR_CantAwaitAsyncSub1 => GetResourceString("ERR_CantAwaitAsyncSub1");

		internal static string ERR_InvalidLambdaModifier => GetResourceString("ERR_InvalidLambdaModifier");

		internal static string ERR_BadAwaitInNonAsyncMethod => GetResourceString("ERR_BadAwaitInNonAsyncMethod");

		internal static string ERR_BadAwaitInNonAsyncVoidMethod => GetResourceString("ERR_BadAwaitInNonAsyncVoidMethod");

		internal static string ERR_BadAwaitInNonAsyncLambda => GetResourceString("ERR_BadAwaitInNonAsyncLambda");

		internal static string ERR_BadAwaitNotInAsyncMethodOrLambda => GetResourceString("ERR_BadAwaitNotInAsyncMethodOrLambda");

		internal static string ERR_StatementLambdaInExpressionTree => GetResourceString("ERR_StatementLambdaInExpressionTree");

		internal static string WRN_UnobservedAwaitableExpression => GetResourceString("WRN_UnobservedAwaitableExpression");

		internal static string WRN_UnobservedAwaitableExpression_Title => GetResourceString("WRN_UnobservedAwaitableExpression_Title");

		internal static string ERR_LoopControlMustNotAwait => GetResourceString("ERR_LoopControlMustNotAwait");

		internal static string ERR_BadStaticInitializerInResumable => GetResourceString("ERR_BadStaticInitializerInResumable");

		internal static string ERR_RestrictedResumableType1 => GetResourceString("ERR_RestrictedResumableType1");

		internal static string ERR_ConstructorAsync => GetResourceString("ERR_ConstructorAsync");

		internal static string ERR_PartialMethodsMustNotBeAsync1 => GetResourceString("ERR_PartialMethodsMustNotBeAsync1");

		internal static string ERR_ResumablesCannotContainOnError => GetResourceString("ERR_ResumablesCannotContainOnError");

		internal static string ERR_ResumableLambdaInExpressionTree => GetResourceString("ERR_ResumableLambdaInExpressionTree");

		internal static string ERR_CannotLiftRestrictedTypeResumable1 => GetResourceString("ERR_CannotLiftRestrictedTypeResumable1");

		internal static string ERR_BadAwaitInTryHandler => GetResourceString("ERR_BadAwaitInTryHandler");

		internal static string WRN_AsyncLacksAwaits => GetResourceString("WRN_AsyncLacksAwaits");

		internal static string WRN_AsyncLacksAwaits_Title => GetResourceString("WRN_AsyncLacksAwaits_Title");

		internal static string WRN_UnobservedAwaitableDelegate => GetResourceString("WRN_UnobservedAwaitableDelegate");

		internal static string WRN_UnobservedAwaitableDelegate_Title => GetResourceString("WRN_UnobservedAwaitableDelegate_Title");

		internal static string ERR_SecurityCriticalAsyncInClassOrStruct => GetResourceString("ERR_SecurityCriticalAsyncInClassOrStruct");

		internal static string ERR_SecurityCriticalAsync => GetResourceString("ERR_SecurityCriticalAsync");

		internal static string ERR_DllImportOnResumableMethod => GetResourceString("ERR_DllImportOnResumableMethod");

		internal static string ERR_SynchronizedAsyncMethod => GetResourceString("ERR_SynchronizedAsyncMethod");

		internal static string ERR_AsyncSubMain => GetResourceString("ERR_AsyncSubMain");

		internal static string WRN_AsyncSubCouldBeFunction => GetResourceString("WRN_AsyncSubCouldBeFunction");

		internal static string WRN_AsyncSubCouldBeFunction_Title => GetResourceString("WRN_AsyncSubCouldBeFunction_Title");

		internal static string ERR_MyGroupCollectionAttributeCycle => GetResourceString("ERR_MyGroupCollectionAttributeCycle");

		internal static string ERR_LiteralExpected => GetResourceString("ERR_LiteralExpected");

		internal static string ERR_WinRTEventWithoutDelegate => GetResourceString("ERR_WinRTEventWithoutDelegate");

		internal static string ERR_MixingWinRTAndNETEvents => GetResourceString("ERR_MixingWinRTAndNETEvents");

		internal static string ERR_EventImplRemoveHandlerParamWrong => GetResourceString("ERR_EventImplRemoveHandlerParamWrong");

		internal static string ERR_AddParamWrongForWinRT => GetResourceString("ERR_AddParamWrongForWinRT");

		internal static string ERR_RemoveParamWrongForWinRT => GetResourceString("ERR_RemoveParamWrongForWinRT");

		internal static string ERR_ReImplementingWinRTInterface5 => GetResourceString("ERR_ReImplementingWinRTInterface5");

		internal static string ERR_ReImplementingWinRTInterface4 => GetResourceString("ERR_ReImplementingWinRTInterface4");

		internal static string ERR_BadIteratorByRefParam => GetResourceString("ERR_BadIteratorByRefParam");

		internal static string ERR_BadIteratorExpressionLambda => GetResourceString("ERR_BadIteratorExpressionLambda");

		internal static string ERR_BadIteratorReturn => GetResourceString("ERR_BadIteratorReturn");

		internal static string ERR_BadReturnValueInIterator => GetResourceString("ERR_BadReturnValueInIterator");

		internal static string ERR_BadYieldInNonIteratorMethod => GetResourceString("ERR_BadYieldInNonIteratorMethod");

		internal static string ERR_BadYieldInTryHandler => GetResourceString("ERR_BadYieldInTryHandler");

		internal static string WRN_DefAsgNoRetValWinRtEventVal1 => GetResourceString("WRN_DefAsgNoRetValWinRtEventVal1");

		internal static string WRN_DefAsgNoRetValWinRtEventVal1_Title => GetResourceString("WRN_DefAsgNoRetValWinRtEventVal1_Title");

		internal static string ERR_PartialMethodDefaultParameterValueMismatch2 => GetResourceString("ERR_PartialMethodDefaultParameterValueMismatch2");

		internal static string ERR_PartialMethodParamArrayMismatch2 => GetResourceString("ERR_PartialMethodParamArrayMismatch2");

		internal static string ERR_NetModuleNameMismatch => GetResourceString("ERR_NetModuleNameMismatch");

		internal static string ERR_BadModuleName => GetResourceString("ERR_BadModuleName");

		internal static string WRN_AssemblyAttributeFromModuleIsOverridden => GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden");

		internal static string WRN_AssemblyAttributeFromModuleIsOverridden_Title => GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden_Title");

		internal static string ERR_CmdOptionConflictsSource => GetResourceString("ERR_CmdOptionConflictsSource");

		internal static string WRN_ReferencedAssemblyDoesNotHaveStrongName => GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName");

		internal static string WRN_ReferencedAssemblyDoesNotHaveStrongName_Title => GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName_Title");

		internal static string ERR_InvalidSignaturePublicKey => GetResourceString("ERR_InvalidSignaturePublicKey");

		internal static string ERR_CollisionWithPublicTypeInModule => GetResourceString("ERR_CollisionWithPublicTypeInModule");

		internal static string ERR_ExportedTypeConflictsWithDeclaration => GetResourceString("ERR_ExportedTypeConflictsWithDeclaration");

		internal static string ERR_ExportedTypesConflict => GetResourceString("ERR_ExportedTypesConflict");

		internal static string WRN_RefCultureMismatch => GetResourceString("WRN_RefCultureMismatch");

		internal static string WRN_RefCultureMismatch_Title => GetResourceString("WRN_RefCultureMismatch_Title");

		internal static string ERR_AgnosticToMachineModule => GetResourceString("ERR_AgnosticToMachineModule");

		internal static string ERR_ConflictingMachineModule => GetResourceString("ERR_ConflictingMachineModule");

		internal static string WRN_ConflictingMachineAssembly => GetResourceString("WRN_ConflictingMachineAssembly");

		internal static string WRN_ConflictingMachineAssembly_Title => GetResourceString("WRN_ConflictingMachineAssembly_Title");

		internal static string ERR_CryptoHashFailed => GetResourceString("ERR_CryptoHashFailed");

		internal static string ERR_CantHaveWin32ResAndManifest => GetResourceString("ERR_CantHaveWin32ResAndManifest");

		internal static string ERR_ForwardedTypeConflictsWithDeclaration => GetResourceString("ERR_ForwardedTypeConflictsWithDeclaration");

		internal static string ERR_ForwardedTypesConflict => GetResourceString("ERR_ForwardedTypesConflict");

		internal static string ERR_TooLongMetadataName => GetResourceString("ERR_TooLongMetadataName");

		internal static string ERR_MissingNetModuleReference => GetResourceString("ERR_MissingNetModuleReference");

		internal static string ERR_NetModuleNameMustBeUnique => GetResourceString("ERR_NetModuleNameMustBeUnique");

		internal static string ERR_ForwardedTypeConflictsWithExportedType => GetResourceString("ERR_ForwardedTypeConflictsWithExportedType");

		internal static string IDS_MSG_ADDREFERENCE => GetResourceString("IDS_MSG_ADDREFERENCE");

		internal static string IDS_MSG_ADDLINKREFERENCE => GetResourceString("IDS_MSG_ADDLINKREFERENCE");

		internal static string IDS_MSG_ADDMODULE => GetResourceString("IDS_MSG_ADDMODULE");

		internal static string ERR_NestingViolatesCLS1 => GetResourceString("ERR_NestingViolatesCLS1");

		internal static string ERR_PDBWritingFailed => GetResourceString("ERR_PDBWritingFailed");

		internal static string ERR_ParamDefaultValueDiffersFromAttribute => GetResourceString("ERR_ParamDefaultValueDiffersFromAttribute");

		internal static string ERR_FieldHasMultipleDistinctConstantValues => GetResourceString("ERR_FieldHasMultipleDistinctConstantValues");

		internal static string ERR_EncNoPIAReference => GetResourceString("ERR_EncNoPIAReference");

		internal static string ERR_EncReferenceToAddedMember => GetResourceString("ERR_EncReferenceToAddedMember");

		internal static string ERR_UnsupportedModule1 => GetResourceString("ERR_UnsupportedModule1");

		internal static string ERR_UnsupportedEvent1 => GetResourceString("ERR_UnsupportedEvent1");

		internal static string PropertiesCanNotHaveTypeArguments => GetResourceString("PropertiesCanNotHaveTypeArguments");

		internal static string IdentifierSyntaxNotWithinSyntaxTree => GetResourceString("IdentifierSyntaxNotWithinSyntaxTree");

		internal static string AnonymousObjectCreationExpressionSyntaxNotWithinTree => GetResourceString("AnonymousObjectCreationExpressionSyntaxNotWithinTree");

		internal static string FieldInitializerSyntaxNotWithinSyntaxTree => GetResourceString("FieldInitializerSyntaxNotWithinSyntaxTree");

		internal static string IDS_TheSystemCannotFindThePathSpecified => GetResourceString("IDS_TheSystemCannotFindThePathSpecified");

		internal static string ThereAreNoPointerTypesInVB => GetResourceString("ThereAreNoPointerTypesInVB");

		internal static string ThereAreNoFunctionPointerTypesInVB => GetResourceString("ThereAreNoFunctionPointerTypesInVB");

		internal static string ThereIsNoDynamicTypeInVB => GetResourceString("ThereIsNoDynamicTypeInVB");

		internal static string ThereAreNoNativeIntegerTypesInVB => GetResourceString("ThereAreNoNativeIntegerTypesInVB");

		internal static string VariableSyntaxNotWithinSyntaxTree => GetResourceString("VariableSyntaxNotWithinSyntaxTree");

		internal static string AggregateSyntaxNotWithinSyntaxTree => GetResourceString("AggregateSyntaxNotWithinSyntaxTree");

		internal static string FunctionSyntaxNotWithinSyntaxTree => GetResourceString("FunctionSyntaxNotWithinSyntaxTree");

		internal static string PositionIsNotWithinSyntax => GetResourceString("PositionIsNotWithinSyntax");

		internal static string RangeVariableSyntaxNotWithinSyntaxTree => GetResourceString("RangeVariableSyntaxNotWithinSyntaxTree");

		internal static string DeclarationSyntaxNotWithinSyntaxTree => GetResourceString("DeclarationSyntaxNotWithinSyntaxTree");

		internal static string StatementOrExpressionIsNotAValidType => GetResourceString("StatementOrExpressionIsNotAValidType");

		internal static string DeclarationSyntaxNotWithinTree => GetResourceString("DeclarationSyntaxNotWithinTree");

		internal static string TypeParameterNotWithinTree => GetResourceString("TypeParameterNotWithinTree");

		internal static string NotWithinTree => GetResourceString("NotWithinTree");

		internal static string LocationMustBeProvided => GetResourceString("LocationMustBeProvided");

		internal static string SemanticModelMustBeProvided => GetResourceString("SemanticModelMustBeProvided");

		internal static string NumberOfTypeParametersAndArgumentsMustMatch => GetResourceString("NumberOfTypeParametersAndArgumentsMustMatch");

		internal static string ERR_ResourceInModule => GetResourceString("ERR_ResourceInModule");

		internal static string NotAVbSymbol => GetResourceString("NotAVbSymbol");

		internal static string ElementsCannotBeNull => GetResourceString("ElementsCannotBeNull");

		internal static string HDN_UnusedImportClause => GetResourceString("HDN_UnusedImportClause");

		internal static string HDN_UnusedImportStatement => GetResourceString("HDN_UnusedImportStatement");

		internal static string WrongSemanticModelType => GetResourceString("WrongSemanticModelType");

		internal static string PositionNotWithinTree => GetResourceString("PositionNotWithinTree");

		internal static string SpeculatedSyntaxNodeCannotBelongToCurrentCompilation => GetResourceString("SpeculatedSyntaxNodeCannotBelongToCurrentCompilation");

		internal static string ChainingSpeculativeModelIsNotSupported => GetResourceString("ChainingSpeculativeModelIsNotSupported");

		internal static string IDS_ToolName => GetResourceString("IDS_ToolName");

		internal static string IDS_LogoLine1 => GetResourceString("IDS_LogoLine1");

		internal static string IDS_LogoLine2 => GetResourceString("IDS_LogoLine2");

		internal static string IDS_LangVersions => GetResourceString("IDS_LangVersions");

		internal static string IDS_VBCHelp => GetResourceString("IDS_VBCHelp");

		internal static string WRN_PdbLocalNameTooLong => GetResourceString("WRN_PdbLocalNameTooLong");

		internal static string WRN_PdbLocalNameTooLong_Title => GetResourceString("WRN_PdbLocalNameTooLong_Title");

		internal static string WRN_PdbUsingNameTooLong => GetResourceString("WRN_PdbUsingNameTooLong");

		internal static string WRN_PdbUsingNameTooLong_Title => GetResourceString("WRN_PdbUsingNameTooLong_Title");

		internal static string WRN_XMLDocCrefToTypeParameter => GetResourceString("WRN_XMLDocCrefToTypeParameter");

		internal static string WRN_XMLDocCrefToTypeParameter_Title => GetResourceString("WRN_XMLDocCrefToTypeParameter_Title");

		internal static string ERR_LinkedNetmoduleMetadataMustProvideFullPEImage => GetResourceString("ERR_LinkedNetmoduleMetadataMustProvideFullPEImage");

		internal static string WRN_AnalyzerCannotBeCreated => GetResourceString("WRN_AnalyzerCannotBeCreated");

		internal static string WRN_AnalyzerCannotBeCreated_Title => GetResourceString("WRN_AnalyzerCannotBeCreated_Title");

		internal static string WRN_NoAnalyzerInAssembly => GetResourceString("WRN_NoAnalyzerInAssembly");

		internal static string WRN_NoAnalyzerInAssembly_Title => GetResourceString("WRN_NoAnalyzerInAssembly_Title");

		internal static string WRN_UnableToLoadAnalyzer => GetResourceString("WRN_UnableToLoadAnalyzer");

		internal static string WRN_UnableToLoadAnalyzer_Title => GetResourceString("WRN_UnableToLoadAnalyzer_Title");

		internal static string INF_UnableToLoadSomeTypesInAnalyzer => GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer");

		internal static string INF_UnableToLoadSomeTypesInAnalyzer_Title => GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer_Title");

		internal static string ERR_CantReadRulesetFile => GetResourceString("ERR_CantReadRulesetFile");

		internal static string ERR_PlatformDoesntSupport => GetResourceString("ERR_PlatformDoesntSupport");

		internal static string ERR_CantUseRequiredAttribute => GetResourceString("ERR_CantUseRequiredAttribute");

		internal static string ERR_EncodinglessSyntaxTree => GetResourceString("ERR_EncodinglessSyntaxTree");

		internal static string ERR_InvalidFormatSpecifier => GetResourceString("ERR_InvalidFormatSpecifier");

		internal static string ERR_InvalidPreprocessorConstantType => GetResourceString("ERR_InvalidPreprocessorConstantType");

		internal static string ERR_ExpectedWarningKeyword => GetResourceString("ERR_ExpectedWarningKeyword");

		internal static string ERR_CannotBeMadeNullable1 => GetResourceString("ERR_CannotBeMadeNullable1");

		internal static string ERR_BadConditionalWithRef => GetResourceString("ERR_BadConditionalWithRef");

		internal static string ERR_NullPropagatingOpInExpressionTree => GetResourceString("ERR_NullPropagatingOpInExpressionTree");

		internal static string ERR_TooLongOrComplexExpression => GetResourceString("ERR_TooLongOrComplexExpression");

		internal static string ERR_ExpressionDoesntHaveName => GetResourceString("ERR_ExpressionDoesntHaveName");

		internal static string ERR_InvalidNameOfSubExpression => GetResourceString("ERR_InvalidNameOfSubExpression");

		internal static string ERR_MethodTypeArgsUnexpected => GetResourceString("ERR_MethodTypeArgsUnexpected");

		internal static string NoNoneSearchCriteria => GetResourceString("NoNoneSearchCriteria");

		internal static string ERR_InvalidAssemblyCulture => GetResourceString("ERR_InvalidAssemblyCulture");

		internal static string ERR_InReferencedAssembly => GetResourceString("ERR_InReferencedAssembly");

		internal static string ERR_InterpolationFormatWhitespace => GetResourceString("ERR_InterpolationFormatWhitespace");

		internal static string ERR_InterpolationAlignmentOutOfRange => GetResourceString("ERR_InterpolationAlignmentOutOfRange");

		internal static string ERR_InterpolatedStringFactoryError => GetResourceString("ERR_InterpolatedStringFactoryError");

		internal static string HDN_UnusedImportClause_Title => GetResourceString("HDN_UnusedImportClause_Title");

		internal static string HDN_UnusedImportStatement_Title => GetResourceString("HDN_UnusedImportStatement_Title");

		internal static string ERR_ConstantStringTooLong => GetResourceString("ERR_ConstantStringTooLong");

		internal static string ERR_LanguageVersion => GetResourceString("ERR_LanguageVersion");

		internal static string ERR_BadPdbData => GetResourceString("ERR_BadPdbData");

		internal static string FEATURE_ArrayLiterals => GetResourceString("FEATURE_ArrayLiterals");

		internal static string FEATURE_AsyncExpressions => GetResourceString("FEATURE_AsyncExpressions");

		internal static string FEATURE_AutoProperties => GetResourceString("FEATURE_AutoProperties");

		internal static string FEATURE_ReadonlyAutoProperties => GetResourceString("FEATURE_ReadonlyAutoProperties");

		internal static string FEATURE_CoContraVariance => GetResourceString("FEATURE_CoContraVariance");

		internal static string FEATURE_CollectionInitializers => GetResourceString("FEATURE_CollectionInitializers");

		internal static string FEATURE_CommentsAfterLineContinuation => GetResourceString("FEATURE_CommentsAfterLineContinuation");

		internal static string FEATURE_GlobalNamespace => GetResourceString("FEATURE_GlobalNamespace");

		internal static string FEATURE_Iterators => GetResourceString("FEATURE_Iterators");

		internal static string FEATURE_LineContinuation => GetResourceString("FEATURE_LineContinuation");

		internal static string FEATURE_StatementLambdas => GetResourceString("FEATURE_StatementLambdas");

		internal static string FEATURE_SubLambdas => GetResourceString("FEATURE_SubLambdas");

		internal static string FEATURE_NullPropagatingOperator => GetResourceString("FEATURE_NullPropagatingOperator");

		internal static string FEATURE_NameOfExpressions => GetResourceString("FEATURE_NameOfExpressions");

		internal static string FEATURE_RegionsEverywhere => GetResourceString("FEATURE_RegionsEverywhere");

		internal static string FEATURE_MultilineStringLiterals => GetResourceString("FEATURE_MultilineStringLiterals");

		internal static string FEATURE_CObjInAttributeArguments => GetResourceString("FEATURE_CObjInAttributeArguments");

		internal static string FEATURE_LineContinuationComments => GetResourceString("FEATURE_LineContinuationComments");

		internal static string FEATURE_TypeOfIsNot => GetResourceString("FEATURE_TypeOfIsNot");

		internal static string FEATURE_YearFirstDateLiterals => GetResourceString("FEATURE_YearFirstDateLiterals");

		internal static string FEATURE_WarningDirectives => GetResourceString("FEATURE_WarningDirectives");

		internal static string FEATURE_PartialModules => GetResourceString("FEATURE_PartialModules");

		internal static string FEATURE_PartialInterfaces => GetResourceString("FEATURE_PartialInterfaces");

		internal static string FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite => GetResourceString("FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite");

		internal static string FEATURE_DigitSeparators => GetResourceString("FEATURE_DigitSeparators");

		internal static string FEATURE_BinaryLiterals => GetResourceString("FEATURE_BinaryLiterals");

		internal static string FEATURE_Tuples => GetResourceString("FEATURE_Tuples");

		internal static string FEATURE_PrivateProtected => GetResourceString("FEATURE_PrivateProtected");

		internal static string ERR_DebugEntryPointNotSourceMethodDefinition => GetResourceString("ERR_DebugEntryPointNotSourceMethodDefinition");

		internal static string ERR_InvalidPathMap => GetResourceString("ERR_InvalidPathMap");

		internal static string SyntaxTreeIsNotASubmission => GetResourceString("SyntaxTreeIsNotASubmission");

		internal static string ERR_TooManyUserStrings => GetResourceString("ERR_TooManyUserStrings");

		internal static string ERR_PeWritingFailure => GetResourceString("ERR_PeWritingFailure");

		internal static string ERR_OptionMustBeAbsolutePath => GetResourceString("ERR_OptionMustBeAbsolutePath");

		internal static string ERR_SourceLinkRequiresPdb => GetResourceString("ERR_SourceLinkRequiresPdb");

		internal static string ERR_TupleDuplicateElementName => GetResourceString("ERR_TupleDuplicateElementName");

		internal static string WRN_TupleLiteralNameMismatch => GetResourceString("WRN_TupleLiteralNameMismatch");

		internal static string WRN_TupleLiteralNameMismatch_Title => GetResourceString("WRN_TupleLiteralNameMismatch_Title");

		internal static string ERR_TupleReservedElementName => GetResourceString("ERR_TupleReservedElementName");

		internal static string ERR_TupleReservedElementNameAnyPosition => GetResourceString("ERR_TupleReservedElementNameAnyPosition");

		internal static string ERR_TupleTooFewElements => GetResourceString("ERR_TupleTooFewElements");

		internal static string ERR_TupleElementNamesAttributeMissing => GetResourceString("ERR_TupleElementNamesAttributeMissing");

		internal static string ERR_ExplicitTupleElementNamesAttribute => GetResourceString("ERR_ExplicitTupleElementNamesAttribute");

		internal static string ERR_RefReturningCallInExpressionTree => GetResourceString("ERR_RefReturningCallInExpressionTree");

		internal static string ERR_CannotEmbedWithoutPdb => GetResourceString("ERR_CannotEmbedWithoutPdb");

		internal static string ERR_InvalidInstrumentationKind => GetResourceString("ERR_InvalidInstrumentationKind");

		internal static string ERR_InvalidHashAlgorithmName => GetResourceString("ERR_InvalidHashAlgorithmName");

		internal static string ERR_DocFileGen => GetResourceString("ERR_DocFileGen");

		internal static string ERR_BadAssemblyName => GetResourceString("ERR_BadAssemblyName");

		internal static string ERR_TypeForwardedToMultipleAssemblies => GetResourceString("ERR_TypeForwardedToMultipleAssemblies");

		internal static string ERR_Merge_conflict_marker_encountered => GetResourceString("ERR_Merge_conflict_marker_encountered");

		internal static string ERR_NoRefOutWhenRefOnly => GetResourceString("ERR_NoRefOutWhenRefOnly");

		internal static string ERR_NoNetModuleOutputWhenRefOutOrRefOnly => GetResourceString("ERR_NoNetModuleOutputWhenRefOutOrRefOnly");

		internal static string ERR_BadNonTrailingNamedArgument => GetResourceString("ERR_BadNonTrailingNamedArgument");

		internal static string ERR_BadDocumentationMode => GetResourceString("ERR_BadDocumentationMode");

		internal static string ERR_BadLanguageVersion => GetResourceString("ERR_BadLanguageVersion");

		internal static string ERR_BadSourceCodeKind => GetResourceString("ERR_BadSourceCodeKind");

		internal static string ERR_TupleInferredNamesNotAvailable => GetResourceString("ERR_TupleInferredNamesNotAvailable");

		internal static string WRN_Experimental => GetResourceString("WRN_Experimental");

		internal static string WRN_Experimental_Title => GetResourceString("WRN_Experimental_Title");

		internal static string ERR_InvalidDebugInfo => GetResourceString("ERR_InvalidDebugInfo");

		internal static string IConversionExpressionIsNotVisualBasicConversion => GetResourceString("IConversionExpressionIsNotVisualBasicConversion");

		internal static string IArgumentIsNotVisualBasicArgument => GetResourceString("IArgumentIsNotVisualBasicArgument");

		internal static string FEATURE_LeadingDigitSeparator => GetResourceString("FEATURE_LeadingDigitSeparator");

		internal static string ERR_ValueTupleResolutionAmbiguous3 => GetResourceString("ERR_ValueTupleResolutionAmbiguous3");

		internal static string ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment => GetResourceString("ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment");

		internal static string FEATURE_InterpolatedStrings => GetResourceString("FEATURE_InterpolatedStrings");

		internal static string FEATURE_UnconstrainedTypeParameterInConditional => GetResourceString("FEATURE_UnconstrainedTypeParameterInConditional");

		internal static string ERR_MultipleAnalyzerConfigsInSameDir => GetResourceString("ERR_MultipleAnalyzerConfigsInSameDir");

		internal static string ERR_DefaultInterfaceImplementationInNoPIAType => GetResourceString("ERR_DefaultInterfaceImplementationInNoPIAType");

		internal static string ERR_ReAbstractionInNoPIAType => GetResourceString("ERR_ReAbstractionInNoPIAType");

		internal static string ERR_BadSwitchValue => GetResourceString("ERR_BadSwitchValue");

		internal static string ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation => GetResourceString("ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation");

		internal static string ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember => GetResourceString("ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember");

		internal static string WRN_AttributeNotSupportedInVB => GetResourceString("WRN_AttributeNotSupportedInVB");

		internal static string WRN_AttributeNotSupportedInVB_Title => GetResourceString("WRN_AttributeNotSupportedInVB_Title");

		internal static string WRN_GeneratorFailedDuringGeneration => GetResourceString("WRN_GeneratorFailedDuringGeneration");

		internal static string WRN_GeneratorFailedDuringGeneration_Description => GetResourceString("WRN_GeneratorFailedDuringGeneration_Description");

		internal static string WRN_GeneratorFailedDuringGeneration_Title => GetResourceString("WRN_GeneratorFailedDuringGeneration_Title");

		internal static string WRN_GeneratorFailedDuringInitialization => GetResourceString("WRN_GeneratorFailedDuringInitialization");

		internal static string WRN_GeneratorFailedDuringInitialization_Description => GetResourceString("WRN_GeneratorFailedDuringInitialization_Description");

		internal static string WRN_GeneratorFailedDuringInitialization_Title => GetResourceString("WRN_GeneratorFailedDuringInitialization_Title");

		internal static string WRN_AnalyzerReferencesFramework => GetResourceString("WRN_AnalyzerReferencesFramework");

		internal static string WRN_AnalyzerReferencesFramework_Title => GetResourceString("WRN_AnalyzerReferencesFramework_Title");

		internal static string FEATURE_InitOnlySettersUsage => GetResourceString("FEATURE_InitOnlySettersUsage");

		internal static string ERR_AssignmentInitOnly => GetResourceString("ERR_AssignmentInitOnly");

		internal static string ERR_OverridingInitOnlyProperty => GetResourceString("ERR_OverridingInitOnlyProperty");

		internal static string ERR_PropertyDoesntImplementInitOnly => GetResourceString("ERR_PropertyDoesntImplementInitOnly");

		internal static string ERR_UnmanagedCallersOnlyNotSupported => GetResourceString("ERR_UnmanagedCallersOnlyNotSupported");

		private VBResources()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetResourceString(string resourceKey, string defaultValue = null)
		{
			return ResourceManager.GetString(resourceKey, Culture);
		}
	}
}
