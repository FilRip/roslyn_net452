Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module ErrorFacts
		Public Function IsFatal(ByVal code As ERRID) As Boolean
			Return If(code <> ERRID.FTL_InvalidInputFileName, False, True)
		End Function

		Public Function IsHidden(ByVal code As ERRID) As Boolean
			Return If(CInt(code) - CInt(ERRID.HDN_UnusedImportClause) > 1, False, True)
		End Function

		Public Function IsInfo(ByVal code As ERRID) As Boolean
			Return If(code <> ERRID.INF_UnableToLoadSomeTypesInAnalyzer, False, True)
		End Function

		Public Function IsWarning(ByVal code As Microsoft.CodeAnalysis.VisualBasic.ERRID) As Boolean
			Dim flag As Boolean
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = code
			If (eRRID <= Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnimplementedCommandLineSwitch) Then
				If (eRRID <= Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_IgnoreModuleManifest) Then
					If (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadSwitch OrElse eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoConfigInResponseFile OrElse eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_IgnoreModuleManifest) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				ElseIf (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadUILang) Then
					Select Case eRRID
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverloadBase4
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefaultnessShadowed4
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration0
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassNoMembers1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MemberShadowsSynthMember6
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsSynthMember7
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor3
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_FieldNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BaseClassNotCLSCompliant2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ProcTypeNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ParamNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InheritedInterfaceNotCLSCompliant2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_CLSMemberInNonCLSType3
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NameNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMemberInCLSInterface1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMustOverrideInCLSType1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ArrayOverloadsNonCLS2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RootNamespaceNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RootNamespaceNotCLSCompliant2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GenericConstraintNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OptionalValueNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_CLSAttrInvalidOnGetSet
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeConflictButMerged6
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ShadowingGenericParamWithParam1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_CannotFindStandardLibrary1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EventDelegateTypeNotCLSCompliant2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DebuggerHiddenIgnoredOnProperties
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SelectCaseInvalidRange
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_CLSEventMethodInNonCLSType3
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ExpectedInitComponentCall2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NamespaceCaseMismatch3
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UndefinedOrEmptyNamespaceOrClass1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UndefinedOrEmptyProjectNamespaceOrClass1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_IndirectRefToLinkedAssembly2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DelaySignButNoKey
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AsyncSubMain Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNotInAsyncMethodOrLambda Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInNonIteratorMethod Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration0 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AsyncSubMain Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_PartialMethodsMustNotBeAsync1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNotInAsyncMethodOrLambda Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadIteratorReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInTryHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInNonIteratorMethod Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadReturnValueInIterator Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitInTryHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverloadBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefaultnessShadowed4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration0 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassNoMembers1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MemberShadowsSynthMember6
						Case 40016
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AsyncSubMain Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadResumableAccessReturnVariable Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConstructorAsync Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsSynthMember7 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AsyncSubMain Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_PartialMethodsMustNotBeAsync1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadResumableAccessReturnVariable Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadIteratorExpressionLambda Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConstructorAsync Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidLambdaModifier Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverloadBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefaultnessShadowed4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsSynthMember7 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoletePropertyAccessor2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReturnFromNonGenericTaskAsync Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_SignButNoPrivateKey Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NamespaceNotAllowedInScript Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMemberInCLSInterface1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExtensionMethodCannotBeLateBound Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInNonIteratorMethod Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReservedAssemblyName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MarshalUnmanagedTypeNotValidForFields Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GenericConstraintNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExtensionMethodCannotBeLateBound Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_TypeInferenceArrayRankMismatch1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNotInAsyncMethodOrLambda Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInNonIteratorMethod Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_SignButNoPrivateKey Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NamespaceNotAllowedInScript Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReservedAssemblyName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConstructorCannotBeDeclaredPartial Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MarshalUnmanagedTypeNotValidForFields Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MarshalUnmanagedTypeOnlyValidForFields Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration0 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMemberInCLSInterface1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GenericConstraintNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeNotCLSCompliant1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_TypeOrMemberNotGeneric2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExtensionMethodCannotBeLateBound Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_TypeInferenceArrayRankMismatch1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_QueryStrictDisallowImplicitObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IfNoType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAsyncInQuery Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_RestrictedResumableType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AsyncSubMain Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_PartialMethodsMustNotBeAsync1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitNotInAsyncMethodOrLambda Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadIteratorReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInTryHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInNonIteratorMethod Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadReturnValueInIterator Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadAwaitInTryHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_SignButNoPrivateKey Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidVersionFormat Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSingleScript Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NamespaceNotAllowedInScript Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_KeywordNotAllowedInScript Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReservedAssemblyName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConstructorCannotBeDeclaredPartial Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ModuleEmitFailure Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ParameterNotValidForType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MarshalUnmanagedTypeNotValidForFields Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MarshalUnmanagedTypeOnlyValidForFields Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AttributeParameterRequired1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_AttributeParameterRequired2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidOverrideDueToTupleNames2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverloadBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverrideType5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustOverride2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefaultnessShadowed4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration0 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassNoMembers1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsMember5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MemberShadowsSynthMember6 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMemberInCLSInterface1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMustOverrideInCLSType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ArrayOverloadsNonCLS2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RootNamespaceNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RootNamespaceNotCLSCompliant2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GenericConstraintNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OptionalValueNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_CLSAttrInvalidOnGetSet Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeConflictButMerged6
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IfNoTypeObjectDisallowed Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ArrayInitNoTypeObjectDisallowed Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_SubRequiresParenthesesLParen Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_SubRequiresParenthesesBang Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadGetAwaiterMethod1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidAsyncIteratorModifiers Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadIteratorReturn Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadResumableAccessReturnVariable Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReturnFromNonGenericTaskAsync Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadOverloadCandidates2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_FailureSigningAssembly Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidVersionFormat Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReservedAssemblyName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ModuleEmitFailure Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidVersionFormat2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidMultipleAttributeUsageInNetModule2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbol2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseOfObsoleteSymbolNoMessage1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyGeneration1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SynthMemberShadowsSynthMember7 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BaseClassNotCLSCompliant2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EnumUnderlyingTypeNotCLS1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NonCLSMustOverrideInCLSType1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GenericConstraintNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OptionalValueNotCLSCompliant1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ShadowingGenericParamWithParam1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EventDelegateTypeNotCLSCompliant2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UndefinedOrEmptyNamespaceOrClass1
							flag = False
							Return flag
						Case Else
							If (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnimplementedCommandLineSwitch) Then
								Exit Select
							End If
							flag = False
							Return flag
					End Select
				End If
			ElseIf (eRRID <= Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_StaticLocalNoInference) Then
				If (CInt(eRRID) - CInt(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase3) > 8) Then
					Select Case eRRID
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveAddHandlerCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionCopyBack
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LateBindingResolution
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath2
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumed1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedProperty1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SharedMemberThroughInstance
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursivePropertyCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_OverlappingCatch
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRef
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DuplicateCatch
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadChecksumValExtChecksum
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MultipleDeclFileExtChecksum
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadGUIDFormatExtChecksum
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMathSelectCase
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EqualToLiteralNothing
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NotEqualToLiteralNothing
							Exit Select
						Case 42001
						Case 42002
						Case 42003
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case 42008
						Case 42009
						Case 42010
						Case 42011
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveAddHandlerCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveAddHandlerCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionCopyBack Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnimplementedCommandLineSwitch Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LateBindingResolution Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumed1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedProperty1
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RequiredNonObsoleteNewCall3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LateBindingResolution Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SharedMemberThroughInstance Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursivePropertyCall
						Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RequiredNonObsoleteNewCall4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal
							flag = False
							Return flag
						Case Else
							Select Case eRRID
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocalConst
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassInterfaceShadows5
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassPropertySetObject1
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRef
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncRef1
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpRef1
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropRef1
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRefStr
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefStr
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_StaticLocalNoInference
									Exit Select
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseSwitchInsteadOfAttribute Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMathSelectCase
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnimplementedCommandLineSwitch Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseSwitchInsteadOfAttribute Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TupleLiteralNameMismatch Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LateBindingResolution Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumed1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedProperty1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadChecksumValExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MultipleDeclFileExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadGUIDFormatExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMathSelectCase Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EqualToLiteralNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NotEqualToLiteralNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocalConst Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassInterfaceShadows5 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassPropertySetObject1
								Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnimplementedCommandLineSwitch Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RequiredNonObsoleteNewCall4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ConstraintsFailedForInferredArgs2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseSwitchInsteadOfAttribute Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveAddHandlerCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedVar1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectAssumedProperty1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursivePropertyCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRef Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MultipleDeclFileExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMathSelectCase Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NotEqualToLiteralNothing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ComClassPropertySetObject1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRef Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpRef1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRefStr
									flag = False
									Return flag
								Case Else
									flag = False
									Return flag
							End Select

					End Select
				End If
			ElseIf (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidAssemblyName) Then
				Select Case eRRID
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadXMLLine
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocMoreThanOneCommentBlock
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocNotFirstOnLine
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInsideMethod
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadParamTag2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParamTagWithoutName
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLMissingFileOrPathAttribute1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLCannotWriteToXMLDocFile2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocOnAPartialType
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnADeclareSub
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocStartTagWithNoEndTag
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadGenericParamTag2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocGenericParamTagWithoutName
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocExceptionTagWithoutCRef
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LiftControlVariableLambda
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaPassedToRemoveHandler
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LiftControlVariableQuery
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AmbiguousCastConversion2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceDeclarationAmbiguous3
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ArrayInitNoTypeObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TypeInferenceAssumed3
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedOut6
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedIn6
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceIEnumerableSuggestion3
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedTryOut4
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedTryIn4
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_IfNoTypeObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_IfTooManyTypesObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ArrayInitTooManyTypesObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaNoTypeObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaTooManyTypesObjectAssumed
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinVarDecl
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinFunction
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinProperty
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObsoleteIdentityDirectCastForValueType
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversion2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MutableStructureInUsing
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MutableGenericStructureInUsing
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncVal1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpVal1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropVal1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AsyncLacksAwaits
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AsyncSubCouldBeFunction
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnobservedAwaitableExpression
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnobservedAwaitableDelegate
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_PrefixAndXmlnsLocalName
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseValueForXmlExpression3
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ReturnTypeAttributeOnWriteOnlyProperty
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InvalidVersionFormat
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MainIgnored
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_EmptyPrefixAndXmlnsLocalName
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValWinRtEventVal1
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AssemblyAttributeFromModuleIsOverridden
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RefCultureMismatch
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ConflictingMachineAssembly
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_PdbLocalNameTooLong
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_PdbUsingNameTooLong
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefToTypeParameter
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AnalyzerCannotBeCreated
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoAnalyzerInAssembly
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnableToLoadAnalyzer
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AttributeIgnoredWhenPublicSigning
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_Experimental
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_AttributeNotSupportedInVB
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToDerivedType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFConversion Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalCondTypeInIIF Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadParamTag2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CantCallIIF Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CantSpecifyAsNewAndNullable Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursiveOperatorCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParamTagWithoutName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocCrefAttributeNotFound1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LiftControlVariableLambda
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MatchingOperatorExpected2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NullableImplicit Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableLogicalOperator3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFConversion Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NullableImplicit Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocOnAPartialType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToDerivedType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MatchingOperatorExpected2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableLogicalOperator3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConditionOperatorRequired3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFConversion Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalCondTypeInIIF Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NullableImplicit Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadParamTag2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocOnAPartialType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnADeclareSub Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableLogicalOperator3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFConversion Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NullableImplicit Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseSwitchInsteadOfAttribute Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursivePropertyCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MultipleDeclFileExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRef Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpRef1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocOnAPartialType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedOut6 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceIEnumerableSuggestion3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaNoTypeObjectAssumed Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinVarDecl Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MutableGenericStructureInUsing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpVal1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_PrefixAndXmlnsLocalName
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToSameType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToInterfaceType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToBaseType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionToDerivedType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConversionFromObject Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MatchingOperatorExpected2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableLogicalOperator3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ConditionOperatorRequired3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnacceptableForLoopRelOperator2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorRequiresIntegerParameter1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFCount Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalOperandInIIFConversion Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalCondTypeInIIF Or Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NullableImplicit Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_NoNonObsoleteConstructorOnBase4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RequiredNonObsoleteNewCall3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseSwitchInsteadOfAttribute Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_TupleLiteralNameMismatch Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MustShadowOnMultipleInheritance2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ImplicitConversionSubst1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LateBindingResolution Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocal Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_SharedMemberThroughInstance Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RecursivePropertyCall Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_ObjectMath1Not Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadChecksumValExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MultipleDeclFileExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_BadGUIDFormatExtChecksum Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UnusedLocalConst Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRef Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncRef1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpRef1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropRef1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocParseError1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocDuplicateXMLNode1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocIllegalTagOnElement2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadParamTag2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocWithoutLanguageElement Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocOnAPartialType Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocReturnsOnADeclareSub Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocInvalidXMLFragment Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_XMLDocBadFormedXML Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_InterfaceConversion2 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_RelDelegatePassedToRemoveHandler Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedOut6 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedIn6 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceIEnumerableSuggestion3 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_VarianceConversionFailedTryOut4 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaNoTypeObjectAssumed Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_LambdaTooManyTypesObjectAssumed Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinVarDecl Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MissingAsClauseinFunction Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_MutableGenericStructureInUsing Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncVal1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpVal1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropVal1 Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_PrefixAndXmlnsLocalName Or Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_UseValueForXmlExpression3
					Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnmanagedCallersOnlyNotSupported
						flag = False
						Return flag
					Case Else
						If (CInt(eRRID) - CInt(Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_GeneratorFailedDuringInitialization) > 2) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			End If
			flag = True
			Return flag
		End Function
	End Module
End Namespace